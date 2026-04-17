using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.DTOs
{
    public class StatisticsDTO
    {

        public int Day { get; set; }
        public int TotalTrans { get; set; }
        public string TotalTransSize { get; set; } = "";
        public string AvgSize { get; set; } = "";
        public string AvgTime { get; set; } = "";
        public string MaxSize { get; set; } = "";
        public string MaxTime { get; set; } = "";
    }

    public class StatsResponse
    {
        public string Period { get; set; } = "";
        public string Month { get; set; } = "";
        public int Year { get; set; }
        public List<StatisticsDTO> Rows { get; set; } = new();
    }


    public class ServerDailyStats
    {
        public string Host { get; set; } = "";
        public StatsResponse? Daily { get; set; }
        public string? Error { get; set; }
    }


}
