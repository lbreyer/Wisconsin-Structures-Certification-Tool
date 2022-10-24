using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class PriorityFactorMeasurement
    {
        public string FactorCode { get; set; }
        public double FactorWeight { get; set; }
        public string MeasurementCode { get; set; }
        public string MeasurementName { get; set; }
        public double Weight { get; set; }
        public bool Active { get; set; }
        public string GrossValue { get; set; }
        public double GrossValueNum { get; set; }
        public string GrossValueFormula { get; set; }
        public bool CalcIndexValue { get; set; }
        //public string IndexFormula { get; set; }
        public double IndexValue { get; set; }
        public double FinalValue { get; set; }

        public PriorityFactorMeasurement() 
        {
            GrossValueNum = 0;
        }

        public PriorityFactorMeasurement(string factorCode)
        {
            FactorCode = factorCode;
            GrossValueNum = 0;
        }
    }
}
