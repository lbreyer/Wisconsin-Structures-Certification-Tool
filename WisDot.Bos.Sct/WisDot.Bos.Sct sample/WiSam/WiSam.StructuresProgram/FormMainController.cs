using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.IO;
using BOS.Box;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using Structure = WisDot.Bos.Sct.Core.Domain.Models.Structure;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
//using WiSam.Business;
//using WiSam.Data;

namespace WiSam.StructuresProgram
{
    class FormMainController
    {
        private Dw.Database warehouseDatabase = new Dw.Database();
        private FormMain formMain;
        private DatabaseService dataServ;
        private Database database;
        private FileManagerService fileManager;
        private UserAccount userAccount;
        private DataGridView dataGridViewP5;
        private DataGridView dataGridViewP6;
        private DataGridView dataGridViewP7;
        private DataGridView dataGridViewP8;
        private DataGridView dataGridViewP9;
        private DataGridView dataGridViewP10;
        private DataGridView dataGridViewP11;
        private DataGridView dataGridViewNewProject;
        private DataGridView dataGridViewProjectsList;
        private List<WorkConcept> allWorkConcepts;
        private List<WorkConcept> eligibleWorkConcepts;
        private List<WorkConcept> quasicertifiedWorkConcepts;
        private List<WorkConcept> precertifiedApprovedWorkConcepts;
        private List<WorkConcept> precertifiedUnapprovedWorkConcepts;
        private List<WorkConcept> certifiedWorkConcepts;
        private List<WorkConcept> fiipsWorkConcepts;
        private List<WorkConcept> selectedWorkConcepts = new List<WorkConcept>();
        private List<string> selectedStructureIds = new List<string>();
        private List<string> selectedProjects = new List<string>();
        private List<Project> existingStructureProjects;
        private List<Project> newStructureProjects;
        private List<Project> existingFiipsProjects;
        private List<WorkConcept> primaryWorkConcepts;
        private DataGridView dataGridViewProjectWorkConcepts;
        private DataGridView dataGridViewAllWorkConcepts;
        private DataGridViewComboBoxColumn comboBoxColumnCertifiedWorkConcept;
        private ComboBox comboBoxNewWorkConcept;
        private ComboBox comboBoxNewWorkConceptReasonCategory;
        private ComboBox comboBoxWisamsReport;
        private GroupBox groupBoxWisamsReports;
        private int currentFiscalYear;
        private int workConceptStartFiscalYear;
        private int workConceptEndFiscalYear;
        private int currentProjectYear = 1;
        private int startEligibilityFiscalYear;
        private int endEligibilityFiscalYear;
        private int startEligibilityProjectYear;
        private int endEligibilityProjectYear;
        private Panel panelMap;
        //private List<Project> structureProjects = new List<Project>();
        private List<Project> fiipsProjects = new List<Project>();
        private TreeView treeViewMap;
        private FormMapping formMapping;
        //bool formStructureIsOpen = false;
        //FormStructure formStructure = null;
        //bool formStructureProjectIsOpen = false;
        //FormStructureProject formStructureProject = null;
        private ExcelReporterService excelReporter = new ExcelReporterService();
        private static IMainService mainServ = new MainService();
        private List<Dw.Report> wisamsReports;

