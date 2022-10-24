using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class StructureCountyRank
    {
        public string StructureId { get; set; }
        public string County { get; set; }
        public double PriorityScore { get; set; }
        public int CountyRank { get; set; }
    }
}
