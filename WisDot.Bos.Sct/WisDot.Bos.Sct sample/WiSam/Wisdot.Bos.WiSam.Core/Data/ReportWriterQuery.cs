using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.WiSam.Core.Data.Interfaces;
using Wisdot.Bos.WiSam.Core.Domain.Models;

namespace Wisdot.Bos.WiSam.Core.Data
{
    public class ReportWriterQuery : IReportWriterQuery
    {
        public void WriteBridgeConditionReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, string outputFilePath, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType)
        {
            string currentLine = "StrID,Year,Str Age,Deck Curve,Sup Curve,Sub Curve,Prog-Sup Age,Prog-Deck Age,Prog-Olay Age,DN-Sup Age,DN-Deck Age,DN-Olay Age,DN-Elig,DN-Rule,DN-Cai,DN-Deck,DN-Sup,DN-Sub,DN-Cul";

            foreach (var elemNum in needsAnalysisInput.ElementsToReport)
            {
                for (int i = 1; i <= 4; i++)
                {
                    currentLine += String.Format(",DN-EL{0}-CS{1}", elemNum, i);
                }

                currentLine += String.Format(",DN-EL{0}-TOTAL", elemNum);
            }

            currentLine += ",Prog,Prog-Rule,Prog-Cai,Prog-Deck,Prog-Sup,Prog-Sub,Prog-Cul";

            foreach (var elemNum in needsAnalysisInput.ElementsToReport)
            {
                for (int i = 1; i <= 4; i++)
                {
                    currentLine += String.Format(",Prog-EL{0}-CS{1}", elemNum, i);
                }

                currentLine += String.Format(",Prog-EL{0}-TOTAL", elemNum);
            }

            currentLine += ",Fiips,Fiips-Cai,Fiips-Deck,Fiips-Sup,Fiips-Sub,Fiips-Cul";

            foreach (var elemNum in needsAnalysisInput.ElementsToReport)
            {
                for (int i = 1; i <= 4; i++)
                {
                    currentLine += String.Format(",Fiips-EL{0}-CS{1}", elemNum, i);
                }

                currentLine += String.Format(",Fiips-EL{0}-TOTAL", elemNum);
            }

            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine(currentLine); // column headers

                foreach (var structure in structures)
                {
                    //for (int year = needsAnalysisInput.AnalysisStartYear; year <= needsAnalysisInput.AnalysisEndYear; year++)
                    for (int year = structure.LastInspectionYear; year <= needsAnalysisInput.AnalysisEndYear; year++)
                    {
                        if (year == 2021)
                        {
                            var stop = true;
                        }
                        try
                        {
                            currentLine = String.Format("{0},{1},{2}", structure.StructureId, year, year - structure.YearBuiltActual);

                            // Det Curves
                            currentLine += String.Format(",{0},{1},{2}", !String.IsNullOrEmpty(structure.NbiDeckQualifiedDeteriorationCurve) ? structure.NbiDeckQualifiedDeteriorationCurve : "DEFAULT",
                                                                            !String.IsNullOrEmpty(structure.NbiSuperQualifiedDeteriorationCurve) ? structure.NbiSuperQualifiedDeteriorationCurve : "DEFAULT",
                                                                            !String.IsNullOrEmpty(structure.NbiSubQualifiedDeteriorationCurve) ? structure.NbiSubQualifiedDeteriorationCurve : "DEFAULT");

                            // Super, Deck, and Overlay Ages in the Program Scenario
                            if (structure.SuperBuilts.Count > 0)
                            {
                                int mostRecentSuper = structure.SuperBuilts.Where(e => e <= year).OrderByDescending(e => e).First();
                                currentLine += String.Format(",{0}", year - mostRecentSuper);
                            }
                            else
                            {
                                currentLine += String.Format(",{0}", "unknown");
                            }

                            if (structure.DeckBuilts.Count > 0)
                            {
                                int mostRecentDeck = structure.DeckBuilts.Where(e => e <= year).OrderByDescending(e => e).First();
                                currentLine += String.Format(",{0}", year - mostRecentDeck);
                            }
                            else
                            {
                                currentLine += String.Format(",{0}", "unknown");
                            }

                            if (structure.Overlays.Count > 0)
                            {
                                int mostRecentOverlay = structure.Overlays.Where(e => e <= year).OrderByDescending(e => e).First();
                                currentLine += String.Format(",{0}", year - mostRecentOverlay);
                            }
                            else
                            {
                                currentLine += String.Format(",{0}", "unknown");
                            }

                            // Super, Deck, and Overlay Ages in the Do-Nothing Scenario
                            currentLine += String.Format(",{0},{1},{2}", year - structure.LastSuperReplacementYear, year - structure.LastDeckReplacementYear, year - structure.LastOverlayYear);

                            // Repeat 3x: Do-Nothing, Optimal, FIIPS scenarios in this order
                            for (int i = 1; i <= 3; i++)
                            {
                                List<StructureWorkAction> workActions = null;
                                StructureWorkAction wa = null;
                                List<Element> allElements = new List<Element>();

                                if (structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == year).Count() == 0)
                                {
                                    allElements = structure.LastInspectionCai.AllElements;
                                    if (i == 1 || i == 2)
                                    {
                                        currentLine += String.Format(",{0},{1},{2}", "", "", structure.LastInspectionCai.CaiValue);
                                    }
                                    else
                                    {
                                        currentLine += String.Format(",{0},{1}", "", structure.LastInspectionCai.CaiValue);
                                    }
                                    currentLine += String.Format(",{0},{1},{2},{3}", structure.LastInspectionCai.NbiRatings.DeckRating, structure.LastInspectionCai.NbiRatings.SuperstructureRating, structure.LastInspectionCai.NbiRatings.SubstructureRating, structure.LastInspectionCai.NbiRatings.CulvertRating);
                                }
                                else
                                {
                                    if (i == 1)
                                    {
                                        workActions = structure.YearlyDoNothings.Where(e => e.WorkActionYear == year).ToList();
                                    }
                                    else if (i == 2)
                                    {
                                        if (structure.YearlyConstrainedOptimalWorkActions.Where(e => e.WorkActionYear == year).Count() == 0
                                            && structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == year).Count() == 0)
                                        {
                                            workActions = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == year).ToList();
                                        }
                                        else
                                        {
                                            if (needsAnalysisFileType == WisamType.NeedsAnalysisFileTypes.ConditionConstrained)
                                            {
                                                workActions = structure.YearlyConstrainedOptimalWorkActions.Where(e => e.WorkActionYear == year).ToList();
                                            }
                                            else if (needsAnalysisFileType == WisamType.NeedsAnalysisFileTypes.ConditionUnconstrained)
                                            {
                                                workActions = structure.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == year).ToList();
                                            }
                                        }
                                    }
                                    else if (i == 3)
                                    {
                                        workActions = structure.YearlyProgrammedWorkActions.Where(e => e.WorkActionYear == year).ToList();
                                    }
                                    wa = workActions.First();
                                    allElements = wa.CAI.AllElements;
                                    if (year == 2021)
                                    {
                                        var stop = true;
                                    }
                                    if (i == 1)
                                    {
                                        List<StructureWorkAction> doNothingOptimalWorkActions = structure.YearlyOptimalWorkActionsBasedOnDoNothingCondition
                                                                                            .Where(e => e.WorkActionYear == year).ToList();
                                        StructureWorkAction dnwa = doNothingOptimalWorkActions.First();
                                        currentLine += String.Format(",({0}){1},{2},{3}", dnwa.WorkActionCode, dnwa.WorkActionCode == Code.DoNothing ? dnwa.WorkActionDesc : dnwa.ControllingCriteria.WorkActionDesc, dnwa.WorkActionCode == Code.DoNothing ? "" : dnwa.ControllingCriteria.RuleId.ToString(), wa.CAI.CaiValue);
                                    }
                                    else if (i == 2)
                                    {
                                        if (wa.ControllingCriteria != null)
                                        {
                                            currentLine += String.Format(",({0}){1},{2},{3}", wa.WorkActionCode, wa.WorkActionCode == Code.DoNothing ? wa.WorkActionDesc : wa.ControllingCriteria.WorkActionDesc, wa.WorkActionCode == Code.DoNothing ? "" : wa.ControllingCriteria.RuleId.ToString(), wa.CAI.CaiValue);
                                        }
                                        else
                                        {
                                            currentLine += String.Format(",({0}){1},{2},{3}", wa.WorkActionCode, wa.WorkActionDesc, "", wa.CAI.CaiValue);
                                        }
                                    }
                                    else
                                    {
                                        currentLine += String.Format(",({0}){1},{2}", wa.WorkActionCode, wa.WorkActionDesc, wa.CAI.CaiValue);
                                    }

                                    currentLine += String.Format(",{0},{1},{2},{3}", wa.CAI.NbiRatings.DeckRating, wa.CAI.NbiRatings.SuperstructureRating, wa.CAI.NbiRatings.SubstructureRating, wa.CAI.NbiRatings.CulvertRating);
                                }

                                foreach (var elemNum in needsAnalysisInput.ElementsToReport)
                                {
                                    bool skipElement = false;
                                    string elementLine = "";

                                    try
                                    {
                                        if (elemNum == 8516 && year == 2020)
                                        {
                                            var stop = true;
                                        }
                                        var elements = allElements.Where(e => e.ElemNum == elemNum).ToList();

                                        if (elemNum == 1080)
                                        {
                                            elements = allElements.Where(e => e.ElemNum == elemNum && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("12")
                                                                                    || e.ParentElemNum.ToString().Equals("16")
                                                                                    || e.ParentElemNum.ToString().Equals("38")
                                                                                    || e.ParentElemNum.ToString().Equals("13")
                                                                                    || e.ParentElemNum.ToString().Equals("15")
                                                                                    || e.ParentElemNum.ToString().Equals("8039")
                                                                                    || e.ParentElemNum.ToString().Equals("60")
                                                                                    || e.ParentElemNum.ToString().Equals("65")
                                                                                )).ToList();
                                        }
                                        else if (elemNum == 1130)
                                        {
                                            elements = allElements.Where(e => e.ElemNum == elemNum && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("12")
                                                                                    || e.ParentElemNum.ToString().Equals("16")
                                                                                    || e.ParentElemNum.ToString().Equals("38")
                                                                                    || e.ParentElemNum.ToString().Equals("60")
                                                                                    || e.ParentElemNum.ToString().Equals("65")
                                                                                )).ToList();
                                        }
                                        else if (elemNum == 3440)
                                        {
                                            elements = allElements.Where(e => e.ElemNum == elemNum && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("8516")

                                                                                )).ToList();
                                        }
                                        else if (elemNum == 1000)
                                        {
                                            elements = allElements.Where(e => e.ElemNum == elemNum && e.ElementClassificationCode.Equals(Code.Defect) &&
                                                                                (e.ParentElemNum.ToString().Equals("330")

                                                                                )).ToList();
                                        }
                                        else if (elemNum == 8516)
                                        {
                                            // steel deck and superstructure elements only for the parents
                                            elements = allElements.Where(e => e.ElemNum == elemNum &&
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
                                            elementLine += String.Format(",{0}", element.Cs1Quantity);
                                            elementLine += String.Format(",{0}", element.Cs2Quantity);
                                            elementLine += String.Format(",{0}", element.Cs3Quantity);
                                            elementLine += String.Format(",{0}", element.Cs4Quantity);
                                            elementLine += String.Format(",{0}", element.TotalQuantity);
                                        }
                                        else
                                        {
                                            elementLine += String.Format(",0,0,0,0,0");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        skipElement = true;
                                    }

                                    if (skipElement)
                                    {
                                        elementLine = ",N,N,N,N,N";
                                    }

                                    currentLine += elementLine;
                                }
                            }

                            sw.WriteLine(currentLine);
                        }
                        catch (Exception ex)
                        {
                            sw.WriteLine(String.Format("{0},{1},{2}", structure.StructureId, year, ex.InnerException));
                        }
                    }

                }
            }
        }

        public void WriteBridgeInventoryReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, string outputFilePath)
        {
            int strCounter = 1;

            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                //Type myType = structures[0].GetType();
                string currentLine = "";
                var props = structures[0].GetType().GetProperties();

                foreach (Structure structure in structures)
                {
                    int propCounter = 1;

                    if (strCounter == 1)
                    {
                        foreach (var prop in props)
                        {
                            if (propCounter > 1)
                            {
                                currentLine += ",";
                            }

                            currentLine += String.Format("{0}", prop.Name);
                            propCounter++;
                        }
                        sw.WriteLine(currentLine);
                        currentLine = "";
                    }

                    propCounter = 1;
                    foreach (var prop in props)
                    {
                        if (propCounter > 1)
                        {
                            currentLine += ",";
                        }

                        var propValue = prop.GetValue(structure, null);

                        if (propValue != null)
                        {
                            currentLine += String.Format("{0}", prop.GetValue(structure, null).ToString().Trim('\r', '\n').Replace(",", ""));
                        }
                        else
                        {
                            currentLine += String.Format("{0}", prop.GetValue(structure, null));
                        }

                        propCounter++;
                    }

                    sw.WriteLine(currentLine);
                    strCounter++;
                    currentLine = "";
                }
            }
        }

        public void WriteCoreDataReport(string outputFilePath, List<Structure> structures, List<string> notFoundIds)
        {
            string currentLine = "";

            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                currentLine = "Str ID,Insp Date,Year Built,Last Super,Last Deck,Last Olay,Str Age,Super Age,Deck Age," +
                                "Olay Age,Elem Num,Elem Desc,Envr Num,Unit,Tot Qty,CS1,CS2,CS3,CS4,CS5,CS Num";
                sw.WriteLine(currentLine);

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
                                currentLine = insp.StructureId;
                                currentLine += "," + String.Format("{0:MM/dd/yyyy}", insp.InspectionDate);
                                currentLine += "," + str.YearBuiltActual;
                                currentLine += "," + str.LastSuperReplacementYear;
                                currentLine += "," + str.LastDeckReplacementYear;

                                if (lastOlay == 0)
                                {
                                    currentLine += ",-1";
                                }
                                else
                                {
                                    currentLine += "," + lastOlay;
                                }

                                currentLine += "," + String.Format("{0}", inspYear - str.YearBuiltActual);
                                currentLine += "," + String.Format("{0}", inspYear - lastSuper);
                                currentLine += "," + String.Format("{0}", inspYear - lastDeck);

                                if (lastOlay == 0)
                                {
                                    currentLine += ",-1";
                                }
                                else
                                {
                                    currentLine += "," + String.Format("{0}", inspYear - lastOlay);
                                }

                                currentLine += "," + elem.ElemNum;
                                currentLine += "," + elem.ElemName;
                                currentLine += "," + elem.EnvironmentNumber;
                                currentLine += "," + elem.UnitOfMeasurement;
                                currentLine += "," + elem.TotalQuantity;
                                currentLine += "," + elem.Cs1Quantity;

                                if (elem.StateCount >= 2)
                                {
                                    currentLine += "," + elem.Cs2Quantity;
                                }
                                else
                                {
                                    currentLine = ",0";
                                }

                                if (elem.StateCount >= 3)
                                {
                                    currentLine += "," + elem.Cs3Quantity;
                                }
                                else
                                {
                                    currentLine += ",0";
                                }

                                if (elem.StateCount >= 4)
                                {
                                    currentLine += "," + elem.Cs4Quantity;
                                }
                                else
                                {
                                    currentLine += ",0";
                                }

                                if (elem.StateCount >= 5)
                                {
                                    currentLine += "," + elem.Cs5Quantity;
                                }
                                else
                                {
                                    currentLine += ",0";
                                }

                                currentLine += "," + elem.StateCount;
                                sw.WriteLine(currentLine);
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public void WriteLetDatesReport(List<ProgrammedWorkAction> pwas, string outputFilePath)
        {
            string currentLine = "Rgn,Cnty,FOS Project ID,Work Type,Existing Str,New Str,Let Date,FY,Earliest Adv Date,Latest Adv Date,Lifecycle,Lifecycle Date,Design Proj ID,PSE Date,Adv PSE Date,Feat On,Feat Und";
            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine(currentLine);
                var pwasOrderByLetDate = pwas.OrderBy(e => e.EstimatedCompletionDate);

                foreach (var pwa in pwasOrderByLetDate)
                {
                    currentLine = String.Format("{0}", pwa.DotRegionCode);
                    currentLine += String.Format(",{0}", pwa.County);
                    currentLine += String.Format(",{0}", pwa.FosProjId);
                    currentLine += String.Format(",({0}){1}", pwa.OriginalWorkActionCode, pwa.OriginalWorkActionDesc);
                    currentLine += String.Format(",{0}", pwa.StructureId);
                    currentLine += String.Format(",{0}", pwa.NewStructureId);

                    if (pwa.EstimatedCompletionDate.Year != 1)
                        currentLine += String.Format(",{0:MM/dd/yyyy}", pwa.EstimatedCompletionDate);
                    else
                        currentLine += String.Format(",{0}", "");

                    currentLine += String.Format(",{0}", pwa.StateFiscalYear);

                    if (pwa.EarliestAdvanceableLetDate.Year != 1)
                        currentLine += String.Format(",{0:MM/dd/yyyy}", pwa.EarliestAdvanceableLetDate);
                    else
                        currentLine += String.Format(",{0}", "");

                    if (pwa.LatestAdvanceableLetDate.Year != 1)
                        currentLine += String.Format(",{0:MM/dd/yyyy}", pwa.LatestAdvanceableLetDate);
                    else
                        currentLine += String.Format(",{0}", "");

                    currentLine += String.Format(",{0}", pwa.LifeCycleStageCode);

                    if (pwa.LifeCycleStageDate.Year != 1)
                        currentLine += String.Format(",{0:MM/dd/yyyy}", pwa.LifeCycleStageDate);
                    else
                        currentLine += String.Format(",{0}", "");

                    currentLine += String.Format(",{0}", pwa.DesignProjectId);

                    if (pwa.PseDate.Year != 1)
                        currentLine += String.Format(",{0:MM/dd/yyyy}", pwa.PseDate);
                    else
                        currentLine += String.Format(",{0}", "");

                    if (pwa.EarliestPseDate.Year != 1)
                        currentLine += String.Format(",{0:MM/dd/yyyy}", pwa.EarliestPseDate);
                    else
                        currentLine += String.Format(",{0}", "");

                    if (pwa.FeatureOn != null && pwa.FeatureOn.Length > 0)
                    {
                        currentLine += String.Format(",{0}", pwa.FeatureOn);
                    }
                    else
                    {
                        currentLine += String.Format(",{0}", "");
                    }

                    if (pwa.FeatureUnder != null && pwa.FeatureUnder.Length > 0)
                    {
                        currentLine += String.Format(",{0}", pwa.FeatureUnder);
                    }
                    else
                    {
                        currentLine += String.Format(",{0}", "");
                    }

                    sw.WriteLine(currentLine);
                }
            }
        }

        public void WritePriorityScoreReport(WisamType.AnalysisReports report, List<Structure> structures, int startYear, int endYear, string outputFilePath, List<PriorityIndexCategory> priorityIndexCategories, List<PriorityIndexFactor> priorityIndexFactors)
        {
            string currentLine = "Structure ID,Year,PI Total Score";
            foreach (var cat in priorityIndexCategories)
            {
                currentLine += String.Format(",{0} Score", cat.PriorityIndexCategoryName);
            }

            foreach (var cat in priorityIndexCategories)
            {
                List<PriorityIndexFactor> factors = priorityIndexFactors.Where(e => e.PriorityIndexCategoryKey == cat.PriorityIndexCategoryKey).ToList();
                foreach (var factor in factors)
                {
                    currentLine += ",Factor Name";
                    currentLine += ",FieldVal";
                    currentLine += ",IndexVal";
                    currentLine += ",Weight";
                    currentLine += ",Score";
                }
            }

            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine(currentLine); // column headers

                for (int year = startYear; year <= endYear; year++)
                {
                    foreach (var structure in structures)
                    {
                        currentLine = "";

                        try
                        {
                            currentLine += String.Format("{0},{1}", structure.StructureId, year);
                            var cats = structure.PriorityIndexCategories.Where(e => e.Year == year).ToList();
                            currentLine += String.Format(",{0}", cats.Sum(e => e.Score));

                            foreach (var cat in cats)
                            {
                                currentLine += String.Format(",{0}", cat.Score);
                            }

                            foreach (var cat in cats)
                            {
                                var factors = structure.PriorityIndexFactors.Where(e => e.PriorityIndexCategoryKey == cat.PriorityIndexCategoryKey && e.Year == year).ToList();
                                foreach (var factor in factors)
                                {
                                    currentLine += String.Format(",{0},{1}", factor.PriorityIndexFactorId, factor.FieldValue);
                                    currentLine += String.Format(",{0},{1},{2}", factor.IndexValue, factor.PriorityIndexFactorWeight, factor.Score);
                                }
                            }
                        }
                        catch (Exception ex)
                        { }

                        sw.WriteLine(currentLine);
                    }
                }
            }
        }

        public void WriteProgram(WisamType.NeedsAnalysisFileTypes fileType, WisamType.AnalysisTypes analysisType, List<Structure> structures, int startYear, int endYear, string outputFilePath)
        {
            string currentLine = "Str,2030 Corr,Region,County,Feat On,Feat Und,Str Type,Matl,Num Spans,Tot Len,Inv Rat,Opr Rat,Load Pos,Last Insp,Const Hist,Year,Str Age,Sup Age,Deck Age,Olay Age,DN CAI,DN NBI Deck,DN NBI Sup,DN NBI Sub,DN NBI Cul,Primary WA,Primary CAI,Primary Cost,Life Ext,Incidental,FIIPS WA,FIIPS CAI,Cost(w/o del),Proj ID,Proj Cncpt,Dot Prog,FIIPS NBI Deck,FIIPS NBI Sup,FIIPS NBI Sub,FIIPS NBI Cul,DN Primary WA,NBI Interpolated Bounded,NBI Inspected,NBI Deck,NBI Sup,NBI Sub,NBI Cul";

            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine(currentLine);

                foreach (Structure str in structures)
                {
                    foreach (var dn in str.YearlyDoNothings.Where(e => e.WorkActionYear >= startYear))
                    {
                        currentLine = String.Format("{0},{1},{2},{3}", str.StructureId, str.CorridorCode, str.Region, str.CorridorCode);
                        currentLine += String.Format(",{0},{1},{2},{3}", str.FeatureOn, str.FeatureUnder, str.StructureType, str.MainSpanMaterial);
                        currentLine += String.Format(",{0},{1},{2},{3}", str.NumSpans, str.TotalLengthSpans, str.InventoryRating, str.OperatingRating);
                        currentLine += String.Format(",{0},{1},{2}", str.LoadPostingCode != 0 ? str.LoadPostingDesc : "None",
                                                                         str.LastInspection.InspectionDate, str.ConstructionHistory);
                        int year = dn.WorkActionYear;
                        int superBuiltYear = str.YearBuiltActual;
                        int deckBuiltYear = str.YearBuiltActual;
                        int lastOlayYear = 0;

                        if (str.SuperBuilts.Count > 0)
                        {
                            superBuiltYear = str.SuperBuilts.Where(e => e <= year).OrderByDescending(e => e).First();
                        }

                        if (str.DeckBuilts.Count > 0)
                        {
                            deckBuiltYear = str.DeckBuilts.Where(e => e <= year).OrderByDescending(e => e).First();
                        }

                        if (str.Overlays.Count > 0)
                        {
                            lastOlayYear = str.Overlays.Where(e => e <= year).OrderByDescending(e => e).First();
                        }

                        currentLine += String.Format(",{0}", year);
                        currentLine += String.Format(",{0},{1},{2},{3}", year - str.YearBuiltActual, year - superBuiltYear, year - deckBuiltYear, lastOlayYear == 0 ? "" : (year - lastOlayYear).ToString());

                        if (dn.CAI != null)
                        {
                            double caiValue = Math.Round(dn.CAI.CaiValue, 1);
                            double nbiDeck = -1;
                            double nbiSup = -1;
                            double nbiSub = -1;
                            double nbiCulv = -1;

                            if (str.StructureType.Equals("BOX CULVERT") || str.StructureType.ToUpper().Contains("CULVERT"))
                            {
                                try
                                {
                                    nbiCulv = Math.Round(dn.CAI.NbiRatings.CulvertRatingVal, 2);
                                }
                                catch { }
                            }
                            else
                            {
                                try
                                {
                                    nbiDeck = Math.Round(dn.CAI.NbiRatings.DeckRatingVal, 2);
                                }
                                catch { }

                                try
                                {
                                    nbiSup = Math.Round(dn.CAI.NbiRatings.SuperstructureRatingVal, 2);
                                }
                                catch { }

                                try
                                {
                                    nbiSub = Math.Round(dn.CAI.NbiRatings.SubstructureRatingVal, 2);
                                }
                                catch { }
                            }

                            currentLine += String.Format(",{0},{1},{2},{3},{4}", caiValue, nbiDeck, nbiSup, nbiSub, nbiCulv);
                        }
                        else
                        {
                            currentLine += String.Format(",{0},{1},{2},{3},{4}", "", "", "", "", "");
                        }

                        sw.WriteLine(currentLine);

                        StructureWorkAction optimalWa = null;
                        try
                        {
                            if (fileType == WisamType.NeedsAnalysisFileTypes.ProgramUnconstrained)
                            {
                                optimalWa = str.YearlyOptimalWorkActions.Where(e => e.WorkActionYear == year).First();
                            }
                            else if (fileType == WisamType.NeedsAnalysisFileTypes.ProgramConstrained)
                            {
                                optimalWa = str.YearlyConstrainedOptimalWorkActions.Where(e => e.WorkActionYear == year).First();
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
    }
}
