using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class WorkAction
    {
        public string WorkActionCode { get; set; }
        public string WorkActionDesc { get; set; }
        public string Unit { get; set; }
        public double UnitCost { get; set; }
        public double MinUnitCost { get; set; }
        public double MaxUnitCost { get; set; }
        public string UnitCostFormula { get; set; }
        public bool UseUnitCostFormula { get; set; }
        public string WorkActionNotes { get; set; }
        public string CostFormula { get; set; }
        public string CostNotes { get; set; }
        public int EarlierFy { get; set; }
        public int LaterFy { get; set; }
    }
}
