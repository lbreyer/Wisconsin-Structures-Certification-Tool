using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private static IDatabaseQuery query = new DatabaseQuery();

        public DateTime CalculateAcceptablePseDateEnd(int fiscalYear)
        {
            DateTime pseDateEnd = new DateTime(fiscalYear, 6, 30);
            return pseDateEnd;
        }

        public DateTime CalculateAcceptablePseDateStart(int fiscalYear)
        {
            DateTime pseDateStart = new DateTime(fiscalYear - 1, 1, 1);
            return pseDateStart;
        }

        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            int length = degreesMinutesSeconds.Length;
            float degrees = Convert.ToSingle(degreesMinutesSeconds.Substring(0, 2));
            float minutes = Convert.ToSingle(degreesMinutesSeconds.Substring(2, 2)) / 60;
            float seconds = Convert.ToSingle(degreesMinutesSeconds.Substring(4, length - 4)) / 3600;
            float decimalValue = minutes + seconds;
            return (degrees + decimalValue).ToString();
        }

        public string FormatEmailAddresses(string addresses)
        {
            string formattedEmailAddresses = addresses.Trim().Replace(" ", ",").Replace(";", ",").Replace("\t", ",").Replace("\r\n", ",");
            return formattedEmailAddresses;
        }

        public List<Project> GetFiipsProjects(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<Project> projects = new List<Project>();
            //projects.AddRange(GetFiipsProjects(region, startFiscalYear, endFiscalYear));
            for (int i = startFiscalYear; i <= endFiscalYear; i++)
            {
                projects.AddRange(query.GetFiipsProjects(i, region));
                //projects.AddRange(GetFiipsProjects(region, i));
            }

            return projects;
        }

        public int GetFiscalYear()
        {
            int currentYear = DateTime.Now.Year;

            if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 7, 1)) >= 0)
            {
                currentYear = currentYear + 1;
            }

            return currentYear;
        }

        public ImpersonationUser GetImpersonationUser()
        {
            ImpersonationUser impersonationUser = new ImpersonationUser();
            impersonationUser.ImpersonationDomain = Database.impersonationDomain;
            impersonationUser.ImpersonationUserId = Database.impersonationUserId;
            impersonationUser.ImpersonationPassword = Database.decryptedImpersonationPassword;

            return impersonationUser;
        }

        public WiSamEntities.Inspection GetLastInspection(string strId)
        {
            WiSamEntities.Inspection lastInspection = null;

            try
            {
                List<WiSamEntities.Element> lastInspectionElements = query.GetLastInspectionElements(strId);
                WiSamEntities.NbiRating nbiRatings = query.GetLastNbiRating(strId);

                if (lastInspectionElements != null && lastInspectionElements.Count > 0 && nbiRatings != null)
                {
                    lastInspection = new WiSamEntities.Inspection();
                    lastInspection.StructureId = strId;
                    lastInspection.Elements = lastInspectionElements;
                    lastInspection.NbiRatings = nbiRatings;

                    if (nbiRatings.InspectionDate < lastInspectionElements[0].InspectionDate)
                    {
                        lastInspection.InspectionDate = nbiRatings.InspectionDate;
                    }
                    else
                    {
                        lastInspection.InspectionDate = lastInspectionElements[0].InspectionDate;
                    }
                }
            }
            catch { }

            return lastInspection;
        }

        public List<string> GetProposedWorkConceptJustifications()
        {
            List<string> justifications = new List<string>();
            foreach (var j in Enum.GetValues(typeof(StructuresProgramType.ProposedWorkConceptJustification)))
            {
                justifications.Add(j.ToString());
            }

            return justifications;
        }

        public string GetRegionComboCode(string region)
        {
            string regionComboCode = "";

            switch (region.ToUpper())
            {
                case "SW":
                    regionComboCode = "1-SW";
                    break;
                case "SE":
                    regionComboCode = "2-SE";
                    break;
                case "NE":
                    regionComboCode = "3-NE";
                    break;
                case "NC":
                    regionComboCode = "4-NC";
                    break;
                case "NW":
                    regionComboCode = "5-NW";
                    break;
            }

            return regionComboCode;
        }

        public string GetWorkflowStatus(Project project)
        {
            string workflowStatus = "";
            //string time = String.Format("{0:u}", project.UserActionDateTime);
            string time = project.UserActionDateTime.ToString("yyyy-MM-dd hh:mm tt");

            switch (project.UserAction)
            {
                case StructuresProgramType.ProjectUserAction.SavedProject:
                    workflowStatus = String.Format("{0} :: Project's saved but not submitted for review by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification:
                    workflowStatus = String.Format("{0} :: Project's submitted for precertification by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.Precertification:
                    workflowStatus = String.Format("{0} :: Project's in precertification; precertification liaison: {1}", time, project.PrecertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment:
                    workflowStatus = String.Format("{0} :: Project's precertification liaison is unassigned by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification:
                    workflowStatus = String.Format("{0} :: Project's approved for precertification by {1}; precertification liaison: {2}", time, project.UserFullName, project.PrecertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.BosRejectedPrecertification:
                    workflowStatus = String.Format("{0} :: Project's rejected for precertification by {1}; precertification liaison: {2}", time, project.UserFullName, project.PrecertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.Certification:
                    workflowStatus = String.Format("{0} :: Project's in certification; certification liaison: {1}", time, project.CertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment:
                    workflowStatus = String.Format("{0} :: Project's certification liaison is unassigned by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.BosTransitionallyCertified:
                    workflowStatus = String.Format("{0} :: Project's transitionally certified by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification:
                    workflowStatus = String.Format("{0} :: Project's certified by {1} pending the review of a BOS certification supervisor; certification liaison: {2}", time, project.UserFullName, project.CertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection:
                    workflowStatus = String.Format("{0} :: Project's rejected for certification by {1} pending the review of a BOS certification supervisor; certification liaison: {2}", time, project.UserFullName, project.CertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.BosCertified:
                    workflowStatus = String.Format("{0} :: Project's approved for certification by {1}; certification liaison: {2}", time, project.UserFullName, project.CertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.BosRejectedCertification:
                    workflowStatus = String.Format("{0} :: Project's rejected for certification by {1}; certification liaison: {2}", time, project.UserFullName, project.CertificationLiaisonUserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.RequestRecertification:
                    workflowStatus = String.Format("{0} :: Project's requested for recertification by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.GrantRecertification:
                    workflowStatus = String.Format("{0} :: Project's request for recertification is granted by {1}", time, project.UserFullName);
                    break;
                case StructuresProgramType.ProjectUserAction.RejectRecertification:
                    workflowStatus = String.Format("{0} :: Project's request for recertification is rejected by {1}", time, project.UserFullName);
                    break;
            }

            return workflowStatus;
        }

        public List<Project> MigrateExcelProjects(List<Project> projects, List<WorkConcept> eligibles)
        {

            DateTime timeStamp = new DateTime(2019, 1, 1);
            int projectCounter = 0;
            int eligibleCounter = 0;
            int proposedCounter = 0;
            UserAccount userAccount = query.GetUserAccount(7);

            foreach (Project project in projects)
            {
                if (projectCounter >= 1)
                {
                    //break;
                }

                // Insert record into Project
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.CertificationLiaisonUserDbId = userAccount.UserDbId;
                project.CertificationLiaisonUserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserAction = StructuresProgramType.ProjectUserAction.BosTransitionallyCertified;
                project.CurrentFiscalYear = GetFiscalYear();
                int projectDbId = query.AddProject(project, timeStamp);
                //int projectDbId = 0;
                project.ProjectDbId = projectDbId;

                // Insert record into ProjectHistory
                int projectHistoryDbId = query.AddProjectHistory(project, timeStamp);
                //int projectHistoryDbId = 0;

                List<WorkConcept> workConcepts = new List<WorkConcept>();

                foreach (WorkConcept wc in project.WorkConcepts)
                {
                    // Determine if there's an eligible work concept for structure
                    // If there is, use it; else it's a proposed work concept
                    var matches = eligibles.Where(el => el.StructureId.Equals(wc.StructureId) && !el.FromProposedList);
                    //var currentWorkConcept = wc;

                    if (matches.Count() > 0)
                    {
                        WorkConcept match = matches.First();
                        wc.WorkConceptDbId = match.WorkConceptDbId;
                        wc.WorkConceptCode = match.WorkConceptCode;
                        wc.WorkConceptDescription = match.WorkConceptDescription;
                        wc.ProjectYear = 1 + (project.FiscalYear - project.CurrentFiscalYear);
                        wc.FromEligibilityList = true;
                        wc.FromProposedList = false;
                        wc.FromFiips = false;
                        wc.Evaluate = false;
                        // Insert record into ProjectWorkConceptHistory
                        query.AddProjectWorkConceptHistory(projectHistoryDbId, wc);
                        eligibleCounter++;
                    }
                    else
                    {
                        WorkConcept p = new WorkConcept();
                        p.StructureId = wc.StructureId;
                        p.Region = wc.Region;
                        p.RegionNumber = wc.Region.Substring(0, 1);
                        p.WorkConceptCode = wc.CertifiedWorkConceptCode;
                        p.WorkConceptDescription = wc.CertifiedWorkConceptDescription;
                        p.FiscalYear = wc.FiscalYear;
                        p.ReasonCategory = "Other";
                        p.ProposedByUserDbId = project.UserDbId;
                        p.ProposedByUserFullName = project.UserFullName;
                        p.ProposedDate = timeStamp;
                        p.Active = true;
                        p.WorkConceptDbId = query.AddProposedWorkConcept(p);
                        p.WorkConceptCode = "PR";
                        p.WorkConceptDescription = "Proposed";
                        p.CertifiedWorkConceptCode = wc.CertifiedWorkConceptCode;
                        p.CertifiedWorkConceptDescription = wc.CertifiedWorkConceptDescription;
                        p.ProjectYear = 1 + (project.FiscalYear - project.CurrentFiscalYear);
                        p.FromEligibilityList = false;
                        p.FromProposedList = true;
                        p.FromFiips = false;
                        p.Evaluate = false;
                        p.Status = wc.Status;
                        // Insert record into ProjectWorkConceptHistory
                        query.AddProjectWorkConceptHistory(projectHistoryDbId, p);
                        //currentWorkConcept = p;
                        proposedCounter++;
                    }

                    //workConcepts.Add(currentWorkConcept);
                }

                //project.WorkConcepts = workConcepts;
                projectCounter++;
            }

            System.Windows.Forms.MessageBox.Show(String.Format("Projects: {0}", projectCounter));
            System.Windows.Forms.MessageBox.Show(String.Format("Eligibles: {0}", eligibleCounter));
            System.Windows.Forms.MessageBox.Show(String.Format("Proposed: {0}", proposedCounter));
            return projects;
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            string[] parsed = workConcept.Split(new string[] { "(", ")", ";" }, StringSplitOptions.RemoveEmptyEntries);
            return parsed;
        }

        public void PopulateProject(Project project, DataRow dr)
        {
            if (project.IsQuasicertified)
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.BosCertified;
                project.UserActionDateTime = new DateTime(2019, 2, 1);
            }
            else
            {

            }
        }
    }
}
