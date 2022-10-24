using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Domain.Services;

namespace WiSam.Business
{
    public class Interface
    {
        private static Database dbObj = null;
        private static CaiFormula caiFormulaObj;
        private static List<WorkActionCriteria> workActionCriteria;
        private static CaiFormula defaultCaiFormula;
        private static List<StructureWorkAction> workActionsAll;
        private static List<StructureWorkAction> workActionsPrimary;
        private List<PriorityFactor> priorityFactors;
        private List<PriorityFactorMeasurement> priorityFactorMeasurements;
        private List<MeasurementIndex> measurementIndices;
        private List<string> planningProjectConcepts;
        private List<string> federalImprovementTypes;
        public readonly string TimesheetDataFileConnectionString = ConfigurationManager.ConnectionStrings["TimesheetDataFile"].ConnectionString;
        public readonly string TimesheetAccessDatabaseConnectionString = ConfigurationManager.ConnectionStrings["StructuresProgressReportAccess"].ConnectionString;
        private static string timesheetAccessDatabaseFilePath = "";
        private static List<PriorityIndexCategory> priorityIndexCategories;
        private static List<PriorityIndexFactor> priorityIndexFactors;
        private static StringBuilder expressionToEvaluate = new StringBuilder();
        private static List<ElementDeterioration> elementDeteriorations;

        #region Constructors
        public Interface()
        {
            dbObj = new Database();
            elementDeteriorations = dbObj.GetElementDeteriorationRates();
            priorityIndexCategories = dbObj.GetPriorityIndexCategories();
            priorityIndexFactors = dbObj.GetPriorityIndexFactors();
            workActionCriteria = dbObj.GetWorkActionCriteria();
            defaultCaiFormula = dbObj.GetDefaultCaiFormula();
            workActionsAll = dbObj.GetStructureWorkActions();
            workActionsPrimary = dbObj.GetStructureWorkActions(true);
            priorityFactors = dbObj.GetPriorityFactors();
            priorityFactorMeasurements = dbObj.GetPriorityFactorMeasurements(priorityFactors);
            measurementIndices = dbObj.GetMeasurementIndices();
            timesheetAccessDatabaseFilePath =
                TimesheetAccessDatabaseConnectionString
                .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];

            planningProjectConcepts = new List<string>();
            planningProjectConcepts.Add("BRIDGE ELIMINATION");
            planningProjectConcepts.Add("BRIDGE REHABILITATION");
            planningProjectConcepts.Add("BRIDGE REHABILITATION (SHRM)");
            planningProjectConcepts.Add("BRIDGE REPLACEMENT, EXPANSION");
            planningProjectConcepts.Add("BRIDGE REPLACEMENT, PRESERVATION");
            planningProjectConcepts.Add("MISCELLANEOUS");
            planningProjectConcepts.Add("NEW BRIDGE");
            planningProjectConcepts.Add("PAVEMENT REPLACEMENT");
            planningProjectConcepts.Add("RECONDITIONING");
            planningProjectConcepts.Add("RECONSTRUCTION, EXPANSION");
            planningProjectConcepts.Add("RECONSTRUCTION, PRESERVATION");
            planningProjectConcepts.Add("RESURFACING");
            planningProjectConcepts.Add("ROADWAY MAINTENANCE, PRESERVATION");

            federalImprovementTypes = new List<string>();
            federalImprovementTypes.Add("BRIDGE PROTECTION");
            federalImprovementTypes.Add("BRIDGE REHABILITATION");
            federalImprovementTypes.Add("BRIDGE REHABILITATION - NO ADDED CAPACITY");
            federalImprovementTypes.Add("BRIDGE REPLACEMENT");
            federalImprovementTypes.Add("BRIDGE REPLACEMENT - NO ADDED CAPACITY");
            federalImprovementTypes.Add("NEW BRIDGE CONSTRUCTION");
            federalImprovementTypes.Add("RECONSTRUCTION WITHOUT ADDED CAPACITY");
            federalImprovementTypes.Add("SPECIAL BRIDGE");
        }

        public Interface(WisamType.Databases hsi, WisamType.Databases wiSam)
        {
            dbObj = new Database(hsi, wiSam);
            workActionCriteria = dbObj.GetWorkActionCriteria();
            defaultCaiFormula = dbObj.GetDefaultCaiFormula();
            workActionsAll = dbObj.GetStructureWorkActions();
            workActionsPrimary = dbObj.GetStructureWorkActions(true);
        }
        #endregion Constructors

        #region UI Methods
        public List<string> GetQualifiedDeteriorationCurves()
        {
            return dbObj.GetQualifiedDeteriorationCurves();
        }

        public bool UpdateTimesheetDbConnection(string newTimesheetAccessDatabaseFilePath)
        {
            bool updatedTimesheetDbConnection = dbObj.UpdateTimesheetDbConnection(newTimesheetAccessDatabaseFilePath);

            return updatedTimesheetDbConnection;
        }

        public string GetTimesheetAccessDatabaseFilePath()
        {
            return timesheetAccessDatabaseFilePath;
        }

        public List<StructureWorkAction> GetWorkActions()
        {
            return workActionsAll;
        }

        public List<StructureWorkAction> GetProgrammableWorkActions()
        {
            return workActionsPrimary;
        }

        public List<string> GetRuleCategories()
        {
            return dbObj.GetRuleCategories();
        }

        public List<WorkActionRule> GetWorkActionRules()
        {

            return dbObj.GetWorkActionRules();
        }

        public void UpdateDbConnections(WisamType.Databases wiSam)
        {
            bool changeDbConnection = dbObj.UpdateDbConnections(wiSam);

            if (changeDbConnection)
            {
                workActionCriteria = dbObj.GetWorkActionCriteria();
                defaultCaiFormula = dbObj.GetDefaultCaiFormula();
                workActionsAll = dbObj.GetStructureWorkActions();
                workActionsPrimary = dbObj.GetStructureWorkActions(true);
                priorityFactors = dbObj.GetPriorityFactors();
                priorityFactorMeasurements = dbObj.GetPriorityFactorMeasurements(priorityFactors);
                measurementIndices = dbObj.GetMeasurementIndices();
            }
        }

        public List<string> GetFiipsStructureIds()
        {
            return dbObj.GetFiipsStructureIds();
        }

        public void UpdateNbiDeteriorationRates()
        {
            dbObj.CalculateNbiDeteriorationRates();
        }

        public void CreateAssetManagementReport(string outputFilePath, int firstYear, int lastYear)
        {
            string planningProjectConceptCodes = @"
                                                    'BRELIM',
                                                    'BRRHB',
                                                    'BRRPL',
                                                    'BRRPLE',
                                                    'BRSHRM',
                                                    'MISC',
                                                    'PVRPLA',
                                                    'RDMTN',
                                                    'RECOND',
                                                    'RECST',
                                                    'RECSTE',
                                                    'RESURF'
                                                ";

            string fedImprovementTypeCodes = @"
                                                '48',
                                                '13',
                                                '14',
                                                '10',
                                                '11',
                                                '08',
                                                '40'
                                              ";



            DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", firstYear));
            DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", lastYear));
            List<ProgrammedWorkAction> pwActions = dbObj.GetProgrammedWorkActions(planningProjectConceptCodes,
                                                                                    fedImprovementTypeCodes,
                                                                                    startDate,
                                                                                    endDate);
            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteAssetManagementReport(pwActions, firstYear, lastYear, planningProjectConcepts, federalImprovementTypes);
            eh.SaveWorkbook();
        }

        public void GetStructuresDataForGis(string outputFilePath)
        {
            List<string> structureIds = dbObj.GetStructuresByRegion("1", true, false, true);
            List<Structure> structures = new List<Structure>();
            int counter = 0;

            foreach (var structureId in structureIds)
            {
                structures.Add(dbObj.GetStructure(structureId));
                counter++;

                if (counter == 10)
                {
                    break;
                }
            }

            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteStructuresDataForGisReport(structures);
            eh.SaveWorkbook();
        }

        public void CreateElementDeteriorationRatesTable(string outputFilePath)
        {
            List<ElementDeterioration> elementDetRates = dbObj.GetElementDeteriorationRates();
            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteElementDeteriorationRatesReport(elementDetRates);
            eh.SaveWorkbook();
        }

        public string ImportTimesheetData(string timesheetDataFilePath, int monthOfWeekEndingDate, int yearOfWeekEndingDate)
        {
            string results = "";
            List<Employee> emps = dbObj.GetEmployees();
            List<EmployeeTimesheet> timesheets = new List<EmployeeTimesheet>();
            ExcelHelperService eh = new ExcelHelperService(timesheetDataFilePath, monthOfWeekEndingDate, yearOfWeekEndingDate, emps, timesheets);
            results += String.Format("Number of Records to Import from Timesheet Data File: {0}\r\n", timesheets.Count);
            int successfullyImported = 0;

            if (timesheets.Count > 0)
            {
                List<int> importedRows = new List<int>();
                foreach (var emp in emps)
                {
                    foreach (var ts in emp.Timesheets)
                    {
                        var structureId = ts.StructureId;
                        var projectId = ts.ProjectId;
                        var workNumber = ts.WorkNumber;

                        if (!String.IsNullOrEmpty(structureId) && !String.IsNullOrEmpty(projectId))
                        {
                            workNumber = dbObj.GetWorkNumber(structureId, projectId);
                        }
                        else if (String.IsNullOrEmpty(structureId) && !String.IsNullOrEmpty(projectId))
                        {
                            workNumber = 99;
                            structureId = "SPECIAL";
                        }
                        else if (String.IsNullOrEmpty(structureId) && String.IsNullOrEmpty(projectId))
                        {
                            workNumber = 99;
                            structureId = "SPECIAL";
                            projectId = "0000-00-00";
                        }
                        else
                        {
                            var shouldnotbehere = "";
                        }

                        if (workNumber != -1)
                        {
                            ts.StructureId = structureId;
                            ts.ProjectId = projectId;
                            ts.WorkNumber = workNumber;
                            bool successful = true;

                            if (dbObj.DoesTimesheetExist(ts))
                            {
                                // Update query doesn't work in Access database; puzzled; thus, delete existing record and insert new record
                                //successful = dbObj.UpdateTimesheet(ts); 
                                dbObj.DeleteTimesheet(ts);
                                dbObj.InsertTimesheet(ts);
                            }
                            else
                            {
                                successful = dbObj.InsertTimesheet(ts);
                            }

                            if (successful)
                            {
                                importedRows.Add(ts.ExcelRowNumber);
                                successfullyImported++;
                            }
                        }
                    }
                }

                if (importedRows.Count > 0)
                {
                    // Not updating because it corrupts the Excel (.xlsm) file
                    //eh.UpdateTimesheetDataFile(monthOfWeekEndingDate, yearOfWeekEndingDate, importedRows);
                    //eh.SaveWorkbook();
                    //eh.CloseWorkbook();
                }
            }

            results += String.Format("Successfully Imported: {0}\r\n", successfullyImported);
            return results;
        }

        public void CreateDesignBillableReport(string outputFilePath, int startMonth, int endMonth, int startYear, int endYear)
        {
            List<Employee> employeesAndTimesheets = dbObj.GetEmployeesAndTimesheets(startMonth, endMonth, startYear, endYear);
            List<WorkActivity> workActivities = dbObj.GetWorkActivities(startMonth, endMonth, startYear, endYear);
            List<WorkActivity> distinctWorkActivities = dbObj.GetWorkActivities();

            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteDesignBillableReport(employeesAndTimesheets, workActivities, startMonth, endMonth, startYear, endYear, distinctWorkActivities);
            eh.SaveWorkbook();
            eh.CloseWorkbook();
        }

        public void CreateBidItemsReport(string outputFilePath, int startLetYear, int endLetYear)
        {
            DateTime startLetDate = Convert.ToDateTime(String.Format("01-01-{0}", startLetYear));
            DateTime endLetDate = Convert.ToDateTime(String.Format("12-31-{0}", endLetYear));
            List<BidItem> bidItems = dbObj.GetStructureBidItems(startLetDate, endLetDate);
            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteBidItemsReport(bidItems);
            eh.SaveWorkbook();
        }

        public void CreateLetDatesReport(string outputFilePath, int startLetYear, int endLetYear)
        {
            DateTime startLetDate = Convert.ToDateTime(String.Format("07-01-{0}", startLetYear - 1));
            DateTime endLetDate = Convert.ToDateTime(String.Format("06-30-{0}", endLetYear));
            List<ProgrammedWorkAction> pwas =
                dbObj.GetProgrammedWorkActionsStructure(WisamType.PmicProjectTypes.AnyProgrammed, startLetDate, endLetDate);

            foreach (var pwa in pwas.Where(pwa => pwa.StructureId.Length > 0))
            {
                try
                {
                    StructureLite strLite = dbObj.GetStructureLite(pwa.StructureId);

                    try
                    {
                        pwa.FeatureOn = strLite.FeatureOn;
                    }
                    catch { }

                    try
                    {
                        pwa.FeatureUnder = strLite.FeatureUnder;
                    }
                    catch { }
                }
                catch { }
            }

            ReportWriterService.WriteLetDatesReport(pwas, outputFilePath);
        }

        public void CreateStructureProgramReport(string outputFilePath, int startLetYear, int endLetYear)
        {
            DateTime startLetDate = Convert.ToDateTime(String.Format("07-01-{0}", startLetYear - 1));
            DateTime endLetDate = Convert.ToDateTime(String.Format("06-30-{0}", endLetYear));
            List<ProgrammedWorkAction> pwas = dbObj.GetProgrammedWorkActionsStructure(WisamType.PmicProjectTypes.AnyProgrammed, startLetDate, endLetDate);
            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            List<string> lifecycleStages = new List<string>()
                {
                    "00;Unprogrammed Projects", "10;Init Prog Estimate",
                    "11;Prog Level Scoping", "12;PMP Approved",
                    "15;Design Study Report", "20;PSE Submittal",
                    "40;Award Estimate", "50;Final Cost"
                };
            eh.WriteStructureProgramReport(pwas, startLetYear, endLetYear, workActionsPrimary, lifecycleStages);
            eh.SaveWorkbook();
        }

        public void CreateRulesTable(string outputFilePath)
        {
            List<WorkActionRule> workActionRules = dbObj.GetWorkActionRules();
            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteRulesReport(workActionRules);
            eh.SaveWorkbook();
        }

        public void CreateFiipsBridgeListFile(string outputFilePath)
        {
            List<string> strIds = dbObj.GetFiipsStructureIds();
            int counter = 0;
            string ids = String.Join(",", strIds);
            StreamWriter writer = new StreamWriter(outputFilePath, true);
            writer.Write(ids);
            writer.Close();
        }

        public void UpdateOverlaysCombinedWorkActions()
        {
            dbObj.InsertOverlaysCombinedWorkActions();
        }

        public void UpdateHighClearanceRoutes(string inputFilePath)
        {
            ExcelHelperService eh = new ExcelHelperService(inputFilePath, "read");
            List<string> strIds = eh.GetStructuresOnHighClearanceRoutes();

            foreach (string strId in strIds)
            {
                dbObj.UpdateHighClearanceRoute(strId);
            }
        }

        public void UpdateStructureCorridorCodes(string inputFilePath)
        {
            ExcelHelperService eh = new ExcelHelperService(inputFilePath, "read");
            List<StructureLite> strLites = eh.GetStructureCorridorCodes();

            foreach (StructureLite strLite in strLites)
            {
                if (dbObj.GetStructureCorridorCode(strLite.StructureId) != null) // structure exists already, so just update corridor code
                {
                    dbObj.UpdateStructureCorridorCode(strLite);
                }
                else // 
                {
                    dbObj.InsertStructureCorridorCode(strLite);
                }
            }
        }

        public bool PullPmicData()
        {
            if (!dbObj.PullPmicData())
            {
                return false;
            }

            return true;
        }

        public void UpdateWisamsWithPmicRoadway()
        {
            DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", DateTime.Now.Year));
            DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", DateTime.Now.Year + 5));
            //List<ProgrammedWorkAction> pWAs = dbObj.GetProgrammedWorkActions(WisamType.PmicProjectTypes.PredictiveAndPlanned, startDate, endDate);
            //dbObj.UpdateWiSamsWithPmic(WisamType.PmicProjectTypes.AnyProgrammed, startDate, endDate);
            dbObj.UpdateWiSamsWithImprovement(startDate, endDate);
        }

        public void DeletePmicStructureProjects()
        {
            dbObj.DeletePmicRows();
        }

        public void UpdateStructureProgramReview(string sourceExcelFile, string destinationDirExcelFile)
        {
            // Copy and archive working Excel file
            string dateToday = DateTime.Now.ToString("MMddyyyy");
            string copiedFile = Path.Combine(destinationDirExcelFile, Path.GetFileName(sourceExcelFile));
            File.Copy(sourceExcelFile, copiedFile, true);
            string archivedFile = Path.Combine(destinationDirExcelFile, Path.GetFileNameWithoutExtension(sourceExcelFile) + "-" + dateToday + Path.GetExtension(sourceExcelFile));
            File.Copy(sourceExcelFile, archivedFile, true);

            List<StructureProgramReview> structureProgramReview = new List<StructureProgramReview>();
            ExcelHelperService eh = new ExcelHelperService(copiedFile, structureProgramReview);
            string year = DateTime.Today.Year.ToString();
            string month = DateTime.Today.Month < 10 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
            string day = DateTime.Today.Day < 10 ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
            int dateStamp = Convert.ToInt32(year + month + day);
            string currentStructureId = "";
            string previousStructureId = "";
            StructureLite currentStructureLite = new StructureLite();

            if (structureProgramReview.Count > 0)
            {
                dbObj.DeleteStructureProgramReviewCurrent();
            }

            var str = structureProgramReview.Where(s => s.StructureId.Equals("B400280"));

            foreach (var spr in structureProgramReview)
            {
                currentStructureId = spr.StructureId;

                //if (currentStructureId.Equals("B400280"))
                {
                    if (!currentStructureId.Equals(previousStructureId))
                    {
                        currentStructureLite = dbObj.GetStructureLite(currentStructureId);
                    }

                    spr.Region = currentStructureLite.Region;
                    spr.RegionNumber = currentStructureLite.RegionNumber;
                    spr.County = currentStructureLite.County;
                    spr.CountyNumber = currentStructureLite.CountyNumber;
                    spr.FundingEligibilityCsv = currentStructureLite.FundingEligibilityCsv;
                    dbObj.CompareToFiips(spr);
                    dbObj.InsertStructureProgramReview(dateStamp, spr);
                }

                previousStructureId = currentStructureId;
            }

            eh.CloseWorkbook();
        }

        public void UpdateIsBridge()
        {
            dbObj.UpdateIsBridge();
        }

        public void UpdateWorkDuplicates()
        {
            dbObj.UpdateWorkDuplicates();
        }

        public void UpdateWiSamsWithPmicStructure()
        {
            DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", DateTime.Now.Year - 15));
            DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", DateTime.Now.Year + 50));
            //List<ProgrammedWorkAction> pWAs = dbObj.GetProgrammedWorkActions(WisamType.PmicProjectTypes.PredictiveAndPlanned, startDate, endDate);
            dbObj.UpdateWiSamsWithPmicStructure(WisamType.PmicProjectTypes.AnyProgrammed, startDate, endDate);
            //dbObj.UpdateWiSamsWithImprovement(startDate, endDate);
        }

        public void UpdateImprovement(string inputFilePath)
        {
            // Read Excel input file
            ExcelHelperService eh = new ExcelHelperService(inputFilePath, "read");
            List<ProgrammedWorkAction> pWAs = eh.GetImprovementWorkActionsFromExcel();

            foreach (ProgrammedWorkAction pWA in pWAs)
            {

            }
        }

        public void UpdatePmic(string inputFilePath)
        {
            // Read Excel input file
            ExcelHelperService eh = new ExcelHelperService(inputFilePath, "read");
            List<ProgrammedWorkAction> pWAs = eh.GetProgrammedWorkActionsFromExcel();
            string year = DateTime.Today.Year.ToString();
            string month = DateTime.Today.Month < 10 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
            string day = DateTime.Today.Day < 10 ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
            int dateStamp = Convert.ToInt32(year + month + day);

            if (pWAs.Count > 0)
            {
                dbObj.DeletePmicRows();
            }

            foreach (ProgrammedWorkAction pWA in pWAs)
            {
                dbObj.GetProgrammedWorkAction(pWA);
                dbObj.InsertPmicRow(pWA, dateStamp);
            }
        }

        public void UpdatePonModDeter(string inputFilePath)
        {
            // Read Excel input file
            ExcelHelperService eh = new ExcelHelperService(inputFilePath, "read");
            List<ElementDeterioration> elemDets = eh.GetElementDeteriorations();

            foreach (ElementDeterioration elemDet in elemDets)
            {
                dbObj.InsertElementDeteriorationRate(elemDet);
            }
        }

        public void DeleteWorkActionCriteria(int workActionRuleId, int ruleSequence)
        {
            dbObj.DeleteWorkActionCriteria(workActionRuleId, ruleSequence);
        }

        public void UpdateWorkActionCriteria(WorkActionRule workActionRule, int oldRuleSequence)
        {
            dbObj.UpdateWorkActionCriteria(workActionRule, oldRuleSequence);
        }

        public void GenerateLocalProgramNeedsReport(int startYear, int endYear, int caiFormulaId, string outputFilePath, bool deteriorateDefects,
            bool improvementProgramWorkActions = false, bool debug = false, bool showPiFactors = false,
            bool interpolateNbi = false, bool countTpo = false)
        {
            List<Structure> structures = dbObj.GetLocalStructuresFundingEligible(interpolateNbi, countTpo, getLastInspection: true);
            List<string> bridgesMissingInspection = new List<string>();
            caiFormulaObj = dbObj.GetCaiFormula(caiFormulaId);
            int counter = 0;
            foreach (Structure structure in structures)
            {
                //if (counter == 5)
                //break;

                if (structure.LastInspection != null)
                {
                    try
                    {
                        AnalyzeStructure(structure, startYear, endYear, caiFormulaId, outputFilePath, deteriorateDefects, improvementProgramWorkActions, debug);
                    }
                    catch
                    {
                        bridgesMissingInspection.Add(structure.StructureId);
                    }
                    counter++;
                }
                else
                {
                    bridgesMissingInspection.Add(structure.StructureId);
                }
            }

            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteReport(WisamType.AnalysisReports.LocalNeeds, structures, bridgesMissingInspection, workActionsPrimary, startYear, endYear, DateTime.Now);
            eh.SaveWorkbook();
        }

        public Structure GenerateCurrentUnconstrainedMaintenanceNeeds(string structureId, int currentYear, int caiFormulaId, CaiFormula caiFormulaObj, List<WorkActionCriteria> workActionCriteria, string workActionCodes)
        {
            Structure structure = dbObj.GetStructure(structureId, false, false, false, false, currentYear, currentYear);

            if (structure != null && structure.LastInspection != null)
            {
                try
                {
                    GetStructureAllCurrentNeeds(structure, caiFormulaId, workActionCriteria);
                }
                catch { }
            }

            return structure;
        }

        public void GenerateStateUnconstrainedMaintenanceNeedsReport(string outputFilePath, int caiFormulaId)
        {
            /*
            List<string> structureIds = dbObj.GetStructuresByRegion("1", true, false, false);
            structureIds.AddRange(dbObj.GetStructuresByRegion("2", true, false, false));
            structureIds.AddRange(dbObj.GetStructuresByRegion("3", true, false, false));
            structureIds.AddRange(dbObj.GetStructuresByRegion("4", true, false, false));
            structureIds.AddRange(dbObj.GetStructuresByRegion("5", true, false, false));*/
            List<StructureMaintenanceItem> maintenanceItems = dbObj.GetStructureMaintenanceItems();
            List<string> structureIds = maintenanceItems.GroupBy(item => item.StructureId).Select(g => g.First().StructureId).ToList();
            List<Structure> structures = new List<Structure>();
            List<string> bridgesMissingInspection = new List<string>();
            caiFormulaObj = dbObj.GetCaiFormula(caiFormulaId);
            int currentYear = 2021;
            int counter = 0;
            string workActionCodes = "77,04,10,12,14,28,29,35,49,66,72,75,79,94";
            List<WorkActionCriteria> workActionCriteria = dbObj.GetWorkActionCriteria(false, workActionCodes);

            foreach (var structureId in structureIds)
            {
                //if (counter > 9)
                //break;

                Structure structure = dbObj.GetStructure(structureId, false, false, false, false, currentYear, currentYear);

                if (structure != null && structure.LastInspection != null)
                {
                    try
                    {
                        GetStructureAllCurrentNeeds(structure, caiFormulaId, workActionCriteria);
                        structures.Add(structure);
                    }
                    catch
                    {
                        bridgesMissingInspection.Add(structureId);
                    }
                    /*
                    try
                    {
                        AnalyzeStructure(structure, startYear, endYear, caiFormulaId, outputFilePath, deteriorateDefects, improvementProgramWorkActions, debug);
                    }
                    catch
                    {
                        bridgesMissingInspection.Add(structure.StructureId);
                    }*/
                    counter++;
                }
                else
                {
                    bridgesMissingInspection.Add(structureId);
                }
            }

            ExcelHelperService eh = new ExcelHelperService(outputFilePath);
            eh.WriteAllCurrentNeedsReport(structures, bridgesMissingInspection, dbObj);
            //eh.WriteReport(WisamType.AnalysisReports.LocalNeeds, structures, bridgesMissingInspection, workActionsPrimary, startYear, endYear, DateTime.Now);
            eh.SaveWorkbook();
        }

