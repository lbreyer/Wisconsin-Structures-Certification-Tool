using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class ExcelReporterService : IExcelReporterService
    {
        private static IExcelReporterRepository repo = new ExcelReporterRepository();
        private static IExcelReporterQuery query = new ExcelReporterQuery();

        public List<StructureMaintenanceItem> CompareToWisamsNeedsList(string structureId, List<StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws)
        {
            return repo.CompareToWisamsNeedsList(structureId, hsiMaintenanceItems, ws);
        }

        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            return repo.ConvertDegreesMinutesSecondsToDecimalDegrees(degreesMinutesSeconds);
        }

        public List<WorkConcept> GetAssociatedCertifiedWorkConcepts(string structureId, List<Project> projs)
        {
            return repo.GetAssociatedCertifiedWorkConcepts(structureId, projs);
        }

        public List<WorkConcept> GetAssociatedSctWorkConcepts(string structureId, List<Project> projects)
        {
            return repo.GetAssociatedSctWorkConcepts(structureId, projects);
        }

        public int GetFiscalYear(DateTime date)
        {
            return repo.GetFiscalYear(date);
        }

        public string GetRandomExcelFileName(string baseDir)
        {
            return repo.GetRandomExcelFileName(baseDir);
        }

        public List<StructureMaintenanceItem> GetWisamsNeedsListNotInHsi(Wisdot.Bos.Dw.Database dwDatabase, List<StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws)
        {
            return repo.GetWisamsNeedsListNotInHsi(dwDatabase, hsiMaintenanceItems, ws);
        }

        public void WriteLoginHistoryReport(string outputFilePath, List<UserActivity> userActivities)
        {
            query.WriteLoginHistoryReport(outputFilePath, userActivities);
        }

        public void WriteMaintenanceNeedsReport(List<StructureMaintenanceItem> maintenanceItems, DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false, string wisamsMaintenanceNeedsListExcelFile = "")
        {
            query.WriteMaintenanceNeedsReport(maintenanceItems, database, outputFilePath, fiipsProjects, fiips, structuresProjects, startFy, endFy, regions, includeState, includeLocal, wisamsMaintenanceNeedsListExcelFile);
        }

        public void WriteMonitoringReport(DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false)
        {
            query.WriteMonitoringReport(database, outputFilePath, fiipsProjects, fiips, structuresProjects, startFy, endFy, regions, includeState, includeLocal); 
        }

        public static string WriteStructureCertificationHistory(Project project, List<WorkConcept> wcs, UserAccount account, DatabaseService database)
        {
            return query.WriteStructureCertificationHistory(project, wcs, account, database);
        }

        public void WriteStructuresGisReport(string outputFilePath, List<string> structureIds, List<WorkConcept> fiipsWorkConcepts, List<Project> structuresProjects)
        {
            query.WriteStructuresGisReport(outputFilePath, structureIds, fiipsWorkConcepts, structuresProjects);
        }
    }
}
