using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.DTOs
{
    public class DailyStatsResponse
    {
        public string Period { get; set; } = "";
        public string Month { get; set; } = "";
        public int Year { get; set; }
        public List<DailyStatisticsDTO> Rows { get; set; } = new();
    }

    public class DailyStatisticsDTO
    {
        public int Day { get; set; }
        public int TotalTrans { get; set; }
    }

}
