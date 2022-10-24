using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class DebugRowSheet2
    {
        public string StructureId { get; set; }
        public string StructureType { get; set; }
        public int WorkActionYear { get; set; }
        // public string CaiBasis { get; set; }
        public double DoNothingCaiValue { get; set; }
        public string DoNothingQuantities { get; set; }
        public string PrimaryWorkCandidate { get; set; }
        public string PerformedIncidentals { get; set; }
        public double ProgrammableWorkCandidateCaiValue { get; set; }
        public string ProgrammableWorkCandidate { get; set; }
        public string ProgrammableWorkCriteria { get; set; }
        public string ProgrammableWorkBenefits { get; set; }
        public string ProgrammableWorkQuantities { get; set; }
        public string EligibleProgrammableIncidentalWorkCandidates { get; set; }
        public string EligibleNonProgrammableIncidentalWorkCandidates { get; set; }
        public string AllEvaluatedIncidentalWorkCandidates { get; set; }
    }
}
