using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using WiSam.Business;

namespace WiSAM.DataPull
{
    public class Program
    {
        private const string logsDirectory = @"\\mad00fph\n4public\BOS\WiSAM\pmic-data-pull-logs\";
        private const string dailyReportsDirectory = @"\\mad00fph\n4public\BOS\WiSAM\dailyReports\";
        private static Interface iface;
        private const string workingDirectory = @"\\mad00fph\n4public\bos\fiips-code-refine-prod\";
        //private const string sourceExcelFile = "\\\\mad00fp1\\w4bhs\\Struct_Devel\\Bridge_Management_Unit\\1 - Asset Management\\Planning involvement\\current recommendations.xlsx";
        private const string sourceExcelFile = @"c:\temp2\current recommendations.xlsx";
        //private const string destinationDirExcelFile = "\\\\mad00fph\\n4public\\bos\\wisam\\fiips-query-tool\\strprogrev";
        private const string destinationDirExcelFile = @"c:\temp";

        //private const string sourceExcelFile = @"c:\temp\current recommendations.xlsx";
        //private const string destinationDirExcelFile = @"c:\temp\rev";

        static void Main(string[] args)
        {
            DateTime dateTimeStart = DateTime.Now;
            string logFileName = Path.Combine(logsDirectory, String.Format("log-{0:MM-dd-yyyy}.txt", dateTimeStart));
            string logText = String.Format("Start: {0}\r\n", dateTimeStart);

            try
            {
                iface = new Interface();
                //iface.UpdateDbConnections(WisamType.Databases.WiSamTest);

                //if (!iface.PullPmicData()) // At least one table doesn't have records
                {
                    //File.WriteAllText(Path.Combine(workingDirectory, "alert.txt"), DateTime.Now.ToString());
                }
                //else
                {
                    // Delete structure projects in WiSAMS
                    //iface.DeletePmicStructureProjects();

                    // Update structure projects
                    //iface.UpdateWiSamsWithPmicStructure();

                    // Update roadway projects
                    //iface.UpdateWisamsWithPmicRoadway();

                    // Delete "alert" file
                    //File.Delete(Path.Combine(workingDirectory, "alert.txt"));

                    iface.UpdateStructureProgramReview(sourceExcelFile, destinationDirExcelFile);
                }
            }
            catch (Exception e)
            {
                logText += String.Format("Error: {0}\r\n", e.Message.ToString());
            }

            logText += String.Format("End: {0}", DateTime.Now);
            File.WriteAllText(logFileName, logText);
        }
    }
}
