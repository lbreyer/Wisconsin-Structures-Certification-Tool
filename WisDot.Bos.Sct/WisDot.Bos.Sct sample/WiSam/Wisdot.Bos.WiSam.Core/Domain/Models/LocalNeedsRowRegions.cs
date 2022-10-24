using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class LocalNeedsRowRegions
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
        public double PrimaryCost { get; set; }
        public double PriorityScore { get; set; }
        public int CountyRank { get; set; }

        public LocalNeedsRowRegions(LocalNeedsRowWebPosting source)
        {
            this.StructureId = source.StructureId;
            this.Region = source.Region;
            this.FeatureOn = source.FeatureOn;
            this.FeatureUnder = source.FeatureUnder;
            this.Muni = source.Muni;
            this.County = source.County;
            this.Owner = source.Owner;
            this.DeckArea = source.DeckArea;
            this.Sufficiency = source.Sufficiency;
            this.Deficiency = source.Deficiency;
            this.LocalFundingEligibility = source.LocalFundingEligibility;
            this.Primary = source.Primary;
            this.PaintWork = source.PaintWork;
            this.RailWork = source.RailWork;
        }
    }
}
