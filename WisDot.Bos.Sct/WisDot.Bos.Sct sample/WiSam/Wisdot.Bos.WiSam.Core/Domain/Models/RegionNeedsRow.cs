using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class RegionNeedsRow
    {
        //public string InventoryData { get; set; }
        //public string StructureId { get; set; }
        //public string StructureType { get; set; }
        public int WorkActionYear { get; set; }
        public int Age { get; set; }
        public double DoNothingCaiValue { get; set; }

        public string Primary { get; set; }
        public double PrimaryCai { get; set; }
        public double PrimaryCost { get; set; }
        public int LifeExtension { get; set; }
        public string Incidentals { get; set; }

        //public int PrimaryLastYear { get; set; }
        //public string MoreSevereNextPrimary { get; set; }
        //public int MoreSevereOccurrence { get; set; }

        public string Fiips { get; set; }
        public double FiipsCai { get; set; }
        public double TotalCostWithoutDelivery { get; set; }
        public string FosProjectId { get; set; }
        public string ProjectConcept { get; set; }
        public string DotProgram { get; set; }
        //public double FiipsCost { get; set; }
        //public string PerformedCriteria { get; set; }
    }
}
