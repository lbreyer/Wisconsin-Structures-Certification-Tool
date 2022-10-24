using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Data;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class StructureHistoryRecord
    {
        public int ProjectDbId { get; set; }
        public int ProjectHistoryDbId { get; set; }
        public string StructureId { get; set; }
        public string CertifiedWorkConceptCode { get; set; }
        public string CertifiedWorkConceptDescription { get; set; }
        public StructuresProgramType.WorkConceptStatus WorkConceptStatus { get; set; }
        public StructuresProgramType.ProjectUserAction ProjectUserAction { get; set; }
        public DateTime ProjectUserActionDateTime { get; set; }
        public string ProjectUser { get; set; }


        public StructureHistoryRecord()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.StructureId = "";
            this.CertifiedWorkConceptCode = "";
            this.CertifiedWorkConceptDescription = "";
            this.ProjectUser = "";
        }
    }
}
