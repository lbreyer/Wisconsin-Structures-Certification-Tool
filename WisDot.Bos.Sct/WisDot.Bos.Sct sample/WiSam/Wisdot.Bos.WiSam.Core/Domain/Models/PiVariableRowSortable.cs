using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class PiVariableRowSortable
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public int Age { get; set; }
        public int DeckAge { get; set; }
        public double DoNothingCaiValue { get; set; }
        public string Primary { get; set; }
        public double PrimaryCai { get; set; }
        public double PrimaryCost { get; set; }
        public int PrimaryLifeExtension { get; set; }
        public string Incidentals { get; set; }
        public float PrimaryCostPerSqFtPerYear { get; set; }

        public string Fiips { get; set; }
        public double FiipsCai { get; set; }
        public double TotalCostWithoutDelivery { get; set; }
        public string FosProjectId { get; set; }
        public string ProjectConcept { get; set; }
        public string DotProgram { get; set; }
        public int FiipsLifeExtension { get; set; }
        public float FiipsCostPerSqFtPerYear { get; set; }

        public string WiSAMSWorkType { get; set; }
        public string RegionWorkType { get; set; }
        public string ComboWorkType { get; set; }
        public bool WorkTypeMatch { get; set; }

        public string DoNothingOptimal { get; set; }
        //public string NbiInterpolated { get; set; }
        public string NbiInspected { get; set; }
        public double NbiDeck { get; set; }
        public double NbiSup { get; set; }
        public double NbiSub { get; set; }
        public double NbiCulv { get; set; }

        public int WearingSurfaceElement { get; set; }
        public int WearingSurfaceCs1 { get; set; }
        public int WearingSurfaceCs2 { get; set; }
        public int WearingSurfaceCs3 { get; set; }
        public int WearingSurfaceCs4 { get; set; }
        public string Deck1080 { get; set; }
    }
}
