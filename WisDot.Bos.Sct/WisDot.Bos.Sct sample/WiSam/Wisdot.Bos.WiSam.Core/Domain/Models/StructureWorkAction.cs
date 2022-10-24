using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class StructureWorkAction : WorkAction
    {
        public string StructureId { get; set; }
        public Structure Structure { get; set; }
        public int WorkActionYear { get; set; }
        public int WorkActionStateFiscalYear { get; set; }
        public int WorkActionFederalFiscalYear { get; set; }
        public DateTime WorkActionDate { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
        
        public Cai CAI { get; set; }
        public double Cost { get; set; }
        public string FosProjId { get; set; }
        public int PProjId { get; set; }
        public bool Improvement { get; set; }
        public bool Preservation { get; set; }
        public double LifeExtension { get; set; }
        public double CostEffectiveness { get; set; }
        public bool CombinedWorkAction { get; set; }
        public string CombinedWorkActionCodes { get; set; }
        public List<StructureWorkAction> CombinedWorkActions { get; set; }
        public bool IncludesOverlay { get; set; }
        public bool Repeatable { get; set; }
        public int RepeatFrequency { get; set; }
        public string ConditionBenefit { get; set; }
        public WorkActionCriteria ControllingCriteria { get; set; }
        public bool BypassCriteria { get; set; }
        public List<StructureWorkAction> AllSecondaryWorkActions { get; set; } // All incidental work actions evaluated for eligibility
        public List<StructureWorkAction> SecondaryWorkActions { get; set; } // All eligible incidental work actions
        public List<CombinedWorkAction> PotentialIncidentals { get; set; }
        public string AlternativeWorkActionCode { get; set; }
        public string AlternativeWorkActionDesc { get; set; }
        public string PlanningProjectConceptCode { get; set; }
        public string PlanningProjectConceptDesc { get; set; }
        public string WisDOTProgramCode { get; set; }
        public string WisDOTProgramDesc { get; set; }
        public double TotalCostWithDeliveryAmount { get; set; }
        public double TotalCostWithoutDeliveryAmount { get; set; }

        public List<PriorityIndexCategory> PriorityIndexCategories { get; set; }
        public List<PriorityIndexFactor> PriorityIndexFactors { get; set; }
        public List<PriorityScorePolicyEffect> PriorityScorePolicyEffects { get; set; }
        public float PriorityScore { get; set; }
        public int PriorityScoreRank { get; set; }
        public string PriorityScoreRelativeRank { get; set; }

        public StructureWorkAction()
        {
            CombinedWorkActions = new List<StructureWorkAction>();
            AllSecondaryWorkActions = new List<StructureWorkAction>();
            SecondaryWorkActions = new List<StructureWorkAction>();
            PriorityIndexCategories = new List<PriorityIndexCategory>();
            PriorityIndexFactors = new List<PriorityIndexFactor>();
            PriorityScorePolicyEffects = new List<PriorityScorePolicyEffect>();
            PriorityScoreRank = -1;
            PriorityScoreRelativeRank = "";
        }

        public StructureWorkAction(string workActionCode)
        {
            CombinedWorkActions = new List<StructureWorkAction>();
            AllSecondaryWorkActions = new List<StructureWorkAction>();
            SecondaryWorkActions = new List<StructureWorkAction>();
            WorkActionCode = workActionCode;
            PriorityIndexCategories = new List<PriorityIndexCategory>();
            PriorityIndexFactors = new List<PriorityIndexFactor>();
            PriorityScorePolicyEffects = new List<PriorityScorePolicyEffect>();
            PriorityScoreRank = -1;
            PriorityScoreRelativeRank = "";
        }
    }
}
