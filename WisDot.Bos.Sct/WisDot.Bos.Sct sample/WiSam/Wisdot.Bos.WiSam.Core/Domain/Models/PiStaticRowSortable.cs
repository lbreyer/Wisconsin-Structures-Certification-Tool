using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class PiStaticRowSortable
    {
        public string StructureId { get; set; }
        public string CorridorCode { get; set; }
        public string Region { get; set; }
        public string County { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
        public string StructureType { get; set; }
        public string MainSpanMaterial { get; set; }
        public int NumSpans { get; set; }
        public double TotalLengthSpans { get; set; }
        public string InventoryRating { get; set; }
        public string OperatingRating { get; set; }
        public string LoadPosting { get; set; }
        public DateTime LastInspectionDate { get; set; }
        public string ConstructionHistory { get; set; }

        public bool CplxStr { get; set; }
        public bool LrgStr { get; set; }
        public bool ScourCritical { get; set; }
        public bool FractureCritical { get; set; }

        //public int Age { get; set; }
        //public int DeckAge { get; set; }
        public int DetLen { get; set; }
        public string FunctionalClassificationOn { get; set; }
        public string FunctionalClassificationUnder { get; set; }
        public string Municipality { get; set; }
        public string MunicipalityNumber { get; set; }
        public int MaxVehicleWeight { get; set; } //kips
        public double LoadPostingTonnage { get; set; } //tons
        public double VerticalClearanceUnderMin { get; set; }
        public double DesiredVerticalClearanceUnder { get; set; }
        public int DamageInspectionsCount { get; set; }
        public bool Nhs { get; set; }
        public string RouteSystemOn { get; set; } // Local system label in HSIS
        public string RouteSystemUnder { get; set; }
        public int Adt { get; set; }
        public int AdttPercent { get; set; }
        public int LanesOn { get; set; }
        public float DeckWidthMin { get; set; }
        public float DeckArea { get; set; }

        public bool BorderBridge { get; set; }
        public string BorderState { get; set; }
        public bool MunicipalPlanningBridge { get; set; }
        public string MunicipalPlanningAgency { get; set; }
        public bool PrimaryHighwayFreightSystemBridge { get; set; }

        public bool GisStateBridgeCoordinatesWithin500FtHsisCoordinates { get; set; }
        public bool GisWislrLocalBridgeCoordinatesWithin500FtHsisCoordinates { get; set; }
        public string GisCorridor2030Code { get; set; }
        public string GisDividedHighwayCode { get; set; }
        public string GisFunctionalClassCode { get; set; }
        public string GisFunctionalClassAbbreviation { get; set; }
        public string GisFunctionalClassDescription { get; set; }
        public string GisNhsDesignation { get; set; }
        public string GisProjectRouteType { get; set; }
        public string GisProjectRouteName { get; set; }
        public string GisLongTruckRouteDesignation { get; set; }
        public string GisMaintenanceJurisdictionCode { get; set; }
        public string GisMaintenanceJurisdictionRouteType { get; set; }
        public string GisMaintenanceJurisdictionRouteName { get; set; }
        public bool GisOsowHighClearanceRoute { get; set; }
        public string GisOsowRouteType { get; set; }
        public string GisOsowRouteName { get; set; }
        public string GisOsowRankingNumber { get; set; }
        public string GisOsowRankingName { get; set; }

        // Traffic properties
        public int MmRoadwaySegmentId { get; set; }
        public int MmForecastedAadtYear1 { get; set; }
        public int MmForecastedAadtYear5 { get; set; }
        public int MmForecastedAadtYear10 { get; set; }
        public int MmForecastedAadtYear15 { get; set; }
        public int MmForecastedAadtYear20 { get; set; }
        public float MmForecastedTruckPercentageAadtYear1 { get; set; }
        public int MmRoadwayPostedSpeedLimit { get; set; }
        public string MmCorridorsCode2030 { get; set; }
        public string MmFunctionalClassificationOn { get; set; }
        public string MmDividedHighwayCode { get; set; }
        public int MmTrafficSegmentId { get; set; }
    }
}
