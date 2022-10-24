using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Data;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class WorkConcept
    {
        public int WorkConceptDbId { get; set; } // Auto-generated database ID
        public int ProjectWorkConceptHistoryDbId { get; set; }
        public DateTime WorkConceptTimeStamp { get; set; }
        public string StructureId { get; set; }
        public string PlannedStructureId { get; set; } // If structure replacement
        public string WorkConceptCode { get; set; }
        public string WorkConceptDescription { get; set; }
        public int CurrentFiscalYear { get; set; }
        public int FiscalYear { get; set; } // Optimal fiscal year for Eligible work concepts
        public int ProjectYear { get; set; } // Optimal project year for Eligible work concepts
        public float PriorityScore { get; set; }
        public int Cost { get; set; }
        public bool FromEligibilityList { get; set; }
        public bool FromFiips { get; set; }
        public bool Evaluate { get; set; }
        public bool Active { get; set; }
        public string SecondaryWorkConcepts { get; set; }
        public int EarlierFiscalYear { get; set; } // for auto-approval
        public int LaterFiscalYear { get; set; } // for auto-approval
        public string DotProgram { get; set; }
        public string CertifiedWorkConceptCode { get; set; } // Initially matches above
        public string CertifiedWorkConceptDescription { get; set; } // Initially matches above
        public string ChangeJustifications { get; set; }
        public string ChangeJustificationNotes { get; set; }
        public bool IsQuasicertified { get; set; } // transitional certification process without BOSCD
        public StructuresProgramType.WorkConceptStatus Status { get; set; }
        public StructuresProgramType.PrecertificatioReviewDecision PrecertificationDecision { get; set; } // BOS to Accept or Reject Unapproved work concept
        public StructuresProgramType.ProjectUserAction ProjectUserAction { get; set; }
        public DateTime ProjectUserActionDateTime { get; set; }
        public string ProjectUserFullName { get; set; }
        public string ProjectCertificationLiaisonUserFullName { get; set; }
        public string ProjectPrecertificationLiaisonUserFullName { get; set; }
        public string PrecertificationDecisionReasonCategory { get; set; }
        public DateTime PrecertificationDecisionDateTime { get; set; }
        public string PrecertificationDecisionReasonExplanation { get; set; }
        public string PrecertificationDecisionInternalComments { get; set; }
        public List<StructureFileInfo> WorkConceptFiles { get; set; }

        // Proposed Work Concept
        public bool FromProposedList { get; set; }
        public string RegionNumber { get; set; }
        public string ReasonCategory { get; set; }
        public string Notes { get; set; }
        public int ProposedByUserDbId { get; set; }
        public string ProposedByUserFullName { get; set; }
        public DateTime ProposedDate { get; set; }

        // Certification
        public string CertificationLiaisonFullName { get; set; }
        public string CertificationLiaisonEmail { get; set; }
        public string CertificationLiaisonPhone { get; set; }
        public string CertificationDecision { get; set; }
        public DateTime CertificationDateTime { get; set; }
        public string CertificationPrimaryWorkTypeComments { get; set; }
        public string CertificationSecondaryWorkTypeComments { get; set; }
        public int EstimatedConstructionCost { get; set; }
        public int EstimatedDesignLevelOfEffort { get; set; }
        public string DesignResourcing { get; set; } // TBD, BOS In-House or Consultant
        public string CertificationAdditionalComments { get; set; }

        // Mapping
        public GeoLocation GeoLocation { get; set; }
        public GMarkerGoogleType MapMarkerType { get; set; }

        // Str Project
        public int ProjectDbId { get; set; }
        public int StructureProjectFiscalYear { get; set; }
        public string Region { get; set; }
        public string StructuresConcept { get; set; }
        public StructuresProgramType.ProjectStatus ProjectStatus { get; set; }

        // Fiips Project
        public bool IsScopeAMatch { get; set; }
        public bool IsYearAMatch { get; set; }
        public string FosProjectId { get; set; }
        public string FiipsDescription { get; set; }
        public string FiipsImprovementConcept { get; set; }
        public string PlanningProjectFunctionalType { get; set; }

        // Use for Propose Work Concept
        public WorkConcept(int workConceptDbId, string structureId, string region, string regionNumber, string proposedWorkConceptCode,
            string proposedWorkConceptDescription, int fiscalYear, string reasonCategory, string notes,
            int proposedByUserDbId, string proposedByUserFullName, DateTime proposedDate, bool active)
        {
            this.Status = StructuresProgramType.WorkConceptStatus.Proposed;
            this.FromEligibilityList = false;
            this.FromProposedList = true;
            this.StructureId = structureId;
            this.Region = region;
            this.RegionNumber = regionNumber;
            this.WorkConceptDbId = workConceptDbId;
            this.WorkConceptCode = "PR";
            this.WorkConceptDescription = "Proposed";
            this.CertifiedWorkConceptCode = proposedWorkConceptCode;
            this.CertifiedWorkConceptDescription = proposedWorkConceptDescription;
            this.FiscalYear = fiscalYear;
            this.ReasonCategory = reasonCategory;
            this.Notes = notes;
            this.ProposedByUserDbId = proposedByUserDbId;
            this.ProposedByUserFullName = proposedByUserFullName;
            this.ProposedDate = proposedDate;
            this.Active = active;
            this.ChangeJustifications = reasonCategory;
            this.ChangeJustificationNotes = notes;
            this.PlannedStructureId = "";
            this.PlanningProjectFunctionalType = "";
            this.DesignResourcing = "";
        }

        public WorkConcept()
        {
            FromEligibilityList = true;
            Status = StructuresProgramType.WorkConceptStatus.Eligible;
            GeoLocation = new GeoLocation();
            MapMarkerType = GMarkerGoogleType.white_small;
            ChangeJustifications = "";
            ChangeJustificationNotes = "";
            StructureId = "";
            PlannedStructureId = "";
            DotProgram = "";
            PlanningProjectFunctionalType = "";
            DesignResourcing = "";
        }

        public WorkConcept(DataRow dr, int currentFiscalYear, int fiscalYear, int currentProjectYear)
        {
            this.FromFiips = true;
            this.Status = StructuresProgramType.WorkConceptStatus.Fiips;
            this.WorkConceptDbId = Convert.ToInt32(dr["fiipsid"]);
            this.StructureId = dr["extg_strc_id"].ToString().Trim();
            this.WorkConceptCode = dr["workactioncode"].ToString().Trim();
            this.WorkConceptDescription = dr["strc_work_tydc"].ToString().Trim();
            this.CertifiedWorkConceptCode = this.WorkConceptCode;

            if (this.WorkConceptCode.Equals("07"))
            {
                this.WorkConceptDescription = "PAINT";
            }

            this.CertifiedWorkConceptDescription = this.WorkConceptDescription;
            this.PlannedStructureId = dr["plnd_strc_id"].ToString().Trim();

            if (this.WorkConceptCode.Equals("01") && this.StructureId.Equals("") && !this.PlannedStructureId.Equals(""))
            {
                this.StructureId = this.PlannedStructureId;
            }

            this.Region = dr["dot_rgn_cd"].ToString();
            this.CurrentFiscalYear = currentFiscalYear;
            this.FiscalYear = fiscalYear;
            this.ProjectYear = currentProjectYear + (this.FiscalYear - this.CurrentFiscalYear);
            this.Cost = Convert.ToInt32(dr["tot_wo_dlvy_amt"]);
            this.FosProjectId = dr["fos_proj_id"].ToString().Trim();
            this.FiipsDescription = dr["fndg_ctgy_desc"].ToString().Trim();
            //wc.SecondaryWorkConcepts = dr2["secondaryworkactions"].ToString().Trim();
            this.FromEligibilityList = false;
            this.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString().Trim();
            this.PlanningProjectFunctionalType = "";

            if (this.StructureId.Equals("B110011"))
            {
                var stop = "here";
            }

            this.DotProgram = dr["wdot_pgm_desc"].ToString().Trim();

            switch (this.Region)
            {
                case "SW":
                    this.Region = "1-" + this.Region;
                    break;
                case "SE":
                    this.Region = "2-" + this.Region;
                    break;
                case "NE":
                    this.Region = "3-" + this.Region;
                    break;
                case "NC":
                    this.Region = "4-" + this.Region;
                    break;
                case "NW":
                    this.Region = "5-" + this.Region;
                    break;
            }
        }

        public WorkConcept(WorkConcept wc)
        {
            this.WorkConceptDbId = wc.WorkConceptDbId; // Auto-generated database ID
            this.StructureId = wc.StructureId;
            this.PlannedStructureId = wc.PlannedStructureId; // If structure replacement
            this.WorkConceptCode = wc.WorkConceptCode;
            this.WorkConceptDescription = wc.WorkConceptDescription;
            this.CurrentFiscalYear = wc.CurrentFiscalYear;
            this.FiscalYear = wc.FiscalYear; // Optimal fiscal year for Eligible work concepts
            this.ProjectYear = wc.ProjectYear; // Optimal project year for Eligible work concepts
            this.PriorityScore = wc.PriorityScore;
            this.Cost = wc.Cost;
            this.FromEligibilityList = wc.FromEligibilityList;
            this.FromFiips = wc.FromFiips;
            this.Evaluate = wc.Evaluate;
            this.FromProposedList = wc.FromProposedList;
            this.SecondaryWorkConcepts = wc.SecondaryWorkConcepts;
            this.EarlierFiscalYear = wc.EarlierFiscalYear; // for auto-approval
            this.LaterFiscalYear = wc.LaterFiscalYear; // for auto-approval
            this.DotProgram = wc.DotProgram;
            this.CertifiedWorkConceptCode = wc.CertifiedWorkConceptCode; // Initially matches above
            this.CertifiedWorkConceptDescription = wc.CertifiedWorkConceptDescription; // Initially matches above
            this.ChangeJustifications = wc.ChangeJustifications;
            this.ChangeJustificationNotes = wc.ChangeJustificationNotes;
            this.IsQuasicertified = wc.IsQuasicertified; // Auto-approval
            this.Status = wc.Status;
            this.FromProposedList = wc.FromProposedList ? true : false;
            this.ReasonCategory = wc.ReasonCategory;
            this.Notes = wc.Notes;
            this.ProposedByUserDbId = wc.ProposedByUserDbId;
            this.ProposedByUserFullName = wc.ProposedByUserFullName;
            this.ProposedDate = wc.ProposedDate;

            // Precertification
            this.ProjectWorkConceptHistoryDbId = wc.ProjectWorkConceptHistoryDbId;
            this.PrecertificationDecision = wc.PrecertificationDecision;
            this.PrecertificationDecisionDateTime = wc.PrecertificationDecisionDateTime;
            this.PrecertificationDecisionReasonCategory = wc.PrecertificationDecisionReasonCategory;
            this.PrecertificationDecisionReasonExplanation = wc.PrecertificationDecisionReasonExplanation;
            this.PrecertificationDecisionInternalComments = wc.PrecertificationDecisionInternalComments;

            this.CertificationDecision = wc.CertificationDecision;
            this.CertificationDateTime = wc.CertificationDateTime;
            this.CertificationPrimaryWorkTypeComments = wc.CertificationPrimaryWorkTypeComments;
            this.CertificationSecondaryWorkTypeComments = wc.CertificationSecondaryWorkTypeComments;
            this.CertificationAdditionalComments = wc.CertificationAdditionalComments;
            this.EstimatedConstructionCost = wc.EstimatedConstructionCost;
            this.EstimatedDesignLevelOfEffort = wc.EstimatedDesignLevelOfEffort;
            this.DesignResourcing = wc.DesignResourcing;

            // Mapping
            this.GeoLocation = wc.GeoLocation;
            this.MapMarkerType = wc.MapMarkerType;

            // Str Project
            this.ProjectDbId = wc.ProjectDbId;
            this.StructureProjectFiscalYear = wc.StructureProjectFiscalYear;
            this.Region = wc.Region;
            this.StructuresConcept = wc.StructuresConcept;

            // Fiips Project
            this.IsScopeAMatch = wc.IsScopeAMatch;
            this.IsYearAMatch = wc.IsYearAMatch;
            this.FosProjectId = wc.FosProjectId;
            this.FiipsDescription = wc.FiipsDescription;
            this.FiipsImprovementConcept = wc.FiipsImprovementConcept;
            this.PlanningProjectFunctionalType = wc.PlanningProjectFunctionalType;
        }
    }
}