        internal FormMainController(FormMain formMain, UserAccount userAccount, 
                                    DataGridView dataGridViewP5, DataGridView dataGridViewP6,
                                    DataGridView dataGridViewP7, DataGridView dataGridViewP8,
                                    DataGridView dataGridViewP9, DataGridView dataGridViewP10,
                                    DataGridView dataGridViewP11,
                                    DataGridView dataGridViewNewProject,
                                    DataGridView dataGridViewProjectsList,
                                    List<Project> existingStructureProjects,
                                    List<Project> newStructureProjects,
                                    List<WorkConcept> primaryWorkConcepts,
                                    ComboBox comboBoxNewWorkConcept,
                                    ComboBox comboBoxNewWorkConceptReasonCategory,
                                    List<WorkConcept> allWorkConcepts,
                                    List<WorkConcept> eligibleWorkConcepts,
                                    DataGridView dataGridViewAllWorkConcepts,
                                    List<WorkConcept> fiipsWorkConcepts,
                                    List<Project> existingFiipsProjects,
                                    List<WorkConcept> quasicertifiedWorkConcepts,
                                    List<WorkConcept> precertifiedApprovedWorkConcepts,
                                    List<WorkConcept> precertifiedUnapprovedWorkConcepts,
                                    List<WorkConcept> certifiedWorkConcepts,
                                    Panel panelMap,
                                    ComboBox comboBoxWisamsReport,
                                    GroupBox groupBoxWisamsReports
                                    )
        {
            this.formMain = formMain;
            this.dataGridViewP5 = dataGridViewP5;
            this.dataGridViewP6 = dataGridViewP6;
            this.dataGridViewP7 = dataGridViewP7;
            this.dataGridViewP8 = dataGridViewP8;
            this.dataGridViewP9 = dataGridViewP9;
            this.dataGridViewP10 = dataGridViewP10;
            this.dataGridViewP11 = dataGridViewP11;
            this.dataGridViewNewProject = dataGridViewNewProject;
            this.dataGridViewProjectsList = dataGridViewProjectsList;
            this.dataGridViewAllWorkConcepts = dataGridViewAllWorkConcepts;
            this.panelMap = panelMap;
            this.userAccount = userAccount;
            this.existingStructureProjects = existingStructureProjects;
            this.newStructureProjects = newStructureProjects;
            this.primaryWorkConcepts = primaryWorkConcepts;
            this.allWorkConcepts = allWorkConcepts;
            this.eligibleWorkConcepts = eligibleWorkConcepts;
            this.fiipsWorkConcepts = fiipsWorkConcepts;
            //this.dataGridViewProjectWorkConcepts = dataGridViewProjectWorkConcepts;
            //this.comboBoxColumnCertifiedWorkConcept = comboBoxColumnCertifiedWorkConcept;
            this.comboBoxNewWorkConcept = comboBoxNewWorkConcept;
            this.comboBoxNewWorkConceptReasonCategory = comboBoxNewWorkConceptReasonCategory;
            this.fiipsWorkConcepts = fiipsWorkConcepts;
            this.existingFiipsProjects = existingFiipsProjects;
            this.quasicertifiedWorkConcepts = quasicertifiedWorkConcepts;
            this.precertifiedApprovedWorkConcepts = precertifiedApprovedWorkConcepts;
            this.precertifiedUnapprovedWorkConcepts = precertifiedUnapprovedWorkConcepts;
            this.certifiedWorkConcepts = certifiedWorkConcepts;
            this.comboBoxWisamsReport = comboBoxWisamsReport;
            this.groupBoxWisamsReports = groupBoxWisamsReports;
            currentFiscalYear = GetCurrentFiscalYear();
            workConceptStartFiscalYear = currentFiscalYear;
            workConceptEndFiscalYear = currentFiscalYear + 11;
            startEligibilityFiscalYear = currentFiscalYear + 5;
            endEligibilityFiscalYear = currentFiscalYear + 11;
            // currentProjectYear is fixed at 1
            startEligibilityProjectYear = currentProjectYear + 5;
            endEligibilityProjectYear = currentProjectYear + 11;
            dataServ = new DatabaseService();
            database = new Database("WISAMS");
            bool connectedToWiSamsDatabase = dataServ.OpenDatabaseConnection("WISAMS");

            if (!connectedToWiSamsDatabase)
            {
                MessageBox.Show("Unable to connect to the Structures Data Warehouse. Please inform BOS.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bool connectedToHsiDatabase = false;

                if (dataServ.EnableHsis())
                {
                    connectedToHsiDatabase = dataServ.OpenDatabaseConnection("HSI");
                }
              
                if (dataServ.EnableHsis() && !connectedToHsiDatabase)
                {
                    MessageBox.Show("Unable to connect to the HSIS database. Please inform BOS.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (fileManager == null)
                {
                    fileManager = new FileManagerService(dataServ);
                }
            }
        }

        internal string GetWisamsExecutablePath()
        {
            return dataServ.GetWisamsExecutablePath();
        }

        internal string GetFiipsQueryToolExecutablePath()
        {
            return dataServ.GetFiipsQueryToolExecutablePath();
        }

        internal int GetProjectYearBasedOnFiscalYear(int fiscalYear)
        {
            return (fiscalYear - currentFiscalYear);
        }

        internal bool ValidateFiscalYear(int fiscalYear)
        {
            return mainServ.ValidateFiscalYear(fiscalYear, workConceptStartFiscalYear, workConceptEndFiscalYear);
        }

        internal int GetWorkConceptStartFiscalYear()
        {
            return workConceptStartFiscalYear;
        }

        internal int GetWorkConceptEndFiscalYear()
        {
            return workConceptEndFiscalYear;
        }

        internal DataGridView GetDataGridViewBasedOnFiscalYear(int fiscalYear)
        {
            int projectYear = (fiscalYear - currentFiscalYear);
            return GetDataGridView(projectYear);
        }

        internal DataGridView GetDataGridView(int projectYear)
        {
            DataGridView dgv = null;
            switch (projectYear)
            {
                case 5:
                    dgv = dataGridViewP5;
                    break;
                case 6:
                    dgv = dataGridViewP6;
                    break;
                case 7:
                    dgv = dataGridViewP7;
                    break;
                case 8:
                    dgv = dataGridViewP8;
                    break;
                case 9:
                    dgv = dataGridViewP9;
                    break;
                case 10:
                    dgv = dataGridViewP10;
                    break;
                case 11:
                    dgv = dataGridViewP11;
                    break;
            }

            return dgv;
        }

        internal void DoFormLoad()
        {
            if (userAccount.IsAdministrator || userAccount.IsSuperUser || userAccount.IsSuperRead)
            {
                eligibleWorkConcepts = dataServ.GetEligibleWorkConcepts(workConceptStartFiscalYear, workConceptEndFiscalYear);
                fiipsWorkConcepts = dataServ.GetFiipsWorkConcepts(workConceptStartFiscalYear, workConceptEndFiscalYear);
                // TODO: Redo Fiips projects query 
                //existingFiipsProjects = database.GetFiipsProjects(workConceptStartFiscalYear, workConceptEndFiscalYear);
                existingFiipsProjects = dataServ.GetProjectsInFiips(workConceptStartFiscalYear, workConceptEndFiscalYear, fiipsWorkConcepts);
                //var wcs = fps.Sum(fp => fp.WorkConcepts.Count());
                //var oldExistingStructureProjects = database.GetStructureProjects(workConceptStartFiscalYear, workConceptEndFiscalYear);
                //var sps = database.GetProjectsInSct(workConceptStartFiscalYear, workConceptEndFiscalYear);
                existingStructureProjects = dataServ.GetProjectsInSct(workConceptStartFiscalYear, workConceptEndFiscalYear);

                /*
                var missing = new List<Project>();
                foreach (var p in oldExistingStructureProjects)
                {
                    if (!existingStructureProjects.Any(e => e.FosProjectId.Equals(p.FosProjectId)))
                    {
                        missing.Add(p);
                    }
                }*/
                //var notInSps = existingStructureProjects.Where(esp => !sps.Any(sp => sp.ProjectDbId == esp.ProjectDbId || (sp.FosProjectId != null && esp.FosProjectId != null && sp.FosProjectId.Equals(esp.FosProjectId))));
                //var notInEsp = sps.Where(sp => !existingStructureProjects.Any(esp => esp.ProjectDbId == sp.ProjectDbId));
            }
            else
            {
                eligibleWorkConcepts = dataServ.GetEligibleWorkConcepts(workConceptStartFiscalYear, workConceptEndFiscalYear, userAccount.Office);
                fiipsWorkConcepts = dataServ.GetFiipsWorkConcepts(workConceptStartFiscalYear, workConceptEndFiscalYear, userAccount.Office.Substring(2));
                //existingFiipsProjects = database.GetFiipsProjects(workConceptStartFiscalYear, workConceptEndFiscalYear, userAccount.Office.Substring(2));
                existingFiipsProjects = dataServ.GetProjectsInFiips(workConceptStartFiscalYear, workConceptEndFiscalYear, fiipsWorkConcepts, userAccount.Office.Substring(2));
                //var wcs = fps.Sum(fp => fp.WorkConcepts.Count());
                //existingStructureProjects = database.GetStructureProjects(workConceptStartFiscalYear, workConceptEndFiscalYear, userAccount.Office.Substring(2));
                existingStructureProjects = dataServ.GetProjectsInSct(workConceptStartFiscalYear, workConceptEndFiscalYear, userAccount.Office.Substring(2));
            }

            // Render Eligible grids
            int gridCounter = 5;
            for (int i = workConceptStartFiscalYear + 5; i <= workConceptEndFiscalYear; i++)
            {
                DataGridView dgv = GetDataGridView(gridCounter);
                RenderEligibilityListGrid(eligibleWorkConcepts.Where(e => e.FiscalYear == i).ToList(), dgv);
                gridCounter++;
            }

            //DoClearGridSelections();

            // Render existing projects
            var allProjects = existingStructureProjects.Concat(existingFiipsProjects);
            RenderProjectsListGrid(allProjects.ToList(), dataGridViewProjectsList);
            //RenderProjectsListGrid(existingStructureProjects, dataGridViewProjectsList);
            //RenderProjectsListGrid(existingFiipsProjects, dataGridViewProjectsList);
            allWorkConcepts = dataServ.GetAllWorkConcepts();
            // Populate "Work Concepts" dropdowns
            primaryWorkConcepts = dataServ.GetPrimaryWorkConcepts();
            PopulateProposedWorkConcepts();
            PopulateProposedWorkConceptJustifications();
            RenderAllWorkConceptsGrid();
            RenderMap();
            string userRegion = "";

            if (!String.IsNullOrEmpty(userAccount.Office) && !userAccount.Office.Equals("BOS"))
            {
                userRegion = userAccount.Office.Substring(2, 2).ToUpper();
            }

            wisamsReports = warehouseDatabase.GetWisamsReports();
            foreach (var report in wisamsReports)
            {
                bool add = false;

                if (userAccount.IsSuperUser || userAccount.IsSuperRead || userAccount.IsAdministrator)
                {
                    add = true;
                }
                else
                {
                    if (report.ReportCategory.Equals("wisams-nc-state") &&
                            userRegion.Equals("NC"))
                    {
                        add = true;

                    }
                    else if (report.ReportCategory.Equals("wisams-ne-state") &&
                                userRegion.Equals("NE"))
                    {
                        add = true;
                    }
                    else if (report.ReportCategory.Equals("wisams-nw-state") &&
                                userRegion.Equals("NW"))
                    {
                        add = true;
                    }
                    else if (report.ReportCategory.Equals("wisams-se-state") &&
                                userRegion.Equals("SE"))
                    {
                        add = true;
                    }
                    else if (report.ReportCategory.Equals("wisams-sw-state") &&
                                userRegion.Equals("SW"))
                    {
                        add = true;

                    }
                }

                if (add)
                {
                    comboBoxWisamsReport.Items.Add(String.Format("{0} ({1}):{2}", report.ReportName, report.ReportDate.ToString("yyyy/MM/dd"), report.BoxId));
                }
            }

            if (comboBoxWisamsReport.Items.Count > 0)
            {
                groupBoxWisamsReports.Visible = true;
                comboBoxWisamsReport.SelectedIndex = 0;
            }
            else
            {
                groupBoxWisamsReports.Visible = false;
            }
        }

        internal bool ValidateStructureId(string structureId, UserAccount userAccount)
        {
            return mainServ.ValidateStructureId(structureId, userAccount);
        }

        internal bool IsFormOpen(string formName)
        {
            return mainServ.IsFormOpen(formName);
        }

        internal void OpenExcelFile(string filePath)
        {
            mainServ.OpenExcelFile(filePath);
        }

        internal List<int> FilterProjectsListGrid(StructuresProgramType.ProjectStatus status, bool show, DataGridView dataGridViewProjectsList)
        {
            return mainServ.FilterProjectsListGrid(status, show, dataGridViewProjectsList);
        }

        internal void RefreshProjectsListGrid()
        {
            dataGridViewProjectsList.Rows.Clear();
            RenderProjectsListGrid(existingStructureProjects, dataGridViewProjectsList);
            RenderProjectsListGrid(existingFiipsProjects, dataGridViewProjectsList);
        }

        internal void RenderAllWorkConceptsGrid()
        {
            dataGridViewAllWorkConcepts.Rows.Clear();
            List<WorkConcept> unapprovedWorkConcepts = new List<WorkConcept>();
            List<WorkConcept> precertifiedWorkConcepts = new List<WorkConcept>();
            List<WorkConcept> certifiedWorkConcepts = new List<WorkConcept>();

            foreach (var esp in existingStructureProjects)
            {
                foreach (var wc in esp.WorkConcepts)
                {
                    switch (wc.Status)
                    {
                        case StructuresProgramType.WorkConceptStatus.Unapproved:
                            unapprovedWorkConcepts.Add(wc);
                            break;
                        case StructuresProgramType.WorkConceptStatus.Precertified:
                            precertifiedWorkConcepts.Add(wc);
                            break;
                        case StructuresProgramType.WorkConceptStatus.Certified:
                        case StructuresProgramType.WorkConceptStatus.Quasicertified:
                            certifiedWorkConcepts.Add(wc);
                            break;
                    }
                }
            }

            RenderAllWorkConceptsGrid(eligibleWorkConcepts.Where(el => !el.FromProposedList).ToList(), dataGridViewAllWorkConcepts);
            var ps = eligibleWorkConcepts.Where(el => el.FromProposedList).ToList();
            RenderAllWorkConceptsGrid(eligibleWorkConcepts.Where(el => el.FromProposedList && el.ProjectDbId == 0).ToList(), dataGridViewAllWorkConcepts, "darkorange");
            RenderAllWorkConceptsGrid(unapprovedWorkConcepts, dataGridViewAllWorkConcepts, "red");
            RenderAllWorkConceptsGrid(precertifiedWorkConcepts, dataGridViewAllWorkConcepts, "yellow");
            RenderAllWorkConceptsGrid(certifiedWorkConcepts, dataGridViewAllWorkConcepts, "lightgreen");
            RenderAllWorkConceptsGrid(fiipsWorkConcepts, dataGridViewAllWorkConcepts, "lightskyblue");
        }

        internal string GetApplicationMode()
        {
            return dataServ.GetApplicationMode();
        }

        public void LogUserActivity(int userDbId, string activity)
        {
            dataServ.LogUserActivity(userDbId, activity);
        }

        internal void DoCloseDatabaseConnection(string dataSource)
        {
            dataServ.CloseDatabaseConnection(dataSource);
        }

        /*
        internal void DoWorkConceptAdd()
        {
            WorkConcept wc = new WorkConcept();
            wc.WorkConceptCode = "00";

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals("FormStructureProject"))
                {
                    formStructureProjectIsOpen = true;
                    formStructureProject = (FormStructureProject)form;
                    break;
                }
            }

            if (formStructureProjectIsOpen)
            {
                formStructureProject.RefreshForm(wc);
            }
            else
            {
                formStructureProject = new FormStructureProject(userAccount, database, wc, primaryWorkConcepts, eligibleWorkConcepts,
                                                                quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                fiipsWorkConcepts, existingStructureProjects, fiipsProjects, 
                                                                allWorkConcepts, formMapping, treeViewMap);
            }

            formStructureProject.WindowState = FormWindowState.Minimized;
            formStructureProject.TopMost = false;
            formStructureProject.StartPosition = FormStartPosition.CenterScreen;
            formStructureProject.Show();
        }*/

        /*
        internal void DoWorkConceptAdd(int workConceptDbId)
        {
            WorkConcept wc = eligibleWorkConcepts.Where(e => e.WorkConceptDbId == workConceptDbId).First();
            FormStructureProject formStructureProject = null;
            bool isFormOpen = false;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals("FormStructureProject"))
                {
                    isFormOpen = true;
                    formStructureProject = (FormStructureProject)form;
                    break;
                }
            }

            if (isFormOpen)
            {
                WorkConcept workConcept = new WorkConcept(wc);
                formStructureProject.RefreshForm(workConcept);
            }
            else
            {
                formStructureProject = new FormStructureProject(userAccount, database, wc, primaryWorkConcepts, eligibleWorkConcepts,
                                                                quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                fiipsWorkConcepts, existingStructureProjects, fiipsProjects, allWorkConcepts, formMapping, treeViewMap);
            }

            formStructureProject.WindowState = FormWindowState.Normal;
            formStructureProject.TopMost = false;
            formStructureProject.StartPosition = FormStartPosition.CenterScreen;
            formStructureProject.Show();
        }*/

        internal void UpdateTimeWindows()
        {
            dataServ.UpdateTimeWindows();
        }

        internal void MigrateExcelProjects()
        {
            List<Project> projects = dataServ.MigrateExcelProjects(existingStructureProjects.Where(sp => sp.FromExcel).ToList(), eligibleWorkConcepts);

            foreach (Project project in projects)
            {
                var count = project.WorkConcepts.Count();
            }
        }

        internal void DoProposedWorkConceptAdd(int workConceptDbId, string structureId, string proposedWorkConceptCode,
            string proposedWorkConceptDescription, int fiscalYear, int projectYear, string reasonCategory, string notes)
        {
            Structure structure = dataServ.GetSptStructure(structureId);
            string region = structure.RegionNumber + "-" + structure.Region;
            string regionNumber = structure.RegionNumber;
            int proposedByUserDbId = userAccount.UserDbId;
            string proposedByUserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName); 
            WorkConcept proposedWorkConcept = null;

            if (workConceptDbId == 0)
            {
                proposedWorkConcept = new WorkConcept(0, structureId, region, regionNumber, proposedWorkConceptCode, proposedWorkConceptDescription, fiscalYear,
                        reasonCategory, notes, proposedByUserDbId, proposedByUserFullName, DateTime.Now, true);
                // Insert it into the database and then update the WorkConceptDbId
                proposedWorkConcept.WorkConceptDbId = InsertProposedWorkConcept(proposedWorkConcept);
                eligibleWorkConcepts.Add(proposedWorkConcept);
            }
            else // Update
            {

            }

            // Add proposed wc to the correct grid
            DataGridView dgv = null;
            int gridCounter = 5;

            for (int i = workConceptStartFiscalYear + 5; i <= workConceptEndFiscalYear; i++)
            {
                if (i == fiscalYear)
                {
                    dgv = GetDataGridView(gridCounter);
                    break;
                }
                
                gridCounter++;
            }

            if (dgv != null)
            {
                AddWorkConceptToEligibilityGrid(dgv, proposedWorkConcept);
            }

            // Update nav tree
            formMapping.EditWorkConcept(proposedWorkConcept, "Add");
        }

        internal void DoProposedWorkConceptAdd(WorkConcept wc)
        {
            wc.GeoLocation = dataServ.GetStructureLatLong(wc.StructureId);
            eligibleWorkConcepts.Add(wc);
            dataGridViewNewProject.Rows.Add();
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[1].Value = wc.CertifiedWorkConceptDescription;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[2].Value = 0;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[4].Value = wc.WorkConceptDbId;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[5].Value = wc.CertifiedWorkConceptCode;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[6].Value = wc.ProjectYear;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[7].Value = "F";
            DataGridView dgv = GetDataGridViewBasedOnFiscalYear(wc.FiscalYear);

            if (dgv != null)
            {
                dgv.Rows.Add();
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = wc.CertifiedWorkConceptDescription;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[2].Value = 0;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[4].Value = wc.WorkConceptDbId;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = wc.CertifiedWorkConceptCode;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[6].Value = wc.ProjectYear;
                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Orange;
            }
        }

        internal void DoWorkConceptAdd(string structureId, string workConceptCode, string workConceptDescription, 
                                        int fiscalYear, float priorityScore, int workConceptDbId, int projectYear, bool isFromEligibilityList)
        {
            dataGridViewNewProject.Rows.Add();
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[0].Value = structureId;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[1].Value = workConceptDescription;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[2].Value = priorityScore;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[3].Value = fiscalYear;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[4].Value = workConceptDbId;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[5].Value = workConceptCode;
            dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[6].Value = projectYear;
            
            if (isFromEligibilityList)
            {
                dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[7].Value = "T";
            }
            else
            {
                dataGridViewNewProject.Rows[dataGridViewNewProject.Rows.GetLastRow(0)].Cells[7].Value = "F";
                DataGridView dgv = GetDataGridViewBasedOnFiscalYear(fiscalYear);

                if (dgv != null)
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[0].Value = structureId;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = workConceptDescription;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[2].Value = priorityScore;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[3].Value = fiscalYear;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[4].Value = workConceptDbId;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = workConceptCode;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[6].Value = projectYear;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Orange;
                }

                // Add to eligibleWorkConcepts list
                WorkConcept wc = new WorkConcept();
                wc.StructureId = structureId;
                wc.WorkConceptDbId = workConceptDbId;
                wc.WorkConceptDescription = workConceptDescription;
                wc.PriorityScore = priorityScore;
                wc.FiscalYear = fiscalYear;
                wc.WorkConceptCode = workConceptCode;
                wc.ProjectYear = projectYear;
                wc.FromEligibilityList = false;
                wc.IsQuasicertified = false;
                wc.GeoLocation = dataServ.GetStructureLatLong(structureId);
                eligibleWorkConcepts.Add(wc);
            }
        }
        
        internal void DoProjectSelect(int projectDbid, string fosProjectId, int numberOfStructures, 
                                    string projectImprovementConcept, int structuresCost, int fiscalYear, bool isApproved)
        {
            DataGridView dgv = dataGridViewProjectWorkConcepts;
            Project project = existingStructureProjects.Where(e => e.ProjectDbId == projectDbid).First();
            dgv.Rows.Clear();

            foreach (WorkConcept pwc in primaryWorkConcepts)
            {
                string workConceptDisplay = String.Format("({0}) {1}", pwc.WorkConceptCode, pwc.WorkConceptDescription);
                comboBoxColumnCertifiedWorkConcept.Items.Add(workConceptDisplay);
            }

            foreach (WorkConcept wc in project.WorkConcepts)
            {
                dgv.Rows.Add();
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = wc.WorkConceptDescription;
                string workConceptDisplay = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[2].Value = workConceptDisplay;
                
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[3].Value = wc.IsQuasicertified;
                
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[4].Value = wc.FiscalYear;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = "";
            }
        }

        internal void DoProjectCreate(int projectDbId, string fosProjectId, 
                                    string projectImprovementConcept, int fiscalYear, string status)
        {
            Project newProject = new Project();
            newProject.ProjectDbId = projectDbId;
            newProject.FosProjectId = fosProjectId;
            newProject.FiipsImprovementConcept = projectImprovementConcept;
            newProject.FiscalYear = fiscalYear;
            newProject.StructuresCost = 0;
            newProject.NumberOfStructures = 0;
            newProject.IsQuasicertified = true;
            newProject.Status = StructuresProgramType.ProjectStatus.Unapproved;

            foreach (DataGridViewRow row in dataGridViewNewProject.Rows)
            {
                string structureId = row.Cells[0].Value.ToString();
                string workConceptDescription = row.Cells[1].Value.ToString();
                float priorityScore = Convert.ToSingle(row.Cells[2].Value);
                int workConceptFiscalYear = Convert.ToInt32(row.Cells[3].Value);
                int workConceptDbId = Convert.ToInt32(row.Cells[4].Value);
                string workConceptCode = row.Cells[5].Value.ToString();
                int projectYear = Convert.ToInt32(row.Cells[6].Value);
                WorkConcept wc = new WorkConcept();
                wc.StructureId = structureId;
                wc.WorkConceptDescription = workConceptDescription;
                wc.PriorityScore = priorityScore;
                wc.FiscalYear = workConceptFiscalYear;
                wc.WorkConceptDbId = workConceptDbId;
                wc.WorkConceptCode = workConceptCode;
                wc.ProjectYear = projectYear;
                wc.ProjectDbId = projectDbId;
                wc.StructureProjectFiscalYear = newProject.FiscalYear;
                wc.GeoLocation = dataServ.GetStructureLatLong(structureId);
                wc.MapMarkerType = dataServ.GetMapMarkerType(workConceptCode);
                wc.IsQuasicertified = true;
                wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;

                if (newProject.WorkConcepts.Where(e => e.StructureId.Equals(structureId)).Count() == 0)
                {
                    newProject.NumberOfStructures++;
                }

                wc.Cost = dataServ.GetEligibleWorkConcept(workConceptDbId).Cost;

                // Work concept automatic approval
                //try
                {
                    wc.EarlierFiscalYear = allWorkConcepts.Where(e => e.WorkConceptCode.Equals(workConceptCode)).First().EarlierFiscalYear;
                }
                //catch { };

                //try
                {
                    wc.LaterFiscalYear = allWorkConcepts.Where(e => e.WorkConceptCode.Equals(workConceptCode)).First().LaterFiscalYear;
                }
                //catch { };

                if (newProject.FiscalYear < wc.FiscalYear)
                {
                    if (wc.EarlierFiscalYear != -99)
                    {
                        if (wc.EarlierFiscalYear == -1) // Check WiSAMS DN scenario
                        {

                        }
                        else
                        {
                            if (newProject.FiscalYear < wc.FiscalYear - wc.EarlierFiscalYear)
                            {
                                wc.IsQuasicertified = false;
                            }
                        }
                    }
                }
                else if (newProject.FiscalYear > wc.FiscalYear)
                {
                    if (wc.LaterFiscalYear != -99)
                    {
                        if (wc.LaterFiscalYear == -1) // Check WiSAMS DN scenario
                        {

                        }
                        else
                        {
                            if (newProject.FiscalYear > wc.FiscalYear + wc.LaterFiscalYear)
                            {
                                wc.IsQuasicertified = false;
                            }
                        }
                    }
                }

                if (!wc.IsQuasicertified)
                {
                    newProject.IsQuasicertified = false;
                    wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                }

                newProject.StructuresCost += wc.Cost;
                newProject.WorkConcepts.Add(wc);

                // Add and remove
                switch (wc.Status)
                {
                    case StructuresProgramType.WorkConceptStatus.Unapproved:
                        precertifiedUnapprovedWorkConcepts.Add(wc);
                        break;
                    case StructuresProgramType.WorkConceptStatus.Precertified:
                        precertifiedApprovedWorkConcepts.Add(wc);
                        break;
                }

                RemoveWorkConceptFromEligibleList(wc);
                ColorCodeWorkConceptRow(workConceptDbId, projectYear);
                ColorCodeAllWorkConceptsGridRow(wc);
            }

            newStructureProjects.Add(newProject);
            existingStructureProjects.Add(newProject);

            // Add new project to the projects grid
            List<Project> projects = new List<Project>();
            projects.Add(newProject);
            RenderProjectsListGrid(projects, dataGridViewProjectsList);

            // Refresh All Work Concepts Grid
            RenderAllWorkConceptsGrid();
        }

        internal void DoFormStructureProjectShow(string projectId)
        {
            Project project = null;

            if (projectId.Length == 8)
            {
                try
                {
                    project = existingFiipsProjects.Where(el => el.FosProjectId.Equals(projectId)).First();
                }
                catch { }
            }
            else
            {
                try
                {
                    project = existingStructureProjects.Where(el => el.ProjectDbId == Convert.ToInt32(projectId)).First();
                }
                catch { }
            }

            if (project == null)
            {
                MessageBox.Show("Unable to find project data for " + FormatConstructionId(projectId) + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            formMapping.OpenStructureProject(project);
            /*
            FormStructureProject formStructureProject = null;

            try
            {
                formStructureProject = (FormStructureProject)Utility.GetForm("FormStructureProject");
            }
            catch { }

            if (formStructureProject != null)
            {
                formStructureProject.Close();
            }

            formStructureProject = new FormStructureProject(userAccount, dataServ, project, formMapping);
            Utility.ShowFormStructureProject(formStructureProject);*/
        }

        internal void DoFormStructureShow(string structureId)
        {
            formMapping.OpenStructureWindow(structureId);
        }

        internal void DoClearGridSelections()
        {
            for (int i = 5; i <= 11; i++)
            {
                DataGridView dgv = GetDataGridView(i);

                if (dgv != null)
                {
                    dgv.ClearSelection();
                }
            }
        }

        internal void DoMapProject(string fosId)
        {
            formMapping.SearchProject(fosId);
            ((TabControl)formMain.Controls["tabControlProjectExplorer"]).SelectedIndex = 0;
        }

        internal void DoMapProjects(List<string> projectIds, bool updateResults = true)
        {
            foreach (string projectId in projectIds)
            {
                if (projectId.Length == 8 || projectId.Replace("-", "").Length == 8)
                {
                    formMapping.SearchProject(projectId.Replace("-", ""), updateResults)
;                }
                else
                {
                    formMapping.SearchProject(Convert.ToInt32(projectId), updateResults);
                }
            }

             ((TabControl)formMain.Controls["tabControlProjectExplorer"]).SelectedIndex = 0;
        }

        internal void DoMapProject(int projectId)
        {
            formMapping.SearchProject(projectId);
            ((TabControl)formMain.Controls["tabControlProjectExplorer"]).SelectedIndex = 0;
        }

        internal void DoMapProject(int projectDbId, object sender)
        {
            selectedWorkConcepts.Clear();

            if (projectDbId == 0)
            {
                foreach (DataGridViewRow row in dataGridViewNewProject.Rows)
                {
                    string structureId = row.Cells[0].Value.ToString();
                    string workConceptDescription = row.Cells[1].Value.ToString();
                    double priorityScore = Convert.ToDouble(row.Cells[2].Value);
                    int workConceptFiscalYear = Convert.ToInt32(row.Cells[3].Value);
                    int workConceptDbId = Convert.ToInt32(row.Cells[4].Value);
                    string workConceptCode = row.Cells[5].Value.ToString();
                    int projectYear = Convert.ToInt32(row.Cells[6].Value);
                    WorkConcept wc = eligibleWorkConcepts.Where(e => e.WorkConceptDbId == workConceptDbId).First();
                    wc.GeoLocation = dataServ.GetStructureLatLong(wc.StructureId);
                    selectedWorkConcepts.Add(wc);
                }
            }
            else
            {
                Project project = existingStructureProjects.Where(e => e.ProjectDbId == projectDbId).First();

                foreach (WorkConcept wc in project.WorkConcepts)
                {
                    wc.GeoLocation = dataServ.GetStructureLatLong(wc.StructureId);
                }

                selectedWorkConcepts.AddRange(project.WorkConcepts);
            }

            if (selectedWorkConcepts.Count > 0)
            {
                //bool formMappingIsOpen = false;
                FormMapping formMapping = null;

                /*
                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name.Equals("FormMapping"))
                    {
                        formMappingIsOpen = true;
                        formMapping = (FormMapping)form;
                    }
                }
                */
                if (!formMain.IsFormOpen("FormMapping"))
                {
                    formMapping = new FormMapping(userAccount, currentFiscalYear, endEligibilityFiscalYear, existingFiipsProjects, 
                        existingStructureProjects, certifiedWorkConcepts, precertifiedApprovedWorkConcepts,
                        precertifiedUnapprovedWorkConcepts, quasicertifiedWorkConcepts, eligibleWorkConcepts,
                        selectedWorkConcepts, fiipsWorkConcepts, sender, dataServ, 
                        primaryWorkConcepts, allWorkConcepts, this);
                    formMapping.WindowState = FormWindowState.Normal;
                }
                else
                {
                    formMapping.RefreshForm(currentFiscalYear, endEligibilityFiscalYear, existingFiipsProjects,
                        existingStructureProjects, certifiedWorkConcepts, precertifiedApprovedWorkConcepts,
                        precertifiedUnapprovedWorkConcepts, quasicertifiedWorkConcepts, eligibleWorkConcepts,
                        selectedWorkConcepts, fiipsWorkConcepts, sender);
                }

                formMapping.TopMost = false;
                formMapping.Show();
            }
            else
            {
                MessageBox.Show("Structure project has no work concepts.");
            }
        }

        internal int DoFindStructuresProject(int structuresProjectIdToFind)
        {
            return mainServ.DoFindStructuresProject(structuresProjectIdToFind, dataGridViewProjectsList);
        }

        internal int DoFindFiipsProject(string fosProjectIdToFind)
        {
            return mainServ.DoFindFiipsProject(fosProjectIdToFind, dataGridViewProjectsList);
        }

        internal int DoFindStructureInProjects(string structureIdToFind)
        {
            return mainServ.DoFindStructureInProjects(structureIdToFind, dataGridViewProjectsList, existingFiipsProjects, existingStructureProjects);
        }

        internal int DoFindStructure(string structureIdToFind)
        {
            int hitCount = 0;

            for (int i = 5; i <= 11; i++)
            {
                DataGridView dgv = GetDataGridView(i);

                hitCount += mainServ.DoFindStructure(structureIdToFind, dgv);
            }

            return hitCount;
        }

        internal void DoMapStructures(bool newProject, object sender)
        {
            selectedStructureIds.Clear();
          
            for (int i = 5; i <= 11; i++)
            {
                DataGridView dgv = GetDataGridView(i);

                if (dgv != null)
                {
                    foreach (DataGridViewRow dgvr in dgv.SelectedRows)
                    {
                        string structureId = dgvr.Cells[0].Value.ToString();
                        selectedStructureIds.Add(structureId);
                    }
                }
            }

            foreach (var structureId in selectedStructureIds)
            {
                formMapping.SearchStructure(structureId);
            }

            ((TabControl)formMain.Controls["tabControlProjectExplorer"]).SelectedIndex = 0;
        }

        internal string FormatConstructionId(string constructionId)
        {
            return mainServ.FormatConstructionId(constructionId);
        }

        internal Structure GetSptStructure(string structureId)
        {
            return dataServ.GetSptStructure(structureId);
        }

        internal bool IsStructureInHsi(string structureId, UserAccount userAccount = null)
        {
            return mainServ.IsStructureInHsi(structureId, userAccount);
        }

        internal async void UpdateBoxDirectory()
        {
            foreach (var p in existingStructureProjects)
            {
                try
                {
                    await fileManager.UpdateBoxCertificationDirectory(p);
                }
                catch (Exception ex) { }
            }

            MessageBox.Show("Completed");
        }

        internal WorkConcept GetEligibleWorkConcept(int workConceptDbId, string structureId)
        {
            WorkConcept wc = null;

            try
            {
                wc = eligibleWorkConcepts
                        .Where(el => el.WorkConceptDbId == workConceptDbId && el.StructureId.Equals(structureId) && el.FromEligibilityList).First();
            }
            catch { }

            return wc;
        }

        internal List<WorkConcept> GetEligibleWorkConceptsForAStructure(string structureId)
        {
            return eligibleWorkConcepts.Where(e => e.StructureId.Equals(structureId)).ToList();
        }

        internal bool DoesStructureHaveAnEligibleWorkConcept(string structureId)
        {
            if (eligibleWorkConcepts.Where(ewc => ewc.StructureId.Equals(structureId)).Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal int InsertProposedWorkConcept(WorkConcept workConcept)
        {
            return dataServ.InsertProposedWorkConcept(workConcept);
        }

        internal WorkConcept GetProposedWorkConcept(int workConceptDbId, string structureId)
        {
            WorkConcept workConcept = null;

            try
            {
                workConcept = eligibleWorkConcepts
                    .Where(w => w.WorkConceptDbId == workConceptDbId && w.StructureId.Equals(structureId) && w.FromProposedList).First();
            }
            catch { }

            return workConcept;
        }

        internal bool IsProposedWorkConceptInAProject(int workConceptDbId, string structureId)
        {
            return mainServ.IsProposedWorkConceptInAProject(workConceptDbId, structureId, existingStructureProjects);
        }

        internal void DeleteProposedWorkConcept(int workConceptDbId, string structureId, int fiscalYear, int rowNumber)
        {
            try
            {
                // Deactivate work concept in dataServ
                dataServ.DeactivateProposedWorkConcept(workConceptDbId, structureId);

                // Remove from projects that are not certified (in-memory)
                foreach (var project in existingStructureProjects.Where(p => p.Status != StructuresProgramType.ProjectStatus.Certified))
                {
                    try
                    {
                        int removeCount = project.WorkConcepts.RemoveAll(w => w.WorkConceptDbId == workConceptDbId && w.StructureId.Equals(structureId)
                                                        && w.FromProposedList);
                    }
                    catch { }
                }

                // Add proposed wc to the correct grid
                DataGridView dgv = null;
                int gridCounter = 5;

                for (int i = workConceptStartFiscalYear + 5; i <= workConceptEndFiscalYear; i++)
                {
                    if (i == fiscalYear)
                    {
                        dgv = GetDataGridView(gridCounter);
                        break;
                    }

                    gridCounter++;
                }

                if (dgv != null)
                {
                    if (rowNumber == -1) // Deleting from the tree
                    {
                        foreach (DataGridViewRow dr in dgv.Rows)
                        {
                            try
                            {
                                string strId = dr.Cells[0].Value.ToString().Trim();
                                int wcDbId = Convert.ToInt32(dr.Cells[4].Value);

                                if (structureId.Equals(strId) && wcDbId == workConceptDbId)
                                {
                                    rowNumber = dr.Index;
                                    break;
                                }
                            }
                            catch { }
                        }
                    }

                    try
                    {
                        dgv.Rows.RemoveAt(rowNumber);
                    }
                    catch { }
                }

                WorkConcept proposedWorkConcept = null;

                try
                {
                    proposedWorkConcept = eligibleWorkConcepts.Where(w => w.WorkConceptDbId == workConceptDbId &&
                                                                        w.StructureId.Equals(structureId) && w.FromProposedList).First();
                    formMapping.EditWorkConcept(proposedWorkConcept, "Delete");
                    eligibleWorkConcepts.Remove(proposedWorkConcept);
                }
                catch { }
            }
            catch { }
        }

        internal List<WorkConcept> GetEligibleWorkConceptsForAStructure(string structureId, string workConceptCode)
        {
            //return eligibleWorkConcepts.Where(e => e.StructureId.Equals(structureId) && e.WorkConceptCode.Equals(workConceptCode)).ToList();
            return eligibleWorkConcepts.Where(e => e.StructureId.Equals(structureId)).ToList();
        }

        internal string[] ParseWorkConceptFullDescription(string workConcept)
        {
            return mainServ.ParseWorkConceptFullDescription(workConcept);
        }

        internal void UpdateEligibleWorkConcepts()
        {
            dataServ.UpdateEligibleWorkConcepts();
        }

        internal int GetCurrentFiscalYear()
        {
            return mainServ.GetCurrentFiscalYear();
        }

        internal bool IsProjectIdInFiips(string projectId)
        {
            return dataServ.IsProjectIdInFiips(projectId);
        }

        private void RenderMap()
        {
            formMapping = new FormMapping(userAccount, currentFiscalYear, endEligibilityFiscalYear, existingFiipsProjects,
                    existingStructureProjects, certifiedWorkConcepts, precertifiedApprovedWorkConcepts,
                    precertifiedUnapprovedWorkConcepts, quasicertifiedWorkConcepts, eligibleWorkConcepts,
                    selectedWorkConcepts, fiipsWorkConcepts, panelMap, dataServ, 
                    primaryWorkConcepts, allWorkConcepts, this);
            formMapping.TopLevel = false;
            formMapping.Dock = DockStyle.Fill;
            panelMap.Controls.Add(formMapping);
            formMapping.Show();
            this.treeViewMap = formMapping.GetTreeViewMap();
        }

        private void RemoveWorkConceptFromEligibleList(WorkConcept workConcept)
        {
            try
            {
                var workConceptToRemove = eligibleWorkConcepts.Single(i => i.WorkConceptDbId == workConcept.WorkConceptDbId);
                eligibleWorkConcepts.Remove(workConceptToRemove);
            }
            catch { }
        }

        private void ColorCodeAllWorkConceptsGridRow(WorkConcept wc)
        {
            var row = dataGridViewAllWorkConcepts
                            .Rows
                            .Cast<DataGridViewRow>()
                            .Where(r => Convert.ToInt32(r.Cells[2].Value) == wc.WorkConceptDbId
                                        && r.Cells[0].Value.ToString() == wc.StructureId
                                        && Convert.ToBoolean(r.Cells[11].Value) == wc.IsQuasicertified)
                            .First();
            row.Cells[4].Value = wc.ProjectDbId;
            row.Cells[5].Value = wc.StructureProjectFiscalYear;
            row.DefaultCellStyle.BackColor = Color.Red;
        }

        private void ColorCodeWorkConceptRow(int workConceptDbId, int projectYear)
        {
            var dgv = GetDataGridView(projectYear);
            var row = dgv
                            .Rows
                            .Cast<DataGridViewRow>()
                            .Where(r => Convert.ToInt32(r.Cells[4].Value) == workConceptDbId)
                            .First();
            dgv.Rows.Remove(row);
            //row.DefaultCellStyle.BackColor = Color.Red; 
        }

        private void RenderAllWorkConceptsGrid(List<WorkConcept> wcs, DataGridView dgv, string color = "")
        {
            mainServ.RenderAllWorkConceptsGrid(wcs, dgv, color);
        }

        private void RenderEligibilityListGrid(List<WorkConcept> wcs, DataGridView dgv)
        {
            foreach (WorkConcept wc in wcs)
            {
                AddWorkConceptToEligibilityGrid(dgv, wc);
                /*
                dgv.Rows.Add();
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;

                if (wc.FromEligibilityList)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = wc.WorkConceptDescription;
                }
                else if (wc.FromProposedList)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = "(PR)" + wc.CertifiedWorkConceptDescription;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Orange;
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[2].Value = wc.PriorityScore;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[4].Value = wc.WorkConceptDbId;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = wc.WorkConceptCode;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[6].Value = wc.ProjectYear;*/
            }
        }

        private void AddWorkConceptToEligibilityGrid(DataGridView dgv, WorkConcept wc)
        {
            mainServ.AddWorkConceptToEligibilityGrid(dgv, wc);
        }

        internal int FilterProjectsListGrid(StructuresProgramType.ProjectStatus status, bool show)
        {
            int counter = 0;

            foreach (DataGridViewRow row in dataGridViewProjectsList.Rows)
            {
                
                string projectStatus = row.Cells["dgvcProjectStatus"].Value.ToString();
                
                if (status == StructuresProgramType.ProjectStatus.QuasiCertified)
                {
                    if (projectStatus.Equals("Transitionally Certified"))
                    {
                        row.Visible = show;
                        counter++;
                    }
                }
                else if (projectStatus.Equals(status.ToString()))
                {
                    row.Visible = show;
                    counter++;
                }
            }

            return counter;
        }

        
        internal void RenderFiipsProjects()
        {
            RenderProjectsListGrid(existingFiipsProjects, dataGridViewProjectsList);
        }

        private void RenderProjectsListGrid(List<Project> projects, DataGridView dgv)
        {
            foreach (Project project in projects)
            {
                dgv.Rows.Add();
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcProjectFiscalYear"].Value = project.FiscalYear;

                if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                {
                    Project correspondingStructuresProject = null;
                    string projectDbId = "";

                    try
                    {
                        correspondingStructuresProject = existingStructureProjects.Where(p => p.FosProjectId.Equals(project.FosProjectId)).First();
                        projectDbId = correspondingStructuresProject.ProjectDbId.ToString();
                    }
                    catch { }

                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcStructuresProjectId"].Value = projectDbId;
                }
                else
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcStructuresProjectId"].Value = project.ProjectDbId.ToString();
                }
                
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcNumberOfStructures"].Value = project.NumberOfStructures;
                string projectStatus = project.Status.ToString();

                if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                {
                    projectStatus = "Transitionally Certified";
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcProjectStatus"].Value = projectStatus;

                if (project.Status != StructuresProgramType.ProjectStatus.Fiips)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcLastWorkflow"].Value = dataServ.GetWorkflowStatus(project);
                }

                if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcMapProject"].Value = String.Format("Map: {0}", project.FosProjectId);
                }
                else
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcMapProject"].Value = String.Format("Map: {0}", project.ProjectDbId);
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcConstructionId"].Value = FormatConstructionId(project.FosProjectId);
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcRegion"].Value = project.Region;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcPrecertificationLiaison"].Value = project.PrecertificationLiaisonUserFullName;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertificationLiaison"].Value = project.CertificationLiaisonUserFullName;

                if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcPse"].Value = project.PseDate.Year != 1 ? project.PseDate.ToString("yyyy-MM-dd") : "";
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcEarlyPse"].Value = project.EpseDate.Year != 1 ? project.EpseDate.ToString("yyyy-MM-dd") : "";
                }
                else
                {
                    Project correspondingFiipsProject = null;
                    string pseDate = "";
                    string epseDate = "";

                    if (!String.IsNullOrEmpty(project.FosProjectId))
                    {
                        try
                        {
                            correspondingFiipsProject = existingFiipsProjects.Where(f => f.FosProjectId.Equals(project.FosProjectId)).First();
                            pseDate = correspondingFiipsProject.PseDate.Year != 1 ? correspondingFiipsProject.PseDate.ToString("yyyy-MM-dd") : "";
                            epseDate = correspondingFiipsProject.EpseDate.Year != 1 ? correspondingFiipsProject.EpseDate.ToString("yyyy-MM-dd") : "";
                        }
                        catch { }

                        if (correspondingFiipsProject == null)
                        {
                            Dw.FiipsProject tempFiipsProject = null;

                            try
                            {
                                tempFiipsProject = warehouseDatabase.GetFiipsProject(project.FosProjectId);
                            }
                            catch { }

                            if (tempFiipsProject != null)
                            {
                                pseDate = tempFiipsProject.PseDate.Year != 1 ? tempFiipsProject.PseDate.ToString("yyyy-MM-dd") : "";
                                epseDate = tempFiipsProject.EarliestPseDate.Year != 1 ? tempFiipsProject.EarliestPseDate.ToString("yyyy-MM-dd") : "";
                            }
                        }
                    }

                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcPse"].Value = pseDate;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcEarlyPse"].Value = epseDate;
                }

                switch (project.Status)
                {
                    case StructuresProgramType.ProjectStatus.Certified:
                        dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.DarkGreen;
                        //dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.ForeColor = Color.White;
                        break;
                    case StructuresProgramType.ProjectStatus.QuasiCertified:
                        dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LawnGreen;
                        //dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.ForeColor = Color.White;
                        break;
                    case StructuresProgramType.ProjectStatus.Precertified:
                        dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Yellow;
                        break;
                    case StructuresProgramType.ProjectStatus.Unapproved:
                        dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Red;
                        dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.ForeColor = Color.White;
                        break;
                    case StructuresProgramType.ProjectStatus.Fiips:
                        dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightBlue;
                        break;
                }
            }
        }

