using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration; // App.config, web.config
using System.Data;
using System.Data.SqlClient; // SQL Server
using Oracle.DataAccess.Client; // Oracle
using System.Diagnostics;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using System.IO;
using System.Reflection;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;

namespace WisDot.Bos.Sct.Core.Data
{
    public class DatabaseQuery : IDatabaseQuery
    {
        Database database = new Database();
        private static IDatabaseRepository repo = new DatabaseRepository();
        private static IDatabaseService dataServ = new DatabaseService();

        #region Adders
        public int AddProject(Project project, DateTime timeStamp)
        {
            int projectDbId = 0;

            lock (database.databaseLock)
            {
                string qry =
                    @"
                        insert into project (region, active, createdate, certifydate, certificationliaisonuserdbid, certificationliaisonuserfullname)
                        values (@region, @active, @createDate, @certifyDate, @certificationLiaisonUserDbId, @certificationLiaisonUserFullName)
                    ";
                SqlParameter[] prms = new SqlParameter[6];
                prms[0] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[0].Value = project.Region;
                prms[1] = new SqlParameter("@active", SqlDbType.Bit);
                prms[1].Value = 1;
                prms[2] = new SqlParameter("@createDate", SqlDbType.DateTime);
                prms[2].Value = timeStamp;
                prms[3] = new SqlParameter("@certifyDate", SqlDbType.DateTime);
                prms[3].Value = timeStamp;
                prms[4] = new SqlParameter("@certificationLiaisonUserDbId", SqlDbType.Int);
                prms[4].Value = project.CertificationLiaisonUserDbId;
                prms[5] = new SqlParameter("@certificationLiaisonUserFullName", SqlDbType.VarChar);
                prms[5].Value = project.CertificationLiaisonUserFullName;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab the ProjectDbId
                qry =
                    @"
                        select max(projectdbid) as maxprojectdbid
                        from project
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
                projectDbId = Convert.ToInt32(dt.Rows[0]["maxprojectdbid"]);
            }

            return projectDbId;
        }

        public int AddProjectHistory(Project project, DateTime timeStamp)
        {
            int projectHistoryDbId = 0;

            lock (database.databaseLock)
            {
                string qry =
                    @"
                    insert into projecthistory (projectdbid, currentfiscalyear, fiscalyear, projectyear, numberofstructures, quasicertified,
                        status, userdbid, userfullname, useraction, useractiondatetime, fosprojectid, certificationliaisonuserdbid,
                        certificationliaisonuserfullname, certificationdatetime, advanceablefiscalyear)
                    values (@projectDbId, @currentFiscalYear, @fiscalYear, @projectYear, @numberOfStructures, @quasiCertified,
                        @status, @userDbId, @userFullName, @userAction, @userActionDateTime, @fosProjectId, @certificationLiaisonUserDbId,
                        @certificationLiaisonUserFullName, @certificationDateTime, @advanceableFiscalYear)
                ";
                SqlParameter[] prms = new SqlParameter[16];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = project.ProjectDbId;
                prms[1] = new SqlParameter("@currentFiscalYear", SqlDbType.Int);
                prms[1].Value = project.CurrentFiscalYear;
                prms[2] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                prms[2].Value = project.FiscalYear;
                prms[3] = new SqlParameter("@projectYear", SqlDbType.Int);
                prms[3].Value = 1 + (project.FiscalYear - project.CurrentFiscalYear);
                prms[4] = new SqlParameter("@numberOfStructures", SqlDbType.Int);
                prms[4].Value = project.NumberOfStructures;
                prms[5] = new SqlParameter("@quasiCertified", SqlDbType.Bit);
                prms[5].Value = project.IsQuasicertified ? 1 : 0;
                prms[6] = new SqlParameter("@status", SqlDbType.Int);
                prms[6].Value = (int)project.Status;
                prms[7] = new SqlParameter("@userDbId", SqlDbType.Int);
                prms[7].Value = project.UserDbId;
                prms[8] = new SqlParameter("@userFullName", SqlDbType.VarChar);
                prms[8].Value = project.UserFullName;
                prms[9] = new SqlParameter("@userAction", SqlDbType.Int);
                prms[9].Value = (int)project.UserAction;
                prms[10] = new SqlParameter("@userActionDateTime", SqlDbType.DateTime);
                prms[10].Value = timeStamp;
                prms[11] = new SqlParameter("@fosProjectId", SqlDbType.VarChar);
                prms[11].Value = project.FosProjectId;
                prms[12] = new SqlParameter("@certificationLiaisonUserDbId", SqlDbType.Int);
                prms[12].Value = project.CertificationLiaisonUserDbId;
                prms[13] = new SqlParameter("@certificationLiaisonUserFullName", SqlDbType.VarChar);
                prms[13].Value = project.CertificationLiaisonUserFullName;
                prms[14] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
                prms[14].Value = timeStamp;
                prms[15] = new SqlParameter("@advanceableFiscalYear", SqlDbType.Int);
                prms[15].Value = project.AdvanceableFiscalYear;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab the ProjectHistoryDbId
                qry =
                    @"
                        select max(projecthistorydbid) as maxprojecthistorydbid
                        from projecthistory
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
                projectHistoryDbId = Convert.ToInt32(dt.Rows[0]["maxprojecthistorydbid"]);
            }

            return projectHistoryDbId;
        }

        public int AddProjectWorkConceptHistory(int projectHistoryDbId, WorkConcept wc)
        {
            int projectWorkConceptHistoryDbId = 0;

            lock (database.databaseLock)
            {
                string qry =
                    @"
                        insert into projectworkconcepthistory (projecthistorydbid, workconceptdbid, workconceptcode, workConceptDescription, 
                            certifiedworkconceptcode, certifiedworkconceptdescription,
                            structureid, fiscalyear, quasicertified, status, fromproposedlist, currentfiscalyear, 
                            projectyear, fromeligibilitylist, fromfiips, evaluate)
                        values (@projectHistoryDbId, @workConceptDbId, @workConceptCode, @workConceptDescription,
                            @certifiedWorkConceptCode, @certifiedWorkConceptDescription,
                            @structureId, @fiscalYear, @quasiCertified, @status, @fromProposedList, @currentFiscalYear, 
                            @projectYear, @fromEligibilityList, @fromFiips, @evaluate)
                    ";
                SqlParameter[] prms = new SqlParameter[16];
                prms[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[0].Value = projectHistoryDbId;
                prms[1] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
                prms[1].Value = wc.WorkConceptDbId;
                prms[2] = new SqlParameter("@workConceptCode", SqlDbType.VarChar);
                prms[2].Value = wc.WorkConceptCode;
                prms[3] = new SqlParameter("@workConceptDescription", SqlDbType.VarChar);
                prms[3].Value = wc.WorkConceptDescription;
                prms[4] = new SqlParameter("@certifiedWorkConceptCode", SqlDbType.VarChar);
                prms[4].Value = wc.CertifiedWorkConceptCode;
                prms[5] = new SqlParameter("@certifiedWorkConceptDescription", SqlDbType.VarChar);
                prms[5].Value = wc.CertifiedWorkConceptDescription;
                prms[6] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[6].Value = wc.StructureId;
                prms[7] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                prms[7].Value = wc.FiscalYear;
                prms[8] = new SqlParameter("@quasiCertified", SqlDbType.Bit);
                prms[8].Value = 1;
                prms[9] = new SqlParameter("@status", SqlDbType.Int);
                prms[9].Value = (int)wc.Status;
                prms[10] = new SqlParameter("@fromProposedList", SqlDbType.Int);
                prms[10].Value = wc.FromProposedList ? 1 : 0;
                prms[11] = new SqlParameter("@currentFiscalYear", SqlDbType.Int);
                prms[11].Value = database.currentFiscalYear;
                prms[12] = new SqlParameter("@projectYear", SqlDbType.Int);
                prms[12].Value = wc.ProjectYear;
                prms[13] = new SqlParameter("@fromEligibilityList", SqlDbType.Int);
                prms[13].Value = wc.FromEligibilityList ? 1 : 0;
                prms[14] = new SqlParameter("@fromFiips", SqlDbType.Int);
                prms[14].Value = wc.FromFiips ? 1 : 0;
                prms[15] = new SqlParameter("@evaluate", SqlDbType.Int);
                prms[15].Value = wc.Evaluate ? 1 : 0;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab the ProjectWorkConceptHistoryDbId
                qry =
                    @"
                        select max(projectworkconcepthistorydbid) as maxprojectworkconcepthistorydbid
                        from projectworkconcepthistory
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
                projectWorkConceptHistoryDbId = Convert.ToInt32(dt.Rows[0]["maxprojectworkconcepthistorydbid"]);
            }

            return projectWorkConceptHistoryDbId;
        }

        public int AddProposedWorkConcept(WorkConcept wc)
        {
            int workConceptDbId = 0;

            lock (database.databaseLock)
            {
                string qry =
                    @"
                        insert into proposedlist (structureid, region, regionnumber, workconceptcode, workconceptdesc, fiscalyear, reasoncategory,
                            proposedbyuserdbid, proposedbyuserfullname, proposeddate, active)
                        values (@structureId, @region, @regionNumber, @workConceptCode, @workConceptDesc, @fiscalYear, @reasonCategory,
                            @proposedByUserDbId, @proposedByUserFullName, @proposedDate, @active)
                    ";
                SqlParameter[] prms = new SqlParameter[11];
                prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[0].Value = wc.StructureId;
                prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[1].Value = wc.Region;
                prms[2] = new SqlParameter("@regionNumber", SqlDbType.VarChar);
                prms[2].Value = wc.RegionNumber;
                prms[3] = new SqlParameter("@workConceptCode", SqlDbType.VarChar);
                prms[3].Value = wc.WorkConceptCode;
                prms[4] = new SqlParameter("@workConceptDesc", SqlDbType.VarChar);
                prms[4].Value = wc.WorkConceptDescription;
                prms[5] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                prms[5].Value = wc.FiscalYear;
                prms[6] = new SqlParameter("@reasonCategory", SqlDbType.VarChar);
                prms[6].Value = wc.ReasonCategory;
                prms[7] = new SqlParameter("@proposedByUserDbId", SqlDbType.Int);
                prms[7].Value = wc.ProposedByUserDbId;
                prms[8] = new SqlParameter("@proposedByUserFullName", SqlDbType.VarChar);
                prms[8].Value = wc.ProposedByUserFullName;
                prms[9] = new SqlParameter("@proposedDate", SqlDbType.DateTime);
                prms[9].Value = wc.ProposedDate;
                prms[10] = new SqlParameter("@active", SqlDbType.Bit);
                prms[10].Value = 1;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab the ProjectDbId
                qry =
                    @"
                        select max(workconceptdbid) as maxworkconceptdbid
                        from proposedlist
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
                workConceptDbId = Convert.ToInt32(dt.Rows[0]["maxworkconceptdbid"]);
            }

            return workConceptDbId;
        }
        #endregion

        public bool AuthenticateUser(string userName, string userPassword)
        {
            bool userIsValid = true;
            string qry = @"
                                select *
                                from users
                                where username = @userName
                                    and userpassword = @userPassword
                            ";

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@userName", SqlDbType.VarChar);
            prms[0].Value = userName;
            prms[1] = new SqlParameter("@userPassword", SqlDbType.NVarChar);
            prms[1].Value = userPassword;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt.Rows.Count == 0)
            {
                userIsValid = false;
            }

            if (userIsValid)
            {
                LogUserActivity(Convert.ToInt32(dt.Rows[0]["userdbid"]), "login");
            }

            return userIsValid;
        }

        public bool CloseDatabaseConnection(string dataSource)
        {
            bool successful = true;

            switch (dataSource.ToUpper())
            {
                case "WISAMS":
                    try
                    {
                        database.wisamsConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        successful = false;
                    }

                    break;
            }

            return successful;
        }

        public void DeactivateProposedWorkConcept(int workConceptDbId, string structureId)
        {
            string qry =
                @"
                    update proposedlist
                    set active = 0
                    where workconceptdbid = @workConceptDbId
                        and structureId = @structureId
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
            prms[0].Value = workConceptDbId;
            prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[1].Value = structureId;
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        #region SQL Execute
        public void ExecuteInsertUpdateDelete(string qry, SqlParameter[] prms, SqlConnection conn)
        {
            try
            {
                SqlCommand cmd1 = new SqlCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddRange(prms);
                cmd1.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally
            {
                //cmd1.Parameters.Clear();
            }
        }

        public void ExecuteInsertUpdateDelete(string qry, SqlConnection conn)
        {
            try
            {
                SqlCommand cmd1 = new SqlCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally { }
        }

        public DataTable ExecuteSelect(string qry, SqlConnection conn)
        {
            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd1 = new SqlCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);
                adp1.Fill(ds);
                adp1.Dispose();
                dt = ds.Tables[0];
            }
            catch (SqlException e)
            {
                Debug.Print("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Debug.Print("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally { }

            return dt;
        }

        public DataTable ExecuteSelect(string qry, SqlParameter[] prms, SqlConnection conn)
        {
            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd1 = new SqlCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddRange(prms);
                SqlDataAdapter adp1 = new SqlDataAdapter(cmd1);
                adp1.Fill(ds);
                adp1.Dispose();
                dt = ds.Tables[0];
            }
            catch (SqlException e)
            {
                Debug.Print("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Debug.Print("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally { }

            return dt;
        }

        public DataTable ExecuteSelect(string qry, OracleConnection conn)
        {
            OpenDatabaseConnection("HSI");
            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                OracleCommand cmd1 = new OracleCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                OracleDataAdapter adp1 = new OracleDataAdapter(cmd1);
                adp1.Fill(ds);
                adp1.Dispose();
                dt = ds.Tables[0];
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally { }

            return dt;
        }

        public DataTable ExecuteSelect(string qry, OracleParameter[] prms, OracleConnection conn)
        {
            OpenDatabaseConnection("HSI");
            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                OracleCommand cmd1 = new OracleCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddRange(prms);
                OracleDataAdapter adp1 = new OracleDataAdapter(cmd1);
                adp1.Fill(ds);
                adp1.Dispose();
                dt = ds.Tables[0];
            }
            catch (SqlException e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally { }

            return dt;
        }
        #endregion

        #region Getters
        public List<WorkConcept> GetAllWorkConcepts()
        {
            if (database.allWorkConcepts.Count() == 0)
            {
                List<WorkConcept> wcs = new List<WorkConcept>();
                database.allWorkConcepts = wcs;

                string qry = @"
                            select *
                            from workaction
                            where active = 1
                            order by workactiondesc, workactioncode
                        ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

                foreach (DataRow dr in dt.Rows)
                {
                    WorkConcept wc = new WorkConcept();
                    wc.WorkConceptCode = dr["workactioncode"].ToString();
                    wc.WorkConceptDescription = dr["workactiondesc"].ToString();

                    if (dr["earlierfy"] != DBNull.Value)
                    {
                        wc.EarlierFiscalYear = Convert.ToInt32(dr["earlierfy"]);
                    }
                    else
                    {
                        wc.EarlierFiscalYear = -99;
                    }

                    if (dr["laterfy"] != DBNull.Value)
                    {
                        wc.LaterFiscalYear = Convert.ToInt32(dr["laterfy"]);
                    }
                    else
                    {
                        wc.LaterFiscalYear = -99;
                    }

                    if (dr["mapmarkertype"] != DBNull.Value)
                    {
                        wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                    }
                    else
                    {
                        wc.MapMarkerType = (GMarkerGoogleType)3;
                    }

                    wcs.Add(wc);
                }
            }

            return database.allWorkConcepts;
        }

        public WiSamEntities.StructureWorkAction GetStructureWorkAction(string workActionCode)
        {
            WiSamEntities.StructureWorkAction swa = null;
            string qry = @"
                                select WorkActionCode, WorkActionDesc, WorkActionNotes, Unit, UnitCost,
                                        MinUnitCost, MaxUnitCost, UnitCostFormula, UseUnitCostFormula,
                                        CostFormula, CostNotes, Improvement, CombinedWorkAction,
                                        Repeatable, RepeatFrequency, Preservation, CombinedWorkActionCodes, IncludesOverlay,
                                        LifeExtension
                                from WorkAction
                                where WorkActionCode = @workActionCode
                                    and Active = 1
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[0].Value = workActionCode;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                swa = new WiSamEntities.StructureWorkAction(dr["WORKACTIONCODE"].ToString());
                swa.WorkActionDesc = dr["WORKACTIONDESC"].ToString();

                if (!String.IsNullOrEmpty(dr["WORKACTIONNOTES"].ToString()))
                {
                    swa.WorkActionNotes = dr["WORKACTIONNOTES"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["UNIT"].ToString()))
                {
                    swa.Unit = dr["UNIT"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["UNITCOST"].ToString()))
                {
                    swa.UnitCost = Convert.ToDouble(dr["UNITCOST"]);
                }

                if (!String.IsNullOrEmpty(dr["MINUNITCOST"].ToString()))
                {
                    swa.MinUnitCost = Convert.ToDouble(dr["MINUNITCOST"]);
                }

                if (!String.IsNullOrEmpty(dr["MAXUNITCOST"].ToString()))
                {
                    swa.MaxUnitCost = Convert.ToDouble(dr["MaxUNITCOST"]);
                }

                if (!String.IsNullOrEmpty(dr["UNITCOSTFORMULA"].ToString()))
                {
                    swa.UnitCostFormula = dr["UNITCOSTFORMULA"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["LIFEEXTENSION"].ToString()))
                {
                    swa.LifeExtension = Convert.ToDouble(dr["LIFEEXTENSION"]);
                }

                if (!String.IsNullOrEmpty(dr["COSTFORMULA"].ToString()))
                {
                    swa.CostFormula = dr["COSTFORMULA"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["COSTNOTES"].ToString()))
                {
                    swa.CostNotes = dr["COSTNOTES"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["COMBINEDWORKACTIONCODES"].ToString()))
                {
                    swa.CombinedWorkActionCodes = dr["COMBINEDWORKACTIONCODES"].ToString();
                }

                swa.UseUnitCostFormula = Convert.ToBoolean(dr["USEUNITCOSTFORMULA"]);
                swa.CombinedWorkAction = Convert.ToBoolean(dr["COMBINEDWORKACTION"]);
                swa.Improvement = Convert.ToBoolean(dr["IMPROVEMENT"]);
                swa.IncludesOverlay = Convert.ToBoolean(dr["INCLUDESOVERLAY"]);
                swa.Preservation = Convert.ToBoolean(dr["PRESERVATION"]);
                swa.Repeatable = Convert.ToBoolean(dr["REPEATABLE"]);
                swa.RepeatFrequency = Convert.ToInt32(dr["REPEATFREQUENCY"]);


                //swa.PotentialIncidentals = GetSecondaryWorkActions(swa.WorkActionCode);
            }

            string qry2 = @"
                                select WorkActionCode, NbiClassificationCode as Code, Convert(varchar(10), Benefit) as Benefit
                                from WorkActionNbiBenefit
                                where WorkActionCode = @workActionCode1
                                union
                                select WorkActionCode, ElementClassificationCode as Code, Benefit
                                from WorkActionElementBenefit
                                where WorkActionCode = @workActionCode2
                            ";
            SqlParameter[] prms2 = new SqlParameter[2];
            prms2[0] = new SqlParameter("@workActionCode1", SqlDbType.VarChar);
            prms2[0].Value = workActionCode;
            prms2[1] = new SqlParameter("@workActionCode2", SqlDbType.VarChar);
            prms2[1].Value = workActionCode;
            DataTable dt2 = ExecuteSelect(qry2, prms2, database.wisamsConnection);

            if (dt2 != null && dt2.Rows.Count > 0)
            {
                foreach (DataRow dr in dt2.Rows)
                {
                    swa.ConditionBenefit += String.Format("{0} to {1}\r\n", dr["CODE"].ToString(), dr["BENEFIT"].ToString());
                }
            }

            return swa;
        }

        public List<Project> GetDeletedProjectsForStructure(string structureId)
        {
            List<Project> projects = new List<Project>();
            string qry =
                @"
                    select p.*, ph.*, ph.precertificationliaisonuserfullname as precertLiaison,
                        pwch.projectworkconcepthistorydbid, pwch.workconceptdbid, pwch.workconceptcode, pwch.workconceptdescription,
                        pwch.certifiedworkconceptcode, pwch.certifiedworkconceptdescription, pwch.structureid,
                        pwch.plannedstructureid, pwch.currentfiscalyear as wcCurrentFiscalYear, pwch.fiscalyear as wcFiscalYear, pwch.projectyear as wcProjectYear,
                        pwch.priorityscore, pwch.cost, pwch.fromeligibilitylist, pwch.fromfiips, pwch.evaluate, pwch.earlierfiscalyear, pwch.laterfiscalyear,
                        pwch.changejustifications, pwch.changenotes, pwch.status as wcstatus, pwch.precertificationdecision, pwch.precertificationdecisiondatetime,
                        pwch.precertificationdecisionreasoncategory, pwch.precertificationdecisionreasonexplanation, pwch.precertificationdecisioninternalcomments,
                        pwch.fromproposedlist, pwch.certificationdecision as wcCertificationDecision, pwch.certificationdatetime as wcCertificationDateTime, pwch.certificationprimaryworktypecomments,
                        pwch.certificationsecondaryworktypecomments, pwch.certificationadditionalcomments, pwch.estimatedconstructioncost,
                        pwch.estimateddesignlevelofeffort, pwch.designresourcing,
                        s.lat, s.lngt, s.latdecimal, s.lngtdecimal
                    from project p
                    left join projecthistory ph
                        on p.projectdbid = ph.projectdbid
                    left join projectworkconcepthistory pwch
                        on ph.projecthistorydbid = pwch.projecthistorydbid
                    left join structure s
                        on pwch.structureid = s.structureid
                    where p.projectdbid = ph.projectdbid
                        and p.deletedate is not null
                        and ph.projecthistorydbid = 
                            (select max(projecthistorydbid)
                                from projecthistory
                                where projectdbid = p.projectdbid
                                    and userdbid is not null and ph.userdbid <> 0
                                    
                            )  
                        and pwch.structureid = @structureId
                        
                ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Project project = new Project();
                    project.ProjectDbId = Convert.ToInt32(dr["projectdbid"]);
                    project.Status = StructuresProgramType.ProjectStatus.Deleted;
                    project.UserAction = StructuresProgramType.ProjectUserAction.DeletedProject;
                    project.UserActionDateTime = Convert.ToDateTime(dr["deletedate"]);
                    project.UserFullName = Convert.ToString(dr["userfullname"]);
                    project.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                    WorkConcept wc = new WorkConcept();
                    wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                    wc.StructureId = dr["structureid"].ToString().Trim();
                    wc.WorkConceptCode = dr["workconceptcode"].ToString().Trim();
                    wc.WorkConceptDescription = dr["workconceptdescription"].ToString().Trim();
                    wc.CertifiedWorkConceptCode = dr["certifiedworkconceptcode"].ToString().Trim();
                    wc.CertifiedWorkConceptDescription = dr["certifiedworkconceptdescription"].ToString().Trim();
                    wc.FiscalYear = Convert.ToInt32(dr["wcfiscalyear"]);

                    try
                    {
                        project.FosProjectId = dr["fosprojectid"].ToString();
                    }
                    catch { }

                    int advanceableFiscalYear = 0;
                    try
                    {
                        advanceableFiscalYear = Convert.ToInt32(dr["advanceablefiscalyear"]);
                    }
                    catch { }

                    project.AdvanceableFiscalYear = advanceableFiscalYear;

                    try
                    {
                        wc.ChangeJustifications = dr["changejustifications"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.ChangeJustificationNotes = dr["changenotes"].ToString();
                    }
                    catch { }

                    try
                    {
                        project.PrecertificationLiaisonUserFullName = dr["precertLiaison"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.PrecertificationDecision = (StructuresProgramType.PrecertificatioReviewDecision)dr["precertificationdecision"];
                    }
                    catch { }

                    try
                    {
                        wc.PrecertificationDecisionReasonCategory = dr["precertificationdecisionreasoncategory"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.PrecertificationDecisionReasonExplanation = dr["precertificationdecisionreasonexplanation"].ToString();
                    }
                    catch { }

                    try
                    {
                        project.CertificationLiaisonUserFullName = dr["certificationliaisonuserfullname"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.CertificationPrimaryWorkTypeComments = dr["certificationprimaryworktypecomments"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.CertificationSecondaryWorkTypeComments = dr["certificationsecondaryworkcomments"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.CertificationAdditionalComments = dr["certificationadditionalcomments"].ToString();
                    }
                    catch { }


                    project.WorkConcepts.Add(wc);
                    projects.Add(project);
                }
            }

            return projects;
        }

        public List<Project> GetDeletedWorkConceptsForStructure(string structureId)
        {
            List<Project> projects = new List<Project>();
            string qry =
                @"
                    select p.*, ph.*, ph.precertificationliaisonuserfullname as precertLiaison,
                        pwch.projectworkconcepthistorydbid, pwch.workconceptdbid, pwch.workconceptcode, pwch.workconceptdescription,
                        pwch.certifiedworkconceptcode, pwch.certifiedworkconceptdescription, pwch.structureid,
                        pwch.plannedstructureid, pwch.currentfiscalyear as wcCurrentFiscalYear, pwch.fiscalyear as wcFiscalYear, pwch.projectyear as wcProjectYear,
                        pwch.priorityscore, pwch.cost, pwch.fromeligibilitylist, pwch.fromfiips, pwch.evaluate, pwch.earlierfiscalyear, pwch.laterfiscalyear,
                        pwch.changejustifications, pwch.changenotes, pwch.status as wcstatus, pwch.precertificationdecision, pwch.precertificationdecisiondatetime,
                        pwch.precertificationdecisionreasoncategory, pwch.precertificationdecisionreasonexplanation, pwch.precertificationdecisioninternalcomments,
                        pwch.fromproposedlist, pwch.certificationdecision as wcCertificationDecision, pwch.certificationdatetime as wcCertificationDateTime, pwch.certificationprimaryworktypecomments,
                        pwch.certificationsecondaryworktypecomments, pwch.certificationadditionalcomments, pwch.estimatedconstructioncost,
                        pwch.estimateddesignlevelofeffort, pwch.designresourcing
                    from project p
                    left join projecthistory ph
                        on p.projectdbid = ph.projectdbid
                    left join projectworkconcepthistory pwch
                        on ph.projecthistorydbid = pwch.projecthistorydbid
                    
                    where p.projectdbid = ph.projectdbid
                        
                        and ph.projecthistorydbid <
                            (select max(projecthistorydbid)
                                from projecthistory
                                where projectdbid = p.projectdbid
                                    and userdbid is not null and userdbid <> 0
                                    
                            )  
                        and pwch.structureid = @structureId
						and pwch.structureid not in
							(select structureid
							from projectworkconcepthistory
							where projecthistorydbid = 
								(select max(projecthistorydbid)
									from projecthistory
									where projectdbid = p.projectdbid
                                    and userdbid is not null and userdbid <> 0
								)
							)
						order by p.projectdbid, useractiondatetime desc
                        
                ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                int currentProjectDbId = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    if (Convert.ToInt32(dr["projectdbid"]) == currentProjectDbId)
                    {
                        continue;
                    }

                    currentProjectDbId = Convert.ToInt32(dr["projectdbid"]);
                    Project project = new Project();
                    project.ProjectDbId = Convert.ToInt32(dr["projectdbid"]);
                    project.UserAction = StructuresProgramType.ProjectUserAction.DeletedWorkConcept;

                    WorkConcept wc = new WorkConcept();
                    wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                    wc.StructureId = dr["structureid"].ToString().Trim();
                    wc.WorkConceptCode = dr["workconceptcode"].ToString().Trim();
                    wc.WorkConceptDescription = dr["workconceptdescription"].ToString().Trim();
                    wc.CertifiedWorkConceptCode = dr["certifiedworkconceptcode"].ToString().Trim();
                    wc.CertifiedWorkConceptDescription = dr["certifiedworkconceptdescription"].ToString().Trim();
                    wc.FiscalYear = Convert.ToInt32(dr["wcfiscalyear"]);
                    project.WorkConcepts.Add(wc);
                    try
                    {
                        wc.ChangeJustifications = dr["changejustifications"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.ChangeJustificationNotes = dr["changenotes"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.PrecertificationDecision = (StructuresProgramType.PrecertificatioReviewDecision)dr["precertificationdecision"];
                    }
                    catch { }

                    try
                    {
                        wc.PrecertificationDecisionReasonCategory = dr["precertificationdecisionreasoncategory"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.PrecertificationDecisionReasonExplanation = dr["precertificationdecisionreasonexplanation"].ToString();
                    }
                    catch { }

                    try
                    {
                        project.CertificationLiaisonUserFullName = dr["certificationliaisonuserfullname"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.CertificationPrimaryWorkTypeComments = dr["certificationprimaryworktypecomments"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.CertificationSecondaryWorkTypeComments = dr["certificationsecondaryworkcomments"].ToString();
                    }
                    catch { }

                    try
                    {
                        wc.CertificationAdditionalComments = dr["certificationadditionalcomments"].ToString();
                    }
                    catch { }

                    string qry2 =
               @"
                    select p.*, ph.*, 
                        pwch.projectworkconcepthistorydbid, pwch.workconceptdbid, pwch.workconceptcode, pwch.workconceptdescription,
                        pwch.certifiedworkconceptcode, pwch.certifiedworkconceptdescription, pwch.structureid,
                        pwch.plannedstructureid, pwch.currentfiscalyear as wcCurrentFiscalYear, pwch.fiscalyear as wcFiscalYear, pwch.projectyear as wcProjectYear,
                        pwch.priorityscore, pwch.cost, pwch.fromeligibilitylist, pwch.fromfiips, pwch.evaluate, pwch.earlierfiscalyear, pwch.laterfiscalyear,
                        pwch.changejustifications, pwch.changenotes, pwch.status as wcstatus, pwch.precertificationdecision, pwch.precertificationdecisiondatetime,
                        pwch.precertificationdecisionreasoncategory, pwch.precertificationdecisionreasonexplanation, pwch.precertificationdecisioninternalcomments,
                        pwch.fromproposedlist, pwch.certificationdecision as wcCertificationDecision, pwch.certificationdatetime as wcCertificationDateTime, pwch.certificationprimaryworktypecomments,
                        pwch.certificationsecondaryworktypecomments, pwch.certificationadditionalcomments, pwch.estimatedconstructioncost,
                        pwch.estimateddesignlevelofeffort, pwch.designresourcing,
                        s.lat, s.lngt, s.latdecimal, s.lngtdecimal
                    from project p
                    left join projecthistory ph
                        on p.projectdbid = ph.projectdbid
                    left join projectworkconcepthistory pwch
                        on ph.projecthistorydbid = pwch.projecthistorydbid
                    left join structure s
                        on pwch.structureid = s.structureid
                    where p.projectdbid = ph.projectdbid
                        and ph.projecthistorydbid = 
                            (select max(projecthistorydbid)
                                from projecthistory
                                where projectdbid = p.projectdbid
                                    and userdbid is not null and ph.userdbid <> 0
                                    
                            )  
                        and p.projectdbid = @projectDbId
                        
                ";

                    SqlParameter[] prms2 = new SqlParameter[1];
                    prms2[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                    prms2[0].Value = currentProjectDbId;
                    DataTable dt2 = ExecuteSelect(qry2, prms2, database.wisamsConnection);

                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        DataRow dr2 = dt2.Rows[0];
                        project.UserActionDateTime = Convert.ToDateTime(dr2["useractiondatetime"]);
                        project.UserFullName = Convert.ToString(dr2["userfullname"]);
                        project.FiscalYear = Convert.ToInt32(dr2["fiscalyear"]);

                        try
                        {
                            project.AdvanceableFiscalYear = Convert.ToInt32(dr2["advanceablefiscalyear"]);
                        }
                        catch { }

                        try
                        {
                            project.PrecertificationLiaisonUserFullName = dr2["precertificationliaisonuserfullname"].ToString();
                        }
                        catch { }

                        try
                        {
                            project.CertificationLiaisonUserFullName = dr2["certificationliaisonuserfullname"].ToString();
                        }
                        catch { }

                        try
                        {
                            project.FosProjectId = dr2["fosprojectid"].ToString();
                        }
                        catch { }
                    }




                    projects.Add(project);
                }
            }

            return projects;
        }

        public List<ElementWorkConcept> GetElementWorkConceptPairings(string structureId, int projectWorkConceptHistoryDbId, DateTime certificationDateTime)
        {
            List<ElementWorkConcept> pairings = new List<ElementWorkConcept>();
            string qry =
                @"
                    select p.*, w.workactiondesc
                    from projectelementworkconcept p left join workaction w
                        on p.workconceptcode = w.workactioncode
                    where p.projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                        and p.certificationdatetime = @certificationDateTime
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
            prms[0].Value = projectWorkConceptHistoryDbId;
            prms[1] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
            prms[1].Value = certificationDateTime;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ElementWorkConcept ewc = new ElementWorkConcept();
                    ewc.StructureId = structureId;
                    ewc.ElementWorkConceptDbId = Convert.ToInt32(dr["elementworkconceptdbid"]);
                    ewc.ProjectWorkConceptHistoryDbId = projectWorkConceptHistoryDbId;

                    try
                    {
                        ewc.CertificationDateTime = certificationDateTime;
                    }
                    catch { }

                    ewc.ElementNumber = Convert.ToInt32(dr["elementnumber"]);
                    ewc.WorkConceptCode = dr["workconceptcode"].ToString();
                    ewc.WorkConceptDescription = dr["workactiondesc"].ToString();
                    ewc.WorkConceptLevel = dr["workconceptlevel"].ToString();

                    if (dr["comments"] != DBNull.Value)
                    {
                        ewc.Comments = dr["comments"].ToString();
                    }

                    pairings.Add(ewc);
                }
            }

            return pairings;
        }

        public WorkConcept GetEligibleWorkConcept(int workConceptDbId)
        {
            WorkConcept wc = null;
            string qry = @"
                            select *
                            from eligibilitylist
                            where workconceptdbid = @workConceptDbId
                        ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
            prms[0].Value = workConceptDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                wc = new WorkConcept();
                wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                wc.StructureId = dr["structureid"].ToString().Trim();
                string[] parsed = repo.ParseWorkConceptFullDescription(dr["primaryworkconcept"].ToString().Trim());
                wc.WorkConceptCode = parsed[0].Trim();
                wc.WorkConceptDescription = parsed[1].Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                wc.Cost = Convert.ToInt32(dr["primarycost"]);
                wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
            }

            return wc;
        }

        public List<WorkConcept> GetEligibleWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select el.*, w.earlierfy, w.laterfy, w.mapmarkertype, s.lat, s.lngt, s.latdecimal, s.lngtdecimal
                            from eligibilitylist el
                            join workaction w
                                on el.primaryworkconceptcode = w.workactioncode
                            left join structure s
                                on el.structureid = s.structureid
                            where fiscalyear >= @startFiscalYear
                                and fiscalyear <= @endFiscalYear
                                and primaryworkconcept is not null
                                and upper(primaryworkconcept) <> 'NONE'
                                and el.active = 1
                        ";
            SqlParameter[] prms = new SqlParameter[2];

            if (!region.Equals("any"))
            {
                prms = null;
                prms = new SqlParameter[3];
                qry +=
                    @"
                        and region = @region
                    ";
            }

            qry += @"
                        order by structureid
                    ";

            prms[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms[0].Value = startFiscalYear;
            prms[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms[1].Value = endFiscalYear;

            if (!region.Equals("any"))
            {
                prms[2] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[2].Value = region;
            }

            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                wc.StructureId = dr["structureid"].ToString().Trim();
                string[] parsed = repo.ParseWorkConceptFullDescription(dr["primaryworkconcept"].ToString().Trim());
                //wc.WorkConceptCode = parsed[0].Trim();
                //wc.WorkConceptDescription = parsed[1].Trim();
                wc.WorkConceptCode = dr["primaryworkconceptcode"].ToString().Trim();
                wc.WorkConceptDescription = GetWorkConceptDescription(wc.WorkConceptCode);
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                wc.Region = dr["region"].ToString();
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                wc.Cost = Convert.ToInt32(dr["primarycost"]);
                wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                wc.FromEligibilityList = true;

                try
                {
                    wc.EarlierFiscalYear = Convert.ToInt32(dr["earlierfy"]);
                }
                catch { }

                try
                {
                    wc.LaterFiscalYear = Convert.ToInt32(dr["laterfy"]);
                }
                catch { }

                //wc.GeoLocation = GetStructureGeoLocation(wc.StructureId);

                if (dr["lat"] != DBNull.Value)
                {
                    wc.GeoLocation.HsiLatitude = dr["lat"].ToString();
                }

                if (dr["lngt"] != DBNull.Value)
                {
                    wc.GeoLocation.HsiLongitude = dr["lngt"].ToString();
                }

                if (dr["latdecimal"] != DBNull.Value)
                {
                    wc.GeoLocation.LatitudeDecimal = Convert.ToSingle(dr["latdecimal"]);
                }

                if (dr["lngtdecimal"] != DBNull.Value)
                {
                    wc.GeoLocation.LongitudeDecimal = Convert.ToSingle(dr["lngtdecimal"]);
                }

                if (dr["mapmarkertype"] != DBNull.Value)
                {
                    wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                }
                else
                {
                    wc.MapMarkerType = (GMarkerGoogleType)3;
                }

                workConcepts.Add(wc);
                counter++;
            }

            workConcepts.AddRange(GetProposedWorkConcepts(startFiscalYear, endFiscalYear, region));
            database.eligibleWorkConcepts = workConcepts;
            return workConcepts;
        }

        public List<WorkConcept> GetEligibleWorkConcepts(int fiscalYear, string region)
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select el.*, w.mapmarkertype
                            from eligibilitylist el, workaction w
                            where fiscalyear = @fiscalYear
                                and region = @region
                                and primaryworkconcept is not null
                                and upper(primaryworkconcept) <> 'NONE'
                                and primaryworkconceptcode = w.workactioncode
                                
                        ";
            //and structureid not in 
            //(select structureid from structureprogramreviewcurrent)
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
            prms[1].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                wc.StructureId = dr["structureid"].ToString().Trim();
                string[] parsed = repo.ParseWorkConceptFullDescription(dr["primaryworkconcept"].ToString().Trim());
                wc.WorkConceptCode = parsed[0].Trim();
                wc.WorkConceptDescription = parsed[1].Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                wc.Region = dr["region"].ToString();
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                wc.Cost = Convert.ToInt32(dr["primarycost"]);
                wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                wc.FromEligibilityList = true;

                if (dr["mapmarkertype"] != DBNull.Value)
                {
                    wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                }
                else
                {
                    wc.MapMarkerType = (GMarkerGoogleType)3;
                }

                workConcepts.Add(wc);
                counter++;
            }

            return workConcepts;
        }

        public List<WorkConcept> GetEligibleWorkConcepts(int fiscalYear)
        {
            /*
            select el.*, wa.mapmarkertype
                            from eligibilitylist el, workaction wa
                            where el.fiscalyear = @fiscalYear
                                and el.primaryworkconcept is not null
                                and el.primaryworkconcept <> 'NONE'
                                and el.primaryworkconceptcode = wa.workactioncode*/

            /*
            select el.*
                            from eligibilitylist el
                            where el.fiscalyear = @fiscalYear
                                and el.primaryworkconcept is not null
                                and el.primaryworkconcept <> 'NONE'

            */
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select el.*, wa.mapmarkertype
                            from eligibilitylist el, workaction wa
                            where el.fiscalyear = @fiscalYear
                                and el.primaryworkconcept is not null
                                and upper(el.primaryworkconcept) <> 'NONE'
                                and el.primaryworkconceptcode = wa.workactioncode
                                
                        ";
            // and structureid not in 
            //(select structureid from structureprogramreviewcurrent)
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                wc.StructureId = dr["structureid"].ToString().Trim();
                string[] parsed = repo.ParseWorkConceptFullDescription(dr["primaryworkconcept"].ToString().Trim());
                wc.WorkConceptCode = parsed[0].Trim();
                wc.WorkConceptDescription = parsed[1].Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                wc.Region = dr["region"].ToString();
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                wc.Cost = Convert.ToInt32(dr["primarycost"]);
                wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                wc.FromEligibilityList = true;

                if (dr["mapmarkertype"] != DBNull.Value)
                {
                    wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                }
                else
                {
                    wc.MapMarkerType = (GMarkerGoogleType)3;
                }

                workConcepts.Add(wc);
                counter++;
            }

            return workConcepts;
        }

        public DataTable GetEligibleWorkConceptsDataTable(int fiscalYear)
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select *
                            from eligibilitylist
                            where fiscalyear = @fiscalYear
                                and primaryworkconcept is not null
                        ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            return dt;
        }

        public List<Project> GetFiipsProjects(string region, int startFiscalYear, int endFiscalYear)
        {
            List<Project> projects = new List<Project>();
            string qry =
                @"
                    select *
                    from pmic
                    where state_fiscal_year >= @startFiscalYear
                        and state_fiscal_year <= @endFiscalYear
                        and ((extg_strc_id is not null
                                        and extg_strc_id <> '') or (plnd_strc_id is not null
                                        and plnd_strc_id <> ''))
                                        and strc_work_tycd is not null
                                        and strc_work_tycd <> ''
                    order by fos_proj_id, extg_strc_id, plnd_strc_id, tot_with_dlvy_amt desc
                ";
            SqlParameter[] prms = new SqlParameter[2];

            if (!region.Equals("any"))
            {
                qry =
                    @"
                        select *
                        from pmic
                        where state_fiscal_year = @startFiscalYear
                            and state_fiscal_year <= @endFiscalYear
                            and dot_rgn_cd = @region
                            and ((extg_strc_id is not null
                                        and extg_strc_id <> '') or (plnd_strc_id is not null
                                        and plnd_strc_id <> ''))
                                        and strc_work_tycd is not null
                                        and strc_work_tycd <> ''
                        order by fos_proj_id, extg_strc_id, plnd_strc_id, tot_with_dlvy_amt desc
                    ";
                prms = null;
                prms = new SqlParameter[3];
            }

            if (!region.Equals("any"))
            {
                prms[2] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[2].Value = region;
            }

            prms[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms[0].Value = startFiscalYear;
            prms[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms[1].Value = endFiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            string currentFosProjectId = "";
            string previousFosProjectId = "";
            Project currentProject = null;
            string currentStructureId = "";
            string previousStructureId = "";

            foreach (DataRow dr in dt.Rows)
            {
                currentFosProjectId = dr["fos_proj_id"].ToString();
                currentStructureId = dr["extg_strc_id"].ToString();

                if (!currentFosProjectId.Equals(previousFosProjectId))
                {
                    currentProject = new Project();
                    projects.Add(currentProject);
                    currentProject.ProjectDbId = 0;
                    currentProject.FosProjectId = currentFosProjectId;
                    currentProject.FiscalYear = Convert.ToInt32(dr["state_fiscal_year"]);
                    currentProject.Region = repo.GetRegionComboCode(dr["dot_rgn_cd"].ToString());
                    currentProject.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString();
                    currentProject.Status = StructuresProgramType.ProjectStatus.Fiips;
                    currentProject.FiipsCost = 0;
                    currentProject.LifecycleStageCode = dr["lfcy_stg_cd"].ToString();
                    currentProject.Description = dr["pproj_fos_cncp_txt"].ToString();

                    try
                    {
                        currentProject.PseDate = Convert.ToDateTime(dr["psedate"]);
                    }
                    catch { }

                    try
                    {
                        currentProject.EpseDate = Convert.ToDateTime(dr["earliestpsedate"]);
                    }
                    catch { }

                    currentProject.WorkConcepts = new List<WorkConcept>();
                    WorkConcept workConcept = new WorkConcept(dr, database.currentFiscalYear, currentProject.FiscalYear, database.currentProjectYear);
                    currentProject.FiipsCost += workConcept.Cost;
                    currentProject.WorkConcepts.Add(workConcept);
                    currentProject.NumberOfStructures++;
                }
                else // 
                {
                    WorkConcept workConcept = new WorkConcept(dr, database.currentFiscalYear, currentProject.FiscalYear, database.currentProjectYear);
                    currentProject.FiipsCost += workConcept.Cost;
                    currentProject.WorkConcepts.Add(workConcept);

                    if (!currentStructureId.Equals(previousStructureId))
                    {
                        currentProject.NumberOfStructures++;
                    }
                }

                previousFosProjectId = currentFosProjectId;
                previousStructureId = currentStructureId;
            }

            return projects;
        }

        public List<Project> GetFiipsProjects(int fiscalYear, string region = "any")
        {
            List<Project> projects = new List<Project>();
            string qry = @"
                                select distinct fos_proj_id, state_fiscal_year, dot_rgn_cd, pproj_cncp_cd
                                from pmic
                                where state_fiscal_year = @fiscalYear
                                    and isduplicate = 0
                                order by fos_proj_id
                            ";
            SqlParameter[] prms = new SqlParameter[1];

            if (!region.Equals("any"))
            {
                qry = @"
                            select distinct fos_proj_id, state_fiscal_year, dot_rgn_cd, pproj_cncp_cd
                                from pmic
                                where state_fiscal_year = @fiscalYear
                                    and dot_rgn_cd = @region
                                    and isduplicate = 0
                                order by fos_proj_id
                        ";
                prms = null;
                prms = new SqlParameter[2];
            }

            if (!region.Equals("any"))
            {
                prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[1].Value = region;
            }

            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                string currentFosProjectId = dr["fos_proj_id"].ToString();
                string qry2 = @"
                                    select *
                                    from pmic
                                    where fos_proj_id = @currentFosProjectId
                                        and ((extg_strc_id is not null
                                        and extg_strc_id <> '') or (plnd_strc_id is not null
                                        and plnd_strc_id <> ''))
                                        and strc_work_tycd is not null
                                        and strc_work_tycd <> ''
                                        and isduplicate = 0
                                    order by extg_strc_id, plnd_strc_id
                                ";
                // and estcp_ty_cd = 'LET'
                SqlParameter[] prms2 = new SqlParameter[1];
                prms2[0] = new SqlParameter("@currentFosProjectId", SqlDbType.VarChar);
                prms2[0].Value = currentFosProjectId;
                DataTable dt2 = ExecuteSelect(qry2, prms2, database.wisamsConnection);

                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    /*
                     currentProject.Region = GetRegionComboCode(dr["dot_rgn_cd"].ToString());
                    currentProject.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString();
                    currentProject.Status = StructuresProgramType.ProjectStatus.Fiips;
                    currentProject.FiipsCost = 0;
                    currentProject.LifecycleStageCode = dr["lfcy_stg_cd"].ToString();
                    currentProject.Description = dr["pproj_fos_cncp_txt"].ToString();
                     */
                    Project project = new Project();
                    project.ProjectDbId = 0;
                    project.FosProjectId = currentFosProjectId;
                    project.FiscalYear = Convert.ToInt32(dt2.Rows[0]["state_fiscal_year"]);
                    project.Region = repo.GetRegionComboCode(dt2.Rows[0]["dot_rgn_cd"].ToString());
                    project.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString();
                    project.Status = StructuresProgramType.ProjectStatus.Fiips;
                    project.FiipsCost = 0;
                    project.LifecycleStageCode = dt2.Rows[0]["lfcy_stg_cd"].ToString();
                    project.Description = dt2.Rows[0]["pproj_fos_cncp_txt"].ToString();
                    project.DesignId = dt2.Rows[0]["designprojectid"].ToString();

                    try
                    {
                        project.PseDate = Convert.ToDateTime(dt2.Rows[0]["psedate"]);
                    }
                    catch { }

                    try
                    {
                        project.EpseDate = Convert.ToDateTime(dt2.Rows[0]["earliestpsedate"]);
                    }
                    catch { }

                    string currentStructureId = "";

                    foreach (DataRow dr2 in dt2.Rows)
                    {
                        WorkConcept wc = new WorkConcept();
                        wc.FromFiips = true;
                        wc.Status = StructuresProgramType.WorkConceptStatus.Fiips;
                        wc.WorkConceptDbId = Convert.ToInt32(dr2["fiipsid"]);
                        wc.StructureId = dr2["extg_strc_id"].ToString().Trim();

                        if (!currentStructureId.Equals(wc.StructureId))
                        {
                            project.NumberOfStructures++;
                        }

                        wc.WorkConceptCode = dr2["workactioncode"].ToString().Trim();
                        wc.WorkConceptDescription = dr2["strc_work_tydc"].ToString().Trim();
                        wc.CertifiedWorkConceptCode = wc.WorkConceptCode;


                        if (wc.WorkConceptCode.Equals("07"))
                        {
                            wc.WorkConceptDescription = "PAINT";
                        }

                        wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                        wc.PlannedStructureId = dr2["plnd_strc_id"].ToString().Trim();

                        if (wc.WorkConceptCode.Equals("01") && wc.StructureId.Equals("") && !wc.PlannedStructureId.Equals(""))
                        {
                            wc.StructureId = wc.PlannedStructureId;
                        }

                        wc.Region = dr2["dot_rgn_cd"].ToString();
                        wc.CurrentFiscalYear = database.currentFiscalYear;
                        wc.FiscalYear = fiscalYear;
                        wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                        wc.Cost = Convert.ToInt32(dr2["tot_wo_dlvy_amt"]);
                        project.FiipsCost += wc.Cost;
                        wc.FosProjectId = dr2["fos_proj_id"].ToString().Trim();
                        wc.FiipsDescription = dr2["fndg_ctgy_desc"].ToString().Trim();
                        //wc.SecondaryWorkConcepts = dr2["secondaryworkactions"].ToString().Trim();
                        wc.FromEligibilityList = false;
                        wc.FiipsImprovementConcept = dr2["pproj_cncp_cd"].ToString().Trim();
                        wc.DotProgram = dr2["wdot_pgm_desc"].ToString().Trim();

                        switch (wc.Region)
                        {
                            case "SW":
                                wc.Region = "1-" + wc.Region;
                                break;
                            case "SE":
                                wc.Region = "2-" + wc.Region;
                                break;
                            case "NE":
                                wc.Region = "3-" + wc.Region;
                                break;
                            case "NC":
                                wc.Region = "4-" + wc.Region;
                                break;
                            case "NW":
                                wc.Region = "5-" + wc.Region;
                                break;
                        }

                        project.WorkConcepts.Add(wc);
                        currentStructureId = wc.StructureId;
                    }

                    projects.Add(project);
                }
            }

            return projects;
        }

        public List<WorkConcept> GetFiipsWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();

            for (int i = startFiscalYear; i <= endFiscalYear; i++)
            {
                if (region.Equals("any"))
                {
                    workConcepts.AddRange(GetFiipsWorkConcepts(i));
                }
                else
                {
                    workConcepts.AddRange(GetFiipsWorkConcepts(i, region));
                }
            }

            database.fiipsWorkConcepts = workConcepts;
            return workConcepts;
        }

        public List<WorkConcept> GetFiipsWorkConcepts(int fiscalYear, string region)
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select *
                            from pmic
                            where state_fiscal_year = @fiscalYear
                                and dot_rgn_cd = @region
                                and ((extg_strc_id is not null
                                        and extg_strc_id <> '') or (plnd_strc_id is not null
                                        and plnd_strc_id <> ''))
                                and strc_work_tycd is not null
                                and strc_work_tycd <> ''
                                and isduplicate = 0
                            order by extg_strc_id
                        ";
            //and estcp_ty_cd = 'LET'
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
            prms[1].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.FromFiips = true;
                wc.FromEligibilityList = false;
                wc.Evaluate = false;
                wc.Status = StructuresProgramType.WorkConceptStatus.Fiips;
                wc.WorkConceptDbId = Convert.ToInt32(dr["fiipsid"]);
                wc.StructureId = dr["extg_strc_id"].ToString().Trim();
                wc.WorkConceptCode = dr["workactioncode"].ToString().Trim();
                wc.WorkConceptDescription = dr["strc_work_tydc"].ToString().Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.CertifiedWorkConceptDescription;
                //wc.MapMarkerType = GMarkerGoogleType.blue_small;

                if (wc.WorkConceptCode.Equals("07"))
                {
                    wc.WorkConceptDescription = "PAINT";
                }

                wc.PlannedStructureId = dr["plnd_strc_id"].ToString().Trim();

                if (wc.WorkConceptCode.Equals("01") && wc.StructureId.Equals("") && !wc.PlannedStructureId.Equals(""))
                {
                    wc.StructureId = wc.PlannedStructureId;
                }

                wc.Region = dr["dot_rgn_cd"].ToString();
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = fiscalYear;
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                wc.Cost = Convert.ToInt32(dr["tot_wo_dlvy_amt"]);
                wc.FosProjectId = dr["fos_proj_id"].ToString().Trim();
                wc.FiipsDescription = dr["fndg_ctgy_desc"].ToString().Trim();
                wc.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString().Trim();

                if (dr["pproj_fnty_desc"] != DBNull.Value)
                {
                    wc.PlanningProjectFunctionalType = dr["pproj_fnty_desc"].ToString().Trim().ToUpper();
                }

                switch (wc.Region)
                {
                    case "SW":
                        wc.Region = "1-" + wc.Region;
                        break;
                    case "SE":
                        wc.Region = "2-" + wc.Region;
                        break;
                    case "NE":
                        wc.Region = "3-" + wc.Region;
                        break;
                    case "NC":
                        wc.Region = "4-" + wc.Region;
                        break;
                    case "NW":
                        wc.Region = "5-" + wc.Region;
                        break;
                }

                //wc.GeoLocation = GetStructureLatLong(wc.StructureId);
                workConcepts.Add(wc);
                counter++;
            }

            return workConcepts;
        }

