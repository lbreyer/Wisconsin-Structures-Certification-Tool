using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;
using Dw = Wisdot.Bos.Dw;

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IExcelReporterRepository
    {
        string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds);
        int GetFiscalYear(DateTime date);
        List<Dw.StructureMaintenanceItem> GetWisamsNeedsListNotInHsi(Dw.Database dwDatabase, List<Dw.StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws);
        List<Dw.StructureMaintenanceItem> CompareToWisamsNeedsList(string structureId, List<Dw.StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws);
        List<WorkConcept> GetAssociatedSctWorkConcepts(string structureId, List<Project> projects);
        List<WorkConcept> GetAssociatedCertifiedWorkConcepts(string structureId, List<Project> projs);
        string GetRandomExcelFileName(string baseDir);

    }
}
