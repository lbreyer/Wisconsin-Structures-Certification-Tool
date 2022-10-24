using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class StructureProgramReview
    {
        public int RowDbId { get; set; }
        public int RowInsertDateStamp { get; set; }
        public string StructureId { get; set; }
        public string WorkActionCode { get; set; }
        public string WorkActionDesc { get; set; }
        public int WorkActionYear { get; set; }
        public string CertificationStatus { get; set; }
        public bool IsCertified { get; set; }
        public DateTime StatusDate { get; set; }
        public string CertificationNotes { get; set; }
        public string DiscussionNotes { get; set; }
        public int AdvanceableWorkActionYear { get; set; }
        public bool IsBridge { get; set; }
        public string Region { get; set; }
        public string RegionNumber { get; set; }
        public string County { get; set; }
        public string CountyNumber { get; set; }
        public string FundingEligibility { get; set; }
        public string FundingEligibilityCsv { get; set; }
        public bool InFiips { get; set; } // True if there's a let programmed work for given structure in fiscal year >= current fiscal year
        public string FosProjectId { get; set; }
        public string FiipsNotes { get; set; }
        public bool IsScopeAMatch { get; set; }
        public bool IsYearAMatch { get; set; }

        public StructureProgramReview()
        {
            StructureId = "";
            WorkActionCode = "";
            WorkActionDesc = "";
            CertificationStatus = "";
            IsCertified = false;
            CertificationNotes = "";
            DiscussionNotes = "";
            IsBridge = false;
            Region = "";
            RegionNumber = "";
            County = "";
            CountyNumber = "";
            FundingEligibility = "";
            FundingEligibilityCsv = "";
            InFiips = false;
            FosProjectId = "";
            FiipsNotes = "";
            IsScopeAMatch = false;
            IsYearAMatch = false;
        }
    }
}
