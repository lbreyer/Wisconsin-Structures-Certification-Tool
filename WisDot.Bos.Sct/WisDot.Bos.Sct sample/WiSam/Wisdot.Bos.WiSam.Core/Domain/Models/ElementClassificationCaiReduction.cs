using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class ElementClassificationCaiReduction
    {
        public string ElementClassificationCode { get; set; }
        public int CaiFormulaId { get; set; }
        public string ReductionFormula { get; set; }
    }
}
