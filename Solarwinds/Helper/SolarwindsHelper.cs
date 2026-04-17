using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NOCAPI.Plugins.Config;
using SolarWinds.Api;
using SolarWinds.Api.Queries;
using NOCAPI.Modules.Solarwinds.Queries;
using NOCAPI.Modules.Solarwinds.Services;

namespace NOCAPI.Modules.Solarwinds.Helpers
{
    public class SolarwindsHelper
    {
        private readonly ILogger<SolarwindsHelper> _logger;

        public SolarwindsHelper(ILogger<SolarwindsHelper> logger)
        {
            _logger = logger;
        }

        //public Dictionary<string, Func<SolarWindsClient>> GetRegionClients()
        //{
        //    return new Dictionary<string, Func<SolarWindsClient>>(StringComparer.OrdinalIgnoreCase)
        //    {
        //        ["NA"] = () =>
        //        {
        //            var hostname = (PluginConfigWrapper.Get("SolarwindsHostname_NA") ?? "Qaspvpanpm01:17774").Trim();
        //            var username = (PluginConfigWrapper.Get("SolarwindsUsername_NA") ?? "emea\\EM_SVC_PRD_CTSOPS").Trim();
        //            var password = (PluginConfigWrapper.GetSecure("SolarwindsPassword_NA") ?? "").Trim();
        //            if (string.IsNullOrWhiteSpace(password))
        //                throw new NullReferenceException("SolarWinds password NA is null/empty");
        //            // NOTE: pick the correct port for NA; earlier you used 17777 in Oliver’s test and 17774 from hostname.
        //            return new SolarWindsClient(hostname, username, password, 17774, true);
        //        },
        //        ["EMEA"] = () =>
        //        {
        //            var hostname = (PluginConfigWrapper.Get("SolarwindsHostname") ?? "orion.emea.cshare.net").Trim();
        //            var username = (PluginConfigWrapper.Get("SolarwindsUsername") ?? "emea_guest").Trim();
        //            var password = (PluginConfigWrapper.GetSecure("SolarwindsPassword") ?? "").Trim();
        //            if (string.IsNullOrWhiteSpace(password))
        //                throw new NullReferenceException("SolarWinds password EMEA is null/empty");
        //            return new SolarWindsClient(hostname, username, password, 17778, true);
        //        },
        //        ["AU"] = () =>
        //        {
        //            var hostname = (PluginConfigWrapper.Get("SolarwindsHostname_AU") ?? "orion.oceania.cshare.net").Trim();
        //            var username = (PluginConfigWrapper.Get("SolarwindsUsername_AU") ?? "emea-comms").Trim();
        //            var password = (PluginConfigWrapper.GetSecure("SolarwindsPassword_AU") ?? "").Trim();
        //            if (string.IsNullOrWhiteSpace(password))
        //                throw new NullReferenceException("SolarWinds password AU is null/empty");
        //            return new SolarWindsClient(hostname, username, password, 17778, true);
        //        },
        //    };
        //}


        public SolarWindsClient CreateClientNA(int defaultPort = 17774, bool ignoreTlsErrors = true)
        {
            var rawHost = PluginConfigWrapper.GetSecure("SolarwindsHostname_NA");
            var username = PluginConfigWrapper.GetSecure("SolarwindsUsername_NA");
            var password = PluginConfigWrapper.GetSecure("SolarwindsPassword_NA");

            if (string.IsNullOrWhiteSpace(password))
                throw new NullReferenceException("SolarWinds NA password is null or empty.");

            string host = rawHost;
            int port = defaultPort;

            var colonIdx = rawHost.IndexOf(':');
            if (colonIdx >= 0 && colonIdx < rawHost.Length - 1)
            {
                host = rawHost[..colonIdx];
                var portText = rawHost[(colonIdx + 1)..];

                if (int.TryParse(portText, out var parsedPort) && parsedPort > 0)
                {
                    port = parsedPort;
                }
            }
            _logger.LogInformation("SolarWindsClient init host='{Host}', user='{User}', port={Port}", host, username, port);

            return new SolarWindsClient(host, username, password, port, ignoreTlsErrors);
        }


