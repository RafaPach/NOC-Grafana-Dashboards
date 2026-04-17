using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NOCAPI.Modules.FTP.DTOs;
using NOCAPI.Modules.FTP.Helpers;
using NOCAPI.Modules.FTP.Prometheus;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using System.Xml.Linq;
using static NOCAPI.Modules.FTP.Prometheus.ModuleRegistry;

namespace NOCAPI.Modules.FTP.Services
{
    public class FtpMetricsBackgroundService : BackgroundService
    {
        private readonly ILogger<FtpMetricsBackgroundService> _logger;
        private readonly HostStatusHelper _statusHelper;
        private readonly EMEAPrimaryNode _emeaPrimaryNode;
        private readonly StatisticsHelper _auStatisticsHelper;
        private readonly NAHelper _naHelper;
        private readonly DailyStatistics _dailyStats;

        private readonly HostStatusMetrics _prometheus;
        private readonly TimeSpan _refreshInterval = TimeSpan.FromMinutes(1);

        //private void LogLocal(string message)
        //{
        //    var p = Path.Combine(AppContext.BaseDirectory, "ftp_debug.log");
        //    File.AppendAllText(p, $"{DateTime.UtcNow:u} {message}{Environment.NewLine}");
        //}

        public static string CachedMetrics = "# No FTP metrics exported yet";

        public FtpMetricsBackgroundService(
            ILogger<FtpMetricsBackgroundService> logger,
            HostStatusHelper statusHelper,
            EMEAPrimaryNode emeaPrimaryNode,
            StatisticsHelper auStatisticsHelper,
            HostStatusMetrics prometheus,
            NAHelper naHelper,
            DailyStatistics dailyStats)
        {
            _logger = logger;
            _statusHelper = statusHelper;
            _emeaPrimaryNode = emeaPrimaryNode;
            _prometheus = prometheus;
            _auStatisticsHelper = auStatisticsHelper;
            _naHelper = naHelper;
            _dailyStats = dailyStats;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting FTP metrics background service...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var hosts = await _statusHelper.GetAllHostStatusesAsync();

                    foreach (var host in hosts)
                    {
                        int statusValue = host.Status switch
                        {
                            "Nominal" => 0,
                            "Warning" => 1,
                            "Error" => 2,
                            _ => 3
                        };

                        _prometheus.FtpHostStatus
                            .WithLabels("Oceania", host.Host)
                            .Set(statusValue);

                        _prometheus.FtpHostTotalErrors
                                .WithLabels("Oceania", host.Host)
                                .Set(host.TotalErrors);

                        _prometheus.FtpHostLoadedActions
                            .WithLabels("Oceania", host.Host)
                            .Set(host.LoadedActions);

                        _prometheus.FtpHostEnabledActions
                            .WithLabels("Oceania", host.Host)
                            .Set(host.EnabledActions);

                        _prometheus.FtpHostRunningActions
                            .WithLabels("Oceania", host.Host)
                            .Set(host.RunningActions);

                        _prometheus.FtpHostFailedActions
                            .WithLabels("Oceania", host.Host)
                            .Set(host.FailedActions);

                    }

                    _logger.LogInformation("FTP metrics updated.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating FTP metrics.");
                }


                // NA BLOCK // 


                try
                {
                    var naNodes = await _naHelper.GetNANodeStatusesAsync();

                    foreach (var node in naNodes)
                    {
                        int statusValue = node.State switch
                        {
                            "Running" => 0,
                            "Warning" => 1,
                            "Error" => 2,
                            _ => 3
                        };

                        _prometheus.FtpHostStatus
                            .WithLabels("NA", node.Machine)
                            .Set(statusValue);

                        _prometheus.FtpHostTotalErrors
                                .WithLabels("NA", node.Machine)
                                .Set(node.TotalErrors);

                        _prometheus.FtpHostLoadedActions
                            .WithLabels("NA", node.Machine)
                            .Set(node.LoadedActions);

                        //_prometheus.FtpHostEnabledActions
                        //    .WithLabels("NA", node.Machine)
                        //    .Set(node.EnabledActions);

                        _prometheus.FtpHostRunningActions
                            .WithLabels("NA", node.Machine)
                            .Set(node.RunningActions);

                        //_prometheus.FtpHostFailedActions
                        //    .WithLabels("NA", node.Machine)
                        //    .Set(node.TotalErrors);
                    }

                    _logger.LogInformation("NA node metrics updated.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed updating NA node metrics.");
                }