        public List<WorkConcept> GetFiipsWorkConcepts(int fiscalYear)
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select p.*, s.lat, s.lngt, s.latdecimal, s.lngtdecimal
                            from pmic p
                            left join structure s
                                on (p.extg_strc_id = s.structureid or p.plnd_strc_id = s.structureid)
                            where state_fiscal_year = @fiscalYear
                                and ((extg_strc_id is not null
                                        and extg_strc_id <> '') or (plnd_strc_id is not null
                                        and plnd_strc_id <> ''))
                                and strc_work_tycd is not null
                                and strc_work_tycd <> ''
                                and isduplicate = 0
                            order by extg_strc_id
                        ";
            //and estcp_ty_cd = 'LET'
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.FromFiips = true;
                wc.Status = StructuresProgramType.WorkConceptStatus.Fiips;
                wc.WorkConceptDbId = Convert.ToInt32(dr["fiipsid"]);
                wc.StructureId = dr["extg_strc_id"].ToString().Trim();
                wc.WorkConceptCode = dr["workactioncode"].ToString().Trim();
                wc.WorkConceptDescription = dr["strc_work_tydc"].ToString().Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.CertifiedWorkConceptDescription;

                if (wc.WorkConceptCode.Equals("07"))
                {
                    wc.WorkConceptDescription = "PAINT";
                }

                wc.PlannedStructureId = dr["plnd_strc_id"].ToString().Trim();

                if (wc.WorkConceptCode.Equals("01") && wc.StructureId.Equals("") && !wc.PlannedStructureId.Equals(""))
                {
                    wc.StructureId = wc.PlannedStructureId;
                }

                wc.Region = repo.GetRegionComboCode(dr["dot_rgn_cd"].ToString());
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = fiscalYear;
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                wc.Cost = Convert.ToInt32(dr["tot_wo_dlvy_amt"]);
                wc.FosProjectId = dr["fos_proj_id"].ToString().Trim();
                wc.FiipsDescription = dr["fndg_ctgy_desc"].ToString().Trim();
                //wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                wc.FromEligibilityList = false;
                wc.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString().Trim();
                //wc.GeoLocation = GetStructureGeoLocation(wc.StructureId);

                if (dr["lat"] != DBNull.Value)
                {
                    wc.GeoLocation.HsiLatitude = dr["lat"].ToString();
                }

                if (dr["lngt"] != DBNull.Value)
                {
                    wc.GeoLocation.HsiLongitude = dr["lngt"].ToString();
                }

                if (dr["latdecimal"] != DBNull.Value)
                {
                    wc.GeoLocation.LatitudeDecimal = Convert.ToSingle(dr["latdecimal"]);
                }

                if (dr["lngtdecimal"] != DBNull.Value)
                {
                    wc.GeoLocation.LongitudeDecimal = Convert.ToSingle(dr["lngtdecimal"]);
                }

                workConcepts.Add(wc);
                counter++;
            }

