using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NOCAPI.Modules.Solarwinds.Helpers;
using NOCAPI.Modules.Solarwinds.Prometheus;
using NOCAPI.Modules.Solarwinds.Queries;
using Prometheus;
using SolarWinds.Api.Orion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static NOCAPI.Modules.Solarwinds.Prometheus.ModuleRegistry;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NOCAPI.Modules.Solarwinds.Services
{
    public class SolarwindsMetricsBackgroundService : BackgroundService
    {
        private readonly ILogger<SolarwindsMetricsBackgroundService> _logger;
        private readonly SolarwindsHelper _helper;
        private readonly PrometheusMetrics _metrics;

        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(1);

        public static string CachedMetrics = "# No Solarwinds Prometheus data yet";

        private readonly Dictionary<string, ThresholdState> _cpuState = new();
        private readonly Dictionary<string, ThresholdState> _memoryState = new();

        private class ThresholdState
        {
            public bool IsHigh;
            public DateTime HighStart;
        }

        public SolarwindsMetricsBackgroundService(
            ILogger<SolarwindsMetricsBackgroundService> logger,
            SolarwindsHelper helper,
            PrometheusMetrics metrics)
        {
            _logger = logger;
            _helper = helper;
            _metrics = metrics;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting SolarWinds metrics background service...");

            //_metrics._nodeStatusGauge.Unpublish();
            //_metrics._nodeUpGauge.Unpublish();
            //_metrics._nodeCpuPercentGauge.Unpublish();
            //_metrics._nodeCpuOver90MinutesGauge.Unpublish();
            //_metrics._nodeMemoryOver90MinutesGauge.Unpublish();
            //_metrics._nodeMemoryUsedPercentGauge.Unpublish();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RefreshNodeStatus(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing SolarWinds node status metrics.");
                }

                try
                {
                    await RefreshCpuAndMemory(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing SolarWinds CPU/Memory metrics.");
                }

                try
                {
                    await RefreshServerTypeMetrics(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing SolarWinds type metrics");
                }

                try
                {
                    await RefreshNetworkInterfaces(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error refreshing SolarWinds NETWORK metrics");
                }

                try
                {
                    using var stream = new MemoryStream();
                    await SolarwindsRegistryHolder.Registry.CollectAndExportAsTextAsync(stream);
                    stream.Position = 0;
                    using var reader = new StreamReader(stream);
                    CachedMetrics = reader.ReadToEnd();

                    _logger.LogInformation("Solarwinds metrics updated.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error exporting SolarWinds metrics.");
                }

                await Task.Delay(_refreshInterval, stoppingToken);
            }
        }

        private async Task RefreshServerTypeMetrics(CancellationToken stoppingToken)
        {
            var results = await _helper.ServerType(stoppingToken);

            var filteredResults = results
                .Where(r =>
                    !Safe(r["NodeName"]).Contains("WAT", StringComparison.OrdinalIgnoreCase) &&
                    !Safe(r["NodeName"]).Contains("TEST", StringComparison.OrdinalIgnoreCase) &&
                    !Safe(r["NodeName"]).Contains("DEV", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var r in filteredResults)
            {
                var region = Safe(r["Region"]);
                var nodeName = Safe(r["NodeName"]);
                var serverType = Safe(r["ServerType"]);
                var rawBus = Safe(r["BusApplication"]);
                var busApplication = NormalizeBusApplication(rawBus);


                int typeCode = serverType switch
                {
                    "SQL" => 1,
                    "Windows" => 2,
                    "Linux" => 3,
                    "Unknown" => 4,
                };

                    _metrics._serverType
                    .WithLabels(region, nodeName, busApplication)
                    .Set(typeCode);
            }
        }


        private async Task RefreshNodeStatus(CancellationToken stoppingToken)
        {
            var results = await _helper.QueryNodesAcrossRegionsAsync(stoppingToken);

            var filteredResults = results
                .Where(r =>
                    !Safe(r["NodeName"]).Contains("WAT", StringComparison.OrdinalIgnoreCase) &&
                    !Safe(r["NodeName"]).Contains("TEST", StringComparison.OrdinalIgnoreCase) &&
                    !Safe(r["NodeName"]).Contains("DEV", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var r in filteredResults)
            {
                var region = Safe(r["Region"]);
                var nodeName = Safe(r["NodeName"]);
                var statusDescription = Safe(r["StatusDescription"]);
                var rawBus = Safe(r["BusApplication"]);
                var busApplication = NormalizeBusApplication(rawBus);
                var statusToken = Safe(r["Status"]);

                int statusCode = 0;
                if (!string.IsNullOrEmpty(statusToken))
                {
                    if (!int.TryParse(statusToken, out statusCode))
                        statusCode = 0;
                }

                //_metrics._nodeStatusGauge
                //    .WithLabels(region, nodeName, busApplication, statusDescription)
                //    .Set(statusCode);


                _metrics._nodeStatusGauge
                    .WithLabels(region, nodeName, busApplication)
                    .Set(statusCode);

                var up = (statusCode == 1) ? 1 : 0;

                _metrics._nodeUpGauge
                    .WithLabels(region, nodeName, busApplication)
                    .Set(up);
            }
        }

        private async Task RefreshCpuAndMemory(CancellationToken stoppingToken)
        {
            var result = await _helper.QueryMemoryAcrossRegionsAsync(stoppingToken);

            var filteredResults = result
              .Where(r =>
                  !Safe(r["NodeName"]).Contains("WAT", StringComparison.OrdinalIgnoreCase) &&
                  !Safe(r["NodeName"]).Contains("TEST", StringComparison.OrdinalIgnoreCase) &&
                  !Safe(r["NodeName"]).Contains("DEV", StringComparison.OrdinalIgnoreCase) &&
                  !Safe(r["NodeName"]).Contains("VUQ", StringComparison.OrdinalIgnoreCase) &&
                  !Safe(r["NodeName"]).Contains("VUA", StringComparison.OrdinalIgnoreCase)
              )
              .ToList();

            foreach (var r in filteredResults)
            {
                var region = Safe(r["Region"]);
                var nodeName = Safe(r["NodeName"]);
                var rawBus = Safe(r["BusApplication"]);
                var busApplication = NormalizeBusApplication(rawBus);

                var cpuStr = Safe(r["CPU"]);
                var memoryUsedStr = Safe(r["MemoryUsed"]);

                var cpuPct = ParseDoubleOrZero(cpuStr);
                var memPct = ParseDoubleOrZero(memoryUsedStr);

                var key = $"{region}|{nodeName}|{busApplication}";

                _metrics._nodeCpuPercentGauge
                    .WithLabels(region, nodeName, busApplication)
                    .Set(cpuPct);

                _metrics._nodeMemoryUsedPercentGauge
                    .WithLabels(region, nodeName, busApplication)
                    .Set(memPct);

                double cpuMinutes = UpdateThresholdTimer(_cpuState, key, cpuPct, 90);

                _metrics._nodeCpuOver90MinutesGauge
                    .WithLabels(region, nodeName, busApplication)
                    .Set(cpuMinutes);

                double memMinutes = UpdateThresholdTimer(_memoryState, key, memPct, 90);

                _metrics._nodeMemoryOver90MinutesGauge
                    .WithLabels(region, nodeName, busApplication)
                    .Set(memMinutes);
            }
        }

        private async Task RefreshNetworkInterfaces(CancellationToken stoppingToken)
        {
            var results = await _helper.QueryNetworkInterfacesAsync(stoppingToken);

            var filteredResults = results
                .Where(r =>
                    !Safe(r["NodeName"]).Contains("TEST", StringComparison.OrdinalIgnoreCase) &&
                    !Safe(r["NodeName"]).Contains("DEV", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var r in filteredResults)
            {


                var region = Safe(r["Region"]);
                var serviceType = Safe(r["ServiceType"]);
                var nodeName = Safe(r["NodeName"]);
                var interfaceName = Safe(r["InterfaceName"]);
                var interfaceAlias = Safe(r["InterfaceAlias"]);

                var inMbps = ParseDoubleOrZero(Safe(r["InMbps"]));
                var outMbps = ParseDoubleOrZero(Safe(r["OutMbps"]));

                //var inUtil = (Safe(r["InUtilPercent"]));
                //var outUtil = (Safe(r["OutUtilPercent"]));

                var inUtil = r["InUtilPercent"]?.Value<double>() ?? 0.0;
                var outUtil = r["OutUtilPercent"]?.Value<double>() ?? 0.0;

                var operStatusRaw = Safe(r["OperStatus"]);
                var operUp = operStatusRaw == "1" ? 1 : 0;

                var isDataCentre = Safe(r["IsDataCentre"]) == "1" ? "true" : "false";

                var cpuLoad = ParseDoubleOrZero(Safe(r["CpuLoadPercent"]));
                var memoryUsed = ParseDoubleOrZero(Safe(r["MemoryUsedPercent"]));
                var packetLoss = ParseDoubleOrZero(Safe(r["PacketLossPercent"]));
                var responseTime = ParseDoubleOrZero(Safe(r["ResponseTimeMs"]));

                var site = Safe(r["Site"]);

                string lat = null;
                string lon = null;

                if (!string.IsNullOrWhiteSpace(site) &&
                    SiteResolver.SiteGeo.TryGetValue(site, out var geo))
                {
                    lat = geo.Lat.ToString(CultureInfo.InvariantCulture);
                    lon = geo.Lon.ToString(CultureInfo.InvariantCulture);
                }

                _metrics._interfaceInMbpsGauge
                    .WithLabels(region, serviceType, nodeName, lat, lon, site ,interfaceName, interfaceAlias, isDataCentre)
                    .Set(inMbps);


                _metrics._interfaceOutMbpsGauge
                    .WithLabels(region, serviceType, nodeName, lat, lon, site, interfaceName, interfaceAlias, isDataCentre)
                    .Set(outMbps);

                _metrics._interfaceInUtilPercentGauge
                    .WithLabels(region, serviceType, nodeName, lat, lon, site, interfaceName, interfaceAlias, isDataCentre)
                    .Set(inUtil);

                _metrics._interfaceOutUtilPercentGauge
                    .WithLabels(region, serviceType, nodeName, lat, lon, site, interfaceName, interfaceAlias, isDataCentre)
                    .Set(outUtil);

                _metrics._interfaceUpGauge
                    .WithLabels(region, serviceType, nodeName, lat, lon, site, interfaceName, interfaceAlias, isDataCentre)
                    .Set(operUp);

                _metrics._interfacenodeCpuLoadGauge
                .WithLabels(region, serviceType, nodeName, site, lat, lon)
                .Set(cpuLoad);

                _metrics._interfacenodeMemoryUsedGauge
                    .WithLabels(region, serviceType, nodeName, site, lat, lon)
                    .Set(memoryUsed);

                _metrics._interfacenodePacketLossGauge
                    .WithLabels(region, serviceType, nodeName, site, lat, lon)
                    .Set(packetLoss);

                _metrics._interfacenodeResponseTimeGauge
                    .WithLabels(region, serviceType, nodeName, site, lat, lon)
                    .Set(responseTime);
            }
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

        private static string Safe(JToken? token)
            => token?.ToString() ?? string.Empty;

        private static double ParseDoubleOrZero(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return 0d;

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v))
                return v;

            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
                return v;

            return 0d;
        }

        static double GetDouble(JToken token)
        {
            if (token == null || token.Type == JTokenType.Null)
                return 0;

            return token.Value<double>();
        }

        private static string NormalizeBusApplication(string bus)
        {
            if (string.IsNullOrWhiteSpace(bus))
                return string.Empty;

            var trimmed = bus.Trim();

            if (trimmed.StartsWith("Investor Center", StringComparison.OrdinalIgnoreCase) ||
                trimmed.StartsWith("Investor Centre", StringComparison.OrdinalIgnoreCase))
            {
                return "Investor Centre";
            }

            if (trimmed.Contains("Global Entity", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Contains("GEMS", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Contains("Global Entity Management", StringComparison.OrdinalIgnoreCase))
            {
                return "GEMS";
            }

            if (trimmed.Contains("Equate Plus", StringComparison.OrdinalIgnoreCase))
            {
                return "EquatePlus";
            }

            if (trimmed.Contains("Ping", StringComparison.OrdinalIgnoreCase))
            {
                return "PING";
            }

            return trimmed;
        }

    }
}

