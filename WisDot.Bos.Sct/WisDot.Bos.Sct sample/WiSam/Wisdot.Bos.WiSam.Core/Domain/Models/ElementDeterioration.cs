using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class ElementDeterioration
    {
        public int ElemNum { get; set; }
        public string ElemName { get; set; }
        public float MedYr1 { get; set; }
        public float MedYr2 { get; set; }
        public float MedYr3 { get; set; }
        public float Beta { get; set; }
        public float RelativeWeight { get; set; }
        public float ScalingFactor1 { get; set; }
        public float ScalingFactor2 { get; set; }
        public float ScalingFactor3 { get; set; }
        public bool Active { get; set; }
    }
}
