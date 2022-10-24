using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class ElementWorkConcept
    {
        public int ElementWorkConceptDbId { get; set; }
        public int ProjectWorkConceptHistoryDbId { get; set; }
        public DateTime CertificationDateTime { get; set; }
        public string StructureId { get; set; }
        public int ElementNumber { get; set; }
        public string ElementName { get; set; }
        public string WorkConceptCode { get; set; }
        public string WorkConceptDescription { get; set; }
        public string WorkConceptLevel { get; set; } // Primary or Secondary
        public string Comments { get; set; }

        public ElementWorkConcept()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.StructureId = "";
            this.ElementName = "";
            this.WorkConceptCode = "";
            this.WorkConceptDescription = "";
            this.WorkConceptLevel = "";
            this.Comments = "";
        }
    }
}
