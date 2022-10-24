using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.Dw;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using System.IO;

namespace WisDot.Bos.Sct.Core.Data
{
    public class ExcelReporterQuery : IExcelReporterQuery
    {
        private static IExcelReporterRepository repo = new ExcelReporterRepository();

        public void WriteLoginHistoryReport(string outputFilePath, List<UserActivity> userActivities)
        {
            XLWorkbook wb = new XLWorkbook();
            wb.AddWorksheet("Logins");
            var ws1 = wb.Worksheet("Logins");
            List<string> ws1ColumnNames = new List<string>()
            {
                "Dot Login", "First Name", "Last Name",
                "Office", "Activity", "Activity Date"
            };
            //ws1.Cell(1, 1).InsertData(ws1ColumnNames);
            int columnCounter = 1;
            int rowCounter = 1;

            foreach (var ws1ColumnName in ws1ColumnNames)
            {
                ws1.Cell(rowCounter, columnCounter).Value = ws1ColumnName;
                columnCounter++;
            }

            rowCounter++;

            foreach (var ua in userActivities)
            {
                ws1.Cell(rowCounter, 1).Value = ua.DotLogin;
                ws1.Cell(rowCounter, 2).Value = ua.FirstName;
                ws1.Cell(rowCounter, 3).Value = ua.LastName;
                ws1.Cell(rowCounter, 4).Value = ua.Office;
                ws1.Cell(rowCounter, 5).Value = ua.Activity;
                ws1.Cell(rowCounter, 6).Value = ua.ActivityDateTime;
                rowCounter++;
            }

            wb.SaveAs(outputFilePath);
            wb.Dispose();
        }

