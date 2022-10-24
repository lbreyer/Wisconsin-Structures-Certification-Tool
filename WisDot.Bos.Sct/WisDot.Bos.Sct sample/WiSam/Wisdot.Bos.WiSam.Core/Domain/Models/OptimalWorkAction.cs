using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class OptimalWorkAction : WorkAction
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        //public float Cai { get; set; }
        public Cai CAI { get; set; }
        public double Cost { get; set; }
    }
}