            return workConcepts;
        }

        public string GetLastPrecertificationOrCertification(int projectDbId, string action = "precertification")
        {
            string last = "Last: ";
            string lastStatus = "";
            string qry =
               @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and (userdbid is not null and userdbid <> 0)
                    order by useractiondatetime desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StructuresProgramType.ProjectUserAction userAction = (StructuresProgramType.ProjectUserAction)(dr["useraction"]);
                    string userFullName = dr["userfullname"].ToString();
                    DateTime userActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                    StructuresProgramType.ProjectStatus status = (StructuresProgramType.ProjectStatus)dr["status"];
                    string precertificationLiaison = "";
                    string certificationLiaison = "";

                    try
                    {
                        precertificationLiaison = dr["precertificationliaisonuserfullname"].ToString();
                    }
                    catch { }

                    try
                    {
                        certificationLiaison = dr["certificationliaisonuserfullname"].ToString();
                    }
                    catch { }

                    if (action.Equals("precertification"))
                    {
                        if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification
                            || userAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification
                            || userAction == StructuresProgramType.ProjectUserAction.BosRejectedPrecertification
                            || userAction == StructuresProgramType.ProjectUserAction.Precertification)
                        {
                            if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification)
                            {
                                if (status == StructuresProgramType.ProjectStatus.Precertified)
                                {
                                    lastStatus += String.Format("Auto-precertified on {0}", userActionDateTime);
                                }
                                else if (status == StructuresProgramType.ProjectStatus.Unapproved)
                                {
                                    lastStatus += String.Format("Auto-unapproved on {0}", userActionDateTime);
                                }
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification)
                            {
                                lastStatus += String.Format("Precertified by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedPrecertification)
                            {
                                lastStatus += String.Format("Rejected for precertification by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.Precertification)
                            {
                                lastStatus += String.Format("Precertification in progress by {0} ", precertificationLiaison);
                            }

                            last += lastStatus;
                            break;
                        }
                    }
                    else if (action.Equals("certification"))
                    {
                        if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification
                            || userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection
                            || userAction == StructuresProgramType.ProjectUserAction.Certification
                            || userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified
                            || userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification
                            || userAction == StructuresProgramType.ProjectUserAction.BosCertified
                            || userAction == StructuresProgramType.ProjectUserAction.RequestRecertification
                            || userAction == StructuresProgramType.ProjectUserAction.GrantRecertification
                            || userAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
                        {
                            if (userAction == StructuresProgramType.ProjectUserAction.Certification)
                            {
                                lastStatus += String.Format("Certification in progress by {0} ", certificationLiaison);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification)
                            {
                                lastStatus += String.Format("Submitted for certification approval by {0} on {1}", certificationLiaison, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection)
                            {
                                lastStatus += String.Format("Submitted for certification rejection by {0} on {1}", certificationLiaison, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                            {
                                lastStatus += String.Format("Transitionally certified by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification)
                            {
                                lastStatus += String.Format("Rejected for certification by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.BosCertified)
                            {
                                lastStatus += String.Format("Certified: Approved by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.RequestRecertification)
                            {
                                lastStatus += String.Format("Recertification requested by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.GrantRecertification)
                            {
                                lastStatus += String.Format("Recertification request granted by {0} on {1}", userFullName, userActionDateTime);
                            }
                            else if (userAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
                            {
                                lastStatus += String.Format("Recertification request rejected by {0} on {1}", userFullName, userActionDateTime);
                            }

                            last += lastStatus;
                            break;
                        }
                    }
                }

                if (String.IsNullOrEmpty(lastStatus))
                {
                    last += "None";
                }
            }
            else
            {
                last += "None";
            }

            return last;
        }

        public string GetPrecertificationLiaison(int projectDbId)
        {
            string precertificationLiaison = "";
            string qry =
                @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and precertificationliaisonuserfullname is not null    
                    order by useractiondatetime desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                precertificationLiaison = dr["precertificationliaisonuserfullname"].ToString();
            }
            else
            {
                qry =
                @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and (useraction = 110)
                    order by useractiondatetime desc
                ";
                prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = projectDbId;
                dt = ExecuteSelect(qry, prms, database.wisamsConnection);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    precertificationLiaison = dr["userfullname"].ToString();
                }
            }
            return precertificationLiaison;
        }

        public List<WorkConcept> GetPrimaryWorkConcepts()
        {
            List<WorkConcept> wcs = new List<WorkConcept>();
            string qry = @"
                            select *
                            from workaction
                            where improvement = 1
                                and active = 1
                            order by workactiondesc, workactioncode
                        ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptCode = dr["workactioncode"].ToString();
                wc.WorkConceptDescription = dr["workactiondesc"].ToString();

                if (dr["earlierfy"] != DBNull.Value)
                {
                    wc.EarlierFiscalYear = Convert.ToInt32(dr["earlierfy"]);
                }
                else
                {
                    wc.EarlierFiscalYear = -99;
                }

                if (dr["laterfy"] != DBNull.Value)
                {
                    wc.LaterFiscalYear = Convert.ToInt32(dr["laterfy"]);
                }
                else
                {
                    wc.LaterFiscalYear = -99;
                }

                if (dr["mapmarkertype"] != DBNull.Value)
                {
                    wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                }
                else
                {
                    wc.MapMarkerType = (GMarkerGoogleType)3;
                }

                wcs.Add(wc);
            }

            return wcs;
        }

        public string GetProjectHistory(int projectDbId)
        {
            string history = "";
            string qry =
                @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and (userdbid is not null and userdbid <> 0)
                    order by useractiondatetime desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                int counter = 0;
                int userDbId = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["userdbid"] != DBNull.Value)
                    {
                        userDbId = Convert.ToInt32(dr["userdbid"]);
                    }

                    if (userDbId != 0)
                    {
                        string userFullName = dr["userfullname"].ToString();
                        StructuresProgramType.ProjectUserAction userAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                        DateTime userActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                        string precertificationLiaison = "";
                        string certificationLiaison = "";
                        Project currentProject = new Project();
                        currentProject.UserAction = userAction;
                        currentProject.UserFullName = userFullName;
                        currentProject.UserActionDateTime = userActionDateTime;

                        if (dr["precertificationliaisonuserfullname"] != DBNull.Value)
                        {
                            precertificationLiaison = dr["precertificationliaisonuserfullname"].ToString();
                        }

                        currentProject.PrecertificationLiaisonUserFullName = precertificationLiaison;

                        if (dr["certificationliaisonuserfullname"] != DBNull.Value)
                        {
                            certificationLiaison = dr["certificationliaisonuserfullname"].ToString();
                        }

                        currentProject.CertificationLiaisonUserFullName = certificationLiaison;
                        string actionByUser = dataServ.GetWorkflowStatus(currentProject);

                        /*
                        if (userAction == StructuresProgramType.ProjectUserAction.CreateProject)
                        {
                            actionByUser = "Project's created";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.SavedProject)
                        {
                            actionByUser = "Project's saved";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification)
                        {
                            actionByUser = "Project's submitted for precertification";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification)
                        {
                            actionByUser = "Project's submitted for certification";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.Precertification)
                        {
                            actionByUser = String.Format("Project's in precertification and assigned to {0}", dr["precertificationliaisonuserfullname"].ToString());
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.Certification)
                        {
                            actionByUser = String.Format("Project's in certification and assigned to {0}", dr["certificationliaisonuserfullname"].ToString());
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification)
                        {
                            actionByUser = "Project's precertified";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedPrecertification)
                        {
                            actionByUser = "Project's been rejected for precertification";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification)
                        {
                            actionByUser = "Project's been rejected for certification";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.BosCertified)
                        {
                            actionByUser = "Project's been certified";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                        {
                            actionByUser = "Project's been transitionally certified";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment)
                        {
                            actionByUser = "Project's been unassigned for precertification";
                        }
                        else if (userAction == StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment)
                        {
                            actionByUser = "Project's been unassigned for certification";
                        }
                        else
                        {
                            actionByUser = userAction.ToString();
                        }*/

                        if (counter > 0)
                        {
                            history += String.Format("\r\n\r\n");
                        }

                        history += String.Format("{0}", actionByUser);
                        counter++;
                    }
                }
            }

            return history;
        }

        public Project GetProjectRecertification(int projectDbId)
        {
            Project project = null;
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        select *
                        from projecthistory
                        where projecthistorydbid =
                            (select max(projecthistorydbid)
                            from projecthistory
                            where useraction in (120, 130, 140)
                                and projectdbid = @projectDbId)
                    ";
                SqlParameter[] prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = projectDbId;
                DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    project = new Project();
                    project.RecertificationReason = Convert.ToString(dr["recertificationreason"]);
                    project.RecertificationComments = Convert.ToString(dr["recertificationcomments"]);
                    int userAction = Convert.ToInt32(dr["useraction"]);

                    if (userAction == 120)
                    {
                        project.UserAction = StructuresProgramType.ProjectUserAction.RequestRecertification;
                    }
                    else if (userAction == 130)
                    {
                        project.UserAction = StructuresProgramType.ProjectUserAction.GrantRecertification;
                    }
                    else if (userAction == 140)
                    {
                        project.UserAction = StructuresProgramType.ProjectUserAction.RejectRecertification;
                    }
                }
            }

            return project;
        }

        public List<Project> GetProjectsInFiips(int startFiscalYear, int endFiscalYear, List<WorkConcept> workConcepts, string region = "any")
        {
            List<Project> projects = new List<Project>();

            string qry =
                @"
                    select distinct fos_proj_id, state_fiscal_year, dot_rgn_cd, pproj_cncp_cd, 
                        lfcy_stg_cd, pproj_fos_cncp_txt, designprojectid, psedate, earliestpsedate,
                        earliestadvanceableletdate, estcp_schd_dt
                    from pmic
                    where state_fiscal_year >= @startFiscalYear
                        and state_fiscal_year <= @endFiscalYear
                        and ((extg_strc_id is not null and extg_strc_id <> '') or (plnd_strc_id is not null and plnd_strc_id <> ''))
                        and strc_work_tycd is not null
                        and strc_work_tycd <> ''
                        and pproj_fnty_desc = 'CONSTRUCTION'
                ";

            SqlParameter[] prms = new SqlParameter[2];

            if (!region.Equals("any"))
            {
                qry += " and dot_rgn_cd = @region";
                prms = new SqlParameter[3];
                prms[2] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[2].Value = region;
            }

            prms[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms[0].Value = startFiscalYear;
            prms[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms[1].Value = endFiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Project project = new Project();
                    project.ProjectDbId = 0;
                    project.FosProjectId = dr["fos_proj_id"].ToString().Trim();
                    project.FiscalYear = Convert.ToInt32(dr["state_fiscal_year"]);
                    project.Region = dataServ.GetRegionComboCode(dr["dot_rgn_cd"].ToString());
                    project.FiipsImprovementConcept = dr["pproj_cncp_cd"].ToString();
                    project.Status = StructuresProgramType.ProjectStatus.Fiips;
                    project.LifecycleStageCode = dr["lfcy_stg_cd"].ToString();
                    project.Description = dr["pproj_fos_cncp_txt"].ToString();
                    project.DesignId = dr["designprojectid"].ToString();

                    if (dr["estcp_schd_dt"] != DBNull.Value)
                    {
                        project.LetDate = Convert.ToDateTime(dr["estcp_schd_dt"]);
                    }

                    if (dr["psedate"] != DBNull.Value)
                    {
                        project.PseDate = Convert.ToDateTime(dr["psedate"]);
                    }

                    if (dr["earliestpsedate"] != DBNull.Value)
                    {
                        project.EpseDate = Convert.ToDateTime(dr["earliestpsedate"]);
                    }

                    if (dr["earliestadvanceableletdate"] != DBNull.Value)
                    {
                        project.EarliestAdvanceableLetDate = Convert.ToDateTime(dr["earliestadvanceableletdate"]);
                    }

                    project.WorkConcepts = workConcepts.Where(wc => wc.FosProjectId.Equals(project.FosProjectId)).ToList();
                    project.NumberOfStructures = project.WorkConcepts.GroupBy(s => s.StructureId).Select(g => g.First()).Count();

                    if (project.WorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Count() > 0)
                    {
                        project.GeoLocation.LatitudeDecimal = project.WorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Average(wc => wc.GeoLocation.LatitudeDecimal);
                    }

                    if (project.WorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Count() > 0)
                    {
                        project.GeoLocation.LongitudeDecimal = project.WorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Average(wc => wc.GeoLocation.LongitudeDecimal);
                    }


                    projects.Add(project);
                }
            }

            database.fiipsProjects = projects;
            return projects;
        }

        public List<Project> GetProjectsInSct(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<Project> projects = new List<Project>();
            List<WorkConcept> workConcepts = new List<WorkConcept>();

            // Grab transitionally certified projects

            /*
            string qry =
                @"
                    select prog.*, s.lat, s.lngt, s.latdecimal, s.lngtdecimal
                    from structureprogramreviewcurrent prog
                    left join structure s
                        on prog.structureid = s.structureid
                    where workactionyear >= @startFiscalYear
                        and workactionyear <= @endFiscalYear
                        and (iscertified = 1)
                        and fosprojectid is not null and fosprojectid <> ''
                ";
            SqlParameter[] prms = new SqlParameter[2];

            if (!region.Equals("any"))
            {
                qry += " and region = @region";
                prms = new SqlParameter[3];
                prms[2] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[2].Value = region;
            }

            qry += " order by fosprojectid";
            prms[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms[0].Value = startFiscalYear;
            prms[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms[1].Value = endFiscalYear; 
            
            
            DataTable dt = ExecuteSelect(qry, prms, wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                string currentFosProjectId = "";
                string previousFosProjectId = "";
                int projectDbIdCounter = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    currentFosProjectId = dr["fosprojectid"].ToString().Trim();
                    
                    if (!currentFosProjectId.Equals(previousFosProjectId))
                    {
                        projectDbIdCounter++;
                        Project project = new Project();
                        projects.Add(project);
                        project.ProjectDbId = projectDbIdCounter;
                        project.FosProjectId = currentFosProjectId;
                        project.StructuresConcept = "Structures";
                        project.Status = StructuresProgramType.ProjectStatus.QuasiCertified;
                        project.IsQuasicertified = true;
                        project.UserAction = StructuresProgramType.ProjectUserAction.BosTransitionallyCertified;
                        project.UserActionDateTime = new DateTime(2019, 2, 1);
                        project.FiscalYear = Convert.ToInt32(dr["workactionyear"]);
                        project.Region = GetRegionComboCode(dr["region"].ToString().Trim());
                        project.BoxId = "-1";
                        project.FromExcel = true;
                        project.History = "";
                        project.Notes = "";

                        try
                        {
                            project.AdvanceableFiscalYear = Convert.ToInt32(dr["advanceableworkactionyear"]);
                        }
                        catch { }

                        var rows = dt.AsEnumerable().Where(r => r["fosprojectid"].ToString().Equals(project.FosProjectId));
                        project.NumberOfStructures = rows.GroupBy(s => s["structureid"]).Select(g => g.First()).Count();

                        if (rows.Count() > 0)
                        {
                            var currentWorkConcepts = new List<WorkConcept>();

                            foreach (var row in rows)
                            {
                                WorkConcept wc = new WorkConcept();
                                workConcepts.Add(wc);
                                currentWorkConcepts.Add(wc);

                                if (row["lat"] != DBNull.Value)
                                {
                                    wc.GeoLocation.HsiLatitude = row["lat"].ToString();
                                }

                                if (row["lngt"] != DBNull.Value)
                                {
                                    wc.GeoLocation.HsiLongitude = row["lngt"].ToString();
                                }

                                if (row["latdecimal"] != DBNull.Value)
                                {
                                    wc.GeoLocation.LatitudeDecimal = Convert.ToSingle(row["latdecimal"]);
                                }

                                if (row["lngtdecimal"] != DBNull.Value)
                                {
                                    wc.GeoLocation.LongitudeDecimal = Convert.ToSingle(row["lngtdecimal"]);
                                }

                                wc.WorkConceptDbId = Convert.ToInt32(row["rowdbid"]);
                                wc.ProjectDbId = project.ProjectDbId;
                                wc.FosProjectId = project.FosProjectId;
                                wc.StructureId = row["structureid"].ToString().Trim();
                                wc.WorkConceptCode = row["workactioncode"].ToString().Trim();
                                wc.WorkConceptDescription = row["workactiondesc"].ToString().Trim();

                                if (wc.WorkConceptCode.Equals("07"))
                                {
                                    wc.WorkConceptDescription = "PAINT";
                                }
                                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                                wc.IsQuasicertified = true;
                                wc.Status = StructuresProgramType.WorkConceptStatus.Quasicertified;
                                wc.CertificationAdditionalComments = "Reviewed and certified (without BOSCD) as part of the transitional implementation of the BOS certification process.";
                                wc.Region = project.Region;
                                
                                wc.CurrentFiscalYear = currentFiscalYear;
                                wc.FiscalYear = project.FiscalYear;
                                wc.ProjectYear = currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                                wc.StructureProjectFiscalYear = project.FiscalYear;
                            }

                            project.WorkConcepts = currentWorkConcepts;
                            // Exclude structures with (0, 0)
                            try
                            {
                                project.GeoLocation.LatitudeDecimal = currentWorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Average(wc => wc.GeoLocation.LatitudeDecimal);
                            }
                            catch { }

                            try
                            {
                                project.GeoLocation.LongitudeDecimal = currentWorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Average(wc => wc.GeoLocation.LongitudeDecimal);
                            }
                            catch { }
                        }
                    }

                    previousFosProjectId = currentFosProjectId;
                }
            }*/


            // Grab regular active structures projects created in Sct and their most current work concepts
            string qry =
                @"
                    select p.*, ph.*, 
                        pwch.projectworkconcepthistorydbid, pwch.workconceptdbid, pwch.workconceptcode, pwch.workconceptdescription,
                        pwch.certifiedworkconceptcode, pwch.certifiedworkconceptdescription, pwch.structureid,
                        pwch.plannedstructureid, pwch.currentfiscalyear as wcCurrentFiscalYear, pwch.fiscalyear as wcFiscalYear, pwch.projectyear as wcProjectYear,
                        pwch.priorityscore, pwch.cost, pwch.fromeligibilitylist, pwch.fromfiips, pwch.evaluate, pwch.earlierfiscalyear, pwch.laterfiscalyear,
                        pwch.changejustifications, pwch.changenotes, pwch.status as wcstatus, pwch.precertificationdecision, pwch.precertificationdecisiondatetime,
                        pwch.precertificationdecisionreasoncategory, pwch.precertificationdecisionreasonexplanation, pwch.precertificationdecisioninternalcomments,
                        pwch.fromproposedlist, pwch.certificationdecision as wcCertificationDecision, pwch.certificationdatetime as wcCertificationDateTime, pwch.certificationprimaryworktypecomments,
                        pwch.certificationsecondaryworktypecomments, pwch.certificationadditionalcomments, pwch.estimatedconstructioncost,
                        pwch.estimateddesignlevelofeffort, pwch.designresourcing,
                        s.lat, s.lngt, s.latdecimal, s.lngtdecimal
                    from project p
                    left join projecthistory ph
                        on p.projectdbid = ph.projectdbid
                    left join projectworkconcepthistory pwch
                        on ph.projecthistorydbid = pwch.projecthistorydbid
                    left join structure s
                        on pwch.structureid = s.structureid
                    where ph.fiscalyear >= @startFiscalYear
                        and ph.fiscalyear <= @endFiscalYear
                        and p.projectdbid = ph.projectdbid
                        and p.deletedate is null
                        and p.active = 1
                        and ph.projecthistorydbid = 
                            (select max(projecthistorydbid)
                                from projecthistory
                                where projectdbid = p.projectdbid
                                    and (ph.userdbid is not null and ph.userdbid <> 0)
                            )  
                        
                ";

            SqlParameter[] prms = new SqlParameter[2];

            if (!region.Equals("any"))
            {
                qry += " and region = @region";
                prms = new SqlParameter[3];
                prms[2] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[2].Value = repo.GetRegionComboCode(region);
            }

            qry += " order by p.projectdbid, ph.projecthistorydbid, pwch.structureid";
            prms[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms[0].Value = startFiscalYear;
            prms[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms[1].Value = endFiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                int currentProjectDbId = 0;
                int previousProjectDbId = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    currentProjectDbId = Convert.ToInt32(dr["projectdbid"]);

                    if (currentProjectDbId != previousProjectDbId)
                    {
                        Project project = new Project();
                        projects.Add(project);
                        project.ProjectDbId = Convert.ToInt32(dr["projectdbid"]);
                        project.ProjectHistoryDbId = Convert.ToInt32(dr["projecthistorydbid"]);
                        project.NotificationRecipients = dr["notificationrecipients"].ToString();

                        if (dr["fosprojectid"] != DBNull.Value)
                        {
                            project.FosProjectId = dr["fosprojectid"].ToString().Trim();
                        }
                        else
                        {
                            project.FosProjectId = "";
                        }

                        if (dr["structurescost"] != DBNull.Value)
                        {
                            project.StructuresCost = Convert.ToInt32(dr["structurescost"]);
                        }

                        project.Status = (StructuresProgramType.ProjectStatus)dr["status"];

                        if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                        {
                            project.IsQuasicertified = true;
                        }
                        else
                        {
                            project.IsQuasicertified = false;
                        }

                        if (dr["structuresconcept"] != DBNull.Value)
                        {
                            project.StructuresConcept = dr["structuresconcept"].ToString();
                        }
                        else
                        {
                            project.StructuresConcept = "";
                        }

                        if (dr["numberofstructures"] != DBNull.Value)
                        {
                            project.NumberOfStructures = Convert.ToInt32(dr["numberofstructures"]);
                        }

                        project.Description = dr["description"].ToString();
                        project.Notes = dr["notes"].ToString();
                        //project.Region = GetRegionComboCode(dr["region"].ToString().Trim());
                        project.Region = dr["region"].ToString().Trim();

                        if (dr["boxid"] != DBNull.Value)
                        {
                            project.BoxId = dr["boxid"].ToString();
                        }
                        else
                        {
                            project.BoxId = "-1";
                        }

                        project.CurrentFiscalYear = Convert.ToInt32(dr["currentfiscalyear"]);
                        project.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                        project.ProjectYear = Convert.ToInt32(dr["projectyear"]);

                        if (dr["advanceablefiscalyear"] != DBNull.Value)
                        {
                            project.AdvanceableFiscalYear = Convert.ToInt32(dr["advanceablefiscalyear"]);
                        }

                        // Latest project status and transaction
                        project.Status = (StructuresProgramType.ProjectStatus)dr["status"];
                        project.UserDbId = Convert.ToInt32(dr["userdbid"]);
                        project.UserAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                        project.UserActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);

                        if (dr["userfullname"] != DBNull.Value)
                        {
                            project.UserFullName = dr["userfullname"].ToString().Trim();
                        }
                        else
                        {
                            project.UserFullName = "";
                        }

                        project.UserDbIds = GetUserIdsForAProject(project.ProjectDbId);

                        if (dr["locked"] != DBNull.Value)
                        {
                            project.Locked = Convert.ToBoolean(dr["locked"]);
                        }
                        else
                        {
                            project.Locked = false;
                        }

                        if (dr["submitdate"] != DBNull.Value)
                        {
                            project.SubmitDate = Convert.ToDateTime(dr["submitdate"]);
                        }

                        if (dr["precertifydate"] != DBNull.Value)
                        {
                            project.PrecertifyDate = Convert.ToDateTime(dr["precertifydate"]);
                        }

                        if (dr["precertificationliaisonuserdbid"] != DBNull.Value)
                        {
                            project.PrecertificationLiaisonUserDbId = Convert.ToInt32(dr["precertificationliaisonuserdbid"]);
                        }

                        if (dr["precertificationliaisonuserfullname"] != DBNull.Value)
                        {
                            project.PrecertificationLiaisonUserFullName = dr["precertificationliaisonuserfullname"].ToString();
                        }

                        if (dr["certifydate"] != DBNull.Value)
                        {
                            project.CertifyDate = Convert.ToDateTime(dr["certifydate"]);
                        }

                        if (dr["inprecertification"] != DBNull.Value)
                        {
                            project.InPrecertification = Convert.ToBoolean(dr["inprecertification"]);
                        }
                        else
                        {
                            project.InPrecertification = false;
                        }

                        if (dr["incertification"] != DBNull.Value)
                        {
                            project.InCertification = Convert.ToBoolean(dr["incertification"]);
                        }
                        else
                        {
                            project.InCertification = false;
                        }

                        if (dr["certificationliaisonuserfullname"] != DBNull.Value)
                        {
                            project.CertificationLiaisonUserFullName = dr["certificationliaisonuserfullname"].ToString();
                        }

                        if (dr["certificationliaisonuserdbid"] != DBNull.Value)
                        {
                            project.CertificationLiaisonUserDbId = Convert.ToInt32(dr["certificationliaisonuserdbid"]);
                            UserAccount liaison = GetUserAccount(project.CertificationLiaisonUserDbId);
                            project.CertificationLiaisonEmail = liaison.EmailAddress;
                            project.CertificationLiaisonPhone = liaison.PhoneNumber;
                        }

                        if (dr["recertificationreason"] != DBNull.Value)
                        {
                            project.RecertificationReason = dr["recertificationreason"].ToString();
                        }

                        if (dr["recertificationcomments"] != DBNull.Value)
                        {
                            project.RecertificationComments = dr["recertificationcomments"].ToString();
                        }

                        if (dr["acceptablepsedatestart"] != DBNull.Value)
                        {
                            project.AcceptablePseDateStart = Convert.ToDateTime(dr["acceptablepsedatestart"]);
                        }
                        else
                        {
                            project.AcceptablePseDateStart = project.AdvanceableFiscalYear != 0 ? dataServ.CalculateAcceptablePseDateStart(project.AdvanceableFiscalYear) : dataServ.CalculateAcceptablePseDateStart(project.FiscalYear);
                        }

                        if (dr["acceptablepsedateend"] != DBNull.Value)
                        {
                            project.AcceptablePseDateEnd = Convert.ToDateTime(dr["acceptablepsedateend"]);
                        }
                        else
                        {
                            project.AcceptablePseDateEnd = dataServ.CalculateAcceptablePseDateEnd(project.FiscalYear);
                        }

                        project.History = GetProjectHistory(project.ProjectDbId);

                        // Get project work concepts
                        var rows = dt.AsEnumerable().Where(r => Convert.ToInt32(r["projectdbid"]) == currentProjectDbId
                                                            && !String.IsNullOrEmpty(r["structureid"].ToString()));
                        project.NumberOfStructures = rows.Count();

                        if (rows.Count() > 0)
                        {
                            var currentWorkConcepts = new List<WorkConcept>();

                            foreach (var row in rows)
                            {

                                WorkConcept wc = new WorkConcept();
                                workConcepts.Add(wc);
                                currentWorkConcepts.Add(wc);
                                wc.ProjectDbId = currentProjectDbId;
                                wc.StructureProjectFiscalYear = project.FiscalYear;
                                wc.Region = project.Region;
                                wc.StructuresConcept = project.StructuresConcept;
                                wc.WorkConceptDbId = Convert.ToInt32(row["workconceptdbid"]);
                                wc.ProjectWorkConceptHistoryDbId = Convert.ToInt32(row["projectworkconcepthistorydbid"]);
                                wc.WorkConceptCode = row["workconceptcode"].ToString().Trim();
                                wc.WorkConceptDescription = row["workconceptdescription"].ToString().Trim();
                                wc.CertifiedWorkConceptCode = row["certifiedworkconceptcode"].ToString().Trim();
                                wc.CertifiedWorkConceptDescription = row["certifiedworkconceptdescription"].ToString().Trim();
                                wc.StructureId = row["structureid"].ToString();
                                wc.PlannedStructureId = row["plannedstructureid"].ToString();
                                wc.CurrentFiscalYear = Convert.ToInt32(row["currentfiscalyear"]);
                                wc.FiscalYear = Convert.ToInt32(row["wcfiscalyear"]);
                                wc.ProjectYear = Convert.ToInt32(row["projectyear"]);

                                if (row["priorityscore"] != DBNull.Value)
                                {
                                    wc.PriorityScore = Convert.ToSingle(row["priorityscore"]);
                                }

                                if (row["cost"] != DBNull.Value)
                                {
                                    wc.Cost = Convert.ToInt32(row["cost"]);
                                }

                                wc.FromEligibilityList = Convert.ToBoolean(row["fromeligibilitylist"]);
                                wc.FromFiips = Convert.ToBoolean(row["fromfiips"]);
                                wc.Evaluate = Convert.ToBoolean(row["evaluate"]);

                                if (row["fromproposedlist"] != DBNull.Value)
                                {
                                    wc.FromProposedList = Convert.ToBoolean(row["fromproposedlist"]);
                                }
                                else
                                {
                                    wc.FromProposedList = false;
                                }

                                if (row["earlierfiscalyear"] != DBNull.Value && Convert.ToInt32(row["earlierfiscalyear"]) != 0)
                                {
                                    wc.EarlierFiscalYear = Convert.ToInt32(row["earlierfiscalyear"]);
                                }
                                else
                                {
                                    if (!wc.FromProposedList)
                                    {
                                        if (database.allWorkConcepts.Where(w => w.WorkConceptCode.Equals(wc.WorkConceptCode)).Count() > 0)
                                        {
                                            wc.EarlierFiscalYear = database.allWorkConcepts.Where(w => w.WorkConceptCode.Equals(wc.WorkConceptCode)).First().EarlierFiscalYear;
                                        }
                                    }
                                }

                                if (row["laterfiscalyear"] != DBNull.Value && Convert.ToInt32(row["laterfiscalyear"]) != 0)
                                {
                                    wc.LaterFiscalYear = Convert.ToInt32(row["laterfiscalyear"]);
                                }
                                else
                                {
                                    if (!wc.FromProposedList)
                                    {
                                        if (database.allWorkConcepts.Where(w => w.WorkConceptCode.Equals(wc.WorkConceptCode)).Count() > 0)
                                        {
                                            wc.LaterFiscalYear = database.allWorkConcepts.Where(w => w.WorkConceptCode.Equals(wc.WorkConceptCode)).First().LaterFiscalYear;
                                        }

                                    }
                                }

                                if (row["changejustifications"] != DBNull.Value && !String.IsNullOrEmpty(row["changejustifications"].ToString()))
                                {
                                    wc.ChangeJustifications = row["changejustifications"].ToString();
                                }
                                else if (wc.FromProposedList)
                                {
                                    wc.ChangeJustifications = GetProposedWorkReasonCategory(wc.StructureId);
                                }

                                if (row["changenotes"] != DBNull.Value && !String.IsNullOrEmpty(row["changenotes"].ToString()))
                                {
                                    wc.ChangeJustificationNotes = row["changenotes"].ToString();
                                }
                                else if (wc.FromProposedList)
                                {
                                    wc.ChangeJustificationNotes = GetProposedWorkNotes(wc.StructureId);
                                }

                                if (row["quasicertified"] != DBNull.Value)
                                {
                                    wc.IsQuasicertified = Convert.ToBoolean(row["quasicertified"]);
                                }
                                else
                                {
                                    wc.IsQuasicertified = false;
                                }

                                var status = row["wcstatus"];
                                wc.Status = (StructuresProgramType.WorkConceptStatus)row["wcstatus"];

                                if (row["precertificationdecision"] != DBNull.Value)
                                {
                                    wc.PrecertificationDecision =
                                        (StructuresProgramType.PrecertificatioReviewDecision)row["precertificationdecision"];
                                }

                                if (row["precertificationdecisiondatetime"] != DBNull.Value)
                                {
                                    wc.PrecertificationDecisionDateTime = Convert.ToDateTime(row["precertificationdecisiondatetime"]);
                                }

                                if (row["precertificationdecisionreasoncategory"] != DBNull.Value)
                                {
                                    wc.PrecertificationDecisionReasonCategory = row["precertificationdecisionreasoncategory"].ToString();
                                }
                                else
                                {
                                    wc.PrecertificationDecisionReasonCategory = "";
                                }

                                if (row["precertificationdecisionreasonexplanation"] != DBNull.Value)
                                {
                                    wc.PrecertificationDecisionReasonExplanation = row["precertificationdecisionreasonexplanation"].ToString();
                                }
                                else
                                {
                                    wc.PrecertificationDecisionReasonExplanation = "";
                                }

                                if (row["precertificationdecisioninternalcomments"] != DBNull.Value)
                                {
                                    wc.PrecertificationDecisionInternalComments = row["precertificationdecisioninternalcomments"].ToString();
                                }
                                else
                                {
                                    wc.PrecertificationDecisionInternalComments = "";
                                }

                                if (row["wccertificationdecision"] != DBNull.Value)
                                {
                                    wc.CertificationDecision = row["wccertificationdecision"].ToString();
                                }
                                else
                                {
                                    wc.CertificationDecision = "";
                                }

                                if (row["wccertificationdatetime"] != DBNull.Value)
                                {
                                    wc.CertificationDateTime = Convert.ToDateTime(row["wccertificationdatetime"]);
                                }

                                if (wc.CertificationDateTime.Year != 1)
                                {
                                    string qry6 =
                                       @"
                                        select p.*, w.workactiondesc
                                        from projectelementworkconcept p left join workaction w
                                            on p.workconceptcode = w.workactioncode
                                        where p.projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                                            and p.certificationdatetime = @certificationDateTime
                                    ";

                                    SqlParameter[] prms6 = new SqlParameter[2];
                                    prms6[0] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
                                    prms6[0].Value = wc.ProjectWorkConceptHistoryDbId;
                                    prms6[1] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
                                    prms6[1].Value = wc.CertificationDateTime;
                                    DataTable dt6 = ExecuteSelect(qry6, prms6, database.wisamsConnection);

                                    if (dt6 != null && dt6.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr6 in dt6.Rows)
                                        {
                                            ElementWorkConcept ewc = new ElementWorkConcept();
                                            ewc.StructureId = wc.StructureId;
                                            ewc.ElementWorkConceptDbId = Convert.ToInt32(dr6["elementworkconceptdbid"]);
                                            ewc.ProjectWorkConceptHistoryDbId = wc.ProjectWorkConceptHistoryDbId;

                                            if (dr6["certificationdatetime"] != DBNull.Value)
                                            {
                                                ewc.CertificationDateTime = Convert.ToDateTime(dr6["certificationdatetime"]);
                                            }

                                            ewc.ElementNumber = Convert.ToInt32(dr6["elementnumber"]);
                                            ewc.WorkConceptCode = dr6["workconceptcode"].ToString();
                                            ewc.WorkConceptDescription = dr6["workactiondesc"].ToString();
                                            ewc.WorkConceptLevel = dr6["workconceptlevel"].ToString();

                                            if (dr6["comments"] != DBNull.Value)
                                            {
                                                ewc.Comments = dr6["comments"].ToString();
                                            }

                                            project.CertifiedElementWorkConceptCombinations.Add(ewc);
                                        }
                                    }
                                }

                                if (row["certificationprimaryworktypecomments"] != DBNull.Value)
                                {
                                    wc.CertificationPrimaryWorkTypeComments = row["certificationprimaryworktypecomments"].ToString();
                                }
                                else
                                {
                                    wc.CertificationPrimaryWorkTypeComments = "";
                                }

                                if (row["certificationsecondaryworktypecomments"] != DBNull.Value)
                                {
                                    wc.CertificationSecondaryWorkTypeComments = row["certificationsecondaryworktypecomments"].ToString();
                                }
                                else
                                {
                                    wc.CertificationSecondaryWorkTypeComments = "";
                                }

                                if (row["certificationadditionalcomments"] != DBNull.Value)
                                {
                                    wc.CertificationAdditionalComments = row["certificationadditionalcomments"].ToString();
                                }
                                else
                                {
                                    wc.CertificationAdditionalComments = "";
                                }

                                if (row["estimatedconstructioncost"] != DBNull.Value)
                                {
                                    wc.EstimatedConstructionCost = Convert.ToInt32(row["estimatedconstructioncost"]);
                                }

                                if (row["estimateddesignlevelofeffort"] != DBNull.Value)
                                {
                                    wc.EstimatedDesignLevelOfEffort = Convert.ToInt32(row["estimateddesignlevelofeffort"]);
                                }

                                if (row["designresourcing"] != DBNull.Value)
                                {
                                    wc.DesignResourcing = row["designresourcing"].ToString();
                                }

                                if (row["lat"] != DBNull.Value)
                                {
                                    wc.GeoLocation.HsiLatitude = row["lat"].ToString();
                                }

                                if (row["lngt"] != DBNull.Value)
                                {
                                    wc.GeoLocation.HsiLongitude = row["lngt"].ToString();
                                }

                                if (row["latdecimal"] != DBNull.Value)
                                {
                                    wc.GeoLocation.LatitudeDecimal = Convert.ToSingle(row["latdecimal"]);
                                }

                                if (row["lngtdecimal"] != DBNull.Value)
                                {
                                    wc.GeoLocation.LongitudeDecimal = Convert.ToSingle(row["lngtdecimal"]);
                                }
                            }

                            project.WorkConcepts = currentWorkConcepts;

                            if (currentWorkConcepts.Any(cw => cw.Status == StructuresProgramType.WorkConceptStatus.Unapproved))
                            {
                                project.Status = StructuresProgramType.ProjectStatus.Unapproved;
                            }

                            // Exclude structures with (0, 0)
                            if (currentWorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Count() > 0)
                            {
                                project.GeoLocation.LatitudeDecimal = currentWorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Average(wc => wc.GeoLocation.LatitudeDecimal);
                            }

                            if (currentWorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Count() > 0)
                            {
                                project.GeoLocation.LongitudeDecimal = currentWorkConcepts.Where(wc => wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0).Average(wc => wc.GeoLocation.LongitudeDecimal);
                            }
                        }
                    }

                    previousProjectDbId = currentProjectDbId;
                }
            }

            database.structureProjects = projects;
            return projects;
        }

        public List<StructuresProgramType.ProjectUserAction> GetProjectUserActionHistory(int projectDbId)
        {
            List<StructuresProgramType.ProjectUserAction> projectUserActionHistory = new List<StructuresProgramType.ProjectUserAction>();
            string qry =
                @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and (userdbid is not null and userdbid <> 0)
                    order by useractiondatetime desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                int userDbId = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        userDbId = Convert.ToInt32(dr["userdbid"]);
                    }
                    catch { }

                    if (userDbId != 0)
                    {
                        StructuresProgramType.ProjectUserAction userAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                        projectUserActionHistory.Add(userAction);
                    }
                }
            }

            return projectUserActionHistory;
        }

        public List<WorkConcept> GetProposedWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry =
                @"
                    select pl.*, w.mapmarkertype
                    from proposedlist pl
                    left join workaction w
                        on pl.workconceptcode = w.workactioncode
                    where fiscalyear >= @startFiscalYear
                        and fiscalyear <= @endFiscalYear
                        and pl.active = 1
                ";
            SqlParameter[] prms = new SqlParameter[2];

            if (!region.Equals("any"))
            {
                prms = null;
                prms = new SqlParameter[3];
                qry +=
                    @"
                        and regionnumber = @regionNumber
                    ";
            }

            qry += @"
                        order by structureid
                    ";

            prms[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms[0].Value = startFiscalYear;
            prms[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms[1].Value = endFiscalYear;

            if (!region.Equals("any"))
            {
                prms[2] = new SqlParameter("@regionNumber", SqlDbType.VarChar);
                prms[2].Value = region.Substring(0, 1);
            }

            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                int workConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                string structureId = dr["structureid"].ToString().Trim();
                string reg = dr["region"].ToString().Trim();
                string regionNumber = dr["regionnumber"].ToString().Trim();
                string proposedWorkConceptCode = dr["workconceptcode"].ToString().Trim();
                string proposedWorkConceptDescription = dr["workconceptdesc"].ToString().Trim();
                int fiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                string reasonCategory = dr["reasoncategory"].ToString().Trim();
                string notes = dr["notes"].ToString().Trim();
                int proposedByUserDbId = Convert.ToInt32(dr["proposedbyuserdbid"]);
                string proposedByUserFullName = dr["proposedbyuserfullname"].ToString().Trim();
                DateTime proposedDate = Convert.ToDateTime(dr["proposeddate"]);
                bool active = Convert.ToBoolean(dr["active"]);
                WorkConcept wc = new WorkConcept(workConceptDbId, structureId, reg, regionNumber, proposedWorkConceptCode,
                                                    proposedWorkConceptDescription, fiscalYear, reasonCategory,
                                                    notes, proposedByUserDbId, proposedByUserFullName, proposedDate,
                                                    active);
                //wc.GeoLocation = GetStructureGeoLocation(structureId);
                wc.GeoLocation = GetStructureLatLong(structureId);
                workConcepts.Add(wc);
            }

            return workConcepts;
        }

        public string GetProposedWorkReasonCategory(string structureId)
        {
            string reasonCategory = "";
            string qry =
                @"
                    select reasoncategory
                    from proposedlist
                    where structureid = @structureId
                    order by proposeddate desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                if (dr["reasoncategory"] != DBNull.Value && !String.IsNullOrEmpty(dr["reasoncategory"].ToString()))
                {
                    reasonCategory = dr["reasoncategory"].ToString();
                    switch (reasonCategory.ToLower())
                    {
                        case "structural":
                            reasonCategory = "(00) Structural";
                            break;
                        case "proximitytootherwork":
                            reasonCategory = "(01) ProximityToOtherWork";
                            break;
                        case "laneclosurerestriction":
                            reasonCategory = "(02) LaneClosureRestriction";
                            break;
                        case "expansiondevelopment":
                            reasonCategory = "(03) ExpansionDevelopment";
                            break;
                        case "lccasecondaryworkconcepts":
                            reasonCategory = "(04) LccaSecondaryWorkConcepts";
                            break;
                        case "lccasharedtrafficcontrolcosts":
                            reasonCategory = "(05) LccaSharedTrafficControlCosts";
                            break;
                        case "lccasharedmobilizationcosts":
                            reasonCategory = "(06) LccaSharedMobilizationCosts";
                            break;
                        case "lccaother":
                            reasonCategory = "(07) LccaOther";
                            break;
                        case "other":
                            reasonCategory = "(50) Other";
                            break;
                    }
                }
            }

            return reasonCategory;
        }

        public List<WorkConcept> GetQuasicertifiedWorkConcepts(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();

            for (int i = startFiscalYear; i <= endFiscalYear; i++)
            {
                if (region.Equals("any"))
                {
                    workConcepts.AddRange(GetQuasicertifiedWorkConcepts(i));
                }
                else
                {
                    workConcepts.AddRange(GetQuasicertifiedWorkConcepts(i, region));
                }
            }

            return workConcepts;
        }

        public List<WorkConcept> GetQuasicertifiedWorkConcepts(int fiscalYear)
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select s.*, w.mapmarkertype
                            from structureprogramreviewcurrent s, workaction w
                            where s.workactionyear = @fiscalYear
                                and s.iscertified = 1
                                and s.workactioncode = w.workactioncode
                        ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptDbId = Convert.ToInt32(dr["rowdbid"]);
                wc.StructureId = dr["structureid"].ToString().Trim();
                wc.WorkConceptCode = dr["workactioncode"].ToString().Trim();
                wc.WorkConceptDescription = dr["workactiondesc"].ToString().Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                wc.Region = dr["regionnumber"].ToString().Trim() + "-" + dr["region"].ToString().Trim();
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = fiscalYear;
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                //wc.Cost = Convert.ToInt32(dr["primarycost"]); NO Cost
                //wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                wc.FromEligibilityList = false;
                wc.IsQuasicertified = true;
                wc.IsScopeAMatch = Convert.ToBoolean(dr["isscopeamatch"]);
                wc.IsYearAMatch = Convert.ToBoolean(dr["isyearamatch"]);
                wc.FosProjectId = dr["fosprojectid"].ToString().Trim();
                //wc.GeoLocation = GetStructureGeoLocation(wc.StructureId);
                wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                wc.Status = StructuresProgramType.WorkConceptStatus.Quasicertified;
                workConcepts.Add(wc);
                counter++;
            }

            return workConcepts;
        }

        public List<WorkConcept> GetQuasicertifiedWorkConcepts(int fiscalYear, string region)
        {
            List<WorkConcept> workConcepts = new List<WorkConcept>();
            string qry = @"
                            select *
                            from structureprogramreviewcurrent
                            where workactionyear = @fiscalYear
                                and regionnumber = @region
                                and iscertified = 1
                        ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
            prms[1].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            int counter = 0;

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptDbId = Convert.ToInt32(dr["rowdbid"]);
                wc.StructureId = dr["structureid"].ToString().Trim();
                wc.WorkConceptCode = dr["workactioncode"].ToString().Trim();
                wc.WorkConceptDescription = dr["workactiondesc"].ToString().Trim();
                wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                wc.Region = dr["regionnumber"].ToString().Trim() + "-" + dr["region"].ToString().Trim();
                wc.CurrentFiscalYear = database.currentFiscalYear;
                wc.FiscalYear = fiscalYear;
                wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                //wc.Cost = Convert.ToInt32(dr["primarycost"]); NO Cost
                //wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                wc.FromEligibilityList = false;
                wc.IsQuasicertified = true;
                wc.IsScopeAMatch = Convert.ToBoolean(dr["isscopeamatch"]);
                wc.IsYearAMatch = Convert.ToBoolean(dr["isyearamatch"]);
                wc.FosProjectId = dr["fosprojectid"].ToString().Trim();
                //wc.GeoLocation = GetStructureGeoLocation(wc.StructureId);
                wc.Status = StructuresProgramType.WorkConceptStatus.Quasicertified;
                workConcepts.Add(wc);
                counter++;
            }

            return workConcepts;
        }

        public string GetRegionNotes(WorkConcept currentWorkConcept)
        {
            string regionNotes = "";
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        select ph.projectdbid, ph.projecthistorydbid, pwch.changejustifications, pwch.changenotes
                        from projecthistory ph, projectworkconcepthistory pwch
                        where changenotes is not null
                            and changenotes <> ''
                            and ph.projecthistorydbid = pwch.projecthistorydbid
                            and pwch.structureid = @structureId
                            and ph.projectdbid = @projectDbId
                        order by ph.projecthistorydbid desc
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = currentWorkConcept.ProjectDbId;
                prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[1].Value = currentWorkConcept.StructureId;
                DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    regionNotes = dr["changenotes"].ToString();
                }
            }

            return regionNotes;
        }

        public List<WorkConcept> GetSecondaryWorkConcepts()
        {
            List<WorkConcept> wcs = new List<WorkConcept>();
            string qry = @"
                            select *
                            from workaction
                            where improvement = 0
                                and active = 1
                                and workactioncode not in (101,201,202,203,204,205)
                            order by workactiondesc, workactioncode
                        ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                WorkConcept wc = new WorkConcept();
                wc.WorkConceptCode = dr["workactioncode"].ToString();
                wc.WorkConceptDescription = dr["workactiondesc"].ToString();

                if (dr["earlierfy"] != DBNull.Value)
                {
                    wc.EarlierFiscalYear = Convert.ToInt32(dr["earlierfy"]);
                }
                else
                {
                    wc.EarlierFiscalYear = -99;
                }

                if (dr["laterfy"] != DBNull.Value)
                {
                    wc.LaterFiscalYear = Convert.ToInt32(dr["laterfy"]);
                }
                else
                {
                    wc.LaterFiscalYear = -99;
                }

                if (dr["mapmarkertype"] != DBNull.Value)
                {
                    wc.MapMarkerType = (GMarkerGoogleType)Convert.ToInt32(dr["mapmarkertype"]);
                }
                else
                {
                    wc.MapMarkerType = (GMarkerGoogleType)3;
                }

                wcs.Add(wc);
            }

            return wcs;
        }

        public Structure GetSptStructure(string structureId)
        {
            Structure structure = new Structure();
            structure.StructureId = structureId;

            string qry =
                @"
                    select struc.strc_id, 
                                        struc.strc_tycd,
                                        struc.strc_plnd_pgm,
                                        struc.stmc_agcy_ty,
                                        struc.stim_agcy_tycd,
                                        struc.dot_rgn_nb as regionNumber,
                                        rgn.dot_rgn_cd as region,
                                        struc.strc_prmy_cnty_cd as countyNumber,
                                        county.wi_cnty_nm as county,
                                        struc.strc_prmy_muni_cd as municipalityNumber,
                                        municipality.cmty_nm as municipality,
                                        struc.strc_ownr_agcy_cd as ownerNumber,
                                        routeon.featureOn,
                                        routeunder.featureUnder
             
                    from dot1stro.dt_strc struc, 
                            dotsys.dt_dot_rgn rgn,
                            dotsys.dt_wi_cnty_pitc county,
                            dotsys.dt_cmty_pitc municipality,
                            (
                                select strc_id, strc_rte_sys as rsOn, ftr_nm as featureOn
                                from dot1stro.dt_strc_rte
                                where strc_rte_ouf = 'O'
                                    and strc_rte_prmy_fl = 'Y'
                            ) routeon,
                            (
                                select strc_id, strc_rte_sys as rsUnder, ftr_nm as featureUnder
                                from dot1stro.dt_strc_rte
                                where strc_rte_ouf = 'U'
                                    and strc_rte_prmy_fl = 'Y'
                            ) routeunder

                    where struc.strc_id = :strId
                        and struc.strc_sscd in ('50', '70')
                        and struc.dot_rgn_nb = rgn.dot_rgn_nb(+)
                        and struc.strc_prmy_cnty_cd = county.dot_cnty_cd(+)
                        and ltrim(municipality.hwy_cmty_cd, '0') = to_char(struc.strc_prmy_muni_cd)
                        and struc.strc_id = routeon.strc_id(+)
                        and struc.strc_id = routeunder.strc_id(+)
                ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                structure.Region = dt.Rows[0]["REGION"].ToString();
                structure.RegionNumber = dt.Rows[0]["REGIONNUMBER"].ToString();
                structure.County = dt.Rows[0]["COUNTY"].ToString();
                structure.Municipality = dt.Rows[0]["MUNICIPALITY"].ToString();
                structure.FeatureOn = dt.Rows[0]["FEATUREON"].ToString();
                structure.FeatureUnder = dt.Rows[0]["FEATUREUNDER"].ToString();
            }

            return structure;
        }

        public List<string> GetStateStructuresByRegionForGisDataPull(string region)
        {
            List<string> strIds = new List<string>();
            string qry =
                @"
                    select strc_id
                    from dot1stro.dt_strc
                    where dot_rgn_nb = :region
                        and strc_sscd in ('10', '50')
                        and (strc_ownr_agcy_cd in ('10','15','16','20','44','45') 
                                or stmc_agcy_ty in ('10','15','16','20','44','45') 
                                or stim_agcy_tycd in ('10','15','16','20','44','45'))
                    order by strc_id
                ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("region", OracleDbType.Varchar2);
            prms[0].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strIds.Add(dr["STRC_ID"].ToString());
                }
            }

            return strIds;
        }

        public WiSamEntities.Structure GetStructure(string strId, bool includeClosedBridges = false, bool interpolateNbi = false, bool includeCoreInspections = false, bool countTpo = false, int startYear = 0, int endYear = 0)
        {
            WiSamEntities.Structure str = null;
            string qry = @"
                                select struc.strc_id, 
                                        struc.strc_tycd,
                                        struc.strc_plnd_pgm,
                                        struc.stmc_agcy_ty,
                                        struc.stim_agcy_tycd,
                                        struc.lat,
                                        struc.lngt,
                                        dt_strc_ty.strc_tycd_desc,
                                        bridge.brdg_ovrb_dpth,
                                        bridge.brdg_max_veh_wt,
                                        bridge.bgar_load_cpty_cd,
                                        bridge.brdg_sfcy_rtg,
                                        bridge.brdg_scur_crtc,
                                        bridge.bgar_cond_cd,
                                        bridge.bgar_gmty_on_cd,
                                        bridge.bgar_aprc_alnm_cd,
                                        bridge.bgar_gmty_undr_cd,
                                        bridge.brdg_inv_ldrtg,
                                        bridge.brdg_oprt_ldrtg,
                                        bridge.bgdk_area,
                                        bridge.brdg_lane_cnt_on,
                                        bridge.brdg_dtr_ln_on,
                                        bridge.brdg_sangl,
                                        bridge.brdg_hcrv_on,
                                        bridge.func_cls_undr_cd,
                                        bridge.func_cls_on_cd,
                                        bridge.trfc_ptrn_on,
                                        bridge.trfc_ptrn_undr,
                                        funcClassOn.brdg_func_cls_desc as fcOn,
                                        funcClassUnder.brdg_func_cls_desc as fcUnder,
                                        struc.dot_rgn_nb as regionNumber,
                                        rgn.dot_rgn_cd as region,
                                        struc.strc_prmy_cnty_cd as countyNumber,
                                        county.wi_cnty_nm as county,
                                        struc.strc_prmy_muni_cd as municipalityNumber,
                                        municipality.cmty_nm as municipality,
                                        struc.strc_ownr_agcy_cd as ownerNumber,
                                        agency.agcy_tydc as owner,
                                        routeon.featureOn,
                                        routeunder.featureUnder,
                                        routeon.rsOn,
                                        routeunder.rsUnder,
                                        loadposting.brdg_load_pstg_cd,
                                        loadposting.load_pstg_desc,
                                        loadposting.load_pstg_tns
                                from dot1stro.dt_strc struc, 
                                        dot1stro.dt_brdg bridge,
                                        dotsys.dt_dot_rgn rgn,
                                        dotsys.dt_wi_cnty_pitc county,
                                        dotsys.dt_cmty_pitc municipality,
                                        dot1stro.dt_agcy_ty agency,
                                        dot1stro.dt_brdg_load_pstg loadposting,
                                        dot1stro.dt_strc_ty,
                                        (
                                            select strc_id, strc_rte_sys as rsOn, ftr_nm as featureOn
                                            from dot1stro.dt_strc_rte
                                            where strc_rte_ouf = 'O'
                                                and strc_rte_prmy_fl = 'Y'
                                        ) routeon,
                                        (
                                            select strc_id, strc_rte_sys as rsUnder, ftr_nm as featureUnder
                                            from dot1stro.dt_strc_rte
                                            where strc_rte_ouf = 'U'
                                                and strc_rte_prmy_fl = 'Y'
                                        ) routeunder,
                                        (
                                            select brdg_func_cls_cd, brdg_func_cls_desc
                                            from dot1stro.dt_brdg_func_cls
                                        ) funcClassOn,
                                        (
                                            select brdg_func_cls_cd, brdg_func_cls_desc
                                            from dot1stro.dt_brdg_func_cls
                                        ) funcClassUnder
                                where struc.strc_id = :strId
                                    and struc.strc_sscd in ('50')
                                    and struc.strc_id = bridge.strc_id(+)
                                    and struc.dot_rgn_nb = rgn.dot_rgn_nb(+)
                                    and struc.strc_prmy_cnty_cd = county.dot_cnty_cd(+)
                                    and ltrim(municipality.hwy_cmty_cd, '0') = to_char(struc.strc_prmy_muni_cd)
                                    and struc.strc_ownr_agcy_cd = agency.agcy_tycd(+)
                                    and struc.strc_tycd = dt_strc_ty.strc_tycd
                                    and struc.strc_id = routeon.strc_id(+)
                                    and struc.strc_id = routeunder.strc_id(+)
                                    and bridge.brdg_load_pstg_cd = loadposting.brdg_load_pstg_cd(+)
                                    and bridge.func_cls_on_cd = funcClassOn.brdg_func_cls_cd(+)
                                    and bridge.func_cls_undr_cd = funcClassUnder.brdg_func_cls_cd(+)
                            ";

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                str = new WiSamEntities.Structure();
                str.StructureId = dt.Rows[0]["STRC_ID"].ToString();

                try
                {
                    str.Latitude = (dt.Rows[0]["LAT"]).ToString();
                }
                catch { }

                try
                {
                    str.Longitude = (dt.Rows[0]["LNGT"]).ToString();
                }
                catch { }

                try
                {
                    str.CorridorCode = dt.Rows[0]["STRC_PLND_PGM"].ToString();
                }
                catch
                {
                    str.CorridorCode = "";
                }

                if (str.StructureId.Length == 11)
                {
                    str.UnitBridge = true;
                }
                else
                {
                    str.UnitBridge = false;
                }

                str.StructureType = "";
                str.StructureTypeCode = "";
                str.MainSpanMaterial = "";
                str.MainSpanMaterialCode = "";
                GetMainSpanInfo(str);
                /*
                str.StructureType = dt.Rows[0]["SPAN_CNFG_TYDC"].ToString();
                str.StructureTypeCode = dt.Rows[0]["BSPN_CNFG_TY"].ToString();
                str.MainSpanMaterial = dt.Rows[0]["SPAN_MATL_DESC"].ToString();
                str.MainSpanMaterialCode = dt.Rows[0]["BSPN_MATL_CD"].ToString();
                */


                try
                {
                    str.MaxVehicleWeight = Convert.ToInt32(dt.Rows[0]["brdg_max_veh_wt"]);
                }
                catch { }

                try
                {
                    str.SkewAngle = Convert.ToInt32(dt.Rows[0]["BRDG_SANGL"]);
                }
                catch { }

                try
                {
                    str.HorizontalCurve = Convert.ToDouble(dt.Rows[0]["BRDG_HCRV_ON"]);
                }
                catch { }

                // Complex Structure?
                //bool b = listOfStrings.Any(s => myString.Contains(s));
                if (database.complexStructureConfigurations.Any(s => str.StructureTypeCode.Contains(s)))
                {
                    str.CplxStr = true;
                }
                else if (str.StructureTypeCode.Equals("40") && (str.MainSpanMaterialCode.Equals("03") || str.MainSpanMaterialCode.Equals("04")))
                {
                    str.CplxStr = true;
                }
                else if (str.StructureTypeCode.Equals("12") && (str.MainSpanMaterialCode.Equals("03") || str.MainSpanMaterialCode.Equals("04")))
                {
                    str.CplxStr = true;
                }
                else if (str.StructureTypeCode.Equals("11") && (str.MainSpanMaterialCode.Equals("03") || str.MainSpanMaterialCode.Equals("04")))
                {
                    str.CplxStr = true;
                }
                else if (str.StructureTypeCode.Equals("14") && (str.MainSpanMaterialCode.Equals("03") || str.MainSpanMaterialCode.Equals("04")))
                {
                    str.CplxStr = true;
                }
                else if (
                            str.StructureTypeCode.Equals("10")
                                && (str.MainSpanMaterialCode.Equals("03") || str.MainSpanMaterialCode.Equals("04"))
                                && (str.HorizontalCurve > 25 || str.SkewAngle > 25)
                        )
                {
                    str.CplxStr = true;
                }

                try
                {
                    str.FunctionalClassificationUnderCode = dt.Rows[0]["FUNC_CLS_UNDR_CD"].ToString();
                    str.FunctionalClassificationUnder = dt.Rows[0]["FCUNDER"].ToString();
                }
                catch { }

                try
                {
                    str.FunctionalClassificationOnCode = dt.Rows[0]["FUNC_CLS_ON_CD"].ToString();
                    if (str.FunctionalClassificationOnCode.Length == 0)
                    {
                        str.FunctionalClassificationOnCode = "NONE";
                    }
                    str.FunctionalClassificationOn = dt.Rows[0]["FCON"].ToString().ToUpper();
                }
                catch { }

                if (!String.IsNullOrEmpty(dt.Rows[0]["BRDG_LOAD_PSTG_CD"].ToString()))
                {
                    str.LoadPostingCode = Convert.ToInt32(dt.Rows[0]["BRDG_LOAD_PSTG_CD"]);
                    str.LoadPostingDesc = dt.Rows[0]["LOAD_PSTG_DESC"].ToString();

                    try
                    {
                        str.LoadPostingTonnage = Convert.ToDouble(dt.Rows[0]["LOAD_PSTG_TNS"]);
                    }
                    catch { }
                }
                else
                {
                    str.LoadPostingDesc = "None";
                }

                try
                {
                    str.LanesOn = Convert.ToInt32(dt.Rows[0]["BRDG_LANE_CNT_ON"]);
                }
                catch { }

                try
                {
                    str.DetLen = Convert.ToInt32(dt.Rows[0]["BRDG_DTR_LN_ON"]);
                }
                catch { }

                try
                {
                    str.trafficPatternOn = Convert.ToInt32(dt.Rows[0]["TRFC_PTRN_ON"]);
                }
                catch { }

                try
                {
                    str.trafficPatternUnder = Convert.ToInt32(dt.Rows[0]["TRFC_PTRN_UNDR"]);
                }
                catch { }

                // Grab other properties

                GetSpanInfo(str);

                try
                {
                    str.LoadCapacity = Convert.ToInt32(dt.Rows[0]["BGAR_LOAD_CPTY_CD"]);
                }
                catch { }

                try
                {
                    str.SufficiencyNumber = Convert.ToDouble(dt.Rows[0]["BRDG_SFCY_RTG"]);
                    str.SufficiencyRatingCurrent = str.SufficiencyNumber;

                    if (str.SufficiencyNumber < 50)
                        str.LocalFundingEligibility = "REPLACEMENT";
                    else if (str.SufficiencyNumber >= 50 && str.SufficiencyNumber <= 80)
                        str.LocalFundingEligibility = "REHABILITATION";
                }
                catch { }

                try
                {
                    str.OverburdenDepth = Convert.ToDouble(dt.Rows[0]["BRDG_OVRB_DPTH"]);
                }
                catch { }

                str.InventoryRating = dt.Rows[0]["BRDG_INV_LDRTG"].ToString().Trim();
                str.OperatingRating = dt.Rows[0]["BRDG_OPRT_LDRTG"].ToString().Trim();

                try
                {
                    str.HsOrRf = str.OperatingRating.Substring(0, 2).ToUpper();
                }
                catch { }

                if (str.HsOrRf.Equals("HS"))
                {
                    str.ratingValue = Convert.ToDouble(str.OperatingRating.Substring(2, str.OperatingRating.Length - 2));
                }
                else if (str.HsOrRf.Equals("RF"))
                {
                    str.ratingValue = Convert.ToDouble(str.OperatingRating.Substring(2, str.OperatingRating.Length - 2));
                }

                str.Region = dt.Rows[0]["REGION"].ToString();
                str.RegionNumber = dt.Rows[0]["REGIONNUMBER"].ToString();
                str.County = dt.Rows[0]["COUNTY"].ToString();
                str.CountyNumber = dt.Rows[0]["COUNTYNUMBER"].ToString();
                str.Municipality = dt.Rows[0]["MUNICIPALITY"].ToString();
                str.MunicipalityNumber = dt.Rows[0]["MUNICIPALITYNUMBER"].ToString();
                str.Owner = dt.Rows[0]["OWNER"].ToString();
                str.OwnerNumber = dt.Rows[0]["OWNERNUMBER"].ToString();

                try
                {
                    str.StmcAgcyTy = dt.Rows[0]["STMC_AGCY_TY"].ToString();
                }
                catch
                {
                    str.StmcAgcyTy = "";
                }

                try
                {
                    str.StimAgcyTycd = dt.Rows[0]["STIMAGCYTYCD"].ToString();
                }
                catch
                {
                    str.StimAgcyTycd = "";
                }

                if (String.IsNullOrEmpty(str.CorridorCode))
                {
                    //SHR3R
                    if (
                            str.OwnerNumber.Equals("10") || str.OwnerNumber.Equals("15") || str.OwnerNumber.Equals("16") || str.OwnerNumber.Equals("20") || str.OwnerNumber.Equals("44") || str.OwnerNumber.Equals("45") ||
                            str.StmcAgcyTy.Equals("10") || str.StmcAgcyTy.Equals("15") || str.StmcAgcyTy.Equals("16") || str.StmcAgcyTy.Equals("20") || str.StmcAgcyTy.Equals("44") || str.StmcAgcyTy.Equals("45") ||
                            str.StimAgcyTycd.Equals("10") || str.StimAgcyTycd.Equals("15") || str.StimAgcyTycd.Equals("16") || str.StimAgcyTycd.Equals("20") || str.StimAgcyTycd.Equals("44") || str.StimAgcyTycd.Equals("45")
                       )
                    {
                        str.FundingEligibility.Add("SHR3R");
                    }
                }
                else
                {
                    if (
                            str.OwnerNumber.Equals("10") || str.OwnerNumber.Equals("15") || str.OwnerNumber.Equals("16") || str.OwnerNumber.Equals("20") || str.OwnerNumber.Equals("44") || str.OwnerNumber.Equals("45") ||
                            str.StmcAgcyTy.Equals("10") || str.StmcAgcyTy.Equals("15") || str.StmcAgcyTy.Equals("16") || str.StmcAgcyTy.Equals("20") || str.StmcAgcyTy.Equals("44") || str.StmcAgcyTy.Equals("45") ||
                            str.StimAgcyTycd.Equals("10") || str.StimAgcyTycd.Equals("15") || str.StimAgcyTycd.Equals("16") || str.StimAgcyTycd.Equals("20") || str.StimAgcyTycd.Equals("44") || str.StimAgcyTycd.Equals("45")
                       )
                    {
                        if (str.CorridorCode.Equals("BB"))
                        {
                            str.FundingEligibility.Add("BB");
                        }
                        else if (str.CorridorCode.Equals("CM") || str.CorridorCode.Equals("C2") || str.CorridorCode.Equals("NO"))
                        {
                            str.FundingEligibility.Add("SHR3R");
                        }
                    }
                }

                str.FeatureOn = dt.Rows[0]["FEATUREON"].ToString();
                str.FeatureUnder = dt.Rows[0]["FEATUREUNDER"].ToString();

                try
                {
                    str.RouteSystemOn = dt.Rows[0]["RSON"].ToString().Trim().ToUpper();
                }
                catch { }

                try
                {
                    str.RouteSystemUnder = dt.Rows[0]["RSUNDER"].ToString().Trim().ToUpper();
                }
                catch { }

                str.ScourCriticalRating = dt.Rows[0]["BRDG_SCUR_CRTC"].ToString();
                str.ScourCritical = false;

                switch (str.ScourCriticalRating)
                {
                    case "3":
                    case "2":
                    case "1":
                    case "0":
                        str.ScourCritical = true;
                        break;
                }

                // Get last inspection ratings for determining SD and FO
                str.LastInspection = repo.GetLastInspection(strId);

                if (str.LastInspection != null)
                {
                    str.LastInspectionYear = str.LastInspection.InspectionDate.Year;

                    if (str.LastInspection.Elements.Where(e => e.ElemNum == 9325).Count() > 0)
                    {
                        str.BuriedStructure = true;
                    }
                }


                // Determine if it's a buried structure
                if (str.OverburdenDepth >= 9)
                {
                    str.BuriedStructure = true;
                }

                // Determine whether it's functionally obsolete
                str.FunctionalObsolete = false;
                str.FunctionalObsoleteDueToApproachRoadwayAlignment = false;
                str.FunctionalObsoleteDueToDeckGeometry = false;
                str.FunctionalObsoleteDueToStructureEvaluation = false;
                str.FunctionalObsoleteDueToVerticalClearance = false;
                str.FunctionalObsoleteDueToWaterwayAdequacy = false;

                // Determine whether it's structurally deficient
                str.StructurallyDeficient = false;

                if (str.LastInspection != null)
                {
                    try
                    {
                        if (!str.LastInspection.NbiRatings.DeckRating.Equals("N") && str.LastInspection.NbiRatings.DeckRatingVal <= 4)
                        {
                            str.StructurallyDeficient = true;
                        }
                    }
                    catch { }

                    try
                    {
                        if (!str.LastInspection.NbiRatings.SuperstructureRating.Equals("N") && str.LastInspection.NbiRatings.SuperstructureRatingVal <= 4)
                        {
                            str.StructurallyDeficient = true;
                        }
                    }
                    catch { }

                    try
                    {
                        if (!str.LastInspection.NbiRatings.SubstructureRating.Equals("N") && str.LastInspection.NbiRatings.SubstructureRatingVal <= 4)
                        {
                            str.StructurallyDeficient = true;
                        }
                    }
                    catch { }

                    try
                    {
                        if (!str.LastInspection.NbiRatings.CulvertRating.Equals("N") && str.LastInspection.NbiRatings.CulvertRatingVal <= 4)
                        {
                            str.StructurallyDeficient = true;
                        }
                    }
                    catch { }

                    try
                    {
                        if (!str.LastInspection.NbiRatings.WaterwayRating.Equals("N") && str.LastInspection.NbiRatings.WaterwayRatingVal == 3)
                        {
                            str.FunctionalObsolete = true;
                            str.FunctionalObsoleteDueToWaterwayAdequacy = true;
                        }

                        if (!str.LastInspection.NbiRatings.WaterwayRating.Equals("N") && str.LastInspection.NbiRatings.WaterwayRatingVal <= 2)
                        {
                            str.StructurallyDeficient = true;
                        }
                    }
                    catch { }
                }

                try
                {
                    int bgarRating = Convert.ToInt32(dt.Rows[0]["BGAR_COND_CD"]);

                    if (bgarRating == 3)
                    {
                        str.FunctionalObsolete = true;
                        str.FunctionalObsoleteDueToStructureEvaluation = true;
                    }

                    if (bgarRating <= 2)
                    {
                        str.StructurallyDeficient = true;
                    }
                }
                catch { }

                try
                {
                    int bgarGmtyOn = Convert.ToInt32(dt.Rows[0]["BGAR_GMTY_ON_CD"]);

                    if (bgarGmtyOn < 4)
                    {
                        str.FunctionalObsolete = true;
                        str.FunctionalObsoleteDueToDeckGeometry = true;
                    }
                }
                catch { }

                try
                {
                    int bgarAprcAlnm = Convert.ToInt32(dt.Rows[0]["BGAR_APRC_ALNM_CD"]);

                    if (bgarAprcAlnm < 4)
                    {
                        str.FunctionalObsolete = true;
                        str.FunctionalObsoleteDueToApproachRoadwayAlignment = true;
                    }
                }
                catch { }

                try
                {
                    int bgarGmtyUnder = Convert.ToInt32(dt.Rows[0]["BGAR_GMTY_UNDR_CD"]);

                    if (bgarGmtyUnder < 4)
                    {
                        str.FunctionalObsolete = true;
                        str.FunctionalObsoleteDueToVerticalClearance = true;
                    }
                }
                catch { }

                if (str.StructurallyDeficient)
                {
                    str.Deficiencies.Add("SD");
                }

                if (str.FunctionalObsolete)
                {
                    str.Deficiencies.Add("FO");
                }

                // Year built, Construction work performed
                qry = @"
                                select strc_id, 
                                        cnst_yr,
                                        proj.cnst_work_pfmd_cd,
                                        work.cnst_work_desc
                                from dot1stro.dt_cnst_proj proj,
                                        dot1stro.dt_cnst_work_pfmd work
                                where strc_id = :strId
                                    and proj.cnst_work_pfmd_cd = work.cnst_work_pfmd_cd                     
                                    and proj.cnst_work_pfmd_cd <> '99'
                                order by cnst_yr asc, proj.cnst_work_pfmd_cd asc
                        ";
                OracleParameter[] prms2 = new OracleParameter[1];
                prms2[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                prms2[0].Value = strId;
                DataTable dt2 = ExecuteSelect(qry, prms2, database.hsiConnection);

                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    int numOverlays = 0;
                    int numThinPolymerOverlays = 0;

                    foreach (DataRow dr in dt2.Rows)
                    {
                        string workPerformedCode = dr["CNST_WORK_PFMD_CD"].ToString();
                        int constYear = Convert.ToInt32(dr["CNST_YR"]);
                        str.ConstructionHistory += String.Format("({0}) {1}\r\n", constYear, dr["CNST_WORK_DESC"].ToString());

                        switch (workPerformedCode)
                        {
                            case "01": // new structure
                                str.YearBuilt = constYear;
                                str.YearBuiltActual = constYear;
                                str.DeckBuiltYearActual = constYear;
                                str.DeckBuilts.Add(constYear);
                                str.LastSuperReplacementYear = constYear;
                                str.SuperBuilts.Add(constYear);
                                str.LastDeckReplacementYear = constYear;
                                str.Overlays.Add(constYear);
                                break;

                            case "08": // new superstructure
                                numOverlays = 0;
                                numThinPolymerOverlays = 0;
                                str.DeckBuiltYearActual = constYear;
                                str.DeckBuilts.Add(constYear);
                                str.LastSuperReplacementYear = constYear;
                                str.SuperBuilts.Add(constYear);
                                str.LastDeckReplacementYear = constYear;
                                str.Overlays.Add(constYear);
                                break;

                            case "06": // new deck
                            case "68":// new deck/widening
                                numOverlays = 0;
                                numThinPolymerOverlays = 0;
                                str.DeckBuiltYearActual = constYear;
                                str.DeckBuilts.Add(constYear);
                                str.LastDeckReplacementYear = constYear;
                                str.Overlays.Add(constYear);
                                break;

                            case "03": // concrete overlay
                            case "58": // concrete overlay, new joints
                            case "20": // concrete overlay, new joints, new rail

                            case "78": // pma olay
                            case "23": // concrete olay, widen
                            case "81": // polyester overlay
                            case "21": // hma/bituminous overlay
                                numOverlays++;
                                str.LastOverlayYear = constYear;
                                str.Overlays.Add(constYear);
                                break;
                            case "65": // epoxy overlay
                            case "77": // thin polymer olay
                                numThinPolymerOverlays++;
                                if (countTpo)
                                    numOverlays++;
                                str.LastOverlayYear = constYear;
                                str.Overlays.Add(constYear);
                                break;
                        }
                    }

                    str.NumOlays = numOverlays;
                    str.NumThinPolymerOverlays = numThinPolymerOverlays;

                    if (!String.IsNullOrEmpty(dt.Rows[0]["BGDK_AREA"].ToString()))
                    {
                        str.DeckArea = Convert.ToInt32(dt.Rows[0]["BGDK_AREA"]);
                    }
                    else if (str.LastInspection != null)
                    {
                        var decks = str.LastInspection.Elements.Where(e => e.ElementClassificationCode.Equals(WiSamEntities.Code.Deck) || e.ElementClassificationCode.Equals(WiSamEntities.Code.Slab)).ToList();

                        if (decks.Count > 0)
                        {
                            str.DeckArea = decks.First().TotalQuantity;
                        }
                    }
                }

                str.FractureCritical = false;
                qry = @"
                                select strc_id, 
                                        stin_tycd
                                from dot1stro.dt_stin_instc
                                where strc_id = :strId
                                    and stin_tycd = 'FC'
                        ";
                OracleParameter[] prms3 = new OracleParameter[1];
                prms3[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                prms3[0].Value = strId;
                DataTable dt3 = ExecuteSelect(qry, prms3, database.hsiConnection);

                if (dt3 != null && dt3.Rows.Count > 0)
                {
                    str.FractureCritical = true;
                }

                if (str.ScourCritical && str.FractureCritical)
                {
                    str.ScFc = "SCFC";
                }
                else if (str.ScourCritical)
                {
                    str.ScFc = "SC";
                }
                else if (str.FractureCritical)
                {
                    str.ScFc = "FC";
                }
                else
                {
                    str.ScFc = "";
                }
            }

            return str;
        }

        public WorkConcept GetStructureCertification(WorkConcept currentWorkConcept)
        {
            WorkConcept wc = null;
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        select ph.projectdbid, ph.status as projectStatus, ph.useraction as projectUserAction, 
                            ph.projecthistorydbid, ph.certificationliaisonuserfullname, ph.userdbid, ph.useractiondatetime, 
                            pwch.certificationdecision, pwch.certificationprimaryworktypecomments,
                            pwch.certificationsecondaryworktypecomments, pwch.certificationadditionalcomments
                        from projecthistory ph, projectworkconcepthistory pwch
                        where ph.status in (4, 12)
                            and ph.useraction in (40, 110)
                            and ph.projecthistorydbid = pwch.projecthistorydbid
                            and pwch.structureid = @structureId
                            and ph.projectdbid = @projectDbId
                        order by ph.projecthistorydbid desc
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = currentWorkConcept.ProjectDbId;
                prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[1].Value = currentWorkConcept.StructureId;
                DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    wc = new WorkConcept();
                    DataRow dr = dt.Rows[0];
                    int projectUserAction = Convert.ToInt32(dr["projectuseraction"]);
                    int projectStatus = Convert.ToInt32(dr["projectstatus"]);
                    UserAccount liaison = GetUserAccount(Convert.ToInt32(dr["userdbid"]));
                    wc.ProjectCertificationLiaisonUserFullName = String.Format("{0} {1}", liaison.FirstName, liaison.LastName);
                    wc.CertificationDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                    wc.CertificationLiaisonEmail = liaison.EmailAddress;
                    wc.CertificationLiaisonPhone = liaison.PhoneNumber;
                    if (projectUserAction == 110)
                    {
                        wc.CertificationDecision = "Transitionally Certified";
                    }
                    else if (projectUserAction == 40)
                    {
                        wc.CertificationDecision = "Fully Certified";
                    }
                    //wc.ProjectCertificationLiaisonUserFullName = dr["certificationliaisonuserfullname"].ToString();
                    //wc.CertificationDecision = dr["certificationdecision"].ToString();
                    wc.CertificationPrimaryWorkTypeComments = dr["certificationprimaryworktypecomments"].ToString();
                    wc.CertificationSecondaryWorkTypeComments = dr["certificationsecondaryworktypecomments"].ToString();
                    wc.CertificationAdditionalComments = dr["certificationadditionalcomments"].ToString();
                }
            }

            return wc;
        }

        public GeoLocation GetStructureGeoLocation(string structureId)
        {
            GeoLocation gl = null;
            string qry = @"
                            select lat, lngt
                            from dot1stro.dt_strc
                            where strc_sscd in ('50', '70')
                                and strc_id = :structureId
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("structureId", OracleDbType.Varchar2);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                gl = new GeoLocation();
                gl.HsiLatitude = dr["lat"].ToString();
                gl.HsiLongitude = dr["lngt"].ToString();
                gl.LatitudeDecimal = Convert.ToSingle(repo.ConvertDegreesMinutesSecondsToDecimalDegrees(gl.HsiLatitude));
                //gl.LongitudeDecimal = Convert.ToSingle(ConvertDegreesMinutesSecondsToDecimalDegrees(gl.HsiLongitude));
                //gl.LatitudeDecimal = Convert.ToSingle(ConvertDegreesMinutesSecondsToDecimalDegrees(gl.HsiLatitude));
                gl.LongitudeDecimal = -Convert.ToSingle(repo.ConvertDegreesMinutesSecondsToDecimalDegrees(gl.HsiLongitude));
            }

            return gl;
        }

        public GeoLocation GetStructureLatLong(string structureId)
        {
            GeoLocation gl = null;
            string qry = @"
                                select lat, lngt, latdecimal, lngtdecimal, regioncode, regionnumber
                                from structure
                                where structureid = @structureId
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                gl = new GeoLocation();
                gl.HsiLatitude = dr["lat"].ToString();
                gl.HsiLongitude = dr["lngt"].ToString();
                gl.LatitudeDecimal = Convert.ToSingle(dr["latdecimal"]);
                gl.LongitudeDecimal = Convert.ToSingle(dr["lngtdecimal"]);
                gl.RegionCode = dr["regioncode"].ToString();
                gl.RegionNumber = Convert.ToInt32(dr["regionnumber"]);
                gl.Region = dr["regionnumber"].ToString() + "-" + dr["regioncode"].ToString();
            }

            return gl;
        }

        public string GetStructureOwnerAgencyCode(string structureId)
        {
            string owner = "";
            string qry =
                @"
                    select owneragencycode
                    from structure
                    where structureid = @structureId
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                owner = dt.Rows[0]["owneragencycode"].ToString();
            }

            return owner;
        }

        public WorkConcept GetStructurePrecertification(WorkConcept currentWorkConcept)
        {
            WorkConcept wc = null;
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        select ph.projectdbid, ph.status as projectStatus, ph.useraction as projectUserAction, 
                            ph.projecthistorydbid, ph.precertificationliaisonuserfullname,
                            pwch.precertificationdecision, pwch.precertificationdecisiondatetime,
                            pwch.precertificationdecisionreasoncategory, pwch.precertificationdecisionreasonexplanation,
                            pwch.precertificationdecisioninternalcomments
                        from projecthistory ph, projectworkconcepthistory pwch
                        where ph.useraction in (2, 30, 31, 70)
                            and ph.projecthistorydbid = pwch.projecthistorydbid
                            and pwch.structureid = @structureId
                            and ph.projectdbid = @projectDbId
                        order by ph.projecthistorydbid desc
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = currentWorkConcept.ProjectDbId;
                prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[1].Value = currentWorkConcept.StructureId;
                DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    wc = new WorkConcept();
                    DataRow dr = dt.Rows[0];
                    int projectUserAction = Convert.ToInt32(dr["projectuseraction"]);
                    int projectStatus = Convert.ToInt32(dr["projectstatus"]);
                    wc.ProjectPrecertificationLiaisonUserFullName = dr["precertificationliaisonuserfullname"].ToString();
                    if (projectStatus == 3 && projectUserAction == 2)
                    {
                        wc.ProjectPrecertificationLiaisonUserFullName = "Auto-precertified";
                        wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.AutoAccept;
                    }
                    else
                    {
                        /*
                        if (projectUserAction == 31) // Need to clean up database for this project user action- should have reject status, but not the case
                        {
                            wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.Reject;
                        }
                        else
                        {
                            if (dr["precertificationdecision"] != DBNull.Value)
                            {
                                wc.PrecertificationDecision = (StructuresProgramType.PrecertificatioReviewDecision)(Convert.ToInt32(dr["precertificationdecision"]));
                            }
                        }*/
                        if (dr["precertificationdecision"] != DBNull.Value)
                        {
                            wc.PrecertificationDecision = (StructuresProgramType.PrecertificatioReviewDecision)(Convert.ToInt32(dr["precertificationdecision"]));
                        }
                        if (dr["precertificationdecisionreasoncategory"] != DBNull.Value)
                        {
                            wc.PrecertificationDecisionReasonCategory = dr["precertificationdecisionreasoncategory"].ToString();
                        }
                        if (dr["precertificationdecisionreasonexplanation"] != DBNull.Value)
                        {
                            wc.PrecertificationDecisionReasonExplanation = dr["precertificationdecisionreasonexplanation"].ToString();
                        }
                        if (dr["precertificationdecisioninternalcomments"] != DBNull.Value)
                        {
                            wc.PrecertificationDecisionInternalComments = dr["precertificationdecisioninternalcomments"].ToString();
                        }
                    }
                }
            }
            return wc;
        }

        public List<Project> GetStructureProjects(int startFiscalYear, int endFiscalYear, string region = "any")
        {
            List<Project> projects = new List<Project>();

            // Grab quasi-certified projects
            for (int i = startFiscalYear; i <= endFiscalYear; i++)
            {
                projects.AddRange(GetStructureProjects(i, region));
            }

            // Grab projects created in the tool
            string qry3 =
               @"
                    select p.*, ph.*
                    from project p, projecthistory ph
                    where ph.fiscalyear >= @startFiscalYear
                        and ph.fiscalyear <= @endFiscalYear
                        and p.projectdbid = ph.projectdbid
                        and p.deletedate is null
                        and p.active = 1
                ";
            SqlParameter[] prms3 = new SqlParameter[2];

            if (!region.Equals("any"))
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

                qry3 +=
                    @"
                        and p.region = @regionComboCode
                        order by p.projectdbid, ph.projecthistorydbid desc
                    ";
                prms3 = null;
                prms3 = new SqlParameter[3];
                prms3[2] = new SqlParameter("@regionComboCode", SqlDbType.VarChar);
                prms3[2].Value = regionComboCode;
            }
            else
            {
                qry3 +=
                    @"
                        order by p.projectdbid, ph.projecthistorydbid desc
                    ";
            }

            prms3[0] = new SqlParameter("@startFiscalYear", SqlDbType.Int);
            prms3[0].Value = startFiscalYear;
            prms3[1] = new SqlParameter("@endFiscalYear", SqlDbType.Int);
            prms3[1].Value = endFiscalYear;
            DataTable dt3 = ExecuteSelect(qry3, prms3, database.wisamsConnection);

            if (dt3 != null && dt3.Rows.Count > 0)
            {
                int previousProjectDbId = 0;
                Project previousProject = new Project();

                foreach (DataRow dr in dt3.Rows)
                {
                    int currentProjectDbId = Convert.ToInt32(dr["projectdbid"]);

                    if (currentProjectDbId != previousProjectDbId)
                    {
                        Project project = new Project();
                        previousProject = project;
                        projects.Add(project);
                        project.ProjectDbId = currentProjectDbId;

                        try
                        {
                            project.BoxId = dr["boxid"].ToString();
                        }
                        catch
                        {
                            project.BoxId = "";
                        }

                        project.ProjectHistoryDbId = Convert.ToInt32(dr["projecthistorydbid"]);
                        project.Region = dr["region"].ToString();
                        project.CurrentFiscalYear = Convert.ToInt32(dr["currentfiscalyear"]);
                        project.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                        project.ProjectYear = Convert.ToInt32(dr["projectyear"]);

                        try
                        {
                            project.AdvanceableFiscalYear = Convert.ToInt32(dr["advanceablefiscalyear"]);
                        }
                        catch { }

                        project.StructuresConcept = dr["structuresconcept"].ToString();
                        project.StructuresCost = Convert.ToInt32(dr["structurescost"]);
                        project.IsQuasicertified = Convert.ToBoolean(dr["quasicertified"]);
                        project.Status = (StructuresProgramType.ProjectStatus)dr["status"];
                        project.Description = dr["description"].ToString();
                        project.Notes = dr["notes"].ToString();
                        project.UserDbId = Convert.ToInt32(dr["userdbid"]);
                        project.UserAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                        project.UserActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                        project.UserFullName = dr["userfullname"].ToString();

                        try
                        {
                            project.Locked = Convert.ToBoolean(dr["locked"]);
                        }
                        catch
                        {
                            // If database value's null
                            project.Locked = false;
                        }

                        try
                        {
                            project.SubmitDate = Convert.ToDateTime(dr["submitdate"]);
                        }
                        catch { }

                        try
                        {
                            project.PrecertifyDate = Convert.ToDateTime(dr["precertifydate"]);
                        }
                        catch { }

                        try
                        {
                            project.CertifyDate = Convert.ToDateTime(dr["certifydate"]);
                        }
                        catch { }

                        try
                        {
                            project.UserFullName = dr["userfullname"].ToString();
                        }
                        catch { }

                        try
                        {
                            project.InPrecertification = Convert.ToBoolean(dr["inprecertification"]);
                        }
                        catch
                        {
                            // If database value's null
                            project.InPrecertification = false;
                        }

                        try
                        {
                            project.InCertification = Convert.ToBoolean(dr["incertification"]);
                        }
                        catch
                        {
                            // If database value's null
                            project.InCertification = false;
                        }

                        try
                        {
                            project.PrecertificationLiaisonUserDbId = Convert.ToInt32(dr["precertificationliaisonuserdbid"]);
                            project.PrecertificationLiaisonUserFullName = dr["precertificationliaisonuserfullname"].ToString();
                        }
                        catch
                        {
                            project.PrecertificationLiaisonUserFullName = "";
                        }

                        try
                        {
                            project.CertificationLiaisonUserDbId = Convert.ToInt32(dr["certificationliaisonuserdbid"]);
                            project.CertificationLiaisonUserFullName = dr["certificationliaisonuserfullname"].ToString();
                        }
                        catch
                        {
                            project.CertificationLiaisonUserFullName = "";
                        }

                        try
                        {
                            project.RequestAdvancedCertification = Convert.ToBoolean(dr["requestadvancedcertification"]);
                            project.AdvancedCertificationDate = Convert.ToDateTime(dr["advancedcertificationdate"]);
                        }
                        catch { }

                        project.History = String.Format("{0}: {1} by {2}", project.UserActionDateTime, project.UserAction, project.UserFullName);
                        project.UserDbIds = new List<int>();
                        project.UserDbIds.Add(project.UserDbId);

                        try
                        {
                            project.FosProjectId = dr["fosprojectid"].ToString();
                        }
                        catch
                        {
                            project.FosProjectId = "";
                        }

                        // Work Concepts
                        string qry4 =
                            @"
                                select *
                                from projectworkconcepthistory
                                where projecthistorydbid = @projectHistoryDbId
                                order by structureid
                            ";
                        SqlParameter[] prms4 = new SqlParameter[1];
                        prms4[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                        prms4[0].Value = project.ProjectHistoryDbId;
                        DataTable dt4 = ExecuteSelect(qry4, prms4, database.wisamsConnection);

                        if (dt4 != null && dt4.Rows.Count > 0)
                        {
                            foreach (DataRow dr4 in dt4.Rows)
                            {
                                WorkConcept wc = new WorkConcept();
                                wc.ProjectDbId = currentProjectDbId;
                                wc.StructureProjectFiscalYear = project.FiscalYear;
                                wc.Region = project.Region;
                                wc.StructuresConcept = project.StructuresConcept;
                                wc.WorkConceptDbId = Convert.ToInt32(dr4["workconceptdbid"]);
                                wc.ProjectWorkConceptHistoryDbId = Convert.ToInt32(dr4["projectworkconcepthistorydbid"]);
                                wc.WorkConceptCode = dr4["workconceptcode"].ToString().Trim();
                                wc.WorkConceptDescription = dr4["workconceptdescription"].ToString().Trim();
                                wc.CertifiedWorkConceptCode = dr4["certifiedworkconceptcode"].ToString().Trim();
                                wc.CertifiedWorkConceptDescription = dr4["certifiedworkconceptdescription"].ToString().Trim();
                                wc.StructureId = dr4["structureid"].ToString();
                                wc.PlannedStructureId = dr4["plannedstructureid"].ToString();
                                wc.CurrentFiscalYear = Convert.ToInt32(dr4["currentfiscalyear"]);
                                wc.FiscalYear = Convert.ToInt32(dr4["fiscalyear"]);
                                wc.ProjectYear = Convert.ToInt32(dr4["projectyear"]);

                                try
                                {
                                    wc.PriorityScore = Convert.ToSingle(dr4["priorityscore"]);
                                }
                                catch { }

                                try
                                {
                                    wc.Cost = Convert.ToInt32(dr4["cost"]);
                                }
                                catch { }

                                wc.FromEligibilityList = Convert.ToBoolean(dr4["fromeligibilitylist"]);
                                wc.FromFiips = Convert.ToBoolean(dr4["fromfiips"]);
                                wc.Evaluate = Convert.ToBoolean(dr4["evaluate"]);

                                try
                                {
                                    wc.FromProposedList = Convert.ToBoolean(dr4["fromproposedlist"]);
                                }
                                catch
                                {
                                    wc.FromProposedList = false;
                                }

                                try
                                {
                                    wc.EarlierFiscalYear = Convert.ToInt32(dr4["earlierfiscalyear"]);
                                    wc.LaterFiscalYear = Convert.ToInt32(dr4["laterfiscalyear"]);
                                }
                                catch { }

                                try
                                {
                                    wc.ChangeJustifications = dr4["changejustifications"].ToString();
                                }
                                catch { }

                                try
                                {
                                    wc.ChangeJustificationNotes = dr4["changenotes"].ToString();
                                }
                                catch { }

                                try
                                {
                                    wc.IsQuasicertified = Convert.ToBoolean(dr4["quasicertified"]);
                                }
                                catch { }

                                wc.Status = (StructuresProgramType.WorkConceptStatus)dr4["status"];

                                try
                                {
                                    wc.GeoLocation.LatitudeDecimal = Convert.ToSingle(dr4["latitude"]);
                                    wc.GeoLocation.LongitudeDecimal = Convert.ToSingle(dr4["longitude"]);
                                }
                                catch { }

                                try
                                {
                                    wc.PrecertificationDecision =
                                        (StructuresProgramType.PrecertificatioReviewDecision)dr4["precertificationdecision"];
                                }
                                catch { }

                                try
                                {
                                    wc.PrecertificationDecisionDateTime = Convert.ToDateTime(dr4["precertificationdecisiondatetime"]);
                                }
                                catch { }

                                try
                                {
                                    wc.PrecertificationDecisionReasonCategory = dr4["precertificationdecisionreasoncategory"].ToString();
                                }
                                catch
                                {
                                    wc.PrecertificationDecisionReasonCategory = "";
                                }

                                try
                                {
                                    wc.PrecertificationDecisionReasonExplanation = dr4["precertificationdecisionreasonexplanation"].ToString();
                                }
                                catch
                                {
                                    wc.PrecertificationDecisionReasonExplanation = "";
                                }

                                try
                                {
                                    wc.PrecertificationDecisionInternalComments = dr4["precertificationdecisioninternalcomments"].ToString();
                                }
                                catch
                                {
                                    wc.PrecertificationDecisionInternalComments = "";
                                }

                                try
                                {
                                    wc.CertificationDecision = dr4["certificationdecision"].ToString();
                                }
                                catch
                                {
                                    wc.CertificationDecision = "";
                                }

                                try
                                {
                                    wc.CertificationDateTime = Convert.ToDateTime(dr4["certificationdatetime"]);
                                }
                                catch { }

                                try
                                {
                                    wc.CertificationPrimaryWorkTypeComments = dr4["certificationprimaryworktypecomments"].ToString();
                                }
                                catch
                                {
                                    wc.CertificationPrimaryWorkTypeComments = "";
                                }

                                try
                                {
                                    wc.CertificationSecondaryWorkTypeComments = dr4["certificationsecondaryworktypecomments"].ToString();
                                }
                                catch
                                {
                                    wc.CertificationSecondaryWorkTypeComments = "";
                                }

                                try
                                {
                                    wc.CertificationAdditionalComments = dr4["certificationadditionalcomments"].ToString();
                                }
                                catch
                                {
                                    wc.CertificationAdditionalComments = "";
                                }

                                try
                                {
                                    wc.EstimatedConstructionCost = Convert.ToInt32(dr4["estimatedconstructioncost"]);
                                }
                                catch { }

                                try
                                {
                                    wc.EstimatedDesignLevelOfEffort = Convert.ToInt32(dr4["estimateddesignlevelofeffort"]);
                                }
                                catch { }

                                try
                                {
                                    wc.DesignResourcing = dr4["designresourcing"].ToString();
                                }
                                catch
                                {
                                    wc.DesignResourcing = "";
                                }

                                if (wc.FromProposedList && project.Status != StructuresProgramType.ProjectStatus.Certified) // Add only if it's active
                                {
                                    string qry5 =
                                        @"
                                            select *
                                            from proposedlist
                                            where workconceptdbid = @workConceptDbId
                                                and structureid = @structureId
                                                and active = 0
                                        ";
                                    SqlParameter[] prms5 = new SqlParameter[2];
                                    prms5[0] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
                                    prms5[0].Value = wc.WorkConceptDbId;
                                    prms5[1] = new SqlParameter("@structureId", wc.StructureId);
                                    prms5[1].Value = wc.StructureId;
                                    DataTable dt5 = ExecuteSelect(qry5, prms5, database.wisamsConnection);

                                    if (dt5 == null || dt5.Rows.Count == 0)
                                    {
                                        project.WorkConcepts.Add(wc);
                                        project.NumberOfStructures++;
                                    }
                                }
                                else
                                {
                                    project.WorkConcepts.Add(wc);
                                    project.NumberOfStructures++;
                                }

                                // Grab Element-Work Concept combinations
                                if (wc.CertificationDateTime.Year != 1)
                                {
                                    string qry6 =
                                        @"
                                        select p.*, w.workactiondesc
                                        from projectelementworkconcept p left join workaction w
                                            on p.workconceptcode = w.workactioncode
                                        where p.projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                                            and p.certificationdatetime = @certificationDateTime
                                    ";

                                    SqlParameter[] prms6 = new SqlParameter[2];
                                    prms6[0] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
                                    prms6[0].Value = wc.ProjectWorkConceptHistoryDbId;
                                    prms6[1] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
                                    prms6[1].Value = wc.CertificationDateTime;
                                    DataTable dt6 = ExecuteSelect(qry6, prms6, database.wisamsConnection);

                                    if (dt6 != null && dt6.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr6 in dt6.Rows)
                                        {
                                            ElementWorkConcept ewc = new ElementWorkConcept();
                                            ewc.StructureId = wc.StructureId;
                                            ewc.ElementWorkConceptDbId = Convert.ToInt32(dr6["elementworkconceptdbid"]);
                                            ewc.ProjectWorkConceptHistoryDbId = wc.ProjectWorkConceptHistoryDbId;

                                            try
                                            {
                                                ewc.CertificationDateTime = Convert.ToDateTime(dr6["certificationdatetime"]);
                                            }
                                            catch { }

                                            ewc.ElementNumber = Convert.ToInt32(dr6["elementnumber"]);
                                            ewc.WorkConceptCode = dr6["workconceptcode"].ToString();
                                            ewc.WorkConceptDescription = dr6["workactiondesc"].ToString();
                                            ewc.WorkConceptLevel = dr6["workconceptlevel"].ToString();

                                            if (dr6["comments"] != DBNull.Value)
                                            {
                                                ewc.Comments = dr6["comments"].ToString();
                                            }

                                            project.CertifiedElementWorkConceptCombinations.Add(ewc);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int userDbId = Convert.ToInt32(dr["userdbid"]);
                        string userFullName = dr["userfullname"].ToString();
                        StructuresProgramType.ProjectUserAction userAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                        DateTime userActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                        previousProject.History += String.Format("\r\n\r\n{0}: {1} by {2}", userActionDateTime, userAction, userFullName);
                        previousProject.UserDbIds.Add(userDbId);
                    }

                    previousProjectDbId = currentProjectDbId;
                }
            }

            return projects;
        }

        public string GetWorkflowTransaction(StructuresProgramType.ProjectUserAction projectUserAction)
        {
            string transaction = "";
            switch (projectUserAction)
            {
                case StructuresProgramType.ProjectUserAction.SavedProject:
                    transaction = "Project's saved but not submitted for review";
                    break;
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification:
                    transaction = "Project's submitted for precertification";
                    break;
                case StructuresProgramType.ProjectUserAction.Precertification:
                    transaction = "Project's in precertification";
                    break;
                case StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment:
                    transaction = "Project's precertification liaison is unassigned";
                    break;
                case StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification:
                    transaction = "Project's approved for precertification";
                    break;
                case StructuresProgramType.ProjectUserAction.BosRejectedPrecertification:
                    transaction = "Project's rejected for precertification";
                    break;
                case StructuresProgramType.ProjectUserAction.Certification:
                    transaction = "Project's in certification";
                    break;
                case StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment:
                    transaction = "Project's certification liaison is unassigned";
                    break;
                case StructuresProgramType.ProjectUserAction.BosTransitionallyCertified:
                    transaction = "Project's transitionally certified";
                    break;
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification:
                    transaction = "Project's certified pending the review of a BOS certification supervisor";
                    break;
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection:
                    transaction = "Project's rejected for certification pending the review of a BOS certification supervisor";
                    break;
                case StructuresProgramType.ProjectUserAction.BosCertified:
                    transaction = "Project's approved for certification";
                    break;
                case StructuresProgramType.ProjectUserAction.BosRejectedCertification:
                    transaction = "Project's rejected for certification";
                    break;
                case StructuresProgramType.ProjectUserAction.RequestRecertification:
                    transaction = "Project's requested for recertification";
                    break;
                case StructuresProgramType.ProjectUserAction.GrantRecertification:
                    transaction = "Project's request for recertification is granted";
                    break;
                case StructuresProgramType.ProjectUserAction.RejectRecertification:
                    transaction = "Project's request for recertification is rejected";
                    break;
                case StructuresProgramType.ProjectUserAction.DeletedProject:
                    transaction = "Project's been deleted";
                    break;
                case StructuresProgramType.ProjectUserAction.Deactivated:
                    transaction = "Project's been deactivated";
                    break;
            }
            return transaction;
        }

        public List<Project> GetStructureProjects(int fiscalYear, string region = "any")
        {
            List<Project> projects = new List<Project>();
            // Grab quasicertified or precertified projects
            string qry =
                @"
                    select distinct fosprojectid
                    from structureprogramreviewcurrent
                    where workactionyear = @fiscalYear
                        and (iscertified = 1)
                        and fosprojectid is not null
                    order by fosprojectid
                ";
            SqlParameter[] prms = new SqlParameter[1];

            if (!region.Equals("any"))
            {
                qry =
                    @"
                        select distinct fosprojectid
                        from structureprogramreviewcurrent
                        where workactionyear = @fiscalYear
                            and region = @region
                            and (iscertified = 1)
                            and fosprojectid is not null
                            and fosprojectid <> ''
                        order by fosprojectid
                    ";
                prms = null;
                prms = new SqlParameter[2];
            }

            //or certificationstatus = 'PRE-CERTIFIED'
            if (!region.Equals("any"))
            {
                prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[1].Value = region;
            }

            prms[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
            prms[0].Value = fiscalYear;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                string currentFosProjectId = dr["fosprojectid"].ToString().Trim();

                if (currentFosProjectId.Length > 0)
                {
                    /*
                    select s.*, w.mapmarkertype
                            from structureprogramreviewcurrent s, workaction w
                            where s.workactionyear = @fiscalYear
                                and s.iscertified = 1
                                and s.workactioncode = w.workactioncode*/
                    string qry2 = @"
                                    select s.*
                                    from structureprogramreviewcurrent s
                                    where fosprojectid = @currentFosProjectId
                                        and workactionyear = @fiscalYear
                                        and (iscertified = 1)
                                    order by structureid
                                ";

                    //or certificationstatus = 'PRE-CERTIFIED'
                    SqlParameter[] prms2 = new SqlParameter[2];
                    prms2[0] = new SqlParameter("@currentFosProjectId", SqlDbType.VarChar);
                    prms2[0].Value = currentFosProjectId;
                    prms2[1] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                    prms2[1].Value = fiscalYear;
                    DataTable dt2 = ExecuteSelect(qry2, prms2, database.wisamsConnection);

                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        Project project = new Project();
                        project.ProjectDbId = database.projectCounter;
                        project.FosProjectId = currentFosProjectId;
                        project.Status = StructuresProgramType.ProjectStatus.Certified;
                        project.UserAction = StructuresProgramType.ProjectUserAction.BosCertified;
                        project.UserActionDateTime = new DateTime(2019, 2, 1);
                        project.UserDbId = 0; // System
                        project.IsQuasicertified = true;
                        project.FiscalYear = fiscalYear;
                        project.Region = dt2.Rows[0]["regionnumber"].ToString() + "-" + dt2.Rows[0]["region"].ToString();
                        project.StructuresConcept = "Structures";
                        List<WorkConcept> workConcepts = new List<WorkConcept>();
                        project.WorkConcepts = workConcepts;
                        string currentStructureId = "";

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            WorkConcept wc = new WorkConcept();
                            wc.WorkConceptDbId = Convert.ToInt32(dr2["rowdbid"]);
                            wc.ProjectDbId = project.ProjectDbId;
                            wc.StructureId = dr2["structureid"].ToString().Trim();
                            wc.PlannedStructureId = "";

                            if (!wc.StructureId.Equals(currentStructureId))
                            {
                                project.NumberOfStructures++;
                            }

                            wc.WorkConceptCode = dr2["workactioncode"].ToString().Trim();
                            wc.WorkConceptDescription = dr2["workactiondesc"].ToString().Trim();

                            if (wc.WorkConceptCode.Equals("07"))
                            {
                                wc.WorkConceptDescription = "PAINT";
                            }

                            wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                            wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                            wc.Region = dr2["regionnumber"].ToString().Trim() + "-" + dr2["region"].ToString().Trim();
                            wc.CurrentFiscalYear = database.currentFiscalYear;
                            wc.FiscalYear = fiscalYear;
                            wc.ProjectYear = database.currentProjectYear + (wc.FiscalYear - wc.CurrentFiscalYear);
                            wc.StructureProjectFiscalYear = fiscalYear;
                            //wc.Cost = Convert.ToInt32(dr["primarycost"]); NO Cost
                            //wc.SecondaryWorkConcepts = dr["secondaryworkactions"].ToString().Trim();
                            wc.FromEligibilityList = false;
                            wc.IsQuasicertified = true;
                            wc.IsScopeAMatch = Convert.ToBoolean(dr2["isscopeamatch"]);
                            wc.IsYearAMatch = Convert.ToBoolean(dr2["isyearamatch"]);
                            wc.FosProjectId = dr2["fosprojectid"].ToString().Trim();
                            //wc.GeoLocation = GetStructureGeoLocation(wc.StructureId);

                            //if (dr2["certificationstatus"].ToString().Trim().Equals("CERTIFIED"))
                            //{
                            wc.Status = StructuresProgramType.WorkConceptStatus.Quasicertified;
                            //}
                            /*
                            else
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                                project.Status = StructuresProgramType.ProjectStatus.Precertified;
                            }*/
                            wc.CertificationAdditionalComments = "Reviewed and certified (without BOSCD) as part of the transitional implementation of the BOS certification process.";
                            workConcepts.Add(wc);
                            currentStructureId = wc.StructureId;
                        }

                        projects.Add(project);
                        database.projectCounter++;
                    }
                }
            }

            // Grab Structure Projects
            /*
            string qry3 =
                @"
                    select p.*, ph.*
                    from project p, projecthistory ph
                    where active = @active
                        and p.projectdbid = ph.projectdbid
                        and ph.projecthistorydbid = 
                            (select max(projecthistorydbid)
                                from projecthistory
                                where projectdbid = p.projectdbid)
                ";*/
            /*
        string qry3 =
           @"
                select p.*, ph.*
                from project p, projecthistory ph
                where ph.fiscalYear = @fiscalYear
                    and p.projectdbid = ph.projectdbid
                    and p.deletedate is null
            ";
        SqlParameter[] prms3 = new SqlParameter[1];

        if (!region.Equals("any"))
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

            qry3 +=
                @"
                    and p.region = @regionComboCode
                    order by p.projectdbid, ph.projecthistorydbid desc
                ";
            prms3 = null;
            prms3 = new SqlParameter[2];
            prms3[1] = new SqlParameter("@regionComboCode", SqlDbType.VarChar);
            prms3[1].Value = regionComboCode;
        }
        else
        {
            qry3 +=
                @"
                    order by p.projectdbid, ph.projecthistorydbid desc
                ";
        }

        prms3[0] = new SqlParameter("@fiscalYear", SqlDbType.Int);
        prms3[0].Value = fiscalYear;
        DataTable dt3 = ExecuteSelect(qry3, prms3, wisamsConnection);

        if (dt3 != null && dt3.Rows.Count > 0)
        {
            int previousProjectDbId = 0;
            Project previousProject = new Project();

            foreach (DataRow dr in dt3.Rows)
            {
                int currentProjectDbId = Convert.ToInt32(dr["projectdbid"]);

                if (currentProjectDbId != previousProjectDbId)
                {
                    Project project = new Project();
                    previousProject = project;
                    projects.Add(project);
                    project.ProjectDbId = currentProjectDbId;
                    project.ProjectHistoryDbId = Convert.ToInt32(dr["projecthistorydbid"]);
                    project.Region = dr["region"].ToString();
                    project.CurrentFiscalYear = Convert.ToInt32(dr["currentfiscalyear"]);
                    project.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                    project.ProjectYear = Convert.ToInt32(dr["projectyear"]);
                    project.StructuresConcept = dr["structuresconcept"].ToString();
                    project.IsQuasicertified = Convert.ToBoolean(dr["quasicertified"]);
                    project.Status = (StructuresProgramType.ProjectStatus)dr["status"];
                    project.Description = dr["description"].ToString();
                    project.Notes = dr["notes"].ToString();
                    project.UserDbId = Convert.ToInt32(dr["userdbid"]);
                    project.UserAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                    project.UserActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                    project.UserFullName = "";

                    try
                    {
                        project.UserFullName = dr["userfullname"].ToString();
                    }
                    catch { }

                    try
                    {
                        project.RequestAdvancedCertification = Convert.ToBoolean(dr["requestadvancedcertification"]);
                        project.AdvancedCertificationDate = Convert.ToDateTime(dr["advancedcertificationdate"]);
                    }
                    catch { }

                    project.History = String.Format("{0}: {1} by {2}", project.UserActionDateTime, project.UserAction, project.UserFullName);
                    project.UserDbIds = new List<int>();
                    project.UserDbIds.Add(project.UserDbId);

                    // Fiips data
                    project.FosProjectId = "";

                    try
                    {
                        project.FosProjectId = dr["fosprojectid"].ToString();
                    }
                    catch { }

                    // Work Concepts
                    string qry4 =
                        @"
                            select *
                            from projectworkconcepthistory
                            where projecthistorydbid = @projectHistoryDbId
                            order by structureid
                        ";
                    SqlParameter[] prms4 = new SqlParameter[1];
                    prms4[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                    prms4[0].Value = project.ProjectHistoryDbId;
                    DataTable dt4 = ExecuteSelect(qry4, prms4, wisamsConnection);

                    if (dt4 != null && dt4.Rows.Count > 0)
                    {
                        foreach (DataRow dr4 in dt4.Rows)
                        {
                            WorkConcept wc = new WorkConcept();
                            project.WorkConcepts.Add(wc);
                            project.NumberOfStructures++;
                            wc.ProjectDbId = currentProjectDbId;
                            wc.StructureProjectFiscalYear = fiscalYear;
                            wc.Region = project.Region;
                            wc.StructuresConcept = project.StructuresConcept;

                            wc.WorkConceptDbId = Convert.ToInt32(dr4["workconceptdbid"]);
                            wc.ProjectWorkConceptHistoryDbId = Convert.ToInt32(dr4["projectworkconcepthistorydbid"]);
                            wc.WorkConceptCode = dr4["workconceptcode"].ToString();
                            wc.WorkConceptDescription = dr4["workconceptdescription"].ToString();
                            wc.CertifiedWorkConceptCode = dr4["certifiedworkconceptcode"].ToString();
                            wc.CertifiedWorkConceptDescription = dr4["certifiedworkconceptdescription"].ToString();
                            wc.StructureId = dr4["structureid"].ToString();
                            wc.PlannedStructureId = dr4["plannedstructureid"].ToString();
                            wc.CurrentFiscalYear = Convert.ToInt32(dr4["currentfiscalyear"]);
                            wc.FiscalYear = Convert.ToInt32(dr4["fiscalyear"]);
                            wc.ProjectYear = Convert.ToInt32(dr4["projectyear"]);

                            try
                            {
                                wc.PriorityScore = Convert.ToSingle(dr4["priorityscore"]);
                            }
                            catch { }

                            try
                            {
                                wc.Cost = Convert.ToInt32(dr4["cost"]);
                            }
                            catch { }

                            wc.FromEligibilityList = Convert.ToBoolean(dr4["fromeligibilitylist"]);
                            wc.FromFiips = Convert.ToBoolean(dr4["fromfiips"]);
                            wc.Evaluate = Convert.ToBoolean(dr4["evaluate"]);

                            try
                            {
                                wc.EarlierFiscalYear = Convert.ToInt32(dr4["earlierfiscalyear"]);
                                wc.LaterFiscalYear = Convert.ToInt32(dr4["laterfiscalyear"]);
                            }
                            catch { }

                            try
                            {
                                wc.ChangeJustifications = dr4["changejustifications"].ToString();
                            }
                            catch { }

                            try
                            {
                                wc.ChangeJustificationNotes = dr4["changenotes"].ToString();
                            }
                            catch { }

                            try
                            {
                                wc.IsQuasicertified = Convert.ToBoolean(dr4["quasicertified"]);
                            }
                            catch { }

                            wc.Status = (StructuresProgramType.WorkConceptStatus)dr4["status"];

                            try
                            {
                                wc.GeoLocation.latitude = Convert.ToSingle(dr4["latitude"]);
                                wc.GeoLocation.longitude = Convert.ToSingle(dr4["longitude"]);
                            }
                            catch { }
                        }
                    }
                }
                else
                {
                    int userDbId = Convert.ToInt32(dr["userdbid"]);
                    string userFullName = dr["userfullname"].ToString();
                    StructuresProgramType.ProjectUserAction userAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                    DateTime userActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                    previousProject.History += String.Format("\r\n\r\n{0}: {1} by {2}", userActionDateTime, userAction, userFullName);
                    previousProject.UserDbIds.Add(userDbId);
                }

                previousProjectDbId = currentProjectDbId;
            }
        }
        */
            return projects;
        }

        public List<string> GetStructuresByRegion(string region, bool stateOwned, bool localOwned, bool includeCStructures = false)
        {
            List<string> strIds = new List<string>();
            string qry = @"
                                select strc_id
                                from dot1stro.dt_strc
                                where dot_rgn_nb = :region
                                    and strc_tycd in ('B','P')
                                    and strc_sscd = '50'
                            ";

            if (includeCStructures)
            {
                qry = @"
                                select strc_id
                                from dot1stro.dt_strc
                                where dot_rgn_nb = :region
                                    and strc_tycd in ('B','P','C')
                                    and strc_sscd = '50'
                            ";
            }

            if (stateOwned && !localOwned)
            {
                qry += @" and (
                                strc_ownr_agcy_cd in ('10','15','16','20','44','45') 
                                or stmc_agcy_ty in ('10','15','16','20','44','45') 
                                or stim_agcy_tycd in ('10','15','16','20','44','45')
                              )
                            order by strc_id
                            ";
            }
            else if (localOwned && !stateOwned)
            {
                qry += @" and (
                                strc_ownr_agcy_cd in ('30','31','32','40','41','42','44','45','46','47','70','80','90')
                                or stmc_agcy_ty in ('30','31','32','40','41','42','44','45','46','47','70','80','90') 
                                or stim_agcy_tycd in ('30','31','32','40','41','42','44','45','46','47','70','80','90')
                              )   
                            order by strc_id
                        ";
            }
            else if (stateOwned && localOwned)
            {
                qry += @" and (
                                strc_ownr_agcy_cd in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stmc_agcy_ty in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stim_agcy_tycd in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                              )
                            order by strc_id
                        ";
            }

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("region", OracleDbType.Varchar2);
            prms[0].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strIds.Add(dr["STRC_ID"].ToString());
                }
            }

            return strIds;
        }

        public List<string> GetStructuresByRegionForGisDataPull(string region)
        {
            List<string> strIds = new List<string>();
            string qry =
                @"
                    select strc_id
                    from dot1stro.dt_strc
                    where dot_rgn_nb = :region
                        and strc_sscd in ('10', '50')
                        and (
                                strc_ownr_agcy_cd in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stmc_agcy_ty in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stim_agcy_tycd in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                              )
                    order by strc_id
                ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("region", OracleDbType.Varchar2);
            prms[0].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strIds.Add(dr["STRC_ID"].ToString());
                }
            }

            return strIds;
        }

        public List<string> GetStructuresByRegions(List<string> regions, bool stateOwned, bool localOwned, bool includeCStructures = false)
        {
            List<string> strIds = new List<string>();
            foreach (var region in regions)
            {
                strIds.AddRange(GetStructuresByRegion(region, stateOwned, localOwned, includeCStructures));
            }

            return strIds;
        }

        public List<UserAccount> GetTopUsers()
        {
            List<UserAccount> userAccounts = new List<UserAccount>();
            string qry =
                @"
                    select ul.userdbid, count(ul.userlogdbid) as loginCount, u.firstname, u.lastname, u.office
                    from userlog ul, users u
                    where ul.userdbid = u.userdbid
                        and activity = 'login'
                        and u.office in ('1-SW', '2-SE', '3-NE', '4-NC', '5-NW')
                    group by ul.userdbid, u.firstname, u.lastname, u.office
                    order by loginCount desc
                ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    UserAccount userAccount = new UserAccount();
                    userAccount.FirstName = dr["firstname"].ToString();
                    userAccount.LastName = dr["lastname"].ToString();
                    userAccount.Office = dr["office"].ToString();
                    userAccounts.Add(userAccount);
                }
            }

            return userAccounts;
        }

        public int GetUnapprovedWindowCurrentFyPlus()
        {
            int plus = 0;
            string qry =
                @"
                    select unapprovedwindowcurrentfyplus
                    from applicationsetting
                ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
            plus = Convert.ToInt32(dt.Rows[0]["unapprovedwindowcurrentfyplus"]);

            return plus;
        }

        public UserAccount GetUserAccount(int userDbId)
        {
            UserAccount userAccount = null;
            string qry =
                @"
                    select *
                    from users
                    where userdbid = @userDbId
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@userDbId", SqlDbType.Int);
            prms[0].Value = userDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                userAccount = GetUserAccount(dt.Rows[0]["username"].ToString(), dt.Rows[0]["userpassword"].ToString());
            }

            return userAccount;
        }

        public UserAccount GetUserAccount(string userName, string userPassword)
        {
            UserAccount userAccount = null;
            string qry =
                @"
                    select *
                    from users
                    where username = @userName
                        and userpassword = @userPassword
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@userName", SqlDbType.VarChar);
            prms[0].Value = userName;
            prms[1] = new SqlParameter("@userPassword", SqlDbType.NVarChar);
            prms[1].Value = userPassword;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                userAccount = new UserAccount();
                userAccount.UserDbId = Convert.ToInt32(dr["userdbid"]);
                userAccount.UserName = dr["username"].ToString();
                userAccount.FirstName = dr["firstname"].ToString();
                userAccount.LastName = dr["lastname"].ToString();
                userAccount.DotLogin = dr["dotlogin"].ToString();
                userAccount.Office = dr["office"].ToString();
                userAccount.EmailAddress = dr["emailaddress"].ToString();
                userAccount.PhoneNumber = dr["phonenumber"].ToString();
                userAccount.RoleTypes = new List<string>();

                if (userAccount.LastName.ToLower().Equals("barut") || userAccount.LastName.ToLower().Equals("becker"))
                {
                    userAccount.IsOmniscient = true;
                }

                string qry2 = @"
                        select *
                        from userrole
                        where userdbid = @userDbId
                        ";
                SqlParameter[] prms2 = new SqlParameter[1];
                prms2[0] = new SqlParameter("@userDbId", SqlDbType.Int);
                prms2[0].Value = userAccount.UserDbId;
                DataTable dt2 = ExecuteSelect(qry2, prms2, database.wisamsConnection);

                foreach (DataRow dr2 in dt2.Rows)
                {
                    string roleType = dr2["roletype"].ToString().ToUpper().Trim();
                    userAccount.RoleTypes.Add(roleType);

                    if (roleType.EndsWith("PRG"))
                    {
                        userAccount.IsRegionalProgrammer = true;
                    }
                    else if (roleType.Equals("NCREAD") || roleType.Equals("NEREAD")
                                || roleType.Equals("NWREAD") || roleType.Equals("SEREAD")
                                || roleType.Equals("SWREAD"))
                    {
                        userAccount.IsRegionalRead = true;
                    }
                    else if (roleType.EndsWith("MNT"))
                    {
                        userAccount.IsRegionalMaintenanceEngineer = true;
                    }
                    else if (roleType.Equals("SUPREAD"))
                    {
                        userAccount.IsSuperRead = true;
                    }
                    else if (roleType.Equals("SUPUSR"))
                    {
                        userAccount.IsSuperUser = true;
                    }
                    else if (roleType.Equals("ADM"))
                    {
                        userAccount.IsAdministrator = true;
                    }
                    else if (roleType.Equals("PRELIAI"))
                    {
                        userAccount.IsPrecertificationLiaison = true;
                    }
                    else if (roleType.Equals("PRESUP"))
                    {
                        userAccount.IsPrecertificationSupervisor = true;
                    }
                    else if (roleType.Equals("CERTLIAI"))
                    {
                        userAccount.IsCertificationLiaison = true;
                    }
                    else if (roleType.Equals("CERTSUP"))
                    {
                        userAccount.IsCertificationSupervisor = true;
                    }
                }
            }

            return userAccount;
        }

        public List<int> GetUserIdsForAProject(int projectDbId)
        {
            List<int> userIds = new List<int>();
            string qry =
                @"
                    select distinct userdbid as userid
                    from projecthistory
                    where projectdbid = @projectDbId
                    union
                    select distinct precertificationliaisonuserdbid as userid
                    from projecthistory
                    where projectdbid = @projectDbId
                        and precertificationliaisonuserdbid is not null
                    union
                    select distinct certificationliaisonuserdbid as userid
                    from projecthistory
                    where projectdbid = @projectDbId
                        and certificationliaisonuserdbid is not null
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    userIds.Add(Convert.ToInt32(dr["userid"]));
                }
            }

            return userIds;
        }
        #endregion

        public void LogUserActivity(int userDbId, string activity)
        {
            string qry =
                @"
                    insert into userlog(userdbid, activity, activitydatetime)
                    values (@userDbId, @activity, @activityDateTime)
                ";
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@userDbId", SqlDbType.Int);
            prms[0].Value = userDbId;
            prms[1] = new SqlParameter("@activity", SqlDbType.VarChar);
            prms[1].Value = activity;
            prms[2] = new SqlParameter("@activityDateTime", SqlDbType.DateTime);
            prms[2].Value = DateTime.Now;
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        public bool OpenDatabaseConnection(string dataSource)
        {
            bool successful = true;

            switch (dataSource.ToUpper())
            {
                case "WISAMS":
                    if (database.wisamsConnection == null)
                    {
                        database.wisamsConnection = new SqlConnection(database.wisamsConnectionString);
                    }

                    if (database.wisamsConnection.State == ConnectionState.Closed || database.wisamsConnection.State == ConnectionState.Broken)
                    {
                        try
                        {
                    if (database.wisamsConnection.State == ConnectionState.Closed || database.wisamsConnection.State == ConnectionState.Broken)
                            database.wisamsConnection.Open();
                            database.UnapprovedWindowCurrentFyPlus = GetUnapprovedWindowCurrentFyPlus();
                            Database.precertificationLiaisons = GetPrecertLiaisons();
                            Database.precertificationLiaisonsEmails = GetPrecertificationLiaisonsEmails();
                            Database.certificationLiaisons = GetCertLiaisons();
                            Database.certificationsLiaisonsEmails = GetCertificationLiaisonsEmails();
                            Database.certificationSupervisors = GetCertSups();
                            Database.certificationSupervisorsEmails = GetCertificationSupervisorsEmails();
                            database.allWorkConcepts = GetAllWorkConcepts();
                        }
                        catch (Exception ex)
                        {
                            successful = false;
                        }
                    }

                    break;
                case "HSI":
                    if (database.hsiConnection == null)
                    {
                        database.hsiConnection = new OracleConnection(database.hsiConnectionString);
                    }

                    if (database.hsiConnection.State == ConnectionState.Closed || database.hsiConnection.State == ConnectionState.Broken)
                    {
                        try
                        {
                            database.hsiConnection.Open();
                        }
                        catch (Exception ex)
                        {
                            successful = false;
                        }
                    }

                    break;
            }

            return successful;
        }

        public void UpdateTimeWindows()
        {
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        select projectworkconcepthistorydbid, workconceptcode, certifiedworkconceptcode
                        from projectworkconcepthistory
                        where workconceptcode <> 'PR'
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string qry2 =
                            @"
                                update projectworkconcepthistory
                                set earlierfiscalyear = @earlierFiscalYear,
                                    laterfiscalyear = @laterFiscalYear
                                where projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                            ";
                        string workConceptCode = dr["workconceptcode"].ToString();
                        /*
                        WorkConcept wc = allWorkConcepts.Where(w => w.WorkConceptCode)
                        SqlParameter[] prms = new SqlParameter[16];
                        prms[0] = new SqlParameter("@earlierFiscalYear", SqlDbType.Int);
                        prms[0].Value = projectHistoryDbId;
                        prms[1] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
                        prms[1].Value = wc.WorkConceptDbId;
                        prms[2] = new SqlParameter("@workConceptCode", SqlDbType.VarChar);
                        prms[2].Value = wc.WorkConceptCode;
                        prms[3] = new SqlParameter("@workConceptDescription", SqlDbType.VarChar);
                        prms[3].Value = wc.WorkConceptDescription;*/
                    }
                }
            }
        }

        public bool IsProjectIdInFiips(string projectId)
        {
            bool inFiips = false;
            string qry = @"
                            select fos_proj_id
                            from pmic
                            where fos_proj_id = @projectId
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectId", SqlDbType.VarChar);
            prms[0].Value = projectId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                inFiips = true;
            }

            return inFiips;
        }

        public bool IsStructureInHsi(string structureId, UserAccount userAccount = null)
        {
            bool inHsi = false;
            string qry = @"
                            select strc_id, strc_sscd
                            from dot1stro.dt_strc
                            where strc_sscd in ('50', '70')
                                and strc_id = :structureId
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("structureId", OracleDbType.Varchar2);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                inHsi = true;
            }

            if (userAccount != null)
            {
                qry =
                    @"
                            select *
                            from structure
                            where structureid = @structureId
                                and regionnumber = @regionNumber
                        ";
                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
                p[0].Value = structureId;
                p[1] = new SqlParameter("@regionNumber", SqlDbType.Int);
                p[1].Value = Convert.ToInt32(userAccount.Office.Substring(0, 1));
                DataTable d = ExecuteSelect(qry, p, database.wisamsConnection);

                if (d == null || d.Rows.Count == 0)
                {
                    inHsi = false;
                }
            }

            return inHsi;
        }

        public void GetMainSpanInfo(WiSamEntities.Structure str)
        {
            string strId = str.StructureId;
            string qry =
                @"
                    select mainspan.strc_id,
                        spanconfig.span_cnfg_tydc,
                        spanconfig.bspn_cnfg_ty,
                        spanmaterial.span_matl_desc,
                        spanmaterial.bspn_matl_cd
                    from dot1stro.dt_brdg_span mainspan,
				        dot1stro.dt_brdg_span_cnfg spanconfig,
                        dot1stro.dt_brdg_span_matl spanmaterial
                    where mainspan.strc_id = :strId
	                    and mainspan.main_span_fl = 'Y'
	                    and mainspan.bspn_cnfg_ty = spanconfig.bspn_cnfg_ty(+)
                        and mainspan.bspn_matl_cd = spanmaterial.bspn_matl_cd(+)
                ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);



            if (dt != null && dt.Rows.Count > 0)
            {
                str.StructureType = dt.Rows[0]["SPAN_CNFG_TYDC"].ToString();
                str.StructureTypeCode = dt.Rows[0]["BSPN_CNFG_TY"].ToString();
                str.MainSpanMaterial = dt.Rows[0]["SPAN_MATL_DESC"].ToString();
                str.MainSpanMaterialCode = dt.Rows[0]["BSPN_MATL_CD"].ToString();
            }
        }

        public List<WiSamEntities.Element> GetLastInspectionElements(string strId)
        {
            List<WiSamEntities.Element> elems = null;
            string qry = @"
                                select insp.strc_id, insp.stin_id, lastInsp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    (select strc_id, max(stin_dt) stin_dt, max(stin_id) maxstinid 
                                        from dot1stro.dt_strc_insp where stin_id in (select stin_id from dot1stro.dt_isel_item) 
                                        group by strc_id) lastInsp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.strc_id = lastInsp.strc_id 
                                    and insp.stin_dt = lastInsp.stin_dt 
                                    and insp.stin_id = elem.stin_id 
                                    and insp.stin_tycd not in ('ED')
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                                    and dt_isel_clsn.isel_clsn_id not in ('E4','E3')
                                order by elem.isel_tyid
                            ";

            // took out qualifier - and elem.stin_id = lastInsp.maxstinid
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                elems = new List<WiSamEntities.Element>();

                foreach (DataRow row in dt.Rows)
                {
                    WiSamEntities.Element elem = new WiSamEntities.Element(Convert.ToInt32(row["ISEL_TYID"]));
                    elem.ElemName = row["ISEL_NM"].ToString();

                    try
                    {
                        elem.Cs1Quantity = Convert.ToInt32(row["FST_COND_ST_QTY"]);
                    }
                    catch
                    {
                        elem.Cs1Quantity = 0;
                    }

                    try
                    {
                        elem.Cs2Quantity = Convert.ToInt32(row["SCND_COND_ST_QTY"]);
                    }
                    catch
                    {
                        elem.Cs2Quantity = 0;
                    }

                    try
                    {
                        elem.Cs3Quantity = Convert.ToInt32(row["THRD_COND_ST_QTY"]);
                    }
                    catch
                    {
                        elem.Cs3Quantity = 0;
                    }

                    try
                    {
                        elem.Cs4Quantity = Convert.ToInt32(row["FRTH_COND_ST_QTY"]);
                    }
                    catch
                    {
                        elem.Cs4Quantity = 0;
                    }

                    try
                    {
                        elem.TotalQuantity = elem.Cs1Quantity + elem.Cs2Quantity + elem.Cs3Quantity + elem.Cs4Quantity;
                    }
                    catch { }

                    elem.ElementClassificationCode = row["ISEL_CLSN_ID"].ToString();
                    elem.StructureId = strId;
                    elem.InspectionDate = Convert.ToDateTime(row["STIN_DT"]);
                    elem.DeteriorationYear = 0;
                    elem.IselItemId = Convert.ToInt32(row["ISEL_ITEM_ID"]);

                    if (!String.IsNullOrEmpty(row["MAIN_ISEL_ID"].ToString()))
                    {
                        elem.MainIselId = Convert.ToInt32(row["MAIN_ISEL_ID"]);
                    }

                    elems.Add(elem);
                }

                // Assign parent element number to elements with a parent
                foreach (WiSamEntities.Element elem in elems.Where(e => e.MainIselId != 0).ToList())
                {
                    elem.ParentElemNum = elems.Where(e => e.IselItemId == elem.MainIselId).First().ElemNum;
                }
            }

            return elems;
        }

        public WiSamEntities.NbiRating GetLastNbiRating(string strId)
        {
            WiSamEntities.NbiRating nr = null;
            string qry = @"
                                select StrcInsp.strc_id,
                                        stin_dt,
                                        stin_deck_cond_rtg,
                                        stin_spsr_cond_rtg,
                                        stin_sbsr_cond_rtg,
                                        stin_culv_rtg_cd, 
                                        stin_wtrw_rtg_cd
                                from dot1stro.dt_strc_insp StrcInsp
                                where StrcInsp.strc_id = :strId
                                    and StrcInsp.stin_dt = (select max(stin_dt)
                                                                from dot1stro.dt_strc_insp MaxPrev
                                                                where StrcInsp.strc_id = MaxPrev.strc_id)
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                nr = new WiSamEntities.NbiRating();
                DataRow dr = dt.Rows[0];
                nr.StructureId = dr["STRC_ID"].ToString();
                nr.InspectionDate = DateTime.Parse(dr["STIN_DT"].ToString());

                if (!String.IsNullOrEmpty(dr["STIN_DECK_COND_RTG"].ToString()))
                {
                    nr.DeckRating = dr["STIN_DECK_COND_RTG"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["STIN_SPSR_COND_RTG"].ToString()))
                {
                    nr.SuperstructureRating = dr["STIN_SPSR_COND_RTG"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["STIN_SBSR_COND_RTG"].ToString()))
                {
                    nr.SubstructureRating = dr["STIN_SBSR_COND_RTG"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["STIN_CULV_RTG_CD"].ToString()))
                {
                    nr.CulvertRating = dr["STIN_CULV_RTG_CD"].ToString();
                }

                if (!String.IsNullOrEmpty(dr["STIN_WTRW_RTG_CD"].ToString()))
                {
                    nr.WaterwayRating = dr["STIN_WTRW_RTG_CD"].ToString();
                }

                int rating;
                /*
                if (Int32.TryParse(nr.DeckRating, out rating))
                {
                    nr.DeckDeteriorationYear = GetNbiDeteriorationYear(WiSam.Entity.Code.NbiDeck, rating);
                    nr.DeckRatingVal = Convert.ToDouble(nr.DeckRating);
                }

                if (Int32.TryParse(nr.SuperstructureRating, out rating))
                {
                    nr.SuperstructureDeteriorationYear = GetNbiDeteriorationYear(WiSam.Entity.Code.NbiSuperstructure, rating);
                    nr.SuperstructureRatingVal = Convert.ToDouble(nr.SuperstructureRating);
                }

                if (Int32.TryParse(nr.SubstructureRating, out rating))
                {
                    nr.SubstructureDeteriorationYear = GetNbiDeteriorationYear(WiSam.Entity.Code.NbiSubstructure, rating);
                    nr.SubstructureRatingVal = Convert.ToDouble(nr.SubstructureRating);
                }

                if (Int32.TryParse(nr.CulvertRating, out rating))
                {
                    nr.CulvertDeteriorationYear = GetNbiDeteriorationYear(WiSam.Entity.Code.NbiCulvert, rating);
                    nr.CulvertRatingVal = Convert.ToDouble(nr.CulvertRating);
                }
                */
                if (Int32.TryParse(nr.WaterwayRating, out rating))
                {
                    nr.WaterwayRatingVal = Convert.ToDouble(nr.WaterwayRating);
                }
            }

            return nr;
        }

        public void GetSpanInfo(WiSamEntities.Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select strc_id, bspn_nb, bspn_ln, main_span_fl
                                from dot1stro.dt_brdg_span
                                where strc_id = :strId
                                order by bspn_nb asc
                            ";

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, database.hsiConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                str.NumSpans = 0;
                str.TotalLengthSpans = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    str.NumSpans++;

                    try
                    {
                        str.TotalLengthSpans += Convert.ToDouble(dr["BSPN_LN"]);
                    }
                    catch { }
                }

                Math.Round(str.TotalLengthSpans, 2);
            }
        }

        public GMarkerGoogleType GetMapMarkerType(string workConceptCode)
        {
            GMarkerGoogleType markerType = (GMarkerGoogleType)26;
            string qry = @"
                            select mapmarkertype
                            from workaction
                            where workactioncode = @workConceptCode
                        ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workConceptCode", SqlDbType.VarChar);
            prms[0].Value = workConceptCode;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                markerType = (GMarkerGoogleType)Convert.ToInt32(dt.Rows[0]["mapmarkertype"]);
            }

            return markerType;
        }

        public List<string> FindStructuresNearMe(string id, StructuresProgramType.ObjectType objectType, float midLatitude, float midLongitude, float radius, string structureTypes = "'B','P'", bool stateOwned = true, bool localOwned = false, string region = "any")
        {
            List<string> structuresFound = new List<string>();

            if (midLatitude != 0 && midLongitude != 0)
            {
                string qry =
                    @"
                        SELECT z.structureid, z.lat, z.lngt, z.regionnumber, z.regioncode,
                            z.latdecimal, z.lngtdecimal,
                            p.distance_unit
                                        * DEGREES(ACOS(COS(RADIANS(p.latpoint))
                                        * COS(RADIANS(z.latdecimal))
                                        * COS(RADIANS(p.longpoint) - RADIANS(z.lngtdecimal))
                                        + SIN(RADIANS(p.latpoint))
                                        * SIN(RADIANS(z.latdecimal)))) AS distance_in_mi
                        FROM structure AS z
                        JOIN (   /* these are the query parameters */
                            SELECT  @latPoint  AS latpoint,  @longPoint AS longpoint,
                                    @radius AS radius,      69 AS distance_unit
                        ) AS p ON 1=1
                        WHERE z.latdecimal
                            BETWEEN p.latpoint  - (p.radius / p.distance_unit)
                                AND p.latpoint  + (p.radius / p.distance_unit)
                            AND z.lngtdecimal
                                BETWEEN p.longpoint - (p.radius / (p.distance_unit * COS(RADIANS(p.latpoint))))
                                    AND p.longpoint + (p.radius / (p.distance_unit * COS(RADIANS(p.latpoint))))
                            AND z.structuresscode = '50'
                    ";
                //AND z.structureid <> @structureId
                //ORDER BY distance_in_mi
                qry += " and z.structuretypecode in (" + structureTypes + ")";

                if (stateOwned && !localOwned)
                {
                    qry += @" and (
                                owneragencycode in ('10','15','16','20','44','45') 
                                or stmcagencycode in ('10','15','16','20','44','45') 
                                or stimagencycode in ('10','15','16','20','44','45')
                              )
                
                            ";
                }
                else if (localOwned && !stateOwned)
                {
                    qry += @" and (
                                owneragencycode in ('30','31','32','40','41','42','44','45','46','47','70','80','90')
                                or stmcagencycode in ('30','31','32','40','41','42','44','45','46','47','70','80','90') 
                                or stimagencycode in ('30','31','32','40','41','42','44','45','46','47','70','80','90')
                              )   
                            
                            ";
                }
                else if (stateOwned && localOwned)
                {
                    qry += @" and (
                                owneragencycode in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stmcagencycode in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stimagencycode in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                              )
               
                            ";
                }

                SqlParameter[] prms = new SqlParameter[4];

                if (!region.Equals("any"))
                {
                    prms = new SqlParameter[5];
                }

                prms[0] = new SqlParameter("@latPoint", SqlDbType.Float);
                prms[0].Value = midLatitude;
                prms[1] = new SqlParameter("@longPoint", SqlDbType.Float);
                prms[1].Value = midLongitude;
                prms[2] = new SqlParameter("@radius", SqlDbType.Float);
                prms[2].Value = radius;
                prms[3] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[3].Value = id;

                if (!region.Equals("any"))
                {
                    qry += @"
                            and regionnumber = @region
                        ";
                    prms[4] = new SqlParameter("@region", SqlDbType.Int);
                    prms[4].Value = Convert.ToInt32(region.ToString());
                }

                DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        structuresFound.Add(dr["structureid"].ToString().Trim());
                    }
                }
            }

            return structuresFound;
        }

        public List<Structure> FindNearMeStructures(float latPoint, float longPoint, float radius, string structureId, bool stateOwned, bool localOwned, string structureTypes, string region = "any")
        {
            List<Structure> structures = new List<Structure>();
            string qry = @"
                                SELECT z.structureid, z.lat, z.lngt, z.regionnumber, z.regioncode,
                                    z.latdecimal, z.lngtdecimal,
                                    p.distance_unit
                                             * DEGREES(ACOS(COS(RADIANS(p.latpoint))
                                             * COS(RADIANS(z.latdecimal))
                                             * COS(RADIANS(p.longpoint) - RADIANS(z.lngtdecimal))
                                             + SIN(RADIANS(p.latpoint))
                                             * SIN(RADIANS(z.latdecimal)))) AS distance_in_mi
                                  FROM structure AS z
                                  JOIN (   /* these are the query parameters */
                                        SELECT  @latPoint  AS latpoint,  @longPoint AS longpoint,
                                                @radius AS radius,      69 AS distance_unit
                                    ) AS p ON 1=1
                                  WHERE z.latdecimal
                                     BETWEEN p.latpoint  - (p.radius / p.distance_unit)
                                         AND p.latpoint  + (p.radius / p.distance_unit)
                                    AND z.lngtdecimal
                                     BETWEEN p.longpoint - (p.radius / (p.distance_unit * COS(RADIANS(p.latpoint))))
                                         AND p.longpoint + (p.radius / (p.distance_unit * COS(RADIANS(p.latpoint))))
                                    AND z.structuresscode = '50'
                            ";
            //AND z.structureid <> @structureId
            //ORDER BY distance_in_mi
            qry += " and z.structuretypecode in (" + structureTypes + ")";

            if (stateOwned && !localOwned)
            {
                qry += @" and (
                                owneragencycode in ('10','15','16','20','44','45') 
                                or stmcagencycode in ('10','15','16','20','44','45') 
                                or stimagencycode in ('10','15','16','20','44','45')
                              )
                          
                            ";
            }
            else if (localOwned && !stateOwned)
            {
                qry += @" and (
                                owneragencycode in ('30','31','32','40','41','42','44','45','46','47','70','80','90')
                                or stmcagencycode in ('30','31','32','40','41','42','44','45','46','47','70','80','90') 
                                or stimagencycode in ('30','31','32','40','41','42','44','45','46','47','70','80','90')
                              )   
                            
                            ";
            }
            else if (stateOwned && localOwned)
            {
                qry += @" and (
                                owneragencycode in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stmcagencycode in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                or stimagencycode in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                              )
                            
                            ";
            }

            SqlParameter[] prms = new SqlParameter[4];

            if (!region.Equals("any"))
            {
                prms = new SqlParameter[5];
            }

            prms[0] = new SqlParameter("@latPoint", SqlDbType.Float);
            prms[0].Value = latPoint;
            prms[1] = new SqlParameter("@longPoint", SqlDbType.Float);
            prms[1].Value = longPoint;
            prms[2] = new SqlParameter("@radius", SqlDbType.Float);
            prms[2].Value = radius;
            prms[3] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[3].Value = structureId;

            if (!region.Equals("any"))
            {
                qry += @"
                            and regionnumber = @region
                        ";
                prms[4] = new SqlParameter("@region", SqlDbType.Int);
                prms[4].Value = Convert.ToInt32(region.ToString());
            }

            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Structure structure = new Structure();
                    structure.StructureId = dr["structureid"].ToString();
                    structure.GeoLocation = new GeoLocation();
                    structure.GeoLocation.HsiLatitude = dr["lat"].ToString();
                    structure.GeoLocation.HsiLongitude = dr["lngt"].ToString();
                    structure.GeoLocation.LatitudeDecimal = Convert.ToSingle(dr["latdecimal"]);
                    structure.GeoLocation.LongitudeDecimal = Convert.ToSingle(dr["lngtdecimal"]);
                    structure.Region = dr["regionnumber"].ToString() + "-" + dr["regioncode"].ToString();
                    structures.Add(structure);
                }
            }

            return structures;
        }

        public List<Structure> FindNearMeStructures(string structureId, float minLat, float maxLat, float minLng, float maxLng)
        {
            List<Structure> structures = new List<Structure>();
            string qry = @"
                                select *
                                from structure
                                where
                                    (latdecimal between @minLat and @maxLat)
                                    and (lngtdecimal between @minLng and @maxLng)  
                            ";
            /*
            string qry = @"
                                select structurei, latitude, longitude, lat__decim, long__deci
                                from structuregis
                                where
                                    (lat__decim between @minLat and @maxLat)
                                    and (long__deci between @minLng and @maxLng)  
                            ";*/
            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@minLat", SqlDbType.Float);
            prms[0].Value = minLat;
            prms[1] = new SqlParameter("@maxLat", SqlDbType.Float);
            prms[1].Value = maxLat;
            prms[2] = new SqlParameter("@minLng", SqlDbType.Float);
            prms[2].Value = minLng;
            prms[3] = new SqlParameter("@maxLng", SqlDbType.Float);
            prms[3].Value = maxLng;
            //prms[4] = new SqlParameter("@structureId", SqlDbType.VarChar);
            //prms[4].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Structure structure = new Structure();
                    structure.StructureId = dr["structureid"].ToString();
                    structure.GeoLocation = new GeoLocation();
                    structure.GeoLocation.HsiLatitude = dr["lat"].ToString();
                    structure.GeoLocation.HsiLongitude = dr["lngt"].ToString();
                    structure.GeoLocation.LatitudeDecimal = Convert.ToSingle(dr["latdecimal"]);
                    structure.GeoLocation.LongitudeDecimal = Convert.ToSingle(dr["lngtdecimal"]);
                    structures.Add(structure);
                }
            }

            return structures;
        }

        public void UpdateOldEvs()
        {
            string qry =
                @"
                    select *
                    FROM ProjectWorkConceptHistory
                    where projecthistorydbid = 5149 
                ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                bool fromProposedList = Convert.ToBoolean(dr["fromproposedlist"]);
                string structureId = dr["structureid"].ToString();
                DateTime dateTime = new DateTime(2020, 10, 28);

                if (!fromProposedList)
                {
                    string qry2 =
                        @"
                            insert into proposedlist
                                (structureid, region, regionnumber, workconceptcode, workconceptdesc,
                                fiscalyear, reasoncategory, notes, proposedbyuserdbid, proposedbyuserfullname,
                                proposeddate, active)
                            values
                                (@structureId, '2-SE', 2, 'EV', 'EVALUATE FOR SECONDARY WORK CONCEPTS',
                                2024, 'OTHER', 'Converted from old evaluate work type', 32, 'Jim Liptack',
                                @dateTime, 1)
                        ";
                    SqlParameter[] prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
                    prms[0].Value = structureId;
                    prms[1] = new SqlParameter("@dateTime", SqlDbType.Date);
                    prms[1].Value = dateTime;
                    ExecuteInsertUpdateDelete(qry2, prms, database.wisamsConnection);
                }
            }
        }

        public void UpdateLatLong()
        {
            string qry = @"
                            select strc_id, lat, lngt, 
                                dot_rgn_nb, 
                                stim_agcy_tycd,
                                stmc_agcy_ty, 
                                strc_ownr_agcy_cd,
                                strc_tycd, 
                                strc_sscd
                            from dot1stro.dt_strc
                          ";
            DataTable dt = ExecuteSelect(qry, database.hsiConnection);

            foreach (DataRow dr in dt.Rows)
            {
                string structureId = dr["strc_id"].ToString();
                float lat = 0;
                float lngt = 0;
                float latDecimal = 0;
                float lngtDecimal = 0;
                int regionNumber = 0;
                string regionCode = "0";
                int stimAgencyCode = 0;
                int stmcAgencyCode = 0;
                int ownerAgencyCode = 0;
                string structureTypeCode = "0";
                string structureSsCode = "0";

                try
                {
                    structureSsCode = dr["strc_sscd"].ToString();
                }
                catch { }

                try
                {
                    structureTypeCode = dr["strc_tycd"].ToString();
                }
                catch { }

                try
                {
                    ownerAgencyCode = Convert.ToInt32(dr["strc_ownr_agcy_cd"]);
                }
                catch { }

                try
                {
                    stmcAgencyCode = Convert.ToInt32(dr["stmc_agcy_ty"]);
                }
                catch { }

                try
                {
                    stimAgencyCode = Convert.ToInt32(dr["stim_agcy_tycd"]);
                }
                catch { }

                try
                {
                    regionNumber = Convert.ToInt32(dr["dot_rgn_nb"]);
                    switch (regionNumber)
                    {
                        case 1:
                            regionCode = "SW";
                            break;
                        case 2:
                            regionCode = "SE";
                            break;
                        case 3:
                            regionCode = "NE";
                            break;
                        case 4:
                            regionCode = "NC";
                            break;
                        case 5:
                            regionCode = "NW";
                            break;
                    }
                }
                catch { }

                try
                {
                    lat = float.Parse(dr["lat"].ToString());
                }
                catch { }

                try
                {
                    lngt = float.Parse(dr["lngt"].ToString());
                }
                catch { }

                try
                {
                    latDecimal = Convert.ToSingle(repo.ConvertDegreesMinutesSecondsToDecimalDegrees(lat.ToString()));
                }
                catch { }

                try
                {
                    lngtDecimal = Convert.ToSingle(repo.ConvertDegreesMinutesSecondsToDecimalDegrees(lngt.ToString()));
                }
                catch { }

                string qry2 = @"
                                    select *
                                    from structure
                                    where structureid = @structureId
                                ";
                SqlParameter[] prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[0].Value = structureId;
                DataTable dt2 = ExecuteSelect(qry2, prms, database.wisamsConnection);

                if (dt2.Rows.Count > 0)
                {
                    string qry3 = @"
                                        update structure
                                        set lat = @lat,
                                            lngt = @lngt,
                                            latDecimal = @latDecimal,
                                            lngtDecimal = @lngtDecimal,
                                            regionnumber = @regionNumber,
                                            regioncode = @regionCode,
                                            structuretypecode = @structureTypeCode,
                                            structuresscode = @structureSsCode,
                                            owneragencycode = @ownerAgencyCode,
                                            stmcagencycode = @stmcAgencycode,
                                            stimagencycode = @stimAgencycode
                                        where structureid = @structureId
                                    ";
                    SqlParameter[] prms3 = new SqlParameter[12];
                    prms3[0] = new SqlParameter("@lat", SqlDbType.Float);
                    prms3[0].Value = lat;
                    prms3[1] = new SqlParameter("@lngt", SqlDbType.Float);
                    prms3[1].Value = lngt;
                    prms3[2] = new SqlParameter("@latDecimal", SqlDbType.Float);
                    prms3[2].Value = latDecimal;
                    prms3[3] = new SqlParameter("@lngtDecimal", SqlDbType.Float);
                    prms3[3].Value = -lngtDecimal;
                    prms3[4] = new SqlParameter("@regionNumber", SqlDbType.Int);
                    prms3[4].Value = regionNumber;
                    prms3[5] = new SqlParameter("@regionCode", SqlDbType.VarChar);
                    prms3[5].Value = regionCode;
                    prms3[6] = new SqlParameter("@structureTypeCode", SqlDbType.VarChar);
                    prms3[6].Value = structureTypeCode;
                    prms3[7] = new SqlParameter("@structureSsCode", SqlDbType.VarChar);
                    prms3[7].Value = structureSsCode;
                    prms3[8] = new SqlParameter("@ownerAgencyCode", SqlDbType.Int);
                    prms3[8].Value = ownerAgencyCode;
                    prms3[9] = new SqlParameter("@stmcAgencyCode", SqlDbType.Int);
                    prms3[9].Value = stmcAgencyCode;
                    prms3[10] = new SqlParameter("@stimAgencyCode", SqlDbType.Int);
                    prms3[10].Value = stimAgencyCode;
                    prms3[11] = new SqlParameter("@structureId", SqlDbType.VarChar);
                    prms3[11].Value = structureId;
                    ExecuteInsertUpdateDelete(qry3, prms3, database.wisamsConnection);
                }
                else
                {
                    string qry3 = @"
                                        insert into structure(structureid, lat, lngt, latDecimal, lngtDecimal)
                                        values(@structureId, @lat, @lngt, @latDecimal, @lngtDecimal)
                                    ";
                    SqlParameter[] prms3 = new SqlParameter[5];
                    prms3[1] = new SqlParameter("@lat", SqlDbType.Float);
                    prms3[1].Value = lat;
                    prms3[2] = new SqlParameter("@lngt", SqlDbType.Float);
                    prms3[2].Value = lngt;
                    prms3[3] = new SqlParameter("@latDecimal", SqlDbType.Float);
                    prms3[3].Value = latDecimal;
                    prms3[4] = new SqlParameter("@lngtDecimal", SqlDbType.Float);
                    prms3[4].Value = -lngtDecimal;
                    prms3[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
                    prms3[0].Value = structureId;
                    ExecuteInsertUpdateDelete(qry3, prms3, database.wisamsConnection);
                }
            }
        }

        public List<Project> GetProjectsInFiips()
        {
            return database.fiipsProjects;
        }
        public List<WorkConcept> GetFiipsWorkConcepts()
        {
            return database.fiipsWorkConcepts;
        }
        public List<Project> GetProjectsInSct()
        {
            return database.structureProjects;
        }

        public string GetCertificationRootFolder()
        {
            return Database.certificationRootFolder;
        }

        public string GetCertificationDirectory()
        {
            return Database.certificationDirectory;
        }

        public string GetBosCdTemplate()
        {
            return Database.bosCdTemplate;
        }

        public string GetTempDirectory()
        {
            return Database.tempDirectory;
        }

        public string GetBosCdSignature()
        {
            return Database.bosCdSignature;
        }

        public string GetApplicationMode()
        {
            return Database.applicationMode;
        }

        public string GetWisamsExecutablePath()
        {
            return Database.wisamsExecutablePath;
        }

        public string GetFiipsQueryToolExecutablePath()
        {
            return Database.fiipsQueryToolExecutablePath;
        }

        public bool EnableHsis()
        {
            return Database.enableHsis;
        }

        public bool EnableBox()
        {
            return Database.enableBox;
        }

        public List<UserAccount> GetPrecertificationLiaisons()
        {
            return Database.precertificationLiaisons;
        }

        public string GetPrecertificationLiaisonsEmails()
        {
            string emails = "";
            int counter = 0;

            foreach (var account in Database.precertificationLiaisons)
            {
                if (counter == 0)
                {
                    emails = account.EmailAddress;
                }
                else
                {
                    emails += "," + account.EmailAddress;
                }

                counter++;
            }

            return emails;
        }

        public string GetPrecertificationLiaisonsEmailAddresses()
        {
            return Database.precertificationLiaisonsEmails;
        }

        public List<UserAccount> GetCertificationLiaisons()
        {
            return Database.certificationLiaisons;
        }

        public string GetCertificationLiaisonsEmails()
        {
            string emails = "";
            int counter = 0;

            foreach (var account in Database.certificationLiaisons)
            {
                if (counter == 0)
                {
                    emails = account.EmailAddress;
                }
                else
                {
                    emails += "," + account.EmailAddress;
                }

                counter++;
            }

            return emails;
        }

        public string GetCertificationLiaisonsEmailAddresses()
        {
            return Database.certificationsLiaisonsEmails;
        }

        public string GetCertificationLiaison(int projectDbId)
        {
            string certificationLiaison = "";
            string qry =
                @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and (certificationliaisonuserfullname is not null or projectUserAction in (40, 110))
                    order by useractiondatetime desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                certificationLiaison = dr["certificationliaisonuserfullname"].ToString();
            }
            else
            {
                qry =
                @"
                    select *
                    from projecthistory
                    where projectdbid = @projectDbID
                        and (useraction = 110)
                    order by useractiondatetime desc
                ";
                prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = projectDbId;
                dt = ExecuteSelect(qry, prms, database.wisamsConnection);
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    certificationLiaison = dr["userfullname"].ToString();
                }
            }
            return certificationLiaison;
        }

        public string GetCertificationSupervisorsEmails()
        {
            string emails = "";
            int counter = 0;

            foreach (var account in Database.certificationSupervisors)
            {
                if (counter == 0)
                {
                    emails = account.EmailAddress;
                }
                else
                {
                    emails += "," + account.EmailAddress;
                }

                counter++;
            }

            return emails;
        }

        public string GetCertificationSupervisorsEmailAddresses()
        {
            return Database.certificationSupervisorsEmails;
        }

        public List<WorkConcept> GetProjectWorkConceptHistory(string structureId, int workConceptDbId, Project project)
        {
            List<WorkConcept> wcs = new List<WorkConcept>();
            string qry =
                @"
                    select ph.userfullname, ph.precertificationliaisonuserfullname, ph.certificationliaisonuserfullname, ph.status as projecthistorystatus, ph.useraction, ph.useractiondatetime, 
                    pwch.status as workconceptstatus, pwch.*
                    from projecthistory ph, projectworkconcepthistory pwch
                    where workconceptdbid = @workConceptDbId
                        and structureid = @structureId
                        and ph.projecthistorydbid = pwch.projecthistorydbid
                        and ph.projecthistorydbid in
                            (select projecthistorydbid
                                from projecthistory
                                where projectdbid = @projectDbId)
                        
                    order by projectworkconcepthistorydbid desc
                ";
            //and (precertificationdecisiondatetime is not null)
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
            prms[0].Value = workConceptDbId;
            prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[1].Value = structureId;
            prms[2] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[2].Value = project.ProjectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var wc = new WorkConcept();
                    wc.ProjectDbId = project.ProjectDbId;
                    wc.StructureProjectFiscalYear = project.FiscalYear;
                    wc.Region = project.Region;
                    wc.StructuresConcept = project.StructuresConcept;
                    wc.WorkConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                    wc.ProjectWorkConceptHistoryDbId = Convert.ToInt32(dr["projectworkconcepthistorydbid"]);
                    wc.WorkConceptCode = dr["workconceptcode"].ToString();
                    wc.WorkConceptDescription = dr["workconceptdescription"].ToString();
                    wc.CertifiedWorkConceptCode = dr["certifiedworkconceptcode"].ToString();
                    wc.CertifiedWorkConceptDescription = dr["certifiedworkconceptdescription"].ToString();
                    wc.StructureId = dr["structureid"].ToString();
                    wc.PlannedStructureId = dr["plannedstructureid"].ToString();
                    wc.CurrentFiscalYear = Convert.ToInt32(dr["currentfiscalyear"]);
                    wc.FiscalYear = Convert.ToInt32(dr["fiscalyear"]);
                    wc.ProjectYear = Convert.ToInt32(dr["projectyear"]);
                    wc.ProjectStatus = (StructuresProgramType.ProjectStatus)dr["projecthistorystatus"];
                    wc.Status = (StructuresProgramType.WorkConceptStatus)dr["workconceptstatus"];
                    if (dr["priorityscore"] != DBNull.Value)
                    {
                        wc.PriorityScore = Convert.ToSingle(dr["priorityscore"]);
                    }
                    if (dr["cost"] != DBNull.Value)
                    {
                        wc.Cost = Convert.ToInt32(dr["cost"]);
                    }
                    if (dr["fromeligibilitylist"] != DBNull.Value)
                    {
                        wc.FromEligibilityList = Convert.ToBoolean(dr["fromeligibilitylist"]);
                    }
                    if (dr["fromfiips"] != DBNull.Value)
                    {
                        wc.FromFiips = Convert.ToBoolean(dr["fromfiips"]);
                    }
                    if (dr["evaluate"] != DBNull.Value)
                    {
                        wc.Evaluate = Convert.ToBoolean(dr["evaluate"]);
                    }
                    if (dr["fromproposedlist"] != DBNull.Value)
                    {
                        wc.FromProposedList = Convert.ToBoolean(dr["fromproposedlist"]);
                    }
                    if (dr["earlierfiscalyear"] != DBNull.Value)
                    {
                        wc.EarlierFiscalYear = Convert.ToInt32(dr["earlierfiscalyear"]);
                    }
                    if (dr["laterfiscalyear"] != DBNull.Value)
                    {
                        wc.LaterFiscalYear = Convert.ToInt32(dr["laterfiscalyear"]);
                    }
                    if (dr["changejustifications"] != DBNull.Value)
                    {
                        wc.ChangeJustifications = dr["changejustifications"].ToString();
                    }
                    if (dr["changenotes"] != DBNull.Value)
                    {
                        wc.ChangeJustificationNotes = dr["changenotes"].ToString();
                    }
                    if (dr["quasicertified"] != DBNull.Value)
                    {
                        wc.IsQuasicertified = Convert.ToBoolean(dr["quasicertified"]);
                    }
                    if (dr["latitude"] != DBNull.Value)
                    {
                        wc.GeoLocation.LatitudeDecimal = Convert.ToSingle(dr["latitude"]);
                    }
                    if (dr["longitude"] != DBNull.Value)
                    {
                        wc.GeoLocation.LongitudeDecimal = Convert.ToSingle(dr["longitude"]);
                    }
                    if (dr["precertificationdecision"] != DBNull.Value)
                    {
                        wc.PrecertificationDecision =
                            (StructuresProgramType.PrecertificatioReviewDecision)dr["precertificationdecision"];
                    }
                    if (dr["useractiondatetime"] != DBNull.Value)
                    {
                        wc.ProjectUserActionDateTime = Convert.ToDateTime(dr["useractiondatetime"]);
                    }
                    if (dr["useraction"] != DBNull.Value)
                    {
                        wc.ProjectUserAction = (StructuresProgramType.ProjectUserAction)dr["useraction"];
                    }
                    if (dr["userfullname"] != DBNull.Value)
                    {
                        wc.ProjectUserFullName = dr["userfullname"].ToString();
                    }
                    else
                    {
                        wc.ProjectUserFullName = "";
                    }
                    if (dr["certificationliaisonuserfullname"] != DBNull.Value)
                    {
                        wc.CertificationLiaisonFullName = dr["certificationliaisonuserfullname"].ToString();
                    }
                    else
                    {
                        wc.CertificationLiaisonFullName = "";
                    }
                    if (dr["precertificationliaisonuserfullname"] != DBNull.Value)
                    {
                        wc.ProjectPrecertificationLiaisonUserFullName = dr["precertificationliaisonuserfullname"].ToString();
                    }
                    else
                    {
                        wc.ProjectPrecertificationLiaisonUserFullName = "";
                    }
                    if (dr["precertificationdecisiondatetime"] != DBNull.Value)
                    {
                        wc.PrecertificationDecisionDateTime = Convert.ToDateTime(dr["precertificationdecisiondatetime"]);
                    }
                    if (dr["precertificationdecisionreasoncategory"] != DBNull.Value)
                    {
                        wc.PrecertificationDecisionReasonCategory = dr["precertificationdecisionreasoncategory"].ToString();
                    }
                    else
                    {
                        wc.PrecertificationDecisionReasonCategory = "";
                    }
                    if (dr["precertificationdecisionreasonexplanation"] != DBNull.Value)
                    {
                        wc.PrecertificationDecisionReasonExplanation = dr["precertificationdecisionreasonexplanation"].ToString();
                    }
                    else
                    {
                        wc.PrecertificationDecisionReasonExplanation = "";
                    }
                    if (dr["precertificationdecisioninternalcomments"] != DBNull.Value)
                    {
                        wc.PrecertificationDecisionInternalComments = dr["precertificationdecisioninternalcomments"].ToString();
                    }
                    else
                    {
                        wc.PrecertificationDecisionInternalComments = "";
                    }
                    if (dr["certificationdecision"] != DBNull.Value)
                    {
                        wc.CertificationDecision = dr["certificationdecision"].ToString();
                    }
                    else
                    {
                        wc.CertificationDecision = "";
                    }
                    if (dr["certificationprimaryworktypecomments"] != DBNull.Value)
                    {
                        wc.CertificationPrimaryWorkTypeComments = dr["certificationprimaryworktypecomments"].ToString();
                    }
                    else
                    {
                        wc.CertificationPrimaryWorkTypeComments = "";
                    }
                    if (dr["certificationsecondaryworktypecomments"] != DBNull.Value)
                    {
                        wc.CertificationSecondaryWorkTypeComments = dr["certificationsecondaryworktypecomments"].ToString();
                    }
                    else
                    {
                        wc.CertificationSecondaryWorkTypeComments = "";
                    }
                    if (dr["certificationadditionalcomments"] != DBNull.Value)
                    {
                        wc.CertificationAdditionalComments = dr["certificationadditionalcomments"].ToString();
                    }
                    else
                    {
                        wc.CertificationAdditionalComments = "";
                    }
                    wcs.Add(wc);
                }
            }
            return wcs;
        }

        public UserAccount GetPrecertificationSubmitter(int projectDbId)
        {
            UserAccount userAccount = null;
            string qry =
                @"
                    select userdbid
                    from projecthistory
                    where projectdbid = @projectDbId
                        and projecthistorydbid =
                            (select max(projecthistorydbid)
                             from projecthistory
                             where projectdbid = @projectDbId
                                and useraction = 2)
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[0].Value = projectDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                int userDbId = Convert.ToInt32(dt.Rows[0]["userdbid"]);
                userAccount = GetUserAccount(userDbId);
            }

            return userAccount;
        }

        public void UpdatePrecertifier(Project project, List<WorkConcept> workConcepts)
        {
            lock (database.databaseLock)
            {
                DateTime dateTimeNow = DateTime.Now;

                // Update Project table
                string qry =
                    @"
                        update project
                        set locked = @locked,
                            inprecertification = @inPrecertification,
                            precertificationliaisonuserdbid = @precertificationLiaisonUserDbId,
                            precertificationliaisonuserfullname = @precertificationLiaisonUserFullName
                        where projectdbid = @projectDbId
                    ";
                SqlParameter[] prms = new SqlParameter[5];
                prms[0] = new SqlParameter("@locked", SqlDbType.Bit);
                prms[0].Value = project.Locked ? 1 : 0;
                prms[1] = new SqlParameter("@inPrecertification", SqlDbType.Bit);
                prms[1].Value = project.InPrecertification ? 1 : 0;
                prms[2] = new SqlParameter("@precertificationLiaisonUserDbId", SqlDbType.Int);
                prms[2].Value = project.PrecertificationLiaisonUserDbId;
                prms[3] = new SqlParameter("@precertificationLiaisonUserFullName", SqlDbType.VarChar);
                prms[3].Value = project.PrecertificationLiaisonUserFullName;
                prms[4] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[4].Value = project.ProjectDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Insert new record in ProjectHistory
                qry =
                    @"
                        insert into projecthistory 
                            (projectdbid, currentfiscalyear, fiscalyear, projectyear, structuresconcept,
                            numberofstructures, structurescost, quasicertified, status, description,
                            notes, requestadvancedcertification, advancedcertificationdate, relatedprojects,
                            fosprojectid, advanceablefiscalyear, recertificationreason)
                        select
                            projectdbid, currentfiscalyear, fiscalyear, projectyear, structuresconcept,
                            numberofstructures, structurescost, quasicertified, status, description,
                            notes, requestadvancedcertification, advancedcertificationdate, relatedprojects,
                            fosprojectid, advanceablefiscalyear, recertificationreason
                        from projecthistory 
                        where projecthistorydbid = @projectHistoryDbId
                    ";
                prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[0].Value = project.ProjectHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab max projecthistorydbid
                qry =
                    @"
                        select max(projecthistorydbid) as maxprojecthistorydbid
                        from projecthistory
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
                int newProjectHistoryDbId = 0;

                if (dt != null)
                {
                    try
                    {
                        newProjectHistoryDbId = Convert.ToInt32(dt.Rows[0]["maxprojecthistorydbid"]);
                        project.ProjectHistoryDbId = newProjectHistoryDbId;
                        // Update ProjectHistory table
                        qry =
                            @"
                        update projecthistory
                        set precertificationliaisonuserdbid = @precertificationLiaisonUserDbId,
                            precertificationliaisonuserfullname = @precertificationLiaisonUserFullName,
                            status = @status,
                            locked = @locked,
                            userdbid = @userDbId,
                            userfullname = @userFullName,
                            useraction = @userAction,
                            useractiondatetime = @dateTimeNow
                        where projecthistorydbid = @newProjectHistoryDbId
                    ";
                        prms = new SqlParameter[9];
                        prms[0] = new SqlParameter("@precertificationLiaisonUserDbId", SqlDbType.Int);
                        prms[0].Value = project.PrecertificationLiaisonUserDbId;
                        prms[1] = new SqlParameter("@precertificationLiaisonUserFullName", SqlDbType.VarChar);
                        prms[1].Value = project.PrecertificationLiaisonUserFullName;
                        prms[2] = new SqlParameter("@status", SqlDbType.Int);
                        prms[2].Value = (int)project.Status;
                        prms[3] = new SqlParameter("@userDbId", SqlDbType.Int);
                        prms[3].Value = project.UserDbId;
                        prms[4] = new SqlParameter("@userFullName", SqlDbType.VarChar);
                        prms[4].Value = project.UserFullName;
                        prms[5] = new SqlParameter("@userAction", SqlDbType.Int);
                        prms[5].Value = (int)project.UserAction;
                        prms[6] = new SqlParameter("@dateTimeNow", SqlDbType.DateTime);
                        prms[6].Value = dateTimeNow;
                        prms[7] = new SqlParameter("@newProjectHistoryDbId", SqlDbType.Int);
                        prms[7].Value = newProjectHistoryDbId;
                        prms[8] = new SqlParameter("@locked", SqlDbType.Bit);
                        prms[8].Value = project.Locked ? 1 : 0;
                        ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                        //List<WorkConcept> wcs = new List<WorkConcept>();
                        foreach (var wc in workConcepts)
                        {
                            int projectWorkConceptHistoryDbId = InsertProjectWorkConcept(newProjectHistoryDbId, wc);
                            wc.ProjectWorkConceptHistoryDbId = projectWorkConceptHistoryDbId;
                        }
                    }
                    catch { }
                }
                //project.WorkConcepts = wcs;
            } // end lock(databaseLock)
        }

        public void CertifyProject(Project project, StructuresProgramType.ProjectUserAction userAction)
        {
            lock (database.databaseLock)
            {
                DateTime dateTimeNow = DateTime.Now;
                string qry = "";
                SqlParameter[] prms = new SqlParameter[1];
                DataTable dt;

                if (userAction == StructuresProgramType.ProjectUserAction.BosCertified || userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                {
                    qry =
                        @"
                            update project
                            set locked = 0,
                                incertification = 0,
                                inprecertification = 0,
                                certifydate = @dateTimeNow
                            where projectdbid = @projectDbId
                        ";
                    prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                    prms[0].Value = project.ProjectDbId;
                    prms[1] = new SqlParameter("@dateTimeNow", SqlDbType.DateTime);
                    prms[1].Value = dateTimeNow;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                }
                else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification)
                {
                    qry =
                       @"
                            update project
                            set locked = 0,
                                incertification = 0,
                                inprecertification = 0
                            where projectdbid = @projectDbId
                        ";
                    prms = new SqlParameter[1];
                    prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                    prms[0].Value = project.ProjectDbId;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                }

                // Insert record into ProjectHistory
                qry =
                    @"
                        insert into projecthistory
                            (projectdbid, currentfiscalyear, fiscalyear, projectyear, structuresconcept,
                            numberofstructures, structurescost, quasicertified, status, description,
                            notes, userdbid, userfullname, useraction,
                            useractiondatetime, requestadvancedcertification, advancedcertificationdate, relatedprojects, fosprojectid,
                            advanceablefiscalyear, locked, precertificationliaisonuserdbid, precertificationliaisonuserfullname, certificationliaisonuserdbid,
                            certificationliaisonuserfullname, precertificationdecision, precertificationdecisiondatetime, certificationdecision, certificationdatetime,
                            recertificationreason)
                        select projectdbid, currentfiscalyear, fiscalyear, projectyear, structuresconcept,
                            numberofstructures, structurescost, quasicertified, status, description,
                            notes, userdbid, userfullname, useraction,
                            useractiondatetime, requestadvancedcertification, advancedcertificationdate, relatedprojects, fosprojectid,
                            advanceablefiscalyear, locked, precertificationliaisonuserdbid, precertificationliaisonuserfullname, certificationliaisonuserdbid,
                            certificationliaisonuserfullname, precertificationdecision, precertificationdecisiondatetime, certificationdecision, certificationdatetime,
                            recertificationreason
                        from projecthistory
                        where projecthistorydbid = @projectHistoryDbId
                    ";
                prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[0].Value = project.ProjectHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab the new projectdbid
                qry =
                    @"
                        select max(projecthistorydbid) as maxprojecthistorydbid
                        from projecthistory
                    ";
                dt = ExecuteSelect(qry, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    int newProjectDbId = Convert.ToInt32(dr["maxprojecthistorydbid"]);
                    project.ProjectHistoryDbId = newProjectDbId;
                    qry =
                        @"
                            update projecthistory
                            set userdbid = @userDbId,
                                userfullname = @userFullName,
                                useraction = @userAction,
                                useractiondatetime = @userActionDateTime,
                                recertificationreason = @recertificationReason,
                                recertificationcomments = @recertificationComments
                            where projecthistorydbid = @projectHistoryDbId
                        ";
                    prms = new SqlParameter[7];
                    prms[0] = new SqlParameter("@userDbId", SqlDbType.Int);
                    prms[0].Value = project.UserDbId;
                    prms[1] = new SqlParameter("@userFullName", SqlDbType.VarChar);
                    prms[1].Value = project.UserFullName;
                    prms[2] = new SqlParameter("@userAction", SqlDbType.Int);
                    prms[2].Value = (int)project.UserAction;
                    prms[3] = new SqlParameter("@userActionDateTime", SqlDbType.DateTime);
                    prms[3].Value = dateTimeNow;
                    prms[4] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                    prms[4].Value = project.ProjectHistoryDbId;
                    prms[5] = new SqlParameter("@recertificationReason", SqlDbType.VarChar);
                    prms[5].Value = project.RecertificationReason;
                    prms[6] = new SqlParameter("@recertificationComments", SqlDbType.VarChar);
                    prms[6].Value = project.RecertificationComments;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                    if (userAction == StructuresProgramType.ProjectUserAction.BosCertified)
                    {
                        project.Status = StructuresProgramType.ProjectStatus.Certified;
                    }
                    else if (userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                    {
                        project.Status = StructuresProgramType.ProjectStatus.QuasiCertified;
                        project.IsQuasicertified = true;
                    }
                    else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification)
                    {
                        project.Status = StructuresProgramType.ProjectStatus.Unapproved;
                    }

                    if (userAction == StructuresProgramType.ProjectUserAction.BosCertified || userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification || userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                    {
                        qry =
                            @"
                                update projecthistory
                                set status = @status
                                where projecthistorydbid = @projectHistoryDbId
                            ";
                        prms = new SqlParameter[2];
                        prms[0] = new SqlParameter("@status", SqlDbType.Int);
                        prms[0].Value = (int)project.Status;
                        prms[1] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                        prms[1].Value = project.ProjectHistoryDbId;
                        ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                        if (userAction == StructuresProgramType.ProjectUserAction.BosCertified || userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                        {
                            qry =
                           @"
                                update projecthistory
                                set certificationdatetime = @certificationDateTime
                                where projecthistorydbid = @projectHistoryDbId
                            ";
                            prms = new SqlParameter[2];
                            prms[0] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
                            prms[0].Value = dateTimeNow;
                            prms[1] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                            prms[1].Value = project.ProjectHistoryDbId;
                            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                        }
                    }

                    // Insert new record for each work concept and then update it
                    List<WorkConcept> workConcepts = new List<WorkConcept>();
                    foreach (var wc in project.WorkConcepts)
                    {
                        wc.ProjectWorkConceptHistoryDbId = InsertProjectWorkConcept(project.ProjectHistoryDbId, wc);

                        UpdateWorkConceptCertification(project.CertifiedElementWorkConceptCombinations
                                                                .Where(el => el.ProjectWorkConceptHistoryDbId == wc.ProjectWorkConceptHistoryDbId
                                                                                        && el.StructureId.Equals(wc.StructureId)).ToList(), wc);
                        workConcepts.Add(wc);
                    }

                    List<ElementWorkConcept> pairings = new List<ElementWorkConcept>();

                    foreach (var wc in workConcepts)
                    {
                        UpdateWorkConceptCertification(project.CertifiedElementWorkConceptCombinations
                                                                .Where(el => el.StructureId.Equals(wc.StructureId)).ToList(), wc);
                        pairings.AddRange(GetElementWorkConceptPairings(wc.StructureId, wc.ProjectWorkConceptHistoryDbId, wc.CertificationDateTime));
                    }

                    project.WorkConcepts = workConcepts;
                    project.CertifiedElementWorkConceptCombinations = pairings;
                }
            }
        }

        public void UpdateWorkConceptCertification(List<ElementWorkConcept> elementWorkConceptCombinations, WorkConcept workConcept)
        {
            lock (database.databaseLock)
            {
                // Update ProjectWorkConceptHistory table
                DateTime dateTimeNow = DateTime.Now;
                string qry =
                    @"
                        update projectworkconcepthistory
                        set status = @status,
                            certificationdecision = @certificationDecision,
                            certificationdatetime = @certificationDateTime,
                            certificationprimaryworktypecomments = @certificationPrimaryWorkTypeComments,
                            certificationsecondaryworktypecomments = @certificationSecondaryWorkTypeComments,
                            certificationAdditionalComments = @certificationAdditionalComments,
                            estimatedConstructionCost = @estimatedConstructionCost,
                            estimatedDesignLevelOfEffort = @estimatedDesignLevelOfEffort,
                            designResourcing = @designResourcing
                        where projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                    ";
                SqlParameter[] prms = new SqlParameter[10];
                prms[0] = new SqlParameter("@status", SqlDbType.Int);
                prms[0].Value = (int)workConcept.Status;
                prms[1] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
                prms[1].Value = dateTimeNow;
                prms[2] = new SqlParameter("@certificationPrimaryWorkTypeComments", SqlDbType.VarChar);
                prms[2].Value = workConcept.CertificationPrimaryWorkTypeComments;
                prms[3] = new SqlParameter("@certificationSecondaryWorkTypeComments", SqlDbType.VarChar);
                prms[3].Value = workConcept.CertificationSecondaryWorkTypeComments;
                prms[4] = new SqlParameter("@certificationAdditionalComments", SqlDbType.VarChar);
                prms[4].Value = workConcept.CertificationAdditionalComments;
                prms[5] = new SqlParameter("@estimatedConstructionCost", SqlDbType.Int);
                prms[5].Value = workConcept.EstimatedConstructionCost;
                prms[6] = new SqlParameter("@estimatedDesignLevelOfEffort", SqlDbType.Int);
                prms[6].Value = workConcept.EstimatedDesignLevelOfEffort;
                prms[7] = new SqlParameter("@designResourcing", SqlDbType.VarChar);
                prms[7].Value = workConcept.DesignResourcing;
                prms[8] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
                prms[8].Value = workConcept.ProjectWorkConceptHistoryDbId;
                prms[9] = new SqlParameter("@certificationDecision", SqlDbType.VarChar);
                prms[9].Value = workConcept.CertificationDecision;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                foreach (var ewc in elementWorkConceptCombinations)
                {
                    qry =
                        @"
                            insert into projectelementworkconcept(projectworkconcepthistorydbid, certificationdatetime, elementnumber, workconceptcode, workconceptlevel, comments)
                            values (@projectWorkConceptHistoryDbId, @certificationDateTime, @elementNumber, @workConceptCode, @workConceptLevel, @comments)
                        ";
                    prms = new SqlParameter[6];
                    prms[0] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
                    prms[0].Value = workConcept.ProjectWorkConceptHistoryDbId;
                    prms[1] = new SqlParameter("@certificationDateTime", SqlDbType.DateTime);
                    prms[1].Value = dateTimeNow;
                    prms[2] = new SqlParameter("@elementNumber", SqlDbType.Int);
                    prms[2].Value = ewc.ElementNumber;
                    prms[3] = new SqlParameter("@workConceptCode", SqlDbType.VarChar);
                    prms[3].Value = ewc.WorkConceptCode;
                    prms[4] = new SqlParameter("@workConceptLevel", SqlDbType.VarChar);
                    prms[4].Value = ewc.WorkConceptLevel;
                    prms[5] = new SqlParameter("@comments", SqlDbType.VarChar);
                    prms[5].Value = ewc.Comments;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                    ewc.CertificationDateTime = dateTimeNow;
                }

                workConcept.CertificationDateTime = dateTimeNow;
            }
        }

        public void UpdateWorkConceptPrecertificationInternalComments(string internalComments, int projectWorkConceptHistoryDbId)
        {
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        update projectworkconcepthistory
                        set precertificationdecisioninternalcomments = @precertificationDecisionInternalComments
                        where projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@precertificationDecisionInternalComments", SqlDbType.VarChar);
                prms[0].Value = internalComments;
                prms[1] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
                prms[1].Value = projectWorkConceptHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
            }
        }

        public void UpdateWorkConceptPrecertification(Project project, WorkConcept workConcept)
        {
            lock (database.databaseLock)
            {
                // Update ProjectHistory table
                string qry =
                    @"
                    update projecthistory
                    set status = @status
                    where projecthistorydbid = @projectHistoryDbId
                ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@status", SqlDbType.Int);
                prms[0].Value = (int)project.Status;
                prms[1] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[1].Value = project.ProjectHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Update ProjectWorkConceptHistory table
                DateTime dateTimeNow = DateTime.Now;
                qry =
                    @"
                        update projectworkconcepthistory
                        set status = @status,
                            precertificationdecision = @precertificationDecision,
                            precertificationdecisiondatetime = @precertificationDecisionDateTime,
                            precertificationdecisionreasoncategory = @precertificationDecisionReasoncategory,
                            precertificationdecisionreasonexplanation = @precertificationDecisionReasonExplanation,
                            precertificationdecisioninternalcomments = @precertificationDecisionInternalComments
                        where projectworkconcepthistorydbid = @projectWorkConceptHistoryDbId
                    ";
                prms = new SqlParameter[7];
                prms[0] = new SqlParameter("@status", SqlDbType.Int);
                prms[0].Value = (int)workConcept.Status;
                prms[1] = new SqlParameter("@precertificationDecision", SqlDbType.Int);
                prms[1].Value = (int)workConcept.PrecertificationDecision;
                prms[2] = new SqlParameter("@precertificationDecisionDateTime", SqlDbType.DateTime);
                prms[2].Value = dateTimeNow;
                prms[3] = new SqlParameter("@precertificationDecisionReasoncategory", SqlDbType.VarChar);
                prms[3].Value = workConcept.PrecertificationDecisionReasonCategory;
                prms[4] = new SqlParameter("@precertificationDecisionReasonExplanation", SqlDbType.VarChar);
                prms[4].Value = workConcept.PrecertificationDecisionReasonExplanation;
                prms[5] = new SqlParameter("@precertificationDecisionInternalComments", SqlDbType.VarChar);
                prms[5].Value = workConcept.PrecertificationDecisionInternalComments;
                prms[6] = new SqlParameter("@projectWorkConceptHistoryDbId", SqlDbType.Int);
                prms[6].Value = workConcept.ProjectWorkConceptHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
            }
        }

        public void SaveCertifier(int liaisonUserDbId, string liaisonType, Project project, List<WorkConcept> workConcepts, UserAccount userAccount)
        {
            if (liaisonUserDbId != 0)
            {
                UserAccount liaisonUserAccount = null;
                string fullName = "";
                project.Locked = true;
                project.UserDbId = userAccount.UserDbId;
                project.UserDbIds.Add(liaisonUserDbId);
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);

                if (liaisonType.Equals("Certification"))
                {
                    liaisonUserAccount = Database.certificationLiaisons.Where(l => l.UserDbId == liaisonUserDbId).First();
                    fullName = liaisonUserAccount.FirstName + " " + liaisonUserAccount.LastName;
                    project.InPrecertification = false;
                    project.InCertification = true;
                    project.CertificationLiaisonUserDbId = liaisonUserDbId;
                    project.CertificationLiaisonUserFullName = fullName;
                    project.UserAction = StructuresProgramType.ProjectUserAction.Certification;
                    UpdateCertifier(project, workConcepts);
                }
                else if (liaisonType.Equals("Precertification"))
                {
                    liaisonUserAccount = Database.precertificationLiaisons.Where(l => l.UserDbId == liaisonUserDbId).First();
                    fullName = liaisonUserAccount.FirstName + " " + liaisonUserAccount.LastName;
                    project.InPrecertification = true;
                    project.InCertification = false;
                    project.PrecertificationLiaisonUserDbId = liaisonUserDbId;
                    project.PrecertificationLiaisonUserFullName = fullName;
                    project.UserAction = StructuresProgramType.ProjectUserAction.Precertification;
                    UpdatePrecertifier(project, workConcepts);
                }

                project.WorkConcepts = workConcepts;
                project.History = GetProjectHistory(project.ProjectDbId);
                //Email.ComposeMessage(project, userAccount, GetEmailAddresses(project.UserDbIds), GetApplicationMode(), Path.Combine(GetMyDirectory(), "bos.jpg"), this);
                EmailService.EmailMessage(project, userAccount, Database.applicationMode, Path.Combine(database.myDir, "bos.jpg"), (DatabaseService)dataServ);
            }
            else // Getting unassigned
            {
                int formerLiaisonUserDbId = 0;

                if (liaisonType.Equals("Certification"))
                {
                    formerLiaisonUserDbId = project.CertificationLiaisonUserDbId;
                }
                else if (liaisonType.Equals("Precertification"))
                {
                    formerLiaisonUserDbId = project.PrecertificationLiaisonUserDbId;
                }

                if (formerLiaisonUserDbId != 0)
                {
                    UserAccount formerLiaisonUserAccount = null;
                    project.UserAction = StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment;
                    //string fullName = ""; formerLiaisonUserAccount.FirstName + " " + formerLiaisonUserAccount.LastName;
                    project.Locked = false;
                    project.InPrecertification = false;
                    project.InCertification = false;
                    project.UserDbId = userAccount.UserDbId;
                    project.UserDbIds.Add(liaisonUserDbId);
                    project.UserDbIds.Add(userAccount.UserDbId);
                    project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);

                    if (liaisonType.Equals("Certification"))
                    {
                        formerLiaisonUserAccount = Database.certificationLiaisons.Where(l => l.UserDbId == project.CertificationLiaisonUserDbId).First();
                        project.CertificationLiaisonUserDbId = 0;
                        project.CertificationLiaisonUserFullName = "";
                        UpdateCertifier(project, workConcepts);
                    }
                    else if (liaisonType.Equals("Precertification"))
                    {
                        formerLiaisonUserAccount = Database.precertificationLiaisons.Where(l => l.UserDbId == project.PrecertificationLiaisonUserDbId).First();
                        project.UserAction = StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment;
                        project.PrecertificationLiaisonUserDbId = 0;
                        project.PrecertificationLiaisonUserFullName = "";
                        UpdatePrecertifier(project, workConcepts);
                    }

                    project.WorkConcepts = workConcepts;
                    project.History = GetProjectHistory(project.ProjectDbId);
                    //Email.ComposeMessage(project, userAccount, GetEmailAddresses(project.UserDbIds), GetApplicationMode(), Path.Combine(GetMyDirectory(), "bos.jpg"), this);
                    EmailService.EmailMessage(project, userAccount, Database.applicationMode, Path.Combine(database.myDir, "bos.jpg"), (DatabaseService)dataServ);
                }
            }
        }

        public string GetMyDirectory()
        {
            return database.myDir;
        }

        public List<WorkConcept> GetEligibleWorkConcepts()
        {
            return database.eligibleWorkConcepts;
        }

        public Dw.Database GetWarehouseDatabase()
        {
            return database.WarehouseDatabase;
        }

        public void UpdateCertifier(Project project, List<WorkConcept> workConcepts)
        {
            lock (database.databaseLock)
            {
                DateTime dateTimeNow = DateTime.Now;

                // Update Project table
                string qry =
                    @"
                        update project
                        set locked = @locked,
                            incertification = @inCertification,
                            certificationliaisonuserdbid = @certificationLiaisonUserDbId,
                            certificationliaisonuserfullname = @certificationLiaisonUserFullName
                        where projectdbid = @projectDbId
                    ";
                SqlParameter[] prms = new SqlParameter[5];
                prms[0] = new SqlParameter("@locked", SqlDbType.Bit);
                prms[0].Value = project.Locked ? 1 : 0;
                prms[1] = new SqlParameter("@inCertification", SqlDbType.Bit);
                prms[1].Value = project.InCertification ? 1 : 0;
                prms[2] = new SqlParameter("@certificationLiaisonUserDbId", SqlDbType.Int);
                prms[2].Value = project.CertificationLiaisonUserDbId;
                prms[3] = new SqlParameter("@certificationLiaisonUserFullName", SqlDbType.VarChar);
                prms[3].Value = project.CertificationLiaisonUserFullName;
                prms[4] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[4].Value = project.ProjectDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Insert new record in ProjectHistory
                qry =
                    @"
                        insert into projecthistory 
                            (projectdbid, currentfiscalyear, fiscalyear, projectyear, structuresconcept,
                            numberofstructures, structurescost, quasicertified, status, description,
                            notes, requestadvancedcertification, advancedcertificationdate, relatedprojects,
                            fosprojectid, advanceablefiscalyear, recertificationreason)
                        select
                            projectdbid, currentfiscalyear, fiscalyear, projectyear, structuresconcept,
                            numberofstructures, structurescost, quasicertified, status, description,
                            notes, requestadvancedcertification, advancedcertificationdate, relatedprojects,
                            fosprojectid, advanceablefiscalyear, recertificationreason
                        from projecthistory 
                        where projecthistorydbid = @projectHistoryDbId
                    ";
                prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[0].Value = project.ProjectHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                // Grab max projecthistorydbid
                qry =
                    @"
                        select max(projecthistorydbid) as maxprojecthistorydbid
                        from projecthistory
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);
                int newProjectHistoryDbId = 0;

                if (dt != null)
                {
                    try
                    {
                        newProjectHistoryDbId = Convert.ToInt32(dt.Rows[0]["maxprojecthistorydbid"]);
                        project.ProjectHistoryDbId = newProjectHistoryDbId;
                        // Update ProjectHistory table
                        qry =
                            @"
                        update projecthistory
                        set certificationliaisonuserdbid = @certificationLiaisonUserDbId,
                            certificationliaisonuserfullname = @certificationLiaisonUserFullName,
                            status = @status,
                            locked = @locked,
                            userdbid = @userDbId,
                            userfullname = @userFullName,
                            useraction = @userAction,
                            useractiondatetime = @dateTimeNow
                        where projecthistorydbid = @newProjectHistoryDbId
                    ";
                        prms = new SqlParameter[9];
                        prms[0] = new SqlParameter("@certificationLiaisonUserDbId", SqlDbType.Int);
                        prms[0].Value = project.CertificationLiaisonUserDbId;
                        prms[1] = new SqlParameter("@certificationLiaisonUserFullName", SqlDbType.VarChar);
                        prms[1].Value = project.CertificationLiaisonUserFullName;
                        prms[2] = new SqlParameter("@status", SqlDbType.Int);
                        prms[2].Value = (int)project.Status;
                        prms[3] = new SqlParameter("@userDbId", SqlDbType.Int);
                        prms[3].Value = project.UserDbId;
                        prms[4] = new SqlParameter("@userFullName", SqlDbType.VarChar);
                        prms[4].Value = project.UserFullName;
                        prms[5] = new SqlParameter("@userAction", SqlDbType.Int);
                        prms[5].Value = (int)project.UserAction;
                        prms[6] = new SqlParameter("@dateTimeNow", SqlDbType.DateTime);
                        prms[6].Value = dateTimeNow;
                        prms[7] = new SqlParameter("@newProjectHistoryDbId", SqlDbType.Int);
                        prms[7].Value = newProjectHistoryDbId;
                        prms[8] = new SqlParameter("@locked", SqlDbType.Bit);
                        prms[8].Value = project.Locked ? 1 : 0;
                        ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                        //List<WorkConcept> wcs = new List<WorkConcept>();
                        foreach (var wc in workConcepts)
                        {
                            int projectWorkConceptHistoryDbId = InsertProjectWorkConcept(newProjectHistoryDbId, wc);
                            wc.ProjectWorkConceptHistoryDbId = projectWorkConceptHistoryDbId;
                        }
                    }
                    catch { }
                }
            }
        }

        public void UpdateProjectCertification(Project project, StructuresProgramType.PrecertificatioReviewDecision decision)
        {
            lock (database.databaseLock)
            {
                DateTime dateTimeNow = DateTime.Now;

                // Update Project table
                string qry =
                    @"
                        update project
                        set locked = 0,
                            inprecertification = 0
                        where projectdbid = @projectDbId
                    ";
                SqlParameter[] prms = new SqlParameter[1];
                prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[0].Value = project.ProjectDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                if (decision == StructuresProgramType.PrecertificatioReviewDecision.Accept)
                {
                    qry =
                        @"
                            update project
                            set precertifydate = @dateTimeNow
                            where projectdbid = @projectDbId
                        ";
                    prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@dateTimeNow", SqlDbType.DateTime);
                    prms[0].Value = dateTimeNow;
                    prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
                    prms[1].Value = project.ProjectDbId;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                    project.PrecertifyDate = dateTimeNow;
                }
                else
                {
                    qry =
                        @"
                            update project
                            set precertifydate = null,
                                precertificationliaisonuserdbid = null,
                                precertificationliaisonuserfullname = null
                            where projectdbid = @projectDbId
                        ";
                    prms = new SqlParameter[1];
                    prms[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                    prms[0].Value = project.ProjectDbId;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                    project.PrecertifyDate = new DateTime(1, 1, 1, 0, 0, 0);
                }

                // Update ProjectHistory table
                qry =
                    @"
                        update projecthistory
                        set status = @status,
                            locked = 0,
                            precertificationdecision = @precertificationDecision,
                            precertificationdecisiondatetime = @precertificationDecisionDateTime,
                            userdbid = @userDbId,
                            userfullname = @userFullName,
                            useraction = @userAction,
                            useractiondatetime = @dateTimeNow
                        where projecthistorydbid = @projectHistoryDbId
                    ";
                prms = new SqlParameter[8];
                prms[0] = new SqlParameter("@status", SqlDbType.Int);
                prms[0].Value = (int)project.Status;
                prms[1] = new SqlParameter("@precertificationDecision", SqlDbType.Int);
                prms[1].Value = (int)decision;
                prms[2] = new SqlParameter("@precertificationDecisionDateTime", SqlDbType.DateTime);
                prms[2].Value = dateTimeNow;
                prms[3] = new SqlParameter("@userDbId", SqlDbType.Int);
                prms[3].Value = project.UserDbId;
                prms[4] = new SqlParameter("@userFullName", SqlDbType.VarChar);
                prms[4].Value = project.UserFullName;
                prms[5] = new SqlParameter("@userAction", SqlDbType.Int);
                prms[5].Value = (int)project.UserAction;
                prms[6] = new SqlParameter("@dateTimeNow", SqlDbType.DateTime);
                prms[6].Value = dateTimeNow;
                prms[7] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[7].Value = project.ProjectHistoryDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
            }
        }

        public List<UserAccount> GetPrecertLiaisons()
        {
            return GetUsersOfARoleType("PreLiai");
        }

        public List<UserAccount> GetCertLiaisons()
        {
            return GetUsersOfARoleType("CertLiai");
        }

        public List<UserAccount> GetCertSups()
        {
            return GetUsersOfARoleType("CertSup");
        }

        public List<UserAccount> GetUsersOfARoleType(string roleType)
        {
            List<UserAccount> userAccounts = new List<UserAccount>();
            string qry =
                @"
                    select u.*, ur.*
                    from users u, userrole ur
                    where u.userdbid = ur.userdbid
                        and ur.roletype = @roleType
                    order by lastname, firstname
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@roleType", SqlDbType.VarChar);
            prms[0].Value = roleType;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    userAccounts.Add(GetUserAccount(dr["username"].ToString(), dr["userpassword"].ToString()));
                }
            }

            return userAccounts;
        }

        public void UpdateProjectBoxId(int projectDbId, string boxId)
        {
            string qry =
                @"
                    update project
                    set boxid = @boxId
                    where projectdbid = @projectDbId
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@boxId", SqlDbType.VarChar);
            prms[0].Value = boxId;
            prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[1].Value = projectDbId;
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        public int InsertProposedWorkConcept(WorkConcept workConcept)
        {
            lock (database.databaseLock)
            {
                int workConceptDbId = 0;
                string qry =
                    @"
                    insert into proposedlist (structureid, region, regionnumber, workconceptcode, workconceptdesc, fiscalyear, reasoncategory,
                                                notes, proposedbyuserdbid, proposedbyuserfullname, proposeddate, active)
                    values (@structureId, @region, @regionNumber, @workConceptCode, @workConceptDesc, @fiscalYear, @reasonCategory,
                                @notes, @proposedByUserDbId, @proposedByUserFullName, @proposedDate, @active)
                ";
                SqlParameter[] prms = new SqlParameter[12];
                prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[0].Value = workConcept.StructureId;
                prms[1] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[1].Value = workConcept.Region; ;
                prms[2] = new SqlParameter("@regionNumber", SqlDbType.VarChar);
                prms[2].Value = workConcept.RegionNumber;
                prms[3] = new SqlParameter("workConceptCode", SqlDbType.VarChar);
                prms[3].Value = workConcept.CertifiedWorkConceptCode;
                prms[4] = new SqlParameter("@workConceptDesc", SqlDbType.VarChar);
                prms[4].Value = workConcept.CertifiedWorkConceptDescription;
                prms[5] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                prms[5].Value = workConcept.FiscalYear;
                prms[6] = new SqlParameter("@reasonCategory", SqlDbType.VarChar);
                prms[6].Value = workConcept.ReasonCategory;
                prms[7] = new SqlParameter("@notes", SqlDbType.VarChar);
                prms[7].Value = workConcept.Notes;
                prms[8] = new SqlParameter("@proposedByUserDbId", SqlDbType.Int);
                prms[8].Value = workConcept.ProposedByUserDbId;
                prms[9] = new SqlParameter("@proposedByUserFullName", SqlDbType.VarChar);
                prms[9].Value = workConcept.ProposedByUserFullName;
                prms[10] = new SqlParameter("@proposedDate", SqlDbType.DateTime);
                prms[10].Value = workConcept.ProposedDate;
                prms[11] = new SqlParameter("@active", SqlDbType.Bit);
                prms[11].Value = 1;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                qry =
                    @"
                        select max(workconceptdbid) as maxWorkConceptDbId
                        from proposedlist
                    ";
                DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    workConceptDbId = Convert.ToInt32(dt.Rows[0]["maxworkconceptdbid"]);
                }

                return workConceptDbId;
            } // lock (databaseLock)
        }

        public bool IsWorkConceptPrimary(string workActionCode)
        {
            bool isPrimary = true;
            string qry = @"
                            select *
                            from workaction
                            where workactioncode = @workActionCode
                                and improvement = 1
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[0].Value = workActionCode;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt == null || dt.Rows.Count == 0)
            {
                isPrimary = false;
            }

            return isPrimary;
        }

        public void UpdateProjectWhileInPrecertificationOrCertification(Project project)
        {
            lock (database.databaseLock)
            {
                string qry =
                    @"
                        update projecthistory
                        set structurescost = @structuresCost,
                            description = @description,
                            notes = @notes,
                            fosprojectid = @fosProjectId
                        where projecthistorydbid = 
                            (select max(projecthistorydbid)
                                from projecthistory
                                where projectdbid = @projectDbId)
                    ";
                SqlParameter[] prms = new SqlParameter[5];
                prms[0] = new SqlParameter("@structuresCost", SqlDbType.Int);
                prms[0].Value = project.StructuresCost;
                prms[1] = new SqlParameter("@description", SqlDbType.VarChar);
                prms[1].Value = project.Description;
                prms[2] = new SqlParameter("@notes", SqlDbType.VarChar);
                prms[2].Value = project.Notes;
                prms[3] = new SqlParameter("@fosProjectId", SqlDbType.VarChar);
                prms[3].Value = project.FosProjectId;
                prms[4] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[4].Value = project.ProjectDbId;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                qry =
                    @"
                        update project
                        set notificationrecipients = @notificationRecipients,
                            acceptablepsedatestart = @acceptablePseDateStart,
                            acceptablepsedateend = @acceptablePseDateEnd
                        where projectdbid = @projectDbId
                    ";
                prms = new SqlParameter[4];
                prms[0] = new SqlParameter("@notificationRecipients", SqlDbType.VarChar);
                prms[0].Value = project.NotificationRecipients;
                prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms[1].Value = project.ProjectDbId;
                prms[2] = new SqlParameter("@acceptablePseDateStart", SqlDbType.Date);
                prms[2].Value = project.AcceptablePseDateStart;
                prms[3] = new SqlParameter("@acceptablePseDateEnd", SqlDbType.Date);
                prms[3].Value = project.AcceptablePseDateEnd;
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
            }
        }

        public int InsertProject(Project project)
        {
            lock (database.databaseLock)
            {
                int projectDbId = project.ProjectDbId;
                DateTime dateTimeNow = project.UserActionDateTime;

                if (projectDbId == 0)
                {
                    string qry =
                        @"
                            insert into project(region, active, createdate)
                            values(@region, @active, @createDate)
                        ";
                    SqlParameter[] prms = new SqlParameter[3];
                    prms[0] = new SqlParameter("@region", SqlDbType.VarChar);
                    prms[0].Value = project.Region;
                    prms[1] = new SqlParameter("@active", SqlDbType.Bit);
                    prms[1].Value = 1;
                    prms[2] = new SqlParameter("@createDate", SqlDbType.DateTime);
                    prms[2].Value = dateTimeNow;
                    //prms[3] = new SqlParameter("@notificationRecipients", SqlDbType.VarChar);
                    //prms[3].Value = FormatEmailAddresses(project.NotificationRecipients);
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                    string qry2 =
                       @"
                            select max(projectdbid) as maxprojectdbid
                            from project
                        ";
                    DataTable dt = ExecuteSelect(qry2, database.wisamsConnection);
                    projectDbId = Convert.ToInt32(dt.Rows[0]["maxprojectdbid"]);
                    project.ProjectDbId = projectDbId;
                }

                string q =
                    @"
                        
                            update project
                            set inprecertification = 0,
                                precertifydate = null,
                                precertificationliaisonuserdbid = null,
                                precertificationliaisonuserfullname = null,
                                notificationrecipients = @notificationRecipients,
                                acceptablepsedatestart = @acceptablePseDateStart,
                                acceptablepsedateend = @acceptablePseDateEnd
                            where projectdbid = @projectDbId
                    ";
                SqlParameter[] p = new SqlParameter[4];
                p[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                p[0].Value = project.ProjectDbId;
                p[1] = new SqlParameter("@notificationRecipients", SqlDbType.VarChar);
                string formattedEmailAddresses = repo.FormatEmailAddresses(project.NotificationRecipients);
                p[1].Value = formattedEmailAddresses;
                p[2] = new SqlParameter("@acceptablePseDateStart", SqlDbType.Date);
                p[2].Value = project.AcceptablePseDateStart;
                p[3] = new SqlParameter("@acceptablePseDateEnd", SqlDbType.Date);
                p[3].Value = project.AcceptablePseDateEnd;
                ExecuteInsertUpdateDelete(q, p, database.wisamsConnection);

                if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification)
                {
                    string qry =
                        @"
                            update project
                            set submitdate = @submitDate
                            where projectdbid = @projectDbId
                        ";
                    SqlParameter[] prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@submitDate", SqlDbType.DateTime);
                    prms[0].Value = dateTimeNow;
                    prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
                    prms[1].Value = project.ProjectDbId;
                    ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
                    project.SubmitDate = dateTimeNow;

                    /*
                    if (project.Status == StructuresProgramType.ProjectStatus.Precertified)
                    {
                        qry =
                            @"
                                update project
                                set precertifydate = @precertifyDate
                                where projectdbid = @projectDbId
                            ";
                        prms = new SqlParameter[2];
                        prms[0] = new SqlParameter("@precertifyDate", SqlDbType.DateTime);
                        prms[0].Value = dateTimeNow;
                        prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
                        prms[1].Value = project.ProjectDbId;
                        ExecuteInsertUpdateDelete(qry, prms, wisamsConnection);
                        project.PrecertifyDate = dateTimeNow;
                    }
                    else
                    {
                        qry =
                            @"
                                update project
                                set precertifydate = @precertifyDate
                                where projectdbid = @projectDbId
                            ";
                        prms = new SqlParameter[2];
                        prms[0] = new SqlParameter("@precertifyDate", SqlDbType.Date);
                        prms[0].Value = DBNull.Value;
                        prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
                        prms[1].Value = project.ProjectDbId;
                        ExecuteInsertUpdateDelete(qry, prms, wisamsConnection);
                        project.PrecertifyDate = new DateTime(1, 1, 1, 0, 0, 0);
                    }*/
                }

                string qry3 =
                    @"
                        insert into projecthistory(projectdbid, currentfiscalyear, fiscalyear,
                                                projectyear, structuresconcept, numberofstructures,
                                                structurescost, quasicertified, status,
                                                description, notes, userdbid,
                                                useraction, useractiondatetime, requestadvancedcertification,
                                                advancedcertificationdate, relatedprojects, userfullname, fosProjectId,
                                                locked, advanceableFiscalYear)
                        values(@projectDbId, @currentFiscalYear, @fiscalYear,
                            @projectYear, @structuresConcept, @numberOfStructures,
                            @structuresCost, @quasicertified, @status,
                            @description, @notes, @userDbId,
                            @userAction, @userActionDateTime, @requestAdvancedCertification,
                            @advancedCertificationDate, @relatedProjects, @userFullName, @fosProjectId,
                            @locked, @advanceableFiscalYear)
                    ";
                SqlParameter[] prms3 = new SqlParameter[21];
                prms3[0] = new SqlParameter("@projectDbId", SqlDbType.Int);
                prms3[0].Value = projectDbId;
                prms3[1] = new SqlParameter("@currentFiscalYear", SqlDbType.Int);
                prms3[1].Value = project.CurrentFiscalYear;
                prms3[2] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                prms3[2].Value = project.FiscalYear;
                prms3[3] = new SqlParameter("@projectYear", SqlDbType.Int);
                prms3[3].Value = project.ProjectYear;
                prms3[4] = new SqlParameter("@structuresConcept", SqlDbType.VarChar);

                if (!String.IsNullOrEmpty(project.StructuresConcept))
                {
                    prms3[4].Value = project.StructuresConcept;
                }
                else
                {
                    prms3[4].Value = DBNull.Value;
                }

                prms3[5] = new SqlParameter("@numberOfStructures", SqlDbType.Int);
                prms3[5].Value = project.NumberOfStructures;

                prms3[6] = new SqlParameter("@structuresCost", SqlDbType.Int);
                prms3[6].Value = project.StructuresCost;
                prms3[7] = new SqlParameter("@quasicertified", SqlDbType.Bit);
                prms3[7].Value = 0;
                prms3[8] = new SqlParameter("@status", SqlDbType.Int);
                prms3[8].Value = (int)project.Status;

                prms3[9] = new SqlParameter("@description", SqlDbType.VarChar);
                prms3[9].Value = project.Description;
                prms3[10] = new SqlParameter("@notes", SqlDbType.VarChar);
                prms3[10].Value = project.Notes;
                prms3[11] = new SqlParameter("@userDbId", SqlDbType.Int);
                prms3[11].Value = project.UserDbId;

                prms3[12] = new SqlParameter("@userAction", SqlDbType.Int);
                prms3[12].Value = (int)project.UserAction;
                prms3[13] = new SqlParameter("@userActionDateTime", SqlDbType.DateTime);
                prms3[13].Value = dateTimeNow;
                prms3[14] = new SqlParameter("@requestAdvancedCertification", SqlDbType.Int);

                if (project.RequestAdvancedCertification)
                {
                    prms3[14].Value = 1;
                }
                else
                {
                    prms3[14].Value = 0;
                }

                prms3[15] = new SqlParameter("@advancedCertificationDate", SqlDbType.Date);

                if (project.RequestAdvancedCertification)
                {
                    prms3[15].Value = project.AdvancedCertificationDate.Date;
                }
                else
                {
                    prms3[15].Value = DBNull.Value;
                }

                prms3[16] = new SqlParameter("@relatedProjects", SqlDbType.VarChar);
                prms3[16].Value = project.RelatedProjects;
                prms3[17] = new SqlParameter("@userFullName", SqlDbType.VarChar);
                prms3[17].Value = project.UserFullName;
                prms3[18] = new SqlParameter("@fosProjectId", SqlDbType.VarChar);
                prms3[18].Value = project.FosProjectId;
                prms3[19] = new SqlParameter("@locked", SqlDbType.Int);

                if (project.Locked)
                {
                    prms3[19].Value = 1;
                }
                else
                {
                    prms3[19].Value = 0;
                }

                prms3[20] = new SqlParameter("@advanceableFiscalYear", SqlDbType.Int);

                if (project.AdvanceableFiscalYear != 0)
                {
                    prms3[20].Value = project.AdvanceableFiscalYear;
                }
                else
                {
                    prms3[20].Value = DBNull.Value;
                }

                ExecuteInsertUpdateDelete(qry3, prms3, database.wisamsConnection);

                string qry4 =
                @"
                    select max(projecthistorydbid) as maxprojecthistorydbid
                    from projecthistory
                ";
                DataTable dt4 = ExecuteSelect(qry4, database.wisamsConnection);

                if (dt4 != null && dt4.Rows.Count > 0)
                {
                    int projectHistoryDbId = Convert.ToInt32(dt4.Rows[0]["maxprojecthistorydbid"]);
                    project.ProjectHistoryDbId = projectHistoryDbId;
                    string previousHistory = project.History;
                    string newHistoryLine = String.Format("{0}: {1} by {2}\r\n\r\n", project.UserActionDateTime, project.UserAction, project.UserFullName);
                    project.History = newHistoryLine + previousHistory;

                    if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification
                            && project.Status == StructuresProgramType.ProjectStatus.Precertified)
                    {
                        /*
                        string qry5 =
                            @"
                                update projecthistory
                                set precertificationdecision = @precertificationDecision,
                                    precertificationdecisiondatetime = @precertificationDecisionDateTime
                                where projecthistorydbid = @projectHistoryDbId
                            ";*/
                    }

                    foreach (var wc in project.WorkConcepts)
                    {
                        int projectWorkConceptHistoryDbId = InsertProjectWorkConcept(projectHistoryDbId, wc);
                    }
                }

                return projectDbId;
            } // End lock (databaseLock)
        }

        public int InsertProjectWorkConcept(int projectHistoryDbId, WorkConcept wc)
        {
            lock (database.databaseLock)
            {
                int projectWorkConceptHistoryDbId = 0;
                string qry =
                    @"
                    insert into projectworkconcepthistory
                        (
                        projecthistorydbid, workconceptdbid, workconceptcode,
                        workconceptdescription, certifiedworkconceptcode, certifiedworkconceptdescription,
                        structureid, plannedstructureid, currentfiscalyear,
                        fiscalyear, projectyear, priorityscore,
                        cost, fromeligibilitylist, fromfiips,
                        evaluate, earlierfiscalyear, laterfiscalyear,
                        changejustifications, changenotes, quasicertified,
                        status, latitude, longitude, fromproposedlist
                        )
                    values
                        (
                        @projectHistoryDbId, @workConceptDbId, @workConceptCode,
                        @workConceptDescription, @certifiedWorkConceptCode, @certifiedWorkConceptDescription,
                        @structureId, @plannedStructureId, @currentFiscalYear,
                        @fiscalYear, @projectYear, @priorityScore,
                        @cost, @fromEligibilityList, @fromFiips,
                        @evaluate, @earlierFiscalYear, @laterFiscalYear,
                        @changeJustifications, @changeNotes, @quasicertified,
                        @status, @latitude, @longitude, @fromProposedList
                        )
                ";
                SqlParameter[] prms = new SqlParameter[25];
                prms[0] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
                prms[0].Value = projectHistoryDbId;
                prms[1] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
                prms[1].Value = wc.WorkConceptDbId;
                prms[2] = new SqlParameter("@workConceptCode", SqlDbType.VarChar);
                prms[2].Value = wc.WorkConceptCode;

                prms[3] = new SqlParameter("@workConceptDescription", SqlDbType.VarChar);
                prms[3].Value = wc.WorkConceptDescription;
                prms[4] = new SqlParameter("@certifiedWorkConceptCode", SqlDbType.VarChar);
                prms[4].Value = wc.CertifiedWorkConceptCode;
                prms[5] = new SqlParameter("@certifiedWorkConceptDescription", SqlDbType.VarChar);
                prms[5].Value = wc.CertifiedWorkConceptDescription;

                /*
                prms[3] = new SqlParameter("@workConceptDescription", SqlDbType.VarChar);
                prms[3].Value = wc.WorkConceptDescription;
                prms[4] = new SqlParameter("@certifiedWorkConceptCode", SqlDbType.VarChar);
                prms[4].Value = wc.CertifiedWorkConceptCode;
                prms[5] = new SqlParameter("@certifiedWorkConceptDescription", SqlDbType.VarChar);
                prms[5].Value = wc.CertifiedWorkConceptDescription;*/

                prms[6] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[6].Value = wc.StructureId;
                prms[7] = new SqlParameter("@plannedStructureId", SqlDbType.VarChar);
                prms[7].Value = wc.PlannedStructureId;
                prms[8] = new SqlParameter("@currentFiscalYear", SqlDbType.Int);
                prms[8].Value = wc.CurrentFiscalYear;

                prms[9] = new SqlParameter("@fiscalYear", SqlDbType.Int);
                prms[9].Value = wc.FiscalYear;
                prms[10] = new SqlParameter("@projectYear", SqlDbType.Int);
                prms[10].Value = wc.ProjectYear;
                prms[11] = new SqlParameter("@priorityScore", SqlDbType.Float);
                prms[11].Value = wc.PriorityScore;

                prms[12] = new SqlParameter("@cost", SqlDbType.Int);
                prms[12].Value = wc.Cost;
                prms[13] = new SqlParameter("@fromEligibilityList", SqlDbType.Bit);

                if (wc.FromEligibilityList)
                {
                    prms[13].Value = 1;
                }
                else
                {
                    prms[13].Value = 0;
                }

                prms[14] = new SqlParameter("@fromFiips", SqlDbType.Bit);

                if (wc.FromFiips)
                {
                    prms[14].Value = 1;
                }
                else
                {
                    prms[14].Value = 0;
                }

                prms[15] = new SqlParameter("@evaluate", SqlDbType.Bit);

                if (wc.Evaluate)
                {
                    prms[15].Value = 1;
                }
                else
                {
                    prms[15].Value = 0;
                }

                prms[16] = new SqlParameter("@earlierFiscalYear", SqlDbType.Int);
                prms[16].Value = wc.EarlierFiscalYear;
                prms[17] = new SqlParameter("@laterFiscalYear", SqlDbType.Int);
                prms[17].Value = wc.LaterFiscalYear;

                prms[18] = new SqlParameter("@changeJustifications", SqlDbType.VarChar);
                prms[18].Value = wc.ChangeJustifications;
                prms[19] = new SqlParameter("@changeNotes", SqlDbType.VarChar);
                prms[19].Value = wc.ChangeJustificationNotes;
                prms[20] = new SqlParameter("@quasicertified", SqlDbType.Bit);
                prms[20].Value = wc.IsQuasicertified;

                prms[21] = new SqlParameter("@status", SqlDbType.Int);
                prms[21].Value = (int)wc.Status;

                if (wc.GeoLocation == null || wc.GeoLocation.LatitudeDecimal == 0 || wc.GeoLocation.LongitudeDecimal == 0)
                {
                    //wc.GeoLocation = GetStructureGeoLocation(wc.StructureId);
                    wc.GeoLocation = GetStructureLatLong(wc.StructureId);
                }

                prms[22] = new SqlParameter("@latitude", SqlDbType.Float);
                prms[22].Value = wc.GeoLocation.LatitudeDecimal;
                prms[23] = new SqlParameter("@longitude", SqlDbType.Float);
                prms[23].Value = wc.GeoLocation.LongitudeDecimal;
                prms[24] = new SqlParameter("@fromProposedList", SqlDbType.Bit);

                if (wc.FromProposedList)
                {
                    prms[24].Value = 1;
                }
                else
                {
                    prms[24].Value = 0;
                }
                ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);

                string qry2 =
                    @"
                    select max(projectworkconcepthistorydbid) as maxprojectworkconcepthistorydbid
                    from projectworkconcepthistory
                ";
                DataTable dt = ExecuteSelect(qry2, database.wisamsConnection);

                if (dt != null && dt.Rows.Count > 0)
                {
                    projectWorkConceptHistoryDbId = Convert.ToInt32(dt.Rows[0]["maxprojectworkconcepthistorydbid"]);
                    wc.ProjectWorkConceptHistoryDbId = projectWorkConceptHistoryDbId;
                }

                return projectWorkConceptHistoryDbId;
            }
        }

        public string GetEmailAddressesRegionalTransactors(List<int> userIds)
        {
            string ids = "";
            int counter = 0;

            foreach (var id in userIds)
            {
                counter++;

                if (counter == 1)
                {
                    ids = id.ToString();
                }
                else
                {
                    ids += String.Format(", {0}", id.ToString());
                }
            }

            string addresses = "";
            string qry =
                @"
                    select distinct emailaddress
                    from users
                    where office in ('1-SW', '2-SE', '3-NE', '4-NC', '5-NW')
                    and userdbid in (
                ";
            qry += ids + ")";

            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                counter = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    counter++;

                    if (counter == 1)
                    {
                        addresses = dr["emailaddress"].ToString();
                    }
                    else
                    {
                        addresses += String.Format(", {0}", dr["emailaddress"].ToString());
                    }
                }
            }

            return addresses;
        }

        public string GetEmailAddress(int userDbId)
        {
            string emailAddress = "";
            string qry =
                @"
                    select emailaddress
                    from users
                    where userdbid = @userDbId
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@userDbId", SqlDbType.Int);
            prms[0].Value = userDbId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                emailAddress = dr["emailaddress"].ToString();
            }

            return emailAddress;
        }

        public string GetEmailAddresses(List<int> userIds)
        {
            string ids = "";
            int counter = 0;

            foreach (var id in userIds)
            {
                counter++;

                if (counter == 1)
                {
                    ids = id.ToString();
                }
                else
                {
                    ids += String.Format(", {0}", id.ToString());
                }
            }

            string addresses = "";
            string qry =
                @"
                    select distinct emailaddress
                    from users
                    where userdbid in (
                ";
            qry += ids + ")";

            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                counter = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    counter++;

                    if (counter == 1)
                    {
                        addresses = dr["emailaddress"].ToString();
                    }
                    else
                    {
                        addresses += String.Format("; {0}", dr["emailaddress"].ToString());
                    }
                }
            }

            return addresses;
        }

        public void UpdateProjectFosProjectId(int projectHistoryDbId, string fosProjectId)
        {
            string qry =
                @"
                    update projecthistory
                    set fosprojectid = @fosProjectId
                    where projecthistorydbid = @projectHistoryDbId
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@fosProjectId", SqlDbType.VarChar);
            prms[0].Value = fosProjectId;
            prms[1] = new SqlParameter("@projectHistoryDbId", SqlDbType.Int);
            prms[1].Value = projectHistoryDbId;
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        public void DeleteProject(int projectDbId)
        {
            string qry =
                @"
                    update project
                    set deletedate = @deleteDate
                    where projectdbid = @projectDbId
                ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@deleteDate", SqlDbType.DateTime);
            prms[0].Value = DateTime.Now;
            prms[1] = new SqlParameter("@projectDbId", SqlDbType.Int);
            prms[1].Value = projectDbId;
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        public string GetWorkConceptDescription(string workActionCode)
        {
            string workActionDescription = "";
            string qry =
                @"
                    select *
                    from workaction
                    where workactioncode = @workActionCode
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[0].Value = workActionCode;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                workActionDescription = dt.Rows[0]["workactiondesc"].ToString().Trim();
            }

            return workActionDescription;
        }

        public List<UserActivity> GetUserActivities()
        {
            List<UserActivity> userActivities = new List<UserActivity>();
            string qry =
                @"
                    select ul.userlogdbid, ul.activity, ul.activitydatetime, ul.userdbid,
                        u.firstname, u.lastname,
                        u.dotlogin, u.office, u.emailaddress
                    from userlog ul, users u
                    where ul.userdbid = u.userdbid
                        and u.userdbid <> 5
                    order by ul.userlogdbid desc
                ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    UserActivity userActivity = new UserActivity();
                    userActivity.UserLogDbId = Convert.ToInt32(dr["userlogdbid"]);
                    userActivity.Activity = dr["activity"].ToString();
                    userActivity.ActivityDateTime = Convert.ToDateTime(dr["activitydatetime"]);
                    userActivity.UserDbId = Convert.ToInt32(dr["userdbid"]);
                    userActivity.FirstName = dr["firstname"].ToString();
                    userActivity.LastName = dr["lastname"].ToString();
                    userActivity.DotLogin = dr["dotlogin"].ToString();
                    userActivity.Office = dr["office"].ToString();
                    userActivity.EmailAddress = dr["emailaddress"].ToString();
                    userActivities.Add(userActivity);
                }
            }

            return userActivities;
        }

        public void UpdateEligibleWorkConcepts()
        {
            UpdateRegionNumber();
            //UpdateGeneratedDate();

            string qry =
                @"
                    select *
                    from eligibilitylist
                    where primaryworkconcept is not null
                        and primaryworkconcept <> ''
                        and active = 1
                ";
            DataTable dt = ExecuteSelect(qry, database.wisamsConnection);

            foreach (DataRow dr in dt.Rows)
            {
                int workConceptDbId = Convert.ToInt32(dr["workconceptdbid"]);
                string primaryWorkConcept = dr["primaryworkconcept"].ToString();
                string primaryWorkConceptCode = repo.ParseWorkConceptFullDescription(primaryWorkConcept)[0];
                string primaryWorkConceptDescription = repo.ParseWorkConceptFullDescription(primaryWorkConcept)[1];
                primaryWorkConcept = "(" + primaryWorkConceptCode + ")" + primaryWorkConceptDescription;

                string qry2 =
                    @"
                        update eligibilitylist
                        set primaryworkconceptcode = @primaryWorkConceptCode,
                            primaryworkconceptdesc = @primaryWorkConceptDescription,
                            primaryworkconcept = @primaryWorkConcept,
                            constructionhistory = null
                        where workconceptdbid = @workConceptDbId
                            and active = 1
                    ";
                SqlParameter[] prms2 = new SqlParameter[4];
                prms2[0] = new SqlParameter("@primaryWorkConceptCode", SqlDbType.VarChar);
                prms2[0].Value = primaryWorkConceptCode;
                prms2[1] = new SqlParameter("@primaryWorkConceptDescription", SqlDbType.VarChar);
                prms2[1].Value = primaryWorkConceptDescription;
                prms2[2] = new SqlParameter("@primaryWorkConcept", SqlDbType.VarChar);
                prms2[2].Value = primaryWorkConcept;
                prms2[3] = new SqlParameter("@workConceptDbId", SqlDbType.Int);
                prms2[3].Value = workConceptDbId;
                ExecuteInsertUpdateDelete(qry2, prms2, database.wisamsConnection);
            }
        }

        public void UpdateGeneratedDate()
        {
            string qry =
                @"
                    update eligibilitylist
                    set generateddate = @generatedDate
                    where generateddate is null
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@generatedDate", SqlDbType.Date);
            prms[0].Value = new DateTime(2019, 4, 17);
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        public void UpdateWorkActionCode(WorkConcept wc)
        {
            string qry = @"
                            update eligibilitylist
                            set primaryworkconceptcode = @conceptCode
                            where workconceptdbid = @dbid
                        ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@conceptCode", SqlDbType.VarChar);
            prms[0].Value = wc.WorkConceptCode.Trim();
            prms[1] = new SqlParameter("@dbid", SqlDbType.Int);
            prms[1].Value = wc.WorkConceptDbId;
            ExecuteInsertUpdateDelete(qry, prms, database.wisamsConnection);
        }

        public void UpdateRegionNumber()
        {
            string qry = @"
                            update eligibilitylist  
                            set regionnumber =  
                                case  
                                    when region = '1-SW' then '1' 
                                    when region = '2-SE' then '2'
                                    when region = '3-NE' then '3'
                                    when region = '4-NC' then '4'
                                    when region = '5-NW' then '5'
                                end
                        ";

            ExecuteInsertUpdateDelete(qry, database.wisamsConnection);
        }

        public void UpdateStructureProgramReviewCurrent()
        {
            string qry = @"
                            delete from structureprogramreviewcurrent
                        ";
            ExecuteInsertUpdateDelete(qry, database.wisamsConnection);

            string qry2 = @"
                            update structureprogramreviewcurrent
                            select 
                        ";
        }

        public string GetProposedWorkNotes(string structureId)
        {
            string notes = "";
            string qry =
                @"
                    select notes
                    from proposedlist
                    where structureid = @structureId
                    order by proposeddate desc
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, database.wisamsConnection);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                if (dr["notes"] != DBNull.Value && !String.IsNullOrEmpty(dr["notes"].ToString()))
                {
                    notes = dr["notes"].ToString();
                }
            }

            return notes;
        }
    }
}