                // EMEA BLOCK //

                try
                {
                    //LogLocal("WindowsIdentity: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                    //LogLocal("ENTER EMEA BLOCK");

                    //LogLocal("Calling EMEA endpoint: " + _emeaPrimaryNode._primaryNodeUrl);

                    var emea = await _emeaPrimaryNode.GetPrimaryNodeStatusAsync();

                    if (emea == null)
                    {
                        //LogLocal("EMEA IS NULL");
                        _prometheus.FtpHostStatus.WithLabels("EMEA", "UNKNOWN").Set(3);
                        return;
                    }

                    //LogLocal("EMEA RESPONSE: " + System.Text.Json.JsonSerializer.Serialize(emea));
                    //LogLocal("Updating metrics for " + emea.Machine);
                    //LogLocal("EMEA call completed");

                    int statusValue = emea.State switch
                    {
                        "Running" => 0,
                        "Warning" => 1,
                        "Error" => 2,
                        _ => 3
                    };

                    _prometheus.FtpHostStatus
                        .WithLabels("EMEA", emea.Machine)
                        .Set(statusValue);

                    _prometheus.FtpHostStatus
                          .WithLabels("EMEA", emea.Machine)
                          .Set(statusValue);

                    _prometheus.FtpHostTotalErrors
                            .WithLabels("EMEA", emea.Machine)
                            .Set(emea.TotalErrors);

                    _prometheus.FtpHostLoadedActions
                        .WithLabels("EMEA", emea.Machine)
                        .Set(emea.LoadedActions);

                    //_prometheus.FtpHostEnabledActions
                    //    .WithLabels("NA", node.Machine)
                    //    .Set(node.EnabledActions);

                    _prometheus.FtpHostRunningActions
                        .WithLabels("EMEA", emea.Machine)
                        .Set(emea.RunningActions);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed updating EMEA primary node metrics.");
                }


                // WEEKLY STATISTICS METRICS // 
                try
                {

                    var auHosts = new[]
                    {
                        "melyvpaftp01",
                        "melyvpaftp02",
                        "melyvpaftp06",
                        "melyvpaftp07",
                        "melyvpaftp08"
                    };

                    var naHosts = new[]
                    {
                        "torfsftp1",
                        "csavftp0",
                        "csavftp1",
                        "csavftp7",
                        "csavftp8",
                        "csavftp9",
                        "csavftp10",
                        "csavftp11",
                        "csavaarftp1",

                    };

                    var emeaHosts = new[]
                    {
                        "nepcvpcrtftp01"
                    };

                    await ProcessWeeklyStatsAsync(
                        "Oceania",
                        auHosts,
                        host => _auStatisticsHelper.GetWeeklyStatsAsync(host));

                    await ProcessWeeklyStatsAsync(
                        "NA",
                        naHosts,
                        host => _auStatisticsHelper.GetWeeklyStatsAsync_NA(host));

                    await ProcessWeeklyStatsAsync(
                        "EMEA",
                        emeaHosts,
                        host => _auStatisticsHelper.GetWeeklyStatsAsync_EMEA(host)
                     );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating FTP weekly statistics metrics.");
                }


                // DAILY STATS //

                try
                {
                    var auHosts = new[]
                    {
                        "melyvpaftp01",
                        "melyvpaftp02",
                        "melyvpaftp06",
                        "melyvpaftp07",
                        "melyvpaftp08"
                    };

                    await ProcessDailyStatsAsync(
                        "Oceania",
                        auHosts,
                        host => _dailyStats.GetDailyForcedAsync(host));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating FTP daily statistics metrics.");
                }

                try
                {
                    using var stream = new MemoryStream();
                    await FTPRegistryHolder.Registry.CollectAndExportAsTextAsync(stream);
                    stream.Position = 0;

                    using var reader = new StreamReader(stream);
                    CachedMetrics = reader.ReadToEnd();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error exporting Prometheus text for FTP metrics.");
                }

