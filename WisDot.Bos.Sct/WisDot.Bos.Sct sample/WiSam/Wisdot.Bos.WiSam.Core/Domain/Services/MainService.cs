using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Domain.Services.Interfaces;
using Wisdot.Bos.WiSam.Core.Infrastructure;
using Wisdot.Bos.WiSam.Core.Infrastructure.Interfaces;

namespace Wisdot.Bos.WiSam.Core.Domain.Services
{
    public class MainService : IMainService
    {
        private static IMainRepository repo = new MainRepository();

        public List<string> ConvertStringToList(string stringToConvert)
        {
            return repo.ConvertStringToList(stringToConvert);
        }

        public NeedsAnalysisFile CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes needsAnalysisFileType, string baseDirectory, string fileExtension)
        {
            return repo.CreateNeedsAnalysisFile(needsAnalysisFileType, baseDirectory, fileExtension);
        }

        public string GetFileName(string filePath)
        {
            return repo.GetFileName(filePath);
        }

        public string GetRandomFileName(string baseDir, string newFileExt)
        {
            return repo.GetRandomFileName(baseDir, newFileExt);
        }

        public void SetAnalysisFiles(NeedsAnalysisInput currentNeedsAnalysisInput)
        {
            repo.SetAnalysisFiles(currentNeedsAnalysisInput);
        }

        public bool ValidateStartEndYears(string startYearTxt, string endYearTxt)
        {
            return repo.ValidateStartEndYears(startYearTxt, endYearTxt);
        }

        public bool ValidateStateOrLocal(bool stateOwned, bool localOwned)
        {
            return repo.ValidateStateOrLocal(stateOwned, localOwned);
        }

        public bool ValidateStructureId(string structureIdsTxt)
        {
            return repo.ValidateStructureId(structureIdsTxt);
        }

        public LinkLabel WriteResult(int fileCounter, NeedsAnalysisFile analysisFile, int pointX, int pointY)
        {
            return repo.WriteResult(fileCounter, analysisFile, pointX, pointY);
        }
    }
}