        public void WriteMaintenanceNeedsReport(List<Dw.StructureMaintenanceItem> maintenanceItems, DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false, string wisamsMaintenanceNeedsListExcelFile = "")
        {
            Dw.Database dwDatabase = null;

            try
            {
                dwDatabase = new Dw.Database();
            }
            catch
            {
                throw new Exception("Unable to connect to the Structures Data Warehouse and thus unable to run the report");
            }

            if (dwDatabase == null)
            {
                return;
            }

            // Open WiSAMS maintenance needs list
            XLWorkbook wisamsWb = new XLWorkbook(wisamsMaintenanceNeedsListExcelFile);
            IXLWorksheet wisamsWs = wisamsWb.Worksheet(1);
            List<Dw.StructureMaintenanceItem> wisamsNeedsListNotInHsi = repo.GetWisamsNeedsListNotInHsi(dwDatabase, maintenanceItems, wisamsWs);

            // List<Dw.Structure> structures = new List<Dw.Structure>(); // structures in Structures Projects
            // List<Dw.Structure> fiipsStructures = new List<Dw.Structure>(); // structures in Fiips Projects
            //List<WorkConcept> primaryWorkConcepts = database.GetPrimaryWorkConcepts();

            XLWorkbook wb = new XLWorkbook();
            wb.AddWorksheet("Sheet1");
            var ws1 = wb.Worksheet(1);
            ws1.Cell(1, 1).Value = "Structure ID";
            ws1.Cell(1, 2).Value = "County";
            ws1.Cell(1, 3).Value = "Feature On";
            ws1.Cell(1, 4).Value = "Location";
            ws1.Cell(1, 5).Value = "Feature Under";
            ws1.Cell(1, 6).Value = "Deck Area";
            ws1.Cell(1, 7).Value = "Overall Length";
            ws1.Cell(1, 8).Value = "Roadway Width";
            ws1.Cell(1, 9).Value = "Source";
            ws1.Cell(1, 10).Value = "Date";
            ws1.Cell(1, 11).Value = "Item Description";
            ws1.Cell(1, 12).Value = "Comments";
            ws1.Cell(1, 13).Value = "Action Priority";
            ws1.Cell(1, 14).Value = "Item Code";
            ws1.Cell(1, 15).Value = "Item ID";

            // To be filled out by regional bridge maintenance
            ws1.Cell(1, 16).Value = "Status";
            ws1.Cell(1, 17).Value = "Contract Type";

            ws1.Cell(1, 18).Value = "Scheduled Year";
            ws1.Cell(1, 19).Value = "Estimated Amount";

            // SCT
            ws1.Cell(1, 20).Value = "SCT Proj";
            ws1.Cell(1, 21).Value = "Primary Work Concept";
            ws1.Cell(1, 22).Value = "Secondary Work Concepts";
            ws1.Cell(1, 23).Value = "FY";
            ws1.Cell(1, 24).Value = "Adv FY";

            // FIIPS
            ws1.Cell(1, 25).Value = "FIIPS Construction ID";
            ws1.Cell(1, 26).Value = "Primary Work Concept";
            ws1.Cell(1, 27).Value = "Let Date";
            ws1.Cell(1, 28).Value = "EPSE Date";

            // To be filled out by County
            ws1.Cell(1, 29).Value = "Actual Amount";
            ws1.Cell(1, 30).Value = "Actual Date";
            ws1.Cell(1, 31).Value = "Performed By";

            // History
            //ws1.Cell(1, 32).Value = "Construction History";
            //ws1.Cell(1, 33).Value = "Maintenance History";
            ws1.Cell(1, 32).Value = "Region";
            ws1.Cell(1, 33).Value = "Planning Corridor";

            // Populate worksheet 1
            int rowCounter = 2;

            // Filter SCT projects
            List<Project> filteredSctProjects = structuresProjects
                                        .Where(proj => proj.FiscalYear >= startFy &&
                                                    (proj.Status == StructuresProgramType.ProjectStatus.QuasiCertified
                                                        || proj.Status == StructuresProgramType.ProjectStatus.Certified
                                                        || proj.Status == StructuresProgramType.ProjectStatus.Precertified)
                                              ).OrderBy(proj => proj.FiscalYear).ToList();
            // Filter FIIPS projects
            List<Project> filteredFiipsProjects = fiipsProjects
                                                    .Where(proj => proj.FiscalYear >= startFy && proj.FiscalYear <= endFy)
                                                    .OrderBy(proj => proj.FiscalYear).ToList();
            bool haveAddedWisamsNeedsListNotInHsi = false;

            foreach (var region in regions)
            {
                string currentStructureId = "";
                string previousStructureId = "";
                List<Project> currentSctProjects = new List<Project>();
                List<Project> currentFiipsProjects = new List<Project>();
                Project firstSctProject = null;
                Project firstFiipsProject = null;
                List<Dw.StructureMaintenanceItem> previousMaintenanceItems = new List<Dw.StructureMaintenanceItem>();
                int currentGroupCounter = 0;

                if (!haveAddedWisamsNeedsListNotInHsi)
                {
                    if (wisamsNeedsListNotInHsi.Count() > 0)
                    {
                        maintenanceItems.AddRange(wisamsNeedsListNotInHsi);
                    }

                    haveAddedWisamsNeedsListNotInHsi = true;
                }

                foreach (var mi in maintenanceItems.Where(item => item.Region.Equals(region)).OrderBy(item => item.StructureId))
                {
                    currentStructureId = mi.StructureId;

                    if (!currentStructureId.Equals(previousStructureId))
                    {
                        if (previousMaintenanceItems.Count() > 0)
                        {
                            currentSctProjects = filteredSctProjects.Where(proj => proj.WorkConcepts.Any(wc => wc.StructureId.Equals(previousStructureId))).ToList();
                            firstSctProject = currentSctProjects.Count > 0 ? currentSctProjects.First() : null;
                            currentFiipsProjects = filteredFiipsProjects.Where(proj => proj.WorkConcepts.Any(wc => wc.StructureId.Equals(previousStructureId))).ToList();
                            firstFiipsProject = currentFiipsProjects.Count > 0 ? currentFiipsProjects.First() : null;

                            //if (previousMaintenanceItems.Count > 0)
                            {
                                List<Dw.StructureMaintenanceItem> wisams = new List<Dw.StructureMaintenanceItem>();

                                if (!previousMaintenanceItems.First().WisamsOnly)
                                {
                                    wisams = repo.CompareToWisamsNeedsList(previousStructureId, previousMaintenanceItems, wisamsWs);
                                    previousMaintenanceItems.AddRange(wisams);
                                }

                                foreach (var cmi in previousMaintenanceItems)
                                {
                                    ws1.Cell(rowCounter, 1).Value = cmi.StructureId;
                                    ws1.Cell(rowCounter, 2).Value = cmi.County;
                                    ws1.Cell(rowCounter, 3).Value = cmi.FeatureOn;
                                    ws1.Cell(rowCounter, 4).Value = cmi.LocationOn;
                                    ws1.Cell(rowCounter, 5).Value = cmi.FeatureUnder;
                                    ws1.Cell(rowCounter, 6).Value = cmi.DeckArea;
                                    ws1.Cell(rowCounter, 7).Value = cmi.OverallLength;
                                    ws1.Cell(rowCounter, 8).Value = cmi.RoadwayWidth;
                                    ws1.Cell(rowCounter, 9).Value = cmi.Source;
                                    DateTime itemDate;

                                    if (cmi.Source.Equals("WISAMS"))
                                    {
                                        itemDate = cmi.EstimatedDate;
                                    }
                                    else if (cmi.InspectionDate.Year != 1)
                                    {
                                        itemDate = cmi.InspectionDate;
                                    }
                                    else
                                    {
                                        itemDate = cmi.StatusDate;
                                    }

                                    ws1.Cell(rowCounter, 10).Value = itemDate.ToString("yyyy-MM-dd");
                                    ws1.Cell(rowCounter, 11).Value = cmi.ItemDescription.ToUpper().Replace("RESTORE CONDITION AND CAPACITY", "");
                                    ws1.Cell(rowCounter, 12).Value = cmi.ItemComments;
                                    ws1.Cell(rowCounter, 13).Value = cmi.Priority;
                                    ws1.Cell(rowCounter, 14).Value = cmi.ItemCode;
                                    ws1.Cell(rowCounter, 15).Value = cmi.ItemId;
                                    ws1.Cell(rowCounter, 18).Value = "2021";
                                    ws1.Cell(rowCounter, 19).Value = !String.IsNullOrEmpty(cmi.EstimatedCost) && !cmi.EstimatedCost.Equals("0") ? cmi.EstimatedCost : "";

                                    if (firstSctProject != null)
                                    {
                                        ws1.Cell(rowCounter, 20).Value = String.Format("SCT{0},{1},{2}{3}", firstSctProject.ProjectDbId,
                                                                            firstSctProject.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : firstSctProject.Status.ToString(),
                                                                            firstSctProject.FosProjectId,
                                                                            currentSctProjects.Count > 1 ? ", Addl proj: " + (currentSctProjects.Count - 1).ToString() : "");
                                        WorkConcept workConcept = firstSctProject.WorkConcepts.Where(wc => wc.StructureId.Equals(previousStructureId)).First();
                                        ws1.Cell(rowCounter, 21).Value = String.Format("({0}) {1}", workConcept.CertifiedWorkConceptCode, workConcept.CertifiedWorkConceptDescription);

                                        var ewcs = firstSctProject.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == workConcept.ProjectWorkConceptHistoryDbId
                                                                                                        && el.StructureId.Equals(workConcept.StructureId)
                                                                                                        && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                                        && el.CertificationDateTime == workConcept.CertificationDateTime)
                                                                                                        .GroupBy(el => el.WorkConceptCode)
                                                                                                        .Select(g => g.First())
                                                                                                        .ToList();
                                        string secondaryWorkConcepts = "";
                                        int ewcCounter = 0;

                                        foreach (var ewc in ewcs)
                                        {
                                            if (ewcCounter == 0)
                                            {
                                                secondaryWorkConcepts = String.Format("({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                                            }
                                            else
                                            {
                                                secondaryWorkConcepts += String.Format("; ({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                                            }

                                            ewcCounter++;
                                        }
                                        ws1.Cell(rowCounter, 22).Value = secondaryWorkConcepts;
                                        ws1.Cell(rowCounter, 23).Value = firstSctProject.FiscalYear != 0 ? firstSctProject.FiscalYear.ToString() : "";
                                        ws1.Cell(rowCounter, 24).Value = firstSctProject.AdvanceableFiscalYear != 0 ? firstSctProject.AdvanceableFiscalYear.ToString() : "";
                                    }

                                    if (firstFiipsProject != null)
                                    {
                                        ws1.Cell(rowCounter, 25).Value = String.Format("{0} {1}", firstFiipsProject.FosProjectId,
                                                                           currentFiipsProjects.Count > 1 ? "Addl proj: " + (currentFiipsProjects.Count - 1).ToString() : "");
                                        WorkConcept workConcept = firstFiipsProject.WorkConcepts.Where(wc => wc.StructureId.Equals(previousStructureId)).First();
                                        ws1.Cell(rowCounter, 26).Value = String.Format("({0}) {1}", workConcept.WorkConceptCode, workConcept.WorkConceptDescription);
                                        ws1.Cell(rowCounter, 27).Value = firstFiipsProject.LetDate.Year != 1 ? firstFiipsProject.LetDate.ToString("yyyy-MM-dd") : "";
                                        ws1.Cell(rowCounter, 28).Value = firstFiipsProject.EpseDate.Year != 1 ? firstFiipsProject.EpseDate.ToString("yyyy-MM-dd") : "";
                                    }

                                    //ws1.Cell(rowCounter, 33).Value = mi.MaintenanceHistory;
                                    ws1.Cell(rowCounter, 32).Value = cmi.Region;
                                    ws1.Cell(rowCounter, 33).Value = cmi.PlanningCorridor;
                                    rowCounter++;
                                }
                            }
                        }

                        currentGroupCounter = 0;
                        previousMaintenanceItems = new List<Dw.StructureMaintenanceItem>();
                        previousMaintenanceItems.Add(mi);
                        currentGroupCounter++;
                    }
                    else
                    {
                        currentGroupCounter++;
                        previousMaintenanceItems.Add(mi);
                    }

                    previousStructureId = currentStructureId;

                    /*
                    if (!currentStructureId.Equals(previousStructureId))
                    {
                        //currentMaintenanceItems.Add(mi);
                        currentSctProjects = filteredSctProjects.Where(proj => proj.WorkConcepts.Any(wc => wc.StructureId.Equals(currentStructureId))).ToList();
                        firstSctProject = currentSctProjects.Count > 0 ? currentSctProjects.First() : null;
                        currentFiipsProjects = filteredFiipsProjects.Where(proj => proj.WorkConcepts.Any(wc => wc.StructureId.Equals(currentStructureId))).ToList();
                        firstFiipsProject = currentFiipsProjects.Count > 0 ? currentFiipsProjects.First() : null;

                        if (previousMaintenanceItems.Count > 0)
                        {
                            List<Dw.StructureMaintenanceItem> wisams = CompareToWisamsNeedsList(currentStructureId, previousMaintenanceItems, wisamsWs);
                            previousMaintenanceItems.AddRange(wisams);

                            foreach (var cmi in previousMaintenanceItems)
                            {
                                ws1.Cell(rowCounter, 1).Value = cmi.StructureId;
                                ws1.Cell(rowCounter, 2).Value = cmi.County;
                                ws1.Cell(rowCounter, 3).Value = cmi.FeatureOn;
                                ws1.Cell(rowCounter, 4).Value = cmi.LocationOn;
                                ws1.Cell(rowCounter, 5).Value = cmi.FeatureUnder;
                                ws1.Cell(rowCounter, 6).Value = cmi.DeckArea;
                                ws1.Cell(rowCounter, 7).Value = cmi.OverallLength;
                                ws1.Cell(rowCounter, 8).Value = cmi.RoadwayWidth;
                                ws1.Cell(rowCounter, 9).Value = cmi.Source;
                                DateTime itemDate;

                                if (cmi.Source.Equals("WISAMS"))
                                {
                                    itemDate = cmi.EstimatedDate;
                                }
                                else if (cmi.InspectionDate.Year != 1)
                                {
                                    itemDate = cmi.InspectionDate;
                                }
                                else
                                {
                                    itemDate = cmi.StatusDate;
                                }

                                ws1.Cell(rowCounter, 10).Value = itemDate.ToString("yyyy-MM-dd");
                                ws1.Cell(rowCounter, 11).Value = cmi.ItemDescription;
                                ws1.Cell(rowCounter, 12).Value = cmi.ItemComments;
                                ws1.Cell(rowCounter, 13).Value = cmi.Priority;
                                ws1.Cell(rowCounter, 14).Value = cmi.ItemCode;
                                ws1.Cell(rowCounter, 15).Value = cmi.ItemId;
                                ws1.Cell(rowCounter, 18).Value = "2021";
                                ws1.Cell(rowCounter, 19).Value = !String.IsNullOrEmpty(cmi.EstimatedCost) && !cmi.EstimatedCost.Equals("0") ? cmi.EstimatedCost : "";

                                if (firstSctProject != null)
                                {
                                    ws1.Cell(rowCounter, 20).Value = String.Format("SCT{0},{1},{2}{3}", firstSctProject.ProjectDbId,
                                                                        firstSctProject.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : firstSctProject.Status.ToString(),
                                                                        firstSctProject.FosProjectId,
                                                                        currentSctProjects.Count > 1 ? ", Addl proj: " + (currentSctProjects.Count - 1).ToString() : "");
                                    WorkConcept workConcept = firstSctProject.WorkConcepts.Where(wc => wc.StructureId.Equals(currentStructureId)).First();
                                    ws1.Cell(rowCounter, 21).Value = String.Format("({0}) {1}", workConcept.CertifiedWorkConceptCode, workConcept.CertifiedWorkConceptDescription);

                                    var ewcs = firstSctProject.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == workConcept.ProjectWorkConceptHistoryDbId
                                                                                                    && el.StructureId.Equals(workConcept.StructureId)
                                                                                                    && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                                    && el.CertificationDateTime == workConcept.CertificationDateTime)
                                                                                                    .GroupBy(el => el.WorkConceptCode)
                                                                                                    .Select(g => g.First())
                                                                                                    .ToList();
                                    string secondaryWorkConcepts = "";
                                    int ewcCounter = 0;

                                    foreach (var ewc in ewcs)
                                    {
                                        if (ewcCounter == 0)
                                        {
                                            secondaryWorkConcepts = String.Format("({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                                        }
                                        else
                                        {
                                            secondaryWorkConcepts += String.Format("; ({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                                        }

                                        ewcCounter++;
                                    }
                                    ws1.Cell(rowCounter, 22).Value = secondaryWorkConcepts;
                                    ws1.Cell(rowCounter, 23).Value = firstSctProject.FiscalYear != 0 ? firstSctProject.FiscalYear.ToString() : "";
                                    ws1.Cell(rowCounter, 24).Value = firstSctProject.AdvanceableFiscalYear != 0 ? firstSctProject.AdvanceableFiscalYear.ToString() : "";
                                }

                                if (firstFiipsProject != null)
                                {
                                    ws1.Cell(rowCounter, 25).Value = String.Format("{0} {1}", firstFiipsProject.FosProjectId,
                                                                       currentFiipsProjects.Count > 1 ? "Addl proj: " + (currentFiipsProjects.Count - 1).ToString() : "");
                                    WorkConcept workConcept = firstFiipsProject.WorkConcepts.Where(wc => wc.StructureId.Equals(currentStructureId)).First();
                                    ws1.Cell(rowCounter, 26).Value = String.Format("({0}) {1}", workConcept.WorkConceptCode, workConcept.WorkConceptDescription);
                                    ws1.Cell(rowCounter, 27).Value = firstFiipsProject.LetDate.Year != 1 ? firstFiipsProject.LetDate.ToString("yyyy-MM-dd") : "";
                                    ws1.Cell(rowCounter, 28).Value = firstFiipsProject.EpseDate.Year != 1 ? firstFiipsProject.EpseDate.ToString("yyyy-MM-dd") : "";
                                }

                                //ws1.Cell(rowCounter, 33).Value = mi.MaintenanceHistory;
                                ws1.Cell(rowCounter, 32).Value = mi.Region;
                                ws1.Cell(rowCounter, 33).Value = mi.PlanningCorridor;
                                rowCounter++;
                            }
                        }

                        
                    }

                    previousMaintenanceItems = new List<Dw.StructureMaintenanceItem>();
                    
                    previousStructureId = currentStructureId;

                    /*
                    ws1.Cell(rowCounter, 1).Value = mi.StructureId;
                    ws1.Cell(rowCounter, 2).Value = mi.County;
                    ws1.Cell(rowCounter, 3).Value = mi.FeatureOn;
                    ws1.Cell(rowCounter, 4).Value = mi.LocationOn;
                    ws1.Cell(rowCounter, 5).Value = mi.FeatureUnder;
                    ws1.Cell(rowCounter, 6).Value = mi.DeckArea;
                    ws1.Cell(rowCounter, 7).Value = mi.OverallLength;
                    ws1.Cell(rowCounter, 8).Value = mi.RoadwayWidth;
                    ws1.Cell(rowCounter, 9).Value = mi.InspectionDate.Year != 1 ? String.Format("INSPECTION ({0})", mi.Recommender) : 
                                                        String.Format("PM ({0})", mi.Recommender);
                    ws1.Cell(rowCounter, 10).Value = mi.InspectionDate.Year != 1 ? mi.InspectionDate.ToString("yyyy-MM-dd") :
                                                        mi.StatusDate.ToString("yyyy-MM-dd");
                    ws1.Cell(rowCounter, 11).Value = mi.ItemDescription;
                    ws1.Cell(rowCounter, 12).Value = mi.ItemComments;
                    ws1.Cell(rowCounter, 13).Value = mi.Priority;
                    ws1.Cell(rowCounter, 14).Value = mi.ItemCode;
                    ws1.Cell(rowCounter, 15).Value = mi.ItemId;

                    ws1.Cell(rowCounter, 16).Value = mi.EstimatedDate.Year != 1 ? mi.EstimatedDate.Year.ToString() : "";
                    ws1.Cell(rowCounter, 17).Value = !String.IsNullOrEmpty(mi.EstimatedCost) ? mi.EstimatedCost : "";

                    // Find SCT projects for given structure
                    if (!currentStructureId.Equals(previousStructureId))
                    {
                        currentSctProjects = filteredSctProjects.Where(proj => proj.WorkConcepts.Any(wc => wc.StructureId.Equals(currentStructureId))).ToList();
                        firstSctProject = currentSctProjects.Count > 0 ? currentSctProjects.First() : null;
                        currentFiipsProjects = filteredFiipsProjects.Where(proj => proj.WorkConcepts.Any(wc => wc.StructureId.Equals(currentStructureId))).ToList();
                        firstFiipsProject = currentFiipsProjects.Count > 0 ? currentFiipsProjects.First() : null;
                    }

                    if (firstSctProject != null)
                    {
                        ws1.Cell(rowCounter, 20).Value = String.Format("SCT{0},{1},{2}{3}", firstSctProject.ProjectDbId,
                                                            firstSctProject.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : firstSctProject.Status.ToString(),
                                                            firstSctProject.FosProjectId,
                                                            currentSctProjects.Count > 1 ? "Addl proj: " + (currentSctProjects.Count - 1).ToString() : "");
                        WorkConcept workConcept = firstSctProject.WorkConcepts.Where(wc => wc.StructureId.Equals(currentStructureId)).First();
                        ws1.Cell(rowCounter, 21).Value = String.Format("({0}) {1}", workConcept.CertifiedWorkConceptCode, workConcept.CertifiedWorkConceptDescription);

                        var ewcs = firstSctProject.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == workConcept.ProjectWorkConceptHistoryDbId
                                                                                        && el.StructureId.Equals(workConcept.StructureId)
                                                                                        && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                        && el.CertificationDateTime == workConcept.CertificationDateTime)
                                                                                        .GroupBy(el => el.WorkConceptCode)
                                                                                        .Select(g => g.First())
                                                                                        .ToList();
                        string secondaryWorkConcepts = "";
                        int ewcCounter = 0;

                        foreach (var ewc in ewcs)
                        {
                            if (ewcCounter == 0)
                            {
                                secondaryWorkConcepts = String.Format("({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                            }
                            else
                            {
                                secondaryWorkConcepts += String.Format("; ({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                            }

                            ewcCounter++;
                        }
                        ws1.Cell(rowCounter, 22).Value = secondaryWorkConcepts;
                        ws1.Cell(rowCounter, 23).Value = firstSctProject.FiscalYear != 0 ? firstSctProject.FiscalYear.ToString() : "";
                        ws1.Cell(rowCounter, 24).Value = firstSctProject.AdvanceableFiscalYear != 0 ? firstSctProject.AdvanceableFiscalYear.ToString() : "";
                    }

                    if (firstFiipsProject != null)
                    {
                        ws1.Cell(rowCounter, 25).Value = String.Format("{0} {1}", firstFiipsProject.FosProjectId,
                                                           currentFiipsProjects.Count > 1 ? "Addl proj: " + (currentFiipsProjects.Count - 1).ToString() : "");
                        WorkConcept workConcept = firstFiipsProject.WorkConcepts.Where(wc => wc.StructureId.Equals(currentStructureId)).First();
                        ws1.Cell(rowCounter, 26).Value = String.Format("({0}) {1}", workConcept.WorkConceptCode, workConcept.WorkConceptDescription);
                        ws1.Cell(rowCounter, 27).Value = firstFiipsProject.LetDate.Year != 1 ? firstFiipsProject.LetDate.ToString("yyyy-MM-dd") : "";
                        ws1.Cell(rowCounter, 28).Value = firstFiipsProject.EpseDate.Year != 1 ? firstFiipsProject.EpseDate.ToString("yyyy-MM-dd") : "";
                    }

                    //ws1.Cell(rowCounter, 33).Value = mi.MaintenanceHistory;
                    ws1.Cell(rowCounter, 32).Value = mi.Region;
                    */


                    //rowCounter++;
                }
            }

            wb.SaveAs(outputFilePath);
            wb.Dispose();
        }

        public void WriteMonitoringReport(DatabaseService database, string outputFilePath, List<Project> fiipsProjects, List<WorkConcept> fiips, List<Project> structuresProjects, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false)
        {
            XLWorkbook wb = new XLWorkbook();
            wb.AddWorksheet("SCT Work Concepts Summary");
            wb.AddWorksheet("SCT Work Concepts");
            wb.AddWorksheet("Fiips Work Concepts Summary");
            wb.AddWorksheet("Fiips Work Concepts");
            //wb.AddWorksheet("Fiips Secondary Wcs");
            //wb.AddWorksheet("Uncertified-Structures-Projects");
            var ws1 = wb.Worksheet(1);
            var ws2 = wb.Worksheet(2);
            var ws3 = wb.Worksheet(3);
            var ws4 = wb.Worksheet(4);
            //var ws5 = wb.Worksheet(5);
            //var ws6 = wb.Worksheet("Uncertified-Structures-Projects");
            int currentFiscalYear = database.GetFiscalYear();
            Dw.Database dwDatabase = null;
            List<Dw.Structure> structures = new List<Dw.Structure>(); // structures in Structures Projects
            List<Dw.Structure> fiipsStructures = new List<Dw.Structure>(); // structures in Fiips Projects
            List<WorkConcept> primaryWorkConcepts = database.GetPrimaryWorkConcepts();

            try
            {
                dwDatabase = new Dw.Database();
            }
            catch
            {
                throw new Exception("Unable to connect to the Structures Data Warehouse and thus unable to run the report");
            }

            if (dwDatabase == null)
            {
                return;
            }

            foreach (var p in structuresProjects)
            {
                foreach (var wc in p.WorkConcepts)
                {
                    try
                    {
                        Dw.Structure structure = dwDatabase.GetStructure(wc.StructureId, false);

                        if (structure != null)
                        {
                            structures.Add(structure);
                        }
                    }
                    catch { }
                }
            }

            foreach (var wc in fiips.GroupBy(f => f.StructureId).Select(g => g.First()))
            {
                try
                {
                    Dw.Structure structure = dwDatabase.GetStructure(wc.StructureId, false);

                    if (structure != null)
                    {
                        fiipsStructures.Add(structure);
                    }
                }
                catch { }
            }

            List<string> ws2ColumnNames = new List<string>()
            {
                "Structures Project Id", "Structures Project Fy", "Structures Project Adv Fy", "Structures Project Status",
                "Structure Id", "Corridors 2030", "Feature On", "Feature Under", "Region", "County",
                "SCT Work Concept (Wc)", "SCT Wc Status",
                "Is Structure In A Corresponding Fiips Project?", "Corresponding FIIPS Wc Scope Match?", "Corresponding FIIPS Wc FY Match?", "Corresponding FIIPS Match?",
                "FIIPS Corresponding Wc", "FIIPS Let Fy", "FIIPS Adv Let Fy", "FIIPS PSE", "FIIPS Adv PSE", "Construction Id", "Lifecycle Stage"
            };

            List<string> ws4ColumnNames = new List<string>()
            {
                "Construction Id", "FIIPS Let Fy", "FIIPS Adv Let Fy", "FIIPS PSE", "FIIPS Adv PSE", "Lifecycle Stage",
                "Structure Id", "Corridors 2030", "Feature On", "Feature Under", "Region", "County",
                "Structure Type", "FIIPS Work Concept (Wc)", "Is Structure In A Structures Project?",
                "Matches Any SCT Wc Scope?", "SCT Wc", "Structures Project Fy", "Structures Project Id", "Structures Project Status"
            };

            int cc = 1; // Column counter
            int ws1rc = 1; // Row counter for worksheet 1

            foreach (var ws1ColumnName in ws2ColumnNames)
            {
                ws2.Cell(ws1rc, cc).Value = ws1ColumnName;
                cc++;
            }

            cc = 1;

            foreach (var ws2ColumnName in ws4ColumnNames)
            {
                ws4.Cell(ws1rc, cc).Value = ws2ColumnName;
                //ws5.Cell(ws1rc, cc).Value = ws2ColumnName;
                cc++;
            }

            ws1rc = 2;
            cc = 1;
            int totalNumberOfSctWorkConcepts = 0;
            List<WorkConcept> sctWorkConceptsNotInFiips = new List<WorkConcept>();
            List<WorkConcept> sctWorkConceptsFiipsMismatches = new List<WorkConcept>();
            int totalNumberOfFiipsWorkConcepts = 0;
            List<WorkConcept> fiipsWorkConceptsNotInSct = new List<WorkConcept>();
            List<WorkConcept> fiipsWorkConceptsSctMismatches = new List<WorkConcept>();

            #region Structures Projects
            foreach (var region in regions)
            {
                foreach (var p in structuresProjects.Where(sp => sp.Region.Equals(region) && sp.FiscalYear >= startFy && sp.FiscalYear <= endFy)
                                                    .OrderBy(sp => sp.ProjectDbId))
                {
                    // Is there a corresponding Fiips project?
                    Project correspondingFiipsProject = null;
                    //List<Project> correspondingFiipsProjects = null;


                    try
                    {
                        correspondingFiipsProject = fiipsProjects.Where(fp => fp.FosProjectId.Equals(p.FosProjectId)).First();
                        //correspondingFiipsProjects = fiipsProjects.Where(fp => fp.FosProjectId.Equals(p.FosProjectId)).ToList();
                    }
                    catch { }

                    foreach (var w in p.WorkConcepts.OrderBy(wc => wc.StructureId))
                    {
                        Dw.Structure structure = null;
                        totalNumberOfSctWorkConcepts++;

                        try
                        {
                            structure = structures.Where(s => s.StructureId.Equals(w.StructureId)).First();
                        }
                        catch { }

                        string lastInspectionFilePath = "";
                        DateTime lastInspectionDate = new DateTime();

                        try
                        {
                            lastInspectionFilePath = dwDatabase.GetLastInspectionFilePath(w.StructureId);
                        }
                        catch { }

                        try
                        {
                            lastInspectionDate = dwDatabase.GetLastInspectionDateTime(w.StructureId).Date;
                        }
                        catch { }

                        ws2.Cell(ws1rc, 1).Value = p.ProjectDbId;
                        ws2.Cell(ws1rc, 2).Value = p.FiscalYear;
                        ws2.Cell(ws1rc, 3).Value = p.AdvanceableFiscalYear != 0 ? p.AdvanceableFiscalYear.ToString() : "";
                        ws2.Cell(ws1rc, 4).Value = p.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : p.Status.ToString();
                        ws2.Cell(ws1rc, 5).Value = w.StructureId;

                        if (structure != null)
                        {
                            ws2.Cell(ws1rc, 6).Value = structure.ProgrammingType;
                            ws2.Cell(ws1rc, 7).Value = structure.ServiceFeatureOn;
                            ws2.Cell(ws1rc, 8).Value = structure.ServiceFeatureUnder;
                            ws2.Cell(ws1rc, 9).Value = structure.GeoLocation.Region;
                            ws2.Cell(ws1rc, 10).Value = structure.GeoLocation.County;
                        }

                        //ws2.Cell(ws1rc, 11).Value = lastInspectionDate.Year != 1 ? lastInspectionDate.ToShortDateString() : "?";

                        //if (lastInspectionDate.Year != 1)
                        //{
                        //ws2.Cell(ws1rc, 11).Hyperlink = new XLHyperlink(lastInspectionFilePath);
                        //}

                        ws2.Cell(ws1rc, 11).Value = String.Format("({0}) {1}", w.CertifiedWorkConceptCode, w.CertifiedWorkConceptDescription);
                        ws2.Cell(ws1rc, 12).Value = w.Status == StructuresProgramType.WorkConceptStatus.Quasicertified ? "Transitionally Certified" : w.Status.ToString();
                        var fiipsMatches = fiips.Where(f => f.StructureId.Equals(w.StructureId) && f.FiscalYear >= currentFiscalYear - 2
                                                        && f.FosProjectId.Equals(p.FosProjectId)).OrderBy(o => o.FiscalYear);

                        if (fiipsMatches.Count() == 0)
                        {
                            ws2.Cell(ws1rc, 13).Value = "No";
                            ws2.Cell(ws1rc, 14).Value = "";
                            ws2.Cell(ws1rc, 15).Value = "";
                            ws2.Cell(ws1rc, 16).Value = "";
                            sctWorkConceptsNotInFiips.Add(w);
                        }
                        else
                        {
                            bool fiipsMatchWorkConcept = true;

                            // Is structure in a corresponding fiips project?
                            ws2.Cell(ws1rc, 13).Value = "Yes";

                            // Matches corresponding fiips scope?
                            if (fiipsMatches.Any(m => m.CertifiedWorkConceptCode.Equals(w.CertifiedWorkConceptCode)))
                            {
                                ws2.Cell(ws1rc, 14).Value = "Yes";
                            }
                            else
                            {
                                ws2.Cell(ws1rc, 14).Value = "No";
                                fiipsMatchWorkConcept = false;
                                sctWorkConceptsFiipsMismatches.Add(w);
                            }

                            // Matches corresponding fiips fy?
                            ws2.Cell(ws1rc, 15).Value = "Yes";

                            if (p.AdvanceableFiscalYear == 0)
                            {
                                if (correspondingFiipsProject != null) // Corresponding Fiips
                                {
                                    if (correspondingFiipsProject.FiscalYear != p.FiscalYear)
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }

                                    if (correspondingFiipsProject.EarliestAdvanceableLetDate.Year != 1
                                        && repo.GetFiscalYear(correspondingFiipsProject.EarliestAdvanceableLetDate) != p.FiscalYear) // Fiips advanceable
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                                else
                                {
                                    if (!fiipsMatches.Any(m => m.FiscalYear == p.FiscalYear))
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }

                                /*
                                if (correspondingFiipsProject != null && correspondingFiipsProject.EpseDate.Year != 1)
                                {
                                    if (p.FiscalYear <= correspondingFiipsProject.FiscalYear && p.FiscalYear >= correspondingFiipsProject.EpseDate.Year)
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                                else
                                {
                                    if (fiipsMatches.Any(m => m.FiscalYear == p.FiscalYear))
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }*/
                            }
                            else // SCT advanceable year
                            {
                                if (correspondingFiipsProject != null)
                                {
                                    if (correspondingFiipsProject.FiscalYear > p.FiscalYear
                                        || correspondingFiipsProject.FiscalYear < p.AdvanceableFiscalYear)
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }

                                    if (correspondingFiipsProject.EarliestAdvanceableLetDate.Year != 1)
                                    {
                                        int advanceableLetYear = repo.GetFiscalYear(correspondingFiipsProject.EarliestAdvanceableLetDate);

                                        if (advanceableLetYear > p.FiscalYear || advanceableLetYear < p.AdvanceableFiscalYear)
                                        {
                                            ws2.Cell(ws1rc, 15).Value = "No";
                                            fiipsMatchWorkConcept = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!fiipsMatches.Any(m => m.FiscalYear <= p.FiscalYear
                                                                && m.FiscalYear >= p.AdvanceableFiscalYear))
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                                /*
                                if (correspondingFiipsProject != null && correspondingFiipsProject.EpseDate.Year != 1)
                                {
                                    if (p.FiscalYear <= correspondingFiipsProject.FiscalYear && p.FiscalYear >= correspondingFiipsProject.EpseDate.Year
                                            && p.AdvanceableFiscalYear <= correspondingFiipsProject.FiscalYear && p.AdvanceableFiscalYear >= correspondingFiipsProject.EpseDate.Year)
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                                else
                                {
                                    if (fiipsMatches.Any(m => m.FiscalYear <= p.FiscalYear && m.FiscalYear >= p.AdvanceableFiscalYear))
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 15).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }*/
                            }

                            if (fiipsMatchWorkConcept)
                            {
                                ws2.Cell(ws1rc, 16).Value = "Yes";
                            }
                            else
                            {
                                ws2.Cell(ws1rc, 16).Value = "No";
                            }

                        }

                        /*
                        var matches = fiips.Where(f => f.StructureId.Equals(w.StructureId) && f.FiscalYear >= currentFiscalYear - 2).OrderBy(o => o.FiscalYear);

                        if (matches.Count() > 0) // In Fiips
                        {
                            ws2.Cell(ws1rc, 14).Value = "Yes";
                            bool fiipsMatchWorkConcept = true;

                            if (matches.Any(m => m.CertifiedWorkConceptCode.Equals(w.CertifiedWorkConceptCode)))
                            {
                                ws2.Cell(ws1rc, 15).Value = "Yes";
                            }
                            else
                            {
                                ws2.Cell(ws1rc, 15).Value = "No";
                                fiipsMatchWorkConcept = false;
                                sctWorkConceptsFiipsMismatches.Add(w);
                            }

                            
                            if (p.AdvanceableFiscalYear == 0)
                            {
                                if (correspondingFiipsProject != null && correspondingFiipsProject.EpseDate.Year != 1)
                                {
                                    if (p.FiscalYear <= correspondingFiipsProject.FiscalYear && p.FiscalYear >= correspondingFiipsProject.EpseDate.Year)
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                                else
                                {
                                    if (matches.Any(m => m.FiscalYear == p.FiscalYear))
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                            }
                            else
                            {
                                if (correspondingFiipsProject != null && correspondingFiipsProject.EpseDate.Year != 1)
                                {
                                    if (p.FiscalYear <= correspondingFiipsProject.FiscalYear && p.FiscalYear >= correspondingFiipsProject.EpseDate.Year
                                        && p.AdvanceableFiscalYear <= correspondingFiipsProject.FiscalYear && p.AdvanceableFiscalYear >= correspondingFiipsProject.EpseDate.Year)
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                                else
                                {
                                    if (matches.Any(m => m.FiscalYear <= p.FiscalYear && p.AdvanceableFiscalYear >= m.FiscalYear))
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "Yes";
                                    }
                                    else
                                    {
                                        ws2.Cell(ws1rc, 16).Value = "No";
                                        fiipsMatchWorkConcept = false;
                                    }
                                }
                            }

                            if (fiipsMatchWorkConcept)
                            {
                                ws2.Cell(ws1rc, 17).Value = "Yes";
                            }
                            else
                            {
                                ws2.Cell(ws1rc, 17).Value = "No";
                            }
                        }
                        else // Not in Fiips
                        {
                            ws2.Cell(ws1rc, 14).Value = "No";
                            ws2.Cell(ws1rc, 15).Value = "";
                            ws2.Cell(ws1rc, 16).Value = "";
                            ws2.Cell(ws1rc, 17).Value = "";
                            sctWorkConceptsNotInFiips.Add(w);
                        }*/

                        cc = 16;

                        // Do a group by to eliminate duplicates as a result of multiple funding sources
                        var fiipsWorkConcepts = fiipsMatches.GroupBy(g => new { g.WorkConceptDescription, g.FiscalYear, g.FosProjectId }).Select(g => g.First()).ToList();

                        if (correspondingFiipsProject != null)
                        {
                            fiipsWorkConcepts = fiipsMatches.GroupBy(g => new { g.WorkConceptDescription, g.FiscalYear, g.FosProjectId }).Select(g => g.First()).Where(g => g.FosProjectId.Equals(p.FosProjectId)).ToList();
                        }

                        //foreach (var m in fiipsMatches.GroupBy(g => new { g.WorkConceptDescription, g.FiscalYear, g.FosProjectId }).Select(g => g.First()).ToList())
                        foreach (var m in fiipsWorkConcepts)
                        {
                            //if (correspondingFiipsProject != null)
                            //if (matches.Where(n => n.FosProjectId.Equals(m.FosProjectId)).Count() == 1)
                            //if (m.WorkConceptCode.Equals(w.WorkConceptCode))
                            {
                                ws2.Cell(ws1rc, ++cc).Value = !String.IsNullOrEmpty(m.WorkConceptDescription) ? m.WorkConceptDescription : m.WorkConceptCode;
                                ws2.Cell(ws1rc, ++cc).Value = m.FiscalYear;
                                ws2.Cell(ws1rc, ++cc).Value = correspondingFiipsProject != null && correspondingFiipsProject.EarliestAdvanceableLetDate.Year != 1 ? repo.GetFiscalYear(correspondingFiipsProject.EarliestAdvanceableLetDate).ToString() : "";
                                ws2.Cell(ws1rc, ++cc).Value = correspondingFiipsProject != null && correspondingFiipsProject.PseDate.Year != 1 ? correspondingFiipsProject.PseDate.ToString("yyyy-MM-dd") : "";
                                ws2.Cell(ws1rc, ++cc).Value = correspondingFiipsProject != null && correspondingFiipsProject.EpseDate.Year != 1 ? correspondingFiipsProject.EpseDate.ToString("yyyy-MM-dd") : "";
                                ws2.Cell(ws1rc, ++cc).Value = m.FosProjectId;
                                ws2.Cell(ws1rc, ++cc).Value = "";

                                if (correspondingFiipsProject != null)
                                {
                                    ws2.Cell(ws1rc, cc).Value = correspondingFiipsProject.LifecycleStageCode;
                                }
                            }
                        }

                        ws1rc++;
                    }
                }
            } // end foreach (var region in regions)

            // Certified roll-up
            ws1.Cell(1, 1).Value = "SCT Work Concepts Summary";
            ws1.Range(1, 1, 1, 2).Merge();

            if (regions.Count == 1)
            {
                ws1.Cell(2, 1).Value = "Region: " + regions.First();
            }
            else
            {
                ws1.Cell(2, 1).Value = "Regions: " + String.Join(",", regions);
            }

            /*
            List<WorkConcept> certifiedWorkConceptsNotInFiips = new List<WorkConcept>();
            List<WorkConcept> certifiedWorkConceptsInFiipsMismatches = new List<WorkConcept>();
            */

            ws1.Range(2, 1, 2, 2).Merge();
            ws1.Cell(3, 1).Value = String.Format("FY: {0} - {1}", startFy, endFy);
            ws1.Range(3, 1, 3, 2).Merge();
            ws1.Cell(4, 1).Value = "Total Number of SCT Work Concepts:";
            ws1.Cell(4, 2).Value = totalNumberOfSctWorkConcepts;
            ws1.Cell(5, 1).Value = "Total Number of SCT Work Concepts NOT IN a Fiips Project:";
            ws1.Cell(5, 2).Value = sctWorkConceptsNotInFiips.Count();
            ws1.Cell(6, 1).Value = "Total Number of SCT Work Concepts IN a Fiips Project BUT Different Scope:";
            ws1.Cell(6, 2).Value = sctWorkConceptsFiipsMismatches.Count();
            #endregion Structures Projects

            #region Fiips
            int rc = 2; // Row counter
            int rcPrimary = 2;
            int rcSecondary = 2;
            var fiipsProjectsSubset = fiipsProjects.Where(fp => regions.Contains(fp.Region) && fp.FiscalYear >= startFy && fp.FiscalYear <= endFy && fp.WorkConcepts != null && fp.WorkConcepts.Count > 0);

            foreach (var p in fiipsProjectsSubset)
            {
                foreach (var w in p.WorkConcepts.OrderBy(wc => wc.StructureId).GroupBy(x => new { x.StructureId }).Select(g => g.First()))
                {
                    Dw.Structure structure = null;
                    string structureId = w.StructureId.Replace("-", "");

                    if (String.IsNullOrEmpty(structureId))
                    {
                        structureId = w.PlannedStructureId;
                    }

                    if (!String.IsNullOrEmpty(structureId) && !String.IsNullOrEmpty(w.WorkConceptDescription))
                    {
                        try
                        {
                            structure = fiipsStructures.Where(s => s.StructureId.Equals(structureId)).First();
                        }
                        catch { }

                        string lastInspectionFilePath = "";
                        DateTime lastInspectionDate = new DateTime();

                        try
                        {
                            lastInspectionFilePath = dwDatabase.GetLastInspectionFilePath(structureId);
                        }
                        catch { }

                        try
                        {
                            lastInspectionDate = dwDatabase.GetLastInspectionDateTime(structureId).Date;
                        }
                        catch { }

                        if (includeState && !includeLocal)
                        {
                            if (structure != null)
                            {
                                if (!structure.OwnerAgencyCode.Equals("10") && !structure.OwnerAgencyCode.Equals("16"))
                                {
                                    continue;
                                }
                            }
                        }
                        else if (!includeState && includeLocal)
                        {
                            if (structure != null)
                            {
                                if (structure.OwnerAgencyCode.Equals("10") || structure.OwnerAgencyCode.Equals("16"))
                                {
                                    continue;
                                }
                            }
                        }

                        var ws = ws4;
                        rc = rcPrimary;
                        totalNumberOfFiipsWorkConcepts++;

                        /*
                        if (primaryWorkConcepts.Any(pwc => pwc.WorkConceptCode.Equals(w.WorkConceptCode)))
                        {
                            
                        }
                        else
                        {
                            ws = ws5;
                            rc = rcSecondary;
                        }*/

                        ws.Cell(rc, 1).Value = p.FosProjectId;
                        ws.Cell(rc, 2).Value = p.FiscalYear;
                        ws.Cell(rc, 3).Value = p.EarliestAdvanceableLetDate.Year != 1 ? repo.GetFiscalYear(p.EarliestAdvanceableLetDate).ToString() : "";
                        ws.Cell(rc, 4).Value = p.PseDate.Year != 1 ? p.PseDate.ToString("yyyy-MM-dd") : "";
                        ws.Cell(rc, 5).Value = p.EpseDate.Year != 1 ? p.EpseDate.ToString("yyyy-MM-dd") : "";
                        ws.Cell(rc, 6).Value = p.LifecycleStageCode;
                        ws.Cell(rc, 7).Value = structureId;

                        if (structure != null)
                        {
                            ws.Cell(rc, 8).Value = structure.ProgrammingType;
                            ws.Cell(rc, 9).Value = structure.ServiceFeatureOn;
                            ws.Cell(rc, 10).Value = structure.ServiceFeatureUnder;
                            ws.Cell(rc, 11).Value = structure.GeoLocation.Region;
                            ws.Cell(rc, 12).Value = structure.GeoLocation.County;
                        }

                        //ws.Cell(rc, 11).Value = p.Region;
                        //ws.Cell(rc, 12).Value = structure != null ? structure.OwnerAgency : "UNKNOWN";
                        // ws.Cell(rc, 11).Value = lastInspectionDate.Year != 1 ? lastInspectionDate.ToShortDateString() : "NONE";

                        //if (lastInspectionDate.Year != 1)
                        //{
                        //ws.Cell(rc, 11).Hyperlink = new XLHyperlink(lastInspectionFilePath);
                        //}
                        try
                        {
                            ws.Cell(rc, 13).Value = w.StructureId.Substring(0, 1);
                        }
                        catch
                        {
                            ws.Cell(rc, 13).Value = "";
                        }

                        ws.Cell(rc, 14).Value = String.Format("({0}) {1}", w.WorkConceptCode, w.WorkConceptDescription);
                        List<WorkConcept> wcs = repo.GetAssociatedSctWorkConcepts(structureId, structuresProjects);

                        if (wcs.Count() > 0)
                        {
                            ws.Cell(rc, 15).Value = "Yes";

                            if (wcs.Any(wc => wc.CertifiedWorkConceptCode.Equals(w.WorkConceptCode)))
                            {
                                ws.Cell(rc, 16).Value = "Yes";
                            }
                            else
                            {
                                ws.Cell(rc, 16).Value = "No";
                                fiipsWorkConceptsSctMismatches.Add(w);
                            }

                            /*
                            if (wcs.Any(wc => wc.StructureProjectFiscalYear.Equals(w.FiscalYear)))
                            {
                                ws.Cell(rc, 11).Value = "Yes";
                            }
                            else
                            {
                                ws.Cell(rc, 11).Value = "No";
                            }*/
                        }
                        else
                        {
                            ws.Cell(rc, 15).Value = "No";
                            ws.Cell(rc, 16).Value = "";
                            //ws.Cell(rc, 11).Value = "";
                            fiipsWorkConceptsNotInSct.Add(w);
                        }

                        cc = 16;

                        foreach (var wc in wcs)
                        {
                            ws.Cell(rc, ++cc).Value = wc.CertifiedWorkConceptDescription;
                            ws.Cell(rc, ++cc).Value = wc.StructureProjectFiscalYear;
                            ws.Cell(rc, ++cc).Value = wc.ProjectDbId;

                            try
                            {
                                var strProj = structuresProjects.Where(s => s.ProjectDbId == wc.ProjectDbId).First();
                                ws.Cell(rc, ++cc).Value = strProj.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : strProj.Status.ToString();
                            }
                            catch { }
                        }

                        rcPrimary++;

                        /*
                        if (primaryWorkConcepts.Any(pwc => pwc.WorkConceptCode.Equals(w.WorkConceptCode)))
                        {
                            
                        }
                        else
                        {
                            rcSecondary++;
                        }*/
                    }
                }
            }

            ws3.Cell(1, 1).Value = "Fiips Work Concepts Summary";
            ws3.Range(1, 1, 1, 2).Merge();

            if (regions.Count == 1)
            {
                ws3.Cell(2, 1).Value = "Region: " + regions.First();
            }
            else
            {
                ws3.Cell(2, 1).Value = "Regions: " + String.Join(",", regions);
            }

            ws3.Range(2, 1, 2, 2).Merge();
            ws3.Cell(3, 1).Value = String.Format("FY: {0} - {1}", startFy, endFy);
            ws3.Range(3, 1, 3, 2).Merge();
            ws3.Cell(4, 1).Value = "Total Number of Fiips Work Concepts:";
            ws3.Cell(4, 2).Value = totalNumberOfFiipsWorkConcepts;
            ws3.Cell(5, 1).Value = "Total Number of Fiips Primary Concepts NOT IN an SCT Project:";
            ws3.Cell(5, 2).Value = fiipsWorkConceptsNotInSct.Count();
            ws3.Cell(6, 1).Value = "Total Number of Fiips Work Concepts IN an SCT Project BUT Different Scope:";
            ws3.Cell(6, 2).Value = fiipsWorkConceptsSctMismatches.Count();
            #endregion Fiips

            wb.SaveAs(outputFilePath);
            wb.Dispose();
        }

        public string WriteStructureCertificationHistory(Project project, List<WorkConcept> wcs, UserAccount account, DatabaseService database)
        {
            string excelFilePath = repo.GetRandomExcelFileName(@"c:\temp");
            if (!Directory.Exists(@"c:\temp"))
            {
                Directory.CreateDirectory(@"c:\temp");
            }
            if (Directory.Exists(@"c:\temp"))
            {
                XLWorkbook wb = new XLWorkbook();
                wb.AddWorksheet(String.Format("History-{0}", wcs.First().StructureId));
                var ws1 = wb.Worksheet(1);
                List<string> cols = new List<string>()
                {
                    "Structures Project Id", "Structure Id", "Transaction", "Transaction Date", "Work Concept to be Certified",
                    "Work Concept Status", "Work Concept Change Justification", "Work Concept Notes",
                    "Precert Decision", "Precert Reason Cat", "Precert Explanation"
                };
                if (account.IsPrecertificationLiaison || account.IsPrecertificationSupervisor
                    || account.IsCertificationLiaison || account.IsCertificationSupervisor
                    || account.IsSuperUser || account.IsAdministrator)
                {
                    cols.Add("Precertification Internal Comments");
                }
                cols.AddRange(new List<string> { "Certification Decision", "Primary Work Concept Comments",
                    "Secondary Work Concepts", "Secondary Work Concepts Comments", "Certification Additional Comments" });
                int cc = 1;
                foreach (var col in cols)
                {
                    ws1.Cell(1, cc).Value = col;
                    cc++;
                }
                int rc = 2;
                foreach (var wc in wcs)
                {
                    ws1.Cell(rc, 1).Value = project.ProjectDbId;
                    ws1.Cell(rc, 2).Value = wc.StructureId;
                    ws1.Cell(rc, 3).Value = String.Format("{0} by {1}", database.GetWorkflowTransaction(wc.ProjectUserAction), wc.ProjectUserFullName);
                    ws1.Cell(rc, 4).Value = wc.ProjectUserActionDateTime;
                    ws1.Cell(rc, 5).Value = wc.CertifiedWorkConceptDescription;
                    if (wc.ProjectUserAction == StructuresProgramType.ProjectUserAction.BosRejectedPrecertification)
                    {
                        ws1.Cell(rc, 6).Value = StructuresProgramType.WorkConceptStatus.Unapproved.ToString();
                    }
                    else
                    {
                        ws1.Cell(rc, 6).Value = wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified || wc.IsQuasicertified ? "Transitionally Certified" : wc.Status.ToString();
                    }
                    ws1.Cell(rc, 7).Value = wc.ChangeJustifications;

                    if (String.IsNullOrEmpty(wc.ChangeJustificationNotes))
                    {
                        ws1.Cell(rc, 8).Value = database.GetRegionNotes(wc);
                    }
                    else
                    {
                        ws1.Cell(rc, 8).Value = wc.ChangeJustificationNotes;
                    }
                    ws1.Cell(rc, 9).Value = wc.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.None ? "" : wc.PrecertificationDecision.ToString();
                    ws1.Cell(rc, 10).Value = wc.PrecertificationDecisionReasonCategory;
                    ws1.Cell(rc, 11).Value = wc.PrecertificationDecisionReasonExplanation;
                    int colCounter = 11;

                    if (account.IsPrecertificationLiaison || account.IsPrecertificationSupervisor
                    || account.IsCertificationLiaison || account.IsCertificationSupervisor
                    || account.IsSuperUser || account.IsAdministrator)
                    {
                        colCounter++;
                        ws1.Cell(rc, colCounter).Value = wc.PrecertificationDecisionInternalComments;
                    }
                    ws1.Cell(rc, ++colCounter).Value = wc.CertificationDecision;
                    ws1.Cell(rc, ++colCounter).Value = wc.CertificationPrimaryWorkTypeComments;
                    string secondaryWorkConcepts = "";
                    foreach (var pairing in project.CertifiedElementWorkConceptCombinations.Where(el => el.StructureId.Equals(wc.StructureId) && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")))
                    {
                        secondaryWorkConcepts += String.Format("({0}) {1}; ", pairing.WorkConceptCode, pairing.WorkConceptDescription);
                    }

                    ws1.Cell(rc, ++colCounter).Value = secondaryWorkConcepts;
                    ws1.Cell(rc, ++colCounter).Value = wc.CertificationSecondaryWorkTypeComments;
                    ws1.Cell(rc, ++colCounter).Value = wc.CertificationAdditionalComments;
                    rc++;
                }
                wb.SaveAs(excelFilePath);
                wb.Dispose();
            }
            return excelFilePath;
        }

        public void WriteStructuresGisReport(string outputFilePath, List<string> structureIds, List<WorkConcept> fiipsWorkConcepts, List<Project> structuresProjects)
        {
            Dw.Database warehouseDatabase = new Dw.Database();
            XLWorkbook wb = new XLWorkbook();
            wb.AddWorksheet("Structures");
            wb.AddWorksheet("Fiips");
            wb.AddWorksheet("Certified");
            var ws1 = wb.Worksheet("Structures");
            var ws2 = wb.Worksheet("Fiips");
            var ws3 = wb.Worksheet("Certified");

            List<string> ws1ColumnNames = new List<string>()
            {
                "Structure_ID", "Corridor_2030", "Region", "County", "Feature_On", "Feature_Under",
                "Structure_Type", "Material", "Number_Spans", "Tot_Len_Ft", "Inv_Rating",
                "Opr_Rating", "Load_Posting", "Const_Hist", "Lat", "Long",
                "Last_Insp", "Insp_Report", "Insp_Report_Full_Path", "NBI_Deck", "NBI_Super", "NBI_Sub", "NBI_Culv",
                "NBI_Waterway", "NBI_Channel", "Culv_Height", "Culv_Width", "NBI_Scour_Critical"
            };

            List<string> ws2ColumnNames = new List<string>()
            {
                "Str ID", "Wc", "Fy", "Fos ID"
            };

            List<string> ws3ColumnNames = new List<string>()
            {
                "Str ID", "Wc", "Fy", "Status", "Str Proj ID", "Fos ID"
            };

            int columnCounter = 1;
            int ws1RowCounter = 1;
            int ws2RowCounter = 1;
            int ws3RowCounter = 1;

            foreach (var ws1ColumnName in ws1ColumnNames)
            {
                ws1.Cell(ws1RowCounter, columnCounter).Value = ws1ColumnName;
                columnCounter++;
            }

            columnCounter--;

            for (int i = 2021; i <= 2032; i++)
            {
                ws1.Cell(1, ++columnCounter).Value = String.Format("FIIPS_Work_Concept_{0}", i);
                ws1.Cell(1, ++columnCounter).Value = String.Format("FIIPS_Project_ID_{0}", i);
            }

            for (int i = 2021; i <= 2032; i++)
            {
                ws1.Cell(1, ++columnCounter).Value = String.Format("Certified_Work_Concept_{0}", i);
            }

            ws1RowCounter++;
            ws2RowCounter++;
            ws3RowCounter++;
            int rowCounter = 1;
            int structureCounter = 0;
            foreach (var structureId in structureIds)
            {
                Dw.Structure str = null;
                try
                {
                    str = warehouseDatabase.GetStructure(structureId, true, true);
                }
                catch { }
                if (str != null)
                {
                    structureCounter++;
                    ws1.Cell(ws1RowCounter, 1).Value = str.StructureId;

                    try
                    {
                        ws1.Cell(ws1RowCounter, 2).Value = str.ProgrammingType;
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 2).Value = "None";
                    }

                    try
                    {
                        ws1.Cell(ws1RowCounter, 3).Value = str.GeoLocation.Region;
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 3).Value = "None";
                    }

                    try
                    {
                        ws1.Cell(ws1RowCounter, 4).Value = str.GeoLocation.County;
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 4).Value = "None";
                    }

                    try
                    {
                        ws1.Cell(ws1RowCounter, 5).Value = str.ServiceFeatureOn;
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 5).Value = "None";
                    }

                    try
                    {
                        ws1.Cell(ws1RowCounter, 6).Value = str.ServiceFeatureUnder;
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 6).Value = "None";
                    }

                    try
                    {
                        ws1.Cell(ws1RowCounter, 7).Value = str.StructureType;
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 7).Value = "None";
                    }

                    ws1.Cell(ws1RowCounter, 26).Value = "";
                    ws1.Cell(ws1RowCounter, 27).Value = "";
                    ws1.Cell(ws1RowCounter, 28).Value = "";

                    if (str is Dw.Bridge)
                    {
                        Dw.Bridge bridge = (Dw.Bridge)str;
                        bridge.LoadPosting = warehouseDatabase.GetLoadPosting(structureId);
                        str = (Dw.Bridge)str;

                        try
                        {
                            ws1.Cell(ws1RowCounter, 8).Value = bridge.Spans.Where(span => span.MainSpan).First().MaterialType;
                        }
                        catch { }

                        ws1.Cell(ws1RowCounter, 9).Value = bridge.NumberOfSpans;
                        ws1.Cell(ws1RowCounter, 10).Value = bridge.StructureLength;
                        ws1.Cell(ws1RowCounter, 11).Value = bridge.InventoryRating;
                        ws1.Cell(ws1RowCounter, 12).Value = bridge.OperatingRating;
                        ws1.Cell(ws1RowCounter, 13).Value = bridge.LoadPosting != null ? bridge.LoadPosting.LoadPostingDescription : "";
                        ws1.Cell(ws1RowCounter, 28).Value = bridge.ScourCriticalRating;
                    }
                    else if (str is Dw.Culvert)
                    {
                        Dw.Culvert culvert = (Dw.Culvert)str;
                        culvert.LoadPosting = warehouseDatabase.GetLoadPosting(structureId);

                        try
                        {
                            ws1.Cell(ws1RowCounter, 8).Value = culvert.Spans.Where(span => span.MainSpan).First().MaterialType;
                        }
                        catch { }

                        ws1.Cell(ws1RowCounter, 9).Value = culvert.NumberOfSpans;
                        ws1.Cell(ws1RowCounter, 10).Value = culvert.StructureLength;
                        ws1.Cell(ws1RowCounter, 11).Value = culvert.InventoryRating;
                        ws1.Cell(ws1RowCounter, 12).Value = culvert.OperatingRating;
                        ws1.Cell(ws1RowCounter, 13).Value = culvert.LoadPosting != null ? culvert.LoadPosting.LoadPostingDescription : "";
                        ws1.Cell(ws1RowCounter, 26).Value = culvert.Height;
                        ws1.Cell(ws1RowCounter, 27).Value = culvert.Width;
                    }
                    else
                    {
                        ws1.Cell(ws1RowCounter, 8).Value = "";
                        ws1.Cell(ws1RowCounter, 9).Value = "";
                        ws1.Cell(ws1RowCounter, 10).Value = "";
                        ws1.Cell(ws1RowCounter, 11).Value = "";
                        ws1.Cell(ws1RowCounter, 12).Value = "";
                        ws1.Cell(ws1RowCounter, 13).Value = "";
                    }

                    ws1.Cell(ws1RowCounter, 14).Value = str.ConstructionHistory;

                    try
                    {
                        //var latDecimal = Convert.ToSingle(ConvertDegreesMinutesSecondsToDecimalDegrees(str.Latitude.ToString()));
                        ws1.Cell(ws1RowCounter, 15).Value = str.GeoLocation.StartLatitude.ToString();
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 15).Value = "";
                    }

                    try
                    {
                        //var longitDecimal = Convert.ToSingle(ConvertDegreesMinutesSecondsToDecimalDegrees(str.Longitude.ToString()));
                        ws1.Cell(ws1RowCounter, 16).Value = (str.GeoLocation.StartLongitude).ToString();
                    }
                    catch
                    {
                        ws1.Cell(ws1RowCounter, 16).Value = "";
                    }

                    if (str.LastInspection != null)
                    {
                        ws1.Cell(ws1RowCounter, 17).Value = str.LastInspection.InspectionDate;

                        try
                        {
                            string link = warehouseDatabase.GetLastInspectionFilePath(structureId);
                            ws1.Cell(ws1RowCounter, 18).Value = "Insp Report";
                            ws1.Cell(ws1RowCounter, 18).Hyperlink = new XLHyperlink(link);
                            ws1.Cell(ws1RowCounter, 19).Value = link;
                        }
                        catch
                        {
                            ws1.Cell(ws1RowCounter, 18).Value = "None";
                            ws1.Cell(ws1RowCounter, 19).Value = "None";
                        }

                        ws1.Cell(ws1RowCounter, 20).Value = str.LastInspection.DeckRating;
                        ws1.Cell(ws1RowCounter, 21).Value = str.LastInspection.SuperstructureRating;
                        ws1.Cell(ws1RowCounter, 22).Value = str.LastInspection.SubstructureRating;
                        ws1.Cell(ws1RowCounter, 23).Value = str.LastInspection.CulvertRating;
                        ws1.Cell(ws1RowCounter, 24).Value = str.LastInspection.WaterwayRating;
                        ws1.Cell(ws1RowCounter, 25).Value = str.LastInspection.ChannelRating;
                    }
                    else
                    {
                        ws1.Cell(ws1RowCounter, 17).Value = "None";
                        ws1.Cell(ws1RowCounter, 18).Value = "None";
                        ws1.Cell(ws1RowCounter, 19).Value = "None";
                        ws1.Cell(ws1RowCounter, 20).Value = "";
                        ws1.Cell(ws1RowCounter, 21).Value = "";
                        ws1.Cell(ws1RowCounter, 22).Value = "";
                        ws1.Cell(ws1RowCounter, 23).Value = "";
                        ws1.Cell(ws1RowCounter, 24).Value = "";
                        ws1.Cell(ws1RowCounter, 25).Value = "";
                    }

                    columnCounter = 28;

                    for (int i = 2021; i <= 2032; i++)
                    {
                        var fiipsWcs = fiipsWorkConcepts.Where(f => f.StructureId.Equals(str.StructureId) && f.FiscalYear == i);
                        //var projects = structuresProjects.Where(p => p.Status == StructuresProgramType.ProjectStatus.Certified && p.FiscalYear == i && p.WorkConcepts.Any(w => w.StructureId.Equals(str.StructureId)));

                        if (fiipsWcs.Count() > 0)
                        {
                            var workConcept = fiipsWcs.First();
                            string wc = String.Format("({0}) {1}", workConcept.WorkConceptCode, workConcept.WorkConceptDescription);
                            // ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = wc;
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = workConcept.FosProjectId;
                        }
                        else
                        {
                            //ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                        }

                        /*
                        if (projects.Count() > 0)
                        {
                            var project = projects.First();
                            var workConcepts = project.WorkConcepts.Where(w => w.StructureId.Equals(str.StructureId));

                            if (workConcepts.Count() > 0)
                            {
                                var workConcept = workConcepts.First();
                                string wc = String.Format("({0}) {1}", workConcept.WorkConceptCode, workConcept.WorkConceptDescription);
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = wc;
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = workConcept.FosProjectId;
                            }
                            else
                            {
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                            }
                        }
                        else
                        {
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                        }*/
                    }

                    for (int i = 2021; i <= 2032; i++)
                    {
                        var projects = structuresProjects.Where(p => (p.Status == StructuresProgramType.ProjectStatus.Certified || p.Status == StructuresProgramType.ProjectStatus.QuasiCertified) && p.FiscalYear == i && p.WorkConcepts.Any(w => w.StructureId.Equals(str.StructureId)));

                        if (projects.Count() > 0)
                        {
                            var project = projects.First();
                            var workConcepts = project.WorkConcepts.Where(w => w.StructureId.Equals(str.StructureId));

                            if (workConcepts.Count() > 0)
                            {
                                var workConcept = workConcepts.First();
                                string wc = String.Format("({0}) {1}", workConcept.CertifiedWorkConceptCode, workConcept.CertifiedWorkConceptDescription);
                                //ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = wc;
                                //ws1.Cell(ws1RowCounter, ++columnCounter).Value = workConcept.FosProjectId;
                            }
                            else
                            {
                                //ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                                ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                                //ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                            }
                        }
                        else
                        {
                            //ws1.Cell(ws1RowCounter, ++columnCounter).Value = i;
                            ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                            //ws1.Cell(ws1RowCounter, ++columnCounter).Value = "";
                        }
                    }

                    ws1RowCounter++;
                }
                /*
                if (structureCounter == 500)
                {
                    break;
                }*/
                /*
                if (ws1RowCounter == 5)
                    break;*/

                /*
                var fiipsWcs = fiipsWorkConcepts.Where(f => f.StructureId.Equals(str.StructureId)).OrderBy(f => f.FiscalYear);
                foreach (var fiipsWc in fiipsWcs)
                {
                    ws2.Cell(ws2RowCounter, 1).Value = fiipsWc.StructureId;
                    ws2.Cell(ws2RowCounter, 2).Value = String.Format("({0}) {1}", fiipsWc.WorkConceptCode, fiipsWc.WorkConceptDescription);
                    ws2.Cell(ws2RowCounter, 3).Value = fiipsWc.FiscalYear;
                    ws2.Cell(ws2RowCounter, 4).Value = fiipsWc.FosProjectId;
                    ws2RowCounter++;
                }

                //"Str ID", "Wc", "Fy", "Status", "Str Proj ID", "Fos ID"
                var projs = structuresProjects.Where(p => p.Status == StructuresProgramType.ProjectStatus.Certified && p.WorkConcepts.Any(w => w.StructureId.Equals(str.StructureId)));
                foreach (var proj in projs)
                {
                    var wcs = proj.WorkConcepts.Where(w => w.StructureId.Equals(str.StructureId));
                    foreach (var wc in wcs)
                    {
                        try
                        {
                            ws3.Cell(ws3RowCounter, 1).Value = wc.StructureId;
                            ws3.Cell(ws3RowCounter, 2).Value = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                            ws3.Cell(ws3RowCounter, 3).Value = proj.FiscalYear;
                            ws3.Cell(ws3RowCounter, 4).Value = "Certified";
                            ws3.Cell(ws3RowCounter, 5).Value = proj.ProjectDbId;
                            ws3.Cell(ws3RowCounter, 6).Value = proj.FosProjectId;
                            ws3RowCounter++;
                        }
                        catch { }
                    }
                }*/
            }

            wb.SaveAs(outputFilePath);
            wb.Dispose();
        }
    }
}
