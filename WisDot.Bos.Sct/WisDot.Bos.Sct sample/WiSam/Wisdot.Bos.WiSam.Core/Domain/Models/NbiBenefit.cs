using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class NbiBenefit
    {
        public string WorkActionCode { get; set; }
        public string NbiClassificationCode { get; set; }
        public int Benefit { get; set; }
        public bool IncludesOverlay { get; set; }
        public float AddedBenefit { get; set; }
        public int NbiMaximumValue { get; set; }
    }
}
