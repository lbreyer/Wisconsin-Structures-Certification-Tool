using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class BridgeProjectRow
    {
        public string FosProjId { get; set; }
        public string PProjId { get; set; }
        public string PlanningProjectConceptCode { get; set; }
        public string PlanningProjectConceptDesc { get; set; }
        public string FederalImprovementTypeCode { get; set; }
        public string FederalImprovementTypeDesc { get; set; }
        public string StructureId { get; set; }
        public string NewStructureId { get; set; }

        public string EstimatedCompletionDate { get; set; }
        public string ProjectStatusFlag { get; set; }
        public string ProjectTotalWithDeliveryAmount { get; set; }
        public string ProjectTotalWithoutDeliveryAmount { get; set; }

        public string FundingSourceTypeCode { get; set; }
        public string FundingCategoryNumber { get; set; }

        public string WorkTotalWithDeliveryAmount { get; set; }
        public string WorkTotalWithoutDeliveryAmount { get; set; }
        public string FundingCategoryDesc { get; set; }

        public string OriginalWorkActionCode { get; set; }
        public string OriginalWorkActionDesc { get; set; }
    }
}
