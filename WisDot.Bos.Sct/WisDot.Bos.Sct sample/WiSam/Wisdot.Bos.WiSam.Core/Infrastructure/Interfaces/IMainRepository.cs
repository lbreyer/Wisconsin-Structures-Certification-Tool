using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wisdot.Bos.WiSam.Core.Domain.Models;

namespace Wisdot.Bos.WiSam.Core.Infrastructure.Interfaces
{
    public interface IMainRepository
    {
        LinkLabel WriteResult(int fileCounter, NeedsAnalysisFile analysisFile, int pointX, int pointY);
        void SetAnalysisFiles(NeedsAnalysisInput currentNeedsAnalysisInput);
        NeedsAnalysisFile CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes needsAnalysisFileType, string baseDirectory, string fileExtension);
        string GetFileName(string filePath);
        string GetRandomFileName(string baseDir, string newFileExt);
        bool ValidateStartEndYears(string startYearTxt, string endYearTxt);
        bool ValidateStructureId(string structureIdsTxt);
        bool ValidateStateOrLocal(bool stateOwned, bool localOwned);
        List<string> ConvertStringToList(string stringToConvert);

    }
}
