using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class StructureLite
    {
        public string StructureId { get; set; }
        public string CorridorCode { get; set; }
        public string CorridorDesc { get; set; }
        public bool HighClearanceRoute { get; set; }
        public string RegionNumber { get; set; }
        public string Region { get; set; }
        public string CountyNumber { get; set; }
        public string County { get; set; }
        public string MunicipalityNumber { get; set; }
        public string Municipality { get; set; }
        public string OwnerNumber { get; set; }
        public string Owner { get; set; }
        public string StmcAgcyTy { get; set; }
        public string StimAgcyTycd { get; set; }
        public List<string> FundingEligibility { get; set; }
        public string FundingEligibilityCsv { get; set; }
        public string FeatureOn { get; set; }
        public string FeatureUnder { get; set; }

        public StructureLite()
        {
            StructureId = "";
            CorridorCode = "";
            CorridorDesc = "";
            HighClearanceRoute = false;
            RegionNumber = "";
            Region = "";
            CountyNumber = "";
            County = "";
            MunicipalityNumber = "";
            Municipality = "";
            OwnerNumber = "";
            Owner = "";
            StmcAgcyTy = "";
            StimAgcyTycd = "";
            FundingEligibility = new List<string>();
            FundingEligibilityCsv = "";
            FeatureOn = "";
            FeatureUnder = "";
        }
    }
}
