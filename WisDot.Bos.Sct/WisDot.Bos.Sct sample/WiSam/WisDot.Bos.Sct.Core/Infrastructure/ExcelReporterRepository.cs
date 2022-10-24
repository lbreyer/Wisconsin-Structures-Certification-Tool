using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using System.IO;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class ExcelReporterRepository : IExcelReporterRepository
    {
        public List<StructureMaintenanceItem> CompareToWisamsNeedsList(string structureId, List<Dw.StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws)
        {
            List<Dw.StructureMaintenanceItem> wisamsMaintenanceItems = new List<Dw.StructureMaintenanceItem>();
            var rows = ws.RowsUsed(r => r.Cell("A").GetString() == structureId);

            if (rows.Count() > 0)
            {
                Dw.StructureMaintenanceItem firstHsiMaintenanceItem = hsiMaintenanceItems.First();

                foreach (var row in rows)
                {
                    string itemDescription = row.Cell("H").Value.ToString();
                    string criteriaMet = row.Cell("E").Value.ToString();
                    string itemCode = criteriaMet.Split(new string[] { ";", "(", ")" }, StringSplitOptions.RemoveEmptyEntries).First().Trim();
                    string priority = row.Cell("I").Value.ToString();
                    string estimatedCost = row.Cell("K").Value.ToString();
                    string constructionHistory = row.Cell("L").Value.ToString();
                    Dw.StructureMaintenanceItem w = new Dw.StructureMaintenanceItem(structureId);
                    w.ItemCode = itemCode;
                    w.ItemDescription = itemDescription;
                    w.Source = "WISAMS";
                    w.Priority = priority;
                    w.EstimatedCost = estimatedCost;
                    w.EstimatedDate = Convert.ToDateTime(row.Cell("G").Value);
                    w.Region = firstHsiMaintenanceItem.Region;
                    w.County = firstHsiMaintenanceItem.County;
                    w.OwnerAgency = firstHsiMaintenanceItem.OwnerAgency;
                    w.MaintainingAgency = firstHsiMaintenanceItem.MaintainingAgency;
                    w.Municipality = firstHsiMaintenanceItem.Municipality;
                    w.FeatureOn = firstHsiMaintenanceItem.FeatureOn;
                    w.FeatureUnder = firstHsiMaintenanceItem.FeatureUnder;
                    w.OverallLength = firstHsiMaintenanceItem.OverallLength;
                    w.DeckArea = firstHsiMaintenanceItem.DeckArea;
                    w.RoadwayWidth = firstHsiMaintenanceItem.RoadwayWidth;
                    w.LocationOn = firstHsiMaintenanceItem.LocationOn;
                    w.ConstructionHistory = constructionHistory;
                    wisamsMaintenanceItems.Add(w);
                }

                foreach (var h in hsiMaintenanceItems)
                {
                    bool isSimilar = false;
                    var currentItemCode = h.ItemCode;

                    switch (currentItemCode)
                    {
                        case "58":
                        case "3":
                        case "7":
                        case "I7":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("4")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("4"));
                            }
                            break;
                        case "14":
                        case "16":
                        case "15":
                        case "12":
                        case "13":
                        case "65":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("10")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("10"));
                            }
                            break;
                        case "67":
                        case "35":
                        case "J7":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("12")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("12"));
                            }
                            break;
                        case "17":
                        case "24":
                        case "22":
                        case "18":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("14")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("14"));
                            }
                            break;
                        case "10":
                        case "62":
                        case "11":
                        case "53":
                        case "Z1":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("29")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("29"));
                            }
                            break;
                        case "43":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("35")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("35"));
                            }
                            break;
                        case "36":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("49")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("49"));
                            }
                            break;
                        case "26":
                        case "68":
                        case "27":
                        case "25":
                        case "G7":
                        case "G6":
                        case "F8":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("66")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("66"));
                            }
                            break;
                        case "31":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("75")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("75"));
                            }
                            break;
                        case "52":
                        case "59":
                        case "30":
                        case "57":
                        case "66":
                        case "34":
                        case "28":
                        case "48":
                        case "29":
                        case "51":
                            if (wisamsMaintenanceItems.Where(w => w.ItemCode.Equals("94")).Count() > 0)
                            {
                                isSimilar = true;
                                h.Source += ", WISAMS";
                            }

                            if (isSimilar)
                            {
                                wisamsMaintenanceItems.RemoveAll(w => w.ItemCode.Equals("94"));
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return wisamsMaintenanceItems;
        }

        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            int length = degreesMinutesSeconds.Length;
            float degrees = Convert.ToSingle(degreesMinutesSeconds.Substring(0, 2));
            float minutes = Convert.ToSingle(degreesMinutesSeconds.Substring(2, 2)) / 60;
            float seconds = Convert.ToSingle(degreesMinutesSeconds.Substring(4, length - 4)) / 3600;
            float decimalValue = minutes + seconds;
            return (degrees + decimalValue).ToString();
        }

        public List<WorkConcept> GetAssociatedCertifiedWorkConcepts(string structureId, List<Project> projs)
        {
            List<WorkConcept> wcs = new List<WorkConcept>();
            foreach (var p in projs)
            {
                foreach (var w in p.WorkConcepts)
                {
                    if (w.StructureId.Equals(structureId) && (w.IsQuasicertified || w.Status == StructuresProgramType.WorkConceptStatus.Certified))
                    {
                        wcs.Add(w);
                    }
                }
            }

            return wcs;
        }

        public List<WorkConcept> GetAssociatedSctWorkConcepts(string structureId, List<Project> projects)
        {
            List<WorkConcept> wcs = new List<WorkConcept>();

            foreach (var p in projects)
            {
                if (p.WorkConcepts != null)
                {
                    foreach (var w in p.WorkConcepts)
                    {
                        try
                        {
                            if (w.StructureId.Equals(structureId))
                            {
                                wcs.Add(w);
                            }
                        }
                        catch { }
                    }
                }
            }

            return wcs;
        }

        public int GetFiscalYear(DateTime date)
        {
            int fiscalYear = date.Year;

            if (DateTime.Compare(date, new DateTime(fiscalYear, 7, 1)) >= 0)
            {
                fiscalYear = fiscalYear + 1;
            }

            return fiscalYear;
        }

        public string GetRandomExcelFileName(string baseDir)
        {
            string newPath = Path.GetRandomFileName();
            string fileExt = Path.GetExtension(newPath);
            newPath = newPath.Replace(fileExt, ".xlsx");
            newPath = Path.Combine(baseDir, newPath);
            return newPath;
        }

        public List<StructureMaintenanceItem> GetWisamsNeedsListNotInHsi(Dw.Database dwDatabase, List<Dw.StructureMaintenanceItem> hsiMaintenanceItems, IXLWorksheet ws)
        {
            List<Dw.StructureMaintenanceItem> wisamsMaintenanceItems = new List<Dw.StructureMaintenanceItem>();
            List<string> hsiStructureIds = hsiMaintenanceItems.GroupBy(item => item.StructureId).Select(g => g.First().StructureId).ToList();
            int rowCounter = 0;

            foreach (var row in ws.RowsUsed())
            {
                if (rowCounter == 0)
                {
                    rowCounter++;
                    continue;
                }

                var wisamsStructureId = row.Cell("A").Value.ToString().Trim();

                if (!hsiStructureIds.Contains(wisamsStructureId))
                {
                    string itemDescription = row.Cell("H").Value.ToString();
                    string criteriaMet = row.Cell("E").Value.ToString();
                    string itemCode = criteriaMet.Split(new string[] { ";", "(", ")" }, StringSplitOptions.RemoveEmptyEntries).First().Trim();
                    string priority = row.Cell("I").Value.ToString();
                    string estimatedCost = row.Cell("K").Value.ToString();
                    string constructionHistory = row.Cell("L").Value.ToString();
                    Dw.StructureMaintenanceItem w = new Dw.StructureMaintenanceItem(wisamsStructureId);
                    w.ItemCode = itemCode;
                    w.ItemDescription = itemDescription;
                    w.Source = "WISAMS";
                    w.Priority = priority;
                    w.EstimatedCost = estimatedCost;
                    w.EstimatedDate = Convert.ToDateTime(row.Cell("G").Value);
                    Dw.Structure str = dwDatabase.GetStructure(wisamsStructureId);
                    w.County = str.GeoLocation.County;
                    w.Region = str.GeoLocation.Region;
                    w.FeatureOn = str.ServiceFeatureOn;
                    w.FeatureUnder = str.ServiceFeatureUnder;
                    w.DeckArea = str.DeckArea;
                    w.OverallLength = str.StructureLength;
                    w.RoadwayWidth = str.RoadwayWidth;
                    w.WisamsOnly = true;
                    wisamsMaintenanceItems.Add(w);
                }
            }

            return wisamsMaintenanceItems.OrderBy(w => w.StructureId).ToList();
        }
    }
}
