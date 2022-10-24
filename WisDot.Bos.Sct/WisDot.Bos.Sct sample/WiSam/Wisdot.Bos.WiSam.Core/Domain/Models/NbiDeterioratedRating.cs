using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class NbiDeterioratedRating
    {
        public int DeteriorationId { get; set; }
        public string NbiClassificationCode { get; set; }
        public string CustomClassificationCode { get; set; }
        public int Year { get; set; }
        public float RatingValue { get; set; }
    }
}
