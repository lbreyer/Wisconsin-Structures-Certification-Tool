using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class NeedsAnalysisInput
    {
        public string RunBy { get; set; }
        public DateTime RunDate { get; set; }
        public string WisamsDatabase { get; set; }
        public WisamType.AnalysisTypes AnalysisType { get; set; }
        public string StructureSelection { get; set; } //ByIds or ByRegions or ByFunding
        public int MaxNumberToAnalyze { get; set; }
        public List<string> StructureIds { get; set; }
        public bool ApplyRegionSelectionsWhenByFunding { get; set; }
        public List<string> Regions { get; set; }
        public List<string> RegionNumbers { get; set; }
        public bool IncludeStateOwned { get; set; }
        public bool IncludeLocalOwned { get; set; }
        public bool IncludeCStructures { get; set; }
        public int AnalysisStartYear { get; set; }
        public int AnalysisEndYear { get; set; }
        public WisamType.CalendarTypes CalendarType { get; set; }
        public bool DeteriorateOverlayDefects { get; set; }
        public bool InterpolateNbiRatings { get; set; }
        public bool CountThinPolymerOverlays { get; set; } //Total # of overlays
        public bool ShowPriorityScore { get; set; }
        public bool CreateDebugFile { get; set; }
        public List<int> ElementsToReport { get; set; }
        public bool CreateBridgeInventoryFile { get; set; }
        public List<string> EligiblePrimaryWorkActionCodes { get; set; }
        public bool RunUnconstrainedAnalysis { get; set; }
        public bool ApplyBudget { get; set; }
        public bool IsMultiYearBudget { get; set; }
        public List<Budget> PrimaryWorkActionBudget { get; set; }
        public List<PriorityScorePolicyEffect> PriorityScorePolicyEffects { get; set; }
        public string AnalysisNotes { get; set; }
        public string BaseDirectoryOfAnalysisFiles { get; set; }
        public List<NeedsAnalysisFile> AnalysisFiles { get; set; }
        public int CaiFormulaId { get; set; } // Set to 10 per database default
        public int DeteriorationStartOffset { get; set; }
        public string ValidationErrors { get; set; }
        public float LeastCostProject { get; set; }
        public List<WisamType.FundingSources> FundingSources { get; set; }
        public WisamType.ElementDeteriorationRates ElementDeteriorationMethod { get; set; }

        public NeedsAnalysisInput()
        {
            BaseDirectoryOfAnalysisFiles = @"c:\temp";
            StructureIds = new List<string>();
            Regions = new List<string>();
            RegionNumbers = new List<string>();
            EligiblePrimaryWorkActionCodes = new List<string>();
            PrimaryWorkActionBudget = new List<Budget>();
            PriorityScorePolicyEffects = new List<PriorityScorePolicyEffect>();
            AnalysisFiles = new List<NeedsAnalysisFile>();
            ElementsToReport = new List<int>();
            CaiFormulaId = 10;
            DeteriorationStartOffset = 2;
            FundingSources = new List<WisamType.FundingSources>();
            RunUnconstrainedAnalysis = true;
        }

        public NeedsAnalysisInput(string runBy)
        {
            RunBy = runBy;
            BaseDirectoryOfAnalysisFiles = @"c:\temp";
            StructureIds = new List<string>();
            Regions = new List<string>();
            RegionNumbers = new List<string>();
            EligiblePrimaryWorkActionCodes = new List<string>();
            PrimaryWorkActionBudget = new List<Budget>();
            PriorityScorePolicyEffects = new List<PriorityScorePolicyEffect>();
            AnalysisFiles = new List<NeedsAnalysisFile>();
            ElementsToReport = new List<int>();
            CaiFormulaId = 10;
            DeteriorationStartOffset = 2;
            FundingSources = new List<WisamType.FundingSources>();
            RunUnconstrainedAnalysis = true;
        }
    }
}
