using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class AllCurrentNeedsRow
    {
        public string StructureId { get; set; }
        public string StructureType { get; set; }
        public string LastInspectionYear { get; set; }
        public string CurrentCai { get; set; }
        public string CurrentNeeds { get; set; }
    }
}
