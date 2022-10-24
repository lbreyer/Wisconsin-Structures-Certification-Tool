using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class NeedsAnalysisRowSortable
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public string Primary { get; set; }
        public double PrimaryCai { get; set; }
        public double PrimaryCost { get; set; }
        public float PriorityScore { get; set; }
        public int PriorityScoreRank { get; set; }
        public string PriorityScoreRelativeRank { get; set; }
        public string Incidentals { get; set; }
        public string Fiips { get; set; }
        //public string DebugInfo { get; set; }
    }
}
