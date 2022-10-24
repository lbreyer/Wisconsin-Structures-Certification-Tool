using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Infrastructure.Interfaces;

namespace Wisdot.Bos.WiSam.Core.Infrastructure
{
    public class ExcelHelperRepository : IExcelHelperRepository
    {
        public List<ElementDeterioration> GetElementDeteriorations(XLWorkbook workBook)
        {
            List<ElementDeterioration> elemDets = new List<ElementDeterioration>();
            var ws1 = workBook.Worksheet(1); // Grab first sheet
            var firstRowUsed = ws1.FirstRowUsed(); // Grab first row, which contains the column titles
            var columnTitlesRow = firstRowUsed.RowUsed();
            var dataRow = columnTitlesRow.RowBelow();

            while (!dataRow.Cell("A").IsEmpty())
            {
                ElementDeterioration ed = new ElementDeterioration();
                ed.ElemNum = Convert.ToInt32(dataRow.Cell("A").GetString());
                ed.RelativeWeight = Convert.ToSingle(dataRow.Cell("D").GetString());
                ed.Beta = Convert.ToSingle(dataRow.Cell("F").GetString());
                ed.MedYr1 = Convert.ToSingle(dataRow.Cell("G").GetString());
                ed.MedYr2 = Convert.ToSingle(dataRow.Cell("H").GetString());
                ed.MedYr3 = Convert.ToSingle(dataRow.Cell("I").GetString());
                ed.Active = true;
                elemDets.Add(ed);
                dataRow = dataRow.RowBelow();
            }

            return elemDets;
        }

        public string GetMaintenanceItemPriority(string workActionCode)
        {
            string priority = "";
            switch (workActionCode)
            {
                case "77":
                case "35":
                case "75":
                case "79":
                    priority = "Low";
                    break;
                case "04":
                case "10":
                case "12":
                case "14":
                case "29":
                case "49":
                case "66":
                case "72":
                case "94":
                    priority = "Medium";
                    break;
                case "28":
                    priority = "High";
                    break;
            }

            return priority;
        }

        public List<ProgrammedWorkAction> GetProgrammedWorkActionsFromExcel(XLWorkbook workBook)
        {
            List<ProgrammedWorkAction> progWorkActions = new List<ProgrammedWorkAction>();
            var ws1 = workBook.Worksheet(1); // Grab first sheet
            var firstRowUsed = ws1.FirstRowUsed(); // Grab first row, which contains the column titles
            var columnTitlesRow = firstRowUsed.RowUsed();
            var dr = columnTitlesRow.RowBelow();

            while (!dr.Cell("A").IsEmpty())
            {
                ProgrammedWorkAction pwa = new ProgrammedWorkAction();
                pwa.OriginalWorkActionCode = dr.Cell("N").GetString().Trim();
                pwa.StructureId = dr.Cell("P").GetString().Trim();
                pwa.NewStructureId = dr.Cell("Q").GetString().Trim();
                pwa.PProjId = Convert.ToInt32(dr.Cell("B").GetString().Trim());
                pwa.FosProjId = dr.Cell("A").GetString().Trim();

                if (pwa.PProjId.ToString().Length > 0 && pwa.FosProjId.Length > 0 && pwa.OriginalWorkActionCode.Length > 0 && (pwa.StructureId.Length > 0 || pwa.NewStructureId.Length > 0))
                {
                    pwa.SubProgramCode = dr.Cell("D").GetString().Trim();
                    pwa.FunctionalTypeCode = dr.Cell("E").GetString().Trim();
                    pwa.LifeCycleStageCode = dr.Cell("G").GetString().Trim();
                    pwa.OriginalWorkActionDesc = dr.Cell("O").GetString().Trim();
                    pwa.WorkActionCode = dr.Cell("T").GetString().Trim();

                    if (pwa.WorkActionCode.Length == 1)
                    {
                        pwa.WorkActionCode = "0" + pwa.WorkActionCode;
                    }

                    pwa.WorkActionDesc = dr.Cell("U").GetString().Trim();
                    pwa.EstimatedCompletionDate = Convert.ToDateTime(dr.Cell("J").GetString().Replace("00.00.00", "").Trim());
                    pwa.DotRegionCode = dr.Cell("V").GetString().Trim();

                    // is this pwa already in collection? pprojid, fosprojid,
                    // estimated completion date, structure id or new structure id
                    // work action code
                    var existingPwa = progWorkActions.Where(e => e.PProjId == pwa.PProjId
                                                                && e.FosProjId.Equals(pwa.FosProjId)
                                                                && e.EstimatedCompletionDate.Equals(pwa.EstimatedCompletionDate)
                                                                && (e.StructureId.Equals(pwa.StructureId) && e.NewStructureId.Equals(pwa.NewStructureId))
                                                                && e.WorkActionCode.Equals(pwa.WorkActionCode)
                                                        ).ToList();
                    if (existingPwa.Count == 0)
                    {
                        progWorkActions.Add(pwa);
                    }
                    else
                    {
                        string sId = pwa.StructureId;
                        string nId = pwa.NewStructureId;
                    }
                }

                dr = dr.RowBelow();
            }

            return progWorkActions;
        }

        public List<StructureLite> GetStructureCorridorCodes(XLWorkbook workBook)
        {
            List<StructureLite> strLites = new List<StructureLite>();
            var ws1 = workBook.Worksheet(1); // Grab first sheet
            var firstRowUsed = ws1.FirstRowUsed(); // Grab first row, which contains the column titles
            var columnTitlesRow = firstRowUsed.RowUsed();
            var dr = columnTitlesRow.RowBelow();

            while (!dr.Cell("A").IsEmpty())
            {
                StructureLite strLite = new StructureLite();
                strLite.StructureId = dr.Cell("A").GetString();
                strLite.CorridorCode = dr.Cell("B").GetString();
                strLites.Add(strLite);
                dr = dr.RowBelow();
            }

            return strLites;
        }

        public List<string> GetStructuresOnHighClearanceRoutes(XLWorkbook workBook)
        {
            List<string> strIds = new List<string>();
            var ws1 = workBook.Worksheet(1); // Grab first sheet
            var firstRowUsed = ws1.FirstRowUsed(); // Grab first row, which contains the column titles
            var columnTitlesRow = firstRowUsed.RowUsed();
            var dr = columnTitlesRow.RowBelow();

            while (!dr.Cell("A").IsEmpty())
            {
                strIds.Add(dr.Cell("C").GetString().Trim());
                dr = dr.RowBelow();
            }

            return strIds;
        }

        public void UpdateTimesheetDataFile(int monthOfWeekEndingDate, int yearOfWeekEndingDate, List<int> importedRows, XLWorkbook workBook)
        {
            try
            {
                //workBook = new XLWorkbook(sourceXlPath);
                //this.xlPath = sourceXlPath;
                var ws = workBook.Worksheet(yearOfWeekEndingDate + "-" + monthOfWeekEndingDate);

                foreach (var row in importedRows)
                {
                    ws.Row(row).Cell("K").Value = true;
                }

                /*
                workBook.Save();
                workBook.Dispose();
                */
            }
            catch { }
        }

        public void WriteAllCurrentNeedsReport(List<Structure> structures, List<string> notFoundIds, Database dbObj, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            ws1.Cell(1, 1).Value = "Structure ID";
            ws1.Cell(1, 2).Value = "Structure Type";
            ws1.Cell(1, 3).Value = "Date Last Insp";
            ws1.Cell(1, 4).Value = "CAI";
            ws1.Cell(1, 5).Value = "Criteria Met";
            ws1.Cell(1, 6).Value = "Source";
            ws1.Cell(1, 7).Value = "Date";
            ws1.Cell(1, 8).Value = "Item_Description";
            ws1.Cell(1, 9).Value = "Action_Priority";
            ws1.Cell(1, 10).Value = "Scheduled Year";
            ws1.Cell(1, 11).Value = "Estimated Amount";
            ws1.Cell(1, 12).Value = "Construction History";

            List<AllCurrentNeedsRow> needsRows = new List<AllCurrentNeedsRow>();
            int rowCounter = 2;

            foreach (var structure in structures)
            {
                foreach (var need in structure.AllCurrentNeeds)
                {
                    ws1.Cell(rowCounter, 1).Value = structure.StructureId;
                    ws1.Cell(rowCounter, 2).Value = structure.MainSpanMaterial + " " + structure.StructureType;
                    ws1.Cell(rowCounter, 3).Value = structure.LastInspection.InspectionDate.ToString("yyyy-MM-dd");
                    ws1.Cell(rowCounter, 4).Value = structure.CurrentCai != null ? Math.Round(structure.CurrentCai.CaiValue, 2).ToString() : "";
                    ws1.Cell(rowCounter, 5).Value = String.Format("({0}) {1}; Rule ID:{2}; Seq:{3}", need.WorkActionCode, need.WorkActionDesc, need.ControllingCriteria.RuleId, need.ControllingCriteria.RuleSequence);
                    ws1.Cell(rowCounter, 6).Value = "WiSAMS";
                    ws1.Cell(rowCounter, 7).Value = DateTime.Now.ToString("yyyy-MM-dd");
                    ws1.Cell(rowCounter, 8).Value = need.WorkActionDesc;
                    ws1.Cell(rowCounter, 9).Value = GetMaintenanceItemPriority(need.WorkActionCode);
                    ws1.Cell(rowCounter, 10).Value = "2021";
                    ws1.Cell(rowCounter, 11).Value = dbObj.GetStructureWorkActionCost(need.WorkActionCode, structure);
                    ws1.Cell(rowCounter, 12).Value = structure.ConstructionHistory;
                    rowCounter++;
                }
                /*
                AllCurrentNeedsRow needsRow = new AllCurrentNeedsRow();
                needsRow.StructureId = structure.StructureId;
                needsRow.StructureType = structure.MainSpanMaterial + " " + structure.StructureType;

                if (structure.CurrentCai != null)
                {
                    needsRow.LastInspectionYear = structure.CurrentCai.Year.ToString();
                    needsRow.CurrentCai = Math.Round(structure.CurrentCai.CaiValue, 2).ToString();
                }

                string needsInfo = "";
                int needsCounter = 1;

                foreach (var need in structure.AllCurrentNeeds)
                {
                    needsInfo += String.Format("{0}. ({1}) {2}\r\n", needsCounter, need.WorkActionCode, need.WorkActionDesc);
                    needsInfo += String.Format("\tRule ID:{0}, Seq:{1}\r\n\r\n", need.ControllingCriteria.RuleId, need.ControllingCriteria.RuleSequence);
                    needsCounter++;
                }

                needsRow.CurrentNeeds = needsInfo;
                needsRows.Add(needsRow);*/
            }

            ws1.Cell(2, 1).InsertData(needsRows.AsEnumerable());
        }

        public void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions, int firstYear, int lastYear, XLWorkbook workBook, List<string> planningProjectConcepts = null, List<string> federalImprovementTypes = null)
        {
            if (federalImprovementTypes == null)
                federalImprovementTypes = pwActions
                                    .Select(t => new { t.FederalImprovementTypeDesc, t.FederalImprovementTypeCode })
                                    .Distinct()
                                    .OrderBy(x => x.FederalImprovementTypeDesc)
                                    .Select(x => x.FederalImprovementTypeDesc)
                                    .ToList();

            //.Select(e => e.FederalImprovementTypeDesc).Distinct().OrderBy.ToList();
            if (planningProjectConcepts == null)
                planningProjectConcepts = pwActions
                                    .Select(t => new { t.PlanningProjectConceptDesc, t.PlanningProjectConceptCode })
                                    .Distinct()
                                    .OrderBy(x => x.PlanningProjectConceptDesc)
                                    .Select(x => x.PlanningProjectConceptDesc)
                                    .ToList();

            var ws1 = workBook.Worksheet("Sheet1");
            var ws2 = workBook.Worksheet("Sheet2");
            var ws3 = workBook.Worksheet("Sheet3");
            var ws4 = workBook.Worksheet("Sheet4");
            var ws5 = workBook.Worksheet("Sheet5");
            var ws6 = workBook.Worksheet("Sheet6");
            var ws7 = workBook.Worksheet("Sheet7");
            var ws8 = workBook.Worksheet("Sheet8");
            var ws9 = workBook.Worksheet("Sheet9");

            // Worksheet 1
            ws1.Name = String.Format("Projects-{0}to{1}", firstYear, lastYear);
            int row = 1;
            int col = 1;

            // Write federal improvement types as column headers
            foreach (var improvementType in federalImprovementTypes)
            {
                col++;
                ws1.Cell(row, col).Value = improvementType;
            }

            row++; // row 2
            col = 1;

            foreach (var projectConcept in planningProjectConcepts)
            {
                // Write current planning project concept as row header
                ws1.Cell(row, col).Value = projectConcept;

                // Write number of projects for current planning project concept and each federal improvement type
                foreach (var improvementType in federalImprovementTypes)
                {
                    col++;
                    var projects = pwActions
                                    .Where(e => e.PlanningProjectConceptDesc.Equals(projectConcept)
                                                    && e.FederalImprovementTypeDesc.Equals(improvementType));
                    int numberOfProjects = projects.Count();
                    ws1.Cell(row, col).Value = numberOfProjects;
                }

                row++;
                col = 1;
            }

            ws1.Columns().AdjustToContents(1, 10, 20);
            ws1.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws1.Columns().Style.Alignment.WrapText = true;
            // end Worksheet 1

            // Worksheet 2
            ws2.Name = String.Format("Costs-{0}to{1}", firstYear, lastYear);
            row = 1;
            col = 1;

            foreach (var improvementType in federalImprovementTypes)
            {
                col++;
                ws2.Cell(row, col).Value = String.Format("{0}-Projects", improvementType);
                col++;
                ws2.Cell(row, col).Value = String.Format("{0}-Cost w/ Del", improvementType);
                col++;
                ws2.Cell(row, col).Value = String.Format("{0}-Cost w/o Del", improvementType);
            }

            row++; // row 2
            col = 1;

            foreach (var projectConcept in planningProjectConcepts)
            {
                // Write current planning project concept as row header
                ws2.Cell(row, col).Value = projectConcept;

                foreach (var improvementType in federalImprovementTypes)
                {
                    col++;
                    var projects = pwActions
                                   .Where(e => e.PlanningProjectConceptDesc.Equals(projectConcept)
                                                   && e.FederalImprovementTypeDesc.Equals(improvementType));
                    int numberOfProjects = projects.Count();
                    ws2.Cell(row, col).Value = numberOfProjects;

                    col++;
                    float costWithDel = projects.Sum(e => e.WorkTotalWithDeliveryAmount);
                    ws2.Cell(row, col).Value = costWithDel;
                    ws2.Cell(row, col).Style.NumberFormat.Format = "$ #,##0";

                    col++;
                    float costWithoutDel = projects.Sum(e => e.WorkTotalWithoutDeliveryAmount);
                    ws2.Cell(row, col).Value = costWithoutDel;
                    ws2.Cell(row, col).Style.NumberFormat.Format = "$ #,##0";
                }

                row++;
                col = 1;
            }

            ws2.Columns().AdjustToContents(1, 10, 20);
            ws2.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws2.Columns().Style.Alignment.WrapText = true;

            // Worksheets by Year
            if (lastYear > firstYear)
            {
                int workSheetNum = 3;

                for (int year = firstYear; year <= lastYear; year++)
                {
                    workBook.Worksheet(workSheetNum).Name = year.ToString();
                    row = 1;
                    col = 1;

                    foreach (var improvementType in federalImprovementTypes)
                    {
                        col++;
                        workBook.Worksheet(workSheetNum).Cell(row, col).Value = String.Format("{0}-Projects", improvementType);
                        col++;
                        workBook.Worksheet(workSheetNum).Cell(row, col).Value = String.Format("{0}-Cost w/ Del", improvementType);
                        col++;
                        workBook.Worksheet(workSheetNum).Cell(row, col).Value = String.Format("{0}-Cost w/o Del", improvementType);
                    }

                    row++; // row 2
                    col = 1;

                    foreach (var projectConcept in planningProjectConcepts)
                    {
                        // Write current planning project concept as row header
                        workBook.Worksheet(workSheetNum).Cell(row, col).Value = projectConcept;

                        foreach (var improvementType in federalImprovementTypes)
                        {
                            col++;
                            var projects = pwActions
                                           .Where(e => e.PlanningProjectConceptDesc.Equals(projectConcept)
                                                           && e.FederalImprovementTypeDesc.Equals(improvementType)
                                                           && e.EstimatedCompletionDate.Year == year);
                            int numberOfProjects = projects.Count();
                            workBook.Worksheet(workSheetNum).Cell(row, col).Value = numberOfProjects;

                            col++;
                            float costWithDel = projects.Sum(e => e.WorkTotalWithDeliveryAmount);
                            workBook.Worksheet(workSheetNum).Cell(row, col).Value = costWithDel;
                            workBook.Worksheet(workSheetNum).Cell(row, col).Style.NumberFormat.Format = "$ #,##0";

                            col++;
                            float costWithoutDel = projects.Sum(e => e.WorkTotalWithoutDeliveryAmount);
                            workBook.Worksheet(workSheetNum).Cell(row, col).Value = costWithoutDel;
                            workBook.Worksheet(workSheetNum).Cell(row, col).Style.NumberFormat.Format = "$ #,##0";
                        }

                        row++;
                        col = 1;
                    }

                    workBook.Worksheet(workSheetNum).Columns().AdjustToContents(1, 10, 20);
                    workBook.Worksheet(workSheetNum).Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                    workBook.Worksheet(workSheetNum).Columns().Style.Alignment.WrapText = true;
                    workSheetNum++;
                }
            }
        }

