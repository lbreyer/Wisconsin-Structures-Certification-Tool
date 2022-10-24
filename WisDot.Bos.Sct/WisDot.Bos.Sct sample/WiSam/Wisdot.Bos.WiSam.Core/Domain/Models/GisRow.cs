using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class GisRow
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
    }
}
