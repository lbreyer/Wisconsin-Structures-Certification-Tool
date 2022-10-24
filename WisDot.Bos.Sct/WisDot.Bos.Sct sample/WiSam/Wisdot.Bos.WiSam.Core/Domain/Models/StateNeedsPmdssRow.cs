using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class StateNeedsPmdssRow
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public int Age { get; set; }

        public double DoNothingNbiDeck { get; set; }
        public double DoNothingNbiSup { get; set; }
        public double DoNothingNbiSub { get; set; }
        public double DoNothingNbiCulv { get; set; }
        public double DoNothingCaiValue { get; set; }
        public string DoNothingOptimalWorkCandidate { get; set; }
        public string DoNothingOptimalWorkCandidateCaiValue { get; set; }

        public string OptimalWorkCandidate { get; set; }
        public double OptimalNbiDeck { get; set; }
        public double OptimalNbiSup { get; set; }
        public double OptimalNbiSub { get; set; }
        public double OptimalNbiCulv { get; set; }
        public double OptimalWorkCandidateCaiValue { get; set; }
        public double EstimatedProjectCost { get; set; }
    }
}
