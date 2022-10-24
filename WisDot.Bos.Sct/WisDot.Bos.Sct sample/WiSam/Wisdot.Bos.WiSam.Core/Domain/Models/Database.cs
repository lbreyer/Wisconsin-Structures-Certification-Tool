using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration; // App.config, web.config
using System.Data;
using System.Data.SqlClient; // SQL Server
using Oracle.DataAccess.Client; // Oracle
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class Database
    {
        #region Database Connections and Configurations
        private static bool prodMode = true; //ConfigurationManager.AppSettings.GetValues("ProdMode")[0].Equals("true") ? true : false;
        private static SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["HsiProdOra"].ConnectionString);
        private static string password = CryptorEngine.Decrypt(csb.Password, true);
        private string hsiProdConnStr = ConfigurationManager.ConnectionStrings["HsiProdOra"].ConnectionString.Replace(csb.Password, password);
        private string hsiAccConnStr = ConfigurationManager.ConnectionStrings["HsiAccOra"].ConnectionString.Replace(csb.Password, password);
        private string hsiDevConnStr = ConfigurationManager.ConnectionStrings["HsiDevOra"].ConnectionString.Replace(csb.Password, password);

        private static SqlConnectionStringBuilder fiipsCsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["FiipsProdOra"].ConnectionString);
        private static string fiipsPassword = CryptorEngine.Decrypt(fiipsCsb.Password, true);
        private string fiipsProdConnStr = ConfigurationManager.ConnectionStrings["FiipsProdOra"].ConnectionString.Replace(fiipsCsb.Password, fiipsPassword);

        private static SqlConnectionStringBuilder aashtoWareProjectCsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["AashtoWareProjectProdOra"].ConnectionString);

        private static string awpEncrypted = CryptorEngine.Encrypt(aashtoWareProjectCsb.Password, true);

        private static string aashtoWareProjectPassword = CryptorEngine.Decrypt(aashtoWareProjectCsb.Password, true);
        private string aashtoWareProjectProdConnStr = ConfigurationManager.ConnectionStrings["AashtoWareProjectProdOra"].ConnectionString.Replace(aashtoWareProjectCsb.Password, aashtoWareProjectPassword);

        private static SqlConnectionStringBuilder wisamsCsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["WiSamProdSql"].ConnectionString);
        private static string wisamsPassword = CryptorEngine.Decrypt(wisamsCsb.Password, true);
        private string samDevConnStr = ConfigurationManager.ConnectionStrings["WiSamDevSql"].ConnectionString.Replace(wisamsCsb.Password, wisamsPassword);
        private string samProdConnStr = ConfigurationManager.ConnectionStrings["WiSamProdSql"].ConnectionString.Replace(wisamsCsb.Password, wisamsPassword);
        private string samTestConnStr = ConfigurationManager.ConnectionStrings["WiSamTestSql"].ConnectionString.Replace(wisamsCsb.Password, wisamsPassword);
        private string samConnStrCurrent;
        private string strProgRptConnStr = ConfigurationManager.ConnectionStrings["StructuresProgressReportAccess"].ConnectionString;
        private static string timesheetAccessDatabaseFilePath = "";

        private OracleConnection hsiConn;
        private OracleConnection fiipsConn;
        private OracleConnection aashtoWareProjectConn;
        private SqlConnection samConn;
        private OleDbConnection strProgRptConn;

        private List<MeasurementIndex> measurementIndices;
        private List<string> complexStructureConfigurations = new List<string>() { "34", "60", "80", "30", "31", "33", "69", "39", "32", "13", "90", "50", "61" };
        private List<int> wearingSurfaceElementsInSf = new List<int>() { 510, 8000, 8511, 8512, 8513, 8514, 8515 };
        private List<int> superstructureElementsInLf = new List<int>() { 102, 107, 113, 120, 141, 147, 148, 152, 105, 110, 116, 144, 155, 104, 109, 115, 143, 154, 111, 117, 135, 146, 156, 145, 106, 112, 118 };
        private List<int> superstructureElementsInSf = new List<int>() { 38, 8039, 54, 65 };
        private List<int> substructureElementsInLf = new List<int>() { 207, 219, 231, 210, 215, 220, 234, 233, 208, 212, 216, 235, 213, 217, 211, 218 };
        private List<StructureDeterioration> StructuresWithQualifiedDeteriorationCurves = new List<StructureDeterioration>();
        #endregion Database Connections and Configurations

        #region Constructors
        public Database()
        {
            //hsiConn = new OracleConnection(hsiDevConnStr);
            //hsiConn = new OracleConnection(hsiAccConnStr);
            hsiConn = new OracleConnection(hsiProdConnStr);
            samConn = new SqlConnection(samProdConnStr);
            samConnStrCurrent = samProdConnStr;
            //samConn = new SqlConnection(samDevConnStr);
            //samConnStrCurrent = samDevConnStr;
            //samConn = new SqlConnection(samTestConnStr);
            //samConnStrCurrent = samTestConnStr;
            fiipsConn = new OracleConnection(fiipsProdConnStr);
            try
            {
                aashtoWareProjectConn = new OracleConnection(aashtoWareProjectProdConnStr);
            }
            catch (Exception ex)
            {

            }

            strProgRptConn = new OleDbConnection();
            strProgRptConn.ConnectionString = strProgRptConnStr;
            timesheetAccessDatabaseFilePath =
                strProgRptConnStr
                .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)[1]
                .Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries)[1];
            OpenDbConnections();

            measurementIndices = GetMeasurementIndices();
        }

        public Database(WisamType.Databases hsi, WisamType.Databases wiSam)
        {
            fiipsConn = new OracleConnection(fiipsProdConnStr);

            if (hsi.Equals(WisamType.Databases.HsiProduction))
            {
                hsiConn = new OracleConnection(hsiProdConnStr);
            }
            else if (hsi.Equals(WisamType.Databases.HsiAcceptance))
            {
                hsiConn = new OracleConnection(hsiAccConnStr);
            }
            else
            {
                hsiConn = new OracleConnection(hsiDevConnStr);
            }

            if (wiSam.Equals(WisamType.Databases.WiSamProduction))
            {
                samConn = new SqlConnection(samProdConnStr);
                samConnStrCurrent = samProdConnStr;
            }
            else if (wiSam.Equals(WisamType.Databases.WiSamDev))
            {
                samConn = new SqlConnection(samDevConnStr);
                samConnStrCurrent = samDevConnStr;
            }
            else if (wiSam.Equals(WisamType.Databases.WiSamTest))
            {
                samConn = new SqlConnection(samTestConnStr);
                samConnStrCurrent = samTestConnStr;
            }

            OpenDbConnections();
        }



        #endregion Constructors

        #region DB Connection and SQL Execution Methods
        public bool UpdateTimesheetDbConnection(string newTimesheetAccessDatabaseFilePath)
        {
            bool updatedTimesheetDbConnection = true;
            strProgRptConnStr = strProgRptConnStr.Replace(timesheetAccessDatabaseFilePath, newTimesheetAccessDatabaseFilePath);

            try
            {
                strProgRptConn.Close();
                strProgRptConn.Dispose();
                strProgRptConn = new OleDbConnection();
                strProgRptConn.ConnectionString = strProgRptConnStr;

                if (strProgRptConn.State == ConnectionState.Closed || strProgRptConn.State == ConnectionState.Broken)
                {
                    try
                    {
                        strProgRptConn.Open();
                    }
                    catch (Exception ex)
                    { }
                }
            }
            catch (Exception ex)
            {
                updatedTimesheetDbConnection = false;
            }

            return updatedTimesheetDbConnection;
        }

        public bool UpdateDbConnections(WisamType.Databases wiSam)
        {
            bool changeDbConnection = false;
            string newConnStr = "";

            if (wiSam.Equals(WisamType.Databases.WiSamProduction))
            {
                if (!samConnStrCurrent.Equals(samProdConnStr))
                {
                    changeDbConnection = true;
                    newConnStr = samProdConnStr;
                }
            }
            else if (wiSam.Equals(WisamType.Databases.WiSamDev))
            {
                if (!samConnStrCurrent.Equals(samDevConnStr))
                {
                    changeDbConnection = true;
                    newConnStr = samDevConnStr;
                }
            }
            else if (wiSam.Equals(WisamType.Databases.WiSamTest))
            {
                if (!samConnStrCurrent.Equals(samTestConnStr))
                {
                    changeDbConnection = true;
                    newConnStr = samTestConnStr;
                }
            }

            if (changeDbConnection)
            {
                samConn.Close();
                samConn.Dispose();
                samConn = new SqlConnection(newConnStr);
                samConnStrCurrent = newConnStr;

                if (samConn.State == ConnectionState.Closed || samConn.State == ConnectionState.Broken)
                {
                    samConn.Open();
                }

                measurementIndices = GetMeasurementIndices();
            }

            return changeDbConnection;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenDbConnections()
        {
            if (hsiConn.State == ConnectionState.Closed || hsiConn.State == ConnectionState.Broken)
            {
                hsiConn.Open();
            }

            if (fiipsConn.State == ConnectionState.Closed || fiipsConn.State == ConnectionState.Broken)
            {
                fiipsConn.Open();
            }

            if (aashtoWareProjectConn.State == ConnectionState.Closed || aashtoWareProjectConn.State == ConnectionState.Broken)
            {
                aashtoWareProjectConn.Open();
            }

            if (samConn.State == ConnectionState.Closed || samConn.State == ConnectionState.Broken)
            {
                samConn.Open();
            }

            if (strProgRptConn.State == ConnectionState.Closed || strProgRptConn.State == ConnectionState.Broken)
            {
                try
                {
                    strProgRptConn.Open();
                }
                catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        public void CloseDbConnections()
        {
            hsiConn.Close();
            hsiConn.Dispose();
            fiipsConn.Close();
            fiipsConn.Dispose();
            aashtoWareProjectConn.Close();
            aashtoWareProjectConn.Dispose();
            samConn.Close();
            samConn.Dispose();
            strProgRptConn.Close();
            strProgRptConn.Dispose();
        }

        public void ExecuteInsertUpdateDelete(string qry, OleDbConnection conn)
        {
            try
            {
                OleDbCommand cmd1 = new OleDbCommand(qry, conn);
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

        public void ExecuteInsertUpdateDelete(string qry, OleDbParameter[] prms, OleDbConnection conn)
        {
            try
            {
                OleDbCommand cmd1 = new OleDbCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddRange(prms);
                cmd1.ExecuteNonQuery();
                //cmd1.Parameters.Clear();
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

        public DataTable ExecuteSelect(string qry, OleDbConnection conn)
        {
            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                OleDbCommand cmd1 = new OleDbCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                OleDbDataAdapter adp1 = new OleDbDataAdapter(cmd1);
                adp1.Fill(ds);
                adp1.Dispose();
                dt = ds.Tables[0];
            }
            catch (OleDbException e)
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
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Error - ExecuteSelect - Query: {0} \nException: {1}", qry, e.StackTrace.ToString());
            }
            finally { }

            return dt;
        }

        public DataTable ExecuteSelect(string qry, OracleConnection conn)
        {
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

        public DataTable ExecuteSelect(string qry, OleDbParameter[] prms, OleDbConnection conn)
        {
            DataTable dt = null;
            DataSet ds = new DataSet();

            try
            {
                OleDbCommand cmd1 = new OleDbCommand(qry, conn);
                cmd1.CommandType = CommandType.Text;
                cmd1.Parameters.AddRange(prms);
                OleDbDataAdapter adp1 = new OleDbDataAdapter(cmd1);
                adp1.Fill(ds);
                adp1.Dispose();
                dt = ds.Tables[0];
            }
            catch (OleDbException e)
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
        #endregion DB Connection and SQL Execution Methods

        #region CAI Methods
        public CaiFormula GetDefaultCaiFormula()
        {
            CaiFormula formula = null;
            string qry = @"
                                select CaiFormulaId, CaiFormulaDesc, CaiFormula, Active, ElementBasedOnly, DefaultFormula
                                from CaiFormula
                                where DefaultFormula = 1
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                formula = new CaiFormula();
                formula.CaiFormulaId = Convert.ToInt32(dt.Rows[0]["CAIFORMULAID"]);
                formula.CaiFormulaDesc = dt.Rows[0]["CAIFORMULADESC"].ToString();
                formula.Formula = dt.Rows[0]["CAIFORMULA"].ToString();
                formula.Active = Convert.ToBoolean(dt.Rows[0]["ACTIVE"]);
                formula.ElementBasedOnly = Convert.ToBoolean(dt.Rows[0]["ELEMENTBASEDONLY"]);
                formula.DefaultFormula = Convert.ToBoolean(dt.Rows[0]["DEFAULTFORMULA"]);
                formula.ElementClassCaiReductions = GetElemClassCaiReductions(formula.CaiFormulaId);
                formula.NbiCaiReductions = GetNbiClassCaiReductions(formula.CaiFormulaId);
            }

            return formula;
        }

        public CaiFormula GetCaiFormula(int cfId)
        {
            CaiFormula formula = null;
            string qry = @"
                                select CaiFormulaId, CaiFormulaDesc, CaiFormula, Active, ElementBasedOnly, DefaultFormula
                                from CaiFormula
                                where CaiFormulaId = @cfId
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@cfId", SqlDbType.Int);
            prms[0].Value = cfId;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                formula = new CaiFormula();
                formula.CaiFormulaId = Convert.ToInt32(dt.Rows[0]["CAIFORMULAID"]);
                formula.CaiFormulaDesc = dt.Rows[0]["CAIFORMULADESC"].ToString();
                formula.Formula = dt.Rows[0]["CAIFORMULA"].ToString();
                formula.Active = Convert.ToBoolean(dt.Rows[0]["ACTIVE"]);
                formula.ElementBasedOnly = Convert.ToBoolean(dt.Rows[0]["ELEMENTBASEDONLY"]);
                formula.DefaultFormula = Convert.ToBoolean(dt.Rows[0]["DEFAULTFORMULA"]);
                formula.ElementClassCaiReductions = GetElemClassCaiReductions(formula.CaiFormulaId);
                formula.NbiCaiReductions = GetNbiClassCaiReductions(formula.CaiFormulaId);
            }

            return formula;
        }

        public string GetCaiFormulaExpression(int cfId)
        {
            string formulaExpression = "";
            string qry = @"
                                select CaiFormulaId, CaiFormulaDesc, CaiFormula, Active
                                from CaiFormula
                                where CaiFormulaId = @cfId
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@cfId", SqlDbType.Int);
            prms[0].Value = cfId;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                formulaExpression = dt.Rows[0]["CAIFORMULA"].ToString();
            }

            return formulaExpression;
        }

        public string[] GetCaiFormulaVariables(int cfId)
        {
            CaiFormula cf = GetCaiFormula(cfId);
            return cf.Formula.Split(new string[5] { " ", "-", "+", "*", "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetCaiFormulaVariables(string formula)
        {
            return formula.Split(new string[5] { " ", "-", "+", "*", "/" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public List<ElementClassificationCaiReduction> GetElemClassCaiReductions(int cfId)
        {
            List<ElementClassificationCaiReduction> elemClassReductions = null;
            string fvList = ConvertToQuotedList(",", GetCaiFormulaVariables(GetCaiFormulaExpression(cfId)));
            string qry = @"
                                select ElementClassificationCode, CaiFormulaId, ReductionFormula
                                from ElementClassificationCaiReduction
                                where CaiFormulaId = @cfId
                            ";
            qry += " and ElementClassificationCode in (" + fvList + ")";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@cfId", SqlDbType.Int);
            prms[0].Value = cfId;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                elemClassReductions = new List<ElementClassificationCaiReduction>();

                foreach (DataRow row in dt.Rows)
                {
                    ElementClassificationCaiReduction eccr = new ElementClassificationCaiReduction();
                    eccr.CaiFormulaId = cfId;
                    eccr.ElementClassificationCode = row["ELEMENTCLASSIFICATIONCODE"].ToString();
                    eccr.ReductionFormula = row["REDUCTIONFORMULA"].ToString();
                    elemClassReductions.Add(eccr);
                }
            }

            return elemClassReductions;
        }
        #endregion CAI Methods

        #region Element and Inspection Methods

        public Inspection GetLastInspection(Structure str, bool interpolateNbi = false, bool getLastInspection = false)
        {
            Inspection lastInspection = null;
            List<Element> lastInspectionElements = GetLastInspectionElements(str.StructureId, getLastInspection);
            NbiRating nbiRatings = GetLastNbiRating(str.StructureId, getLastInspection);

            if (lastInspectionElements != null && nbiRatings != null)
            {
                lastInspection = new Inspection();
                lastInspection.StructureId = str.StructureId;
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

                if (!lastInspection.NbiRatings.CulvertRating.Equals("N"))
                {
                    str.IsCulvert = true;
                }

                if (!str.IsCulvert)
                {
                    str.DeckRatingInspected = lastInspection.NbiRatings.DeckRatingVal;
                    str.SuperstructureRatingInspected = lastInspection.NbiRatings.SuperstructureRatingVal;
                    str.SubstructureRatingInspected = lastInspection.NbiRatings.SubstructureRatingVal;
                }

                if (interpolateNbi && !str.IsCulvert)
                {
                    // Determine percentage of deck quantity in CS1
                    double totalQuantity = 0;
                    int totalQuantityCs1 = 0;
                    var wearingSurfaceElements = lastInspectionElements
                                                    .Where(el => wearingSurfaceElementsInSf.Contains(el.ElemNum));

                    foreach (var elem in wearingSurfaceElements)
                    {
                        totalQuantityCs1 += elem.Cs1Quantity;
                        totalQuantity += elem.TotalQuantity;
                    }

                    str.DeckPercentageCs1 = totalQuantityCs1 / totalQuantity * 100;

                    if (str.DeckPercentageCs1 > 0)
                    {
                        str.DeckRatingInterpolated = (str.DeckPercentageCs1 / 100 - 0.1105) / 0.1034;

                        if (str.DeckRatingInterpolated > lastInspection.NbiRatings.DeckRatingVal)
                        {
                            str.DeckRatingInterpolated = lastInspection.NbiRatings.DeckRatingVal;
                        }
                        else if (str.DeckRatingInterpolated < lastInspection.NbiRatings.DeckRatingVal - 1)
                        {
                            str.DeckRatingInterpolated = lastInspection.NbiRatings.DeckRatingVal - 1;
                        }

                        lastInspection.NbiRatings.DeckRatingVal = str.DeckRatingInterpolated;
                        lastInspection.NbiRatings.DeckRating = Convert.ToString(str.DeckRatingInterpolated);
                    }
                    else
                    {
                        str.DeckRatingInterpolated = lastInspection.NbiRatings.DeckRatingVal;
                    }

                    lastInspection.NbiRatings.DeckDeteriorationYear = GetNbiDeteriorationYear(Code.NbiDeck, lastInspection.NbiRatings.DeckRatingVal, lastInspection.StructureId);

                    // Exclude steel structures from NBI superstructure interpolation
                    if (!str.MainSpanMaterial.ToUpper().Contains("STEEL"))
                    {
                        // Determine percentage of superstructure quantity in CS1
                        totalQuantity = 0;
                        totalQuantityCs1 = 0;
                        bool slabStructure = false;
                        var superstructureElements = lastInspection.Elements
                                                            .Where(el => superstructureElementsInSf.Contains(el.ElemNum));

                        if (superstructureElements.Count() == 0)
                        {
                            superstructureElements = lastInspection.Elements
                                                                .Where(el => superstructureElementsInLf.Contains(el.ElemNum));
                        }
                        else
                        {
                            slabStructure = true;
                        }

                        foreach (var elem in superstructureElements)
                        {
                            totalQuantityCs1 += elem.Cs1Quantity;
                            totalQuantity += elem.TotalQuantity;
                        }

                        str.SuperstructurePercentageCs1 = totalQuantityCs1 / totalQuantity * 100;

                        if (str.SuperstructurePercentageCs1 > 0)
                        {
                            if (slabStructure)
                            {
                                str.SuperstructureRatingInterpolated = (str.SuperstructurePercentageCs1 / 100 - 0.2226) / 0.0956;
                            }
                            else
                            {
                                str.SuperstructureRatingInterpolated = (str.SuperstructurePercentageCs1 / 100 + 0.2459) / 0.145;
                            }

                            if (str.SuperstructureRatingInterpolated > lastInspection.NbiRatings.SuperstructureRatingVal)
                            {
                                str.SuperstructureRatingInterpolated = lastInspection.NbiRatings.SuperstructureRatingVal;
                            }
                            else if (str.SuperstructureRatingInterpolated < lastInspection.NbiRatings.SuperstructureRatingVal - 1)
                            {
                                str.SuperstructureRatingInterpolated = lastInspection.NbiRatings.SuperstructureRatingVal - 1;
                            }

                            lastInspection.NbiRatings.SuperstructureRatingVal = str.SuperstructureRatingInterpolated;
                            lastInspection.NbiRatings.SuperstructureRating = Convert.ToString(str.SuperstructureRatingInterpolated);
                        }
                        else
                        {
                            str.SuperstructureRatingInterpolated = lastInspection.NbiRatings.SuperstructureRatingVal;
                        }
                    }

                    lastInspection.NbiRatings.SuperstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSuperstructure, lastInspection.NbiRatings.SuperstructureRatingVal, lastInspection.StructureId);

                    // Determine percentage of substructure quantity in CS1
                    totalQuantity = 0;
                    totalQuantityCs1 = 0;
                    var substructureElements = lastInspection.Elements
                                                    .Where(el => substructureElementsInLf.Contains(el.ElemNum));

                    foreach (var elem in substructureElements)
                    {
                        totalQuantityCs1 += elem.Cs1Quantity;
                        totalQuantity += elem.TotalQuantity;
                    }

                    str.SubstructurePercentageCs1 = totalQuantityCs1 / totalQuantity * 100;

                    if (str.SubstructurePercentageCs1 > 0)
                    {
                        str.SubstructureRatingInterpolated = (str.SubstructurePercentageCs1 / 100 + 0.0175) / 0.121;

                        if (str.SubstructureRatingInterpolated > lastInspection.NbiRatings.SubstructureRatingVal)
                        {
                            str.SubstructureRatingInterpolated = lastInspection.NbiRatings.SubstructureRatingVal;
                        }
                        else if (str.SubstructureRatingInterpolated < lastInspection.NbiRatings.SubstructureRatingVal - 1)
                        {
                            str.SubstructureRatingInterpolated = lastInspection.NbiRatings.SubstructureRatingVal - 1;
                        }

                        lastInspection.NbiRatings.SubstructureRatingVal = str.SubstructureRatingInterpolated;
                        lastInspection.NbiRatings.SubstructureRating = Convert.ToString(str.SubstructureRatingInterpolated);
                    }
                    else
                    {
                        str.SubstructureRatingInterpolated = lastInspection.NbiRatings.SubstructureRatingVal;
                    }

                    lastInspection.NbiRatings.SubstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSubstructure, lastInspection.NbiRatings.SubstructureRatingVal, lastInspection.StructureId);
                }

            }

            //var els = lastInspection.Elements.Where(e => e.ElemNum == 1080);
            return lastInspection;
        }

        public List<CoreInspection> GetCoreInspections(string strId)
        {
            List<CoreInspection> coreInspections = new List<CoreInspection>();

            string qry =
                @"
                    select insp.stin_id, insp.stin_dt, insp.strc_id, insp.stin_culv_rtg_cd, insp.stin_deck_cond_rtg,
                        insp.stin_spsr_cond_rtg, insp.stin_sbsr_cond_rtg, inspelem.stin_elmt_envr_nb,
                        inspelem.stin_elmt_cd, inspelem.stin_elmt_envr_nb,
                        inspelem.elmt_st_one_qty, inspelem.elmt_st_two_qty, inspelem.elmt_st_thre_qty,
                        inspelem.elmt_st_four_qty, inspelem.elmt_st_five_qty,
                        elem.stin_elmt_desc, elem.stin_elmt_long_txt, elem.stin_elmt_st_cnt,
                        elem.std_elmt_uom
                    from dot1stro.dt_strc_insp insp, dot1stro.dt_stin_item inspelem, dot1stro.dt_stin_elmt elem
                    where insp.strc_id = :strId
                        and insp.stin_id = inspelem.stin_id
                        and inspelem.stin_elmt_cd = elem.stin_elmt_cd
                    order by stin_id, stin_dt, inspelem.stin_elmt_cd
                ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                int previousStinId = -1;
                int currentStinId = 0;
                CoreInspection coreInsp = null;

                foreach (DataRow dr in dt.Rows)
                {
                    currentStinId = Convert.ToInt32(dr["stin_id"]);

                    if (currentStinId != previousStinId)
                    {
                        coreInsp = new CoreInspection();
                        coreInsp.StructureId = strId;
                        coreInsp.Elements = new List<CoreElement>();
                        coreInsp.InspectionDate = Convert.ToDateTime(dr["stin_dt"]);
                        coreInspections.Add(coreInsp);
                    }

                    CoreElement ce = new CoreElement();
                    ce.ElemNum = Convert.ToInt32(dr["stin_elmt_cd"]);
                    ce.ElemName = Convert.ToString(dr["stin_elmt_desc"]);
                    ce.StateCount = Convert.ToInt32(dr["stin_elmt_st_cnt"]);
                    ce.StructureId = strId;
                    ce.UnitOfMeasurement = Convert.ToString(dr["std_elmt_uom"]);
                    ce.EnvironmentNumber = Convert.ToInt32(dr["stin_elmt_envr_nb"]);

                    try
                    {
                        ce.Cs1Quantity = Convert.ToInt32(dr["elmt_st_one_qty"]);
                    }
                    catch { }

                    try
                    {
                        ce.Cs2Quantity = Convert.ToInt32(dr["elmt_st_two_qty"]);
                    }
                    catch { }

                    try
                    {
                        ce.Cs3Quantity = Convert.ToInt32(dr["elmt_st_thre_qty"]);
                    }
                    catch { }

                    try
                    {
                        ce.Cs4Quantity = Convert.ToInt32(dr["elmt_st_four_qty"]);
                    }
                    catch { }

                    try
                    {
                        ce.Cs5Quantity = Convert.ToInt32(dr["elmt_st_five_qty"]);
                    }
                    catch { }

                    ce.TotalQuantity = ce.Cs1Quantity + ce.Cs2Quantity + ce.Cs3Quantity + ce.Cs4Quantity + ce.Cs5Quantity;
                    coreInsp.Elements.Add(ce);
                    previousStinId = currentStinId;
                }
            }

            return coreInspections;
        }

        public Inspection GetLastInspection(string strId)
        {
            Inspection lastInspection = null;
            List<Element> lastInspectionElements = GetLastInspectionElements(strId);
            NbiRating nbiRatings = GetLastNbiRating(strId);

            if (lastInspectionElements != null && nbiRatings != null)
            {
                lastInspection = new Inspection();
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

            return lastInspection;
        }

        public List<Element> GetLastInspectionElements(string strId, bool getLastInspection = false)
        {
            List<Element> elems = null;
            /*
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
                            ";*/
            string qry = @"
                                select insp.strc_id, insp.stin_id, lastInsp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    (select strc_id, max(stin_dt) stin_dt, max(insp.stin_id) maxstinid 
                                        from dot1stro.dt_strc_insp insp, dot1stro.dt_insp_insp_ty insptype
                                        where insp.stin_id in (select stin_id from dot1stro.dt_isel_item)
                                            and insp.stin_id = insptype.stin_id
                                            and insptype.stin_tycd in ('R','C')
                                        group by strc_id) lastInsp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.strc_id = lastInsp.strc_id 
                                    and insp.stin_dt = lastInsp.stin_dt 
                                    and insp.stin_id = elem.stin_id 
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                                    and dt_isel_clsn.isel_clsn_id in ('ABUT','APP','ARCH','BEAM','BEAR','CULV','DECK','DFCT','GIRD','JNT','OLAY','OTHR','PIER','PNT','RAIL','TRUS','WALL','SLAB','SUB')
                                order by elem.isel_tyid
                            ";

            if (getLastInspection)
            {
                qry = @"
                                select insp.strc_id, insp.stin_id, lastInsp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    (select strc_id, max(stin_dt) stin_dt, max(insp.stin_id) maxstinid 
                                        from dot1stro.dt_strc_insp insp, dot1stro.dt_insp_insp_ty insptype
                                        where insp.stin_id in (select stin_id from dot1stro.dt_isel_item)
                                            and insp.stin_id = insptype.stin_id
                                        group by strc_id) lastInsp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.strc_id = lastInsp.strc_id 
                                    and insp.stin_dt = lastInsp.stin_dt 
                                    and insp.stin_id = elem.stin_id 
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                                    and dt_isel_clsn.isel_clsn_id in ('ABUT','APP','ARCH','BEAM','BEAR','CULV','DECK','DFCT','GIRD','JNT','OLAY','OTHR','PIER','PNT','RAIL','TRUS','WALL','SLAB','SUB')
                                order by elem.isel_tyid
                            ";
            }
            // and insp.stin_tycd not in ('ED')
            // took out qualifier - and elem.stin_id = lastInsp.maxstinid
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                elems = new List<Element>();

                foreach (DataRow row in dt.Rows)
                {

                    Element elem = new Element(Convert.ToInt32(row["ISEL_TYID"]));
                    elem.ElementClassificationCode = row["ISEL_CLSN_ID"].ToString();
                    elem.ElemName = row["ISEL_NM"].ToString();

                    if (row["FST_COND_ST_QTY"] != DBNull.Value)
                    {
                        elem.Cs1Quantity = Convert.ToInt32(row["FST_COND_ST_QTY"]);
                    }

                    if (row["SCND_COND_ST_QTY"] != DBNull.Value)
                    {
                        elem.Cs2Quantity = Convert.ToInt32(row["SCND_COND_ST_QTY"]);
                    }

                    if (row["THRD_COND_ST_QTY"] != DBNull.Value)
                    {
                        elem.Cs3Quantity = Convert.ToInt32(row["THRD_COND_ST_QTY"]);
                    }

                    if (row["FRTH_COND_ST_QTY"] != DBNull.Value)
                    {
                        elem.Cs4Quantity = Convert.ToInt32(row["FRTH_COND_ST_QTY"]);
                    }

                    if (!elem.ElementClassificationCode.Equals("DFCT"))
                    {
                        elem.TotalQuantity = elem.Cs1Quantity + elem.Cs2Quantity + elem.Cs3Quantity + elem.Cs4Quantity;
                    }

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
                foreach (Element elem in elems.Where(e => e.MainIselId != 0).ToList())
                {
                    if (elems.Where(e => e.IselItemId == elem.MainIselId).Count() > 0)
                    {
                        var parentElem = elems.Where(e => e.IselItemId == elem.MainIselId).First();
                        elem.ParentElemNum = parentElem.ElemNum;

                        if (elem.ElementClassificationCode.Equals("DFCT"))
                        {
                            elem.Cs1Quantity = parentElem.TotalQuantity - elem.Cs2Quantity - elem.Cs3Quantity - elem.Cs4Quantity;
                            elem.TotalQuantity = elem.Cs1Quantity + elem.Cs2Quantity + elem.Cs3Quantity + elem.Cs4Quantity;
                        }
                    }
                }
            }

            //var els = elems.Where(e => e.ElemNum == 1080);
            return elems;
            /*
            OracleCommand cmd1 = new OracleCommand(qry1, hsiConn);
            cmd1.CommandType = CommandType.Text;
            cmd1.Parameters.Add("strId", strId);
            OracleDataAdapter adp1 = new OracleDataAdapter(cmd1);
            adp1.TableMappings.Add("Table", "Elements");
            DataSet ds1 = new DataSet("Elements");
            adp1.Fill(ds1);
            adp1.Dispose();
            */
        }

        public List<Element> GetCaiLastInspectionElements(string strId, int cfId)
        {
            List<Element> allElems = GetLastInspectionElements(strId);
            List<Element> caiElems = new List<Element>();
            List<string> caiFormulaVariables = GetCaiFormulaVariables(cfId).ToList();

            if (caiFormulaVariables.Count() > 0)
            {
                foreach (Element elem in allElems)
                {
                    if (caiFormulaVariables.FindIndex(e => e == elem.ElementClassificationCode) >= 0)
                    {
                        caiElems.Add(elem);
                    }
                }
            }
            else
            {
                caiElems = allElems;
            }

            return caiElems;
        }

        public List<Element> GetElements(string strId, int year)
        {
            List<Element> elems = new List<Element>();
            string qry = @"
                                select insp.strc_id, insp.stin_id, lastInsp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    (select strc_id, max(stin_dt) stin_dt, max(stin_id) maxstinid 
                                        from dot1stro.dt_strc_insp 
                                        where stin_id in (select stin_id from dot1stro.dt_isel_item) 
                                            and extract(year from stin_dt) = :year
                                        group by strc_id) lastInsp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.strc_id = lastInsp.strc_id 
                                    and insp.stin_dt = lastInsp.stin_dt 
                                    and insp.stin_id = elem.stin_id 
                                    and elem.stin_id = lastInsp.maxstinid
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                                order by elem.isel_tyid
                            ";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("year", OracleDbType.Int32);
            prms[0].Value = year;
            prms[1] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[1].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime lastInspectionDate = GetLastInspectionDate(strId);

                foreach (DataRow row in dt.Rows)
                {
                    Element elem = new Element(Convert.ToInt32(row["ISEL_TYID"]));
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

                    if (lastInspectionDate.Year == elem.InspectionDate.Year)
                    {
                        elem.DeteriorationYear = 0;
                    }
                    elem.IselItemId = Convert.ToInt32(row["ISEL_ITEM_ID"]);

                    if (!String.IsNullOrEmpty(row["MAIN_ISEL_ID"].ToString()))
                    {
                        elem.MainIselId = Convert.ToInt32(row["MAIN_ISEL_ID"]);
                    }

                    elems.Add(elem);
                }

                // Assign parent element number to elements with a parent
                foreach (Element elem in elems.Where(e => e.MainIselId != 0).ToList())
                {
                    elem.ParentElemNum = elems.Where(e => e.IselItemId == elem.MainIselId).First().ElemNum;
                }
            }

            return elems;
        }

        public List<Element> GetElements(string strId, DateTime inspectionDate)
        {
            List<Element> elems = new List<Element>();
            string qry = @"
                                select insp.strc_id, insp.stin_id, insp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.stin_dt = :inspectionDate
                                    and insp.stin_id = elem.stin_id 
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                                order by elem.isel_tyid
                            ";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            prms[1] = new OracleParameter("inspectionDate", OracleDbType.Date);
            prms[1].Value = inspectionDate;

            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime lastInspectionDate = GetLastInspectionDate(strId);

                foreach (DataRow row in dt.Rows)
                {
                    Element elem = new Element(Convert.ToInt32(row["ISEL_TYID"]));
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

                    if (lastInspectionDate.Year == elem.InspectionDate.Year)
                    {
                        elem.DeteriorationYear = 0;
                    }

                    elem.IselItemId = Convert.ToInt32(row["ISEL_ITEM_ID"]);

                    if (!String.IsNullOrEmpty(row["MAIN_ISEL_ID"].ToString()))
                    {
                        elem.MainIselId = Convert.ToInt32(row["MAIN_ISEL_ID"]);
                    }

                    elems.Add(elem);
                }

                // Assign parent element number to elements with a parent
                foreach (Element elem in elems.Where(e => e.MainIselId != 0).ToList())
                {
                    elem.ParentElemNum = elems.Where(e => e.IselItemId == elem.MainIselId).First().ElemNum;
                }
            }

            return elems;
        }

        public List<Element> GetElements(string strId, int cfId, int year)
        {
            List<Element> elems = new List<Element>();
            string fvList = ConvertToQuotedList(",", GetCaiFormulaVariables(GetCaiFormulaExpression(cfId)));
            string qry = @"
                                select insp.strc_id, insp.stin_id, lastInsp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    (select strc_id, max(stin_dt) stin_dt, max(stin_id) maxstinid 
                                        from dot1stro.dt_strc_insp 
                                        where stin_id in (select stin_id from dot1stro.dt_isel_item) 
                                            and extract(year from stin_dt) = :year
                                        group by strc_id) lastInsp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.strc_id = lastInsp.strc_id 
                                    and insp.stin_dt = lastInsp.stin_dt 
                                    and insp.stin_id = elem.stin_id 
                                    and elem.stin_id = lastInsp.maxstinid
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                            ";
            qry += " and dt_isel_clsn.isel_clsn_id in (" + fvList + ") order by elem.isel_tyid";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("year", OracleDbType.Int32);
            prms[0].Value = year;
            prms[1] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[1].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime lastInspectionDate = GetLastInspectionDate(strId);

                foreach (DataRow row in dt.Rows)
                {
                    Element elem = new Element(Convert.ToInt32(row["ISEL_TYID"]));
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

                    if (lastInspectionDate.Year == elem.InspectionDate.Year)
                    {
                        elem.DeteriorationYear = 0;
                    }

                    elem.IselItemId = Convert.ToInt32(row["ISEL_ITEM_ID"]);

                    if (!String.IsNullOrEmpty(row["MAIN_ISEL_ID"].ToString()))
                    {
                        elem.MainIselId = Convert.ToInt32(row["MAIN_ISEL_ID"]);
                    }

                    elems.Add(elem);
                }

                // Assign parent element number to elements with a parent
                foreach (Element elem in elems.Where(e => e.MainIselId != 0).ToList())
                {
                    elem.ParentElemNum = elems.Where(e => e.IselItemId == elem.MainIselId).First().ElemNum;
                }
            }

            return elems;
        }

        public List<Element> GetElements(string strId, int cfId, DateTime inspectionDate)
        {
            List<Element> elems = new List<Element>();
            string fvList = ConvertToQuotedList(",", GetCaiFormulaVariables(GetCaiFormulaExpression(cfId)));

            string qry = @"
                                select insp.strc_id, insp.stin_id, insp.stin_dt, elem.isel_tyid, isel.isel_nm, isel.main_or_sub_indc, elem.main_isel_id, elem.insp_envr_id, 
                                    elem.fst_cond_st_qty, elem.scnd_cond_st_qty, elem.thrd_cond_st_qty, elem.frth_cond_st_qty,
                                    dt_isel_clsn.isel_clsn_id,
                                    isel_item_id
                                from dot1stro.dt_strc_insp insp, 
                                    dot1stro.dt_isel_item elem, 
                                    dot1stro.dt_isel_ty isel,
                                    dot1stro.dt_isel_clsn
                                where insp.strc_id = :strId 
                                    and insp.stin_dt = :inspectionDate
                                    and insp.stin_id = elem.stin_id 
                                    and isel.isel_tyid = elem.isel_tyid 
                                    and elem.isel_tyid = dt_isel_clsn.isel_tyid
                            ";
            qry += " and dt_isel_clsn.isel_clsn_id in (" + fvList + ") order by elem.isel_tyid";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            prms[1] = new OracleParameter("inspectionDate", OracleDbType.Date);
            prms[1].Value = inspectionDate;

            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime lastInspectionDate = GetLastInspectionDate(strId);

                foreach (DataRow row in dt.Rows)
                {
                    Element elem = new Element(Convert.ToInt32(row["ISEL_TYID"]));
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

                    if (lastInspectionDate.Year == elem.InspectionDate.Year)
                    {
                        elem.DeteriorationYear = 0;
                    }

                    elem.IselItemId = Convert.ToInt32(row["ISEL_ITEM_ID"]);

                    if (!String.IsNullOrEmpty(row["MAIN_ISEL_ID"].ToString()))
                    {
                        elem.MainIselId = Convert.ToInt32(row["MAIN_ISEL_ID"]);
                    }

                    elems.Add(elem);
                }

                // Assign parent element number to elements with a parent
                foreach (Element elem in elems.Where(e => e.MainIselId != 0).ToList())
                {
                    try
                    {
                        elem.ParentElemNum = elems.Where(e => e.IselItemId == elem.MainIselId).First().ElemNum;
                    }
                    catch { }
                }
            }

            return elems;
        }

        public int GetNumberOfInspectionsOfAGivenType(string strId, string inspectionType)
        {
            int numOfInspections = 0;
            string qry = @"
                                select strc_id, inspty.stin_tycd
                                from dot1stro.dt_strc_insp insp, dot1stro.dt_insp_insp_ty inspty
                                where insp.strc_id = :strId
                                    and insp.stin_id = inspty.stin_id
                                    and inspty.stin_tycd = :inspectionType
                            ";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            prms[1] = new OracleParameter("inspectionType", OracleDbType.Varchar2);
            prms[1].Value = inspectionType;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null)
            {
                numOfInspections = dt.Rows.Count;
            }

            return numOfInspections;
        }

        public Inspection GetCurrentInspection(string strId)
        {
            Inspection currentInspection = new Inspection();
            currentInspection.NbiRatings = GetLastNbiRating(strId);
            currentInspection.Elements = GetLastInspectionElements(strId);

            if (currentInspection.Elements != null)
            {
                currentInspection.StructureId = strId;
                currentInspection.InspectionDate = currentInspection.Elements[0].InspectionDate;
            }

            return currentInspection;
        }

        public List<DateTime> GetInspectionDatesAsc(string strId)
        {
            List<DateTime> inspDates = new List<DateTime>();
            string qry = @"
                                select distinct stin_dt
                                from dot1stro.dt_strc_insp insp
                                where insp.strc_id = :strId
                                order by stin_dt asc
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    inspDates.Add(Convert.ToDateTime(dr["STIN_DT"]));
                }
            }

            return inspDates;
        }

        public List<DateTime> GetElementInspectionDatesAsc(string strId)
        {
            List<DateTime> inspDates = new List<DateTime>();
            string qry = @"
                                select distinct stin_dt
                                from dot1stro.dt_strc_insp insp, dot1stro.dt_isel_item elem
                                where insp.strc_id = :strId
                                    and insp.stin_id = elem.stin_id
                                order by stin_dt asc
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    inspDates.Add(Convert.ToDateTime(dr["STIN_DT"]));
                }
            }

            return inspDates;
        }

        public List<DateTime> GetElementInspectionDates(string strId)
        {
            List<DateTime> inspDates = new List<DateTime>();
            string qry = @"
                                select distinct insp.stin_id, stin_dt, insptype.stin_tycd
                                from dot1stro.dt_strc_insp insp, dot1stro.dt_isel_item elem, dot1stro.dt_insp_insp_ty insptype
                                where insp.strc_id = :strId
                                    and insp.stin_id = elem.stin_id
                                    and insp.stin_id = insptype.stin_id
                                    and insptype.stin_tycd in ('R','C')
                                order by stin_dt desc
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    inspDates.Add(Convert.ToDateTime(dr["STIN_DT"]));
                }
            }

            return inspDates;
        }

        public DateTime GetLastInspectionDate(string strId)
        {
            DateTime currentInspectionDate = new DateTime(1, 1, 1);
            string qry = @"
                                select insp.strc_id, insp.stin_id, lastInsp.stin_dt
                                from dot1stro.dt_strc_insp insp,
                                    (select strc_id, max(stin_dt) stin_dt, max(stin_id) maxstinid
                                        from dot1stro.dt_strc_insp
                                        where stin_id in (select stin_id from dot1stro.dt_isel_item)
                                        group by strc_id) lastInsp
                                where insp.strc_id = :strId
                                    and insp.strc_id = lastInsp.strc_id
                                    and insp.stin_dt = lastInsp.stin_dt
                                    and insp.stin_id = lastInsp.maxstinid
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                currentInspectionDate = Convert.ToDateTime(dt.Rows[0]["STIN_DT"]);
            }

            return currentInspectionDate;
        }

        public List<ElementClassification> GetElemClassifications()
        {
            List<ElementClassification> elemClassifications = new List<ElementClassification>();
            string qry = @"
                                select ElementClassificationCode, ElementClassificationName
                                from ElementClassification
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ElementClassification ec = new ElementClassification();
                    ec.ElementClassificationCode = row["ELEMENTCLASSIFICATIONCODE"].ToString();
                    ec.ElementClassificationName = row["ELEMENTCLASSIFICATIONNAME"].ToString();
                    elemClassifications.Add(ec);
                }
            }

            return elemClassifications;
        }

        public ElementDeterioration GetElementDeterioration(int elemNum)
        {
            ElementDeterioration elemDeter = null;
            string qry = @"
                                select ElementNumber, RelativeWeight, MedYr1, MedYr2, MedYr3, Beta, Active
                                from ElementDeterioration
                                where ElementNumber = @elemNum
                                    and Active = 1
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@elemNum", SqlDbType.Int);
            prms[0].Value = elemNum;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                var dr = dt.Rows[0];
                elemDeter = new ElementDeterioration();
                elemDeter.ElemNum = elemNum;
                elemDeter.Beta = Convert.ToSingle(dr["BETA"]);
                elemDeter.RelativeWeight = Convert.ToSingle(dr["RELATIVEWEIGHT"]);
                elemDeter.MedYr1 = Convert.ToSingle(dr["MEDYR1"]);
                elemDeter.MedYr2 = Convert.ToSingle(dr["MEDYR2"]);
                elemDeter.MedYr3 = Convert.ToSingle(dr["MEDYR3"]);
                elemDeter.Active = Convert.ToBoolean(dr["ACTIVE"]);
                double ln2 = Math.Log(2, Math.E);
                double power = 1 / elemDeter.Beta;
                elemDeter.ScalingFactor1 = Convert.ToSingle(elemDeter.MedYr1 / (Math.Pow(ln2, power)));
                elemDeter.ScalingFactor2 = Convert.ToSingle(elemDeter.MedYr2 / (Math.Pow(ln2, power)));
                elemDeter.ScalingFactor3 = Convert.ToSingle(elemDeter.MedYr3 / (Math.Pow(ln2, power)));
            }

            return elemDeter;
        }

        public List<ElementClassificationDeterioration> GetElemClassificationsDeterioration()
        {
            List<ElementClassificationDeterioration> ecds = new List<ElementClassificationDeterioration>();
            string qry = @"
                                select ElementClassificationCode, MedYr1, MedYr2, MedYr3, Beta, Active
                                from ElementClassificationDeterioration
                                where Active = 1
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ElementClassificationDeterioration ecd = new ElementClassificationDeterioration();
                    ecd.ElementClassificationCode = row["ELEMENTCLASSIFICATIONCODE"].ToString();
                    ecd.MedYr1 = float.Parse(row["MEDYR1"].ToString());
                    ecd.MedYr2 = float.Parse(row["MEDYR2"].ToString());
                    ecd.MedYr3 = float.Parse(row["MEDYR3"].ToString());
                    ecd.Beta = float.Parse(row["BETA"].ToString());
                    ecd.Active = true;
                    ecds.Add(ecd);
                }
            }

            return ecds;
        }
        #endregion Element and Inspection Methods

        #region NBI Methods
        public NbiRating GetNbiRating(string strId, DateTime inspectionDate)
        {
            NbiRating nr = null;
            string qry = @"
                                select StrcInsp.strc_id,
                                        stin_dt,
                                        stin_deck_cond_rtg,
                                        stin_spsr_cond_rtg,
                                        stin_sbsr_cond_rtg,
                                        stin_culv_rtg_cd,
                                        InspType.stin_tycd,
                                        InspType.stin_tydc
                                from dot1stro.dt_strc_insp StrcInsp, dot1stro.dt_stin_ty InspType
                                where StrcInsp.strc_id = :strId
                                    and StrcInsp.stin_dt = :inspectionDate
                                    and StrcInsp.stin_tycd = InspType.stin_tycd(+)
                            ";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            prms[1] = new OracleParameter("year", OracleDbType.Date);
            prms[1].Value = inspectionDate;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                nr = new NbiRating();
                DataRow dr = dt.Rows[0];
                nr.StructureId = dr["STRC_ID"].ToString();
                nr.InspectionDate = DateTime.Parse(dr["STIN_DT"].ToString());
                nr.DeckRating = dr["STIN_DECK_COND_RTG"].ToString();
                nr.SuperstructureRating = dr["STIN_SPSR_COND_RTG"].ToString();
                nr.SubstructureRating = dr["STIN_SBSR_COND_RTG"].ToString();
                nr.CulvertRating = dr["STIN_CULV_RTG_CD"].ToString();

                int rating;
                if (Int32.TryParse(nr.DeckRating, out rating))
                {
                    try
                    {
                        nr.DeckDeteriorationYear = GetNbiDeteriorationYear(Code.NbiDeck, rating, strId);
                        nr.DeckRatingVal = Convert.ToDouble(nr.DeckRating);
                    }
                    catch { }
                }

                if (Int32.TryParse(nr.SuperstructureRating, out rating))
                {
                    try
                    {
                        nr.SuperstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSuperstructure, rating, strId);
                        nr.SuperstructureRatingVal = Convert.ToDouble(nr.SuperstructureRating);
                    }
                    catch { }
                }

                if (Int32.TryParse(nr.SubstructureRating, out rating))
                {
                    try
                    {
                        nr.SubstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSubstructure, rating, strId);
                        nr.SubstructureRatingVal = Convert.ToDouble(nr.SubstructureRating);
                    }
                    catch { }
                }

                if (Int32.TryParse(nr.CulvertRating, out rating))
                {
                    try
                    {
                        nr.CulvertDeteriorationYear = GetNbiDeteriorationYear(Code.NbiCulvert, rating, strId);
                        nr.CulvertRatingVal = Convert.ToDouble(nr.CulvertRating);
                    }
                    catch { }
                }

                if (!String.IsNullOrEmpty(dr["STIN_TYCD"].ToString().Trim()))
                {
                    nr.InspectionTypeCode = dr["STIN_TYCD"].ToString().Trim();
                    nr.InspectionTypeDescription = dr["STIN_TYDC"].ToString().Trim();
                }
            }

            return nr;
        }

        public NbiRating GetNbiRating(string strId, int year)
        {
            NbiRating nr = null;
            string qry = @"
                                select StrcInsp.strc_id,
                                        stin_dt,
                                        stin_deck_cond_rtg,
                                        stin_spsr_cond_rtg,
                                        stin_sbsr_cond_rtg,
                                        stin_culv_rtg_cd
                                from dot1stro.dt_strc_insp StrcInsp
                                where StrcInsp.strc_id = :strId
                                    and StrcInsp.stin_dt = (select max(stin_dt) MaxDate
                                                                from dot1stro.dt_strc_insp MaxPrev
                                                                where StrcInsp.strc_id = MaxPrev.strc_id
                                                                    and extract(year from stin_dt) = :year)
                            ";
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            prms[1] = new OracleParameter("year", OracleDbType.Int32);
            prms[1].Value = year;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                nr = new NbiRating();
                DataRow dr = dt.Rows[0];
                nr.StructureId = dr["STRC_ID"].ToString();
                nr.InspectionDate = DateTime.Parse(dr["STIN_DT"].ToString());
                nr.DeckRating = dr["STIN_DECK_COND_RTG"].ToString();
                nr.SuperstructureRating = dr["STIN_SPSR_COND_RTG"].ToString();
                nr.SubstructureRating = dr["STIN_SBSR_COND_RTG"].ToString();
                nr.CulvertRating = dr["STIN_CULV_RTG_CD"].ToString();

                int rating;
                if (Int32.TryParse(nr.DeckRating, out rating))
                {
                    nr.DeckDeteriorationYear = GetNbiDeteriorationYear(Code.NbiDeck, rating, strId);
                    nr.DeckRatingVal = Convert.ToDouble(nr.DeckRating);
                }

                if (Int32.TryParse(nr.SuperstructureRating, out rating))
                {
                    nr.SuperstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSuperstructure, rating, strId);
                    nr.SuperstructureRatingVal = Convert.ToDouble(nr.SuperstructureRating);
                }

                if (Int32.TryParse(nr.SubstructureRating, out rating))
                {
                    nr.SubstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSubstructure, rating, strId);
                    nr.SubstructureRatingVal = Convert.ToDouble(nr.SubstructureRating);
                }

                if (Int32.TryParse(nr.CulvertRating, out rating))
                {
                    nr.CulvertDeteriorationYear = GetNbiDeteriorationYear(Code.NbiCulvert, rating, strId);
                    nr.CulvertRatingVal = Convert.ToDouble(nr.CulvertRating);
                }
            }

            return nr;
        }

        /*
        public NbiRating GetLastNbiRating(Structure str)
        {
            NbiRating nr = GetLastNbiRating(str.StructureId);

            
            return nr;
        }
        */

        public NbiRating GetLastNbiRating(string strId, bool getLastInspection = false)
        {
            NbiRating nr = null;
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
                                                                from dot1stro.dt_strc_insp MaxPrev, dot1stro.dt_insp_insp_ty insptype
                                                                where StrcInsp.strc_id = MaxPrev.strc_id
                                                                    and maxprev.stin_id = insptype.stin_id
                                                                    and insptype.stin_tycd in ('R','C'))
                            ";

            if (getLastInspection)
            {
                qry = @"
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
                                                                from dot1stro.dt_strc_insp MaxPrev, dot1stro.dt_insp_insp_ty insptype
                                                                where StrcInsp.strc_id = MaxPrev.strc_id
                                                                    and maxprev.stin_id = insptype.stin_id)
                            ";
            }

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                nr = new NbiRating();
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
                if (Int32.TryParse(nr.DeckRating, out rating))
                {
                    nr.DeckDeteriorationYear = GetNbiDeteriorationYear(Code.NbiDeck, rating, strId);
                    nr.DeckRatingVal = Convert.ToDouble(nr.DeckRating);
                }

                if (Int32.TryParse(nr.SuperstructureRating, out rating))
                {
                    nr.SuperstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSuperstructure, rating, strId);
                    nr.SuperstructureRatingVal = Convert.ToDouble(nr.SuperstructureRating);
                }

                if (Int32.TryParse(nr.SubstructureRating, out rating))
                {
                    nr.SubstructureDeteriorationYear = GetNbiDeteriorationYear(Code.NbiSubstructure, rating, strId);
                    nr.SubstructureRatingVal = Convert.ToDouble(nr.SubstructureRating);
                }

                if (Int32.TryParse(nr.CulvertRating, out rating))
                {
                    nr.CulvertDeteriorationYear = GetNbiDeteriorationYear(Code.NbiCulvert, rating, strId);
                    nr.CulvertRatingVal = Convert.ToDouble(nr.CulvertRating);
                }

                if (Int32.TryParse(nr.WaterwayRating, out rating))
                {
                    nr.WaterwayRatingVal = Convert.ToDouble(nr.WaterwayRating);
                }
            }

            return nr;
        }

        public double GetNbiReductionValue(int nbiRating, int cfId, WisamType.NbiRatingTypes ratingType)
        {
            double reduction = 0;
            string qry = "";

            switch (ratingType)
            {
                case WisamType.NbiRatingTypes.Deck:
                    qry = @"
                                select NbiRating, CaiFormulaId, DeckReductionValue
                                from NbiCaiReduction
                                where CaiFormulaId = @cfId
                                    and NbiRating = @nbiRating
                            ";
                    break;
                case WisamType.NbiRatingTypes.Superstructure:
                    qry = @"
                                select NbiRating, CaiFormulaId, SuperStructureReductionValue
                                from NbiCaiReduction
                                where CaiFormulaId = @cfId
                                    and NbiRating = @nbiRating
                            ";
                    break;
                case WisamType.NbiRatingTypes.Substructure:
                    qry = @"
                                select NbiRating, CaiFormulaId, SubStructureReductionValue
                                from NbiCaiReduction
                                where CaiFormulaId = @cfId
                                    and NbiRating = @nbiRating
                            ";
                    break;
                case WisamType.NbiRatingTypes.Culvert:
                    qry = @"
                                select NbiRating, CaiFormulaId, CulvertReductionValue
                                from NbiCaiReduction
                                where CaiFormulaId = @cfId
                                    and NbiRating = @nbiRating
                            ";
                    break;
            }

            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@cfId", SqlDbType.Int);
            prms[0].Value = cfId;
            prms[1] = new SqlParameter("@nbiRating", SqlDbType.Int);
            prms[1].Value = nbiRating;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                reduction = Convert.ToDouble(dr[2]);
            }

            return reduction;
        }

        public double GetNbiDeteriorationRating(string classification, int year, string strId)
        {
            StructureDeterioration str = GetStructureForDeterioration(strId);
            double deterioratedRating = 0;
            string qry = "";
            SqlParameter[] prms = new SqlParameter[2];

            if ((classification.Equals("NDEC") && !String.IsNullOrEmpty(str.NbiDeckQualifiedDeteriorationCurve))
                    || (classification.Equals("NSUP") && !String.IsNullOrEmpty(str.NbiSuperQualifiedDeteriorationCurve))
                    || (classification.Equals("NSUB") && !String.IsNullOrEmpty(str.NbiSubQualifiedDeteriorationCurve))
                )
            {
                string qualifiedCode = "";
                switch (classification)
                {
                    case "NDEC":
                        qualifiedCode = str.NbiDeckQualifiedDeteriorationCurve;
                        break;
                    case "NSUP":
                        qualifiedCode = str.NbiSuperQualifiedDeteriorationCurve;
                        break;
                    case "NSUB":
                        qualifiedCode = str.NbiSubQualifiedDeteriorationCurve;
                        break;
                }

                qry =
                    @"
                        select rating
                        from nbiqualifieddeterioration
                        where qualifiedcode = @qualifiedCode
                            and year = @year
                    ";
                prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
                prms[0].Value = qualifiedCode;
                prms[1] = new SqlParameter("@year", SqlDbType.Int);
                prms[1].Value = year;
                DataTable dt = ExecuteSelect(qry, prms, samConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    deterioratedRating = Convert.ToDouble(dr["RATING"]);
                }
            }
            else
            {
                qry =
                    @"
                        select Rating
                        from NbiDeterioration
                        where ClassificationCode = @classification
                            and Year = @year
                    ";
                prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@classification", SqlDbType.VarChar);
                prms[0].Value = classification;
                prms[1] = new SqlParameter("@year", SqlDbType.Int);
                prms[1].Value = year;
                DataTable dt = ExecuteSelect(qry, prms, samConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    deterioratedRating = Convert.ToDouble(dr["RATING"]);
                }
            }

            return deterioratedRating;
        }

        public int GetNbiDeteriorationYear(string classification, double rating, string strId)
        {
            StructureDeterioration str = GetStructureForDeterioration(strId);
            int deterioratedYear = -1;
            string qry = "";

            if ((classification.Equals("NDEC") && !String.IsNullOrEmpty(str.NbiDeckQualifiedDeteriorationCurve))
                    || (classification.Equals("NSUP") && !String.IsNullOrEmpty(str.NbiSuperQualifiedDeteriorationCurve))
                    || (classification.Equals("NSUB") && !String.IsNullOrEmpty(str.NbiSubQualifiedDeteriorationCurve))
                )
            {
                string qualifiedCode = "";
                switch (classification)
                {
                    case "NDEC":
                        qualifiedCode = str.NbiDeckQualifiedDeteriorationCurve;
                        break;
                    case "NSUP":
                        qualifiedCode = str.NbiSuperQualifiedDeteriorationCurve;
                        break;
                    case "NSUB":
                        qualifiedCode = str.NbiSubQualifiedDeteriorationCurve;
                        break;
                }

                qry =
                    @"
                        select max(Year) as DetYr
                        from NbiQualifiedDeterioration
                        where Rating >= @rating
                            and QualifiedCode = @qualifiedCode
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@rating", SqlDbType.Float);
                prms[0].Value = rating;
                prms[1] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
                prms[1].Value = qualifiedCode;
                DataTable dt = ExecuteSelect(qry, prms, samConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    deterioratedYear = Convert.ToInt32(dr["DETYR"]);
                }
            }
            else
            {
                qry = @"
                            select max(Year) as DetYr
                            from NbiDeterioration
                            where Rating >= @rating
                                and ClassificationCode = @classification
                        ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@rating", SqlDbType.Float);
                prms[0].Value = rating;
                prms[1] = new SqlParameter("@classification", SqlDbType.VarChar);
                prms[1].Value = classification;
                DataTable dt = ExecuteSelect(qry, prms, samConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    deterioratedYear = Convert.ToInt32(dr["DETYR"]);
                }
            }

            return deterioratedYear;
        }

        public int GetNbiDeteriorationYear(string classification, int rating, string strId)
        {
            StructureDeterioration str = GetStructureForDeterioration(strId);
            int deterioratedYear = -1;
            string qry = "";

            if ((classification.Equals("NDEC") && !String.IsNullOrEmpty(str.NbiDeckQualifiedDeteriorationCurve))
                    || (classification.Equals("NSUP") && !String.IsNullOrEmpty(str.NbiSuperQualifiedDeteriorationCurve))
                    || (classification.Equals("NSUB") && !String.IsNullOrEmpty(str.NbiSubQualifiedDeteriorationCurve))
                )
            {
                string qualifiedCode = "";
                switch (classification)
                {
                    case "NDEC":
                        qualifiedCode = str.NbiDeckQualifiedDeteriorationCurve;
                        break;
                    case "NSUP":
                        qualifiedCode = str.NbiSuperQualifiedDeteriorationCurve;
                        break;
                    case "NSUB":
                        qualifiedCode = str.NbiSubQualifiedDeteriorationCurve;
                        break;
                }

                qry =
                    @"
                        select max(Year) as DetYr
                        from NbiQualifiedDeterioration
                        where floor(Rating) = @rating
                            and QualifiedCode = @qualifiedCode
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@rating", SqlDbType.Int);
                prms[0].Value = rating;
                prms[1] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
                prms[1].Value = qualifiedCode;
                DataTable dt = ExecuteSelect(qry, prms, samConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    deterioratedYear = Convert.ToInt32(dr["DETYR"]);
                }
            }
            else
            {
                qry =
                    @"
                        select max(Year) as DetYr
                        from NbiDeterioration
                        where floor(Rating) = @rating
                            and ClassificationCode = @classification
                    ";
                SqlParameter[] prms = new SqlParameter[2];
                prms[0] = new SqlParameter("@rating", SqlDbType.Int);
                prms[0].Value = rating;
                prms[1] = new SqlParameter("@classification", SqlDbType.VarChar);
                prms[1].Value = classification;
                DataTable dt = ExecuteSelect(qry, prms, samConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    deterioratedYear = Convert.ToInt32(dr["DETYR"]);
                }
            }

            return deterioratedYear;
        }



        public void CalculateNbiDeteriorationRates()
        {
            DataTable nbiDeteriorationCurves = GetNbiDeteriorationCurves();

            if (nbiDeteriorationCurves != null && nbiDeteriorationCurves.Rows.Count > 0)
            {
                foreach (DataRow row in nbiDeteriorationCurves.Rows)
                {
                    string classificationCode = row["CLASSIFICATIONCODE"].ToString();
                    string formula = row["DETERIORATIONFORMULA"].ToString();

                    for (int i = 0; i <= 100; i++)
                    {
                        string newExpression = formula.Replace("x", i.ToString());
                        double rating = Convert.ToDouble(new DataTable().Compute(newExpression, null));

                        if (rating < 0)
                        {
                            rating = 0;
                        }

                        string qry = @"
                                insert into NbiDeterioration(ClassificationCode, Year, Rating)
                                values (@classificationCode, @year, @rating)
                            ";

                        SqlParameter[] prms = new SqlParameter[3];
                        prms[0] = new SqlParameter("@classificationCode", SqlDbType.VarChar);
                        prms[0].Value = classificationCode;
                        prms[1] = new SqlParameter("@year", SqlDbType.Int);
                        prms[1].Value = i;
                        prms[2] = new SqlParameter("@rating", SqlDbType.Float);
                        prms[2].Value = rating;
                        ExecuteInsertUpdateDelete(qry, prms, samConn);
                    }
                }
            }
        }

        public void DeleteNbiDeteriorations()
        {
            string qry = @"
                                delete
                                from NbiDeterioration
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);
        }

        public void UpdateNbiQualifiedDeterioration(string qualifiedCode, string deteriorationFormula, string qualificationExpression)
        {
            string qry =
                @"
                    update nbiqualifieddeteriorationcurve
                    set deteriorationformula = @deteriorationFormula,
                        qualificationexpression = @qualificationExpression
                    where qualifiedCode = @qualifiedCode
                ";
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@deteriorationFormula", SqlDbType.VarChar);
            prms[0].Value = deteriorationFormula;
            prms[1] = new SqlParameter("@qualificationExpression", SqlDbType.VarChar);
            prms[1].Value = qualificationExpression;
            prms[2] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
            prms[2].Value = qualifiedCode;
            ExecuteInsertUpdateDelete(qry, prms, samConn);

            // Delete existing rating values
            qry = @"
                        delete
                        from NbiQualifiedDeterioration
                        where qualifiedcode = @qualifiedCode
                    ";
            prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
            prms[0].Value = qualifiedCode;
            ExecuteInsertUpdateDelete(qry, prms, samConn);

            // Insert new rating values
            for (int i = 0; i <= 100; i++)
            {
                string newExpression = deteriorationFormula.Replace("x", i.ToString());
                float rating = Convert.ToSingle(new DataTable().Compute(newExpression, null));

                if (rating < 0)
                {
                    rating = 0;
                }

                qry = @"
                            insert into NbiQualifiedDeterioration(QualifiedCode, Year, Rating)
                            values (@qualifiedCode, @year, @rating)
                        ";

                SqlParameter[] prms2 = new SqlParameter[3];
                prms2[0] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
                prms2[0].Value = qualifiedCode;
                prms2[1] = new SqlParameter("@year", SqlDbType.Int);
                prms2[1].Value = i;
                prms2[2] = new SqlParameter("@rating", SqlDbType.Float);
                prms2[2].Value = rating;

                ExecuteInsertUpdateDelete(qry, prms2, samConn);
            }
        }

        public void UpdateNbiDeterioration(WisamType.NbiRatingTypes nbiRatingType, string deteriorationFormula)
        {
            string nbiClassificationCode = "NDEC";

            if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Superstructure))
            {
                nbiClassificationCode = "NSUP";
            }
            else if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Substructure))
            {
                nbiClassificationCode = "NSUB";
            }
            else if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Culvert))
            {
                nbiClassificationCode = "NCUL";
            }

            // Update the formula
            string qry = @"
                                update NbiDeteriorationCurve
                                set DeteriorationFormula = @deteriorationFormula
                                where ClassificationCode = @nbiClassificationCode
                            ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@deteriorationFormula", SqlDbType.VarChar);
            prms[0].Value = deteriorationFormula;
            prms[1] = new SqlParameter("@nbiClassificationCode", SqlDbType.VarChar);
            prms[1].Value = nbiClassificationCode;
            ExecuteInsertUpdateDelete(qry, prms, samConn);

            // Delete existing rating values
            qry = @"
                        delete
                        from NbiDeterioration
                        where ClassificationCode = @nbiClassificationCode
                    ";
            prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@nbiClassificationCode", SqlDbType.VarChar);
            prms[0].Value = nbiClassificationCode;
            ExecuteInsertUpdateDelete(qry, prms, samConn);

            // Insert new rating values
            for (int i = 0; i <= 100; i++)
            {
                string newExpression = deteriorationFormula.Replace("x", i.ToString());
                float rating = Convert.ToSingle(new DataTable().Compute(newExpression, null));

                if (rating < 0)
                {
                    rating = 0;
                }

                qry = @"
                            insert into NbiDeterioration(ClassificationCode, Year, Rating)
                            values (@nbiClassificationCode, @year, @rating)
                        ";

                SqlParameter[] prms2 = new SqlParameter[3];
                prms2[0] = new SqlParameter("@nbiClassificationCode", SqlDbType.VarChar);
                prms2[0].Value = nbiClassificationCode;
                prms2[1] = new SqlParameter("@year", SqlDbType.Int);
                prms2[1].Value = i;
                prms2[2] = new SqlParameter("@rating", SqlDbType.Float);
                prms2[2].Value = rating;

                ExecuteInsertUpdateDelete(qry, prms2, samConn);
            }
        }

        public string GetQualificationExpression(string qualifiedCode)
        {
            string qualificationExpression = "";
            string qry =
                @"
                    select *
                    from nbiqualifieddeteriorationcurve
                    where qualifiedcode = @qualifiedCode
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
            prms[0].Value = qualifiedCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                qualificationExpression = dr["qualificationexpression"].ToString();
            }

            return qualificationExpression;
        }

        public string GetQualifiedDeteriorationFormula(string qualifiedCode)
        {
            string deteriorationFormula = "";
            string qry =
                @"
                    select *
                    from nbiqualifieddeteriorationcurve
                    where qualifiedcode = @qualifiedCode
                ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
            prms[0].Value = qualifiedCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                deteriorationFormula = dr["deteriorationformula"].ToString();
            }

            return deteriorationFormula;
        }

        public string GetNbiDeteriorationFormula(WisamType.NbiRatingTypes nbiRatingType)
        {
            string deteriorationFormula = "";
            string nbiClassificationCode = "NDEC";

            if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Superstructure))
            {
                nbiClassificationCode = "NSUP";
            }
            else if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Substructure))
            {
                nbiClassificationCode = "NSUB";
            }
            else if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Culvert))
            {
                nbiClassificationCode = "NCUL";
            }

            string qry = @"
                                select DeteriorationFormula
                                from NbiDeteriorationCurve
                                where ClassificationCode = @nbiClassificationCode
                            ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@nbiClassificationCode", SqlDbType.VarChar);
            prms[0].Value = nbiClassificationCode;

            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    deteriorationFormula = row["DETERIORATIONFORMULA"].ToString();
                }
            }

            return deteriorationFormula;
        }

        public List<NbiDeterioratedRating> GetNbiQualifiedDeterioratedRatings(string qualifiedCode)
        {
            List<NbiDeterioratedRating> nbiDeterioratedRatings = new List<NbiDeterioratedRating>();
            string qry =
                @"
                    select det.*, curve.classificationcode
                    from nbiqualifieddeterioration det, nbiqualifieddeteriorationcurve curve
                    where det.qualifiedcode = @qualifiedCode
                        and det.qualifiedcode = curve.qualifiedcode
                ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@qualifiedCode", SqlDbType.VarChar);
            prms[0].Value = qualifiedCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    NbiDeterioratedRating nbiDeterioratedRating = new NbiDeterioratedRating();
                    nbiDeterioratedRating.DeteriorationId = Convert.ToInt32(row["DETERIORATIONID"]);
                    nbiDeterioratedRating.NbiClassificationCode = row["CLASSIFICATIONCODE"].ToString();
                    nbiDeterioratedRating.Year = Convert.ToInt32(row["YEAR"]);
                    nbiDeterioratedRating.RatingValue = Convert.ToSingle(row["RATING"]);
                    nbiDeterioratedRatings.Add(nbiDeterioratedRating);
                }
            }
            return nbiDeterioratedRatings;
        }

        public List<NbiDeterioratedRating> GetNbiDeterioratedRatings(WisamType.NbiRatingTypes nbiRatingType)
        {
            List<NbiDeterioratedRating> nbiDeterioratedRatings = new List<NbiDeterioratedRating>();
            string nbiClassificationCode = "NDEC";

            if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Superstructure))
            {
                nbiClassificationCode = "NSUP";
            }
            else if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Substructure))
            {
                nbiClassificationCode = "NSUB";
            }
            else if (nbiRatingType.Equals(WisamType.NbiRatingTypes.Culvert))
            {
                nbiClassificationCode = "NCUL";
            }

            string qry = @"
                                select DeteriorationId, ClassificationCode, Year, Rating
                                from NbiDeterioration
                                where ClassificationCode = @nbiClassificationCode
                                order by Year
                            ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@nbiClassificationCode", SqlDbType.VarChar);
            prms[0].Value = nbiClassificationCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    NbiDeterioratedRating nbiDeterioratedRating = new NbiDeterioratedRating();
                    nbiDeterioratedRating.DeteriorationId = Convert.ToInt32(row["DETERIORATIONID"]);
                    nbiDeterioratedRating.NbiClassificationCode = row["CLASSIFICATIONCODE"].ToString();
                    nbiDeterioratedRating.Year = Convert.ToInt32(row["YEAR"]);
                    nbiDeterioratedRating.RatingValue = Convert.ToSingle(row["RATING"]);
                    nbiDeterioratedRatings.Add(nbiDeterioratedRating);
                }
            }

            return nbiDeterioratedRatings;
        }

        private DataTable GetNbiDeteriorationCurves()
        {
            string qry = @"
                                select ClassificationCode, DeteriorationFormula
                                from NbiDeteriorationCurve
                            ";

            return ExecuteSelect(qry, samConn);
        }

        public List<NbiClassificationCaiReduction> GetNbiClassCaiReductions(int cfId)
        {
            List<NbiClassificationCaiReduction> nbiClassReductions = null;
            string fvList = ConvertToQuotedList(",", GetCaiFormulaVariables(GetCaiFormulaExpression(cfId)));
            string qry = @"
                                select NbiClassificationCode
                                from NbiClassification
                            ";
            qry += " where NbiClassificationCode in (" + fvList + ")";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                nbiClassReductions = new List<NbiClassificationCaiReduction>();

                foreach (DataRow row in dt.Rows)
                {
                    NbiClassificationCaiReduction nccr = new NbiClassificationCaiReduction();
                    nccr.CaiFormulaId = cfId;
                    nccr.NbiClassificationCode = row["NBICLASSIFICATIONCODE"].ToString();
                    nbiClassReductions.Add(nccr);
                }
            }

            return nbiClassReductions;
        }
        #endregion NBI Methods

        #region Work Action Methods
        public void DeleteWorkActionCriteria(int workActionRuleId, int ruleSequence)
        {
            string qry = "";

            qry = @"
                        update RuleWorkAction
                        set RuleSequence = RuleSequence - 1
                        where RuleSequence > @ruleSequence
                            and Active = 1
                    ";

            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@ruleSequence", SqlDbType.Int);
            prms[0].Value = ruleSequence;

            ExecuteInsertUpdateDelete(qry, prms, samConn);

            qry = @"
                        delete from RuleWorkAction
                        where RuleId = @workActionRuleId
                    ";

            prms[0] = new SqlParameter("@workActionRuleId", SqlDbType.Int);
            prms[0].Value = workActionRuleId;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public void UpdateWorkActionCriteria(WorkActionRule workActionRule, int oldRuleSequence)
        {
            // Resequence
            if (workActionRule.RuleSequence != oldRuleSequence && workActionRule.Active)
            {
                string qry = "";

                /*
                if (workActionRule.RuleSequence == 1)
                {
                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = RuleSequence + 1
                                where Active = 1
                           ";

                    ExecuteInsertUpdateDelete(qry, samConn);

                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = 1
                                where RuleId = @ruleId
                            ";
                    SqlParameter[] prms = new SqlParameter[1];
                    prms[0] = new SqlParameter("@ruleId", SqlDbType.Int);
                    prms[0].Value = workActionRule.RuleId;

                    ExecuteInsertUpdateDelete(qry, prms, samConn);
                }
                */
                if (workActionRule.RuleSequence == oldRuleSequence - 1 || workActionRule.RuleSequence == oldRuleSequence + 1)
                {
                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = @oldRuleSequence 
                                where RuleSequence = @ruleSequence
                            ";

                    SqlParameter[] prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@oldRuleSequence", SqlDbType.Int);
                    prms[0].Value = oldRuleSequence;
                    prms[1] = new SqlParameter("@ruleSequence", SqlDbType.Int);
                    prms[1].Value = workActionRule.RuleSequence;

                    ExecuteInsertUpdateDelete(qry, prms, samConn);

                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = @ruleSequence
                                where RuleId = @ruleId
                            ";

                    SqlParameter[] prms1 = new SqlParameter[2];
                    prms1[0] = new SqlParameter("@ruleSequence", SqlDbType.Int);
                    prms1[0].Value = workActionRule.RuleSequence;
                    prms1[1] = new SqlParameter("@ruleId", SqlDbType.Int);
                    prms1[1].Value = workActionRule.RuleId;

                    ExecuteInsertUpdateDelete(qry, prms1, samConn);

                }
                /*
                else if (workActionRule.RuleSequence == oldRuleSequence + 1)
                {
                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = @oldRuleSequence 
                                where RuleSequence = @ruleSequence
                            ";

                    SqlParameter[] prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@oldRuleSequence", SqlDbType.Int);
                    prms[0].Value = oldRuleSequence;
                    prms[1] = new SqlParameter("@ruleSequence", SqlDbType.Int);
                    prms[1].Value = workActionRule.RuleSequence;
                }
                */
                else
                {
                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = RuleSequence + 1
                                where Active = 1
                                    and RuleSequence >= @ruleSequence
                            ";
                    SqlParameter[] prms = new SqlParameter[1];
                    prms[0] = new SqlParameter("@ruleSequence", SqlDbType.Int);
                    prms[0].Value = workActionRule.RuleSequence;

                    ExecuteInsertUpdateDelete(qry, prms, samConn);

                    qry = @"
                                update RuleWorkAction
                                set RuleSequence = @ruleSequence
                                where RuleId = @ruleId
                            ";
                    SqlParameter[] prms1 = new SqlParameter[2];
                    prms1[0] = new SqlParameter("@ruleSequence", SqlDbType.Int);
                    prms1[0].Value = workActionRule.RuleSequence;
                    prms1[1] = new SqlParameter("@ruleId", SqlDbType.Int);
                    prms1[1].Value = workActionRule.RuleId;

                    ExecuteInsertUpdateDelete(qry, prms1, samConn);
                }

                // Resequence so that there are no gaps if not a new rule
                if (workActionRule.RuleId != -1)
                {
                    List<int> workActionRuleIds = GetWorkActionRuleIds();

                    int sequenceCounter = 1;
                    foreach (var workActionRuleId in workActionRuleIds)
                    {
                        qry = @"
                                update RuleWorkAction
                                set RuleSequence = @sequenceCounter
                                where RuleId = @workActionRuleId
                            ";

                        SqlParameter[] prms = new SqlParameter[2];
                        prms[0] = new SqlParameter("@sequenceCounter", SqlDbType.Int);
                        prms[0].Value = sequenceCounter;
                        prms[1] = new SqlParameter("@workActionRuleId", SqlDbType.Int);
                        prms[1].Value = workActionRuleId;

                        ExecuteInsertUpdateDelete(qry, prms, samConn);

                        sequenceCounter++;
                    }
                }
            }

            // New rule
            if (workActionRule.RuleId == -1)
            {
                int newRuleId = GetMaxRuleId() + 1;

                string qry = @"
                                insert into RuleWorkAction(RuleId, RuleFormula, RuleCategory, WorkActionCode, RuleSequence, Active, Notes, RuleWorkActionNotes)
                                values (@ruleId, @ruleFormula, @ruleCategory, @workActionCode, @ruleSequence, @active, @notes, @ruleWorkActionNotes)
                            ";
                SqlParameter[] prms = new SqlParameter[8];
                prms[0] = new SqlParameter("@ruleId", SqlDbType.Int);
                prms[0].Value = newRuleId;
                prms[1] = new SqlParameter("@ruleFormula", SqlDbType.VarChar);
                prms[1].Value = workActionRule.RuleFormula;
                prms[2] = new SqlParameter("@ruleCategory", SqlDbType.VarChar);
                prms[2].Value = workActionRule.RuleCategory;
                prms[3] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                prms[3].Value = workActionRule.ResultingWorkActionCode;
                prms[4] = new SqlParameter("@ruleSequence", SqlDbType.Int);
                prms[4].Value = workActionRule.RuleSequence;
                prms[5] = new SqlParameter("@active", SqlDbType.Bit);
                prms[5].Value = workActionRule.Active ? 1 : 0;
                prms[6] = new SqlParameter("@notes", SqlDbType.VarChar);
                prms[6].Value = workActionRule.RuleNotes;
                prms[7] = new SqlParameter("@ruleWorkActionNotes", SqlDbType.VarChar);
                prms[7].Value = workActionRule.RuleWorkActionNotes;

                ExecuteInsertUpdateDelete(qry, prms, samConn);
            } // Existing rule
            else
            {
                string qry = @"
                                update RuleWorkAction
                                set RuleFormula = @ruleFormula,
                                    RuleCategory = @ruleCategory,
                                    WorkActionCode = @workActionCode,
                                    Notes = @notes,
                                    Active = @active,
                                    RuleWorkActionNotes = @ruleWorkActionNotes
                                where RuleId = @ruleId
                            ";

                SqlParameter[] prms = new SqlParameter[7];
                prms[0] = new SqlParameter("@ruleFormula", SqlDbType.VarChar);
                prms[0].Value = workActionRule.RuleFormula;
                prms[1] = new SqlParameter("@notes", SqlDbType.VarChar);
                prms[1].Value = workActionRule.RuleNotes;
                prms[2] = new SqlParameter("@ruleId", SqlDbType.Int);
                prms[2].Value = workActionRule.RuleId;
                prms[3] = new SqlParameter("@active", SqlDbType.Bit);
                prms[3].Value = workActionRule.Active ? 1 : 0;
                prms[4] = new SqlParameter("@ruleCategory", SqlDbType.VarChar);
                prms[4].Value = workActionRule.RuleCategory;
                prms[5] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                prms[5].Value = workActionRule.ResultingWorkActionCode;
                prms[6] = new SqlParameter("@ruleWorkActionNotes", SqlDbType.VarChar);
                prms[6].Value = workActionRule.RuleWorkActionNotes;

                ExecuteInsertUpdateDelete(qry, prms, samConn);
            }
        }

        public void UpdateWorkActionCriteria(int ruleId)
        {
            string qry = "";

            switch (ruleId)
            {
                case 35:
                    qry = @"
                                select *
                                from RuleWorkAction
                                where WorkActionCode in ('03','49')
                            ";

                    break;
            }

            DataTable dt = ExecuteSelect(qry, samConn);
            string newValue = "";

            if (dt != null && dt.Rows.Count > 0)
            {
                switch (ruleId)
                {
                    case 35:
                        string crit1 = "";
                        int crit1Counter = 0;
                        string crit2 = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["WORKACTIONCODE"].ToString().Equals("03"))
                            {
                                if (crit1Counter > 0)
                                {
                                    crit1 += " OR ";
                                }

                                crit1 += String.Format(" ({0}) ", dr["RULEFORMULA"].ToString());
                                crit1Counter++;
                            }
                            else if (dr["WORKACTIONCODE"].ToString().Equals("49"))
                            {
                                crit2 = String.Format("({0})", dr["RULEFORMULA"].ToString());
                            }
                        }
                        //newValue = String.Format("({0})", crit1);
                        newValue = String.Format("({0}) AND ({1})", crit1, crit2);
                        break;
                }
            }
        }

        public List<CombinedWorkAction> GetSecondaryWorkActions(string primaryWorkActionCode)
        {
            List<CombinedWorkAction> combinedWorkActions = new List<CombinedWorkAction>();
            string qry = @"
                                select cwa.MainWorkActionCode, cwa.SecondaryWorkActionCode, cwa.CombinedWorkActionCode, cwa.BypassRule, cwa.Active
                                from CombinedWorkAction cwa
                                    left join RuleWorkAction rwa
                                        on cwa.SecondaryWorkActionCode = rwa.WorkActionCode
                                    join WorkAction wa
                                        on wa.WorkActionCode = rwa.WorkActionCode
                                where MainWorkActionCode = @primaryWorkActionCode
                                    and (SecondaryWorkActionCode is null or SecondaryWorkActionCode != @primaryWorkActionCode)  
                                    and cwa.Active = 1                        
                                    and rwa.Active = 1
                                    and wa.Active = 1
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@primaryWorkActionCode", SqlDbType.VarChar);
            prms[0].Value = primaryWorkActionCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    CombinedWorkAction cwa = new CombinedWorkAction();
                    cwa.MainWorkActionCode = primaryWorkActionCode.ToString();
                    cwa.SecondaryWorkActionCode = dr["SECONDARYWORKACTIONCODE"].ToString();
                    cwa.BypassRule = Convert.ToBoolean(dr["BYPASSRULE"]);
                    cwa.Active = Convert.ToBoolean(dr["ACTIVE"]);

                    if (!String.IsNullOrEmpty(dr["COMBINEDWORKACTIONCODE"].ToString()))
                    {
                        cwa.CombinedWorkActionCode = dr["COMBINEDWORKACTIONCODE"].ToString();
                    }

                    if (combinedWorkActions.Where(e => e.MainWorkActionCode.Equals(cwa.MainWorkActionCode) && e.SecondaryWorkActionCode.Equals(cwa.SecondaryWorkActionCode)).Count() == 0)
                    {
                        combinedWorkActions.Add(cwa);
                    }
                }
            }

            return combinedWorkActions;
        }

        public double GetStructureWorkActionCost(string workActionCode, Structure str)
        {
            double workCost = 0;
            StructureWorkAction swa = GetStructureWorkAction(workActionCode);
            string costExpression = swa.CostFormula;

            if (swa.CombinedWorkAction)
            {
                foreach (var code in swa.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    workCost += GetStructureWorkActionCost(code, str);
                }
            }
            else
            {
                if (swa.UseUnitCostFormula)
                {
                    var val = Math.Log(str.SuperstructurePaintArea, Math.E);
                    string unitCostExpression = swa.UnitCostFormula.ToUpper();

                    if (swa.UnitCostFormula.ToUpper().Contains("LN("))
                    {
                        var natLogVal = Math.Log(str.SuperstructurePaintArea, Math.E);
                        unitCostExpression = unitCostExpression.Replace("LN(SUPPAINTAREA)", natLogVal.ToString());
                    }

                    try
                    {
                        double unitCost = Convert.ToDouble(new DataTable().Compute(unitCostExpression, null));

                        if (unitCost > swa.MaxUnitCost)
                        {
                            unitCost = swa.MaxUnitCost;
                        }
                        else if (unitCost < swa.MinUnitCost)
                        {
                            unitCost = swa.MinUnitCost;
                        }

                        swa.UnitCost = unitCost;
                    }
                    catch { }
                }

                costExpression = costExpression.Replace("DECKAREA", str.DeckArea.ToString())
                                .Replace("UNITCOST", swa.UnitCost.ToString())
                                .Replace("SUPPAINTAREASPOT", str.SuperstructureSpotPaintArea.ToString())
                                .Replace("SUPPAINTAREA", str.SuperstructurePaintArea.ToString())
                                .Replace("JTLENGTH", str.JointLength.ToString());

                try
                {
                    var cost = new DataTable().Compute(costExpression, null);
                    workCost = Convert.ToDouble(cost);
                }
                catch { }
            }

            return workCost;
        }

        public StructureWorkAction GetStructureWorkAction(string workActionCode, Structure str)
        {
            StructureWorkAction swa = GetStructureWorkAction(workActionCode);
            swa.Cost = GetStructureWorkActionCost(workActionCode, str);
            /*
            string costExpression = swa.CostFormula;
            costExpression = costExpression.Replace("DECKAREA", str.DeckArea.ToString())
                            .Replace("UNITCOST", swa.UnitCost.ToString())
                            .Replace("SUPPAINTAREA", str.SuperstructurePaintArea.ToString())
                            .Replace("JTLENGTH", str.JointLength.ToString());

            try
            {
                var cost = new DataTable().Compute(costExpression, null);
                swa.Cost = Convert.ToDouble(cost);
            }
            catch { }
            */

            return swa;
        }


        public List<StructureWorkAction> GetStructureWorkActions(bool improvementProgramWorkActions = false)
        {
            List<StructureWorkAction> swas = new List<StructureWorkAction>();
            string qry = @"
                                select WorkActionCode
                                from WorkAction
                                where Active = 1
                                order by WorkActionDesc
                            ";

            if (improvementProgramWorkActions)
            {
                qry = @"
                                select WorkActionCode
                                from WorkAction
                                where Active = 1
                                    and Improvement = 1
                                order by WorkActionDesc
                            ";
            }

            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string workActionCode = dr["WORKACTIONCODE"].ToString();

                    if (!workActionCode.Equals("00") && !workActionCode.Equals("01"))
                    {
                        swas.Add(GetStructureWorkAction(workActionCode));
                    }
                }
            }

            return swas;
        }

        public StructureWorkAction GetStructureWorkAction(string workActionCode)
        {
            StructureWorkAction swa = null;
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
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                swa = new StructureWorkAction(dr["WORKACTIONCODE"].ToString());
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
                swa.PotentialIncidentals = GetSecondaryWorkActions(swa.WorkActionCode);
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
            DataTable dt2 = ExecuteSelect(qry2, prms2, samConn);

            if (dt2 != null && dt2.Rows.Count > 0)
            {
                foreach (DataRow dr in dt2.Rows)
                {
                    swa.ConditionBenefit += String.Format("{0} to {1}\r\n", dr["CODE"].ToString(), dr["BENEFIT"].ToString());
                }
            }

            return swa;
        }

        /*
        public List<NbiBenefit> GetWorkActionAddedNbiBenefits(string workActionCode)
        {
            List<NbiBenefit> nbiBenefits = new List<NbiBenefit>();
        }*/

        public List<NbiBenefit> GetWorkActionNbiBenefits(string workActionCode)
        {
            List<NbiBenefit> nbiBenefits = new List<NbiBenefit>();
            string qry = @"
                                select ben.WorkActionCode, ben.NbiClassificationCode, ben.Benefit, wa.IncludesOverlay
                                from WorkActionNbiBenefit ben, WorkAction wa
                                where ben.WorkActionCode = @workActionCode
                                    and ben.WorkActionCode = wa.WorkActionCode
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[0].Value = workActionCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    NbiBenefit nb = new NbiBenefit();
                    nb.WorkActionCode = dr["WORKACTIONCODE"].ToString();
                    nb.NbiClassificationCode = dr["NBICLASSIFICATIONCODE"].ToString();
                    nb.Benefit = Convert.ToInt32(dr["BENEFIT"]);
                    nb.AddedBenefit = 0;
                    nb.IncludesOverlay = Convert.ToBoolean(dr["INCLUDESOVERLAY"]);
                    nbiBenefits.Add(nb);
                }
            }
            //else
            {
                qry =
                    @"
                        select ben.WorkActionCode, ben.NbiClassificationCode, ben.AddedBenefit, ben.NbiMaximumValue, wa.IncludesOverlay
                                from WorkActionNbiAddedBenefit ben, WorkAction wa
                                where ben.WorkActionCode = @workActionCode
                                    and ben.WorkActionCode = wa.WorkActionCode
                    ";
                SqlParameter[] prms2 = new SqlParameter[1];
                prms2[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                prms2[0].Value = workActionCode;
                DataTable dt2 = ExecuteSelect(qry, prms2, samConn);

                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt2.Rows)
                    {
                        NbiBenefit nb = new NbiBenefit();
                        nb.WorkActionCode = dr["WORKACTIONCODE"].ToString();
                        nb.NbiClassificationCode = dr["NBICLASSIFICATIONCODE"].ToString();
                        nb.Benefit = 0;
                        nb.AddedBenefit = Convert.ToSingle(dr["ADDEDBENEFIT"]);
                        nb.NbiMaximumValue = Convert.ToInt32(dr["NBIMAXIMUMVALUE"]);
                        nb.IncludesOverlay = Convert.ToBoolean(dr["INCLUDESOVERLAY"]);

                        if (nbiBenefits.Where(b => b.NbiClassificationCode.Equals(nb.NbiClassificationCode)).Count() == 0)
                        {
                            nbiBenefits.Add(nb);
                        }
                    }
                }
            }

            return nbiBenefits;
        }

        public List<ElementBenefit> GetWorkActionElementBenefits(string workActionCode)
        {
            List<ElementBenefit> elementBenefits = new List<ElementBenefit>();
            string qry = @"
                                select WorkActionCode, ElementClassificationCode, Benefit
                                from WorkActionElementBenefit
                                where WorkActionCode = @workActionCode
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[0].Value = workActionCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ElementBenefit eb = new ElementBenefit();
                    eb.WorkActionCode = dr["WORKACTIONCODE"].ToString();
                    eb.ElementClassificationCode = dr["ELEMENTCLASSIFICATIONCODE"].ToString();
                    eb.Benefit = dr["BENEFIT"].ToString();
                    elementBenefits.Add(eb);
                }
            }

            return elementBenefits;
        }

        public List<WorkActionCriteria> GetWorkActionCriteriaForGivenWorkAction(string workActionCode)
        {
            List<WorkActionCriteria> wacs = new List<WorkActionCriteria>();
            string qry = @"
                                select RuleId, RuleFormula, RuleCategory, rw.WorkActionCode, wa.WorkActionDesc, RuleSequence, wa.Active
                                from RuleWorkAction rw, WorkAction wa
                                where rw.Active = 1     
                                    and wa.Active = 1
                                    and rw.WorkActionCode = @workActionCode
                                    and rw.WorkActionCode = wa.WorkActionCode
                                order by RuleSequence
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[0].Value = workActionCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkActionCriteria wac = new WorkActionCriteria();
                    wac.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    wac.RuleId = Convert.ToInt32(dr["RULEID"]);
                    wac.RuleFormula = dr["RULEFORMULA"].ToString();
                    wac.RuleCategory = dr["RULECATEGORY"].ToString();
                    wac.WorkActionCode = dr["WORKACTIONCODE"].ToString();
                    wac.WorkActionDesc = dr["WORKACTIONDESC"].ToString();
                    wac.RuleSequence = Convert.ToInt32(dr["RULESEQUENCE"]);
                    wacs.Add(wac);
                }
            }

            return wacs;
        }

        public List<WorkActionCriteria> GetWorkActionCriteria(bool improvementProgramWorkActions = false, string workActionCodes = "")
        {
            List<WorkActionCriteria> wacs = new List<WorkActionCriteria>();
            string qry = @"
                                select RuleId, RuleFormula, RuleCategory, rw.WorkActionCode, rw.AlternativeWorkActionCode, wa.WorkActionDesc, RuleSequence, wa.Active, wa.CombinedWorkActionCodes, rw.RuleWorkActionNotes
                                from RuleWorkAction rw, WorkAction wa
                                where rw.Active = 1     
                                    and wa.Active = 1
                                    and rw.WorkActionCode = wa.WorkActionCode
                            ";

            if (improvementProgramWorkActions)
            {
                qry = @"
                                select RuleId, RuleFormula, RuleCategory, rw.WorkActionCode, rw.AlternativeWorkActionCode, wa.WorkActionDesc, RuleSequence, wa.Active, wa.CombinedWorkActionCodes, rw.RuleWorkActionNotes
                                from RuleWorkAction rw, WorkAction wa
                                where rw.Active = 1
                                    and wa.Active = 1
                                    and rw.WorkActionCode = wa.WorkActionCode
                                    and wa.Improvement = 1
                            ";
            }

            if (!String.IsNullOrEmpty(workActionCodes))
            {
                string workActionCodeList = ConvertToQuotedList(",", workActionCodes.Split(','));
                qry += @" and wa.WorkActionCode in (" + workActionCodeList + ")";
            }

            qry += @" order by RuleSequence";

            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkActionCriteria wac = new WorkActionCriteria();
                    wac.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    wac.RuleId = Convert.ToInt32(dr["RULEID"]);

                    var ruleFormula = dr["RULEFORMULA"].ToString().ToUpper();
                    wac.RuleFormula = ruleFormula;

                    if (wac.RuleId == 62 || wac.RuleId == 63)
                    {
                        var stop = "here";
                    }

                    if (ruleFormula.Equals("CONCATENATE"))
                    {
                        var concatenatedFormula = "";
                        var combinedWorkActionCodes = dr["COMBINEDWORKACTIONCODES"].ToString();
                        int counter1 = 0;

                        foreach (var workActionCode in combinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            qry = @"
                                        select RuleFormula
                                        from RuleWorkAction
                                        where WorkActionCode = @workActionCode
                                            and Active = 1
                                    ";
                            SqlParameter[] prms2 = new SqlParameter[1];
                            prms2[0] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                            prms2[0].Value = workActionCode;
                            DataTable dt2 = ExecuteSelect(qry, prms2, samConn);

                            if (dt2 != null && dt2.Rows.Count > 0)
                            {
                                string formula = "";
                                int counter2 = 0;

                                foreach (DataRow dr2 in dt2.Rows)
                                {
                                    if (counter2 > 0)
                                    {
                                        formula += " OR ";
                                    }

                                    formula += String.Format("(  {0}   )", dr2["RULEFORMULA"].ToString());
                                    counter2++;
                                }

                                if (counter1 > 0)
                                {
                                    concatenatedFormula += " AND ";
                                }

                                concatenatedFormula += String.Format("(  {0}   )", formula);
                                counter1++;
                            }
                        }

                        wac.RuleFormula = concatenatedFormula;
                    }

                    var altWorkActionCode = dr["ALTERNATIVEWORKACTIONCODE"].ToString().Trim();
                    if (!String.IsNullOrEmpty(altWorkActionCode))
                    {
                        qry = @"
                                    select WorkActionCode, WorkActionDesc
                                    from WorkAction
                                    where WorkActionCode = @altWorkActionCode
                                ";
                        SqlParameter[] prms3 = new SqlParameter[1];
                        prms3[0] = new SqlParameter("@altWorkActionCode", SqlDbType.VarChar);
                        prms3[0].Value = altWorkActionCode;
                        DataTable dt3 = ExecuteSelect(qry, prms3, samConn);

                        if (dt3 != null && dt.Rows.Count > 0)
                        {
                            wac.AlternativeWorkActionCode = altWorkActionCode;
                            wac.AlternativeWorkActionDesc = dt3.Rows[0]["WORKACTIONDESC"].ToString();
                        }
                    }

                    wac.RuleCategory = dr["RULECATEGORY"].ToString();
                    wac.WorkActionCode = dr["WORKACTIONCODE"].ToString();
                    wac.WorkActionDesc = dr["WORKACTIONDESC"].ToString();

                    if (!String.IsNullOrEmpty(dr["RULEWORKACTIONNOTES"].ToString()))
                    {
                        wac.WorkActionDesc += String.Format(" ({0})", dr["RULEWORKACTIONNOTES"].ToString());
                    }

                    wac.RuleSequence = Convert.ToInt32(dr["RULESEQUENCE"]);
                    wacs.Add(wac);
                }
            }

            return wacs;
        }

        #endregion Work Action Methods

        #region FIIPS Methods
        public List<string> GetFiipsStructureIds()
        {
            List<string> structureIds = new List<string>();
            string qry = @"
                                select distinct Extg_Strc_Id
                                from Pmic
                                order by Extg_Strc_Id
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    structureIds.Add(dr["EXTG_STRC_ID"].ToString());
                }
            }

            return structureIds;
        }

        public List<string> GetFederalImprovementTypes()
        {
            List<string> federalImprovementTypes = new List<string>();

            string qry = @"
                                
                            ";

            return federalImprovementTypes;
        }

        public List<string> GetPlanningProjectConcepts()
        {
            List<string> planningProjectConcepts = new List<string>();

            string qry = @"
                                
                            ";

            return planningProjectConcepts;
        }

        /*
        select WorkActionCode, WorkActionDesc, WorkActionNotes, Unit, UnitCost,
                                       MinUnitCost, MaxUnitCost, UnitCostFormula, UseUnitCostFormula,
                                       CostFormula, CostNotes, Improvement, CombinedWorkAction,
                                       Repeatable, RepeatFrequency, Preservation, CombinedWorkActionCodes, IncludesOverlay,
                                       LifeExtension
        */
        public List<StructureWorkAction> GetProgrammedWorkActions(DateTime startDate, DateTime endDate, Structure str, WisamType.CalendarTypes calendarType = WisamType.CalendarTypes.CalendarYear)
        {
            List<StructureWorkAction> swas = GetProgrammedWorkActions(startDate, endDate, str.StructureId, calendarType);

            foreach (var swa in swas)
            {
                try
                {
                    swa.Cost = GetStructureWorkActionCost(swa.WorkActionCode, str);
                }
                catch (Exception ex)
                {
                    swa.Cost = 0;
                }
            }

            return swas;
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

        public void CompareToFiips(StructureProgramReview spr)
        {
            List<StructureWorkAction> letProgrammedWorkForGivenStructure = GetProgrammedWorkActions(spr.StructureId, WisamType.CalendarTypes.StateFiscalYear);
            spr.InFiips = false;
            spr.IsScopeAMatch = false;
            spr.IsYearAMatch = false;

            if (letProgrammedWorkForGivenStructure.Count > 0)
            {
                int currentFiscalYear = GetFiscalYear();
                /*
                List<StructureWorkAction> wacs = swas.Where(swa => swa.WorkActionStateFiscalYear >= currentFiscalYear && (swa.WorkActionCode == spr.WorkActionCode
                                                || (swa.CombinedWorkAction && swa.CombinedWorkActionCodes.Contains(spr.WorkActionCode))))
                                                .OrderBy(swa => swa.WorkActionStateFiscalYear)
                                                .ToList();*/
                List<StructureWorkAction> wacs =
                    letProgrammedWorkForGivenStructure.Where(swa => swa.WorkActionStateFiscalYear >= currentFiscalYear)
                    .OrderBy(swa => swa.WorkActionStateFiscalYear)
                    .ToList();

                if (wacs.Count > 0)
                {
                    spr.InFiips = true;

                    if (spr.FosProjectId.Length != 8)
                    {
                        spr.FosProjectId = wacs.First().FosProjId;
                    }

                    // Scope match?
                    var matches = wacs.Where(wac => wac.WorkActionCode == spr.WorkActionCode
                                                || (wac.CombinedWorkAction && wac.CombinedWorkActionCodes.Contains(spr.WorkActionCode))
                                            )
                                      .OrderBy(wac => wac.WorkActionStateFiscalYear)
                                      .ToList();

                    if (matches.Count > 0)
                    {
                        spr.IsScopeAMatch = true;
                        spr.FosProjectId = matches.First().FosProjId;
                        spr.IsYearAMatch = matches.Any(wac => wac.WorkActionStateFiscalYear == spr.WorkActionYear);
                    }
                }

                foreach (var swa in letProgrammedWorkForGivenStructure)
                {
                    spr.FiipsNotes += swa.WorkActionYear + ":" + "(" + swa.WorkActionCode + ")" + swa.WorkActionDesc + ";";
                }
            }
            else
            {
                spr.FiipsNotes = "No programmed work in FIIPS";
            }
        }

        public List<StructureWorkAction> GetProgrammedWorkActions(string strId, WisamType.CalendarTypes calendarType = WisamType.CalendarTypes.StateFiscalYear)
        {
            List<StructureWorkAction> swas = new List<StructureWorkAction>();
            string qry = @"
                            select Pmic.*, 
                                        wa.WorkActionDesc as wad, 
                                        wa.WorkActionNotes, wa.Unit, wa.UnitCost,
                                        wa.MinUnitCost, wa.MaxUnitCost, wa.UnitCostFormula, wa.UseUnitCostFormula,
                                        wa.CostFormula, wa.CostNotes, wa.Improvement, wa.CombinedWorkAction,
                                        wa.Repeatable, wa.RepeatFrequency, wa.Preservation, wa.CombinedWorkActionCodes, 
                                        wa.IncludesOverlay,
                                        wa.LifeExtension
                                from Pmic
                                    left outer join WorkAction wa
                                        on Pmic.WorkActionCode = wa.WorkActionCode
                                where Extg_Strc_Id = @strId
                                    and Estcp_Ty_Cd = 'LET'
                                    and Pproj_Fnty_Cd = '3'
                                    and Pmic.IsDuplicate = 0
                                    and Pmic.WorkActionCode is not NULL
                                    and Pmic.WorkActionCode not in ('NB','BR','OT','EL','OL','RE')
                                order by Estcp_Schd_Dt
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    {
                        StructureWorkAction swa = new StructureWorkAction();
                        swa.StructureId = strId;
                        swa.WorkActionCode = row["WORKACTIONCODE"].ToString();
                        swa.WorkActionDesc = row["wad"].ToString();
                        swa.EstimatedCompletionDate = Convert.ToDateTime(row["ESTCP_SCHD_DT"]);
                        swa.WorkActionYear = swa.EstimatedCompletionDate.Year;

                        if (calendarType == WisamType.CalendarTypes.StateFiscalYear)
                        {
                            if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 7, 1)) >= 0)
                            {
                                swa.WorkActionYear = swa.WorkActionYear + 1;
                            }
                        }
                        else if (calendarType == WisamType.CalendarTypes.FederalFiscalYear)
                        {
                            if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 11, 1)) >= 0)
                            {
                                swa.WorkActionYear = swa.WorkActionYear + 1;
                            }
                        }

                        if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 7, 1)) >= 0)
                        {
                            swa.WorkActionStateFiscalYear = swa.WorkActionYear + 1;
                        }
                        else
                        {
                            swa.WorkActionStateFiscalYear = swa.WorkActionYear;
                        }

                        if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 11, 1)) >= 0)
                        {
                            swa.WorkActionFederalFiscalYear = swa.WorkActionYear + 1;
                        }
                        else
                        {
                            swa.WorkActionFederalFiscalYear = swa.WorkActionYear;
                        }

                        swa.FosProjId = row["FOS_PROJ_ID"].ToString();
                        swa.PProjId = Convert.ToInt32(row["PPROJ_ID"]);
                        swa.PlanningProjectConceptCode = row["pproj_cncp_cd"].ToString();
                        swa.WisDOTProgramCode = row["wdot_pgm_cd"].ToString();
                        swa.WisDOTProgramDesc = row["wdot_pgm_desc"].ToString();

                        try
                        {
                            swa.TotalCostWithDeliveryAmount = Convert.ToDouble(row["tot_with_dlvy_amt"]);
                        }
                        catch { }

                        try
                        {
                            swa.TotalCostWithoutDeliveryAmount = Convert.ToDouble(row["tot_wo_dlvy_amt"]);
                        }
                        catch { }

                        if (!String.IsNullOrEmpty(row["WORKACTIONNOTES"].ToString()))
                        {
                            swa.WorkActionNotes = row["WORKACTIONNOTES"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["UNIT"].ToString()))
                        {
                            swa.Unit = row["UNIT"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["UNITCOST"].ToString()))
                        {
                            swa.UnitCost = Convert.ToDouble(row["UNITCOST"]);
                        }

                        if (!String.IsNullOrEmpty(row["MINUNITCOST"].ToString()))
                        {
                            swa.MinUnitCost = Convert.ToDouble(row["MINUNITCOST"]);
                        }

                        if (!String.IsNullOrEmpty(row["MAXUNITCOST"].ToString()))
                        {
                            swa.MaxUnitCost = Convert.ToDouble(row["MaxUNITCOST"]);
                        }

                        if (!String.IsNullOrEmpty(row["UNITCOSTFORMULA"].ToString()))
                        {
                            swa.UnitCostFormula = row["UNITCOSTFORMULA"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["LIFEEXTENSION"].ToString()))
                        {
                            swa.LifeExtension = Convert.ToDouble(row["LIFEEXTENSION"]);
                        }

                        if (!String.IsNullOrEmpty(row["COSTFORMULA"].ToString()))
                        {
                            swa.CostFormula = row["COSTFORMULA"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["COSTNOTES"].ToString()))
                        {
                            swa.CostNotes = row["COSTNOTES"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["COMBINEDWORKACTIONCODES"].ToString()))
                        {
                            swa.CombinedWorkActionCodes = row["COMBINEDWORKACTIONCODES"].ToString();
                        }

                        swa.UseUnitCostFormula = Convert.ToBoolean(row["USEUNITCOSTFORMULA"]);
                        swa.CombinedWorkAction = Convert.ToBoolean(row["COMBINEDWORKACTION"]);

                        if (swa.CombinedWorkAction)
                        {
                            foreach (var code in swa.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                StructureWorkAction cwa = GetStructureWorkAction(code);
                                swa.CombinedWorkActions.Add(cwa);
                            }
                        }

                        swa.Improvement = Convert.ToBoolean(row["IMPROVEMENT"]);
                        swa.IncludesOverlay = Convert.ToBoolean(row["INCLUDESOVERLAY"]);
                        swa.Preservation = Convert.ToBoolean(row["PRESERVATION"]);
                        swa.Repeatable = Convert.ToBoolean(row["REPEATABLE"]);
                        swa.RepeatFrequency = Convert.ToInt32(row["REPEATFREQUENCY"]);
                        swas.Add(swa);
                    }
                }
            }

            return swas;
        }

        public List<StructureWorkAction> GetProgrammedWorkActions(DateTime startDate, DateTime endDate, string strId, WisamType.CalendarTypes calendarType = WisamType.CalendarTypes.CalendarYear)
        {
            List<StructureWorkAction> swas = new List<StructureWorkAction>();
            string qry = @"
                                select Pmic.*, 
                                        wa.WorkActionDesc as wad, 
                                        wa.WorkActionNotes, wa.Unit, wa.UnitCost,
                                        wa.MinUnitCost, wa.MaxUnitCost, wa.UnitCostFormula, wa.UseUnitCostFormula,
                                        wa.CostFormula, wa.CostNotes, wa.Improvement, wa.CombinedWorkAction,
                                        wa.Repeatable, wa.RepeatFrequency, wa.Preservation, wa.CombinedWorkActionCodes, 
                                        wa.IncludesOverlay,
                                        wa.LifeExtension
                                from Pmic
                                    left outer join WorkAction wa
                                        on Pmic.WorkActionCode = wa.WorkActionCode
                                where Extg_Strc_Id = @strId
                                    and Estcp_Schd_Dt >= @startDate
                                    and Estcp_Schd_Dt <= @endDate
                                    and Estcp_Ty_Cd = 'LET'
                                    and Pproj_Fnty_Cd = '3'
                                    and Pmic.WorkActionCode is not NULL
                                    and Pmic.WorkActionCode not in ('NB','BR','OT','EL','OL','RE')
                                order by Estcp_Schd_Dt desc, wdot_pgm_desc
                            ";
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prms[0].Value = strId;
            prms[1] = new SqlParameter("@startDate", SqlDbType.Date);
            prms[1].Value = startDate;
            prms[2] = new SqlParameter("@endDate", SqlDbType.Date);
            prms[2].Value = endDate;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DateTime previousEstimatedCompletionDate = DateTime.Now;
                DateTime currentEstimatedCompletionDate;
                string previousWorkActionCode = "";
                string currentWorkActionCode = "";

                foreach (DataRow row in dt.Rows)
                {
                    currentEstimatedCompletionDate = Convert.ToDateTime(row["ESTCP_SCHD_DT"]);
                    currentWorkActionCode = row["WORKACTIONCODE"].ToString();

                    if (currentWorkActionCode.Equals(previousWorkActionCode)
                            && DateTime.Compare(currentEstimatedCompletionDate, previousEstimatedCompletionDate) == 0)
                    {
                        swas.Last().WisDOTProgramCode += "; " + row["wdot_pgm_cd"].ToString();
                        swas.Last().WisDOTProgramDesc += "; " + row["wdot_pgm_desc"].ToString();
                    }
                    else
                    {
                        StructureWorkAction swa = new StructureWorkAction();
                        swa.StructureId = strId;
                        swa.WorkActionCode = row["WORKACTIONCODE"].ToString();
                        swa.WorkActionDesc = row["wad"].ToString();
                        swa.EstimatedCompletionDate = Convert.ToDateTime(row["ESTCP_SCHD_DT"]);
                        swa.WorkActionYear = swa.EstimatedCompletionDate.Year;

                        if (calendarType == WisamType.CalendarTypes.StateFiscalYear)
                        {
                            if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 7, 1)) >= 0)
                            {
                                swa.WorkActionYear = swa.WorkActionYear + 1;
                            }
                        }
                        else if (calendarType == WisamType.CalendarTypes.FederalFiscalYear)
                        {
                            if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 11, 1)) >= 0)
                            {
                                swa.WorkActionYear = swa.WorkActionYear + 1;
                            }
                        }

                        if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 7, 1)) >= 0)
                        {
                            swa.WorkActionStateFiscalYear = swa.WorkActionYear + 1;
                        }
                        else
                        {
                            swa.WorkActionStateFiscalYear = swa.WorkActionYear;
                        }

                        if (DateTime.Compare(swa.EstimatedCompletionDate, new DateTime(swa.WorkActionYear, 11, 1)) >= 0)
                        {
                            swa.WorkActionFederalFiscalYear = swa.WorkActionYear + 1;
                        }
                        else
                        {
                            swa.WorkActionFederalFiscalYear = swa.WorkActionYear;
                        }

                        swa.FosProjId = row["FOS_PROJ_ID"].ToString();
                        swa.PProjId = Convert.ToInt32(row["PPROJ_ID"]);
                        swa.PlanningProjectConceptCode = row["pproj_cncp_cd"].ToString();
                        //swa.PlanningProjectConceptDesc = row["pproj_cncp_desc"].ToString();
                        swa.WisDOTProgramCode = row["wdot_pgm_cd"].ToString();
                        swa.WisDOTProgramDesc = row["wdot_pgm_desc"].ToString();

                        try
                        {
                            swa.TotalCostWithDeliveryAmount = Convert.ToDouble(row["tot_with_dlvy_amt"]);
                        }
                        catch { }

                        try
                        {
                            swa.TotalCostWithoutDeliveryAmount = Convert.ToDouble(row["tot_wo_dlvy_amt"]);
                        }
                        catch { }

                        if (!String.IsNullOrEmpty(row["WORKACTIONNOTES"].ToString()))
                        {
                            swa.WorkActionNotes = row["WORKACTIONNOTES"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["UNIT"].ToString()))
                        {
                            swa.Unit = row["UNIT"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["UNITCOST"].ToString()))
                        {
                            swa.UnitCost = Convert.ToDouble(row["UNITCOST"]);
                        }

                        if (!String.IsNullOrEmpty(row["MINUNITCOST"].ToString()))
                        {
                            swa.MinUnitCost = Convert.ToDouble(row["MINUNITCOST"]);
                        }

                        if (!String.IsNullOrEmpty(row["MAXUNITCOST"].ToString()))
                        {
                            swa.MaxUnitCost = Convert.ToDouble(row["MaxUNITCOST"]);
                        }

                        if (!String.IsNullOrEmpty(row["UNITCOSTFORMULA"].ToString()))
                        {
                            swa.UnitCostFormula = row["UNITCOSTFORMULA"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["LIFEEXTENSION"].ToString()))
                        {
                            swa.LifeExtension = Convert.ToDouble(row["LIFEEXTENSION"]);
                        }

                        if (!String.IsNullOrEmpty(row["COSTFORMULA"].ToString()))
                        {
                            swa.CostFormula = row["COSTFORMULA"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["COSTNOTES"].ToString()))
                        {
                            swa.CostNotes = row["COSTNOTES"].ToString();
                        }

                        if (!String.IsNullOrEmpty(row["COMBINEDWORKACTIONCODES"].ToString()))
                        {
                            swa.CombinedWorkActionCodes = row["COMBINEDWORKACTIONCODES"].ToString();
                        }

                        swa.UseUnitCostFormula = Convert.ToBoolean(row["USEUNITCOSTFORMULA"]);
                        swa.CombinedWorkAction = Convert.ToBoolean(row["COMBINEDWORKACTION"]);

                        if (swa.CombinedWorkAction)
                        {
                            foreach (var code in swa.CombinedWorkActionCodes.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                //StructureWorkAction cwa = dbObj.GetStructureWorkAction(code);
                                StructureWorkAction cwa = GetStructureWorkAction(code);
                                //swa.Cost += cwa.Cost;
                                swa.CombinedWorkActions.Add(cwa);
                            }
                        }

                        swa.Improvement = Convert.ToBoolean(row["IMPROVEMENT"]);
                        swa.IncludesOverlay = Convert.ToBoolean(row["INCLUDESOVERLAY"]);
                        swa.Preservation = Convert.ToBoolean(row["PRESERVATION"]);
                        swa.Repeatable = Convert.ToBoolean(row["REPEATABLE"]);
                        swa.RepeatFrequency = Convert.ToInt32(row["REPEATFREQUENCY"]);
                        /*
                        string qry2 = @"
                                            select distinct 
                                                        project.pproj_cncp_cd, 
                                                        project.pproj_cncp_desc, 
                                                        project.fos_proj_id, 
                                                        project.estcp_schd_dt, 
                                                        funding.strc_work_tycd, 
                                                        funding.extg_strc_id, 
                                                        funding.wdot_pgm_cd,
                                                        sum(funding.tot_with_dlvy_amt) as cawd, 
                                                        sum(funding.tot_wo_dlvy_amt) as ca

                                            from dot1pmic.dw_pmic_fiip_proj project, 
                                                    dot1pmic.dw_pmic_fiip_ctgy funding

                                            where project.pproj_id = funding.pproj_id
                                                and project.pproj_stus_fl = 'A'
                                                and project.lfcy_stg_cd >= '10'
                                                and trim(project.pproj_fnty_cd) = '3'
                                                and project.estcp_ty_cd = 'LET'
                                                and project.estcp_prmy_cpnt_fl = 'Y'

                                                and funding.extg_strc_id = :strId
                                                and project.fos_proj_id = :fosProjId

                                            group by
                                                project.pproj_cncp_cd, 
                                                project.pproj_cncp_desc, 
                                                project.fos_proj_id, 
                                                project.estcp_schd_dt, 
                                                funding.strc_work_tycd, 
                                                funding.extg_strc_id, 
                                                funding.wdot_pgm_cd
                                        ";

                        OracleParameter[] prms2 = new OracleParameter[2];
                        prms2[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                        prms2[0].Value = swa.StructureId;
                        prms2[1] = new OracleParameter("fosProjId", OracleDbType.Varchar2);
                        prms2[1].Value = swa.FosProjId;
                        DataTable dt2 = ExecuteSelect(qry2, prms2, fiipsConn);

                        if (dt2 != null && dt2.Rows.Count > 0)
                        {
                            // TODO: there may be multipe- handle cases where work type codes are different
                            DataRow dr = dt2.Rows[0];
                            swa.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString();
                            swa.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString();
                            swa.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString();

                            try
                            {
                                swa.TotalCostWithDeliveryAmount = Convert.ToDouble(dr["CAWD"]);
                                swa.TotalCostWithoutDeliveryAmount = Convert.ToDouble(dr["CA"]);
                            }
                            catch { }
                        }
                        */
                        swas.Add(swa);
                        previousEstimatedCompletionDate = currentEstimatedCompletionDate;
                        previousWorkActionCode = currentWorkActionCode;
                    }
                }
            }

            return swas;
        }

        /*
        public List<ProgrammedWorkAction> GetProgrammedWorkActions(DateTime startDate, DateTime endDate, string strId)
        {
            List<ProgrammedWorkAction> pwActions = new List<ProgrammedWorkAction>();
            string qry = @"
                                select Extg_Strc_Id, WorkActionCode, Fos_Project_Id, Pproj_Id, Estcp_Schd_Dt
                                from Pmic
                                where Extg_Strc_Id = @strId
                                    and Estcp_Schd_Dt >= @startDate
                                    and Estcp_Schd_Dt <= @endDate
                            ";
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prms[0].Value = strId;
            prms[1] = new SqlParameter("@startDate", SqlDbType.Date);
            prms[1].Value = startDate;
            prms[2] = new SqlParameter("@endDate", SqlDbType.Date);
            prms[2].Value = endDate;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ProgrammedWorkAction pwAction = new ProgrammedWorkAction();
                    pwAction.StructureId = strId;
                    pwAction.EstimatedCompletionDate = Convert.ToDateTime(row["ESTCP_SCHD_DT"]);
                    pwAction.WorkActionYear = pwAction.EstimatedCompletionDate.Year;
                    pwAction.FosProjId = row["FOS_PROJECT_ID"].ToString();
                    pwAction.PProjId = Convert.ToInt32(row["PPROJ_ID"]);
                    pwActions.Add(pwAction);
                }
            }

            return pwActions;
        }
        */


        public List<ProgrammedWorkAction> GetProgrammedWorkActions(string planningProjectConceptCodes,
                                                                    string fedImprovementTypeCodes,
                                                                    DateTime startDate,
                                                                    DateTime endDate)
        {
            List<ProgrammedWorkAction> pwActions = new List<ProgrammedWorkAction>();
            /*and trim(project.pproj_fnty_cd) = '3'*/
            string qry = @"
                            select distinct 
                                    project.fos_proj_id, 
                                    project.pproj_id, 
                                    project.pproj_cncp_cd, 
                                    project.pproj_cncp_desc,
                                    project.estcp_schd_dt, 
                                    project.pproj_stus_fl,
                                    project.estcp_ty_cd,
                                    category.fed_impt_ty_cd, 
                                    category.fed_impt_ty_desc,
                                    category.extg_strc_id, 
                                    category.plnd_strc_id,   
                                    category.fndg_ctgy_desc,
                                    category.strc_work_tycd, 
                                    category.strc_work_tydc,
                                    category.fndg_ctgy_nb,
                                    trim(project.pproj_fnty_cd) as planningProjFuncTypeCode,
                                    sum(project.tot_with_dlvy_amt) as pawd, 
                                    sum(project.tot_wo_dlvy_amt) as pa,
                                    sum(category.tot_with_dlvy_amt) as cawd, 
                                    sum(category.tot_wo_dlvy_amt) as ca

                            from dot1pmic.dw_pmic_fiip_proj project, 
                                    dot1pmic.dw_pmic_fiip_ctgy category

                            where project.pproj_id = category.pproj_id
                                    and (category.extg_strc_id is not null or category.plnd_strc_id is not null)
                                    and project.pproj_stus_fl = 'A'
                                    and project.lfcy_stg_cd >= '10'
                                    
                                    and project.estcp_ty_cd = 'LET'
                                    and project.estcp_prmy_cpnt_fl = 'Y'
                                    and project.estcp_schd_dt >= :startDate
                                    and project.estcp_schd_dt <= :endDate   

                            group by
                                    project.fos_proj_id, 
                                    project.pproj_id, 
                                    project.pproj_cncp_cd, 
                                    project.pproj_cncp_desc,
                                    project.estcp_schd_dt, 
                                    project.pproj_stus_fl,
                                    project.estcp_ty_cd,
                                    category.fed_impt_ty_cd, 
                                    category.fed_impt_ty_desc,
                                    category.extg_strc_id, 
                                    category.plnd_strc_id,   
                                    category.fndg_ctgy_desc,
                                    category.strc_work_tycd, 
                                    category.strc_work_tydc,
                                    category.fndg_ctgy_nb,
                                    project.pproj_fnty_cd

                            order by project.pproj_cncp_cd, category.fed_impt_ty_cd,
                                    category.extg_strc_id, project.estcp_schd_dt
                        ";

            /*
            string qry = @"
                                select distinct 
                                        project.fos_proj_id, 
                                        project.pproj_id, 
                                        project.pproj_cncp_cd, 
                                        project.pproj_cncp_desc,
                                        project.estcp_schd_dt, 
                                        project.pproj_stus_fl,
                                        project.tot_with_dlvy_amt as pawd, 
                                        project.tot_wo_dlvy_amt as pa,

                                        category.fed_impt_ty_cd, 
                                        category.fed_impt_ty_desc,
                                        category.extg_strc_id, 
                                        category.plnd_strc_id,   
                                        category.fdsrc_tycd,
                                        category.tot_with_dlvy_amt as cawd, 
                                        category.tot_wo_dlvy_amt as ca,
                                        category.fndg_ctgy_desc,
                                        category.strc_work_tycd, 
                                        category.strc_work_tydc,
                                        category.fndg_ctgy_nb

                                from dot1pmic.dw_pmic_fiip_proj project, 
                                        dot1pmic.dw_pmic_fiip_ctgy category

                                where project.pproj_id = category.pproj_id
                            ";

            qry += " and trim(project.pproj_cncp_cd) in (" + planningProjectConceptCodes + ")";
            qry += " and trim(category.fed_impt_ty_cd) in (" + fedImprovementTypeCodes + ")";
            qry += " order by project.pproj_cncp_cd, category.fed_impt_ty_cd";
            */

            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("startDate", OracleDbType.Date);
            prms[0].Value = startDate;
            prms[1] = new OracleParameter("endDate", OracleDbType.Date);
            prms[1].Value = endDate;
            DataTable dt = ExecuteSelect(qry, prms, fiipsConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ProgrammedWorkAction pwAction = new ProgrammedWorkAction();
                    pwAction.FosProjId = dr["FOS_PROJ_ID"].ToString();
                    pwAction.PProjId = Convert.ToInt32(dr["PPROJ_ID"]);
                    pwAction.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim().ToUpper();
                    pwAction.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString();
                    pwAction.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                    pwAction.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();
                    pwAction.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                    pwAction.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();

                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                        pwAction.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);

                    pwAction.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString();

                    try
                    {
                        pwAction.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                    }
                    catch { }

                    try
                    {
                        pwAction.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                    }
                    catch { }

                    //pwAction.FundingSourceTypeCode = dr["FDSRC_TYCD"].ToString();
                    pwAction.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                    pwAction.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                    pwAction.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString();

                    pwAction.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString();
                    pwAction.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString();

                    pwAction.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString();

                    pwActions.Add(pwAction);
                }
            }

            return pwActions;
        }

        public void GetProgrammedWorkAction(ProgrammedWorkAction pWA)
        {
            string qry = @"
                                select distinct 
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,

                                sum(project.tot_with_dlvy_amt) as pawd, 
                                sum(project.tot_wo_dlvy_amt) as pa,
        
                                sum(category.tot_with_dlvy_amt) as cawd, 
                                sum(category.tot_wo_dlvy_amt) as ca

                        from dot1pmic.dw_pmic_fiip_proj project, 
                                dot1pmic.dw_pmic_fiip_ctgy category,
                                dot1pmic.dw_pmic_fiip_cnty county,
                                dot1pmic.dw_pmic_fiip_loc location,
                                dot1pmic.dw_pmic_fiip_pgm program

                        where project.pproj_id = category.pproj_id
                                and project.fos_proj_id = category.fos_proj_id
                                and project.pproj_id = county.pproj_id                                
                                and project.fos_proj_id = county.fos_proj_id
                                and project.pproj_id = location.pproj_id                                
                                and project.fos_proj_id = location.fos_proj_id
                                and project.pproj_id = program.pproj_id                                
                                and project.fos_proj_id = program.fos_proj_id
                                and county.pproj_cnty_prmy_fl = 'Y'
                                and location.ppjl_prte_fl = 'Y'
                                and project.estcp_prmy_cpnt_fl = 'Y'
                                and project.pproj_stus_fl = 'A'
                                and project.lfcy_stg_cd >= '10'
                                and project.pproj_id = :pProjId
                                and project.fos_proj_id = :fosProjId
                                and project.estcp_schd_dt = :estimatedCompletionDate
                                and category.strc_work_tycd = :originalWorkActionCode
                                and category.extg_strc_id = :structureId
                                

                        group by
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl
                            ";
            string strId = pWA.StructureId;

            if (pWA.StructureId.Length == 0 && pWA.NewStructureId.Length > 0)
            {
                strId = pWA.NewStructureId;

                qry = @"select distinct 
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,

                                sum(project.tot_with_dlvy_amt) as pawd, 
                                sum(project.tot_wo_dlvy_amt) as pa,
        
                                sum(category.tot_with_dlvy_amt) as cawd, 
                                sum(category.tot_wo_dlvy_amt) as ca

                        from dot1pmic.dw_pmic_fiip_proj project, 
                                dot1pmic.dw_pmic_fiip_ctgy category,
                                dot1pmic.dw_pmic_fiip_cnty county,
                                dot1pmic.dw_pmic_fiip_loc location,
                                dot1pmic.dw_pmic_fiip_pgm program

                        where project.pproj_id = category.pproj_id
                                and project.fos_proj_id = category.fos_proj_id
                                and project.pproj_id = county.pproj_id                                
                                and project.fos_proj_id = county.fos_proj_id
                                and project.pproj_id = location.pproj_id                                
                                and project.fos_proj_id = location.fos_proj_id
                                and project.pproj_id = program.pproj_id                                
                                and project.fos_proj_id = program.fos_proj_id
                                and county.pproj_cnty_prmy_fl = 'Y'
                                and location.ppjl_prte_fl = 'Y'
                                and project.estcp_prmy_cpnt_fl = 'Y'
                                and project.pproj_stus_fl = 'A'
                                and project.lfcy_stg_cd >= '10'
                                and project.pproj_id = :pProjId
                                and project.fos_proj_id = :fosProjId
                                and project.estcp_schd_dt = :estimatedCompletionDate
                                and category.strc_work_tycd = :originalWorkActionCode
                                and category.plnd_strc_id = :structureId
                                

                        group by
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl
                            ";
            }

            OracleParameter[] prms = new OracleParameter[5];
            prms[0] = new OracleParameter("pProjId", OracleDbType.Int32);
            prms[0].Value = pWA.PProjId;
            prms[1] = new OracleParameter("fosProjId", OracleDbType.Varchar2);
            prms[1].Value = pWA.FosProjId;
            prms[2] = new OracleParameter("estimatedCompletionDate", OracleDbType.Date);
            prms[2].Value = pWA.EstimatedCompletionDate;
            prms[3] = new OracleParameter("workActionCode", OracleDbType.Varchar2);
            prms[3].Value = pWA.OriginalWorkActionCode;
            prms[4] = new OracleParameter("structureId", OracleDbType.Varchar2);
            prms[4].Value = strId;
            //prms[5] = new OracleParameter("newStructureId", OracleDbType.Varchar2);
            //prms[5].Value = pWA.NewStructureId;
            DataTable dt = ExecuteSelect(qry, prms, fiipsConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                pWA.DotRegionCode = dr["DOT_RGN_CD"].ToString().Trim();
                pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                pWA.County = dr["WICY_BASD_RGN_NM"].ToString().Trim();
                pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString().Trim();
                pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString().Trim();
                pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString().Trim();
                pWA.Route = dr["PPJL_RTNM_TXT"].ToString().Trim();
                pWA.SubProgramCode = dr["SUB_PGM_CD"].ToString().Trim();
                pWA.SubProgramDesc = dr["SUB_PGM_DESC"].ToString().Trim();
                pWA.Title = dr["PPROJ_FOS_TITL_TXT"].ToString().Trim();
                pWA.Limit = dr["PPROJ_FOS_LMT_TXT"].ToString().Trim();
                pWA.Concept = dr["PPROJ_FOS_CNCP_TXT"].ToString().Trim();
                pWA.FunctionalTypeCode = dr["PPROJ_FNTY_CD"].ToString().Trim();
                pWA.FunctionalTypeDesc = dr["PPROJ_FNTY_DESC"].ToString().Trim();
                pWA.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString().Trim();
                pWA.WisDOTProgramDesc = dr["WDOT_PGM_DESC"].ToString().Trim();
                pWA.LifeCycleStageCode = dr["LFCY_STG_CD"].ToString().Trim();
                pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim();
                pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString().Trim();
                pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();
                pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();

                try
                {
                    pWA.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                }
                catch { }

                try
                {
                    pWA.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                }
                catch { }

                try
                {
                    pWA.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                }
                catch { }

                try
                {
                    pWA.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                }
                catch { }
            }
        }

        /* Commented out 1/31/17
        public void GetProgrammedWorkAction(ProgrammedWorkAction pWA)
        {
            string qry = @"
                                SELECT
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.FOS_PROJ_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PRM_ORG_BUR,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.SUB_PGM_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FOS_CNCP_TXT,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.FNDG_CTGY_DESC,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYCD,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYDC,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.PLND_STRC_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.TOT_WITH_DLVY_AMT,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.TOT_WO_DLVY_AMT

                                FROM DOT1PMIC.DW_PMIC_FIIP_PROJ,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY
                                
                                WHERE DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID = :strId
                                    AND DOT1PMIC.DW_PMIC_FIIP_PROJ.FOS_PROJ_ID = :fosProjId
                                    AND DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID = :pprojId
                                    AND DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID = DOT1PMIC.DW_PMIC_FIIP_CTGY.PPROJ_ID

                            ";
            OracleParameter[] prms = new OracleParameter[3];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = pWA.StructureId;
            prms[1] = new OracleParameter("fosProjId", OracleDbType.Varchar2);
            prms[1].Value = pWA.FosProjId;
            prms[2] = new OracleParameter("pprojId", OracleDbType.Varchar2);
            prms[2].Value = pWA.PProjId;
            DataTable dt = ExecuteSelect(qry, prms, fiipsConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                pWA.EstimatedCompletionDate = DateTime.Parse(dr["ESTCP_SCHD_DT"].ToString());
            }
        }
        */
        /*
         SELECT DISTINCT
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.FOS_PROJ_ID,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PRM_ORG_BUR,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.SUB_PGM_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FOS_CNCP_TXT,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.FNDG_CTGY_DESC,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYCD,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYDC,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.PLND_STRC_ID,

                                  Sum(DOT1PMIC.DW_PMIC_FIIP_CTGY.TOT_WITH_DLVY_AMT) AS SumOfTOT_WITH_DLVY_AMT,

                                  Sum(DOT1PMIC.DW_PMIC_FIIP_CTGY.TOT_WO_DLVY_AMT) AS SumOfTOT_WO_DLVY_AMT

                              FROM DOT1PMIC.DW_PMIC_FIIP_PROJ,
                                        DOT1PMIC.DW_PMIC_FIIP_CTGY
  
                              WHERE DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID = DOT1PMIC.DW_PMIC_FIIP_CTGY.PPROJ_ID
                   

                              GROUP BY
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.FOS_PROJ_ID,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PRM_ORG_BUR,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.SUB_PGM_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FOS_CNCP_TXT,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD,
                                  DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.FNDG_CTGY_DESC,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYCD,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYDC, 
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID,
                                  DOT1PMIC.DW_PMIC_FIIP_CTGY.PLND_STRC_ID

                              HAVING 
                               (   (  ( DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL ) = 'A' )  AND
   
                                   (  ( DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD ) >= '10' )  AND
       
                                   (  trim( DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD ) = '3' )  AND       

                                   (  ( DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD ) = 'LET' )  AND
       
                                   (  ( DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL ) = 'Y' )  AND
       
                                   (  ( DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT )  >=  :startDate And
                                      ( DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT )  <=  :endDate )     )
        */



        /*
        ing qry = @"
                                select Extg_Strc_Id, Pmic.WorkActionCode, WorkAction.WorkActionDesc, Fos_Proj_Id, Pproj_Id, Estcp_Schd_Dt
                                from Pmic, WorkAction
                                where Extg_Strc_Id = @strId
                                    and Estcp_Schd_Dt >= @startDate
                                    and Estcp_Schd_Dt <= @endDate
                                    and Pmic.WorkActionCode = WorkAction.WorkActionCode
                                order by Estcp_Schd_Dt desc
                            ";
            SqlParameter[] prms = new SqlParameter[3];
            prms[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prms[0].Value = strId;
            prms[1] = new SqlParameter("@startDate", SqlDbType.Date);
            prms[1].Value = startDate;
            prms[2] = new SqlParameter("@endDate", SqlDbType.Date);
            prms[2].Value = endDate;
            DataTable dt = ExecuteSelect(qry, prms, samConn);
        */

        // Determine if existing improvement action from PMIC is already in WiSAMS
        public ProgrammedWorkAction GetWiSamsImprovementAction(ProgrammedWorkAction existingPmicWa)
        {
            ProgrammedWorkAction pWA = null;
            string qry = @"
                                select ic.*, c.Pproj_Cncp_Desc
                                from Improvement ic, ImprovementConcept c
                                where ic.Pproj_Cncp_Cd = c.Pproj_Cncp_Cd
                                    and ic.Pproj_Id = @pProjId
                                    and ic.Wdot_Pgm_Cd = @wisDOTProgramCode
                            ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@pProjId", SqlDbType.VarChar);
            prms[0].Value = existingPmicWa.PProjId;
            prms[1] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
            prms[1].Value = existingPmicWa.WisDOTProgramCode;

            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                pWA = new ProgrammedWorkAction();
                pWA.ImprId = Convert.ToInt32(dr["IMPRID"]);
                pWA.DotRegionCode = dr["DOT_RGN_CD"].ToString().Trim();
                //pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                //pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                pWA.County = dr["WICY_BASD_RGN_NM"].ToString().Trim();
                //pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString().Trim();
                //pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString().Trim();
                //pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString().Trim();
                //pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString().Trim();
                pWA.Route = dr["PPJL_RTNM_TXT"].ToString().Trim();
                pWA.SubProgramCode = dr["SUB_PGM_CD"].ToString().Trim();
                pWA.SubProgramDesc = dr["SUB_PGM_DESC"].ToString().Trim();
                pWA.Title = dr["PPROJ_FOS_TITL_TXT"].ToString().Trim();
                pWA.Limit = dr["PPROJ_FOS_LMT_TXT"].ToString().Trim();
                pWA.Concept = dr["PPROJ_FOS_CNCP_TXT"].ToString().Trim();
                pWA.FunctionalTypeCode = dr["PPROJ_FNTY_CD"].ToString().Trim();
                pWA.FunctionalTypeDesc = dr["PPROJ_FNTY_DESC"].ToString().Trim();
                pWA.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString().Trim();
                pWA.WisDOTProgramDesc = dr["WDOT_PGM_DESC"].ToString().Trim();
                pWA.LifeCycleStageCode = dr["LFCY_STG_CD"].ToString().Trim();
                //pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim();
                //pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString().Trim();
                pWA.PlanningProjectConceptCodeNew = dr["PPROJ_CNCP_CD_NEW"].ToString().Trim();
                //pWA.PlanningProjectConceptDescNew = dr["PPROJ_CNCP_DESC_NEW"].ToString().Trim();

                //pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();
                //pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                //WA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();
            }

            return pWA;
        }

        // Determine if existing work action from PMIC is already in WiSAMS
        public ProgrammedWorkAction GetWiSamsWorkAction(ProgrammedWorkAction existingPmicWa)
        {
            ProgrammedWorkAction pWA = null;
            string qry = @"
                                select *
                                from Pmic
                                where Pproj_Id = @pProjId
                                    and Fos_Proj_Id = @fosProjId
                                    and Fndg_Ctgy_Nb = @fundingCategoryNumber
                                    and Wdot_Pgm_Cd = @wisDOTProgramCode
                            ";

            SqlParameter[] prms = new SqlParameter[4];
            prms[0] = new SqlParameter("@pProjId", SqlDbType.VarChar);
            prms[0].Value = existingPmicWa.PProjId;
            prms[1] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
            prms[1].Value = existingPmicWa.FosProjId;
            prms[2] = new SqlParameter("@fundingCategoryNumber", SqlDbType.VarChar);
            prms[2].Value = existingPmicWa.FundingCategoryNumber;
            prms[3] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
            prms[3].Value = existingPmicWa.WisDOTProgramCode;

            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                pWA = new ProgrammedWorkAction();
                pWA.FiipsId = Convert.ToInt32(dr["FIIPSID"]);
                pWA.DotRegionCode = dr["DOT_RGN_CD"].ToString().Trim();
                pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                pWA.County = dr["WICY_BASD_RGN_NM"].ToString().Trim();
                pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString().Trim();
                pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString().Trim();
                pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString().Trim();
                pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString().Trim();
                pWA.WorkActionCode = dr["WORKACTIONCODE"].ToString().Trim();
                pWA.Route = dr["PPJL_RTNM_TXT"].ToString().Trim();
                pWA.SubProgramCode = dr["SUB_PGM_CD"].ToString().Trim();
                pWA.SubProgramDesc = dr["SUB_PGM_DESC"].ToString().Trim();
                pWA.Title = dr["PPROJ_FOS_TITL_TXT"].ToString().Trim();
                pWA.Limit = dr["PPROJ_FOS_LMT_TXT"].ToString().Trim();
                pWA.Concept = dr["PPROJ_FOS_CNCP_TXT"].ToString().Trim();
                pWA.FunctionalTypeCode = dr["PPROJ_FNTY_CD"].ToString().Trim();
                pWA.FunctionalTypeDesc = dr["PPROJ_FNTY_DESC"].ToString().Trim();
                pWA.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString().Trim();
                pWA.WisDOTProgramDesc = dr["WDOT_PGM_DESC"].ToString().Trim();
                pWA.LifeCycleStageCode = dr["LFCY_STG_CD"].ToString().Trim();
                pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim();
                //pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString().Trim();
                pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();
                pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();

                /*
                try
                {
                    pWA.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                }
                catch { }

                try
                {
                    pWA.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                }
                catch { }

                try
                {
                    pWA.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                }
                catch { }

                try
                {
                    pWA.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                }
                catch { }
                */
            }

            return pWA;
        }

        public bool PullPmicData()
        {
            if (!DoPmicTablesHaveRecords())
            {
                return false;
            }

            return true;
        }

        public bool DoPmicTablesHaveRecords()
        {
            string qry = @"
                                select count(pproj_id) as pprojIdCount
                                from dot1pmic.dw_pmic_fiip_proj
                            ";
            DataTable dt = ExecuteSelect(qry, fiipsConn);

            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            qry = @"
                        select count(pproj_id) as pprojIdCount
                        from dot1pmic.dw_pmic_fiip_ctgy
                    ";

            dt = ExecuteSelect(qry, fiipsConn);

            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            qry = @"
                        select count(pproj_id) as pprojIdCount
                        from dot1pmic.dw_pmic_fiip_cnty
                    ";

            dt = ExecuteSelect(qry, fiipsConn);

            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            qry = @"
                        select count(pproj_id) as pprojIdCount
                        from dot1pmic.dw_pmic_fiip_loc
                    ";

            dt = ExecuteSelect(qry, fiipsConn);

            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            qry = @"
                        select count(pproj_id) as pprojIdCount
                        from dot1pmic.dw_pmic_fiip_pgm
                    ";

            dt = ExecuteSelect(qry, fiipsConn);

            if (dt == null || dt.Rows.Count == 0)
            {
                return false;
            }

            return true;
        }

        public void UpdateWiSamsWithImprovement(DateTime startDate, DateTime endDate)
        {
            List<ProgrammedWorkAction> pWAs = GetImprovementWorkActions(startDate, endDate);

            foreach (var pWA in pWAs)
            {
                ProgrammedWorkAction samWA = GetWiSamsImprovementAction(pWA);

                if (samWA != null)
                {
                    string qry = @"
                                    update Improvement

                                    set Dot_Rgn_Cd = @dotRegionCode,
                                            Wicy_Basd_Rgn_Nm = @county,
                                            Fos_Proj_Id = @fosProjId,
                                            Pproj_Id = @pProjId,

                                            Estcp_Schd_Dt = @estimatedCompletionDate,
                                            State_Fiscal_Year = @stateFiscalYear,
                                            Ppjl_Rtnm_Txt = @route,
                                            Sub_Pgm_Cd = @subProgramCode,
                                            Sub_Pgm_Desc = @subProgramDesc,
                                            Pproj_Fos_Titl_Txt = @title,
                                            Pproj_Fos_Lmt_Txt = @limit,
                                            Pproj_Fos_Cncp_Txt = @concept,
                                            Pproj_Fnty_Cd = @functionalTypeCode,
                                            Pproj_Fnty_Desc = @functionalTypeDesc,
                                            Wdot_Pgm_Cd = @wisDOTProgramCode,
                                            Wdot_Pgm_Desc = @wisDOTProgramDesc,
                                            Lfcy_Stg_Cd = @lifeCycleStageCode,

                                            Row_Isrt_Tmst = @rowInsertTimeStamp,
                                            
                                           

                                            Pproj_Cncp_Cd = @planningProjectConceptCode,
                                            
                                            Pproj_Cncp_Cd_New = @planningProjectConceptCodeNew,

                                            Mgr_Pproj_Ptcp_Nm = @projectManager

                                    where ImprId = @imprId
                                    ";

                    SqlParameter[] prms = new SqlParameter[22];
                    prms[0] = new SqlParameter("@dotRegionCode", SqlDbType.VarChar);
                    prms[0].Value = pWA.DotRegionCode;
                    prms[1] = new SqlParameter("@county", SqlDbType.VarChar);
                    prms[1].Value = pWA.County;
                    prms[2] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
                    prms[2].Value = pWA.FosProjId;
                    prms[3] = new SqlParameter("@pProjId", SqlDbType.Int);
                    prms[3].Value = pWA.PProjId;

                    prms[4] = new SqlParameter("@estimatedCompletionDate", SqlDbType.DateTime);
                    prms[4].Value = pWA.EstimatedCompletionDate;

                    if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
                    {
                        prms[4].Value = DBNull.Value;
                    }

                    int fiscalYear = pWA.EstimatedCompletionDate.Year;

                    if (pWA.EstimatedCompletionDate.Month > 6)
                    {
                        fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
                    }

                    prms[5] = new SqlParameter("@stateFiscalYear", SqlDbType.Int);
                    prms[5].Value = fiscalYear;

                    if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
                    {
                        prms[5].Value = DBNull.Value;
                    }

                    prms[6] = new SqlParameter("@route", SqlDbType.VarChar);
                    prms[6].Value = pWA.Route;
                    prms[7] = new SqlParameter("@subProgramCode", SqlDbType.VarChar);
                    prms[7].Value = pWA.SubProgramCode;
                    prms[8] = new SqlParameter("@subProgramDesc", SqlDbType.VarChar);
                    prms[8].Value = pWA.SubProgramDesc;
                    prms[9] = new SqlParameter("@title", SqlDbType.VarChar);
                    prms[9].Value = pWA.Title;
                    prms[10] = new SqlParameter("@limit", SqlDbType.VarChar);
                    prms[10].Value = pWA.Limit;
                    prms[11] = new SqlParameter("@concept", SqlDbType.VarChar);
                    prms[11].Value = pWA.Concept;
                    prms[12] = new SqlParameter("@functionalTypeCode", SqlDbType.VarChar);
                    prms[12].Value = pWA.FunctionalTypeCode;
                    prms[13] = new SqlParameter("@functionalTypeDesc", SqlDbType.VarChar);
                    prms[13].Value = pWA.FunctionalTypeDesc;
                    prms[14] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
                    prms[14].Value = pWA.WisDOTProgramCode;
                    prms[15] = new SqlParameter("@wisDOTProgramDesc", SqlDbType.VarChar);
                    prms[15].Value = pWA.WisDOTProgramDesc;
                    prms[16] = new SqlParameter("@lifeCycleStageCode", SqlDbType.VarChar);
                    prms[16].Value = pWA.LifeCycleStageCode;

                    prms[17] = new SqlParameter("@rowInsertTimeStamp", SqlDbType.DateTime);
                    prms[17].Value = DateTime.Now;


                    prms[18] = new SqlParameter("@planningProjectConceptCode", SqlDbType.VarChar);
                    prms[18].Value = pWA.PlanningProjectConceptCode;

                    prms[19] = new SqlParameter("@planningProjectConceptCodeNew", SqlDbType.VarChar);
                    prms[19].Value = pWA.PlanningProjectConceptCode;

                    /*
                    prms[19] = new SqlParameter("@planningProjectConceptCodeNew", SqlDbType.VarChar);
                    prms[19].Value = samWA.PlanningProjectConceptCodeNew;
                    */

                    /*
                    if (!pWA.PlanningProjectConceptCode.Equals(samWA.PlanningProjectConceptCode) || String.IsNullOrEmpty(samWA.PlanningProjectConceptCodeNew))
                    {
                        prms[19].Value = DBNull.Value;
                    }
                    */

                    prms[20] = new SqlParameter("@imprId", SqlDbType.Int);
                    prms[20].Value = samWA.ImprId;

                    prms[21] = new SqlParameter("@projectManager", SqlDbType.VarChar);
                    prms[21].Value = pWA.ProjectManager;

                    ExecuteInsertUpdateDelete(qry, prms, samConn);
                }
                else
                {
                    InsertImprovementRow(pWA);
                }
            }

        }
        /*
         @dotRegionCode,
                                    @county,
                                    @fosProjId,
                                    @pProjId,
                                    
                                    @estimatedCompletionDate,
                                    @stateFiscalYear,
                                    @route,
                                    @subProgramCode,
                                    @subProgramDesc,
                                    @title,
                                    @limit,
                                    @concept,
                                    @functionalTypeCode,
                                    @functionalTypeDesc,
                                    @wisDOTProgramCode,
                                    @wisDOTProgramDesc,
                                    @lifeCycleStageCode,
                                    
                                    @rowInsertTimeStamp,

                                    @planningProjectConceptCode,
                                    
                                    @planningProjectConceptCodeNew*/

        public void UpdateWiSamsWithPmicStructure(WisamType.PmicProjectTypes pmicProjectType, DateTime startDate, DateTime endDate)
        {
            List<ProgrammedWorkAction> pWAs = GetProgrammedWorkActionsStructure(pmicProjectType, startDate, endDate);
            string year = DateTime.Today.Year.ToString();
            string month = DateTime.Today.Month < 10 ? "0" + DateTime.Today.Month.ToString() : DateTime.Today.Month.ToString();
            string day = DateTime.Today.Day < 10 ? "0" + DateTime.Today.Day.ToString() : DateTime.Today.Day.ToString();
            int dateStamp = Convert.ToInt32(year + month + day);

            foreach (var pWA in pWAs)
            {
                // Refreshing Pmic table

                ProgrammedWorkAction samWA = GetWiSamsWorkAction(pWA);

                if (samWA != null) //exists in WiSAMS
                {
                    string qry = @"
                                    update Pmic
                                    set Dot_Rgn_Cd = @dotRegionCode,
                                        Extg_Strc_Id = @structureId,
                                        Wicy_Basd_Rgn_Nm = @county,
                                        Fos_Proj_Id = @fosProjId,
                                        Pproj_Id = @pProjId,
                                        Plnd_Strc_Id = @newStructureId,
                                        Fndg_Ctgy_Nb = @fundingCategoryNumber,
                                        Strc_Work_Tycd = @originalWorkActionCode,
                                        Strc_Work_Tydc = @originalWorkActionDesc,
                                        WorkActionCode = @workActionCode,
                                        WorkActionDesc = @workActionDesc,
                                        Estcp_Schd_Dt = @estimatedCompletionDate,
                                        State_Fiscal_Year = @stateFiscalYear,
                                        Ppjl_Rtnm_Txt = @route,
                                        Sub_Pgm_Cd = @subProgramCode,
                                        Sub_Pgm_Desc = @subProgramDesc,
                                        Pproj_Fos_Titl_Txt = @title,
                                        Pproj_Fos_Lmt_Txt = @limit,
                                        Pproj_Fos_Cncp_Txt = @concept,
                                        Pproj_Fnty_Cd = @functionalTypeCode,
                                        Pproj_Fnty_Desc = @functionalTypeDesc,
                                        Wdot_Pgm_Cd = @wisDOTProgramCode,
                                        Wdot_Pgm_Desc = @wisDOTProgramDesc,
                                        Lfcy_Stg_Cd = @lifeCycleStageCode,
                                        Tot_With_Dlvy_Amt = @workTotalWithDeliveryAmount,
                                        Tot_Wo_Dlvy_Amt = @workTotalWithoutDeliveryAmount,

                                        Row_Isrt_Tmst = @rowInsertTimeStamp,
                                        Pproj_Cncp_Cd = @planningProjectConceptCode,
                                        Estcp_Ty_Cd = @componentTypeCode,
                                        Pproj_Stus_Fl = @projectStatusFlag,
                                        Fndg_Ctgy_Desc = @fundingCategoryDesc,
                                        Estcp_Prmy_Cpnt_Fl = 'Y',
                                        Fed_Impt_Ty_Cd = @federalImprovementTypeCode,
                                        Fed_Impt_Ty_Desc = @federalImprovementTypeDesc,
                                        Mgr_Pproj_Ptcp_Nm = @projectManager,
                                        RowInsertDateStamp = @rowInsertDateStamp

                                    where FiipsId = @fiipsId     
                                ";
                    SqlParameter[] prms = new SqlParameter[36];
                    prms[0] = new SqlParameter("@dotRegionCode", SqlDbType.VarChar);
                    prms[0].Value = pWA.DotRegionCode;
                    prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
                    prms[1].Value = pWA.StructureId;
                    prms[2] = new SqlParameter("@county", SqlDbType.VarChar);
                    prms[2].Value = pWA.County;
                    prms[3] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
                    prms[3].Value = pWA.FosProjId;
                    prms[4] = new SqlParameter("@pProjId", SqlDbType.Int);
                    prms[4].Value = pWA.PProjId;
                    prms[5] = new SqlParameter("@newStructureId", SqlDbType.VarChar);
                    prms[5].Value = pWA.NewStructureId;
                    prms[6] = new SqlParameter("@fundingCategoryNumber", SqlDbType.VarChar);
                    prms[6].Value = pWA.FundingCategoryNumber;
                    prms[7] = new SqlParameter("@originalWorkActionCode", SqlDbType.VarChar);
                    prms[7].Value = pWA.OriginalWorkActionCode;
                    prms[8] = new SqlParameter("@originalWorkActionDesc", SqlDbType.VarChar);
                    prms[8].Value = pWA.OriginalWorkActionDesc;

                    // 4/25/2017 PMIC has new structure work types so the original and new/translated work types should be the same
                    prms[9] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                    prms[9].Value = pWA.OriginalWorkActionCode;
                    prms[10] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
                    prms[10].Value = pWA.OriginalWorkActionDesc;

                    /*
                    prms[9] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                    prms[9].Value = samWA.WorkActionCode;
                    prms[10] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
                    prms[10].Value = "";
                    */

                    /*
                    if (String.IsNullOrEmpty(samWA.WorkActionCode))
                    {
                        string xxx = "";
                    }
                    */

                    /*
                    if (!pWA.OriginalWorkActionCode.Equals(samWA.OriginalWorkActionCode) || String.IsNullOrEmpty(samWA.WorkActionCode))
                    {
                        // Nullify new work action
                        prms[9].Value = DBNull.Value;
                        prms[10].Value = DBNull.Value;
                    }
                    */

                    prms[11] = new SqlParameter("@estimatedCompletionDate", SqlDbType.DateTime);
                    prms[11].Value = pWA.EstimatedCompletionDate;

                    if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
                    {
                        prms[11].Value = DBNull.Value;
                    }

                    int fiscalYear = pWA.EstimatedCompletionDate.Year;

                    if (pWA.EstimatedCompletionDate.Month > 6)
                    {
                        fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
                    }

                    prms[12] = new SqlParameter("@stateFiscalYear", SqlDbType.Int);
                    prms[12].Value = fiscalYear;

                    if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
                    {
                        prms[12].Value = DBNull.Value;
                    }

                    prms[13] = new SqlParameter("@route", SqlDbType.VarChar);
                    prms[13].Value = pWA.Route;
                    prms[14] = new SqlParameter("@subProgramCode", SqlDbType.VarChar);
                    prms[14].Value = pWA.SubProgramCode;
                    prms[15] = new SqlParameter("@subProgramDesc", SqlDbType.VarChar);
                    prms[15].Value = pWA.SubProgramDesc;
                    prms[16] = new SqlParameter("@title", SqlDbType.VarChar);
                    prms[16].Value = pWA.Title;
                    prms[17] = new SqlParameter("@limit", SqlDbType.VarChar);
                    prms[17].Value = pWA.Limit;
                    prms[18] = new SqlParameter("@concept", SqlDbType.VarChar);
                    prms[18].Value = pWA.Concept;
                    prms[19] = new SqlParameter("@functionalTypeCode", SqlDbType.VarChar);
                    prms[19].Value = pWA.FunctionalTypeCode;
                    prms[20] = new SqlParameter("@functionalTypeDesc", SqlDbType.VarChar);
                    prms[20].Value = pWA.FunctionalTypeDesc;
                    prms[21] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
                    prms[21].Value = pWA.WisDOTProgramCode;
                    prms[22] = new SqlParameter("@wisDOTProgramDesc", SqlDbType.VarChar);
                    prms[22].Value = pWA.WisDOTProgramDesc;
                    prms[23] = new SqlParameter("@lifeCycleStageCode", SqlDbType.VarChar);
                    prms[23].Value = pWA.LifeCycleStageCode;
                    prms[24] = new SqlParameter("@workTotalWithDeliveryAmount", SqlDbType.Float);
                    prms[24].Value = pWA.WorkTotalWithDeliveryAmount;
                    prms[25] = new SqlParameter("@workTotalWithoutDeliveryAmount", SqlDbType.Float);
                    prms[25].Value = pWA.WorkTotalWithoutDeliveryAmount;

                    prms[26] = new SqlParameter("@rowInsertTimeStamp", SqlDbType.DateTime);
                    prms[26].Value = DateTime.Now.Date;
                    prms[27] = new SqlParameter("@planningProjectConceptCode", SqlDbType.VarChar);
                    prms[27].Value = pWA.PlanningProjectConceptCode;
                    prms[28] = new SqlParameter("@componentTypeCode", SqlDbType.VarChar);
                    prms[28].Value = pWA.ComponentTypeCode;
                    prms[29] = new SqlParameter("@projectStatusFlag", SqlDbType.VarChar);
                    prms[29].Value = pWA.ProjectStatusFlag;
                    prms[30] = new SqlParameter("@fundingCategoryDesc", SqlDbType.VarChar);
                    prms[30].Value = pWA.FundingCategoryDesc;
                    prms[31] = new SqlParameter("@federalImprovementTypeCode", SqlDbType.VarChar);
                    prms[31].Value = pWA.FederalImprovementTypeCode;
                    prms[32] = new SqlParameter("@federalImprovementTypeDesc", SqlDbType.VarChar);
                    prms[32].Value = pWA.FederalImprovementTypeDesc;
                    prms[33] = new SqlParameter("@projectManager", SqlDbType.VarChar);
                    prms[33].Value = pWA.ProjectManager;
                    prms[34] = new SqlParameter("@fiipsId", SqlDbType.Int);
                    prms[34].Value = samWA.FiipsId;
                    prms[35] = new SqlParameter("@rowInsertDateStamp", SqlDbType.Int);
                    prms[35].Value = dateStamp;

                    ExecuteInsertUpdateDelete(qry, prms, samConn);
                }
                else // doesn't exist in WiSAMS- insert into WiSAMS
                {
                    InsertPmicRow(pWA, dateStamp);
                }

                // Inserting row into StructureProgramDailyInventory
                InsertStructureProgramDailyInventoryRow(dateStamp, pWA);
            }
        }

        public List<ProgrammedWorkAction> GetImprovementWorkActions(DateTime startDate, DateTime endDate)
        {
            List<ProgrammedWorkAction> pwActions = new List<ProgrammedWorkAction>();
            string qry = @"
                            SELECT DISTINCT                                                                                                                      
                              DW_PMIC_FIIP_PROJ.FOS_PROJ_ID,                                                                                                    
                              DW_PMIC_FIIP_PROJ.PPROJ_ID,                                                                                                       
                              DW_PMIC_FIIP_PROJ.PRM_ORG_BUR,                                                                                                    
                              DW_PMIC_FIIP_PROJ.SUB_PGM_CD,
                              DW_PMIC_FIIP_PROJ.SUB_PGM_DESC,  
                              DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD,                                                                                                    
                              DW_PMIC_FIIP_PROJ.PPROJ_FNTY_DESC,                                                                                                  
                              DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD, 
                              DW_PMIC_FIIP_PROJ.PPROJ_CNCP_DESC,                                                                                                 
                              DW_PMIC_FIIP_PROJ.LFCY_STG_CD,                                                                                                    
                              DW_PMIC_FIIP_PROJ.PPROJ_FOS_TITL_TXT,                                                                                             
                              DW_PMIC_FIIP_PROJ.PPROJ_FOS_LMT_TXT,                                                                                              
                              DW_PMIC_FIIP_PROJ.PPROJ_FOS_CNCP_TXT,                                                                                             
                              DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL,                                                                                             
                              DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT,                                                                                                  
                              DW_PMIC_FIIP_PROJ.ESTCP_TY_CD,                                                                                                    
                              DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL,
      
                              DW_PMIC_FIIP_LOC.PPJL_RTNM_TXT,
      
                              DW_PMIC_FIIP_CNTY.DOT_RGN_CD,
                              DW_PMIC_FIIP_CNTY.WICY_BASD_RGN_NM,
                              DW_PMIC_FIIP_PGM.wdot_pgm_cd,
                              DW_PMIC_FIIP_PGM.wdot_pgm_desc,
                              DW_PMIC_FIIP_PROJ.mgr_pproj_ptcp_nm
      
                                                                                                                                        
                          FROM DOT1PMIC.DW_PMIC_FIIP_PROJ
                                 INNER JOIN
                               DOT1PMIC.DW_PMIC_FIIP_LOC
     
                                ON DW_PMIC_FIIP_PROJ.PPROJ_ID = DW_PMIC_FIIP_LOC.PPROJ_ID AND
                                trim(DW_PMIC_FIIP_PROJ.pproj_rcntl_tycd) not in ('P','R','X') AND                                                                                        
                                DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL  = 'A'   AND                                                                                  
                                DW_PMIC_FIIP_PROJ.LFCY_STG_CD  >= '10'   AND                                                                                    
                                DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL  = 'Y'   AND                                                                               
                                                                                                                                        
                                                                                                        
                                                                                                                                        
                                
 
                                DW_PMIC_FIIP_LOC.PPJL_PRTE_FL = 'Y'        
             
             
                                INNER JOIN
        
                             DOT1PMIC.DW_PMIC_FIIP_CNTY
                                ON DW_PMIC_FIIP_PROJ.PPROJ_ID = DW_PMIC_FIIP_CNTY.PPROJ_ID AND
                                   DW_PMIC_FIIP_CNTY.PPROJ_CNTY_PRMY_FL = 'Y' 

                                INNER JOIN

                             DOT1PMIC.DW_PMIC_FIIP_PGM
                                ON DW_PMIC_FIIP_PROJ.PPROJ_ID = DW_PMIC_FIIP_PGM.PPROJ_ID

                            where
                                DOT1PMIC.DW_PMIC_FIIP_PROJ.estcp_schd_dt >= :startDate
                                and DOT1PMIC.DW_PMIC_FIIP_PROJ.estcp_schd_dt <= :endDate
                            ";

            /*
             ( DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD = 'RDMTN ' or                                                                                   
                                DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD = 'RESURF' or                                                                                   
                                DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD = 'RECOND' or                                                                                   
                                DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD = 'PVRPLA')        AND
*/

            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("startDate", OracleDbType.Date);
            prms[0].Value = startDate;
            prms[1] = new OracleParameter("endDate", OracleDbType.Date);
            prms[1].Value = endDate;
            DataTable dt = ExecuteSelect(qry, fiipsConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ProgrammedWorkAction pWA = new ProgrammedWorkAction();
                    pWA.FosProjId = dr["FOS_PROJ_ID"].ToString().Trim();
                    pWA.PProjId = Convert.ToInt32(dr["PPROJ_ID"]);
                    pWA.DotRegionCode = dr["DOT_RGN_CD"].ToString().Trim();
                    //pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                    //pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                    pWA.County = dr["WICY_BASD_RGN_NM"].ToString().Trim();
                    //pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString().Trim();
                    //pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString().Trim();
                    //pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString().Trim();
                    //pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString().Trim();

                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                    {
                        pWA.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);
                    }
                    //pWA.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);
                    //pWA.WorkActionCode = "";
                    //pWA.WorkActionDesc = "";
                    pWA.Route = dr["PPJL_RTNM_TXT"].ToString().Trim();
                    pWA.SubProgramCode = dr["SUB_PGM_CD"].ToString().Trim();
                    pWA.SubProgramDesc = dr["SUB_PGM_DESC"].ToString().Trim();
                    pWA.Title = dr["PPROJ_FOS_TITL_TXT"].ToString().Trim();
                    pWA.Limit = dr["PPROJ_FOS_LMT_TXT"].ToString().Trim();
                    pWA.Concept = dr["PPROJ_FOS_CNCP_TXT"].ToString().Trim();
                    pWA.FunctionalTypeCode = dr["PPROJ_FNTY_CD"].ToString().Trim();
                    pWA.FunctionalTypeDesc = dr["PPROJ_FNTY_DESC"].ToString().Trim();
                    pWA.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString().Trim();
                    pWA.WisDOTProgramDesc = dr["WDOT_PGM_DESC"].ToString().Trim();
                    pWA.LifeCycleStageCode = dr["LFCY_STG_CD"].ToString().Trim();
                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                    pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim();
                    pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString().Trim();
                    pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();
                    pWA.ProjectManager = dr["MGR_PPROJ_PTCP_NM"].ToString().Trim();
                    //pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                    //pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();

                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                    {
                        int fiscalYear = pWA.EstimatedCompletionDate.Year;

                        if (pWA.EstimatedCompletionDate.Month > 6)
                        {
                            fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
                        }

                        pWA.StateFiscalYear = fiscalYear;
                    }

                    pwActions.Add(pWA);
                }
            }

            return pwActions;
        }

        public List<ProgrammedWorkAction> GetProgrammedWorkActionsStructure(string fosProjectId, DateTime letDate)
        {
            List<ProgrammedWorkAction> programmedWorkActions = new List<ProgrammedWorkAction>();
            string qry = "";


            qry = @"
                        select distinct 
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.lfcy_stg_desc,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm,

                                sum(project.tot_with_dlvy_amt) as pawd, 
                                sum(project.tot_wo_dlvy_amt) as pa,
        
                                sum(category.tot_with_dlvy_amt) as cawd, 
                                sum(category.tot_wo_dlvy_amt) as ca

                        from dot1pmic.dw_pmic_fiip_proj project, 
                                dot1pmic.dw_pmic_fiip_ctgy category,
                                dot1pmic.dw_pmic_fiip_cnty county,
                                dot1pmic.dw_pmic_fiip_loc location,
                                dot1pmic.dw_pmic_fiip_pgm program

                        where project.pproj_id = category.pproj_id
                                and project.fos_proj_id = :fosProjectId
                                and project.fos_proj_id = category.fos_proj_id
                                and project.pproj_id = county.pproj_id                                
                                and project.fos_proj_id = county.fos_proj_id
                                and project.pproj_id = location.pproj_id                                
                                and project.fos_proj_id = location.fos_proj_id
                                and project.pproj_id = program.pproj_id                                
                                and project.fos_proj_id = program.fos_proj_id
                                and county.pproj_cnty_prmy_fl = 'Y'
                                and location.ppjl_prte_fl = 'Y'
                                and project.pproj_stus_fl = 'A'
                                and project.lfcy_stg_cd >= '10'
                                
                                and project.estcp_prmy_cpnt_fl = 'Y'
                                and project.estcp_schd_dt = :letDate
                                
                                and trim(project.pproj_rcntl_tycd) not in ('P','R','X')
                                
                        group by
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.lfcy_stg_desc,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm

                            order by strc_work_tycd,
                                        extg_strc_id
                            ";

            DataTable dt = null;
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("fosProjectId", OracleDbType.Varchar2);
            prms[0].Value = fosProjectId;
            prms[1] = new OracleParameter("letDate", OracleDbType.Date);
            prms[1].Value = letDate;
            dt = ExecuteSelect(qry, prms, fiipsConn);

            //and category.fed_impt_ty_cd <> '04'
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ProgrammedWorkAction pWA = new ProgrammedWorkAction();
                    pWA.FosProjId = dr["FOS_PROJ_ID"].ToString().Trim();
                    pWA.PProjId = Convert.ToInt32(dr["PPROJ_ID"]);
                    pWA.DotRegionCode = dr["DOT_RGN_CD"].ToString().Trim();
                    pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                    pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                    pWA.County = dr["WICY_BASD_RGN_NM"].ToString().Trim();
                    pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString().Trim();
                    pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString().Trim();
                    pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString().Trim();
                    pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString().Trim();

                    //dr["ESTCP_SCHD_DT"]
                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                    {
                        pWA.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);
                    }

                    //pWA.WorkActionCode = "";
                    //pWA.WorkActionDesc = "";
                    pWA.Route = dr["PPJL_RTNM_TXT"].ToString().Trim();
                    pWA.SubProgramCode = dr["SUB_PGM_CD"].ToString().Trim();
                    pWA.SubProgramDesc = dr["SUB_PGM_DESC"].ToString().Trim();
                    pWA.Title = dr["PPROJ_FOS_TITL_TXT"].ToString().Trim();
                    pWA.Limit = dr["PPROJ_FOS_LMT_TXT"].ToString().Trim();
                    pWA.Concept = dr["PPROJ_FOS_CNCP_TXT"].ToString().Trim();
                    pWA.FunctionalTypeCode = dr["PPROJ_FNTY_CD"].ToString().Trim();
                    pWA.FunctionalTypeDesc = dr["PPROJ_FNTY_DESC"].ToString().Trim();
                    pWA.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString().Trim();
                    pWA.WisDOTProgramDesc = dr["WDOT_PGM_DESC"].ToString().Trim();
                    pWA.LifeCycleStageCode = dr["LFCY_STG_CD"].ToString().Trim();
                    pWA.LifeCycleStageDesc = dr["LFCY_STG_DESC"].ToString().Trim();
                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                    pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim();
                    pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString().Trim();
                    pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();
                    pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                    pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();
                    pWA.ProjectManager = dr["mgr_pproj_ptcp_nm"].ToString();

                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                    {
                        int fiscalYear = pWA.EstimatedCompletionDate.Year;

                        if (pWA.EstimatedCompletionDate.Month > 6)
                        {
                            fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
                        }

                        pWA.StateFiscalYear = fiscalYear;
                    }

                    try
                    {
                        pWA.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                    }
                    catch { }

                    programmedWorkActions.Add(pWA);

                    /*
                    pWA.FosProjId = dr["FOS_PROJ_ID"].ToString();
                    pWA.PProjId = Convert.ToInt32(dr["PPROJ_ID"]);
                    pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim().ToUpper();
                    pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString();
                    pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                    pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();
                    pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                    pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                    pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();

                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                        pWA.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);

                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString();

                    try
                    {
                        pWA.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                    }
                    catch { }
                    
                    pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString();
                    pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString();

                    pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString();
                    pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString();
                    */


                }
            }

            return programmedWorkActions;
        }

        public List<ProgrammedWorkAction> GetProgrammedWorkActionsStructure(WisamType.PmicProjectTypes pmicProjectType, DateTime startDate, DateTime endDate)
        {
            List<ProgrammedWorkAction> pwActions = new List<ProgrammedWorkAction>();
            string qry = "";

            switch (pmicProjectType)
            {
                case WisamType.PmicProjectTypes.Predictive:
                    qry = @"
                            select distinct 
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm,

                                sum(project.tot_with_dlvy_amt) as pawd, 
                                sum(project.tot_wo_dlvy_amt) as pa,
        
                                sum(category.tot_with_dlvy_amt) as cawd, 
                                sum(category.tot_wo_dlvy_amt) as ca

                        from dot1pmic.dw_pmic_fiip_proj project, 
                                dot1pmic.dw_pmic_fiip_ctgy category,
                                dot1pmic.dw_pmic_fiip_cnty county,
                                dot1pmic.dw_pmic_fiip_loc location,
                                dot1pmic.dw_pmic_fiip_pgm program

                        where project.pproj_id = category.pproj_id
                                and project.fos_proj_id = category.fos_proj_id
                                and project.pproj_id = county.pproj_id                                
                                and project.fos_proj_id = county.fos_proj_id
                                and project.pproj_id = location.pproj_id                                
                                and project.fos_proj_id = location.fos_proj_id
                                and project.pproj_id = program.pproj_id                                
                                and project.fos_proj_id = program.fos_proj_id
                                and county.pproj_cnty_prmy_fl = 'Y'
                                and location.ppjl_prte_fl = 'Y'
                                and category.extg_strc_id is not null 
                                and category.strc_work_tycd <> 'NB' 
                                and category.strc_work_tycd <> '01'
                                and project.pproj_stus_fl = 'A'
                                and project.lfcy_stg_cd >= '10'
                                and trim(project.pproj_fnty_cd) = '3'
                                and project.estcp_ty_cd = 'LET'
                                and project.estcp_prmy_cpnt_fl = 'Y'
                                and project.estcp_schd_dt >= :startDate
                                
                                and category.strc_work_tycd is not null

                        group by
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm
                            ";
                    break;

                case WisamType.PmicProjectTypes.Planned:
                    qry = @"
                              select distinct 
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.project.mgr_pproj_ptcp_nm,

                                

                                sum(project.tot_with_dlvy_amt) as pawd, 
                                sum(project.tot_wo_dlvy_amt) as pa,
        
                                sum(category.tot_with_dlvy_amt) as cawd, 
                                sum(category.tot_wo_dlvy_amt) as ca

                        from dot1pmic.dw_pmic_fiip_proj project, 
                                dot1pmic.dw_pmic_fiip_ctgy category,
                                dot1pmic.dw_pmic_fiip_cnty county,
                                dot1pmic.dw_pmic_fiip_loc location,
                                dot1pmic.dw_pmic_fiip_pgm program

                        where project.pproj_id = category.pproj_id
                                and project.fos_proj_id = category.fos_proj_id
                                and project.pproj_id = county.pproj_id                                
                                and project.fos_proj_id = county.fos_proj_id
                                and project.pproj_id = location.pproj_id                                
                                and project.fos_proj_id = location.fos_proj_id
                                and project.pproj_id = program.pproj_id                                
                                and project.fos_proj_id = program.fos_proj_id
                                and (category.extg_strc_id is not null or category.plnd_strc_id is not null)
                                and county.pproj_cnty_prmy_fl = 'Y'
                                and location.ppjl_prte_fl = 'Y'
                                and category.extg_strc_id is null
                                and category.plnd_strc_id is not null 
                                and (category.strc_work_tycd = 'NB' or category.strc_work_tycd = '01')
                                and project.pproj_stus_fl = 'A'
                                and project.lfcy_stg_cd >= '10'
                                and trim(project.pproj_fnty_cd) = '3'
                                and project.estcp_ty_cd = 'LET'
                                and project.estcp_prmy_cpnt_fl = 'Y'
                                and project.estcp_schd_dt >= :startDate
                                
                                and category.strc_work_tycd is not null

                        group by
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm
                            ";
                    break;


                case WisamType.PmicProjectTypes.AnyProgrammed:
                    qry = @"
                        select distinct 
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm,

                                sched.estcp_epsbl_schdt as earliestadvanceableletdate,
                                sched.estcp_lpsbl_schdt as latestadvanceableletdate,
                                project.pproj_lcstg_dt,
                                grp.grp_fos_proj_id as designprojectid,
                                pmpassdproj.pproj_antd_pse_dt as psedate,
                                project.pproj_epsbl_pse_dt as earliestpsedate,

                                sum(project.tot_with_dlvy_amt) as pawd, 
                                sum(project.tot_wo_dlvy_amt) as pa,
        
                                sum(category.tot_with_dlvy_amt) as cawd, 
                                sum(category.tot_wo_dlvy_amt) as ca
                          

                        from dot1pmic.dw_pmic_fiip_proj project, 
                                dot1pmic.dw_pmic_fiip_ctgy category,
                                dot1pmic.dw_pmic_fiip_cnty county,
                                dot1pmic.dw_pmic_fiip_loc location,
                                dot1pmic.dw_pmic_fiip_pgm program,
                                dot1pmic.dw_pmic_fiip_estcp sched,
                                dot1pmic.dw_pmic_fiip_grp grp,
                                dot1pmic.dw_pmp_assd_proj pmpassdproj

                        where project.pproj_id = category.pproj_id
                                and project.fos_proj_id = category.fos_proj_id
                                and project.pproj_id = county.pproj_id                                
                                and project.fos_proj_id = county.fos_proj_id
                                and project.pproj_id = location.pproj_id                                
                                and project.fos_proj_id = location.fos_proj_id
                                and project.pproj_id = program.pproj_id                                
                                and project.fos_proj_id = program.fos_proj_id
                                and project.fos_proj_id = grp.fos_proj_id(+)
                                and grp.pproj_grp_ty_cd = 'DES'
                                and project.fos_proj_id = pmpassdproj.fos_proj_id(+)

                                and project.pproj_id = sched.pproj_id                                
                                and project.fos_proj_id = sched.fos_proj_id
                                and project.estcp_prmy_cpnt_fl = sched.estcp_prmy_cpnt_fl
                                and project.estcp_schd_dt = sched.estcp_schd_dt

                                and county.pproj_cnty_prmy_fl = 'Y'
                                and location.ppjl_prte_fl = 'Y'
                                
                                and project.pproj_stus_fl = 'A'
                                
                                
                                and project.estcp_prmy_cpnt_fl = 'Y'
                                and project.estcp_schd_dt >= :startDate
                                and project.estcp_schd_dt <= :endDate
                                and trim(project.pproj_rcntl_tycd) not in ('P','R','X')
                                
                                and (category.strc_work_tycd is not null or (upper(fndg_ctgy_desc) like '%NOISE%N-%' or upper(fndg_ctgy_desc) like '%RETAINING%R-%' or upper(fndg_ctgy_desc) like '%SIGN%BRIDGE%S-%' or upper(fndg_ctgy_desc) like '%CULVERT%C-%'))
                        group by
                                county.dot_rgn_cd,
                                category.extg_strc_id,
                                county.wicy_basd_rgn_nm,
                                project.fos_proj_id, 
                                project.pproj_id, 
                                category.plnd_strc_id,
                                category.fndg_ctgy_nb,
                                category.fndg_ctgy_desc,
                                category.strc_work_tycd, 
                                category.strc_work_tydc,
                                
                                project.estcp_schd_dt, 
                                location.ppjl_rtnm_txt,
                                project.sub_pgm_cd,
                                project.sub_pgm_desc,
                                project.pproj_fos_titl_txt,
                                project.pproj_fos_lmt_txt,
                                project.pproj_fos_cncp_txt,
                                project.pproj_fnty_cd,
                                project.pproj_fnty_desc,
                                program.wdot_pgm_cd,
                                program.wdot_pgm_desc,
                                project.lfcy_stg_cd,
                                project.pproj_cncp_cd, 
                                project.pproj_cncp_desc,
                                project.pproj_stus_fl,
                                category.fed_impt_ty_cd, 
                                category.fed_impt_ty_desc,
                                project.estcp_ty_cd,
                                project.estcp_prmy_cpnt_fl,
                                project.pproj_stus_fl,
                                project.mgr_pproj_ptcp_nm,
                                sched.estcp_epsbl_schdt,
                                sched.estcp_lpsbl_schdt,
                                project.pproj_lcstg_dt,
                                grp.grp_fos_proj_id,
                                pmpassdproj.pproj_antd_pse_dt,
                                project.pproj_epsbl_pse_dt
                            order by
                                project.fos_proj_id,
                                category.extg_strc_id,
                                category.plnd_strc_id,
                                category.strc_work_tycd,
                                project.estcp_schd_dt,
                                program.wdot_pgm_desc
                            ";
                    break;

                    // pulled out of the above query
                    //
                    // and project.lfcy_stg_cd >= '10'
                    //

            }

            DataTable dt = null;
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("startDate", OracleDbType.Date);
            prms[0].Value = startDate;
            prms[1] = new OracleParameter("endDate", OracleDbType.Date);
            prms[1].Value = endDate;
            dt = ExecuteSelect(qry, prms, fiipsConn);

            /*
            if (pmicProjectType == WisamType.PmicProjectTypes.AnyProgrammed)
            {
                dt = ExecuteSelect(qry, fiipsConn);
            }
            else
            {
                OracleParameter[] prms = new OracleParameter[2];
                prms[0] = new OracleParameter("startDate", OracleDbType.Date);
                prms[0].Value = startDate;
                prms[1] = new OracleParameter("endDate", OracleDbType.Date);
                prms[1].Value = endDate;
                dt = ExecuteSelect(qry, prms, fiipsConn);
            }
            */

            if (dt != null && dt.Rows.Count > 0)
            {
                ProgrammedWorkAction previousWorkAction = null;

                foreach (DataRow dr in dt.Rows)
                {
                    ProgrammedWorkAction pWA = new ProgrammedWorkAction();
                    pWA.FosProjId = dr["FOS_PROJ_ID"].ToString().Trim();
                    pWA.PProjId = Convert.ToInt32(dr["PPROJ_ID"]);
                    pWA.DotRegionCode = dr["DOT_RGN_CD"].ToString().Trim();
                    pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                    pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();

                    if (pWA.StructureId.StartsWith("B") || pWA.NewStructureId.StartsWith("B")
                            || pWA.StructureId.StartsWith("P") || pWA.NewStructureId.StartsWith("P"))
                    {
                        pWA.IsBridge = true;
                    }
                    else
                    {
                        pWA.IsBridge = false;
                    }

                    pWA.County = dr["WICY_BASD_RGN_NM"].ToString().Trim();
                    pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString().Trim();
                    pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString().Trim();
                    pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString().Trim();
                    pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString().Trim();
                    pWA.DesignProjectId = dr["designprojectid"].ToString().Trim();

                    //dr["ESTCP_SCHD_DT"]
                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                    {
                        pWA.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);

                        int fiscalYear = pWA.EstimatedCompletionDate.Year;

                        if (pWA.EstimatedCompletionDate.Month > 6)
                        {
                            fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
                        }

                        pWA.StateFiscalYear = fiscalYear;
                    }

                    if (!String.IsNullOrEmpty(dr["earliestadvanceableletdate"].ToString()))
                    {
                        pWA.EarliestAdvanceableLetDate = Convert.ToDateTime(dr["earliestadvanceableletdate"]);
                    }

                    if (!String.IsNullOrEmpty(dr["latestadvanceableletdate"].ToString()))
                    {
                        pWA.LatestAdvanceableLetDate = Convert.ToDateTime(dr["latestadvanceableletdate"]);
                    }

                    if (!String.IsNullOrEmpty(dr["pproj_lcstg_dt"].ToString()))
                    {
                        pWA.LifeCycleStageDate = Convert.ToDateTime(dr["pproj_lcstg_dt"]);
                    }

                    if (!String.IsNullOrEmpty(dr["psedate"].ToString()))
                    {
                        pWA.PseDate = Convert.ToDateTime(dr["psedate"]);
                    }

                    if (!String.IsNullOrEmpty(dr["earliestpsedate"].ToString()))
                    {
                        pWA.EarliestPseDate = Convert.ToDateTime(dr["earliestpsedate"]);
                    }

                    //pWA.WorkActionCode = "";
                    //pWA.WorkActionDesc = "";
                    pWA.Route = dr["PPJL_RTNM_TXT"].ToString().Trim();
                    pWA.SubProgramCode = dr["SUB_PGM_CD"].ToString().Trim();
                    pWA.SubProgramDesc = dr["SUB_PGM_DESC"].ToString().Trim();
                    pWA.Title = dr["PPROJ_FOS_TITL_TXT"].ToString().Trim();
                    pWA.Limit = dr["PPROJ_FOS_LMT_TXT"].ToString().Trim();
                    pWA.Concept = dr["PPROJ_FOS_CNCP_TXT"].ToString().Trim();
                    pWA.FunctionalTypeCode = dr["PPROJ_FNTY_CD"].ToString().Trim();
                    pWA.FunctionalTypeDesc = dr["PPROJ_FNTY_DESC"].ToString().Trim();
                    pWA.WisDOTProgramCode = dr["WDOT_PGM_CD"].ToString().Trim();
                    pWA.WisDOTProgramDesc = dr["WDOT_PGM_DESC"].ToString().Trim();
                    pWA.LifeCycleStageCode = dr["LFCY_STG_CD"].ToString().Trim();
                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                    pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim();
                    pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString().Trim();
                    pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();
                    pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                    pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();
                    pWA.ProjectManager = dr["mgr_pproj_ptcp_nm"].ToString();

                    try
                    {
                        pWA.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                    }
                    catch { }

                    /*
                    try
                    {
                        pWA.WorkWithDeliveryCost = Convert.ToSingle(dr["COSTWITHDELIVERY"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkWithoutDeliveryCost = Convert.ToSingle(dr["COSTWITHOUTDELIVERY"]);
                    }
                    catch { }
                    */

                    bool isDuplicate = false;

                    if (previousWorkAction != null)
                    {
                        // Determine if work is a duplicate of previous work- just has a different funding source
                        if (previousWorkAction.FosProjId.Equals(pWA.FosProjId)
                            && previousWorkAction.StructureId.Equals(pWA.StructureId)
                            && previousWorkAction.NewStructureId.Equals(pWA.NewStructureId)
                            && previousWorkAction.OriginalWorkActionCode.Equals(pWA.OriginalWorkActionCode)
                            && DateTime.Compare(previousWorkAction.EstimatedCompletionDate, pWA.EstimatedCompletionDate) == 0
                            && previousWorkAction.FundingCategoryDesc.Equals(pWA.FundingCategoryDesc)
                            && !previousWorkAction.WisDOTProgramCode.Equals(pWA.WisDOTProgramCode))
                        {
                            isDuplicate = true;
                        }
                    }

                    pWA.IsDuplicate = isDuplicate;
                    pwActions.Add(pWA);

                    previousWorkAction = new ProgrammedWorkAction();
                    previousWorkAction.FosProjId = pWA.FosProjId;
                    previousWorkAction.StructureId = pWA.StructureId;
                    previousWorkAction.NewStructureId = pWA.NewStructureId;
                    previousWorkAction.EstimatedCompletionDate = pWA.EstimatedCompletionDate;
                    previousWorkAction.OriginalWorkActionCode = pWA.OriginalWorkActionCode;
                    previousWorkAction.WisDOTProgramCode = pWA.WisDOTProgramCode;
                    previousWorkAction.FundingCategoryDesc = pWA.FundingCategoryDesc;



                    /*
                    pWA.FosProjId = dr["FOS_PROJ_ID"].ToString();
                    pWA.PProjId = Convert.ToInt32(dr["PPROJ_ID"]);
                    pWA.PlanningProjectConceptCode = dr["PPROJ_CNCP_CD"].ToString().Trim().ToUpper();
                    pWA.PlanningProjectConceptDesc = dr["PPROJ_CNCP_DESC"].ToString();
                    pWA.FederalImprovementTypeCode = dr["FED_IMPT_TY_CD"].ToString().Trim().ToUpper();
                    pWA.FederalImprovementTypeDesc = dr["FED_IMPT_TY_DESC"].ToString();
                    pWA.StructureId = dr["EXTG_STRC_ID"].ToString().Trim();
                    pWA.NewStructureId = dr["PLND_STRC_ID"].ToString().Trim();
                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString().Trim();
                    pWA.ComponentTypeCode = dr["ESTCP_TY_CD"].ToString().Trim();

                    if (!String.IsNullOrEmpty(dr["ESTCP_SCHD_DT"].ToString()))
                        pWA.EstimatedCompletionDate = Convert.ToDateTime(dr["ESTCP_SCHD_DT"]);

                    pWA.ProjectStatusFlag = dr["PPROJ_STUS_FL"].ToString();

                    try
                    {
                        pWA.ProjectTotalWithDeliveryAmount = Convert.ToSingle(dr["PAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.ProjectTotalWithoutDeliveryAmount = Convert.ToSingle(dr["PA"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithDeliveryAmount = Convert.ToSingle(dr["CAWD"]);
                    }
                    catch { }

                    try
                    {
                        pWA.WorkTotalWithoutDeliveryAmount = Convert.ToSingle(dr["CA"]);
                    }
                    catch { }
                    
                    pWA.OriginalWorkActionCode = dr["STRC_WORK_TYCD"].ToString();
                    pWA.OriginalWorkActionDesc = dr["STRC_WORK_TYDC"].ToString();

                    pWA.FundingCategoryNumber = dr["FNDG_CTGY_NB"].ToString();
                    pWA.FundingCategoryDesc = dr["FNDG_CTGY_DESC"].ToString();
                    */


                }
            }

            int countDup = pwActions.Where(e => e.IsDuplicate).Count();
            return pwActions;
        }

        public void UpdateIsBridge()
        {
            string qry = @"
                                update Pmic
                                set IsBridge = 1
                                where Extg_Strc_Id like 'B%'
                                    or Extg_Strc_Id like 'P%'
                                    or Plnd_Strc_Id like 'B%'
                                    or Plnd_Strc_Id like 'P%'
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);

            qry = @"
                                update StructureProgramDailyInventory
                                set IsBridge = 1
                                where Extg_Strc_Id like 'B%'
                                    or Extg_Strc_Id like 'P%'
                                    or Plnd_Strc_Id like 'B%'
                                    or Plnd_Strc_Id like 'P%'
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);

            qry = @"
                                update StructureProgramReview
                                set IsBridge = 1
                                where StructureId like 'B%'
                                    or StructureId like 'P%'
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);

            qry = @"
                                update Pmic
                                set IsBridge = 0
                                where IsBridge is null
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);

            qry = @"
                                update StructureProgramDailyInventory
                                set IsBridge = 0
                                where IsBridge is null
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);

            qry = @"
                                update StructureProgramReview
                                set IsBridge = 0
                                where IsBridge is null
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);
        }

        public void UpdateWorkDuplicates()
        {
            string qry = @"
                            select *
                            from Pmic
                            where IsDuplicate = 1
                            order by Fos_Proj_Id, Extg_Strc_Id, Plnd_Strc_Id, Strc_Work_Tycd, Estcp_Schd_Dt
                          ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    qry = @"
                              update StructureProgramDailyInventory
                              set IsDuplicate = 1
                              where Fos_Proj_Id = @fosProjId
                                and Extg_Strc_Id = @extgStrcId
                                and Plnd_Strc_Id = @plndStrcId
                                and Strc_Work_Tycd = @strcWorkTycd
                                and Estcp_Schd_Dt = @estcpSchdDt
                                and Wdot_Pgm_Cd = @wdotPgmCd
                                and Fndg_Ctgy_Desc = @fndgCtgyDesc
                                and RowInsertDateStamp < 20180925
                           ";
                    SqlParameter[] prms = new SqlParameter[7];
                    prms[0] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
                    prms[0].Value = row["Fos_Proj_Id"].ToString().Trim();
                    prms[1] = new SqlParameter("@extgStrcId", SqlDbType.VarChar);
                    prms[1].Value = row["Extg_Strc_Id"].ToString().Trim();
                    prms[2] = new SqlParameter("@plndStrcId", SqlDbType.VarChar);
                    prms[2].Value = row["Plnd_Strc_Id"].ToString().Trim();
                    prms[3] = new SqlParameter("@strcWorkTycd", SqlDbType.VarChar);
                    prms[3].Value = row["Strc_Work_Tycd"].ToString().Trim();
                    prms[4] = new SqlParameter("@estcpSchdDt", SqlDbType.Date);
                    prms[4].Value = Convert.ToDateTime(row["Estcp_Schd_Dt"]);
                    prms[5] = new SqlParameter("@wdotPgmCd", SqlDbType.VarChar);
                    prms[5].Value = row["Wdot_Pgm_Cd"].ToString().Trim();
                    prms[6] = new SqlParameter("@fndgCtgyDesc", SqlDbType.VarChar);
                    prms[6].Value = row["Fndg_Ctgy_Desc"].ToString().Trim();
                    //prms[7] = new SqlParameter("@rowInsertDateStamp", SqlDbType.Int);
                    //prms[7].Value = Convert.ToInt32(row["RowInsertDateStamp"]);

                    ExecuteInsertUpdateDelete(qry, prms, samConn);
                }
            }
        }

        public List<ProgrammedWorkAction> GetProgrammedWorkActions(string strId, DateTime startDate, DateTime endDate)
        {
            List<ProgrammedWorkAction> pwActions = new List<ProgrammedWorkAction>();
            string qry = @"
                                SELECT DISTINCT
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.FOS_PROJ_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PRM_ORG_BUR,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.SUB_PGM_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FOS_CNCP_TXT,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.FNDG_CTGY_DESC,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYCD,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYDC,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.PLND_STRC_ID,

                                    Sum(DOT1PMIC.DW_PMIC_FIIP_CTGY.TOT_WITH_DLVY_AMT) AS SumOfTOT_WITH_DLVY_AMT,

                                    Sum(DOT1PMIC.DW_PMIC_FIIP_CTGY.TOT_WO_DLVY_AMT) AS SumOfTOT_WO_DLVY_AMT

                                FROM DOT1PMIC.DW_PMIC_FIIP_PROJ,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY
                                
                                WHERE DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID = DOT1PMIC.DW_PMIC_FIIP_CTGY.PPROJ_ID
                                    AND DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID = :strId

                                GROUP BY
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.FOS_PROJ_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PRM_ORG_BUR,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.SUB_PGM_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FNTY_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_CNCP_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_FOS_CNCP_TXT,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD,
                                    DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.FNDG_CTGY_DESC,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYCD,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.STRC_WORK_TYDC, 
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.EXTG_STRC_ID,
                                    DOT1PMIC.DW_PMIC_FIIP_CTGY.PLND_STRC_ID

                                HAVING 
                                    (((DOT1PMIC.DW_PMIC_FIIP_PROJ.SUB_PGM_CD) In ('301','302','303','304')) AND
                                    ((DOT1PMIC.DW_PMIC_FIIP_PROJ.LFCY_STG_CD)>='10') AND
                                    ((DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_PRMY_CPNT_FL)='Y') AND

                                    ((DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT) >= :startDate And
                                    (DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT) <= :endDate) AND

                                    ((DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_TY_CD)='LET') AND
                                    ((DOT1PMIC.DW_PMIC_FIIP_PROJ.PPROJ_STUS_FL)='A'))
                            ";
            OracleParameter[] prms = new OracleParameter[3];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            prms[1] = new OracleParameter("startDate", OracleDbType.Date);
            prms[1].Value = startDate;
            prms[2] = new OracleParameter("endDate", OracleDbType.Date);
            prms[2].Value = endDate;
            DataTable dt = ExecuteSelect(qry, prms, fiipsConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    ProgrammedWorkAction pwAction = new ProgrammedWorkAction();
                    pwActions.Add(pwAction);
                }
            }

            return pwActions;

            //((DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT)>=to_date('1-jul-2014') And
            //(DOT1PMIC.DW_PMIC_FIIP_PROJ.ESTCP_SCHD_DT)<=to_date('30-jun-2021')) AND
        }
        #endregion FIIPS Methods

        #region AASHTOWare Project Methods
        public string GetVendorName(int vendorId)
        {
            string vendorName = "";
            string qry = @"
                                select refvendor_id,
                                        refvendor_nm,
                                        vendorname
                                from dot1prwo.refvendor
                                where refvendor_id = :vendorId
                            ";

            DataTable dt = null;
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("vendorId", OracleDbType.Int32);
            prms[0].Value = vendorId;
            dt = ExecuteSelect(qry, prms, aashtoWareProjectConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                vendorName = dt.Rows[0]["vendorname"].ToString().Trim();
            }

            return vendorName;
        }

        public List<BidItem> GetStructureBidItems(DateTime startLetDate, DateTime endLetDate)
        {
            string qry = @"
                                select 
                                        projitem.refitem_id as BidItemDbId,
                                        refitem.refitem_nm as BidItemName, 
                                        refitem.descr as BidItemDescription,
                                        projitem.qty as BidItemQuantity,
                                        projitem.unitprice as BidItemUnitPrice,
                                        projitem.extendedamount as BidItemCost,
                                        projitem.suppdescr as BidItemSupplementalDescription,
                                        proj.project_id as ProjectDbId, 
                                        proj.descr as ProjectDescription, 
                                        proj.projectworktype as ProjectWorkType, 
                                        proj.project_nm as ProjectName,
                                        proj.pjdt2 as LetDate,
                                        proj.pjdt1 as AdvanceableLetDate,
                                         
                                        propos.proposal_id as ProposalDbId,
                                        propos.awardedvendor_id as AwardedVendorDbId,
                                        propos.awardedamount as AwardedAmount
                                        
                                from 
                                        dot1prwo.project proj,
                                        dot1prwo.projectitem projitem,
                                        dot1prwo.refitem refitem,
                                        dot1prwo.proposal propos
                                        
                                where
                                        proj.project_id = projitem.project_id
                                        and projitem.specbook = '03'
                                        and proj.proposal_id = propos.proposal_id(+)

                                        and projitem.refitem_id = refitem.refitem_id
                                        
                                        and proj.pjdt2 >= :startLetDate
                                        and proj.pjdt2 <= :endLetDate
                                        and (TRIM(UPPER(refitem.refitem_nm)) in (
                                          '502.3100',
                                            '503.0128',
                                            '503.0136',
                                            '503.0137',
                                            '503.0145',
                                            '503.0146',
                                            '503.0154',
                                            '503.0155',
                                            '503.0170',
                                            '503.0172',
                                            '503.0182',
                                            '504.0100',
                                            '504.0200',
                                            '504.0500',
                                            '504.0600',
                                            '506.0605',
                                            '506.0105',
                                            '506.1105',
                                            '506.2605',
                                            '506.2610',
                                            '506.4000',
                                            '506.5000',
                                            '506.6000',
                                            '512.0600',
                                            '513.2001',
                                            '513.4051',
                                            '513.4056',
                                            '513.4061',
                                            '513.4066',
                                            '513.4091',
                                            '513.7006',
                                            '513.7011',
                                            '513.7016',
                                            '513.7021',
                                            '513.7026',
                                            '513.7031',
                                            '513.7051',
                                            '513.7083',
                                            '513.7084',
                                            '513.8006',
                                            '513.8011',
                                            '513.8016',
                                            '513.8021',
                                            '513.8026',
                                            '513.8031',
                                            '517.0600',
                                            '517.1000',
                                            '641.0100',
                                            '641.0600',
                                            '641.1200',
                                            '641.5100',
                                            '641.6600',
                                            '641.8100',
                                            '660.0100',
                                            '660.0200',
                                            '504.2000.S',
                                            '502.3110.S',
                                            '503.0400.S',
                                            '506.7070.S',
                                            '506.8005.S',
                                            '513.2050.S',
                                            '513.9005.S',
                                            '531.0200.S',
                                            '531.0300.S',
                                            '531.0400.S',
                                            '517.0650.S'
                                          ) 
                                          or (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0090%' 
                                                and (TRIM(UPPER(projitem.suppdescr)) like '%PRESTRESSED%GIRDER%' or TRIM(UPPER(projitem.suppdescr)) like '%PRECAST%PIER%' 
                                                        or TRIM(UPPER(projitem.suppdescr)) like '%COMBINATION%RAILING%' or TRIM(UPPER(projitem.suppdescr)) like '%THREE%PRECAST%'
                                                    ) 
                                             )
                                          or (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0060%' 
                                                and (TRIM(UPPER(projitem.suppdescr)) like '%BEARING REPAIR' or TRIM(UPPER(projitem.suppdescr)) like '%BEARINGS%HIGH%'
                                                        or TRIM(UPPER(projitem.suppdescr)) like '%FABRICATED%EXPANSION%'
                                                    ) 
                                             )
                                          or (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0105%' 
                                                and (TRIM(UPPER(projitem.suppdescr)) like '%METALIZING%' or TRIM(UPPER(projitem.suppdescr)) like '%METALLIZING%') 
                                             )
                                          or (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0165%' 
                                                and (TRIM(UPPER(projitem.suppdescr)) like '%PRESTRESS%PRECAST%CONCRETE%WALL%' or TRIM(UPPER(projitem.suppdescr)) like '%WALL%MODULAR%BLOCK%'
                                                        or TRIM(UPPER(projitem.suppdescr)) like '%WALL%WIRE%FACED%' or TRIM(UPPER(projitem.suppdescr)) like '%WALL%CONCRETE%PANEL%'
                                                        or TRIM(UPPER(projitem.suppdescr)) like '%WALL%GABION%' or TRIM(UPPER(projitem.suppdescr)) like '%WALL%MODULAR%BIN%'
                                                        or TRIM(UPPER(projitem.suppdescr)) like '%TEMPORARY%WALL%WIRE%' or TRIM(UPPER(projitem.suppdescr)) like '%WALL%CIP%FACING%'
                                                        or TRIM(UPPER(projitem.suppdescr)) like '%GEOSYNTHETIC%REINFORCED%SOIL%'
                                                    ) 
                                             )
                                            or ( 
                                  (TRIM(UPPER(projitem.suppdescr)) like '%STRUCTURAL%STEEL%BASCULE%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%STRUCTURAL%STEEL%CARBON%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%STRUCTURAL%STEEL%BRIDGE%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%STEEL%TREADS%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%STEEL%CASTINGS%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%REAR%LOCKS%BASCULE%'
                                    )
                                    and (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0085%')
                                )
                                
                                or 
                                (
                                  (TRIM(UPPER(projitem.suppdescr)) like '%STEEL%GRID%FLOOR%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%STEEL%GRATING%'
                                    )
                                    and (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0165%')
                                )
                                
                                or 
                                (
                                  (TRIM(UPPER(projitem.suppdescr)) like '%STEEL%STAIRS%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%MECHANICAL%WORK%BASCULE%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%ELECTRICAL%WORK%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%RAILING%STEEL%SPECIAL%'
                                        or TRIM(UPPER(projitem.suppdescr)) like '%FENDER%RUB%RAILS%'
                                    )
                                    and (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0105%')
                                )
                                
                                 or 
                                (
                                  (TRIM(UPPER(projitem.suppdescr)) like '%COUNTERWEIGHT%CONCRETE%'
                                        
                                    )
                                    and (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0035%')
                                )
                                
                                or 
                                (
                                  (TRIM(UPPER(projitem.suppdescr)) like '%BASCULE%GIRDER%'
                                        
                                    )
                                    and (TRIM(UPPER(refitem.refitem_nm)) like '%SPV.0060%')
                                )
                                        )
                                
                                order by 
                                    LetDate,
                                    ProjectDbId,
                                    ProjectName,
                                    BidItemName
                            ";

            DataTable dt = null;
            OracleParameter[] prms = new OracleParameter[2];
            prms[0] = new OracleParameter("startLetDate", OracleDbType.Date);
            prms[0].Value = startLetDate;
            prms[1] = new OracleParameter("endLetDate", OracleDbType.Date);
            prms[1].Value = endLetDate;
            dt = ExecuteSelect(qry, prms, aashtoWareProjectConn);

            List<BidItem> bidItems = new List<BidItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    BidItem bidItem = new BidItem();
                    bidItem.BidItemDbId = Convert.ToInt32(dr["BidItemDbId"]);
                    bidItem.BidItemName = dr["BidItemName"].ToString().Trim();
                    bidItem.BidItemDescription = dr["BidItemDescription"].ToString().Trim();

                    if (!String.IsNullOrEmpty(dr["BidItemSupplementalDescription"].ToString().Trim()))
                    {
                        bidItem.BidItemDescription += String.Format("; {0}", dr["BidItemSupplementalDescription"].ToString().Trim());
                    }

                    try
                    {
                        bidItem.BidItemQuantity = Convert.ToSingle(dr["BidItemQuantity"]);
                    }
                    catch { }

                    try
                    {
                        bidItem.BidItemUnitPrice = Convert.ToSingle(dr["BidItemUnitPrice"]);
                    }
                    catch { }

                    try
                    {
                        bidItem.BidItemCost = Convert.ToSingle(dr["BidItemCost"]);
                    }
                    catch { }

                    bidItem.ProjectDbId = Convert.ToInt32(dr["ProjectDbId"]);
                    bidItem.ProjectDescription = dr["ProjectDescription"].ToString().Trim();
                    bidItem.ProjectWorkType = dr["ProjectWorkType"].ToString().Trim();
                    bidItem.ProjectName = dr["ProjectName"].ToString().Trim();
                    bidItem.FosProjectId = bidItem.ProjectName.Replace("-", "");

                    if (!String.IsNullOrEmpty(dr["LetDate"].ToString()))
                    {
                        bidItem.LetDate = Convert.ToDateTime(dr["LetDate"]);
                    }

                    if (!String.IsNullOrEmpty(dr["AdvanceableLetDate"].ToString()))
                    {
                        bidItem.AdvanceableLetDate = Convert.ToDateTime(dr["AdvanceableLetDate"]);
                    }

                    if (!String.IsNullOrEmpty(dr["ProposalDbId"].ToString()))
                    {
                        bidItem.ProposalDbId = Convert.ToInt32(dr["ProposalDbId"]);
                    }

                    if (!String.IsNullOrEmpty(dr["AwardedVendorDbId"].ToString()))
                    {
                        bidItem.AwardedVendorDbId = Convert.ToInt32(dr["AwardedVendorDbId"]);
                        bidItem.AwardedVendorName = GetVendorName(bidItem.AwardedVendorDbId);
                    }

                    if (!String.IsNullOrEmpty(dr["AwardedAmount"].ToString()))
                    {
                        bidItem.AwardedAmount = Convert.ToSingle(dr["AwardedAmount"]);
                    }

                    // Grab FIIPS project info
                    if (!String.IsNullOrEmpty(bidItem.FosProjectId) && !String.IsNullOrEmpty(bidItem.LetDate.ToString()))
                    {
                        List<ProgrammedWorkAction> programmedWorkActions = GetProgrammedWorkActionsStructure(bidItem.FosProjectId, bidItem.LetDate);

                        if (programmedWorkActions.Count > 0)
                        {
                            bidItem.ProjectStatus = String.Format("{0}-{1}", programmedWorkActions.First().LifeCycleStageCode, programmedWorkActions.First().LifeCycleStageDesc);
                        }

                        foreach (var programmedWorkAction in programmedWorkActions)
                        {
                            if (!String.IsNullOrEmpty(programmedWorkAction.OriginalWorkActionCode))
                            {
                                bidItem.ExistingStructureIds += String.Format("{0} ({1}-{2})", programmedWorkAction.StructureId, programmedWorkAction.OriginalWorkActionCode, programmedWorkAction.OriginalWorkActionDesc);

                                if (programmedWorkAction.OriginalWorkActionCode.Equals("91") || programmedWorkAction.OriginalWorkActionCode.Equals("01") || programmedWorkAction.OriginalWorkActionCode.Equals("NB"))
                                {
                                    bidItem.ExistingStructureIds += String.Format("; new {0}", programmedWorkAction.NewStructureId);
                                }

                                bidItem.ExistingStructureIds += String.Format("\r\n");
                            }
                            else
                            {
                                bidItem.ExistingStructureIds += String.Format("{0}\r\n", programmedWorkAction.FundingCategoryDesc);
                            }

                            bidItem.ProjectManager = programmedWorkAction.ProjectManager;
                        }
                    }

                    bidItems.Add(bidItem);
                }
            }

            return bidItems;
        }
        #endregion AASHTOWare Project Methods

        #region Structures Progress Report Methods
        public void AddTimesheets(List<EmployeeTimesheet> empTimesheets)
        {
            foreach (var empTimesheet in empTimesheets)
            {
                InsertTimesheet(empTimesheet);
            }
        }

        public bool DoesTimesheetExist(EmployeeTimesheet empTimesheet)
        {
            bool exists = false;
            string qry =
                @"
                    select *
                    from employee_weekly_hours_activity_project
                    where [structure number] = @structureId
                        and project_id = @projectId
                        and activity_code = @activityCode
                        and emp_ssn = @employeeId
                        and week_ending_date = @weekEndingDate
                        and work_number = @workNumber
                ";

            OleDbParameter[] prms = new OleDbParameter[6];
            prms[0] = new OleDbParameter("@structureId", OleDbType.VarChar);
            prms[0].Value = empTimesheet.StructureId;
            prms[1] = new OleDbParameter("@projectId", OleDbType.VarChar);
            prms[1].Value = empTimesheet.ProjectId;
            prms[2] = new OleDbParameter("@activityCode", OleDbType.Integer);
            prms[2].Value = empTimesheet.ActivityCode;
            prms[3] = new OleDbParameter("@employeeId", OleDbType.Integer);
            prms[3].Value = empTimesheet.EmployeeId;
            prms[4] = new OleDbParameter("@weekEndingDate", OleDbType.Date);
            prms[4].Value = empTimesheet.WeekEndingDate;
            prms[5] = new OleDbParameter("@workNumber", OleDbType.Integer);
            prms[5].Value = empTimesheet.WorkNumber;
            DataTable dt = ExecuteSelect(qry, prms, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                exists = true;
            }

            return exists;
        }
        public bool InsertTimesheet(EmployeeTimesheet empTimesheet)
        {
            bool successful = true;
            string qry =
                @"
                    insert into employee_weekly_hours_activity_project ([structure number], project_id, activity_code, emp_ssn, week_ending_date, work_number, total_hours,
                                month_of_week_ending_date, year_of_week_ending_date)
                    values (@structureId, @projectId, @activityCode, @employeeId, @weekEndingDate, @workNumber, @totalHours,
                            @monthWeekEndingDate, @yearWeekEndingDate)
                ";

            try
            {
                OleDbParameter[] prms = new OleDbParameter[9];
                prms[0] = new OleDbParameter("@structureId", OleDbType.VarChar);
                prms[0].Value = empTimesheet.StructureId;
                prms[1] = new OleDbParameter("@projectId", OleDbType.VarChar);
                prms[1].Value = empTimesheet.ProjectId;
                prms[2] = new OleDbParameter("@activityCode", OleDbType.Integer);
                prms[2].Value = empTimesheet.ActivityCode;
                prms[3] = new OleDbParameter("@employeeId", OleDbType.Integer);
                prms[3].Value = empTimesheet.EmployeeId;
                prms[4] = new OleDbParameter("@weekEndingDate", OleDbType.Date);
                prms[4].Value = empTimesheet.WeekEndingDate;
                prms[5] = new OleDbParameter("@workNumber", OleDbType.Integer);
                prms[5].Value = empTimesheet.WorkNumber;
                prms[6] = new OleDbParameter("@totalHours", OleDbType.Double);
                prms[6].Value = Convert.ToDouble(empTimesheet.TotalHours);
                prms[7] = new OleDbParameter("@monthWeekEndingDate", OleDbType.Integer);
                prms[7].Value = empTimesheet.MonthWeekEndingDate;
                prms[8] = new OleDbParameter("@yearWeekEndingDate", OleDbType.Integer);
                prms[8].Value = empTimesheet.YearWeekEndingDate;
                ExecuteInsertUpdateDelete(qry, prms, strProgRptConn);
            }
            catch (Exception ex)
            {
                successful = false;
            }

            return successful;
        }

        public bool DeleteTimesheet(EmployeeTimesheet empTimesheet)
        {
            bool successful = true;
            string qry =
                @"
                    delete from employee_weekly_hours_activity_project
                    where [structure number] = @structureId
                        and project_id = @projectId
                        and activity_code = @activityCode
                        and emp_ssn = @employeeId
                        and week_ending_date = @weekEndingDate
                        and work_number = @workNumber
                ";

            try
            {
                OleDbParameter[] prms = new OleDbParameter[6];
                prms[0] = new OleDbParameter("@structureId", OleDbType.VarChar);
                prms[0].Value = empTimesheet.StructureId;
                prms[1] = new OleDbParameter("@projectId", OleDbType.VarChar);
                prms[1].Value = empTimesheet.ProjectId;
                prms[2] = new OleDbParameter("@activityCode", OleDbType.Integer);
                prms[2].Value = empTimesheet.ActivityCode;
                prms[3] = new OleDbParameter("@employeeId", OleDbType.Integer);
                prms[3].Value = empTimesheet.EmployeeId;
                prms[4] = new OleDbParameter("@weekEndingDate", OleDbType.Date);
                prms[4].Value = empTimesheet.WeekEndingDate;
                prms[5] = new OleDbParameter("@workNumber", OleDbType.Integer);
                prms[5].Value = empTimesheet.WorkNumber;
                ExecuteInsertUpdateDelete(qry, prms, strProgRptConn);
            }
            catch (Exception ex)
            {
                successful = false;
            }

            return successful;
        }

        public bool UpdateTimesheet(EmployeeTimesheet empTimesheet)
        {
            bool successful = true;
            string qry =
                @"
                    update employee_weekly_hours_activity_project
                    set total_hours = @totalHours,
                        month_of_week_ending_date = @monthWeekEndingDate,
                        year_of_week_ending_date = @yearWeekendingDate
                    where [structure number] = @structureId
                        and project_id = @projectId
                        and activity_code = @activityCode
                        and emp_ssn = @employeeId
                        and week_ending_date = @weekEndingDate
                        and work_number = @workNumber
                ";

            try
            {
                OleDbParameter[] prms = new OleDbParameter[9];
                prms[0] = new OleDbParameter("@structureId", OleDbType.VarChar);
                prms[0].Value = empTimesheet.StructureId;
                prms[1] = new OleDbParameter("@projectId", OleDbType.VarChar);
                prms[1].Value = empTimesheet.ProjectId;
                prms[2] = new OleDbParameter("@activityCode", OleDbType.Integer);
                prms[2].Value = empTimesheet.ActivityCode;
                prms[3] = new OleDbParameter("@employeeId", OleDbType.Integer);
                prms[3].Value = empTimesheet.EmployeeId;
                prms[4] = new OleDbParameter("@weekEndingDate", OleDbType.Date);
                prms[4].Value = empTimesheet.WeekEndingDate;
                prms[5] = new OleDbParameter("@workNumber", OleDbType.Integer);
                prms[5].Value = empTimesheet.WorkNumber;
                prms[6] = new OleDbParameter("@totalHours", OleDbType.Double);
                prms[6].Value = Convert.ToDouble(empTimesheet.TotalHours);
                prms[7] = new OleDbParameter("@monthWeekEndingDate", OleDbType.Integer);
                prms[7].Value = empTimesheet.MonthWeekEndingDate;
                prms[8] = new OleDbParameter("@yearWeekEndingDate", OleDbType.Integer);
                prms[8].Value = empTimesheet.YearWeekEndingDate;
                ExecuteInsertUpdateDelete(qry, prms, strProgRptConn);
            }
            catch (Exception ex)
            {
                successful = false;
            }

            return successful;
        }

        public int GetWorkNumber(string structureId, string projectId)
        {
            int workNumber = -1;
            string qry =
                @"
                    select [work number] as worknumber
                    from project
                    where [structure number] = @structureId
                        and [project id] = @projectId
                ";
            OleDbParameter[] prms = new OleDbParameter[2];
            prms[0] = new OleDbParameter("@structureId", OleDbType.VarChar);
            prms[0].Value = structureId;
            prms[1] = new OleDbParameter("@projectId", OleDbType.VarChar);
            prms[1].Value = projectId;
            DataTable dt = ExecuteSelect(qry, prms, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                workNumber = Convert.ToInt32(dr["worknumber"]);
            }

            return workNumber;
        }

        public int GetEmployeeId(string firstName, string lastName)
        {
            int employeeId = -1;
            string qry =
                @"
                    select *
                    from employee
                    where ucase(first_name) = @firstName
                        and ucase(last_name) = @lastName
                ";
            OleDbParameter[] prms = new OleDbParameter[2];
            prms[0] = new OleDbParameter("@firstName", OleDbType.VarChar);
            prms[0].Value = firstName.ToUpper();
            prms[1] = new OleDbParameter("@lastName", OleDbType.VarChar);
            prms[1].Value = firstName.ToUpper();
            DataTable dt = ExecuteSelect(qry, prms, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                employeeId = Convert.ToInt32(dr["emp_ssn"].ToString());
            }

            return employeeId;
        }

        public List<Employee> GetEmployeesAndTimesheets(int startMonth, int endMonth, int startYear, int endYear)
        {
            List<Employee> emps = new List<Employee>();
            string qry =
                @"
                    select emp_ssn
                    from employee
                    order by last_name, first_name
                ";
            DataTable dt = ExecuteSelect(qry, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string empId = dr["emp_ssn"].ToString().Trim();
                    Employee emp = GetEmployee(empId);
                    emp.Timesheets = GetEmployeeTimesheets(empId, startMonth, endMonth, startYear, endYear);
                    emps.Add(emp);
                }
            }

            return emps;
        }

        public List<Employee> GetEmployees()
        {
            List<Employee> emps = new List<Employee>();
            string qry =
                @"
                    select emp_ssn
                    from employee
                    order by last_name, first_name
                ";
            DataTable dt = ExecuteSelect(qry, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string empId = dr["emp_ssn"].ToString().Trim();
                    Employee emp = GetEmployee(empId);
                    emps.Add(emp);
                }
            }

            return emps;
        }

        public Employee GetEmployee(string employeeId)
        {
            Employee emp = new Employee();
            string qry =
                @"
                    select emp.emp_ssn, emp.first_name, emp.last_name, emp.hourly_rate
                    from employee emp
                    where emp.emp_ssn = @employeeId
                ";
            OleDbParameter[] prms = new OleDbParameter[1];
            prms[0] = new OleDbParameter("@employeeId", OleDbType.VarChar);
            prms[0].Value = employeeId;
            DataTable dt = ExecuteSelect(qry, prms, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                emp.EmployeeId = dr["emp_ssn"].ToString().Trim();
                emp.FirstName = dr["first_name"].ToString().Trim().Replace(" ", "");
                emp.LastName = dr["last_name"].ToString().Trim().Replace(" ", "");
                emp.HourlyRate = Convert.ToSingle(dr["hourly_rate"]);
            }

            return emp;
        }

        public List<WorkActivity> GetWorkActivities()
        {
            List<WorkActivity> workActivities = new List<WorkActivity>();
            string qry =
                @"
                    select act.activity_code, act.description as actdescription, cat.catid, cat.description as catdescription
                    from activity act, activitycat cat
                    where act.catid = cat.catid
                    order by act.activity_code
                ";
            DataTable dt = ExecuteSelect(qry, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkActivity wa = new WorkActivity();
                    wa.WorkActivityCategory = dr["catdescription"].ToString().Trim();
                    wa.WorkActivityCode = Convert.ToInt32(dr["activity_code"]);
                    wa.WorkActivityDescription = dr["actdescription"].ToString().Trim();
                    workActivities.Add(wa);
                }
            }

            return workActivities;
        }

        public List<WorkActivity> GetWorkActivities(int startMonth, int endMonth, int startYear, int endYear)
        {
            List<WorkActivity> workActivities = new List<WorkActivity>();
            string qry =
                @"
                    select 
                        distinct emphours.activity_code, act.description as actdescription, cat.description as catdescription
                    from 
                        (
                            employee_weekly_hours_activity_project emphours
                            left join activity act
                            on emphours.activity_code = act.activity_code
                        )
                            left join activitycat cat
                            on act.catid = cat.catid
                    where
                        month_of_week_ending_date >= @startMonth
                        and month_of_week_ending_date <= @endMonth
                        and year_of_week_ending_date >= @startYear
                        and year_of_week_ending_date <= @endYear
                    order by
                        cat.description, emphours.activity_code, act.description
                ";
            OleDbParameter[] prms = new OleDbParameter[4];
            prms[0] = new OleDbParameter("@startMonth", OleDbType.Integer);
            prms[0].Value = startMonth;
            prms[1] = new OleDbParameter("@endMonth", OleDbType.Integer);
            prms[1].Value = endMonth;
            prms[2] = new OleDbParameter("@startYear", OleDbType.Integer);
            prms[2].Value = startYear;
            prms[3] = new OleDbParameter("@endYear", OleDbType.Integer);
            prms[3].Value = endYear;
            DataTable dt = ExecuteSelect(qry, prms, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkActivity wa = new WorkActivity();
                    wa.WorkActivityCategory = dr["catdescription"].ToString().Trim();
                    wa.WorkActivityCode = Convert.ToInt32(dr["activity_code"]);
                    wa.WorkActivityDescription = dr["actdescription"].ToString().Trim();
                    workActivities.Add(wa);
                }
            }

            return workActivities;
        }

        public List<EmployeeTimesheet> GetEmployeeTimesheets(string employeeId, int startMonth, int endMonth, int startYear, int endYear)
        {
            List<EmployeeTimesheet> empTimesheets = new List<EmployeeTimesheet>();
            string qry =
                @"
                    select emp.emp_ssn, emp.first_name, emp.last_name, emp.hourly_rate,
                        project_id, work_number, total_hours, week_ending_date,
                        [structure number] as structureid, emphours.activity_code, 
                        act.description as actdescription, cat.catid, 
                        cat.description as catdescription,
                        month_of_week_ending_date, year_of_week_ending_date
                    from 
                                ((employee emp
                                    inner join employee_weekly_hours_activity_project emphours
                                    on emp.emp_ssn = emphours.emp_ssn)

                                left join activity act
                                on emphours.activity_code = act.activity_code)

                                left join activitycat cat
                                on act.catid = cat.catid
                    where 
                        emp.emp_ssn = @employeeId
                        and month_of_week_ending_date >= @startMonth
                        and month_of_week_ending_date <= @endMonth
                        and year_of_week_ending_date >= @startYear
                        and year_of_week_ending_date <= @endYear
                    order by
                        week_ending_date, emphours.activity_code
                ";
            OleDbParameter[] prms = new OleDbParameter[5];
            prms[0] = new OleDbParameter("@employeeId", OleDbType.VarChar);
            prms[0].Value = employeeId;
            prms[1] = new OleDbParameter("@startMonth", OleDbType.Integer);
            prms[1].Value = startMonth;
            prms[2] = new OleDbParameter("@endMonth", OleDbType.Integer);
            prms[2].Value = endMonth;
            prms[3] = new OleDbParameter("@startYear", OleDbType.Integer);
            prms[3].Value = startYear;
            prms[4] = new OleDbParameter("@endYear", OleDbType.Integer);
            prms[4].Value = endYear;
            DataTable dt = ExecuteSelect(qry, prms, strProgRptConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    EmployeeTimesheet empTimesheet = new EmployeeTimesheet();
                    empTimesheet.EmployeeId = Convert.ToInt32(dr["emp_ssn"]);
                    empTimesheet.StructureId = dr["structureid"].ToString().Trim();
                    empTimesheet.ProjectId = dr["project_id"].ToString().Trim();
                    empTimesheet.ActivityCode = Convert.ToInt32(dr["activity_code"]);
                    empTimesheet.WeekEndingDate = Convert.ToDateTime(dr["week_ending_date"]);
                    empTimesheet.WorkNumber = Convert.ToInt32(dr["work_number"]);
                    empTimesheet.TotalHours = Convert.ToSingle(dr["total_hours"]);
                    empTimesheet.ActivityDescription = dr["actdescription"].ToString().Trim();
                    empTimesheet.ActivityCategoryId = Convert.ToInt32(dr["catid"]);
                    empTimesheet.ActivityCategoryDescription = dr["catdescription"].ToString().Trim();
                    empTimesheet.MonthWeekEndingDate = Convert.ToInt32(dr["month_of_week_ending_date"]);
                    empTimesheet.YearWeekEndingDate = Convert.ToInt32(dr["year_of_week_ending_date"]);
                    empTimesheets.Add(empTimesheet);
                }
            }

            return empTimesheets;
        }

        #endregion Structures Progress Report Methods

        #region Supporting Methods
        public bool IsProd()
        {
            return prodMode;
        }

        public int GetMaxRuleId()
        {
            int maxRuleID = 1;

            string qry = @"
                                select MAX(RuleID) as MaxRuleID
                                from RuleWorkAction
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                maxRuleID = Convert.ToInt32(dr["MAXRULEID"]);
            }

            return maxRuleID;
        }

        public List<string> GetRuleCategories()
        {
            List<string> ruleCategories = new List<string>();
            string qry = @"
                                select distinct RuleCategory
                                from RuleWorkAction
                                order by RuleCategory
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ruleCategories.Add(dr["RULECATEGORY"].ToString().ToUpper());
                }
            }

            return ruleCategories;
        }

        public List<string> GetSimilarComboWorkActions()
        {
            List<string> similarComboWorkActions = new List<string>();
            string qry =
                @"
                    select comboworkaction
                    from SimilarWorkAction
                    where similar = 1
                ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    similarComboWorkActions.Add(dr["comboworkaction"].ToString());
                }
            }

            return similarComboWorkActions;
        }

        public List<int> GetWorkActionRuleIds()
        {
            List<int> workActionRuleIds = new List<int>();
            string qry = @"
                                select RuleId
                                from RuleWorkAction
                                where Active = 1
                                order by RuleSequence
                            ";

            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    workActionRuleIds.Add(Convert.ToInt32(dr["RULEID"]));
                }
            }

            return workActionRuleIds;
        }

        public List<ElementDeterioration> GetElementDeteriorationRates()
        {
            List<ElementDeterioration> elemDetRates = new List<ElementDeterioration>();
            string qry = @"
                                select *
                                from ElementDeterioration
                                where Active = 1
                                order by ElementNumber
                            ";

            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ElementDeterioration ed = new ElementDeterioration();
                    ed.ElemNum = Convert.ToInt32(dr["ELEMENTNUMBER"]);
                    ed.Beta = Convert.ToSingle(dr["BETA"]);
                    ed.RelativeWeight = Convert.ToSingle(dr["RELATIVEWEIGHT"]);
                    ed.MedYr1 = Convert.ToSingle(dr["MEDYR1"]);
                    ed.MedYr2 = Convert.ToSingle(dr["MEDYR2"]);
                    ed.MedYr3 = Convert.ToSingle(dr["MEDYR3"]);
                    ed.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    double ln2 = Math.Log(2, Math.E);
                    double power = 1 / ed.Beta;
                    ed.ScalingFactor1 = Convert.ToSingle(ed.MedYr1 / (Math.Pow(ln2, power)));
                    ed.ScalingFactor2 = Convert.ToSingle(ed.MedYr2 / (Math.Pow(ln2, power)));
                    ed.ScalingFactor3 = Convert.ToSingle(ed.MedYr3 / (Math.Pow(ln2, power)));

                    string qryo = @"
                                        select isel_nm
                                        from dot1stro.dt_isel_ty
                                        where isel_tyid = :elemNum
                                    ";
                    OracleParameter[] prms = new OracleParameter[1];
                    prms[0] = new OracleParameter("elemNum", OracleDbType.Int32);
                    prms[0].Value = ed.ElemNum;
                    DataTable dto = ExecuteSelect(qryo, prms, hsiConn);

                    if (dto != null && dto.Rows.Count > 0)
                    {
                        ed.ElemName = dto.Rows[0]["ISEL_NM"].ToString();
                    }

                    elemDetRates.Add(ed);
                }
            }

            return elemDetRates;
        }

        public List<WorkActionRule> GetWorkActionRules()
        {
            List<WorkActionRule> workActionRules = new List<WorkActionRule>();
            string qry = @"
                                select rwa.RuleId, rwa.RuleFormula, rwa.RuleCategory, rwa.WorkActionCode, rwa.RuleSequence,
                                            rwa.Active, rwa.Notes, rwa.AlternativeWorkActionCode, wa.WorkActionDesc,
                                            wa.CombinedWorkActionCodes, wa.CombinedWorkAction, rwa.RuleWorkActionNotes
                                from RuleWorkAction rwa, WorkAction wa
                                where rwa.WorkActionCode = wa.WorkActionCode
                                        and rwa.RuleId > 0
                                        and wa.Active = 1
                                order by rwa.RuleSequence
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkActionRule war = new WorkActionRule();
                    war.RuleId = Convert.ToInt32(dr["RULEID"]);
                    war.RuleFormula = dr["RULEFORMULA"].ToString().Trim();
                    war.RuleCategory = dr["RULECATEGORY"].ToString();
                    war.RuleSequence = Convert.ToInt32(dr["RULESEQUENCE"]);
                    war.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    war.RuleNotes = dr["NOTES"].ToString().Trim();
                    war.RuleWorkActionNotes = dr["RULEWORKACTIONNOTES"].ToString().Trim();
                    war.ResultingWorkAction = GetStructureWorkAction(dr["WORKACTIONCODE"].ToString());

                    var alternativeWorkActionCodes = dr["ALTERNATIVEWORKACTIONCODE"].ToString().Trim();
                    foreach (var code in alternativeWorkActionCodes.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        StructureWorkAction swa = GetStructureWorkAction(code);
                        war.AlternativeWorkActions.Add(swa);
                    }

                    if (Convert.ToBoolean(dr["COMBINEDWORKACTION"]))
                    {
                        var comprisedWorkActionCodes = dr["COMBINEDWORKACTIONCODES"].ToString();
                        foreach (var code in comprisedWorkActionCodes.Split(new string[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            StructureWorkAction swa = GetStructureWorkAction(code);
                            if (swa != null)
                            {
                                war.ComprisedWorkActions.Add(swa);
                            }
                        }
                    }

                    string qry2 = @"
                                        select SecondaryWorkActionCode
                                        from CombinedWorkAction
                                        where MainWorkActionCode = @mainWorkActionCode
                                    ";
                    SqlParameter[] prms2 = new SqlParameter[1];
                    prms2[0] = new SqlParameter("@mainWorkActionCode", SqlDbType.VarChar);
                    prms2[0].Value = war.ResultingWorkAction.WorkActionCode;
                    DataTable dt2 = ExecuteSelect(qry2, prms2, samConn);

                    if (dt2 != null && dt2.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            StructureWorkAction swa = GetStructureWorkAction(dr2["SECONDARYWORKACTIONCODE"].ToString());
                            if (swa != null)
                            {
                                war.PotentialCombinedWorkActions.Add(swa);
                            }
                        }
                    }

                    workActionRules.Add(war);
                }
            }

            return workActionRules;
        }

        public void InsertOverlaysCombinedWorkActions()
        {
            List<string> overlays = new List<string>() { "77", "65", "03" };
            List<string> combinations = new List<string>() { "07", "75", "49", "04", "42", "11", "12", "101", "66", "14", "40", "201", "29", "204", "206" };

            foreach (var overlay in overlays)
            {
                foreach (var combination in combinations)
                {
                    string qry = @"
                                        insert into CombinedWorkAction(MainWorkActionCode, SecondaryWorkActionCode, CombinedWorkActionCode, BypassRule, Active)
                                        values (@mainWorkActionCode, @secondaryWorkActionCode, '', 0, 1)
                                    ";
                    SqlParameter[] prms = new SqlParameter[2];
                    prms[0] = new SqlParameter("@mainWorkActionCode", SqlDbType.VarChar);
                    prms[0].Value = overlay;
                    prms[1] = new SqlParameter("@secondaryWorkActionCode", SqlDbType.VarChar);
                    prms[1].Value = combination;

                    ExecuteInsertUpdateDelete(qry, prms, samConn);
                }
            }
        }

        public void DeletePmicRows()
        {
            string qry = @"
                                delete
                                from Pmic
                            ";
            ExecuteInsertUpdateDelete(qry, samConn);
        }

        public bool IsStructureOnHighClearanceRoute(string strId)
        {
            bool highClearanceRoute = false;

            string qry = @"
                                select StructureId, HighClearanceRoute
                                from Structure str, Corridor
                                where StructureId = @strId
                            ";
            SqlParameter[] prm = new SqlParameter[1];
            prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prm[0].Value = strId;
            DataTable d = ExecuteSelect(qry, prm, samConn);

            if (d != null && d.Rows.Count > 0 && Convert.ToBoolean(d.Rows[0]["HIGHCLEARANCEROUTE"]))
            {
                highClearanceRoute = true;
            }

            return highClearanceRoute;
        }

        public void UpdateHighClearanceRoute(string strId)
        {
            string qry = @"
                                update structure
                                set HighClearanceRoute = 1
                                where structureid = @structureId
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = strId;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public List<string> GetDeckComposition(string strId)
        {
            List<string> deckComposition = new List<string>();
            string qry = @"
                                select brmtl_layr_tycd
                                from dot1stro.dt_brmtl_layr
                                where strc_id = :strId
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            foreach (DataRow dr in dt.Rows)
            {
                deckComposition.Add(dr["brmtl_layr_tycd"].ToString());
            }

            return deckComposition;
        }

        public StructureLite GetStructureCorridorCode(string strId)
        {
            StructureLite strLite = null;

            string qry = @"
                                select StructureId, str.CorridorCode, CorridorDesc
                                from Structure str, Corridor
                                where StructureId = @strId
                                    and str.CorridorCode = Corridor.CorridorCode
                            ";
            SqlParameter[] prm = new SqlParameter[1];
            prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prm[0].Value = strId;
            DataTable d = ExecuteSelect(qry, prm, samConn);

            if (d != null && d.Rows.Count > 0)
            {
                strLite = new StructureLite();
                strLite.StructureId = strId;
                strLite.CorridorCode = d.Rows[0]["CORRIDORCODE"].ToString();
                strLite.CorridorDesc = d.Rows[0]["CORRIDORDESC"].ToString();
            }

            return strLite;
        }

        public void UpdateStructureCorridorCode(StructureLite strLite)
        {
            string qry = @"
                                update structure
                                set corridorcode = @corridorCode
                                where structureid = @structureId
                            ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@corridorCode", SqlDbType.VarChar);
            prms[0].Value = strLite.CorridorCode;
            prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[1].Value = strLite.StructureId;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public void InsertStructureCorridorCode(StructureLite strLite)
        {
            string qry = @"
                                insert into structure(structureid, corridorcode)
                                values (@structureId, @corridorCode)
                            ";
            SqlParameter[] prms = new SqlParameter[2];
            prms[0] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[0].Value = strLite.StructureId;
            prms[1] = new SqlParameter("@corridorCode", SqlDbType.VarChar);
            prms[1].Value = strLite.CorridorCode;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public void InsertImprovementRow(ProgrammedWorkAction pWA)
        {
            string qry =
                @"
                        insert into Improvement(
                                            Dot_Rgn_Cd,
                                            Wicy_Basd_Rgn_Nm,
                                            Fos_Proj_Id,
                                            Pproj_Id,

                                            Estcp_Schd_Dt,
                                            State_Fiscal_Year,
                                            Ppjl_Rtnm_Txt,
                                            Sub_Pgm_Cd,
                                            Sub_Pgm_Desc,
                                            Pproj_Fos_Titl_Txt,
                                            Pproj_Fos_Lmt_Txt,
                                            Pproj_Fos_Cncp_Txt,
                                            Pproj_Fnty_Cd,
                                            Pproj_Fnty_Desc,
                                            Wdot_Pgm_Cd,
                                            Wdot_Pgm_Desc,
                                            Lfcy_Stg_Cd,

                                            Row_Isrt_Tmst,
                                            
                                           

                                            Pproj_Cncp_Cd,
                                            
                                            Pproj_Cncp_Cd_New,
                                            Mgr_Pproj_Ptcp_Nm
                                        )
                        values (
                                    @dotRegionCode,
                                    @county,
                                    @fosProjId,
                                    @pProjId,
                                    
                                    @estimatedCompletionDate,
                                    @stateFiscalYear,
                                    @route,
                                    @subProgramCode,
                                    @subProgramDesc,
                                    @title,
                                    @limit,
                                    @concept,
                                    @functionalTypeCode,
                                    @functionalTypeDesc,
                                    @wisDOTProgramCode,
                                    @wisDOTProgramDesc,
                                    @lifeCycleStageCode,
                                    
                                    @rowInsertTimeStamp,

                                    @planningProjectConceptCode,
                                    
                                    @planningProjectConceptCodeNew,
                                    @projectManager
                                    
                                )
                 ";

            /*
            public string PlanningProjectConceptCode { get; set; } //Improvement Type
        public string PlanningProjectConceptDesc { get; set; }
            */

            SqlParameter[] prms = new SqlParameter[21];
            prms[0] = new SqlParameter("@dotRegionCode", SqlDbType.VarChar);
            prms[0].Value = pWA.DotRegionCode;
            prms[1] = new SqlParameter("@county", SqlDbType.VarChar);
            prms[1].Value = pWA.County;
            prms[2] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
            prms[2].Value = pWA.FosProjId;
            prms[3] = new SqlParameter("@pProjId", SqlDbType.Int);
            prms[3].Value = pWA.PProjId;

            prms[4] = new SqlParameter("@estimatedCompletionDate", SqlDbType.DateTime);
            prms[4].Value = pWA.EstimatedCompletionDate;

            if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
            {
                prms[4].Value = DBNull.Value;
            }

            int fiscalYear = pWA.EstimatedCompletionDate.Year;

            if (pWA.EstimatedCompletionDate.Month > 6)
            {
                fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
            }

            prms[5] = new SqlParameter("@stateFiscalYear", SqlDbType.Int);
            prms[5].Value = fiscalYear;

            if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
            {
                prms[5].Value = DBNull.Value;
            }

            prms[6] = new SqlParameter("@route", SqlDbType.VarChar);
            prms[6].Value = pWA.Route;
            prms[7] = new SqlParameter("@subProgramCode", SqlDbType.VarChar);
            prms[7].Value = pWA.SubProgramCode;
            prms[8] = new SqlParameter("@subProgramDesc", SqlDbType.VarChar);
            prms[8].Value = pWA.SubProgramDesc;
            prms[9] = new SqlParameter("@title", SqlDbType.VarChar);
            prms[9].Value = pWA.Title;
            prms[10] = new SqlParameter("@limit", SqlDbType.VarChar);
            prms[10].Value = pWA.Limit;
            prms[11] = new SqlParameter("@concept", SqlDbType.VarChar);
            prms[11].Value = pWA.Concept;
            prms[12] = new SqlParameter("@functionalTypeCode", SqlDbType.VarChar);
            prms[12].Value = pWA.FunctionalTypeCode;
            prms[13] = new SqlParameter("@functionalTypeDesc", SqlDbType.VarChar);
            prms[13].Value = pWA.FunctionalTypeDesc;
            prms[14] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
            prms[14].Value = pWA.WisDOTProgramCode;
            prms[15] = new SqlParameter("@wisDOTProgramDesc", SqlDbType.VarChar);
            prms[15].Value = pWA.WisDOTProgramDesc;
            prms[16] = new SqlParameter("@lifeCycleStageCode", SqlDbType.VarChar);
            prms[16].Value = pWA.LifeCycleStageCode;

            prms[17] = new SqlParameter("@rowInsertTimeStamp", SqlDbType.DateTime);
            prms[17].Value = DateTime.Now;


            prms[18] = new SqlParameter("@planningProjectConceptCode", SqlDbType.VarChar);
            prms[18].Value = pWA.PlanningProjectConceptCode;

            prms[19] = new SqlParameter("@planningProjectConceptCodeNew", SqlDbType.VarChar);
            prms[19].Value = pWA.PlanningProjectConceptCode;

            /*
            prms[19] = new SqlParameter("@planningProjectConceptCodeNew", SqlDbType.VarChar);
            prms[19].Value = pWA.PlanningProjectConceptCodeNew;
            

            if (String.IsNullOrEmpty(pWA.PlanningProjectConceptCodeNew))
            {
                prms[19].Value = DBNull.Value;
            }
            */

            prms[20] = new SqlParameter("@projectManager", SqlDbType.VarChar);
            prms[20].Value = pWA.ProjectManager;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public void DeleteStructureProgramReviewCurrent()
        {
            string qry = @"
                            delete from StructureProgramReviewCurrent
                        ";
            ExecuteInsertUpdateDelete(qry, samConn);
        }

        public void InsertStructureProgramReview(int dateStamp, StructureProgramReview spr)
        {
            for (int i = 0; i <= 1; i++)
            {
                string qry = "";
                if (i == 0)
                {
                    qry = @"insert into StructureProgramReview(";
                }
                else
                {
                    qry = @"insert into StructureProgramReviewCurrent(";
                }

                qry += @"
                                    RowInsertDateStamp,
                                    StructureId,
                                    WorkActionCode,
                                    WorkActionDesc,
                                    WorkActionYear,
                                    CertificationStatus,
                                    StatusDate,
                                    IsCertified,
                                    CertificationNotes,
                                    DiscussionNotes,
                                    IsBridge,
                                    Region,
                                    RegionNumber,
                                    County,
                                    CountyNumber,
                                    FundingEligibility,
                                    InFiips,
                                    FosProjectId,
                                    FiipsNotes,
                                    AdvanceableWorkActionYear,
                                    IsScopeAMatch,
                                    IsYearAMatch)
                                values(
                                        @rowInsertDateStamp,
                                        @structureId,
                                        @workActionCode,
                                        @workActionDesc,
                                        @workActionYear,
                                        @certificationStatus,
                                        @statusDate,
                                        @isCertified,
                                        @certificationNotes,
                                        @discussionNotes,
                                        @isBridge,
                                        @region,
                                        @regionNumber,
                                        @county,
                                        @countyNumber,
                                        @fundingEligibility,
                                        @inFiips,
                                        @fosProjectId,
                                        @fiipsNotes,
                                        @advanceableWorkActionYear,
                                        @isScopeAMatch,
                                        @isYearAMatch
                                    )
                            ";

                SqlParameter[] prms = new SqlParameter[22];
                prms[0] = new SqlParameter("@rowInsertDateStamp", SqlDbType.Int);
                prms[0].Value = dateStamp;
                prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
                prms[1].Value = spr.StructureId;
                prms[2] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
                prms[2].Value = spr.WorkActionCode;
                prms[3] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
                prms[3].Value = spr.WorkActionDesc;
                prms[4] = new SqlParameter("@workActionYear", SqlDbType.Int);
                prms[4].Value = spr.WorkActionYear;
                prms[5] = new SqlParameter("@certificationStatus", SqlDbType.VarChar);
                prms[5].Value = spr.CertificationStatus;
                prms[6] = new SqlParameter("@statusDate", SqlDbType.DateTime);
                prms[6].Value = spr.StatusDate;

                if (spr.StatusDate == null || spr.StatusDate.Year == 1)
                    prms[6].Value = DBNull.Value;

                prms[7] = new SqlParameter("@isCertified", SqlDbType.Bit);
                prms[7].Value = spr.IsCertified;
                prms[8] = new SqlParameter("@certificationNotes", SqlDbType.VarChar);
                prms[8].Value = spr.CertificationNotes;
                prms[9] = new SqlParameter("@discussionNotes", SqlDbType.VarChar);
                prms[9].Value = spr.DiscussionNotes;
                prms[10] = new SqlParameter("@isBridge", SqlDbType.Bit);
                prms[10].Value = spr.IsBridge;
                prms[11] = new SqlParameter("@region", SqlDbType.VarChar);
                prms[11].Value = spr.Region;
                prms[12] = new SqlParameter("@regionNumber", SqlDbType.VarChar);
                prms[12].Value = spr.RegionNumber;
                prms[13] = new SqlParameter("@county", SqlDbType.VarChar);
                prms[13].Value = spr.County;
                prms[14] = new SqlParameter("@countyNumber", SqlDbType.VarChar);
                prms[14].Value = spr.CountyNumber;
                prms[15] = new SqlParameter("@fundingEligibility", SqlDbType.VarChar);
                prms[15].Value = spr.FundingEligibilityCsv;
                prms[16] = new SqlParameter("@inFiips", SqlDbType.Bit);
                prms[16].Value = spr.InFiips;
                prms[17] = new SqlParameter("@fosProjectId", SqlDbType.VarChar);
                prms[17].Value = spr.FosProjectId;
                prms[18] = new SqlParameter("@fiipsNotes", SqlDbType.VarChar);
                prms[18].Value = spr.FiipsNotes;
                prms[19] = new SqlParameter("@advanceableWorkActionYear", SqlDbType.Int);
                prms[19].Value = spr.AdvanceableWorkActionYear;

                if (spr.AdvanceableWorkActionYear == 0 || spr.AdvanceableWorkActionYear == -1)
                {
                    prms[19].Value = DBNull.Value;
                }

                prms[20] = new SqlParameter("@isScopeAMatch", SqlDbType.Bit);
                prms[20].Value = spr.IsScopeAMatch;
                prms[21] = new SqlParameter("@isYearAMatch", SqlDbType.Bit);
                prms[21].Value = spr.IsYearAMatch;

                ExecuteInsertUpdateDelete(qry, prms, samConn);
            }
        }

        public void InsertStructureProgramDailyInventoryRow(int dateStamp, ProgrammedWorkAction pWA)
        {
            string qry =
                @"
                        insert into StructureProgramDailyInventory(
                                            Dot_Rgn_Cd,
                                            Extg_Strc_Id,
                                            Wicy_Basd_Rgn_Nm,
                                            Fos_Proj_Id,
                                            Pproj_Id,
                                            Plnd_Strc_Id,
                                            Fndg_Ctgy_Nb,
                                            Strc_Work_Tycd,
                                            Strc_Work_Tydc,
                                            WorkActionCode,
                                            WorkActionDesc,
                                            Estcp_Schd_Dt,
                                            State_Fiscal_Year,
                                            Ppjl_Rtnm_Txt,
                                            Sub_Pgm_Cd,
                                            Sub_Pgm_Desc,
                                            Pproj_Fos_Titl_Txt,
                                            Pproj_Fos_Lmt_Txt,
                                            Pproj_Fos_Cncp_Txt,
                                            Pproj_Fnty_Cd,
                                            Pproj_Fnty_Desc,
                                            Wdot_Pgm_Cd,
                                            Wdot_Pgm_Desc,
                                            Lfcy_Stg_Cd,
                                            Tot_With_Dlvy_Amt,
                                            Tot_Wo_Dlvy_Amt,

                                            Row_Isrt_Tmst,
                                            Pproj_Cncp_Cd,
                                            Estcp_Ty_Cd,
                                            Pproj_Stus_Fl,
                                            Fndg_Ctgy_Desc,
                                            Estcp_Prmy_Cpnt_Fl,
                                            Fed_Impt_Ty_Cd,
                                            Fed_Impt_Ty_Desc,
                                            Mgr_Pproj_Ptcp_Nm, 

                                            EarliestAdvanceableLetDate,
                                            LatestAdvanceableLetDate,
                                            LifecycleStageDate,
                                            RowInsertDateStamp,
                                            IsDuplicate,
                                            IsBridge,
                                            DesignProjectId
                                        )
                        values (
                                    @dotRegionCode,
                                    @structureId,
                                    @county,
                                    @fosProjId,
                                    @pProjId,
                                    @newStructureId,
                                    @fundingCategoryNumber,
                                    @originalWorkActionCode,
                                    @originalWorkActionDesc,
                                    @workActionCode,
                                    @workActionDesc,
                                    @estimatedCompletionDate,
                                    @stateFiscalYear,
                                    @route,
                                    @subProgramCode,
                                    @subProgramDesc,
                                    @title,
                                    @limit,
                                    @concept,
                                    @functionalTypeCode,
                                    @functionalTypeDesc,
                                    @wisDOTProgramCode,
                                    @wisDOTProgramDesc,
                                    @lifeCycleStageCode,
                                    @workTotalWithDeliveryAmount,
                                    @workTotalWithoutDeliveryAmount,

                                    @rowInsertTimeStamp,
                                    @planningProjectConceptCode,
                                    @componentTypeCode,
                                    @projectStatusFlag,
                                    @fundingCategoryDesc,
                                    'Y',
                                    @federalImprovementTypeCode,
                                    @federalImprovementTypeDesc,
                                    @projectManager,

                                    @earliestAdvanceableLetDate,
                                    @latestAdvanceableLetDate,
                                    @lifeCycleStageDate,
                                    @dateStamp,
                                    @isDuplicate,
                                    @isBridge,
                                    @designProjectId
                                )
                 ";

            /*
            public string PlanningProjectConceptCode { get; set; } //Improvement Type
        public string PlanningProjectConceptDesc { get; set; }
            */

            SqlParameter[] prms = new SqlParameter[41];
            prms[0] = new SqlParameter("@dotRegionCode", SqlDbType.VarChar);
            prms[0].Value = pWA.DotRegionCode;
            prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[1].Value = pWA.StructureId;
            prms[2] = new SqlParameter("@county", SqlDbType.VarChar);
            prms[2].Value = pWA.County;
            prms[3] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
            prms[3].Value = pWA.FosProjId;
            prms[4] = new SqlParameter("@pProjId", SqlDbType.Int);
            prms[4].Value = pWA.PProjId;
            prms[5] = new SqlParameter("@newStructureId", SqlDbType.VarChar);
            prms[5].Value = pWA.NewStructureId;
            prms[6] = new SqlParameter("@fundingCategoryNumber", SqlDbType.VarChar);
            prms[6].Value = pWA.FundingCategoryNumber;
            prms[7] = new SqlParameter("@originalWorkActionCode", SqlDbType.VarChar);
            prms[7].Value = pWA.OriginalWorkActionCode;
            prms[8] = new SqlParameter("@originalWorkActionDesc", SqlDbType.VarChar);
            prms[8].Value = pWA.OriginalWorkActionDesc;

            // PMIC has new work types so original and new/translated work types should be the same
            prms[9] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[9].Value = pWA.OriginalWorkActionCode;
            prms[10] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
            prms[10].Value = pWA.OriginalWorkActionDesc;
            /*
            prms[9] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[9].Value = pWA.WorkActionCode;
            prms[10] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
            prms[10].Value = pWA.WorkActionDesc;
            */
            /*
            if (String.IsNullOrEmpty(pWA.WorkActionCode))
            {
                prms[9].Value = DBNull.Value;
                prms[10].Value = DBNull.Value;
            }
            */

            prms[11] = new SqlParameter("@estimatedCompletionDate", SqlDbType.DateTime);
            prms[11].Value = pWA.EstimatedCompletionDate;

            if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
            {
                prms[11].Value = DBNull.Value;
            }

            int fiscalYear = pWA.EstimatedCompletionDate.Year;

            if (pWA.EstimatedCompletionDate.Month > 6)
            {
                fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
            }

            prms[12] = new SqlParameter("@stateFiscalYear", SqlDbType.Int);
            prms[12].Value = fiscalYear;

            if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
            {
                prms[12].Value = DBNull.Value;
            }

            prms[13] = new SqlParameter("@route", SqlDbType.VarChar);
            prms[13].Value = pWA.Route;
            prms[14] = new SqlParameter("@subProgramCode", SqlDbType.VarChar);
            prms[14].Value = pWA.SubProgramCode;
            prms[15] = new SqlParameter("@subProgramDesc", SqlDbType.VarChar);
            prms[15].Value = pWA.SubProgramDesc;
            prms[16] = new SqlParameter("@title", SqlDbType.VarChar);
            prms[16].Value = pWA.Title;
            prms[17] = new SqlParameter("@limit", SqlDbType.VarChar);
            prms[17].Value = pWA.Limit;
            prms[18] = new SqlParameter("@concept", SqlDbType.VarChar);
            prms[18].Value = pWA.Concept;
            prms[19] = new SqlParameter("@functionalTypeCode", SqlDbType.VarChar);
            prms[19].Value = pWA.FunctionalTypeCode;
            prms[20] = new SqlParameter("@functionalTypeDesc", SqlDbType.VarChar);
            prms[20].Value = pWA.FunctionalTypeDesc;
            prms[21] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
            prms[21].Value = pWA.WisDOTProgramCode;
            prms[22] = new SqlParameter("@wisDOTProgramDesc", SqlDbType.VarChar);
            prms[22].Value = pWA.WisDOTProgramDesc;
            prms[23] = new SqlParameter("@lifeCycleStageCode", SqlDbType.VarChar);
            prms[23].Value = pWA.LifeCycleStageCode;
            prms[24] = new SqlParameter("@workTotalWithDeliveryAmount", SqlDbType.Float);
            prms[24].Value = pWA.WorkTotalWithDeliveryAmount;
            prms[25] = new SqlParameter("@workTotalWithoutDeliveryAmount", SqlDbType.Float);
            prms[25].Value = pWA.WorkTotalWithoutDeliveryAmount;

            prms[26] = new SqlParameter("@rowInsertTimeStamp", SqlDbType.DateTime);
            prms[26].Value = DateTime.Now;
            prms[27] = new SqlParameter("@planningProjectConceptCode", SqlDbType.VarChar);
            prms[27].Value = pWA.PlanningProjectConceptCode;
            prms[28] = new SqlParameter("@componentTypeCode", SqlDbType.VarChar);
            prms[28].Value = pWA.ComponentTypeCode;
            prms[29] = new SqlParameter("@projectStatusFlag", SqlDbType.VarChar);
            prms[29].Value = pWA.ProjectStatusFlag;
            prms[30] = new SqlParameter("@fundingCategoryDesc", SqlDbType.VarChar);
            prms[30].Value = pWA.FundingCategoryDesc;
            prms[31] = new SqlParameter("@federalImprovementTypeCode", SqlDbType.VarChar);
            prms[31].Value = pWA.FederalImprovementTypeCode;
            prms[32] = new SqlParameter("@federalImprovementTypeDesc", SqlDbType.VarChar);
            prms[32].Value = pWA.FederalImprovementTypeDesc;
            prms[33] = new SqlParameter("@projectManager", SqlDbType.VarChar);
            prms[33].Value = pWA.ProjectManager;

            prms[34] = new SqlParameter("@earliestAdvanceableLetDate", SqlDbType.DateTime);
            prms[34].Value = pWA.EarliestAdvanceableLetDate;

            if (pWA.EarliestAdvanceableLetDate == null || pWA.EarliestAdvanceableLetDate.Year == 1)
            {
                prms[34].Value = DBNull.Value;
            }

            prms[35] = new SqlParameter("@latestAdvanceableLetDate", SqlDbType.DateTime);
            prms[35].Value = pWA.LatestAdvanceableLetDate;

            if (pWA.LatestAdvanceableLetDate == null || pWA.LatestAdvanceableLetDate.Year == 1)
            {
                prms[35].Value = DBNull.Value;
            }

            prms[36] = new SqlParameter("@lifeCycleStageDate", SqlDbType.DateTime);
            prms[36].Value = pWA.LifeCycleStageDate;

            if (pWA.LifeCycleStageDate == null || pWA.LifeCycleStageDate.Year == 1)
            {
                prms[36].Value = DBNull.Value;
            }

            prms[37] = new SqlParameter("@dateStamp", SqlDbType.Int);
            prms[37].Value = dateStamp;

            prms[38] = new SqlParameter("@isDuplicate", SqlDbType.Bit);
            prms[38].Value = pWA.IsDuplicate ? 1 : 0;

            prms[39] = new SqlParameter("@isBridge", SqlDbType.Bit);
            prms[39].Value = pWA.IsBridge ? 1 : 0;

            prms[40] = new SqlParameter("@designProjectId", SqlDbType.VarChar);
            prms[40].Value = pWA.DesignProjectId;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public void InsertPmicRow(ProgrammedWorkAction pWA, int dateStamp)
        {
            string qry =
                @"
                        insert into Pmic(
                                            Dot_Rgn_Cd,
                                            Extg_Strc_Id,
                                            Wicy_Basd_Rgn_Nm,
                                            Fos_Proj_Id,
                                            Pproj_Id,
                                            Plnd_Strc_Id,
                                            Fndg_Ctgy_Nb,
                                            Strc_Work_Tycd,
                                            Strc_Work_Tydc,
                                            WorkActionCode,
                                            WorkActionDesc,
                                            Estcp_Schd_Dt,
                                            State_Fiscal_Year,
                                            Ppjl_Rtnm_Txt,
                                            Sub_Pgm_Cd,
                                            Sub_Pgm_Desc,
                                            Pproj_Fos_Titl_Txt,
                                            Pproj_Fos_Lmt_Txt,
                                            Pproj_Fos_Cncp_Txt,
                                            Pproj_Fnty_Cd,
                                            Pproj_Fnty_Desc,
                                            Wdot_Pgm_Cd,
                                            Wdot_Pgm_Desc,
                                            Lfcy_Stg_Cd,
                                            Tot_With_Dlvy_Amt,
                                            Tot_Wo_Dlvy_Amt,

                                            Row_Isrt_Tmst,
                                            Pproj_Cncp_Cd,
                                            Estcp_Ty_Cd,
                                            Pproj_Stus_Fl,
                                            Fndg_Ctgy_Desc,
                                            Estcp_Prmy_Cpnt_Fl,
                                            Fed_Impt_Ty_Cd,
                                            Fed_Impt_Ty_Desc,
                                            Mgr_Pproj_Ptcp_Nm, 

                                            EarliestAdvanceableLetDate,
                                            LatestAdvanceableLetDate,
                                            LifecycleStageDate,
                                            RowInsertDateStamp, 
                                            IsDuplicate,
                                            IsBridge,
                                            DesignProjectId,

                                            PseDate,
                                            EarliestPseDate
                                        )
                        values (
                                    @dotRegionCode,
                                    @structureId,
                                    @county,
                                    @fosProjId,
                                    @pProjId,
                                    @newStructureId,
                                    @fundingCategoryNumber,
                                    @originalWorkActionCode,
                                    @originalWorkActionDesc,
                                    @workActionCode,
                                    @workActionDesc,
                                    @estimatedCompletionDate,
                                    @stateFiscalYear,
                                    @route,
                                    @subProgramCode,
                                    @subProgramDesc,
                                    @title,
                                    @limit,
                                    @concept,
                                    @functionalTypeCode,
                                    @functionalTypeDesc,
                                    @wisDOTProgramCode,
                                    @wisDOTProgramDesc,
                                    @lifeCycleStageCode,
                                    @workTotalWithDeliveryAmount,
                                    @workTotalWithoutDeliveryAmount,

                                    @rowInsertTimeStamp,
                                    @planningProjectConceptCode,
                                    @componentTypeCode,
                                    @projectStatusFlag,
                                    @fundingCategoryDesc,
                                    'Y',
                                    @federalImprovementTypeCode,
                                    @federalImprovementTypeDesc,
                                    @projectManager,

                                    @earliestAdvanceableLetDate,
                                    @latestAdvanceableLetDate,
                                    @lifeCycleStageDate,
                                    @dateStamp,
                                    @isDuplicate,
                                    @isBridge,
                                    @designProjectId,

                                    @pseDate,
                                    @earliestPseDate
                                )
                 ";

            /*
            public string PlanningProjectConceptCode { get; set; } //Improvement Type
        public string PlanningProjectConceptDesc { get; set; }
            */

            SqlParameter[] prms = new SqlParameter[43];
            prms[0] = new SqlParameter("@dotRegionCode", SqlDbType.VarChar);
            prms[0].Value = pWA.DotRegionCode;
            prms[1] = new SqlParameter("@structureId", SqlDbType.VarChar);
            prms[1].Value = pWA.StructureId;
            prms[2] = new SqlParameter("@county", SqlDbType.VarChar);
            prms[2].Value = pWA.County;
            prms[3] = new SqlParameter("@fosProjId", SqlDbType.VarChar);
            prms[3].Value = pWA.FosProjId;
            prms[4] = new SqlParameter("@pProjId", SqlDbType.Int);
            prms[4].Value = pWA.PProjId;
            prms[5] = new SqlParameter("@newStructureId", SqlDbType.VarChar);
            prms[5].Value = pWA.NewStructureId;
            prms[6] = new SqlParameter("@fundingCategoryNumber", SqlDbType.VarChar);
            prms[6].Value = pWA.FundingCategoryNumber;
            prms[7] = new SqlParameter("@originalWorkActionCode", SqlDbType.VarChar);
            prms[7].Value = pWA.OriginalWorkActionCode;
            prms[8] = new SqlParameter("@originalWorkActionDesc", SqlDbType.VarChar);
            prms[8].Value = pWA.OriginalWorkActionDesc;

            // PMIC has new work types so original and new/translated work types should be the same
            prms[9] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[9].Value = pWA.OriginalWorkActionCode;
            prms[10] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
            prms[10].Value = pWA.OriginalWorkActionDesc;
            /*
            prms[9] = new SqlParameter("@workActionCode", SqlDbType.VarChar);
            prms[9].Value = pWA.WorkActionCode;
            prms[10] = new SqlParameter("@workActionDesc", SqlDbType.VarChar);
            prms[10].Value = pWA.WorkActionDesc;
            */
            /*
            if (String.IsNullOrEmpty(pWA.WorkActionCode))
            {
                prms[9].Value = DBNull.Value;
                prms[10].Value = DBNull.Value;
            }
            */

            prms[11] = new SqlParameter("@estimatedCompletionDate", SqlDbType.DateTime);
            prms[11].Value = pWA.EstimatedCompletionDate;

            if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
            {
                prms[11].Value = DBNull.Value;
            }

            int fiscalYear = pWA.EstimatedCompletionDate.Year;

            if (pWA.EstimatedCompletionDate.Month > 6)
            {
                fiscalYear = pWA.EstimatedCompletionDate.Year + 1;
            }

            prms[12] = new SqlParameter("@stateFiscalYear", SqlDbType.Int);
            prms[12].Value = fiscalYear;

            if (pWA.EstimatedCompletionDate == null || pWA.EstimatedCompletionDate.Year == 1)
            {
                prms[12].Value = DBNull.Value;
            }

            prms[13] = new SqlParameter("@route", SqlDbType.VarChar);
            prms[13].Value = pWA.Route;
            prms[14] = new SqlParameter("@subProgramCode", SqlDbType.VarChar);
            prms[14].Value = pWA.SubProgramCode;
            prms[15] = new SqlParameter("@subProgramDesc", SqlDbType.VarChar);
            prms[15].Value = pWA.SubProgramDesc;
            prms[16] = new SqlParameter("@title", SqlDbType.VarChar);
            prms[16].Value = pWA.Title;
            prms[17] = new SqlParameter("@limit", SqlDbType.VarChar);
            prms[17].Value = pWA.Limit;
            prms[18] = new SqlParameter("@concept", SqlDbType.VarChar);
            prms[18].Value = pWA.Concept;
            prms[19] = new SqlParameter("@functionalTypeCode", SqlDbType.VarChar);
            prms[19].Value = pWA.FunctionalTypeCode;
            prms[20] = new SqlParameter("@functionalTypeDesc", SqlDbType.VarChar);
            prms[20].Value = pWA.FunctionalTypeDesc;
            prms[21] = new SqlParameter("@wisDOTProgramCode", SqlDbType.VarChar);
            prms[21].Value = pWA.WisDOTProgramCode;
            prms[22] = new SqlParameter("@wisDOTProgramDesc", SqlDbType.VarChar);
            prms[22].Value = pWA.WisDOTProgramDesc;
            prms[23] = new SqlParameter("@lifeCycleStageCode", SqlDbType.VarChar);
            prms[23].Value = pWA.LifeCycleStageCode;
            prms[24] = new SqlParameter("@workTotalWithDeliveryAmount", SqlDbType.Float);
            prms[24].Value = pWA.WorkTotalWithDeliveryAmount;
            prms[25] = new SqlParameter("@workTotalWithoutDeliveryAmount", SqlDbType.Float);
            prms[25].Value = pWA.WorkTotalWithoutDeliveryAmount;

            prms[26] = new SqlParameter("@rowInsertTimeStamp", SqlDbType.DateTime);
            prms[26].Value = DateTime.Now;
            prms[27] = new SqlParameter("@planningProjectConceptCode", SqlDbType.VarChar);
            prms[27].Value = pWA.PlanningProjectConceptCode;
            prms[28] = new SqlParameter("@componentTypeCode", SqlDbType.VarChar);
            prms[28].Value = pWA.ComponentTypeCode;
            prms[29] = new SqlParameter("@projectStatusFlag", SqlDbType.VarChar);
            prms[29].Value = pWA.ProjectStatusFlag;
            prms[30] = new SqlParameter("@fundingCategoryDesc", SqlDbType.VarChar);
            prms[30].Value = pWA.FundingCategoryDesc;
            prms[31] = new SqlParameter("@federalImprovementTypeCode", SqlDbType.VarChar);
            prms[31].Value = pWA.FederalImprovementTypeCode;
            prms[32] = new SqlParameter("@federalImprovementTypeDesc", SqlDbType.VarChar);
            prms[32].Value = pWA.FederalImprovementTypeDesc;
            prms[33] = new SqlParameter("@projectManager", SqlDbType.VarChar);
            prms[33].Value = pWA.ProjectManager;

            prms[34] = new SqlParameter("@earliestAdvanceableLetDate", SqlDbType.DateTime);
            prms[34].Value = pWA.EarliestAdvanceableLetDate;

            if (pWA.EarliestAdvanceableLetDate == null || pWA.EarliestAdvanceableLetDate.Year == 1)
            {
                prms[34].Value = DBNull.Value;
            }

            prms[35] = new SqlParameter("@latestAdvanceableLetDate", SqlDbType.DateTime);
            prms[35].Value = pWA.LatestAdvanceableLetDate;

            if (pWA.LatestAdvanceableLetDate == null || pWA.LatestAdvanceableLetDate.Year == 1)
            {
                prms[35].Value = DBNull.Value;
            }

            prms[36] = new SqlParameter("@lifeCycleStageDate", SqlDbType.DateTime);
            prms[36].Value = pWA.LifeCycleStageDate;

            if (pWA.LifeCycleStageDate == null || pWA.LifeCycleStageDate.Year == 1)
            {
                prms[36].Value = DBNull.Value;
            }

            prms[37] = new SqlParameter("@dateStamp", SqlDbType.Int);
            prms[37].Value = dateStamp;

            prms[38] = new SqlParameter("@isDuplicate", SqlDbType.Bit);
            prms[38].Value = pWA.IsDuplicate ? 1 : 0;

            prms[39] = new SqlParameter("@isBridge", SqlDbType.Bit);
            prms[39].Value = pWA.IsBridge ? 1 : 0;

            prms[40] = new SqlParameter("@designProjectId", SqlDbType.VarChar);
            prms[40].Value = pWA.DesignProjectId;

            prms[41] = new SqlParameter("@pseDate", SqlDbType.DateTime);
            prms[41].Value = pWA.PseDate;

            if (pWA.PseDate == null || pWA.PseDate.Year == 1)
            {
                prms[41].Value = DBNull.Value;
            }

            prms[42] = new SqlParameter("@earliestPseDate", SqlDbType.DateTime);
            prms[42].Value = pWA.EarliestPseDate;

            if (pWA.EarliestPseDate == null || pWA.EarliestPseDate.Year == 1)
            {
                prms[42].Value = DBNull.Value;
            }

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        /*
        public void InsertPmicRowFromSpreadsheet(ProgrammedWorkAction pWA)
        {
            string qry = @"
                                insert into Pmic(Fos_Proj_Id, Pproj_Id, Extg_Strc_Id, Strc_Work_Tycd, Strc_Work_Tydc, WorkActionCode, WorkActionDesc, Estcp_Schd_Dt, Dot_Rgn_Cd, Plnd_Strc_Id)
                                values (@fos_proj_id, @pproj_id, @extg_strc_id, @strc_work_tycd, @strc_work_tydc, @workactioncode, @workactiondesc, @estcp_schd_dt, @dot_rgn_cd, @plnd_strc_id)
                            ";
            SqlParameter[] prms = new SqlParameter[10];
            prms[0] = new SqlParameter("@fos_proj_id", SqlDbType.VarChar);
            prms[0].Value = pWA.FosProjId;
            prms[1] = new SqlParameter("@pproj_id", SqlDbType.Int);
            prms[1].Value = pWA.PProjId;
            prms[2] = new SqlParameter("@extg_strc_id", SqlDbType.VarChar);
            prms[2].Value = pWA.StructureId;
            prms[3] = new SqlParameter("@strc_work_tycd", SqlDbType.VarChar);
            prms[3].Value = pWA.OriginalWorkActionCode;
            prms[4] = new SqlParameter("@strc_work_tydc", SqlDbType.VarChar);
            prms[4].Value = pWA.OriginalWorkActionDesc;
            prms[5] = new SqlParameter("@workactioncode", SqlDbType.VarChar);
            prms[5].Value = pWA.WorkActionCode;
            prms[6] = new SqlParameter("@workactiondesc", SqlDbType.VarChar);
            prms[6].Value = pWA.WorkActionDesc;
            prms[7] = new SqlParameter("@estcp_schd_dt", SqlDbType.Date);
            prms[7].Value = pWA.EstimatedCompletionDate;
            prms[8] = new SqlParameter("@dot_rgn_cd", SqlDbType.VarChar);
            prms[8].Value = pWA.DotRegionCode;
            prms[9] = new SqlParameter("@plnd_strc_id", SqlDbType.VarChar);
            prms[9].Value = pWA.NewStructureId;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }
        */

        public void InsertElementDeteriorationRate(ElementDeterioration elemDeter)
        {
            string qry = @"
                                insert into ElementDeterioration(ElementNumber, RelativeWeight, MedYr1, MedYr2, MedYr3, Beta, Active)
                                values (@elementNumber, @relativeWeight, @medYr1, @medYr2, @medYr3, @beta, @active)
                            ";

            SqlParameter[] prms = new SqlParameter[7];
            prms[0] = new SqlParameter("@elementNumber", SqlDbType.Int);
            prms[0].Value = elemDeter.ElemNum;
            prms[1] = new SqlParameter("@relativeWeight", SqlDbType.Float);
            prms[1].Value = elemDeter.RelativeWeight;
            prms[2] = new SqlParameter("@medYr1", SqlDbType.Float);
            prms[2].Value = elemDeter.MedYr1;
            prms[3] = new SqlParameter("@medYr2", SqlDbType.Float);
            prms[3].Value = elemDeter.MedYr2;
            prms[4] = new SqlParameter("@medYr3", SqlDbType.Float);
            prms[4].Value = elemDeter.MedYr3;
            prms[5] = new SqlParameter("@beta", SqlDbType.Float);
            prms[5].Value = elemDeter.Beta;
            prms[6] = new SqlParameter("@active", SqlDbType.Bit);
            prms[6].Value = elemDeter.Active;

            ExecuteInsertUpdateDelete(qry, prms, samConn);
        }

        public List<Structure> GetLocalStructuresFundingEligible(bool interpolateNbi = false, bool countTpo = false, bool getLastInspection = false)
        {
            List<Structure> strs = new List<Structure>();
            string qry = @"
                                select strc.strc_id, brdg.brdg_sfcy_rtg
                                from dot1stro.dt_strc strc, dot1stro.dt_brdg brdg
                                where strc.strc_id = brdg.strc_id
                                    and brdg_sfcy_rtg <= 80
                                    and strc_tycd in ('B','P')
                                    and strc_sscd in ('50', '70')
                                    and strc_ownr_agcy_cd not in ('20','50','51','52','60','70','10','15','16')
                                order by strc.strc_id
                            ";

            DataTable dt = ExecuteSelect(qry, hsiConn);

            if (dt != null)
            {
                int counter = 0;
                int currentYear = DateTime.Now.Year;

                foreach (DataRow dr in dt.Rows)
                {
                    string strId = dr["STRC_ID"].ToString();
                    Structure str = GetStructure(strId, true, interpolateNbi, false, countTpo, getLastInspection: getLastInspection);

                    if (str != null)
                    {
                        if (
                                !string.IsNullOrEmpty(str.LocalFundingEligibility.Trim())
                                && (str.StructurallyDeficient || str.FunctionalObsolete)
                                && (currentYear - str.YearBuiltActual > 10)
                                && (PastWorkEligible(str))
                            )
                        {
                            strs.Add(str);
                        }
                    }
                    else
                    {
                        //str = new Structure(strId);
                        //strs.Add(str);
                    }

                    counter++;
                }
            }

            return strs;
        }

        private bool PastWorkEligible(Structure str)
        {
            bool eligible = true;
            int currentYear = DateTime.Now.Year;
            var works = str.ConstructionHistoryProjects
                            .Where(e => e.WorkActionYear >= currentYear - 10
                                        && (
                                                e.WorkActionCode.Equals("02")
                                                || e.WorkActionCode.Equals("06")
                                                || e.WorkActionCode.Equals("08")
                                                || e.WorkActionCode.Equals("09")
                                                || e.WorkActionCode.Equals("15")
                                                || e.WorkActionCode.Equals("17")
                                                || e.WorkActionCode.Equals("23")
                                                || e.WorkActionCode.Equals("24")
                                                || e.WorkActionCode.Equals("26")
                                                || e.WorkActionCode.Equals("27")
                                                || e.WorkActionCode.Equals("36")
                                                || e.WorkActionCode.Equals("40")
                                                || e.WorkActionCode.Equals("44")
                                                || e.WorkActionCode.Equals("46")
                                                || e.WorkActionCode.Equals("51")
                                                || e.WorkActionCode.Equals("56")
                                                || e.WorkActionCode.Equals("62")
                                                || e.WorkActionCode.Equals("68")
                                            )

                                    );
            if (works.Count() > 0)
            {
                eligible = false;
            }

            return eligible;
        }

        public List<string> GetStructuresAll()
        {
            List<string> strIds = new List<string>();
            string qry = @"
                                select strc_id
                                from dot1stro.dt_strc
                                where strc_tycd in ('B','P', 'C')
                                    and strc_sscd = '50'
                                    and strc_ownr_agcy_cd in ('10','15','16','20','44','45','30','31','32','40','41','42','46','47','70','80','90')
                                order by strc_id
                            ";

            DataTable dt = ExecuteSelect(qry, hsiConn);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strIds.Add(dr["STRC_ID"].ToString());
                }
            }

            return strIds;
        }

        public List<string> GetStructuresByFundingSources(NeedsAnalysisInput needsAnalysisInput)
        {
            List<string> strIds = new List<string>();
            foreach (var fundingSource in needsAnalysisInput.FundingSources)
            {
                strIds.AddRange(GetStructuresByFundingSource(fundingSource, needsAnalysisInput.IncludeCStructures, needsAnalysisInput));
            }

            return strIds;
        }

        public List<string> GetStructuresByFundingSource(WisamType.FundingSources fundingSource, bool includeCStructures, NeedsAnalysisInput needsAnalysisInput)
        {
            List<string> strIds = new List<string>();
            string qry = @"
                            select strc_id
                            from dot1stro.dt_strc
                            where strc_sscd = '50'
                        ";

            if (needsAnalysisInput.ApplyRegionSelectionsWhenByFunding)
            {
                qry += @" and dot_rgn_nb in (0";

                foreach (var regionNumber in needsAnalysisInput.RegionNumbers)
                {
                    qry += "," + regionNumber;
                }

                qry += ") ";
            }

            switch (fundingSource)
            {
                case WisamType.FundingSources.Backbone:
                    qry += @" and (
                                strc_ownr_agcy_cd in ('10','15','16','20','44','45') 
                                or stmc_agcy_ty in ('10','15','16','20','44','45') 
                                or stim_agcy_tycd in ('10','15','16','20','44','45')
                              )";
                    qry += @" and strc_plnd_pgm = 'BB'";
                    break;

                case WisamType.FundingSources.SHR3R:
                    qry += @" and (
                                strc_ownr_agcy_cd in ('10','15','16','20','44','45') 
                                or stmc_agcy_ty in ('10','15','16','20','44','45') 
                                or stim_agcy_tycd in ('10','15','16','20','44','45')
                              )";
                    qry += @" and (strc_plnd_pgm in ('CM','C2','NO') or strc_plnd_pgm is null)";
                    break;
            }

            if (includeCStructures)
            {
                qry += @" and strc_tycd in ('B','P', 'C')";
            }
            else
            {
                qry += @" and strc_tycd in ('B','P')";
            }

            qry += " order by strc_id";

            DataTable dt = ExecuteSelect(qry, hsiConn);

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
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strIds.Add(dr["STRC_ID"].ToString());
                }
            }

            return strIds;
        }

        public List<string> GetStructuresByRegion(string region)
        {
            List<string> strIds = new List<string>();
            string qry = @"
                                select strc_id
                                from dot1stro.dt_strc
                                where dot_rgn_nb = :region
                                    and strc_tycd in ('B','P')
                                    and strc_sscd = '50'
                                order by strc_id
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("region", OracleDbType.Varchar2);
            prms[0].Value = region;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    strIds.Add(dr["STRC_ID"].ToString());
                }
            }

            return strIds;
        }

        public List<PriorityFactor> GetPriorityFactors()
        {
            List<PriorityFactor> pfs = new List<PriorityFactor>();
            string qry = @"
                                select FactorCode, FactorName, FactorDesc, FactorWeight, Active
                                from PriorityFactor
                                where Active = 1
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PriorityFactor pf = new PriorityFactor();
                    pf.FactorCode = dr["FACTORCODE"].ToString();
                    pf.FactorName = dr["FACTORNAME"].ToString();
                    pf.FactorDesc = dr["FACTORDESC"].ToString();
                    pf.FactorWeight = Convert.ToInt32(dr["FACTORWEIGHT"]);
                    pf.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    pfs.Add(pf);
                }
            }

            return pfs;
        }

        public List<MeasurementIndex> GetMeasurementIndices()
        {
            List<MeasurementIndex> mis = new List<MeasurementIndex>();
            string qry = @"
                                select MeasurementCode, GrossValue, GrossExpression, IndexValue, IndexExpression
                                from MeasurementIndex
                            ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    MeasurementIndex mi = new MeasurementIndex();
                    mi.MeasurementCode = dr["MEASUREMENTCODE"].ToString();
                    mi.GrossValue = dr["GROSSVALUE"].ToString();
                    mi.GrossExpression = Convert.ToBoolean(dr["GROSSEXPRESSION"]);
                    mi.IndexValue = dr["INDEXVALUE"].ToString();
                    mi.IndexExpression = Convert.ToBoolean(dr["INDEXEXPRESSION"]);
                    mis.Add(mi);
                }
            }

            return mis;
        }

        public List<PriorityFactorMeasurement> GetPriorityFactorMeasurements(List<PriorityFactor> pfs)
        {
            List<PriorityFactorMeasurement> pfms = new List<PriorityFactorMeasurement>();

            foreach (var pf in pfs)
            {
                pfms.AddRange(GetPriorityFactorMeasurements(pf.FactorCode));
            }

            return pfms;
        }

        public List<PriorityFactorMeasurement> GetPriorityFactorMeasurements(string factorCode)
        {
            List<PriorityFactorMeasurement> pfms = new List<PriorityFactorMeasurement>();
            string qry = @"
                                select pfm.FactorCode, pf.FactorWeight, pfm.MeasurementCode, Weight, pfm.Active,
                                    m.MeasurementName, m.ValueFormula, m.CalcIndexValue
                                from PriorityFactor pf, PriorityFactorMeasurement pfm, Measurement m
                                where pfm.FactorCode = @factorCode
                                    and pf.FactorCode = pfm.FactorCode
                                    and pfm.MeasurementCode = m.MeasurementCode
                                    and pfm.Active = 1
                                    and m.Active = 1
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@factorCode", SqlDbType.VarChar);
            prms[0].Value = factorCode;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PriorityFactorMeasurement pfm = new PriorityFactorMeasurement(factorCode);
                    pfm.MeasurementCode = dr["MEASUREMENTCODE"].ToString();
                    pfm.Weight = Convert.ToDouble(dr["WEIGHT"]);
                    pfm.Active = Convert.ToBoolean(dr["ACTIVE"]);
                    pfm.MeasurementName = dr["MEASUREMENTNAME"].ToString();
                    pfm.GrossValueFormula = dr["VALUEFORMULA"].ToString();
                    pfm.CalcIndexValue = Convert.ToBoolean(dr["CALCINDEXVALUE"]);
                    pfm.FactorWeight = Convert.ToDouble(dr["FACTORWEIGHT"]);
                    //pfm.IndexFormula = dr["INDEXFORMULA"].ToString();
                    pfms.Add(pfm);
                }
            }

            return pfms;
        }

        public Risk GetRisk(WisamType.Risks riskId)
        {
            Risk risk = null;
            string qry = @"
                                select RiskId, RiskName, RiskDesc, RiskMaxValue, RiskNotes
                                from Risk
                                where RiskId = @riskId
                                    and Active = 1
                            ";
            SqlParameter[] prms = new SqlParameter[1];
            prms[0] = new SqlParameter("@riskId", SqlDbType.Int);
            prms[0].Value = (int)riskId;
            DataTable dt = ExecuteSelect(qry, prms, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                risk = new Risk(riskId);
                risk.RiskName = dr["RISKNAME"].ToString();
                risk.RiskDesc = dr["RISKDESC"].ToString();
                risk.RiskMaxValue = Convert.ToInt32(dr["RISKMAXVALUE"]);
                risk.RiskNotes = dr["RISKNOTES"].ToString();
                risk.RiskValue = 0;
                risk.Active = true;
            }

            return risk;
        }

        public Risk GetRisk(string strId, WisamType.Risks riskId)
        {
            Risk risk = GetRisk(riskId);

            if (risk != null)
            {
                string qry = "";
                switch (riskId)
                {
                    case WisamType.Risks.Nhs:
                        qry = @"
                                select ftr_nm
                                from dot1stro.dt_strc_rte r
                                where r.strc_id = :strId
                                    and r.strc_rte_prmy_fl = 'Y'
                                    and r.strc_rte_ouf = 'O'
                                    and r.hwy_on_inv_rte in ('NHS','NHS')
                                ";
                        break;
                    case WisamType.Risks.Strahnet:
                        qry = @"
                                select ftr_nm
                                from dot1stro.dt_strc_rte r
                                where r.strc_id = :strId
                                    and r.strc_rte_prmy_fl = 'Y'
                                    and r.strc_rte_ouf = 'O'
                                    and r.stgc_hwy_ntwk_dsgt in ('1','2')
                                ";
                        break;
                    case WisamType.Risks.Sccr:
                        qry = @"
                                select brdg_scur_crtc
                                from dot1stro.dt_brdg
                                where strc_id = :strId
                                    and brdg_scur_crtc in ('0','1','2','3')
                                ";
                        break;
                    case WisamType.Risks.Frcr:
                        qry = @"
                                select strc_id
                                from dot1stro.dt_stin_instc
                                where strc_id = :strId
                                    and stin_tycd = 'FC'
                                ";
                        break;
                    case WisamType.Risks.LoadCap:
                        qry = @"
                                select strc_id
                                from dot1stro.dt_brdg
                                where strc_id = :strId
                                    and dot1stro.dt_brdg.bgar_load_cpty_cd in ('0','1','2','3','4')
                                ";
                        break;
                    case WisamType.Risks.Mvw:
                        qry = @"
                                select brdg_max_veh_wt
                                from dot1stro.dt_brdg
                                where strc_id = :strId
                                ";
                        break;
                    case WisamType.Risks.VertClr:
                        qry = @"
                                select clrn_minm_vert_dis
                                from dot1stro.dt_strc_clrn
                                where strc_id = :strId
                                ";
                        break;
                    case WisamType.Risks.Adt:
                    case WisamType.Risks.Adtt:
                        qry = @"
                                select brdg_adt_cnt, brdg_adt_trk
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'O'
                                ";
                        break;
                    case WisamType.Risks.DetLength:
                        qry = @"
                                select brdg_dtr_ln_on
                                from dot1stro.dt_brdg
                                where strc_id = :strId
                                ";
                        break;
                }

                if (!qry.Equals(String.Empty))
                {
                    OracleParameter[] prms = new OracleParameter[1];
                    prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                    prms[0].Value = strId;
                    DataTable dt = ExecuteSelect(qry, prms, hsiConn);

                    switch (riskId)
                    {
                        case WisamType.Risks.Nhs:
                        case WisamType.Risks.Strahnet:
                        case WisamType.Risks.Sccr:
                        case WisamType.Risks.Frcr:
                        case WisamType.Risks.LoadCap:
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                risk.RiskValue = risk.RiskMaxValue;
                            }
                            break;
                        case WisamType.Risks.Mvw:
                            try
                            {
                                int maxVehicleWeight = Convert.ToInt32(dt.Rows[0]["BRDG_MAX_VEH_WT"]);
                                if (maxVehicleWeight < 120)
                                {
                                    risk.RiskValue = 5;
                                }
                                else if (maxVehicleWeight < 170)
                                {
                                    risk.RiskValue = 1;
                                }
                            }
                            catch { }
                            break;
                        case WisamType.Risks.Adt:
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                try
                                {
                                    int adt = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_CNT"]);
                                    risk.RiskValue = (adt / 118000) * risk.RiskMaxValue;

                                    if (risk.RiskValue > risk.RiskMaxValue)
                                    {
                                        risk.RiskValue = risk.RiskMaxValue;
                                    }
                                }
                                catch { }
                            }
                            break;
                        case WisamType.Risks.Adtt:
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                try
                                {
                                    int adt = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_CNT"]);
                                    int adtt = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_TRK"]);
                                    risk.RiskValue = (adt * (adtt / 100) / 17700) * risk.RiskMaxValue;

                                    if (risk.RiskValue > risk.RiskMaxValue)
                                    {
                                        risk.RiskValue = risk.RiskMaxValue;
                                    }
                                }
                                catch { }
                            }
                            break;
                        case WisamType.Risks.VertClr:
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                try
                                {
                                    float verticalClearance = Convert.ToSingle(dt.Rows[0]["CLRN_MINM_VERT_DIS"]);
                                    if (verticalClearance > 16.25)
                                    {
                                        risk.RiskValue = 0;
                                    }
                                    else if (verticalClearance > 13.99 && verticalClearance < 16.26)
                                    {
                                        risk.RiskValue = 1;
                                    }
                                    else if (verticalClearance > 12 && verticalClearance < 14)
                                    {
                                        risk.RiskValue = 2;
                                    }
                                    else if (verticalClearance < 11.99)
                                    {
                                        risk.RiskValue = 5;
                                    }
                                }
                                catch { }
                            }
                            break;
                        case WisamType.Risks.DetLength:
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                try
                                {
                                    float detourLength = Convert.ToSingle(dt.Rows[0]["BRDG_DTR_LN_ON"]);
                                    risk.RiskValue = (detourLength / 99) * risk.RiskMaxValue;

                                    if (risk.RiskValue > risk.RiskMaxValue)
                                    {
                                        risk.RiskValue = risk.RiskMaxValue;
                                    }
                                }
                                catch { }
                            }
                            break;
                    }
                }
            }

            return risk;
        }

        /*
        private DataTable GetLastInspection(string strId)
        {
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
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);
            return dt;
        }
        */

        /*
        
        */

        public void GetStnInventoryInfo(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select distinct invbrlc.rwlk_id
                                from dotsys.dt_inv_brlc_pitc invbrlc
                                where trim(brdg_id) = :strId
                            ";

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {

            }
        }

        public void GetPrimaryHighwayFreightSystemInfo(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select structureid
                                from primaryhighwayfreightsystembridge
                                where structureid = @strId
                            ";

            SqlParameter[] prm = new SqlParameter[1];
            prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prm[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prm, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                str.PrimaryHighwayFreightSystemBridge = true;
            }
        }

        public void GetBridgeMunicipalPlanningInfo(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select structureid, municipalplanningagency
                                from municipalplanningbridge
                                where structureid = @strId
                            ";

            SqlParameter[] prm = new SqlParameter[1];
            prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prm[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prm, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                str.MunicipalPlanningBridge = true;
                str.MunicipalPlanningAgency = dr["municipalplanningagency"].ToString();
            }
        }

        public void GetBorderBridgeInfo(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select structureid, statename
                                from borderbridge, borderstate
                                where structureid = @strId
                                    and borderbridge.borderstatecode = borderstate.statecode
                            ";

            SqlParameter[] prm = new SqlParameter[1];
            prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prm[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prm, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                str.BorderBridge = true;
                str.BorderState = dr["statename"].ToString();
            }
        }

        public void GetGisInfo(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select *
                                from StructureGis
                                where rtrim(ltrim(structurei)) = @strId
                            ";
            SqlParameter[] prm = new SqlParameter[1];
            prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
            prm[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prm, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                if (dr["inv500bffr"].ToString().Trim().ToUpper().Equals("Y"))
                {
                    str.GisStateBridgeCoordinatesWithin500FtHsisCoordinates = true;
                }
                else
                {
                    str.GisStateBridgeCoordinatesWithin500FtHsisCoordinates = false;
                }

                if (dr["wislr500bf"].ToString().Trim().ToUpper().Equals("Y"))
                {
                    str.GisWislrLocalBridgeCoordinatesWithin500FtHsisCoordinates = true;
                }
                else
                {
                    str.GisWislrLocalBridgeCoordinatesWithin500FtHsisCoordinates = false;
                }

                str.GisCorridor2030Code = dr["c2030"].ToString().Trim();
                str.GisDividedHighwayCode = dr["dhwy_cd"].ToString().Trim();
                str.GisFunctionalClassCode = dr["ffcl_cd"].ToString().Trim();
                str.GisFunctionalClassAbbreviation = dr["ffcl_abbr_"].ToString().Trim();
                str.GisFunctionalClassDescription = dr["ffcl_desc"].ToString().Trim();
                str.GisNhsDesignation = dr["nhs_dsgt"].ToString().Trim();
                str.GisProjectRouteType = dr["pproj_rte_"].ToString().Trim();
                str.GisProjectRouteName = dr["pproj_rt_1"].ToString().Trim();

                str.GisLongTruckRouteDesignation = dr["ltrk_dsgt"].ToString().Trim();
                str.GisMaintenanceJurisdictionCode = dr["mjrdn_cd"].ToString().Trim();
                str.GisMaintenanceJurisdictionRouteType = dr["type"].ToString().Trim();
                str.GisMaintenanceJurisdictionRouteName = dr["name"].ToString().Trim();

                if (dr["osow_hi_cl"].ToString().Trim().ToUpper().Equals('Y'))
                {
                    str.GisOsowHighClearanceRoute = true;
                }
                else
                {
                    str.GisOsowHighClearanceRoute = false;
                }

                str.GisOsowRouteType = dr["type_1"].ToString().Trim();
                str.GisOsowRouteName = dr["name_1"].ToString().Trim();
                str.GisOsowRankingNumber = dr["osow_rnkg"].ToString().Trim();
                str.GisOsowRankingName = dr["osow_rnkg_"].ToString().Trim();
            }
        }

        public void GetTrafficInfo(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select bridge.brdg_id, 
                                        bridge.mmgr_seg_id,
                                        mmgr.aadtf_1_yr_out,
                                        mmgr.aadtf_5_yrs_out,
                                        mmgr.aadtf_10_yrs_out,
                                        mmgr.aadtf_15_yrs_out,
                                        mmgr.aadtf_20_yrs_out,
                                        mmgr.pcnt_trk_dht_1_yr,
                                        mmgr.rdwy_pstd_spd,
                                        mmgr.crdr_2020_id,
                                        mmgr.ffcl_cd,
                                        mmgr.dhwy_cd,
                                        mmgr.mmgr_trfc_seg_id
                                from dot1hamo.dt_hacs_mmgr_brdg bridge, dot1hamo.dt_hacs_mmgr mmgr
                                where trim(bridge.brdg_id) = :strId
                                    and bridge.mmgr_seg_id = mmgr.mmgr_seg_id
                            ";

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                try
                {
                    str.MmRoadwaySegmentId = Convert.ToInt32(dr["mmgr_seg_id"]);
                }
                catch { }

                try
                {
                    str.MmForecastedAadtYear1 = Convert.ToInt32(dr["aadtf_1_yr_out"]);
                }
                catch { }

                try
                {
                    str.MmForecastedAadtYear5 = Convert.ToInt32(dr["aadtf_5_yrs_out"]);
                }
                catch { }

                try
                {
                    str.MmForecastedAadtYear10 = Convert.ToInt32(dr["aadtf_10_yrs_out"]);
                }
                catch { }

                try
                {
                    str.MmForecastedAadtYear15 = Convert.ToInt32(dr["aadtf_15_yrs_out"]);
                }
                catch { }

                try
                {
                    str.MmForecastedAadtYear20 = Convert.ToInt32(dr["aadtf_20_yrs_out"]);
                }
                catch { }

                try
                {
                    str.MmForecastedTruckPercentageAadtYear1 = Convert.ToSingle(dr["pcnt_trk_dht_1_yr"]);
                }
                catch { }

                try
                {
                    str.MmRoadwayPostedSpeedLimit = Convert.ToInt32(dr["rdwy_pstd_spd"]);
                }
                catch { }

                str.MmCorridorsCode2030 = dr["crdr_2020_id"].ToString().Trim();
                str.MmFunctionalClassificationOn = dr["ffcl_cd"].ToString().Trim();
                str.MmDividedHighwayCode = dr["dhwy_cd"].ToString().Trim();

                try
                {
                    str.MmTrafficSegmentId = Convert.ToInt32(dr["mmgr_trfc_seg_id"]);
                }
                catch { }
            }
        }

        public List<string> GetQualifiedDeteriorationCurves()
        {
            List<string> qualifiedCurves = new List<string>();
            string qry =
                @"
                    select *
                    from nbiqualifieddeteriorationcurve
                    order by qualifiedcode
                ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    qualifiedCurves.Add(dr["qualifiedcode"].ToString());
                }
            }

            return qualifiedCurves;
        }

        public StructureDeterioration GetStructureForDeterioration(string structureId)
        {
            StructureDeterioration str = new StructureDeterioration();
            str.StructureId = structureId;
            string qry = @"
                                select struc.strc_id, 
                                        funcClassOn.brdg_func_cls_desc as fcOn,
                                        spanconfig.span_cnfg_tydc,
                                        spanmaterial.span_matl_desc,
                                        routeunder.featureUnder,
                                        desig.hwy_ftr_dsgt_desc
                                from dot1stro.dt_strc struc, 
                                        dot1stro.dt_brdg bridge,
                                        dot1stro.dt_brdg_span mainspan,
                                        dot1stro.dt_brdg_span_cnfg spanconfig,
                                        dot1stro.dt_brdg_span_matl spanmaterial,
                                        (
                                            select strc_id, hwy_ftr_dsgt_cd, strc_rte_sys as rsUnder, ftr_nm as featureUnder
                                            from dot1stro.dt_strc_rte
                                            where strc_rte_ouf = 'U'
                                                and strc_rte_prmy_fl = 'Y'
                                        ) routeunder,
                                        (
                                            select brdg_func_cls_cd, brdg_func_cls_desc
                                            from dot1stro.dt_brdg_func_cls
                                        ) funcClassOn,
                                        dot1stro.dt_hwy_ftr_dsgt desig
                                where struc.strc_id = :strId
                                    and struc.strc_sscd = '50'
                                    and struc.strc_id = bridge.strc_id
                                    and bridge.strc_id = mainspan.strc_id
                                    and mainspan.main_span_fl = 'Y'
                                    and mainspan.bspn_cnfg_ty = spanconfig.bspn_cnfg_ty(+)
                                    and mainspan.bspn_matl_cd = spanmaterial.bspn_matl_cd(+)
                                    and struc.strc_id = routeunder.strc_id(+)
                                    and routeunder.hwy_ftr_dsgt_cd = desig.hwy_ftr_dsgt_cd
                                    and bridge.func_cls_on_cd = funcClassOn.brdg_func_cls_cd(+)
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = structureId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                str.FeatureUnder = dt.Rows[0]["hwy_ftr_dsgt_desc"].ToString();
                str.StructureType = dt.Rows[0]["SPAN_CNFG_TYDC"].ToString();
                str.MainSpanMaterial = dt.Rows[0]["SPAN_MATL_DESC"].ToString();

                try
                {
                    str.FunctionalClassificationOn = dt.Rows[0]["FCON"].ToString().ToUpper();
                }
                catch { }

                GetQualifiedDeteriorationCurves(str);
            }

            return str;
        }

        public void GetQualifiedDeteriorationCurves(StructureDeterioration str)
        {
            string qry =
                @"
                    select *
                    from nbiqualifieddeteriorationcurve
                    order by qualifiedcode
                ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var originalExpression = dr["qualificationexpression"].ToString();
                    var modifiedExpression =
                        originalExpression.Replace("func", String.Format("'{0}'", str.FunctionalClassificationOn))
                            .Replace("material", String.Format("'{0}'", str.MainSpanMaterial))
                            .Replace("config", String.Format("'{0}'", str.StructureType))
                            .Replace("featunder", String.Format("'{0}'", str.FeatureUnder));

                    bool isExpressionTrue = false;
                    var classificationCode = dr["classificationcode"].ToString();

                    try
                    {
                        isExpressionTrue = Convert.ToBoolean(new DataTable().Compute(modifiedExpression, null));
                    }
                    catch
                    { }

                    if (isExpressionTrue)
                    {
                        var qualifiedCode = dr["qualifiedcode"].ToString();

                        if (classificationCode.Equals("NDEC"))
                        {
                            str.NbiDeckQualifiedDeteriorationCurve = qualifiedCode;
                        }
                        else if (classificationCode.Equals("NSUP"))
                        {
                            str.NbiSuperQualifiedDeteriorationCurve = qualifiedCode;
                        }
                        else if (classificationCode.Equals("NSUB"))
                        {
                            str.NbiSubQualifiedDeteriorationCurve = qualifiedCode;
                        }
                    }
                }
            }
        }

        public void GetSpecialComponents(Structure str)
        {
            string strId = str.StructureId;
            string qry = @"
                                select special.strc_id, special.spcl_cpnt_tycd
                                from dot1stro.dt_strc_spcl_cpnt special
                                where strc_id = :strId
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                int count = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string component = dr["spcl_cpnt_tycd"].ToString();
                    if (count == 0)
                    {
                        str.SpecialComponents = component;
                    }
                    else
                    {
                        str.SpecialComponents += String.Format(", {0}", component);
                    }
                    count++;
                }
            }
            else
            {
                str.SpecialComponents = "-1";
            }
        }
        public void GetSpanInfo(Structure str)
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
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

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

        public Element GetElement(int ElemNum)
        {
            Element elem = null;
            return elem;
        }

        public List<int> GetDefects(int elemNum)
        {
            List<int> defects = new List<int>();
            string qry = @"
                                select prmy_isel_id, scdy_isel_id
                                from dot1stro.dt_elmt_rlsp rel, dot1stro.dt_isel_clsn cla
                                where rel.scdy_isel_id = cla.isel_tyid
                                    and cla.isel_clsn_id = 'DFCT'
                                    and rel.prmy_isel_id = :elemNum
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("elemNum", OracleDbType.Int32);
            prms[0].Value = elemNum;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int defectNum = Convert.ToInt32(dr["SCDY_ISEL_ID"]);
                    defects.Add(defectNum);
                }
            }

            return defects;
        }

        public int GetDwIndexValueCollectorOrLocalRoad(Structure str)
        {
            int indexValue = 0;

            if (str.Adt < 1500 && str.MinDeckWidth < 28)
            {
                indexValue = 1;
            }
            else if (str.Adt >= 1500 && str.Adt <= 3500 && str.MinDeckWidth < 32)
            {
                indexValue = 1;
            }
            else if (str.Adt > 3500 && str.MinDeckWidth < 40)
            {
                indexValue = 1;
            }

            return indexValue;
        }

        public int GetDwIndexValueArterial(Structure str)
        {
            int indexValue = 0;

            if (str.Adt < 3500 && str.MinDeckWidth < 36)
            {
                indexValue = 1;
            }
            else if (str.Adt >= 3500 && str.Adt < 15000 && str.MinDeckWidth < 44)
            {
                indexValue = 1;
            }
            else if (str.Adt >= 15000 && str.Adt <= 60000)
            {
                if (str.trafficPatternOn == 1 && str.MinDeckWidth < 40)
                {
                    indexValue = 1;
                }
                else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 80)
                {
                    indexValue = 1;
                }
            }
            else if (str.Adt > 60000)
            {
                if (str.trafficPatternOn == 1 && str.MinDeckWidth < 56)
                {
                    indexValue = 1;
                }
                else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 112)
                {
                    indexValue = 1;
                }
            }

            return indexValue;
        }

        public int GetDwIndexValueUrban(Structure str)
        {
            int indexValue = 0;

            switch (str.LanesOn)
            {
                case 1:
                    if (str.MinDeckWidth < 18)
                    {
                        indexValue = 1;
                    }

                    break;

                case 2:
                    if (str.Adt >= 0 && str.Adt <= 100 && str.MinDeckWidth < 18)
                    {
                        indexValue = 1;
                    }
                    else if (str.Adt >= 101 && str.Adt <= 400 && str.MinDeckWidth < 20)
                    {
                        indexValue = 1;
                    }
                    else if (str.Adt >= 401 && str.Adt <= 1000 && str.MinDeckWidth < 22)
                    {
                        indexValue = 1;
                    }
                    else if (str.Adt >= 1001 && str.Adt <= 2000 && str.MinDeckWidth < 24)
                    {
                        indexValue = 1;
                    }
                    else if (str.Adt >= 2001 && str.MinDeckWidth < 28)
                    {
                        indexValue = 1;
                    }
                    break;

                case 3:
                    if (str.MinDeckWidth < 40)
                    {
                        indexValue = 1;
                    }

                    break;

                case 4:
                    if (str.MinDeckWidth < 51)
                    {
                        indexValue = 1;
                    }

                    break;

                case 5:
                    if (str.MinDeckWidth < 62)
                    {
                        indexValue = 1;
                    }

                    break;

                default:
                    if (str.MinDeckWidth < 80)
                    {
                        indexValue = 1;
                    }

                    break;
            }

            return indexValue;
        }

        public int GetDwIndexValueInterstate(Structure str)
        {
            int indexValue = 0;

            switch (str.LanesOn)
            {
                case 1:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 26)
                    {
                        indexValue = 1;
                    }

                    break;

                case 2:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 38)
                    {
                        indexValue = 1;
                    }
                    else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 52)
                    {
                        indexValue = 1;
                    }

                    break;

                case 3:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 56)
                    {
                        indexValue = 1;
                    }
                    else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 64)
                    {
                        indexValue = 1;
                    }

                    break;

                case 4:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 68)
                    {
                        indexValue = 1;
                    }
                    else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 76)
                    {
                        indexValue = 1;
                    }

                    break;

                case 5:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 80)
                    {
                        indexValue = 1;
                    }
                    else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 94)
                    {
                        indexValue = 1;
                    }

                    break;

                case 6:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 92)
                    {
                        indexValue = 1;
                    }
                    else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 112)
                    {
                        indexValue = 1;
                    }

                    break;

                case 7:
                    if (str.trafficPatternOn == 1 && str.MinDeckWidth < 104)
                    {
                        indexValue = 1;
                    }
                    else if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 124)
                    {
                        indexValue = 1;
                    }

                    break;

                case 8:
                    if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 136)
                    {
                        indexValue = 1;
                    }

                    break;

                case 9:
                    if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 148)
                    {
                        indexValue = 1;
                    }

                    break;

                case 10:
                    if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 160)
                    {
                        indexValue = 1;
                    }

                    break;

                case 11:
                    if (str.trafficPatternOn >= 2 && str.MinDeckWidth < 172)
                    {
                        indexValue = 1;
                    }

                    break;
            }

            return indexValue;
        }

        public List<PriorityIndexFactor> GetPriorityIndexFactors()
        {
            List<PriorityIndexFactor> piFactors = new List<PriorityIndexFactor>();
            string qry = @"
                            select pif.*, pic.PriorityIndexCategoryName
                            from PriorityIndexFactor pif, PriorityIndexCategory pic
                            where pif.IsCurrentRow = 1
                                and pic.IsCurrentRow = 1
                                and pif.PriorityIndexCategoryKey = pic.PriorityIndexCategoryKey
                            order by PriorityIndexFactorId;
                        ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PriorityIndexFactor piFactor = new PriorityIndexFactor();
                    piFactor.PriorityIndexFactorKey = Convert.ToInt32(dr["PriorityIndexFactorKey"]);
                    piFactor.PriorityIndexCategoryKey = Convert.ToInt32(dr["PriorityIndexCategoryKey"]);
                    piFactor.PriorityIndexFactorId = dr["PriorityIndexFactorId"].ToString().Trim();
                    piFactor.PriorityIndexCategoryName = dr["PriorityIndexCategoryName"].ToString().Trim();
                    piFactor.PriorityIndexFactorDesc = dr["PriorityIndexFactorDesc"].ToString().Trim();
                    piFactor.PriorityIndexFactorWeight = Convert.ToSingle(dr["PriorityIndexFactorWeight"]);
                    piFactor.PriorityIndexFactorType = dr["PriorityIndexFactorType"].ToString().Trim();
                    piFactor.IndexValueFormula = dr["IndexValueFormula"].ToString().Trim();
                    piFactor.IsCurrentRow = Convert.ToBoolean(dr["IsCurrentRow"]);
                    piFactor.RowEffectiveDate = Convert.ToDateTime(dr["RowEffectiveDate"]);
                    piFactor.RowExpirationDate = Convert.ToDateTime(dr["RowExpirationDate"]);
                    piFactor.RowLastUpdateDate = Convert.ToDateTime(dr["RowLastUpdateDate"]);
                    piFactor.Notes = dr["Notes"].ToString().Trim();
                    piFactors.Add(piFactor);
                }
            }

            return piFactors;
        }

        public List<PriorityIndexCategory> GetPriorityIndexCategories()
        {
            List<PriorityIndexCategory> piCats = new List<PriorityIndexCategory>();
            string qry = @"
                            select *
                            from PriorityIndexCategory
                            where IsCurrentRow = 1
                            order by PriorityIndexCategoryName
                        ";
            DataTable dt = ExecuteSelect(qry, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PriorityIndexCategory piCat = new PriorityIndexCategory();
                    piCat.PriorityIndexCategoryKey = Convert.ToInt32(dr["PriorityIndexCategoryKey"]);
                    piCat.PriorityIndexCategoryName = dr["PriorityIndexCategoryName"].ToString().Trim();
                    piCat.PriorityIndexCategoryDesc = dr["PriorityIndexCategoryDesc"].ToString().Trim();
                    piCat.PriorityIndexMaxValue = Convert.ToSingle(dr["PriorityIndexMaxValue"]);
                    piCat.IsCurrentRow = Convert.ToBoolean(dr["IsCurrentRow"]);
                    piCat.RowEffectiveDate = Convert.ToDateTime(dr["RowEffectiveDate"]);
                    piCat.RowExpirationDate = Convert.ToDateTime(dr["RowExpirationDate"]);
                    piCat.RowLastUpdateDate = Convert.ToDateTime(dr["RowLastUpdateDate"]);
                    piCat.Notes = dr["Notes"].ToString().Trim();
                    piCats.Add(piCat);
                }
            }

            return piCats;
        }

        public float LookupFactorIndexValue(string priorityIndexFactorId, string fieldValue)
        {
            float indexValue = 0;
            string qry = @"
                            select IndexValue
                            from PriorityIndexFactorIndexValue
                            where IsActive = 1
                                and UPPER(PriorityIndexFactorId) = @priorityIndexFactorId
                                and UPPER(FieldValue) = @fieldValue
                        ";

            SqlParameter[] prm = new SqlParameter[2];
            prm[0] = new SqlParameter("@priorityIndexFactorId", SqlDbType.VarChar);
            prm[0].Value = priorityIndexFactorId.ToUpper();
            prm[1] = new SqlParameter("@fieldValue", SqlDbType.VarChar);
            prm[1].Value = fieldValue.ToUpper();
            DataTable dt = ExecuteSelect(qry, prm, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                indexValue = Convert.ToSingle(dr["IndexValue"]);
            }

            return indexValue;
        }

        public void CalculatePriorityScoreFactorsCategories(Structure str, StructureWorkAction swa, int startYear, int endYear)
        {
            for (int year = startYear; year <= endYear; year++)
            {
                foreach (var priorityIndexCategory in GetPriorityIndexCategories())
                {
                    List<PriorityIndexFactor> piFactors = GetPriorityIndexFactors().Where(e => e.PriorityIndexCategoryKey == priorityIndexCategory.PriorityIndexCategoryKey).ToList();
                    float piCategoryScore = 0;

                    foreach (var piFactor in piFactors)
                    {
                        string ivf = piFactor.IndexValueFormula.Trim();
                        string factorId = piFactor.PriorityIndexFactorId;
                        object fieldObj = null;
                        float indexValue = 0;

                        try
                        {
                            fieldObj = str.GetType().GetProperty(factorId).GetValue(str, null);
                        }
                        catch { }

                        // Use a formula to calculate factor's index value
                        if (!String.IsNullOrEmpty(ivf))
                        {
                            if (ivf.StartsWith("="))
                            {
                                // Handle special cases that require hardcoding
                                switch (factorId)
                                {
                                    case "MaxVehicleWeight":
                                        // =MIN(MAX(-0.011*SPV+1.89,0),1)
                                        double mvwIndexValue = Math.Min(Math.Max(-0.011 * Convert.ToSingle(fieldObj.ToString()) + 1.89, 0), 1);
                                        ivf = mvwIndexValue.ToString();
                                        break;

                                    case "LoadPostingTonnage":
                                        //= IFERROR(IF(POSTING = -1, 0, MIN(5 / POSTING + 0.5, 1)), 0)
                                        if (Convert.ToSingle(fieldObj.ToString()) <= 0)
                                        {
                                            ivf = "0";
                                        }
                                        else
                                        {
                                            double lptIndexValue = Math.Min(5 / Convert.ToSingle(fieldObj.ToString()) + 0.5, 1);
                                            ivf = lptIndexValue.ToString();
                                        }
                                        break;

                                    default:
                                        if (ivf.Contains(String.Format("LN({0})", factorId)))
                                        {
                                            double natLogVal = 0;

                                            try
                                            {
                                                natLogVal = Math.Log(Convert.ToDouble(fieldObj.ToString()), Math.E);
                                            }
                                            catch { }

                                            ivf = ivf.Replace(String.Format("LN({0})", factorId), natLogVal.ToString());
                                        }

                                        // TODO: Use regular expression to make it more extensible
                                        // Example: MIN(DetLen/15.0,1), MAX(num1,num2)
                                        int countMinMax = Regex.Matches(ivf, @"(MIN|MAX)").Count;

                                        if (countMinMax == 1)
                                        {
                                            Match minOrMaxMatch = Regex.Match(ivf, @"(MIN|MAX).*\(.+,.+\)", RegexOptions.IgnoreCase);

                                            if (minOrMaxMatch.Success)
                                            {
                                                Match num1 = Regex.Match(minOrMaxMatch.Value, @"(?<=\()(.*?)(?=,)", RegexOptions.IgnoreCase);
                                                Match num2 = Regex.Match(minOrMaxMatch.Value, @"(?<=,)(.*?)(?=\))", RegexOptions.IgnoreCase);
                                                float num1Value = Convert.ToSingle(new DataTable().Compute(num1.Value.Replace(factorId, fieldObj.ToString()), null));
                                                float num2Value = Convert.ToSingle(new DataTable().Compute(num2.Value.Replace(factorId, fieldObj.ToString()), null));
                                                float minOrMaxValue = 0;

                                                if (num1.Success & num2.Success)
                                                {
                                                    if (minOrMaxMatch.Value.StartsWith("MIN"))
                                                    {
                                                        minOrMaxValue = Math.Min(num1Value, num2Value);
                                                    }
                                                    else
                                                    {
                                                        minOrMaxValue = Math.Max(num1Value, num2Value);
                                                    }

                                                    ivf = ivf.Replace(minOrMaxMatch.Value, minOrMaxValue.ToString());
                                                }
                                            }
                                            // Non-Regular expression 
                                            /*
                                            string[] seps = new string[] { "(", ")", ",", "=" };
                                            string[] parts = ivf.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                                            float interimValue = 0;

                                            try
                                            {
                                                string part1 = parts[0];
                                                string part2 = parts[1].Replace(factorId, fieldObj.ToString());
                                                string part3 = parts[2].Replace(factorId, fieldObj.ToString());
                                                float val1 = Convert.ToSingle(new DataTable().Compute(part2, null));
                                                float val2 = Convert.ToSingle(new DataTable().Compute(part3, null));

                                                if (part1.Equals("MIN"))
                                                {
                                                    interimValue = Math.Min(val1, val2);
                                                }
                                                else if (part1.Equals("MAX"))
                                                {
                                                    interimValue = Math.Max(val1, val2);
                                                }

                                                ivf = interimValue.ToString();
                                            }
                                            catch (Exception ex)
                                            { }
                                            */
                                        }
                                        break;
                                }



                                ivf = ivf.Replace(factorId, fieldObj.ToString());
                                ivf = ivf.Replace("=", "");

                                try
                                {
                                    // Evaluate the index value formula
                                    indexValue = Convert.ToSingle(new DataTable().Compute(ivf, null));
                                }
                                catch (Exception ex)
                                {
                                    indexValue = 0;
                                }
                            }
                            else if (ivf.StartsWith("?"))
                            {
                                // ?DamageInspectionsCount>=2,1,0
                                ivf = ivf.Replace("?", "");
                                string[] seps = new string[] { "," };
                                string[] parts = ivf.Split(seps, StringSplitOptions.RemoveEmptyEntries);

                                if (parts.Count() == 3)
                                {
                                    string expressionToEvaluate = parts[0];

                                    try
                                    {
                                        expressionToEvaluate = expressionToEvaluate.Replace(factorId, fieldObj.ToString());
                                    }
                                    catch (Exception ex)
                                    { }

                                    if (factorId.Equals("DesiredVerticalClearanceUnder"))
                                    {
                                        if (str.VerticalClearanceUnderMin == -1)
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("VerticalClearanceUnderMin", 100.ToString());
                                        }
                                        else
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("VerticalClearanceUnderMin", str.VerticalClearanceUnderMin.ToString());
                                        }
                                    }
                                    else if (factorId.Equals("SingleAccess"))
                                    {
                                        expressionToEvaluate = expressionToEvaluate.Replace("DetLen", str.DetLen.ToString());
                                    }

                                    try
                                    {
                                        if (Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null)))
                                        {
                                            indexValue = Convert.ToSingle(parts[1]);
                                        }
                                        else
                                        {
                                            indexValue = Convert.ToSingle(parts[2]);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        indexValue = 0;
                                    }
                                }
                                else
                                {
                                    indexValue = 0;
                                }
                            }
                            else if (ivf.StartsWith("V"))
                            {

                            }
                        }
                        // Look up factor's index value in the database
                        else
                        {
                            try
                            {
                                indexValue = LookupFactorIndexValue(factorId, fieldObj.ToString());
                            }
                            catch { }
                        }

                        try
                        {
                            switch (factorId)
                            {
                                case "DesiredVerticalClearanceUnder":
                                    piFactor.FieldValue = str.VerticalClearanceUnderMin.ToString();
                                    break;

                                case "SingleAccess":
                                    piFactor.FieldValue = str.DetLen.ToString();
                                    break;

                                default:
                                    piFactor.FieldValue = fieldObj.ToString();
                                    break;
                            }

                        }
                        catch
                        {
                            piFactor.FieldValue = "";
                        }

                        if (indexValue > 1)
                        {
                            indexValue = 1;
                        }
                        else if (indexValue < 0)
                        {
                            indexValue = 0;
                        }

                        piFactor.Year = year;
                        piFactor.IndexValue = indexValue;
                        piFactor.Score = piFactor.PriorityIndexFactorWeight * piFactor.IndexValue;
                        piCategoryScore += piFactor.Score;
                        // Add factor to category
                        swa.PriorityIndexFactors.Add(piFactor);
                    }
                    // end foreach (var piFactor in piFactors) for a category

                    // Add category to structure
                    priorityIndexCategory.Year = year;
                    piCategoryScore = Math.Min(piCategoryScore, priorityIndexCategory.PriorityIndexMaxValue);
                    priorityIndexCategory.Score = piCategoryScore;
                    swa.PriorityIndexCategories.Add(priorityIndexCategory);
                }
                // end foreach (var priorityIndexCategory in GetPriorityIndexCategories())
            }
            // end for (int year = str.StartYear; year <= str.EndYear; year++)
        }

        public void CalculatePriorityIndex(Structure str)
        {
            for (int year = str.StartYear; year <= str.EndYear; year++)
            {
                /*
                PriorityScorePolicyEffect priorityScorePolicyEffect = new PriorityScorePolicyEffect();
                priorityScorePolicyEffect.Year = year;
                priorityScorePolicyEffect.ScoreEffect = 0;
                str.PriorityScorePolicyEffects.Add(priorityScorePolicyEffect);
                */

                foreach (var priorityIndexCategory in GetPriorityIndexCategories())
                {
                    List<PriorityIndexFactor> piFactors = GetPriorityIndexFactors().Where(e => e.PriorityIndexCategoryKey == priorityIndexCategory.PriorityIndexCategoryKey).ToList();
                    float piCategoryScore = 0;

                    foreach (var piFactor in piFactors)
                    {
                        string ivf = piFactor.IndexValueFormula.Trim();
                        string factorId = piFactor.PriorityIndexFactorId;
                        object fieldObj = null;
                        float indexValue = 0;

                        try
                        {
                            fieldObj = str.GetType().GetProperty(factorId).GetValue(str, null);
                        }
                        catch
                        {
                            //continue;
                        }

                        // Use a formula to calculate factor's index value
                        if (!String.IsNullOrEmpty(ivf))
                        {
                            if (ivf.StartsWith("="))
                            {
                                // Handle special cases that require hardcoding
                                switch (factorId)
                                {
                                    case "MaxVehicleWeight":
                                        // =MIN(MAX(-0.011*SPV+1.89,0),1)
                                        double mvwIndexValue = Math.Min(Math.Max(-0.011 * Convert.ToSingle(fieldObj.ToString()) + 1.89, 0), 1);
                                        ivf = mvwIndexValue.ToString();
                                        break;

                                    case "LoadPostingTonnage":
                                        //= IFERROR(IF(POSTING = -1, 0, MIN(5 / POSTING + 0.5, 1)), 0)
                                        if (Convert.ToSingle(fieldObj.ToString()) <= 0)
                                        {
                                            ivf = "0";
                                        }
                                        else
                                        {
                                            double lptIndexValue = Math.Min(5 / Convert.ToSingle(fieldObj.ToString()) + 0.5, 1);
                                            ivf = lptIndexValue.ToString();
                                        }
                                        break;

                                    default:
                                        if (ivf.Contains(String.Format("LN({0})", factorId)))
                                        {
                                            double natLogVal = 0;

                                            try
                                            {
                                                natLogVal = Math.Log(Convert.ToDouble(fieldObj.ToString()), Math.E);
                                            }
                                            catch { }

                                            ivf = ivf.Replace(String.Format("LN({0})", factorId), natLogVal.ToString());
                                        }

                                        // TODO: Use regular expression to make it more extensible
                                        // Example: MIN(DetLen/15.0,1), MAX(num1,num2)
                                        int countMinMax = Regex.Matches(ivf, @"(MIN|MAX)").Count;

                                        if (countMinMax == 1)
                                        {
                                            Match minOrMaxMatch = Regex.Match(ivf, @"(MIN|MAX).*\(.+,.+\)", RegexOptions.IgnoreCase);

                                            if (minOrMaxMatch.Success)
                                            {
                                                Match num1 = Regex.Match(minOrMaxMatch.Value, @"(?<=\()(.*?)(?=,)", RegexOptions.IgnoreCase);
                                                Match num2 = Regex.Match(minOrMaxMatch.Value, @"(?<=,)(.*?)(?=\))", RegexOptions.IgnoreCase);
                                                float num1Value = Convert.ToSingle(new DataTable().Compute(num1.Value.Replace(factorId, fieldObj.ToString()), null));
                                                float num2Value = Convert.ToSingle(new DataTable().Compute(num2.Value.Replace(factorId, fieldObj.ToString()), null));
                                                float minOrMaxValue = 0;

                                                if (num1.Success & num2.Success)
                                                {
                                                    if (minOrMaxMatch.Value.StartsWith("MIN"))
                                                    {
                                                        minOrMaxValue = Math.Min(num1Value, num2Value);
                                                    }
                                                    else
                                                    {
                                                        minOrMaxValue = Math.Max(num1Value, num2Value);
                                                    }

                                                    ivf = ivf.Replace(minOrMaxMatch.Value, minOrMaxValue.ToString());
                                                }
                                            }
                                            // Non-Regular expression 
                                            /*
                                            string[] seps = new string[] { "(", ")", ",", "=" };
                                            string[] parts = ivf.Split(seps, StringSplitOptions.RemoveEmptyEntries);
                                            float interimValue = 0;

                                            try
                                            {
                                                string part1 = parts[0];
                                                string part2 = parts[1].Replace(factorId, fieldObj.ToString());
                                                string part3 = parts[2].Replace(factorId, fieldObj.ToString());
                                                float val1 = Convert.ToSingle(new DataTable().Compute(part2, null));
                                                float val2 = Convert.ToSingle(new DataTable().Compute(part3, null));

                                                if (part1.Equals("MIN"))
                                                {
                                                    interimValue = Math.Min(val1, val2);
                                                }
                                                else if (part1.Equals("MAX"))
                                                {
                                                    interimValue = Math.Max(val1, val2);
                                                }

                                                ivf = interimValue.ToString();
                                            }
                                            catch (Exception ex)
                                            { }
                                            */
                                        }
                                        break;
                                }



                                ivf = ivf.Replace(factorId, fieldObj.ToString());
                                ivf = ivf.Replace("=", "");

                                try
                                {
                                    // Evaluate the index value formula
                                    indexValue = Convert.ToSingle(new DataTable().Compute(ivf, null));
                                }
                                catch (Exception ex)
                                {
                                    indexValue = 0;
                                }
                            }
                            else if (ivf.StartsWith("?"))
                            {
                                // ?DamageInspectionsCount>=2,1,0
                                ivf = ivf.Replace("?", "");
                                string[] seps = new string[] { "," };
                                string[] parts = ivf.Split(seps, StringSplitOptions.RemoveEmptyEntries);

                                if (parts.Count() == 3)
                                {
                                    string expressionToEvaluate = parts[0];

                                    try
                                    {
                                        expressionToEvaluate = expressionToEvaluate.Replace(factorId, fieldObj.ToString());
                                    }
                                    catch (Exception ex)
                                    { }

                                    if (factorId.Equals("DesiredVerticalClearanceUnder"))
                                    {
                                        if (str.VerticalClearanceUnderMin == -1)
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("VerticalClearanceUnderMin", 100.ToString());
                                        }
                                        else
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("VerticalClearanceUnderMin", str.VerticalClearanceUnderMin.ToString());
                                        }
                                    }
                                    else if (factorId.Equals("SingleAccess"))
                                    {
                                        expressionToEvaluate = expressionToEvaluate.Replace("DetLen", str.DetLen.ToString());
                                    }

                                    try
                                    {
                                        if (Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null)))
                                        {
                                            indexValue = Convert.ToSingle(parts[1]);

                                            if (factorId.Equals("SingleAccess"))
                                            {
                                                indexValue = Convert.ToSingle((str.Adt * 0.5 / 1000) + 0.5);
                                            }
                                        }
                                        else
                                        {
                                            indexValue = Convert.ToSingle(parts[2]);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        indexValue = 0;
                                    }
                                }
                                else
                                {
                                    indexValue = 0;
                                }
                            }
                            else if (ivf.StartsWith("V"))
                            {

                            }
                        }
                        // Look up factor's index value in the database
                        else
                        {
                            try
                            {
                                indexValue = LookupFactorIndexValue(factorId, fieldObj.ToString());
                            }
                            catch { }
                        }

                        try
                        {
                            switch (factorId)
                            {
                                case "DesiredVerticalClearanceUnder":
                                    piFactor.FieldValue = str.VerticalClearanceUnderMin.ToString();
                                    break;

                                case "SingleAccess":
                                    piFactor.FieldValue = str.DetLen.ToString();
                                    break;

                                default:
                                    piFactor.FieldValue = fieldObj.ToString();
                                    break;
                            }

                        }
                        catch
                        {
                            piFactor.FieldValue = "";
                        }

                        if (indexValue > 1)
                        {
                            indexValue = 1;
                        }
                        else if (indexValue < 0)
                        {
                            indexValue = 0;
                        }

                        piFactor.Year = year;
                        piFactor.IndexValue = indexValue;
                        piFactor.Score = piFactor.PriorityIndexFactorWeight * piFactor.IndexValue;
                        piCategoryScore += piFactor.Score;
                        // Add factor to category
                        str.PriorityIndexFactors.Add(piFactor);
                    }
                    // end foreach (var piFactor in piFactors) for a category

                    // Add category to structure
                    priorityIndexCategory.Year = year;
                    piCategoryScore = Math.Min(piCategoryScore, priorityIndexCategory.PriorityIndexMaxValue);
                    priorityIndexCategory.Score = piCategoryScore;
                    str.PriorityIndexCategories.Add(priorityIndexCategory);
                }
                // end foreach (var priorityIndexCategory in GetPriorityIndexCategories())
            }
            // end for (int year = str.StartYear; year <= str.EndYear; year++)
        }

        public StructureLite GetStructureLite(string strId)
        {
            StructureLite str = null;
            string qry = @"
                                select struc.strc_id,
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
                                        agency.agcy_tydc as owner,
                                        routeon.featureon,
                                        routeunder.featureunder
                                from dot1stro.dt_strc struc,
                                        dotsys.dt_dot_rgn rgn,
                                        dotsys.dt_wi_cnty_pitc county,
                                        dotsys.dt_cmty_pitc municipality,
                                        dot1stro.dt_agcy_ty agency,
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
                                        and struc.dot_rgn_nb = rgn.dot_rgn_nb(+)
                                        and struc.strc_prmy_cnty_cd = county.dot_cnty_cd(+)
                                        and ltrim(municipality.hwy_cmty_cd, '0') = to_char(struc.strc_prmy_muni_cd)
                                        and struc.strc_ownr_agcy_cd = agency.agcy_tycd(+)  
                                        and struc.strc_id = routeon.strc_id(+)
                                        and struc.strc_id = routeunder.strc_id(+)
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                str = new StructureLite();
                str.StructureId = strId;

                try
                {
                    str.CorridorCode = dt.Rows[0]["STRC_PLND_PGM"].ToString();
                }
                catch
                {
                    str.CorridorCode = "";
                }

                str.Region = dt.Rows[0]["REGION"].ToString().Trim();
                str.RegionNumber = dt.Rows[0]["REGIONNUMBER"].ToString().Trim();
                str.County = dt.Rows[0]["COUNTY"].ToString().Trim();
                str.CountyNumber = dt.Rows[0]["COUNTYNUMBER"].ToString().Trim();
                str.Municipality = dt.Rows[0]["MUNICIPALITY"].ToString().Trim();
                str.MunicipalityNumber = dt.Rows[0]["MUNICIPALITYNUMBER"].ToString().Trim();
                str.Owner = dt.Rows[0]["OWNER"].ToString().Trim();
                str.OwnerNumber = dt.Rows[0]["OWNERNUMBER"].ToString().Trim();
                str.FeatureOn = dt.Rows[0]["FEATUREON"].ToString();
                str.FeatureUnder = dt.Rows[0]["FEATUREUNDER"].ToString();

                try
                {
                    str.StmcAgcyTy = dt.Rows[0]["STMC_AGCY_TY"].ToString().Trim();
                }
                catch
                {
                    str.StmcAgcyTy = "";
                }

                try
                {
                    str.StimAgcyTycd = dt.Rows[0]["STIMAGCYTYCD"].ToString().Trim();
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

                string fundingEligibilityCsv = "";
                int counter = 0;
                foreach (var f in str.FundingEligibility)
                {
                    fundingEligibilityCsv += f;
                    if (counter > 0)
                    {
                        fundingEligibilityCsv += ",";
                    }
                    counter++;
                }
                str.FundingEligibilityCsv = fundingEligibilityCsv;
            }

            return str;
        }

        public Structure GetStructure(string strId, bool includeClosedBridges = false, bool interpolateNbi = false, bool includeCoreInspections = false,
                                        bool countTpo = false, int startYear = 0, int endYear = 0, bool getLastInspection = false)
        {
            Structure str = null;
            string qry = @"
                                select struc.strc_id, 
                                        struc.strc_tycd,
                                        struc.strc_plnd_pgm,
                                        struc.stmc_agcy_ty,
                                        struc.stim_agcy_tycd,
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
                                        mainspan.bspn_nb,
                                        mainspan.main_span_fl,
                                        spanconfig.span_cnfg_tydc,
                                        spanconfig.bspn_cnfg_ty,
                                        spanmaterial.span_matl_desc,
                                        spanmaterial.bspn_matl_cd,
                                        routeon.featureOn,
                                        routeunder.featureUnder,
                                        routeon.rsOn,
                                        routeunder.rsUnder,
                                        loadposting.brdg_load_pstg_cd,
                                        loadposting.load_pstg_desc,
                                        loadposting.load_pstg_tns
                                from dot1stro.dt_strc struc, 
                                        dot1stro.dt_brdg bridge,
                                        dot1stro.dt_brdg_span mainspan,
                                        dot1stro.dt_brdg_span_cnfg spanconfig,
                                        dot1stro.dt_brdg_span_matl spanmaterial,
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
                                    and struc.strc_sscd = '50'
                                    and struc.strc_id = bridge.strc_id
                                    and bridge.strc_id = mainspan.strc_id
                                    and mainspan.main_span_fl = 'Y'
                                    and mainspan.bspn_cnfg_ty = spanconfig.bspn_cnfg_ty(+)
                                    and mainspan.bspn_matl_cd = spanmaterial.bspn_matl_cd(+)
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

            if (includeClosedBridges)
            {
                qry = @"
                                select struc.strc_id, 
                                        struc.strc_tycd,
                                        struc.strc_plnd_pgm,
                                        struc.stmc_agcy_ty,
                                        struc.stim_agcy_tycd,
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
                                        mainspan.bspn_nb,
                                        mainspan.main_span_fl,
                                        spanconfig.span_cnfg_tydc,
                                        spanconfig.bspn_cnfg_ty,
                                        spanmaterial.span_matl_desc,
                                        spanmaterial.bspn_matl_cd,
                                        routeon.featureOn,
                                        routeunder.featureUnder,
                                        routeon.rsOn,
                                        routeunder.rsUnder,
                                        loadposting.brdg_load_pstg_cd,
                                        loadposting.load_pstg_desc,
                                        loadposting.load_pstg_tns
                                from dot1stro.dt_strc struc, 
                                        dot1stro.dt_brdg bridge,
                                        dot1stro.dt_brdg_span mainspan,
                                        dot1stro.dt_brdg_span_cnfg spanconfig,
                                        dot1stro.dt_brdg_span_matl spanmaterial,
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
                                    and struc.strc_sscd in ('50', '70')
                                    and struc.strc_id = bridge.strc_id
                                    and bridge.strc_id = mainspan.strc_id
                                    and mainspan.main_span_fl = 'Y'
                                    and mainspan.bspn_cnfg_ty = spanconfig.bspn_cnfg_ty(+)
                                    and mainspan.bspn_matl_cd = spanmaterial.bspn_matl_cd(+)
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
            }

            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                str = new Structure();
                str.StartYear = startYear;
                str.EndYear = endYear;

                if (countTpo)
                    str.CountTpo = true;

                if (includeCoreInspections)
                {
                    str.CoreInspections = new List<CoreInspection>();
                    str.CoreInspections = GetCoreInspections(strId);
                }

                str.PriorityFactors = GetPriorityFactors();
                str.PriorityFactorMeasurements = GetPriorityFactorMeasurements(GetPriorityFactors());
                str.StructureId = dt.Rows[0]["STRC_ID"].ToString();

                try
                {
                    str.CorridorCode = dt.Rows[0]["STRC_PLND_PGM"].ToString();
                }
                catch (Exception ex)
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

                str.StructureType = dt.Rows[0]["SPAN_CNFG_TYDC"].ToString();
                str.StructureTypeCode = dt.Rows[0]["BSPN_CNFG_TY"].ToString();
                str.MainSpanMaterial = dt.Rows[0]["SPAN_MATL_DESC"].ToString();
                str.MainSpanMaterialCode = dt.Rows[0]["BSPN_MATL_CD"].ToString();

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
                if (complexStructureConfigurations.Any(s => str.StructureTypeCode.Contains(s)))
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
                GetGisInfo(str);
                GetTrafficInfo(str);
                GetStnInventoryInfo(str);
                GetBorderBridgeInfo(str);
                GetBridgeMunicipalPlanningInfo(str);
                GetPrimaryHighwayFreightSystemInfo(str);
                GetSpecialComponents(str);

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
                // DataTable dtSdFo = GetLastInspection(strId);
                StructureDeterioration strDet = GetStructureForDeterioration(strId);
                str.NbiDeckQualifiedDeteriorationCurve = strDet.NbiDeckQualifiedDeteriorationCurve;
                str.NbiSuperQualifiedDeteriorationCurve = strDet.NbiSuperQualifiedDeteriorationCurve;
                str.NbiSubQualifiedDeteriorationCurve = strDet.NbiSubQualifiedDeteriorationCurve;
                str.InterpolateNbi = interpolateNbi;
                str.LastInspection = GetLastInspection(str, str.InterpolateNbi, getLastInspection);

                if (str.LastInspection == null)
                    return null;

                str.LastInspectionYear = str.LastInspection.InspectionDate.Year;

                // Determine if it's a buried structure
                if (str.OverburdenDepth >= 9)
                {
                    str.BuriedStructure = true;
                }

                if (str.LastInspection.Elements.Where(e => e.ElemNum == 9325).Count() > 0)
                {
                    str.BuriedStructure = true;
                }

                str.DeckComposition = GetDeckComposition(str.StructureId);
                if (str.DeckComposition.Where(e => e == "03" || e == "04").Count() > 0)
                {
                    str.BuriedStructure = true;
                }

                var paintedSteelElements = str.LastInspection.Elements.Where(e => e.ElemNum.Equals(8516)
                                                                                && (e.ParentElemNum.Equals(28)
                                                                                || e.ParentElemNum.Equals(29)
                                                                                || e.ParentElemNum.Equals(102)
                                                                                || e.ParentElemNum.Equals(107)
                                                                                || e.ParentElemNum.Equals(120)
                                                                                || e.ParentElemNum.Equals(141))
                                                                             );

                foreach (var paintedSteelElement in paintedSteelElements)
                {
                    str.SuperstructurePaintArea += paintedSteelElement.Cs1Quantity + paintedSteelElement.Cs2Quantity +
                                                        paintedSteelElement.Cs3Quantity + paintedSteelElement.Cs4Quantity;

                    str.SuperstructureSpotPaintArea = paintedSteelElement.Cs4Quantity;
                }

                var jointElements = str.LastInspection.Elements.Where(e => e.ElemNum.Equals(300)
                                                                        || e.ElemNum.Equals(301)
                                                                        || e.ElemNum.Equals(302)
                                                                        || e.ElemNum.Equals(303)
                                                                        || e.ElemNum.Equals(304)
                                                                        || e.ElemNum.Equals(305)
                                                                        || e.ElemNum.Equals(306)
                                                                     );

                foreach (var jointElement in jointElements)
                {
                    str.JointLength += jointElement.Cs1Quantity + jointElement.Cs2Quantity + jointElement.Cs3Quantity + jointElement.Cs4Quantity;
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

                // Get corridor of structure
                /*
                qry = @"
                                select StructureId, str.CorridorCode, CorridorDesc
                                from Structure str, Corridor
                                where StructureId = @strId
                                    and str.CorridorCode = Corridor.CorridorCode
                            ";
                SqlParameter[] prm = new SqlParameter[1];
                prm[0] = new SqlParameter("@strId", SqlDbType.VarChar);
                prm[0].Value = strId;
                DataTable d = ExecuteSelect(qry, prm, samConn);

                if (d != null && d.Rows.Count > 0)
                {
                    string cc = d.Rows[0]["CORRIDORCODE"].ToString();
                    str.CorridorCode = cc;
                    str.CorridorDesc = d.Rows[0]["CORRIDORDESC"].ToString();
                }
                */
                StructureLite strLite = GetStructureCorridorCode(strId);

                /*
                if (strLite != null)
                {
                    str.CorridorCode = strLite.CorridorCode;
                    str.CorridorDesc = strLite.CorridorDesc;
                }
                */

                if (IsStructureOnHighClearanceRoute(strId))
                {
                    str.HighClearanceRoute = true;
                }

                // Get other structure properties
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
                DataTable dt2 = ExecuteSelect(qry, prms2, hsiConn);

                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    int numOverlays = 0;
                    int numThinPolymerOverlays = 0;

                    foreach (DataRow dr in dt2.Rows)
                    {
                        string workPerformedCode = dr["CNST_WORK_PFMD_CD"].ToString();
                        int constYear = Convert.ToInt32(dr["CNST_YR"]);
                        str.ConstructionHistory += String.Format("({0}){1}  ", constYear, dr["CNST_WORK_DESC"].ToString());

                        switch (workPerformedCode)
                        {
                            case "01": // new structure
                                str.YearBuilt = constYear;
                                str.YearBuiltActual = constYear;
                                //str.DeckBuiltYear = Convert.ToInt32(dr["CNST_YR"]);
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
                                //str.DeckBuiltYear = Convert.ToInt32(dr["CNST_YR"]);
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
                                //str.DeckBuiltYear = Convert.ToInt32(dr["CNST_YR"]);
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

                        StructureWorkAction swa = GetStructureWorkAction(workPerformedCode);

                        if (swa != null)
                        {
                            swa.WorkActionYear = Convert.ToInt32(dr["CNST_YR"]);
                            str.ConstructionHistoryProjects.Add(swa);
                        }
                        else
                        {
                            swa = new StructureWorkAction(workPerformedCode);
                            swa.WorkActionYear = Convert.ToInt32(dr["CNST_YR"]);
                            str.ConstructionHistoryProjects.Add(swa);
                        }
                    }

                    str.NumOlays = numOverlays;
                    str.NumThinPolymerOverlays = numThinPolymerOverlays;

                    var decks = str.LastInspection.Elements.Where(e => e.ElementClassificationCode.Equals(Code.Deck) || e.ElementClassificationCode.Equals(Code.Slab)).ToList();

                    if (!String.IsNullOrEmpty(dt.Rows[0]["BGDK_AREA"].ToString()))
                    {
                        str.DeckArea = Convert.ToInt32(dt.Rows[0]["BGDK_AREA"]);
                    }
                    else if (decks.Count > 0)
                    {
                        str.DeckArea = decks.First().TotalQuantity;
                    }

                    if (str.NumOlays == 0 && str.NumThinPolymerOverlays == 0)
                    {
                        try
                        {
                            var bareWearingSurfaces = str.LastInspection.Elements.Where(e => e.ElemNum == Code.BareWearingSurface).ToList();
                            if (bareWearingSurfaces.Count > 0)
                            {
                                str.OverlayQuantity = bareWearingSurfaces.First().TotalQuantity;
                            }
                            else
                            {
                                if (decks.Count > 0)
                                {
                                    str.OverlayQuantity = decks.First().TotalQuantity;
                                }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            var overlays = str.LastInspection.Elements.Where(e => e.ElementClassificationCode.Equals(Code.Overlay)).ToList();
                            str.OverlayQuantity = overlays.Max(e => e.TotalQuantity);
                        }
                        catch { }
                    }
                }

                if (str.YearBuiltActual == 0)
                    return null;

                // Is fracture critical inspection required?
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
                DataTable dt3 = ExecuteSelect(qry, prms3, hsiConn);

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

                // Risks
                var riskTypes = Enum.GetValues(typeof(WisamType.Risks)).Cast<WisamType.Risks>();
                foreach (var riskType in riskTypes)
                {
                    Risk risk = null;

                    switch (riskType)
                    {
                        case WisamType.Risks.Sd:
                            risk = GetRisk(WisamType.Risks.Sd);
                            if (str.StructurallyDeficient)
                            {
                                risk.RiskValue = risk.RiskMaxValue;
                            }
                            break;
                        case WisamType.Risks.Fo:
                            risk = GetRisk(WisamType.Risks.Fo);
                            if (str.FunctionalObsolete)
                            {
                                risk.RiskValue = risk.RiskMaxValue;
                            }
                            break;
                        case WisamType.Risks.BridgeAge:
                            risk = GetRisk(WisamType.Risks.BridgeAge);
                            risk.RiskValue = ((DateTime.Now.Year - str.YearBuilt) / 75) * risk.RiskMaxValue;

                            if (risk.RiskValue > risk.RiskMaxValue)
                            {
                                risk.RiskValue = risk.RiskMaxValue;
                            }
                            break;
                        default:
                            risk = GetRisk(strId, riskType);
                            break;
                    }
                    str.Risks.Add(risk);
                    str.PriorityScore += risk.RiskValue;
                }

                str.PriorityScore = Convert.ToSingle(Math.Round(str.PriorityScore, 2));



                // **************
                str.DeckWidths = GetDeckWidths(strId);

                if (str.DeckWidths.Count > 0)
                {
                    str.MinDeckWidth = str.DeckWidths.Min();
                }

                str.AdtUnder = GetAdtUnder(strId);
                str.Adt = GetAdt(strId);
                str.Adtt = GetAdtt(strId) / 100.0 * str.Adt;
                str.AdtYr = GetAdtYr(strId);
                str.AdtFut = GetAdtFut(strId);
                str.AdtFutYr = GetAdtFutYr(strId);
                str.Strahnet = IsStrahnet(strId);
                str.Staa = IsStaa(strId);
                str.Nhs = IsNhs(strId);
                str.HighwayFeatureDesignationOn = GetHighwayFeatureDesignationOn(strId);
                str.HighwayFeatureDesignationUnder = GetHighwayFeatureDesignationUnder(strId);
                str.ServiceFeatureUnderTypeCode = GetServiceFeatureUnderTypeCode(strId);
                str.DamageInspectionsCount = GetNumberOfInspectionsOfAGivenType(strId, "D");
                str.VerticalClearanceUnderMin = GetVerticalClearanceUnderMin(strId, str.ServiceFeatureUnderTypeCode);
                str.VerticalClearanceOnMin = GetVerticalClearanceOnMin(strId);
                str.DesiredVerticalClearanceUnder = GetDesiredVerticalClearanceUnder(str.FunctionalClassificationUnder, str.RouteSystemUnder);

                if (str.DamageInspectionsCount > 0)
                {
                    str.DmgInsp = true;
                }
                else
                {
                    str.DmgInsp = false;
                }

                if (str.UnitBridge)
                {
                    str.WholeStructureDeckArea = GetWholeStructureDeckArea(str.StructureId);
                }



                foreach (var pf in str.PriorityFactors)
                {
                    foreach (var pfm in str.PriorityFactorMeasurements.Where(e => e.FactorCode.Equals(pf.FactorCode)))
                    {
                        double grossValueNum = -1;
                        string grossValue = "";
                        string expressionToEvaluate = "";
                        string valueFormula = pfm.GrossValueFormula.Trim();
                        bool goOn = true;

                        if (!String.IsNullOrEmpty(valueFormula))
                        {

                        }

                        switch (pfm.MeasurementCode.ToUpper())
                        {
                            case "DW": // hard-coded
                                pfm.IndexValue = 0;

                                if (str.MinDeckWidth > 0)
                                {
                                    if (str.RouteSystemOn.Equals("IH"))
                                    {
                                        pfm.IndexValue = GetDwIndexValueInterstate(str);
                                    }
                                    else if (str.FunctionalClassificationOn.Equals("INTERSTATE-RURAL")
                                                || str.FunctionalClassificationOn.Equals("INTERSTATE-URBAN")
                                                || str.FunctionalClassificationOn.Equals("OTH FWY/EWY-URBAN")
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueInterstate(str);
                                    }
                                    else if (String.IsNullOrEmpty(str.FunctionalClassificationOn)
                                                && str.RouteSystemOn.Equals("USH")
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueInterstate(str);
                                    }
                                    else if (str.FunctionalClassificationOn.Equals("COLLECTOR-URBAN")
                                                || str.FunctionalClassificationOn.Equals("LOCAL-URBAN")
                                                || str.FunctionalClassificationOn.Equals("MINOR ART-URBAN")
                                                || str.FunctionalClassificationOn.Equals("OTH PRIN ART-URBAN")
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueUrban(str);
                                    }
                                    else if (str.FunctionalClassificationOn.Equals("MINOR ART-RURAL")
                                                || str.FunctionalClassificationOn.Equals("OTH PRIN ART-RURAL")
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueArterial(str);
                                    }
                                    else if (String.IsNullOrEmpty(str.FunctionalClassificationOn)
                                                && (str.RouteSystemOn.Equals("CTH") || str.RouteSystemOn.Equals("STH"))
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueArterial(str);
                                    }
                                    else if (str.FunctionalClassificationOn.Equals("LOCAL-RURAL")
                                                || str.FunctionalClassificationOn.Equals("MAJOR COL-RURAL")
                                                || str.FunctionalClassificationOn.Equals("MINOR COL-RURAL")
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueCollectorOrLocalRoad(str);
                                    }
                                    else if (String.IsNullOrEmpty(str.FunctionalClassificationOn)
                                                && str.RouteSystemOn.Equals("LRD")
                                            )
                                    {
                                        pfm.IndexValue = GetDwIndexValueArterial(str);
                                    }
                                }

                                pfm.FinalValue = pfm.Weight * pfm.IndexValue;
                                pf.FactorValue += pfm.FinalValue;
                                goOn = false;
                                break;

                            case "HGCLRRT":
                                if (str.HighClearanceRoute)
                                {
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }

                                pfm.GrossValue = grossValue;
                                break;

                            case "VCU":
                                grossValueNum = str.VerticalClearanceUnderMin;
                                pfm.GrossValueNum = grossValueNum;

                                if (pfm.GrossValueNum == -1)
                                {
                                    goOn = false;
                                }

                                break;

                            case "VCO":
                                grossValueNum = str.VerticalClearanceOnMin;
                                pfm.GrossValueNum = grossValueNum;

                                if (pfm.GrossValueNum == -1)
                                {
                                    goOn = false;
                                }

                                break;

                            case "CPLXSTR":
                                if (str.CplxStr)
                                {
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }

                                pfm.GrossValue = grossValue;
                                break;

                            case "LDRG":
                                grossValueNum = str.ratingValue;
                                pfm.GrossValueNum = grossValueNum;
                                //pfm.GrossValueNum = 21;
                                break;

                            case "LDPG":
                                grossValueNum = str.LoadPostingTonnage * 2; // kips
                                pfm.GrossValueNum = grossValueNum;

                                if (pfm.GrossValueNum == 0)
                                {
                                    goOn = false;
                                }
                                break;

                            case "DMGINSP":
                                if (str.DmgInsp)
                                {
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }

                                pfm.GrossValue = grossValue;
                                break;
                            case "LRGSTR":
                                if (str.DeckArea > 40000 || str.WholeStructureDeckArea > 40000)
                                {
                                    str.LrgStr = true;
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }

                                pfm.GrossValue = grossValue;
                                break;

                            case "BHN":
                                if (str.Nhs && str.HighwayFeatureDesignationOn.Equals("01")) // 01- MAINLINE
                                {
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }

                                pfm.GrossValue = grossValue;
                                break;

                            case "SCCR-FRCR":
                                if (str.ScourCritical)
                                {
                                    grossValue = "TRUE;";
                                }
                                else
                                {
                                    grossValue = "FALSE;";
                                }

                                if (str.FractureCritical)
                                {
                                    grossValue += "TRUE";
                                }
                                else
                                {
                                    grossValue += "FALSE";
                                }

                                pfm.GrossValue = grossValue;
                                break;

                            case "CORR2030":
                                grossValue = str.CorridorCode;
                                pfm.GrossValue = grossValue;
                                break;

                            case "STRAHNET":
                                if (str.Strahnet)
                                {
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }
                                pfm.GrossValue = grossValue;
                                break;

                            case "STAA":
                                if (str.Staa)
                                {
                                    grossValue = "TRUE";
                                }
                                else
                                {
                                    grossValue = "FALSE";
                                }
                                pfm.GrossValue = grossValue;
                                break;

                            case "ADTPERLANE":
                                try
                                {
                                    grossValueNum = str.Adt / str.LanesOn;
                                    pfm.GrossValueNum = grossValueNum;
                                }
                                catch { }
                                break;

                            case "ADTTPERLANE":
                                try
                                {
                                    grossValueNum = ((str.Adtt / 100.0) * str.Adt) / str.LanesOn;
                                    pfm.GrossValueNum = grossValueNum;
                                }
                                catch { }
                                break;

                            case "AGR":
                                try
                                {
                                    double x = 1.0 * str.AdtFut / str.Adt;
                                    double y = 1.0 / (str.AdtFutYr - str.AdtYr + 1);
                                    grossValueNum = Math.Pow(x, y) - 1;
                                    pfm.GrossValueNum = grossValueNum;
                                }
                                catch { }
                                break;

                            case "ADT":
                                try
                                {
                                    grossValueNum = str.Adt;
                                    pfm.GrossValueNum = grossValueNum;
                                }
                                catch { }
                                break;

                            case "DETLEN":
                                try
                                {
                                    grossValueNum = str.DetLen;
                                    pfm.GrossValueNum = grossValueNum;
                                }
                                catch { }
                                break;

                            case "VC":
                                goOn = false;
                                break;

                            case "CFINDEX":
                                goOn = false;
                                break;
                        }

                        if (goOn && pfm.CalcIndexValue)
                        {
                            var mis = measurementIndices.Where(e => e.MeasurementCode.Equals(pfm.MeasurementCode));
                            bool isExpressionTrue = false;

                            foreach (var mi in mis)
                            {
                                if (mi.GrossExpression)
                                {
                                    expressionToEvaluate = mi.GrossValue.Trim().Replace(mi.MeasurementCode, grossValueNum.ToString());

                                    // Special cases
                                    if (pfm.MeasurementCode.Equals("LDPG"))
                                    {
                                        expressionToEvaluate = expressionToEvaluate.Replace("RSON", String.Format("'{0}'", str.RouteSystemOn));
                                    }
                                    else if (pfm.MeasurementCode.Equals("LDRG"))
                                    {
                                        if (str.HsOrRf.Equals("HS"))
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("HS", str.ratingValue.ToString());
                                            //expressionToEvaluate = expressionToEvaluate.Replace("HS", 21.ToString());
                                        }
                                        else if (str.HsOrRf.Equals("RF"))
                                        {
                                            expressionToEvaluate = expressionToEvaluate.Replace("RF", str.ratingValue.ToString());
                                        }
                                    }
                                    else if (pfm.MeasurementCode.Equals("VCU"))
                                    {
                                        expressionToEvaluate = expressionToEvaluate.Replace("HFDU", String.Format("'{0}'", str.ServiceFeatureUnderTypeCode));
                                        expressionToEvaluate = expressionToEvaluate.Replace("FUCLU", String.Format("'{0}'", str.FunctionalClassificationUnderCode));
                                    }

                                    try
                                    {
                                        isExpressionTrue = Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null));
                                    }
                                    catch { }

                                    if (isExpressionTrue)
                                    {
                                        if (!mi.IndexExpression) // straight value, not an expression
                                        {
                                            pfm.IndexValue = Convert.ToDouble(mi.IndexValue);
                                            pfm.FinalValue = pfm.Weight * pfm.IndexValue;
                                            pf.FactorValue += pfm.FinalValue;
                                        }
                                        else // have an index expression, so evaluate expression
                                        {
                                            string indexExpression = mi.IndexValue.Trim().ToUpper();

                                            if (indexExpression.Contains(String.Format("LN({0})", mi.MeasurementCode)))
                                            {
                                                var natLogVal = Math.Log(pfm.GrossValueNum, Math.E);
                                                indexExpression = indexExpression.Replace(String.Format("LN({0})", mi.MeasurementCode), natLogVal.ToString());
                                            }

                                            if (indexExpression.Contains("POW"))
                                            {
                                                try
                                                {
                                                    Match m = Regex.Match(indexExpression, @"(?<=\().+?(?=\))", RegexOptions.IgnoreCase);
                                                    var stringToReplace = String.Format("POW({0})", m.Value);
                                                    string[] items = m.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                                    double baseNum = Convert.ToDouble(items[0].Replace(pfm.MeasurementCode, pfm.GrossValueNum.ToString()));
                                                    double exponent = Convert.ToDouble(items[1]);
                                                    var powerVal = Math.Pow(baseNum, exponent);
                                                    indexExpression = indexExpression.Replace(stringToReplace, powerVal.ToString());
                                                }
                                                catch { }
                                            }

                                            if (pfm.MeasurementCode.Equals("LDRG"))
                                            {
                                                if (str.HsOrRf.Equals("HS"))
                                                {
                                                    indexExpression = indexExpression.Replace("HS", str.ratingValue.ToString());
                                                    //indexExpression = indexExpression.Replace("HS", 21.ToString());
                                                }
                                                else if (str.HsOrRf.Equals("RF"))
                                                {
                                                    indexExpression = indexExpression.Replace("RF", str.ratingValue.ToString());
                                                }
                                            }

                                            indexExpression = indexExpression.Replace(pfm.MeasurementCode, pfm.GrossValueNum.ToString());

                                            try
                                            {
                                                pfm.IndexValue = Convert.ToDouble(new DataTable().Compute(indexExpression, null));
                                                pfm.FinalValue = pfm.Weight * pfm.IndexValue;
                                                pf.FactorValue += pfm.FinalValue;
                                            }
                                            catch { }
                                        }

                                        break;
                                    } // else you move on to the next index evaluation
                                }
                                else
                                {
                                    try
                                    {
                                        if (pfm.GrossValue.ToUpper().Equals(mi.GrossValue.ToUpper()))
                                        {
                                            pfm.IndexValue = Convert.ToDouble(mi.IndexValue);
                                            pfm.FinalValue = pfm.Weight * pfm.IndexValue;
                                            pf.FactorValue += pfm.FinalValue;

                                            break;
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                        else
                        {

                        }
                    }

                    if (pf.FactorCode.Equals("SCF"))
                    {
                        var vco = str.PriorityFactorMeasurements
                                    .Where(e => e.FactorCode.Equals("SCF") && e.MeasurementCode.Equals("VCO")).ToList();

                        var vcu = str.PriorityFactorMeasurements
                                    .Where(e => e.FactorCode.Equals("SCF") && e.MeasurementCode.Equals("VCU")).ToList();

                        var vc = str.PriorityFactorMeasurements
                                    .Where(e => e.FactorCode.Equals("SCF") && e.MeasurementCode.Equals("VC")).ToList();

                        try
                        {
                            if (vco.First().IndexValue != 0 || vcu.First().IndexValue != 0)
                            {
                                vc.First().IndexValue = 1;
                                vc.First().FinalValue = vc.First().Weight * vc.First().IndexValue;
                            }
                            else
                            {
                                vc.First().IndexValue = 0;
                                vc.First().FinalValue = 0;
                            }

                            pf.FactorValue += vc.First().FinalValue;
                        }
                        catch { }
                    }
                    else if (pf.FactorCode.Equals("CEF") && pf.FactorValue > 1)
                    {
                        pf.FactorValue = 1;
                    }
                }

                foreach (var pf in str.PriorityFactors)
                {
                    str.PriorityIndex += pf.FactorWeight * pf.FactorValue;
                }

                CalculatePriorityIndex(str);
            }

            return str;
        }

        public List<StructureMaintenanceItem> GetStructureMaintenanceItems()
        {
            List<StructureMaintenanceItem> maintenanceItems = new List<StructureMaintenanceItem>();
            string qry =
                @"
                    SELECT DOT1STRO.DT_STRC.STRC_ID     AS Bridge_ID,
                      RGN.DOT_RGN_CD                    AS Region,
                      DOTSYS.DT_WI_CNTY_PITC.WI_CNTY_NM AS County,
                      owneragency.agcy_tydc     as Owner,
                      maintaineragency.AGCY_TYDC              AS Maintainer,
                      DOTSYS.DT_CMTY_PITC.CMTY_TY
                      || '-'
                      || DOTSYS.DT_CMTY_PITC.CMTY_NM                                                                                                                            AS Muni,
                      DOT1STRO.DT_STRC_RTE.FTR_NM                                                                                                                               AS Feature_On,
                      dot1stro.dt_strc_rte.strc_rte_loc                                                                                                                         as Location_On,
                      U.FEATURE_UNDER                                                                                                                                           AS Feature_Under,
                      TO_CHAR(DOT1STRO.DT_STRC.STRC_LN,'fm9999.9')                                                                                                              AS Overall_Length,
                      DOT1STRO.DT_BRDG.BGDK_AREA                                                                                                                                AS Deck_Area,
                      TO_CHAR(ABT.BGAB_RDWY_WD,'fm999.9')                                                                                                                       AS Roadway_Width,
                      insp.stin_dt                                                                                                                                              AS inspection_date,
                      a.stmc_actn_item_nb                                                                                                                                       as Maintenance_Id,
                      B.ACTN_ITEM_DESC                                                                                                                                          AS Maintenance_Item,
                      DECODE(A.STMC_ACTN_SSCD,0,'IDENTIFIED',1,'APPROVED',2,'REJECTED',3,'DEFERRED',4,'ASSIGNED',5,'WORK COMPLETED',7,'INFORMATIONAL',8,'OBSOLETE',9,'UNKNOWN') AS STATUS,
                      '$'
                      || A.STMC_ACTN_EST_AMT                   AS Estimated_Amount,
                      TO_CHAR(A.STMC_ACTN_EST_DT,'YYYY-MM-DD') AS Estimated_Date,
                      to_char(A.stmc_actn_ssdt, 'YYYY-MM-DD') AS Status_Date,
                      '$'
                      || A.STMC_ACTN_ACTL_AMT                   AS Actual_Amount,
                      TO_CHAR(A.STMC_ACTN_ACTL_DT,'YYYY-MM-DD') AS Actual_Date,
                      A.STMC_ACTN_PFMD_BY                       AS PERFORMED_BY,
                      A.STMC_ACTN_APVD_BY                       AS APPROVED_BY,
                      A.STMC_ACTN_RCMDD_BY                      AS RECOMMEND_BY,
                      dot1stro.dt_strc_insr.strc_insr_lnm || ', ' || dot1stro.dt_strc_insr.strc_insr_fnm  AS Recommender,
                      A.STMC_ACPRY                              AS ACTION_PRIORITY,
                      pry.stmc_acpry_nm,
                      A.STIN_ID                                 AS INSPECTION_ID,
                      MG.IMAGES,
                      A.CNST_PROJ_SN      AS CONSTRUCTION_ID,
                      A.STMC_ACTN_ITEM_CM AS Comments
                    FROM DOTSYS.DT_DOT_RGN RGN,
                      DOT1STRO.DT_AGCY_TY maintaineragency,
                      dot1stro.dt_agcy_ty owneragency,
                      DOT1STRO.DT_STRC,
                      DOT1STRO.DT_STRC_RTE,
                      DOT1STRO.DT_BRDG,
                      DOTSYS.DT_WI_CNTY_PITC,
                      DOTSYS.DT_CMTY_PITC,
                      (SELECT STRC_ID,
                        MAX(FTR_NM) AS FEATURE_UNDER
                      FROM DOT1STRO.DT_STRC_RTE
                      WHERE STRC_RTE_OUF = 'U'
                      GROUP BY STRC_ID
                      ) U,
                      (SELECT STMC_ACTN_NB,
                        COUNT(*) IMAGES
                      FROM DOT1STRO.DT_STMC_ACTN_GRHC
                      GROUP BY STMC_ACTN_NB
                      ) MG,
                      DOT1STRO.DT_STMC_ACTN A,
                      DOT1STRO.DT_STMC_ACTN_ITEM B,
                      DOT1STRO.DT_BRDG_ABTM ABT,
                      dot1stro.dt_strc_insp insp,
                      dot1stro.dt_strc_insr,
                      dot1stro.dt_stmc_acpry_cd pry
                    WHERE insp.stin_id (+)                             = A.stin_id
                    AND MG.STMC_ACTN_NB (+)                            = A.STMC_ACTN_NB
                    AND RGN.DOT_RGN_NB                                 = DOT1STRO.DT_STRC.DOT_RGN_NB
                    AND DOT1STRO.DT_STRC.STRC_ID                       = DOT1STRO.DT_STRC_RTE.STRC_ID
                    AND DOT1STRO.DT_STRC.STRC_ID                       = DOT1STRO.DT_BRDG.STRC_ID
                    AND maintaineragency.AGCY_TYCD                  = DOT1STRO.DT_STRC.STMC_AGCY_TY
                    AND DOTSYS.DT_WI_CNTY_PITC.DOT_CNTY_CD             = DOT1STRO.DT_STRC.STRC_PRMY_CNTY_CD
                    AND DOT1STRO.DT_STRC.STRC_TYCD                    IN('B','P','S','G','C','L','R','N')
                    AND((DOT1STRO.DT_STRC_RTE.STRC_RTE_OUF)            = 'O')
                    AND((DOT1STRO.DT_STRC_RTE.STRC_RTE_PRMY_FL)        = 'Y')
                    AND LPAD(DOT1STRO.DT_STRC.STRC_PRMY_MUNI_CD,5,'0') = DOTSYS.DT_CMTY_PITC.HWY_CMTY_CD
                    AND U.STRC_ID                                      = DOT1STRO.DT_STRC.STRC_ID
                    AND ABT.STRC_ID (+)                                = DOT1STRO.DT_BRDG.STRC_ID
                    AND ABT.BGAB_CNF                                   = 'C'
                    AND DOT1STRO.DT_STRC.STRC_SSCD                    <> 60
                    AND A.STMC_ACTN_ITEM_NB                            = B.STMC_ACTN_ITEM_NB
                    --and b.actn_item_desc not like 'IMP%'
                    AND A.STRC_ID                                      = DOT1STRO.DT_STRC.STRC_ID
                    and dot1stro.dt_strc_insr.strc_insr_id (+) = A.stmc_actn_rcmdd_by
                    and pry.stmc_acpry (+) = A.stmc_acpry
                    and owneragency.agcy_tycd (+) = dot1stro.dt_strc.strc_ownr_agcy_cd
                    --and dot1stro.dt_strc.strc_id = 'B230115'
                    AND A.STMC_ACTN_SSCD                          IN('0') --Identified maintenance items only
                    AND DOT1STRO.DT_STRC.DOT_RGN_NB                   IN(4,3,5,2,1)
                    and dot1stro.dt_strc.strc_tycd in ('B','P')
                    and (
                                                    strc_ownr_agcy_cd in ('10','15','16','20','44','45') 
                                                    or stmc_agcy_ty in ('10','15','16','20','44','45') 
                                                    or stim_agcy_tycd in ('10','15','16','20','44','45')
                                                  )
                    and A.stmc_actn_ssdt >= to_date('01/01/2019','MM/dd/yyyy')
                ";
            DataTable dt = ExecuteSelect(qry, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                string currentStructureId = "";
                string previousStructureId = "";
                //Structure structure = null;

                foreach (DataRow dr in dt.Rows)
                {
                    currentStructureId = dr["bridge_id"].ToString().Trim();
                    StructureMaintenanceItem maintenanceItem = new StructureMaintenanceItem(currentStructureId);

                    /*
                    if (currentStructureId != previousStructureId)
                    {
                        structure = GetStructure(currentStructureId, true, true);
                    }
                    
                    try
                    {
                        maintenanceItem.GeoLocation = structure.GeoLocation;
                    }
                    catch { }

                    try
                    {
                        maintenanceItem.LastInspection = structure.LastInspection;
                    }
                    catch { }*/
                    maintenanceItem.Region = dr["region"].ToString().Trim();
                    maintenanceItem.County = dr["county"].ToString().Trim();
                    maintenanceItem.OwnerAgency = dr["owner"].ToString().Trim();
                    maintenanceItem.Municipality = dr["muni"].ToString().Trim();
                    maintenanceItem.FeatureOn = dr["feature_on"].ToString().Trim();
                    maintenanceItem.LocationOn = dr["location_on"].ToString().Trim();
                    maintenanceItem.FeatureUnder = dr["feature_under"].ToString().Trim();


                    if (dr["overall_length"] != DBNull.Value)
                    {
                        maintenanceItem.OverallLength = Convert.ToSingle(dr["overall_length"]);
                    }

                    /*
                    try
                    {
                        maintenanceItem.DeckArea = Convert.ToInt32(dr["deck_area"]);
                    }
                    catch { }*/

                    if (dr["deck_area"] != DBNull.Value)
                    {
                        maintenanceItem.DeckArea = Convert.ToInt32(dr["deck_area"]);
                    }

                    /*
                    try
                    {
                        maintenanceItem.RoadwayWidth = Convert.ToSingle(dr["roadway_width"]);
                    }
                    catch { }*/

                    if (dr["roadway_width"] != DBNull.Value)
                    {
                        maintenanceItem.RoadwayWidth = Convert.ToSingle(dr["roadway_width"]);
                    }

                    maintenanceItem.ItemId = dr["maintenance_id"].ToString().Trim();
                    maintenanceItem.ItemDescription = dr["maintenance_item"].ToString().Trim();
                    maintenanceItem.Status = dr["status"].ToString().Trim();

                    /*
                    try
                    {
                        maintenanceItem.StatusDate = Convert.ToDateTime(dr["status_date"]);
                    }
                    catch { }*/

                    if (dr["status_date"] != DBNull.Value)
                    {
                        maintenanceItem.StatusDate = Convert.ToDateTime(dr["status_date"]);
                    }



                    /*
                    try
                    {
                        maintenanceItem.InspectionDate = Convert.ToDateTime(dr["inspection_date"]);
                        maintenanceItem.FromInspection = true;
                        maintenanceItem.UserEntered = false;
                    }
                    catch { }*/
                    if (dr["inspection_date"] != DBNull.Value)
                    {
                        maintenanceItem.InspectionDate = Convert.ToDateTime(dr["inspection_date"]);
                        maintenanceItem.FromInspection = true;
                    }
                    else
                    {
                        maintenanceItem.UserEntered = true;
                    }

                    maintenanceItem.Recommender = dr["recommend_by"].ToString().Trim();
                    maintenanceItem.Priority = dr["stmc_acpry_nm"].ToString().Trim();
                    maintenanceItem.EstimatedCost = dr["estimated_amount"].ToString().Replace("$", "").Trim();

                    /*
                    try
                    {
                        maintenanceItem.EstimatedDate = Convert.ToDateTime(dr["estimated_date"]);
                    }
                    catch { }*/
                    if (dr["estimated_date"] != DBNull.Value)
                    {
                        maintenanceItem.EstimatedDate = Convert.ToDateTime(dr["estimated_date"]);
                    }

                    maintenanceItem.ItemComments = dr["comments"].ToString().Trim();

                    maintenanceItems.Add(maintenanceItem);
                    previousStructureId = currentStructureId;
                }
            }

            return maintenanceItems;
        }

        public double GetVerticalClearanceOnMin(string strId)
        {
            double minVCOn = -1;
            string qry = @"
                                select clrn_minm_vert_dis from 
                                    (
                                        select clrn_minm_vert_dis 
                                        from dot1stro.dt_strc_clrn
                                        where strc_id = :strId
                                          and strc_clrn_ty in ('OC','ON')
                                        order by clrn_minm_vert_dis asc
                                    ) 
                                where rownum = 1
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["CLRN_MINM_VERT_DIS"] != DBNull.Value)
            {
                minVCOn = Convert.ToDouble(dt.Rows[0]["CLRN_MINM_VERT_DIS"]);
            }

            return minVCOn;
        }

        public double GetDesiredVerticalClearanceUnder(string functionalClassificationUnder, string routeSystemUnder)
        {
            double desiredVerticalClearanceUnder = 14.75;
            string qry = @"
                            select DesiredVerticalClearanceUnder
                            from DesiredVerticalClearanceUnder
                            where FunctionalClassificationUnder = @functionalClassificationUnder
                                and RouteSystemUnder = @routeSystemUnder
                         ";

            SqlParameter[] prm = new SqlParameter[2];
            prm[0] = new SqlParameter("@functionalClassificationUnder", SqlDbType.VarChar);
            prm[0].Value = functionalClassificationUnder;
            prm[1] = new SqlParameter("@routeSystemUnder", SqlDbType.VarChar);
            prm[1].Value = routeSystemUnder;
            DataTable dt = ExecuteSelect(qry, prm, samConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                desiredVerticalClearanceUnder = Convert.ToDouble(dt.Rows[0]["DesiredVerticalClearanceUnder"]);
            }

            return desiredVerticalClearanceUnder;
        }

        public double GetVerticalClearanceUnderMin(string strId, string serviceFeatureUnderTypeCode = "")
        {
            double minVCUnder = -1;
            string qry = "";

            if (serviceFeatureUnderTypeCode.Equals("02") || serviceFeatureUnderTypeCode.Equals("18")) //Railroad
            {
                qry = @"
                                select clrn_rr_vert_dis from 
                                    (
                                        select clrn_rr_vert_dis 
                                        from dot1stro.dt_strc_clrn
                                        where strc_id = :strId
                                          and strc_clrn_ty in ('UC','UN')
                                        order by clrn_rr_vert_dis asc
                                    ) 
                                where rownum = 1
                            ";

                OracleParameter[] prms = new OracleParameter[1];
                prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                prms[0].Value = strId;
                DataTable dt = ExecuteSelect(qry, prms, hsiConn);

                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["CLRN_RR_VERT_DIS"] != DBNull.Value)
                {
                    minVCUnder = Convert.ToDouble(dt.Rows[0]["CLRN_RR_VERT_DIS"]);
                }
            }
            /*
            else if (serviceFeatureUnderTypeCode.Equals("17") || serviceFeatureUnderTypeCode.Equals("19")) // Hwy/RR or Hwy/RR/Water
            {
                qry = @"
                                        select clrn_minm_vert_dis, clrn_rr_vert_dis 
                                        from dot1stro.dt_strc_clrn
                                        where strc_id = :strId
                                          and strc_clrn_ty in ('UC','UN')
                            ";

                OracleParameter[] prms = new OracleParameter[1];
                prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                prms[0].Value = strId;
                DataTable dt = ExecuteSelect(qry, prms, hsiConn);

                if (dt != null && dt.Rows.Count > 0)
                {
                    List<double> clearances = new List<double>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            clearances.Add(Convert.ToDouble(dr["CLRN_MINM_VERT_DIS"]));
                        }
                        catch { }

                        try
                        {
                            clearances.Add(Convert.ToDouble(dr["CLRN_RR_VERT_DIS"]));
                        }
                        catch { }
                    }

                    try
                    {

                        minVCUnder = clearances.Min();
                    }
                    catch { }
                }
            }*/
            else
            {
                qry = @"
                                select clrn_minm_vert_dis from 
                                    (
                                        select clrn_minm_vert_dis 
                                        from dot1stro.dt_strc_clrn
                                        where strc_id = :strId
                                          and strc_clrn_ty in ('UC','UN')
                                        order by clrn_minm_vert_dis asc
                                    ) 
                                where rownum = 1
                            ";

                OracleParameter[] prms = new OracleParameter[1];
                prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
                prms[0].Value = strId;
                DataTable dt = ExecuteSelect(qry, prms, hsiConn);

                if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["CLRN_MINM_VERT_DIS"] != DBNull.Value)
                {
                    minVCUnder = Convert.ToDouble(dt.Rows[0]["CLRN_MINM_VERT_DIS"]);
                }
            }

            return minVCUnder;
        }

        public double GetWholeStructureDeckArea(string strId)
        {
            double deckArea = 0;
            string mainStrId = strId.Substring(0, 7);
            List<string> unitBridges = GetUnitBridges(mainStrId);

            foreach (var unitBridge in unitBridges)
            {
                deckArea += GetDeckArea(unitBridge);
            }

            return deckArea;
        }

        public double GetDeckArea(string strId)
        {
            double deckArea = 0;
            string qry = @"
                                select bgdk_area
                                from dot1stro.dt_brdg
                                where strc_id = :strId
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                try
                {
                    deckArea = Convert.ToDouble(dt.Rows[0]["BGDK_AREA"]);
                }
                catch { }
            }

            return deckArea;
        }

        public List<string> GetUnitBridges(string mainStrId)
        {
            List<string> unitBridges = new List<string>();
            string qry = @"
                                select strc_id
                                from dot1stro.dt_strc
                                where strc_id like :mainStrId || '%'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("mainStrId", OracleDbType.Varchar2);
            prms[0].Value = mainStrId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    unitBridges.Add(dr["STRC_ID"].ToString());
                }
            }

            return unitBridges;
        }

        public string GetHighwayFeatureDesignationUnder(string strId)
        {
            string highwayFeatureDesignation = "";
            string qry = @"
                            select hwy_ftr_dsgt_cd
                                from dot1stro.dt_strc_rte r
                                where r.strc_id = :strId
                                    and r.strc_rte_prmy_fl = 'Y'
                                    and r.strc_rte_ouf = 'U'
                                    
                            ";
            //and r.hwy_on_inv_rte in ('NHS','NHI')
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                highwayFeatureDesignation = dt.Rows[0]["HWY_FTR_DSGT_CD"].ToString();
            }

            return highwayFeatureDesignation;
        }

        public string GetServiceFeatureUnderTypeCode(string strId)
        {
            string serviceFeatureUnder = "";
            string qry = @"
                            select strc_srvc_tycd
                                from dot1stro.dt_strc_srvc_ftr r
                                where r.strc_id = :strId
                                    and r.strc_srvc_ty_ouf = 'U'
                                    
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                serviceFeatureUnder = dt.Rows[0]["STRC_SRVC_TYCD"].ToString();
            }

            return serviceFeatureUnder;
        }

        public string GetHighwayFeatureDesignationOn(string strId)
        {
            string highwayFeatureDesignation = "";
            string qry = @"
                            select hwy_ftr_dsgt_cd
                                from dot1stro.dt_strc_rte r
                                where r.strc_id = :strId
                                    and r.strc_rte_prmy_fl = 'Y'
                                    and r.strc_rte_ouf = 'O'
                                    
                            ";
            //and r.hwy_on_inv_rte in ('NHS','NHI')
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                highwayFeatureDesignation = dt.Rows[0]["HWY_FTR_DSGT_CD"].ToString();
            }

            return highwayFeatureDesignation;
        }

        public bool IsNhs(string strId)
        {
            bool isNhs = false;
            string qry = @"
                            select ftr_nm
                                from dot1stro.dt_strc_rte r
                                where r.strc_id = :strId
                                    and r.strc_rte_prmy_fl = 'Y'
                                    and r.strc_rte_ouf = 'O'
                                    and r.hwy_on_inv_rte in ('NHS','NHI')
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                isNhs = true;
            }

            return isNhs;
        }

        public bool IsStaa(string strId)
        {
            bool isStaa = false;
            string qry = @"
                            select ftr_nm
                            from dot1stro.dt_strc_rte r
                            where r.strc_id = :strId
                                and r.strc_rte_prmy_fl = 'Y'
                                and r.strc_rte_ouf = 'O'
                                and r.dsgnd_natl_ntwk_fl in ('1')
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                isStaa = true;
            }

            return isStaa;
        }

        public bool IsStrahnet(string strId)
        {
            bool isStrahnet = false;
            string qry = @"
                            select ftr_nm
                            from dot1stro.dt_strc_rte r
                            where r.strc_id = :strId
                                and r.strc_rte_prmy_fl = 'Y'
                                and r.strc_rte_ouf = 'O'
                                and r.stgc_hwy_ntwk_dsgt in ('1','2')
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0)
            {
                isStrahnet = true;
            }

            return isStrahnet;
        }

        public int GetAdtFutYr(string strId)
        {
            int adtFutYr = 0;
            string qry = @"
                                select brdg_adt_fut_yr
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'O'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BRDG_ADT_FUT_YR"] != DBNull.Value)
            {
                adtFutYr = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_FUT_YR"]);
            }

            return adtFutYr;
        }

        public int GetAdtFut(string strId)
        {
            int adtFut = 0;
            string qry = @"
                                select brdg_adt_fut
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'O'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BRDG_ADT_FUT"] != DBNull.Value)
            {
                adtFut = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_FUT"]);
            }

            return adtFut;
        }

        public int GetAdtYr(string strId)
        {
            int adtYr = 0;
            string qry = @"
                                select brdg_adt_yr
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'O'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BRDG_ADT_YR"] != DBNull.Value)
            {
                adtYr = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_YR"]);
            }

            return adtYr;
        }

        public int GetAdt(string strId)
        {
            int adt = 0;
            string qry = @"
                                select brdg_adt_cnt
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'O'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BRDG_ADT_CNT"] != DBNull.Value)
            {
                try
                {
                    adt = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_CNT"]);
                }
                catch { }
            }

            return adt;
        }

        public int GetAdtt(string strId)
        {
            int adtt = 0;
            string qry = @"
                                select brdg_adt_trk
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'O'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BRDG_ADT_TRK"] != DBNull.Value)
            {
                try
                {
                    adtt = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_TRK"]);
                }
                catch { }
            }

            return adtt;
        }

        public int GetAdtUnder(string strId)
        {
            int adt = 0;
            string qry = @"
                                select brdg_adt_cnt
                                from dot1stro.dt_brdg_adt
                                where strc_id = :strId
                                    and brdg_adt_ouf = 'U'
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["BRDG_ADT_CNT"] != DBNull.Value)
            {
                try
                {
                    adt = Convert.ToInt32(dt.Rows[0]["BRDG_ADT_CNT"]);
                }
                catch { }
            }

            return adt;
        }

        public List<float> GetDeckWidths(string strId)
        {
            List<float> deckWidths = new List<float>();

            string qry = @"
                                select bgab_deck_wd
                                from dot1stro.dt_brdg_abtm
                                where strc_id = :strId
                            ";
            OracleParameter[] prms = new OracleParameter[1];
            prms[0] = new OracleParameter("strId", OracleDbType.Varchar2);
            prms[0].Value = strId;
            DataTable dt = ExecuteSelect(qry, prms, hsiConn);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row["BGAB_DECK_WD"] != DBNull.Value)
                    {
                        deckWidths.Add(Convert.ToSingle(row["BGAB_DECK_WD"]));
                    }
                }
            }

            return deckWidths;
        }

        private string ConvertToQuotedList(string sep, string[] items)
        {
            return String.Join(sep, items.Select(x => String.Format("\'{0}\'", x)));
        }
        #endregion 
    }
}

/*
select StrcInsp.strc_id,
                                        stin_dt,
                                        stin_deck_cond_rtg,
                                        stin_spsr_cond_rtg,
                                        stin_sbsr_cond_rtg,
                                        stin_culv_rtg_cd
                                from dot1stro.dt_strc_insp StrcInsp
                                where StrcInsp.strc_id = :strId
                                    and StrcInsp.stin_dt = (select max(stin_dt) MaxDate
                                                                from dot1stro.dt_strc_insp MaxPrev
                                                                where StrcInsp.strc_id = MaxPrev.strc_id
                                                                    and extract(year from stin_dt) = :year)



*/