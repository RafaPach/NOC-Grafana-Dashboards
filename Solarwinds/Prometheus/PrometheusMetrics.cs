using NOCAPI.Modules.Solarwinds.Queries;
using Prometheus;
using static NOCAPI.Modules.Solarwinds.Prometheus.ModuleRegistry;

namespace NOCAPI.Modules.Solarwinds.Prometheus
{
    public class PrometheusMetrics
    {
        // Numeric status as reported by SolarWinds (1=Up, others are non-up states)
        public readonly Gauge _nodeStatusGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_node_status",
            "SolarWinds node numeric status (1=Up).",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "node", "busApplication" }
            });

        // Convenience 'up' flag derived from status (1=Up, 0=Down/Other)
        public readonly Gauge _nodeUpGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_node_up",
            "SolarWinds node up flag (1=Up, 0=Down/Other).",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "node", "busApplication" }
            });


        //CPU usage
        public readonly Gauge _nodeCpuPercentGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_node_cpu_percent",
            "SolarWinds node CPU percentage (0-100).",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "node", "busApplication" }
            });

        //Memory used
        public readonly Gauge _nodeMemoryUsedPercentGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_node_memory_used_percent",
            "SolarWinds node memory used percentage (0-100).",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "node", "busApplication" }
            });

        //public readonly Gauge _nodeCpuOver90MinutesGauge = SolarwindsRegistryHolder.M.CreateGauge(
        //    "solarwinds_cpu_over_90_minutes",
        //    "Minutes the node has been over 90% CPU",
        //    new GaugeConfiguration
        //    {
        //        LabelNames = new[] { "region", "node", "busApplication" }
        //});

        public readonly Gauge _nodeMemoryOver90MinutesGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_memory_over_90_minutes",
            "Minutes the node has been over 90% Memory",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "node", "busApplication" }
            });

        public readonly Gauge _nodeCpuOver90MinutesGauge = SolarwindsRegistryHolder.M.CreateGauge(
         "solarwinds_cpu_over_90_minutes",
         "Minutes the node has been over 90% CPU",
         new GaugeConfiguration
         {
             LabelNames = new[] { "region", "node", "busApplication" }
         });

        public readonly Gauge _serverType = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_server_type",
            "The type of server detected (Windows, Linux, SQL, etc.)",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "node", "busApplication" }
            });


        // NETWORKS //


        // Interface throughput (Mbps)
        public readonly Gauge _interfaceInMbpsGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_interface_in_mbps",
            "SolarWinds interface inbound traffic in Mbps.",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "service_type", "node", "lat", "lon", "site", "interface", "alias", "is_datacentre" }
            });

        public readonly Gauge _interfaceOutMbpsGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_interface_out_mbps",
            "SolarWinds interface outbound traffic in Mbps.",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "service_type", "node", "lat", "lon", "site", "interface", "alias", "is_datacentre" }
            });

        // Interface utilisation (%)
        public readonly Gauge _interfaceInUtilPercentGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_interface_in_util_percent",
            "SolarWinds interface inbound utilisation percentage.",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "service_type", "node", "lat", "lon", "site", "interface", "alias", "is_datacentre" }
            });

        public readonly Gauge _interfaceOutUtilPercentGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_interface_out_util_percent",
            "SolarWinds interface outbound utilisation percentage.",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "service_type", "node", "lat", "lon", "site", "interface", "alias", "is_datacentre" }
            });

        // Interface state (up/down)
        public readonly Gauge _interfaceUpGauge = SolarwindsRegistryHolder.M.CreateGauge(
            "solarwinds_interface_up",
            "SolarWinds interface operational state (1 = up, 0 = down).",
            new GaugeConfiguration
            {
                LabelNames = new[] { "region", "service_type", "node", "lat", "lon", "site", "interface", "alias", "is_datacentre" }
            });

        public readonly Gauge _interfacenodeCpuLoadGauge =
     SolarwindsRegistryHolder.M.CreateGauge(
        "solarwinds_interface_node_cpu_load_percent",
        "Node CPU usage percentage",
        new[] { "region", "service_type", "node", "site" , "lat", "lon" });

        public readonly Gauge _interfacenodeMemoryUsedGauge =
            SolarwindsRegistryHolder.M.CreateGauge(
                "solarwinds_interface_node_memory_used_percent",
                "Node memory usage percentage",
                new[] { "region", "service_type", "node","site", "lat", "lon" });

        public readonly Gauge _interfacenodePacketLossGauge =
            SolarwindsRegistryHolder.M.CreateGauge(
                "solarwinds_interface_node_packet_loss_percent",
                "Node ICMP packet loss percentage",
                new[] { "region", "service_type", "node", "site", "lat", "lon" });

        public readonly Gauge _interfacenodeResponseTimeGauge =
            SolarwindsRegistryHolder.M.CreateGauge(
                "solarwinds_interface_node_response_time_ms",
                "Node ICMP response time in ms",
                new[] { "region", "service_type", "node", "site", "lat", "lon"});
    }
}