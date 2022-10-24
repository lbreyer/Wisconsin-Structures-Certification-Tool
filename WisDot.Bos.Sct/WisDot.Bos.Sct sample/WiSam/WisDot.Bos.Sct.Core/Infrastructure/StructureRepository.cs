using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class StructureRepository : IStructureRepository
    {
        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            int length = degreesMinutesSeconds.Length;
            float degrees = Convert.ToSingle(degreesMinutesSeconds.Substring(0, 2));
            float minutes = Convert.ToSingle(degreesMinutesSeconds.Substring(2, 2)) / 60;
            float seconds = Convert.ToSingle(degreesMinutesSeconds.Substring(4, length - 4)) / 3600;
            float decimalValue = minutes + seconds;
            return (degrees + decimalValue).ToString();
        }

        public void SetToolTipProperties(ToolTip toolTip, bool isBalloon, int autoPopDelay)
        {
            toolTip.IsBalloon = isBalloon;
            toolTip.AutoPopDelay = autoPopDelay;
            toolTip.AutomaticDelay = 0;
        }

        public string StructureIdToFolderName(string structureId)
        {
            string folderName = structureId.Substring(0, 3) + "-";

            if (!structureId.Substring(3, 1).Equals("0"))
            {
                folderName += structureId.Substring(3, 1);
            }

            folderName += structureId.Substring(4, 3);

            if (structureId.Length == 11)
            {
                folderName += "-" + structureId.Substring(7, 4);
            }

            return folderName;
        }
    }
}
