using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class StructureFileInfo
    {
        public int FileDbId { get; set; }
        public string FileName { get; set; }
        public string FileDirectory { get; set; }
        public DateTime FileDate { get; set; }

        // A file is either a project or work concept file
        public int ProjectHistoryDbId { get; set; } // Non-zero value indicates a project file
        public int WorkConceptHistoryDbId { get; set; } // Non-zero value indicates a work concept file
    }
}
