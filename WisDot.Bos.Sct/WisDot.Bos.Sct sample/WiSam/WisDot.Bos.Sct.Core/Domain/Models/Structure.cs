using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class Structure
    {
        public string StructureId { get; set; }
        public GeoLocation GeoLocation { get; set; }
        public string Region { get; set; }
        public string RegionNumber { get; set; }
        public string County { get; set; }
        public string Municipality { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }
    }
}
