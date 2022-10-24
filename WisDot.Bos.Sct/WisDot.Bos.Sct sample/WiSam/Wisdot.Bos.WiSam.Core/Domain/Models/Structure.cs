using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Structure
    {
        public string StructureId { get; set; }
        public string StructureType { get; set; } // main span configuration
        public string StructureTypeCode { get; set; }
        public bool IsCulvert { get; set; }
        public int NumSpans { get; set; }
        public double TotalLengthSpans { get; set; }
        public string MainSpanMaterial { get; set; }
        public string MainSpanMaterialCode { get; set; }
        //public string MainSpanConfiguration { get; set; }
        public int SkewAngle { get; set; }
        public double HorizontalCurve { get; set; }
        public string InventoryRating { get; set; }
        public string OperatingRating { get; set; }
        public int LoadPostingCode { get; set; }
        public string LoadPostingDesc { get; set; }
        public double LoadPostingTonnage { get; set; }
        public int MaxVehicleWeight { get; set; }
        public string RegionNumber { get; set; }
        public string Region { get; set; }
        public string CountyNumber { get; set; }
        public string County { get; set; }
        public string MunicipalityNumber { get; set; }
        public string Municipality { get; set; }
        public string OwnerNumber { get; set; }
        public string Owner { get; set; }
        public string StmcAgcyTy { get; set; }
        public string StimAgcyTycd { get; set; }
        public List<string> FundingEligibility { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
        public int YearBuiltActual { get; set; }
        public int YearBuilt { get; set; }
        public int LoadCapacity { get; set; }
        public double SufficiencyNumber { get; set; }
        public double SufficiencyRatingCurrent { get; set; }
        public int NumOlays { get; set; }
        public int NumThinPolymerOverlays { get; set; }
        public bool CountTpo { get; set; }
        public double DeckArea { get; set; }
        public int SuperstructurePaintArea { get; set; }
        public int SuperstructureSpotPaintArea { get; set; }
        public int JointLength { get; set; }
        public int DeckBuiltYear { get; set; }
        public int DeckBuiltYearActual { get; set; }
        public List<int> DeckBuilts { get; set; }
        public List<int> SuperBuilts { get; set; }
        public List<int> Overlays { get; set; }
        public List<float> DeckWidths { get; set; }
        public List<string> DeckComposition { get; set; }
        public float MinDeckWidth { get; set; }

        public bool BorderBridge { get; set; }
        public string BorderState { get; set; }
        public bool MunicipalPlanningBridge { get; set; }
        public string MunicipalPlanningAgency { get; set; }
        public bool PrimaryHighwayFreightSystemBridge { get; set; }

        // Qualified NBI Deterioration Curves
        public string NbiDeckQualifiedDeteriorationCurve { get; set; }
        public string NbiSuperQualifiedDeteriorationCurve { get; set; }
        public string NbiSubQualifiedDeteriorationCurve { get; set; }

        // GIS properties
        public string Latitude { get; set; }
        public string Longitude { get; set; }
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

        // STN properties
        public int StnRoadwayLinkId { get; set; }
        public DateTime StnStructureHistoricalDate { get; set; }
        public string StnRoadwayLongTruckRouteDesignation { get; set; }

        // Importance Factor Measurements
        public int Adt { get; set; }
        public int AdtYr { get; set; }
        public int AdtFut { get; set; }
        public int AdtFutYr { get; set; }
        public int AdtUnder { get; set; }
        public double Agr { get; set; }
        public int LanesOn { get; set; }
        public double AdtPerLane { get; set; }
        public double Adtt { get; set; }
        public int AdttPercent { get; set; }
        public double AdttPerLane { get; set; }
        public int DetLen { get; set; }
        public bool Strahnet { get; set; }
        public string CorridorCode { get; set; }
        public string CorridorDesc { get; set; }
        public bool Bhn { get; set; }
        public bool Nhs { get; set; }
        public string HighwayFeatureDesignationOn { get; set; }
        public string HighwayFeatureDesignationUnder { get; set; }
        public string ServiceFeatureUnderTypeCode { get; set; }
        public bool Staa { get; set; }

        // Risk Factor Measurements
        public bool CplxStr { get; set; }
        public bool UnitBridge { get; set; }
        public bool LrgStr { get; set; }
        public double WholeStructureDeckArea { get; set; } // Sum of deck areas of unit bridges; otherwise 0 if not applicable
        public bool DmgInsp { get; set; }
        public int DamageInspectionsCount { get; set; }
        public bool ScourCritical { get; set; }
        public string ScourCriticalRating { get; set; }
        public bool FractureCritical { get; set; }
        public string ScFc { get; set; }
        public bool HighClearanceRoute { get; set; }

        // Structure Capacity Factor Measurements
        public int trafficPatternOn { get; set; }
        public int trafficPatternUnder { get; set; }
        public string RouteSystemOn { get; set; }
        public string RouteSystemUnder { get; set; }
        public string HsOrRf { get; set; }
        public double ratingValue { get; set; }
        public double VerticalClearanceUnderMin { get; set; }
        public double DesiredVerticalClearanceUnder { get; set; }
        public double VerticalClearanceOnMin { get; set; }
        public string FunctionalClassificationUnderCode { get; set; }
        public string FunctionalClassificationUnder { get; set; }
        public string FunctionalClassificationOnCode { get; set; }
        public string FunctionalClassificationOn { get; set; }
        public bool StructurallyDeficient { get; set; }
        public bool FunctionalObsolete { get; set; }
        public bool FunctionalObsoleteDueToStructureEvaluation { get; set; }
        public bool FunctionalObsoleteDueToWaterwayAdequacy { get; set; }
        public bool FunctionalObsoleteDueToVerticalClearance { get; set; }
        public bool FunctionalObsoleteDueToDeckGeometry { get; set; }
        public bool FunctionalObsoleteDueToApproachRoadwayAlignment { get; set; }
        public string ConstructionHistory { get; set; }
        public double OverburdenDepth { get; set; }
        public bool BuriedStructure { get; set; }
        public bool NewStructureInCurrentYear { get; set; }
        public List<string> Deficiencies { get; set; }
        public string LocalFundingEligibility { get; set; }

        public Inspection LastInspection { get; set; }
        public int LastInspectionYear { get; set; }
        public Cai LastInspectionCai { get; set; }
        public Cai LastInspectionCaiForAnalysis { get; set; } // Gets updated during analysis
        public List<CoreInspection> CoreInspections { get; set; }
        public bool InterpolateNbi { get; set; }
        public double DeckPercentageCs1 { get; set; }
        public double SuperstructurePercentageCs1 { get; set; }
        public double SubstructurePercentageCs1 { get; set; }
        public double DeckRatingInterpolated { get; set; }
        public double SuperstructureRatingInterpolated { get; set; }
        public double SubstructureRatingInterpolated { get; set; }
        public double DeckRatingInspected { get; set; }
        public double SuperstructureRatingInspected { get; set; }
        public double SubstructureRatingInspected { get; set; }
        public Cai CurrentCai { get; set; }
        public string SpecialComponents { get; set; }

        public List<StructureWorkAction> AllCurrentNeeds { get; set; }
        public List<StructureWorkAction> YearlyProgrammedWorkActions { get; set; }
        public List<StructureWorkAction> YearlyOptimalWorkActions { get; set; }
        public List<StructureWorkAction> YearlyConstrainedOptimalWorkActions { get; set; }
        public List<StructureWorkAction> YearlyDoNothings { get; set; }
        public List<StructureWorkAction> YearlyOptimalWorkActionsBasedOnDoNothingCondition { get; set; }
        //public List<StructureWorkAction> YearlyWorstCaseScenario { get; set; }
        public List<StructureWorkAction> ConstructionHistoryProjects { get; set; }

        public List<Risk> Risks { get; set; }
        public double PriorityScore { get; set; }
        public int OverlayQuantity { get; set; }

        public bool AddedFiipsCurrentYearProject { get; set; }

        public double ImportanceFactor { get; set; }
        public double RiskFactor { get; set; }
        public double StructureCapacityFactor { get; set; }
        public List<PriorityFactor> PriorityFactors { get; set; }
        public List<PriorityFactorMeasurement> PriorityFactorMeasurements { get; set; }
        public double PriorityIndex { get; set; }

        public int LastSuperReplacementYear { get; set; }
        public int LastDeckReplacementYear { get; set; }
        public int LastOverlayYear { get; set; }

        public List<PriorityIndexCategory> PriorityIndexCategories { get; set; }
        public List<PriorityIndexFactor> PriorityIndexFactors { get; set; }
        public List<PriorityScorePolicyEffect> PriorityScorePolicyEffects { get; set; }
        public List<StructureWorkAction> DidNotMakeCutWorkActions { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public Structure()
        {
            YearlyDoNothings = new List<StructureWorkAction>();
            YearlyOptimalWorkActions = new List<StructureWorkAction>();
            YearlyConstrainedOptimalWorkActions = new List<StructureWorkAction>();
            YearlyOptimalWorkActionsBasedOnDoNothingCondition = new List<StructureWorkAction>();
            ConstructionHistoryProjects = new List<StructureWorkAction>();
            AllCurrentNeeds = new List<StructureWorkAction>();
            YearlyProgrammedWorkActions = new List<StructureWorkAction>();
            Risks = new List<Risk>();
            Deficiencies = new List<string>();
            LoadPostingTonnage = 0;
            CplxStr = false;
            VerticalClearanceOnMin = -1; // -1 indicates null
            VerticalClearanceUnderMin = -1; // -1 indicates null
            HsOrRf = "";
            LoadCapacity = 5;
            DeckBuilts = new List<int>();
            DeckWidths = new List<float>();
            MinDeckWidth = -1; // -1 indicates null
            OverburdenDepth = 0;
            BuriedStructure = false;
            DeckComposition = new List<string>();
            SuperBuilts = new List<int>();
            Overlays = new List<int>();
            PriorityIndexCategories = new List<PriorityIndexCategory>();
            PriorityIndexFactors = new List<PriorityIndexFactor>();
            PriorityScorePolicyEffects = new List<PriorityScorePolicyEffect>();
            DidNotMakeCutWorkActions = new List<StructureWorkAction>();
            FundingEligibility = new List<string>();
            SpecialComponents = "";
        }

        public Structure(string structureId)
        {
            StructureId = structureId;
            OverburdenDepth = 0;
            BuriedStructure = false;
            PriorityIndexCategories = new List<PriorityIndexCategory>();
            PriorityIndexFactors = new List<PriorityIndexFactor>();
            PriorityScorePolicyEffects = new List<PriorityScorePolicyEffect>();
            DidNotMakeCutWorkActions = new List<StructureWorkAction>();
            FundingEligibility = new List<string>();
            SpecialComponents = "";
        }
    }
}
