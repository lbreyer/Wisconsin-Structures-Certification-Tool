using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Data;
using Wisdot.Bos.WiSam.Core.Data.Interfaces;

namespace Wisdot.Bos.WiSam.Core.Domain.Services
{
    public class ReportWriterService
    {
        private static IReportWriterQuery query = new ReportWriterQuery();

        /*
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
        */
        public static void WriteLetDatesReport(List<ProgrammedWorkAction> pwas, string outputFilePath)
        {
            query.WriteLetDatesReport(pwas, outputFilePath);
        }

        public static void WriteBridgeInventoryReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, string outputFilePath)
        {
            query.WriteBridgeInventoryReport(structures, needsAnalysisInput, outputFilePath);
        }

        public static void WriteBridgeConditionReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, string outputFilePath, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType)
        {
            query.WriteBridgeConditionReport(structures, needsAnalysisInput, outputFilePath, needsAnalysisFileType);
        }

        public static void WriteProgram(WisamType.NeedsAnalysisFileTypes fileType, WisamType.AnalysisTypes analysisType, List<Structure> structures, int startYear, int endYear, string outputFilePath)
        {
            query.WriteProgram(fileType, analysisType, structures, startYear, endYear, outputFilePath);
        }

        public static void WritePriorityScoreReport(WisamType.AnalysisReports report, List<Structure> structures,
                                            int startYear, int endYear, string outputFilePath,
                                            List<PriorityIndexCategory> priorityIndexCategories,
                                            List<PriorityIndexFactor> priorityIndexFactors)
        {
            query.WritePriorityScoreReport(report, structures, startYear, endYear, outputFilePath, priorityIndexCategories, priorityIndexFactors);
        }

        public static void WriteCoreDataReport(string outputFilePath, List<Structure> structures, List<string> notFoundIds)
        {
            query.WriteCoreDataReport(outputFilePath, structures, notFoundIds);
        }
    }
}
