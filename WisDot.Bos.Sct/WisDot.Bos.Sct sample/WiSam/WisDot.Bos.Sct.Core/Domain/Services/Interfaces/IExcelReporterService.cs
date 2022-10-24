using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
//using WiSamEntities = WiSam.Entity;
using System.IO;
using Dw = Wisdot.Bos.Dw;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using ClosedXML.Excel;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;

namespace WisDot.Bos.Sct.Core.Domain.Services.Interfaces
{
    public interface IExcelReporterService
    {
        string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds);
        int GetFiscalYear(DateTime date);
        List<Dw.StructureMaintenanceItem> GetWisamsNeedsListNotInHsi(Dw.Database dwDatabase, List<Dw.StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws);
        List<Dw.StructureMaintenanceItem> CompareToWisamsNeedsList(string structureId, List<Dw.StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws);
        void WriteMaintenanceNeedsReport(List<Dw.StructureMaintenanceItem> maintenanceItems, DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false, string wisamsMaintenanceNeedsListExcelFile = "");
        void WriteMonitoringReport(DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false);
        List<WorkConcept> GetAssociatedSctWorkConcepts(string structureId, List<Project> projects);
        List<WorkConcept> GetAssociatedCertifiedWorkConcepts(string structureId, List<Project> projs);
        void WriteStructuresGisReport(string outputFilePath, List<string> structureIds, List<WorkConcept> fiipsWorkConcepts, List<Project> structuresProjects);
        //string WriteStructureCertificationHistory(Project project, List<WorkConcept> wcs, UserAccount account, DatabaseService database);
        string GetRandomExcelFileName(string baseDir);
        void WriteLoginHistoryReport(string outputFilePath, List<UserActivity> userActivities);

    }
}
