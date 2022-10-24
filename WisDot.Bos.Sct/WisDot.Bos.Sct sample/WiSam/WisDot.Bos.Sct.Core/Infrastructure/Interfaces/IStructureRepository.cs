using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IStructureRepository
    {
        string StructureIdToFolderName(string structureId);
        string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds);
        void SetToolTipProperties(ToolTip toolTip, bool isBalloon, int autoPopDelay);

    }
}
