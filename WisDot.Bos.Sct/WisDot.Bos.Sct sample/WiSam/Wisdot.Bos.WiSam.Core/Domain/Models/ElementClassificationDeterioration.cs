using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class ElementClassificationDeterioration
    {
        public string ElementClassificationCode { get; set; }
        public float MedYr1 { get; set; }
        public float MedYr2 { get; set; }
        public float MedYr3 { get; set; }
        public float Beta { get; set; }
        public bool Active { get; set; }
    }
}
