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
    public class NAHelper
    {
        private readonly List<string> _nodes = new()
        {
            "http://torfsftp1/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp0/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp1/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp7/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp8/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp9/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp11/ftpschedadmin/ServiceManager.aspx",
            "http://csavftp10/ftpschedadmin/ServiceManager.aspx",
            "http://csavaarftp1/ftpschedadmin/ServiceManager.aspx"
        };

        private HttpClient CreateWindowsAuthClient()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,
                PreAuthenticate = true,
                Credentials = CredentialCache.DefaultNetworkCredentials,
                AllowAutoRedirect = true,
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        public async Task<List<NADTO>> GetNANodeStatusesAsync()
        {
            using var client = CreateWindowsAuthClient();

            var results = new List<NADTO>();

            foreach (var url in _nodes)
            {
                try
                {
                    string html = await client.GetStringAsync(url);

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    string GetSpanText(string id)
                    {
                        var node = doc.DocumentNode.SelectSingleNode($"//span[@id='{id}']");
                        return node?.InnerText.Trim() ?? "";
                    }

                    var dto = new NADTO
                    {
                        Machine = GetSpanText("lblMachineName"),
                        State = GetSpanText("lblState"),
                        //StartedOn = GetSpanText("lblStarted"),
                        LoadedActions = ParseInt(GetSpanText("lblLoadedActions")),
                        RunningActions = ParseInt(GetSpanText("lblRunningActions")),
                        DisabledActions = ParseInt(GetSpanText("lblDisabledActions")),
                        TotalErrors = ParseInt(GetSpanText("lblTotalErrors"))
                    };

                    results.Add(dto);
                }
                catch (Exception ex)
                {
                    results.Add(new NADTO
                    {
                        Machine = url,
                        State = "ERROR",
                        StartedOn = "",
                        TotalErrors = -1
                    });
                }
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