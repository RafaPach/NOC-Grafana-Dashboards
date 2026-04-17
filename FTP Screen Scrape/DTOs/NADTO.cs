using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.DTOs
{
    public class NADTO
    {
        public string Machine { get; set; }
        public string State { get; set; }
        //public string Version { get; set; }
        //public string UpTime { get; set; }
        public string StartedOn { get; set; }
        //public string Interval { get; set; }
        public int LoadedActions { get; set; }
        public int RunningActions { get; set; }
        public int DisabledActions { get; set; }
        public int TotalErrors { get; set; }
    }
}
