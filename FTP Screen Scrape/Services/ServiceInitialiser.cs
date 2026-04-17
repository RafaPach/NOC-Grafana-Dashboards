using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NOCAPI.Modules.FTP.Helpers;
using NOCAPI.Modules.FTP.Prometheus;
using System.Net;

namespace NOCAPI.Modules.FTP.Services
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

                //services.AddSingleton<FtpHelper>();
                services.AddSingleton<StatisticsHelper>();
                services.AddSingleton<HostStatusHelper>();
                services.AddSingleton<EMEAPrimaryNode>();
                services.AddSingleton<HostStatusMetrics>();
                services.AddSingleton<NAHelper>();
                services.AddSingleton<DailyStatistics>();


                services.AddHostedService<FtpMetricsBackgroundService>();

                services.AddHttpClient();

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