        public void GenerateAnalysisReport(WisamType.AnalysisReports report, string regionNumbers, bool stateOwned, bool localOwned,
            int startYear, int endYear, int caiFormulaId, string outputFilePath, bool deteriorateDefects, DateTime startTime,
            bool improvementProgramWorkActions = true, bool debug = false, bool showPiFactors = false, string workActionCodes = "",
            bool interpolateNbi = false, bool includeCStructures = false, bool countTpo = false, string debugFilePath = "")
        {
            List<Structure> structures = new List<Structure>();
            List<string> notFoundIds = new List<string>();
            caiFormulaObj = dbObj.GetCaiFormula(caiFormulaId);
            int strCounter = 0;
            int notFoundStrCounter = 0;
            List<string> strIds = dbObj.GetStructuresByRegions(regionNumbers.Split(new char[] { ',' }).ToList(), stateOwned, localOwned, includeCStructures);
            //List<string> strIds = dbObj.GetStructuresByRegion(regionNumber, stateOwned, localOwned, includeCStructures);
            List<WorkActionCriteria> workActionCriteria = null;
            List<StructureWorkAction> workActions = new List<StructureWorkAction>();
            List<string> similarComboWorkActions = dbObj.GetSimilarComboWorkActions();

            if (!String.IsNullOrEmpty(workActionCodes))
            {
                workActionCriteria = dbObj.GetWorkActionCriteria(true, workActionCodes);
                foreach (var workActionCode in workActionCodes.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    workActions.Add(dbObj.GetStructureWorkAction(workActionCode));
                }
            }
            else
            {
                workActionCriteria = dbObj.GetWorkActionCriteria(true);
                workActions = workActionsPrimary;
            }

            Progress progressDialog = new Progress(strIds.Count());
            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    // Process
                    int i = 1;
                    foreach (var strId in strIds)
                    {
                        if (dbObj.GetElementInspectionDates(strId).Count > 0)
                        {
                            Structure str = null;

                            try
                            {
                                str = AnalyzeStructure(report, strId, startYear, endYear, deteriorateDefects, caiFormulaId, improvementProgramWorkActions, workActionCriteria, interpolateNbi, countTpo);
                            }
                            catch (Exception e)
                            {
                                notFoundIds.Add(strId + ": " + e.StackTrace);
                                notFoundStrCounter++;
                            }

                            if (str != null)
                            {
                                structures.Add(str);
                                strCounter++;
                            }
                            else
                            {
                                notFoundIds.Add(strId);
                                notFoundStrCounter++;
                            }
                        }
                        else
                        {
                            notFoundIds.Add(strId + ": no element inspection");
                            notFoundStrCounter++;
                        }

                        progressDialog.UpdateProgress(i, strId, strIds.Count());
                        i++;

                        if (i > 5)
                            break;
                    }

                    progressDialog.UpdateProgress(-1, "Writing report...", strIds.Count());

                    // Write Excel report
                    ExcelHelperService eh = new ExcelHelperService(outputFilePath);
                    eh.WriteReport(report, structures, notFoundIds, workActions, startYear, endYear, startTime, debug, showPiFactors,
                                    "", false, false, similarComboWorkActions);
                    eh.SaveWorkbook();

                    if (!String.IsNullOrEmpty(debugFilePath))
                    {
                        ReportWriterService.WritePriorityScoreReport(report, structures, startYear, endYear, debugFilePath, priorityIndexCategories, priorityIndexFactors);
                    }

                    progressDialog.UpdateProgress(-2, "Completed analysis...", strIds.Count());
                    Thread.Sleep(2000);
                    progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                }
            ));

            backgroundThread.Start();
            progressDialog.ShowDialog();

            /*
            foreach (var strId in strIds)
            {
               // if (counter > 4)
                    //break;

                if (dbObj.GetElementInspectionDates(strId).Count > 0)
                {
                    Structure str = null;

                    try
                    {
                        str = AnalyzeStructure(report, strId, startYear, endYear, deteriorateDefects, caiFormulaId, improvementProgramWorkActions, workActionCriteria);
                    }
                    catch (Exception e)
                    {
                        notFoundIds.Add(strId + ": " + e.StackTrace);
                        notFoundStrCounter++;
                    }

                    if (str != null)
                    {
                        structures.Add(str);
                        strCounter++;
                    }
                    else
                    {
                        //notFoundIds.Add(strId);
                        //notFoundStrCounter++;
                    }
                }
                else
                {
                    notFoundIds.Add(strId + ": no element inspection");
                    notFoundStrCounter++;
                }

                strCounter++;
            }
            */


        }

        public List<DateTime> GetElementInspectionDates(string strId)
        {
            return dbObj.GetElementInspectionDates(strId);
        }

        public void GenerateCoreDataReport(string strIdList, string outputFilePath, bool countTpo = false)
        {
            List<Structure> structures = new List<Structure>();
            List<string> strIds = strIdList.Split(new string[] { ",", " ", ";", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int strCounter = 0;
            int notFoundStrCounter = 0;
            List<string> notFoundIds = new List<string>();
            Progress progressDialog = new Progress(strIds.Count());

            Thread backgroundThread = new Thread(
               new ThreadStart(() =>
               {
                   // Process
                   int i = 1;
                   foreach (var strId in strIds)
                   {
                       //progressDialog.UpdateProgress(-1, strId);

                       if (dbObj.GetElementInspectionDates(strId).Count > 0)
                       {
                           Structure str = null;

                           try
                           {
                               //str = AnalyzeStructure(report, strId, startYear, endYear, deteriorateDefects, caiFormulaId, improvementProgramWorkActions, workActionCriteria, interpolateNbi);
                               str = dbObj.GetStructure(strId, true, true, true, countTpo);
                           }
                           catch (Exception e)
                           {
                               notFoundIds.Add(strId + ": " + e.StackTrace);
                               notFoundStrCounter++;
                           }

                           if (str != null)
                           {
                               structures.Add(str);
                               strCounter++;
                           }
                           else
                           {
                               notFoundIds.Add(strId);
                               notFoundStrCounter++;
                           }
                       }
                       else
                       {
                           notFoundIds.Add(strId + ": no element inspection");
                           notFoundStrCounter++;
                       }

                       progressDialog.UpdateProgress(i, strId, strIds.Count());

                       i++;
                   }

                   progressDialog.UpdateProgress(-1, "Writing report...", strIds.Count());

                   // Write Excel report
                   /*
                   ExcelHelperService eh = new ExcelHelperService(outputFilePath);
                   eh.WriteCoreDataReport(structures, notFoundIds);
                   eh.SaveWorkbook();
                    */
                   ReportWriterService.WriteCoreDataReport(outputFilePath, structures, notFoundIds);

                   progressDialog.UpdateProgress(-2, "Completed analysis...", strIds.Count());
                   Thread.Sleep(2000);

                   progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));

               }
           ));

            backgroundThread.Start();
            progressDialog.ShowDialog();
        }

        public void GenerateCoreDataReport(List<string> regions, string outputFilePath, bool stateOwned, bool localOwned, bool includeCStructures, bool countTpo = false)
        {
            List<Structure> structures = new List<Structure>();
            List<string> notFoundIds = new List<string>();
            int strCounter = 0;
            int notFoundStrCounter = 0;
            List<string> strIds = dbObj.GetStructuresByRegions(regions, stateOwned, localOwned, includeCStructures);
            Progress progressDialog = new Progress(strIds.Count());

            Thread backgroundThread = new Thread(
               new ThreadStart(() =>
               {
                   // Process
                   int i = 1;
                   foreach (var strId in strIds)
                   {
                       //progressDialog.UpdateProgress(-1, strId);

                       if (dbObj.GetElementInspectionDates(strId).Count > 0)
                       {
                           Structure str = null;

                           try
                           {
                               //str = AnalyzeStructure(report, strId, startYear, endYear, deteriorateDefects, caiFormulaId, improvementProgramWorkActions, workActionCriteria, interpolateNbi);
                               str = dbObj.GetStructure(strId, true, true, true, countTpo);
                           }
                           catch (Exception e)
                           {
                               notFoundIds.Add(strId + ": " + e.StackTrace);
                               notFoundStrCounter++;
                           }

                           if (str != null)
                           {
                               structures.Add(str);
                               strCounter++;
                           }
                           else
                           {
                               notFoundIds.Add(strId);
                               notFoundStrCounter++;
                           }
                       }
                       else
                       {
                           notFoundIds.Add(strId + ": no element inspection");
                           notFoundStrCounter++;
                       }

                       progressDialog.UpdateProgress(i, strId, strIds.Count());
                       i++;

                       /*
                       if (i == 20)
                       {
                           break;
                       }
                       */
                   }

                   progressDialog.UpdateProgress(-1, "Writing report...", strIds.Count());

                   // Write Excel report
                   /*
                   ExcelHelperService eh = new ExcelHelperService(outputFilePath);
                   eh.WriteCoreDataReport(structures, notFoundIds);
                   eh.SaveWorkbook();
                   */
                   ReportWriterService.WriteCoreDataReport(outputFilePath, structures, notFoundIds);

                   progressDialog.UpdateProgress(-2, "Completed analysis...", strIds.Count());
                   Thread.Sleep(2000);

                   progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));

               }
           ));

            backgroundThread.Start();
            progressDialog.ShowDialog();
        }

        public List<PriorityIndexCategory> GetPriorityIndexCategories()
        {
            return dbObj.GetPriorityIndexCategories();
        }

        public void GenerateAnalysisReport(
            WisamType.AnalysisReports report, string strIdList, int startYear, int endYear, int caiFormulaId,
            string outputFilePath, bool deteriorateDefects, DateTime startTime, bool improvementProgramWorkActions = true, bool debug = false,
            bool showPiFactors = false, string workActionCodes = "", bool interpolateNbi = false, bool countTpo = false, string debugFilePath = "",
             List<Budget> budget = null, string constrainedOutputFilePath = "")
        {
            List<Structure> structures = new List<Structure>();
            List<string> strIds = strIdList.Split(new string[] { ",", " ", ";", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            caiFormulaObj = dbObj.GetCaiFormula(caiFormulaId);
            int strCounter = 0;
            int notFoundStrCounter = 0;
            int fiipsCounter = 0;
            List<string> notFoundIds = new List<string>();
            List<WorkActionCriteria> workActionCriteria = null;
            List<StructureWorkAction> workActions = new List<StructureWorkAction>();
            List<string> similarComboWorkActions = dbObj.GetSimilarComboWorkActions();

            if (!String.IsNullOrEmpty(workActionCodes))
            {
                workActionCriteria = dbObj.GetWorkActionCriteria(true, workActionCodes);
                foreach (var workActionCode in workActionCodes.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    workActions.Add(dbObj.GetStructureWorkAction(workActionCode));
                }
            }
            else
            {
                workActionCriteria = dbObj.GetWorkActionCriteria(true);
                workActions = workActionsPrimary;
            }

            if (report.Equals(WisamType.AnalysisReports.FiipsAnalysisDebug))
            {
                strIds = dbObj.GetFiipsStructureIds();
            }

            Progress progressDialog = new Progress(strIds.Count());

            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    // Process
                    int i = 1;
                    foreach (var strId in strIds)
                    {
                        //progressDialog.UpdateProgress(-1, strId);

                        if (dbObj.GetElementInspectionDates(strId).Count > 0)
                        {
                            Structure str = null;

                            try
                            {
                                str = AnalyzeStructure(report, strId, startYear, endYear, deteriorateDefects, caiFormulaId, improvementProgramWorkActions, workActionCriteria, interpolateNbi, countTpo);
                            }
                            catch (Exception e)
                            {
                                notFoundIds.Add(strId + ": " + e.StackTrace);
                                notFoundStrCounter++;
                            }

                            if (str != null)
                            {
                                structures.Add(str);
                                strCounter++;
                            }
                            else
                            {
                                notFoundIds.Add(strId);
                                notFoundStrCounter++;
                            }
                        }
                        else
                        {
                            notFoundIds.Add(strId + ": no element inspection");
                            notFoundStrCounter++;
                        }

                        progressDialog.UpdateProgress(i, strId, strIds.Count());

                        i++;
                    }

                    progressDialog.UpdateProgress(-1, "Writing report...", strIds.Count());

                    // Write Excel report
                    ExcelHelperService eh = new ExcelHelperService(outputFilePath);
                    eh.WriteReport(report, structures, notFoundIds, workActions, startYear, endYear, startTime, debug,
                                    showPiFactors, "", false, false, similarComboWorkActions);

                    try
                    {
                        eh.SaveWorkbook();
                    }
                    catch (Exception ex)
                    { }

                    if (!String.IsNullOrEmpty(debugFilePath))
                    {
                        ReportWriterService.WritePriorityScoreReport(report, structures, startYear, endYear, debugFilePath, priorityIndexCategories, priorityIndexFactors);
                    }

                    progressDialog.UpdateProgress(-2, "Completed analysis...", strIds.Count());
                    Thread.Sleep(2000);
                    progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                }
            ));

            backgroundThread.Start();
            progressDialog.ShowDialog();

            /*
            foreach (string strId in strIds)
            {
                //if (strCounter >= 20)
                    //break;

                if (dbObj.GetElementInspectionDates(strId).Count > 0)
                {
                    Structure str = null;

                    try
                    {
                        str = AnalyzeStructure(report, strId, startYear, endYear, deteriorateDefects, caiFormulaId, improvementProgramWorkActions, workActionCriteria);
                    }
                    catch(Exception e)
                    {
                        notFoundIds.Add(strId + ": " + e.StackTrace);
                        notFoundStrCounter++;
                    }

                    if (str != null)
                    {
                        structures.Add(str);
                        strCounter++;
                    }
                    else
                    {
                        notFoundIds.Add(strId);
                        notFoundStrCounter++;
                    }
                }
                else
                {
                    notFoundIds.Add(strId + ": no element inspection");
                    notFoundStrCounter++;
                }
            }
            */


        }


        public Structure AnalyzeStructure(Structure structure, int startYear, int endYear, int caiFormulaId, string outputFilePath, bool deteriorateDefects, bool improvementProgramWorkActions = false, bool debug = false)
        {
            GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, deteriorateDefects, 2, false, improvementProgramWorkActions);
            return structure;
        }

        public Structure AnalyzeStructure(WisamType.AnalysisReports report, string strId, int startYear, int endYear, bool deteriorateDefects, int caiFormulaId, bool improvementProgramWorkActions = false, List<WorkActionCriteria> workActionCriteria = null, bool interpolateNbi = false, bool countTpo = false)
        {
            Structure structure = GetStructure(strId, interpolateNbi, countTpo, startYear, endYear);

            if (structure != null)
            {
                switch (report)// Revise pass in 'structure' object?
                {
                    case WisamType.AnalysisReports.StateNeedsPmdss:
                        try
                        {
                            GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, deteriorateDefects, 2, true, improvementProgramWorkActions);
                        }
                        catch { }
                        GetStructureDoNothings(structure, startYear, endYear, 2, deteriorateDefects, caiFormulaId, true);
                        structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(strId, startYear, endYear, 2, deteriorateDefects, caiFormulaId, improvementProgramWorkActions);
                        //GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, improvementProgramWorkActions);
                        break;
                    /*
                    case WisamType.AnalysisReports.StatePmdss:
                        GetStructureDoNothings(structure, startYear, endYear, caiFormulaId);
                        structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(strId, startYear, endYear, caiFormulaId, improvementProgramWorkActions);
                        //GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(structure, startYear, endYear, caiFormulaId, improvementProgramWorkActions);
                        break;
                    */
                    case WisamType.AnalysisReports.StateFiips:
                        structure.YearlyProgrammedWorkActions = GetStructureProgrammedWorkCandidates(structure, startYear, endYear, 2, caiFormulaId, deteriorateDefects);
                        break;
                    /*
                    case WisamType.AnalysisReports.StateNeeds:
                        //structure.YearlyOptimalWorkActions = GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId);
                        GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, improvementProgramWorkActions);
                        break;
                     */
                    case WisamType.AnalysisReports.RegionNeedsNew:
                    case WisamType.AnalysisReports.StrDeckReplacements:
                    case WisamType.AnalysisReports.Flexible:
                    case WisamType.AnalysisReports.MetaManager:
                        try
                        {
                            GetStructureWorkActions(structure, startYear, endYear, caiFormulaId, deteriorateDefects, 2, workActionCriteria);
                        }
                        catch (Exception ex) { }
                        break;
                    case WisamType.AnalysisReports.RegionNeeds:
                        try
                        {
                            GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, deteriorateDefects, 2, true, improvementProgramWorkActions);
                            //GetStructureWorkActions(structure, startYear, endYear, caiFormulaId, deteriorateDefects, 2, true, improvementProgramWorkActions);
                        }
                        catch { }
                        GetStructureDoNothings(structure, startYear, endYear, 2, deteriorateDefects, caiFormulaId, true);
                        structure.YearlyProgrammedWorkActions = GetStructureProgrammedWorkCandidates(structure, startYear, endYear, 2, caiFormulaId, deteriorateDefects);
                        break;
                    case WisamType.AnalysisReports.AnalysisDebug:
                        try
                        {
                            GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, deteriorateDefects, 2, true, improvementProgramWorkActions);
                        }
                        catch { }
                        GetStructureDoNothings(structure, startYear, endYear, 2, deteriorateDefects, caiFormulaId, true);
                        structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(strId, startYear, endYear, 2, deteriorateDefects, caiFormulaId, improvementProgramWorkActions);

                        //GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId, 2, true, improvementProgramWorkActions);
                        structure.YearlyProgrammedWorkActions = GetStructureProgrammedWorkCandidates(structure, startYear, endYear, 2, caiFormulaId, deteriorateDefects);
                        break;
                    case WisamType.AnalysisReports.AllCurrentNeeds:
                        //GetStructureAllCurrentNeeds(structure, startYear, caiFormulaId, improvementProgramWorkActions);
                        break;
                }
            }

            return structure;
        }
        /*
        public Structure GetStructureFullAnalysis(string strId, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects)
        {
            Structure structure = GetStructure(strId);

            // PMDSS Report
            structure.YearlyDoNothings = GetStructureDoNothings(structure, startYear, endYear, caiFormulaId, deteriorateDefects);
            structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(strId, startYear, endYear, caiFormulaId);

            // FIIPS Report
            structure.YearlyProgrammedWorkActions = GetStructureProgrammedWorkCandidates(strId, startYear, endYear, caiFormulaId);

            // Needs Reports (2)
            //structure.YearlyOptimalWorkActions = GetStructureOptimalWorkCandidates(structure, startYear, endYear, caiFormulaId);
            
            return structure;
        }
        */
        public List<Structure> GetStructuresProgrammedWorkCandidates(string strIds, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects)
        {
            List<string> structureIds = StructureIdsToList(strIds);
            List<Structure> structures = new List<Structure>();
            DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", startYear));
            DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", endYear));

            foreach (string structureId in structureIds)
            {
                Structure structure = dbObj.GetStructure(structureId);

                if (structure != null)
                {
                    // Grab programmed work candidates for given structure
                    List<StructureWorkAction> programmedWcs = dbObj.GetProgrammedWorkActions(startDate, endDate, structureId);

                    // Complete list of work candidates; insert do-nothing for years that don't have programmed work candidates
                    List<StructureWorkAction> completeWcs = new List<StructureWorkAction>();

                    for (int i = startYear; i <= endYear; i++)
                    {
                        var yearWcs = programmedWcs.Where(e => e.EstimatedCompletionDate.Year == i).ToList();

                        if (yearWcs.Count == 0)
                        {
                            StructureWorkAction swa = new StructureWorkAction();
                            swa.WorkActionYear = i;
                            swa.WorkActionCode = Code.DoNothing;
                            completeWcs.Add(swa);
                        }
                        else
                        {
                            foreach (var yearWc in yearWcs)
                            {
                                completeWcs.Add(yearWc);
                            }
                        }
                    }

                    structure.YearlyProgrammedWorkActions = completeWcs;
                    GetCais(structure, startYear, endYear, caiFormulaId, completeWcs, deteriorateDefects);
                    structures.Add(structure);
                }
            }

            return structures;
        }

        public Structure GetStructure(string strId, bool interpolateNbi = false, bool countTpo = false, int startYear = 0, int endYear = 0, int caiFormulaId = 10)
        {
            Structure str = dbObj.GetStructure(strId, false, interpolateNbi, false, countTpo, startYear, endYear);
            str.LastInspectionCai = GetLastInspectionBasedCai(str, caiFormulaId);
            return str;
        }



        /*
        public List<StructureWorkAction> GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(Structure structure, int startYear, int endYear, bool deteriorateDefects, int caiFormulaId, bool programmableWorkActionsOnly = false)
        {
            List<StructureWorkAction> doNothingOptimalWcs = new List<StructureWorkAction>();
            GetStructureDoNothings(structure, startYear, endYear, deteriorateDefects, caiFormulaId);

            if (programmableWorkActionsOnly)
            {
                workActionCriteria = dbObj.GetWorkActionCriteria(programmableWorkActionsOnly);
            }

            for (int i = startYear; i <= endYear; i++)
            {
                // Grab Do-Nothing CAI for given year, which will be null if there's no inspection (actual or deteriorated) for given year
                Cai doNothingCai = structure.YearlyDoNothings.Where(e => e.WorkActionYear == i).First().CAI;

                if (doNothingCai != null)
                {
                    // Detetermine optimal work candidate; initialize it as Do Nothing
                    StructureWorkAction newOptimalWc = new StructureWorkAction(Code.DoNothing);
                    newOptimalWc.WorkActionDesc = Code.DoNothingDesc;
                    newOptimalWc.WorkActionYear = i;

                    // Loop through work actions
                    foreach (WorkActionCriteria wac in workActionCriteria)
                    {
                        if (IsWorkActionCriteriaMet(structure, doNothingCai, wac.RuleFormula, wac)) // Meets criteria for Do Something work action
                        {
                            newOptimalWc.WorkActionCode = wac.WorkActionCode;
                            newOptimalWc.WorkActionDesc = wac.WorkActionDesc;
                            Cai newCai = new Cai(doNothingCai, doNothingCai.Year, doNothingCai.AllElements[0].DeteriorationYear);
                            newOptimalWc.CAI = newCai;

                            List<StructureWorkAction> improvements = new List<StructureWorkAction>();
                            improvements.Add(newOptimalWc);

                            // Improve CAI
                            ImproveCai(newCai, improvements);

                            break;
                        }
                    }

                    doNothingOptimalWcs.Add(newOptimalWc);
                }
            }

            structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = doNothingOptimalWcs;
            return doNothingOptimalWcs;
        }
        */
        // 02/22/2016
        public List<StructureWorkAction> GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(string strId, int startYear, int endYear, int detStart, bool deteriorateDefects, int caiFormulaId, bool programmableWorkActionsOnly = false)
        {
            Structure structure = dbObj.GetStructure(strId);
            List<StructureWorkAction> doNothingOptimalWcs = new List<StructureWorkAction>();
            List<StructureWorkAction> doNothings = GetStructureDoNothings(structure, startYear, endYear, detStart, deteriorateDefects, caiFormulaId, true);
            List<WorkActionCriteria> workActionCriteria = dbObj.GetWorkActionCriteria(programmableWorkActionsOnly);

            for (int i = startYear; i <= endYear; i++)
            {
                // Grab Do-Nothing CAI for given year, which will be null if there's no inspection (actual or deteriorated) for given year
                Cai doNothingCai = doNothings.Where(e => e.WorkActionYear == i).First().CAI;

                if (doNothingCai != null)
                {
                    // Detetermine optimal work candidate; initialize it as Do Nothing
                    StructureWorkAction newOptimalWc = new StructureWorkAction(Code.DoNothing);
                    newOptimalWc.WorkActionDesc = Code.DoNothingDesc;
                    newOptimalWc.WorkActionYear = i;

                    // Loop through work actions
                    foreach (WorkActionCriteria wac in workActionCriteria)
                    {
                        //if (IsWorkActionCriteriaMet(structure, doNothingCai, wac.RuleFormula, wac)) // Meets criteria for Do Something work action
                        //if (IsWorkActionCriteriaMet(wac, structure, doNothingCai))
                        if (IsWorkActionEligible(wac, structure, doNothingCai))
                        {
                            StructureWorkAction candidate = dbObj.GetStructureWorkAction(wac.WorkActionCode);
                            candidate.WorkActionYear = i;

                            if (!IsStructureWorkActionRepeatable(candidate, structure.ConstructionHistoryProjects, i))
                            {
                                continue;
                            }

                            newOptimalWc = candidate;
                            /*
                            newOptimalWc.WorkActionCode = wac.WorkActionCode;
                            newOptimalWc.WorkActionDesc = wac.WorkActionDesc;
                            newOptimalWc.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                            newOptimalWc.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                            */

                            newOptimalWc.Cost = dbObj.GetStructureWorkActionCost(wac.WorkActionCode, structure);
                            Cai newCai = new Cai(doNothingCai, doNothingCai.Year, doNothingCai.AllElements[0].DeteriorationYear);
                            newOptimalWc.CAI = newCai;

                            List<StructureWorkAction> improvements = new List<StructureWorkAction>();
                            improvements.Add(newOptimalWc);

                            // Improve CAI
                            ImproveCai(newCai, improvements, structure);

                            break;
                        }
                    }

                    doNothingOptimalWcs.Add(newOptimalWc);
                }
            }

            return doNothingOptimalWcs;
        }

        /*
        public List<StructureWorkAction> GetStructureOptimalWorkCandidatesBasedOnDoNothingCondition(string strId, int startYear, int endYear, int caiFormulaId, bool programmableWorkActionsOnly = false)
        {
            Structure structure = dbObj.GetStructure(strId);
            List<StructureWorkAction> doNothingOptimalWcs = new List<StructureWorkAction>();
            List<StructureWorkAction> doNothings = GetStructureDoNothings(structure, startYear, endYear, caiFormulaId);
            List<WorkActionCriteria> workActionCriteria = dbObj.GetWorkActionCriteria(programmableWorkActionsOnly);

            for (int i = startYear; i <= endYear; i++)
            {
                // Grab Do-Nothing CAI for given year, which will be null if there's no inspection (actual or deteriorated) for given year
                Cai doNothingCai = doNothings.Where(e => e.WorkActionYear == i).First().CAI;

                if (doNothingCai != null)
                {
                    // Detetermine optimal work candidate; initialize it as Do Nothing
                    StructureWorkAction newOptimalWc = new StructureWorkAction(Code.DoNothing);
                    newOptimalWc.WorkActionDesc = Code.DoNothingDesc;
                    newOptimalWc.WorkActionYear = i;

                    // Loop through work actions
                    foreach (WorkActionCriteria wac in workActionCriteria)
                    {
                        if (IsWorkActionCriteriaMet(structure, doNothingCai, wac.RuleFormula, wac)) // Meets criteria for Do Something work action
                        {
                            newOptimalWc.WorkActionCode = wac.WorkActionCode;
                            newOptimalWc.WorkActionDesc = wac.WorkActionDesc;
                            newOptimalWc.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                            newOptimalWc.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                            newOptimalWc.Cost = dbObj.GetStructureWorkActionCost(wac.WorkActionCode, structure);
                            Cai newCai = new Cai(doNothingCai, doNothingCai.Year, doNothingCai.AllElements[0].DeteriorationYear);
                            newOptimalWc.CAI = newCai;

                            List<StructureWorkAction> improvements = new List<StructureWorkAction>();
                            improvements.Add(newOptimalWc);

                            // Improve CAI
                            ImproveCai(newCai, improvements);

                            break;
                        }
                    }

                    doNothingOptimalWcs.Add(newOptimalWc);
                }
            }
            
            return doNothingOptimalWcs; 
        }
        */

        // 6/20/2018: Rewrite of IsWorkActionCriteriaMet using StringBuilder instead of String
        private bool IsWorkActionEligible(WorkActionCriteria wac, Structure structure, Cai cai)
        {
            /*
            var debug = false;
            if (wac.RuleId == 20)
            {
                debug = true;
            }*/
            bool isMet = false;
            expressionToEvaluate.Clear();
            expressionToEvaluate.Append(wac.RuleFormula);
            expressionToEvaluate.Replace("NCUL", cai.NbiRatings.CulvertRating)
                                .Replace("SCCR", structure.ScourCritical.ToString())
                                .Replace("FRCR", structure.FractureCritical.ToString())
                                .Replace("FODG", structure.FunctionalObsoleteDueToDeckGeometry.ToString())
                                .Replace("FOVC", structure.FunctionalObsoleteDueToVerticalClearance.ToString())
                                .Replace("FUOB", structure.FunctionalObsolete.ToString())
                                .Replace("LOADCAP", structure.LoadCapacity.ToString())
                                .Replace("BRIDGEAGE", (cai.Year - structure.YearBuilt).ToString())
                                .Replace("NSUB", cai.NbiRatings.SubstructureRating)
                                .Replace("NSUP", cai.NbiRatings.SuperstructureRating)
                                .Replace("NDEC", cai.NbiRatings.DeckRating)
                                .Replace("NUMOVERLAY", structure.NumOlays.ToString())
                                .Replace("NUMTHINPOLYMEROVERLAYS", structure.NumThinPolymerOverlays.ToString())
                                .Replace("OVBD", structure.OverburdenDepth.ToString())
                                .Replace("ADTON", structure.Adt.ToString())
                                .Replace("SPECIAL", String.Format("({0})", structure.SpecialComponents));
            try
            {
                int currentDeckBuiltYear = structure.DeckBuilts.Where(e => e <= cai.Year).OrderByDescending(e => e).First();
                expressionToEvaluate.Replace("DECKAGE", (cai.Year - currentDeckBuiltYear).ToString());
            }
            catch { }

            List<int> elementNumbers = new List<int>();

            // Sample patterns to match: Q3OF1080, QTOF1080PARENT, QTOF216
            MatchCollection matches = Regex.Matches(expressionToEvaluate.ToString(), @"OF[0-9]+");

            foreach (Match match in matches)
            {
                var elemNum = Convert.ToInt32(match.ToString().Split(new string[] { "OF" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                if (!elementNumbers.Contains(elemNum))
                {
                    elementNumbers.Add(elemNum);
                }
            }

            foreach (var elementNumber in elementNumbers)
            {
                List<Element> elements = cai.AllElements.Where(e => e.ElemNum == elementNumber).ToList();

                if (elements.Count > 0)
                {
                    switch (elementNumber)
                    {
                        case 1080:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                        (e.ParentElemNum == 12 || e.ParentElemNum == 16 || e.ParentElemNum == 38 || e.ParentElemNum == 15
                                            || e.ParentElemNum == 13 || e.ParentElemNum == 8039 || e.ParentElemNum == 60
                                            || e.ParentElemNum == 65)).ToList();
                            break;
                        case 1130:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                        (e.ParentElemNum == 12 || e.ParentElemNum == 16 || e.ParentElemNum == 38
                                            || e.ParentElemNum == 60 || e.ParentElemNum == 65)).ToList();
                            break;
                        case 3440:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                                    e.ParentElemNum == 8516).ToList();
                            break;
                        case 1000:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                                    e.ParentElemNum == 330).ToList();
                            break;
                        case 8516:

                            elements = elements.Where(e => e.ParentElemNum == 28 || e.ParentElemNum == 29
                                                            || e.ParentElemNum == 102 || e.ParentElemNum == 107
                                                            || e.ParentElemNum == 120 || e.ParentElemNum == 141).ToList();
                            break;
                    }

                    if (elements.Count > 0)
                    {
                        var element = elements.First();
                        expressionToEvaluate.Replace("Q1OF" + elementNumber.ToString(), element.Cs1Quantity.ToString())
                                            .Replace("Q2OF" + elementNumber.ToString(), element.Cs2Quantity.ToString())
                                            .Replace("Q3OF" + elementNumber.ToString(), element.Cs3Quantity.ToString())
                                            .Replace("Q4OF" + elementNumber.ToString(), element.Cs4Quantity.ToString());
                    }
                    else
                    {
                        expressionToEvaluate.Replace("Q1OF" + elementNumber.ToString(), "0")
                                            .Replace("Q2OF" + elementNumber.ToString(), "0")
                                            .Replace("Q3OF" + elementNumber.ToString(), "0")
                                            .Replace("Q4OF" + elementNumber.ToString(), "0");
                    }
                }
                else
                {
                    expressionToEvaluate.Replace("Q1OF" + elementNumber.ToString(), "0")
                                        .Replace("Q2OF" + elementNumber.ToString(), "0")
                                        .Replace("Q3OF" + elementNumber.ToString(), "0")
                                        .Replace("Q4OF" + elementNumber.ToString(), "0");
                }

                MatchCollection parentMatches = Regex.Matches(expressionToEvaluate.ToString(), @"QTOF" + elementNumber.ToString() + "PARENT");

                if (parentMatches.Count > 0)
                {
                    if (elements.Count > 0)
                    {
                        var element = elements.First();
                        var parent = cai.AllElements.Where(e => e.ElemNum == element.ParentElemNum).ToList();

                        if (parent.Count() > 0)
                        {
                            expressionToEvaluate.Replace("QTOF" + elementNumber.ToString() + "PARENT", parent.First().TotalQuantity.ToString());
                        }
                        else
                        {
                            expressionToEvaluate.Replace("QTOF" + elementNumber.ToString() + "PARENT", "0");
                        }
                    }
                    else
                    {
                        expressionToEvaluate.Replace("QTOF" + elementNumber.ToString() + "PARENT", "0");
                    }
                }

                MatchCollection tqMatches = Regex.Matches(expressionToEvaluate.ToString(), @"QTOF" + elementNumber.ToString());

                if (tqMatches.Count > 0)
                {
                    if (elements.Count > 0)
                    {
                        var element = elements.First();
                        expressionToEvaluate.Replace("QTOF" + elementNumber.ToString(), element.TotalQuantity.ToString());
                    }
                    else
                    {
                        expressionToEvaluate.Replace("QTOF" + elementNumber.ToString(), "0");
                    }
                }
            }

            MatchCollection bearingMatches = Regex.Matches(expressionToEvaluate.ToString(), @"OFBEAR");
            if (bearingMatches.Count > 0)
            {
                expressionToEvaluate = expressionToEvaluate.Replace("Q1OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs1Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q2OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs2Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q3OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs3Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q4OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs4Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("QTOFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.TotalQuantity).ToString());
            }

            MatchCollection railMatches = Regex.Matches(expressionToEvaluate.ToString(), @"OFRAIL");
            if (railMatches.Count > 0)
            {
                expressionToEvaluate = expressionToEvaluate.Replace("Q1OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs1Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q2OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs2Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q3OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs3Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q4OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs4Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("QTOFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.TotalQuantity).ToString());
            }

            expressionToEvaluate = expressionToEvaluate.Replace("/0", "/1000000"); // to handle division by zero

            try
            {
                isMet = Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate.ToString(), null));

                //bool wtf = Convert.ToBoolean(new DataTable().Compute("(20 + 0)/0.0 > 0.15", null));
                if (isMet && wac.RuleCategory.Equals("OLAY"))
                {
                    // If structure's a "timber slab or deck", only allowed overlay is AC Overlay (HMA)
                    if (structure.MainSpanMaterial.ToUpper().Contains("TIMBER") && (structure.StructureType.ToUpper().Contains("SLAB") || structure.StructureType.ToUpper().Contains("DECK")))
                    {
                        if (!wac.WorkActionCode.Equals(Code.OverlayHma))
                        {
                            isMet = false;
                        }
                    }
                }
            }
            catch (Exception e) { }

            return isMet;
        }

        // 6/15/2018: Rewrite of IsWorkActionCriteriaMet
        private bool IsWorkActionCriteriaMet(WorkActionCriteria wac, Structure structure, Cai cai)
        {
            bool isMet = false;
            string expressionToEvaluate = wac.RuleFormula.ToUpper().Trim();
            expressionToEvaluate = expressionToEvaluate.Replace("NCUL", cai.NbiRatings.CulvertRating);
            expressionToEvaluate = expressionToEvaluate
                                    .Replace("SCCR", structure.ScourCritical.ToString().ToUpper())
                                    .Replace("FRCR", structure.FractureCritical.ToString().ToUpper())
                                    .Replace("FODG", structure.FunctionalObsoleteDueToDeckGeometry.ToString().ToUpper())
                                    .Replace("FOVC", structure.FunctionalObsoleteDueToVerticalClearance.ToString().ToUpper())
                                    .Replace("FUOB", structure.FunctionalObsolete.ToString().ToUpper())
                                    .Replace("LOADCAP", structure.LoadCapacity.ToString())
                                    .Replace("BRIDGEAGE", (cai.Year - structure.YearBuilt).ToString())
                                    .Replace("NSUB", cai.NbiRatings.SubstructureRating)
                                    .Replace("NSUP", cai.NbiRatings.SuperstructureRating)
                                    .Replace("NDEC", cai.NbiRatings.DeckRating)
                                    .Replace("NUMOVERLAY", structure.NumOlays.ToString())
                                    .Replace("NUMTHINPOLYMEROVERLAYS", structure.NumThinPolymerOverlays.ToString())
                                    .Replace("OVBD", structure.OverburdenDepth.ToString());

            try
            {
                int currentDeckBuiltYear = structure.DeckBuilts.Where(e => e <= cai.Year).OrderByDescending(e => e).First();
                expressionToEvaluate = expressionToEvaluate.Replace("DECKAGE", (cai.Year - currentDeckBuiltYear).ToString());
            }
            catch { }

            List<int> elementNumbers = new List<int>();

            // Q3OF1080
            MatchCollection matches = Regex.Matches(expressionToEvaluate, @"Q[1-4]OF[0-9]+");

            foreach (Match match in matches)
            {
                var elemNum = Convert.ToInt32(match.ToString().Split(new string[] { "OF" }, StringSplitOptions.RemoveEmptyEntries)[1]);

                if (!elementNumbers.Contains(elemNum))
                {
                    elementNumbers.Add(elemNum);
                }
            }

            // QTOF1080PARENT, QTOF216
            MatchCollection totalQtyMatches = Regex.Matches(expressionToEvaluate, @"QTOF[0-9]+");

            foreach (Match match in totalQtyMatches)
            {
                var elemNum = Convert.ToInt32(match.ToString().Split(new string[] { "OF", "PARENT" }, StringSplitOptions.RemoveEmptyEntries)[1]);

                if (!elementNumbers.Contains(elemNum))
                {
                    elementNumbers.Add(elemNum);
                }
            }

            foreach (var elementNumber in elementNumbers)
            {
                List<Element> elements = cai.AllElements.Where(e => e.ElemNum == elementNumber).ToList();

                if (elements.Count > 0)
                {
                    switch (elementNumber)
                    {
                        case 1080:
                        case 1130:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                        (e.ParentElemNum == 12 || e.ParentElemNum == 16 || e.ParentElemNum == 38
                                            || e.ParentElemNum == 13 || e.ParentElemNum == 8039 || e.ParentElemNum == 60 || e.ParentElemNum == 65)).ToList();

                            break;
                        case 3440:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                                    e.ParentElemNum == 8516).ToList();
                            break;
                        case 1000:
                            elements = elements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) &&
                                                    e.ParentElemNum == 330).ToList();
                            break;
                        case 8516:

                            elements = elements.Where(e => e.ParentElemNum == 28 || e.ParentElemNum == 29
                                                            || e.ParentElemNum == 102 || e.ParentElemNum == 107
                                                            || e.ParentElemNum == 120 || e.ParentElemNum == 141).ToList();
                            break;
                    }

                    if (elements.Count > 0)
                    {
                        var element = elements.First();
                        expressionToEvaluate = expressionToEvaluate.Replace("Q1OF" + elementNumber.ToString(), element.Cs1Quantity.ToString())
                                                                    .Replace("Q2OF" + elementNumber.ToString(), element.Cs2Quantity.ToString())
                                                                    .Replace("Q3OF" + elementNumber.ToString(), element.Cs3Quantity.ToString())
                                                                    .Replace("Q4OF" + elementNumber.ToString(), element.Cs4Quantity.ToString());
                    }
                    else
                    {
                        expressionToEvaluate = expressionToEvaluate.Replace("Q1OF" + elementNumber.ToString(), "0")
                                                                .Replace("Q2OF" + elementNumber.ToString(), "0")
                                                                .Replace("Q3OF" + elementNumber.ToString(), "0")
                                                                .Replace("Q4OF" + elementNumber.ToString(), "0");
                    }
                }
                else
                {
                    expressionToEvaluate = expressionToEvaluate.Replace("Q1OF" + elementNumber.ToString(), "0")
                                                                .Replace("Q2OF" + elementNumber.ToString(), "0")
                                                                .Replace("Q3OF" + elementNumber.ToString(), "0")
                                                                .Replace("Q4OF" + elementNumber.ToString(), "0");
                }

                MatchCollection parentMatches = Regex.Matches(expressionToEvaluate, @"QTOF" + elementNumber.ToString() + "PARENT");

                if (parentMatches.Count > 0)
                {
                    if (elements.Count > 0)
                    {
                        var element = elements.First();
                        var parent = cai.AllElements.Where(e => e.ElemNum == element.ParentElemNum).ToList();

                        if (element.ElemNum == 3210 || element.ElemNum == 3220 || element.ElemNum == 8911)
                        {
                            var stop = true;
                        }

                        if (parent.Count() > 0)
                        {
                            expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementNumber.ToString() + "PARENT", parent.First().TotalQuantity.ToString());
                        }
                        else
                        {
                            expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementNumber.ToString() + "PARENT", "0");
                        }
                    }
                    else
                    {
                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementNumber.ToString() + "PARENT", "0");
                    }
                }

                MatchCollection tqMatches = Regex.Matches(expressionToEvaluate, @"QTOF" + elementNumber.ToString());

                if (tqMatches.Count > 0)
                {
                    if (elements.Count > 0)
                    {
                        var element = elements.First();
                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementNumber.ToString(), element.TotalQuantity.ToString());
                    }
                    else
                    {
                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementNumber.ToString(), "0");
                    }
                }
            }

            MatchCollection bearingMatches = Regex.Matches(expressionToEvaluate, @"OFBEAR");
            if (bearingMatches.Count > 0)
            {
                expressionToEvaluate = expressionToEvaluate.Replace("Q1OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs1Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q2OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs2Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q3OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs3Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q4OFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.Cs4Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("QTOFBEAR", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("BEAR")).Sum(g => g.TotalQuantity).ToString());
            }

            MatchCollection railMatches = Regex.Matches(expressionToEvaluate, @"OFRAIL");
            if (railMatches.Count > 0)
            {
                expressionToEvaluate = expressionToEvaluate.Replace("Q1OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs1Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q2OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs2Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q3OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs3Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("Q4OFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.Cs4Quantity).ToString());
                expressionToEvaluate = expressionToEvaluate.Replace("QTOFRAIL", cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals("RAIL")).Sum(g => g.TotalQuantity).ToString());
            }

            expressionToEvaluate = expressionToEvaluate.Replace("/0", "/1"); // to handle division by zero

            try
            {
                isMet = Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null));

                if (isMet && wac.RuleCategory.Equals("OLAY"))
                {
                    // If structure's a "timber slab or deck", only allowed overlay is AC Overlay (HMA)
                    if (structure.MainSpanMaterial.ToUpper().Contains("TIMBER") && (structure.StructureType.ToUpper().Contains("SLAB") || structure.StructureType.ToUpper().Contains("DECK")))
                    {
                        if (!wac.WorkActionCode.Equals(Code.OverlayHma))
                        {
                            isMet = false;
                        }
                    }
                }
            }
            catch { }

            return isMet;
        }

        private bool IsWorkActionCriteriaMet(Structure structure, Cai cai, string criteria, WorkActionCriteria wac)
        {
            bool isMet = false;
            string expressionToEvaluate = criteria;

            if (wac.RuleId == 62)
            {
                var pause = true;
            }

            try
            {
                if (cai.AllElements.Where(e => e.ElemNum == 28).Count() > 0 && wac.RuleCategory.Equals("OLAY"))
                {
                    return false;
                }
            }
            catch (Exception ex) { }


            if (wac.WorkActionCode.Equals("03"))
            {
                var stop = true;
            }

            // Split big expression into individual expressions, separated by "ANDs" and "ORs"
            var expressions = criteria.Split(new string[] { "AND", "OR", ">", "=", "<", "(", ")" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (!cai.NbiRatings.DeckRating.Trim().ToUpper().Equals("N"))
            {
                foreach (var expression in expressions)
                {
                    switch (expression.ToUpper().Trim())
                    {
                        case "OVLYAGE":
                            int lastOlay = 0;
                            try
                            {
                                lastOlay = structure.Overlays.OrderByDescending(e => e).First();
                            }
                            catch (Exception ex)
                            { }

                            break;
                        case "SCCR":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.ScourCritical.ToString().ToUpper());
                            break;

                        case "FRCR":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.FractureCritical.ToString().ToUpper());
                            break;

                        case "FODG":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.FunctionalObsoleteDueToDeckGeometry.ToString().ToUpper());
                            break;

                        case "FOVC":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.FunctionalObsoleteDueToVerticalClearance.ToString().ToUpper());
                            break;

                        case "FUOB":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.FunctionalObsolete.ToString().ToUpper());
                            break;

                        case "LOADCAP":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.LoadCapacity.ToString());
                            break;

                        case "BRIDGEAGE":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), (cai.Year - structure.YearBuilt).ToString());
                            break;

                        case "DECKAGE":
                            if (structure.DeckBuilts.Count > 0)
                            {
                                int currentDeckBuiltYear = structure.DeckBuilts.Where(e => e <= cai.Year).OrderByDescending(e => e).First();
                                expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), (cai.Year - currentDeckBuiltYear).ToString());
                            }
                            break;

                        case "NSUB":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), cai.NbiRatings.SubstructureRating);
                            //expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), Math.Round(cai.NbiRatings.SubstructureRatingVal, 0, MidpointRounding.AwayFromZero).ToString());
                            break;

                        case "NSUP":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), cai.NbiRatings.SuperstructureRating);
                            //expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), Math.Round(cai.NbiRatings.SuperstructureRatingVal, 0, MidpointRounding.AwayFromZero).ToString());
                            break;

                        case "NDEC":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), cai.NbiRatings.DeckRating);
                            //expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), Math.Round(cai.NbiRatings.DeckRatingVal, 0, MidpointRounding.AwayFromZero).ToString());
                            break;

                        /*
                        case "SUFF":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.SufficiencyNumber.ToString());
                            break;
                        */

                        case "NUMOVERLAY":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.NumOlays.ToString());
                            break;

                        case "NUMTHINPOLYMEROVERLAYS":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.NumThinPolymerOverlays.ToString());
                            break;

                        case "OVBD":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), structure.OverburdenDepth.ToString());
                            break;

                        default:
                            int someInt;
                            double someDouble;

                            if (expression.Trim().Length != 0 && !Int32.TryParse(expression, out someInt) && !Double.TryParse(expression, out someDouble))
                            {
                                foreach (string elementToCheck in Code.ElementsToCheck.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int elementToCheckNumber = Convert.ToInt32(elementToCheck);

                                    /*
                                    if (elementToCheckNumber == 206 || elementToCheckNumber == 228)
                                    {
                                        int j = 0;
                                    }
                                    */

                                    if (expression.ToUpper().IndexOf("OF" + elementToCheck) >= 0)
                                    {
                                        var elements = cai.AllElements.Where(e => e.ElemNum == elementToCheckNumber).ToList();
                                        int parentElemNum = 0;

                                        if (elementToCheckNumber == 1080 || elementToCheckNumber == 1130)
                                        {
                                            elements = cai.AllElements.Where(e => e.ElemNum == elementToCheckNumber && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("12")
                                                                                    || e.ParentElemNum.ToString().Equals("16")
                                                                                    || e.ParentElemNum.ToString().Equals("38")
                                                                                    || e.ParentElemNum.ToString().Equals("13")
                                                                                    || e.ParentElemNum.ToString().Equals("8039")
                                                                                    || e.ParentElemNum.ToString().Equals("60")
                                                                                    || e.ParentElemNum.ToString().Equals("65")
                                                                                )).ToList();
                                        }
                                        else if (elementToCheckNumber == 3440)
                                        {
                                            elements = cai.AllElements.Where(e => e.ElemNum == elementToCheckNumber && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("8516")

                                                                                )).ToList();
                                        }
                                        else if (elementToCheckNumber == 1000)
                                        {
                                            elements = cai.AllElements.Where(e => e.ElemNum == elementToCheckNumber && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("330")

                                                                                )).ToList();
                                        }
                                        else if (elementToCheckNumber == 8516)
                                        {
                                            // steel deck and superstructure elements only for the parents
                                            elements = cai.AllElements.Where(e => e.ElemNum == elementToCheckNumber &&
                                                                                (e.ParentElemNum.ToString().Equals("28")
                                                                                    || e.ParentElemNum.ToString().Equals("29")
                                                                                    || e.ParentElemNum.ToString().Equals("102")
                                                                                    || e.ParentElemNum.ToString().Equals("107")
                                                                                    || e.ParentElemNum.ToString().Equals("120")
                                                                                    || e.ParentElemNum.ToString().Equals("141")
                                                                                )).ToList();
                                        }



                                        if (elements.Count > 0)
                                        {
                                            var element = elements.First();

                                            expressionToEvaluate = expressionToEvaluate.Replace("Q1OF" + elementToCheck, element.Cs1Quantity.ToString())
                                                                                    .Replace("Q2OF" + elementToCheck, element.Cs2Quantity.ToString())
                                                                                    .Replace("Q3OF" + elementToCheck, element.Cs3Quantity.ToString())
                                                                                    .Replace("Q4OF" + elementToCheck, element.Cs4Quantity.ToString());
                                            //try
                                            {
                                                var parentElems = cai.AllElements.Where(e => e.ElemNum == element.ParentElemNum);

                                                if (parentElems.Count() > 0)
                                                {
                                                    Element parentElem = parentElems.First();
                                                    if (parentElem.TotalQuantity > 0)
                                                    {
                                                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck + "PARENT", parentElem.TotalQuantity.ToString());
                                                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck, element.TotalQuantity.ToString());
                                                    }
                                                    else
                                                    {
                                                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck + "PARENT", "1");
                                                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck, element.TotalQuantity.ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck, element.TotalQuantity.ToString());
                                                }
                                            }
                                            //catch { }

                                            //expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck, element.TotalQuantity.ToString());
                                        }
                                        else
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("Q1OF" + elementToCheck, "0")
                                                                                    .Replace("Q2OF" + elementToCheck, "0")
                                                                                    .Replace("Q3OF" + elementToCheck, "0")
                                                                                    .Replace("Q4OF" + elementToCheck, "0");
                                            expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck + "PARENT", "1");
                                            expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementToCheck, "0");
                                        }
                                    }
                                }


                                foreach (string elementClassificationToCheck in Code.ElementClassificationsToCheck.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (expression.ToUpper().IndexOf("OF" + elementClassificationToCheck) >= 0)
                                    {
                                        int sumQ1s = cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals(elementClassificationToCheck.ToUpper()))
                                                                    .Sum(g => g.Cs1Quantity);

                                        int sumQ2s = cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals(elementClassificationToCheck.ToUpper()))
                                                                   .Sum(g => g.Cs2Quantity);

                                        int sumQ3s = cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals(elementClassificationToCheck.ToUpper()))
                                                                   .Sum(g => g.Cs3Quantity);

                                        int sumQ4s = cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals(elementClassificationToCheck.ToUpper()))
                                                                   .Sum(g => g.Cs4Quantity);

                                        int sumTotals = cai.AllElements.Where(e => e.ElementClassificationCode.ToUpper().Equals(elementClassificationToCheck.ToUpper()))
                                                                   .Sum(g => g.TotalQuantity);

                                        if (sumTotals == 0)
                                        {
                                            sumTotals = 1; // so you're not dividing by zero in cases where this element doesn't exist
                                        }

                                        expressionToEvaluate = expressionToEvaluate.Replace("Q1OF" + elementClassificationToCheck, sumQ1s.ToString())
                                                                                    .Replace("Q2OF" + elementClassificationToCheck, sumQ2s.ToString())
                                                                                    .Replace("Q3OF" + elementClassificationToCheck, sumQ3s.ToString())
                                                                                    .Replace("Q4OF" + elementClassificationToCheck, sumQ4s.ToString());
                                        expressionToEvaluate = expressionToEvaluate.Replace("QTOF" + elementClassificationToCheck, sumTotals.ToString());
                                    }
                                }
                            }

                            break;
                    }
                }
            }
            else // Culvert
            {
                expressionToEvaluate = expressionToEvaluate.Replace("NCUL", cai.NbiRatings.CulvertRating);
                /*
                foreach (var expression in expressions)
                {
                    switch (expression.ToUpper().Trim())
                    {
                        case "NCUL":
                            expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), cai.NbiRatings.CulvertRating);
                            //expressionToEvaluate = expressionToEvaluate.Replace(expression.Trim(), Math.Round(cai.NbiRatings.CulvertRatingVal, 0, MidpointRounding.AwayFromZero).ToString());
                            break;
                    }
                }
                */
            }

            if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
            {
                try
                {
                    isMet = Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null));
                }
                catch { }
            }
            else
            {
                try
                {
                    isMet = Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null));
                }
                catch { }
            }

            if (isMet)
            {
                int m = 0;
            }

            if (wac.RuleCategory.Equals("OLAY") && isMet)
            {
                // If structure's a "timber slab or deck", only allowed overlay is AC Overlay (HMA)
                if (structure.MainSpanMaterial.ToUpper().Contains("TIMBER") && (structure.StructureType.ToUpper().Contains("SLAB") || structure.StructureType.ToUpper().Contains("DECK")))
                {
                    if (!wac.WorkActionCode.Equals(Code.OverlayHma))
                    {
                        isMet = false;
                    }
                }

                /*
                if (wac.AlternativeWorkActionCode != null && wac.AlternativeWorkActionCode.Equals(Code.OverlayHma))
                {
                    if (!structure.MainSpanMaterial.ToUpper().Contains("TIMBER") && structure.StructureType.ToUpper().Contains("SLAB"))
                    {
                        isMet = false;
                    }
                }*/

                /*
                if (!wac.WorkActionCode.Equals(Code.OverlayThinPolymer) && !wac.WorkActionCode.Equals(Code.OverlayThinPolymerNewJoints))
                {
                    if (structure.NumThinPolymerOverlays > 0)
                    {
                        isMet = false;
                    }
                }
                else
                {
                    if (structure.NumOlays > 0)
                    {
                        isMet = false;
                    }
                }
                */
            }

            return isMet;
        }

        public void GetStructureAllCurrentNeeds(Structure structure, int caiFormulaId, List<WorkActionCriteria> workActionCriteria)
        {
            // Grab last inspection date
            //DateTime lastInspectionDate = dbObj.GetLastInspectionDate(structure.StructureId);

            if (structure.LastInspection != null)
            {
                //List<WorkActionCriteria> workActionCriteria = dbObj.GetWorkActionCriteria(improvementWorkActions, workActionCodes);
                Cai lastCai = GetInspectionBasedCai(structure.StructureId, caiFormulaId, structure.LastInspection.InspectionDate);
                structure.CurrentCai = lastCai;

                foreach (var criterion in workActionCriteria)
                {
                    //if (IsWorkActionCriteriaMet(structure, lastCai, criterion.RuleFormula, criterion))
                    //if (IsWorkActionCriteriaMet(criterion, structure, lastCai))
                    if (IsWorkActionEligible(criterion, structure, lastCai))
                    {
                        StructureWorkAction need = dbObj.GetStructureWorkAction(criterion.WorkActionCode);
                        need.ControllingCriteria = criterion;
                        structure.AllCurrentNeeds.Add(need);
                    }
                }
            }
        }

        // 02/22/2016 
        public List<StructureWorkAction> GetStructureDoNothings(Structure structure, int startYear, int endYear, int detStart, bool deteriorateDefects, int caiFormulaId, bool applyCurrentYearFiipsWorkAction)
        {
            List<StructureWorkAction> doNothings = new List<StructureWorkAction>();
            List<DateTime> inspectionDates = dbObj.GetElementInspectionDatesAsc(structure.StructureId); // sorted oldest to newest

            if (inspectionDates.Count > 0)
            {
                for (int i = startYear; i <= endYear; i++)
                {
                    StructureWorkAction swa = new StructureWorkAction();

                    if (applyCurrentYearFiipsWorkAction && i == startYear)
                    {
                        List<StructureWorkAction> improvements = GetStructureProgrammedWorkCandidates(structure, startYear, startYear, caiFormulaId, deteriorateDefects);

                        if (improvements.Count() > 0)
                        {
                            swa = improvements.First();
                            swa.WorkActionYear = i;
                            //swa.WorkActionYear = startYear;

                            if (!structure.AddedFiipsCurrentYearProject)
                            {
                                if (swa.WorkActionCode != Code.DoNothing)
                                {
                                    structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)", i, swa.WorkActionDesc);
                                    structure.ConstructionHistoryProjects.Add(swa);
                                }
                                structure.AddedFiipsCurrentYearProject = true;
                            }
                        }
                    }
                    else
                    {
                        swa.WorkActionYear = i;
                        swa.WorkActionCode = Code.DoNothing;
                        swa.WorkActionDesc = Code.DoNothingDesc;
                    }

                    //swa.CAI = new Cai();
                    structure.YearlyDoNothings.Add(swa);
                }

                GetCais(structure, startYear, endYear, caiFormulaId, structure.YearlyDoNothings, detStart, deteriorateDefects);
            }

            return structure.YearlyDoNothings;
        }


        public List<StructureWorkAction> GetStructureDoNothings(Structure structure, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects)
        {
            List<StructureWorkAction> doNothings = new List<StructureWorkAction>();
            List<DateTime> inspectionDates = dbObj.GetElementInspectionDatesAsc(structure.StructureId); // sorted oldest to newest

            if (inspectionDates.Count > 0)
            {
                for (int i = startYear; i <= endYear; i++)
                {
                    StructureWorkAction swa = new StructureWorkAction();
                    swa.WorkActionYear = i;
                    swa.WorkActionCode = Code.DoNothing;
                    swa.WorkActionDesc = Code.DoNothingDesc;

                    //swa.CAI = new Cai();
                    structure.YearlyDoNothings.Add(swa);
                }

                GetCais(structure, startYear, endYear, caiFormulaId, structure.YearlyDoNothings, deteriorateDefects);
            }

            return structure.YearlyDoNothings;
        }

        private bool IsStructureWorkActionRepeatable(StructureWorkAction swa, List<StructureWorkAction> swas, int workActionYear)
        {
            bool repeatable = false;

            if (swa.WorkActionCode.Equals("77"))
            {
                int j = 0;
            }

            if (swa.Repeatable)
            {
                if (swa.RepeatFrequency == 1)
                {
                    repeatable = true;
                }
                else
                {
                    int prevWorkActionYear = 0;

                    if (swa.IncludesOverlay)
                    {
                        // Check primary optimal work actions
                        if (swas.Where(e => e.IncludesOverlay).Count() > 0)
                        {
                            StructureWorkAction prevSwa = swas.Last(e => e.IncludesOverlay);
                            prevWorkActionYear = prevSwa.WorkActionYear;
                        }

                        // Check primary combined work actions
                        var prevCwas = swas.Where(e => e.CombinedWorkActions.Where(c => c.IncludesOverlay).Count() > 0);
                        if (prevCwas.Count() > 0)
                        {
                            StructureWorkAction prevCwa = prevCwas.Last();
                            if (prevWorkActionYear == 0)
                            {
                                prevWorkActionYear = prevCwa.WorkActionYear;
                            }
                            else
                            {
                                prevWorkActionYear = Math.Max(prevWorkActionYear, prevCwa.WorkActionYear);
                            }
                        }

                        // Check secondary work actions
                        var prevSwas = swas.Where(e => e.SecondaryWorkActions.Where(s => s.IncludesOverlay && s.Improvement == true && (s.BypassCriteria == true || s.ControllingCriteria != null)).Count() > 0);
                        if (prevSwas.Count() > 0)
                        {
                            StructureWorkAction prevSwa = prevSwas.Last();

                            if (prevWorkActionYear == 0)
                            {
                                prevWorkActionYear = prevSwa.WorkActionYear;
                            }
                            else
                            {
                                prevWorkActionYear = Math.Max(prevWorkActionYear, prevSwa.WorkActionYear);
                            }
                        }
                    }
                    else
                    {
                        // Check primary optimal work actions
                        if (swas.Where(e => e.WorkActionCode == swa.WorkActionCode).Count() > 0)
                        {
                            StructureWorkAction prevSwa = swas.Last(e => e.WorkActionCode == swa.WorkActionCode);
                            prevWorkActionYear = prevSwa.WorkActionYear;
                        }

                        // Check primary combined work actions
                        var prevCwas = swas.Where(e => e.CombinedWorkActions.Where(c => c.WorkActionCode == swa.WorkActionCode).Count() > 0);
                        if (prevCwas.Count() > 0)
                        {
                            StructureWorkAction prevCwa = prevCwas.Last();
                            if (prevWorkActionYear == 0)
                            {
                                prevWorkActionYear = prevCwa.WorkActionYear;
                            }
                            else
                            {
                                prevWorkActionYear = Math.Max(prevWorkActionYear, prevCwa.WorkActionYear);
                            }
                        }

                        // Check secondary work actions
                        //var prevSwas = swas.Where(e => e.SecondaryWorkActions.Where(s => s.WorkActionCode == swa.WorkActionCode && s.Improvement == true && (s.BypassCriteria == true || s.ControllingCriteria != null)).Count() > 0);

                        /* commented out 3/17
                        var prevSwas = swas.Where(e => e.SecondaryWorkActions.Where(s => s.WorkActionCode == swa.WorkActionCode && s.Improvement == true && (s.BypassCriteria == true)).Count() > 0);
                        if (prevSwas.Count() > 0)
                        {
                            StructureWorkAction prevSwa = prevSwas.Last();
                            if (prevWorkActionYear == 0)
                            {
                                prevWorkActionYear = prevSwa.WorkActionYear;
                            }
                            else
                            {
                                prevWorkActionYear = Math.Max(prevWorkActionYear, prevSwa.WorkActionYear);
                            }
                        }
                        */
                    }

                    if (prevWorkActionYear == 0)
                    {
                        repeatable = true;
                    }
                    else if (prevWorkActionYear > 0)
                    {
                        if (workActionYear - prevWorkActionYear >= swa.RepeatFrequency)
                        {
                            repeatable = true;
                        }
                    }
                }
            }

            return repeatable;
        }

        /*
        private bool IsStructureWorkActionRepeatable(StructureWorkAction swa, List<StructureWorkAction> swas, int workActionYear)
        {
            bool repeatable = false;

            //var v = swas.Where(e => e.CombinedWorkActions.Where(f => f.WorkActionCode == swa.WorkActionCode)).Count() > 0);
            bool inCombinedWorkActions = false;

            foreach (var wa in swas)
            {
                foreach (var c in wa.CombinedWorkActions)
                {
                    if (c.WorkActionCode == swa.WorkActionCode)
                    {
                        inCombinedWorkActions = true;
                        break;
                    }
                }

                if (inCombinedWorkActions)
                {
                    break;
                }
            }

            bool inSecondaryWorkActions = false;

            foreach (var wa in swas)
            {
                if (wa.SecondaryWorkActions != null)
                {
                    foreach (var s in wa.SecondaryWorkActions)
                    {
                        if (s.WorkActionCode == swa.WorkActionCode)
                        {
                            inSecondaryWorkActions = true;
                            break;
                        }
                    }
                }
            }

            if (swas.Where(e => e.WorkActionCode == swa.WorkActionCode).Count() > 0 || inCombinedWorkActions || inSecondaryWorkActions) // Work action's already included in the list of work actions
            {
                if (swa.Repeatable)
                {
                    // Determine whether repeatable work action meets frequency criterion
                    if (swa.IncludesOverlay)
                    {
                        int prevWorkActionYear = 0;

                        try
                        {
                            StructureWorkAction prevSwa = swas.Last(e => e.IncludesOverlay == true);
                            if (prevSwa != null)
                            {
                                prevWorkActionYear = prevSwa.WorkActionYear;
                            }
                        }
                        catch { }

                        if (inSecondaryWorkActions)
                        {
                            StructureWorkAction s = swas.Last(e => e.SecondaryWorkActions.Where(f => f.WorkActionCode == swa.WorkActionCode).Count() > 0);
                            prevWorkActionYear = Math.Max(prevWorkActionYear, s.WorkActionYear);
                        }

                        if (workActionYear - prevWorkActionYear > swa.RepeatFrequency)
                        {
                            repeatable = true;
                        }
                    }
                    else
                    {
                        int prevWorkActionYear = 0;

                        try
                        {
                            StructureWorkAction prevSwa = swas.Last(e => e.WorkActionCode == swa.WorkActionCode);
                            if (prevSwa != null)
                            {
                                prevWorkActionYear = prevSwa.WorkActionYear;
                            }
                        }
                        catch { }

                        if (inCombinedWorkActions)
                        {
                            StructureWorkAction cwa = swas.Last(e => e.CombinedWorkActions.Where(f => f.WorkActionCode == swa.WorkActionCode).Count() > 0);
                            prevWorkActionYear = Math.Max(prevWorkActionYear, cwa.WorkActionYear);
                        }

                        if (inSecondaryWorkActions)
                        {
                            StructureWorkAction s = swas.Last(e => e.SecondaryWorkActions.Where(f => f.WorkActionCode == swa.WorkActionCode).Count() > 0);
                            prevWorkActionYear = Math.Max(prevWorkActionYear, s.WorkActionYear);
                        }

                        if (workActionYear - prevWorkActionYear > swa.RepeatFrequency)
                        {
                            repeatable = true;
                        }
                    }
                }
            }
            else // Specific Work action's not yet included
            {
                // But it's overlay, which there are different types
                if (swa.IncludesOverlay && (swas.Where(e => e.IncludesOverlay).Count() > 0 || inSecondaryWorkActions))
                {
                    if (swa.Repeatable)
                    {
                        // Determine whether repeatable work action meets frequency criterion
                        int prevWorkActionYear = 0;

                        try
                        {
                            StructureWorkAction prevSwa = swas.Last(e => e.IncludesOverlay == true);
                            if (prevSwa != null)
                            {
                                prevWorkActionYear = prevSwa.WorkActionYear;
                            }
                        }
                        catch { }

                        if (inSecondaryWorkActions)
                        {
                            StructureWorkAction s = swas.Last(e => e.SecondaryWorkActions.Where(f => f.WorkActionCode == swa.WorkActionCode).Count() > 0);
                            prevWorkActionYear = Math.Max(prevWorkActionYear, s.WorkActionYear);
                        }

                        if (workActionYear - prevWorkActionYear > swa.RepeatFrequency)
                        {
                            repeatable = true;
                        }
                    }
                }
                else
                {
                    repeatable = true;
                }
            }

            return repeatable;
        }
        */
        private void EvaluateFunctionalObsolete(Structure structure)
        {
            if (structure.FunctionalObsoleteDueToApproachRoadwayAlignment || structure.FunctionalObsoleteDueToDeckGeometry
                || structure.FunctionalObsoleteDueToStructureEvaluation || structure.FunctionalObsoleteDueToVerticalClearance
                || structure.FunctionalObsoleteDueToWaterwayAdequacy)
            {
                structure.FunctionalObsolete = true;
            }
        }

        private void UpdateElements(Structure structure, int parentElemNum, Cai cai, string workActionCode, int overlayQuantity, int deteriorationYear = 0, Cai lastInspectionCai = null)
        {
            var stop = true;
            // Remove overlay defects
            foreach (var parentElem in structure.LastInspection.Elements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)))
            {
                try
                {
                    cai.AllElements.RemoveAll(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == parentElem.ElemNum);
                }
                catch { }

                try
                {
                    cai.CaiElements.RemoveAll(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == parentElem.ElemNum);
                }
                catch { }
            }

            try
            {
                cai.AllElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Overlay));
                cai.CaiElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Overlay));
            }
            catch { }

            int olayElemNum = 0;
            List<int> olayDefects = new List<int>() { 3210, 3220, 8911 };

            switch (workActionCode)
            {
                case Code.OverlayConcrete:
                case Code.OverlayConcretePaint:
                case Code.OverlayConcreteNewJoints:
                case Code.OverlayConcreteNewRailJoints:
                    olayElemNum = 8514;
                    break;
                case Code.OverlayHma:
                    olayElemNum = 8511;
                    break;
                case Code.OverlayPma:
                    olayElemNum = 8512;
                    break;
                case Code.OverlayPolyesterPolymer:
                    olayElemNum = 8515;
                    break;
                case Code.OverlayThinPolymer:
                case Code.OverlayThinPolymerNewJoints:
                case Code.ReplaceDeckOverlayThinPolymer:
                case Code.ReplaceDeckOverlayThinPolymerPaintComplete:
                case Code.OverlayThinPolymerRepairJoints:
                    olayElemNum = 8513;
                    break;
                case Code.ReplaceStructure:
                case Code.NewStructure:
                case Code.ReplaceSuperstructure:
                case Code.ReplaceDeckRaiseStructure:
                case Code.ReplaceDeck:
                case Code.ReplaceDeckPaintComplete:
                case Code.ReplaceDeckWidenBridge:
                    Element deckSlab = cai.AllElements.Where(e => e.ElemNum == parentElemNum).First();
                    // Handle 1080 and 1130
                    if (cai.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == parentElemNum).Count() > 0)
                    {
                        cai.AllElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == parentElemNum);
                        AddDefects(deckSlab, cai.AllElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 });
                    }
                    if (cai.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == parentElemNum).Count() > 0)
                    {
                        cai.CaiElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == parentElemNum);
                        AddDefects(deckSlab, cai.CaiElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 });
                    }
                    olayElemNum = 8000;
                    break;
            }

            Element olayElem = new Element(olayElemNum);
            olayElem.Cs1Quantity = overlayQuantity;
            olayElem.Cs2Quantity = 0;
            olayElem.Cs3Quantity = 0;
            olayElem.Cs4Quantity = 0;
            olayElem.TotalQuantity = overlayQuantity;
            olayElem.ElementClassificationCode = Code.Overlay;
            olayElem.DeteriorationYear = deteriorationYear;
            olayElem.ParentElemNum = parentElemNum;

            try
            {
                cai.AllElements.Add(olayElem);
                cai.CaiElements.Add(olayElem);
            }
            catch { }

            foreach (var defectNum in olayDefects)
            {
                Element defect = new Element(defectNum);
                defect.ParentElemNum = olayElemNum;
                defect.ElementClassificationCode = Code.Defect;
                defect.DeteriorationYear = deteriorationYear;
                defect.EquivalentAge = 0;
                int cs1Quantity = 0;

                switch (defectNum)
                {
                    case 3210:
                        cs1Quantity = Convert.ToInt32(olayElem.TotalQuantity * 0.5);
                        break;
                    case 3220:
                    case 8911:
                        cs1Quantity = Convert.ToInt32(olayElem.TotalQuantity * 0.25);
                        break;
                }

                defect.Cs1Quantity = cs1Quantity;
                defect.Cs2Quantity = 0;
                defect.Cs3Quantity = 0;
                defect.Cs4Quantity = 0;
                cai.AllElements.Add(defect);
                cai.CaiElements.Add(defect);
            }
        }

        private void UpdateElements(Structure structure, Cai cai, string workActionCode, int overlayQuantity, int deteriorationYear = 0, Cai lastInspectionCai = null)
        {
            // Get parent deck/slab element of overlay elements
            int deckSlabElement = 0;

            if (structure.StructureType.ToUpper().Equals("BOX CULVERT"))
            {
                deckSlabElement = 241;
            }
            else
            {
                deckSlabElement = cai.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).First().ParentElemNum;
            }
            Element deckSlab = cai.AllElements.Where(e => e.ElemNum == deckSlabElement).First();
            // Reset overlay/wearing surface and defects
            var olayElements = cai.AllElements.Where(el => el.ElementClassificationCode.Equals(Code.Overlay)).ToList();
            switch (workActionCode)
            {
                case Code.RepairDeck:
                    foreach (var olayElement in olayElements)
                    {
                        foreach (Element defect in cai.AllElements.Where(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == olayElement.ElemNum).ToList())
                        {
                            defect.Cs2Quantity = defect.Cs2Quantity + defect.Cs3Quantity + defect.Cs4Quantity;
                            defect.Cs3Quantity = 0;
                            defect.Cs4Quantity = 0;
                        }
                        olayElement.Cs2Quantity = olayElement.Cs2Quantity + olayElement.Cs3Quantity + olayElement.Cs4Quantity;
                        olayElement.Cs3Quantity = 0;
                        olayElement.Cs4Quantity = 0;
                    }
                    foreach (var olayElement in cai.CaiElements.Where(el => el.ElementClassificationCode.Equals(Code.Overlay)).ToList())
                    {
                        foreach (Element defect in cai.CaiElements.Where(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == olayElement.ElemNum).ToList())
                        {
                            defect.Cs2Quantity = defect.Cs2Quantity + defect.Cs3Quantity + defect.Cs4Quantity;
                            defect.Cs3Quantity = 0;
                            defect.Cs4Quantity = 0;
                        }
                        olayElement.Cs2Quantity = olayElement.Cs2Quantity + olayElement.Cs3Quantity + olayElement.Cs4Quantity;
                        olayElement.Cs3Quantity = 0;
                        olayElement.Cs4Quantity = 0;
                    }
                    break;
                default:
                    foreach (var olayElement in olayElements)
                    {
                        if (cai.AllElements.Where(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == olayElement.ElemNum).Count() > 0)
                        {
                            cai.AllElements.RemoveAll(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == olayElement.ElemNum);
                        }
                        if (cai.CaiElements.Where(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == olayElement.ElemNum).Count() > 0)
                        {
                            cai.CaiElements.RemoveAll(f => f.ElementClassificationCode.Equals(Code.Defect) && f.ParentElemNum == olayElement.ElemNum);
                        }
                    }
                    if (cai.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).Count() > 0)
                    {
                        cai.AllElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Overlay));
                    }
                    if (cai.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).Count() > 0)
                    {
                        cai.CaiElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Overlay));
                    }
                    int olayElemNum = 0;
                    List<int> olayDefects = new List<int>() { 3210, 3220, 8911 };
                    switch (workActionCode)
                    {
                        case Code.OverlayConcrete:
                        case Code.OverlayConcretePaint:
                        case Code.OverlayConcreteNewJoints:
                        case Code.OverlayConcreteNewRailJoints:
                            olayElemNum = 8514;
                            break;
                        case Code.OverlayHma:
                            olayElemNum = 8511;
                            break;
                        case Code.OverlayPma:
                            olayElemNum = 8512;
                            break;
                        case Code.OverlayPolyesterPolymer:
                            olayElemNum = 8515;
                            break;
                        case Code.OverlayThinPolymer:
                        case Code.OverlayThinPolymerNewJoints:
                        //case Code.ReplaceDeckOverlayThinPolymer:
                        //case Code.ReplaceDeckOverlayThinPolymerPaintComplete:
                        case Code.OverlayThinPolymerRepairJoints:
                            olayElemNum = 8513;
                            break;
                        case Code.ReplaceDeckOverlayThinPolymer:
                        case Code.ReplaceDeckOverlayThinPolymerPaintComplete:
                            // Handle 1080 and 1130
                            if (cai.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement).Count() > 0)
                            {
                                cai.AllElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement);
                                AddDefects(deckSlab, cai.AllElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 });
                            }
                            if (cai.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement).Count() > 0)
                            {
                                cai.CaiElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement);
                                AddDefects(deckSlab, cai.CaiElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 });
                            }
                            olayElemNum = 8513;
                            break;
                        case Code.ReplaceStructure:
                        case Code.NewStructure:
                        case Code.ReplaceSuperstructure:
                        case Code.ReplaceDeckRaiseStructure:
                        case Code.ReplaceDeck:
                        case Code.ReplaceDeckPaintComplete:
                        case Code.ReplaceDeckWidenBridge:
                            // Handle 1080 and 1130
                            if (cai.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement).Count() > 0)
                            {
                                cai.AllElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement);
                                AddDefects(deckSlab, cai.AllElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 });
                            }
                            if (cai.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement).Count() > 0)
                            {
                                cai.CaiElements.RemoveAll(e => e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckSlabElement);
                                AddDefects(deckSlab, cai.CaiElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 });
                            }
                            olayElemNum = 8000;
                            break;
                    }
                    Element olayElem = new Element(olayElemNum);
                    olayElem.Cs1Quantity = overlayQuantity;
                    olayElem.Cs2Quantity = 0;
                    olayElem.Cs3Quantity = 0;
                    olayElem.Cs4Quantity = 0;
                    olayElem.TotalQuantity = overlayQuantity;
                    olayElem.ElementClassificationCode = Code.Overlay;
                    olayElem.DeteriorationYear = deteriorationYear;
                    olayElem.ParentElemNum = deckSlabElement;
                    olayElem.EquivalentAge = 0;
                    try
                    {
                        cai.AllElements.Add(olayElem);
                        cai.CaiElements.Add(olayElem);
                    }
                    catch { }
                    foreach (var defectNum in olayDefects)
                    {
                        Element defect = new Element(defectNum);
                        defect.ParentElemNum = olayElemNum;
                        defect.ElementClassificationCode = Code.Defect;
                        defect.DeteriorationYear = deteriorationYear;
                        defect.EquivalentAge = 0;
                        int cs1Quantity = 0;
                        switch (defectNum)
                        {
                            case 3210:
                                cs1Quantity = Convert.ToInt32(olayElem.TotalQuantity * 0.5);
                                break;
                            case 3220:
                            case 8911:
                                cs1Quantity = Convert.ToInt32(olayElem.TotalQuantity * 0.25);
                                break;
                        }
                        defect.Cs1Quantity = cs1Quantity;
                        defect.Cs2Quantity = 0;
                        defect.Cs3Quantity = 0;
                        defect.Cs4Quantity = 0;
                        defect.TotalQuantity = defect.Cs1Quantity;
                        cai.AllElements.Add(defect);
                        cai.CaiElements.Add(defect);
                    }
                    break;
            }
        }

        public void InitializeStructure(Structure structure, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects,
                                        int detStartOffset, List<WorkActionCriteria> workActionCriteria, NeedsAnalysisInput needsAnalysisInput = null)
        {
            //List<StructureWorkAction> optimalWas = new List<StructureWorkAction>();
            List<StructureWorkAction> doNothingWas = new List<StructureWorkAction>();
            List<StructureWorkAction> completeFiipsWas = new List<StructureWorkAction>(); // includes do-nothings in years where there isn't do-something
            List<StructureWorkAction> doNothingOptimalWas = new List<StructureWorkAction>();

            if (structure.LastInspection != null)
            {
                // Get structure for DoNothing Optimal
                //Structure structureForDoNothingOptimal = dbObj.GetStructure(structure.StructureId, false, structure.InterpolateNbi, false, structure.CountTpo);

                int lastInspectionYear = structure.LastInspection.InspectionDate.Year;
                int currentYear = DateTime.Now.Year;

                if (needsAnalysisInput != null)
                {
                    if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                    {
                        if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 7, 1)) >= 0)
                        {
                            lastInspectionYear = lastInspectionYear + 1;
                        }

                        if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 7, 1)) >= 0)
                        {
                            currentYear = currentYear + 1;
                        }
                    }
                    else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                    {
                        if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 11, 1)) >= 0)
                        {
                            lastInspectionYear = lastInspectionYear + 1;
                        }

                        if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 11, 1)) >= 0)
                        {
                            currentYear = currentYear + 1;
                        }
                    }

                }


                Cai lastCai = GetLastInspectionBasedCai(structure, caiFormulaId);
                Cai lastInspectionCai = new Cai(lastCai, lastInspectionYear, 0);
                Cai lastInspectionCaiFiips = new Cai(lastCai, lastInspectionYear, 0);
                Cai lastInspectionCaiDoNothing = new Cai(lastCai, lastInspectionYear, 0);
                //Cai lastInspectionCai = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);
                //Cai lastInspectionCaiFiips = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);
                //Cai lastInspectionCaiDoNothing = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);

                int deteriorationYear = 0;
                Cai previousOptimalCai = new Cai(lastInspectionCai, lastInspectionYear, 0);
                Cai previousDoNothingCai = new Cai(lastInspectionCai, lastInspectionYear, 0);
                Cai previousFiipsCai = new Cai(lastInspectionCai, lastInspectionYear, 0);

                DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", structure.LastInspection.InspectionDate.Year));
                DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", endYear));

                if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                {
                    startDate = Convert.ToDateTime(String.Format("07-01-{0}", structure.LastInspection.InspectionDate.Year - 1));
                    endDate = Convert.ToDateTime(String.Format("06-30-{0}", endYear));
                }
                else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                {
                    startDate = Convert.ToDateTime(String.Format("11-01-{0}", structure.LastInspection.InspectionDate.Year - 1));
                    endDate = Convert.ToDateTime(String.Format("10-31-{0}", endYear));
                }

                //List<StructureWorkAction> fiipsWas = dbObj.GetProgrammedWorkActions(startDate, endDate, structure.StructureId);
                List<StructureWorkAction> fiipsWas = dbObj.GetProgrammedWorkActions(startDate, endDate, structure, needsAnalysisInput.CalendarType);

                for (int year = lastInspectionYear; year <= endYear; year++)
                {
                    StructureWorkAction newDoNothing = new StructureWorkAction(Code.DoNothing);
                    newDoNothing.WorkActionDesc = Code.DoNothingDesc;
                    newDoNothing.WorkActionYear = year;
                    newDoNothing.CombinedWorkActions = new List<StructureWorkAction>();

                    // Unconstrained Optimal
                    // Initialize optimal work action for year
                    //StructureWorkAction newOptimalSwa = new StructureWorkAction(Code.DoNothing);
                    //newOptimalSwa.WorkActionDesc = Code.DoNothingDesc;
                    //newOptimalSwa.WorkActionYear = year;
                    //newOptimalSwa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Initialize optimal work action CAI for year
                    //Cai newOptimalCai = new Cai(previousOptimalCai, year, deteriorationYear);

                    // Initialize DN work action for year
                    StructureWorkAction newDoNothingSwa = new StructureWorkAction(Code.DoNothing);
                    newDoNothingSwa.WorkActionDesc = Code.DoNothingDesc;
                    newDoNothingSwa.WorkActionYear = year;
                    newDoNothingSwa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Initialize optimal work action for year under the DoNothing scenario
                    StructureWorkAction newDoNothingOptimalSwa = new StructureWorkAction(Code.DoNothing);
                    newDoNothingOptimalSwa.WorkActionDesc = Code.DoNothingDesc;
                    newDoNothingOptimalSwa.WorkActionYear = year;
                    newDoNothingOptimalSwa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Initialize optimal work action CAI for year
                    Cai newDoNothingCai = new Cai(previousDoNothingCai, year, deteriorationYear);

                    // FIIPS work action
                    Cai newFiipsCai = new Cai(previousFiipsCai, year, deteriorationYear);
                    //var yearlyFiipsWas = fiipsWas.Where(e => e.EstimatedCompletionDate.Year == year).ToList();
                    var yearlyFiipsWas = fiipsWas.Where(e => e.WorkActionYear == year).ToList();
                    StructureWorkAction newFiipsSwa = new StructureWorkAction();

                    if (yearlyFiipsWas.Count() > 0) // there's FIIPS programmed work action
                    {
                        //newFiipsSwa = dbObj.GetStructureWorkAction(yearlyFiipsWas.First().WorkActionCode); // only 
                        newFiipsSwa = yearlyFiipsWas.First();
                        //newFiipsSwa.WorkActionYear = year;
                        completeFiipsWas.Add(newFiipsSwa);
                        /*
                        foreach (var yearlyFiipsWa in yearlyFiipsWas)
                        {
                            completeFiipsWas.Add(yearlyFiipsWa);
                        }*/
                    }
                    else // no FIIPS programmed work action
                    {
                        newFiipsSwa = newDoNothing;
                        newFiipsSwa.WorkActionYear = year;
                        completeFiipsWas.Add(newFiipsSwa);
                    }

                    // Apply deterioration
                    // Example: lastInspectionYear = 2016, start deterioration year = 2016 + offset
                    //if (year >= lastInspectionYear + detStartOffset - 1)
                    if (deteriorationYear > 0) // see when deteriorationYear gets incremented
                    {
                        //newOptimalCai.Basis = WisamType.CaiBases.Deterioration;
                        //DeteriorateCai(newOptimalCai, previousOptimalCai, lastInspectionCai, deteriorateDefects);

                        newDoNothingCai.Basis = WisamType.CaiBases.Deterioration;
                        DeteriorateCai(newDoNothingCai, previousDoNothingCai, lastInspectionCaiDoNothing, deteriorateDefects);

                        newFiipsCai.Basis = WisamType.CaiBases.Deterioration;
                        DeteriorateCai(newFiipsCai, previousFiipsCai, lastInspectionCaiFiips, deteriorateDefects);
                    }

                    // FIIPS only scenario
                    if (yearlyFiipsWas.Count() > 0)
                    {
                        ImproveCai(structure, newFiipsCai, yearlyFiipsWas, year, lastInspectionCaiFiips, false);
                    }


                    // Determine optimal work action
                    // Apply FIIPS up to (Start Year - 1), provided year is within FIIPS 6 year window from current year
                    if (startYear > currentYear)
                    {
                        // Apply FIIPS
                        if (startYear <= currentYear + 5)
                        {
                            if (year < startYear)
                            {
                                // Apply FIIPS
                                if (yearlyFiipsWas.Count() > 0)
                                {
                                    //ImproveCai(structure, newOptimalCai, yearlyFiipsWas, year, lastInspectionCai);
                                    ImproveCai(structure, newDoNothingCai, yearlyFiipsWas, year, lastInspectionCaiDoNothing);
                                    //optimalWas.Add(newFiipsSwa);
                                    structure.ConstructionHistoryProjects.Add(newFiipsSwa);
                                    structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)  ", newFiipsSwa.WorkActionYear, newFiipsSwa.WorkActionDesc);
                                    /*
                                    foreach (var yearlyFiipsWa in yearlyFiipsWas)
                                    {
                                        structure.ConstructionHistoryProjects.Add(yearlyFiipsWa);
                                        structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)  ", yearlyFiipsWa.WorkActionYear, yearlyFiipsWa.WorkActionDesc);
                                    }*/
                                }
                            }
                        }
                        else if (startYear > currentYear + 5)
                        {
                            if (year <= currentYear + 5) // FIIPS through 6 year program
                            {
                                // Apply FIIPS
                                if (yearlyFiipsWas.Count() > 0)
                                {
                                    //ImproveCai(structure, newOptimalCai, yearlyFiipsWas, year, lastInspectionCai);
                                    ImproveCai(structure, newDoNothingCai, yearlyFiipsWas, year, lastInspectionCaiDoNothing);
                                    //optimalWas.Add(newFiipsSwa);
                                    structure.ConstructionHistoryProjects.Add(newFiipsSwa);
                                    structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)  ", newFiipsSwa.WorkActionYear, newFiipsSwa.WorkActionDesc);
                                    /*
                                    foreach (var yearlyFiipsWa in yearlyFiipsWas)
                                    {
                                        structure.ConstructionHistoryProjects.Add(yearlyFiipsWa);
                                        structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)  ", yearlyFiipsWa.WorkActionYear, yearlyFiipsWa.WorkActionDesc);
                                        optimalWas.Add(yearlyFiipsWa);
                                    }*/
                                }
                            }
                        }

                        // Start analysis
                        if (year >= startYear)
                        {
                            /*
                            // Loop through work action rules in sequence
                            foreach (WorkActionCriteria wac in workActionCriteria)
                            {
                                // If buried structure and category of work action criteria is overlay, SKIP
                                if (structure.BuriedStructure && wac.RuleCategory.ToUpper().Equals("OLAY"))
                                {
                                    continue;
                                }

                                List<StructureWorkAction> optimalImprovements = new List<StructureWorkAction>();

                                // Rule evaluates to true
                                if (IsWorkActionCriteriaMet(structure, newOptimalCai, wac.RuleFormula, wac))
                                {
                                    StructureWorkAction potentialOptimal = dbObj.GetStructureWorkAction(wac.WorkActionCode, structure);
                                    potentialOptimal.WorkActionYear = year;

                                    // Check whether potential work action meets repeat frequency
                                    if (!potentialOptimal.CombinedWorkAction) // Single primary work action
                                    {
                                        if (!IsStructureWorkActionRepeatable(potentialOptimal, structure.ConstructionHistoryProjects, year))
                                        {
                                            continue;
                                        }

                                        if (!IsStructureWorkActionRepeatable(potentialOptimal, optimalWas, year))
                                        {
                                            continue;
                                        }
                                    }
                                    else // Primary work action is a combined work action
                                    {
                                        bool repeatable = true;

                                        foreach (var waCode in potentialOptimal.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            StructureWorkAction curCwa = dbObj.GetStructureWorkAction(waCode, structure);
                                            potentialOptimal.CombinedWorkActions.Add(curCwa);

                                            if (!IsStructureWorkActionRepeatable(curCwa, structure.ConstructionHistoryProjects, year))
                                            {
                                                repeatable = false;
                                                break;
                                            }

                                            if (!IsStructureWorkActionRepeatable(curCwa, optimalWas, year))
                                            {
                                                repeatable = false;
                                                break;
                                            }
                                        }

                                        if (!repeatable)
                                        {
                                            continue;
                                        }
                                    }

                                    newOptimalSwa = potentialOptimal;
                                    newOptimalSwa.ControllingCriteria = wac;
                                    newOptimalSwa.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                                    newOptimalSwa.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                                    optimalImprovements.Add(newOptimalSwa);
                                    ImproveCai(structure, newOptimalCai, optimalImprovements, year, lastInspectionCai);

                                    // Determine Incidental/Secondary work actions and their eligibility
                                    List<CombinedWorkAction> potentialIncidentals = dbObj.GetSecondaryWorkActions(newOptimalSwa.WorkActionCode);

                                    if (!structure.StructureType.Equals("BOX CULVERT"))
                                    {
                                        foreach (var potentialIncidental in potentialIncidentals)
                                        {
                                            StructureWorkAction curPotentialIncidentalWorkAction = dbObj.GetStructureWorkAction(potentialIncidental.SecondaryWorkActionCode, structure);
                                            newOptimalSwa.AllSecondaryWorkActions.Add(curPotentialIncidentalWorkAction); // All incidentals evaluated for eligibility

                                            if (potentialIncidental.BypassRule) // Rule bypass
                                            {
                                                curPotentialIncidentalWorkAction.BypassCriteria = true;
                                                newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                            }
                                            else // Else Evaluate eligibility
                                            {
                                                // Get pertinent work action criteria
                                                List<WorkActionCriteria> rules = dbObj.GetWorkActionCriteriaForGivenWorkAction(curPotentialIncidentalWorkAction.WorkActionCode);

                                                if (rules.Count() == 0) // No rules, so eligible
                                                {
                                                    newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                                }
                                                else
                                                {
                                                    foreach (var rule in rules)
                                                    {
                                                        if (IsWorkActionCriteriaMet(structure, newOptimalCai, rule.RuleFormula, rule))
                                                        {
                                                            curPotentialIncidentalWorkAction.ControllingCriteria = rule;
                                                            newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            }
                            */

                            // Loop through work action rules in sequence
                            foreach (WorkActionCriteria wac in workActionCriteria)
                            {
                                // If buried structure and category of work action criteria is overlay, SKIP
                                if (structure.BuriedStructure && wac.RuleCategory.ToUpper().Equals("OLAY"))
                                {
                                    continue;
                                }

                                List<StructureWorkAction> optimalImprovements = new List<StructureWorkAction>();

                                // Rule evaluates to true
                                //if (IsWorkActionCriteriaMet(structureForDoNothingOptimal, newDoNothingCai, wac.RuleFormula, wac))
                                //if (IsWorkActionCriteriaMet(wac, structureForDoNothingOptimal, newDoNothingCai))
                                //if (IsWorkActionCriteriaMet(wac, structure, newDoNothingCai))
                                if (IsWorkActionEligible(wac, structure, newDoNothingCai))
                                {
                                    //StructureWorkAction potentialOptimal = dbObj.GetStructureWorkAction(wac.WorkActionCode, structureForDoNothingOptimal);
                                    StructureWorkAction potentialOptimal = dbObj.GetStructureWorkAction(wac.WorkActionCode, structure);
                                    potentialOptimal.WorkActionYear = year;

                                    // Check whether potential work action meets repeat frequency
                                    if (!potentialOptimal.CombinedWorkAction) // Single primary work action
                                    {
                                        //if (!IsStructureWorkActionRepeatable(potentialOptimal, structureForDoNothingOptimal.ConstructionHistoryProjects, year))
                                        if (!IsStructureWorkActionRepeatable(potentialOptimal, structure.ConstructionHistoryProjects, year))
                                        {
                                            continue;
                                        }

                                        //if (!IsStructureWorkActionRepeatable(potentialOptimal, optimalWas, year))
                                        //{
                                        //    continue;
                                        //}
                                    }
                                    else // Primary work action is a combined work action
                                    {
                                        bool repeatable = true;

                                        foreach (var waCode in potentialOptimal.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            //StructureWorkAction curCwa = dbObj.GetStructureWorkAction(waCode, structureForDoNothingOptimal);
                                            StructureWorkAction curCwa = dbObj.GetStructureWorkAction(waCode, structure);
                                            potentialOptimal.CombinedWorkActions.Add(curCwa);

                                            //if (!IsStructureWorkActionRepeatable(curCwa, structureForDoNothingOptimal.ConstructionHistoryProjects, year))
                                            if (!IsStructureWorkActionRepeatable(curCwa, structure.ConstructionHistoryProjects, year))
                                            {
                                                repeatable = false;
                                                break;
                                            }

                                            //if (!IsStructureWorkActionRepeatable(curCwa, optimalWas, year))
                                            //{
                                            //    repeatable = false;
                                            //    break;
                                            //}
                                        }

                                        if (!repeatable)
                                        {
                                            continue;
                                        }
                                    }

                                    newDoNothingOptimalSwa = potentialOptimal;
                                    newDoNothingOptimalSwa.ControllingCriteria = wac;
                                    newDoNothingOptimalSwa.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                                    newDoNothingOptimalSwa.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                                    //optimalImprovements.Add(newOptimalSwa);
                                    //ImproveCai(structure, newOptimalCai, optimalImprovements, year, lastInspectionCai);

                                    // Determine Incidental/Secondary work actions and their eligibility
                                    /*
                                    List<CombinedWorkAction> potentialIncidentals = dbObj.GetSecondaryWorkActions(newOptimalSwa.WorkActionCode);

                                    if (!structure.StructureType.Equals("BOX CULVERT"))
                                    {
                                        foreach (var potentialIncidental in potentialIncidentals)
                                        {
                                            StructureWorkAction curPotentialIncidentalWorkAction = dbObj.GetStructureWorkAction(potentialIncidental.SecondaryWorkActionCode, structure);
                                            newOptimalSwa.AllSecondaryWorkActions.Add(curPotentialIncidentalWorkAction); // All incidentals evaluated for eligibility

                                            if (potentialIncidental.BypassRule) // Rule bypass
                                            {
                                                curPotentialIncidentalWorkAction.BypassCriteria = true;
                                                newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                            }
                                            else // Else Evaluate eligibility
                                            {
                                                // Get pertinent work action criteria
                                                List<WorkActionCriteria> rules = dbObj.GetWorkActionCriteriaForGivenWorkAction(curPotentialIncidentalWorkAction.WorkActionCode);

                                                if (rules.Count() == 0) // No rules, so eligible
                                                {
                                                    newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                                }
                                                else
                                                {
                                                    foreach (var rule in rules)
                                                    {
                                                        if (IsWorkActionCriteriaMet(structure, newOptimalCai, rule.RuleFormula, rule))
                                                        {
                                                            curPotentialIncidentalWorkAction.ControllingCriteria = rule;
                                                            newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    */

                                    break;
                                }
                            }
                        }
                    }
                    else if (startYear <= currentYear)
                    {
                        // Haven't decided how to handle this case; not needed at this time
                    }

                    //optimalWas.Add(newOptimalSwa);
                    //newOptimalSwa.CAI = newOptimalCai;
                    //previousOptimalCai = newOptimalSwa.CAI;

                    doNothingWas.Add(newDoNothingSwa);
                    newDoNothingSwa.CAI = newDoNothingCai;
                    previousDoNothingCai = newDoNothingSwa.CAI;
                    doNothingOptimalWas.Add(newDoNothingOptimalSwa);
                    //completeFiipsWas.Add(newFiipsSwa);
                    newFiipsSwa.CAI = newFiipsCai;
                    previousFiipsCai = newFiipsSwa.CAI;

                    if (year >= lastInspectionYear + detStartOffset - 1)
                    {
                        deteriorationYear++;
                    }
                }
            }

            structure.YearlyDoNothings = doNothingWas;
            //structure.YearlyOptimalWorkActions = optimalWas;
            structure.YearlyProgrammedWorkActions = completeFiipsWas;
            structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = doNothingOptimalWas;
            //CalculatePriorityIndexVariableFactors(structure, startYear, endYear, needsAnalysisInput);

            if (needsAnalysisInput != null)
            {
                CalculatePriorityScorePolicyEffects(structure, startYear, endYear, needsAnalysisInput);
            }
        }

        // new 01/11/2017
        // updated 4/4/2018 
        public void GetStructureWorkActions(Structure structure, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects, int detStartOffset, List<WorkActionCriteria> workActionCriteria,
                        NeedsAnalysisInput needsAnalysisInput = null, WisamType.ElementDeteriorationRates elementDeteriorationRates = WisamType.ElementDeteriorationRates.ByBrm)
        {
            List<StructureWorkAction> optimalWas = new List<StructureWorkAction>();
            List<StructureWorkAction> doNothingWas = new List<StructureWorkAction>();
            List<StructureWorkAction> completeFiipsWas = new List<StructureWorkAction>(); // includes do-nothings in years where there isn't do-something
            List<StructureWorkAction> doNothingOptimalWas = new List<StructureWorkAction>();

            if (structure.LastInspection != null)
            {
                // Get structure for DoNothing Optimal
                Structure structureForDoNothingOptimal = dbObj.GetStructure(structure.StructureId, false, structure.InterpolateNbi, false, structure.CountTpo);
                int lastInspectionYear = structure.LastInspection.InspectionDate.Year;
                int currentYear = DateTime.Now.Year;

                if (needsAnalysisInput != null)
                {
                    if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                    {
                        if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 7, 1)) >= 0)
                        {
                            lastInspectionYear = lastInspectionYear + 1;
                        }

                        if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 7, 1)) >= 0)
                        {
                            currentYear = currentYear + 1;
                        }
                    }
                    else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                    {
                        if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 11, 1)) >= 0)
                        {
                            lastInspectionYear = lastInspectionYear + 1;
                        }

                        if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 11, 1)) >= 0)
                        {
                            currentYear = currentYear + 1;
                        }
                    }
                }

                Cai lastCai = GetLastInspectionBasedCai(structure, caiFormulaId);
                Cai lastInspectionCai = new Cai(lastCai, lastInspectionYear, 0);
                Cai lastInspectionCaiFiips = new Cai(lastCai, lastInspectionYear, 0);
                Cai lastInspectionCaiDoNothing = new Cai(lastCai, lastInspectionYear, 0);
                //Cai lastInspectionCai = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);
                //Cai lastInspectionCaiFiips = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);
                //Cai lastInspectionCaiDoNothing = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);
                int deteriorationYear = 0;
                Cai previousOptimalCai = new Cai(lastInspectionCai, lastInspectionYear, 0);
                Cai previousDoNothingCai = new Cai(lastInspectionCai, lastInspectionYear, 0);
                Cai previousFiipsCai = new Cai(lastInspectionCai, lastInspectionYear, 0);
                DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", structure.LastInspection.InspectionDate.Year));
                DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", endYear));

                if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                {
                    startDate = Convert.ToDateTime(String.Format("07-01-{0}", structure.LastInspection.InspectionDate.Year - 1));
                    endDate = Convert.ToDateTime(String.Format("06-30-{0}", endYear));
                }
                else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                {
                    startDate = Convert.ToDateTime(String.Format("11-01-{0}", structure.LastInspection.InspectionDate.Year - 1));
                    endDate = Convert.ToDateTime(String.Format("10-31-{0}", endYear));
                }

                List<StructureWorkAction> fiipsWas = dbObj.GetProgrammedWorkActions(startDate, endDate, structure, needsAnalysisInput.CalendarType);

                for (int year = lastInspectionYear; year <= endYear; year++)
                {
                    StructureWorkAction newDoNothing = new StructureWorkAction(Code.DoNothing);
                    newDoNothing.WorkActionDesc = Code.DoNothingDesc;
                    newDoNothing.WorkActionYear = year;
                    newDoNothing.CombinedWorkActions = new List<StructureWorkAction>();

                    // Unconstrained Optimal
                    // Initialize optimal work action for year
                    StructureWorkAction newOptimalSwa = new StructureWorkAction(Code.DoNothing);
                    newOptimalSwa.WorkActionDesc = Code.DoNothingDesc;
                    newOptimalSwa.WorkActionYear = year;
                    newOptimalSwa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Initialize optimal work action CAI for year
                    Cai newOptimalCai = new Cai(previousOptimalCai, year, deteriorationYear);

                    // Initialize DN work action for year
                    StructureWorkAction newDoNothingSwa = new StructureWorkAction(Code.DoNothing);
                    newDoNothingSwa.WorkActionDesc = Code.DoNothingDesc;
                    newDoNothingSwa.WorkActionYear = year;
                    newDoNothingSwa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Initialize optimal work action for year under the DoNothing scenario
                    StructureWorkAction newDoNothingOptimalSwa = new StructureWorkAction(Code.DoNothing);
                    newDoNothingOptimalSwa.WorkActionDesc = Code.DoNothingDesc;
                    newDoNothingOptimalSwa.WorkActionYear = year;
                    newDoNothingOptimalSwa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Initialize optimal work action CAI for year
                    Cai newDoNothingCai = new Cai(previousDoNothingCai, year, deteriorationYear);

                    // FIIPS work action
                    Cai newFiipsCai = new Cai(previousFiipsCai, year, deteriorationYear);
                    //var yearlyFiipsWas = fiipsWas.Where(e => e.EstimatedCompletionDate.Year == year).ToList();
                    var yearlyFiipsWas = fiipsWas.Where(e => e.WorkActionYear == year).ToList();

                    /*
                    if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                    {
                        yearlyFiipsWas = fiipsWas.Where(e => e.WorkActionStateFiscalYear == year).ToList();
                    }
                    else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                    {
                        yearlyFiipsWas = fiipsWas.Where(e => e.WorkActionFederalFiscalYear == year).ToList();
                    }
                    */
                    StructureWorkAction newFiipsSwa = new StructureWorkAction();

                    if (yearlyFiipsWas.Count() > 0) // there's FIIPS programmed work action
                    {
                        //newFiipsSwa = dbObj.GetStructureWorkAction(yearlyFiipsWas.First().WorkActionCode); // only 
                        newFiipsSwa = yearlyFiipsWas.First();
                        //newFiipsSwa.WorkActionYear = year;
                        completeFiipsWas.Add(newFiipsSwa);
                        /*
                        foreach (var yearlyFiipsWa in yearlyFiipsWas)
                        {
                            completeFiipsWas.Add(yearlyFiipsWa);
                        }*/
                    }
                    else // no FIIPS programmed work action
                    {
                        newFiipsSwa = newDoNothing;
                        newFiipsSwa.WorkActionYear = year;
                        completeFiipsWas.Add(newFiipsSwa);
                    }

                    // Apply deterioration
                    // Example: lastInspectionYear = 2016, start deterioration year = 2016 + offset
                    //if (year >= lastInspectionYear + detStartOffset - 1)
                    if (deteriorationYear > 0) // see when deteriorationYear gets incremented
                    {
                        newOptimalCai.Basis = WisamType.CaiBases.Deterioration;
                        DeteriorateCai(newOptimalCai, previousOptimalCai, lastInspectionCai, deteriorateDefects, elementDeteriorationRates);

                        newDoNothingCai.Basis = WisamType.CaiBases.Deterioration;
                        DeteriorateCai(newDoNothingCai, previousDoNothingCai, lastInspectionCaiDoNothing, deteriorateDefects, elementDeteriorationRates);

                        newFiipsCai.Basis = WisamType.CaiBases.Deterioration;
                        DeteriorateCai(newFiipsCai, previousFiipsCai, lastInspectionCaiFiips, deteriorateDefects, elementDeteriorationRates);
                    }

                    // FIIPS only scenario
                    if (yearlyFiipsWas.Count() > 0)
                    {
                        ImproveCai(structure, newFiipsCai, yearlyFiipsWas, year, lastInspectionCaiFiips, false);
                    }

                    // Determine optimal work action
                    // Apply FIIPS up to (Start Year - 1), provided year is within FIIPS 6 year window from current year
                    if (startYear >= currentYear)
                    {
                        // Apply FIIPS
                        if (startYear <= currentYear + 5)
                        {
                            if (year < startYear)
                            {
                                // Apply FIIPS
                                if (yearlyFiipsWas.Count() > 0)
                                {
                                    ImproveCai(structure, newOptimalCai, yearlyFiipsWas, year, lastInspectionCai);
                                    ImproveCai(structure, newDoNothingCai, yearlyFiipsWas, year, lastInspectionCaiDoNothing);
                                    optimalWas.Add(newFiipsSwa);
                                    structure.ConstructionHistoryProjects.Add(newFiipsSwa);
                                    structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)  ", newFiipsSwa.WorkActionYear, newFiipsSwa.WorkActionDesc);
                                }
                            }
                        }
                        else if (startYear > currentYear + 5)
                        {
                            if (year <= currentYear + 5) // FIIPS through 6 year program
                            {
                                // Apply FIIPS
                                if (yearlyFiipsWas.Count() > 0)
                                {
                                    ImproveCai(structure, newOptimalCai, yearlyFiipsWas, year, lastInspectionCai);
                                    ImproveCai(structure, newDoNothingCai, yearlyFiipsWas, year, lastInspectionCaiDoNothing);
                                    optimalWas.Add(newFiipsSwa);
                                    structure.ConstructionHistoryProjects.Add(newFiipsSwa);
                                    structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)  ", newFiipsSwa.WorkActionYear, newFiipsSwa.WorkActionDesc);
                                }
                            }
                        }

                        // Start analysis
                        if (year >= startYear)
                        {
                            // Loop through work action rules in sequence
                            foreach (WorkActionCriteria wac in workActionCriteria)
                            {
                                // If buried structure and category of work action criteria is overlay, SKIP
                                if (structure.BuriedStructure && wac.RuleCategory.ToUpper().Equals("OLAY"))
                                {
                                    continue;
                                }

                                List<StructureWorkAction> optimalImprovements = new List<StructureWorkAction>();
                                // Rule evaluates to true
                                if (IsWorkActionEligible(wac, structure, newOptimalCai))
                                {
                                    StructureWorkAction potentialOptimal = dbObj.GetStructureWorkAction(wac.WorkActionCode, structure);
                                    potentialOptimal.WorkActionYear = year;

                                    // Check whether potential work action meets repeat frequency
                                    if (!potentialOptimal.CombinedWorkAction) // Single primary work action
                                    {
                                        if (!IsStructureWorkActionRepeatable(potentialOptimal, structure.ConstructionHistoryProjects, year))
                                        {
                                            continue;
                                        }

                                        if (!IsStructureWorkActionRepeatable(potentialOptimal, optimalWas, year))
                                        {
                                            continue;
                                        }
                                    }
                                    else // Primary work action is a combined work action
                                    {
                                        bool repeatable = true;

                                        foreach (var waCode in potentialOptimal.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            StructureWorkAction curCwa = dbObj.GetStructureWorkAction(waCode, structure);
                                            potentialOptimal.CombinedWorkActions.Add(curCwa);

                                            if (!IsStructureWorkActionRepeatable(curCwa, structure.ConstructionHistoryProjects, year))
                                            {
                                                repeatable = false;
                                                break;
                                            }

                                            if (!IsStructureWorkActionRepeatable(curCwa, optimalWas, year))
                                            {
                                                repeatable = false;
                                                break;
                                            }
                                        }

                                        if (!repeatable)
                                        {
                                            continue;
                                        }
                                    }

                                    newOptimalSwa = potentialOptimal;
                                    newOptimalSwa.ControllingCriteria = wac;
                                    newOptimalSwa.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                                    newOptimalSwa.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                                    optimalImprovements.Add(newOptimalSwa);
                                    ImproveCai(structure, newOptimalCai, optimalImprovements, year, lastInspectionCai);

                                    // Determine Incidental/Secondary work actions and their eligibility
                                    List<CombinedWorkAction> potentialIncidentals = dbObj.GetSecondaryWorkActions(newOptimalSwa.WorkActionCode);

                                    if (!structure.StructureType.Equals("BOX CULVERT"))
                                    {
                                        foreach (var potentialIncidental in potentialIncidentals)
                                        {
                                            StructureWorkAction curPotentialIncidentalWorkAction = dbObj.GetStructureWorkAction(potentialIncidental.SecondaryWorkActionCode, structure);
                                            newOptimalSwa.AllSecondaryWorkActions.Add(curPotentialIncidentalWorkAction); // All incidentals evaluated for eligibility

                                            if (potentialIncidental.BypassRule) // Rule bypass
                                            {
                                                curPotentialIncidentalWorkAction.BypassCriteria = true;
                                                newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                            }
                                            else // Else Evaluate eligibility
                                            {
                                                // Get pertinent work action criteria
                                                List<WorkActionCriteria> rules = dbObj.GetWorkActionCriteriaForGivenWorkAction(curPotentialIncidentalWorkAction.WorkActionCode);

                                                if (rules.Count() == 0) // No rules, so eligible
                                                {
                                                    newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                                }
                                                else
                                                {
                                                    foreach (var rule in rules)
                                                    {
                                                        if (IsWorkActionEligible(rule, structure, newOptimalCai))
                                                        {
                                                            curPotentialIncidentalWorkAction.ControllingCriteria = rule;
                                                            newOptimalSwa.SecondaryWorkActions.Add(curPotentialIncidentalWorkAction);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                            }

                            // Loop through work action rules in sequence
                            foreach (WorkActionCriteria wac in workActionCriteria)
                            {
                                // If buried structure and category of work action criteria is overlay, SKIP
                                if (structure.BuriedStructure && wac.RuleCategory.ToUpper().Equals("OLAY"))
                                {
                                    continue;
                                }

                                List<StructureWorkAction> optimalImprovements = new List<StructureWorkAction>();

                                // Rule evaluates to true
                                if (IsWorkActionEligible(wac, structureForDoNothingOptimal, newDoNothingCai))
                                {
                                    StructureWorkAction potentialOptimal = dbObj.GetStructureWorkAction(wac.WorkActionCode, structureForDoNothingOptimal);
                                    potentialOptimal.WorkActionYear = year;

                                    // Check whether potential work action meets repeat frequency
                                    if (!potentialOptimal.CombinedWorkAction) // Single primary work action
                                    {
                                        if (!IsStructureWorkActionRepeatable(potentialOptimal, structureForDoNothingOptimal.ConstructionHistoryProjects, year))
                                        {
                                            continue;
                                        }
                                    }
                                    else // Primary work action is a combined work action
                                    {
                                        bool repeatable = true;

                                        foreach (var waCode in potentialOptimal.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                        {
                                            StructureWorkAction curCwa = dbObj.GetStructureWorkAction(waCode, structureForDoNothingOptimal);
                                            potentialOptimal.CombinedWorkActions.Add(curCwa);

                                            if (!IsStructureWorkActionRepeatable(curCwa, structureForDoNothingOptimal.ConstructionHistoryProjects, year))
                                            {
                                                repeatable = false;
                                                break;
                                            }
                                        }

                                        if (!repeatable)
                                        {
                                            continue;
                                        }
                                    }

                                    newDoNothingOptimalSwa = potentialOptimal;
                                    newDoNothingOptimalSwa.ControllingCriteria = wac;
                                    newDoNothingOptimalSwa.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                                    newDoNothingOptimalSwa.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                                    optimalImprovements.Add(newOptimalSwa);
                                    break;
                                }
                            }
                        }
                    }
                    else if (startYear <= currentYear)
                    {
                        // Haven't decided how to handle this case; not needed at this time
                    }

                    optimalWas.Add(newOptimalSwa);
                    newOptimalSwa.CAI = newOptimalCai;
                    previousOptimalCai = newOptimalSwa.CAI;

                    doNothingWas.Add(newDoNothingSwa);
                    newDoNothingSwa.CAI = newDoNothingCai;
                    previousDoNothingCai = newDoNothingSwa.CAI;

                    doNothingOptimalWas.Add(newDoNothingOptimalSwa);
                    //completeFiipsWas.Add(newFiipsSwa);
                    newFiipsSwa.CAI = newFiipsCai;
                    previousFiipsCai = newFiipsSwa.CAI;

                    if (elementDeteriorationRates == WisamType.ElementDeteriorationRates.ByBrm)
                    {
                        deteriorationYear++;
                    }
                    else
                    {
                        if (year >= lastInspectionYear + detStartOffset - 1)
                        {
                            deteriorationYear++;
                        }
                    }
                }
            }

            structure.YearlyDoNothings = doNothingWas;
            structure.YearlyOptimalWorkActions = optimalWas;
            structure.YearlyProgrammedWorkActions = completeFiipsWas;
            structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition = doNothingOptimalWas;
            CalculatePriorityIndexVariableFactors(structure, startYear, endYear, needsAnalysisInput);

            if (needsAnalysisInput != null)
            {
                CalculatePriorityScorePolicyEffects(structure, startYear, endYear, needsAnalysisInput);
            }
        }

        public void DetermineSecondaryWorkActions(StructureWorkAction primaryWorkAction, Structure structure)
        {
            if (!structure.IsCulvert)
            {
                var primary = workActionsPrimary.Where(e => e.WorkActionCode.Equals(primaryWorkAction.WorkActionCode)).First();

                foreach (var potentialIncidental in primary.PotentialIncidentals)
                {
                    var potential = workActionsAll.Where(e => e.WorkActionCode.Equals(potentialIncidental.SecondaryWorkActionCode)).First();
                    primaryWorkAction.AllSecondaryWorkActions.Add(potential);

                    if (potential.BypassCriteria)
                    {
                        primaryWorkAction.SecondaryWorkActions.Add(potential);
                    }
                    else
                    {
                        List<WorkActionCriteria> rules = dbObj.GetWorkActionCriteriaForGivenWorkAction(potential.WorkActionCode);

                        if (rules.Count() == 0)
                        {
                            primaryWorkAction.SecondaryWorkActions.Add(potential);
                        }
                        else
                        {
                            foreach (var rule in rules)
                            {
                                //if (IsWorkActionCriteriaMet(structure, primaryWorkAction.CAI, rule.RuleFormula, rule))
                                //if (IsWorkActionCriteriaMet(rule, structure, primaryWorkAction.CAI))
                                if (IsWorkActionEligible(rule, structure, primaryWorkAction.CAI))
                                {
                                    primaryWorkAction.SecondaryWorkActions.Add(potential);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DetermineSecondaryWorkActions(Structure structure, StructureWorkAction primaryWorkAction)
        {
            if (!structure.IsCulvert)
            {
                List<CombinedWorkAction> potentialSecondaryWorkActions = dbObj.GetSecondaryWorkActions(primaryWorkAction.WorkActionCode);

                foreach (var potential in potentialSecondaryWorkActions)
                {
                    StructureWorkAction wa = dbObj.GetStructureWorkAction(potential.SecondaryWorkActionCode, structure);
                    primaryWorkAction.AllSecondaryWorkActions.Add(wa);

                    if (potential.BypassRule)
                    {
                        wa.BypassCriteria = true;
                        primaryWorkAction.SecondaryWorkActions.Add(wa);
                    }
                    else
                    {
                        List<WorkActionCriteria> rules = dbObj.GetWorkActionCriteriaForGivenWorkAction(wa.WorkActionCode);

                        if (rules.Count() == 0)
                        {
                            primaryWorkAction.SecondaryWorkActions.Add(wa);
                        }
                        else
                        {
                            foreach (var rule in rules)
                            {
                                //if (IsWorkActionCriteriaMet(structure, primaryWorkAction.CAI, rule.RuleFormula, rule))
                                //if (IsWorkActionCriteriaMet(rule, structure, primaryWorkAction.CAI))
                                if (IsWorkActionEligible(rule, structure, primaryWorkAction.CAI))
                                {
                                    wa.ControllingCriteria = rule;
                                    primaryWorkAction.SecondaryWorkActions.Add(wa);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<PriorityScorePolicyEffect> CalculatePriorityScorePolicyEffects(NeedsAnalysisInput needAnalysisInput, Structure str, int startYear, int endYear, StructureWorkAction swa = null)
        {
            List<PriorityScorePolicyEffect> effects = new List<PriorityScorePolicyEffect>();
            for (int year = startYear; year <= endYear; year++)
            {
                foreach (var policy in needAnalysisInput.PriorityScorePolicyEffects)
                {
                    string policyCriteria = policy.PolicyCriteria;
                    bool isCriteriaMet = false;

                    // Example Criteria: StructureTypeCode=50 (tied arch)
                    if (policyCriteria.Contains("StructureTypeCode"))
                    {
                        try
                        {
                            object fieldObj = str.GetType().GetProperty("StructureTypeCode").GetValue(str, null);
                            policyCriteria = policyCriteria.Replace("StructureTypeCode", fieldObj.ToString());
                        }
                        catch (Exception ex)
                        { }
                    }

                    if (policyCriteria.Contains("WorkActionCode") && swa != null)
                    {
                        try
                        {
                            object fieldObj = swa.GetType().GetProperty("WorkActionCode").GetValue(swa, null);
                            policyCriteria = policyCriteria.Replace("WorkActionCode", fieldObj.ToString());
                        }
                        catch (Exception ex)
                        { }
                    }

                    try
                    {
                        isCriteriaMet = Convert.ToBoolean(new DataTable().Compute(policyCriteria, null));
                    }
                    catch (Exception ex)
                    { }

                    if (isCriteriaMet)
                    {
                        effects.Add(new PriorityScorePolicyEffect(year, policy.ScoreEffect, policy.MathOperation, policy.Policy, policy.PolicyCriteria));
                    }
                }
            }

            return effects;
        }

        public void CalculatePriorityScorePolicyEffects(Structure str, int startYear, int endYear, NeedsAnalysisInput needAnalysisInput = null)
        {
            for (int year = startYear; year <= endYear; year++)
            {
                foreach (var policy in needAnalysisInput.PriorityScorePolicyEffects)
                {
                    string policyCriteria = policy.PolicyCriteria;
                    bool isCriteriaMet = false;

                    // Example Criteria: StructureTypeCode=50 (tied arch)
                    if (policyCriteria.Contains("StructureTypeCode"))
                    {
                        try
                        {
                            object fieldObj = str.GetType().GetProperty("StructureTypeCode").GetValue(str, null);
                            policyCriteria = policyCriteria.Replace("StructureTypeCode", fieldObj.ToString());
                        }
                        catch (Exception ex)
                        { }
                    }

                    try
                    {
                        isCriteriaMet = Convert.ToBoolean(new DataTable().Compute(policyCriteria, null));
                    }
                    catch (Exception ex)
                    { }

                    if (isCriteriaMet)
                    {
                        str.PriorityScorePolicyEffects.Add(new PriorityScorePolicyEffect(year, policy.ScoreEffect, policy.MathOperation, policy.Policy, policy.PolicyCriteria));
                    }
                }
            }
        }

        public void CalculatePriorityIndexVariableFactors(Structure str, StructureWorkAction swa, int startYear, int endYear)
        {
            for (int year = startYear; year <= endYear; year++)
            {
                // Get variable priority index factors for given year
                var variablePIFactors = swa.PriorityIndexFactors.Where(e => e.Year == year && e.PriorityIndexFactorType.ToUpper().Trim().Equals("VARIABLE"));
                StructureWorkAction optimalWa = swa;

                foreach (var variablePIFactor in variablePIFactors)
                {
                    float indexValue = 0;

                    switch (variablePIFactor.PriorityIndexFactorId)
                    {
                        case "ClosureRisk":
                            double minNbi = 0;
                            double nbiDeck = Math.Round(optimalWa.CAI.NbiRatings.DeckRatingVal, 2);
                            double nbiSup = Math.Round(optimalWa.CAI.NbiRatings.SuperstructureRatingVal, 2);
                            double nbiSub = Math.Round(optimalWa.CAI.NbiRatings.SubstructureRatingVal, 2);

                            if (!str.IsCulvert)
                            {
                                minNbi = Math.Min(Math.Min(nbiDeck, nbiSup), nbiSub);
                            }
                            else
                            {
                                minNbi = Math.Round(optimalWa.CAI.NbiRatings.CulvertRatingVal, 2);
                            }

                            if (minNbi <= 3.5)
                            {
                                try
                                {
                                    indexValue = Convert.ToSingle(0.1605 * minNbi * minNbi - 1.2602 * minNbi + 2.4456);
                                    if (indexValue > 1)
                                    {
                                        indexValue = 1;
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }

                            variablePIFactor.FieldValue = minNbi.ToString();
                            break;

                        case "StructureAge":
                            int strAge = year - str.YearBuiltActual;

                            if (str.YearBuilt > str.YearBuiltActual)
                            {
                                if (year >= str.YearBuilt)
                                {
                                    strAge = year - str.YearBuilt;
                                }
                            }

                            indexValue = Convert.ToSingle(Math.Max(Math.Min(1, 0.0132 * strAge - 0.3096), 0));
                            variablePIFactor.FieldValue = strAge.ToString();
                            break;

                        case "RideQuality":
                            var wearingSurfaces = optimalWa.CAI.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).ToList();
                            var totalQuantity = 0;
                            var wearingSurfacesCs2AndWorseTotalQuantity = 0;
                            int parentElement = 0;

                            foreach (var wearingSurface in wearingSurfaces)
                            {
                                if (parentElement == 0)
                                {
                                    parentElement = wearingSurface.ParentElemNum;
                                    totalQuantity += optimalWa.CAI.AllElements.Where(e => e.ElemNum == parentElement).First().TotalQuantity;
                                }
                                else
                                {
                                    if (parentElement != wearingSurface.ParentElemNum)
                                    {
                                        totalQuantity += optimalWa.CAI.AllElements.Where(e => e.ElemNum == wearingSurface.ParentElemNum).First().TotalQuantity;
                                    }
                                }

                                wearingSurfacesCs2AndWorseTotalQuantity += wearingSurface.Cs2Quantity + wearingSurface.Cs3Quantity + wearingSurface.Cs4Quantity;
                            }

                            if (wearingSurfacesCs2AndWorseTotalQuantity == 0)
                            {
                                indexValue = 0;
                                variablePIFactor.FieldValue = "0/" + totalQuantity.ToString();
                            }
                            else
                            {
                                variablePIFactor.FieldValue = wearingSurfacesCs2AndWorseTotalQuantity.ToString() + "/" + totalQuantity.ToString();
                                indexValue = Convert.ToSingle(wearingSurfacesCs2AndWorseTotalQuantity * 1.0 / totalQuantity);
                            }
                            /*
                            if (wearingSurface.Count > 0)
                            {
                                var parentElemTotalQuantity = optimalWa.CAI.CaiElements.Where(e => e.ElemNum == wearingSurface.First().ParentElemNum).First().TotalQuantity;
                                indexValue = Convert.ToSingle(wearingSurface.First().Cs2Quantity * 1.0 / parentElemTotalQuantity / 0.4);
                            }
                            */
                            break;
                    }
                    // end switch (variablePIFactor.PriorityIndexFactorId)

                    if (indexValue > 1)
                    {
                        indexValue = 1;
                    }

                    variablePIFactor.IndexValue = indexValue;
                    variablePIFactor.Score = variablePIFactor.IndexValue * variablePIFactor.PriorityIndexFactorWeight;
                    var piCat = swa.PriorityIndexCategories.Where(e => e.Year == year && e.PriorityIndexCategoryKey == variablePIFactor.PriorityIndexCategoryKey).ToList().First();
                    piCat.Score += variablePIFactor.Score;

                    if (piCat.Score > piCat.PriorityIndexMaxValue)
                    {
                        piCat.Score = piCat.PriorityIndexMaxValue;
                    }
                }
            }
        }

        public void CalculatePriorityIndexVariableFactors(Structure str, int startYear, int endYear, NeedsAnalysisInput needAnalysisInput = null)
        {
            for (int year = startYear; year <= endYear; year++)
            {
                // Get variable priority index factors for given year
                var variablePIFactors = str.PriorityIndexFactors.Where(e => e.Year == year && e.PriorityIndexFactorType.ToUpper().Trim().Equals("VARIABLE"));
                StructureWorkAction optimalWa = str.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == year).First();

                foreach (var variablePIFactor in variablePIFactors)
                {
                    float indexValue = 0;

                    switch (variablePIFactor.PriorityIndexFactorId)
                    {
                        case "ClosureRisk":
                            double minNbi = 0;
                            double nbiDeck = Math.Round(optimalWa.CAI.NbiRatings.DeckRatingVal, 2);
                            double nbiSup = Math.Round(optimalWa.CAI.NbiRatings.SuperstructureRatingVal, 2);
                            double nbiSub = Math.Round(optimalWa.CAI.NbiRatings.SubstructureRatingVal, 2);

                            if (!str.IsCulvert)
                            {
                                minNbi = Math.Min(Math.Min(nbiDeck, nbiSup), nbiSub);
                            }
                            else
                            {
                                minNbi = Math.Round(optimalWa.CAI.NbiRatings.CulvertRatingVal, 2);
                            }

                            if (minNbi <= 3.5)
                            {
                                try
                                {
                                    indexValue = Convert.ToSingle(0.1605 * minNbi * minNbi - 1.2602 * minNbi + 2.4456);
                                    if (indexValue > 1)
                                    {
                                        indexValue = 1;
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }

                            variablePIFactor.FieldValue = minNbi.ToString();
                            break;

                        case "StructureAge":
                            int strAge = year - str.YearBuiltActual;

                            if (str.YearBuilt > str.YearBuiltActual)
                            {
                                if (year >= str.YearBuilt)
                                {
                                    strAge = year - str.YearBuilt;
                                }
                            }

                            indexValue = Convert.ToSingle(Math.Max(Math.Min(1, 0.0132 * strAge - 0.3096), 0));
                            variablePIFactor.FieldValue = strAge.ToString();
                            break;

                        case "RideQuality":
                            var wearingSurfaces = optimalWa.CAI.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).ToList();
                            var totalQuantity = 0;
                            var wearingSurfacesCs2AndWorseTotalQuantity = 0;
                            int parentElement = 0;

                            foreach (var wearingSurface in wearingSurfaces)
                            {
                                if (parentElement == 0)
                                {
                                    parentElement = wearingSurface.ParentElemNum;
                                    totalQuantity += optimalWa.CAI.AllElements.Where(e => e.ElemNum == parentElement).First().TotalQuantity;
                                }
                                else
                                {
                                    if (parentElement != wearingSurface.ParentElemNum)
                                    {
                                        totalQuantity += optimalWa.CAI.AllElements.Where(e => e.ElemNum == wearingSurface.ParentElemNum).First().TotalQuantity;
                                    }
                                }

                                wearingSurfacesCs2AndWorseTotalQuantity += wearingSurface.Cs2Quantity + wearingSurface.Cs3Quantity + wearingSurface.Cs4Quantity;
                            }

                            if (wearingSurfacesCs2AndWorseTotalQuantity == 0)
                            {
                                indexValue = 0;
                                variablePIFactor.FieldValue = "0/" + totalQuantity.ToString();
                            }
                            else
                            {
                                variablePIFactor.FieldValue = wearingSurfacesCs2AndWorseTotalQuantity.ToString() + "/" + totalQuantity.ToString();
                                indexValue = Convert.ToSingle(wearingSurfacesCs2AndWorseTotalQuantity * 1.0 / totalQuantity);
                            }
                            /*
                            if (wearingSurface.Count > 0)
                            {
                                var parentElemTotalQuantity = optimalWa.CAI.CaiElements.Where(e => e.ElemNum == wearingSurface.First().ParentElemNum).First().TotalQuantity;
                                indexValue = Convert.ToSingle(wearingSurface.First().Cs2Quantity * 1.0 / parentElemTotalQuantity / 0.4);
                            }
                            */
                            break;
                    }
                    // end switch (variablePIFactor.PriorityIndexFactorId)

                    if (indexValue > 1)
                    {
                        indexValue = 1;
                    }

                    variablePIFactor.IndexValue = indexValue;
                    variablePIFactor.Score = variablePIFactor.IndexValue * variablePIFactor.PriorityIndexFactorWeight;
                    var piCat = str.PriorityIndexCategories.Where(e => e.Year == year && e.PriorityIndexCategoryKey == variablePIFactor.PriorityIndexCategoryKey).ToList().First();
                    piCat.Score += variablePIFactor.Score;

                    if (piCat.Score > piCat.PriorityIndexMaxValue)
                    {
                        piCat.Score = piCat.PriorityIndexMaxValue;
                    }
                }
            }
        }

        // new 02/19/2016
        public List<StructureWorkAction> GetStructureOptimalWorkCandidates(Structure structure, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects, int detStart, bool applyCurrentYearFiipsWorkAction, bool improvementWorkActions = false)
        {
            List<StructureWorkAction> optimalWcs = new List<StructureWorkAction>();
            //List<DateTime> inspectionDates = dbObj.GetElementInspectionDates(structure.StructureId);


            Cai lastInspectionCai = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);

            //Cai lastInspectionCai = GetLastInspectionBasedCai(structure);

            if (lastInspectionCai != null)
            {
                if (improvementWorkActions)
                {
                    workActionCriteria = dbObj.GetWorkActionCriteria(improvementWorkActions);
                }

                Cai previousCai = new Cai(lastInspectionCai, lastInspectionCai.Year, 0);
                previousCai.DebugInfo = lastInspectionCai.DebugInfo;
                int detYear = 0;

                for (int year = startYear; year <= endYear; year++)
                {
                    StructureWorkAction swa = new StructureWorkAction();
                    swa.WorkActionCode = Code.DoNothing;
                    swa.WorkActionDesc = Code.DoNothingDesc;
                    swa.WorkActionYear = year;
                    swa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Create new CAI
                    Cai newCai = null;

                    if (detYear < detStart)
                    {
                        newCai = new Cai(previousCai, year, 0);
                    }
                    else
                    {
                        //newCai = new Cai(previousCai, year, detYear - detStart + 1);
                        newCai = new Cai(previousCai, year, previousCai.AllElements.First().DeteriorationYear + 1);
                    }
                    newCai.Basis = WisamType.CaiBases.Deterioration;

                    if (applyCurrentYearFiipsWorkAction && year == startYear)
                    {
                        List<StructureWorkAction> improvements = GetStructureProgrammedWorkCandidates(structure, startYear, startYear, caiFormulaId, deteriorateDefects);

                        if (improvements.Count() > 0)
                        {
                            swa = improvements.First();
                            ImproveCai(structure, newCai, improvements, year, lastInspectionCai);

                            if (!structure.AddedFiipsCurrentYearProject)
                            {
                                if (swa.WorkActionCode != Code.DoNothing)
                                {
                                    structure.ConstructionHistory += String.Format("({0}){1}(FIIPS)", year, swa.WorkActionDesc);
                                    structure.ConstructionHistoryProjects.Add(swa);

                                    if (swa.WorkActionCode == Code.NewStructure || swa.WorkActionCode == Code.ReplaceStructure)
                                    {
                                        structure.NewStructureInCurrentYear = true;
                                    }
                                }
                                structure.AddedFiipsCurrentYearProject = true;
                            }
                        }

                        optimalWcs.Add(swa);
                        swa.CAI = newCai;
                        previousCai = swa.CAI;
                        detYear++;
                        continue;
                    }

                    if (detYear < detStart)
                    {
                        newCai.Basis = WisamType.CaiBases.Inspection;
                        newCai.DebugInfo = previousCai.DebugInfo;
                    }

                    swa.CAI = newCai;


                    // Deteriorate CAI
                    if (detYear >= detStart)
                    {
                        if (year == 2032 || year == 2033)
                        {
                            int zz = 0;
                        }
                        DeteriorateCai(newCai, previousCai, lastInspectionCai, deteriorateDefects);
                    }

                    // Determine optimal work candidate
                    foreach (WorkActionCriteria wac in workActionCriteria)
                    {
                        //if (IsWorkActionCriteriaMet(structure, swa.CAI, wac.RuleFormula, wac))
                        //if (IsWorkActionCriteriaMet(wac, structure, swa.CAI))
                        if (IsWorkActionEligible(wac, structure, swa.CAI))
                        {
                            StructureWorkAction optimalWc = dbObj.GetStructureWorkAction(wac.WorkActionCode, structure);
                            //StructureWorkAction optimalWc = dbObj.GetStructureWorkAction(wac.WorkActionCode);

                            optimalWc.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                            optimalWc.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;

                            // Check repeatability
                            if (!optimalWc.CombinedWorkAction)
                            {
                                if (!IsStructureWorkActionRepeatable(optimalWc, structure.ConstructionHistoryProjects, year))
                                {
                                    continue;
                                }

                                if (!IsStructureWorkActionRepeatable(optimalWc, optimalWcs, year))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                bool repeatable = true;
                                foreach (var code in optimalWc.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    // StructureWorkAction cwa = dbObj.GetStructureWorkAction(code);
                                    StructureWorkAction cwa = dbObj.GetStructureWorkAction(code, structure);
                                    optimalWc.Cost += cwa.Cost;

                                    if (!IsStructureWorkActionRepeatable(cwa, structure.ConstructionHistoryProjects, year))
                                    {
                                        repeatable = false;
                                        break;
                                    }

                                    if (!IsStructureWorkActionRepeatable(cwa, optimalWcs, year))
                                    {
                                        repeatable = false;
                                        break;
                                    }
                                }

                                if (!repeatable)
                                {
                                    continue;
                                }
                            }

                            swa = optimalWc;
                            swa.CAI = newCai;
                            swa.WorkActionYear = year;
                            swa.ControllingCriteria = wac;

                            swa.CombinedWorkActions = new List<StructureWorkAction>();

                            if (!structure.StructureType.Equals("BOX CULVERT"))
                            {
                                if (swa.CombinedWorkAction)
                                {
                                    foreach (var code in swa.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                    {
                                        //StructureWorkAction cwa = dbObj.GetStructureWorkAction(code);
                                        StructureWorkAction cwa = dbObj.GetStructureWorkAction(code, structure);
                                        //swa.Cost += cwa.Cost;
                                        swa.CombinedWorkActions.Add(cwa);
                                    }
                                }
                            }

                            List<StructureWorkAction> improvements = new List<StructureWorkAction>();
                            improvements.Add(swa);

                            // Check eligibility of specific secondary work actions (incidentals)
                            swa.AllSecondaryWorkActions = new List<StructureWorkAction>();
                            swa.SecondaryWorkActions = new List<StructureWorkAction>(); // List of eligible secondary work actions of primary work action

                            // Grab secondary work actions for current primary work action and evaluate their eligibility
                            List<CombinedWorkAction> combinedWorkActions = dbObj.GetSecondaryWorkActions(swa.WorkActionCode);

                            if (!structure.StructureType.Equals("BOX CULVERT"))
                            {
                                foreach (CombinedWorkAction combinedWorkAction in combinedWorkActions)
                                {
                                    //StructureWorkAction secondaryWa = dbObj.GetStructureWorkAction(combinedWorkAction.SecondaryWorkActionCode);
                                    StructureWorkAction secondaryWa = dbObj.GetStructureWorkAction(combinedWorkAction.SecondaryWorkActionCode, structure);
                                    swa.AllSecondaryWorkActions.Add(secondaryWa);

                                    if (combinedWorkAction.BypassRule)
                                    {
                                        secondaryWa.BypassCriteria = true;
                                        swa.SecondaryWorkActions.Add(secondaryWa);
                                        //improvements.Add(secondaryWa);
                                    }
                                    else
                                    {
                                        List<WorkActionCriteria> swacs = dbObj.GetWorkActionCriteriaForGivenWorkAction(combinedWorkAction.SecondaryWorkActionCode);

                                        if (swacs.Count() == 0) // no eligibility criteria for this secondary work action
                                        {
                                            swa.SecondaryWorkActions.Add(secondaryWa);
                                        }
                                        else
                                        {
                                            foreach (var swac in swacs)
                                            {
                                                //if (IsWorkActionCriteriaMet(structure, swa.CAI, swac.RuleFormula, swac))
                                                //if (IsWorkActionCriteriaMet(swac, structure, swa.CAI))
                                                if (IsWorkActionEligible(swac, structure, swa.CAI))
                                                {
                                                    secondaryWa.ControllingCriteria = swac;
                                                    swa.SecondaryWorkActions.Add(secondaryWa);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // Finish: Check eligibility of specific secondary work actions

                            //ImproveCai(swa.CAI, improvements);

                            // CHECK!!!!!!
                            if (detYear < detStart && structure.NewStructureInCurrentYear)
                            {

                            }
                            else
                            {
                                ImproveCai(structure, swa.CAI, improvements, year, lastInspectionCai);
                            }

                            break;
                        }
                    }

                    optimalWcs.Add(swa);
                    previousCai = swa.CAI;
                    detYear++;
                }
            } // end - if (inspectionDates.Count > 0)

            structure.YearlyOptimalWorkActions = optimalWcs;
            return optimalWcs;
        }

        // new 11/25/2015
        /*
        public List<StructureWorkAction> GetStructureOptimalWorkCandidates(Structure structure, int startYear, int endYear, int caiFormulaId, bool improvementWorkActions = false)
        {
            List<StructureWorkAction> optimalWcs = new List<StructureWorkAction>();
            List<DateTime> inspectionDates = dbObj.GetElementInspectionDates(structure.StructureId);

            if (inspectionDates.Count > 0)
            {
                if (improvementWorkActions)
                {
                    workActionCriteria = dbObj.GetWorkActionCriteria(improvementWorkActions);
                }

                Cai lastInspectionCai = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);
                Cai previousCai = new Cai(lastInspectionCai, lastInspectionCai.Year, 0);
                previousCai.DebugInfo = lastInspectionCai.DebugInfo;
                int detYear = 0;

                for (int year = startYear; year <= endYear; year++)
                {
                    //workActionCriteria = dbObj.GetWorkActionCriteria(programmableWorkActionsOnly);
                    StructureWorkAction swa = new StructureWorkAction();
                    swa.WorkActionCode = Code.DoNothing;
                    swa.WorkActionDesc = Code.DoNothingDesc;
                    swa.WorkActionYear = year;
                    swa.CombinedWorkActions = new List<StructureWorkAction>();

                    // Create new CAI
                    Cai newCai = new Cai(previousCai, year, detYear);
                    newCai.Basis = WisamType.CaiBases.Deterioration;
                    if (detYear == 0)
                    {
                        newCai.Basis = WisamType.CaiBases.Inspection;
                        newCai.DebugInfo = previousCai.DebugInfo;
                    }
                    swa.CAI = newCai;

                    // Deteriorate CAI
                    if (detYear > 0)
                    {
                        DeteriorateCai(newCai, previousCai, lastInspectionCai, deteriorateDefects);
                    }

                    // Determine optimal work candidate
                    foreach (WorkActionCriteria wac in workActionCriteria)
                    {
                        if (IsWorkActionCriteriaMet(structure, swa.CAI, wac.RuleFormula, wac))
                        {
                            StructureWorkAction optimalWc = dbObj.GetStructureWorkAction(wac.WorkActionCode, structure);
                            //StructureWorkAction optimalWc = dbObj.GetStructureWorkAction(wac.WorkActionCode);
                            optimalWc.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                            optimalWc.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                           
                            if (!optimalWc.CombinedWorkAction)
                            {
                                if (!IsStructureWorkActionRepeatable(optimalWc, optimalWcs, year))
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                bool repeatable = true;
                                foreach (var code in optimalWc.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                   // StructureWorkAction cwa = dbObj.GetStructureWorkAction(code);
                                    StructureWorkAction cwa = dbObj.GetStructureWorkAction(code, structure);
                                    optimalWc.Cost += cwa.Cost;

                                    if (!IsStructureWorkActionRepeatable(cwa, optimalWcs, year))
                                    {
                                        repeatable = false;
                                        break;
                                    }
                                }

                                if (!repeatable)
                                {
                                    continue;
                                }
                            }

                            swa = optimalWc;
                            swa.CAI = newCai;
                            swa.WorkActionYear = year;
                            swa.ControllingCriteria = wac;

                            switch (swa.WorkActionCode)
                            {
                                case Code.ReplaceStructure:
                                    structure.YearBuilt = year;
                                    structure.DeckBuiltYear = year;
                                    structure.NumOlays = 0;
                                    structure.NumThinPolymerOverlays = 0;
                                    structure.LoadCapacity = 5;
                                    structure.DeckBuiltYear = year;
                                    structure.SufficiencyNumber = 99.9;
                                    structure.ScourCritical = false;
                                    structure.ScourCriticalRating = "8";
                                    structure.FractureCritical = false;
                                    structure.StructurallyDeficient = false;
                                    structure.FunctionalObsolete = false;
                                    structure.FunctionalObsoleteDueToApproachRoadwayAlignment = false;
                                    structure.FunctionalObsoleteDueToDeckGeometry = false;
                                    structure.FunctionalObsoleteDueToStructureEvaluation = false;
                                    structure.FunctionalObsoleteDueToVerticalClearance = false;
                                    structure.FunctionalObsoleteDueToWaterwayAdequacy = false;
                                    break;
                                case Code.ReplaceDeckRaiseStructure:
                                    structure.DeckBuiltYear = year;
                                    structure.NumOlays = 0;
                                    structure.NumThinPolymerOverlays = 0;
                                    structure.FunctionalObsoleteDueToVerticalClearance = false;
                                    EvaluateFunctionalObsolete(structure);
                                    break;
                                case Code.ReplaceDeck:
                                case Code.ReplaceSuperstructure:
                                case Code.ReplaceDeckPaintComplete:
                                    structure.DeckBuiltYear = year;
                                    structure.NumOlays = 0;
                                    structure.NumThinPolymerOverlays = 0;
                                    break;
                                case Code.OverlayConcrete:
                                case Code.OverlayConcretePaint:
                                case Code.OverlayConcreteNewJoints:
                                case Code.OverlayConcreteNewRailJoints:
                                case Code.OverlayHma:
                                case Code.OverlayPma:
                                case Code.OverlayPolyesterPolymer:
                                    if (structure.NumOlays == 0)
                                    {
                                        // Add to CAI's list of elements - All and CAI
                                        UpdateElements(structure, swa.CAI, swa.WorkActionCode, structure.OverlayQuantity, detYear);
                                    }
                                    structure.NumOlays++;
                                    break;
                                case Code.OverlayThinPolymer:
                                case Code.OverlayThinPolymerNewJoints:
                                    if (structure.NumOlays == 0)
                                    {
                                        // Add to CAI's list of elements - All and CAI
                                        UpdateElements(structure, swa.CAI, swa.WorkActionCode, structure.OverlayQuantity, detYear);
                                    }
                                    structure.NumThinPolymerOverlays++;
                                    break;
                                case Code.RaiseStructure:
                                    structure.FunctionalObsoleteDueToVerticalClearance = false;
                                    EvaluateFunctionalObsolete(structure);
                                    break;
                                case Code.WidenBridge:
                                    structure.FunctionalObsoleteDueToDeckGeometry = false;
                                    EvaluateFunctionalObsolete(structure);
                                    break;
                            }

                            swa.CombinedWorkActions = new List<StructureWorkAction>();
                            if (swa.CombinedWorkAction)
                            {
                                foreach (var code in swa.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    //StructureWorkAction cwa = dbObj.GetStructureWorkAction(code);
                                    StructureWorkAction cwa = dbObj.GetStructureWorkAction(code, structure);
                                    //swa.Cost += cwa.Cost;
                                    swa.CombinedWorkActions.Add(cwa);

                                    switch (code)
                                    {
                                        case Code.OverlayConcrete:
                                        case Code.OverlayConcretePaint:
                                        case Code.OverlayConcreteNewJoints:
                                        case Code.OverlayConcreteNewRailJoints:
                                        case Code.OverlayHma:
                                        case Code.OverlayPma:
                                        case Code.OverlayPolyesterPolymer:
                                            if (structure.NumOlays == 0)
                                            {
                                                // Add to CAI's list of elements - All and CAI
                                                UpdateElements(structure, swa.CAI, code, structure.OverlayQuantity, detYear);
                                            }
                                            structure.NumOlays++;
                                            break;
                                        case Code.OverlayThinPolymer:
                                        case Code.OverlayThinPolymerNewJoints:
                                            if (structure.NumOlays == 0)
                                            {
                                                // Add to CAI's list of elements - All and CAI
                                                UpdateElements(structure, swa.CAI, code, structure.OverlayQuantity, detYear);
                                            }
                                            structure.NumThinPolymerOverlays++;
                                            break;
                                        case Code.RaiseStructure:
                                            structure.FunctionalObsoleteDueToVerticalClearance = false;
                                            EvaluateFunctionalObsolete(structure);
                                            break;
                                        case Code.WidenBridge:
                                            structure.FunctionalObsoleteDueToDeckGeometry = false;
                                            EvaluateFunctionalObsolete(structure);
                                            break;
                                    }
                                }
                            }

                            List<StructureWorkAction> improvements = new List<StructureWorkAction>();
                            improvements.Add(swa);

                            // Check eligibility of specific secondary work actions
                            swa.AllSecondaryWorkActions = new List<StructureWorkAction>();
                            swa.SecondaryWorkActions = new List<StructureWorkAction>(); // List of eligible secondary work actions of primary work action

                            // Grab secondary work actions for current primary work action and evaluate their eligibility
                            List<CombinedWorkAction> combinedWorkActions = dbObj.GetSecondaryWorkActions(swa.WorkActionCode);

                            foreach (CombinedWorkAction combinedWorkAction in combinedWorkActions)
                            {
                                //StructureWorkAction secondaryWa = dbObj.GetStructureWorkAction(combinedWorkAction.SecondaryWorkActionCode);
                                StructureWorkAction secondaryWa = dbObj.GetStructureWorkAction(combinedWorkAction.SecondaryWorkActionCode, structure);
                                swa.AllSecondaryWorkActions.Add(secondaryWa);

                                if (combinedWorkAction.BypassRule)
                                {
                                    secondaryWa.BypassCriteria = true;
                                    swa.SecondaryWorkActions.Add(secondaryWa);
                                    //improvements.Add(secondaryWa);
                                }
                                else
                                {
                                    List<WorkActionCriteria> swacs = dbObj.GetWorkActionCriteriaForGivenWorkAction(combinedWorkAction.SecondaryWorkActionCode);

                                    if (swacs.Count() == 0) // no eligibility criteria for this secondary work action
                                    {
                                        swa.SecondaryWorkActions.Add(secondaryWa);
                                    }
                                    else
                                    {
                                        foreach (var swac in swacs)
                                        {
                                            if (IsWorkActionCriteriaMet(structure, swa.CAI, swac.RuleFormula, swac))
                                            {
                                                secondaryWa.ControllingCriteria = swac;
                                                swa.SecondaryWorkActions.Add(secondaryWa);
                                                break;
                                            }
                                        }
                                    }
                                }

                                switch (secondaryWa.WorkActionCode)
                                {
                                    case Code.OverlayConcrete:
                                    case Code.OverlayConcretePaint:
                                    case Code.OverlayConcreteNewJoints:
                                    case Code.OverlayConcreteNewRailJoints:
                                    case Code.OverlayHma:
                                    case Code.OverlayPma:
                                    case Code.OverlayPolyesterPolymer:
                                        if (structure.NumOlays == 0)
                                        {
                                            // Add to CAI's list of elements - All and CAI
                                            UpdateElements(structure, swa.CAI, secondaryWa.WorkActionCode, structure.OverlayQuantity, detYear);
                                        }
                                        structure.NumOlays++;
                                        break;
                                    case Code.OverlayThinPolymer:
                                    case Code.OverlayThinPolymerNewJoints:
                                        if (structure.NumOlays == 0)
                                        {
                                            // Add to CAI's list of elements - All and CAI
                                            UpdateElements(structure, swa.CAI, secondaryWa.WorkActionCode, structure.OverlayQuantity, detYear);
                                        }
                                        structure.NumThinPolymerOverlays++;
                                        break;
                                    case Code.RaiseStructure:
                                        structure.FunctionalObsoleteDueToVerticalClearance = false;
                                        EvaluateFunctionalObsolete(structure);
                                        break;
                                    case Code.WidenBridge:
                                        structure.FunctionalObsoleteDueToDeckGeometry = false;
                                        EvaluateFunctionalObsolete(structure);
                                        break;
                                }
                            }
                            // Finish: Check eligibility of specific secondary work actions

                            ImproveCai(swa.CAI, improvements);

                            break;
                        }
                    }

                    optimalWcs.Add(swa);
                    previousCai = swa.CAI;
                    detYear++;
                }
            } // end - if (inspectionDates.Count > 0)

            structure.YearlyOptimalWorkActions = optimalWcs;
            return optimalWcs;
        }
        */



        // TODO: Make more concise



        //02/22/2016
        public List<StructureWorkAction> GetStructureProgrammedWorkCandidates(Structure structure, int startYear, int endYear, int detStart, int caiFormulaId, bool deteriorateDefects)
        {
            // Grab programmed work candidates for given structure
            DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", startYear));
            DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", endYear));
            List<StructureWorkAction> programmedWcs = dbObj.GetProgrammedWorkActions(startDate, endDate, structure.StructureId);

            // Complete list of work candidates; insert do-nothing for years that don't have programmed work candidates
            List<StructureWorkAction> completeWcs = new List<StructureWorkAction>();

            for (int i = startYear; i <= endYear; i++)
            {
                var yearWcs = programmedWcs.Where(e => e.EstimatedCompletionDate.Year == i).ToList();

                if (yearWcs.Count == 0)
                {
                    StructureWorkAction swa = new StructureWorkAction();
                    swa.WorkActionYear = i;
                    swa.WorkActionCode = Code.DoNothing;
                    swa.WorkActionDesc = Code.DoNothingDesc;

                    //swa.CAI = new Cai();
                    completeWcs.Add(swa);
                }
                else
                {
                    foreach (var yearWc in yearWcs)
                    {
                        yearWc.CAI = new Cai();
                        //yearWc.Cost = dbObj.GetStructureWorkActionCost(yearWc.WorkActionCode, )
                        completeWcs.Add(yearWc);
                    }
                }
            }

            GetCais(structure, startYear, endYear, caiFormulaId, completeWcs, detStart, deteriorateDefects);
            return completeWcs;
        }

        public List<StructureWorkAction> GetStructureProgrammedWorkCandidates(Structure structure, int startYear, int endYear, int caiFormulaId, bool deteriorateDefects)
        {
            // Grab programmed work candidates for given structure
            DateTime startDate = Convert.ToDateTime(String.Format("01-01-{0}", startYear));
            DateTime endDate = Convert.ToDateTime(String.Format("12-31-{0}", endYear));
            List<StructureWorkAction> programmedWcs = dbObj.GetProgrammedWorkActions(startDate, endDate, structure.StructureId);

            // Complete list of work candidates; insert do-nothing for years that don't have programmed work candidates
            List<StructureWorkAction> completeWcs = new List<StructureWorkAction>();

            for (int i = startYear; i <= endYear; i++)
            {
                var yearWcs = programmedWcs.Where(e => e.EstimatedCompletionDate.Year == i).ToList();

                if (yearWcs.Count == 0)
                {
                    StructureWorkAction swa = new StructureWorkAction();
                    swa.WorkActionYear = i;
                    swa.WorkActionCode = Code.DoNothing;
                    swa.WorkActionDesc = Code.DoNothingDesc;

                    //swa.CAI = new Cai();
                    completeWcs.Add(swa);
                }
                else
                {
                    foreach (var yearWc in yearWcs)
                    {
                        yearWc.CAI = new Cai();
                        //yearWc.Cost = dbObj.GetStructureWorkActionCost(yearWc.WorkActionCode, )
                        completeWcs.Add(yearWc);
                    }
                }
            }

            GetCais(structure, startYear, endYear, caiFormulaId, completeWcs, deteriorateDefects);
            return completeWcs;
        }
        #endregion UI Methods

        #region NeedsAnalysisEventHandlers
        public void AnalyzeStructures(NeedsAnalysisInput needsAnalysisInput)
        {
            // Write the input file
            bool writtenBridgeInventoryFile = false;

            // Note: CAI formula's no longer selectable by the user
            caiFormulaObj = dbObj.GetCaiFormula(needsAnalysisInput.CaiFormulaId);

            // Structure IDs to be analyzed
            List<string> structureIds = new List<string>();

            if (needsAnalysisInput.StructureSelection.Equals("ByRegions"))
            {
                structureIds = dbObj.GetStructuresByRegions(needsAnalysisInput.RegionNumbers, needsAnalysisInput.IncludeStateOwned,
                                                            needsAnalysisInput.IncludeLocalOwned, needsAnalysisInput.IncludeCStructures);
            }
            else if (needsAnalysisInput.StructureSelection.Equals("ByIds"))
            {
                structureIds = needsAnalysisInput.StructureIds;
            }
            else if (needsAnalysisInput.StructureSelection.Equals("ByFundings"))
            {
                structureIds = dbObj.GetStructuresByFundingSources(needsAnalysisInput);
            }

            // Determine rules in effect based on selected eligible primary work actions
            List<StructureWorkAction> selectedPrimaryworkActions = new List<StructureWorkAction>();
            List<WorkActionCriteria> workActionRulesInEffect = new List<WorkActionCriteria>();

            if (needsAnalysisInput.EligiblePrimaryWorkActionCodes.Count > 0)
            {
                workActionRulesInEffect = dbObj.GetWorkActionCriteria(true, String.Join(",", needsAnalysisInput.EligiblePrimaryWorkActionCodes.Select(x => x.ToString()).ToArray()));
                foreach (var workActionCode in needsAnalysisInput.EligiblePrimaryWorkActionCodes)
                {
                    selectedPrimaryworkActions.Add(dbObj.GetStructureWorkAction(workActionCode));
                }
            }
            else
            {
                workActionRulesInEffect = dbObj.GetWorkActionCriteria(true);
                selectedPrimaryworkActions = workActionsPrimary;
            }

            List<Structure> analyzedStructures = new List<Structure>();
            List<string> unanalyzedStructureIds = new List<string>();

            #region UnconstrainedAnalysis
            if (needsAnalysisInput.RunUnconstrainedAnalysis)
            {
                Progress progressDialog = new Progress(structureIds.Count(), needsAnalysisInput.AnalysisType.ToString() + "-Unconstrained");

                Thread backgroundThread = new Thread(
                    new ThreadStart(() =>
                    {
                        // Process
                        int analyzedStructureCounter = 1;
                        foreach (var structureId in structureIds)
                        {
                            if (dbObj.GetElementInspectionDates(structureId).Count > 0)
                            {
                                Structure analyzedStructure = null;
                                try
                                {
                                    analyzedStructure = AnalyzeStructure(structureId, needsAnalysisInput, workActionRulesInEffect);
                                }
                                catch (Exception e)
                                {
                                    unanalyzedStructureIds.Add(structureId + ": " + e.StackTrace);
                                }

                                if (analyzedStructure != null)
                                {
                                    analyzedStructures.Add(analyzedStructure);
                                }
                                else
                                {
                                    unanalyzedStructureIds.Add(structureId + ": null structure");
                                }
                            }
                            else
                            {
                                unanalyzedStructureIds.Add(structureId + ": no routine inspection");
                            }

                            progressDialog.UpdateProgress(analyzedStructureCounter, structureId, structureIds.Count());

                            if (needsAnalysisInput.MaxNumberToAnalyze > 0 && analyzedStructureCounter == needsAnalysisInput.MaxNumberToAnalyze)
                            {
                                break;
                            }

                            analyzedStructureCounter++;
                        } // end foreach (var structureId in structureIds)

                        progressDialog.UpdateProgress(-1, "Writing results...", structureIds.Count());

                        foreach (var analysisFile in needsAnalysisInput.AnalysisFiles)
                        {
                            switch (analysisFile.NeedsAnalysisFileType)
                            {
                                case WisamType.NeedsAnalysisFileTypes.ProgramUnconstrained:
                                    // Rewrite

                                    ExcelHelperService eh = new ExcelHelperService(analysisFile.FilePath);
                                    eh.WriteReport(WisamType.AnalysisReports.RegionNeedsNew,
                                                    analyzedStructures,
                                                    unanalyzedStructureIds,
                                                    selectedPrimaryworkActions,
                                                    needsAnalysisInput.AnalysisStartYear,
                                                    needsAnalysisInput.AnalysisEndYear,
                                                    DateTime.Now,
                                                    needsAnalysisInput.CreateDebugFile,
                                                    needsAnalysisInput.ShowPriorityScore,
                                                    String.Join(",", needsAnalysisInput.Regions.Select(x => x.ToString()).ToArray()),
                                                    needsAnalysisInput.IncludeStateOwned,
                                                    needsAnalysisInput.IncludeLocalOwned, dbObj.GetSimilarComboWorkActions());
                                    eh.SaveWorkbook();

                                    /*
                                    ReportWriterService.WriteProgram(analysisFile.NeedsAnalysisFileType, needsAnalysisInput.AnalysisType,
                                                                analyzedStructures,
                                                                needsAnalysisInput.AnalysisStartYear,
                                                                needsAnalysisInput.AnalysisEndYear,
                                                                analysisFile.FilePath);*/
                                    break;

                                case WisamType.NeedsAnalysisFileTypes.PriorityUnconstrained:
                                    ReportWriterService.WritePriorityScoreReport(WisamType.AnalysisReports.AnalysisDebug,
                                                                    analyzedStructures,
                                                                    needsAnalysisInput.AnalysisStartYear,
                                                                    needsAnalysisInput.AnalysisEndYear,
                                                                    analysisFile.FilePath,
                                                                    priorityIndexCategories,
                                                                    priorityIndexFactors);
                                    break;

                                case WisamType.NeedsAnalysisFileTypes.ConditionUnconstrained:
                                    ReportWriterService.WriteBridgeConditionReport(analyzedStructures, needsAnalysisInput, analysisFile.FilePath, WisamType.NeedsAnalysisFileTypes.ConditionUnconstrained);
                                    break;

                                case WisamType.NeedsAnalysisFileTypes.Inventory:
                                    ReportWriterService.WriteBridgeInventoryReport(analyzedStructures, needsAnalysisInput, analysisFile.FilePath);
                                    writtenBridgeInventoryFile = true;
                                    break;
                            }
                        }

                        progressDialog.UpdateProgress(-2, "Completed unconstrained analysis...", structureIds.Count());
                        Thread.Sleep(2000);
                        progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    }
                ));

                backgroundThread.Start();
                progressDialog.ShowDialog();
            }
            #endregion UnconstrainedAnalysis

            #region ConstrainedAnalysis
            if (needsAnalysisInput.ApplyBudget)
            {
                Progress progressDialog2 = new Progress(needsAnalysisInput.AnalysisStartYear, needsAnalysisInput.AnalysisEndYear, needsAnalysisInput.AnalysisType.ToString() + "-Constrained");
                Thread backgroundThread2 = new Thread(
                    new ThreadStart(() =>
                    {
                        // Process
                        int analyzedYearCounter = 1;
                        int numberOfAnalysisYears = needsAnalysisInput.AnalysisEndYear - needsAnalysisInput.AnalysisStartYear + 1;
                        int currentYear = DateTime.Now.Year;
                        int startYear = needsAnalysisInput.AnalysisStartYear;
                        int endYear = needsAnalysisInput.AnalysisEndYear;
                        float multiYearBudget = 0;
                        float multiYearBudgetBalance = 0;
                        int deteriorationYear = 0;

                        if (needsAnalysisInput.IsMultiYearBudget)
                        {
                            multiYearBudget = needsAnalysisInput.PrimaryWorkActionBudget.First().Amount;
                            multiYearBudgetBalance = multiYearBudget;
                        }

                        List<Structure> constrainedAnalysisStructures = new List<Structure>();
                        progressDialog2.UpdateProgress(-1, "Initializing structures...", structureIds.Count());
                        int initializedStructureCounter = 0;

                        foreach (var structureId in structureIds)
                        {
                            if (dbObj.GetElementInspectionDates(structureId).Count > 0)
                            {
                                Structure analyzedStructure = null;

                                try
                                {
                                    analyzedStructure = GetStructure(structureId, needsAnalysisInput.InterpolateNbiRatings, needsAnalysisInput.CountThinPolymerOverlays, needsAnalysisInput.AnalysisStartYear, needsAnalysisInput.AnalysisEndYear);
                                    InitializeStructure(analyzedStructure, needsAnalysisInput.AnalysisStartYear,
                                                    needsAnalysisInput.AnalysisEndYear, needsAnalysisInput.CaiFormulaId,
                                                    needsAnalysisInput.DeteriorateOverlayDefects, needsAnalysisInput.DeteriorationStartOffset,
                                                    workActionRulesInEffect,
                                                    needsAnalysisInput);
                                    constrainedAnalysisStructures.Add(analyzedStructure);
                                }
                                catch (Exception e)
                                {
                                    unanalyzedStructureIds.Add(structureId + ": " + e.StackTrace);
                                }

                                if (analyzedStructure != null)
                                {
                                    analyzedStructures.Add(analyzedStructure);
                                }
                                else
                                {
                                    unanalyzedStructureIds.Add(structureId + ": null structure");
                                }
                            }
                            else
                            {
                                unanalyzedStructureIds.Add(structureId + ": no inspection");
                            }

                            initializedStructureCounter++;
                            progressDialog2.UpdateProgress(-1, String.Format("Initialized {0}... ({1}/{2})", structureId, initializedStructureCounter, structureIds.Count), structureIds.Count());

                            if (needsAnalysisInput.MaxNumberToAnalyze > 0 && initializedStructureCounter == needsAnalysisInput.MaxNumberToAnalyze)
                            {
                                break;
                            }
                        }

                        progressDialog2.UpdateProgress(-1, "", structureIds.Count());

                        for (int analysisYear = startYear; analysisYear <= endYear; analysisYear++)
                        {
                            progressDialog2.UpdateProgress(analyzedYearCounter, analysisYear, numberOfAnalysisYears, "Analyzing");

                            // Rewrite of below
                            /*
                            if (analyzedYearCounter == 1)
                            {
                                foreach (Structure structure in constrainedAnalysisStructures)
                                {

                                }
                            }
                            */

                            // Initialize each structure's YearlyConstrainedOptimalWorkActions with FIIPS
                            if (analyzedYearCounter == 1)
                            {
                                foreach (Structure structure in constrainedAnalysisStructures)
                                {
                                    if (structure.LastInspection != null)
                                    {
                                        int lastInspectionYear = structure.LastInspection.InspectionDate.Year;

                                        if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                                        {
                                            if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 7, 1)) >= 0)
                                            {
                                                lastInspectionYear = lastInspectionYear + 1;
                                            }

                                            if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 7, 1)) >= 0)
                                            {
                                                currentYear = currentYear + 1;
                                            }
                                        }
                                        else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                                        {
                                            if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 11, 1)) >= 0)
                                            {
                                                lastInspectionYear = lastInspectionYear + 1;
                                            }

                                            if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 11, 1)) >= 0)
                                            {
                                                currentYear = currentYear + 1;
                                            }
                                        }

                                        for (int year = lastInspectionYear; year <= endYear; year++)
                                        //for (int year = structure.LastInspection.InspectionDate.Year; year <= endYear; year++)
                                        {
                                            var yearlyFiipsWas = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == year).ToList();
                                            // Ex. 2019 > 2018 
                                            // 7/12/2018: Added = to below. Need to review
                                            if (startYear > currentYear)
                                            {
                                                // Ex. 2019 <= 2018 + 5
                                                if (startYear <= currentYear + 5)
                                                {
                                                    if (year < startYear)
                                                    {
                                                        if (yearlyFiipsWas.Count() > 0)
                                                        {
                                                            structure.YearlyConstrainedOptimalWorkActions.Add(yearlyFiipsWas.First());
                                                        }
                                                    }
                                                }
                                                // Ex. 2024 > 2018 + 5
                                                else if (startYear > currentYear + 5)
                                                {
                                                    if (year <= currentYear + 5)
                                                    {
                                                        if (yearlyFiipsWas.Count() > 0)
                                                        {
                                                            structure.YearlyConstrainedOptimalWorkActions.Add(yearlyFiipsWas.First());
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                } // end foreach (Structure structure in analyzedStructures)
                            } // end if (analyzedYearCounter == 1)


                            // Deteriorate and get optimal work action and priority score for each structure
                            List<StructureWorkAction> doNothingsForTheYear = new List<StructureWorkAction>();
                            List<StructureWorkAction> doSomethingsForTheYear = new List<StructureWorkAction>();
                            List<StructureWorkAction> madeCutForTheYear = new List<StructureWorkAction>();
                            List<StructureWorkAction> didNotMakeCutForTheYear = new List<StructureWorkAction>();
                            float yearBudget = 0;
                            float yearBudgetBalance = 0;

                            if (!needsAnalysisInput.IsMultiYearBudget)
                            {
                                yearBudget = needsAnalysisInput.PrimaryWorkActionBudget.Where(e => e.StartYear == analysisYear).First().Amount;
                                yearBudgetBalance = yearBudget;
                            }

                            foreach (Structure structure in constrainedAnalysisStructures)
                            {
                                if (structure.LastInspection != null)
                                {
                                    int lastInspectionYear = structure.LastInspection.InspectionDate.Year;

                                    if (structure.LastInspectionCaiForAnalysis == null)
                                    {
                                        if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.StateFiscalYear)
                                        {
                                            if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 7, 1)) >= 0)
                                            {
                                                lastInspectionYear = lastInspectionYear + 1;
                                            }
                                        }
                                        else if (needsAnalysisInput.CalendarType == WisamType.CalendarTypes.FederalFiscalYear)
                                        {
                                            if (DateTime.Compare(structure.LastInspection.InspectionDate, new DateTime(lastInspectionYear, 11, 1)) >= 0)
                                            {
                                                lastInspectionYear = lastInspectionYear + 1;
                                            }
                                        }
                                        Cai lastCai = GetLastInspectionBasedCai(structure, needsAnalysisInput.CaiFormulaId);
                                        //structure.LastInspectionCaiForAnalysis = new Cai(lastCai, structure.LastInspectionYear, 0);
                                        structure.LastInspectionCaiForAnalysis = new Cai(lastCai, lastInspectionYear, 0);
                                    }
                                    //int lastInspectionYear = structure.LastInspection.InspectionDate.Year;
                                    //Cai lastCai = GetLastInspectionBasedCai(structure, needsAnalysisInput.CaiFormulaId);
                                    //Cai lastInspectionCai = new Cai(lastCai, lastInspectionYear, 0);

                                    Cai previousOptimalCai = null;
                                    List<StructureWorkAction> previousOptimalWorkActions = null;

                                    if (structure.YearlyConstrainedOptimalWorkActions.Count == 1)
                                    {
                                        previousOptimalCai = structure.YearlyConstrainedOptimalWorkActions.First().CAI;
                                        previousOptimalWorkActions = structure.YearlyConstrainedOptimalWorkActions.ToList();
                                    }
                                    else
                                    {
                                        previousOptimalCai =
                                            structure.YearlyConstrainedOptimalWorkActions
                                            .Where(e => e.WorkActionYear == analysisYear - 1).First().CAI;

                                        previousOptimalWorkActions =
                                            structure.YearlyConstrainedOptimalWorkActions
                                            .Where(e => e.WorkActionYear < analysisYear).ToList();
                                    }

                                    /*
                                    if (analysisYear >= structure.LastInspection.InspectionDate.Year + needsAnalysisInput.DeteriorationStartOffset)
                                    {
                                        deteriorationYear = analysisYear - structure.LastInspection.InspectionDate.Year - needsAnalysisInput.DeteriorationStartOffset + 1;
                                    }
                                    else
                                    {
                                        deteriorationYear = 0;
                                    }
                                    */

                                    if (analysisYear >= lastInspectionYear + needsAnalysisInput.DeteriorationStartOffset)
                                    {
                                        deteriorationYear = analysisYear - lastInspectionYear - needsAnalysisInput.DeteriorationStartOffset + 1;
                                    }
                                    else
                                    {
                                        deteriorationYear = 0;
                                    }

                                    StructureWorkAction newDoNothing = new StructureWorkAction(Code.DoNothing);
                                    newDoNothing.StructureId = structure.StructureId;
                                    newDoNothing.WorkActionDesc = Code.DoNothingDesc;
                                    newDoNothing.WorkActionYear = analysisYear;
                                    newDoNothing.CombinedWorkActions = new List<StructureWorkAction>();
                                    Cai newDoNothingCai = new Cai(previousOptimalCai, analysisYear, deteriorationYear);
                                    newDoNothing.CAI = newDoNothingCai;
                                    // Calculate Priority Score
                                    CalculatePriorityScoreForWorkActionForYear(structure, newDoNothing, needsAnalysisInput, analysisYear, analysisYear);
                                    doNothingsForTheYear.Add(newDoNothing);

                                    StructureWorkAction newOptimalSwa = new StructureWorkAction(Code.DoNothing);
                                    newOptimalSwa.StructureId = structure.StructureId;
                                    newOptimalSwa.WorkActionDesc = Code.DoNothingDesc;
                                    newOptimalSwa.WorkActionYear = analysisYear;
                                    newOptimalSwa.CombinedWorkActions = new List<StructureWorkAction>();
                                    Cai newOptimalCai = new Cai(previousOptimalCai, analysisYear, deteriorationYear);
                                    newOptimalSwa.CAI = newOptimalCai;

                                    // Deteriorate
                                    if (deteriorationYear > 0)
                                    {
                                        newDoNothingCai.Basis = WisamType.CaiBases.Deterioration;
                                        //DeteriorateCai(newDoNothingCai, previousOptimalCai, lastInspectionCai, needsAnalysisInput.DeteriorateOverlayDefects);
                                        DeteriorateCai(newDoNothingCai, previousOptimalCai, structure.LastInspectionCaiForAnalysis, needsAnalysisInput.DeteriorateOverlayDefects);

                                        newOptimalCai.Basis = WisamType.CaiBases.Deterioration;
                                        //DeteriorateCai(newOptimalCai, previousOptimalCai, lastInspectionCai, needsAnalysisInput.DeteriorateOverlayDefects);
                                        DeteriorateCai(newOptimalCai, previousOptimalCai, structure.LastInspectionCaiForAnalysis, needsAnalysisInput.DeteriorateOverlayDefects);
                                    }

                                    // Determine optimal work action
                                    foreach (WorkActionCriteria wac in workActionRulesInEffect)
                                    {
                                        if (structure.BuriedStructure && wac.RuleCategory.ToUpper().Equals("OLAY"))
                                        {
                                            continue;
                                        }

                                        //if (IsWorkActionCriteriaMet(structure, newOptimalCai, wac.RuleFormula, wac))
                                        //if (IsWorkActionCriteriaMet(wac, structure, newOptimalCai))
                                        if (IsWorkActionEligible(wac, structure, newOptimalCai))
                                        {
                                            StructureWorkAction potentialOptimal = dbObj.GetStructureWorkAction(wac.WorkActionCode, structure);
                                            potentialOptimal.StructureId = structure.StructureId;
                                            potentialOptimal.WorkActionYear = analysisYear;

                                            // Check whether potential work action meets repeat frequency
                                            if (!potentialOptimal.CombinedWorkAction) // Single primary work action
                                            {
                                                if (!IsStructureWorkActionRepeatable(potentialOptimal, structure.ConstructionHistoryProjects, analysisYear))
                                                {
                                                    continue;
                                                }

                                                if (!IsStructureWorkActionRepeatable(potentialOptimal, previousOptimalWorkActions, analysisYear))
                                                {
                                                    continue;
                                                }
                                            }
                                            else // Combined primary work action
                                            {
                                                bool repeatable = true;

                                                foreach (var waCode in potentialOptimal.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                                                {
                                                    StructureWorkAction curCwa = dbObj.GetStructureWorkAction(waCode, structure);
                                                    potentialOptimal.CombinedWorkActions.Add(curCwa);

                                                    if (!IsStructureWorkActionRepeatable(curCwa, structure.ConstructionHistoryProjects, analysisYear))
                                                    {
                                                        repeatable = false;
                                                        break;
                                                    }

                                                    if (!IsStructureWorkActionRepeatable(curCwa, previousOptimalWorkActions, analysisYear))
                                                    {
                                                        repeatable = false;
                                                        break;
                                                    }
                                                }

                                                if (!repeatable)
                                                {
                                                    continue;
                                                }
                                            } // end if (!potentialOptimal.CombinedWorkAction)

                                            newOptimalSwa = potentialOptimal;
                                            newOptimalSwa.StructureId = potentialOptimal.StructureId;
                                            newOptimalSwa.CAI = newOptimalCai;
                                            newOptimalSwa.ControllingCriteria = wac;
                                            newOptimalSwa.AlternativeWorkActionCode = wac.AlternativeWorkActionCode;
                                            newOptimalSwa.AlternativeWorkActionDesc = wac.AlternativeWorkActionDesc;
                                            // Calculate Priority Score
                                            CalculatePriorityScoreForWorkActionForYear(structure, newOptimalSwa, needsAnalysisInput, analysisYear, analysisYear);
                                            doSomethingsForTheYear.Add(newOptimalSwa);
                                            break;
                                        } // end if (IsWorkActionCriteriaMet(structure, newOptimalCai, wac.RuleFormula, wac))
                                    } // end foreach (WorkActionCriteria wac in workActionRulesInEffect)
                                } // end if (structure.LastInspection != null)
                            } // end foreach (Structure structure in analyzedStructures)

                            // Constrained analysis
                            var rankedDoSomethings = doSomethingsForTheYear.OrderByDescending(e => e.PriorityScore);

                            if (rankedDoSomethings.Count() > 0)
                            {
                                int numberOfRankedDoSomethings = rankedDoSomethings.Count();

                                // Apply budget: 2 cases
                                if (needsAnalysisInput.IsMultiYearBudget)
                                {
                                    int rank = 1;
                                    foreach (StructureWorkAction swa in rankedDoSomethings)
                                    {
                                        Structure structure = constrainedAnalysisStructures.Where(e => e.StructureId.Equals(swa.StructureId)).First();
                                        //Structure structure = swa.Structure;
                                        //int lastInspectionYear = structure.LastInspection.InspectionDate.Year;
                                        //Cai lastCai = GetLastInspectionBasedCai(structure, needsAnalysisInput.CaiFormulaId);
                                        //Cai lastInspectionCai = new Cai(lastCai, lastInspectionYear, 0);

                                        swa.PriorityScoreRank = rank;
                                        swa.PriorityScoreRelativeRank = String.Format("{0}/{1}", rank, numberOfRankedDoSomethings);

                                        if (multiYearBudgetBalance < needsAnalysisInput.LeastCostProject)
                                        {
                                            //DetermineSecondaryWorkActions(structure, swa);
                                            DetermineSecondaryWorkActions(swa, structure);
                                            didNotMakeCutForTheYear.Add(swa);
                                            structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                            rank++;
                                            continue;
                                        }

                                        if (swa.Cost < multiYearBudgetBalance)
                                        {
                                            multiYearBudgetBalance = multiYearBudgetBalance - Convert.ToSingle(swa.Cost);
                                            madeCutForTheYear.Add(swa);
                                            List<StructureWorkAction> workActions = new List<StructureWorkAction>();

                                            if (swa.CombinedWorkAction)
                                            {
                                                workActions = swa.CombinedWorkActions;
                                            }
                                            else
                                            {
                                                workActions.Add(swa);
                                            }

                                            //ImproveCai(structure, swa.CAI, workActions, analysisYear, lastInspectionCai);
                                            //int deckSlabElement = swa.CAI.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).First().ParentElemNum;
                                            try
                                            {
                                                ImproveCai(structure, swa.CAI, workActions, analysisYear, structure.LastInspectionCaiForAnalysis, true);
                                                //DetermineSecondaryWorkActions(structure, swa);
                                                DetermineSecondaryWorkActions(swa, structure);
                                                structure.YearlyConstrainedOptimalWorkActions.Add(swa);
                                            }
                                            catch (Exception ex)
                                            {
                                                structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                            }
                                        }
                                        else
                                        {
                                            //DetermineSecondaryWorkActions(structure, swa);
                                            DetermineSecondaryWorkActions(swa, structure);
                                            didNotMakeCutForTheYear.Add(swa);
                                            structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                        }

                                        rank++;
                                    }
                                }
                                else // Budget amount each year
                                {
                                    int rank = 1;
                                    foreach (StructureWorkAction swa in rankedDoSomethings)
                                    {
                                        Structure structure = constrainedAnalysisStructures.Where(e => e.StructureId.Equals(swa.StructureId)).First();
                                        //Structure structure = swa.Structure;
                                        //int lastInspectionYear = structure.LastInspection.InspectionDate.Year;
                                        //Cai lastCai = GetLastInspectionBasedCai(structure, needsAnalysisInput.CaiFormulaId);
                                        //Cai lastInspectionCai = new Cai(lastCai, lastInspectionYear, 0);

                                        swa.PriorityScoreRank = rank;
                                        swa.PriorityScoreRelativeRank = String.Format("{0}/{1}", rank, numberOfRankedDoSomethings);

                                        if (yearBudgetBalance < needsAnalysisInput.LeastCostProject)
                                        {
                                            //DetermineSecondaryWorkActions(structure, swa);
                                            DetermineSecondaryWorkActions(swa, structure);
                                            didNotMakeCutForTheYear.Add(swa);
                                            structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                            rank++;
                                            continue;
                                        }

                                        if (swa.Cost < yearBudgetBalance)
                                        {
                                            yearBudgetBalance = yearBudgetBalance - Convert.ToSingle(swa.Cost);
                                            madeCutForTheYear.Add(swa);
                                            List<StructureWorkAction> workActions = new List<StructureWorkAction>();

                                            if (swa.CombinedWorkAction)
                                            {
                                                workActions = swa.CombinedWorkActions;
                                            }
                                            else
                                            {
                                                workActions.Add(swa);
                                            }

                                            //ImproveCai(structure, swa.CAI, workActions, analysisYear, lastInspectionCai);
                                            //int deckSlabElement = swa.CAI.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).First().ParentElemNum;
                                            try
                                            {
                                                ImproveCai(structure, swa.CAI, workActions, analysisYear, structure.LastInspectionCaiForAnalysis, true);
                                                //DetermineSecondaryWorkActions(structure, swa);
                                                DetermineSecondaryWorkActions(swa, structure);
                                                structure.YearlyConstrainedOptimalWorkActions.Add(swa);
                                            }
                                            catch (Exception ex)
                                            {
                                                structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                            }
                                        }
                                        else
                                        {
                                            //DetermineSecondaryWorkActions(structure, swa);
                                            DetermineSecondaryWorkActions(swa, structure);
                                            didNotMakeCutForTheYear.Add(swa);
                                            structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                        }

                                        rank++;
                                    }
                                }

                                foreach (StructureWorkAction swa in didNotMakeCutForTheYear)
                                {
                                    Structure structure = constrainedAnalysisStructures.Where(e => e.StructureId.Equals(swa.StructureId)).First();
                                    //Structure structure = swa.Structure;
                                    structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(swa.StructureId)).First());
                                    structure.DidNotMakeCutWorkActions.Add(swa);
                                }

                                foreach (Structure structure in constrainedAnalysisStructures)
                                {
                                    if (structure.YearlyConstrainedOptimalWorkActions.Where(e => e.WorkActionYear == analysisYear).Count() == 0)
                                    {
                                        structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId == structure.StructureId && e.WorkActionYear == analysisYear).First());
                                    }
                                }
                            }
                            else
                            {
                                foreach (Structure structure in constrainedAnalysisStructures)
                                {
                                    structure.YearlyConstrainedOptimalWorkActions.Add(doNothingsForTheYear.Where(e => e.StructureId.Equals(structure.StructureId)).First());
                                }
                            }

                            progressDialog2.UpdateProgress(analyzedYearCounter, analysisYear, numberOfAnalysisYears, "Analyzed");
                            analyzedYearCounter++;
                        } // end for (int analysisYear = startYear; analysisYear <= endYear; analysisYear++)

                        progressDialog2.UpdateProgress(-1, "Writing results...", numberOfAnalysisYears);

                        foreach (var analysisFile in needsAnalysisInput.AnalysisFiles)
                        {
                            switch (analysisFile.NeedsAnalysisFileType)
                            {
                                case WisamType.NeedsAnalysisFileTypes.ProgramConstrained:

                                    ExcelHelperService eh = new ExcelHelperService(analysisFile.FilePath);
                                    eh.WriteReport(constrainedAnalysisStructures, needsAnalysisInput, analysisFile.NeedsAnalysisFileType);
                                    eh.SaveWorkbook();

                                    /*
                                    ReportWriterService.WriteProgram(analysisFile.NeedsAnalysisFileType, needsAnalysisInput.AnalysisType,
                                                                constrainedAnalysisStructures,
                                                                needsAnalysisInput.AnalysisStartYear,
                                                                needsAnalysisInput.AnalysisEndYear,
                                                                analysisFile.FilePath);*/
                                    break;

                                case WisamType.NeedsAnalysisFileTypes.PriorityConstrained:
                                    ReportWriterService.WritePriorityScoreReport(WisamType.AnalysisReports.AnalysisDebug,
                                                                    constrainedAnalysisStructures,
                                                                    needsAnalysisInput.AnalysisStartYear,
                                                                    needsAnalysisInput.AnalysisEndYear,
                                                                    analysisFile.FilePath,
                                                                    priorityIndexCategories,
                                                                    priorityIndexFactors);
                                    break;

                                case WisamType.NeedsAnalysisFileTypes.ConditionConstrained:
                                    ReportWriterService.WriteBridgeConditionReport(constrainedAnalysisStructures, needsAnalysisInput, analysisFile.FilePath, WisamType.NeedsAnalysisFileTypes.ConditionConstrained);
                                    break;

                                case WisamType.NeedsAnalysisFileTypes.Inventory:
                                    if (!writtenBridgeInventoryFile)
                                    {
                                        ReportWriterService.WriteBridgeInventoryReport(analyzedStructures, needsAnalysisInput, analysisFile.FilePath);
                                    }
                                    break;
                            }
                        }

                        progressDialog2.UpdateProgress(-2, "Completed unconstrained analysis...", numberOfAnalysisYears);
                        Thread.Sleep(2000);
                        progressDialog2.BeginInvoke(new Action(() => progressDialog2.Close()));
                    }
                ));

                backgroundThread2.Start();
                progressDialog2.ShowDialog();
            } // end if (needsAnalysisInput.ApplyBudget)
            #endregion ConstrainedAnalysis
        }

        public Structure AnalyzeStructure(string structureId, NeedsAnalysisInput needAnalysisInput, List<WorkActionCriteria> workActionRulesInEffect)
        {
            Structure analyzedStructure = GetStructure(structureId, needAnalysisInput.InterpolateNbiRatings, needAnalysisInput.CountThinPolymerOverlays, needAnalysisInput.AnalysisStartYear, needAnalysisInput.AnalysisEndYear);

            if (analyzedStructure != null)
            {
                switch (needAnalysisInput.AnalysisType)
                {
                    case WisamType.AnalysisTypes.Optimal:
                        try
                        {
                            GetStructureWorkActions(analyzedStructure, needAnalysisInput.AnalysisStartYear,
                                                    needAnalysisInput.AnalysisEndYear, needAnalysisInput.CaiFormulaId,
                                                    needAnalysisInput.DeteriorateOverlayDefects, needAnalysisInput.DeteriorationStartOffset,
                                                    workActionRulesInEffect,
                                                    needAnalysisInput, needAnalysisInput.ElementDeteriorationMethod);
                        }
                        catch (Exception ex)
                        { }
                        break;
                }
            }

            return analyzedStructure;
        }

        public void CalculatePriorityScoreForWorkActionForYear(Structure str, StructureWorkAction swa, NeedsAnalysisInput needsAnalysisInput, int startYear, int endYear)
        {
            // Policy
            swa.PriorityScorePolicyEffects = CalculatePriorityScorePolicyEffects(needsAnalysisInput, str, startYear, endYear, swa);
            dbObj.CalculatePriorityScoreFactorsCategories(str, swa, startYear, endYear);
            CalculatePriorityIndexVariableFactors(str, swa, startYear, endYear);
            float priorityScore = 0;

            foreach (var effect in swa.PriorityScorePolicyEffects)
            {
                priorityScore += effect.ScoreEffect;
            }

            foreach (var cat in swa.PriorityIndexCategories)
            {
                priorityScore += cat.Score;
            }

            swa.PriorityScore = priorityScore;
        }

        public int GetFiscalYear()
        {
            return dbObj.GetFiscalYear();
        }
        #endregion NeedsAnalysisEventHandlers


        #region CAI Methods
        public void GetCaiFormula()
        {
            dbObj.GetCaiFormula(1);
        }

        /*
        public void ComputeCais(string strId, int startYear, int endYear, int caiFormulaId, List<StructureWorkAction> swas)
        {
            List<Cai> cais = new List<Cai>();
            //Cai lastInspectionCai = GetLastInspectionBasedCai(strId, caiFormulaId);
            List<DateTime> inspectionDates = dbObj.GetElementInspectionDates(strId); // sorted newest to oldest

            // Need at least 1 inspection 
            if (inspectionDates.Count > 0)
            {
                DateTime lastInspectionDate = inspectionDates[0];

                // Add CAIs based on inspection that occurred after given start year
                foreach (DateTime inspectionDate in inspectionDates.Where(e => e.Year >= startYear && e.Year <= lastInspectionDate.Year))
                {
                    Cai newCai = GetInspectionBasedCai(strId, caiFormulaId, inspectionDate);
                    foreach (StructureWorkAction swa in swas.Where(e => e.WorkActionYear == inspectionDate.Year))
                    {
                        swa.CAI = newCai;
                    }
                }
                
                // Add CAI based on last inspection; if this last inspection occurred in the current year,
                // determine element and NBI benefits of any work type scheduled in the current year
                cais.Add(lastInspectionCai);

                if (lastInspectionCai.Year == DateTime.Now.Year)
                {
                    var yearSwas = swas.Where(e => e.WorkActionYear == lastInspectionCai.Year && e.EstimatedCompletionDate > lastInspectionCai.AllElements[0].InspectionDate && e.WorkActionCode != Code.DoNothing).ToList();

                    if (yearSwas.Count > 0)
                    {
                        ImproveCai(lastInspectionCai, yearSwas);
                    }
                }

                foreach (StructureWorkAction swa in swas.Where(e => e.WorkActionYear == lastInspectionCai.Year))
                {
                    swa.CAI = lastInspectionCai;
                }

                Cai previousCai = new Cai(lastInspectionCai, lastInspectionCai.Year, 0);
                int detYear = 1;

                for (int year = lastInspectionDate.Year + 1; year <= endYear; year++)
                {
                    // Create new CAI
                    Cai newCai = new Cai(previousCai, year, detYear);

                    // Deteriorate CAI
                    DeteriorateCai(newCai, previousCai, lastInspectionCai);

                    // Improve CAI
                    var yearSwas = swas.Where(e => e.WorkActionYear == year && e.EstimatedCompletionDate > lastInspectionCai.AllElements[0].InspectionDate && e.WorkActionCode != Code.DoNothing).ToList();

                    if (yearSwas.Count > 0)
                    {
                        ImproveCai(newCai, yearSwas);
                    }

                    cais.Add(newCai);

                    foreach (StructureWorkAction swa in swas.Where(e => e.WorkActionYear == year))
                    {
                        swa.CAI = newCai;
                    }

                    previousCai = newCai;
                    detYear++;
                }
            }

            //return cais;
        }
        */

        //Redo 11/25/15
        //Redo 02/19/16
        public List<Cai> GetCais(Structure structure, int startYear, int endYear, int caiFormulaId, List<StructureWorkAction> swas, int detStart, bool deteriorateDefects)
        {
            List<Cai> cais = new List<Cai>();
            Cai lastInspectionCai = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);

            //Cai lastInspectionCai = GetLastInspectionBasedCai(structure);

            // Need at least 1 inspection 
            if (lastInspectionCai != null)
            {
                //List<DateTime> inspectionDates = dbObj.GetElementInspectionDates(strId); // sorted newest to oldest
                //DateTime lastInspectionDate = inspectionDates[0];


                // Add CAI based on last inspection; if this last inspection occurred in the current year,
                // determine element and NBI benefits of any work type scheduled in the current year
                //cais.Add(lastInspectionCai);
                Cai previousCai = new Cai(lastInspectionCai, lastInspectionCai.Year, 0);
                previousCai.DebugInfo = lastInspectionCai.DebugInfo;
                int detYear = 0;

                for (int year = startYear; year <= endYear; year++)
                {
                    // Create new CAI
                    Cai newCai = null;

                    //new Cai(previousCai, year, detYear);

                    if (detYear < detStart)
                    {
                        newCai = new Cai(previousCai, year, 0);
                        newCai.Basis = WisamType.CaiBases.Inspection;
                        newCai.DebugInfo = previousCai.DebugInfo;
                    }
                    else
                    {
                        newCai = new Cai(previousCai, year, detYear - detStart + 1);
                    }

                    // Deteriorate CAI
                    if (detYear >= detStart)
                    {
                        DeteriorateCai(newCai, previousCai, lastInspectionCai, deteriorateDefects);
                        newCai.Basis = WisamType.CaiBases.Deterioration;
                    }

                    // Improve CAI
                    //var yearSwas = swas.Where(e => e.WorkActionYear == year && e.EstimatedCompletionDate > lastInspectionCai.AllElements[0].InspectionDate && e.WorkActionCode != Code.DoNothing).ToList();
                    var yearSwas = swas.Where(e => e.WorkActionYear == year && e.WorkActionCode != Code.DoNothing).ToList();

                    if (yearSwas.Count > 0)
                    {
                        //ImproveCai(newCai, yearSwas);
                        ImproveCai(structure, newCai, yearSwas, year, lastInspectionCai);
                    }

                    cais.Add(newCai);

                    foreach (StructureWorkAction swa in swas.Where(e => e.WorkActionYear == year))
                    {
                        swa.CAI = newCai;
                    }

                    previousCai = newCai;
                    detYear++;
                }
            }

            return cais;
        }

        public List<Cai> GetCais(Structure structure, int startYear, int endYear, int caiFormulaId, List<StructureWorkAction> swas, bool deteriorateDefects)
        {
            List<Cai> cais = new List<Cai>();
            Cai lastInspectionCai = GetLastInspectionBasedCai(structure.StructureId, caiFormulaId);

            // Need at least 1 inspection 
            if (lastInspectionCai != null)
            {
                List<DateTime> inspectionDates = dbObj.GetElementInspectionDates(structure.StructureId); // sorted newest to oldest
                DateTime lastInspectionDate = inspectionDates[0];


                // Add CAI based on last inspection; if this last inspection occurred in the current year,
                // determine element and NBI benefits of any work type scheduled in the current year
                //cais.Add(lastInspectionCai);
                Cai previousCai = new Cai(lastInspectionCai, lastInspectionCai.Year, 0);
                previousCai.DebugInfo = lastInspectionCai.DebugInfo;
                int detYear = 0;

                for (int year = startYear; year <= endYear; year++)
                {
                    // Create new CAI
                    Cai newCai = new Cai(previousCai, year, detYear);
                    newCai.Basis = WisamType.CaiBases.Deterioration;

                    if (detYear == 0)
                    {
                        newCai.Basis = WisamType.CaiBases.Inspection;
                        newCai.DebugInfo = previousCai.DebugInfo;
                    }

                    // Deteriorate CAI
                    if (detYear > 0)
                    {
                        DeteriorateCai(newCai, previousCai, lastInspectionCai, deteriorateDefects);
                    }

                    // Improve CAI
                    var yearSwas = swas.Where(e => e.WorkActionYear == year && e.EstimatedCompletionDate > lastInspectionCai.AllElements[0].InspectionDate && e.WorkActionCode != Code.DoNothing).ToList();

                    if (yearSwas.Count > 0)
                    {
                        ImproveCai(newCai, yearSwas, structure);
                    }

                    cais.Add(newCai);

                    foreach (StructureWorkAction swa in swas.Where(e => e.WorkActionYear == year))
                    {
                        swa.CAI = newCai;
                    }

                    previousCai = newCai;
                    detYear++;
                }
            }

            return cais;
        }

        // Revising 11/16/17
        // Revising 02/04/2020

        private void ApplyWorkActionNbiBenefits(List<NbiBenefit> nbiBenefits, NbiRating nbiRatings, Structure structure)
        {
            foreach (NbiBenefit nbiBenefit in nbiBenefits)
            {
                if (nbiRatings.CulvertRating.Equals("N"))
                {
                    if (nbiBenefit.NbiClassificationCode.ToUpper().Equals(Code.NbiDeck))
                    {
                        if (nbiBenefit.Benefit > 0)
                        {
                            if (nbiRatings.DeckRatingVal < nbiBenefit.Benefit)
                            {
                                nbiRatings.DeckRatingVal = Convert.ToDouble(nbiBenefit.Benefit);
                                nbiRatings.DeckRating = nbiBenefit.Benefit.ToString();
                                nbiRatings.DeckDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiDeck, nbiBenefit.Benefit, structure.StructureId);
                            }
                        }
                        else
                        {
                            if (nbiRatings.DeckRatingVal < Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.DeckRatingVal + nbiBenefit.AddedBenefit)))
                            {
                                nbiRatings.DeckRatingVal = Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.DeckRatingVal + nbiBenefit.AddedBenefit));
                                nbiRatings.DeckRating = nbiRatings.DeckRatingVal.ToString();
                                nbiRatings.DeckDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiDeck, nbiRatings.DeckRatingVal, nbiRatings.StructureId);
                            }
                        }

                        if (structure.StructureType.ToUpper().Contains("SLAB") && structure.MainSpanMaterial.ToUpper().Contains("CONCRETE") && nbiBenefit.IncludesOverlay)
                        {
                            /*
                            NbiBenefit nb = new NbiBenefit();
                            nb.WorkActionCode = nbiBenefit.WorkActionCode;
                            nb.NbiClassificationCode = Code.NbiSuperstructure;
                            nb.Benefit = 6;
                            nb.IncludesOverlay = true;
                            */

                            if (nbiBenefit.Benefit > 0)
                            {
                                if (nbiRatings.SuperstructureRatingVal < nbiBenefit.Benefit)
                                {
                                    nbiRatings.SuperstructureRatingVal = Convert.ToDouble(nbiBenefit.Benefit);
                                    nbiRatings.SuperstructureRating = nbiBenefit.Benefit.ToString();
                                    nbiRatings.SuperstructureDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiSuperstructure, nbiRatings.SuperstructureRatingVal, nbiRatings.StructureId);
                                }
                            }
                            else
                            {
                                if (nbiRatings.SuperstructureRatingVal < Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.DeckRatingVal + nbiBenefit.AddedBenefit)))
                                {
                                    nbiRatings.SuperstructureRatingVal = Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.DeckRatingVal + nbiBenefit.AddedBenefit));
                                    nbiRatings.SuperstructureRating = nbiRatings.DeckRatingVal.ToString();
                                    nbiRatings.SuperstructureDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiSuperstructure, nbiRatings.SuperstructureRatingVal, nbiRatings.StructureId);
                                }
                            }
                        }
                    }
                    else if (nbiBenefit.NbiClassificationCode.ToUpper().Equals(Code.NbiSuperstructure))
                    {
                        if (nbiBenefit.Benefit > 0)
                        {
                            if (nbiRatings.SuperstructureRatingVal < nbiBenefit.Benefit)
                            {
                                nbiRatings.SuperstructureRatingVal = Convert.ToDouble(nbiBenefit.Benefit);
                                nbiRatings.SuperstructureRating = nbiBenefit.Benefit.ToString();
                                nbiRatings.SuperstructureDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiSuperstructure, nbiBenefit.Benefit, structure.StructureId);
                            }
                        }
                        else
                        {
                            if (nbiRatings.SuperstructureRatingVal < Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.SuperstructureRatingVal + nbiBenefit.AddedBenefit)))
                            {
                                nbiRatings.SuperstructureRatingVal = Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.SuperstructureRatingVal + nbiBenefit.AddedBenefit));
                                nbiRatings.SuperstructureRating = nbiRatings.SuperstructureRatingVal.ToString();
                                nbiRatings.SuperstructureDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiSuperstructure, nbiRatings.SuperstructureRatingVal, nbiRatings.StructureId);
                            }
                        }
                    }
                    else if (nbiBenefit.NbiClassificationCode.ToUpper().Equals(Code.NbiSubstructure))
                    {
                        if (nbiBenefit.Benefit > 0)
                        {
                            if (nbiRatings.SubstructureRatingVal < nbiBenefit.Benefit)
                            {
                                nbiRatings.SubstructureRatingVal = Convert.ToDouble(nbiBenefit.Benefit);
                                nbiRatings.SubstructureRating = nbiBenefit.Benefit.ToString();
                                nbiRatings.SubstructureDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiSubstructure, nbiBenefit.Benefit, structure.StructureId);
                            }
                        }
                        else
                        {
                            if (nbiRatings.SubstructureRatingVal < Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.SubstructureRatingVal + nbiBenefit.AddedBenefit)))
                            {
                                nbiRatings.SubstructureRatingVal = Math.Min(nbiBenefit.NbiMaximumValue, Convert.ToDouble(nbiRatings.SubstructureRatingVal + nbiBenefit.AddedBenefit));
                                nbiRatings.SubstructureRating = nbiRatings.SubstructureRatingVal.ToString();

                                if (nbiRatings.SubstructureRatingVal == 7.22)
                                {
                                    var stop = true;
                                }
                                nbiRatings.SubstructureDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiSubstructure, nbiRatings.SubstructureRatingVal, nbiRatings.StructureId);
                            }
                        }
                    }
                }
                else // culvert
                {
                    if (nbiBenefit.NbiClassificationCode.ToUpper().Equals(Code.NbiCulvert))
                    {
                        nbiRatings.CulvertRatingVal = Convert.ToDouble(nbiBenefit.Benefit);
                        nbiRatings.CulvertRating = nbiBenefit.Benefit.ToString();
                        nbiRatings.CulvertDeteriorationYear = dbObj.GetNbiDeteriorationYear(Code.NbiCulvert, nbiBenefit.Benefit, structure.StructureId);
                    }
                }
            }
        }

        private void ApplyWorkActionElementBenefits(List<ElementBenefit> elementBenefits, List<Element> elements, Element deckSlab = null)
        {
            foreach (ElementBenefit elementBenefit in elementBenefits)
            {
                string benefit = elementBenefit.Benefit.ToUpper();
                List<Element> elementsBenefitting = new List<Element>();
                if (elementBenefit.ElementClassificationCode.ToUpper().Equals(Code.AllElements))
                {
                    elementsBenefitting = elements.Where(e => !e.ElementClassificationCode.Equals(Code.Defect)).ToList();
                }
                else
                {
                    elementsBenefitting = elements.Where(e => e.ElementClassificationCode == elementBenefit.ElementClassificationCode).ToList();
                    if (elementBenefit.ElementClassificationCode.Equals(Code.Deck) && deckSlab.ElementClassificationCode.Equals(Code.Slab))
                    {
                        elementsBenefitting = elements.Where(e => e.ElementClassificationCode.Equals(Code.Slab)).ToList();
                    }

                }
                foreach (Element element in elementsBenefitting)
                {
                    element.DeteriorationYear = 0; // Recalibrate for Old Deterioration Method; Not needed for BrM Deterioration Method
                    switch (benefit)
                    {
                        case "Q1":
                            element.Cs1Quantity = element.TotalQuantity;
                            element.Cs2Quantity = 0;
                            element.Cs3Quantity = 0;
                            element.Cs4Quantity = 0;
                            element.EquivalentAge = 0;
                            break;
                        case "Q2TOQ1":
                            element.Cs1Quantity += element.Cs2Quantity;
                            element.Cs2Quantity = 0;
                            break;
                        case "Q3TOQ1":
                            element.Cs1Quantity = element.Cs1Quantity + element.Cs2Quantity + element.Cs3Quantity;
                            element.Cs2Quantity = 0;
                            element.Cs3Quantity = 0;
                            break;
                        case "Q3TOQ2":
                            element.Cs2Quantity += element.Cs3Quantity;
                            element.Cs3Quantity = 0;
                            break;
                        case "Q4TOQ2":
                            element.Cs2Quantity = element.Cs2Quantity + element.Cs3Quantity + element.Cs4Quantity;
                            element.Cs3Quantity = 0;
                            element.Cs4Quantity = 0;
                            break;
                    }
                }
            }
        }

        public void ImproveCai(Structure structure, Cai improvedCai, List<StructureWorkAction> strWorkActions, int year,
            Cai lastInspectionCai, bool improveStructure = true, int deckSlabElement = 0)
        {
            // Get parent deck/slab element of overlay elements
            if (structure.StructureType.ToUpper().Equals("BOX CULVERT"))
            {
                deckSlabElement = 241;
            }
            else
            {
                deckSlabElement = improvedCai.AllElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).First().ParentElemNum;
            }
            Element deckSlab = improvedCai.AllElements.Where(e => e.ElemNum == deckSlabElement).First();
            foreach (StructureWorkAction strWorkAction in strWorkActions)
            {
                List<ElementBenefit> elementBenefits = dbObj.GetWorkActionElementBenefits(strWorkAction.WorkActionCode);
                if (elementBenefits.Count > 0)
                {
                    ApplyWorkActionElementBenefits(elementBenefits, improvedCai.AllElements, deckSlab);
                    ApplyWorkActionElementBenefits(elementBenefits, improvedCai.CaiElements, deckSlab);
                    ApplyWorkActionElementBenefits(elementBenefits, lastInspectionCai.AllElements, deckSlab);
                    ApplyWorkActionElementBenefits(elementBenefits, lastInspectionCai.CaiElements, deckSlab);
                }
                List<NbiBenefit> nbiBenefits = dbObj.GetWorkActionNbiBenefits(strWorkAction.WorkActionCode);
                if (nbiBenefits.Count > 0)
                {
                    ApplyWorkActionNbiBenefits(nbiBenefits, improvedCai.NbiRatings, structure);
                }
                if (improveStructure)
                {
                    switch (strWorkAction.WorkActionCode)
                    {
                        case Code.ReplaceStructure:
                        case Code.NewStructure:
                            structure.YearBuilt = year;
                            //structure.DeckBuiltYear = year;
                            structure.SuperBuilts.Add(year); //new 6/6/18
                            structure.DeckBuilts.Add(year);
                            structure.Overlays.Add(year); //new 6/6/18
                            structure.NumOlays = 0;
                            structure.NumThinPolymerOverlays = 0;
                            structure.LoadCapacity = 5;
                            //structure.DeckBuiltYear = year;
                            structure.SufficiencyNumber = 99.9;
                            structure.ScourCritical = false;
                            structure.ScourCriticalRating = "8";
                            structure.FractureCritical = false;
                            structure.StructurallyDeficient = false;
                            structure.FunctionalObsolete = false;
                            structure.FunctionalObsoleteDueToApproachRoadwayAlignment = false;
                            structure.FunctionalObsoleteDueToDeckGeometry = false;
                            structure.FunctionalObsoleteDueToStructureEvaluation = false;
                            structure.FunctionalObsoleteDueToVerticalClearance = false;
                            structure.FunctionalObsoleteDueToWaterwayAdequacy = false;
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            /*
                            if (deckSlabElement == 0)
                            {
                                UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            }
                            else
                            {
                                UpdateElements(structure, deckSlabElement, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            }*/
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                        case Code.ReplaceSuperstructure:
                            structure.SuperBuilts.Add(year);
                            structure.DeckBuilts.Add(year);
                            structure.Overlays.Add(year);
                            structure.NumOlays = 0;
                            structure.NumThinPolymerOverlays = 0;
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                        case Code.ReplaceDeckRaiseStructure:
                            //structure.DeckBuiltYear = year;
                            structure.DeckBuilts.Add(year);
                            structure.Overlays.Add(year); //new 6/6/18
                            structure.NumOlays = 0;
                            structure.NumThinPolymerOverlays = 0;
                            structure.FunctionalObsoleteDueToVerticalClearance = false;
                            EvaluateFunctionalObsolete(structure);
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                        case Code.ReplaceDeck:
                        case Code.ReplaceDeckPaintComplete:
                            //structure.DeckBuiltYear = year;
                            structure.DeckBuilts.Add(year);
                            structure.Overlays.Add(year);
                            structure.NumOlays = 0;
                            structure.NumThinPolymerOverlays = 0;
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                        case Code.OverlayConcrete:
                        case Code.OverlayConcretePaint:
                        case Code.OverlayConcreteNewJoints:
                        case Code.OverlayConcreteNewRailJoints:
                        case Code.OverlayHma:
                        case Code.OverlayPma:
                        case Code.OverlayPolyesterPolymer:
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            structure.NumOlays++;
                            structure.Overlays.Add(year);
                            structure.NumThinPolymerOverlays = 0;
                            break;
                        case Code.OverlayThinPolymer:
                        case Code.OverlayThinPolymerNewJoints:
                        case Code.OverlayThinPolymerRepairJoints:
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            if (structure.CountTpo)
                            {
                                structure.NumOlays++;
                            }
                            structure.Overlays.Add(year);
                            structure.NumThinPolymerOverlays++;
                            break;
                        case Code.RaiseStructure:
                            structure.FunctionalObsoleteDueToVerticalClearance = false;
                            EvaluateFunctionalObsolete(structure);
                            break;
                        case Code.WidenBridge:
                            structure.FunctionalObsoleteDueToDeckGeometry = false;
                            EvaluateFunctionalObsolete(structure);
                            break;
                        case Code.ReplaceDeckWidenBridge:
                            structure.FunctionalObsoleteDueToDeckGeometry = false;
                            EvaluateFunctionalObsolete(structure);
                            structure.DeckBuilts.Add(year);
                            structure.Overlays.Add(year);
                            structure.NumOlays = 0;
                            structure.NumThinPolymerOverlays = 0;
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                        case Code.ReplaceDeckOverlayThinPolymer:
                        case Code.ReplaceDeckOverlayThinPolymerPaintComplete:
                            structure.DeckBuilts.Add(year);
                            structure.Overlays.Add(year);
                            structure.NumOlays = 0;
                            structure.NumThinPolymerOverlays = 1;
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                        case Code.RepairDeck:
                            UpdateElements(structure, improvedCai, strWorkAction.WorkActionCode, structure.OverlayQuantity, 0);
                            foreach (Element elem in improvedCai.AllElements.Except(lastInspectionCai.AllElements))
                            {
                                lastInspectionCai.AllElements.Add(elem);
                            }
                            foreach (Element elem in improvedCai.CaiElements.Except(lastInspectionCai.CaiElements))
                            {
                                lastInspectionCai.CaiElements.Add(elem);
                            }
                            break;
                    }
                }
            }

            //CaiFormula caif = dbObj.GetCaiFormula(improvedCai.CaiFormulaId);
            string formulaExpression = caiFormulaObj.Formula;
            bool isCulvert = IsCulvert(improvedCai.NbiRatings.CulvertRating);
            formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, improvedCai.CaiElements, improvedCai);
            formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, improvedCai.NbiRatings, caiFormulaObj.CaiFormulaId, isCulvert, improvedCai);
            try
            {
                improvedCai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
            }
            catch { }
        }

        public void ImproveCai(Cai improvedCai, List<StructureWorkAction> strWorkActions, Structure structure)
        {
            foreach (StructureWorkAction strWorkAction in strWorkActions)
            {
                List<ElementBenefit> elementBenefits = dbObj.GetWorkActionElementBenefits(strWorkAction.WorkActionCode);
                if (elementBenefits.Count > 0)
                {
                    ApplyWorkActionElementBenefits(elementBenefits, improvedCai.AllElements);
                    ApplyWorkActionElementBenefits(elementBenefits, improvedCai.CaiElements);
                }

                List<NbiBenefit> nbiBenefits = dbObj.GetWorkActionNbiBenefits(strWorkAction.WorkActionCode);
                if (nbiBenefits.Count > 0)
                {
                    ApplyWorkActionNbiBenefits(nbiBenefits, improvedCai.NbiRatings, structure);
                }
            }

            //CaiFormula caif = dbObj.GetCaiFormula(improvedCai.CaiFormulaId);
            string formulaExpression = caiFormulaObj.Formula;
            bool isCulvert = IsCulvert(improvedCai.NbiRatings.CulvertRating);
            formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, improvedCai.CaiElements, improvedCai);
            formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, improvedCai.NbiRatings, caiFormulaObj.CaiFormulaId, isCulvert, improvedCai);

            try
            {
                improvedCai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
            }
            catch { }
        }

        public void DeteriorateCai(Cai deterioratedCai, Cai previousCai, Cai baseCai, bool deteriorateDefects, WisamType.ElementDeteriorationRates elementDeteriorationRates = WisamType.ElementDeteriorationRates.ByBrm)
        {
            //DeteriorateElementsOverAYear(deterioratedCai.AllElements, previousCai.AllElements, baseCai.AllElements, WisamType.ElementDeteriorationRates.ByElementClassification);
            //DeteriorateElementsOverAYear(deterioratedCai.CaiElements, previousCai.CaiElements, baseCai.CaiElements, WisamType.ElementDeteriorationRates.ByElementClassification);
            try
            {
                //DeteriorateElementsOverAYear(deterioratedCai.AllElements, previousCai.AllElements, baseCai.AllElements, WisamType.ElementDeteriorationRates.ByElement, deteriorateDefects);
                DeteriorateElementsOverAYear(deterioratedCai.AllElements, previousCai.AllElements, baseCai.AllElements, elementDeteriorationRates, deteriorateDefects);
            }
            catch { }
            //DeteriorateElementsOverAYear(deterioratedCai.CaiElements, previousCai.CaiElements, baseCai.CaiElements, WisamType.ElementDeteriorationRates.ByElement, deteriorateDefects);
            DeteriorateElementsOverAYear(deterioratedCai.CaiElements, previousCai.CaiElements, baseCai.CaiElements, elementDeteriorationRates, deteriorateDefects);
            DeteriorateNbiRatingsOverAYear(deterioratedCai.NbiRatings, previousCai.NbiRatings);
            //CaiFormula caif = dbObj.GetCaiFormula(deterioratedCai.CaiFormulaId);

            if (previousCai.CaiValue < 0)
            {
                deterioratedCai.CaiValue = 0;
            }
            else
            {
                string formulaExpression = caiFormulaObj.Formula;
                bool isCulvert = IsCulvert(deterioratedCai.NbiRatings.CulvertRating);
                formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, deterioratedCai.CaiElements, deterioratedCai);
                formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, deterioratedCai.NbiRatings, caiFormulaObj.CaiFormulaId, isCulvert, deterioratedCai);

                try
                {
                    deterioratedCai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
                }
                catch { }

                if (deterioratedCai.CaiValue < 0)
                {
                    deterioratedCai.CaiValue = 0;
                }
            }
        }

        public List<Cai> GetDeteriorationBasedCais(string strId, Cai currentInspectionBasedCai, int deteriorationLength, CaiFormula caif)
        {
            List<Cai> cais = new List<Cai>();
            bool isCulvert = IsCulvert(currentInspectionBasedCai.NbiRatings.CulvertRating);
            Cai previousCai = new Cai(currentInspectionBasedCai);
            int baseYear = currentInspectionBasedCai.Year;
            List<ElementClassificationDeterioration> ecDeteriorations = dbObj.GetElemClassificationsDeterioration();

            for (int i = 1; i <= deteriorationLength; i++)
            {
                Cai cai = new Cai(previousCai, baseYear + i, i);
                cai.Basis = WisamType.CaiBases.Deterioration;
                string formulaExpression = caif.Formula;

                // Determine quantities (in 4 condition states) of CAI elements in deterioration year i
                foreach (Element elem in cai.CaiElements)
                {
                    try
                    {
                        ElementClassificationDeterioration ecDeterioration =
                                                                ecDeteriorations.Where(e => e.ElementClassificationCode == elem.ElementClassificationCode).First();
                        Element previousQuantities = previousCai.CaiElements.Where(e => e.ElemNum == elem.ElemNum).First();
                        int cs1BaseQuantity = currentInspectionBasedCai.CaiElements.Where(e => e.ElemNum == elem.ElemNum).First().Cs1Quantity;

                        // CS1 quantity
                        double q1 = previousQuantities.Cs1Quantity - (cs1BaseQuantity * (50 / ecDeterioration.MedYr1) / 100);
                        elem.Cs1Quantity = Convert.ToInt32(Math.Round(q1, MidpointRounding.AwayFromZero));

                        // CS2 quantity
                        double q2 = previousQuantities.Cs2Quantity * (1 - ((50 / ecDeterioration.MedYr2) / 100))
                                        + previousQuantities.Cs1Quantity
                                        - (previousQuantities.Cs1Quantity * (1 - ((50 / ecDeterioration.MedYr1) / 100)));
                        elem.Cs2Quantity = Convert.ToInt32(Math.Round(q2, MidpointRounding.AwayFromZero));

                        // CS3 quantity
                        double q3 = previousQuantities.Cs3Quantity * (1 - ((50 / ecDeterioration.MedYr3) / 100))
                                        + previousQuantities.Cs2Quantity
                                        - (previousQuantities.Cs2Quantity * (1 - ((50 / ecDeterioration.MedYr2) / 100)));
                        elem.Cs3Quantity = Convert.ToInt32(Math.Round(q3, MidpointRounding.AwayFromZero));

                        // CS4 quantity
                        elem.Cs4Quantity = previousQuantities.TotalQuantity - elem.Cs1Quantity - elem.Cs2Quantity - elem.Cs3Quantity;

                        if (elem.Cs4Quantity < 0)
                        {
                            elem.Cs4Quantity = 0;
                        }

                        // Update same element in AllElements
                        Element aeElem = cai.AllElements.Where(e => e.ElemNum == elem.ElemNum).First();
                        aeElem.Cs1Quantity = elem.Cs1Quantity;
                        aeElem.Cs2Quantity = elem.Cs2Quantity;
                        aeElem.Cs3Quantity = elem.Cs3Quantity;
                        aeElem.Cs4Quantity = elem.Cs4Quantity;
                    }
                    catch { }
                }

                formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caif.ElementClassCaiReductions, cai.CaiElements, cai);

                // NBI 
                if (!isCulvert)
                {
                    cai.NbiRatings.DeckRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiDeck, currentInspectionBasedCai.NbiRatings.DeckDeteriorationYear + i, strId);
                    cai.NbiRatings.DeckRating = cai.NbiRatings.DeckRatingVal.ToString();
                    cai.NbiRatings.DeckDeteriorationYear = currentInspectionBasedCai.NbiRatings.DeckDeteriorationYear + i;
                    cai.NbiRatings.SuperstructureRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiSuperstructure, currentInspectionBasedCai.NbiRatings.SuperstructureDeteriorationYear + i, strId);
                    cai.NbiRatings.SuperstructureRating = cai.NbiRatings.SuperstructureRatingVal.ToString();
                    cai.NbiRatings.SuperstructureDeteriorationYear = currentInspectionBasedCai.NbiRatings.SuperstructureDeteriorationYear + i;
                    cai.NbiRatings.SubstructureRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiSubstructure, currentInspectionBasedCai.NbiRatings.SubstructureDeteriorationYear + i, strId);
                    cai.NbiRatings.SubstructureRating = cai.NbiRatings.SubstructureRatingVal.ToString();
                    cai.NbiRatings.SubstructureDeteriorationYear = currentInspectionBasedCai.NbiRatings.SubstructureDeteriorationYear + i;
                }
                else
                {
                    cai.NbiRatings.CulvertRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiCulvert, currentInspectionBasedCai.NbiRatings.CulvertDeteriorationYear + i, strId);
                    cai.NbiRatings.CulvertRating = cai.NbiRatings.CulvertRatingVal.ToString();
                    cai.NbiRatings.CulvertDeteriorationYear = currentInspectionBasedCai.NbiRatings.CulvertDeteriorationYear + i;
                }

                formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caif.NbiCaiReductions, cai.NbiRatings, caif.CaiFormulaId, isCulvert, cai);

                try
                {
                    cai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
                }
                catch { }

                cais.Add(cai);
                previousCai = cai;
            }

            return cais;
        }

        private string UpdateCaiFormulaExpressionElement(string formulaExpression, List<ElementClassificationCaiReduction> elementCaiReductions,
                                                            List<Element> caiElements, Cai cai)
        {
            string updatedExpression = formulaExpression;
            string debugInfo = "";

            foreach (ElementClassificationCaiReduction eccr in elementCaiReductions)
            {
                string elementInfo = "";
                double reductionValue = 0;
                debugInfo += String.Format("{0}: -{1} = ", eccr.ElementClassificationCode, eccr.ReductionFormula);

                // Elements of this classification
                int q1Total = 0;
                int q2Total = 0;
                int q3Total = 0;
                int q4Total = 0;
                int totalTotal = 0;
                string reductionFormula = eccr.ReductionFormula;

                foreach (Element elem in caiElements.Where(e => e.ElementClassificationCode == eccr.ElementClassificationCode).ToList())
                {
                    //string reductionFormula = eccr.ReductionFormula;
                    //reductionFormula = reductionFormula.Replace("Q1", elem.Cs1Quantity.ToString()).Replace("Q2", elem.Cs2Quantity.ToString())
                    //.Replace("Q3", elem.Cs3Quantity.ToString()).Replace("Q4", elem.Cs4Quantity.ToString())
                    //.Replace("QT", (elem.Cs1Quantity + elem.Cs2Quantity + elem.Cs3Quantity + elem.Cs4Quantity).ToString());
                    //TODO: 10/7
                    //reductionValue += Convert.ToDouble(new DataTable().Compute(reductionFormula, null));
                    q1Total += elem.Cs1Quantity;
                    q2Total += elem.Cs2Quantity;
                    q3Total += elem.Cs3Quantity;
                    q4Total += elem.Cs4Quantity;

                    elementInfo += String.Format("{0}: {1}, {2}, {3}, {4}\r\n", elem.ElemNum, elem.Cs1Quantity, elem.Cs2Quantity, elem.Cs3Quantity, elem.Cs4Quantity);
                }

                totalTotal = q1Total + q2Total + q3Total + q4Total;
                if (totalTotal == 0)
                {
                    totalTotal = 1;
                }
                reductionFormula = reductionFormula.Replace("Q1", q1Total.ToString()).Replace("Q2", q2Total.ToString())
                                                        .Replace("Q3", q3Total.ToString()).Replace("Q4", q4Total.ToString())
                                                        .Replace("QT", totalTotal.ToString());
                reductionValue += Convert.ToDouble(new DataTable().Compute(reductionFormula, null));

                debugInfo += String.Format("-{0}\r\n", reductionValue);
                debugInfo += String.Format("{0}\r\n", elementInfo);
                updatedExpression = updatedExpression.Replace(eccr.ElementClassificationCode, reductionValue.ToString());
            }

            cai.DebugInfo += debugInfo;
            return updatedExpression;
        }

        private string UpdateCaiFormulaExpressionNbi(string formulaExpression, List<NbiClassificationCaiReduction> nbiCaiReductions,
                                                        NbiRating nbiRatings, int caiFormulaId, bool isCulvert, Cai cai)
        {
            string updatedExpression = formulaExpression;
            string debugInfo = "";
            bool finishedCulvert = false;

            foreach (NbiClassificationCaiReduction nccr in nbiCaiReductions)
            {
                double reductionValue = 0;
                double actualRating = 0;
                int roundedUpRating = 0;

                // Deteriorated ratings are decimal numbers. They're rounded to nearest integer using this rule - round up .5 and higher, else round down.
                if (!isCulvert)
                {
                    switch (nccr.NbiClassificationCode)
                    {
                        case Code.NbiDeck:
                            actualRating = nbiRatings.DeckRatingVal;
                            roundedUpRating = Convert.ToInt32(Math.Round(nbiRatings.DeckRatingVal, MidpointRounding.AwayFromZero));
                            reductionValue = dbObj.GetNbiReductionValue(roundedUpRating, caiFormulaId, WisamType.NbiRatingTypes.Deck);
                            break;
                        case Code.NbiSuperstructure:
                            actualRating = nbiRatings.SuperstructureRatingVal;
                            roundedUpRating = Convert.ToInt32(Math.Round(nbiRatings.SuperstructureRatingVal, MidpointRounding.AwayFromZero));
                            reductionValue = dbObj.GetNbiReductionValue(roundedUpRating, caiFormulaId, WisamType.NbiRatingTypes.Superstructure);
                            break;
                        case Code.NbiSubstructure:
                            actualRating = nbiRatings.SubstructureRatingVal;
                            roundedUpRating = Convert.ToInt32(Math.Round(nbiRatings.SubstructureRatingVal, MidpointRounding.AwayFromZero));
                            reductionValue = dbObj.GetNbiReductionValue(roundedUpRating, caiFormulaId, WisamType.NbiRatingTypes.Substructure);
                            break;
                    }
                }
                else
                {
                    if (!finishedCulvert)
                    {
                        actualRating = nbiRatings.CulvertRatingVal;
                        roundedUpRating = Convert.ToInt32(Math.Round(nbiRatings.CulvertRatingVal, MidpointRounding.AwayFromZero));
                        reductionValue = dbObj.GetNbiReductionValue(roundedUpRating, caiFormulaId, WisamType.NbiRatingTypes.Culvert);
                        finishedCulvert = true;
                    }
                }

                debugInfo += String.Format("{0}: -{1} at {2} ({3})\r\n", nccr.NbiClassificationCode, reductionValue, actualRating, roundedUpRating);
                updatedExpression = updatedExpression.Replace(nccr.NbiClassificationCode, reductionValue.ToString());
            }

            cai.DebugInfo += String.Format("{0}-------------------------------------------------------\r\n\r\n", debugInfo);
            return updatedExpression;
        }

        public Cai GetInspectionBasedCai(string strId, int caiFormulaId, DateTime inspectionDate)
        {
            Cai cai = new Cai();
            cai.Year = inspectionDate.Year;
            cai.CaiFormulaId = caiFormulaId;
            cai.AllElements = dbObj.GetElements(strId, inspectionDate);
            cai.CaiElements = dbObj.GetElements(strId, caiFormulaId, inspectionDate);
            cai.NbiRatings = dbObj.GetNbiRating(strId, inspectionDate);
            cai.Basis = WisamType.CaiBases.Inspection;
            bool isCulvert = IsCulvert(cai.NbiRatings.CulvertRating);
            //CaiFormula caif = dbObj.GetCaiFormula(caiFormulaId);
            string formulaExpression = caiFormulaObj.Formula;
            formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, cai.CaiElements, cai);
            formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, cai.NbiRatings, caiFormulaId, isCulvert, cai);

            try
            {
                cai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
            }
            catch { }

            return cai;
        }

        public Cai GetInspectionBasedCai(string strId, int caiFormulaId, int inspectionYear)
        {
            Cai cai = new Cai();
            cai.Year = inspectionYear;
            cai.CaiFormulaId = caiFormulaId;
            cai.AllElements = dbObj.GetElements(strId, inspectionYear);
            cai.CaiElements = dbObj.GetElements(strId, caiFormulaId, inspectionYear);
            cai.NbiRatings = dbObj.GetNbiRating(strId, inspectionYear);
            cai.Basis = WisamType.CaiBases.Inspection;
            bool isCulvert = IsCulvert(cai.NbiRatings.CulvertRating);
            //CaiFormula caif = dbObj.GetCaiFormula(caiFormulaId);
            string formulaExpression = caiFormulaObj.Formula;
            formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, cai.CaiElements, cai);
            formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, cai.NbiRatings, caiFormulaId, isCulvert, cai);

            try
            {
                cai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
            }
            catch { }

            return cai;
        }



        //CaiFormula cf = GetCaiFormula(cfId);
        //return cf.Formula.Split(new string[5] { " ", "-", "+", "*", "/" }, StringSplitOptions.RemoveEmptyEntries);

        public List<Element> GetCaiElements(List<Element> sourceElements, CaiFormula caif)
        {
            List<Element> caiElements = new List<Element>();
            List<string> caiClassifications = caif.Formula.Split(new string[5] { " ", "-", "+", "*", "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (caiClassifications.Count() > 0)
            {
                foreach (Element elem in sourceElements)
                {
                    if (caiClassifications.FindIndex(e => e == elem.ElementClassificationCode) >= 0)
                    {
                        caiElements.Add(elem);
                    }
                }
            }

            return caiElements;
        }

        public Cai GetLastInspectionBasedCai(Structure str)
        {
            Cai cai = null;
            string formulaExpression = caiFormulaObj.Formula.ToUpper();
            List<Element> allElements = str.LastInspection.Elements;
            List<Element> caiElements = GetCaiElements(allElements, caiFormulaObj);
            NbiRating nbi = str.LastInspection.NbiRatings;

            if (allElements != null || nbi != null)
            {
                cai = new Cai();
                cai.CaiFormulaId = caiFormulaObj.CaiFormulaId;
                cai.Formula = caiFormulaObj.Formula;
                cai.Basis = WisamType.CaiBases.Inspection;

                if (allElements != null)
                {
                    cai.AllElements = allElements;
                    cai.Year = allElements[0].InspectionDate.Year;

                    if (caiElements != null)
                    {
                        cai.CaiElements = caiElements;
                        formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, cai.CaiElements, cai);
                    }
                }

                if (nbi != null)
                {
                    cai.NbiRatings = nbi;
                    bool isCulvert = IsCulvert(cai.NbiRatings.CulvertRating);
                    formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, cai.NbiRatings, caiFormulaObj.CaiFormulaId, isCulvert, cai);
                }

                cai.FormulaWithValues = formulaExpression;
                try
                {
                    cai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
                }
                catch { }
            }

            return cai;
        }

        public void AddOverlayDefects(Element parentElement, List<Element> allElements, int deteriorationYear, List<int> defects)
        {
            foreach (int defect in defects)
            {
                Element defectElement = null;
                int cs1Quantity = 0;

                try
                {
                    defectElement = allElements.Where(e => e.ElemNum == defect && e.ParentElemNum == parentElement.ElemNum).First();
                }
                catch { }

                // defect doesn't exist
                if (defectElement == null)
                {
                    defectElement = new Element(defect);
                    defectElement.ParentElemNum = parentElement.ElemNum;
                    defectElement.StructureId = parentElement.StructureId;
                    defectElement.ElementClassificationCode = Code.Defect;
                    defectElement.DeteriorationYear = deteriorationYear;
                    defectElement.Cs2Quantity = 0;
                    defectElement.Cs3Quantity = 0;
                    defectElement.Cs4Quantity = 0;

                    switch (defect)
                    {
                        case 3210:
                            cs1Quantity = Convert.ToInt32(parentElement.TotalQuantity * 0.5);
                            break;
                        case 3220:
                        case 8911:
                            cs1Quantity = Convert.ToInt32(parentElement.TotalQuantity * 0.25);
                            break;
                    }

                    defectElement.Cs1Quantity = cs1Quantity;
                    defectElement.TotalQuantity = cs1Quantity;
                    allElements.Add(defectElement);
                }
                else if (defectElement.TotalQuantity >= 0) // defect exists
                {
                    switch (defect)
                    {
                        case 3210:
                            cs1Quantity = Convert.ToInt32((parentElement.TotalQuantity - (defectElement.Cs2Quantity + defectElement.Cs3Quantity + defectElement.Cs4Quantity)) * 0.5);
                            break;
                        case 3220:
                        case 8911:
                            cs1Quantity = Convert.ToInt32((parentElement.TotalQuantity - (defectElement.Cs2Quantity + defectElement.Cs3Quantity + defectElement.Cs4Quantity)) * 0.25);
                            break;
                    }

                    defectElement.Cs1Quantity = cs1Quantity;
                    defectElement.TotalQuantity = defectElement.Cs1Quantity + defectElement.Cs2Quantity + defectElement.Cs3Quantity + defectElement.Cs4Quantity;
                }
            }
        }

        public void AddDefects(Element parentElem, List<Element> elementsToAddDefectsTo, int deteriorationYear, List<int> defectsList = null, List<int> defectsPercentages = null,
                                List<Element> existingDefects = null)
        {
            if (defectsList == null)
            {
                if (defectsPercentages == null)
                {
                    return;
                }

                defectsList = dbObj.GetDefects(parentElem.ElemNum);
            }

            int counter = 0;
            Element defect1080 = null;

            foreach (int defectNum in defectsList)
            {
                int cs1Quantity = Convert.ToInt32(Math.Truncate((defectsPercentages[counter] / 100.0) * parentElem.Cs1Quantity));
                Element defectElement = null;

                try
                {
                    defectElement = elementsToAddDefectsTo.Where(e => e.ElemNum == defectNum && e.ParentElemNum == parentElem.ElemNum).First();
                }
                catch { }


                if (defectElement == null) // Defect doesn't exist; create it and assign 25% of parent's CS1 quantity to its CS1 quantity 
                {
                    Element defect = new Element(defectNum);
                    defect.ParentElemNum = parentElem.ElemNum;
                    defect.StructureId = elementsToAddDefectsTo[0].StructureId;
                    defect.ElementClassificationCode = Code.Defect;
                    defect.DeteriorationYear = deteriorationYear;
                    defect.EquivalentAge = 0;
                    defect.Cs1Quantity = cs1Quantity;
                    defect.Cs2Quantity = 0;
                    defect.Cs3Quantity = 0;
                    defect.Cs4Quantity = 0;
                    defect.TotalQuantity = cs1Quantity;
                    elementsToAddDefectsTo.Add(defect);
                    if (defectNum == 1080)
                    {
                        defect1080 = defect;
                    }
                }
                else if (defectElement.TotalQuantity > 0) // Defect exists
                {
                    //defectElement.Cs1Quantity = cs1Quantity;
                    if (defectNum == 1080)
                    {
                        defectElement.Cs1Quantity = Convert.ToInt32(Math.Truncate(0.25 * (parentElem.TotalQuantity - defectElement.Cs2Quantity - defectElement.Cs3Quantity - defectElement.Cs4Quantity)));
                        defectElement.TotalQuantity = defectElement.Cs1Quantity + defectElement.Cs2Quantity + defectElement.Cs3Quantity + defectElement.Cs4Quantity;
                        defect1080 = defectElement;
                    }
                    else if (defectElement.ElemNum == 1130)
                    {
                        if (defect1080 != null)
                        {
                            defectElement.Cs1Quantity = Convert.ToInt32(0.25 * (parentElem.TotalQuantity - defect1080.Cs2Quantity - defect1080.Cs3Quantity
                                                                                - defect1080.Cs4Quantity - defectElement.Cs2Quantity - defectElement.Cs3Quantity
                                                                                - defectElement.Cs4Quantity));
                        }
                        else
                        {
                            defectElement.Cs1Quantity = Convert.ToInt32(0.25 * (parentElem.TotalQuantity - -defectElement.Cs2Quantity - defectElement.Cs3Quantity
                                                                                - defectElement.Cs4Quantity));
                        }
                        defectElement.TotalQuantity = defectElement.Cs1Quantity + defectElement.Cs2Quantity + defectElement.Cs3Quantity + defectElement.Cs4Quantity;
                    }
                }

                counter++;
            }

        }

        /*
        public void AddDefects(int parentElemNum, List<Element> elementsToAddDefectsTo, int deteriorationYear, List<int> defectsList = null, List<int> defectsPercentages = null)
        {
            if (defectsList == null)
            {
                if (defectsPercentages == null)
                {
                    return;
                }

                defectsList = dbObj.GetDefects(parentElemNum);
            }

            Element parentElem = null;

            try
            {
                parentElem = elementsToAddDefectsTo.Where(e => e.ElemNum == parentElemNum).First();
            }
            catch 
            { }

            if (parentElem != null)
            {
                int counter = 0;
                
                foreach (int defectNum in defectsList)
                {
                    int cs1Quantity = Convert.ToInt32(Math.Truncate((defectsPercentages[counter] / 100.0) * parentElem.Cs1Quantity));
                    Element defectElement = null;

                    try
                    {
                        defectElement = elementsToAddDefectsTo.Where(e => e.ElemNum == defectNum && e.ParentElemNum == parentElemNum).First();
                    }
                    catch { }

                    if (defectElement == null)
                    {
                        Element defect = new Element(defectNum);
                        defect.ParentElemNum = parentElemNum;
                        defect.StructureId = elementsToAddDefectsTo[0].StructureId;
                        defect.ElementClassificationCode = Code.Defect;
                        defect.DeteriorationYear = deteriorationYear;
                        defect.Cs1Quantity = cs1Quantity;
                        defect.Cs2Quantity = 0;
                        defect.Cs3Quantity = 0;
                        defect.Cs4Quantity = 0;
                        defect.TotalQuantity = cs1Quantity;
                        elementsToAddDefectsTo.Add(defect);
                    }
                    else
                    {
                        defectElement.Cs1Quantity = cs1Quantity;
                        defectElement.TotalQuantity = defectElement.Cs1Quantity + defectElement.Cs2Quantity + defectElement.Cs3Quantity + defectElement.Cs4Quantity;
                    }

                    counter++;
                }
            }
        }
        */

        /*
        public void AddOverlayDefects(List<Element> elementsToAddDefectsTo, int deteriorationYear)
        {
            // Get defects of elementsToAddDefectsTo
            // hard code for now
            List<int> defectNums = new List<int> { 3210, 3220, 8911 };
            var olayElements = elementsToAddDefectsTo.Where(e => e.ElementClassificationCode == Code.Overlay).ToList();

            foreach (Element olayElement in olayElements)
            {
                foreach (int defectNum in defectNums)
                {
                    if (elementsToAddDefectsTo.Where(e => e.ElemNum == defectNum && e.ParentElemNum == olayElement.ElemNum).Count() == 0)
                    {
                        Element defect = new Element(defectNum);
                        defect.ParentElemNum = olayElement.ElemNum;
                        defect.StructureId = olayElement.StructureId;
                        defect.ElementClassificationCode = Code.Defect;
                        defect.DeteriorationYear = deteriorationYear;
                        defect.Cs1Quantity = 0;
                        defect.Cs2Quantity = 0;
                        defect.Cs3Quantity = 0;
                        defect.Cs4Quantity = 0;
                        elementsToAddDefectsTo.Add(defect);
                    }
                }
            }
        }
        */

        public Cai GetLastInspectionBasedCai(Structure str, int caiFormulaId)
        {
            Cai cai = null;
            string formulaExpression = caiFormulaObj.Formula.ToUpper();
            List<Element> allElements = str.LastInspection.Elements;
            List<Element> caiElements = dbObj.GetCaiLastInspectionElements(str.StructureId, caiFormulaId);
            NbiRating nbi = str.LastInspection.NbiRatings;

            if (allElements != null || nbi != null)
            {
                cai = new Cai();
                cai.CaiFormulaId = caiFormulaId;
                cai.Formula = formulaExpression;
                cai.Basis = WisamType.CaiBases.Inspection;

                if (allElements != null)
                {
                    foreach (Element element in allElements)
                    {
                        if (element.Cs1Quantity > 0)
                        {
                            if (element.Cs1Quantity == element.TotalQuantity)
                            {
                                element.EquivalentAge = 0;
                            }
                            else
                            {
                                ElementDeterioration elementDeterioration = null;

                                // Check whether element has its own deterioration model
                                if (elementDeteriorations.Where(e => e.ElemNum == element.ElemNum).Count() > 0)
                                {
                                    elementDeterioration = elementDeteriorations.Where(e => e.ElemNum == element.ElemNum).First();
                                }
                                else // If not, use its parent's
                                {
                                    if (element.ParentElemNum != 0)
                                    {
                                        if (elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).Count() > 0)
                                        {
                                            elementDeterioration = elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).First();
                                        }
                                    }
                                }

                                if (elementDeterioration != null)
                                {
                                    double cs1Percentage = (element.Cs1Quantity * 1.0 / element.TotalQuantity * 1.0);
                                    double naturalLog = -Math.Log(cs1Percentage, Math.E);
                                    double power = Math.Log(naturalLog, 10) / elementDeterioration.Beta;
                                    double powerResult = Math.Pow(10, power);
                                    double age = elementDeterioration.ScalingFactor1 * powerResult;
                                    element.EquivalentAge = Convert.ToInt32(Math.Round(age));
                                }
                            }
                        }
                        else
                        {
                            element.EquivalentAge = 0;
                        }
                    }

                    var deckElements = allElements.Where(e => (e.ElementClassificationCode == Code.Deck || e.ElementClassificationCode == Code.Slab)
                                                                                && (e.ElemNum.ToString().Equals("12")
                                                                                    || e.ElemNum.ToString().Equals("16")
                                                                                    || e.ElemNum.ToString().Equals("38")
                                                                                    || e.ElemNum.ToString().Equals("13")
                                                                                    || e.ElemNum.ToString().Equals("8039")
                                                                                    || e.ElemNum.ToString().Equals("60")
                                                                                    || e.ElemNum.ToString().Equals("65"))).ToList();

                    foreach (Element deckElement in deckElements)
                    {
                        var existingDefects = allElements.Where(e => (e.ElemNum == 1080 || e.ElemNum == 1130)
                                && (e.Cs2Quantity > 0 || e.Cs3Quantity > 0 || e.Cs4Quantity > 0)
                                && e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckElement.ElemNum).ToList();
                        // Add 1080 or 1130 if existing
                        if (existingDefects.Count() > 0)
                        {
                            AddDefects(deckElement, allElements, 0, new List<int>() { 1080, 1130 }, new List<int>() { 25, 25 }, existingDefects);
                        }
                    }

                    var overlayElements = allElements.Where(e => e.ElementClassificationCode == Code.Overlay).ToList();
                    foreach (Element overlayElement in overlayElements)
                    {
                        //AddDefects(overlayElement.ElemNum, allElements, 0, new List<int>() { 3210, 3220, 8911 }, new List<int>() { 50, 25, 25 });
                        //AddDefects(overlayElement, allElements, 0, new List<int>() { 3210, 3220, 8911 }, new List<int>() { 50, 25, 25 });
                        AddOverlayDefects(overlayElement, allElements, 0, new List<int>() { 3210, 3220, 8911 });
                    }

                    cai.AllElements = allElements;
                    cai.Year = allElements[0].InspectionDate.Year;

                    if (caiElements != null)
                    {
                        cai.CaiElements = caiElements;
                        formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, cai.CaiElements, cai);
                    }
                }

                if (nbi != null)
                {
                    cai.NbiRatings = nbi;
                    bool isCulvert = IsCulvert(cai.NbiRatings.CulvertRating);
                    formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, cai.NbiRatings, caiFormulaId, isCulvert, cai);
                }

                cai.FormulaWithValues = formulaExpression;
                try
                {
                    cai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
                }
                catch { }
            }

            return cai;
        }

        public Cai GetLastInspectionBasedCai(string strId, int caiFormulaId)
        {
            Cai cai = null;
            //CaiFormula caif = dbObj.GetCaiFormula(caiFormulaId);
            string formulaExpression = caiFormulaObj.Formula.ToUpper();
            List<Element> allElements = dbObj.GetLastInspectionElements(strId);
            List<Element> caiElements = dbObj.GetCaiLastInspectionElements(strId, caiFormulaId);
            NbiRating nbi = dbObj.GetLastNbiRating(strId);

            if (allElements != null || nbi != null)
            {
                cai = new Cai();
                cai.CaiFormulaId = caiFormulaId;
                cai.Formula = formulaExpression;
                cai.Basis = WisamType.CaiBases.Inspection;

                if (allElements != null)
                {


                    var deckElements = allElements.Where(e => e.ElementClassificationCode == Code.Deck
                                                                                && (e.ElemNum.ToString().Equals("12")
                                                                                    || e.ElemNum.ToString().Equals("16")
                                                                                    || e.ElemNum.ToString().Equals("38")
                                                                                    || e.ElemNum.ToString().Equals("13")
                                                                                    || e.ElemNum.ToString().Equals("8039")
                                                                                    || e.ElemNum.ToString().Equals("60")
                                                                                    || e.ElemNum.ToString().Equals("65"))).ToList();
                    foreach (Element deckElement in deckElements)
                    {
                        if (allElements.Where(e => e.ElemNum == 1080
                                && (e.Cs2Quantity > 0 || e.Cs3Quantity > 0 || e.Cs4Quantity > 0)
                                && e.ElementClassificationCode.Equals(Code.Defect) && e.ParentElemNum == deckElement.ElemNum).Count() > 0)
                        {
                            AddDefects(deckElement, allElements, 0, new List<int>() { 1080 }, new List<int>() { 25 });
                        }
                    }

                    var overlayElements = allElements.Where(e => e.ElementClassificationCode == Code.Overlay).ToList();
                    foreach (Element overlayElement in overlayElements)
                    {
                        //AddDefects(overlayElement.ElemNum, allElements, 0, new List<int>() { 3210, 3220, 8911 }, new List<int>() { 50, 25, 25 });
                        //AddDefects(overlayElement, allElements, 0, new List<int>() { 3210, 3220, 8911 }, new List<int>() { 50, 25, 25 });
                        AddOverlayDefects(overlayElement, allElements, 0, new List<int>() { 3210, 3220, 8911 });
                    }

                    cai.AllElements = allElements;
                    cai.Year = allElements[0].InspectionDate.Year;

                    if (caiElements != null)
                    {
                        cai.CaiElements = caiElements;
                        formulaExpression = UpdateCaiFormulaExpressionElement(formulaExpression, caiFormulaObj.ElementClassCaiReductions, cai.CaiElements, cai);
                    }
                }

                if (nbi != null)
                {
                    cai.NbiRatings = nbi;
                    bool isCulvert = IsCulvert(cai.NbiRatings.CulvertRating);
                    formulaExpression = UpdateCaiFormulaExpressionNbi(formulaExpression, caiFormulaObj.NbiCaiReductions, cai.NbiRatings, caiFormulaId, isCulvert, cai);
                }

                cai.FormulaWithValues = formulaExpression;
                try
                {
                    cai.CaiValue = Convert.ToDouble(new DataTable().Compute(formulaExpression, null));
                }
                catch { }
            }

            return cai;
        }

        public double GetLastInspectionBasedCaiValue(string strId, int caiFormulaId)
        {
            return GetLastInspectionBasedCai(strId, caiFormulaId).CaiValue;
        }

        public List<Cai> GetInspectionBasedCais(string strId, int caiFormulaId)
        {
            List<Cai> cais = new List<Cai>();
            List<DateTime> inspectionDates = dbObj.GetElementInspectionDates(strId);

            foreach (DateTime inspectionDate in inspectionDates)
            {
                cais.Add(GetInspectionBasedCai(strId, caiFormulaId, inspectionDate));
            }

            return cais;
        }

        public List<Cai> GetDoNothingCais(string strId, int caiFormulaId, int startYear, int endYear)
        {
            List<Cai> cais = null;
            Cai currentCai = GetLastInspectionBasedCai(strId, caiFormulaId);
            //CaiFormula caif = dbObj.GetCaiFormula(caiFormulaId);

            if (currentCai != null)
            {
                cais = new List<Cai>();
                cais.Add(currentCai);
                cais.AddRange(GetDeteriorationBasedCais(strId, currentCai, endYear - currentCai.Year, caiFormulaObj));
            }

            return cais;
        }
        #endregion CAI Methods

        #region Utility Methods
        public string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public string GetRandomFileName(string baseDir, string newFileExt)
        {
            string newPath = Path.GetRandomFileName();
            string fileExt = Path.GetExtension(newPath);
            newPath = newPath.Replace(fileExt, newFileExt);
            newPath = Path.Combine(baseDir, newPath);
            return newPath;
        }

        public string GetRandomExcelFileName(string baseDir)
        {
            string newPath = Path.GetRandomFileName();
            string fileExt = Path.GetExtension(newPath);
            newPath = newPath.Replace(fileExt, ".xlsx");
            newPath = Path.Combine(baseDir, newPath);
            return newPath;
        }
        #endregion Utility Methods

        #region FIIPS Methods
        public List<StructureWorkAction> GetProgrammedWorkActions(DateTime startDate, DateTime endDate, string strId)
        {
            return dbObj.GetProgrammedWorkActions(startDate, endDate, strId);
        }
        #endregion FIIPS Methods


        #region Element, NBI and Inspection Methods
        public List<Element> DeteriorateElements(string strId)
        {
            List<Element> lastInspElems = dbObj.GetLastInspectionElements(strId);
            List<Element> deterioratedElems = DeteriorateElementsOverAYear(lastInspElems, lastInspElems, WisamType.ElementDeteriorationRates.ByElementClassification);

            return deterioratedElems;
        }

        public void DeteriorateElementsOverAYear(List<Element> elements, List<Element> previousElements, List<Element> baseElements, WisamType.ElementDeteriorationRates deteriorationRates, bool deteriorateDefects)
        {
            // Deterioration rates are for element classifications (vs. element-specific)
            if (deteriorationRates == WisamType.ElementDeteriorationRates.ByElementClassification)
            {
                // Grab deterioration rates for all element classifications
                List<ElementClassificationDeterioration> ecDeteriorations = dbObj.GetElemClassificationsDeterioration();

                // Loop through elements and determine new quantities based on 1-year deterioration
                foreach (Element previousElement in previousElements)
                {
                    // Grab element to deteriorate

                    if (previousElement.ElemNum == 8000 || previousElement.ElemNum == 8511 ||
                        previousElement.ElemNum == 8512 || previousElement.ElemNum == 8513 ||
                        previousElement.ElemNum == 8514 || previousElement.ElemNum == 8515 || previousElement.ParentElemNum == 8516)
                    {
                        int z = 0;
                    }

                    Element element = elements.Where(e => e.ElemNum == previousElement.ElemNum).First();
                    element.DeteriorationYear = previousElement.DeteriorationYear + 1;

                    // Grab deterioration rates for the element based on its classification code; then deteriorate element
                    var currentDRList = ecDeteriorations.Where(e => e.ElementClassificationCode == element.ElementClassificationCode).ToList();

                    if (currentDRList.Count() > 0)
                    {
                        var currentDR = currentDRList.First();
                        try
                        {
                            // Grab element's initial CS1 quantity
                            int baseCs1Quantity = 0;
                            if (baseElements.Where(e => e.ElemNum == element.ElemNum).Count() > 0)
                            {
                                baseCs1Quantity = baseElements.Where(e => e.ElemNum == element.ElemNum).First().Cs1Quantity;
                            }
                            else
                            {
                                // TODO: 
                                baseCs1Quantity = previousElement.TotalQuantity;
                            }

                            // New CS1 quantity
                            double newCs1Quantity = previousElement.Cs1Quantity - (baseCs1Quantity * (50 / currentDR.MedYr1) / 100);
                            element.Cs1Quantity = Convert.ToInt32(Math.Round(newCs1Quantity, MidpointRounding.AwayFromZero));

                            if (element.Cs1Quantity < 0)
                            {
                                element.Cs1Quantity = 0;
                            }

                            // New CS2 quantity
                            double newCs2Quantity = previousElement.Cs2Quantity * (1 - ((50 / currentDR.MedYr2) / 100))
                                                        + previousElement.Cs1Quantity
                                                        - (previousElement.Cs1Quantity * (1 - ((50 / currentDR.MedYr1) / 100)));
                            element.Cs2Quantity = Convert.ToInt32(Math.Round(newCs2Quantity, MidpointRounding.AwayFromZero));

                            if (element.Cs2Quantity < 0)
                            {
                                element.Cs2Quantity = 0;
                            }

                            // New CS3 quantity
                            double newCs3Quantity = previousElement.Cs3Quantity * (1 - ((50 / currentDR.MedYr3) / 100))
                                                        + previousElement.Cs2Quantity
                                                        - (previousElement.Cs2Quantity * (1 - ((50 / currentDR.MedYr2) / 100)));
                            element.Cs3Quantity = Convert.ToInt32(Math.Round(newCs3Quantity, MidpointRounding.AwayFromZero));

                            if (element.Cs3Quantity < 0)
                            {
                                element.Cs3Quantity = 0;
                            }

                            // New CS4 quantity
                            element.Cs4Quantity = previousElement.TotalQuantity - element.Cs1Quantity - element.Cs2Quantity - element.Cs3Quantity;

                            if (element.Cs4Quantity < 0)
                            {
                                element.Cs4Quantity = 0;
                            }

                            /*
                            foreach (Element defect in elements.Where(e => e.ParentElemNum == element.ElemNum && e.ElementClassificationCode.Equals(Code.Defect)))
                            {
                                defect.Cs1Quantity = 0;
                                defect.Cs2Quantity = element.Cs2Quantity;
                                defect.Cs3Quantity = element.Cs3Quantity;
                                defect.Cs4Quantity = element.Cs4Quantity;
                            }
                            */
                        }
                        catch { }
                    }
                }
            } // Deterioration rates are element-specific
            else if (deteriorationRates == WisamType.ElementDeteriorationRates.ByElement)
            {
                foreach (Element previousElement in previousElements)
                {
                    // Grab element in current year
                    Element element = null;
                    Element defect1080ParentElement = null;

                    try
                    {
                        element = elements.Where(e => e.ElemNum == previousElement.ElemNum && e.ParentElemNum == previousElement.ParentElemNum).First();
                        element.DeteriorationYear = previousElement.DeteriorationYear + 1;
                    }
                    catch (Exception ex)
                    { }

                    // TODO: get element rates once and store?
                    ElementDeterioration elemDet = null;

                    if (element.ElementClassificationCode != Code.Defect)
                    {
                        if (elementDeteriorations.Where(e => e.ElemNum == element.ElemNum).Any())
                        {
                            elemDet = elementDeteriorations.Where(e => e.ElemNum == element.ElemNum).First();
                        }
                        else
                        {
                            elemDet = dbObj.GetElementDeterioration(element.ElemNum);
                        }
                    }
                    else //Defect
                    {
                        if (deteriorateDefects)
                        {
                            var parents = elements.Where(e => e.ElemNum == element.ParentElemNum);

                            if (parents.Count() > 0)
                            {
                                var parent = parents.Where(e => (e.ElemNum.ToString().Equals("12")
                                                                                    || e.ElemNum.ToString().Equals("16")
                                                                                    || e.ElemNum.ToString().Equals("38")
                                                                                    || e.ElemNum.ToString().Equals("13")
                                                                                    || e.ElemNum.ToString().Equals("8039")
                                                                                    || e.ElemNum.ToString().Equals("60")
                                                                                    || e.ElemNum.ToString().Equals("65")
                                                                )
                                                                || e.ElementClassificationCode == Code.Overlay
                                                          );

                                if (parent.Count() > 0)
                                {
                                    // Right now, deteriorate only these defects: 1080 for decks, 3210/3220/8911 for overlays
                                    //if (parentElement.ElementClassificationCode == Code.Deck || parentElement.ElementClassificationCode == Code.Overlay)
                                    {
                                        if ((element.ElemNum == 1080 && (element.Cs2Quantity > 0 || element.Cs3Quantity > 0 || element.Cs4Quantity > 0)
                                                && (element.ParentElemNum.ToString().Equals("12")
                                                                                    || element.ParentElemNum.ToString().Equals("16")
                                                                                    || element.ParentElemNum.ToString().Equals("38")
                                                                                    || element.ParentElemNum.ToString().Equals("13")
                                                                                    || element.ParentElemNum.ToString().Equals("8039")
                                                                                    || element.ParentElemNum.ToString().Equals("60")
                                                                                    || element.ParentElemNum.ToString().Equals("65"))
                                            ))
                                        {
                                            Element parentElement = parent.Where(e => (e.ElemNum == element.ParentElemNum)).First();
                                            double defectPercentage = (element.Cs2Quantity + element.Cs3Quantity + element.Cs4Quantity) * 1.0 / parentElement.TotalQuantity * 1.0;

                                            if (defectPercentage > 0.05)
                                            {
                                                //elemDet = dbObj.GetElementDeterioration(element.ParentElemNum);
                                                if (elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).Any())
                                                {
                                                    elemDet = elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).First();
                                                }
                                                else
                                                {
                                                    elemDet = dbObj.GetElementDeterioration(element.ParentElemNum);
                                                }
                                                defect1080ParentElement = parentElement;
                                            }
                                        }

                                        if (element.ElemNum == 3210 || element.ElemNum == 3220 || element.ElemNum == 8911)
                                        {
                                            //elemDet = dbObj.GetElementDeterioration(element.ParentElemNum);
                                            if (elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).Any())
                                            {
                                                elemDet = elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).First();
                                            }
                                            else
                                            {
                                                elemDet = dbObj.GetElementDeterioration(element.ParentElemNum);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (elemDet != null)
                    {
                        // Grab element's initial CS1 quantity
                        int baseCs1Quantity = 0;

                        if (baseElements.Where(e => e.ElemNum == element.ElemNum && e.ParentElemNum == element.ParentElemNum).Count() > 0)
                        {
                            var els = baseElements.Where(e => e.ElemNum == element.ElemNum && e.ParentElemNum == element.ParentElemNum);

                            if (els.Count() > 0)
                            {
                                baseCs1Quantity = els.First().Cs1Quantity;
                            }
                        }

                        // New CS1 quantity
                        double part1 = Math.Pow(Math.Log(2), (1 / elemDet.Beta));
                        double part2 = elemDet.MedYr1 / part1;
                        double part3 = Math.Pow((element.DeteriorationYear / part2), elemDet.Beta);
                        double part4 = Math.Pow(Math.E, -part3);
                        double allParts = Math.Truncate(baseCs1Quantity * part4);
                        double newCs1Quantity = allParts;
                        element.Cs1Quantity = Convert.ToInt32(Math.Truncate(newCs1Quantity));

                        if (element.Cs1Quantity < 0)
                        {
                            element.Cs1Quantity = 0;
                        }

                        // New CS2 quantity
                        double newCs2Quantity = previousElement.Cs2Quantity * (1 - ((50 / elemDet.MedYr2) / 100))
                                                    + previousElement.Cs1Quantity
                                                    - element.Cs1Quantity;

                        if (newCs2Quantity < 0)
                        {
                            newCs2Quantity = 0;
                        }

                        element.Cs2Quantity = Convert.ToInt32(Math.Round(newCs2Quantity));
                        // New CS3 quantity
                        double newCs3Quantity = previousElement.Cs3Quantity * (1 - ((50 / elemDet.MedYr3) / 100))
                                                    + previousElement.Cs2Quantity
                                                    - (previousElement.Cs2Quantity * (1 - ((50 / elemDet.MedYr2) / 100)));

                        if (newCs3Quantity < 0)
                        {
                            newCs3Quantity = 0;
                        }

                        element.Cs3Quantity = Convert.ToInt32(Math.Round(newCs3Quantity));
                        // New CS4 quantity
                        element.Cs4Quantity = previousElement.TotalQuantity - element.Cs1Quantity - element.Cs2Quantity - element.Cs3Quantity;

                        if (element.Cs4Quantity < 0)
                        {
                            element.Cs4Quantity = 0;
                        }

                        element.Cs1Quantity = element.TotalQuantity - element.Cs2Quantity - element.Cs3Quantity - element.Cs4Quantity;
                    } // end- if (elemDet != null)
                }
            }
            else if (deteriorationRates == WisamType.ElementDeteriorationRates.ByBrm)
            {
                foreach (Element previousElement in previousElements)
                {
                    if (previousElement.EquivalentAge < 0)
                    {
                        continue;
                    }
                    Element element = null;
                    if (elements.Where(e => e.ElemNum == previousElement.ElemNum && e.ParentElemNum == previousElement.ParentElemNum).Any())
                    {
                        ElementDeterioration elementDeterioration = null;
                        element = elements.Where(e => e.ElemNum == previousElement.ElemNum && e.ParentElemNum == previousElement.ParentElemNum).First();
                        element.DeteriorationYear = previousElement.DeteriorationYear + 1;
                        element.EquivalentAge = previousElement.EquivalentAge + 1;
                        if (element.ElementClassificationCode.Equals(Code.Defect))
                        {
                            if (deteriorateDefects)
                            {
                                // Only defect 1080 for deck elements and defect 3210/3220/8911 for overlay elements
                                switch (element.ElemNum)
                                {
                                    case 1080:
                                        if ((element.ParentElemNum == 12
                                            || element.ParentElemNum == 13
                                            || element.ParentElemNum == 16
                                            || element.ParentElemNum == 38
                                            || element.ParentElemNum == 60
                                            || element.ParentElemNum == 65
                                            || element.ParentElemNum == 8039)
                                            && (element.Cs2Quantity > 0 || element.Cs3Quantity > 0 || element.Cs4Quantity > 0))
                                        {
                                            var deckElement = elements.Where(e => e.ElemNum == element.ParentElemNum).First();
                                            var defectPercentage = (element.Cs2Quantity + element.Cs3Quantity + element.Cs4Quantity) * 1.0 / deckElement.TotalQuantity;
                                            if (defectPercentage > 0.05)
                                            {
                                                if (elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).Any())
                                                {
                                                    elementDeterioration = elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).First();
                                                }
                                                else
                                                {
                                                    elementDeterioration = dbObj.GetElementDeterioration(element.ParentElemNum);
                                                }
                                            }
                                        }
                                        break;
                                    case 3210:
                                    case 3220:
                                    case 8911:
                                        if (elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).Any())
                                        {
                                            elementDeterioration = elementDeteriorations.Where(e => e.ElemNum == element.ParentElemNum).First();
                                        }
                                        else
                                        {
                                            elementDeterioration = dbObj.GetElementDeterioration(element.ParentElemNum);
                                        }
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (elementDeteriorations.Where(e => e.ElemNum == element.ElemNum).Any())
                            {
                                elementDeterioration = elementDeteriorations.Where(e => e.ElemNum == element.ElemNum).First();
                            }
                            else
                            {
                                elementDeterioration = dbObj.GetElementDeterioration(element.ElemNum);
                            }
                        }
                        if (elementDeterioration != null)
                        {
                            if (element.ElemNum == 3210)
                            {
                                var step = true;
                            }
                            // CS1 quantity
                            double cs1Quantity = 0;
                            if (previousElement.Cs1Quantity > 0)
                            {
                                double power = -1 * Math.Pow(element.EquivalentAge / elementDeterioration.ScalingFactor1, elementDeterioration.Beta)
                                                        + Math.Pow(previousElement.EquivalentAge / elementDeterioration.ScalingFactor1, elementDeterioration.Beta);
                                double exponent = Math.Pow(Math.E, power);
                                cs1Quantity = previousElement.Cs1Quantity * exponent;
                                element.Cs1Quantity = Convert.ToInt32(Math.Truncate(cs1Quantity));
                            }
                            // CS2 quantity
                            double cs2Quantity = 0;
                            cs2Quantity = previousElement.Cs2Quantity * (Math.Pow(0.5, 1 / elementDeterioration.MedYr2)) + previousElement.Cs1Quantity - element.Cs1Quantity;
                            element.Cs2Quantity = Convert.ToInt32(Math.Round(cs2Quantity));
                            // CS3 quantity
                            double cs3Quantity = 0;
                            cs3Quantity = previousElement.Cs3Quantity * Math.Pow(0.5, 1 / elementDeterioration.MedYr3)
                                + previousElement.Cs2Quantity
                                - (previousElement.Cs2Quantity * Math.Pow(0.5, 1 / elementDeterioration.MedYr2));
                            element.Cs3Quantity = Convert.ToInt32(Math.Round(cs3Quantity));
                            // CS4 quantity
                            element.Cs4Quantity = previousElement.TotalQuantity - element.Cs1Quantity - element.Cs2Quantity - element.Cs3Quantity;
                        }
                    }
                }
            }
        }

        public List<Element> DeteriorateElementsOverAYear(List<Element> baseElements, List<Element> previousElements, WisamType.ElementDeteriorationRates deteriorationRates)
        {
            List<Element> deterioratedElements = new List<Element>();

            // Deterioration rates are for element classifications (vs. element-specific)
            if (deteriorationRates == WisamType.ElementDeteriorationRates.ByElementClassification)
            {
                // Grab deterioration rates for all element classifications
                List<ElementClassificationDeterioration> ecDeteriorations = dbObj.GetElemClassificationsDeterioration();

                // Loop through elements and determine new quantities based on 1-year deterioration
                foreach (Element previousElement in previousElements)
                {
                    Element thisYearElement = new Element(previousElement, previousElement.DeteriorationYear + 1);
                    deterioratedElements.Add(thisYearElement);

                    if (thisYearElement.ElementClassificationCode.Equals(Code.Bearing))
                    {
                        // debug
                        int someInt = 0;
                    }

                    // Grab deterioration rates for the element based on its classification code; then deteriorate element
                    var currentDRList = ecDeteriorations.Where(e => e.ElementClassificationCode == thisYearElement.ElementClassificationCode).ToList();

                    if (currentDRList.Count() > 0)
                    {
                        var currentDR = currentDRList.First();
                        try
                        {
                            // Grab element's initial CS1 quantity
                            int baseCs1Quantity = baseElements.Where(e => e.ElemNum == thisYearElement.ElemNum).First().Cs1Quantity;

                            // New CS1 quantity
                            double newCs1Quantity = previousElement.Cs1Quantity - (baseCs1Quantity * (50 / currentDR.MedYr1) / 100);
                            thisYearElement.Cs1Quantity = Convert.ToInt32(Math.Round(newCs1Quantity, MidpointRounding.AwayFromZero));

                            // New CS2 quantity
                            double newCs2Quantity = previousElement.Cs2Quantity * (1 - ((50 / currentDR.MedYr2) / 100))
                                                        + previousElement.Cs1Quantity
                                                        - (previousElement.Cs1Quantity * (1 - ((50 / currentDR.MedYr1) / 100)));
                            thisYearElement.Cs2Quantity = Convert.ToInt32(Math.Round(newCs2Quantity, MidpointRounding.AwayFromZero));

                            // New CS3 quantity
                            double newCs3Quantity = previousElement.Cs3Quantity * (1 - ((50 / currentDR.MedYr3) / 100))
                                                        + previousElement.Cs2Quantity
                                                        - (previousElement.Cs2Quantity * (1 - ((50 / currentDR.MedYr2) / 100)));
                            thisYearElement.Cs3Quantity = Convert.ToInt32(Math.Round(newCs3Quantity, MidpointRounding.AwayFromZero));

                            // New CS4 quantity
                            thisYearElement.Cs4Quantity = previousElement.TotalQuantity - thisYearElement.Cs1Quantity - thisYearElement.Cs2Quantity - thisYearElement.Cs3Quantity;

                            if (thisYearElement.Cs4Quantity < 0)
                            {
                                thisYearElement.Cs4Quantity = 0;
                            }
                        }
                        catch { }
                    }
                }
            } // Deterioration rates are element-specific
            else if (deteriorationRates == WisamType.ElementDeteriorationRates.ByElement)
            {

            }

            return deterioratedElements;
        }

        public Inspection GetCurrentInspection(string strId)
        {
            return dbObj.GetCurrentInspection(strId);
        }

        public NbiRating DeteriorateNbiRatings(string strId)
        {
            NbiRating lastInspNbiRatings = dbObj.GetLastNbiRating(strId);
            NbiRating deterioratedNbiRatings = DeteriorateNbiRatingsOverAYear(lastInspNbiRatings);

            return deterioratedNbiRatings;
        }

        public void DeteriorateNbiRatingsOverAYear(NbiRating nbiRatings, NbiRating previousNbiRatings)
        {
            bool isCulvert = IsCulvert(previousNbiRatings.CulvertRating);

            if (!isCulvert)
            {
                // Deteriorate deck
                nbiRatings.DeckRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiDeck, previousNbiRatings.DeckDeteriorationYear + 1, previousNbiRatings.StructureId);
                nbiRatings.DeckRating = nbiRatings.DeckRatingVal.ToString();
                nbiRatings.DeckDeteriorationYear = previousNbiRatings.DeckDeteriorationYear + 1;

                // Deteriorate superstructure
                nbiRatings.SuperstructureRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiSuperstructure, previousNbiRatings.SuperstructureDeteriorationYear + 1, previousNbiRatings.StructureId);
                nbiRatings.SuperstructureRating = nbiRatings.SuperstructureRatingVal.ToString();
                nbiRatings.SuperstructureDeteriorationYear = previousNbiRatings.SuperstructureDeteriorationYear + 1;

                // Deteriorate substructure
                nbiRatings.SubstructureRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiSubstructure, previousNbiRatings.SubstructureDeteriorationYear + 1, previousNbiRatings.StructureId);
                nbiRatings.SubstructureRating = nbiRatings.SubstructureRatingVal.ToString();
                nbiRatings.SubstructureDeteriorationYear = previousNbiRatings.SubstructureDeteriorationYear + 1;
            }
            else
            {
                // Deteriorate culvert
                nbiRatings.CulvertRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiCulvert, previousNbiRatings.CulvertDeteriorationYear + 1, previousNbiRatings.StructureId);
                nbiRatings.CulvertRating = nbiRatings.CulvertRatingVal.ToString();
                nbiRatings.CulvertDeteriorationYear = previousNbiRatings.CulvertDeteriorationYear + 1;
            }
        }

        public NbiRating DeteriorateNbiRatingsOverAYear(NbiRating previousNbiRatings)
        {
            NbiRating deterioratedNbiRatings = new NbiRating(previousNbiRatings);
            bool isCulvert = IsCulvert(previousNbiRatings.CulvertRating);

            if (!isCulvert)
            {
                // Deteriorate deck
                deterioratedNbiRatings.DeckRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiDeck, previousNbiRatings.DeckDeteriorationYear + 1, previousNbiRatings.StructureId);
                if (deterioratedNbiRatings.DeckRatingVal < 0)
                {
                    deterioratedNbiRatings.DeckRatingVal = 0;
                }
                deterioratedNbiRatings.DeckRating = deterioratedNbiRatings.DeckRatingVal.ToString();
                deterioratedNbiRatings.DeckDeteriorationYear = previousNbiRatings.DeckDeteriorationYear + 1;

                // Deteriorate superstructure
                deterioratedNbiRatings.SuperstructureRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiSuperstructure, previousNbiRatings.SuperstructureDeteriorationYear + 1, previousNbiRatings.StructureId);
                if (deterioratedNbiRatings.SuperstructureRatingVal < 0)
                {
                    deterioratedNbiRatings.SuperstructureRatingVal = 0;
                }
                deterioratedNbiRatings.SuperstructureRating = deterioratedNbiRatings.SuperstructureRatingVal.ToString();
                deterioratedNbiRatings.SuperstructureDeteriorationYear = previousNbiRatings.SuperstructureDeteriorationYear + 1;

                // Deteriorate substructure
                deterioratedNbiRatings.SubstructureRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiSubstructure, previousNbiRatings.SubstructureDeteriorationYear + 1, previousNbiRatings.StructureId);
                if (deterioratedNbiRatings.SubstructureRatingVal < 0)
                {
                    deterioratedNbiRatings.SubstructureRatingVal = 0;
                }
                deterioratedNbiRatings.SubstructureRating = deterioratedNbiRatings.SubstructureRatingVal.ToString();
                deterioratedNbiRatings.SubstructureDeteriorationYear = previousNbiRatings.SubstructureDeteriorationYear + 1;
            }
            else
            {
                // Deteriorate culvert
                deterioratedNbiRatings.CulvertRatingVal = dbObj.GetNbiDeteriorationRating(Code.NbiCulvert, previousNbiRatings.CulvertDeteriorationYear + 1, previousNbiRatings.StructureId);
                if (deterioratedNbiRatings.CulvertRatingVal < 0)
                {
                    deterioratedNbiRatings.CulvertRatingVal = 0;
                }
                deterioratedNbiRatings.CulvertRating = deterioratedNbiRatings.CulvertRatingVal.ToString();
                deterioratedNbiRatings.CulvertDeteriorationYear = previousNbiRatings.CulvertDeteriorationYear + 1;
            }

            return deterioratedNbiRatings;
        }

        public NbiRating GetCurrentNbiRating(string strId)
        {
            NbiRating nr = null;
            nr = dbObj.GetLastNbiRating(strId);

            return nr;
        }

        public void CalculateNbiDeteriorationRates()
        {
            dbObj.CalculateNbiDeteriorationRates();
        }

        public void DeleteNbiDeteriorations()
        {
            dbObj.DeleteNbiDeteriorations();
        }
        #endregion Element, NBI and Inspection Methods


        #region Supporting Methods
        public void UpdateNbiQualifiedDeterioration(string qualifiedCode, string deteriorationFormula, string qualificationExpression)
        {
            dbObj.UpdateNbiQualifiedDeterioration(qualifiedCode, deteriorationFormula, qualificationExpression);
        }

        public void UpdateNbiDeterioration(WisamType.NbiRatingTypes nbiRatingType, string deteriorationFormula)
        {
            dbObj.UpdateNbiDeterioration(nbiRatingType, deteriorationFormula);
        }

        public List<NbiDeterioratedRating> RecalcNbiComponentDeterioration(string deteriorationFormula)
        {
            List<NbiDeterioratedRating> deterioratedRatings = new List<NbiDeterioratedRating>();

            for (int i = 0; i <= 100; i++)
            {
                string newExpression = deteriorationFormula.Replace("x", i.ToString());
                float rating = Convert.ToSingle(new DataTable().Compute(newExpression, null));

                if (rating < 0)
                {
                    rating = 0;
                }

                NbiDeterioratedRating ndr = new NbiDeterioratedRating();
                ndr.Year = i;
                ndr.RatingValue = rating;
                deterioratedRatings.Add(ndr);
            }

            return deterioratedRatings;
        }

        public string GetNbiDeteriorationFormula(WisamType.NbiRatingTypes nbiRatingType)
        {
            return dbObj.GetNbiDeteriorationFormula(nbiRatingType);
        }

        public string GetNbiQualificationExpression(string qualifiedCode)
        {
            return dbObj.GetQualificationExpression(qualifiedCode);
        }

        public string GetNbiQualifiedDeteriorationFormula(string qualifiedCode)
        {
            return dbObj.GetQualifiedDeteriorationFormula(qualifiedCode);
        }

        public List<NbiDeterioratedRating> GetNbiQualifiedDeterioratedRatings(string qualifiedCode)
        {
            return dbObj.GetNbiQualifiedDeterioratedRatings(qualifiedCode);
        }

        public List<NbiDeterioratedRating> GetNbiDeterioratedRatings(WisamType.NbiRatingTypes nbiRatingType)
        {
            return dbObj.GetNbiDeterioratedRatings(nbiRatingType);
        }

        public string GetRegionNbiRatingHistory(string region, string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath);
            string history = "";
            List<string> strIds = dbObj.GetStructuresByRegion(region);

            int i = 0;
            foreach (string strId in strIds)
            {
                //string lines = GetNbiRatingChangeHistory(strId);
                string lines = GetNbiRatingHistory(strId);

                if (!string.IsNullOrEmpty(lines))
                {
                    i++;
                    history += lines;
                    sw.Write(lines);
                }
            }

            sw.Close();
            return history;
        }

        public string GetRegionNbiRatingChangeHistory(string region, string filePath)
        {
            StreamWriter sw = new StreamWriter(filePath);
            string history = "";
            List<string> strIds = dbObj.GetStructuresByRegion(region);

            int i = 0;
            foreach (string strId in strIds)
            {
                string lines = GetNbiRatingChangeHistory(strId);

                if (!string.IsNullOrEmpty(lines))
                {
                    i++;
                    history += lines;
                    sw.Write(lines);
                }
            }

            sw.Close();
            return history;
        }

        public string GetNbiRatingHistory(string strId)
        {
            string history = "";
            List<DateTime> inspDates = dbObj.GetInspectionDatesAsc(strId);
            List<NbiRating> nbiRatings = new List<NbiRating>();

            foreach (DateTime inspDate in inspDates)
            {
                nbiRatings.Add(dbObj.GetNbiRating(strId, inspDate));
            }

            if (nbiRatings.Count > 0)
            {
                Structure str = dbObj.GetStructure(strId);

                if (str != null)
                {
                    string strData = String.Format("{0},{1}", str.StructureId, str.Region);
                    strData += String.Format(",{0}", str.Owner);
                    string funcClassOn = str.FunctionalClassificationOnCode.Length > 0 ? str.FunctionalClassificationOn : "NONE";
                    strData += String.Format(",{0},{1}", str.CorridorDesc, funcClassOn);
                    string funcClassUnder = str.FunctionalClassificationUnderCode.Length > 0 ? str.FunctionalClassificationUnder : "NONE";
                    strData += String.Format(",{0},{1}", str.Adt, funcClassUnder);
                    float minDeckWidth = str.DeckWidths.Count > 0 ? str.DeckWidths.Min() : -1;
                    strData += String.Format(",{0},{1}", str.AdtUnder, minDeckWidth);
                    strData += String.Format(",{0},{1},{2}", str.TotalLengthSpans, str.StructureType, str.MainSpanMaterial);

                    foreach (NbiRating nbiRating in nbiRatings)
                    {
                        history += String.Format("{0},{1},", nbiRating.InspectionDate.ToString("MM-dd-yyyy"), nbiRating.InspectionTypeCode.Length > 0 ? nbiRating.InspectionTypeDescription : "NONE");
                        history += String.Format("{0},{1},", nbiRating.DeckRatingVal, nbiRating.SuperstructureRatingVal);
                        history += String.Format("{0},{1},", nbiRating.SubstructureRatingVal, nbiRating.CulvertRatingVal);
                        history += String.Format("{0},", nbiRating.InspectionDate.Year - str.YearBuiltActual);
                        history += String.Format("{0}\r\n", strData);
                    }
                }
            }

            return history;
        }

        public string GetNbiRatingChangeHistory(string strId)
        {
            string history = "";
            List<DateTime> inspDates = dbObj.GetInspectionDatesAsc(strId);
            List<NbiRating> nbiRatings = new List<NbiRating>();

            foreach (DateTime inspDate in inspDates)
            {
                nbiRatings.Add(dbObj.GetNbiRating(strId, inspDate));
            }

            if (nbiRatings.Count > 0)
            {
                Structure str = dbObj.GetStructure(strId);

                if (str != null)
                {
                    string strData = String.Format("{0},{1}", DateTime.Now.Year - str.YearBuiltActual, str.Region);
                    string funcClassOn = str.FunctionalClassificationOnCode.Length > 0 ? str.FunctionalClassificationOn : "NONE";
                    strData += String.Format(",{0},{1}", str.CorridorDesc, funcClassOn);
                    string funcClassUnder = str.FunctionalClassificationUnderCode.Length > 0 ? str.FunctionalClassificationUnder : "NONE";
                    strData += String.Format(",{0},{1}", str.Adt, funcClassUnder);

                    if (nbiRatings[0].CulvertRating.ToUpper().Equals("N"))
                    {
                        int i = 1;
                        NbiRating oldR = new NbiRating();
                        NbiRating newR = new NbiRating();
                        bool hasDeckChanged = false;

                        foreach (NbiRating nbiRating in nbiRatings)
                        {
                            if (i == 1)
                            {
                                oldR = nbiRating;
                            }
                            else
                            {
                                newR = nbiRating;

                                if (newR.DeckRatingVal < oldR.DeckRatingVal)
                                {
                                    hasDeckChanged = true;
                                    history += string.Format("{0},", "DECK");
                                    history += string.Format("{0},{1},{2},", strId, oldR.DeckRatingVal, oldR.InspectionDate.ToString("MM-dd-yyyy"));
                                    history += string.Format("{0},{1},{2}", newR.DeckRatingVal, newR.InspectionDate.ToString("MM-dd-yyyy"), newR.InspectionDate.Year - oldR.InspectionDate.Year);
                                    oldR = newR;
                                    history += string.Format(",{0}\r\n", strData);
                                }
                            }

                            i++;
                        }

                        if (!hasDeckChanged)
                        {
                            history += string.Format("{0},", "DECK");
                            history += string.Format("{0},{1},{2},", strId, nbiRatings.First().DeckRatingVal, nbiRatings.First().InspectionDate.ToString("MM-dd-yyyy"));
                            history += string.Format("{0},{1},{2}", nbiRatings.Last().DeckRatingVal, nbiRatings.Last().InspectionDate.ToString("MM-dd-yyyy"), nbiRatings.Last().InspectionDate.Year - nbiRatings.First().InspectionDate.Year);
                            history += string.Format(",{0}\r\n", strData);
                        }

                        i = 1;
                        bool hasSuperChanged = false;
                        foreach (NbiRating nbiRating in nbiRatings)
                        {
                            if (i == 1)
                            {
                                oldR = nbiRating;
                            }
                            else
                            {
                                newR = nbiRating;

                                if (newR.SuperstructureRatingVal < oldR.SuperstructureRatingVal)
                                {
                                    hasSuperChanged = true;
                                    history += string.Format("{0},", "SUPER");
                                    history += string.Format("{0},{1},{2},", strId, oldR.SuperstructureRatingVal, oldR.InspectionDate.ToString("MM-dd-yyyy"));
                                    history += string.Format("{0},{1},{2}", newR.SuperstructureRatingVal, newR.InspectionDate.ToString("MM-dd-yyyy"), newR.InspectionDate.Year - oldR.InspectionDate.Year);
                                    oldR = newR;
                                    history += string.Format(",{0}\r\n", strData);
                                }
                            }

                            i++;
                        }

                        if (!hasSuperChanged)
                        {
                            history += string.Format("{0},", "SUPER");
                            history += string.Format("{0},{1},{2},", strId, nbiRatings.First().SuperstructureRatingVal, nbiRatings.First().InspectionDate.ToString("MM-dd-yyyy"));
                            history += string.Format("{0},{1},{2}", nbiRatings.Last().SuperstructureRatingVal, nbiRatings.Last().InspectionDate.ToString("MM-dd-yyyy"), nbiRatings.Last().InspectionDate.Year - nbiRatings.First().InspectionDate.Year);
                            history += string.Format(",{0}\r\n", strData);
                        }

                        i = 1;
                        bool hasSubChanged = false;
                        foreach (NbiRating nbiRating in nbiRatings)
                        {
                            if (i == 1)
                            {
                                oldR = nbiRating;
                            }
                            else
                            {
                                newR = nbiRating;

                                if (newR.SubstructureRatingVal < oldR.SubstructureRatingVal)
                                {
                                    hasSubChanged = true;
                                    history += string.Format("{0},", "SUB");
                                    history += string.Format("{0},{1},{2},", strId, oldR.SubstructureRatingVal, oldR.InspectionDate.ToString("MM-dd-yyyy"));
                                    history += string.Format("{0},{1},{2}", newR.SubstructureRatingVal, newR.InspectionDate.ToString("MM-dd-yyyy"), newR.InspectionDate.Year - oldR.InspectionDate.Year);
                                    oldR = newR;
                                    history += string.Format(",{0}\r\n", strData);
                                }
                            }

                            i++;
                        }

                        if (!hasSubChanged)
                        {
                            history += string.Format("{0},", "SUB");
                            history += string.Format("{0},{1},{2},", strId, nbiRatings.First().SubstructureRatingVal, nbiRatings.First().InspectionDate.ToString("MM-dd-yyyy"));
                            history += string.Format("{0},{1},{2}", nbiRatings.Last().SubstructureRatingVal, nbiRatings.Last().InspectionDate.ToString("MM-dd-yyyy"), nbiRatings.Last().InspectionDate.Year - nbiRatings.First().InspectionDate.Year);
                            history += string.Format(",{0}\r\n", strData);
                        }
                    }
                    else
                    {
                        int i = 1;
                        NbiRating oldR = new NbiRating();
                        NbiRating newR = new NbiRating();
                        bool hasCulvChanged = false;

                        foreach (NbiRating nbiRating in nbiRatings)
                        {
                            if (i == 1)
                            {
                                oldR = nbiRating;
                            }
                            else
                            {
                                newR = nbiRating;

                                if (newR.CulvertRatingVal < oldR.CulvertRatingVal)
                                {
                                    hasCulvChanged = true;
                                    history += string.Format("{0},", "CULV");
                                    history += string.Format("{0},{1},{2},", strId, oldR.CulvertRatingVal, oldR.InspectionDate.ToString("MM-dd-yyyy"));
                                    history += string.Format("{0},{1},{2}", newR.CulvertRatingVal, newR.InspectionDate.ToString("MM-dd-yyyy"), newR.InspectionDate.Year - oldR.InspectionDate.Year);
                                    oldR = newR;
                                    history += string.Format(",{0}\r\n", strData);
                                }
                            }

                            i++;
                        }

                        if (!hasCulvChanged)
                        {
                            history += string.Format("{0},", "CULV");
                            history += string.Format("{0},{1},{2},", strId, nbiRatings.First().CulvertRatingVal, nbiRatings.First().InspectionDate.ToString("MM-dd-yyyy"));
                            history += string.Format("{0},{1},{2}", nbiRatings.Last().CulvertRatingVal, nbiRatings.Last().InspectionDate.ToString("MM-dd-yyyy"), nbiRatings.Last().InspectionDate.Year - nbiRatings.First().InspectionDate.Year);
                            history += string.Format(",{0}\r\n", strData);
                        }
                    }
                }
            }

            return history;
        }

        public void CloseDb()
        {
            dbObj.CloseDbConnections();
        }

        public bool IsCulvert(string culvertRating)
        {
            return culvertRating.ToUpper().Equals("N") ? false : true;
        }

        public List<string> StructureIdsToList(string strIds)
        {
            return strIds.Split(new string[] { ",", ";", " " }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        #endregion Supporting Methods)
    }
}

/*
                if (ecDeteriorations.Count > 0)
                {
                    var elemsToDeteriorate = from e in deterioratedElements
                                             join c in ecDeteriorations
                                                on e.ElementClassificationCode equals c.ElementClassificationCode
                                             select new
                                             {
                                                 elemNum = e.ElemNum,
                                                 q1 = e.Cs1Quantity,
                                                 q2 = e.Cs2Quantity,
                                                 q3 = e.Cs3Quantity,
                                                 q4 = e.Cs4Quantity,
                                                 my1 = c.MedYr1,
                                                 my2 = c.MedYr2,
                                                 my3 = c.MedYr3
                                             };

                    foreach (var elemToDeteriorate in elemsToDeteriorate)
                    {

                    }
                }
                */

