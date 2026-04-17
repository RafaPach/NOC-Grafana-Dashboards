using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NOCAPI.Modules.Solarwinds.Queries
{
    public static class SiteResolver
    {
        private static readonly Dictionary<string, string> ExplicitMap =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // US
                ["USCOIYBNKIL01PVCE01_EHA"] = "BOLINGBROOK",
                ["USCOIYCHIIL01PVCE01"] = "CHICAGO",
                ["USCOIYCLTNC01PVCE01_EHA"] = "CHARLOTTE",
                ["USCOIYDENCO01PVCE01_EHA"] = "DENVER",
                ["USCOIYEDNNJ01PVCE01_EHA"] = "EDISON",
                ["USCOIYFIXTX02PVCE01_EHA"] = "FRISCO",
                ["USCOIYHLGWV01PVCE01_EHA"] = "WHEELING",
                ["USCOIYHOUTX01PVCE01_EHA"] = "HOUSTON",
                ["USCOIYNYCNY01PVCE01_EHA"] = "NEW YORK",
                ["USCOIYPBNUS01PVCE01_EHA"] = "NORTH PALM BEACH",

                // Canada – Toronto
                ["CACOEYTORON01R"] = "TORONTO",
                ["CACOEYTORON02R"] = "TORONTO",
                ["CACOIYRHICA01PVCE01_EHA"] = "TORONTO",
                ["CACOIYTORCA01PVCE01_EHA"] = "TORONTO",
                ["NATORPDC01SW01R"] = "TORONTO",
                ["NATORPDC01SW02R"] = "TORONTO",
                ["NATORPDC01SW03"] = "TORONTO",
                ["NATORPDC01SW04"] = "TORONTO",
                ["NATORPDC01SW05"] = "TORONTO",

                // Hong Kong
                ["HKCISLWAN0001R"] = "HONG KONG",

                // Ashburn
                ["NAQASFDC101SW01"] = "ASHBURN",
                ["NAQASFDC102SW01"] = "ASHBURN",
                ["NAQASPDC101SW01"] = "ASHBURN",
                ["NAQASPIN202SW02-INTERNET"] = "ASHBURN",

                // Watertown
                ["NAWATAIN32SW01-INTERNET"] = "WATERTOWN",

                // New York legacy
                ["USCISLNEWNY05R"] = "NEW YORK",
                ["USCISLNEWNY06R"] = "NEW YORK",

                ["GBCISLNEW0006R (NEWPORT MPLS)"] = "NEWPORT"
            };

        private static readonly Regex MplsRegex =
        new(@"\(([A-Z ]+?)(?:\s[SBKLPH])?\sMP?LS(?:\s[PS])?\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Resolve(string nodeName)
        {
            var n = nodeName.ToUpperInvariant();

            if (ExplicitMap.TryGetValue(n, out var site))
                return site;

            if (n.StartsWith("HK"))
                return "HONG KONG";

            if (n.StartsWith("CACISLTORON"))
                return "TORONTO";

            if (n.StartsWith("NAQASFDC") || n.StartsWith("NAQASPDC"))
                return "ASHBURN";

            if (n.StartsWith("IN3221BLR"))
                return "BANGALORE";

            if (n.StartsWith("IN3221HYD"))
                return "HYDERABAD";

            if (n.Contains("LONDONDERRY"))
                return "LONDONDERRY";

            if (n.Contains("FORT WASHINGTON"))
                return "FORT WASHINGTON";

            if (n.Contains("MELD"))
                return "MELBOURNE";

            if (n.Contains("PERTH") || n.Contains("AUCOEYABB"))
                return "PERTH";
            if (n.Contains("AUCOEYPOR"))
                return "MELBOURNE";

            var match = MplsRegex.Match(n);
            if (match.Success)
                return match.Groups[1].Value.Trim();

            return "UNKNOWN";
        }



        public static readonly Dictionary<string, (double Lat, double Lon)> SiteGeo =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["ADELAIDE"] = (-34.9285, 138.6007),
                ["ASHBURN"] = (39.0438, -77.4874),
                ["AUCKLAND"] = (-36.8485, 174.7633),
                ["BANGALORE"] = (12.9716, 77.5946),
                ["BASIGLIO"] = (45.3533, 9.1580),
                ["BEIJING"] = (39.9042, 116.4074),
                ["BOLINGBROOK"] = (41.6986, -88.0684),
                ["BOLLINGBROOK"] = (41.6986, -88.0684), 
                ["BRISBANE"] = (-27.4698, 153.0251),
                ["BRISTOL"] = (51.4545, -2.5879),
                ["CALGARY"] = (51.0447, -114.0719),
                ["CANTON"] = (42.3086, -83.4822),
                ["CHARLOTTE"] = (35.2271, -80.8431),
                ["CHICAGO"] = (41.8781, -87.6298),
                ["COLUMBIA"] = (34.0007, -81.0348),
                ["COPENHAGEN"] = (55.6761, 12.5683),
                ["CROSSHILLS"] = (53.9050, -1.9908),
                ["DENVER"] = (39.7392, -104.9903),
                ["DUBLIN"] = (53.3498, -6.2603),
                ["EDINBURGH"] = (55.9533, -3.1883),
                ["EDISON"] = (40.5187, -74.4121),
                ["ELSENHEIMER"] = (48.1286, 11.5586),
                ["FORT WASHINGTON"] = (40.1376, -75.2091),
                ["FRANKFURT"] = (50.1109, 8.6821),
                ["FRISCO"] = (33.1507, -96.8236),
                ["GENEVA"] = (46.2044, 6.1432),
                ["HONG KONG"] = (22.3193, 114.1694),
                ["HOUGHTON"] = (47.1210, -88.5690),
                ["HOUSTON"] = (29.7604, -95.3698),
                ["HYDERABAD"] = (17.3850, 78.4867),
                ["JERSEY"] = (40.7178, -74.0431),
                ["JERSEY CITY"] = (40.7178, -74.0431),
                ["JOHANNESBURG"] = (-26.2041, 28.0473),
                ["LAUSANNE"] = (46.5197, 6.6323),
                ["LONDONDERRY"] = (55.0068, -7.3183),
                ["LOUISVILLE"] = (38.2527, -85.7585),
                ["MADRID"] = (40.4168, -3.7038),
                ["MAROOCHYDORE"] = (-26.6603, 153.0981),
                ["MELBORNE Y"] = (-37.8136, 144.9631), 
                ["MELBOURNE"] = (-37.8136, 144.9631),
                ["MILAN"] = (45.4642, 9.1900),
                ["MINNEAPOLIS"] = (44.9778, -93.2650),
                ["MONAGHAN"] = (54.2500, -6.9667),
                ["MONTREAL"] = (45.5017, -73.5673),
                ["NEW YORK"] = (40.7128, -74.0060),
                ["NEWPORT"] = (51.5842, -2.9977),
                ["NORTH PALM BEACH"] = (26.8176, -80.0814),
                ["OLTEN"] = (47.3499, 7.9033),
                ["OSLO"] = (59.9139, 10.7522),
                ["PERTH"] = (-31.9505, 115.8605),
                ["PLYMOUTH"] = (50.3755, -4.1427),
                ["ROME"] = (41.9028, 12.4964),
                ["SHELTON"] = (41.3165, -73.0932),
                ["SKIPTON"] = (53.9607, -2.0168),
                ["ST PAUL"] = (44.9537, -93.0900),
                ["SYDNEY"] = (-33.8688, 151.2093),
                ["TORONTO"] = (43.6532, -79.3832),
                ["TURIN"] = (45.0703, 7.6869),
                ["VANCOUVER"] = (49.2827, -123.1207),
                ["WARSAW"] = (52.2297, 21.0122),
                ["WATERTOWN"] = (43.9748, -75.9108),
                ["WHEELING"] = (40.0639, -80.7209),
                ["ZURICH"] = (47.3769, 8.5417),
            };

    }
}
