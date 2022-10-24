using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class GeoLocation
    {
        public string HsiLatitude { get; set; }
        public string HsiLongitude { get; set; }
        public float LatitudeDecimal { get; set; }
        public float LongitudeDecimal { get; set; }
        public string RegionCode { get; set; }
        public int RegionNumber { get; set; }
        public string Region { get; set; }

        public GeoLocation()
        {
            Initialize();
        }

        public void Initialize()
        {
            HsiLatitude = "";
            HsiLongitude = "";
            LatitudeDecimal = 0;
            LongitudeDecimal = 0;
            RegionCode = "";
            RegionNumber = 0;
            Region = "";
        }
    }
}
