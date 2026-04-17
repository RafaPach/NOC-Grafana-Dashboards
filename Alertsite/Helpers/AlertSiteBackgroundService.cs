using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NOCAPI.Modules.Users.DTOs;
using NOCAPI.Modules.Users.Prometheus;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using static NOCAPI.Modules.Alertsite.Prometheus.ModuleRegistry;

namespace NOCAPI.Modules.Users.Helpers
{
    public class AlertsiteMetricsBackgroundService : BackgroundService
    {
        private readonly AlertsiteHelper _alertsiteHelper;
        private readonly TokenService _tokenService;
        private readonly ILogger<AlertsiteMetricsBackgroundService> _logger;
        private readonly PrometheusMetrics _prometheusGauges;

        public static string CachedMetrics = "# No Prometheus data yet";

        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(1);

        private readonly Dictionary<string, ThresholdState> _alertsiteErrorState = new();

        private class ThresholdState
        {
            public bool IsHigh;
            public DateTime HighStart;
        }

        private static int ExtractStatusCode(PrometheusMetric metric)
        {
            if (metric.Value == 0)
                return 0; // ok

            if (metric.InfoMsg?.Contains("500") == true) return 500;
            if (metric.InfoMsg?.Contains("404") == true) return 404;
            if (metric.InfoMsg?.Contains("403") == true) return 403;

            return 1; 
        }

        private static string Categorise(PrometheusMetric metric)
        {
            if (metric.Value == 0)
                return "OK";

            if (metric.InfoMsg == null)
                return "unknown";

            if (metric.InfoMsg.Contains("500"))
                return "http_5xx";

            if (metric.InfoMsg.Contains("404"))
                return "http_404";
            if (metric.InfoMsg.Contains("403"))

                return "http_403";

            if (metric.InfoMsg.Contains("timeout", StringComparison.OrdinalIgnoreCase))
                return "timeout";

            return "other";
        }

        public AlertsiteMetricsBackgroundService(
            AlertsiteHelper alertsiteHelper,
            TokenService tokenService,
            ILogger<AlertsiteMetricsBackgroundService> logger, PrometheusMetrics prometheuGauges)
        {
            _alertsiteHelper = alertsiteHelper;
            _tokenService = tokenService;
            _logger = logger;
            _prometheusGauges = prometheuGauges;
        }

        public static bool TryParseAlertSiteTimestamp(string? dt, out long unix)
        {
            unix = 0;

            if (string.IsNullOrWhiteSpace(dt))
                return false;

            if (!DateTime.TryParseExact(
                    dt,
                    "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var parsedUtc))
            {
                return false;
            }

            unix = new DateTimeOffset(parsedUtc).ToUnixTimeSeconds();
            return true;
        }



        private double UpdateThresholdTimer(
           Dictionary<string, ThresholdState> states,
           string key,
           double value,
           double threshold)
        {
            if (!states.TryGetValue(key, out var state))
            {
                state = new ThresholdState();
                states[key] = state;
            }

            if (value >= threshold)
            {
                if (!state.IsHigh)
                {
                    state.IsHigh = true;
                    state.HighStart = DateTime.UtcNow;
                }

                return Math.Floor((DateTime.UtcNow - state.HighStart).TotalMinutes);
            }
            else
            {
                state.IsHigh = false;
                state.HighStart = DateTime.UtcNow;
                return 0;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting Alertsite metrics background service...");

            //_prometheusGauges._appStatusCodeGauge.Unpublish();
            //_prometheusGauges._appHealthGauge.Unpublish();
            //_prometheusGauges._appLastStatusTsGauge.Unpublish();
            //_prometheusGauges._appResponseSecondsGauge.Unpublish();
            //_prometheusGauges._appErrorCategoryGauge.Unpublish();
            _prometheusGauges._appErrorCategoryGauge.Unpublish();

            _prometheusGauges._alertsite_heartbeat
            .WithLabels(Environment.MachineName)
            .Inc();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var token = await _tokenService.GetAccessTokenAsync(stoppingToken);

                    foreach (var region in _alertsiteHelper.GetRegions().Keys)
                    {
                        //var metrics = await _alertsiteHelper.GetRegionMetricsAsync(token, region);

                        _logger.LogWarning("ALERTSITE: calling API for region {Region}", region);
                        var metrics = await _alertsiteHelper.GetRegionMetricsAsync(token, region);
                        _logger.LogWarning("ALERTSITE: API returned {Count} metrics for region {Region}", metrics.Count, region);

                        foreach (var metric in metrics)
                        {
                            var regionLabel = metric.Region;
                            var appLabel = metric.App;
                            var monitorId = metric.MonitorId;
                            var monitorInterval = metric.MonitorInterval;

                            _prometheusGauges._appHealthGauge
                                .WithLabels(regionLabel, monitorId, appLabel)
                                .Set(metric.Value); 

                            var statusCode = ExtractStatusCode(metric);
                            _prometheusGauges._appStatusCodeGauge
                                .WithLabels(regionLabel, appLabel)
                                .Set(statusCode);

                            var category = Categorise(metric);
                            //_prometheusGauges._appErrorCategoryGauge
                            //    .WithLabels(regionLabel, appLabel, category)
                            //    .Set(1);


                            var allCategories = new[] { "OK", "http_5xx", "http_404", "http_403", "timeout", "other", "unknown" };

                            foreach (var cat in allCategories)
                            {
                                _prometheusGauges._appErrorCategoryGauge
                                    .WithLabels(regionLabel, appLabel, cat)
                                    .Set(cat == category ? 1 : 0);
                            }


                             //ERROR DURATION CLOCK

                            var key = $"{regionLabel}|{monitorId}|{appLabel}";

                            double errorMinutes = UpdateThresholdTimer(
                                _alertsiteErrorState,
                                key,
                                metric.Value,   // 1 = error, 0 = ok
                                1               
                            );

                            _prometheusGauges._appErrorMinutesGauge
                                .WithLabels(regionLabel, appLabel, monitorId)
                                .Set(errorMinutes);



                            if (double.TryParse(metric.ResponseTime, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var rtSeconds))
                                {
                                    _prometheusGauges._appResponseSecondsGauge
                                        .WithLabels(regionLabel, appLabel)
                                        .Set(rtSeconds);
                                }

                            if (TryParseAlertSiteTimestamp(metric.LastStatusAt, out var unix))
                            {
                                _prometheusGauges._appLastStatusTsGauge
                                    .WithLabels(regionLabel, appLabel, monitorInterval)
                                    .Set(unix);
                            }


                        }

                        _logger.LogInformation(
                            "Refreshed Alertsite metrics for region {Region}, {Count} apps.",
                            region, metrics.Count);
                    }     
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing Alertsite metrics.");
                }

                try
                {

                    using var stream = new MemoryStream();
                    await AlertsiteRegistryHolder.Registry.CollectAndExportAsTextAsync(stream);
                    stream.Position = 0;
                    using var reader = new StreamReader(stream);
                    CachedMetrics = reader.ReadToEnd();

                    _logger.LogInformation("Prometheus metrics updated.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error exporting Alertsite metrics.");
                }

                await Task.Delay(_refreshInterval, stoppingToken);
            }
         
        }


    }

}
