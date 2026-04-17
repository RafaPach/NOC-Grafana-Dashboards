using NOCAPI.Plugins.Config;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NOCAPI.Modules.Zdx.Mobile.InvestorCentre
{
    public class ICMobileAppHelper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private string MobileAppPropertyId = PluginConfigWrapper.GetSecure("ICmobileapp");

        public ICMobileAppHelper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateAuthClient(string token)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token?.Trim());
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<string> GetMobileAppActiveUsersAsync(string token)
        {
            var client = CreateAuthClient(token);

            var url =
                $"https://analyticsdata.googleapis.com/v1beta/properties/{MobileAppPropertyId}:runRealtimeReport";

            var body = new
            {
                metrics = new[]
                {
                    new { name = "activeUsers" }
                },
                dimensions = new[]
                {
                    new { name = "country" },
                    new { name = "platform" }
                },
                limit = 1000  
            };

            using var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception(json);

            return json;
        }

        public async Task<string> GetMobileHeartbeatEventsAsync(string token)
        {
            var client = CreateAuthClient(token);

            var url =
                $"https://analyticsdata.googleapis.com/v1beta/properties/{MobileAppPropertyId}:runRealtimeReport";

            var body = new
            {
                minuteRanges = new[]
                {
                    new
                    {
                        startMinutesAgo = 29,
                        endMinutesAgo = 0
                    }
                },
                            metrics = new[]
                            {
                    new { name = "eventCount" }
                },
                            dimensions = new[]
                            {
                    new { name = "eventName" }
                },
                dimensionFilter = new
                {
                    filter = new
                    {
                        fieldName = "eventName",
                        inListFilter = new
                        {
                            values = new[]
                            {
                    "session_start",
                    "screen_view",
                    "security_checks"
                }
                        }
                    }
                }
            };

            using var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(url, content);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"GA runReport failed {(int)response.StatusCode}: {json}");
            }

            return json;
        }
    }
}