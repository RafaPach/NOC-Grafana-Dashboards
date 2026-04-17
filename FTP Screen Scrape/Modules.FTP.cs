using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NOCAPI.Modules.FTP.Helpers;
using NOCAPI.Modules.FTP.Services;

namespace NOCAPI.Modules.FTP
{


    [ApiController]
    [Route("api/FTP")]
    public class FTPController : ControllerBase
    {

        public FTPController() { 

            ServiceInitialiser.Initialize();

        }

        [HttpGet("Global/HostStatus")]
        public IActionResult GetStatus()
        {
            try
            {

                var metrics = FtpMetricsBackgroundService.CachedMetrics;

                if (string.IsNullOrWhiteSpace(metrics) || metrics == "# No FTP metrics exported yet")
                {
                    return Content("# No prometheus metrics yet, waiting for background refresh.", "text/plain");
                }

                return Content(metrics, "text/plain; version=0.0.4");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to fetch metrics.");
            }
        }

    }
}