        private void PopulateProposedWorkConceptJustifications()
        {
            List<string> justifications = dataServ.GetProposedWorkConceptJustifications();
            foreach (var j in justifications)
            {
                comboBoxNewWorkConceptReasonCategory.Items.Add(j);
            }
        }

        private void PopulateProposedWorkConcepts()
        {
            foreach (WorkConcept pwc in primaryWorkConcepts)
            {
                string workConceptDisplay = String.Format("({0}) {1}", pwc.WorkConceptCode, pwc.WorkConceptDescription);

                if (!pwc.WorkConceptCode.Equals("00") && !pwc.WorkConceptCode.Equals("01"))
                {
                    comboBoxNewWorkConcept.Items.Add(workConceptDisplay);
                }
            }
        }

        #region Housekeeping
        internal List<UserActivity> GetUserActivities()
        {
            return dataServ.GetUserActivities();
        }

        internal string WriteLoginHistoryReport(string baseDir)
        {
            string outputFilePath = GetRandomExcelFileName(baseDir);
            List<UserActivity> userActivities = dataServ.GetUserActivities();
            excelReporter.WriteLoginHistoryReport(outputFilePath, userActivities);
            return outputFilePath;
        }

        internal DatabaseService GetDatabase()
        {
            return dataServ;
        }