        public void WriteAssetManagementReport(List<ProgrammedWorkAction> pwActions, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            var ws2 = workBook.Worksheet("Sheet2");
            var ws3 = workBook.Worksheet("Sheet3");
            var ws4 = workBook.Worksheet("Sheet4");

            var ws5 = workBook.Worksheet("Sheet5");
            var ws6 = workBook.Worksheet("Sheet6");
            var ws7 = workBook.Worksheet("Sheet7");
            var ws8 = workBook.Worksheet("Sheet8");
            var ws9 = workBook.Worksheet("Sheet9");
            var ws10 = workBook.Worksheet("Sheet10");
            var ws11 = workBook.Worksheet("Sheet11");

            ws1.Name = "Bridge Improvement Concepts";
            ws2.Name = "Misc Improvement Concepts";
            ws3.Name = "Number of Structures";
            ws4.Name = "Total Costs";

            int sheetCounter = 5;
            for (int i = DateTime.Today.Year; i <= DateTime.Today.Year + 5; i++)
            {
                workBook.Worksheet(sheetCounter).Name = i.ToString();
                sheetCounter++;
            }
            ws11.Name = ">= " + (DateTime.Today.Year + 6).ToString();

            List<string> ws3Columns = new List<string>()
                                        {
                                            "Impr Concept",
                                            "Bridge Protection (48)", "Bridge Rehabilitation (13)", "Bridge Rehababilitation - No Added Capacity (14)",
                                            "Bridge Replacement (10)", "Bridge Replacement - No Added Capacity (11)",
                                            "New Bridge Construction (08)", "Special Bridge (40)"
                                        };

            List<string> costColumns = new List<string>()
                                        {
                                            "Impr Concept",

                                            "Bridge Protection (48)-Distinct Str IDs", "Bridge Protection (48) Cost w/ Del", "Bridge Protection (48) Cost w/o Del",
                                            "Bridge Protection (48)-Blank Str IDs", "Bridge Protection (48) Cost w/ Del-Blank Str IDs", "Bridge Protection (48) Cost w/o Del-Blank Str IDs",

                                            "Bridge Rehabilitation (13)-Distinct Str IDs", "Bridge Rehabilitation (13) Cost w/ Del", "Bridge Rehabilitation (13) Cost w/o Del",
                                            "Bridge Rehabilitation (13)-Blank Str IDs", "Bridge Rehabilitation (13) Cost w/ Del-Blank Str IDs", "Bridge Rehabilitation (13) Cost w/o Del-Blank Str IDs",

                                            "Bridge Rehababilitation-No Added Capacity (14)-Distinct Str IDs", "Bridge Rehababilitation-No Added Capacity (14) Cost w/ Del", "Bridge Rehababilitation-No Added Capacity (14) Cost w/o Del",
                                            "Bridge Rehababilitation-No Added Capacity (14)-Blank Str IDs", "Bridge Rehababilitation-No Added Capacity (14) Cost w/ Del-Blank Str IDs", "Bridge Rehababilitation-No Added Capacity (14) Cost w/o Del-Blank Str IDs",

                                            "Bridge Replacement (10)-Distinct Str IDs", "Bridge Replacement (10) Cost w/ Del", "Bridge Replacement (10) Cost w/o Del",
                                            "Bridge Replacement (10)-Blank Str IDs", "Bridge Replacement (10) Cost w/ Del-Blank Str IDs", "Bridge Replacement (10) Cost w/o Del-Blank Str IDs",

                                            "Bridge Replacement-No Added Capacity (11)-Distinct Str IDs", "Bridge Replacement-No Added Capacity (11) Cost w/ Del", "Bridge Replacement-No Added Capacity (11) Cost w/o Del",
                                            "Bridge Replacement-No Added Capacity (11)-Blank Str IDs", "Bridge Replacement-No Added Capacity (11) Cost w/ Del-Blank Str IDs", "Bridge Replacement-No Added Capacity (11) Cost w/o Del-Blank Str IDs",

                                            "New Bridge Construction (08)-Distinct Str IDs", "New Bridge Construction (08) Cost w/ Del", "New Bridge Construction (08) Cost w/o Del",
                                            "New Bridge Construction (08)-Blank Str IDs", "New Bridge Construction (08) Cost w/ Del-Blank Str IDs", "New Bridge Construction (08) Cost w/o Del-Blank Str IDs",

                                            "Special Bridge (40)-Distinct Str IDs", "Special Bridge (40) Cost w/ Del", "Special Bridge (40) Cost w/o Del",
                                            "Special Bridge (40)-Blank Str IDs", "Special Bridge (40) Cost w/ Del-Blank Str IDs", "Special Bridge (40) Cost w/o Del-Blank Str IDs"
                                        };

            string[] imprConceptsDescs = new string[] {
                                                        "BRIDGE ELIMINATION",
                                                        "BRIDGE REHABILITATION",
                                                        "BRIDGE REPLACEMENT, PRESERVATION",
                                                        "BRIDGE REPLACEMENT, EXPANSION",
                                                        "BRIDGE REHABILITATION (SHRM)",
                                                        "MISCELLANEOUS",
                                                        "PAVEMENT REPLACEMENT",
                                                        "ROADWAY MAINT, PRESERVATION",
                                                        "RECONDITIONING",
                                                        "RECONSTRUCTION, PRESERVATION",
                                                        "RECONSTRUCTION, EXPANSION",
                                                        "RESURFACING"
                                                        };

            List<string> imprConcepts = new List<string>()
                                            {
                                                "BRELIM","BRRHB","BRRPL","BRRPLE","BRSHRM","MISC","PVRPLA","RDMTN","RECOND","RECST","RECSTE","RESURF"
                                            };

            List<string> fedImprs = new List<string>()
                                        {
                                            "48","13","14","10","11","08","40"
                                        };

            int descCtr = 0;
            List<CountsRow> countsRows = new List<CountsRow>();
            List<CountsCostsRow> countsCostsRows = new List<CountsCostsRow>();


            foreach (var imprConcept in imprConcepts)
            {
                CountsRow countsRow = new CountsRow();
                CountsCostsRow countsCostsRow = new CountsCostsRow();
                countsRow.ImprConcept = imprConceptsDescs[descCtr];
                countsCostsRow.ImprConcept = imprConceptsDescs[descCtr];
                int colCtr = 0;
                //ws3Rows[rowCtr, colCtr] = imprConceptsDescs[descCtr];

                foreach (var fedImpr in fedImprs)
                {
                    colCtr++;

                    var strs = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && !String.IsNullOrEmpty(e.StructureId))
                                .GroupBy(e => e.StructureId)
                                .Select(g => new { Str = g.First().StructureId })
                                .OrderBy(g => g.Str);

                    if (fedImpr.Equals("08"))
                    {
                        strs = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId)))
                                .GroupBy(e => new { Str = e.StructureId, NewStr = e.NewStructureId })
                                .Select(g => new { Str = g.First().StructureId })
                                .OrderBy(g => g.Str);
                    }

                    var blanks = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && String.IsNullOrEmpty(e.StructureId))
                                .Select(g => new { CostDel = g.WorkTotalWithDeliveryAmount, CostNoDel = g.WorkTotalWithoutDeliveryAmount });

                    if (fedImpr.Equals("08"))
                    {
                        blanks = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && String.IsNullOrEmpty(e.StructureId)
                                                && String.IsNullOrEmpty(e.NewStructureId))
                                .Select(g => new { CostDel = g.WorkTotalWithDeliveryAmount, CostNoDel = g.WorkTotalWithoutDeliveryAmount });
                    }

                    var countStrs = strs.Count();
                    var countBlanks = blanks.Count();

                    var totalWithDelStrs = pwActions
                                        .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                        && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                        && !String.IsNullOrEmpty(e.StructureId))
                                        .Select(e => e.WorkTotalWithDeliveryAmount).Sum();

                    if (fedImpr.Equals("08"))
                    {
                        totalWithDelStrs = pwActions
                                        .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                        && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                        && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId)))
                                        .Select(e => e.WorkTotalWithDeliveryAmount).Sum();
                    }

                    var totalWithDelBlanks = blanks.Select(e => e.CostDel).Sum();

                    var totalWithoutDelStrs = pwActions
                                            .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                            && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                            && !String.IsNullOrEmpty(e.StructureId))
                                            .Select(e => e.WorkTotalWithoutDeliveryAmount).Sum();

                    if (fedImpr.Equals("08"))
                    {
                        totalWithoutDelStrs = pwActions
                                            .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                            && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                            && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId)))
                                            .Select(e => e.WorkTotalWithoutDeliveryAmount).Sum();
                    }

                    var totalWithoutDelBlanks = blanks.Select(e => e.CostNoDel).Sum();

                    switch (colCtr)
                    {
                        case 1:
                            countsRow.Count1 = countStrs;
                            countsCostsRow.Count1 = countStrs;
                            countsCostsRow.Cost1a = totalWithDelStrs;
                            countsCostsRow.Cost1b = totalWithoutDelStrs;
                            countsCostsRow.Count1Null = countBlanks;
                            countsCostsRow.Cost1Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost1Nullb = totalWithoutDelBlanks;
                            break;
                        case 2:
                            countsRow.Count2 = countStrs;
                            countsCostsRow.Count2 = countStrs;
                            countsCostsRow.Cost2a = totalWithDelStrs;
                            countsCostsRow.Cost2b = totalWithoutDelStrs;
                            countsCostsRow.Count2Null = countBlanks;
                            countsCostsRow.Cost2Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost2Nullb = totalWithoutDelBlanks;
                            break;
                        case 3:
                            countsRow.Count3 = countStrs;
                            countsCostsRow.Count3 = countStrs;
                            countsCostsRow.Cost3a = totalWithDelStrs;
                            countsCostsRow.Cost3b = totalWithoutDelStrs;
                            countsCostsRow.Count3Null = countBlanks;
                            countsCostsRow.Cost3Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost3Nullb = totalWithoutDelBlanks;
                            break;
                        case 4:
                            countsRow.Count4 = countStrs;
                            countsCostsRow.Count4 = countStrs;
                            countsCostsRow.Cost4a = totalWithDelStrs;
                            countsCostsRow.Cost4b = totalWithoutDelStrs;
                            countsCostsRow.Count4Null = countBlanks;
                            countsCostsRow.Cost4Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost4Nullb = totalWithoutDelBlanks;
                            break;
                        case 5:
                            countsRow.Count5 = countStrs;
                            countsCostsRow.Count5 = countStrs;
                            countsCostsRow.Cost5a = totalWithDelStrs;
                            countsCostsRow.Cost5b = totalWithoutDelStrs;
                            countsCostsRow.Count5Null = countBlanks;
                            countsCostsRow.Cost5Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost5Nullb = totalWithoutDelBlanks;
                            break;
                        case 6:
                            countsRow.Count6 = countStrs;
                            countsCostsRow.Count6 = countStrs;
                            countsCostsRow.Cost6a = totalWithDelStrs;
                            countsCostsRow.Cost6b = totalWithoutDelStrs;
                            countsCostsRow.Count6Null = countBlanks;
                            countsCostsRow.Cost6Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost6Nullb = totalWithoutDelBlanks;
                            break;
                        case 7:
                            countsRow.Count7 = countStrs;
                            countsCostsRow.Count7 = countStrs;
                            countsCostsRow.Cost7a = totalWithDelStrs;
                            countsCostsRow.Cost7b = totalWithoutDelStrs;
                            countsCostsRow.Count7Null = countBlanks;
                            countsCostsRow.Cost7Nulla = totalWithDelBlanks;
                            countsCostsRow.Cost7Nullb = totalWithoutDelBlanks;
                            break;
                    }
                }

                //rowCtr++;
                descCtr++;
                countsRows.Add(countsRow);
                countsCostsRows.Add(countsCostsRow);
            }

            int colCounter = 1;
            foreach (var ws3Column in ws3Columns)
            {
                ws3.Cell(1, colCounter).Value = ws3Column;
                colCounter++;
            }
            ws3.Cell(2, 1).InsertData(countsRows.AsEnumerable());

            colCounter = 1;
            foreach (var costColumn in costColumns)
            {
                ws4.Cell(1, colCounter).Value = costColumn;
                colCounter++;
            }
            ws4.Cell(2, 1).InsertData(countsCostsRows.AsEnumerable());

            List<string> ws1Columns = new List<string>()
                                            { "Fos Proj ID", "Plan Project ID", "Impr Concept Code", "Impr Concept Desc",
                                                "Fed Impr Type", "Fed Impr Desc", "Str Id", "New Str Id",
                                                "Est Comp Date", "Proj Status", "Tot Cost - Del", "Tot Cost - No Del",
                                                "Fund Src", "Fund Cat Number", "Fund Amt - Del", "Fund Amt - No Del", "Fund Cat Desc",
                                                "Work Action Code", "Work Action Desc"
                                            };

            colCounter = 1;
            foreach (var ws1Column in ws1Columns)
            {
                ws1.Cell(1, colCounter).Value = ws1Column;
                ws2.Cell(1, colCounter).Value = ws1Column;
                colCounter++;
            }

            List<BridgeProjectRow> bpRows1 = new List<BridgeProjectRow>();
            List<BridgeProjectRow> bpRows2 = new List<BridgeProjectRow>();
            // List<BridgeProjectRow> 
            /*
              public string FosProjId { get; set; }
        public string PProjId { get; set; }
        public string PlanningProjectConceptCode { get; set; }
        public string PlanningProjectConceptDesc { get; set; }
        public string FederalImprovementTypeCode { get; set; }
        public string FederalImprovementTypeDesc { get; set; }
        public string StructureId { get; set; }
        public string NewStructureId { get; set; }
        pwAction.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTP_SCHD_DT"]);
                    pwAction.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString();
                    pwAction.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                    pwAction.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                    pwAction.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["FAWD"]);
                    pwAction.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["FA"]);
                    pwAction.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString();
            */

            foreach (var pwAction in pwActions)
            {
                BridgeProjectRow bpRow = new BridgeProjectRow();
                bpRow.FosProjId = pwAction.FosProjId;
                bpRow.PProjId = pwAction.PProjId.ToString();
                bpRow.PlanningProjectConceptCode = pwAction.PlanningProjectConceptCode;
                bpRow.PlanningProjectConceptDesc = pwAction.PlanningProjectConceptDesc;
                bpRow.FederalImprovementTypeCode = pwAction.FederalImprovementTypeCode;
                bpRow.FederalImprovementTypeDesc = pwAction.FederalImprovementTypeDesc;
                bpRow.StructureId = pwAction.StructureId;
                bpRow.NewStructureId = pwAction.NewStructureId;

                bpRow.EstimatedCompletionDate = pwAction.EstimatedCompletionDate.ToString();
                bpRow.ProjectStatusFlag = pwAction.ProjectStatusFlag;
                bpRow.ProjectTotalWithDeliveryAmount = pwAction.ProjectTotalWithDeliveryAmount.ToString();
                bpRow.ProjectTotalWithoutDeliveryAmount = pwAction.ProjectTotalWithoutDeliveryAmount.ToString();

                bpRow.FundingSourceTypeCode = pwAction.FundingSourceTypeCode;
                bpRow.FundingCategoryNumber = pwAction.FundingCategoryNumber;

                bpRow.WorkTotalWithDeliveryAmount = pwAction.WorkTotalWithDeliveryAmount.ToString();
                bpRow.WorkTotalWithoutDeliveryAmount = pwAction.WorkTotalWithoutDeliveryAmount.ToString();
                bpRow.FundingCategoryDesc = pwAction.FundingCategoryDesc;

                bpRow.OriginalWorkActionCode = pwAction.OriginalWorkActionCode;
                bpRow.OriginalWorkActionDesc = pwAction.OriginalWorkActionDesc;

                switch (pwAction.PlanningProjectConceptCode.ToUpper().Trim())
                {
                    case "BRELIM":
                    case "BRRHB":
                    case "BRRPL":
                    case "BRRPLE":
                    case "BRSHRM":
                        bpRows1.Add(bpRow);
                        break;

                    default:
                        bpRows2.Add(bpRow);
                        break;
                }
            }

            ws1.Cell(2, 1).InsertData(bpRows1.AsEnumerable());
            ws2.Cell(2, 1).InsertData(bpRows2.AsEnumerable());

            List<CountsCostsRow>[] costsByYear = new List<CountsCostsRow>[7];
            for (int costsByYearCtr = 0; costsByYearCtr <= 6; costsByYearCtr++)
            {
                costsByYear[costsByYearCtr] = new List<CountsCostsRow>();
                descCtr = 0;

                foreach (var imprConcept in imprConcepts)
                {
                    // create new row
                    CountsCostsRow ccyRow = new CountsCostsRow();
                    ccyRow.ImprConcept = imprConceptsDescs[descCtr];
                    int colCtr = 0;

                    foreach (var fedImpr in fedImprs)
                    {
                        // Populate row
                        colCtr++;

                        var strs = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && !String.IsNullOrEmpty(e.StructureId)
                                                && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                .GroupBy(e => e.StructureId)
                                .Select(g => new { Str = g.First().StructureId })
                                .OrderBy(g => g.Str);

                        if (fedImpr.Equals("08"))
                        {
                            strs = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId))
                                                && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                .GroupBy(e => new { Str = e.StructureId, NewStr = e.NewStructureId })
                                .Select(g => new { Str = g.First().StructureId })
                                .OrderBy(g => g.Str);
                        }

                        if (costsByYearCtr == 6)
                        {
                            strs = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && !String.IsNullOrEmpty(e.StructureId)
                                                && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                .GroupBy(e => e.StructureId)
                                .Select(g => new { Str = g.First().StructureId })
                                .OrderBy(g => g.Str);

                            if (fedImpr.Equals("08"))
                            {
                                strs = pwActions
                               .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                               && e.FederalImprovementTypeCode.Equals(fedImpr)
                                               && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId))
                                               && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                               .GroupBy(e => new { Str = e.StructureId, NewStr = e.NewStructureId })
                               .Select(g => new { Str = g.First().StructureId })
                               .OrderBy(g => g.Str);
                            }
                        }

                        var blanks = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && String.IsNullOrEmpty(e.StructureId)
                                                && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                .Select(g => new { CostDel = g.WorkTotalWithDeliveryAmount, CostNoDel = g.WorkTotalWithoutDeliveryAmount });

                        if (fedImpr.Equals("08"))
                        {
                            blanks = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && String.IsNullOrEmpty(e.StructureId)
                                                && String.IsNullOrEmpty(e.NewStructureId)
                                                && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                .Select(g => new { CostDel = g.WorkTotalWithDeliveryAmount, CostNoDel = g.WorkTotalWithoutDeliveryAmount });
                        }

                        if (costsByYearCtr == 6)
                        {
                            blanks = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && String.IsNullOrEmpty(e.StructureId)
                                                && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                .Select(g => new { CostDel = g.WorkTotalWithDeliveryAmount, CostNoDel = g.WorkTotalWithoutDeliveryAmount });

                            if (fedImpr.Equals("08"))
                            {
                                blanks = pwActions
                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                && String.IsNullOrEmpty(e.StructureId)
                                                && String.IsNullOrEmpty(e.NewStructureId)
                                                && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                .Select(g => new { CostDel = g.WorkTotalWithDeliveryAmount, CostNoDel = g.WorkTotalWithoutDeliveryAmount });
                            }
                        }

                        var countStrs = strs.Count();
                        var countBlanks = blanks.Count();

                        var totalWithDelStrs = pwActions
                                            .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                            && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                            && !String.IsNullOrEmpty(e.StructureId)
                                                            && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                            .Select(e => e.WorkTotalWithDeliveryAmount).Sum();

                        if (fedImpr.Equals("08"))
                        {
                            totalWithDelStrs = pwActions
                                            .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                            && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                            && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId))
                                                            && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                            .Select(e => e.WorkTotalWithDeliveryAmount).Sum();
                        }

                        if (costsByYearCtr == 6)
                        {
                            totalWithDelStrs = pwActions
                                            .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                            && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                            && !String.IsNullOrEmpty(e.StructureId)
                                                            && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                            .Select(e => e.WorkTotalWithDeliveryAmount).Sum();

                            if (fedImpr.Equals("08"))
                            {
                                totalWithDelStrs = pwActions
                                            .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                            && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                            && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId))
                                                            && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                            .Select(e => e.WorkTotalWithDeliveryAmount).Sum();
                            }
                        }

                        var totalWithDelBlanks = blanks.Select(e => e.CostDel).Sum();

                        var totalWithoutDelStrs = pwActions
                                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                                && !String.IsNullOrEmpty(e.StructureId)
                                                                && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                                .Select(e => e.WorkTotalWithoutDeliveryAmount).Sum();

                        if (fedImpr.Equals("08"))
                        {
                            totalWithoutDelStrs = pwActions
                                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                                && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId))
                                                                && e.EstimatedCompletionDate.Year.Equals(DateTime.Today.Year + costsByYearCtr))
                                                .Select(e => e.WorkTotalWithoutDeliveryAmount).Sum();
                        }

                        if (costsByYearCtr == 6)
                        {
                            totalWithoutDelStrs = pwActions
                                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                                && !String.IsNullOrEmpty(e.StructureId)
                                                                && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                                .Select(e => e.WorkTotalWithoutDeliveryAmount).Sum();

                            if (fedImpr.Equals("08"))
                            {
                                totalWithoutDelStrs = pwActions
                                                .Where(e => e.PlanningProjectConceptCode.Equals(imprConcept)
                                                                && e.FederalImprovementTypeCode.Equals(fedImpr)
                                                                && (!String.IsNullOrEmpty(e.StructureId) || !String.IsNullOrEmpty(e.NewStructureId))
                                                                && e.EstimatedCompletionDate.Year >= (DateTime.Today.Year + costsByYearCtr))
                                                .Select(e => e.WorkTotalWithoutDeliveryAmount).Sum();
                            }
                        }

                        var totalWithoutDelBlanks = blanks.Select(e => e.CostNoDel).Sum();

                        switch (colCtr)
                        {
                            case 1:
                                ccyRow.Count1 = countStrs;
                                ccyRow.Cost1a = totalWithDelStrs;
                                ccyRow.Cost1b = totalWithoutDelStrs;
                                ccyRow.Count1Null = countBlanks;
                                ccyRow.Cost1Nulla = totalWithDelBlanks;
                                ccyRow.Cost1Nullb = totalWithoutDelBlanks;
                                break;
                            case 2:
                                ccyRow.Count2 = countStrs;
                                ccyRow.Cost2a = totalWithDelStrs;
                                ccyRow.Cost2b = totalWithoutDelStrs;
                                ccyRow.Count2Null = countBlanks;
                                ccyRow.Cost2Nulla = totalWithDelBlanks;
                                ccyRow.Cost2Nullb = totalWithoutDelBlanks;
                                break;
                            case 3:
                                ccyRow.Count3 = countStrs;
                                ccyRow.Cost3a = totalWithDelStrs;
                                ccyRow.Cost3b = totalWithoutDelStrs;
                                ccyRow.Count3Null = countBlanks;
                                ccyRow.Cost3Nulla = totalWithDelBlanks;
                                ccyRow.Cost3Nullb = totalWithoutDelBlanks;
                                break;
                            case 4:
                                ccyRow.Count4 = countStrs;
                                ccyRow.Cost4a = totalWithDelStrs;
                                ccyRow.Cost4b = totalWithoutDelStrs;
                                ccyRow.Count4Null = countBlanks;
                                ccyRow.Cost4Nulla = totalWithDelBlanks;
                                ccyRow.Cost4Nullb = totalWithoutDelBlanks;
                                break;
                            case 5:
                                ccyRow.Count5 = countStrs;
                                ccyRow.Cost5a = totalWithDelStrs;
                                ccyRow.Cost5b = totalWithoutDelStrs;
                                ccyRow.Count5Null = countBlanks;
                                ccyRow.Cost5Nulla = totalWithDelBlanks;
                                ccyRow.Cost5Nullb = totalWithoutDelBlanks;
                                break;
                            case 6:
                                ccyRow.Count6 = countStrs;
                                ccyRow.Cost6a = totalWithDelStrs;
                                ccyRow.Cost6b = totalWithoutDelStrs;
                                ccyRow.Count6Null = countBlanks;
                                ccyRow.Cost6Nulla = totalWithDelBlanks;
                                ccyRow.Cost6Nullb = totalWithoutDelBlanks;
                                break;
                            case 7:
                                ccyRow.Count7 = countStrs;
                                ccyRow.Cost7a = totalWithDelStrs;
                                ccyRow.Cost7b = totalWithoutDelStrs;
                                ccyRow.Count7Null = countBlanks;
                                ccyRow.Cost7Nulla = totalWithDelBlanks;
                                ccyRow.Cost7Nullb = totalWithoutDelBlanks;
                                break;
                        }
                    }

                    // add row to collection
                    costsByYear[costsByYearCtr].Add(ccyRow);
                    descCtr++;
                }

                // populate worksheet
                colCounter = 1;
                foreach (var costColumn in costColumns)
                {
                    workBook.Worksheet(5 + costsByYearCtr).Cell(1, colCounter).Value = costColumn;
                    colCounter++;
                }
                workBook.Worksheet(5 + costsByYearCtr).Cell(2, 1).InsertData(costsByYear[costsByYearCtr].AsEnumerable());
            }

        }

        public void WriteBidItemsReport(List<BidItem> bidItems, XLWorkbook workBook)
        {
            /*
            var groupedCustomerList = userList
    .GroupBy(u => u.GroupID)
    .Select(grp => grp.ToList())
    .ToList();*/

            //var grouped
            var ws1 = workBook.Worksheet("Sheet1");
            var ws2 = workBook.Worksheet("Sheet2");
            ws1.Name = "Fabricated Bid Items";
            ws2.Name = "Sortable";
            ws1.Cell(1, 1).Value = "Let Date";
            ws1.Cell(1, 2).Value = "Project ID";
            ws1.Cell(1, 3).Value = "Project Description";
            ws1.Cell(1, 4).Value = "Project Status";
            ws1.Cell(1, 5).Value = "PM / Item No";
            ws1.Cell(1, 6).Value = "Contractor / Item Description";
            ws1.Cell(1, 7).Value = "FIIPS / Quantity x Unit Price = Cost";
            //ws1.Cell(1, 7).Value = "Unit Price";
            //ws1.Cell(1, 8).Value = "Cost";
            //ws1.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws1.Row(1).Style.Font.Bold = true;

            DateTime previousLetDate = new DateTime(1, 1, 1);
            string previousProjectId = "";
            int rowCounter = 2;

            foreach (var bidItem in bidItems)
            {
                DateTime currentLetDate = bidItem.LetDate;
                string currentProjectId = bidItem.FosProjectId;

                if (DateTime.Compare(previousLetDate, currentLetDate) != 0 || !currentProjectId.Equals(previousProjectId))
                {
                    ws1.Cell(rowCounter, 1).Value = String.Format("{0:MM/dd/yyyy}", bidItem.LetDate);
                    ws1.Cell(rowCounter, 2).Value = bidItem.FosProjectId;
                    ws1.Cell(rowCounter, 3).Value = bidItem.ProjectDescription;
                    ws1.Cell(rowCounter, 4).Value = bidItem.ProjectStatus;
                    ws1.Cell(rowCounter, 5).Value = bidItem.ProjectManager;
                    ws1.Cell(rowCounter, 6).Value = bidItem.AwardedVendorName;
                    ws1.Cell(rowCounter, 7).Value = bidItem.ExistingStructureIds;
                    ws1.Row(rowCounter).Style.Fill.BackgroundColor = XLColor.Almond;
                    rowCounter++;
                    ws1.Cell(rowCounter, 1).Value = "";
                    ws1.Cell(rowCounter, 2).Value = "";
                    ws1.Cell(rowCounter, 3).Value = "";
                    ws1.Cell(rowCounter, 4).Value = "";
                    ws1.Cell(rowCounter, 5).Value = bidItem.BidItemName;
                    ws1.Cell(rowCounter, 6).Value = bidItem.BidItemDescription;
                    ws1.Cell(rowCounter, 7).Value = String.Format("{0}x{1}={2}", bidItem.BidItemQuantity, bidItem.BidItemUnitPrice, bidItem.BidItemCost);
                }
                else
                {
                    ws1.Cell(rowCounter, 1).Value = "";
                    ws1.Cell(rowCounter, 2).Value = "";
                    ws1.Cell(rowCounter, 3).Value = "";
                    ws1.Cell(rowCounter, 4).Value = "";
                    ws1.Cell(rowCounter, 5).Value = bidItem.BidItemName;
                    ws1.Cell(rowCounter, 6).Value = bidItem.BidItemDescription;
                    ws1.Cell(rowCounter, 7).Value = String.Format("{0}x{1}={2}", bidItem.BidItemQuantity, bidItem.BidItemUnitPrice, bidItem.BidItemCost);
                }

                previousLetDate = currentLetDate;
                previousProjectId = currentProjectId;
                rowCounter++;
            }

            ws1.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws1.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws1.Columns("A-B").AdjustToContents();
            ws1.Columns("C").Style.Alignment.WrapText = true;
            ws1.Columns("C").Width = 16;
            //ws1.Columns("D").AdjustToContents();
            ws1.Columns("D-G").AdjustToContents();

            ws2.Cell(1, 1).Value = "Let Date";
            ws2.Cell(1, 2).Value = "Project ID";
            ws2.Cell(1, 3).Value = "Project Description";
            ws2.Cell(1, 4).Value = "Project Status";
            ws2.Cell(1, 5).Value = "PM";
            ws2.Cell(1, 6).Value = "Contractor";
            ws2.Cell(1, 7).Value = "FIIPS";
            ws2.Cell(1, 8).Value = "Item No";
            ws2.Cell(1, 9).Value = "Item Description";
            ws2.Cell(1, 10).Value = "Quantity";
            ws2.Cell(1, 11).Value = "Unit Price";
            ws2.Cell(1, 12).Value = "Cost";
            ws2.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws2.Row(1).Style.Font.Bold = true;

            rowCounter = 2;
            foreach (var bidItem in bidItems)
            {

                ws2.Cell(rowCounter, 1).Value = String.Format("{0:MM/dd/yyyy}", bidItem.LetDate);
                ws2.Cell(rowCounter, 2).Value = bidItem.FosProjectId;
                ws2.Cell(rowCounter, 3).Value = bidItem.ProjectDescription;
                ws2.Cell(rowCounter, 4).Value = bidItem.ProjectStatus;
                ws2.Cell(rowCounter, 5).Value = bidItem.ProjectManager;
                ws2.Cell(rowCounter, 6).Value = bidItem.AwardedVendorName;
                ws2.Cell(rowCounter, 7).Value = bidItem.ExistingStructureIds;

                ws2.Cell(rowCounter, 8).Value = bidItem.BidItemName;
                ws2.Cell(rowCounter, 9).Value = bidItem.BidItemDescription;
                ws2.Cell(rowCounter, 10).Value = bidItem.BidItemQuantity;
                ws2.Cell(rowCounter, 11).Value = bidItem.BidItemUnitPrice;
                ws2.Cell(rowCounter, 12).Value = bidItem.BidItemCost;
                //ws2.Cell(rowCounter, 6).Value = String.Format("{0}x{1}={2}", bidItem.BidItemQuantity, bidItem.BidItemUnitPrice, bidItem.BidItemCost);

                rowCounter++;
            }

            ws2.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws2.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws2.Columns().AdjustToContents();
            //ws1.Columns("A-B").AdjustToContents();
            //ws1.Columns("C").Style.Alignment.WrapText = true;
            //ws1.Columns("C").Width = 16;
            //ws1.Columns("D").AdjustToContents();
            //ws1.Columns("D-F").AdjustToContents();
        }

        public void WriteCoreDataReport(List<Structure> structures, List<string> notFoundIds, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            ws1.Name = "Core Data";
            ws1.Cell(1, 1).Value = "Str ID";
            ws1.Cell(1, 2).Value = "Insp Date";
            ws1.Cell(1, 3).Value = "Year Built";
            ws1.Cell(1, 4).Value = "Last Super";
            ws1.Cell(1, 5).Value = "Last Deck";
            ws1.Cell(1, 6).Value = "Last Olay";
            ws1.Cell(1, 7).Value = "Str Age";
            ws1.Cell(1, 8).Value = "Super Age";
            ws1.Cell(1, 9).Value = "Deck Age";
            ws1.Cell(1, 10).Value = "Olay Age";
            ws1.Cell(1, 11).Value = "Elem Num";
            ws1.Cell(1, 12).Value = "Elem Desc";
            ws1.Cell(1, 13).Value = "Unit";
            ws1.Cell(1, 14).Value = "Tot Qty";
            ws1.Cell(1, 15).Value = "CS1";
            ws1.Cell(1, 16).Value = "CS2";
            ws1.Cell(1, 17).Value = "CS3";
            ws1.Cell(1, 18).Value = "CS4";
            ws1.Cell(1, 19).Value = "CS5";
            ws1.Cell(1, 20).Value = "CS Num";
            int rowCounter = 2;
            int currentYear = DateTime.Now.Year;

            foreach (var str in structures)
            {
                foreach (var insp in str.CoreInspections)
                {
                    try
                    {
                        int inspYear = insp.InspectionDate.Year;
                        int lastOlay = 0;

                        try
                        {
                            lastOlay = str.Overlays.Where(e => e <= inspYear).Last();
                        }
                        catch { }

                        int lastSuper = str.SuperBuilts.Where(e => e <= inspYear).Last();
                        int lastDeck = str.DeckBuilts.Where(e => e <= inspYear).Last();

                        foreach (var elem in insp.Elements)
                        {
                            ws1.Cell(rowCounter, 1).Value = insp.StructureId;
                            ws1.Cell(rowCounter, 2).Value = String.Format("{0:MM/dd/yyyy}", insp.InspectionDate);
                            ws1.Cell(rowCounter, 3).Value = str.YearBuiltActual;
                            ws1.Cell(rowCounter, 4).Value = str.LastSuperReplacementYear;
                            ws1.Cell(rowCounter, 5).Value = str.LastDeckReplacementYear;

                            if (lastOlay == 0)
                            {
                                ws1.Cell(rowCounter, 6).Value = "";
                                ws1.Cell(rowCounter, 10).Value = "";
                            }
                            else
                            {
                                ws1.Cell(rowCounter, 6).Value = lastOlay;
                                ws1.Cell(rowCounter, 10).Value = String.Format("{0}", inspYear - lastOlay);
                            }

                            ws1.Cell(rowCounter, 7).Value = String.Format("{0}", inspYear - str.YearBuiltActual);
                            ws1.Cell(rowCounter, 8).Value = String.Format("{0}", inspYear - lastSuper);
                            ws1.Cell(rowCounter, 9).Value = String.Format("{0}", inspYear - lastDeck);
                            ws1.Cell(rowCounter, 11).Value = elem.ElemNum;
                            ws1.Cell(rowCounter, 12).Value = elem.ElemName;
                            ws1.Cell(rowCounter, 13).Value = elem.UnitOfMeasurement;
                            ws1.Cell(rowCounter, 14).Value = elem.TotalQuantity;
                            ws1.Cell(rowCounter, 15).Value = elem.Cs1Quantity;

                            if (elem.StateCount >= 2)
                            {
                                ws1.Cell(rowCounter, 16).Value = elem.Cs2Quantity;
                            }
                            else
                            {
                                ws1.Cell(rowCounter, 16).Value = "";
                            }

                            if (elem.StateCount >= 3)
                            {
                                ws1.Cell(rowCounter, 17).Value = elem.Cs3Quantity;
                            }
                            else
                            {
                                ws1.Cell(rowCounter, 17).Value = "";
                            }

                            if (elem.StateCount >= 4)
                            {
                                ws1.Cell(rowCounter, 18).Value = elem.Cs4Quantity;
                            }
                            else
                            {
                                ws1.Cell(rowCounter, 18).Value = "";
                            }

                            if (elem.StateCount >= 5)
                            {
                                ws1.Cell(rowCounter, 19).Value = elem.Cs5Quantity;
                            }
                            else
                            {
                                ws1.Cell(rowCounter, 19).Value = "";
                            }

                            ws1.Cell(rowCounter, 20).Value = elem.StateCount;
                            rowCounter++;
                        }

                    }
                    catch { }

                }


            }

            ws1.Columns().AdjustToContents();
        }

        public void WriteDesignBillableReport(List<Employee> emps, List<WorkActivity> workActivities, int startMonth, int endMonth, int startYear, int endYear, List<WorkActivity> distinctWorkActivities, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            var ws2 = workBook.Worksheet("Sheet2");
            var ws3 = workBook.Worksheet("Sheet3");
            ws1.Name = "By Employee";
            ws2.Name = "Summary";
            ws3.Name = "Activity Codes";

            ws1.Cell(1, 1).Value = String.Format("Bureau of Structures - Design Hours for {0}/{1} - {2}/{3}", startMonth, startYear, endMonth, endYear);
            ws1.Range(1, 1, 1, 9).Merge();
            ws1.Cell(2, 1).Value = "Employee";
            ws1.Cell(2, 2).Value = "Total Hours";
            ws1.Cell(2, 3).Value = "Billable Hours";
            ws1.Cell(2, 4).Value = "Non-Billable Hours";
            ws1.Cell(2, 5).Value = "Paid Leave";
            ws1.Cell(2, 6).Value = "Unpaid Leave";
            //ws1.Cell(2, 7).Value = "Uncategorized";
            ws1.Cell(2, 7).Value = "% Billable";
            ws1.Cell(2, 8).Value = "% Non-Billable";
            ws1.Cell(2, 9).Value = "% Paid Leave";
            ws1.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws1.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws1.Row(1).Style.Font.Bold = true;
            ws1.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws1.Row(2).Style.Font.Bold = true;

            var unpaidLeaveActivityCodes = new List<int>()
            {
                140, 141, 171, 177, 178, 179, 182
            };

            var billableActivityCodes = new List<int>()
            {
                214, 249, 654, 656, 657, 658, 659, 741, 746, 747, 771, 776, 779,
                780, 781, 782, 783, 786, 790, 792, 855, 856
            };

            // Categorized as "Other" in Access Database
            var nonBillableActivityCodes = new List<int>()
            {
                101, 102, 103, 106, 107, 108, 110, 120, 121, 122,
                123, 129, 142, 143, 144, 156, 167, 201, 275, 306,
                328, 647, 648, 649, 650, 651, 652, 686, 688, 689,
                690, 838, 853
            };

            var paidLeaveActivityCodes = new List<int>()
            {
                132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 151, 153, 154,
                157, 161, 163, 165, 169, 174, 175, 176, 177, 181
            };

            int rowCounter = 3;
            float totalTotalHours = 0;
            float totalBillableHours = 0;
            float totalNonBillableHours = 0;
            float totalPaidLeaveHours = 0;
            float totalUnpaidLeaveHours = 0;

            foreach (var emp in emps)
            {
                string empLastName = emp.LastName.ToUpper();

                if (emp.Timesheets.Count > 0)
                {
                    ws1.Cell(rowCounter, 1).Value = String.Format("{0}, {1}", emp.LastName, emp.FirstName);

                    // Total hours, excluding unpaid leave
                    var totalHours = emp.Timesheets
                        .Where(e => !unpaidLeaveActivityCodes.Contains(e.ActivityCode))
                        .Select(e => e.TotalHours)
                        .Sum();
                    ws1.Cell(rowCounter, 2).Value = totalHours;
                    totalTotalHours += totalHours;

                    // Billable hours
                    /*
                    var billableActivities = emp.Timesheets
                        .Where(e => (e.ActivityCode.Equals(104) && !e.ProjectId.Equals("0656-22-09"))
                                        || (e.ActivityCode.Equals(653) && !e.ProjectId.Equals("0656-22-09"))

                                    || billableActivityCodes.Contains(e.ActivityCode)
                                )
                        .Select(e => new { });
                    */
                    var billableHours = emp.Timesheets
                        .Where(e => (e.ActivityCode.Equals(104) && !e.ProjectId.StartsWith("06"))
                                        || (e.ActivityCode.Equals(653) && !e.ProjectId.StartsWith("06"))
                                        || billableActivityCodes.Contains(e.ActivityCode)
                                        || (nonBillableActivityCodes.Contains(e.ActivityCode) && !e.ProjectId.StartsWith("06"))
                                )
                        .Select(e => e.TotalHours)
                        .Sum();
                    ws1.Cell(rowCounter, 3).Value = billableHours;
                    ws1.Cell(rowCounter, 7).Value = Math.Round(billableHours / totalHours * 100);
                    totalBillableHours += billableHours;

                    // Non-Billable hours
                    var nonBillableHours = emp.Timesheets
                        .Where(e => (nonBillableActivityCodes.Contains(e.ActivityCode) && e.ProjectId.StartsWith("06"))
                                        || (e.ActivityCode.Equals(104) && e.ProjectId.StartsWith("06"))
                                        || (e.ActivityCode.Equals(653) && e.ProjectId.StartsWith("06"))
                              )
                        .Select(e => e.TotalHours)
                        .Sum();
                    ws1.Cell(rowCounter, 4).Value = nonBillableHours;
                    ws1.Cell(rowCounter, 8).Value = Math.Round(nonBillableHours / totalHours * 100);
                    totalNonBillableHours += nonBillableHours;

                    // Paid Leave hours
                    var paidLeaveHours = emp.Timesheets
                        .Where(e => paidLeaveActivityCodes.Contains(e.ActivityCode))
                        .Select(e => e.TotalHours)
                        .Sum();
                    ws1.Cell(rowCounter, 5).Value = paidLeaveHours;
                    ws1.Cell(rowCounter, 9).Value = Math.Round(paidLeaveHours / totalHours * 100);
                    totalPaidLeaveHours += paidLeaveHours;

                    // Unpaid Leave hours
                    var unpaidLeaveHours = emp.Timesheets
                        .Where(e => unpaidLeaveActivityCodes.Contains(e.ActivityCode))
                        .Select(e => e.TotalHours)
                        .Sum();
                    ws1.Cell(rowCounter, 6).Value = unpaidLeaveHours;
                    totalUnpaidLeaveHours += unpaidLeaveHours;

                    rowCounter++;
                }
            } // end- foreach (var emp in emps)

            ws1.Columns().AdjustToContents();

            // Summary sheet
            ws2.Cell(1, 1).Value = String.Format("Bureau of Structures - Design Total Hours for {0}/{1} - {2}/{3}", startMonth, startYear, endMonth, endYear);
            ws2.Range(1, 1, 1, 8).Merge();
            ws2.Cell(2, 1).Value = "Total Hours";
            ws2.Cell(2, 2).Value = "Total Billable";
            ws2.Cell(2, 3).Value = "Total Non-Billable";
            ws2.Cell(2, 4).Value = "Total Paid Leave";
            ws2.Cell(2, 5).Value = "Total Unpaid Leave";
            ws2.Cell(2, 6).Value = "% Total Billable";
            ws2.Cell(2, 7).Value = "% Total Non-Billable";
            ws2.Cell(2, 8).Value = "% Total Paid Leave";
            ws2.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws2.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws2.Row(1).Style.Font.Bold = true;
            ws2.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws2.Row(2).Style.Font.Bold = true;
            ws2.Cell(3, 1).Value = totalTotalHours;
            ws2.Cell(3, 2).Value = totalBillableHours;
            ws2.Cell(3, 3).Value = totalNonBillableHours;
            ws2.Cell(3, 4).Value = totalPaidLeaveHours;
            ws2.Cell(3, 5).Value = totalUnpaidLeaveHours;
            ws2.Cell(3, 6).Value = Math.Round(totalBillableHours / totalTotalHours * 100);
            ws2.Cell(3, 7).Value = Math.Round(totalNonBillableHours / totalTotalHours * 100);
            ws2.Cell(3, 8).Value = Math.Round(totalPaidLeaveHours / totalTotalHours * 100);
            ws2.Columns().AdjustToContents();

            // Activity Codes sheet
            ws3.Cell(1, 1).Value = String.Format("Bureau of Structures - Design Activities for {0}/{1} - {2}/{3}", startMonth, startYear, endMonth, endYear);
            ws3.Range(1, 1, 1, 3).Merge();
            ws3.Cell(2, 1).Value = "Activity Category";
            ws3.Cell(2, 2).Value = "Activity Code";
            ws3.Cell(2, 3).Value = "Activity Description";

            rowCounter = 3;
            foreach (var wa in workActivities)
            {
                ws3.Cell(rowCounter, 1).Value = wa.WorkActivityCategory;
                ws3.Cell(rowCounter, 2).Value = wa.WorkActivityCode;
                ws3.Cell(rowCounter, 3).Value = wa.WorkActivityDescription;
                rowCounter++;
            }

            ws3.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws3.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws3.Row(1).Style.Font.Bold = true;
            ws3.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws3.Row(2).Style.Font.Bold = true;
            ws3.Columns().AdjustToContents();
        }

        public void WriteElementDeteriorationRatesReport(List<ElementDeterioration> elemDetRates, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            ws1.Cell(1, 1).Value = "Element Number";
            ws1.Cell(1, 2).Value = "Element";
            ws1.Cell(1, 3).Value = "Median Yr 1";
            ws1.Cell(1, 4).Value = "Median Yr 2";
            ws1.Cell(1, 5).Value = "Median Yr 3";
            ws1.Cell(1, 6).Value = "Beta";
            ws1.Cell(1, 7).Value = "Relative Weight";

            List<ElementDeteriorationRatesRow> rows = new List<ElementDeteriorationRatesRow>();

            foreach (var rate in elemDetRates)
            {
                ElementDeteriorationRatesRow row = new ElementDeteriorationRatesRow();
                row.ElemNum = rate.ElemNum;
                row.ElemName = rate.ElemName;
                row.MedYr1 = rate.MedYr1;
                row.MedYr2 = rate.MedYr2;
                row.MedYr3 = rate.MedYr3;
                row.Beta = rate.Beta;
                row.RelativeWeight = rate.RelativeWeight;
                rows.Add(row);
            }

            ws1.Cell(2, 1).InsertData(rows.AsEnumerable());
        }

        public void WriteReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, XLWorkbook workBook, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType = WisamType.NeedsAnalysisFileTypes.ProgramConstrained)
        {
            #region WisamType.NeedsAnalysisFileTypes.Constrained
            if (needsAnalysisFileType == WisamType.NeedsAnalysisFileTypes.ProgramConstrained)
            {
                var ws1 = workBook.Worksheet("Sheet1");
                var ws2 = workBook.Worksheet("Sheet2");
                var ws3 = workBook.Worksheet("Sheet3");
                var ws4 = workBook.Worksheet("Sheet4");
                var ws5 = workBook.Worksheet("Sheet5");
                var ws6 = workBook.Worksheet("Sheet6");
                ws1.Name = "SUMMARY-ROLLUP";
                ws2.Name = "SUMMARY-PROGRAM";
                ws3.Name = "SORTABLE-PROGRAM";
                ws4.Name = "SUMMARY-BACKLOG";
                ws5.Name = "SORTABLE-BACKLOG";
                ws6.Name = "MISSING DATA";
                int colCounter = 1;

                var sortableColumnNames = new List<string>()
                {
                    "Str", "Year", "Primary WA", "Primary CAI", "Primary Cost", "P(riority) Score", "P Rank", "P Rel Rank",
                    "Incidental", "FIIPS"
                };

                foreach (var sortableColumnName in sortableColumnNames)
                {
                    ws3.Cell(1, colCounter).Value = sortableColumnName;
                    ws5.Cell(1, colCounter).Value = sortableColumnName;
                    colCounter++;
                }

                /*
                var missedCutColumnNames = new List<string>()
                {
                    "Str", "Year", "Primary WA", "Primary CAI", "Primary Cost", "P(riority) Score", "P Rank", "P Rel Rank",
                    "Incidental", "FIIPS"
                };

                colCounter = 1;
                foreach (var missedCutColumnName in missedCutColumnNames)
                {
                    ws5.Cell(1, colCounter).Value = missedCutColumnName;
                    colCounter++;
                }
                */

                // Write data rows
                List<NeedsAnalysisRowSortable> naRowsSortable = new List<NeedsAnalysisRowSortable>();
                List<NeedsAnalysisRowSortable> mcRows = new List<NeedsAnalysisRowSortable>();

                foreach (var structure in structures)
                {
                    for (int year = needsAnalysisInput.AnalysisStartYear; year <= needsAnalysisInput.AnalysisEndYear; year++)
                    {
                        // Program
                        StructureWorkAction swa = structure.YearlyConstrainedOptimalWorkActions.Where(e => e.WorkActionYear == year).First();
                        NeedsAnalysisRowSortable naRowSortable = new NeedsAnalysisRowSortable();
                        naRowSortable.StructureId = structure.StructureId;
                        naRowSortable.WorkActionYear = year;
                        naRowSortable.Primary = String.Format("({0}){1}", swa.WorkActionCode, swa.WorkActionCode == Code.DoNothing ? swa.WorkActionDesc : swa.ControllingCriteria == null ? swa.WorkActionDesc : swa.ControllingCriteria.WorkActionDesc);
                        naRowSortable.PrimaryCai = swa.CAI.CaiValue;
                        naRowSortable.PrimaryCost = swa.Cost;
                        naRowSortable.PriorityScore = swa.PriorityScore;
                        naRowSortable.PriorityScoreRank = swa.PriorityScoreRank;
                        naRowSortable.PriorityScoreRelativeRank = swa.PriorityScoreRelativeRank;

                        foreach (var secondaryWa in swa.SecondaryWorkActions)
                        {
                            naRowSortable.Incidentals += String.Format("({0}){1};", secondaryWa.WorkActionCode, secondaryWa.WorkActionDesc);
                        }

                        var fiips = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == year).ToList();
                        if (fiips.Count() > 0)
                        {
                            naRowSortable.Fiips = String.Format("({0})-{1}", fiips.First().WorkActionCode, fiips.First().WorkActionDesc);
                        }

                        naRowsSortable.Add(naRowSortable);

                        // Backlog 
                        var mc = structure.DidNotMakeCutWorkActions.Where(e => e.WorkActionYear == year).ToList();

                        if (mc.Count() > 0)
                        {
                            StructureWorkAction mcwa = mc.First();
                            NeedsAnalysisRowSortable mcRow = new NeedsAnalysisRowSortable();
                            mcRow.StructureId = structure.StructureId;
                            mcRow.WorkActionYear = year;
                            mcRow.Primary = String.Format("({0}){1}", mcwa.WorkActionCode, mcwa.WorkActionCode == Code.DoNothing ? mcwa.WorkActionDesc : mcwa.ControllingCriteria.WorkActionDesc);
                            mcRow.PrimaryCai = swa.CAI.CaiValue;
                            mcRow.PrimaryCost = mcwa.Cost;
                            mcRow.PriorityScore = mcwa.PriorityScore;
                            mcRow.PriorityScoreRank = mcwa.PriorityScoreRank;
                            mcRow.PriorityScoreRelativeRank = mcwa.PriorityScoreRelativeRank;

                            foreach (var secondaryWa in mcwa.SecondaryWorkActions)
                            {
                                mcRow.Incidentals += String.Format("({0}){1};", secondaryWa.WorkActionCode, secondaryWa.WorkActionDesc);
                            }

                            if (fiips.Count() > 0)
                            {
                                mcRow.Fiips = String.Format("({0})-{1}", fiips.First().WorkActionCode, fiips.First().WorkActionDesc);
                            }

                            mcRows.Add(mcRow);
                        }
                    }
                }

                ws3.Cell(2, 1).InsertData(naRowsSortable.AsEnumerable());
                ws5.Cell(2, 1).InsertData(mcRows.AsEnumerable());
            }
            #endregion WisamType.NeedsAnalysisFileTypes.Constrained
        }

        public void WriteReport(WisamType.AnalysisReports report, List<Structure> structures, List<string> notFoundIds, List<StructureWorkAction> swas, int startYear, int endYear, DateTime startTime, XLWorkbook workBook, bool debug = false, bool showPiFactors = false, string regionNumber = "", bool state = false, bool local = false, List<string> similarComboWorkActions = null)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            var ws2 = workBook.Worksheet("Sheet2");
            var ws3 = workBook.Worksheet("Sheet3");
            var ws4 = workBook.Worksheet("Sheet4");
            var ws5 = workBook.Worksheet("Sheet5");
            var ws6 = workBook.Worksheet("Sheet6");
            List<string> ws1Columns;
            List<string> ws3Columns;
            List<string> caiColumns;
            int colCounter = 1;

            switch (report)
            {
                #region LocalNeeds
                case WisamType.AnalysisReports.LocalNeeds:
                    ws1.Name = "WEB POSTING - " + DateTime.Now.Year;
                    ws2.Name = "LIST FOR REGIONAL - " + DateTime.Now.Year;
                    ws3.Name = "LIST FOR COUNTIES - " + DateTime.Now.Year;

                    ws1Columns = new List<string>()
                        {
                            "Structure", "Region", "Feat On", "Feat Und",
                            "Muni", "County", "Owner", "Deck Area",
                            "Sufficiency", "Deficiency", "Base Eligibility",
                            "Optimal Work", "Paint Work", "Rail Work"
                        };

                    List<string> ws2Columns = new List<string>()
                        {
                            "Structure", "Region", "Feat On", "Feat Und",
                            "Muni", "County", "Owner", "Deck Area",
                            "Sufficiency", "Deficiency", "Base Eligibility",
                            "Optimal Work", "Paint Work", "Rail Work",
                            "Optimal Work Cost", "Priority Index", "County Rank"
                        };

                    ws3Columns = new List<string>()
                        {
                            "Structure", "Region", "Feat On", "Feat Und",
                            "Muni", "County", "Owner", "Deck Area",
                            "Sufficiency", "Deficiency", "Base Eligibility",
                            "Optimal Work", "Paint Work", "Rail Work",
                            "Priority Index"
                        };

                    colCounter = 1;
                    foreach (var ws1Column in ws1Columns)
                    {
                        ws1.Cell(1, colCounter).Value = ws1Column;
                        colCounter++;
                    }

                    colCounter = 1;
                    foreach (var ws2Column in ws2Columns)
                    {
                        ws2.Cell(1, colCounter).Value = ws2Column;
                        colCounter++;
                    }

                    colCounter = 1;
                    foreach (var ws3Column in ws3Columns)
                    {
                        ws3.Cell(1, colCounter).Value = ws3Column;
                        colCounter++;
                    }

                    // Rank works by county
                    List<StructureCountyRank> structuresRanked = new List<StructureCountyRank>();
                    var counties = structures.Select(c => c.County).Distinct();
                    foreach (var county in counties)
                    {
                        int rank = 0;
                        var strs = structures.OrderByDescending(s => s.PriorityScore).Where(s => s.County != null && s.County.Equals(county));
                        double previousPriorityScore = -1;

                        foreach (var str in strs)
                        {
                            StructureCountyRank scr = new StructureCountyRank();
                            scr.StructureId = str.StructureId;
                            scr.County = county;
                            scr.PriorityScore = Math.Round(str.PriorityScore, 2);

                            if (str.PriorityScore != previousPriorityScore)
                                rank++;

                            scr.CountyRank = rank;

                            structuresRanked.Add(scr);
                            previousPriorityScore = str.PriorityScore;
                        }
                    }

                    List<LocalNeedsRowWebPosting> lnwpRows = new List<LocalNeedsRowWebPosting>();
                    List<LocalNeedsRowRegions> lnrrRows = new List<LocalNeedsRowRegions>();
                    List<LocalNeedsRowCounties> lnrcRows = new List<LocalNeedsRowCounties>();

                    foreach (var structure in structures)
                    {
                        for (int year = startYear; year <= endYear; year++)
                        {
                            LocalNeedsRowWebPosting lnwpRow = new LocalNeedsRowWebPosting();

                            lnwpRow.StructureId = structure.StructureId;
                            lnwpRow.Region = structure.Region;
                            lnwpRow.FeatureOn = structure.FeatureOn;
                            lnwpRow.FeatureUnder = structure.FeatureUnder;
                            lnwpRow.Muni = structure.Municipality;
                            lnwpRow.County = structure.County;
                            lnwpRow.Owner = structure.Owner;
                            lnwpRow.DeckArea = structure.DeckArea;
                            lnwpRow.Sufficiency = Math.Round(structure.SufficiencyRatingCurrent, 1);

                            if (structure.Deficiencies.Count() == 2)
                            {
                                lnwpRow.Deficiency = "SD and FO";
                            }
                            else if (structure.Deficiencies.Count() == 1)
                            {
                                lnwpRow.Deficiency = structure.Deficiencies.First();
                            }

                            lnwpRow.LocalFundingEligibility = structure.LocalFundingEligibility;

                            StructureWorkAction optimalWa = null;
                            try
                            {
                                optimalWa = structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == year).First();
                            }
                            catch { }

                            if (optimalWa != null)
                            {
                                lnwpRow.Primary = String.Format("({0}){1}", optimalWa.WorkActionCode, optimalWa.WorkActionDesc);

                                if (optimalWa.WorkActionDesc.Contains("PAINT") && optimalWa.CombinedWorkAction)
                                {
                                    lnwpRow.PaintWork = "PAINT (COMPLETE)";
                                }
                                else
                                {
                                    if (optimalWa.SecondaryWorkActions != null)
                                    {
                                        if (optimalWa.SecondaryWorkActions.Where(e => e.WorkActionDesc.Contains("PAINT")
                                                && e.WorkActionDesc.Contains("ZONE")).Count() > 0)
                                        {
                                            lnwpRow.PaintWork = "PAINT (ZONE OR SPOT)";
                                        }
                                    }
                                }

                                if (optimalWa.SecondaryWorkActions != null)
                                {
                                    var railWork = optimalWa.SecondaryWorkActions.Where(e => e.WorkActionDesc.Contains("RAILING"));
                                    if (railWork.Count() > 0)
                                    {
                                        lnwpRow.RailWork = railWork.First().WorkActionDesc;
                                    }
                                }
                            }

                            lnwpRows.Add(lnwpRow);

                            // Row for Sheet 2
                            LocalNeedsRowRegions lnrrRow = new LocalNeedsRowRegions(lnwpRow);
                            lnrrRow.PriorityScore = Math.Round(structure.PriorityScore, 2);
                            lnrrRow.CountyRank = structuresRanked.Where(s => s.StructureId.Equals(lnrrRow.StructureId)).First().CountyRank;

                            if (optimalWa != null)
                            {
                                lnrrRow.PrimaryCost = Math.Round(optimalWa.Cost);
                            }

                            lnrrRows.Add(lnrrRow);

                            // Row for Sheet 3
                            LocalNeedsRowCounties lnrcRow = new LocalNeedsRowCounties(lnwpRow);
                            lnrcRow.PriorityScore = Math.Round(structure.PriorityScore, 2);
                            lnrcRows.Add(lnrcRow);
                        }
                    }

                    // Determine ranking of works within each county using the priority score
                    ws1.Cell(2, 1).InsertData(lnwpRows.AsEnumerable());
                    ws2.Cell(2, 1).InsertData(lnrrRows.AsEnumerable());
                    ws3.Cell(2, 1).InsertData(lnrcRows.AsEnumerable());
                    ws1.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    ws1.Columns().AdjustToContents();
                    ws2.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    ws2.Columns().AdjustToContents();
                    ws3.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    ws3.Columns().AdjustToContents();
                    break;
                #endregion LocalNeeds

                #region MetaManager
                case WisamType.AnalysisReports.MetaManager:
                    ws1.Name = "Meta-Manager";
                    List<string> colHeaders = new List<string>()
                        {
                            "Str", "2030 Corr", "Region", "County", "Feat On", "Feat Und",
                            "Str Type", "Matl", "Num Spans", "Tot Len(ft)", "Inv Rating",
                            "Opr Rating", "Load Posting", "Last Insp", "Const Hist",
                            "Year", "Str Age", "Deck Age",
                            "DN CAI", "DN NBI Deck", "DN NBI Sup", "DN NBI Sub", "DN NBI Cul",
                            "Primary WA", "Primary CAI", "NBI Deck", "NBI Sup", "NBI Sub", "NBI Cul",
                            "Primary Cost", "Life Ext(yrs)",
                            "FIIPS WA", "FIIPS CAI", "FIIPS NBI Deck", "FIIPS NBI Sup", "FIIPS NBI Sub", "FIIPS NBI Cul",
                            "COST(W/O DEL)", "FIIPS Proj ID", "PROJ CONCEPT", "DOT PROG"
                        };

                    colCounter = 1;
                    foreach (var colHeader in colHeaders)
                    {
                        ws1.Cell(1, colCounter).Value = colHeader;
                        colCounter++;
                    }

                    List<MetaRowSortable> metaRows = new List<MetaRowSortable>();

                    foreach (var structure in structures)
                    {
                        foreach (var doNothing in structure.YearlyDoNothings)
                        {
                            if (doNothing.WorkActionYear < startYear)
                                continue;

                            try
                            {
                                MetaRowSortable metaRow = new MetaRowSortable();
                                metaRow.StructureId = structure.StructureId;
                                metaRow.CorridorCode = structure.CorridorCode;
                                metaRow.Region = structure.Region;
                                metaRow.County = structure.County;
                                metaRow.FeatureOn = structure.FeatureOn;
                                metaRow.FeatureUnder = structure.FeatureUnder;
                                metaRow.StructureType = structure.StructureType;
                                metaRow.MainSpanMaterial = structure.MainSpanMaterial;
                                metaRow.NumSpans = structure.NumSpans;
                                metaRow.TotalLengthSpans = structure.TotalLengthSpans;
                                metaRow.InventoryRating = structure.InventoryRating;
                                metaRow.OperatingRating = structure.OperatingRating;

                                if (structure.LoadPostingCode != 0)
                                {
                                    metaRow.LoadPosting = string.Format("{0}", structure.LoadPostingDesc);
                                }
                                else
                                {
                                    metaRow.LoadPosting = "None";
                                }

                                metaRow.LastInspectionDate = structure.LastInspection.InspectionDate;
                                metaRow.ConstructionHistory = structure.ConstructionHistory;
                                metaRow.WorkActionYear = doNothing.WorkActionYear;
                                metaRow.Age = doNothing.WorkActionYear - structure.YearBuiltActual;

                                if (structure.DeckBuilts.Count > 0)
                                {
                                    int currentDeckBuilt = structure.DeckBuilts.Where(e => e <= doNothing.WorkActionYear).OrderByDescending(e => e).First();
                                    metaRow.DeckAge = doNothing.WorkActionYear - (currentDeckBuilt);
                                }

                                if (doNothing.CAI != null)
                                {
                                    metaRow.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 1);

                                    if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                    {
                                        metaRow.DoNothingNbiDeck = -1;
                                        metaRow.DoNothingNbiSup = -1;
                                        metaRow.DoNothingNbiSub = -1;

                                        try
                                        {
                                            metaRow.DoNothingNbiCulv = Math.Round(doNothing.CAI.NbiRatings.CulvertRatingVal, 2);
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            metaRow.DoNothingNbiDeck = Math.Round(doNothing.CAI.NbiRatings.DeckRatingVal, 2);

                                        }
                                        catch { }

                                        try
                                        {
                                            metaRow.DoNothingNbiSup = Math.Round(doNothing.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                        }
                                        catch { }

                                        try
                                        {
                                            metaRow.DoNothingNbiSub = Math.Round(doNothing.CAI.NbiRatings.SubstructureRatingVal, 2);
                                        }
                                        catch { }

                                        metaRow.DoNothingNbiCulv = -1;
                                    }
                                }
                                // end if (doNothing.CAI != null)

                                try
                                {
                                    StructureWorkAction optimalWa = structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (optimalWa != null)
                                    {
                                        if (optimalWa.WorkActionCode != Code.DoNothing)
                                        {
                                            metaRow.Primary = String.Format("({0}){1}", optimalWa.WorkActionCode, optimalWa.ControllingCriteria.WorkActionDesc);
                                        }
                                        // end if (optimalWa.WorkActionCode != Code.DoNothing)

                                        metaRow.PrimaryCai = Math.Round(optimalWa.CAI.CaiValue, 1);
                                        metaRow.PrimaryCost = Math.Round(optimalWa.Cost, 0);
                                        metaRow.PrimaryLifeExtension = Convert.ToInt32(optimalWa.LifeExtension);

                                        if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                        {
                                            metaRow.NbiDeck = -1;
                                            metaRow.NbiSup = -1;
                                            metaRow.NbiSub = -1;
                                            metaRow.NbiCulv = Math.Round(optimalWa.CAI.NbiRatings.CulvertRatingVal, 2);
                                        }
                                        else
                                        {
                                            metaRow.NbiDeck = Math.Round(optimalWa.CAI.NbiRatings.DeckRatingVal, 2);
                                            metaRow.NbiSup = Math.Round(optimalWa.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                            metaRow.NbiSub = Math.Round(optimalWa.CAI.NbiRatings.SubstructureRatingVal, 2);
                                            metaRow.NbiCulv = -1;
                                        }
                                    }
                                    // end if (optimalWa != null)
                                }
                                catch { }

                                try
                                {
                                    StructureWorkAction programmedWorkAction = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (programmedWorkAction != null)
                                    {
                                        metaRow.FiipsCai = Math.Round(programmedWorkAction.CAI.CaiValue, 1);

                                        if (programmedWorkAction.WorkActionCode != Code.DoNothing)
                                        {
                                            metaRow.Fiips = String.Format("({0}){1}", programmedWorkAction.WorkActionCode, programmedWorkAction.WorkActionDesc);
                                            metaRow.FosProjectId = programmedWorkAction.FosProjId;
                                            metaRow.ProjectConcept = programmedWorkAction.PlanningProjectConceptCode;
                                            metaRow.DotProgram = programmedWorkAction.WisDOTProgramDesc;
                                            metaRow.TotalCostWithoutDelivery = programmedWorkAction.TotalCostWithoutDeliveryAmount;
                                        }
                                        // end if (programmedWorkAction.WorkActionCode != Code.DoNothing)

                                        if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                        {
                                            metaRow.FiipsNbiDeck = -1;
                                            metaRow.FiipsNbiSup = -1;
                                            metaRow.FiipsNbiSub = -1;

                                            try
                                            {
                                                metaRow.FiipsNbiCulv = Math.Round(programmedWorkAction.CAI.NbiRatings.CulvertRatingVal, 2);
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                metaRow.FiipsNbiDeck = Math.Round(programmedWorkAction.CAI.NbiRatings.DeckRatingVal, 2);

                                            }
                                            catch { }

                                            try
                                            {
                                                metaRow.FiipsNbiSup = Math.Round(programmedWorkAction.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                            }
                                            catch { }

                                            try
                                            {
                                                metaRow.FiipsNbiSub = Math.Round(programmedWorkAction.CAI.NbiRatings.SubstructureRatingVal, 2);
                                            }
                                            catch { }

                                            metaRow.FiipsNbiCulv = -1;
                                        }
                                        // end if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                    }
                                    // end if (programmedWorkAction != null)
                                }
                                catch { }

                                metaRows.Add(metaRow);
                            }
                            catch { }
                        }
                        // end foreach (var doNothing in structure.YearlyDoNothings)
                    }
                    // end foreach (var structure in structures)

                    ws1.Cell(2, 1).InsertData(metaRows.AsEnumerable());
                    break;
                #endregion MetaManager

                #region RegionNeeds, StrDeckReplacements, Flexible
                case WisamType.AnalysisReports.RegionNeeds:
                case WisamType.AnalysisReports.RegionNeedsNew:
                case WisamType.AnalysisReports.StrDeckReplacements:
                case WisamType.AnalysisReports.Flexible:
                    ws3.Name = "SORTABLE";
                    ws3Columns = new List<string>()
                        {
                            "Str", "2030 Corr", "Region", "County", "Feat On", "Feat Und",
                            "Str Type", "Matl", "Num Spans", "Tot Len(ft)", "Inv Rating",
                            "Opr Rating", "Load Posting", "Last Insp", "Const Hist",
                            "Year",
                            "Str Age", "Prog Deck Age",
                            "DN CAI", "DN NBI Deck", "DN NBI Sup", "DN NBI Sub", "DN NBI Cul",
                            "Prog WA", "Prog CAI", "Prog Cost",
                            "Life Ext(yrs)", "Incidental", "FIIPS WA", "FIIPS CAI", "COST(W/O DEL)", "FIIPS Proj ID",
                            "PROJ CONCEPT", "DOT PROG",
                            "FIIPS NBI Deck", "FIIPS NBI Sup", "FIIPS NBI Sub", "FIIPS NBI Cul",
                            "DN Primary WA",
                            "NBI Interpolated Bounded",
                            "NBI Inspected",
                            "NBI Deck", "NBI Sup", "NBI Sub", "NBI Cul"
                        };

                    /*
                    ,
                            "IF", "RF", "SCF", "CF", "CEF", "PI",
                            "Deck 1080", "Debug Info"*/
                    colCounter = 1;
                    foreach (var ws3Column in ws3Columns)
                    {
                        ws3.Cell(1, colCounter).Value = ws3Column;
                        colCounter++;
                    }

                    List<RegionNeedsRowSortable> rnsRows = new List<RegionNeedsRowSortable>();
                    List<PiStaticRowSortable> piStaticRows = new List<PiStaticRowSortable>();
                    List<PiVariableRowSortable> piVariableRows = new List<PiVariableRowSortable>();

                    foreach (var structure in structures)
                    {
                        try
                        {
                            PiStaticRowSortable piStaticRow = new PiStaticRowSortable();
                            piStaticRow.StructureId = structure.StructureId;
                            piStaticRow.CorridorCode = structure.CorridorCode;
                            piStaticRow.Region = structure.Region;
                            piStaticRow.County = structure.County;
                            piStaticRow.FeatureOn = structure.FeatureOn;
                            piStaticRow.FeatureUnder = structure.FeatureUnder;
                            piStaticRow.StructureType = structure.StructureType;
                            piStaticRow.MainSpanMaterial = structure.MainSpanMaterial;
                            piStaticRow.NumSpans = structure.NumSpans;
                            piStaticRow.TotalLengthSpans = structure.TotalLengthSpans;
                            piStaticRow.InventoryRating = structure.InventoryRating;
                            piStaticRow.OperatingRating = structure.OperatingRating;

                            if (structure.LoadPostingCode != 0)
                            {
                                piStaticRow.LoadPosting = string.Format("{0}", structure.LoadPostingDesc);
                                piStaticRow.LoadPostingTonnage = structure.LoadPostingTonnage;
                            }
                            else
                            {
                                piStaticRow.LoadPosting = "None";
                                piStaticRow.LoadPostingTonnage = -1;
                            }

                            piStaticRow.LastInspectionDate = structure.LastInspection.InspectionDate;
                            piStaticRow.ConstructionHistory = structure.ConstructionHistory;

                            piStaticRow.CplxStr = structure.CplxStr;
                            piStaticRow.LrgStr = structure.LrgStr;
                            piStaticRow.ScourCritical = structure.ScourCritical;
                            piStaticRow.FractureCritical = structure.FractureCritical;
                            //piStaticRow.Age = DateTime.Now.Year - structure.YearBuiltActual;
                            //piStaticRow.DeckAge = DateTime.Now.Year - structure.DeckBuiltYearActual;
                            piStaticRow.DetLen = structure.DetLen;
                            piStaticRow.FunctionalClassificationOn = structure.FunctionalClassificationOn;
                            piStaticRow.FunctionalClassificationUnder = structure.FunctionalClassificationUnder;
                            piStaticRow.Municipality = structure.Municipality;
                            piStaticRow.MunicipalityNumber = structure.MunicipalityNumber;
                            piStaticRow.MaxVehicleWeight = structure.MaxVehicleWeight;
                            piStaticRow.VerticalClearanceUnderMin = structure.VerticalClearanceUnderMin;
                            piStaticRow.DamageInspectionsCount = structure.DamageInspectionsCount;
                            piStaticRow.Nhs = structure.Nhs;
                            piStaticRow.RouteSystemOn = structure.RouteSystemOn;
                            piStaticRow.RouteSystemUnder = structure.RouteSystemUnder;
                            piStaticRow.Adt = structure.Adt;
                            piStaticRow.AdttPercent = structure.AdttPercent;
                            piStaticRow.LanesOn = structure.LanesOn;
                            piStaticRow.DeckWidthMin = structure.MinDeckWidth;
                            piStaticRow.DeckArea = Convert.ToSingle(structure.DeckArea);
                            piStaticRow.BorderBridge = structure.BorderBridge;
                            piStaticRow.BorderState = structure.BorderState;
                            piStaticRow.MunicipalPlanningBridge = structure.MunicipalPlanningBridge;
                            piStaticRow.MunicipalPlanningAgency = structure.MunicipalPlanningAgency;
                            piStaticRow.PrimaryHighwayFreightSystemBridge = structure.PrimaryHighwayFreightSystemBridge;
                            piStaticRow.GisStateBridgeCoordinatesWithin500FtHsisCoordinates = structure.GisStateBridgeCoordinatesWithin500FtHsisCoordinates;
                            piStaticRow.GisWislrLocalBridgeCoordinatesWithin500FtHsisCoordinates = structure.GisWislrLocalBridgeCoordinatesWithin500FtHsisCoordinates;
                            piStaticRow.GisCorridor2030Code = structure.GisCorridor2030Code;
                            piStaticRow.GisDividedHighwayCode = structure.GisDividedHighwayCode;
                            piStaticRow.GisFunctionalClassCode = structure.GisFunctionalClassCode;
                            piStaticRow.GisFunctionalClassAbbreviation = structure.GisFunctionalClassAbbreviation;
                            piStaticRow.GisFunctionalClassDescription = structure.GisFunctionalClassDescription;
                            piStaticRow.GisNhsDesignation = structure.GisNhsDesignation;
                            piStaticRow.GisProjectRouteType = structure.GisProjectRouteType;
                            piStaticRow.GisProjectRouteName = structure.GisProjectRouteName;
                            piStaticRow.GisLongTruckRouteDesignation = structure.GisLongTruckRouteDesignation;
                            piStaticRow.GisMaintenanceJurisdictionCode = structure.GisMaintenanceJurisdictionCode;
                            piStaticRow.GisMaintenanceJurisdictionRouteType = structure.GisMaintenanceJurisdictionRouteType;
                            piStaticRow.GisMaintenanceJurisdictionRouteName = structure.GisMaintenanceJurisdictionRouteName;
                            piStaticRow.GisOsowHighClearanceRoute = structure.GisOsowHighClearanceRoute;
                            piStaticRow.GisOsowRouteType = structure.GisOsowRouteType;
                            piStaticRow.GisOsowRouteName = structure.GisOsowRouteName;
                            piStaticRow.GisOsowRankingNumber = structure.GisOsowRankingNumber;
                            piStaticRow.GisOsowRankingName = structure.GisOsowRankingName;

                            piStaticRow.MmRoadwaySegmentId = structure.MmRoadwaySegmentId;
                            piStaticRow.MmForecastedAadtYear1 = structure.MmForecastedAadtYear1;
                            piStaticRow.MmForecastedAadtYear5 = structure.MmForecastedAadtYear5;
                            piStaticRow.MmForecastedAadtYear10 = structure.MmForecastedAadtYear10;
                            piStaticRow.MmForecastedAadtYear15 = structure.MmForecastedAadtYear15;
                            piStaticRow.MmForecastedAadtYear20 = structure.MmForecastedAadtYear20;
                            piStaticRow.MmForecastedTruckPercentageAadtYear1 = structure.MmForecastedTruckPercentageAadtYear1;
                            piStaticRow.MmRoadwayPostedSpeedLimit = structure.MmRoadwayPostedSpeedLimit;
                            piStaticRow.MmCorridorsCode2030 = structure.MmCorridorsCode2030;
                            piStaticRow.MmFunctionalClassificationOn = structure.MmFunctionalClassificationOn;
                            piStaticRow.MmDividedHighwayCode = structure.MmDividedHighwayCode;
                            piStaticRow.MmTrafficSegmentId = structure.MmTrafficSegmentId;

                            foreach (var doNothing in structure.YearlyDoNothings)
                            {
                                if (doNothing.WorkActionYear < startYear)
                                    continue;

                                PiVariableRowSortable piVarRow = new PiVariableRowSortable();
                                RegionNeedsRowSortable rnsRow = new RegionNeedsRowSortable();
                                rnsRow.StructureId = structure.StructureId;
                                piVarRow.StructureId = structure.StructureId;
                                rnsRow.CorridorCode = structure.CorridorCode;
                                rnsRow.Region = String.Format("{0}-{1}", structure.RegionNumber, structure.Region);
                                rnsRow.County = String.Format("{0}-{1}", structure.CountyNumber, structure.County);
                                rnsRow.FeatureOn = structure.FeatureOn;
                                rnsRow.FeatureUnder = structure.FeatureUnder;
                                rnsRow.StructureType = structure.StructureType;
                                rnsRow.MainSpanMaterial = structure.MainSpanMaterial;
                                rnsRow.NumSpans = structure.NumSpans;
                                rnsRow.TotalLengthSpans = Math.Round(structure.TotalLengthSpans, 2);
                                rnsRow.InventoryRating = structure.InventoryRating;
                                rnsRow.OperatingRating = structure.OperatingRating;

                                if (structure.LoadPostingCode != 0)
                                {
                                    rnsRow.LoadPosting = string.Format("{0}", structure.LoadPostingDesc);
                                }
                                else
                                {
                                    rnsRow.LoadPosting = "None";
                                }

                                rnsRow.LastInspectionDate = structure.LastInspection.InspectionDate;
                                rnsRow.ConstructionHistory = structure.ConstructionHistory;

                                //double importance = structure.PriorityFactors.Where(e => e.FactorCode.Equals("IF")).First().FactorValue
                                //* structure.PriorityFactors.Where(e => e.FactorCode.Equals("IF")).First().FactorWeight;
                                //rnsRow.ImportanceFactor = Math.Round(importance, 3);
                                //rnsRow.RiskFactor = Math.Round(structure.PriorityFactors.Where(e => e.FactorCode.Equals("RF")).First().FactorValue
                                //* structure.PriorityFactors.Where(e => e.FactorCode.Equals("RF")).First().FactorWeight, 3);
                                //rnsRow.StructuralCapacityFactor = Math.Round(structure.PriorityFactors.Where(e => e.FactorCode.Equals("SCF")).First().FactorValue
                                //* structure.PriorityFactors.Where(e => e.FactorCode.Equals("SCF")).First().FactorWeight, 3);

                                PriorityFactorMeasurement conditionFactorMeasurement = structure.PriorityFactorMeasurements.Where(e => e.FactorCode.Equals("CF")).First();
                                string conditionFactorMeasurementExpression = conditionFactorMeasurement.GrossValueFormula;
                                //rnsRow.ConditionFactor = 0;

                                PriorityFactorMeasurement cefMeasurement = structure.PriorityFactorMeasurements.Where(e => e.FactorCode.Equals("CEF")).First();
                                string cefExpression = cefMeasurement.GrossValueFormula;
                                //rnsRow.CostEffectivenessFactor = 0;

                                //rnsRow.PriorityIndex = Math.Round(structure.PriorityIndex, 1);
                                rnsRow.WorkActionYear = doNothing.WorkActionYear;
                                rnsRow.Age = doNothing.WorkActionYear - structure.YearBuiltActual;
                                //piStaticRow.Age = rnsRow.Age;
                                piVarRow.WorkActionYear = rnsRow.WorkActionYear;
                                piVarRow.Age = rnsRow.Age;

                                if (structure.DeckBuilts.Count > 0)
                                {
                                    int currentDeckBuilt = structure.DeckBuilts.Where(e => e < doNothing.WorkActionYear).OrderByDescending(e => e).First();
                                    rnsRow.DeckAge = doNothing.WorkActionYear - (currentDeckBuilt);
                                    piVarRow.DeckAge = rnsRow.DeckAge;
                                }

                                if (doNothing.CAI != null)
                                {
                                    rnsRow.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 1);

                                    if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                    {
                                        rnsRow.DoNothingNbiDeck = -1;
                                        rnsRow.DoNothingNbiSup = -1;
                                        rnsRow.DoNothingNbiSub = -1;

                                        try
                                        {
                                            rnsRow.DoNothingNbiCulv = Math.Round(doNothing.CAI.NbiRatings.CulvertRatingVal, 2);
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            rnsRow.DoNothingNbiDeck = Math.Round(doNothing.CAI.NbiRatings.DeckRatingVal, 2);

                                        }
                                        catch { }

                                        try
                                        {
                                            rnsRow.DoNothingNbiSup = Math.Round(doNothing.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                        }
                                        catch { }

                                        try
                                        {
                                            rnsRow.DoNothingNbiSub = Math.Round(doNothing.CAI.NbiRatings.SubstructureRatingVal, 2);
                                        }
                                        catch { }

                                        rnsRow.DoNothingNbiCulv = -1;
                                    }

                                    piVarRow.DoNothingCaiValue = rnsRow.DoNothingCaiValue;
                                }

                                try
                                {
                                    StructureWorkAction optimalWa = structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (optimalWa != null)
                                    {
                                        if (optimalWa.WorkActionCode != Code.DoNothing)
                                        {
                                            rnsRow.Primary = String.Format("({0}){1}", optimalWa.WorkActionCode, optimalWa.ControllingCriteria.WorkActionDesc);
                                            piVarRow.Primary = rnsRow.Primary;
                                            //piVarRow.WiSAMSWorkType = optimalWa.WorkActionCode;

                                            if (debug && optimalWa.ControllingCriteria != null)
                                            {
                                                rnsRow.Primary += String.Format("; Rule ID {0}", optimalWa.ControllingCriteria.RuleId.ToString());
                                                piVarRow.Primary = rnsRow.Primary;
                                            }
                                        }
                                        else
                                        {
                                            //piVarRow.WiSAMSWorkType = "DN";
                                        }

                                        //piVarRow.ComboWorkType += piVarRow.WiSAMSWorkType;
                                        rnsRow.PrimaryCai = Math.Round(optimalWa.CAI.CaiValue, 1);
                                        rnsRow.PrimaryCost = Math.Round(optimalWa.Cost, 0);
                                        rnsRow.PrimaryLifeExtension = Convert.ToInt32(optimalWa.LifeExtension);
                                        piVarRow.PrimaryCai = rnsRow.PrimaryCai;
                                        piVarRow.PrimaryCost = rnsRow.PrimaryCost;
                                        piVarRow.PrimaryLifeExtension = rnsRow.PrimaryLifeExtension;

                                        if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                        {
                                            rnsRow.NbiDeck = -1;
                                            rnsRow.NbiSup = -1;
                                            rnsRow.NbiSub = -1;
                                            rnsRow.NbiCulv = Math.Round(optimalWa.CAI.NbiRatings.CulvertRatingVal, 2);
                                            rnsRow.NbiInterpolated = "";
                                            rnsRow.NbiInspected = "";

                                            piVarRow.NbiDeck = rnsRow.NbiDeck;
                                            piVarRow.NbiSup = rnsRow.NbiSup;
                                            piVarRow.NbiSub = rnsRow.NbiSub;
                                            piVarRow.NbiCulv = rnsRow.NbiCulv;
                                            piVarRow.NbiInspected = rnsRow.NbiInspected;
                                        }
                                        else
                                        {
                                            rnsRow.NbiDeck = Math.Round(optimalWa.CAI.NbiRatings.DeckRatingVal, 2);
                                            rnsRow.NbiSup = Math.Round(optimalWa.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                            rnsRow.NbiSub = Math.Round(optimalWa.CAI.NbiRatings.SubstructureRatingVal, 2);
                                            rnsRow.NbiCulv = -1;
                                            rnsRow.NbiInterpolated = Math.Round(structure.DeckRatingInterpolated, 2).ToString()
                                                                        + ", "
                                                                        + Math.Round(structure.SuperstructureRatingInterpolated, 2).ToString()
                                                                         + ", "
                                                                        + Math.Round(structure.SubstructureRatingInterpolated, 2).ToString();
                                            rnsRow.NbiInspected = Math.Round(structure.DeckRatingInspected, 2).ToString()
                                                                        + ", "
                                                                        + Math.Round(structure.SuperstructureRatingInspected, 2).ToString()
                                                                         + ", "
                                                                        + Math.Round(structure.SubstructureRatingInspected, 2).ToString();

                                            piVarRow.NbiDeck = rnsRow.NbiDeck;
                                            piVarRow.NbiSup = rnsRow.NbiSup;
                                            piVarRow.NbiSub = rnsRow.NbiSub;
                                            piVarRow.NbiCulv = rnsRow.NbiCulv;

                                            piVarRow.NbiInspected = rnsRow.NbiInspected;
                                        }

                                        // Condition Factor
                                        string cfExpression = conditionFactorMeasurementExpression.Replace("CAI", rnsRow.DoNothingCaiValue.ToString());
                                        //rnsRow.ConditionFactor = Math.Round(Convert.ToDouble(new DataTable().Compute(cfExpression, null)), 3);

                                        /*
                                        if (rnsRow.ConditionFactor > 1)
                                        {
                                            rnsRow.ConditionFactor = 1;
                                        }

                                        rnsRow.ConditionFactor = rnsRow.ConditionFactor * conditionFactorMeasurement.Weight * conditionFactorMeasurement.FactorWeight;
                                        rnsRow.PriorityIndex += rnsRow.ConditionFactor;
                                        */
                                        // Cost Effectiveness Factor
                                        if (optimalWa.WorkActionCode != Code.DoNothing)
                                        {
                                            cefExpression = cefExpression.Replace("COST", optimalWa.Cost.ToString());
                                            cefExpression = cefExpression.Replace("DECKAREA", structure.DeckArea.ToString());

                                            if (!optimalWa.CombinedWorkAction)
                                            {
                                                cefExpression = cefExpression.Replace("EXT", optimalWa.LifeExtension.ToString());
                                                piVarRow.PrimaryCostPerSqFtPerYear = Convert.ToSingle(optimalWa.Cost / structure.DeckArea / optimalWa.LifeExtension);
                                            }
                                            else
                                            {
                                                double totalLifeExtension = optimalWa.CombinedWorkActions.Sum(e => e.LifeExtension);

                                                if (totalLifeExtension > 75)
                                                {
                                                    totalLifeExtension = 75;
                                                }

                                                cefExpression = cefExpression.Replace("EXT", totalLifeExtension.ToString());
                                                piVarRow.PrimaryCostPerSqFtPerYear = Convert.ToSingle(optimalWa.Cost / structure.DeckArea / totalLifeExtension);
                                            }
                                        }
                                        else
                                        {
                                            cefExpression = cefExpression.Replace("COST", "3.2");
                                            cefExpression = cefExpression.Replace("DECKAREA", "1");
                                            cefExpression = cefExpression.Replace("EXT", "1");
                                            piVarRow.PrimaryCostPerSqFtPerYear = 3.2F;
                                        }

                                        /*
                                        rnsRow.CostEffectivenessFactor = Math.Round(Convert.ToDouble(new DataTable().Compute(cefExpression, null)), 3);

                                        if (rnsRow.CostEffectivenessFactor > 1)
                                        {
                                            rnsRow.CostEffectivenessFactor = 1;
                                        }

                                        rnsRow.CostEffectivenessFactor = rnsRow.CostEffectivenessFactor * cefMeasurement.Weight * cefMeasurement.FactorWeight;
                                        rnsRow.PriorityIndex += rnsRow.CostEffectivenessFactor;
                                        */

                                        if (optimalWa.SecondaryWorkActions != null)
                                        {
                                            //var distinctSecondaries = optimalWa.SecondaryWorkActions.OrderBy(x => x.WorkActionDesc).Select(e => e.WorkActionCode.Distinct()).ToList();
                                            var distinctSecondaries =
                                                from sec in optimalWa.SecondaryWorkActions
                                                orderby sec.WorkActionDesc
                                                group sec by sec.WorkActionCode into g
                                                select g.First();

                                            foreach (var distinctSecondary in distinctSecondaries)
                                            {
                                                rnsRow.Incidentals += String.Format("({0}){1}; ", distinctSecondary.WorkActionCode, distinctSecondary.WorkActionDesc);
                                            }

                                            piVarRow.Incidentals = rnsRow.Incidentals;
                                        }

                                        if (debug || showPiFactors)
                                        {
                                            var deck1080Elements = optimalWa.CAI.AllElements.Where(e => e.ElemNum == 1080 && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                    (e.ParentElemNum.ToString().Equals("12")
                                                                                        || e.ParentElemNum.ToString().Equals("16")
                                                                                        || e.ParentElemNum.ToString().Equals("38")
                                                                                        || e.ParentElemNum.ToString().Equals("13")
                                                                                        || e.ParentElemNum.ToString().Equals("8039")
                                                                                        || e.ParentElemNum.ToString().Equals("60")
                                                                                        || e.ParentElemNum.ToString().Equals("65")
                                                                                    )).ToList();
                                            /*
                                            if (deck1080Elements.Count > 0)
                                            {
                                                foreach (var deck1080Element in deck1080Elements)
                                                {
                                                    var parentElem = optimalWa.CAI.AllElements.Where(e => e.ElemNum == deck1080Element.ParentElemNum).First();
                                                    rnsRow.Deck1080 = String.Format(parentElem.ElemNum + ": CS1-{0};CS2-{1};CS3-{2};CS4-{3}\r\n", parentElem.Cs1Quantity, parentElem.Cs2Quantity, parentElem.Cs3Quantity, parentElem.Cs4Quantity);
                                                    rnsRow.Deck1080 += String.Format("1080: CS1-{0};CS2-{1};CS3-{2};CS4-{3}\r\n", deck1080Element.Cs1Quantity, deck1080Element.Cs2Quantity, deck1080Element.Cs3Quantity, deck1080Element.Cs4Quantity);
                                                }
                                            }
                                            else
                                            {
                                                rnsRow.Deck1080 = "N/A";
                                            }
                                            */
                                            var wearingSurfaceelements = optimalWa.CAI.CaiElements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).ToList();

                                            if (wearingSurfaceelements.Count > 0)
                                            {
                                                piVarRow.WearingSurfaceElement = wearingSurfaceelements.First().ElemNum;
                                                piVarRow.WearingSurfaceCs1 = wearingSurfaceelements.First().Cs1Quantity;
                                                piVarRow.WearingSurfaceCs2 = wearingSurfaceelements.First().Cs2Quantity;
                                                piVarRow.WearingSurfaceCs3 = wearingSurfaceelements.First().Cs3Quantity;
                                                piVarRow.WearingSurfaceCs4 = wearingSurfaceelements.First().Cs4Quantity;

                                            }

                                            //piVarRow.Deck1080 = rnsRow.Deck1080;

                                            //rnsRow.DebugInfo = optimalWa.CAI.DebugInfo;
                                        }
                                    }
                                }
                                catch { }

                                try
                                {
                                    StructureWorkAction doNothingOptimal = structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();
                                    if (doNothingOptimal != null && doNothingOptimal.WorkActionCode != Code.DoNothing)
                                    {
                                        rnsRow.DoNothingOptimal = String.Format("({0}){1}", doNothingOptimal.WorkActionCode, doNothingOptimal.WorkActionDesc);
                                        piVarRow.DoNothingOptimal = rnsRow.DoNothingOptimal;
                                        piVarRow.WiSAMSWorkType = doNothingOptimal.WorkActionCode;
                                    }
                                    else
                                    {
                                        piVarRow.WiSAMSWorkType = "DN";
                                    }

                                    piVarRow.ComboWorkType += piVarRow.WiSAMSWorkType;
                                }
                                catch { }

                                try
                                {
                                    StructureWorkAction programmedWorkAction = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (programmedWorkAction != null)
                                    {
                                        if (programmedWorkAction.WorkActionCode != Code.DoNothing)
                                        {

                                            /*
                                            string wisDotProgramDesc = programmedWorkAction.WisDOTProgramCode;

                                            switch (programmedWorkAction.WisDOTProgramCode.Trim())
                                            {
                                                case "42":
                                                    wisDotProgramDesc = "BACKBONE";
                                                    break;
                                                case "7":
                                                    wisDotProgramDesc = "MAJORS";
                                                    break;
                                                case "11":
                                                    wisDotProgramDesc = "SHR BRIDGES";
                                                    break;
                                                case "8":
                                                    wisDotProgramDesc = "STATE 3R";
                                                    break;
                                            }
                                            */

                                            rnsRow.Fiips = String.Format("({0}){1}", programmedWorkAction.WorkActionCode, programmedWorkAction.WorkActionDesc);
                                            rnsRow.FosProjectId = programmedWorkAction.FosProjId;
                                            rnsRow.ProjectConcept = programmedWorkAction.PlanningProjectConceptCode;
                                            rnsRow.DotProgram = programmedWorkAction.WisDOTProgramDesc;
                                            rnsRow.TotalCostWithoutDelivery = programmedWorkAction.TotalCostWithoutDeliveryAmount;
                                            piVarRow.Fiips = rnsRow.Fiips;
                                            piVarRow.RegionWorkType = programmedWorkAction.WorkActionCode;
                                            piVarRow.FosProjectId = rnsRow.FosProjectId;
                                            piVarRow.ProjectConcept = rnsRow.ProjectConcept;
                                            piVarRow.DotProgram = rnsRow.DotProgram;
                                            piVarRow.TotalCostWithoutDelivery = rnsRow.TotalCostWithoutDelivery;
                                            piVarRow.FiipsLifeExtension = Convert.ToInt32(programmedWorkAction.LifeExtension);

                                            if (!programmedWorkAction.CombinedWorkAction)
                                            {
                                                piVarRow.FiipsCostPerSqFtPerYear = Convert.ToSingle(programmedWorkAction.Cost / structure.DeckArea / programmedWorkAction.LifeExtension);
                                            }
                                            else
                                            {
                                                double totalLifeExtension = programmedWorkAction.CombinedWorkActions.Sum(e => e.LifeExtension);

                                                if (totalLifeExtension > 75)
                                                {
                                                    totalLifeExtension = 75;
                                                }

                                                piVarRow.FiipsCostPerSqFtPerYear = Convert.ToSingle(programmedWorkAction.Cost / structure.DeckArea / totalLifeExtension);
                                            }
                                            //rnsRow.Fiips = String.Format("({0}){1}", programmedWorkAction.WorkActionCode, programmedWorkAction.WorkActionDesc);
                                            //rnsRow.FosProjectId = programmedWorkAction.FosProjId.ToString();
                                        }
                                        else
                                        {
                                            piVarRow.RegionWorkType = "DN";
                                            piVarRow.FiipsCostPerSqFtPerYear = 3.2F;
                                        }

                                        piVarRow.ComboWorkType += piVarRow.RegionWorkType;
                                        rnsRow.FiipsCai = Math.Round(programmedWorkAction.CAI.CaiValue, 1);

                                        if (structure.StructureType.Equals("BOX CULVERT") || structure.StructureType.ToUpper().Contains("CULVERT"))
                                        {
                                            rnsRow.FiipsNbiDeck = -1;
                                            rnsRow.FiipsNbiSup = -1;
                                            rnsRow.FiipsNbiSub = -1;

                                            try
                                            {
                                                rnsRow.FiipsNbiCulv = Math.Round(programmedWorkAction.CAI.NbiRatings.CulvertRatingVal, 2);
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                rnsRow.FiipsNbiDeck = Math.Round(programmedWorkAction.CAI.NbiRatings.DeckRatingVal, 2);

                                            }
                                            catch { }

                                            try
                                            {
                                                rnsRow.FiipsNbiSup = Math.Round(programmedWorkAction.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                            }
                                            catch { }

                                            try
                                            {
                                                rnsRow.FiipsNbiSub = Math.Round(programmedWorkAction.CAI.NbiRatings.SubstructureRatingVal, 2);
                                            }
                                            catch { }

                                            rnsRow.FiipsNbiCulv = -1;
                                        }

                                        piVarRow.FiipsCai = rnsRow.FiipsCai;
                                    }
                                }
                                catch { }

                                rnsRows.Add(rnsRow);

                                if (similarComboWorkActions.Where(e => e.Equals(piVarRow.ComboWorkType)).Count() > 0)
                                {
                                    piVarRow.WorkTypeMatch = true;
                                }
                                else
                                {
                                    piVarRow.WorkTypeMatch = false;
                                }

                                piVariableRows.Add(piVarRow);
                            }

                            piStaticRows.Add(piStaticRow);
                        }
                        catch { }
                    }
                    //ws1.Cell(i, j).Style.NumberFormat.Format = "$ #,##0";
                    ws3.Cell(2, 1).InsertData(rnsRows.AsEnumerable());
                    //ws3.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    //ws3.Columns().AdjustToContents(1, 10, 50);
                    //ws3.Columns().Style.Alignment.WrapText = true;
                    ws3.Column("AE").Style.NumberFormat.Format = "$ #,##0";
                    ws3.Column("Z").Style.NumberFormat.Format = "$ #,##0";
                    //ws3.Columns().AdjustToContents();

                    /*
                    if (!showPiFactors)
                    {
                        ws3.Column("AE").Delete();
                        ws3.Column("AE").Delete();
                        ws3.Column("AE").Delete();
                        ws3.Column("AE").Delete();
                        ws3.Column("AE").Delete();
                        ws3.Column("AE").Delete();
                    }
                    */

                    ws1.Name = "SUMMARY";
                    ws2.Name = "DETAILS";

                    // Sheet1: Summary Table
                    string structureSelection = "";

                    if (regionNumber.Equals(""))
                    {
                        structureSelection = "BY INDIVIDUAL STRUCTURE IDs";
                    }
                    else
                    {
                        structureSelection = String.Format("REGION {0}, STATE: {1}, LOCAL: {2}", regionNumber, state, local);
                    }

                    var summaryTitle = "";

                    switch (report)
                    {
                        case WisamType.AnalysisReports.RegionNeeds:
                        case WisamType.AnalysisReports.RegionNeedsNew:
                            summaryTitle = "NEEDS ANALYSIS- OPTIMAL SCENARIO";
                            break;
                        case WisamType.AnalysisReports.StrDeckReplacements:
                            summaryTitle = "NEEDS ANALYSIS- STR & DECK REPLACEMENTS ONLY";
                            break;
                        case WisamType.AnalysisReports.Flexible:
                            summaryTitle = "NEEDS ANALYSIS- FLEXIBLE SCENARIO";
                            break;
                    }
                    ws1.Cell(2, 2).Value = summaryTitle;
                    // ws1.Range(2, 9, 2, 10).Merge();
                    ws1.Cell(3, 2).Value = "STRUCTURE SELECTION: " + structureSelection;
                    //ws1.Cell(3, 10).Value = structureSelection;
                    ws1.Cell(4, 2).Value = "# OF STRUCTURES: " + structures.Count();
                    //ws1.Cell(4, 10).Value = structures.Count();
                    //ws1.Cell(5, 10).Value = DateTime.Now.ToShortDateString();
                    //ws1.Range(2, 1, 5, 1).Style.Font.Bold = true;
                    ws1.Range(2, 2, 5, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    ws1.Range(2, 2, 5, 2).Style.Font.Bold = true;
                    ws1.Range(2, 2, 5, 2).Style.Font.FontSize = 16;
                    ws1.Column(1).AdjustToContents(2, 2, 5, 2);
                    //ws1.Range(2, 1, 5, 1).Style.Alignment.WrapText = true;
                    //ws1Columns = new List<string>() { "Year" };
                    ws1.Cell(8, 1).Value = "YEAR";
                    colCounter = 2;

                    ws1.Cell(7, colCounter).Value = "PROGRAM";
                    ws1.Cell(7, colCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    ws1.Cell(7, colCounter).Style.Alignment.WrapText = true;
                    colCounter++;
                    colCounter++;

                    foreach (var swa in swas)
                    {
                        ws1.Cell(7, colCounter).Value = String.Format("({0}){1}", swa.WorkActionCode, swa.WorkActionDesc);
                        ws1.Cell(7, colCounter).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws1.Cell(7, colCounter).Style.Alignment.WrapText = true;
                        colCounter++;
                        colCounter++;
                    }

                    colCounter = 2;
                    ws1.Cell(8, colCounter).Value = "OCCURRENCES";
                    colCounter++;
                    ws1.Cell(8, colCounter).Value = "TOTAL COST";
                    colCounter++;

                    foreach (var swa in swas)
                    {
                        ws1.Cell(8, colCounter).Value = "OCCURRENCES";
                        colCounter++;
                        ws1.Cell(8, colCounter).Value = "TOTAL COST";
                        colCounter++;
                    }
                    ws1.Columns().AdjustToContents(8);

                    var listOfArray = new List<int[]>();

                    for (int i = startYear; i <= endYear; i++)
                    {
                        if (i >= DateTime.Now.Year)
                        {
                            int programTotalOccurrences = 0;
                            int programTotalCost = 0;
                            int[] arr = new int[swas.Count * 2 + 1 + 2];
                            arr[0] = i;
                            arr[1] = -1;
                            arr[2] = -1;
                            int swaCounter = 2;

                            foreach (var swa in swas)
                            {
                                List<StructureWorkAction> matches = new List<StructureWorkAction>();

                                try
                                {
                                    matches =
                                    (from structure in structures
                                     from workAction in structure.YearlyOptimalWorkActions
                                     where workAction.WorkActionYear == i
                                             && workAction.WorkActionCode.Equals(swa.WorkActionCode)
                                     select workAction)
                                        .ToList();
                                }
                                catch { }

                                swaCounter++;
                                swaCounter++;

                                if (matches.Count > 0)
                                {
                                    arr[swaCounter - 1] = matches.Count;
                                    programTotalOccurrences += matches.Count;
                                    arr[swaCounter] = Convert.ToInt32(matches.Sum(e => e.Cost));
                                    programTotalCost += arr[swaCounter];
                                }
                            }

                            arr[1] = programTotalOccurrences;
                            arr[2] = programTotalCost;
                            listOfArray.Add(arr);
                        }
                    }

                    ws1.Cell(9, 1).InsertData(listOfArray);

                    for (int i = 9; i <= ws1.LastRowUsed().RowNumber(); i++)
                    {
                        for (int j = 3; j <= ws1.LastColumnUsed().ColumnNumber(); j += 2)
                        {
                            ws1.Cell(i, j).Style.NumberFormat.Format = "$ #,##0";
                        }
                    }

                    //SaveWorkbook();

                    //ce.RegisterFunction("DOLLAR", 1, 2, Dollar); // Converts a number to text, using the $ (dollar) currency format

                    // Sheet2: bridge by bridge breakdown
                    int strCounter = 0;
                    int rowNumber = 1;

                    /*
                    foreach (var structure in structures)
                    {
                        List<RegionNeedsRow> rnRows = new List<RegionNeedsRow>();

                        try
                        {
                            foreach (var doNothing in structure.YearlyDoNothings)
                            {
                                if (doNothing.WorkActionYear < startYear)
                                    continue;

                                RegionNeedsRow rnRow = new RegionNeedsRow();
                                rnRow.WorkActionYear = doNothing.WorkActionYear;
                                rnRow.Age = doNothing.WorkActionYear - structure.YearBuiltActual;

                                if (doNothing.CAI != null)
                                {
                                    rnRow.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 1);
                                }

                                try
                                {
                                    StructureWorkAction optimalWa = structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (optimalWa != null)
                                    {
                                        if (optimalWa.WorkActionCode != Code.DoNothing)
                                        {
                                            //rnRow.Primary = String.Format("({0}){1}", optimalWa.WorkActionCode, optimalWa.WorkActionDesc);
                                            rnRow.Primary = String.Format("({0}){1}", optimalWa.WorkActionCode, optimalWa.ControllingCriteria.WorkActionDesc);

                                            if (debug && optimalWa.ControllingCriteria != null)
                                            {
                                                rnRow.Primary += String.Format("; Rule ID {0}", optimalWa.ControllingCriteria.RuleId.ToString());
                                            }
                                        }

                                        rnRow.PrimaryCai = Math.Round(optimalWa.CAI.CaiValue, 1);
                                        rnRow.PrimaryCost = Math.Round(optimalWa.Cost, 0);
                                        rnRow.LifeExtension = Convert.ToInt32(optimalWa.LifeExtension);

                                       

                                        if (optimalWa.SecondaryWorkActions != null)
                                        {
                                            //var distinctSecondaries = optimalWa.SecondaryWorkActions.OrderBy(x => x.WorkActionDesc).Select(e => e.WorkActionCode.Distinct()).ToList();
                                            var distinctSecondaries =
                                                from sec in optimalWa.SecondaryWorkActions
                                                orderby sec.WorkActionDesc
                                                group sec by sec.WorkActionCode into g
                                                select g.First();

                                            foreach (var distinctSecondary in distinctSecondaries)
                                            {
                                                rnRow.Incidentals += String.Format("({0}){1}; ", distinctSecondary.WorkActionCode, distinctSecondary.WorkActionDesc);
                                            }
                                        }
                                    }
                                }
                                catch { }

                                try
                                {
                                    StructureWorkAction programmedWorkAction = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (programmedWorkAction != null)
                                    {
                                        if (programmedWorkAction.WorkActionCode != Code.DoNothing)
                                        {
                                            

                                            rnRow.Fiips = String.Format("({0}){1}", programmedWorkAction.WorkActionCode, programmedWorkAction.WorkActionDesc);
                                            rnRow.FosProjectId = programmedWorkAction.FosProjId;
                                            rnRow.ProjectConcept = programmedWorkAction.PlanningProjectConceptCode;
                                            rnRow.DotProgram = programmedWorkAction.WisDOTProgramDesc;
                                            rnRow.TotalCostWithoutDelivery = programmedWorkAction.TotalCostWithoutDeliveryAmount;
                                        }

                                        rnRow.FiipsCai = Math.Round(programmedWorkAction.CAI.CaiValue, 1);
                                    }
                                    
                                }
                                catch { }

                                rnRows.Add(rnRow);
                            }

                            // Header Rowshttp://wisconsinhomes.com/listing/1751651?o=45#
                            // Header Row 1
                            ws2.Cell(rowNumber, 1).Value = structure.StructureId;
                            ws2.Cell(rowNumber, 1).Style.Font.FontSize = 14;
                            ws2.Range(rowNumber, 1, rowNumber + 1, 1).Merge();

                            string addlInfo = "";

                            if (!String.IsNullOrEmpty(structure.CorridorCode))
                            {
                                //ws2.Cell(rowNumber, 2).Value = String.Format("Corridor: {0}", structure.CorridorCode);
                                addlInfo += String.Format("Corridor: {0}\r\n", structure.CorridorDesc);
                            }
                            else
                            {
                                addlInfo += String.Format("Corridor: {0}\r\n", "Unknown");
                            }

                            /*
                            if (showPiFactors)
                            {
                                addlInfo += String.Format("PI: {0}\r\n", Math.Round(structure.PriorityIndex, 3));

                                foreach (var pf in structure.PriorityFactors)
                                {
                                    addlInfo += String.Format("{0}: {1}\r\n", pf.FactorCode, Math.Round(pf.FactorValue, 3));
                                }
                            }
                            
                            if (debug)
                            {
                                foreach (var pfm in structure.PriorityFactorMeasurements)
                                {
                                    addlInfo += String.Format("{0}: {1}*{2}={3}\r\n", pfm.MeasurementCode, Math.Round(pfm.IndexValue, 3), pfm.Weight, Math.Round(pfm.Weight * pfm.IndexValue, 3));
                                }
                            }

                            ws2.Cell(rowNumber, 2).Value = addlInfo;

                            ws2.Range(rowNumber, 2, rowNumber + 1, 2).Merge();

                            ws2.Cell(rowNumber, 3).Value = "YEAR";
                            ws2.Range(rowNumber, 3, rowNumber + 1, 3).Merge();

                            ws2.Cell(rowNumber, 4).Value = "AGE";
                            ws2.Range(rowNumber, 4, rowNumber + 1, 4).Merge();

                            ws2.Cell(rowNumber, 5).Value = "NO ACTION TAKEN";

                            ws2.Cell(rowNumber, 6).Value = "OPTIMAL IMPROVEMENT SCENARIO";
                            if (report.Equals(WisamType.AnalysisReports.StrDeckReplacements))
                            {
                                ws2.Cell(rowNumber, 6).Value = "STR & DECK REPL SCENARIO";
                            }
                            else if (report.Equals(WisamType.AnalysisReports.Flexible))
                            {
                                ws2.Cell(rowNumber, 6).Value = "FLEXIBLE SCENARIO";
                            }

                            //ws2.Range(rowNumber, 6, rowNumber, 10).Merge();

                            ws2.Cell(rowNumber, 11).Value = "FIIPS PROGRAM";
                            //ws2.Range(rowNumber, 11, rowNumber, 12).Merge();

                            
                            //ws2.Range(rowNumber, 1, rowNumber, 12).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                            //ws2.Range(rowNumber, 1, rowNumber, 12).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            //ws2.Range(rowNumber, 2, rowNumber, 12).Style.Alignment.WrapText = true;

                            // Header Row 2
                            ws2.Cell(rowNumber + 1, 5).Value = "CAI";
                            ws2.Cell(rowNumber + 1, 6).Value = "PRIMARY WORK ACTION";
                            ws2.Cell(rowNumber + 1, 7).Value = "CAI";
                            ws2.Cell(rowNumber + 1, 8).Value = "COST: PRIMARY WORK ACTION";
                            ws2.Cell(rowNumber + 1, 9).Value = "EST. LIFE EXTENSION (YRS)";
                            ws2.Cell(rowNumber + 1, 10).Value = "INCIDENTAL WORK ACTIONS";
                            ws2.Cell(rowNumber + 1, 11).Value = "PROGRAMMED WORK ACTION";
                            ws2.Cell(rowNumber + 1, 12).Value = "CAI";

                            ws2.Cell(rowNumber + 1, 13).Value = "COST(W/O DEL)";

                            ws2.Cell(rowNumber + 1, 14).Value = "FOS PROJ ID";
                            ws2.Cell(rowNumber + 1, 15).Value = "PROJ CONCEPT";
                            ws2.Cell(rowNumber + 1, 16).Value = "DOT PROGRAM";

                            ws2.Range(rowNumber, 1, rowNumber + 1, 16).Style.Font.Bold = true;

                            ws2.Cell(rowNumber + 2, 1).Value = "FEAT ON/UNDER:";
                            ws2.Cell(rowNumber + 2, 2).Value = String.Format("{0} {1}", structure.FeatureOn.ToUpper(), structure.FeatureUnder.Length > 0 ? "over " + structure.FeatureUnder : "");
                            ws2.Cell(rowNumber + 3, 1).Value = "STRUCTURE TYPE:";
                            ws2.Cell(rowNumber + 3, 2).Value = structure.StructureType;
                            ws2.Cell(rowNumber + 4, 1).Value = "MATERIAL:";
                            ws2.Cell(rowNumber + 4, 2).Value = structure.MainSpanMaterial;
                            ws2.Cell(rowNumber + 5, 1).Value = "NUM SPANS:";
                            ws2.Cell(rowNumber + 5, 2).Value = structure.NumSpans;
                            ws2.Cell(rowNumber + 6, 1).Value = "TOT LENGTH (FT):";
                            ws2.Cell(rowNumber + 6, 2).Value = Math.Round(structure.TotalLengthSpans, 2);
                            ws2.Cell(rowNumber + 7, 1).Value = "INVENTORY RATING:";
                            ws2.Cell(rowNumber + 7, 2).Value = structure.InventoryRating;
                            ws2.Cell(rowNumber + 8, 1).Value = "OPERATING RATING:";
                            ws2.Cell(rowNumber + 8, 2).Value = structure.OperatingRating;

                            try
                            {
                                string hsOrRf = structure.OperatingRating.Trim().Substring(0, 2).ToUpper();
                                //int hsRatingValue = ;
                                
                                if ((structure.HsOrRf.Equals("HS") && structure.ratingValue < 25)
                                        || (structure.HsOrRf.Equals("RF") && structure.ratingValue < 1.06))
                                {
                                    //ws2.Cell(rowNumber + 7, 1).Style.Font.FontSize = 16;
                                    //ws2.Cell(rowNumber + 7, 1).Style.Font.Bold = true;
                                    //ws2.Cell(rowNumber + 8, 1).Style.Font.FontColor = XLColor.Blue;
                                    ws2.Cell(rowNumber + 8, 1).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                                    //ws2.Cell(rowNumber + 7, 2).Style.Font.FontSize = 16;
                                    //ws2.Cell(rowNumber + 7, 2).Style.Font.Bold = true;
                                    //ws2.Cell(rowNumber + 8, 2).Style.Font.FontColor = XLColor.Blue;
                                    ws2.Cell(rowNumber + 8, 2).Style.Fill.SetBackgroundColor(XLColor.Yellow);
                                }
                            }
                            catch { }

                            ws2.Cell(rowNumber + 9, 1).Value = "LOAD POSTING:";
                            ws2.Cell(rowNumber + 9, 2).Value = structure.LoadPostingDesc;
                            //ws2.Cell(rowNumber + 9, 2).Value = ;
                            ws2.Cell(rowNumber + 10, 1).Value = "LAST INSPECTION:";
                            ws2.Cell(rowNumber + 10, 2).Value = structure.LastInspection.InspectionDate.ToShortDateString();
                            ws2.Cell(rowNumber + 11, 1).Value = "CONSTR HIST:";
                            ws2.Cell(rowNumber + 11, 2).Value = structure.ConstructionHistory;
                            //ws2.Range(rowNumber + 1, 1, rowNumber + 10, 2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                            //ws2.Range(rowNumber + 1, 1, rowNumber + 10, 2).Style.Alignment.WrapText = true;
                            //ws2.Column(1).AdjustToContents(rowNumber + 1, rowNumber + 10);
                            //ws2.Column(2).AdjustToContents(rowNumber + 1, rowNumber + 10);
                            // Data Rows
                            ws2.Cell(rowNumber + 2, 3).InsertData(rnRows.AsEnumerable());
                            //ws2.Rows().AdjustToContents();
                            //ws2.Columns().AdjustToContents();
                            strCounter++;
                            //rowNumber = strCounter * rnRows.Count + 5;
                            rowNumber = ws2.LastRowUsed().RowNumber() + 3;
                        }
                        catch { }
                    }
                    */


                    //ws1.Cell(5, 2).Value = "ANALYSIS DATE: " + DateTime.Now.ToShortDateString();
                    DateTime endDateTime = DateTime.Now;
                    //ws1.Cell(5, 2).Value = String.Format("ANALYSIS DATE: {0}  START: {1}   END: {2}", endDateTime.ToShortDateString(), startTime.ToShortTimeString(), endDateTime.ToShortTimeString());
                    ws1.Cell(5, 2).Value = String.Format("ANALYSIS DATE: {0}", endDateTime.ToShortDateString());

                    /*
                    if (structures.Count > 0)
                    {
                        ws2.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws2.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        //ws2.Range(1, 3, 2, 21).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws2.Columns().AdjustToContents();
                        ws2.Column("B").Width = 20;
                        ws2.Column("B").Style.Alignment.WrapText = true;
                        ws2.Column("E").Width = 10;
                        ws2.Column("F").Width = 15;
                        ws2.Column("F").Style.Alignment.WrapText = true;
                        ws2.Column("H").Width = 10;
                        ws2.Column("H").Style.Alignment.WrapText = true;
                        ws2.Column("H").Style.NumberFormat.Format = "$ #,##0";
                        ws2.Column("I").Width = 10;
                        ws2.Column("I").Style.Alignment.WrapText = true;
                        ws2.Column("J").Width = 20;
                        ws2.Column("J").Style.Alignment.WrapText = true;
                        ws2.Column("K").Width = 20;
                        ws2.Column("K").Style.Alignment.WrapText = true;
                        ws2.Column("M").Style.NumberFormat.Format = "$ #,##0";
                    }
                    */
                    //SaveWorkbook();

                    ws4.Name = "MISSING DATA";
                    int missingCounter = 0;
                    foreach (string strId in notFoundIds)
                    {
                        missingCounter++;
                        ws4.Cell(missingCounter, 1).Value = strId;
                    }

                    if (showPiFactors)
                    {
                        ws5.Name = "Priority Index - Static Data";
                        List<string> ws5Columns = new List<string>()
                        {
                            "Str", "2030 Corr", "Region", "County", "Feat On", "Feat Und",
                            "Str Type", "Matl", "Num Spans", "Tot Len(ft)", "Inv Rating",
                            "Opr Rating", "Load Posting", "Last Insp", "Const Hist",
                            "Complex?", "Large?", "Scour Crit?", "Frac Crit",
                            "Detour Len", "Func Class On", "Func Class Under",
                            "Muni", "Muni No", "MVW", "Load Posting Tonnage",
                            "VC Under", "Damage Inspections Count", "NHS", "Local System On", "Local System Under",
                            "ADT", "ADT Truck %", "Lanes On", "Deck Width", "Deck Area",
                            "Border Bridge", "Border State", "MPA Bridge", "MPA", "PHFS",
                            "State Coords within 500 Ft HSIS", "Local Coords within 500 Ft HSIS",
                            "C2030", "Divided Hwy Code", "Func Class Code", "Func Class Abbr", "Func Class Desc",
                            "Nhs Desig", "Proj Rte Type", "Proj Rte Name", "Truck Rte Desig",
                            "Juris Code", "Juris Rte Type", "Juris Rte Name",
                            "Osow High Clr Rte?", "Osow Rte Type", "Osow Rte Name", "Osow Rank No", "Osow Rank Name",
                            "Rdwy Seg Id", "ADT 1", "ADT 5", "ADT 10", "ADT 15", "ADT 20", "Trk ADT 1", "Posted Speed Limit",
                            "Meta C2030", "Meta Func Class On", "Meta Divided Hwy Code", "Traf Seg Id"
                        };

                        colCounter = 1;
                        foreach (var ws5Column in ws5Columns)
                        {
                            ws5.Cell(1, colCounter).Value = ws5Column;
                            colCounter++;
                        }

                        ws5.Cell(2, 1).InsertData(piStaticRows.AsEnumerable());
                        //ws5.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        //ws5.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        ws6.Name = "Priority Index - Variable Data";
                        List<string> ws6Columns = new List<string>()
                        {
                            "Str", "Year", "Str Age", "Deck Age", "DN CAI",
                            "Primary WA", "Primary CAI", "Primary Cost", "Life Ext (yrs)", "Incidental", "Primary Cost/SF/YR",

                            "FIIPS WA", "FIIPS CAI", "FIIPS Cost (w/o Del)", "FIIPS Proj ID",
                            "FIIPS Proj Concept", "FIIPS Dot Prog", "FIIPS Life Ext (yrs)", "FIIPS Cost/SF/YR",

                            "WiSAMS Work Type", "Region Work Type", "Combo Work Type", "Work Type Match?",

                            "DN Primary WA",
                            "NBI Inspected",
                            "NBI Deck", "NBI Sup", "NBI Sub", "NBI Culv",
                            "WS Elem", "CS1", "CS2", "CS3", "CS4",
                            "Deck 1080"
                        };

                        colCounter = 1;
                        foreach (var ws6Column in ws6Columns)
                        {
                            ws6.Cell(1, colCounter).Value = ws6Column;
                            colCounter++;
                        }

                        ws6.Cell(2, 1).InsertData(piVariableRows.AsEnumerable());
                        //ws6.RangeUsed().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        //ws6.RangeUsed().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    }

                    //SaveWorkbook();
                    break;

                // END case WisamType.AnalysisReports.RegionNeeds:
                #endregion RegionNeeds, StrDeckReplacements, Flexible

                #region StateNeedsPmdss
                case WisamType.AnalysisReports.StateNeedsPmdss:
                    ws1Columns = new List<string>() { "Structure", "Year", "Age",
                                                        "Deck", "Super", "Sub", "Culv", "CAI", "Yearly Optimal WA", "CAI-Reset",
                                                        "Optimal WA", "Deck", "Super", "Sub", "Culv", "CAI", "Estimated Project Cost" };

                    foreach (var ws1Column in ws1Columns)
                    {
                        ws1.Cell(1, colCounter).Value = ws1Column;
                        colCounter++;
                    }

                    List<StateNeedsPmdssRow> snpRows = new List<StateNeedsPmdssRow>();

                    foreach (var structure in structures)
                    {
                        try
                        {
                            foreach (var doNothing in structure.YearlyDoNothings)
                            {
                                if (doNothing.WorkActionYear == DateTime.Now.Year)
                                    continue;

                                StateNeedsPmdssRow snpRow = new StateNeedsPmdssRow();
                                snpRow.StructureId = structure.StructureId;
                                snpRow.WorkActionYear = doNothing.WorkActionYear;
                                snpRow.Age = snpRow.WorkActionYear - structure.YearBuiltActual;
                                snpRow.DoNothingOptimalWorkCandidate = "";

                                if (doNothing.CAI != null)
                                {
                                    snpRow.DoNothingNbiDeck = Math.Round(doNothing.CAI.NbiRatings.DeckRatingVal, 2);
                                    snpRow.DoNothingNbiSup = Math.Round(doNothing.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                    snpRow.DoNothingNbiSub = Math.Round(doNothing.CAI.NbiRatings.SubstructureRatingVal, 2);
                                    snpRow.DoNothingNbiCulv = Math.Round(doNothing.CAI.NbiRatings.CulvertRatingVal, 2);
                                    snpRow.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 2);
                                }

                                try
                                {
                                    StructureWorkAction doNothingOptimalWa = structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (doNothingOptimalWa.WorkActionCode != Code.DoNothing)
                                    {
                                        snpRow.DoNothingOptimalWorkCandidate = String.Format("({0}) {1}", doNothingOptimalWa.WorkActionCode, doNothingOptimalWa.WorkActionDesc);
                                        snpRow.DoNothingOptimalWorkCandidateCaiValue = Math.Round(doNothingOptimalWa.CAI.CaiValue, 2).ToString();
                                    }
                                }
                                catch { }

                                try
                                {
                                    StructureWorkAction optimalWa = structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                    if (optimalWa != null)
                                    {
                                        if (optimalWa.WorkActionCode != Code.DoNothing)
                                        {
                                            snpRow.OptimalWorkCandidate = String.Format("({0}) {1}", optimalWa.WorkActionCode, optimalWa.WorkActionDesc);
                                        }

                                        snpRow.OptimalNbiDeck = Math.Round(optimalWa.CAI.NbiRatings.DeckRatingVal, 2);
                                        snpRow.OptimalNbiSup = Math.Round(optimalWa.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                        snpRow.OptimalNbiSub = Math.Round(optimalWa.CAI.NbiRatings.SubstructureRatingVal, 2);
                                        snpRow.OptimalNbiCulv = Math.Round(optimalWa.CAI.NbiRatings.CulvertRatingVal, 2);
                                        snpRow.OptimalWorkCandidateCaiValue = Math.Round(optimalWa.CAI.CaiValue, 2);
                                        snpRow.EstimatedProjectCost = Math.Round(optimalWa.Cost, 0);
                                    }
                                }
                                catch { }

                                snpRows.Add(snpRow);
                            }
                        }
                        catch { }
                    }

                    ws1.Cell(2, 1).InsertData(snpRows.AsEnumerable());
                    break;
                #endregion StateNeedsPmdss

                case WisamType.AnalysisReports.AnalysisDebug:
                    ws1.Name = "Report";
                    ws2.Name = "General Debug";
                    ws3.Name = "Do-Nothing CAI Debug";
                    ws4.Name = "Performed CAI Debug";
                    ws5.Name = "Structures Not Found in HSI";

                    ws1Columns = new List<string>()
                        {
                            "Structure", "Structure Type", "Feat On", "Feat Under", "County", "Region", "Year", "Age", "Priority Score",
                            "DN CAI-Deteriorated", "DN Yearly Optimal", "DN Yearly Optimal CAI-Reset", "DN Yearly Optimal Cost",
                            "FIIPS (Actual)", "FIIPS CAI-Reset", "Performed Primary", "Performed Primary Cost",
                            "Performed Secondary", "Performed Secondary Cost",
                            "Performed CAI-Deteriorated",
                            "FIIPS (Reported)", "FIIPS (Reported) Cost", "Performed Criteria", "Elig Prog Incidentals", "Elig Non-Prog Incidentals"
                        };

                    foreach (var ws1Column in ws1Columns)
                    {
                        ws1.Cell(1, colCounter).Value = ws1Column;
                        colCounter++;
                    }

                    ws2.Cell(1, 1).Value = "Structure ID";
                    ws2.Cell(1, 2).Value = "Structure Type";
                    ws2.Cell(1, 3).Value = "Year";
                    ws2.Cell(1, 4).Value = "Do-Nothing CAI";
                    ws2.Cell(1, 5).Value = "Do-Nothing Quantities";
                    ws2.Cell(1, 6).Value = "Performed Primary";
                    ws2.Cell(1, 7).Value = "Performed Secondary";
                    ws2.Cell(1, 8).Value = "Performed CAI";
                    ws2.Cell(1, 9).Value = "FIIPS (Reported)";
                    ws2.Cell(1, 10).Value = "Criteria";
                    ws2.Cell(1, 11).Value = "Performed Benefits";
                    ws2.Cell(1, 12).Value = "Performed Quantities";
                    ws2.Cell(1, 13).Value = "Elig Prog Incidentals";
                    ws2.Cell(1, 14).Value = "Elig Non-Prog Incidentals";
                    ws2.Cell(1, 15).Value = "All Evaluated Incidentals";

                    caiColumns = new List<string>()
                        {
                            "Structure", "Year", "Age", "CAI", "Deck", "Sup", "Sub", "Culv", "Quantities"
                        };

                    colCounter = 1;
                    foreach (var caiColumn in caiColumns)
                    {
                        ws3.Cell(1, colCounter).Value = caiColumn;
                        colCounter++;
                    }

                    colCounter = 1;
                    foreach (var caiColumn in caiColumns)
                    {
                        ws4.Cell(1, colCounter).Value = caiColumn;
                        colCounter++;
                    }

                    ws5.Cell(1, 1).Value = "Structure";

                    List<DebugRowSheet1> deRowsSheet1 = new List<DebugRowSheet1>();
                    List<DebugRowSheet2> deRowsSheet2 = new List<DebugRowSheet2>();
                    List<CaiRow> rowsSheet3 = new List<CaiRow>();
                    List<CaiRow> rowsSheet4 = new List<CaiRow>();
                    List<MissingStructureRow> rowsSheet5 = new List<MissingStructureRow>();

                    foreach (var structure in structures)
                    {
                        foreach (var optimalWorkAction in structure.YearlyOptimalWorkActions)
                        {
                            //if (optimalWorkAction.WorkActionYear == DateTime.Now.Year)
                            //continue;

                            DebugRowSheet1 deRowSheet1 = new DebugRowSheet1();
                            DebugRowSheet2 deRowSheet2 = new DebugRowSheet2();
                            CaiRow rowSheet3 = new CaiRow();
                            CaiRow rowSheet4 = new CaiRow();

                            deRowSheet1.StructureId = structure.StructureId;
                            deRowSheet1.StructureType = structure.MainSpanMaterial + " " + structure.StructureType;
                            deRowSheet1.FeatureOn = structure.FeatureOn;
                            deRowSheet1.FeatureUnder = structure.FeatureUnder;
                            deRowSheet1.County = String.Format("{0} - {1}", structure.CountyNumber, structure.County);
                            deRowSheet1.Region = Convert.ToInt32(structure.RegionNumber);
                            deRowSheet1.WorkActionYear = optimalWorkAction.WorkActionYear;
                            deRowSheet1.Age = optimalWorkAction.WorkActionYear - structure.YearBuiltActual;
                            deRowSheet1.PriorityScore = structure.PriorityScore;

                            deRowSheet2.StructureId = structure.StructureId;
                            deRowSheet2.StructureType = structure.MainSpanMaterial + " " + structure.StructureType;
                            deRowSheet2.WorkActionYear = optimalWorkAction.WorkActionYear;

                            rowSheet3.StructureId = structure.StructureId;
                            rowSheet3.WorkActionYear = optimalWorkAction.WorkActionYear;
                            rowSheet3.Age = optimalWorkAction.WorkActionYear - structure.YearBuiltActual;
                            rowSheet4.StructureId = structure.StructureId;
                            rowSheet4.WorkActionYear = optimalWorkAction.WorkActionYear;
                            rowSheet4.Age = optimalWorkAction.WorkActionYear - structure.YearBuiltActual;

                            try
                            {
                                StructureWorkAction doNothing = structure.YearlyDoNothings.Where(e => e.WorkActionYear == optimalWorkAction.WorkActionYear).First();
                                deRowSheet1.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 2);
                                deRowSheet2.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 2);
                                deRowSheet2.DoNothingQuantities = String.Format("{0}", doNothing.CAI.DebugInfo);

                                rowSheet3.CaiValue = Math.Round(doNothing.CAI.CaiValue, 2);
                                rowSheet3.NbiDeck = doNothing.CAI.NbiRatings.DeckRating;
                                rowSheet3.NbiSup = doNothing.CAI.NbiRatings.SuperstructureRating;
                                rowSheet3.NbiSub = doNothing.CAI.NbiRatings.SubstructureRating;
                                rowSheet3.NbiCulv = doNothing.CAI.NbiRatings.CulvertRating;
                                rowSheet3.Quantities = String.Format("{0}", doNothing.CAI.DebugInfo);
                            }
                            catch { }

                            try
                            {
                                StructureWorkAction doNothingOptimal = structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition.Where(e => e.WorkActionYear == optimalWorkAction.WorkActionYear).First();
                                if (doNothingOptimal.WorkActionCode != Code.DoNothing)
                                {
                                    deRowSheet1.DoNothingProgrammableWorkCandidate = String.Format("({0}) {1}", doNothingOptimal.WorkActionCode, doNothingOptimal.WorkActionDesc);

                                    if (!String.IsNullOrEmpty(doNothingOptimal.AlternativeWorkActionCode))
                                    {
                                        deRowSheet1.DoNothingProgrammableWorkCandidate += String.Format("  (OR ({0}) {1})", optimalWorkAction.AlternativeWorkActionCode, optimalWorkAction.AlternativeWorkActionDesc);
                                    }
                                }
                                deRowSheet1.DoNothingProgrammableWorkCandidateCaiValue = Math.Round(doNothingOptimal.CAI.CaiValue, 2);
                                deRowSheet1.DoNothingProgrammableWorkCandidateCost = Math.Round(doNothingOptimal.Cost, 0);
                            }
                            catch { }

                            try
                            {
                                StructureWorkAction fiipsWA = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == optimalWorkAction.WorkActionYear).First();
                                if (fiipsWA.WorkActionCode != Code.DoNothing)
                                {
                                    deRowSheet1.ImprovementType = String.Format("({0}) {1}", fiipsWA.WorkActionCode, fiipsWA.WorkActionDesc);
                                }
                                deRowSheet1.ImprovementTypeCaiValue = Math.Round(fiipsWA.CAI.CaiValue, 2);
                            }
                            catch { }

                            if (optimalWorkAction.CAI != null)
                            {
                                string performedIncidentalsInfo = "";
                                string eligibleSecondaryWorkActionsImprovementInfo = "";
                                string eligibleSeSecondaryWorkActionsOthersInfo = "";
                                string nonCriteriadSecondaryWorkActionsInfo = "";

                                //deRowSheet2.CaiBasis = optimalWorkAction.CAI.Basis.ToString();
                                //deRowSheet1.CaiBasis = optimalWorkAction.CAI.Basis.ToString();

                                if (optimalWorkAction.CombinedWorkActions.Count == 0)
                                {
                                    if (!optimalWorkAction.WorkActionCode.Equals(Code.DoNothing))
                                    {
                                        deRowSheet1.PerformedPrimary = String.Format("({0}) {1}", optimalWorkAction.WorkActionCode, optimalWorkAction.WorkActionDesc);
                                        deRowSheet1.PerformedPrimaryCost = Math.Round(optimalWorkAction.Cost, 2);

                                        if (!String.IsNullOrEmpty(optimalWorkAction.AlternativeWorkActionCode))
                                        {
                                            deRowSheet1.PerformedPrimary += String.Format("  (OR ({0}) {1})", optimalWorkAction.AlternativeWorkActionCode, optimalWorkAction.AlternativeWorkActionDesc);
                                        }
                                        deRowSheet1.ProgrammableWorkCandidate = deRowSheet1.PerformedPrimary;
                                        deRowSheet1.ProgrammableWorkCandidateCost = deRowSheet1.PerformedPrimaryCost;
                                        deRowSheet2.PrimaryWorkCandidate = deRowSheet1.ProgrammableWorkCandidate;
                                        deRowSheet2.ProgrammableWorkCandidate = deRowSheet1.ProgrammableWorkCandidate;
                                    }
                                }
                                else
                                {
                                    double programmableWorkCost = 0;
                                    double performedIncidentalsCost = 0;
                                    deRowSheet1.PerformedPrimary = String.Format("({0}) {1}", optimalWorkAction.CombinedWorkActions[0].WorkActionCode, optimalWorkAction.CombinedWorkActions[0].WorkActionDesc);
                                    deRowSheet1.PerformedPrimaryCost = Math.Round(optimalWorkAction.CombinedWorkActions[0].Cost, 0);
                                    programmableWorkCost += optimalWorkAction.CombinedWorkActions[0].Cost;
                                    deRowSheet1.ProgrammableWorkCandidate = String.Format("({0}) {1}", optimalWorkAction.WorkActionCode, optimalWorkAction.WorkActionDesc);
                                    deRowSheet2.PrimaryWorkCandidate = deRowSheet1.PerformedPrimary;
                                    deRowSheet2.ProgrammableWorkCandidate = deRowSheet1.ProgrammableWorkCandidate;

                                    int counter = 0;
                                    foreach (var cwa in optimalWorkAction.CombinedWorkActions)
                                    {
                                        if (counter > 0)
                                        {
                                            performedIncidentalsInfo += String.Format("({0}) {1}\r\n", cwa.WorkActionCode, cwa.WorkActionDesc);
                                            performedIncidentalsCost += cwa.Cost;
                                            programmableWorkCost += cwa.Cost;
                                        }
                                        counter++;
                                    }

                                    deRowSheet1.PerformedIncidentalsCost = Math.Round(performedIncidentalsCost, 2);
                                    deRowSheet1.ProgrammableWorkCandidateCost = Math.Round(programmableWorkCost, 2);
                                }

                                if (optimalWorkAction.WorkActionCode != Code.DoNothing)
                                {
                                    try
                                    {
                                        deRowSheet1.PerformedCriteria = String.Format("Rule ID: {0}, Rule Seq: {1}\r\n{2}", optimalWorkAction.ControllingCriteria.RuleId, optimalWorkAction.ControllingCriteria.RuleSequence, optimalWorkAction.ControllingCriteria.RuleFormula);
                                        deRowSheet2.ProgrammableWorkCriteria = String.Format("Rule ID: {0}, Rule Seq: {1}\r\n{2}", optimalWorkAction.ControllingCriteria.RuleId, optimalWorkAction.ControllingCriteria.RuleSequence, optimalWorkAction.ControllingCriteria.RuleFormula);

                                    }
                                    catch { }
                                }

                                deRowSheet2.ProgrammableWorkBenefits = optimalWorkAction.ConditionBenefit;
                                deRowSheet2.ProgrammableWorkCandidateCaiValue = Math.Round(optimalWorkAction.CAI.CaiValue, 2);
                                deRowSheet2.ProgrammableWorkQuantities = String.Format("{0}", optimalWorkAction.CAI.DebugInfo);
                                deRowSheet1.PerformedCaiValue = Math.Round(optimalWorkAction.CAI.CaiValue, 2);

                                if (optimalWorkAction.SecondaryWorkActions != null)
                                {
                                    foreach (var secondaryWa in optimalWorkAction.SecondaryWorkActions.Where(e => e.Improvement == true && (e.BypassCriteria == true || e.ControllingCriteria != null)))
                                    {
                                        eligibleSecondaryWorkActionsImprovementInfo += String.Format("({0}) {1}\r\n", secondaryWa.WorkActionCode, secondaryWa.WorkActionDesc);
                                    }

                                    foreach (var secondaryWa in optimalWorkAction.SecondaryWorkActions.Where(e => e.Improvement == false && (e.BypassCriteria == true || e.ControllingCriteria != null)))
                                    {
                                        eligibleSeSecondaryWorkActionsOthersInfo += String.Format("({0}) {1}\r\n", secondaryWa.WorkActionCode, secondaryWa.WorkActionDesc);
                                    }

                                    foreach (var secondaryWa in optimalWorkAction.SecondaryWorkActions.Where(e => e.ControllingCriteria == null))
                                    {
                                        nonCriteriadSecondaryWorkActionsInfo += String.Format("({0}) {1}\r\n", secondaryWa.WorkActionCode, secondaryWa.WorkActionDesc);
                                    }
                                }

                                deRowSheet1.PerformedIncidentals = performedIncidentalsInfo;
                                deRowSheet1.EligibleProgrammableIncidentalWorkCandidates = eligibleSecondaryWorkActionsImprovementInfo;
                                deRowSheet1.EligibleNonProgrammableIncidentalWorkCandidates = eligibleSeSecondaryWorkActionsOthersInfo;
                                deRowSheet2.PerformedIncidentals = performedIncidentalsInfo;
                                deRowSheet2.EligibleProgrammableIncidentalWorkCandidates = eligibleSecondaryWorkActionsImprovementInfo;
                                deRowSheet2.EligibleNonProgrammableIncidentalWorkCandidates = eligibleSeSecondaryWorkActionsOthersInfo;
                                deRowSheet2.AllEvaluatedIncidentalWorkCandidates += eligibleSecondaryWorkActionsImprovementInfo;

                                if (optimalWorkAction.AllSecondaryWorkActions != null)
                                {
                                    foreach (var secondaryWa in optimalWorkAction.AllSecondaryWorkActions)
                                    {
                                        deRowSheet2.AllEvaluatedIncidentalWorkCandidates += String.Format("({0}) {1}\r\n", secondaryWa.WorkActionCode, secondaryWa.WorkActionDesc);
                                    }
                                }

                                rowSheet4.CaiValue = Math.Round(optimalWorkAction.CAI.CaiValue, 2);
                                rowSheet4.NbiDeck = optimalWorkAction.CAI.NbiRatings.DeckRating;
                                rowSheet4.NbiSup = optimalWorkAction.CAI.NbiRatings.SuperstructureRating;
                                rowSheet4.NbiSub = optimalWorkAction.CAI.NbiRatings.SubstructureRating;
                                rowSheet4.NbiCulv = optimalWorkAction.CAI.NbiRatings.CulvertRating;
                                rowSheet4.Quantities = String.Format("{0}", optimalWorkAction.CAI.DebugInfo);
                            }

                            deRowsSheet1.Add(deRowSheet1);
                            deRowsSheet2.Add(deRowSheet2);
                            rowsSheet3.Add(rowSheet3);
                            rowsSheet4.Add(rowSheet4);
                        }
                    }

                    ws1.Cell(2, 1).InsertData(deRowsSheet1.AsEnumerable());
                    ws2.Cell(2, 1).InsertData(deRowsSheet2.AsEnumerable());
                    ws3.Cell(2, 1).InsertData(rowsSheet3.AsEnumerable());
                    ws4.Cell(2, 1).InsertData(rowsSheet4.AsEnumerable());

                    foreach (var strId in notFoundIds)
                    {
                        MissingStructureRow missing = new MissingStructureRow();
                        missing.StructureId = strId;
                        rowsSheet5.Add(missing);
                    }
                    ws5.Cell(2, 1).InsertData(rowsSheet5.AsEnumerable());

                    break;

                case WisamType.AnalysisReports.StateFiips:
                    ws1.Cell(1, 1).Value = "Structure";
                    ws1.Cell(1, 2).Value = "Year";
                    ws1.Cell(1, 3).Value = "FIIPS Programmed Work Action";
                    ws1.Cell(1, 4).Value = "CAI-Deteriorated";

                    List<StateFiipsRow> sfRows = new List<StateFiipsRow>();

                    foreach (var structure in structures)
                    {
                        foreach (var programmedWorkAction in structure.YearlyProgrammedWorkActions)
                        {
                            if (programmedWorkAction.WorkActionYear == DateTime.Now.Year)
                                continue;


                            StateFiipsRow sfRow = new StateFiipsRow();
                            sfRow.StructureId = structure.StructureId;
                            sfRow.WorkActionYear = programmedWorkAction.WorkActionYear;
                            sfRow.ProgrammedWorkCandidate = "";
                            //sfRow.ProgrammedWorkCandidateCaiValue = "";

                            if (programmedWorkAction.CAI != null)
                            {
                                if (programmedWorkAction.WorkActionCode != Code.DoNothing)
                                {
                                    sfRow.ProgrammedWorkCandidate = String.Format("({0}) {1}", programmedWorkAction.WorkActionCode, programmedWorkAction.WorkActionDesc);
                                }

                                sfRow.ProgrammedWorkCandidateCaiValue = Math.Round(programmedWorkAction.CAI.CaiValue, 2);
                            }

                            sfRows.Add(sfRow);
                        }
                    }

                    ws1.Cell(2, 1).InsertData(sfRows.AsEnumerable());
                    break;

                case WisamType.AnalysisReports.AllCurrentNeeds:
                    ws1.Cell(1, 1).Value = "Structure ID";
                    ws1.Cell(1, 2).Value = "Structure Type";
                    ws1.Cell(1, 3).Value = "Year Last Insp";
                    ws1.Cell(1, 4).Value = "CAI";
                    ws1.Cell(1, 5).Value = "Criteria Met";

                    List<AllCurrentNeedsRow> needsRows = new List<AllCurrentNeedsRow>();

                    foreach (var structure in structures)
                    {
                        AllCurrentNeedsRow needsRow = new AllCurrentNeedsRow();
                        needsRow.StructureId = structure.StructureId;
                        needsRow.StructureType = structure.MainSpanMaterial + " " + structure.StructureType;
                        needsRow.LastInspectionYear = structure.CurrentCai.Year.ToString();
                        needsRow.CurrentCai = Math.Round(structure.CurrentCai.CaiValue, 2).ToString();
                        string needsInfo = "";
                        int needsCounter = 1;

                        foreach (var need in structure.AllCurrentNeeds)
                        {
                            needsInfo += String.Format("{0}. ({1}) {2}\r\n", needsCounter, need.WorkActionCode, need.WorkActionDesc);
                            needsInfo += String.Format("\tRule ID:{0}, Seq:{1}\r\n\r\n", need.ControllingCriteria.RuleId, need.ControllingCriteria.RuleSequence);
                            needsCounter++;
                        }

                        needsRow.CurrentNeeds = needsInfo;
                        needsRows.Add(needsRow);
                    }

                    ws1.Cell(2, 1).InsertData(needsRows.AsEnumerable());
                    break;

                case WisamType.AnalysisReports.StatePmdss:
                    ws1Columns = new List<string>()
                        { "Structure", "Year", "Do-Nothing CAI-Deteriorated", "Yearly Optimal Work Action", "CAI-Reset" };

                    foreach (var ws1Column in ws1Columns)
                    {
                        ws1.Cell(1, colCounter).Value = ws1Column;
                        colCounter++;
                    }

                    List<StatePmdssRow> spRows = new List<StatePmdssRow>();

                    foreach (var structure in structures)
                    {
                        foreach (var doNothing in structure.YearlyDoNothings)
                        {
                            StatePmdssRow spRow = new StatePmdssRow();
                            spRow.StructureId = structure.StructureId;
                            spRow.WorkActionYear = doNothing.WorkActionYear;
                            //spRow.DoNothingCaiValue = "";
                            spRow.OptimalWorkCandidate = "";
                            spRow.OptimalWorkCandidateCaiValue = "";

                            if (doNothing.CAI != null)
                            {
                                spRow.DoNothingCaiValue = Math.Round(doNothing.CAI.CaiValue, 2);
                                StructureWorkAction optimalWa = structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition.Where(e => e.WorkActionYear == doNothing.WorkActionYear).First();

                                if (optimalWa.WorkActionCode != Code.DoNothing)
                                {
                                    spRow.OptimalWorkCandidate = String.Format("({0}) {1}", optimalWa.WorkActionCode, optimalWa.WorkActionDesc);
                                    spRow.OptimalWorkCandidateCaiValue = Math.Round(optimalWa.CAI.CaiValue, 2).ToString();
                                }
                            }

                            spRows.Add(spRow);
                        }
                    }

                    ws1.Cell(2, 1).InsertData(spRows.AsEnumerable());
                    break;

                case WisamType.AnalysisReports.StateNeeds:
                    ws1Columns = new List<string>()
                        { "Structure", "Year", "Performed Optimal Work Action", "CAI-Deteriorated", "Estimated Project Cost" };

                    foreach (var ws1Column in ws1Columns)
                    {
                        ws1.Cell(1, colCounter).Value = ws1Column;
                        colCounter++;
                    }

                    List<StateNeedsRow> snRows = new List<StateNeedsRow>();

                    foreach (var structure in structures)
                    {
                        foreach (var optimalWorkAction in structure.YearlyOptimalWorkActions)
                        {
                            StateNeedsRow snRow = new StateNeedsRow();
                            snRow.StructureId = structure.StructureId;
                            snRow.WorkActionYear = optimalWorkAction.WorkActionYear;
                            snRow.OptimalWorkCandidate = "";
                            //snRow.OptimalWorkCandidateCaiValue = "";
                            //snRow.EstimatedProjectCost = "";

                            if (optimalWorkAction.CAI != null)
                            {
                                snRow.OptimalWorkCandidate = String.Format("({0}) {1}", optimalWorkAction.WorkActionCode, optimalWorkAction.WorkActionDesc);
                                snRow.OptimalWorkCandidateCaiValue = Math.Round(optimalWorkAction.CAI.CaiValue, 2);
                                snRow.EstimatedProjectCost = Math.Round(optimalWorkAction.Cost, 0);
                            }

                            snRows.Add(snRow);
                        }
                    }

                    ws1.Cell(2, 1).InsertData(snRows.AsEnumerable());
                    break;

            }
        
        }

        public void WriteRulesReport(List<WorkActionRule> workActionRules, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            ws1.Cell(1, 1).Value = "Rule Seq";
            ws1.Cell(1, 2).Value = "Rule ID";
            ws1.Cell(1, 3).Value = "Rule Cat";
            ws1.Cell(1, 4).Value = "Criteria";
            ws1.Cell(1, 5).Value = "Main Work (MW)";
            ws1.Cell(1, 6).Value = "MW Consists Of";
            ws1.Cell(1, 7).Value = "Alt MW";
            ws1.Cell(1, 8).Value = "Poten Works to Combine";

            List<WorkActionRuleRow> ruleRows = new List<WorkActionRuleRow>();

            foreach (var workActionRule in workActionRules)
            {
                WorkActionRuleRow ruleRow = new WorkActionRuleRow();
                ruleRow.RuleSequence = workActionRule.RuleSequence.ToString();
                ruleRow.RuleId = workActionRule.RuleId.ToString();
                ruleRow.RuleCategory = workActionRule.RuleCategory;
                ruleRow.RuleFormula = workActionRule.RuleFormula;
                ruleRow.ResultingWorkAction = String.Format("({0}) {1}", workActionRule.ResultingWorkAction.WorkActionCode, workActionRule.ResultingWorkAction.WorkActionDesc);
                string comprisedWorkActionsInfo = "";
                string altWorkActionsInfo = "";
                string potentialWorkActionsToCombineInfo = "";

                foreach (var w in workActionRule.ComprisedWorkActions)
                {
                    comprisedWorkActionsInfo += String.Format("({0}) {1}\r\n", w.WorkActionCode, w.WorkActionDesc);
                }
                ruleRow.ComprisedWorkActions = comprisedWorkActionsInfo;

                foreach (var w in workActionRule.AlternativeWorkActions)
                {
                    altWorkActionsInfo += String.Format("({0}) {1}\r\n", w.WorkActionCode, w.WorkActionDesc);
                }
                ruleRow.AlternativeWorkActions = altWorkActionsInfo;

                foreach (var w in workActionRule.PotentialCombinedWorkActions)
                {
                    if (!ruleRow.RuleFormula.ToUpper().Contains("NCUL"))
                    {
                        potentialWorkActionsToCombineInfo += String.Format("({0}) {1}\r\n", w.WorkActionCode, w.WorkActionDesc);
                    }
                    else
                    {
                        if (!w.WorkActionDesc.ToUpper().Contains("RAISE"))
                        {
                            potentialWorkActionsToCombineInfo += String.Format("({0}) {1}\r\n", w.WorkActionCode, w.WorkActionDesc);
                        }
                    }
                }
                ruleRow.PotentialCombinedWorkActions = potentialWorkActionsToCombineInfo;


                ruleRows.Add(ruleRow);
            }

            ws1.Cell(2, 1).InsertData(ruleRows.AsEnumerable());
        }

        public void WriteStructureProgramReport(List<ProgrammedWorkAction> pwas, int startFY, int endFY, List<StructureWorkAction> workTypes, List<string> lifecycleStages, XLWorkbook workBook)
        {
            List<string> regions = new List<string>()
                {
                    "SW", "SE", "NE", "NC", "NW"
                };
            var ws1 = workBook.Worksheet(1);
            ws1.Name = "Summary by Work Type";
            var ws2 = workBook.Worksheet(2);
            ws2.Name = "Summary by Lifecycle Stage";
            var ws3 = workBook.Worksheet(3);
            ws3.Name = "Sortable";

            // Summary by Work Type
            ws1.Cell(2, 2).Value = String.Format("Structures Program FY {0} - {1}", startFY, endFY);
            ws1.Cell(3, 2).Value = "Projects by Work Type";
            ws1.Cell(4, 2).Value = String.Format("Report Date: {0:MM/dd/yyyy}", DateTime.Now);
            ws1.Cell(6, 3).Value = "ENTIRE PROGRAM";

            int columnNumber = 5;
            foreach (var workType in workTypes)
            {
                ws1.Cell(6, columnNumber).Value = String.Format("({0}) {1}", workType.WorkActionCode, workType.WorkActionDesc);
                columnNumber++;
                columnNumber++;
            }
            ws1.Cell(6, columnNumber).Value = "(01) NEW STRUCTURE";
            ws1.Cell(6, columnNumber + 2).Value = "OTHER";
            ws1.Cell(7, 1).Value = "FY";
            ws1.Cell(7, 2).Value = "Region";
            ws1.Cell(7, 3).Value = "Occurrences";
            ws1.Cell(7, 4).Value = "Total Cost (w/ Del)";

            columnNumber = 5;
            foreach (var workType in workTypes)
            {
                ws1.Cell(7, columnNumber).Value = "Occurrences";
                ws1.Cell(7, columnNumber + 1).Value = "Total Cost (w/ Del)";
                columnNumber++;
                columnNumber++;
            }

            ws1.Cell(7, columnNumber).Value = "Occurrences";
            ws1.Cell(7, columnNumber + 1).Value = "Total Cost (w/ Del)";
            ws1.Cell(7, columnNumber + 2).Value = "Occurrences";
            ws1.Cell(7, columnNumber + 3).Value = "Total Cost (w/ Del)";

            int rowNumber = 8;
            for (int i = startFY; i <= endFY; i++)
            {
                // totals for current year for all work types
                int rowNumberForCurrentYear = rowNumber;
                var yearlyPwas = pwas.Where(e => e.StateFiscalYear == i).ToList();
                var yearlyPwasCount = yearlyPwas.Count();
                var yearlyPwasCost = yearlyPwas.Sum(e => e.WorkTotalWithDeliveryAmount);
                ws1.Cell(rowNumber, 1).Value = i;
                ws1.Cell(rowNumber, 3).Value = yearlyPwasCount;
                ws1.Cell(rowNumber, 4).Value = yearlyPwasCost;

                columnNumber = 5;
                float yearlyAccumulatingCount = 0;
                float yearlyAccumulatingCost = 0;

                foreach (var workType in workTypes)
                {
                    // totals for current year per work type; default to zeros
                    ws1.Cell(rowNumber, columnNumber).Value = 0;
                    ws1.Cell(rowNumber, columnNumber + 1).Value = 0;

                    try
                    {
                        var yearlyPwasPerWorkType = yearlyPwas.Where(e => e.OriginalWorkActionCode.Equals(workType.WorkActionCode)).ToList();
                        ws1.Cell(rowNumber, columnNumber).Value = yearlyPwasPerWorkType.Count();
                        var yearlyPwasPerWorkTypeCost = yearlyPwasPerWorkType.Sum(e => e.WorkTotalWithDeliveryAmount);
                        ws1.Cell(rowNumber, columnNumber + 1).Value = yearlyPwasPerWorkTypeCost;
                        yearlyAccumulatingCount += yearlyPwasPerWorkType.Count();
                        yearlyAccumulatingCost += yearlyPwasPerWorkTypeCost;
                    }
                    catch { }

                    columnNumber++;
                    columnNumber++;
                }
                // end foreach (var workType in workTypes)

                foreach (var region in regions)
                {
                    float yearlyRegionAccumulatingCount = 0;
                    float yearlyRegionAccumulatingCost = 0;
                    rowNumber++;
                    ws1.Cell(rowNumber, 2).Value = region;
                    ws1.Cell(rowNumber, 3).Value = 0;
                    ws1.Cell(rowNumber, 4).Value = 0;

                    // totals for the year per region
                    try
                    {
                        var yearlyPwasPerRegion = yearlyPwas.Where(e => e.DotRegionCode.Equals(region)).ToList();
                        var yearlyPwasPerRegionCount = yearlyPwasPerRegion.Count();
                        var yearlyPwasPerRegionCost = yearlyPwasPerRegion.Sum(e => e.WorkTotalWithDeliveryAmount);
                        ws1.Cell(rowNumber, 3).Value = yearlyPwasPerRegionCount;
                        ws1.Cell(rowNumber, 4).Value = yearlyPwasPerRegionCost;

                        int colCounter = 5;
                        foreach (var workType in workTypes)
                        {
                            ws1.Cell(rowNumber, colCounter).Value = 0;
                            ws1.Cell(rowNumber, colCounter + 1).Value = 0;

                            try
                            {
                                var yearlyPwasPerRegionPerWorkType = yearlyPwasPerRegion.Where(e => e.OriginalWorkActionCode.Equals(workType.WorkActionCode));
                                var yearlyPwasPerRegionPerWorkTypeCount = yearlyPwasPerRegionPerWorkType.Count();
                                var yearlyPwasPerRegionPerWorkTypeCost = yearlyPwasPerRegionPerWorkType.Sum(e => e.WorkTotalWithDeliveryAmount);
                                ws1.Cell(rowNumber, colCounter).Value = yearlyPwasPerRegionPerWorkTypeCount;
                                ws1.Cell(rowNumber, colCounter + 1).Value = yearlyPwasPerRegionPerWorkTypeCost;
                                yearlyRegionAccumulatingCount += yearlyPwasPerRegionPerWorkTypeCount;
                                yearlyRegionAccumulatingCost += yearlyPwasPerRegionPerWorkTypeCost;
                            }
                            catch { }

                            colCounter++;
                            colCounter++;
                        }
                        // end foreach (var workType in workTypes)

                        // At the region level
                        // Special Work Type Case (01) NEW STRUCTURE
                        ws1.Cell(rowNumber, colCounter).Value = 0;
                        ws1.Cell(rowNumber, colCounter + 1).Value = 0;

                        try
                        {
                            var yearlyPwasPerRegionPerWorkType = yearlyPwasPerRegion.Where(e => e.OriginalWorkActionCode.Equals("01"));
                            var yearlyPwasPerRegionPerWorkTypeCount = yearlyPwasPerRegionPerWorkType.Count();
                            var yearlyPwasPerRegionPerWorkTypeCost = yearlyPwasPerRegionPerWorkType.Sum(e => e.WorkTotalWithDeliveryAmount);
                            ws1.Cell(rowNumber, colCounter).Value = yearlyPwasPerRegionPerWorkTypeCount;
                            ws1.Cell(rowNumber, colCounter + 1).Value = yearlyPwasPerRegionPerWorkTypeCost;
                            yearlyRegionAccumulatingCount += yearlyPwasPerRegionPerWorkTypeCount;
                            yearlyRegionAccumulatingCost += yearlyPwasPerRegionPerWorkTypeCost;
                        }
                        catch { }

                        // All other work types
                        ws1.Cell(rowNumber, colCounter + 2).Value = yearlyPwasPerRegionCount - yearlyRegionAccumulatingCount;
                        ws1.Cell(rowNumber, colCounter + 3).Value = yearlyPwasPerRegionCost - yearlyRegionAccumulatingCost;
                    }
                    catch { }
                }
                // end foreach (var region in regions)

                // At the year level
                // Special Work Type Case (01) NEW STRUCTURE
                ws1.Cell(rowNumberForCurrentYear, columnNumber).Value = 0;
                ws1.Cell(rowNumberForCurrentYear, columnNumber + 1).Value = 0;

                try
                {
                    var yearlyPwasPerWorkType = yearlyPwas.Where(e => e.OriginalWorkActionCode.Equals("01")).ToList();
                    ws1.Cell(rowNumberForCurrentYear, columnNumber).Value = yearlyPwasPerWorkType.Count();
                    var yearlyPwasPerWorkTypeCost = yearlyPwasPerWorkType.Sum(e => e.WorkTotalWithDeliveryAmount);
                    ws1.Cell(rowNumberForCurrentYear, columnNumber + 1).Value = yearlyPwasPerWorkTypeCost;
                    yearlyAccumulatingCount += yearlyPwasPerWorkType.Count();
                    yearlyAccumulatingCost += yearlyPwasPerWorkTypeCost;
                }
                catch { }

                // At the year level
                // All other work types
                ws1.Cell(rowNumberForCurrentYear, columnNumber + 2).Value = yearlyPwasCount - yearlyAccumulatingCount;
                ws1.Cell(rowNumberForCurrentYear, columnNumber + 3).Value = yearlyPwasCost - yearlyAccumulatingCost;

                rowNumber++;
            }
            // end worksheet 1


            ws2.Cell(2, 2).Value = String.Format("Structures Program FY {0} - {1}", startFY, endFY);
            ws2.Cell(3, 2).Value = "Projects by Lifecycle Stage";
            ws2.Cell(4, 2).Value = String.Format("Report Date: {0:MM/dd/yyyy}", DateTime.Now);
            // end worksheet 2

            ws3.Cell(1, 1).Value = "Rgn";
            ws3.Cell(1, 2).Value = "Cnty";
            ws3.Cell(1, 3).Value = "FOS Project ID";
            ws3.Cell(1, 4).Value = "Work Type";
            ws3.Cell(1, 5).Value = "Existing Str";
            ws3.Cell(1, 6).Value = "New Str";
            ws3.Cell(1, 7).Value = "Let Date";
            ws3.Cell(1, 8).Value = "Earliest Adv Let Date";
            ws3.Cell(1, 9).Value = "Latest Adv Let Date";
            ws3.Cell(1, 10).Value = "FY";
            ws3.Cell(1, 11).Value = "Fndg Ctgy";
            ws3.Cell(1, 12).Value = "Fndg Ctgy Desc";
            ws3.Cell(1, 13).Value = "Route";
            ws3.Cell(1, 14).Value = "Sub Prog";
            ws3.Cell(1, 15).Value = "Proj Mgr";
            ws3.Cell(1, 16).Value = "Title";
            ws3.Cell(1, 17).Value = "Limit";
            ws3.Cell(1, 18).Value = "Concept";
            ws3.Cell(1, 19).Value = "Func Type";
            ws3.Cell(1, 20).Value = "DOT Prog";
            ws3.Cell(1, 21).Value = "Lifecycle";
            ws3.Cell(1, 22).Value = "Lifecycle Date";
            ws3.Cell(1, 23).Value = "Plng Proj Cncpt";
            ws3.Cell(1, 24).Value = "Fed Impr Type";
            ws3.Cell(1, 25).Value = "Design Proj ID";
            ws3.Cell(1, 26).Value = "PSE Date";
            ws3.Cell(1, 27).Value = "Adv PSE Date";

            var pwasOrderByLetDate = pwas.OrderBy(e => e.EstimatedCompletionDate);
            int rowCounter = 2;

            foreach (var pwa in pwasOrderByLetDate)
            {
                ws3.Cell(rowCounter, 1).Value = pwa.DotRegionCode;
                ws3.Cell(rowCounter, 2).Value = pwa.County;
                ws3.Cell(rowCounter, 3).Value = pwa.FosProjId;
                ws3.Cell(rowCounter, 4).Value = String.Format("({0})-{1}", pwa.OriginalWorkActionCode, pwa.OriginalWorkActionDesc);
                ws3.Cell(rowCounter, 5).Value = pwa.StructureId;
                ws3.Cell(rowCounter, 6).Value = pwa.NewStructureId;
                if (pwa.EstimatedCompletionDate.Year != 1)
                    ws3.Cell(rowCounter, 7).Value = String.Format("{0:MM/dd/yyyy}", pwa.EstimatedCompletionDate);
                if (pwa.EarliestAdvanceableLetDate.Year != 1)
                    ws3.Cell(rowCounter, 8).Value = String.Format("{0:MM/dd/yyyy}", pwa.EarliestAdvanceableLetDate);
                if (pwa.LatestAdvanceableLetDate.Year != 1)
                    ws3.Cell(rowCounter, 9).Value = String.Format("{0:MM/dd/yyyy}", pwa.LatestAdvanceableLetDate);
                ws3.Cell(rowCounter, 10).Value = pwa.StateFiscalYear;
                ws3.Cell(rowCounter, 11).Value = pwa.FundingCategoryNumber;
                ws3.Cell(rowCounter, 12).Value = pwa.FundingCategoryDesc;
                ws3.Cell(rowCounter, 13).Value = pwa.Route;
                ws3.Cell(rowCounter, 14).Value = pwa.SubProgramDesc;
                ws3.Cell(rowCounter, 15).Value = pwa.ProjectManager;
                ws3.Cell(rowCounter, 16).Value = pwa.Title;
                ws3.Cell(rowCounter, 17).Value = pwa.Limit;
                ws3.Cell(rowCounter, 18).Value = pwa.Concept;
                ws3.Cell(rowCounter, 19).Value = String.Format("{0}-{1}", pwa.FunctionalTypeCode, pwa.FunctionalTypeDesc);
                ws3.Cell(rowCounter, 20).Value = pwa.WisDOTProgramDesc;
                ws3.Cell(rowCounter, 21).Value = pwa.LifeCycleStageCode;
                if (pwa.LifeCycleStageDate.Year != 1)
                    ws3.Cell(rowCounter, 22).Value = String.Format("{0:MM/dd/yyyy}", pwa.LifeCycleStageDate);
                ws3.Cell(rowCounter, 23).Value = String.Format("{0}-{1}", pwa.PlanningProjectConceptCode, pwa.PlanningProjectConceptDesc);
                ws3.Cell(rowCounter, 24).Value = String.Format("{0}-{1}", pwa.FederalImprovementTypeCode, pwa.FederalImprovementTypeDesc);
                ws3.Cell(rowCounter, 25).Value = pwa.DesignProjectId;
                if (pwa.PseDate.Year != 1)
                    ws3.Cell(rowCounter, 26).Value = String.Format("{0:MM/dd/yyyy}", pwa.PseDate);
                if (pwa.EarliestPseDate.Year != 1)
                    ws3.Cell(rowCounter, 27).Value = String.Format("{0:MM/dd/yyyy}", pwa.EarliestPseDate);
                rowCounter++;
            }
        }

        public void WriteStructuresDataForGisReport(List<Structure> structures, XLWorkbook workBook)
        {
            var ws1 = workBook.Worksheet("Sheet1");
            //ws1.Cell(2, 1).InsertData(structures);

            List<string> ws1Columns = new List<string>()
            {
                "Str ID", "Crdr", "Region", "County", "Feat On", "Feat Und",
                "Str Type", "Matl", "Num Spans", "Tot Len(ft)", "Inv Rating",
                "Opr Rating", "Load Posting", "Last Insp", "Const Hist"
            };


            int colCounter = 1;

            foreach (var ws1Column in ws1Columns)
            {
                ws1.Cell(1, colCounter).Value = ws1Column;
                colCounter++;
            }

            List<GisRow> gisRows = new List<GisRow>();

            foreach (var structure in structures)
            {
                try
                {
                    GisRow gisRow = new GisRow();
                    gisRow.StructureId = structure.StructureId;
                    gisRow.CorridorCode = structure.CorridorCode;
                    gisRow.Region = structure.Region;
                    gisRow.County = structure.County;
                    gisRow.FeatureOn = structure.FeatureOn;
                    gisRow.FeatureUnder = structure.FeatureUnder;
                    gisRow.StructureType = structure.StructureType;
                    gisRow.MainSpanMaterial = structure.MainSpanMaterial;
                    gisRow.NumSpans = structure.NumSpans;
                    gisRow.TotalLengthSpans = Math.Round(structure.TotalLengthSpans, 2);
                    gisRow.InventoryRating = structure.InventoryRating;
                    gisRow.OperatingRating = structure.OperatingRating;

                    if (structure.LoadPostingCode != 0)
                    {
                        gisRow.LoadPosting = string.Format("{0}", structure.LoadPostingDesc);
                    }
                    else
                    {
                        gisRow.LoadPosting = "None";
                    }

                    gisRow.LastInspectionDate = structure.LastInspection.InspectionDate;
                    gisRow.ConstructionHistory = structure.ConstructionHistory;
                    gisRows.Add(gisRow);
                }
                catch { }
            }

            ws1.Cell(2, 1).InsertData(gisRows.AsEnumerable());
        }
    }
}
