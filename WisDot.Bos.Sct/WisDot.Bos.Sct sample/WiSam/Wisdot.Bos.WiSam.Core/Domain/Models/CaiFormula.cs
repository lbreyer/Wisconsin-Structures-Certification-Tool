using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class CaiFormula
    { 
        public int CaiFormulaId { get; set; }
        public string CaiFormulaDesc { get; set; }
        public string Formula { get; set; }
        public bool Active { get; set; }
        public bool ElementBasedOnly { get; set; }
        public bool DefaultFormula { get; set; }

        // Element classifications (e.g., bearings, joints, overlays, paint) that reduce the CAI value of given CAI formula
        public List<ElementClassificationCaiReduction> ElementClassCaiReductions; 

        // NBI classifications (e.g., deck, super, sub, culvert) that reduce the CAI value of given CAI formula
        public List<NbiClassificationCaiReduction> NbiCaiReductions;
    }
}
