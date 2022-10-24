using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using Wisdot.Bos.WiSam.Core.Domain.Services;
using Wisdot.Bos.WiSam.Core.Domain.Services.Interfaces;

namespace WiSam.Desktop
{
    public class MainController
    {
        private static IMainService mainServ = new MainService();

        public LinkLabel WriteResult(int fileCounter, NeedsAnalysisFile analysisFile, int pointX, int pointY)
        {
            return mainServ.WriteResult(fileCounter, analysisFile, pointX, pointY);
        }

        public void SetAnalysisFiles(NeedsAnalysisInput currentNeedsAnalysisInput)
        {
            mainServ.SetAnalysisFiles(currentNeedsAnalysisInput);
        }

        public NeedsAnalysisFile CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes needsAnalysisFileType, string baseDirectory, string fileExtension)
        {
            return mainServ.CreateNeedsAnalysisFile(needsAnalysisFileType, baseDirectory, fileExtension);
        }

        public string GetFileName(string filePath)
        {
            return mainServ.GetFileName(filePath);
        }

        public string GetRandomFileName(string baseDir, string newFileExt)
        {
            return mainServ.GetRandomFileName(baseDir, newFileExt);
        }
        public bool ValidateStartEndYears(string startYearTxt, string endYearTxt)
        {
            return mainServ.ValidateStartEndYears(startYearTxt, endYearTxt);
        }
        public bool ValidateStructureId(string structureIdsTxt)
        {
            return mainServ.ValidateStructureId(structureIdsTxt);
        }
        public bool ValidateStateOrLocal(bool stateOwned, bool localOwned)
        {
            return mainServ.ValidateStateOrLocal(stateOwned, localOwned);
        }
        public List<string> ConvertStringToList(string stringToConvert)
        {
            return mainServ.ConvertStringToList(stringToConvert);
        }
    }
}
