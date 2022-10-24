using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class ProgrammedWorkAction : WorkAction
    {
        public int FiipsId { get; set; }
        public int ImprId { get; set; }
        public int WorkActionYear { get; set; }
        public Cai CAI { get; set; }
        public double Cost { get; set; }

        public string DotRegionCode { get; set; }
        public string StructureId { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
        public string County { get; set; }
        public string FosProjId { get; set; }
        public string DesignProjectId { get; set; }
        public int PProjId { get; set; }
        public string NewStructureId { get; set; }
        public string FundingCategoryNumber { get; set; }
        public string FundingCategoryDesc { get; set; }

        public string OriginalWorkActionCode { get; set; }
        public string OriginalWorkActionDesc { get; set; }
        public string WorkActionCode { get; set; }
        public string WorkActionDesc { get; set; }

        public DateTime EstimatedCompletionDate { get; set; }
        //public int EstimateCompletionDateFiscalYear { get; set; }
        public DateTime EarliestAdvanceableLetDate { get; set; }
        public DateTime LatestAdvanceableLetDate { get; set; }
        public DateTime PseDate { get; set; }
        public DateTime EarliestPseDate { get; set; }
        public int StateFiscalYear { get; set; }
        public string Route { get; set; }

        public string SubProgramCode { get; set; }
        public string SubProgramDesc { get; set; }

        public string Title { get; set; }
        public string Limit { get; set; }
        public string Concept { get; set; }

        public string FunctionalTypeCode { get; set; }
        public string FunctionalTypeDesc { get; set; }

        public string WisDOTProgramCode { get; set; }
        public string WisDOTProgramDesc { get; set; }

        public string PlanningProjectConceptCode { get; set; } //Improvement Type
        public string PlanningProjectConceptDesc { get; set; }
        public string PlanningProjectConceptCodeNew { get; set; } //Improvement Type
        public string PlanningProjectConceptDescNew { get; set; }

        public string LifeCycleStageCode { get; set; }
        public string LifeCycleStageDesc { get; set; }
        public DateTime LifeCycleStageDate { get; set; }

        public string ComponentTypeCode { get; set; }
        public string ProjectStatusFlag { get; set; }

        public float ProjectTotalWithDeliveryAmount { get; set; } //total project cost with delivery
        public float ProjectTotalWithoutDeliveryAmount { get; set; } //total project cost without delivery
        public float WorkTotalWithDeliveryAmount { get; set; } //structure cost with delivery
        public float WorkTotalWithoutDeliveryAmount { get; set; } //structure cost with delivery
        public float WorkWithDeliveryCost { get; set; } // structure cost with delivery per funding source
        public float WorkWithoutDeliveryCost { get; set; } // structure cost without delivery per funding source

        public string FundingSourceTypeCode { get; set; }
        public float FundingCategoryEstimatedAmount { get; set; }
        public string FederalImprovementTypeCode { get; set; }
        public string FederalImprovementTypeDesc { get; set; }
        public string ProjectManager { get; set; }
        public bool IsDuplicate { get; set; }
        public bool IsBridge { get; set; }
    }
}
