using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NOCAPI.Modules.Solarwinds.Helpers;
using NOCAPI.Modules.Solarwinds.Prometheus;
using NOCAPI.Modules.Solarwinds.Services;
using System.Net;

namespace NOCAPI.Modules.Solarwinds.Initialiser
{
    public class ServiceInitialiser
    {
        private static readonly object _lock = new();
        private static bool _initialized = false;

        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public static void Initialize()
        {
            if (_initialized)
                return;

            lock (_lock)
            {
                if (_initialized)
                    return;

                var services = new ServiceCollection();

                services.AddMemoryCache();
                services.AddScoped<SolarwindsHelper>();
                services.AddSingleton<PrometheusMetrics>();
                services.AddSingleton<ModuleRegistry>();
                services.AddHostedService<SolarwindsMetricsBackgroundService>();

                services.AddHttpClient("Default")
                    .ConfigurePrimaryHttpMessageHandler(() =>
                        new HttpClientHandler
                        {
                            UseProxy = true,
                            Proxy = WebRequest.GetSystemWebProxy(),
                            ServerCertificateCustomValidationCallback =
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        });

                ServiceProvider = services.BuildServiceProvider();

                foreach (var hosted in ServiceProvider.GetServices<IHostedService>())
                {
                    hosted.StartAsync(CancellationToken.None)
                          .GetAwaiter()
                          .GetResult();
                }

                _initialized = true;
            }
        }
    }
}
