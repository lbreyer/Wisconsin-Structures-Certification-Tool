using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class StatePmdssRow
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public double DoNothingCaiValue { get; set; }
        public string OptimalWorkCandidate { get; set; }
        public string OptimalWorkCandidateCaiValue { get; set; }
    }
}
