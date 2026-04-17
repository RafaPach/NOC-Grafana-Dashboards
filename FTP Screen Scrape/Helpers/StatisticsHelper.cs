using HtmlAgilityPack;
using NOCAPI.Modules.FTP.DTOs;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.Helpers
{
    public class StatisticsHelper
    {
        private readonly HttpClient _http;
        private readonly string _defaultUrl =
            "http://melyvpaftp01/FTPSchedulerAdmin/Statistics.aspx";

        public StatisticsHelper()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,    
                PreAuthenticate = true,
                Credentials = CredentialCache.DefaultNetworkCredentials,
                AllowAutoRedirect = true
            };

            _http = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        public async Task<StatsResponse?> GetWeeklyStatsAsync(string hostOrUrl)
        {
            string url = hostOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? hostOrUrl
                : $"http://{hostOrUrl}/FTPSchedulerAdmin/Statistics.aspx";

            string html = await _http.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Month
            var monthNode = doc.DocumentNode.SelectSingleNode("//select[@id='listMonth']/option[@selected]");
            int month = monthNode != null ? int.Parse(monthNode.GetAttributeValue("value", "0")) : 0;

            // Year
            string yearStr = doc.DocumentNode.SelectSingleNode("//input[@id='textYear']")
                ?.GetAttributeValue("value", "0") ?? "0";
            int year = int.TryParse(yearStr, out var y) ? y : DateTime.Now.Year;

            // Table
            var table = doc.DocumentNode.SelectSingleNode("//table[@id='dgRecords']");
            if (table == null) return null;

            var rows = new List<StatisticsDTO>();
            var trs = table.SelectNodes(".//tr[td and not(@class='tableHeader')]");

            if (trs != null)
            {
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes("./td");
                    if (tds == null || tds.Count < 7)
                        continue;

                    rows.Add(new StatisticsDTO
                    {
                        Day = SafeInt(tds[0].InnerText),         
                        TotalTrans = SafeInt(tds[1].InnerText),
                        TotalTransSize = tds[2].InnerText.Trim(),
                        AvgSize = tds[3].InnerText.Trim(),
                        AvgTime = tds[4].InnerText.Trim(),
                        MaxSize = tds[5].InnerText.Trim(),
                        MaxTime = tds[6].InnerText.Trim(),
                    });
                }
            }

            return new StatsResponse
            {
                Period = "Weekly",
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                Year = year,
                Rows = rows
            };
        }


        public async Task<StatsResponse?> GetWeeklyStatsAsync_NA(string hostOrUrl)
        {
            string url = hostOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? hostOrUrl
                : $"http://{hostOrUrl}/ftpschedadmin/Statistics.aspx";

            string html = await _http.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Month
            var monthNode = doc.DocumentNode.SelectSingleNode("//select[@id='listMonth']/option[@selected]");
            int month = monthNode != null ? int.Parse(monthNode.GetAttributeValue("value", "0")) : 0;

            // Year
            string yearStr = doc.DocumentNode.SelectSingleNode("//input[@id='textYear']")
                ?.GetAttributeValue("value", "0") ?? "0";
            int year = int.TryParse(yearStr, out var y) ? y : DateTime.Now.Year;

            // Table
            var table = doc.DocumentNode.SelectSingleNode("//table[@id='dgRecords']");
            if (table == null) return null;

            var rows = new List<StatisticsDTO>();
            var trs = table.SelectNodes(".//tr[td and not(@class='tableHeader')]");

            if (trs != null)
            {
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes("./td");
                    if (tds == null || tds.Count < 7)
                        continue;

                    rows.Add(new StatisticsDTO
                    {
                        Day = SafeInt(tds[0].InnerText),
                        TotalTrans = SafeInt(tds[1].InnerText),
                        TotalTransSize = tds[2].InnerText.Trim(),
                        AvgSize = tds[3].InnerText.Trim(),
                        AvgTime = tds[4].InnerText.Trim(),
                        MaxSize = tds[5].InnerText.Trim(),
                        MaxTime = tds[6].InnerText.Trim(),
                    });
                }
            }

            return new StatsResponse
            {
                Period = "Weekly",
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                Year = year,
                Rows = rows
            };
        }


        public async Task<StatsResponse?> GetWeeklyStatsAsync_EMEA(string hostOrUrl)
        {
            string url = hostOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? hostOrUrl
                : $"http://{hostOrUrl}.emea.cshare.net/FTPSchedulerAdmin/Statistics.aspx";

            string html = await _http.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Month
            var monthNode = doc.DocumentNode.SelectSingleNode("//select[@id='listMonth']/option[@selected]");
            int month = monthNode != null ? int.Parse(monthNode.GetAttributeValue("value", "0")) : 0;

            // Year
            string yearStr = doc.DocumentNode.SelectSingleNode("//input[@id='textYear']")
                ?.GetAttributeValue("value", "0") ?? "0";
            int year = int.TryParse(yearStr, out var y) ? y : DateTime.Now.Year;

            // Table
            var table = doc.DocumentNode.SelectSingleNode("//table[@id='dgRecords']");
            if (table == null) return null;

            var rows = new List<StatisticsDTO>();
            var trs = table.SelectNodes(".//tr[td and not(@class='tableHeader')]");

            if (trs != null)
            {
                foreach (var tr in trs)
                {
                    var tds = tr.SelectNodes("./td");
                    if (tds == null || tds.Count < 7)
                        continue;

                    rows.Add(new StatisticsDTO
                    {
                        Day = SafeInt(tds[0].InnerText),
                        TotalTrans = SafeInt(tds[1].InnerText),
                        TotalTransSize = tds[2].InnerText.Trim(),
                        AvgSize = tds[3].InnerText.Trim(),
                        AvgTime = tds[4].InnerText.Trim(),
                        MaxSize = tds[5].InnerText.Trim(),
                        MaxTime = tds[6].InnerText.Trim(),
                    });
                }
            }

            return new StatsResponse
            {
                Period = "Weekly",
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                Year = year,
                Rows = rows
            };
        }

        public async Task<Dictionary<string, StatsResponse?>> GetWeeklyForHostsAsync(IEnumerable<string> hosts)
        {
            var tasks = new List<Task<(string Host, StatsResponse? Data)>>();

            foreach (var host in hosts)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var data = await GetWeeklyStatsAsync(host);
                        return (host, data);
                    }
                    catch
                    {
                        return (host, (StatsResponse?)null);
                    }
                }));
            }

            var results = await Task.WhenAll(tasks);

            var dict = new Dictionary<string, StatsResponse?>(StringComparer.OrdinalIgnoreCase);
            foreach (var (host, data) in results)
                dict[host] = data;

            return dict;
        }

        private static int SafeInt(string s)
    => int.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n) ? n : 0;
    }
}