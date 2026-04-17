namespace NOCAPI.Modules.Solarwinds.Queries
{
    public static class SolarWindsQueries
    {
  

        public const string ServerStatus = @"
            SELECT
                n.Caption AS NodeName,
                n.Status,
                n.StatusDescription,
                cp.BusApplication
            FROM Orion.Nodes AS n
            JOIN Orion.NodesCustomProperties AS cp
                ON cp.NodeID = n.NodeID
            WHERE 
                   cp.BusApplication LIKE '%Ping%'
                OR cp.BusApplication LIKE '%Investorv%'
                OR cp.BusApplication LIKE '%Investor C%'
                OR cp.BusApplication LIKE '%Global Entity%'
                OR cp.BusApplication = 'FTP'
                OR cp.BusApplication LIKE '%Issuer Online%'
                OR cp.BusApplication LIKE '%Equate Plus%'
                OR cp.BusApplication LIKE '%Sphere%'
                OR cp.BusApplication LIKE '%Summit%'
                OR cp.BusApplication LIKE '%Global Viewpoint%'
            ORDER BY n.Caption";

        public const string ServerCpuMemory = @"
            SELECT
                n.Caption AS NodeName,
                cp.BusApplication,
                n.CPULoad AS CPU,
                n.PercentMemoryUsed AS MemoryUsed
            FROM Orion.Nodes n
            JOIN Orion.NodesCustomProperties cp
                ON cp.NodeID = n.NodeID
            WHERE 
                   cp.BusApplication LIKE '%Ping%'
                OR cp.BusApplication LIKE '%Investorv%'
                OR cp.BusApplication LIKE '%Investor C%'
                OR cp.BusApplication LIKE '%Global Entity%'
                OR cp.BusApplication = 'FTP'
                OR cp.BusApplication LIKE '%Issuer Online%'
                OR cp.BusApplication LIKE '%Equate Plus%'
                OR cp.BusApplication LIKE '%Sphere%'
                OR cp.BusApplication LIKE '%Summit%'
                OR cp.BusApplication LIKE '%Global Viewpoint%'
            ORDER BY cp.BusApplication, n.Caption";

        public const string ServerType = @"
            SELECT
                n.Caption AS NodeName,
                cp.BusApplication,
                CASE
                    WHEN (
                        SELECT TOP 1 v.VolumeID
                        FROM Orion.Volumes v
                        WHERE v.NodeID = n.NodeID
                          AND v.Caption LIKE '%SQL%'
                          AND v.Caption NOT LIKE 'Physical Memory%'
                          AND v.Caption NOT LIKE 'Virtual Memory%'
                    ) IS NOT NULL THEN 'SQL'
                    WHEN n.MachineType LIKE '%Windows%' THEN 'Windows'
                    WHEN n.MachineType LIKE '%Red Hat%'
                      OR n.MachineType LIKE '%Linux%' THEN 'Linux'
                    WHEN n.MachineType LIKE '%VMware%' THEN 'VMware ESXi'
                    WHEN n.MachineType IS NULL OR n.MachineType = '' THEN 'Unknown'
                    ELSE n.MachineType
                END AS ServerType
            FROM Orion.Nodes n
            JOIN Orion.NodesCustomProperties cp
                ON cp.NodeID = n.NodeID
            WHERE
                   cp.BusApplication LIKE '%Ping%'
                OR cp.BusApplication LIKE '%Investorv%'
                OR cp.BusApplication LIKE '%Investor C%'
                OR cp.BusApplication LIKE '%Global Entity%'
                OR cp.BusApplication = 'FTP'
                OR cp.BusApplication LIKE '%Issuer Online%'
                OR cp.BusApplication LIKE '%Equate Plus%'
                OR cp.BusApplication LIKE '%Sphere%'
                OR cp.BusApplication LIKE '%Summit%'
                OR cp.BusApplication LIKE '%Global Viewpoint%'
            ORDER BY n.Caption";

        // ============================
        // Network / Interface Queries
        // ============================

        public const string NetworkInterfaces = @"
               SELECT
                    n.NodeID,
                    n.Caption AS NodeName,

                    CASE
                        WHEN n.Caption LIKE '%QASF%'
                          OR n.Caption LIKE '%QASP%'
                          OR n.Caption LIKE '%WATA%'
                          OR n.Caption LIKE '%TORP%'
                          OR n.Caption LIKE '%TORL%'
                        THEN 1
                        ELSE 0
                    END AS IsDataCentre,

                    i.InterfaceID,
                    i.InterfaceName,
                    i.InterfaceAlias,
                    i.OperStatus,
                    i.AdminStatus,
                    i.Inbps  / 1000000 AS InMbps,
                    i.Outbps / 1000000 AS OutMbps,
                    (i.Inbps + i.Outbps) / 1000000 AS TotalMbps,
                    i.Speed / 1000000 AS InterfaceSpeedMbps,
                    ROUND((i.Inbps  / i.Speed) * 100, 2) AS InUtilPercent_RealTime,
                    ROUND((i.Outbps / i.Speed) * 100, 2) AS OutUtilPercent_RealTime,
                    i.InPercentUtil  AS InUtilPercent_SW,
                    i.OutPercentUtil AS OutUtilPercent_SW
                FROM Orion.NPM.Interfaces i
                JOIN Orion.Nodes n ON n.NodeID = i.NodeID
                WHERE
                    n.NodeID IN (
                        SELECT NodeID
                        FROM Orion.Nodes
                        WHERE
               Caption LIKE '%Bristol%'
            OR Caption LIKE '%Beijing%'
            OR Caption LIKE '%Ashburn%'
            OR Caption LIKE '%Canton%'
            OR Caption LIKE '%Chicago%'
            OR Caption LIKE '%Connecticut%'
            OR Caption LIKE '%New Jersey%'
            OR Caption LIKE '%Watertown%'
            OR Caption LIKE '%Toronto%'
            OR Caption LIKE '%Montreal%'
            OR Caption LIKE '%Calgary%'
            OR Caption LIKE '%Ontario%'
            OR Caption LIKE '%Melbourne%'
            OR Caption LIKE '%ADELAIDE%'
            OR Caption LIKE '%SYDNEY%'
            OR Caption LIKE '%MELD%'
            OR Caption LIKE '%Yarra%'
            OR Caption LIKE '%OLTEN%'
            OR Caption LIKE '%COPENHAGEN%'
            OR Caption LIKE '%BASIGLIO%'
            OR Caption LIKE '%TURIN%'
            OR Caption LIKE '%ROME%'
            OR Caption LIKE '%ELSENHEIMER%'
            OR Caption LIKE '%LAUSANNE%'
            OR Caption LIKE '%BRISBANE%'
            OR Caption LIKE '%PERTH%'
            OR Caption LIKE '%MAROOCHYDORE%'
            OR Caption LIKE '%MELYDC%'
            OR Caption LIKE '%Newport%'
            OR Caption LIKE '%Frankfurt%'
            OR Caption LIKE '%Edinburgh%'
            OR Caption LIKE '%London%'
            OR Caption LIKE '%Jersey%'
            OR Caption LIKE '%Skipton%'
            OR Caption LIKE '%LondonDerry%'
            OR Caption LIKE '%Dublin%'
            OR Caption LIKE '%COLUMBIA%'
            OR Caption LIKE '%MINNEAPOLIS%'
            OR Caption LIKE '%ST PAUL%'
            OR Caption LIKE '%LOUISVILLE%'
            OR Caption LIKE '%BOLLINGBROOK%'
            OR Caption LIKE '%EDISON%'
            OR Caption LIKE '%CANTON%'
            OR Caption LIKE '%Oslo%'    
            OR Caption LIKE '%Madrid%'
            OR Caption LIKE '%Milan%'
            OR Caption LIKE '%WHEELING%'
            OR Caption LIKE '%Warsaw%'
            OR Caption LIKE '%WILMINGTON%'
            OR Caption LIKE '%Zurich%'
            OR Caption LIKE '%WATA%'
            OR Caption LIKE '%QASP%'
            OR Caption LIKE '%QASF%'
    )

                    -- WAN-facing only
                AND (
                       i.InterfaceAlias LIKE '%link to PE%'
                    OR n.Caption LIKE '%AT&T INTRTR%'
                    OR n.Caption LIKE '%AT&T INTERNET ROUTER%'
                )

                    -- Office-safe exclusions
                AND i.InterfaceAlias NOT LIKE '%Customer LAN%'
                AND i.InterfaceAlias NOT LIKE '%att-unman%'
                AND i.InterfaceAlias NOT LIKE '%LAN INT%'
                AND i.InterfaceAlias NOT LIKE '%LAN interface%'
                AND i.InterfaceAlias NOT LIKE '%Loopback%'
                AND i.InterfaceAlias NOT LIKE '%DCI%'
                AND i.InterfaceAlias NOT LIKE '%OSPF%'
                AND i.InterfaceAlias NOT LIKE '%Orphan%'
                AND i.InterfaceName  NOT LIKE '%port-channel%'
                AND i.OperStatus = 1

                    -- Tunnel / GRE logic:
                    -- ❌ Offices: excluded
                    -- ✅ Data Centres (QASF / QASP / WATA): allowed
                AND (
                        (
                            n.Caption NOT LIKE '%QASF%'
                        AND n.Caption NOT LIKE '%QASP%'
                        AND n.Caption NOT LIKE '%WATA%'
                        AND i.InterfaceAlias NOT LIKE '%Tunnel%'
                        AND i.InterfaceAlias NOT LIKE '%GRE%'
                        AND i.InterfaceName  NOT LIKE '%.%'
                        )
                     OR (
                            n.Caption LIKE '%QASF%'
                         OR n.Caption LIKE '%QASP%'
                         OR n.Caption LIKE '%WATA%'
                        )
                )

                ORDER BY n.Caption, TotalMbps DESC;
            ";

        public const string NetworkInterfaceAfterSpreadSheet = @"SELECT
    n.NodeID,
    n.Caption AS NodeName,
    i.InterfaceID,
    i.InterfaceName,
    i.InterfaceAlias,
    i.OperStatus,
    i.AdminStatus,
    i.Inbps / 1000000 AS InMbps,
    i.Outbps / 1000000 AS OutMbps,
    (i.Inbps + i.Outbps) / 1000000 AS TotalMbps,
    i.Speed / 1000000 AS InterfaceSpeedMbps,
    i.InPercentUtil  AS InUtilPercent,
    i.OutPercentUtil AS OutUtilPercent
FROM Orion.NPM.Interfaces i
JOIN Orion.Nodes n ON n.NodeID = i.NodeID
WHERE
(
       n.Caption LIKE '%LBRI%'
    OR n.Caption LIKE '%LEDI%'
    OR n.Caption LIKE '%LDUB%'
    OR n.Caption LIKE '%LFRA%'
    OR n.Caption LIKE '%LGEN%'
    OR n.Caption LIKE '%LSTH%'
    OR n.Caption LIKE '%LLAU%'
    OR n.Caption LIKE '%LMAD%'
    OR n.Caption LIKE '%LMIL%'
    OR n.Caption LIKE '%LBAS%'
    OR n.Caption LIKE '%LMON%'
    OR n.Caption LIKE '%LNEW%'
    OR n.Caption LIKE '%LOLT%'
    OR n.Caption LIKE '%LFOR%'
    OR n.Caption LIKE '%LHOU%'
    OR n.Caption LIKE '%LZUR%'
    OR n.Caption LIKE '%LTUR%'
    OR n.Caption LIKE '%LWAR%'
    OR n.Caption LIKE 'USCISL%'
    OR n.Caption LIKE 'CACISL%'
    OR n.Caption LIKE 'CACOIY%'
    OR n.Caption LIKE 'NAWATA%'
    OR n.Caption LIKE 'NAQASF%'
    OR n.Caption LIKE 'NAQASP%'
    OR n.Caption LIKE 'NATORP%'
    OR n.Caption LIKE 'NAILG%'
    OR n.Caption LIKE 'nasacfsw%'
    OR n.Caption LIKE 'AUCISL%'
    OR n.Caption LIKE 'AUCOIY%'
    OR n.Caption LIKE 'AUCOEY%'
    OR n.Caption LIKE 'NZCISL%'
    OR n.Caption LIKE 'IN3221%'
    OR n.Caption LIKE 'CNCISL%'
    OR n.Caption LIKE 'HKGTL%'
    OR n.Caption LIKE 'HKEQX%'
    OR n.Caption LIKE 'aumeld%'
    OR n.Caption IN (
        'USCOIYBNKIL01PVCE01_EHA',
        'USCOIYCLTNC01PVCE01_EHA',
        'USCOIYCHIIL01PVCE01',
        'USCOIYDENCO01PVCE01_EHA',
        'USCOIYEDNNJ01PVCE01_EHA',
        'USCOIYFIXTX02PVCE01_EHA',
        'USCOIYHOUTX01PVCE01_EHA',
        'USCOIYNYCNY01PVCE01_EHA',
        'USCOIYPBNUS01PVCE01_EHA',
        'USCOIYWTWMA01PVCE01_SHA',
        'USCOIYHLGWV01PVCE01_EHA'
    )
)
AND
(
    n.Caption IN (
        'USCOIYBNKIL01PVCE01_EHA',
        'USCOIYCLTNC01PVCE01_EHA',
        'USCOIYCHIIL01PVCE01',
        'USCOIYDENCO01PVCE01_EHA',
        'USCOIYEDNNJ01PVCE01_EHA',
        'USCOIYFIXTX02PVCE01_EHA',
        'USCOIYHOUTX01PVCE01_EHA',
        'USCOIYNYCNY01PVCE01_EHA',
        'USCOIYPBNUS01PVCE01_EHA',
        'USCOIYWTWMA01PVCE01_SHA',
        'USCOIYHLGWV01PVCE01_EHA'
    )
    OR (
        n.Caption LIKE 'NAWATA%'
        AND i.InterfaceName LIKE 'Ethernet1/49%'
    )
    OR (
        n.Caption LIKE 'NAQASF%'
        AND (
               i.InterfaceAlias LIKE '%DCI%'
            OR i.InterfaceAlias LIKE '%OSPF%'
            OR i.InterfaceAlias LIKE '%Orphan%'
        )
    )
    OR (
        n.Caption LIKE 'NAQASP%'
        AND (
               i.InterfaceAlias LIKE '%Internet%'
            OR i.InterfaceAlias LIKE '%OSPF%'
            OR i.InterfaceAlias LIKE '%DCI%'
        )
    )
    OR (
        n.Caption LIKE 'NATORP%'
        AND i.InterfaceName LIKE 'Ethernet1/47%'
    )
    OR (
        n.Caption LIKE 'HKGTL%'
        AND (
               i.InterfaceName LIKE 'Ethernet1/47%'
            OR i.InterfaceName LIKE 'Ethernet1/44%'
            OR i.InterfaceName LIKE 'Ethernet1/43%'
            OR i.InterfaceAlias LIKE '%AT&T%'
            OR i.InterfaceAlias LIKE '%wan%'
        )
    )
    OR (
        n.Caption LIKE 'HKEQX%'
        AND i.InterfaceAlias LIKE '%DCI%'
    )
    OR (
        n.Caption LIKE 'aumeld%'
        AND i.InterfaceAlias LIKE '%DWDM%'
    )
    OR (
        n.Caption NOT LIKE 'USCOIY%'
        AND (
               i.InterfaceAlias LIKE '%MPLS%'
            OR i.InterfaceAlias LIKE '%ATT%'
            OR i.InterfaceAlias LIKE '%AT&T%'
            OR i.InterfaceAlias LIKE '%INTERNET%'
            OR i.InterfaceAlias LIKE '%WAN%'
            OR i.InterfaceName  LIKE 'GE%'
            OR i.InterfaceName  LIKE 'GigabitEthernet%'
        )
    )
)
AND i.InterfaceAlias NOT LIKE '%LAN%'
AND i.InterfaceAlias NOT LIKE '%Customer%'
AND i.InterfaceAlias NOT LIKE '%Loopback%'
AND i.InterfaceAlias NOT LIKE '%GRE%'
AND i.InterfaceName  NOT LIKE '%Port-channel%'
AND i.InterfaceName  NOT LIKE '%.%'
AND i.OperStatus = 1
ORDER BY
    n.Caption,
    TotalMbps DESC;
";

        private const string MEANETWORKQUERYONly = @"
SELECT
    n.NodeID,
    n.Caption AS NodeName,
    i.InterfaceID,
    i.InterfaceName,
    i.InterfaceAlias,
    i.OperStatus,
    i.AdminStatus,
    i.Inbps / 1000000 AS InMbps,
    i.Outbps / 1000000 AS OutMbps,
    (i.Inbps + i.Outbps) / 1000000 AS TotalMbps,
    i.Speed / 1000000 AS InterfaceSpeedMbps,
    i.InPercentUtil  AS InUtilPercent,
    i.OutPercentUtil AS OutUtilPercent
FROM Orion.NPM.Interfaces i
JOIN Orion.Nodes n ON n.NodeID = i.NodeID
WHERE
(
       n.Caption LIKE '%LBRI%'
    OR n.Caption LIKE '%BRSP%'
    OR n.Caption LIKE '%LEDI%'
    OR n.Caption LIKE '%LDUB%'
    OR n.Caption LIKE '%LFRA%'
    OR n.Caption LIKE '%LGEN%'
    OR n.Caption LIKE '%LSTH%'
    OR n.Caption LIKE '%LLAU%'
    OR n.Caption LIKE '%LMAD%'
    OR n.Caption LIKE '%LMIL%'
    OR n.Caption LIKE '%LBAS%'
    OR n.Caption LIKE '%LMON%'
    OR n.Caption LIKE '%LNEW%'
    OR n.Caption LIKE '%LOLT%'
    OR n.Caption LIKE '%LFOR%'
    OR n.Caption LIKE '%LHOU%'
    OR n.Caption LIKE '%LZUR%'
    OR n.Caption LIKE '%LTUR%'
    OR n.Caption LIKE '%LWAR%'
    OR n.Caption LIKE '%LMUN%'

    OR n.Caption LIKE '%Barcelona%'
    OR n.Caption LIKE '%BCN%'
    OR n.Caption LIKE '%Bristol%'
    OR n.Caption LIKE '%Crosshills%'
    OR n.Caption LIKE '%Dublin%'
    OR n.Caption LIKE '%Edinburgh%'
    OR n.Caption LIKE '%Frankfurt%'
    OR n.Caption LIKE '%Geneva%'
    OR n.Caption LIKE '%Jersey%'
    OR n.Caption LIKE '%Lausanne%'
    OR n.Caption LIKE '%London%'
    OR n.Caption LIKE '%Londonderry%'
    OR n.Caption LIKE '%Madrid%'
    OR n.Caption LIKE '%Milan%'
    OR n.Caption LIKE '%Basiglio%'
    OR n.Caption LIKE '%Monaghan%'
    OR n.Caption LIKE '%Newport%'
    OR n.Caption LIKE '%Olten%'
    OR n.Caption LIKE '%Oslo%'
    OR n.Caption LIKE '%Plymouth%'
    OR n.Caption LIKE '%Rome%'
    OR n.Caption LIKE '%Skipton%'
    OR n.Caption LIKE '%Turin%'
    OR n.Caption LIKE '%Warsaw%'
    OR n.Caption LIKE '%Zurich%'
    OR n.Caption LIKE '%Elsenheimer%'

    OR n.Caption IN (
        'UKNEPCGFP04SW01',
        'UKNEPCGFP30SW02',
        'UKNEPCGFP06SW62',
        'UKNEPCGFP06SW56',
        'UKNEPCGFP32SW57'
    )

    OR n.Caption IN (
        'ROMN001GFCR001',
        'ROMN001GFCR002',
        'DEFRAKGF122CR01',
        'UKCRODGFSW01',
        'UKCRODGFSW02'
    )
 OR n.Caption IN (
        'CHGVARF1P203CR01',
        'CHGVARF1P203CR02'
    )

    OR n.Caption IN (
        'ZAJNBBGF03CR01',
        'ZAJNBBGF03CR02',
        'ZAJNBBGF03SW02',
        'JNBM6CR-IS-Primary-INET',
        'ZAJNBKGF01FW01.cshare.net'
    )
)
AND
(
       i.InterfaceAlias LIKE '%MPLS%'
    OR i.InterfaceAlias LIKE '%ATT%'
    OR i.InterfaceAlias LIKE '%AT&T%'
    OR i.InterfaceAlias LIKE '%INTERNET%'
    OR i.InterfaceAlias LIKE '%WAN%'
    OR i.InterfaceName  LIKE 'GE%'
    OR i.InterfaceName  LIKE 'GigabitEthernet%'
)
AND i.InterfaceAlias NOT LIKE '%LAN%'
AND i.InterfaceAlias NOT LIKE '%Customer%'
AND i.InterfaceAlias NOT LIKE '%Loopback%'
AND i.InterfaceAlias NOT LIKE '%GRE%'
AND i.InterfaceName  NOT LIKE '%Port-channel%'
AND i.InterfaceName  NOT LIKE '%.%'
AND i.OperStatus = 1
ORDER BY n.Caption;
        ";

        public const string Filtered_Network_Interfaces = @"SELECT
    n.NodeID,
    n.Caption AS NodeName,

    i.InterfaceID,
    i.InterfaceName,
    i.InterfaceAlias,
    i.OperStatus,
    i.AdminStatus,
    i.Inbps / 1000000 AS InMbps,
    i.Outbps / 1000000 AS OutMbps,
    (i.Inbps + i.Outbps) / 1000000 AS TotalMbps,
    i.Speed / 1000000 AS InterfaceSpeedMbps,
    i.InPercentUtil  AS InUtilPercent,
    i.OutPercentUtil AS OutUtilPercent,

    n.CPULoad            AS CpuLoadPercent,
    n.PercentMemoryUsed  AS MemoryUsedPercent,
    n.PercentLoss        AS PacketLossPercent,
    n.ResponseTime       AS ResponseTimeMs,
    n.Status             AS NodeStatus

FROM Orion.NPM.Interfaces i
JOIN Orion.Nodes n ON n.NodeID = i.NodeID
WHERE
(
       n.Caption LIKE '%LBRI%'
    OR n.Caption LIKE '%BRSP%'
    OR n.Caption LIKE '%LEDI%'
    OR n.Caption LIKE '%LDUB%'
    OR n.Caption LIKE '%LFRA%'
    OR n.Caption LIKE '%LGEN%'
    OR n.Caption LIKE '%LSTH%'
    OR n.Caption LIKE '%LLAU%'
    OR n.Caption LIKE '%LMAD%'
    OR n.Caption LIKE '%LMIL%'
    OR n.Caption LIKE '%LBAS%'
    OR n.Caption LIKE '%LMON%'
    OR n.Caption LIKE '%LNEW%'
    OR n.Caption LIKE '%LOLT%'
    OR n.Caption LIKE '%LFOR%'
    OR n.Caption LIKE '%LHOU%'
    OR n.Caption LIKE '%LZUR%'
    OR n.Caption LIKE '%LTUR%'
    OR n.Caption LIKE '%LWAR%'
    OR n.Caption LIKE '%LMUN%'
    OR n.Caption LIKE '%Barcelona%'
    OR n.Caption LIKE '%Bristol%'
    OR n.Caption LIKE '%Crosshills%'
    OR n.Caption LIKE '%Dublin%'
    OR n.Caption LIKE '%Edinburgh%'
    OR n.Caption LIKE '%Frankfurt%'
    OR n.Caption LIKE '%Geneva%'
    OR n.Caption LIKE '%Jersey%'
    OR n.Caption LIKE '%Lausanne%'
    OR n.Caption LIKE '%JOHANNESBURG%'
    OR n.Caption LIKE '%London%'
    OR n.Caption LIKE '%Madrid%'
    OR n.Caption LIKE '%Milan%'
    OR n.Caption LIKE '%Monaghan%'
    OR n.Caption LIKE '%Newport%'
    OR n.Caption LIKE '%Olten%'
    OR n.Caption LIKE '%Oslo%'
    OR n.Caption LIKE '%Plymouth%'
    OR n.Caption LIKE '%Rome%'
    OR n.Caption LIKE '%Skipton%'
    OR n.Caption LIKE '%Turin%'
    OR n.Caption LIKE '%Warsaw%'
    OR n.Caption LIKE '%Zurich%'
    OR n.Caption LIKE '%Elsenheimer%'
    OR n.Caption LIKE '%GENEVA%'
    OR n.Caption LIKE '%COPENHAGEN%'
    OR n.Caption LIKE 'USCISL%'
    OR n.Caption LIKE 'CACISL%'
    OR n.Caption LIKE 'AUCISL%'
    OR n.Caption LIKE 'AUCOIY%'
    OR n.Caption LIKE 'AUCOEY%'
    OR n.Caption LIKE 'NZCISL%'
    OR n.Caption LIKE 'CACOEYTORON01R%'
    OR n.Caption LIKE 'CACOEYTORON02R%'
    OR n.Caption LIKE 'CACOIYRHICA01PVCE01_EHA'
    OR n.Caption LIKE 'CACOIYTORCA01PVCE01_EHA'
    OR n.Caption LIKE 'CHCISLGEN%'
    OR n.Caption LIKE 'CHGVARF1P203%'
    OR n.Caption LIKE 'DEFRAKGF122CR01'
    OR n.Caption LIKE 'DKCISLHOV%'
    OR n.Caption LIKE '%Dublin SDWAN%'
    OR n.Caption LIKE '%Jersey SDWAN%'
    OR n.Caption LIKE '%London SDWAN%'
    OR n.Caption LIKE '%Monaghan SDWAN%'
    OR n.Caption LIKE 'NATORPDC01%'
    OR n.Caption LIKE '%Newport SDWAN%'
    OR n.Caption LIKE 'UKBRSP%'
    OR n.Caption LIKE 'UKCRODGFSW%'
    OR n.Caption LIKE 'UKNEPCGFP%'
    OR n.Caption LIKE 'USCOIYWTWMA01PVCE01_SHA'
    OR n.Caption LIKE 'ZACISLJOH%'
    OR n.Caption LIKE 'ZAJNBBGF03%'
    OR n.Caption LIKE 'ZAJNBKGF01FW01%'
    OR n.Caption LIKE 'IN3221%'
    OR n.Caption LIKE 'CNCISL%'
    OR n.Caption LIKE 'HKCISL%'
    OR n.Caption LIKE 'HKGTL%'
    OR n.Caption LIKE 'HKKOW%'
    OR n.Caption LIKE 'HKEQX%'
    OR n.Caption LIKE 'aumeld%'
    OR n.Caption LIKE 'aubne%'
    OR n.Caption LIKE 'HKKOWE3601CORE'
    OR n.Caption LIKE 'NAQASFDC%'
    OR n.Caption LIKE '%NAQASPDC10%'
    OR n.Caption IN (
        'USCOIYBNKIL01PVCE01_EHA',
        'USCOIYCLTNC01PVCE01_EHA',
        'USCOIYCHIIL01PVCE01',
        'USCOIYDENCO01PVCE01_EHA',
        'USCOIYEDNNJ01PVCE01_EHA',
        'USCOIYFIXTX02PVCE01_EHA',
        'USCOIYHOUTX01PVCE01_EHA',
        'USCOIYNYCNY01PVCE01_EHA',
        'USCOIYPBNUS01PVCE01_EHA',
        'USCOIYWTWMA01PVCE01_SHA',
        'USCOIYHLGWV01PVCE01_EHA',
        'AUCOEYABB0001R.attgmis.com',
        'AUCOEYPOR0002R.attgmis.com',
        'AUCOIYABAAU01PVCE01_SHA',
        'AUCOIYCIAAU01PVCE01_EHA',
        'AUCOIYPERAU01PVCE01_EHA',
        'AUCOIYPORAU02PVCE01_SHA',
        'AUCOIYSYDAU02PVCE01_EHA',
        'ZACISLJOH0009R',
        'ZACISLJOH0010R',
        'ZAJNBBGF03CR01',
        'ZAJNBBGF03CR02',
        'ZAJNBBGF03SW02',
        'ZAJNBKGF01FW01.cshare.net'
    )
)
AND
(
    ( n.Caption LIKE 'USCOIY%' AND i.InterfaceAlias LIKE '%ATT%' )
    OR ( n.Caption = 'AUCOEYABB0001R.attgmis.com (Internet)' AND i.InterfaceAlias LIKE '%link to PE%' )
    OR ( n.Caption = 'AUCOEYPOR0002R.attgmis.com (Internet)' AND i.InterfaceAlias LIKE '%link to PE%' )
    OR ( n.Caption LIKE 'AUCOIY%' AND i.InterfaceAlias LIKE '%ATT%' )
    OR ( n.Caption LIKE 'HKEQX5R101Cr%' AND (i.InterfaceName LIKE '%1/45%' OR i.InterfaceName LIKE '%1/46%') )
    OR ( n.Caption LIKE 'IN3221%' AND (i.InterfaceName LIKE '%dp0p5%' OR i.InterfaceName LIKE '%dp0p6%') )
    OR ( n.Caption LIKE 'NATORPDC01SW0%' AND i.InterfaceName LIKE 'Ethernet1/47%' )
    OR ( n.Caption LIKE 'ZACISLJOH%' AND i.InterfaceAlias LIKE '%link to PE%' )
    OR ( n.Caption LIKE 'CHGVARF1P203%' AND i.InterfaceAlias LIKE '%P2P%' )

    OR ( n.Caption LIKE 'UK%' AND (i.InterfaceAlias LIKE '%Internet%' OR i.InterfaceAlias LIKE '%ISL%' OR i.InterfaceAlias LIKE '%vPC%'))
    
    OR (
        n.Caption LIKE 'CACOEYTORON01R%'
        AND i.InterfaceAlias LIKE '%LAN INT PRMY%'
    )
    OR (
        n.Caption LIKE 'CACOEYTORON02R%'
        AND i.InterfaceAlias LIKE '%LAN interface%'
    )

    OR (
        n.Caption = 'CACOIYTORCA01PVCE01_EHA'
        AND i.InterfaceAlias LIKE '%ATT%'
    )

    OR (
        n.Caption LIKE 'DKCISLHOV%'
        AND i.InterfaceAlias LIKE '%link to PE%'
    )

    OR (
        n.Caption = 'CHCISLGEN0001R (GENEVA MPLS)'
        AND i.InterfaceName IN (
            'GigabitEthernet0/0/0'        )
    )
    
    OR (
        n.Caption LIKE 'NAQASFDC%'
        AND (
            i.InterfaceName LIKE 'Ethernet1/33%'
            OR i.InterfaceName LIKE 'Ethernet1/34%'
            OR i.InterfaceName LIKE 'Ethernet1/8%'
        )
    )
    OR (
        n.Caption = 'NAQASPDC101SW01'
        AND i.InterfaceName LIKE 'Ethernet1/25%'
    )

   
    OR (
        n.Caption = 'DEFRAKGF122CR01'
        AND i.InterfaceAlias LIKE '%P2P%'
    )

    
    OR (
        n.Caption IN ('ZAJNBBGF03CR01','ZAJNBBGF03CR02')
        AND (
            i.InterfaceName LIKE 'Ethernet1/40%'
            OR i.InterfaceName LIKE 'Ethernet1/41%'
        )
    )
    OR (
        n.Caption = 'ZAJNBBGF03SW02'
        AND i.InterfaceName LIKE 'Ethernet1/47%'
)


    OR (
           i.InterfaceAlias LIKE '%MPLS%'
        OR i.InterfaceAlias LIKE '%ATT%'
        OR i.InterfaceAlias LIKE '%AT&T%'
        OR i.InterfaceAlias LIKE '%INTERNET%'
        OR i.InterfaceAlias LIKE '%WAN%'
        OR i.InterfaceAlias LIKE '%link to PE%'
        OR i.InterfaceName  LIKE 'GE%'
        OR i.InterfaceName  LIKE 'GigabitEthernet%'
        OR i.InterfaceName  LIKE 'TenGigabitEthernet%'
    )
)
AND NOT (
    n.Caption IN ('ZACISLJOH0009R','ZACISLJOH0010R')
    AND i.InterfaceAlias NOT LIKE '%link to PE%'
)


AND NOT (
    n.Caption LIKE 'USCOIY%'
    AND i.InterfaceAlias NOT LIKE '%ATT%'
)

AND NOT (
    n.Caption = 'CACOIYTORCA01PVCE01_EHA'
    AND i.InterfaceAlias NOT LIKE '%ATT%'
)

AND (
    i.InterfaceAlias IS NOT NULL
    OR n.Caption = 'CHCISLGEN0001R (GENEVA MPLS)'
)


AND (
    i.InterfaceAlias <> ''
    OR n.Caption = 'CHCISLGEN0001R (GENEVA MPLS)'
)

AND i.InterfaceAlias NOT LIKE '%B2B%'
AND (
    i.InterfaceAlias NOT LIKE '%LAN%'
    OR n.Caption IN (
        'AUCOEYPOR0002R.attgmis.com (Internet)',
        'CACOEYTORON01R',
        'CACOEYTORON02R',
        'IN3221BLR0001R',
        'IN3221BLR0002R',
        'IN3221HYD0001R',
        'IN3221HYD0002R',
        'ZAJNBBGF03CR01',
        'ZAJNBBGF03CR02',
        'ZAJNBBGF03SW02'

    )
)
AND i.InterfaceAlias NOT LIKE '%Customer%'
AND i.InterfaceAlias NOT LIKE '%Loopback%'
AND i.InterfaceAlias NOT LIKE '%GRE%'
AND i.InterfaceName  NOT LIKE '%Port-channel%'
AND (
    i.InterfaceName NOT LIKE '%.%'
    OR i.InterfaceAlias LIKE '%link to PE%'
    OR i.InterfaceAlias LIKE '%ATT%'
)
AND i.OperStatus = 1
ORDER BY n.Caption;";
    }
}