        internal string WriteMaintenanceNeedsReport(DatabaseService database, string baseDir, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false, string wisamsMaintenanceNeedsListExcelFile = "")
        {
            // Get HSIS maintenance needs
            List<Dw.StructureMaintenanceItem> maintenanceItems = warehouseDatabase.GetStructureMaintenanceItems();
            string outputFilePath = GetRandomExcelFileName(baseDir);
            excelReporter.WriteMaintenanceNeedsReport(maintenanceItems, database, outputFilePath, existingFiipsProjects, fiipsWorkConcepts, existingStructureProjects, startFy, endFy, regions, includeState, includeLocal, wisamsMaintenanceNeedsListExcelFile);
            return outputFilePath;
        }

        internal string WriteMonitoringReport(DatabaseService database, string baseDir, int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false)
        {
            string outputFilePath = GetRandomExcelFileName(baseDir);
            excelReporter.WriteMonitoringReport(database, outputFilePath, existingFiipsProjects, fiipsWorkConcepts, existingStructureProjects, startFy, endFy, regions, includeState, includeLocal);
            return outputFilePath;
        }

        internal string WriteStructuresGisReport(string baseDir, List<string> regions)
        {
            string outputFilePath = GetRandomExcelFileName(baseDir);
            List<string> structureIds = new List<string>();
            
            foreach (var region in regions)
            {
                //structureIds.AddRange(dataServ.GetStateStructuresByRegionForGisDataPull(region));
                structureIds.AddRange(dataServ.GetStructuresByRegionForGisDataPull(region));
            }
            
            excelReporter.WriteStructuresGisReport(outputFilePath, structureIds, fiipsWorkConcepts, existingStructureProjects);
            return outputFilePath;
        }

        internal string GetRandomExcelFileName(string baseDir)
        {
            return mainServ.GetRandomExcelFileName(baseDir);
        }

        #endregion Housekeeping

        #region Diagnostics
        internal void UpdateRegionNumber()
        {
            dataServ.UpdateRegionNumber();
        }

        internal void UpdateWorkActionCode()
        {
            foreach (WorkConcept wc in eligibleWorkConcepts)
            {
                dataServ.UpdateWorkActionCode(wc);
            }
        }

        internal void UpdateOldEvs()
        {
            dataServ.UpdateOldEvs();
        }

        internal void UpdateLatLong()
        {
            dataServ.UpdateLatLong();
        }
        #endregion Diagnostics

    }
}
