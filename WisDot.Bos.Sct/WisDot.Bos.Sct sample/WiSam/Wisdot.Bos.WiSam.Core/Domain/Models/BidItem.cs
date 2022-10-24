using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class BidItem
    {
        public int BidItemDbId { get; set; }
        public string BidItemName { get; set; }
        public string BidItemDescription { get; set; }
        public string BidItemSupplementalDescription { get; set; }
        public float BidItemQuantity { get; set; }
        public float BidItemUnitPrice { get; set; }
        public float BidItemCost { get; set; }
        public int ProjectDbId { get; set; }
        public string ProjectDescription { get; set; }
        public string ProjectWorkType { get; set; }
        public string ProjectStatus { get; set; }
        public string ProjectName { get; set; } // Fos Project ID with dashes, e.g., 1000-19-85
        public string FosProjectId { get; set; } // 10001985
        public string ExistingStructureIds { get; set; }
        public string PlannedStructureIds { get; set; }
        public string ProjectManager { get; set; }
        public DateTime LetDate { get; set; }
        public DateTime AdvanceableLetDate { get; set; }
        public string SpecBook { get; set; } // 03 by default
        public int ProposalDbId { get; set; }
        public string ProposalStatus { get; set; }
        public DateTime ProposalStatusDate { get; set; }
        public int AwardedVendorDbId { get; set; }
        public int ReferenceVendorId { get; set; }
        public string AwardedVendorName { get; set; }
        public float AwardedAmount { get; set; }
    }
}

