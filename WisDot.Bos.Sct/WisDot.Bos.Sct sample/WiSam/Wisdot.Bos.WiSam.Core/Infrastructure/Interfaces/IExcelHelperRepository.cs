using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.WiSam.Core.Domain.Models;

namespace Wisdot.Bos.WiSam.Core.Infrastructure.Interfaces
{
    public interface IExcelHelperRepository
    {
        void UpdateTimesheetDataFile(int monthOfWeekEndingDate, int yearOfWeekEndingDate, List<int> importedRows, XLWorkbook workBook);
        List<string> GetStructuresOnHighClearanceRoutes(XLWorkbook workBook);
        List<StructureLite> GetStructureCorridorCodes(XLWorkbook workBook);
        List<ProgrammedWorkAction> GetProgrammedWorkActionsFromExcel(XLWorkbook workBook);
        List<ElementDeterioration> GetElementDeteriorations(XLWorkbook workBook);
        void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions, int firstYear, int lastYear, XLWorkbook workBook, List<string> planningProjectConcepts = null, List<string> federalImprovementTypes = null);
        void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions, XLWorkbook workBook);
        void WriteElementDeteriorationRatesReport(List<ElementDeterioration> elemDetRates, XLWorkbook workBook);
        void WriteDesignBillableReport(List<Employee> emps, List<WorkActivity> workActivities, int startMonth, int endMonth, int startYear, int endYear, List<WorkActivity> distinctWorkActivities, XLWorkbook workBook);
        void WriteStructureProgramReport(List<ProgrammedWorkAction> pwas, int startFY, int endFY, List<StructureWorkAction> workTypes, List<string> lifecycleStages, XLWorkbook workBook);
        void WriteBidItemsReport(List<BidItem> bidItems, XLWorkbook workBook);
        void WriteRulesReport(List<WorkActionRule> workActionRules, XLWorkbook workBook);
        void WriteCoreDataReport(List<Structure> structures, List<string> notFoundIds, XLWorkbook workBook);
        void WriteReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, XLWorkbook workBook, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType = WisamType.NeedsAnalysisFileTypes.ProgramConstrained);
        void WriteStructuresDataForGisReport(List<Structure> structures, XLWorkbook workBook);
        string GetMaintenanceItemPriority(string workActionCode);
        void WriteAllCurrentNeedsReport(List<Structure> structures, List<string> notFoundIds, Database dbObj, XLWorkbook workBook);
        void WriteReport(WisamType.AnalysisReports report, List<Structure> structures, List<string> notFoundIds,
                                    List<StructureWorkAction> swas, int startYear, int endYear, DateTime startTime, XLWorkbook workBook,
                                    bool debug = false, bool showPiFactors = false, string regionNumber = "",
                                    bool state = false, bool local = false, List<string> similarComboWorkActions = null);

    }
}
