using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;

namespace WisDot.Bos.Sct.Core.Data.Interfaces
{
    public interface IExcelReporterQuery
    {
        void WriteMaintenanceNeedsReport(List<Dw.StructureMaintenanceItem> maintenanceItems, DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false, string wisamsMaintenanceNeedsListExcelFile = "");
        void WriteMonitoringReport(DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false);
        void WriteStructuresGisReport(string outputFilePath, List<string> structureIds, List<WorkConcept> fiipsWorkConcepts, List<Project> structuresProjects);
        string WriteStructureCertificationHistory(Project project, List<WorkConcept> wcs, UserAccount account, DatabaseService database);
        void WriteLoginHistoryReport(string outputFilePath, List<UserActivity> userActivities);

    }
}
