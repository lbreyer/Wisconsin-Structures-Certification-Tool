using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class StateNeedsRow
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public string OptimalWorkCandidate { get; set; }
        public double OptimalWorkCandidateCaiValue { get; set; }
        public double EstimatedProjectCost { get; set; }
    }
}
