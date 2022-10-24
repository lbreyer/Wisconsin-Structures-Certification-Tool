using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class MetaRowSortable
    {
        public string StructureId { get; set; }
        public string CorridorCode { get; set; }
        public string Region { get; set; }
        public string County { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
        public string StructureType { get; set; }
        public string MainSpanMaterial { get; set; }
        public int NumSpans { get; set; }
        public double TotalLengthSpans { get; set; }
        public string InventoryRating { get; set; }
        public string OperatingRating { get; set; }
        public string LoadPosting { get; set; }
        public DateTime LastInspectionDate { get; set; }
        public string ConstructionHistory { get; set; }
        public int WorkActionYear { get; set; }
        public int Age { get; set; }
        public int DeckAge { get; set; }

        public double DoNothingCaiValue { get; set; }
        public double DoNothingNbiDeck { get; set; }
        public double DoNothingNbiSup { get; set; }
        public double DoNothingNbiSub { get; set; }
        public double DoNothingNbiCulv { get; set; }

        public string Primary { get; set; }
        public double PrimaryCai { get; set; }
        public double NbiDeck { get; set; }
        public double NbiSup { get; set; }
        public double NbiSub { get; set; }
        public double NbiCulv { get; set; }
        public double PrimaryCost { get; set; }
        public int PrimaryLifeExtension { get; set; }

        public string Fiips { get; set; }
        public double FiipsCai { get; set; }
        public double FiipsNbiDeck { get; set; }
        public double FiipsNbiSup { get; set; }
        public double FiipsNbiSub { get; set; }
        public double FiipsNbiCulv { get; set; }
        public double TotalCostWithoutDelivery { get; set; }
        public string FosProjectId { get; set; }
        public string ProjectConcept { get; set; }
        public string DotProgram { get; set; }
    }
}
