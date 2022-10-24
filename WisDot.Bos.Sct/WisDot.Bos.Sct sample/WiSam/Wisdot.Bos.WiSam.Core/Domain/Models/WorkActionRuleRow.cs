using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class WorkActionRuleRow
    {
        public string RuleSequence { get; set; }
        public string RuleId { get; set; }
        public string RuleCategory { get; set; }
        public string RuleFormula { get; set; }
        public string ResultingWorkAction { get; set; }
        public string ComprisedWorkActions { get; set; }
        public string AlternativeWorkActions { get; set; }
        public string PotentialCombinedWorkActions { get; set; }
    }
}
