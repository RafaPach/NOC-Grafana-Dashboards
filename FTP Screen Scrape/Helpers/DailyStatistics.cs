using HtmlAgilityPack;
using NOCAPI.Modules.FTP.DTOs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.Helpers
{
    public class DailyStatistics
    {
        private readonly HttpClient _http;
        private readonly string _defaultUrl;

        public DailyStatistics()
        {
            _defaultUrl = "http://melyvpaftp01/FTPSchedulerAdmin/Statistics.aspx";

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

        public Task<DailyStatsResponse> GetDailyForcedAsync()
            => GetDailyForcedAsync(_defaultUrl);

        public async Task<DailyStatsResponse> GetDailyForcedAsync(string hostOrUrl)
        {
            string url = hostOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? hostOrUrl
                : $"http://{hostOrUrl}/FTPSchedulerAdmin/Statistics.aspx";

            string html = await _http.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string viewstate = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATE']")
                ?.GetAttributeValue("value", "") ?? "";

            string eventValidation = doc.DocumentNode.SelectSingleNode("//input[@id='__EVENTVALIDATION']")
                ?.GetAttributeValue("value", "") ?? "";

            string viewstateGen = doc.DocumentNode.SelectSingleNode("//input[@id='__VIEWSTATEGENERATOR']")
                ?.GetAttributeValue("value", "") ?? "";


            var periodNode = doc.DocumentNode.SelectSingleNode("//select[@id='listPeriod']/option[@selected]");
            string period = periodNode?.GetAttributeValue("value", "0") ?? "0";


            // Default month
            var monthNode = doc.DocumentNode.SelectSingleNode("//select[@id='listMonth']/option[@selected]");
            int month = monthNode != null
                ? int.Parse(monthNode.GetAttributeValue("value", "1"))
                : 1;

            // Default year
            string yearStr = doc.DocumentNode.SelectSingleNode("//input[@id='textYear']")
                ?.GetAttributeValue("value", "0") ?? "0";

            int year = int.TryParse(yearStr, out var y) ? y : DateTime.Now.Year;

            // POST BACK forcing daily (period = 5)
            var form = new List<KeyValuePair<string, string>>
                    {
                        new("__VIEWSTATE", viewstate),
                        new("__VIEWSTATEGENERATOR", viewstateGen),
                        new("__EVENTVALIDATION", eventValidation),

                        new("listPeriod", "5"),               // FORCE DAILY
                        //new("listPeriod", period),
                        new("listMonth", month.ToString()),   // USE DEFAULT MONTH
                        new("textYear", year.ToString()),     // USE DEFAULT YEAR

                        new("btnRefresh", "Refresh")
                    };

            var content = new FormUrlEncodedContent(form);
            var post = await _http.PostAsync(url, content);
            post.EnsureSuccessStatusCode();

            string postHtml = await post.Content.ReadAsStringAsync();

            //  Parse DAILY TABLE
            var postDoc = new HtmlDocument();
            postDoc.LoadHtml(postHtml);

            var table = postDoc.DocumentNode.SelectSingleNode("//table[@id='dgRecords']");
            var rows = new List<DailyStatisticsDTO>();

            if (table != null)
            {
                var trs = table.SelectNodes(".//tr[td and not(@class='tableHeader')]");
                if (trs != null)
                {
                    foreach (var tr in trs)
                    {
                        var tds = tr.SelectNodes("./td");
                        if (tds == null || tds.Count < 7) continue;

                        rows.Add(new DailyStatisticsDTO
                        {
                            Day = SafeInt(tds[0].InnerText),
                            TotalTrans = SafeInt(tds[1].InnerText)
                        });
                    }
                }
            }

            return new DailyStatsResponse
            {
                Period = "Daily",
                Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                Year = year,
                Rows = rows
            };
        }

        public async Task<Dictionary<string, DailyStatsResponse?>> GetDailyForcedForHostsAsync(IEnumerable<string> hosts)
        {
            var tasks = new List<Task<(string Host, DailyStatsResponse? Data)>>();

            foreach (var host in hosts)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var data = await GetDailyForcedAsync(host);
                        return (host, data);
                    }
                    catch
                    {
                        return (host, (DailyStatsResponse?)null);
                    }
                }));
            }

            var results = await Task.WhenAll(tasks);

            var dict = new Dictionary<string, DailyStatsResponse?>(StringComparer.OrdinalIgnoreCase);
            foreach (var (host, data) in results)
                dict[host] = data;

            return dict;
        }

        private static int SafeInt(string s)
            => int.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n) ? n : 0;
    }
}
