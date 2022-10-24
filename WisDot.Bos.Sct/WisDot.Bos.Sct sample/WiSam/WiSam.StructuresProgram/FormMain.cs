using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using BOS.Box;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Domain.Services;
//using Wisdot.Bos.Dw;
//using Microsoft.Office.Interop.Excel;

namespace WiSam.StructuresProgram
{
    public partial class FormMain : Form
    {
        private FormMainController formMainController;
        private UserAccount userAccount;
        private List<Project> existingStructureProjects = new List<Project>();
        private List<Project> newStructureProjects = new List<Project>();
        private List<WorkConcept> primaryWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> allWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> eligibleWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> fiipsWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> quasicertifiedWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> precertifiedApprovedWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> precertifiedUnapprovedWorkConcepts = new List<WorkConcept>();
        private List<WorkConcept> certifiedWorkConcepts = new List<WorkConcept>();
        private List<Project> existingFiipsProjects = new List<Project>();
        private List<WorkConcept> selectedWorkConcepts = new List<WorkConcept>();
        private FormLogin formLogin;
        private FormHelp formHelp;
        private int currentFiscalYear;
        private int currentProjectDbId = 0;
        private int currentWorkConceptDbId = 0;
        private static Excel.Application xlsApp;
        private static Excel.Workbooks xlsBooks;
        private string wisamsExecutablePath;
        private string fiipsQueryToolExecutablePath;
        private FileManagerService fileManager;
        private Item boxItem;
        private FormLoggingIn formLoading;
        private bool fiipsProjectsVisible = false;

        public FormMain(UserAccount userAccount, FormLogin formLogin = null, FormHelp formHelp = null)
        {
            InitializeComponent();
            this.userAccount = userAccount;
            this.formLogin = formLogin;
            this.formHelp = formHelp;
            
            formMainController = new FormMainController(this, this.userAccount, dataGridViewP5, dataGridViewP6,
                                                        dataGridViewP7, dataGridViewP8, dataGridViewP9,
                                                        dataGridViewP10, dataGridViewP11,
                                                        dataGridViewNewProject, dataGridViewProjectsList, 
                                                        existingStructureProjects, newStructureProjects, primaryWorkConcepts,
                                                        
                                                       
                                                        comboBoxNewWorkConcept,
                                                        comboBoxNewWorkConceptReasonCategory,
                                                        allWorkConcepts,
                                                        eligibleWorkConcepts,
                                                        dataGridViewAllWorkConcepts,
                                                        fiipsWorkConcepts,
                                                        existingFiipsProjects,
                                                        quasicertifiedWorkConcepts,
                                                        precertifiedApprovedWorkConcepts, 
                                                        precertifiedUnapprovedWorkConcepts,
                                                        certifiedWorkConcepts,
                                                        panelMap, comboBoxWisamsReport, groupBoxWisamsReports
                                                        );
            wisamsExecutablePath = formMainController.GetWisamsExecutablePath();
            fiipsQueryToolExecutablePath = formMainController.GetFiipsQueryToolExecutablePath();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Text = String.Format("{0}Structures Certification Tool (SCT)", !formMainController.GetApplicationMode().Equals("PROD") ? formMainController.GetApplicationMode() + ": " : "");
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
            formMainController.DoFormLoad();
            currentFiscalYear = formMainController.GetCurrentFiscalYear();
            string userText = String.Format("User: {0} {1}", userAccount.FirstName, userAccount.LastName);
            toolStripLabelUser.Text = userText;
            toolStripLabelDate.Text = String.Format("Today's Date: {0}", DateTime.Now.ToString("MM/dd/yyyy"));
            toolStripLabelFiscalYear.Text = String.Format("Current FY: {0}", currentFiscalYear);
            tabControlProjectExplorer.Visible = true;
            tabControlProjectExplorer.Dock = DockStyle.Fill;
            groupBoxP5.Text = String.Format("FY{0}", currentFiscalYear + 5);
            groupBoxP6.Text = String.Format("FY{0}", currentFiscalYear + 6);
            groupBoxP7.Text = String.Format("FY{0}", currentFiscalYear + 7);
            groupBoxP8.Text = String.Format("FY{0}", currentFiscalYear + 8);
            groupBoxP9.Text = String.Format("FY{0}", currentFiscalYear + 9);
            groupBoxP10.Text = String.Format("FY{0}", currentFiscalYear + 10);
            groupBoxP11.Text = String.Format("FY{0}", currentFiscalYear + 11);
            groupBoxP11.Visible = true;
            groupBoxP10.Visible = true;
            //checkBoxShowP10.Text = String.Format("FY{0}", currentFiscalYear + 10);
            //checkBoxShowP11.Text = String.Format("FY{0}", currentFiscalYear + 11);
            //int xLoc = groupBox1.Location.X;
            splitContainerNewProject.SplitterDistance = splitContainerNewProject.SplitterDistance + 250;
            
            //splitContainerNewProject.SplitterDistance = xLoc + groupBoxP5.Width + 10;
            //splitContainerNewProject.SplitterDistance = this.Width - panelNewProject.Width - 20; // - groupBoxP5.Width;
            //splitContainerExistingProjects.SplitterDistance = dataGridViewProjectsList.Location.X + dataGridViewProjectsList.Width + 100;
            
            if (userAccount.IsOmniscient)
            {
                buttonUpdateEligibleWorkConcepts.Visible = true;
            }
            else
            {
                buttonUpdateEligibleWorkConcepts.Visible = false;
            }

            if (userAccount.IsAdministrator || userAccount.IsSuperUser || userAccount.LastName.ToUpper().Equals("BARUT"))
            {
                toolStripButtonWisams.Visible = true;
            }
            else
            {
                toolStripButtonWisams.Visible = false;
            }

            if (userAccount.IsAdministrator || userAccount.LastName.ToUpper().Equals("BARUT"))
            {
                toolStripButtonAdministration.Visible = true;
            }
            else
            {
                toolStripButtonAdministration.Visible = false;
            }

            if (userAccount.IsAdministrator || userAccount.IsSuperUser
                        || userAccount.IsRegionalProgrammer || userAccount.IsRegionalMaintenanceEngineer
                        || userAccount.IsSuperRead)
            {
                //buttonDownloadWisamsReport.Visible = true;
            }

            if (userAccount.IsAdministrator || userAccount.IsSuperUser || userAccount.IsRegionalProgrammer)
            {
                //groupBoxNewWorkConcept.Visible = true;
                buttonAddNewWorkConcept.Visible = true;
                buttonDeleteProposedWorkConcept.Visible = true;
                buttonClearProposedWorkConceptFields.Visible = true;
            }
            else
            {
                //groupBoxNewWorkConcept.Visible = false;
                buttonAddNewWorkConcept.Visible = false;
                buttonDeleteProposedWorkConcept.Visible = false;
                buttonClearProposedWorkConceptFields.Visible = false;
            }

            if (userAccount.Office.Equals("BOS") || userAccount.Office.Equals("DTIM") || userAccount.Equals("DTSD"))
            {
                checkBoxFiipsProject.Enabled = false;
            }
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            string structureId = "";
            int workConceptDbId = 0;
            string workConceptDescription = "";

            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                structureId = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                workConceptDescription = senderGrid.Rows[e.RowIndex].Cells[1].Value.ToString().Trim();

                try
                {
                    workConceptDbId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[4].Value);
                }
                catch { }
            }

