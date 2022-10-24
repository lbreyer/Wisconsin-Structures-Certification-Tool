using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class StructureService : IStructureService
    {
        //T
        private static IStructureRepository repo = new StructureRepository();
        
        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            return repo.ConvertDegreesMinutesSecondsToDecimalDegrees(degreesMinutesSeconds);
        }

        public void SetToolTipProperties(ToolTip toolTip, bool isBalloon, int autoPopDelay)
        {
            repo.SetToolTipProperties(toolTip, isBalloon, autoPopDelay);
        }

        public string StructureIdToFolderName(string structureId)
        {
            return repo.StructureIdToFolderName(structureId);
        }
    }
}
