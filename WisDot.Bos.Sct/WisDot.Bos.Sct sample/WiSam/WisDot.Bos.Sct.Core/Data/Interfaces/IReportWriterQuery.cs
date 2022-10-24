using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;

namespace WisDot.Bos.Sct.Core.Data.Interfaces
{
    public interface IReportWriterQuery
    {
        bool MergePdfs(List<string> fileNames, string targetPdf);
        void CreateStructureCd(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject, bool openFile = false, WorkConcept certificationWorkConcept = null);
        string CreateProjectCd(DatabaseService database, Project project, List<Structure> structures, DateTime dateTime, Project fiipsProject);
        void CreatePdfFile(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath);
        void CreateXpdfFile(string xfdfFilePath, string newPdfFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject);
        void CreateCdPdfFile(string xfdfFilePath, string pdfFilePath, string newPdfFilePath, string signatureFilePath);
        void CreateCdXpdfFile(string xfdfFilePath, string newPdfFilePath, Project project, Structure structure, WorkConcept workConcept, DateTime dateTime, Project fiipsProject, WorkConcept certificationWorkConcept);
    }
}
