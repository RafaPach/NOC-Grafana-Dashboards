using HtmlAgilityPack;
using NOCAPI.Modules.FTP.DTOs;
using System.Globalization;
using System.Net;

namespace NOCAPI.Modules.FTP.Helpers
{
    public class HostStatusHelper
    {
        private readonly string _statusUrl =
            "http://ftpschedulerstatus/FTPSchedulerStatus/";

        private HttpClient CreateWindowsAuthClient()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,    // Its using kerberos
                PreAuthenticate = true,
                Credentials = CredentialCache.DefaultNetworkCredentials,
                AllowAutoRedirect = true,
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }


        public async Task<List<HostStatusDTO>> GetAllHostStatusesAsync()
        {


            using var client = CreateWindowsAuthClient();

            string html = await client.GetStringAsync(_statusUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Locate the Host Status Table
            var table = doc.DocumentNode.SelectSingleNode("//table[@id='HostsStatusGridView']");
            var results = new List<HostStatusDTO>();

            if (table == null)
                return results;

            // Select all non-header rows
            var trs = table.SelectNodes(".//tr[td and not(@class='tableHeader')]");
            if (trs == null)
                return results;

            foreach (var tr in trs)
            {
                var tds = tr.SelectNodes("./td");
                if (tds == null || tds.Count < 11)
                    continue;

                var adminNode = tds[10].SelectSingleNode(".//a");
                string adminUrl = adminNode?.GetAttributeValue("href", "") ?? "";

                results.Add(new HostStatusDTO
                {
                    Host = tds[0].InnerText.Trim(),
                    Status = tds[1].InnerText.Trim(),
                    UpTime = tds[4].InnerText.Trim(),
                    TotalErrors = ParseInt(tds[5].InnerText),
                    LoadedActions = ParseInt(tds[6].InnerText),
                    EnabledActions = ParseInt(tds[7].InnerText),
                    RunningActions = ParseInt(tds[8].InnerText),
                    FailedActions = ParseInt(tds[9].InnerText),
                });
            }

            return results;
        }

        private static int ParseInt(string s)
        {
            return int.TryParse(s.Trim(), NumberStyles.Any,
                CultureInfo.InvariantCulture, out var n)
                ? n
                : 0;
        }
    }
}