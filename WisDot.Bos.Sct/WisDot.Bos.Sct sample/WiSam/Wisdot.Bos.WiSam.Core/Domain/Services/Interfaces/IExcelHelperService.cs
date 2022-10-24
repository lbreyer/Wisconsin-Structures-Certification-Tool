using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.WiSam.Core.Domain.Models;

namespace Wisdot.Bos.WiSam.Core.Domain.Services.Interfaces
{
    public interface IExcelHelperService
    {
        void UpdateTimesheetDataFile(int monthOfWeekEndingDate, int yearOfWeekEndingDate, List<int> importedRows);
        List<string> GetStructuresOnHighClearanceRoutes();
        List<StructureLite> GetStructureCorridorCodes();
        List<ProgrammedWorkAction> GetImprovementWorkActionsFromExcel();
        List<ProgrammedWorkAction> GetProgrammedWorkActionsFromExcel();
        List<ElementDeterioration> GetElementDeteriorations();
        void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions, int firstYear, int lastYear, List<string> planningProjectConcepts = null, List<string> federalImprovementTypes = null);
        void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions);
        void WriteElementDeteriorationRatesReport(List<ElementDeterioration> elemDetRates);
        void WriteDesignBillableReport(List<Employee> emps, List<WorkActivity> workActivities, int startMonth, int endMonth, int startYear, int endYear, List<WorkActivity> distinctWorkActivities);
        void WriteStructureProgramReport(List<ProgrammedWorkAction> pwas, int startFY, int endFY, List<StructureWorkAction> workTypes, List<string> lifecycleStages);
        void WriteBidItemsReport(List<BidItem> bidItems);
        void WriteRulesReport(List<WorkActionRule> workActionRules);
        void WriteCoreDataReport(List<Structure> structures, List<string> notFoundIds);
        void WriteReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType = WisamType.NeedsAnalysisFileTypes.ProgramConstrained);
        void WriteStructuresDataForGisReport(List<Structure> structures);
        string GetMaintenanceItemPriority(string workActionCode);
        void WriteAllCurrentNeedsReport(List<Structure> structures, List<string> notFoundIds, Database dbObj);
        void WriteReport(WisamType.AnalysisReports report, List<Structure> structures, List<string> notFoundIds,
                                    List<StructureWorkAction> swas, int startYear, int endYear, DateTime startTime,
                                    bool debug = false, bool showPiFactors = false, string regionNumber = "",
                                    bool state = false, bool local = false, List<string> similarComboWorkActions = null);

    }
}
