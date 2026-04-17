using System;
using System.Linq;

namespace NOCAPI.Modules.Solarwinds.Queries
{
    public static class ServiceTypeFiltering
    {
        public enum ServiceType
        {
            MPLS,
            ADIG,
            ADI,
            INTERNET,
            MNET_INTERNET,
            SDWAN_MPLS,
            SDWAN_ADI,
            P2P,
            Unknown
        }

        private static readonly string[] MplsPrefixes =
        {
            "AUCISL",
            "CACISL",
            "CHCISL",
            "CNCISL",
            "DECISL",
            "DKCISL",
            "ESCISL",
            "GBCISL",
            "HKCISL",
            "IECISL",
            "ITCISL",
            "NOCISL",
            "NZCISL",
            "PLCISL",
            "USCISL",
            "ZACIS"
        };

        private static readonly string[] MplsNamedNodes =
        {
            "Bristol Secondary AT&T MPLS",
            "Crosshills Primary AT&T",
            "Edinburgh Primary AT&T MPLS",
            "Edinburgh Secondary AT&T MPLS",
            "London Primary AT&T MPLS",
            "London Secondary AT&T MPLS",
            "LondonDerry Primary AT&T MPLS",
            "LondonDerry Secondary AT&T MPLS",
            "Plymouth Primary AT&T MPLS",
            "Plymouth Secondary AT&T MPLS",
            "Rome (ST) Primary AT&T MPLS",
            "Skipton Primary AT&T MPLS",
            "Skipton Secondary AT&T MPLS"
        };

        private static readonly string[] AdiNodes =
        {
            "CACOEYTORON01R",
            "CACOEYTORON02R",
            "NAILGCFW01",
            "NAQASPIN202SW02-INTERNET",
            "NASACFSW01",
            "NAWATAIN32SW01-INTERNET",
            "QASF-AT&T INTRTR",
            "QASP-AT&T 10G INTRTR"
        };

        private static readonly string[] AdigNodes =
        {
            "AUCOEYABB0001R",
            "AUCOEYPOR0002R",
            "Basiglio DC Primary AT&T ADIG Internet",
            "Basiglio DC Secondary AT&T ADIG Internet",
            "Bristol Secondary AT&T ADIG Internet",
            "LAUSANNE AT&T ADIG Internet Secondary",
            "Newport Primary AT&T ADIG Internet",
            "ROMN001GFCR001",
            "ROMN001GFCR002",
            "IN3221BLR0001R",
            "IN3221BLR0002R",
            "IN3221HYD0001R",
            "IN3221HYD0002R"
        };

        private static readonly string[] InternetNodes =
        {
            "HKEQX2R101IntSW01",
            "HKEQX5R101IntSW01"
        };

        private static readonly string[] MnetInternetNodes =
        {
            "ELSENHEIMER INTERNET"
        };

        private static readonly string[] P2pNodes =
        {
            "CHGVARF1P203CR01",
            "CHGVARF1P203CR02",
            "DEFRAKGF122CR01",
            "HKEQX5R101CR01",
            "HKEQX5R101CR02",
            "HKGTL12DCCR01",
            "HKGTL12DCCR02",
            "HKKOWE3601CORE",
            "NAQASFDC",
            "NAQASPDC",
            "NATORPDC"
        };

        private static readonly string[] SdwanAdiNodes =
        {
            "CACOIYRHICA01PVCE01_EHA",
            "CACOIYTORCA01PVCE01_EHA",
            "USCOIY"
        };

        private static readonly string[] SdwanMplsNodes =
        {
            "USCOIYBNKIL01PVCE01_EHA",
            "USCOIYHLGWV01PVCE01_EHA"
        };

        public static ServiceType Resolve(string nodeCaption)
        {
            if (string.IsNullOrWhiteSpace(nodeCaption))
                return ServiceType.Unknown;

            var name = ExtractBaseName(nodeCaption);

            if (SdwanMplsNodes.Any(name.EqualsIgnoreCase))
                return ServiceType.SDWAN_MPLS;

            if (SdwanAdiNodes.Any(name.StartsWithIgnoreCase))
                return ServiceType.SDWAN_MPLS;

            if (AdiNodes.Any(name.StartsWithIgnoreCase))
                return ServiceType.ADI;

            if (AdigNodes.Any(name.StartsWithIgnoreCase))
                return ServiceType.ADIG;

            if (InternetNodes.Any(name.StartsWithIgnoreCase))
                return ServiceType.INTERNET;

            if (MnetInternetNodes.Any(nodeCaption.ContainsIgnoreCase))
                return ServiceType.MNET_INTERNET;

            if (P2pNodes.Any(name.StartsWithIgnoreCase))
                return ServiceType.P2P;

            if (MplsPrefixes.Any(name.StartsWithIgnoreCase) ||
                MplsNamedNodes.Any(nodeCaption.ContainsIgnoreCase))
                return ServiceType.MPLS;

            return ServiceType.Unknown;
        }

        private static string ExtractBaseName(string caption)
        {
            var idx = caption.IndexOf(' ');
            return idx > 0 ? caption[..idx] : caption;
        }

        private static bool StartsWithIgnoreCase(this string value, string prefix) =>
            value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

        private static bool EqualsIgnoreCase(this string value, string other) =>
            value.Equals(other, StringComparison.OrdinalIgnoreCase);

        private static bool ContainsIgnoreCase(this string value, string other) =>
            value.Contains(other, StringComparison.OrdinalIgnoreCase);
    }
}