using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class CoreInspection
    {
        public string StructureId { get; set; }
        public DateTime InspectionDate { get; set; }
        public NbiRating NbiRatings { get; set; }
        public List<CoreElement> Elements { get; set; }
    }
}
