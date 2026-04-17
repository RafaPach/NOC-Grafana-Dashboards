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
    public class EMEAPrimaryNode
    {
        public readonly string _primaryNodeUrl = "http://nepcvpcrtftp01.emea.cshare.net/FTPSchedulerAdmin/ServiceManager.aspx";

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

        public async Task<EMEAPrimaryNodeDTO> GetPrimaryNodeStatusAsync()
        {
            using var client = CreateWindowsAuthClient();

            string html = await client.GetStringAsync(_primaryNodeUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            string GetSpanText(string id)
            {
                var node = doc.DocumentNode.SelectSingleNode($"//span[@id='{id}']");
                return node?.InnerText.Trim() ?? "";
            }

            var dto = new EMEAPrimaryNodeDTO
            {
                Machine = GetSpanText("lblMachineName"),
                State = GetSpanText("lblState"),
                //Version = GetSpanText("lblVersion"),
                //UpTime = GetSpanText("lblUptime"),
                StartedOn = GetSpanText("lblStarted"),
                //Interval = GetSpanText("lblInterval"),
                LoadedActions = ParseInt(GetSpanText("lblLoadedActions")),
                RunningActions = ParseInt(GetSpanText("lblRunningActions")),
                DisabledActions = ParseInt(GetSpanText("lblDisabledActions")),
                TotalErrors = ParseInt(GetSpanText("lblTotalErrors"))
            };

            return dto;
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
