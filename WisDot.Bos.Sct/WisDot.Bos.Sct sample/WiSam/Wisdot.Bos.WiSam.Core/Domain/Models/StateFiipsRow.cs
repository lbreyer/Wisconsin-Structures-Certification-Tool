using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class StateFiipsRow
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public string ProgrammedWorkCandidate { get; set; }
        public double ProgrammedWorkCandidateCaiValue { get; set; }
    }
}
