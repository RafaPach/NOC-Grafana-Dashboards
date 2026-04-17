using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.DTOs
{
    public class HostStatusDTO
    {

        public string Host { get; set; } = "";
        public string Status { get; set; } = "";
        public string UpTime { get; set; } = "";
        public int TotalErrors { get; set; }
        public int LoadedActions { get; set; }
        public int EnabledActions { get; set; }
        public int RunningActions { get; set; }
        public int FailedActions { get; set; }
    }
}
