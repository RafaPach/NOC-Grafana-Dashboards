using Prometheus;

namespace NOCAPI.Modules.Solarwinds.Prometheus
{
    public class ModuleRegistry
    {
        public static class SolarwindsRegistryHolder
        {
            public static readonly CollectorRegistry Registry = new CollectorRegistry();
            public static readonly MetricFactory M = Metrics.WithCustomRegistry(Registry);
        }
    }
}