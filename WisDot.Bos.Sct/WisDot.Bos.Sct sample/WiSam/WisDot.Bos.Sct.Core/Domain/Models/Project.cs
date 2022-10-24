using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Data;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class Project
    {
        public int ProjectDbId { get; set; }
        public int ProjectHistoryDbId { get; set; }
        //public Guid ProjectGuid { get; set; }
        public string Region { get; set; }
        public int ProjectManagerDbId { get; set; }
        public string ProjectManager { get; set; }
        public int BosLiaisonDbId { get; set; }
        public string BosLiaison { get; set; }
        public int CurrentFiscalYear { get; set; }
        public int FiscalYear { get; set; }
        public DateTime LetDate { get; set; }
        public int AdvanceableFiscalYear { get; set; }
        public int ProjectYear { get; set; }
        public string StructuresConcept { get; set; }
        public int NumberOfStructures { get; set; }
        public int StructuresCost { get; set; }
        public bool IsQuasicertified { get; set; }
        public StructuresProgramType.ProjectStatus Status { get; set; }
        public List<WorkConcept> WorkConcepts { get; set; }
        public List<ElementWorkConcept> CertifiedElementWorkConceptCombinations { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string History { get; set; }
        public string RelatedProjects { get; set; }
        public int UserDbId { get; set; }
        public List<int> UserDbIds { get; set; } // for "My Projects"
        public string UserFullName { get; set; }
        public StructuresProgramType.ProjectUserAction UserAction { get; set; }
        public DateTime UserActionDateTime { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SubmitDate { get; set; }
        public DateTime PrecertifyDate { get; set; }
        public DateTime CertifyDate { get; set; }
        public bool RequestAdvancedCertification { get; set; }
        public DateTime AdvancedCertificationDate { get; set; }
        public bool Locked { get; set; }
        public bool InPrecertification { get; set; }
        public int PrecertificationLiaisonUserDbId { get; set; }
        public string PrecertificationLiaisonUserFullName { get; set; }
        public bool InCertification { get; set; }
        public int CertificationLiaisonUserDbId { get; set; }
        public string CertificationLiaisonUserFullName { get; set; }
        public string CertificationLiaisonEmail { get; set; }
        public string CertificationLiaisonPhone { get; set; }
        public List<StructureFileInfo> ProjectFiles { get; set; }
        public bool FromExcel { get; set; }
        public string RecertificationReason { get; set; }
        public string RecertificationComments { get; set; }
        public string NotificationRecipients { get; set; }
        public DateTime AcceptablePseDateStart { get; set; }
        public DateTime AcceptablePseDateEnd { get; set; }

        // Fiips
        public string FosProjectId { get; set; }
        public string DesignId { get; set; }
        public string FiipsImprovementConcept { get; set; }
        public string FiipsDescription { get; set; }
        public int FiipsCost { get; set; }
        public string LifecycleStageCode { get; set; }
        public DateTime PseDate { get; set; }
        public DateTime EpseDate { get; set; }
        public DateTime EarliestAdvanceableLetDate { get; set; }
        public DateTime LatestAdvanceableLetDate { get; set; }
        public string PlanningProjectFunctionalType { get; set; }

        // Mapping
        public GeoLocation GeoLocation { get; set; }
        public bool IsProjectRouteOn { get; set; }

        // Box
        public string BoxId { get; set; }

        public Project()
        {
            WorkConcepts = new List<WorkConcept>();
            CertifiedElementWorkConceptCombinations = new List<ElementWorkConcept>();
            GeoLocation = new GeoLocation();
            History = "";
            UserDbIds = new List<int>();
            PlanningProjectFunctionalType = "";
            RecertificationReason = "";
            RecertificationComments = "";
            Description = "";
            Notes = "";
            NotificationRecipients = "";
        }

        public Project(int projectDbId, int fiscalYear,
                        int numberOfStructures, int structuresCost,
                        string fosProjectId, string fiipsImprovementConcept)
        {
            ProjectDbId = projectDbId;
            FiscalYear = fiscalYear;
            NumberOfStructures = numberOfStructures;
            StructuresCost = structuresCost;
            FosProjectId = fosProjectId;
            FiipsImprovementConcept = fiipsImprovementConcept;
            WorkConcepts = new List<WorkConcept>();
            CertifiedElementWorkConceptCombinations = new List<ElementWorkConcept>();
            GeoLocation = new GeoLocation();
            History = "";
            UserDbIds = new List<int>();
            PlanningProjectFunctionalType = "";
            RecertificationReason = "";
            RecertificationComments = "";
            Notes = "";
            Description = "";
            NotificationRecipients = "";
        }
    }
}
