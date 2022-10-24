using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.WiSam.Core.Domain.Models;

namespace Wisdot.Bos.WiSam.Core.Domain.Services.Interfaces
{
    interface IReportWriterService
    {
        void WriteLetDatesReport(List<ProgrammedWorkAction> pwas, string outputFilePath);
        void WriteBridgeInventoryReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, string outputFilePath);
        void WriteBridgeConditionReport(List<Structure> structures, NeedsAnalysisInput needsAnalysisInput, string outputFilePath, WisamType.NeedsAnalysisFileTypes needsAnalysisFileType);
        void WriteProgram(WisamType.NeedsAnalysisFileTypes fileType, WisamType.AnalysisTypes analysisType, List<Structure> structures, int startYear, int endYear, string outputFilePath);
        void WritePriorityScoreReport(WisamType.AnalysisReports report, List<Structure> structures,
                                            int startYear, int endYear, string outputFilePath,
                                            List<PriorityIndexCategory> priorityIndexCategories,
                                            List<PriorityIndexFactor> priorityIndexFactors);
        void WriteCoreDataReport(string outputFilePath, List<Structure> structures, List<string> notFoundIds);

    }
}
