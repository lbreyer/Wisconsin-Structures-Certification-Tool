using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public static class Constants
    {
        public static string NbiCulvertDeteriorationCurve = @"-0.0004*x*x - 0.0169*x + 8";
        public static string NbiBridgeDeteriorationCurve = @"-0.0009*x*x - 0.0251*x + 8";
        public static DateTime StateFiscalYearStartDate = new DateTime(DateTime.Now.Year, 7, 1);
        public static DateTime StateFiscalYearEndDate = new DateTime(DateTime.Now.Year + 1, 6, 30);
        public static DateTime FederalFiscalYearStateDate = new DateTime(DateTime.Now.Year, 11, 1);
        public static DateTime FederalFiscalYearEndDate = new DateTime(DateTime.Now.Year + 1, 10, 31);
    }
}
