using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class DebugRowSheet1
    {
        public string StructureId { get; set; }
        public string StructureType { get; set; }

        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
        public string County { get; set; }
        public int Region { get; set; }

        public int WorkActionYear { get; set; }

        public int Age { get; set; }
        public double PriorityScore { get; set; }

        //public string CaiBasis { get; set; }
        public double DoNothingCaiValue { get; set; }
        public string DoNothingProgrammableWorkCandidate { get; set; }
        public double DoNothingProgrammableWorkCandidateCaiValue { get; set; }
        public double DoNothingProgrammableWorkCandidateCost { get; set; }

        public string ImprovementType { get; set; }
        public double ImprovementTypeCaiValue { get; set; }

        public string PerformedPrimary { get; set; }
        public double PerformedPrimaryCost { get; set; }
        public string PerformedIncidentals { get; set; }
        public double PerformedIncidentalsCost { get; set; }
        public double PerformedCaiValue { get; set; }
        public string ProgrammableWorkCandidate { get; set; }
        public double ProgrammableWorkCandidateCost { get; set; }
        public string PerformedCriteria { get; set; }
        //public string ProgrammableWorkCost { get; set; }

        public string EligibleProgrammableIncidentalWorkCandidates { get; set; }
        public string EligibleNonProgrammableIncidentalWorkCandidates { get; set; }
        //public string NonCriteriadSecondaryWorkCandidates { get; set; }
        //public string EstimatedProjectCost { get; set; }
    }
}