                await Task.Delay(_refreshInterval, stoppingToken);
            }
        }


        private static double ParseByteSize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0d;

            s = s.Trim().Replace(",", "");
            // Match number and optional unit; unit may be just "B", "KB", "MB", "GB", "TB"
            var m = Regex.Match(s, @"^\s*(?<num>[0-9]*\.?[0-9]+)\s*(?<unit>[KMGTP]?B)?\s*$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            if (!m.Success)
            {
                if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                    return val;
                return 0d;
            }

            var num = double.Parse(m.Groups["num"].Value, CultureInfo.InvariantCulture);
            var unit = (m.Groups["unit"].Success ? m.Groups["unit"].Value : "B").ToUpperInvariant();

            return unit switch
            {
                "B" => num,
                "KB" => num * 1_000d,
                "MB" => num * 1_000_000d,
                "GB" => num * 1_000_000_000d,
                "TB" => num * 1_000_000_000_000d,
                _ => num
            };
        }

        /// <summary>
        /// Parses "HH:MM:SS" or "HH:MM:SS.fffffff" into seconds (double).
        /// Also tolerates "MM:SS".
        /// </summary>
        private static double ParseDurationToSeconds(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0d;

            s = s.Trim();

            if (TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out var ts))
                return ts.TotalSeconds;

            // Fallback for "MM:SS"
            var parts = s.Split(':');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var mm) &&
                double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var ss))
            {
                return mm * 60 + ss;
            }

            return 0d;
        }

        private async Task ProcessWeeklyStatsAsync(
        string region,
        string[] hosts,
        Func<string, Task<StatsResponse?>> fetchFunc)
        {
            var results = new Dictionary<string, StatsResponse?>();

            foreach (var host in hosts)
            {
                results[host] = await fetchFunc(host);
            }

            foreach (var kvp in results)
            {
                var host = kvp.Key.ToUpper();
                var data = kvp.Value;

                if (data == null)
                {
                    _logger.LogWarning($"No weekly stats for host {host} in region {region}");
                    continue;
                }

                foreach (var row in data.Rows)
                {
                    var weekLabel = row.Day.ToString(CultureInfo.InvariantCulture);

                    var totalTrans = row.TotalTrans;
                    var totalBytes = ParseByteSize(row.TotalTransSize);
                    var avgBytes = ParseByteSize(row.AvgSize);
                    var maxBytes = ParseByteSize(row.MaxSize);
                    var avgSec = ParseDurationToSeconds(row.AvgTime);
                    var maxSec = ParseDurationToSeconds(row.MaxTime);

                    var labels = new[] { region, host, weekLabel };

                    _prometheus.FtpWeeklyStats.WithLabels(labels.Append("total_trans").ToArray()).Set(totalTrans);
                    _prometheus.FtpWeeklyStats.WithLabels(labels.Append("total_bytes").ToArray()).Set(totalBytes);
                    _prometheus.FtpWeeklyStats.WithLabels(labels.Append("avg_bytes").ToArray()).Set(avgBytes);
                    _prometheus.FtpWeeklyStats.WithLabels(labels.Append("max_bytes").ToArray()).Set(maxBytes);
                    _prometheus.FtpWeeklyStats.WithLabels(labels.Append("avg_seconds").ToArray()).Set(avgSec);
                    _prometheus.FtpWeeklyStats.WithLabels(labels.Append("max_seconds").ToArray()).Set(maxSec);
                }
            }

            _logger.LogInformation($"FTP weekly statistics metrics updated for region {region}.");
        }

        private async Task ProcessDailyStatsAsync(
        string region,
        IEnumerable<string> hosts,
        Func<string, Task<DailyStatsResponse?>> getDailyStats)
        {
            var tasks = hosts.Select(async host =>
            {
                try
                {
                    var stats = await getDailyStats(host);
                    if (stats == null || stats.Rows.Count == 0)
                        return;

                    foreach (var row in stats.Rows)
                    {
                        _prometheus.FtpDailyStatsThisMonth
                            .WithLabels(region, host.ToUpper(), row.Day.ToString())
                            .Set(row.TotalTrans);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing daily stats for host {Host}", host);
                }
            });

            await Task.WhenAll(tasks);

            _logger.LogInformation("Daily stats updated for region {Region}.", region);
        }

    }
}