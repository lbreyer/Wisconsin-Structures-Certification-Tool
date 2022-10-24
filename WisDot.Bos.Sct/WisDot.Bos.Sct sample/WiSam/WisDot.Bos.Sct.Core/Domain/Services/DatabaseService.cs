using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using GMap.NET.WindowsForms.Markers;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class DatabaseService : IDatabaseService
    {
        private static IDatabaseRepository repo = new DatabaseRepository();
        private static IDatabaseQuery query = new DatabaseQuery();

        #region Adders
        public int AddProject(Project project, DateTime timeStamp)
        {
            return query.AddProject(project, timeStamp);
        }

        public int AddProjectHistory(Project project, DateTime timeStamp)
        {
            return query.AddProjectHistory(project, timeStamp);
        }

        public int AddProjectWorkConceptHistory(int projectHistoryDbId, WorkConcept wc)
        {
            return query.AddProjectWorkConceptHistory(projectHistoryDbId, wc);
        }

        public int AddProposedWorkConcept(WorkConcept wc)
        {
            return query.AddProposedWorkConcept(wc);
        }
        #endregion

        public bool AuthenticateUser(string userName, string userPassword)
        {
            return query.AuthenticateUser(userName, userPassword);
        }

        public DateTime CalculateAcceptablePseDateEnd(int fiscalYear)
        {
            return repo.CalculateAcceptablePseDateEnd(fiscalYear);
        }

        public DateTime CalculateAcceptablePseDateStart(int fiscalYear)
        {
            return repo.CalculateAcceptablePseDateStart(fiscalYear);
        }

        public void CertifyProject(Project project, StructuresProgramType.ProjectUserAction userAction)
        {
            query.CertifyProject(project, userAction);
        }

        public bool CloseDatabaseConnection(string dataSource)
        {
            return query.CloseDatabaseConnection(dataSource);
        }

        public string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            return repo.ConvertDegreesMinutesSecondsToDecimalDegrees(degreesMinutesSeconds);
        }

        public void DeactivateProposedWorkConcept(int workConceptDbId, string structureId)
        {
            query.DeactivateProposedWorkConcept(workConceptDbId, structureId);
        }

        public void DeleteProject(int projectDbId)
        {
            query.DeleteProject(projectDbId);
        }

        #region SQL Execute
        public void ExecuteInsertUpdateDelete(string qry, SqlParameter[] prms, SqlConnection conn)
        {
            query.ExecuteInsertUpdateDelete(qry, prms, conn);
        }

        public void ExecuteInsertUpdateDelete(string qry, SqlConnection conn)
        {
            query.ExecuteInsertUpdateDelete(qry, conn);
        }

        public DataTable ExecuteSelect(string qry, SqlConnection conn)
        {
            return query.ExecuteSelect(qry, conn);
        }

        public DataTable ExecuteSelect(string qry, SqlParameter[] prms, SqlConnection conn)
        {
            return query.ExecuteSelect(qry, prms, conn);
        }

        public DataTable ExecuteSelect(string qry, OracleConnection conn)
        {
            return query.ExecuteSelect(qry, conn);
        }

        public DataTable ExecuteSelect(string qry, OracleParameter[] prms, OracleConnection conn)
        {
            return query.ExecuteSelect(qry, prms, conn);
        }

        #endregion

        public List<Structure> FindNearMeStructures(float latPoint, float longPoint, float radius, string structureId, bool stateOwned, bool localOwned, string structureTypes, string region = "any")
        {
            return query.FindNearMeStructures(latPoint, longPoint, radius, structureId, stateOwned, localOwned, structureTypes, region);
        }

        public List<Structure> FindNearMeStructures(string structureId, float minLat, float maxLat, float minLng, float maxLng)
        {
            return query.FindNearMeStructures(structureId, minLat, maxLat, minLng, maxLng);
        }

        public List<string> FindStructuresNearMe(string id, StructuresProgramType.ObjectType objectType, float midLatitude, float midLongitude, float radius, string structureTypes = "'B','P'", bool stateOwned = true, bool localOwned = false, string region = "any")
        {
            return query.FindStructuresNearMe(id, objectType, midLatitude, midLongitude, radius, structureTypes, stateOwned, localOwned, region);
        }

        public string FormatEmailAddresses(string addresses)
        {
            return repo.FormatEmailAddresses(addresses);
        }

        #region Getters
        public List<WorkConcept> GetAllWorkConcepts()
        {
            return query.GetAllWorkConcepts();
        }

        public string GetCertificationLiaisonsEmails()
        {
            return query.GetCertificationLiaisonsEmails();
        }

        public string GetCertificationSupervisorsEmails()
        {
            return query.GetCertificationSupervisorsEmails();
        }

        public List<UserAccount> GetCertLiaisons()
        {
            return query.GetCertLiaisons();
        }

        public List<UserAccount> GetCertSups()
        {
            return query.GetCertSups();
        }

        public List<Project> GetDeletedProjectsForStructure(string structureId)
        {
            return query.GetDeletedProjectsForStructure(structureId);
        }

        public List<Project> GetDeletedWorkConceptsForStructure(string structureId)
        {
            return query.GetDeletedWorkConceptsForStructure(structureId);
        }

        public List<ElementWorkConcept> GetElementWorkConceptPairings(string structureId, int projectWorkConceptHistoryDbId, DateTime certificationDateTime)
        {
            return query.GetElementWorkConceptPairings(structureId, projectWorkConceptHistoryDbId, certificationDateTime);
        }

        public WorkConcept GetEligibleWorkConcept(int workConceptDbId)
        {
            return query.GetEligibleWorkConcept(workConceptDbId);
        }

        public List<WorkConcept> GetEligibleWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return query.GetEligibleWorkConcepts(startFiscalYear, endFiscalYear, region);
        }

        public List<WorkConcept> GetEligibleWorkConcepts(int fiscalYear, string region)
        {
            return query.GetEligibleWorkConcepts(fiscalYear, region);
        }

        public List<WorkConcept> GetEligibleWorkConcepts(int fiscalYear)
        {
            return query.GetEligibleWorkConcepts(fiscalYear);
        }
        public List<Project> GetProjectsInFiips()
        {
            return query.GetProjectsInFiips();
        }
        public List<WorkConcept> GetFiipsWorkConcepts()
        {
            return query.GetFiipsWorkConcepts();
        }
        public List<Project> GetProjectsInSct()
        {
            return query.GetProjectsInSct();
        }

        public string GetCertificationRootFolder()
        {
            return query.GetCertificationRootFolder();
        }

        public string GetCertificationDirectory()
        {
            return query.GetCertificationDirectory();
        }

        public string GetBosCdTemplate()
        {
            return query.GetBosCdTemplate();
        }

        public string GetTempDirectory()
        {
            return query.GetTempDirectory();
        }

        public string GetBosCdSignature()
        {
            return query.GetBosCdSignature();
        }

        public string GetApplicationMode()
        {
            return query.GetApplicationMode();
        }

        public string GetWisamsExecutablePath()
        {
            return query.GetWisamsExecutablePath();
        }

        public string GetFiipsQueryToolExecutablePath()
        {
            return query.GetFiipsQueryToolExecutablePath();
        }

        public bool EnableHsis()
        {
            return query.EnableHsis();
        }

        public bool EnableBox()
        {
            return query.EnableBox();
        }

        public DataTable GetEligibleWorkConceptsDataTable(int fiscalYear)
        {
            return query.GetEligibleWorkConceptsDataTable(fiscalYear);
        }

        public string GetEmailAddress(int userDbId)
        {
            return query.GetEmailAddress(userDbId);
        }

        public string GetEmailAddresses(List<int> userIds)
        {
            return query.GetEmailAddresses(userIds);
        }

        public string GetEmailAddressesRegionalTransactors(List<int> userIds)
        {
            return query.GetEmailAddressesRegionalTransactors(userIds);
        }

        public List<Project> GetFiipsProjects(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return repo.GetFiipsProjects(startFiscalYear, endFiscalYear, region);
        }

        public List<Project> GetFiipsProjects(string region, int startFiscalYear, int endFiscalYear)
        {
            return query.GetFiipsProjects(region, startFiscalYear, endFiscalYear);
        }

        public List<Project> GetFiipsProjects(int fiscalYear, string region = "any")
        {
            return query.GetFiipsProjects(fiscalYear, region);
        }

        public List<WorkConcept> GetFiipsWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return query.GetFiipsWorkConcepts(startFiscalYear, endFiscalYear, region);
        }

        public List<WorkConcept> GetFiipsWorkConcepts(int fiscalYear, string region)
        {
            return query.GetFiipsWorkConcepts(fiscalYear, region);
        }

        public List<WorkConcept> GetFiipsWorkConcepts(int fiscalYear)
        {
            return query.GetFiipsWorkConcepts(fiscalYear);
        }

        public int GetFiscalYear()
        {
            return repo.GetFiscalYear();
        }

        public ImpersonationUser GetImpersonationUser()
        {
            return repo.GetImpersonationUser();
        }

        public WiSamEntities.Inspection GetLastInspection(string strId)
        {
            return repo.GetLastInspection(strId);
        }

        public List<WiSamEntities.Element> GetLastInspectionElements(string strId)
        {
            return query.GetLastInspectionElements(strId);
        }

        public WiSamEntities.NbiRating GetLastNbiRating(string strId)
        {
            return query.GetLastNbiRating(strId);
        }

        public string GetLastPrecertificationOrCertification(int projectDbId, string action = "precertification")
        {
            return query.GetLastPrecertificationOrCertification(projectDbId, action);
        }

        public void GetMainSpanInfo(WiSamEntities.Structure str)
        {
            query.GetMainSpanInfo(str);
        }

        public GMarkerGoogleType GetMapMarkerType(string workConceptCode)
        {
            return query.GetMapMarkerType(workConceptCode);
        }

        public string GetPrecertificationLiaison(int projectDbId)
        {
            return query.GetPrecertificationLiaison(projectDbId);
        }

        public string GetPrecertificationLiaisonsEmails()
        {
            return query.GetPrecertificationLiaisonsEmails();
        }

        public UserAccount GetPrecertificationSubmitter(int projectDbId)
        {
            return query.GetPrecertificationSubmitter(projectDbId);
        }

        public List<UserAccount> GetPrecertLiaisons()
        {
            return query.GetPrecertLiaisons();
        }

        public List<WorkConcept> GetPrimaryWorkConcepts()
        {
            return query.GetPrimaryWorkConcepts();
        }

        public string GetProjectHistory(int projectDbId)
        {
            return query.GetProjectHistory(projectDbId);
        }

        public Project GetProjectRecertification(int projectDbId)
        {
            return query.GetProjectRecertification(projectDbId);
        }

        public List<Project> GetProjectsInFiips(int startFiscalYear, int endFiscalYear, List<WorkConcept> workConcepts, string region = "any")
        {
            return query.GetProjectsInFiips(startFiscalYear, endFiscalYear, workConcepts, region);
        }

        public List<Project> GetProjectsInSct(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return query.GetProjectsInSct(startFiscalYear, endFiscalYear, region);
        }

        public List<StructuresProgramType.ProjectUserAction> GetProjectUserActionHistory(int projectDbId)
        {
            return query.GetProjectUserActionHistory(projectDbId);
        }

        public List<WorkConcept> GetProjectWorkConceptHistory(string structureId, int workConceptDbId, Project project)
        {
            return query.GetProjectWorkConceptHistory(structureId, workConceptDbId, project);
        }

        public List<string> GetProposedWorkConceptJustifications()
        {
            return repo.GetProposedWorkConceptJustifications();
        }

        public List<WorkConcept> GetProposedWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return query.GetProposedWorkConcepts(startFiscalYear, endFiscalYear, region);
        }

        public string GetProposedWorkNotes(string structureId)
        {
            return query.GetProposedWorkNotes(structureId);
        }

        public string GetProposedWorkReasonCategory(string structureId)
        {
            return query.GetProposedWorkReasonCategory(structureId);
        }

        public List<WorkConcept> GetQuasicertifiedWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return query.GetQuasicertifiedWorkConcepts(startFiscalYear, startFiscalYear, region);
        }

        public List<WorkConcept> GetQuasicertifiedWorkConcepts(int fiscalYear)
        {
            return query.GetQuasicertifiedWorkConcepts(fiscalYear);
        }

        public List<WorkConcept> GetQuasicertifiedWorkConcepts(int fiscalYear, string region)
        {
            return query.GetQuasicertifiedWorkConcepts(fiscalYear, region);
        }

        public string GetRegionComboCode(string region)
        {
            return repo.GetRegionComboCode(region);
        }

        public string GetRegionNotes(WorkConcept currentWorkConcept)
        {
            return query.GetRegionNotes(currentWorkConcept);
        }

        public List<WorkConcept> GetSecondaryWorkConcepts()
        {
            return query.GetSecondaryWorkConcepts();
        }

        public void GetSpanInfo(WiSamEntities.Structure str)
        {
            query.GetSpanInfo(str);
        }

        public Structure GetSptStructure(string structureId)
        {
            return query.GetSptStructure(structureId);
        }

        public List<string> GetStateStructuresByRegionForGisDataPull(string region)
        {
            return query.GetStateStructuresByRegionForGisDataPull(region);
        }

        public WiSamEntities.Structure GetStructure(string strId, bool includeClosedBridges = false, bool interpolateNbi = false, bool includeCoreInspections = false, bool countTpo = false, int startYear = 0, int endYear = 0)
        {
            return query.GetStructure(strId, includeClosedBridges, interpolateNbi, includeCoreInspections, countTpo, startYear, endYear);
        }

        public WorkConcept GetStructureCertification(WorkConcept currentWorkConcept)
        {
            return query.GetStructureCertification(currentWorkConcept);
        }

        public GeoLocation GetStructureGeoLocation(string structureId)
        {
            return query.GetStructureGeoLocation(structureId);
        }

        public GeoLocation GetStructureLatLong(string structureId)
        {
            return query.GetStructureLatLong(structureId);
        }

        public string GetStructureOwnerAgencyCode(string structureId)
        {
            return query.GetStructureOwnerAgencyCode(structureId);
        }

        public WorkConcept GetStructurePrecertification(WorkConcept currentWorkConcept)
        {
            return query.GetStructurePrecertification(currentWorkConcept);
        }

        public List<Project> GetStructureProjects(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            return query.GetStructureProjects(startFiscalYear, endFiscalYear, region);
        }

        public List<Project> GetStructureProjects(int fiscalYear, string region = "any")
        {
            return query.GetStructureProjects(fiscalYear, region);
        }

        public List<string> GetStructuresByRegion(string region, bool stateOwned, bool localOwned, bool includeCStructures = false)
        {
            return query.GetStructuresByRegion(region, stateOwned, localOwned, includeCStructures);
        }

        public List<string> GetStructuresByRegionForGisDataPull(string region)
        {
            return query.GetStructuresByRegionForGisDataPull(region);
        }

        public List<string> GetStructuresByRegions(List<string> regions, bool stateOwned, bool localOwned, bool includeCStructures = false)
        {
            return query.GetStructuresByRegions(regions, stateOwned, localOwned, includeCStructures);
        }

        public WiSamEntities.StructureWorkAction GetStructureWorkAction(string workActionCode)
        {
            return query.GetStructureWorkAction(workActionCode);
        }

        public List<UserAccount> GetTopUsers()
        {
            return query.GetTopUsers();
        }

        public int GetUnapprovedWindowCurrentFyPlus()
        {
            return query.GetUnapprovedWindowCurrentFyPlus();
        }

        public UserAccount GetUserAccount(int userDbId)
        {
            return query.GetUserAccount(userDbId);
        }

        public UserAccount GetUserAccount(string userName, string userPassword)
        {
            return query.GetUserAccount(userName, userPassword);
        }

        public List<UserActivity> GetUserActivities()
        {
            return query.GetUserActivities();
        }

        public List<int> GetUserIdsForAProject(int projectDbId)
        {
            return query.GetUserIdsForAProject(projectDbId);
        }

        public List<UserAccount> GetUsersOfARoleType(string roleType)
        {
            return query.GetUsersOfARoleType(roleType);
        }

        public string GetWorkConceptDescription(string workActionCode)
        {
            return query.GetWorkConceptDescription(workActionCode);
        }

        public string GetWorkflowStatus(Project project)
        {
            return repo.GetWorkflowStatus(project);
        }

        public int InsertProject(Project project)
        {
            return query.InsertProject(project);
        }

        public int InsertProjectWorkConcept(int projectHistoryDbId, WorkConcept wc)
        {
            return query.InsertProjectWorkConcept(projectHistoryDbId, wc);
        }
        #endregion

        public int InsertProposedWorkConcept(WorkConcept workConcept)
        {
            return query.InsertProposedWorkConcept(workConcept);
        }

        public bool IsProjectIdInFiips(string projectId)
        {
            return query.IsProjectIdInFiips(projectId);
        }

        public bool IsStructureInHsi(string structureId, UserAccount userAccount = null)
        {
            return query.IsStructureInHsi(structureId, userAccount);
        }

        public bool IsWorkConceptPrimary(string workActionCode)
        {
            return query.IsWorkConceptPrimary(workActionCode);
        }

        public void LogUserActivity(int userDbId, string activity)
        {
            query.LogUserActivity(userDbId, activity);
        }

        public List<Project> MigrateExcelProjects(List<Project> projects, List<WorkConcept> eligibles)
        {
            return repo.MigrateExcelProjects(projects, eligibles);
        }

        public bool OpenDatabaseConnection(string dataSource)
        {
            return query.OpenDatabaseConnection(dataSource);
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            return repo.ParseWorkConceptFullDescription(workConcept);
        }

        public void PopulateProject(Project project, DataRow dr)
        {
            repo.PopulateProject(project, dr);
        }

        public void SaveCertifier(int liaisonUserDbId, string liaisonType, Project project, List<WorkConcept> workConcepts, UserAccount userAccount)
        {
            query.SaveCertifier(liaisonUserDbId, liaisonType, project, workConcepts, userAccount);
        }

        #region Update
        public void UpdateCertifier(Project project, List<WorkConcept> workConcepts)
        {
            query.UpdateCertifier(project, workConcepts);
        }

        public void UpdateEligibleWorkConcepts()
        {
            query.UpdateEligibleWorkConcepts();
        }

        public void UpdateGeneratedDate()
        {
            query.UpdateGeneratedDate();
        }

        public void UpdateLatLong()
        {
            query.UpdateLatLong();
        }

        public void UpdateOldEvs()
        {
            query.UpdateOldEvs();
        }

        public void UpdatePrecertifier(Project project, List<WorkConcept> workConcepts)
        {
            query.UpdatePrecertifier(project, workConcepts);
        }

        public void UpdateProjectBoxId(int projectDbId, string boxId)
        {
            query.UpdateProjectBoxId(projectDbId, boxId);
        }

        public void UpdateProjectCertification(Project project, StructuresProgramType.PrecertificatioReviewDecision decision)
        {
            query.UpdateProjectCertification(project, decision);
        }

        public void UpdateProjectFosProjectId(int projectHistoryDbId, string fosProjectId)
        {
            query.UpdateProjectFosProjectId(projectHistoryDbId, fosProjectId);
        }

        public void UpdateProjectWhileInPrecertificationOrCertification(Project project)
        {
            query.UpdateProjectWhileInPrecertificationOrCertification(project);
        }

        public void UpdateRegionNumber()
        {
            query.UpdateRegionNumber();
        }

        public void UpdateStructureProgramReviewCurrent()
        {
            query.UpdateStructureProgramReviewCurrent();
        }

        public void UpdateTimeWindows()
        {
            query.UpdateTimeWindows();
        }

        public void UpdateWorkActionCode(WorkConcept wc)
        {
            query.UpdateWorkActionCode(wc);
        }

        public void UpdateWorkConceptCertification(List<ElementWorkConcept> elementWorkConceptCombinations, WorkConcept workConcept)
        {
            query.UpdateWorkConceptCertification(elementWorkConceptCombinations, workConcept);
        }

        public void UpdateWorkConceptPrecertification(Project project, WorkConcept workConcept)
        {
            query.UpdateWorkConceptPrecertification(project, workConcept);
        }

        public void UpdateWorkConceptPrecertificationInternalComments(string internalComments, int projectWorkConceptHistoryDbId)
        {
            query.UpdateWorkConceptPrecertificationInternalComments(internalComments, projectWorkConceptHistoryDbId);
        }

        public List<UserAccount> GetPrecertificationLiaisons()
        {
            return query.GetPrecertificationLiaisons();
        }

        public string GetPrecertificationLiaisonsEmailAddresses()
        {
            return query.GetPrecertificationLiaisonsEmailAddresses();
        }

        public List<UserAccount> GetCertificationLiaisons()
        {
            return query.GetCertificationLiaisons();
        }

        public string GetCertificationLiaisonsEmailAddresses()
        {
            return query.GetCertificationLiaisonsEmailAddresses();
        }

        public string GetCertificationSupervisorsEmailAddresses()
        {
            return query.GetCertificationSupervisorsEmailAddresses();
        }

        public string GetMyDirectory()
        {
            return query.GetMyDirectory();
        }

        public List<WorkConcept> GetEligibleWorkConcepts()
        {
            return query.GetEligibleWorkConcepts();
        }

        public Wisdot.Bos.Dw.Database GetWarehouseDatabase()
        {
            return query.GetWarehouseDatabase();
        }

        public string GetCertificationLiaison(int projectDbId)
        {
            return query.GetCertificationLiaison(projectDbId);
        }

        public string GetWorkflowTransaction(StructuresProgramType.ProjectUserAction projectUserAction)
        {
            return query.GetWorkflowTransaction(projectUserAction);
        }
        #endregion
    }
}
