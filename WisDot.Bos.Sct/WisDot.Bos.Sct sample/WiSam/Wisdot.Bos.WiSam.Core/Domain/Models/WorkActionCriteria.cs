using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class WorkActionCriteria
    {
        public int RuleId { get; set; }
        public string RuleFormula { get; set; }
        public string RuleCategory { get; set; }
        public string WorkActionCode { get; set; }
        public string WorkActionDesc { get; set; }
        public int RuleSequence { get; set; }
        public bool Active { get; set; }
        public string RuleNotes { get; set; }
        public string AlternativeWorkActionCode { get; set; }
        public string AlternativeWorkActionDesc { get; set; }
    }
}
