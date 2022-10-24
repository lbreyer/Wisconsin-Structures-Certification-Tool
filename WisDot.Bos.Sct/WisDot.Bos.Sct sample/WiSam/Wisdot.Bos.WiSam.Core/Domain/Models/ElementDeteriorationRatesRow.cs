using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class ElementDeteriorationRatesRow
    {
        public int ElemNum { get; set; }
        public string ElemName { get; set; }
        public float MedYr1 { get; set; }
        public float MedYr2 { get; set; }
        public float MedYr3 { get; set; }
        public float Beta { get; set; }
        public float RelativeWeight { get; set; }
    }
}
