using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class WisamType
    {
        public enum CalendarTypes
        {
            StateFiscalYear = 1,
            FederalFiscalYear = 2,
            CalendarYear = 3,
        }

        public enum AnalysisTypes
        {
            Optimal,
            MetaManager,
            LocalProgram,
            CoreData,
        }

        public enum PmicProjectTypes
        {
            Predictive = 1,
            Planned = 2, // New non-replacement structures
            AnyProgrammed = 3,
            PredictiveAndPlanned = 4,
            Roadway = 5,
        }

        public enum Risks
        {
            Nhs = 1,
            Strahnet = 2,
            Sccr = 3,
            Frcr = 4,
            Sd = 5,
            Fo = 6,
            LoadCap = 7,
            Mvw = 8,
            VertClr = 9,
            Adt = 10,
            Adtt = 11,
            DetLength = 12,
            BridgeAge = 13,
        }

        public enum ElementDeteriorationRates
        {
            ByElementClassification,
            ByElement,
            ByBrm,
        }

        public enum Databases
        {
            HsiProduction,
            HsiAcceptance,
            HsiDev,
            Fiips,
            WiSamProduction,
            WiSamDev,
            WiSamTest,
        }

        public enum NbiRatingTypes
        {
            Deck,
            Superstructure,
            Substructure,
            Culvert,
        }

        public enum CaiBases
        {
            Inspection,
            Deterioration,
        }

        public enum AnalysisReports
        {
            Optimal,
            StrDeckReplacements,
            Flexible,
            StatePmdss,
            StateFiips,
            StateNeeds,
            StateNeedsPlus,
            LocalNeedsRanking,
            LocalNeedsPriority,
            AnalysisDebug,
            AllCurrentNeeds,
            FiipsAnalysisDebug,
            StateNeedsPmdss,
            RegionNeeds,
            RegionNeedsNew,
            LocalNeeds,
            NbiDeterioration,
            MetaManager,
        }

        public enum NeedsAnalysisFileTypes
        {
            ProgramUnconstrained,
            ProgramConstrained,
            LocalProgram,
            MetaManager,
            ConditionUnconstrained, // Bridge condition, deterioration
            ConditionConstrained, // Bridge condition, deterioration
            PriorityUnconstrained,
            PriorityConstrained,
            Inventory,
            InputFile,
            CoreData,
        }

        public enum FundingSources
        {
            Backbone,
            SHR3R,
            SHRLargeBridge,
            SEF,
            Major,
            MajorInterstate,
            HighCostSTHBridge,
            LocalProgram,
            Maintenance,
        }
    }
}
