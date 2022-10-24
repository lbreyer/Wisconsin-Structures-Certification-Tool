using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class MeasurementIndex
    {
        public string MeasurementCode { get; set; }
        public string GrossValue { get; set; }
        public bool GrossExpression { get; set; }
        public string IndexValue { get; set; }
        public bool IndexExpression { get; set; }
    }
}
