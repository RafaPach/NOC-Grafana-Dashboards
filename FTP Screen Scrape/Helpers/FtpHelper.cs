using HtmlAgilityPack;
using NOCAPI.Modules.FTP.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NOCAPI.Modules.FTP.Helpers
{
    public class FtpHelper
    {

        private readonly HttpClient _http;
        private const string Url = "http://nepcvpcrtftp01.emea.cshare.net/FTPSchedulerAdmin/ServiceManager.aspx";

        public FtpHelper()
        {
            var handler = new HttpClientHandler
            {
                UseDefaultCredentials = true,                       
                PreAuthenticate = true,
                Credentials = CredentialCache.DefaultNetworkCredentials
            };

            _http = new HttpClient(handler);
        }

        public async Task<FtpDto> GetStatusAsync()
        {
            var html = await _http.GetStringAsync(Url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // TODO: replace with your actual node IDs after inspecting the HTML
            var statusNode =
                doc.DocumentNode.SelectSingleNode("//*[@id='ftpStatus']");

            var fileCountNode =
                doc.DocumentNode.SelectSingleNode("//*[@id='fileCount']");

            var dto = new FtpDto
            {
                Status = statusNode?.InnerText?.Trim() ?? "UNKNOWN",
                FileCount = int.TryParse(fileCountNode?.InnerText?.Trim(), out var count) ? count : -1,
                RawHtml = html
            };

            return dto;
        }

    }
}
