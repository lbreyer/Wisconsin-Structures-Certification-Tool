using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Runtime.InteropServices;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Infrastructure;
using System.Configuration;
using WisDot.Bos.Sct.Core.Data.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class ReportWriterService
    {
        //private const string dir = @"\\mad00fph\n4public\bos\sctware\sct20\";
        private static string bosCdPdfFilePath = ConfigurationManager.AppSettings["bosCdPdfFilePath"].ToString();
        private static string signatureFilePath = ConfigurationManager.AppSettings["signatureFilePath"].ToString();
        private static string tempDir = ConfigurationManager.AppSettings["tempDir"].ToString();

        private static IReportWriterQuery query = new ReportWriterQuery();

        public static bool MergePdfs(List<string> fileNames, string targetPdf)
        {
            return query.MergePdfs(fileNames, targetPdf);
        }

        /*
        public static void CreateStructureCd(Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject)
        {
            string xfdfFilePath = Path.Combine(dir, String.Format("{0}-{1}.xfdf", structure.StructureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
            string pdfFilePath = Path.Combine(dir, "boscd.pdf");
            string newPdfFilePath = Path.Combine(dir, String.Format("{0}-{1}.pdf", structure.StructureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
            //string signatureFilePath = Path.Combine(dir, "billdreher.jpg");
        }*/

        public static void CreateStructureCd(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject, bool openFile = false, WorkConcept certificationWorkConcept = null)
        {
            query.CreateStructureCd(xfdfFilePath, pdfFilePath, newPdfFilePath, signatureFilePath, project, structure, workConcept, dateTime, fiipsProject, openFile, certificationWorkConcept);
        }

        public static string CreateProjectCd(DatabaseService database, Project project, List<Structure> structures, DateTime dateTime, Project fiipsProject)
        {
            return query.CreateProjectCd(database, project, structures, dateTime, fiipsProject);
        }

        // For previous version of BOSCD
        private static void CreatePdfFile(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath)
        {
            query.CreatePdfFile(xfdfFilePath, pdfFilePath, newPdfFilePath, signatureFilePath);
        }

        // For previous version of BOSCD
        private static void CreateXpdfFile(string xfdfFilePath, string newPdfFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject)
        {
            query.CreateXpdfFile(xfdfFilePath, newPdfFilePath, project, structure, workConcept, dateTime, fiipsProject);
        }

        // For current version of BOSCD
        private static void CreateCdPdfFile(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath)
        {
            query.CreateCdPdfFile(xfdfFilePath, pdfFilePath, newPdfFilePath, signatureFilePath);
        }

        // For current version of BOSCD
        private static void CreateCdXpdfFile(string xfdfFilePath, string newPdfFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject, WorkConcept certificationWorkConcept)
        {
            query.CreateCdXpdfFile(xfdfFilePath, newPdfFilePath, project, structure, workConcept, dateTime, fiipsProject, certificationWorkConcept);
        }

    }
}
