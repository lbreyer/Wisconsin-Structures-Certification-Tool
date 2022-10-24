using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class StructureDeterioration
    {
        public string StructureId { get; set; }
        public string FunctionalClassificationOn { get; set; }
        public string MainSpanMaterial { get; set; }
        public string StructureType { get; set; }
        public string FeatureUnder { get; set; }
        public string NbiDeckQualifiedDeteriorationCurve { get; set; }
        public string NbiSuperQualifiedDeteriorationCurve { get; set; }
        public string NbiSubQualifiedDeteriorationCurve { get; set; }

        public StructureDeterioration()
        {
            StructureId = "";
            FunctionalClassificationOn = "";
            MainSpanMaterial = "";
            StructureType = "";
            FeatureUnder = "";
            NbiDeckQualifiedDeteriorationCurve = "";
            NbiSuperQualifiedDeteriorationCurve = "";
            NbiSubQualifiedDeteriorationCurve = "";
        }
    }
}
