using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class Database
    {
        public static string impersonationDomain = ConfigurationManager.AppSettings["ImpersonationDomain"];
        public static string impersonationUserId = ConfigurationManager.AppSettings["ImpersonationUser"];
        public static string encryptedImpersonationPassword = ConfigurationManager.AppSettings["ImpersonationPassword"];
        public static string decryptedImpersonationPassword = CryptorEngineService.Decrypt(encryptedImpersonationPassword, true);
        public static string certificationRootFolder = ConfigurationManager.AppSettings["certificationRootFolder"];
        public static string certificationDirectory = ConfigurationManager.AppSettings["certificationDirectory"];
        public static string bosCdTemplate = ConfigurationManager.AppSettings["bosCdTemplate"];
        public static string tempDirectory = ConfigurationManager.AppSettings["tempDirectory"];
        public static string bosCdSignature = ConfigurationManager.AppSettings["bosCdSignature"];
        public static bool enableBox = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableBox"]);
        public static bool enableHsis = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableHsis"]);
        public static string applicationMode = ConfigurationManager.AppSettings["ApplicationMode"].ToUpper();
        public static string wisamsExecutablePath = ConfigurationManager.AppSettings["WisamsExecutablePath"];
        public static string fiipsQueryToolExecutablePath = ConfigurationManager.AppSettings["FiipsQueryToolExecutablePath"];
        public static List<UserAccount> precertificationLiaisons = new List<UserAccount>();
        public static string precertificationLiaisonsEmails = "";
        public static List<UserAccount> certificationLiaisons = new List<UserAccount>();
        public static string certificationsLiaisonsEmails = "";
        public static List<UserAccount> certificationSupervisors = new List<UserAccount>();
        public static string certificationSupervisorsEmails = "";
        public readonly object databaseLock = new object();

        // SQL Server database
        public static string wisamsDatabase = ConfigurationManager.AppSettings["WisamsDatabase"];
        public SqlConnection wisamsConnection { get; set; }
        public static SqlConnectionStringBuilder wisamsCsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings[wisamsDatabase].ConnectionString);

        // Decrypt the database password and create connection string
        public static string wisamsPassword = CryptorEngineService.Decrypt(wisamsCsb.Password, true);
        public string wisamsConnectionString { get; set; }

        // Oracle databases
        public OracleConnection hsiConnection { get; set; }
        public static SqlConnectionStringBuilder hsiCsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["HsiProdOra"].ConnectionString);
        public static string hsiPassword = CryptorEngineService.Decrypt(hsiCsb.Password, true);
        public string hsiConnectionString { get; set; }

        // Presets
        public List<string> complexStructureConfigurations { get; set; }
        public int currentFiscalYear { get; set; }
        public int currentProjectYear { get; set; }
        public int projectCounter { get; set; }
        public int UnapprovedWindowCurrentFyPlus { get; set; }
        public string myDir { get; set; }

        // Work concepts and projects
        public List<WorkConcept> eligibleWorkConcepts { get; set; }
        public List<WorkConcept> fiipsWorkConcepts { get; set; }
        public List<Project> structureProjects { get; set; }
        public List<Project> fiipsProjects { get; set; }
        public List<WorkConcept> allWorkConcepts { get; set; }
        public Wisdot.Bos.Dw.Database WarehouseDatabase { get; set; }

        private static Dw.Database warehouseDatabase = new Dw.Database();
        private static IDatabaseService serv = new DatabaseService();

        public Database()
        {
            Initialize();
        }

        public Database(string database)
        {
            Initialize();
            currentFiscalYear = serv.GetFiscalYear();
            myDir = AppDomain.CurrentDomain.BaseDirectory;

            switch (database.ToUpper())
            {
                case "WISAMS":
                    wisamsConnection = new SqlConnection(wisamsConnectionString);
                    break;
            }
        }

        private void Initialize()
        {
            wisamsConnectionString = ConfigurationManager.ConnectionStrings[wisamsDatabase].ConnectionString.Replace(wisamsCsb.Password, wisamsPassword);
            hsiConnectionString = ConfigurationManager.ConnectionStrings["HsiProdOra"].ConnectionString.Replace(hsiCsb.Password, hsiPassword);
            complexStructureConfigurations = new List<string>() { "34", "60", "80", "30", "31", "33", "69", "39", "32", "13", "90", "50", "61" };
            currentProjectYear = 1;
            projectCounter = 1;
            myDir = "";
            eligibleWorkConcepts = new List<WorkConcept>();
            fiipsWorkConcepts = new List<WorkConcept>();
            structureProjects = new List<Project>();
            fiipsProjects = new List<Project>();
            allWorkConcepts = new List<WorkConcept>();

        }

        
    }
}
