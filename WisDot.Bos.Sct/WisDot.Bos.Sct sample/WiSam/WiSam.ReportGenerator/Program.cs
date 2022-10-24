using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WiSam.Business;

namespace WiSam.ReportGenerator
{
    public class Program
    {
        private static Interface iface;
        private const string workingDirectory = @"\\mad00fph\n4public\Bos\struct_devel\StrProgReport\let-dates";
        private const string filePath = @"\\mad00fph\n4public\Bos\struct_devel\StrProgReport\let-dates\let-dates.csv";

        static void Main(string[] args)
        {
            string errorFile = String.Format("{0}error-{1:MMddyyyy}.txt", @"\\mad00fph\n4public\Bos\struct_devel\StrProgReport\let-dates\", DateTime.Now);

            try
            {
                iface = new Interface();
                iface.CreateLetDatesReport(filePath, 2010, DateTime.Now.Year + 32);
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = new StreamWriter(errorFile))
                {
                    sw.WriteLine(ex.Message);
                }
            }
        }
    }
}
