using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class LocalNeedsRowWebPosting
    {
        public string StructureId { get; set; }
        public string Region { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
        public string Muni { get; set; }
        public string County { get; set; }
        public string Owner { get; set; }
        public double DeckArea { get; set; }
        public double Sufficiency { get; set; }
        public string Deficiency { get; set; }
        public string LocalFundingEligibility { get; set; }
        public string Primary { get; set; }
        public string PaintWork { get; set; }
        public string RailWork { get; set; }
    }
}
