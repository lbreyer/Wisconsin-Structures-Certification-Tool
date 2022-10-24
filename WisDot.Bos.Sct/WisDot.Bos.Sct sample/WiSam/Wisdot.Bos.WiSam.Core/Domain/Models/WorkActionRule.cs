using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class WorkActionRule
    {
        public int RuleId { get; set; }
        public string RuleFormula { get; set; }
        public string RuleCategory { get; set; }
        public string ResultingWorkActionCode { get; set; }
        public WorkAction ResultingWorkAction { get; set; }
        public int RuleSequence { get; set; }
        public bool Active { get; set; }
        public string RuleNotes { get; set; }
        public string RuleWorkActionNotes { get; set; }
        public List<WorkAction> AlternativeWorkActions { get; set; }
        public List<WorkAction> ComprisedWorkActions { get; set; }
        public List<WorkAction> PotentialCombinedWorkActions { get; set; }

        public WorkActionRule()
        {
            AlternativeWorkActions = new List<WorkAction>();
            ComprisedWorkActions = new List<WorkAction>();
            PotentialCombinedWorkActions = new List<WorkAction>();
        }
    }
}
