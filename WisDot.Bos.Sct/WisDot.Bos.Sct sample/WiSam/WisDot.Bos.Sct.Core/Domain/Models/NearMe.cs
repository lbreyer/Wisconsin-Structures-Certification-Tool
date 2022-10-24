using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class NearMeWorkConcept
    {
        public string NearStructureId { get; set; }
        public string StructureId { get; set; }
        public float WithinDistance { get; set; } // in miles
        public WorkConcept WorkConcept { get; set; }

        public NearMeWorkConcept(string nearStructureId, string structureId, float withinDistance)
        {
            NearStructureId = nearStructureId;
            StructureId = structureId;
            WithinDistance = withinDistance;
            WorkConcept = new WorkConcept();
            WorkConcept.StructureId = structureId;
            WorkConcept.WorkConceptCode = "ev";
            WorkConcept.WorkConceptDescription = "Evaluate";
        }
    }
}
