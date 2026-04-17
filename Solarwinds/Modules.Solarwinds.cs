using Microsoft.AspNetCore.Mvc;
using NOCAPI.Modules.Solarwinds.Initialiser;
using NOCAPI.Modules.Solarwinds.Services;

namespace NOCAPI.Modules.Solarwinds
{
    [ApiController]
    [Route("api/solarwinds")]
    public class SolarwindsController : ControllerBase
    {
        public SolarwindsController()
        {
            ServiceInitialiser.Initialize();
        }

        [HttpGet("server")]
        public IActionResult GetServerStatusMetrics()
        {
            var metrics = SolarwindsMetricsBackgroundService.CachedMetrics;

            if (string.IsNullOrWhiteSpace(metrics) || metrics == "# No Prometheus data yet")
            {
                return Content("# No prometheus metrics yet, waiting for background refresh.", "text/plain");
            }

            return Content(metrics, "text/plain; version=0.0.4");
        }
    }
}