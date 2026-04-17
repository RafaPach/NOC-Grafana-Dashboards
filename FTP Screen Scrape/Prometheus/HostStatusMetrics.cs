using Prometheus;
using static NOCAPI.Modules.FTP.Prometheus.ModuleRegistry;

namespace NOCAPI.Modules.FTP.Prometheus
{
    public class HostStatusMetrics
    {
        public readonly Gauge FtpHostStatus = FTPRegistryHolder.M.CreateGauge(
        "ftp_host_status",
          "FTP Host Status (0=Nominal, 1=Warning, 2=Error)",
        new GaugeConfiguration
        {
            LabelNames = new[] { "region","host" }}
        );

        public readonly Gauge FtpHostTotalErrors = FTPRegistryHolder.M.CreateGauge(
            "ftp_host_total_errors",
            "Total number of errors for the FTP host",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "host" }
            }
        );

        public readonly Gauge FtpHostLoadedActions = FTPRegistryHolder.M.CreateGauge(
            "ftp_host_loaded_actions",
            "Total loaded actions for the FTP host",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "host" }
            }
        );

        public readonly Gauge FtpHostEnabledActions = FTPRegistryHolder.M.CreateGauge(
            "ftp_host_enabled_actions",
            "Total enabled actions for the FTP host",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "host" }
            }
        );

        public readonly Gauge FtpHostRunningActions = FTPRegistryHolder.M.CreateGauge(
            "ftp_host_running_actions",
            "Currently running actions for the FTP host",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "host" }
            }
        );

        public readonly Gauge FtpHostFailedActions = FTPRegistryHolder.M.CreateGauge(
            "ftp_host_failed_actions",
            "Currently failed actions for the FTP host",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "host" }
            }
        );


        public readonly Gauge FtpWeeklyStats = FTPRegistryHolder.M.CreateGauge(
        "ftp_weekly_stats",
        "Weekly FTP statistics per host (value depends on 'type' label)",
        new GaugeConfiguration
        {
            LabelNames = new[] { "region", "host", "week", "type" } // type = total_trans|total_bytes|avg_bytes|max_bytes|avg_seconds|max_seconds
        });

        public readonly Gauge FtpDailyStatsThisMonth = FTPRegistryHolder.M.CreateGauge(
            "ftp_daily_stats",
            "Daily FTP statistics per host (Total Nr Transactions ",
            new GaugeConfiguration
            {
                LabelNames = new[] {"region", "host", "day"} 
            }
            );
    }
}