            if (e.ColumnIndex == 0 && e.RowIndex >= 0 && !String.IsNullOrEmpty(structureId))
            {
                formMainController.DoFormStructureShow(structureId);
            }
            else if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                if (userAccount.IsAdministrator || userAccount.IsSuperUser || userAccount.IsRegionalProgrammer)
                {
                    if (workConceptDescription.StartsWith("(PR)"))
                    {
                        WorkConcept wc = formMainController.GetProposedWorkConcept(workConceptDbId, structureId);

                        if (wc != null)
                        {
                            buttonAddNewWorkConcept.Enabled = false;
                            buttonDeleteProposedWorkConcept.Enabled = true;
                            PopulateProposeAWorkConcept(wc, e.RowIndex);
                        }
                    }
                }
            }
            else if ((e.ColumnIndex == 8) && e.RowIndex >= 0 && senderGrid.Rows[e.RowIndex].Cells[8].Value != null)
            {
                string projectId = senderGrid.Rows[e.RowIndex].Cells[8].Value.ToString().Trim();
                formMainController.DoFormStructureProjectShow(projectId);
            }
            else if ((e.ColumnIndex == 4) && e.RowIndex >= 0 && senderGrid.Rows[e.RowIndex].Cells[4].Value != null)
            {
                string projectId = senderGrid.Rows[e.RowIndex].Cells[4].Value.ToString().Trim();
                formMainController.DoFormStructureProjectShow(projectId);
            }
        }

        internal void PopulateProposeAWorkConcept(WorkConcept wc, int rowNumber)
        {
            textBoxNewWorkConceptDbId.Text = wc.WorkConceptDbId.ToString();
            textBoxNewWorkConceptStructureId.ReadOnly = true;
            textBoxNewWorkConceptStructureId.Text = wc.StructureId;
            comboBoxNewWorkConcept.Enabled = false;
            comboBoxNewWorkConcept.SelectedItem = String.Format("({0}) {1}", wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription);
            textBoxNewWorkConceptFiscalYear.ReadOnly = true;
            textBoxNewWorkConceptFiscalYear.Text = wc.FiscalYear.ToString();
            comboBoxNewWorkConceptReasonCategory.SelectedItem = wc.ReasonCategory;
            comboBoxNewWorkConceptReasonCategory.Enabled = false;
            textBoxNewWorkConceptReasonExplanation.Text = wc.Notes;
            textBoxNewWorkConceptReasonExplanation.ReadOnly = true;
            textBoxRowNumber.Text = rowNumber.ToString();
        }

        private void buttonDeleteWorkConceptFromProject_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow selectedRow in dataGridViewNewProject.SelectedRows)
            {
                int fiscalYear = Convert.ToInt32(selectedRow.Cells[3].Value);
                int projectYear = Convert.ToInt32(selectedRow.Cells[6].Value);
                int workConceptDbId = Convert.ToInt32(selectedRow.Cells[4].Value);
                var dgv = formMainController.GetDataGridView(projectYear);

                var row = dgv
                            .Rows
                            .Cast<DataGridViewRow>()
                            .Where(r => Convert.ToInt32(r.Cells[4].Value) == workConceptDbId)
                            .First();
                row.DefaultCellStyle.BackColor = Color.Empty;
                dataGridViewNewProject.Rows.RemoveAt(selectedRow.Index);
            }
        }

        private void buttonCreateProject_Click(object sender, EventArgs e)
        {
            // Validate first
            bool validDataEntry = true;
            int projectDbId = 0;
            string fosProjectId = "";
            string projectImprovementConcept = "";
            int projectFiscalYear = 0;
            
            try
            {
                projectFiscalYear = Convert.ToInt32(textBoxNewProjectFiscalYear.Text.Trim());

                if (projectFiscalYear < currentFiscalYear)
                {
                    validDataEntry = false;
                    MessageBox.Show("Enter project fiscal year >= " + currentFiscalYear, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                validDataEntry = false;
                MessageBox.Show("Enter project fiscal year >= " + currentFiscalYear, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (validDataEntry)
            {
                fosProjectId = textBoxNewProjectFosProjectId.Text.Trim();

                if (comboBoxNewProjectImprovementConcept.SelectedItem != null)
                {
                    projectImprovementConcept = comboBoxNewProjectImprovementConcept.SelectedItem.ToString();
                }

                currentProjectDbId++;
                projectDbId = currentProjectDbId;
                projectFiscalYear = Convert.ToInt32(textBoxNewProjectFiscalYear.Text.Trim());
                formMainController.DoProjectCreate(projectDbId, fosProjectId, projectImprovementConcept, projectFiscalYear, "Saved");
                dataGridViewNewProject.Rows.Clear();
                //textBoxNewProjectFosProjectId.Text = "";
                comboBoxNewProjectImprovementConcept.SelectedItem = null;
                textBoxNewProjectFiscalYear.Text = "";
            }
        }

        private void dataGridViewProjectsList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;

            if (rowIndex >= 0 && columnIndex >= 0)
            {
                string columnName = senderGrid.Columns[columnIndex].Name;

                switch (columnName.ToUpper())
                {
                    case "DGVCSTRUCTURESPROJECTID":
                        try
                        {
                            string structuresProjectId = senderGrid.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                            formMainController.DoFormStructureProjectShow(structuresProjectId);
                        }
                        catch { }
                        break;
                    case "DGVCCONSTRUCTIONID":
                        try
                        {
                            string constructionId = senderGrid.Rows[rowIndex].Cells[columnIndex].Value.ToString().Trim().Replace("-", "");

                            if (constructionId.Length > 0)
                            {
                                formMainController.DoFormStructureProjectShow(constructionId);
                            }
                        }
                        catch { }
                        break;
                    case "DGVCMAPPROJECT":
                        try
                        {
                            string cellValue = senderGrid.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                            string id = cellValue.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                            if (id.Length == 8)
                            {
                                formMainController.DoMapProject(id);
                            }
                            else
                            {
                                formMainController.DoMapProject(Convert.ToInt32(id));
                            }
                        }
                        catch { }
                        break;
                }
            }

            /*
            if ((e.ColumnIndex == 1 || e.ColumnIndex == 7 || e.ColumnIndex == 9) && e.RowIndex >= 0)
            {
                try
                {
                    string projectId = senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    formMainController.DoFormStructureProjectShow(projectId.Replace("-", ""));
                }
                catch { }
            }
            else if (e.ColumnIndex == 8) // Map
            {
                string projectId = "";

                if (senderGrid.Rows[e.RowIndex].Cells[1].Value != null)
                {
                    projectId = senderGrid.Rows[e.RowIndex].Cells[1].Value.ToString();
                }
                else if (senderGrid.Rows[e.RowIndex].Cells[9].Value != null)
                {
                    projectId = senderGrid.Rows[e.RowIndex].Cells[9].Value.ToString();
                }

                projectId = projectId.Replace("-", "");

                if (projectId.Length == 8) // Fiips Fos ID
                {
                    formMainController.DoMapProject(projectId);
                }
                else
                {
                    formMainController.DoMapProject(Convert.ToInt32(projectId));
                }
            }*/
        }

        private void buttonNewProject_Click(object sender, EventArgs e)
        {
            dataGridViewNewProject.Rows.Clear();
        }

        private bool ValidateStructureId(string structureId)
        {
            return formMainController.ValidateStructureId(structureId, userAccount);
        }

        private void buttonAddNewWorkConcept_Click(object sender, EventArgs e)
        {
            bool validDataEntry = true;
            string structureId = "";
            string workConceptCode = "";
            string workConceptDescription = "";
            int fiscalYear = 0;
            int projectYear = 0;
            bool validFiscalYear = true;
            structureId = textBoxNewWorkConceptStructureId.Text.Trim().ToUpper();

            if (!ValidateStructureId(structureId))
            {
                return;
            }
           
            if (comboBoxNewWorkConcept.SelectedItem == null)
            {
                MessageBox.Show("Select a proposed work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboBoxNewWorkConceptReasonCategory.SelectedItem == null)
            {
                MessageBox.Show("Select a reason category for the proposed work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxNewWorkConceptReasonExplanation.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter an explanation for the proposed work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                fiscalYear = Convert.ToInt32(textBoxNewWorkConceptFiscalYear.Text.Trim());
                validFiscalYear = formMainController.ValidateFiscalYear(fiscalYear);
            }
            catch
            { }

            if (!validFiscalYear)
            {
                MessageBox.Show(String.Format("Enter a year >= {0} but <= {1}", formMainController.GetWorkConceptStartFiscalYear() + 2,
                                               formMainController.GetWorkConceptEndFiscalYear()), "SCT",
                                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            projectYear = formMainController.GetProjectYearBasedOnFiscalYear(fiscalYear);

            if (validDataEntry)
            {
                if (formMainController.DoesStructureHaveAnEligibleWorkConcept(structureId))
                {
                    MessageBox.Show(String.Format("{0} already has an eligible work concept.", structureId), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    string[] workConcept = formMainController.ParseWorkConceptFullDescription(comboBoxNewWorkConcept.SelectedItem.ToString().Trim());
                    workConceptCode = workConcept[0];
                    workConceptDescription = workConcept[1];
                    string reasonCategory = comboBoxNewWorkConceptReasonCategory.SelectedItem.ToString();
                    string notes = textBoxNewWorkConceptReasonExplanation.Text.Trim();
                    formMainController.DoProposedWorkConceptAdd(0, structureId, workConceptCode, workConceptDescription, fiscalYear, projectYear,
                                                                reasonCategory, notes);
                }
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            formMainController.LogUserActivity(userAccount.UserDbId, "logout");
            formMainController.DoCloseDatabaseConnection("WISAMS");
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (formHelp != null)
            {
                formHelp.Close();
            }

            if (formLogin != null)
            {
                formLogin.Close();
            }
        }

        private void dataGridViewProjectWorkConcepts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex == 0) && e.RowIndex >= 0)
            {
                var senderGrid = (DataGridView)sender;
                string structureId = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                //formMainController.DoFormStructureShow(structureId, workConceptCode, WorkConceptDescription, fiscalYear, projectYear);
            }
        }

        private void toolStripMain_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ToolStripItem tsItem = e.ClickedItem;
            tabControlProjectExplorer.Visible = false;
            tabControlHelp.Visible = false;
            tabControlAdministration.Visible = false;
            ProcessStartInfo psi;

            switch (tsItem.Name.ToLower())
            {
                case "toolstripbuttonprojectexplorer":
                    tabControlProjectExplorer.Visible = true;
                    tabControlProjectExplorer.Dock = DockStyle.Fill;
                    break;
                case "toolstripbuttonhelp":
                    tabControlHelp.Visible = true;
                    tabControlHelp.Dock = DockStyle.Fill;
                    break;
                case "toolstripbuttonadministration":
                    tabControlAdministration.Visible = true;
                    tabControlAdministration.Dock = DockStyle.Fill;
                    break;
                case "toolstripbuttonfiipsquerytool":
                    psi = new ProcessStartInfo();
                    psi.WorkingDirectory = Path.GetDirectoryName(fiipsQueryToolExecutablePath);
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    psi.FileName = Path.GetFileName(fiipsQueryToolExecutablePath);
                    Process.Start(psi);
                    break;
                case "toolstripbuttonwisams":
                    psi = new ProcessStartInfo();
                    psi.WorkingDirectory = Path.GetDirectoryName(wisamsExecutablePath);
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    psi.FileName = Path.GetFileName(wisamsExecutablePath);
                    Process.Start(psi);
                    break;
            }

            foreach (ToolStripItem item in ((ToolStrip)sender).Items)
            {
                if (item == e.ClickedItem && item is ToolStripButton)
                {
                    //((ToolStripButton)item).CheckState = true;
                    //((ToolStripButton)item).CheckState = CheckState.Checked;
                }
                else if (item is ToolStripButton)
                {
                    //((ToolStripButton)item).Checked = false;
                    ((ToolStripButton)item).CheckState = CheckState.Unchecked;
                }
            }
        }

        private void checkBoxShowP10_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            
            if (checkBox.Checked)
            {
                groupBoxP10.Visible = true;
            }
            else
            {
                groupBoxP10.Visible = false;
            }
        }

        private void checkBoxShowP11_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                groupBoxP11.Visible = true;
            }
            else
            {
                groupBoxP11.Visible = false;
            }
        }

        private void textBoxNewProjectFiscalYear_Validating(object sender, CancelEventArgs e)
        {
            int projectFiscalYear = 0;
            bool validDataEntry = true;

            if (textBoxNewProjectFiscalYear.Text.Trim().Length > 0)
            {
                try
                {
                    projectFiscalYear = Convert.ToInt32(textBoxNewProjectFiscalYear.Text.Trim());

                    if (projectFiscalYear < currentFiscalYear)
                    {
                        e.Cancel = true;
                        validDataEntry = false;
                        MessageBox.Show("Enter project fiscal year >= " + currentFiscalYear, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    e.Cancel = true;
                    validDataEntry = false;
                    MessageBox.Show("Enter project fiscal year >= " + currentFiscalYear, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (validDataEntry)
                {
                    buttonAddNewWorkConcept.Enabled = true;
                    textBoxNewWorkConceptFiscalYear.Text = projectFiscalYear.ToString();
                }
            }
            else
            {
                buttonAddNewWorkConcept.Enabled = false;
            }
        }

        private void dgv_DoubleCellClick(object sender, DataGridViewCellEventArgs e)
        {
            /*
            return;

            var senderGrid = (DataGridView)sender;

            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                int workConceptDbId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[4].Value);
                formMainController.DoWorkConceptAdd(workConceptDbId);
            }*/
            // e.ColumnIndex == 1: checks that work concept description cell's clicked
            /*
            if (e.ColumnIndex == 1 && e.RowIndex >= 0)
            {
                var senderGrid = (DataGridView)sender;

                if (senderGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Yellow &&
                    senderGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Red &&
                    senderGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor != Color.Orange)
                {
                    string structureId = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();
                    string workConceptDescription = senderGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim();
                    double priorityScore = Convert.ToDouble(senderGrid.Rows[e.RowIndex].Cells[2].Value);
                    int fiscalYear = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[3].Value);
                    int workConceptDbId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[4].Value);
                    string workConceptCode = senderGrid.Rows[e.RowIndex].Cells[5].Value.ToString().Trim();
                    int projectYear = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[6].Value);
                    senderGrid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
                    formMainController.DoWorkConceptAdd(structureId, workConceptCode, workConceptDescription, fiscalYear, priorityScore, workConceptDbId, projectYear, true);

                    if (textBoxNewProjectFiscalYear.Text.Trim().Length == 0)
                    {
                        textBoxNewProjectFiscalYear.Text = fiscalYear.ToString();
                        textBoxNewWorkConceptFiscalYear.Text = fiscalYear.ToString();
                        buttonAddNewWorkConcept.Enabled = true;
                    }
                }
            }*/
        }

        private void dgv_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            /*
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow dgvr = dgv.CurrentRow;
                int workConceptDbId = Convert.ToInt32(dgvr.Cells[4].Value);
                WorkConcept wc = formMainController.GetEligibleWorkConcept(workConceptDbId);

                if (wc != null)
                {
                    selectedWorkConcepts.Add(wc);
                }
            }
            */
        }

        private void buttonMapSelections_Click(object sender, EventArgs e)
        {
            formMainController.DoMapStructures(false, sender);
        }

        private void buttonClearSelections_Click(object sender, EventArgs e)
        {
            formMainController.DoClearGridSelections();
        }

        private void buttonValidateFiipsProjId_Click(object sender, EventArgs e)
        {
            if (textBoxNewProjectFosProjectId.Text.Trim().Length == 8)
            {
                bool inFiips = formMainController.IsProjectIdInFiips(textBoxNewProjectFosProjectId.Text.Trim());
                MessageBox.Show(String.Format("{0} in Fiips? {1}", textBoxNewProjectFosProjectId.Text.Trim(), inFiips));
            }
        }

        private void buttonMapStructures_Click(object sender, EventArgs e)
        {
            formMainController.DoMapProject(currentProjectDbId, sender);
        }

        private void buttonMapProject_Click(object sender, EventArgs e)
        {
            /*
            if (textBoxCurrentProjectDbId.Text.Length > 0)
            {
                formMainController.DoMapProject(Convert.ToInt32(textBoxCurrentProjectDbId.Text), sender);
            }
            else
            {
                MessageBox.Show("Select a project to map.");
            }*/
        }

        private void tabControlProjectExplorer_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            if ((tabControlProjectExplorer.SelectedTab == tabPageJoeOnly) && !userAccount.IsOmniscient)
            {
                MessageBox.Show("You have insufficient permission.");
                tabControlProjectExplorer.SelectedTab = tabPageProjectProgramming;
            }*/
        }

        #region Diagnostics
        private void button10_Click(object sender, EventArgs e)
        {
            formMainController.UpdateRegionNumber();
        }

        
        #endregion Diagnostics

        private void checkBoxShowWorkConceptsGrid_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            bool show = checkBox.Checked;
            Color color = Color.Empty;

            switch (checkBox.Name.ToLower())
            {
                case "checkboxeligible":
                    color = Color.Empty;
                    break;
                case "checkboxunapproved":
                    color = Color.Red;
                    break;
                case "checkboxprecertified":
                    color = Color.Yellow;
                    break;
                case "checkboxcertified":
                    color = Color.LightGreen;
                    break;
                case "checkboxfiips":
                    color = Color.LightSkyBlue;
                    break;
            }

            foreach (DataGridViewRow row in dataGridViewAllWorkConcepts.Rows)
            {
                if (row.DefaultCellStyle.BackColor == color)
                {
                    row.Visible = show;
                }
            }
        }

        private void buttonMapNewProject_Click(object sender, EventArgs e)
        {
            formMainController.DoMapStructures(true, sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            formMainController.UpdateWorkActionCode();
        }

        private void buttonWorkConceptsMapSelections_Click(object sender, EventArgs e)
        {
            formMainController.DoMapStructures(false, sender);
        }

        private void buttonMapSelectedProjects_Click(object sender, EventArgs e)
        {
            formMainController.DoMapStructures(false, sender);
        }

        internal bool IsFormOpen(string formName)
        {
            return formMainController.IsFormOpen(formName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            formMainController.UpdateLatLong();
        }

        /*
        private void button9_Click(object sender, EventArgs e)
        {

        }*/

        private void buttonRefreshProjectsGrid_Click(object sender, EventArgs e)
        {
            formMainController.RefreshProjectsListGrid();
        }

        private void buttonRefreshWorkConcepts_Click(object sender, EventArgs e)
        {
            formMainController.RenderAllWorkConceptsGrid();
            checkBoxEligible.Checked = true;
            checkBoxUnapproved.Checked = true;
            checkBoxPrecertified.Checked = true;
            checkBoxCertified.Checked = true;
            checkBoxFiips.Checked = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:dotdtsdstructuresprogram@dot.wi.gov");
        }

        private void buttonFindStructureOnMap_Click(object sender, EventArgs e)
        {
            string structureId = textBoxStructureId.Text.Trim().ToUpper();
            string translatedStructureId = Utility.TranslateStructureId(structureId);

            if ((translatedStructureId.Length != 7 && translatedStructureId.Length != 11))
            {
                MessageBox.Show("Structure ID must be 7 or 11 characters long.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int hitCount = formMainController.DoFindStructure(translatedStructureId);
                string message = "";

                if (hitCount == 0)
                {
                    message = String.Format("Found no instance of {0}.", structureId);
                }
                else if (hitCount == 1)
                {
                    message = String.Format("Found an instance of {0} indicated by row selection.", structureId);
                }
                else
                {
                    message = String.Format("Found {0} instances of {1} indicated by row selection.", hitCount, structureId);
                }

                MessageBox.Show(message, "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information); ;
            }
        }

       
        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void buttonFindStructureInProjects_Click(object sender, EventArgs e)
        {
            /*
            string structureId = textBoxProjectsListStructureId.Text.Trim().ToUpper();
            string translatedStructureId = Utility.TranslateStructureId(structureId);

            if (translatedStructureId.Length != 7 && translatedStructureId.Length != 11)
            {
                MessageBox.Show("Invalid Structure Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                int hitCount = formMainController.DoFindStructureInProjects(structureId);
                MessageBox.Show(String.Format("Found {0} instance(s) of {1} indicated by row selection.", hitCount, structureId)); ;
            }*/
        }

        private void buttonFindFiipsProject_Click(object sender, EventArgs e)
        {
            string fosProjectId = textBoxProjectsListFosProjectId.Text.Trim();
            fosProjectId = fosProjectId.Replace("-", "");

            if (fosProjectId.Length != 8)
            {
                MessageBox.Show("Invalid Construction Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                OpenLoading();
                int hitCount = formMainController.DoFindFiipsProject(fosProjectId);
                CloseLoading();
                MessageBox.Show(String.Format("Found {0} instance(s) of {1} indicated by row selection.", hitCount, FormatConstructionId(fosProjectId)));
            }
        }

        internal string FormatConstructionId(string constructionId)
        {
            string formattedId = constructionId;

            try
            {
                formattedId = String.Format("{0}-{1}-{2}", formattedId.Substring(0, 4), formattedId.Substring(4, 2), formattedId.Substring(6, 2));
            }
            catch { }

            return formattedId;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            formMainController.UpdateEligibleWorkConcepts();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string path = @"\\mad00fph\n4public\bos\wisams-reports";
            string excelFile = "";

            if (userAccount.IsSuperUser || userAccount.IsAdministrator || userAccount.IsSuperRead)
            {
                //Process.Start(path);
                excelFile = "all-state-owned.xlsx";
            }
            else
            {
                excelFile = userAccount.Office.Substring(2, 2) + "-state-owned.xlsx";
            }

            try
            {
                string fullPath = Path.Combine(path, excelFile);
                formMainController.OpenExcelFile(fullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Downloading error. Please email dotdtsdstructuresprogram@dot.wi.gov.");
            }
        }

        private void buttonFindProjectId_Click(object sender, EventArgs e)
        {
            string projectId = textBoxSearchProjectId.Text.Trim();
            int projectDbId = 0;

            try
            {
                projectDbId = Convert.ToInt32(projectId);
            }
            catch { }

            if (projectId.Length == 0 || projectDbId == 0)
            {
                MessageBox.Show("Invalid Structures Project Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                OpenLoading();
                int hitCount = formMainController.DoFindStructuresProject(projectDbId);
                CloseLoading();
                MessageBox.Show(String.Format("Found {0} instance(s) of {1} indicated by row selection.", hitCount, projectDbId));
            }
        }

        private void buttonUpdateLatLong_Click(object sender, EventArgs e)
        {
            formMainController.UpdateLatLong();
        }

        private string GetLoginReport()
        {
            string outputFilePath = formMainController.WriteLoginHistoryReport(@"c:\temp");
            return outputFilePath;
        }

        private async void buttonGetLoginHistoryReport_Click(object sender, EventArgs e)
        {
            linkLabelLoginReport.Visible = false;
            textBoxLoginReport.Text = "";
            pictureBoxLoginReport.Visible = true;
            buttonRunLoginReport.Enabled = false;
            string outputFilePath = await Task.Run(() => GetLoginReport());
            textBoxLoginReport.Text = outputFilePath;
            
            if (System.IO.File.Exists(outputFilePath))
            {
                linkLabelLoginReport.Visible = true;
            }

            pictureBoxLoginReport.Visible = false;
            buttonRunLoginReport.Enabled = true;
        }

        private void linkLabelExcelOutputFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenExcelFile(textBoxLoginReport.Text);
        }

        private string GetGisReport()
        {
            List<string> regions = new List<string>() { "1", "2", "3", "4", "5" };
            //List<string> regions = new List<string>() { "3" };
            string outputFilePath = formMainController.WriteStructuresGisReport(@"c:\temp", regions);
            return outputFilePath;
        }

        private async void buttonPullStructuresDataForGis_Click(object sender, EventArgs e)
        {
            linkLabelAgoReport.Visible = false;
            textBoxAgoReport.Text = "";
            pictureBoxAgoReport.Visible = true;
            buttonRunAgoReport.Enabled = false;
            string outputFilePath = await Task.Run(() => GetGisReport());
            textBoxAgoReport.Text = outputFilePath;

            if (System.IO.File.Exists(outputFilePath))
            {
                linkLabelAgoReport.Visible = true;
            }

            pictureBoxAgoReport.Visible = false;
            buttonRunAgoReport.Enabled = true;
        }

        private void buttonUpdateRegionNumbers_Click(object sender, EventArgs e)
        {
            formMainController.UpdateRegionNumber();
        }

        private void buttonUpdateEligibleWorkConcepts_Click(object sender, EventArgs e)
        {
            formMainController.UpdateEligibleWorkConcepts();
        }

        private void textBoxNewWorkConceptFiscalYear_Validating(object sender, CancelEventArgs e)
        {
            
        }

        private void buttonClearProposedWorkConceptFields_Click(object sender, EventArgs e)
        {
            ClearProposedWorkConceptFields();
        }

        private void ClearProposedWorkConceptFields()
        {
            textBoxNewWorkConceptDbId.Text = "0";
            textBoxNewWorkConceptStructureId.Clear();
            textBoxNewWorkConceptStructureId.ReadOnly = false;
            comboBoxNewWorkConcept.SelectedItem = null;
            comboBoxNewWorkConcept.Enabled = true;
            textBoxNewWorkConceptFiscalYear.Clear();
            textBoxNewWorkConceptFiscalYear.ReadOnly = false;
            comboBoxNewWorkConceptReasonCategory.SelectedItem = null;
            comboBoxNewWorkConceptReasonCategory.Enabled = true;
            textBoxNewWorkConceptReasonExplanation.Clear();
            textBoxNewWorkConceptReasonExplanation.ReadOnly = false;
            buttonAddNewWorkConcept.Enabled = true;
            buttonDeleteProposedWorkConcept.Enabled = false;
        }

        private void buttonDeleteProposedWorkConcept_Click(object sender, EventArgs e)
        {
            int workConceptDbId = Convert.ToInt32(textBoxNewWorkConceptDbId.Text);
            string structureId = textBoxNewWorkConceptStructureId.Text.Trim();
            int fiscalYear = Convert.ToInt32(textBoxNewWorkConceptFiscalYear.Text);
            int rowNumber = Convert.ToInt32(textBoxRowNumber.Text);

            if (formMainController.IsProposedWorkConceptInAProject(workConceptDbId, structureId))
            {
                DialogResult dr = MessageBox.Show("You're about to delete a proposed work concept that's already in a project. Continue with the deletion?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.Yes)
                {
                    formMainController.DeleteProposedWorkConcept(workConceptDbId, structureId, fiscalYear, rowNumber);
                    ClearProposedWorkConceptFields();
                }
            }
            else
            {
                formMainController.DeleteProposedWorkConcept(workConceptDbId, structureId, fiscalYear, rowNumber);
                ClearProposedWorkConceptFields();
            }
        }

        private void OpenExcelFile(string filePath)
        {
            formMainController.OpenExcelFile(filePath);
        }

        private string GetMaintenanceNeedsReport(int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false, string wisamsMaintenanceNeedsListExcelFile = "")
        {
            string outputFilePath = formMainController.WriteMaintenanceNeedsReport(formMainController.GetDatabase(), @"c:\temp", startFy, endFy, regions, includeState, includeLocal, wisamsMaintenanceNeedsListExcelFile);
            return outputFilePath;
        }

        private string GetMonitoringReport(int startFy, int endFy, List<string> regions, bool includeState = true, bool includeLocal = false)
        {
            string outputFilePath = formMainController.WriteMonitoringReport(formMainController.GetDatabase(), @"c:\temp", startFy, endFy, regions, includeState, includeLocal);
            return outputFilePath;
        }

        private async void buttonRunMonitoringReport_Click(object sender, EventArgs e)
        {
            int startFy = 0;

            try
            {
                startFy = Convert.ToInt32(textBoxStartFy.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Enter a valid start year.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int endFy = 0;

            try
            {
                endFy = Convert.ToInt32(textBoxEndFy.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Enter a valid end year.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> regions = new List<string>();
            
            if (chkSouthwest.Checked)
            {
                regions.Add("1-SW");
            }

            if (chkSoutheast.Checked)
            {
                regions.Add("2-SE");
            }

            if (chkNortheast.Checked)
            {
                regions.Add("3-NE");
            }

            if (chkNorthcentral.Checked)
            {
                regions.Add("4-NC");
            }

            if (chkNorthwest.Checked)
            {
                regions.Add("5-NW");
            }

            if (regions.Count == 0)
            {
                MessageBox.Show("Select at least one region.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool includeState = chkStateBridges.Checked ? true : false;
            bool includeLocal = chkLocalBridges.Checked ? true : false;

            if (!includeState && !includeLocal)
            {
                MessageBox.Show("Select State and/or Local.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            linkLabelMonitoringReport.Visible = false;
            textBoxMonitoringReport.Text = "";
            pictureBoxMonitoringReport.Visible = true;
            buttonRunMonitoringReport.Enabled = false;
            string outputFilePath = await Task.Run(() => GetMonitoringReport(startFy, endFy, regions, includeState, includeLocal));
            //string outputFilePath = await Task.Run(() => GetMaintenanceNeedsReport(startFy, endFy, regions, includeState, includeLocal));
            textBoxMonitoringReport.Text = outputFilePath;

            if (System.IO.File.Exists(outputFilePath))
            {
                linkLabelMonitoringReport.Visible = true;
            }

            pictureBoxMonitoringReport.Visible = false;
            buttonRunMonitoringReport.Enabled = true;
        }

        private void textBoxNewWorkConceptStructureId_Validating(object sender, CancelEventArgs e)
        {
            string structureId = textBoxNewWorkConceptStructureId.Text.Trim().ToUpper();

            if (!String.IsNullOrEmpty(structureId))
            {
                if (!ValidateStructureId(structureId))
                {
                    e.Cancel = true;
                }
            }
        }

        private void buttonUpdateBoxDirectory_Click(object sender, EventArgs e)
        {
            formMainController.UpdateBoxDirectory();
        }

        private void linkLabelMonitoringReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenExcelFile(textBoxMonitoringReport.Text);
        }

        private void linkLabelAgoReport_Click(object sender, EventArgs e)
        {
            OpenExcelFile(textBoxAgoReport.Text);
        }

        private async void buttonDownloadWisamsReportInBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxWisamsReport.SelectedIndex >= 0)
                {
                    buttonDownloadWisamsReportInBox.Enabled = false;
                    string[] sep = new string[] { ":" };
                    string boxId = comboBoxWisamsReport.SelectedItem.ToString().Split(sep, StringSplitOptions.RemoveEmptyEntries)[1];
                    boxItem = await BOS.Box.File.GetFileAsync(boxId);
                    saveFileDialog1.DefaultExt = System.IO.Path.GetExtension(boxItem.Name);
                    saveFileDialog1.FileName = boxItem.Name;
                    saveFileDialog1.ShowDialog();
                    buttonDownloadWisamsReportInBox.Enabled = true;
                }
            }
            catch
            {}
        }

        private async void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            using (var fileStream = System.IO.File.Create(saveFileDialog1.FileName))
            {
                //Item file = tvFiles.SelectedNode.Tag as Item;
                var dl = await BOS.Box.File.DownloadAsync(boxItem.Id);
                dl.CopyTo(fileStream);
            }
        }

        private void comboBoxWisamsReport_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
        }

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("561115884472", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("561118178733", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxStructureId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonFindStructureOnMap_Click(sender, e);
            }
        }

        private void buttonMigrateExcelProjects_Click(object sender, EventArgs e)
        {
            formMainController.MigrateExcelProjects();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            formMainController.UpdateTimeWindows();
        }

        private void textBoxProjectsListStructureId_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            dataGridViewProjectsList.ClearSelection();
        }

        private void textBoxProjectsListFosProjectId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonFindFiipsProject_Click(sender, e);
            }
        }

        private void textBoxSearchProjectId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonFindProjectId_Click(sender, e);
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            List<string> projectIds = new List<string>();

            foreach (DataGridViewRow row in dataGridViewProjectsList.Rows)
            {
                if (row.Selected)
                {
                    if (row.Cells["dgvcStructuresProjectId"].Value != null && !String.IsNullOrEmpty(row.Cells["dgvcStructuresProjectId"].Value.ToString()))
                    {
                        projectIds.Add(row.Cells["dgvcStructuresProjectId"].Value.ToString());
                    }

                    if (row.Cells["dgvcConstructionId"].Value != null && !String.IsNullOrEmpty(row.Cells["dgvcConstructionId"].Value.ToString()))
                    {
                        projectIds.Add(row.Cells["dgvcConstructionId"].Value.ToString());
                    }
                }
            }

            if (projectIds.Count > 0)
            {
                formMainController.DoMapProjects(projectIds, false);
            }
            else
            {
                MessageBox.Show("No selections to map", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OpenLoading()
        {
            formLoading = new FormLoggingIn();
            formLoading.StartPosition = FormStartPosition.Manual;
            formLoading.Location = new Point(this.Width / 2, this.Height / 2);
            formLoading.Show();
        }

        private void CloseLoading()
        {
            if (formLoading != null)
            {
                formLoading.Close();
            }
        }

        private void checkBoxUnapprovedProject_CheckedChanged(object sender, EventArgs e)
        {
            //dataGridViewProjectsList.Rows.Clear();
            //formMainController.RenderFiipsProjects();
            /*
            OpenLoading();
            List<int> affectedRows = await Task.Run(() => FilterProjectsListGrid(StructuresProgramType.ProjectStatus.Unapproved, checkBoxUnapprovedProject.Checked));

            foreach (var affectedRow in affectedRows)
            {
                dataGridViewProjectsList.Rows[affectedRow].Visible = checkBoxUnapprovedProject.Checked;
            }

            CloseLoading();*/
            FilterProjects(Color.Red, StructuresProgramType.ProjectStatus.Unapproved, checkBoxUnapprovedProject.Checked);
        }

        private void FilterProjects(Color color, StructuresProgramType.ProjectStatus status, bool show)
        {
            OpenLoading();

            /*
            List<int> affectedRows = await Task.Run(() => FilterProjectsListGrid(status, show));

            foreach (var affectedRow in affectedRows)
            {
                dataGridViewProjectsList.Rows[affectedRow].Visible = show;
            }*/

            List<int> affectedRows = new List<int>();
            //dataGridViewProjectsList.Rows.

            foreach (DataGridViewRow row in dataGridViewProjectsList.Rows)
            {
                //string projectStatus = row.Cells["dgvcProjectStatus"].Value.ToString();

                if (row.DefaultCellStyle.BackColor == color)
                {
                    affectedRows.Add(row.Index);
                    row.Visible = show;
                }
            }

            CloseLoading();

            //if (show)
            {
                MessageBox.Show(String.Format("Number of affected rows: {0}", affectedRows.Count), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private List<int> FilterProjectsListGrid(StructuresProgramType.ProjectStatus status, bool show)
        {
            return formMainController.FilterProjectsListGrid(status, show, dataGridViewProjectsList);
        }

        private void checkBoxPrecertifiedProject_CheckedChanged(object sender, EventArgs e)
        {
            FilterProjects(Color.Yellow, StructuresProgramType.ProjectStatus.Precertified, checkBoxPrecertifiedProject.Checked);
        }

        private void checkBoxFiipsProject_CheckedChanged(object sender, EventArgs e)
        {
            FilterProjects(Color.LightBlue, StructuresProgramType.ProjectStatus.Fiips, checkBoxFiipsProject.Checked);
            /*
            if (!fiipsProjectsVisible)
            {
                fiipsProjectsVisible = true;
                OpenLoading();
                formMainController.RenderFiipsProjects();
                CloseLoading();
            }
            else
            {
                FilterProjects(Color.LightBlue, StructuresProgramType.ProjectStatus.Fiips, checkBoxFiipsProject.Checked);
            }*/
        }

        private void checkBoxCertifiedProject_CheckedChanged(object sender, EventArgs e)
        {
            FilterProjects(Color.DarkGreen, StructuresProgramType.ProjectStatus.Certified, checkBoxCertifiedProject.Checked);
        }

        private void checkBoxTransitionallyCertifiedProject_CheckedChanged(object sender, EventArgs e)
        {
            FilterProjects(Color.LawnGreen, StructuresProgramType.ProjectStatus.QuasiCertified, checkBoxTransitionallyCertifiedProject.Checked);
        }

        private async void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("561143875772", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("561116302882", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("561113383875", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("542610178937", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("542592113652", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                string url = await BOS.Box.File.PreviewAsync("522495541843", true);
                Process.Start(url);
            }
            catch
            {
                MessageBox.Show("Error viewing file", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUpdateOldEvs_Click(object sender, EventArgs e)
        {
            formMainController.UpdateOldEvs();
        }

        private async void buttonRunMaintenanceNeedsReport_Click(object sender, EventArgs e)
        {
            int startFy = 0;

            try
            {
                startFy = Convert.ToInt32(textBoxStartFy.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Enter a valid start year.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int endFy = 0;

            try
            {
                endFy = Convert.ToInt32(textBoxEndFy.Text.Trim());
            }
            catch
            {
                MessageBox.Show("Enter a valid end year.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> regions = new List<string>();

            if (chkSouthwest.Checked)
            {
                regions.Add("SW");
            }

            if (chkSoutheast.Checked)
            {
                regions.Add("SE");
            }

            if (chkNortheast.Checked)
            {
                regions.Add("NE");
            }

            if (chkNorthcentral.Checked)
            {
                regions.Add("NC");
            }

            if (chkNorthwest.Checked)
            {
                regions.Add("NW");
            }

            if (regions.Count == 0)
            {
                MessageBox.Show("Select at least one region.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            linkLabelMonitoringReport.Visible = false;
            textBoxMonitoringReport.Text = "";
            pictureBoxMonitoringReport.Visible = true;
            buttonRunMonitoringReport.Enabled = false;
            string outputFilePath = await Task.Run(() => GetMaintenanceNeedsReport(startFy, endFy, regions, true, false, textBoxWisamsNeedsList.Text.Trim()));
            textBoxMonitoringReport.Text = outputFilePath;

            if (System.IO.File.Exists(outputFilePath))
            {
                linkLabelMonitoringReport.Visible = true;
            }

            pictureBoxMonitoringReport.Visible = false;
            buttonRunMonitoringReport.Enabled = true;
        }
    }
}