        public async Task<List<JObject>> QueryNodesAcrossRegionsAsync(CancellationToken ct)
        {
            //var results = new List<JObject>();
            //var regions = GetRegionClients();

            //foreach (var kvp in regions)
            //{
            //    var region = kvp.Key;
            //    try
            //    {
            //        var client = kvp.Value();
            //        var query = new SqlQuery { Sql = ServerSwql };
            //        var resp = await client.SqlJObjectQueryAsync(query);

            //        foreach (var r in resp.Results)
            //        {
            //            r["Region"] = region;
            //            results.Add(r);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, "SolarWinds query error for region {Region}", region);
            //    }
            //}

            //return results;


            var results = new List<JObject>();

            try
            {
                var client = CreateClientNA();
                var query = new SqlQuery { Sql = SolarWindsQueries.ServerStatus };
                var resp = await client.SqlJObjectQueryAsync(query);

                foreach (var r in resp.Results)
                {
                    r["Region"] = "NA";
                    results.Add(r);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SolarWinds query error (NA).");
            }

            return results;

        }


        public async Task<List<JObject>> QueryMemoryAcrossRegionsAsync(CancellationToken ct)
        {

            var results = new List<JObject>();

            try
            {
                var client = CreateClientNA();
                var query = new SqlQuery { Sql = SolarWindsQueries.ServerCpuMemory };
                var resp = await client.SqlJObjectQueryAsync(query);

                foreach (var r in resp.Results)
                {
                    r["Region"] = "NA";

                    results.Add(r);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SolarWinds query error (NA).");
            }

            return results;

        }

        public async Task<List<JObject>> ServerType(CancellationToken ct)
        {

            var results = new List<JObject>();

            try
            {
                var client = CreateClientNA();
                var query = new SqlQuery { Sql = SolarWindsQueries.ServerType };
                var resp = await client.SqlJObjectQueryAsync(query);

                foreach (var r in resp.Results)
                {
                    r["Region"] = "NA";

                    results.Add(r);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SolarWinds query error (NA).");
            }

            return results;

        }

        public async Task<List<JObject>> QueryNetworkInterfacesAsync(CancellationToken ct)
        {
            var results = new List<JObject>();

            try
            {
                var client = CreateClientNA();
                var query = new SqlQuery { Sql = SolarWindsQueries.Filtered_Network_Interfaces };

                var resp = await client.SqlJObjectQueryAsync(query);

                foreach (var r in resp.Results)
                {


                    var nodeName = Safe(r["NodeName"]);
                    var region = ResolveRegion(nodeName);
                    var serviceType = ServiceTypeFiltering.Resolve(nodeName);
                    var site = SiteResolver.Resolve(nodeName);


                    r["Region"] = region;
                    r["DataType"] = "NetworkInterface";
                    r["ServiceType"] = serviceType.ToString();
                    r["Site"] = site;

                    results.Add(r);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SolarWinds network interface query error (NA).");
            }

            return results;
        }

        private static string ResolveRegion(string nodeName)
        {
            if (string.IsNullOrWhiteSpace(nodeName))
                return "Unknown";

            var n = nodeName.ToUpperInvariant();

            if (
                   n.StartsWith("GBCISL")
                || n.StartsWith("DECISL")
                || n.StartsWith("ESCISL")
                || n.StartsWith("ITCISL")
                || n.StartsWith("CHCISL")
                || n.StartsWith("DKCISL")
                || n.StartsWith("PLCISL")
                || n.StartsWith("IECISL")
                || n.StartsWith("NOCISL")
                || n.Contains("BRISTOL")
                || n.Contains("CROSSHILLS")
                || n.Contains("EDINBURGH")
                || n.Contains("HOUGHTON")
                || n.Contains("JERSEY")
                || n.Contains("LONDON")
                || n.Contains("LONDONDERRY")
                || n.Contains("MONAGHAN")
                || n.Contains("NEWPORT")
                || n.Contains("PLYMOUTH")
                || n.Contains("SKIPTON")
                || n.Contains("BASIGLIO")
                || n.Contains("MILAN")
                || n.Contains("ROME")
                || n.Contains("TURIN")
                || n.Contains("FRANKFURT")
                || n.Contains("ELSENHEIMER")
                || n.Contains("GENEVA")
                || n.Contains("LAUSANNE")
                || n.Contains("OLTEN")
                || n.Contains("ZURICH")
                || n.Contains("COPENHAGEN")
                || n.Contains("OSLO")
                || n.Contains("MADRID")
                || n.Contains("WARSAW")
                || n.Contains("DUBLIN")
            )
                return "EMEA";

            if (
                   n.StartsWith("USCISL")
                || n.StartsWith("USCOIY")
                || n.StartsWith("NAQASF")
                || n.StartsWith("NAQASP")
                || n.StartsWith("NATORP")
                || n.Contains("ASHBURN")
                || n.Contains("COLUMBIA")
                || n.Contains("BOLLINGBROOK")
                || n.Contains("CHICAGO")
                || n.Contains("CONNECTICUT")
                || n.Contains("SHELTON")
                || n.Contains("NEW JERSEY")
                || n.Contains("WATERTOWN")
                || n.Contains("EDISON")
                || n.Contains("CANTON")
                || n.Contains("MINNEAPOLIS")
                || n.Contains("ST PAUL")
                || n.Contains("LOUISVILLE")
                || n.Contains("WILMINGTON")
            )
                return "US";

            if (
                   n.StartsWith("CACISL")
                || n.StartsWith("CACOEY")
                || n.StartsWith("CACOIY")
                || n.Contains("TORONTO")
                || n.Contains("MONTREAL")
                || n.Contains("VANCOUVER")
                || n.Contains("CALGARY")
                || n.Contains("ONTARIO")
                || n.Contains("RICHMOND HILL")
                || n.Contains("UNIVERSITY AVENUE")
            )
                return "Canada";

            if (
                   n.StartsWith("AUCISL")
                || n.StartsWith("AUCOIY")
                || n.StartsWith("AUCOEY")
                || n.StartsWith("NZCISL")
                || n.Contains("MELBOURNE")
                || n.Contains("SYDNEY")
                || n.Contains("BRISBANE")
                || n.Contains("ADELAIDE")
                || n.Contains("PERTH")
                || n.Contains("MAROOCHYDORE")
                || n.StartsWith("AUMELD")
                || n.StartsWith("AUBNE")
                || n.StartsWith("IN3221")
                || n.StartsWith("HK")
                || n.StartsWith("CNCISL")
            )
                return "Oceania";

            if (
                   n.StartsWith("ZACISL")
                || n.StartsWith("ZAJN")
                || n.Contains("JOHANNESBURG")
            )
                return "Africa";

            return "Unknown";
        }


        private static Dictionary<string,string> siteMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // US
            ["USCOIYBNKIL01PVCE01_EHA"] = "Bolingbrook",
            ["USCOIYCHIIL01PVCE01"] = "Chicago IL",
            ["USCOIYCLTNC01PVCE01_EHA"] = "Charlotte NC",
            ["USCOIYDENCO01PVCE01_EHA"] = "Denver CO",
            ["USCOIYEDNNJ01PVCE01_EHA"] = "Edison",
            ["USCOIYFIXTX02PVCE01_EHA"] = "Frisco TX",
            ["USCOIYHLGWV01PVCE01_EHA"] = "Wheeling WV",
            ["USCOIYHOUTX01PVCE01_EHA"] = "Houston TX",
            ["USCOIYNYCNY01PVCE01_EHA"] = "New York NY",
            ["USCOIYPBNUS01PVCE01_EHA"] = "North Palm Beach FL",

            // Canada – Toronto variants
            ["CACOEYTORON01R"] = "Toronto",
            ["CACOEYTORON02R"] = "Toronto",
            ["CACOIYRHICA01PVCE01_EHA"] = "Toronto",
            ["CACOIYTORCA01PVCE01_EHA"] = "Toronto",
            ["NATORPDC01SW01R"] = "Toronto",
            ["NATORPDC01SW02R"] = "Toronto",

            // Hong Kong
            ["HKCISLWAN0001R"] = "Hong Kong",

            // Ashburn
            ["NAQASFDC101SW01"] = "Ashburn",
            ["NAQASFDC102SW01"] = "Ashburn",
            ["NAQASPDC101SW01"] = "Ashburn",
            ["NAQASPIN202SW02-INTERNET"] = "Ashburn",

            // Watertown
            ["NAWATAIN32SW01-INTERNET"] = "Watertown",

            // New York legacy
            ["USCISLNEWNY05R"] = "New York",
            ["USCISLNEWNY06R"] = "New York",
        };

        public static string Safe(JToken? token)
            => token?.ToString() ?? string.Empty;

    }
}
