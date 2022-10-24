using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class Risk
    {
        public WisamType.Risks RiskId { get; set; }
        public string RiskName { get; set; }
        public string RiskDesc { get; set; }
        public float RiskValue { get; set; }
        public int RiskMaxValue { get; set; }
        public string RiskNotes { get; set; }
        public bool Active { get; set; }

        public Risk(WisamType.Risks riskId)
        {
            RiskId = riskId;
        }
    }
}
