using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ClosedXML.Excel;
using ClosedXML.Utils;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Domain.Services.Interfaces;
using Wisdot.Bos.WiSam.Core.Infrastructure.Interfaces;
using Wisdot.Bos.WiSam.Core.Infrastructure;

namespace Wisdot.Bos.WiSam.Core.Domain.Services
{
    public class ExcelHelperService : IExcelHelperService

    {
        private XLWorkbook workBook;
        private string xlPath;
        private static IExcelHelperRepository repo = new ExcelHelperRepository();

        //private void UpdateTimesheetDataFile()
        public void UpdateFilePath(string newPath)
        {
            xlPath = newPath;
        }

        public ExcelHelperService(string sourceXlPath, int monthOfWeekEndingDate, int yearOfWeekEndingDate, List<Employee> emps, List<EmployeeTimesheet> timesheets)
        {
            //emps = new List<Employee>();
            //List<EmployeeTimesheet> timesheets = new List<EmployeeTimesheet>();
            try
            {
                workBook = new XLWorkbook(sourceXlPath);
                this.xlPath = sourceXlPath;
                var ws = workBook.Worksheet(yearOfWeekEndingDate + "-" + monthOfWeekEndingDate);

                // Look for the first row used
                var firstRowUsed = ws.FirstRowUsed();

                // Narrow down the row so that it only includes the used part
                var tsRow = firstRowUsed.RowUsed();

                // Move to the next row (data)
                tsRow = tsRow.RowBelow();

                // Loop through all rows
                while (!tsRow.Cell("A").IsEmpty())
                {
                    if (!Convert.ToString(tsRow.Cell("K").Value).ToUpper().Equals("TRUE"))
                    {
                        try
                        {
                            var rn = tsRow.RowNumber();
                            string firstName = Convert.ToString(tsRow.Cell("B").Value).ToUpper().Replace(" ", "");
                            string lastName = Convert.ToString(tsRow.Cell("C").Value).ToUpper().Replace(" ", "");
                            Employee emp = emps.Where(e => e.LastName.ToUpper().Equals(lastName) && e.FirstName.ToUpper().Equals(firstName)).First();
                            int employeeId = Convert.ToInt32(emp.EmployeeId);
                            //int employeeId = Convert.ToInt32(tsRow.Cell("M").Value);
                            string structureId = Convert.ToString(tsRow.Cell("D").Value);
                            string projectId = Convert.ToString(tsRow.Cell("E").Value);
                            string activityCodeDesc = Convert.ToString(tsRow.Cell("F").Value);
                            int activityCode = Convert.ToInt32(activityCodeDesc.Split(new string[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries).First());
                            //activityCode = Convert.ToInt32(tsRow.Cell("F").Value);
                            DateTime weekEndingDate = Convert.ToDateTime(tsRow.Cell("A").Value).AddDays(6);
                            int workNumber = -1;
                            //int workNumber = Convert.ToInt32(tsRow.Cell("O").Value);
                            float overTimeHours = 0;
                            try
                            {
                                overTimeHours = Convert.ToSingle(tsRow.Cell("H").Value);
                            }
                            catch { }
                            float totalHours = Convert.ToSingle(tsRow.Cell("G").Value) + overTimeHours;
                            int monthWeekEndingDate = weekEndingDate.Month;
                            int yearWeekEndingDate = weekEndingDate.Year;
                            EmployeeTimesheet ets = new EmployeeTimesheet();
                            ets.EmployeeId = employeeId;
                            ets.StructureId = structureId;
                            ets.ProjectId = projectId;
                            ets.ActivityCode = activityCode;
                            ets.WeekEndingDate = weekEndingDate;
                            ets.WorkNumber = workNumber;
                            ets.TotalHours = totalHours;
                            ets.MonthWeekEndingDate = monthWeekEndingDate;
                            ets.YearWeekEndingDate = yearWeekEndingDate;
                            ets.ExcelRowNumber = tsRow.RowNumber();
                            emp.Timesheets.Add(ets);
                            timesheets.Add(ets);
                        }
                        catch (Exception e)
                        {
                            var error = e.Message;
                        }
                    }

                    tsRow = tsRow.RowBelow();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(String.Format("{0}", ex.Message), "Error!");
            }

            var totalTimesheets = timesheets.Count;
        }

        public ExcelHelperService(string sourceXlPath, List<StructureProgramReview> structureProgramReview)
        {
            try
            {
                workBook = new XLWorkbook(sourceXlPath);
                this.xlPath = sourceXlPath;
                var ws = workBook.Worksheet(1); //Indexing starts @ 1, not 0
                var tsRow = ws.Row(4);

                while (!tsRow.Cell("A").IsEmpty())
                {
                    try
                    {
                        string certificationStatus = Convert.ToString(tsRow.Cell("W").Value).Trim().ToUpper();
                        string workAction = Convert.ToString(tsRow.Cell("U").Value).Trim().ToUpper();
                        string sfy = Convert.ToString(tsRow.Cell("T").Value).Trim();
                        int workActionYear = -1;
                        string advanceableSfy = Convert.ToString(tsRow.Cell("V").Value).Trim();
                        int advanceableWorkActionYear = -1;
                        string structureId = Convert.ToString(tsRow.Cell("A").Value).Trim().ToUpper();

                        /*
                        if (structureId.Equals("B400280"))
                        {
                            var stop = true;
                            string hval = Convert.ToString(tsRow.Cell("H").Value);
                        }*/

                        string fosProjectId = "";

                        try
                        {
                            fosProjectId = Convert.ToString(tsRow.Cell("H").Value).Trim();
                        }
                        catch (Exception e) { }

                        try
                        {
                            workActionYear = int.Parse(sfy);
                        }
                        catch { }

                        try
                        {
                            advanceableWorkActionYear = int.Parse(advanceableSfy);
                        }
                        catch { }

                        if (!String.IsNullOrEmpty(certificationStatus) && !String.IsNullOrEmpty(workAction) && workActionYear != -1)
                        {
                            // (03)OVERLAY DECK - CONCRETE
                            string[] w = workAction.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);

                            if (w.Length >= 2 && w[0].Length == 2)
                            {
                                StructureProgramReview spr = new StructureProgramReview();
                                if (workAction.Equals("NA"))
                                {
                                    spr.WorkActionCode = "00";
                                    spr.WorkActionDesc = "DO NOTHING";
                                }
                                else
                                {
                                    spr.WorkActionCode = w[0];
                                    spr.WorkActionDesc = workAction.Remove(0, 4);
                                }
                                spr.WorkActionYear = workActionYear;
                                spr.AdvanceableWorkActionYear = advanceableWorkActionYear;
                                spr.StructureId = structureId;

                                if (fosProjectId.Length == 8)
                                {
                                    spr.FosProjectId = fosProjectId;
                                }

                                if (spr.StructureId.StartsWith("B") || spr.StructureId.StartsWith("P"))
                                {
                                    spr.IsBridge = true;
                                }
                                else
                                {
                                    spr.IsBridge = false;
                                }

                                spr.CertificationStatus = certificationStatus;
                                spr.CertificationNotes = Convert.ToString(tsRow.Cell("Y").Value).Trim();
                                spr.DiscussionNotes = Convert.ToString(tsRow.Cell("S").Value).Trim();

                                if (certificationStatus.Equals("CERTIFIED"))
                                {
                                    spr.IsCertified = true;
                                }

                                string statusDate = Convert.ToString(tsRow.Cell("X").Value).Trim();
                                try
                                {
                                    DateTime sd = Convert.ToDateTime(statusDate);
                                    spr.StatusDate = sd;
                                }
                                catch { }

                                structureProgramReview.Add(spr);

                                // Check for additional work concepts 
                                certificationStatus = Convert.ToString(tsRow.Cell("AC").Value).Trim().ToUpper();
                                workAction = Convert.ToString(tsRow.Cell("AA").Value).Trim().ToUpper();
                                sfy = Convert.ToString(tsRow.Cell("Z").Value).Trim();
                                workActionYear = -1;
                                advanceableSfy = Convert.ToString(tsRow.Cell("AB").Value).Trim();
                                advanceableWorkActionYear = -1;

                                try
                                {
                                    workActionYear = int.Parse(sfy);
                                }
                                catch { }

                                try
                                {
                                    advanceableWorkActionYear = int.Parse(advanceableSfy);
                                }
                                catch { }

                                if (!String.IsNullOrEmpty(certificationStatus) && !String.IsNullOrEmpty(workAction) && workActionYear != -1)
                                {
                                    // (03)OVERLAY DECK - CONCRETE
                                    w = workAction.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);

                                    if (w.Length >= 2 && w[0].Length == 2)
                                    {
                                        StructureProgramReview spr2 = new StructureProgramReview();
                                        if (workAction.Equals("NA"))
                                        {
                                            spr2.WorkActionCode = "00";
                                            spr2.WorkActionDesc = "DO NOTHING";
                                        }
                                        else
                                        {
                                            spr2.WorkActionCode = w[0];
                                            spr2.WorkActionDesc = workAction.Remove(0, 4);
                                        }
                                        spr2.WorkActionYear = workActionYear;
                                        spr2.AdvanceableWorkActionYear = advanceableWorkActionYear;
                                        spr2.StructureId = structureId;

                                        if (spr2.StructureId.StartsWith("B") || spr2.StructureId.StartsWith("P"))
                                        {
                                            spr2.IsBridge = true;
                                        }
                                        else
                                        {
                                            spr2.IsBridge = false;
                                        }

                                        spr2.CertificationStatus = certificationStatus;
                                        spr2.CertificationNotes = Convert.ToString(tsRow.Cell("AE").Value).Trim();
                                        spr2.DiscussionNotes = Convert.ToString(tsRow.Cell("S").Value).Trim();

                                        if (certificationStatus.Equals("CERTIFIED"))
                                        {
                                            spr2.IsCertified = true;
                                        }

                                        statusDate = Convert.ToString(tsRow.Cell("AD").Value).Trim();
                                        try
                                        {
                                            DateTime sd = Convert.ToDateTime(statusDate);
                                            spr2.StatusDate = sd;
                                        }
                                        catch { }

                                        structureProgramReview.Add(spr2);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                    tsRow = tsRow.RowBelow();
                }
            }
            catch (Exception ex)
            { }
        }

        public ExcelHelperService(string xlPath, string command)
        {
            workBook = new XLWorkbook(xlPath);
            this.xlPath = xlPath;

            switch (command)
            {
                case "GetTimesheetTabs":

                    break;
            }
        }

        public ExcelHelperService(string xlPath)
        {
            workBook = new XLWorkbook();
            this.xlPath = xlPath;
            workBook.AddWorksheet("Sheet1");
            workBook.AddWorksheet("Sheet2");
            workBook.AddWorksheet("Sheet3");
            workBook.AddWorksheet("Sheet4");
            workBook.AddWorksheet("Sheet5");
            workBook.AddWorksheet("Sheet6");
            workBook.AddWorksheet("Sheet7");
            workBook.AddWorksheet("Sheet8");
            workBook.AddWorksheet("Sheet9");
            workBook.AddWorksheet("Sheet10");
            workBook.AddWorksheet("Sheet11");
            workBook.AddWorksheet("Sheet12");
        }

        public void UpdateTimesheetDataFile(int monthOfWeekEndingDate, int yearOfWeekEndingDate, List<int> importedRows)
        {
            repo.UpdateTimesheetDataFile(monthOfWeekEndingDate, yearOfWeekEndingDate, importedRows, workBook);
        }

        public List<string> GetStructuresOnHighClearanceRoutes()
        {
            return repo.GetStructuresOnHighClearanceRoutes(workBook);
        }

        public List<StructureLite> GetStructureCorridorCodes()
        {
            return repo.GetStructureCorridorCodes(workBook);
        }

        public List<ProgrammedWorkAction> GetImprovementWorkActionsFromExcel()
        {
            List<ProgrammedWorkAction> progWorkActions = new List<ProgrammedWorkAction>();

            return progWorkActions;
        }

        public List<ProgrammedWorkAction> GetProgrammedWorkActionsFromExcel()
        {
            return repo.GetProgrammedWorkActionsFromExcel(workBook);
        }

        public List<ElementDeterioration> GetElementDeteriorations()
        {
            return repo.GetElementDeteriorations(workBook);
        }

        public void SaveWorkbook()
        {
            workBook.SaveAs(xlPath);
        }

        public void CloseWorkbook()
        {
            workBook.Dispose();
        }


        /*
        class WorkActionRuleRow
    {
        public string RuleId { get; set; }
        public string RuleFormula { get; set; }
        public string RuleCategory { get; set; }
        public string ResultingWorkAction { get; set; }
        public string RuleSequence { get; set; }
        public string AlternativeWorkActions { get; set; }
        public string ComprisedWorkActions { get; set; }
        public string PotentialCombinedWorkActions { get; set; }
    }
        */

        public void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions, int firstYear, int lastYear, List<string> planningProjectConcepts = null, List<string> federalImprovementTypes = null)
        {
            repo.WriteAssetManagementReport(pwActions, firstYear, lastYear, workBook, planningProjectConcepts, federalImprovementTypes);

        }

        public void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions)
        {
            repo.WriteAssetManagementReport(pwActions, workBook);
        }

        public void WriteElementDeteriorationRatesReport(List<ElementDeterioration> elemDetRates)
        {
            repo.WriteElementDeteriorationRatesReport(elemDetRates, workBook);
        }



        public void WriteDesignBillableReport(List<Employee> emps, List<WorkActivity> workActivities, int startMonth, int endMonth, int startYear, int endYear, List<WorkActivity> distinctWorkActivities)
        {
            repo.WriteDesignBillableReport(emps, workActivities, startMonth, endMonth, startYear, endYear, distinctWorkActivities, workBook);
        }

        public void WriteStructureProgramReport(List<ProgrammedWorkAction> pwas, int startFY, int endFY, List<StructureWorkAction> workTypes, List<string> lifecycleStages)
        {
            repo.WriteStructureProgramReport(pwas, startFY, endFY, workTypes, lifecycleStages, workBook);
        }

        public void WriteBidItemsReport(List<BidItem> bidItems)
        {
            repo.WriteBidItemsReport(bidItems, workBook);
        }

        public void WriteRulesReport(List<WorkActionRule> workActionRules)
        {
            repo.WriteRulesReport(workActionRules, workBook);
        }

        public void WriteCoreDataReport(List<Structure> structures, List<string> notFoundIds)
        {
            repo.WriteCoreDataReport(structures, notFoundIds, workBook);
        }

        /*
        public List<StructureWorkAction> AllCurrentNeeds { get; set; }
        public List<StructureWorkAction> YearlyProgrammedWorkActions { get; set; }
        public List<StructureWorkAction> YearlyOptimalWorkActions { get; set; }
        public List<StructureWorkAction> YearlyConstrainedOptimalWorkActions { get; set; }
        public List<StructureWorkAction> YearlyDoNothings { get; set; }
        public List<StructureWorkAction> YearlyOptimalWorkActionsBasedOnDoNothingCondition { get; set; }
        public List<StructureWorkAction> YearlyWorstCaseScenario { get; set; }
        public List<StructureWorkAction> ConstructionHistoryProjects { get; set; }
        */
        public void WriteReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType = WisamType.NeedsAnalysisFileTypes.ProgramConstrained)
        {
            repo.WriteReport(structures, needsAnalysisInput, workBook, needsAnalysisFileType);
        }

        public void WriteStructuresDataForGisReport(List<Structure> structures)
        {
            repo.WriteStructuresDataForGisReport(structures, workBook);
        }

        public string GetMaintenanceItemPriority(string workActionCode)
        {
            return repo.GetMaintenanceItemPriority(workActionCode);
        }

        public void WriteAllCurrentNeedsReport(List<Structure> structures, List<string> notFoundIds, Database dbObj)
        {
            repo.WriteAllCurrentNeedsReport(structures, notFoundIds, dbObj, workBook);
        }

        public void WriteReport(WisamType.AnalysisReports report, List<Structure> structures, List<string> notFoundIds,
                                    List<StructureWorkAction> swas, int startYear, int endYear, DateTime startTime,
                                    bool debug = false, bool showPiFactors = false, string regionNumber = "",
                                    bool state = false, bool local = false, List<string> similarComboWorkActions = null)
        {
            repo.WriteReport(report, structures, notFoundIds, swas, startYear, endYear, startTime, workBook, debug, showPiFactors, regionNumber, state, local, similarComboWorkActions);

        }
    }
}
