using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;

namespace WiSam.StructuresProgram
{
    public class FormStructureController
    {
        private static IStructureService serv = new StructureService();

        public string StructureIdToFolderName(string structureId)
        {
            return serv.StructureIdToFolderName(structureId);
        }

        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            return serv.ConvertDegreesMinutesSecondsToDecimalDegrees(degreesMinutesSeconds);
        }

        public void SetToolTipProperties(ToolTip toolTip, bool isBalloon, int autoPopDelay)
        {
            serv.SetToolTipProperties(toolTip, isBalloon, autoPopDelay);
        }
    }
}
