using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using System.IO;
using System.Diagnostics;
using BOS.Box;
using System.Threading;
using Dw = Wisdot.Bos.Dw;
using System.Globalization;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Infrastructure;

namespace WiSam.StructuresProgram
{
    public partial class FormStructureProject : Form
    {
        private Dw.Database warehouseDatabase = new Dw.Database();
        private FileManagerService fileManager;
        private List<WorkConcept> primaryWorkConcepts;
        private List<WorkConcept> secondaryWorkConcepts;
        private List<WorkConcept> workConceptsAdded = new List<WorkConcept>();
        private List<WorkConcept> originalWorkConcepts = new List<WorkConcept>();
        private Project project;
        //private List<WorkConcept> eligibleWorkConcepts;
        //private List<WorkConcept> quasicertifiedWorkConcepts;
        //private List<WorkConcept> precertifiedApprovedWorkConcepts;
        //private List<WorkConcept> precertifiedUnapprovedWorkConcepts;
        //private List<WorkConcept> certifiedWorkConcepts;
        private List<WorkConcept> fiipsWorkConcepts;
        private List<Project> structureProjects;
        private List<Project> fiipsProjects;
        private List<WorkConcept> allWorkConcepts;
        private FormMapping formMapping;
        //private TreeView treeViewMap;
        //private DataGridView dataGridViewProjectsList;
        private int currentFiscalYear = 0;
        private UserAccount userAccount;
        private int currentProjectYear = 1;
        private Project associatedProject = null;
        private string certificationDirectory = null;
        private string bosCdTemplate = null;
        private string tempDirectory = null;
        private WorkConcept currentWorkConcept = null;
        private List<Dw.Structure> structures = new List<Dw.Structure>();
        private FormRequestRecertification formRequestRecertification;
        internal bool savedTransaction { get; set;  }
        private StructuresProgramType.ProjectStatus originalProjectStatus = StructuresProgramType.ProjectStatus.Precertified;
        private bool changesMade = false;
        int previousFiscalYear = 0;
        int previousAdvanceableYear = 0;
        internal bool isDirty { get; set; }
        private Dw.Inspection inspection;

        private static IDatabaseService dataServ;

        FormStructureProjectController controller = new FormStructureProjectController();

        /* NOT USED
        public FormStructureProject(UserAccount userAccount, Database database, WorkConcept wc,
                                    List<WorkConcept> primaryWorkConcepts,
                                    List<WorkConcept> eligibleWorkConcepts,
                                    List<WorkConcept> quasicertifiedWorkConcepts,
                                    List<WorkConcept> precertifiedUnapprovedWorkConcepts,
                                    List<WorkConcept> precertifiedApprovedWorkConcepts,
                                    List<WorkConcept> certifiedWorkConcepts,
                                    List<WorkConcept> fiipsWorkConcepts,
                                    List<Project> structureProjects,
                                    List<Project> fiipsProjects,
                                    List<WorkConcept> allWorkConcepts, 
                                    FormMapping formMapping,
                                    TreeView treeViewMap = null,
                                    DataGridView dataGridViewProjectsList = null)
        {
            InitializeComponent();
            project = new Project();
            project.UserAction = StructuresProgramType.ProjectUserAction.CreateProject;
            project.Status = StructuresProgramType.ProjectStatus.New;
            this.userAccount = userAccount;
            this.database = database;
            this.primaryWorkConcepts = primaryWorkConcepts;
            this.fiipsWorkConcepts = fiipsWorkConcepts;
            this.structureProjects = structureProjects;
            this.fiipsProjects = fiipsProjects;
            this.allWorkConcepts = database.GetAllWorkConcepts();
            this.formMapping = formMapping;
            
            if (fileManager == null)
            {
                fileManager = new FileManager(database);
            }

            currentFiscalYear = GetFiscalYear();
            comboBoxStructureProjectImprovementConcept.SelectedIndex = 0;
            PopulateWorkConcepts();
            PopulateChangeJustifications();
            PopulatePrecertificationLiaisons();
            PopulateCertificationLiaisons();
            
            if (project.Status == StructuresProgramType.ProjectStatus.New) // New Structure Project
            {
                UpdateProjectInfo(project);
                WorkConcept workConcept = new WorkConcept(wc);
                RefreshForm(workConcept);
            }
        }*/

        public FormStructureProject(UserAccount userAccount, DatabaseService database, WorkConcept wc,
                                    FormMapping formMapping)
        {
            InitializeComponent();
            this.userAccount = userAccount;
            this.primaryWorkConcepts = database.GetPrimaryWorkConcepts();
            this.fiipsWorkConcepts = database.GetFiipsWorkConcepts();
            this.structureProjects = database.GetProjectsInSct();
            this.fiipsProjects = database.GetProjectsInFiips();
            this.allWorkConcepts = database.GetAllWorkConcepts();
            this.formMapping = formMapping;
            dataServ = database;
            project = new Project();
            project.UserAction = StructuresProgramType.ProjectUserAction.CreateProject;
            project.Status = StructuresProgramType.ProjectStatus.New;
            //changesMade = true;

            if (fileManager == null)
            {
                fileManager = new FileManagerService(database);
            }

            currentFiscalYear = GetFiscalYear();
            comboBoxStructureProjectImprovementConcept.SelectedIndex = 0;
            PopulateWorkConcepts();
            PopulateChangeJustifications();
            PopulatePrecertificationLiaisons();
            PopulateCertificationLiaisons();
            certificationDirectory = database.GetCertificationDirectory();
            bosCdTemplate = database.GetBosCdTemplate();
            tempDirectory = database.GetTempDirectory();

            if (project.Status == StructuresProgramType.ProjectStatus.New) // New Structure Project
            {
                project.FiscalYear = wc.FiscalYear;
                UpdateProjectInfo(project);
                WorkConcept workConcept = new WorkConcept(wc);
                RefreshForm(workConcept);
                savedTransaction = false;
                isDirty = true;
            }

            AddOnChangeHandlerToInputControls(this);
        }

        /* NOT USED
        public FormStructureProject(UserAccount userAccount, Database database, Project project,
                                    List<WorkConcept> primaryWorkConcepts,
                                    List<WorkConcept> eligibleWorkConcepts,
                                    List<WorkConcept> quasicertifiedWorkConcepts,
                                    List<WorkConcept> precertifiedUnapprovedWorkConcepts,
                                    List<WorkConcept> precertifiedApprovedWorkConcepts,
                                    List<WorkConcept> certifiedWorkConcepts,
                                    List<WorkConcept> fiipsWorkConcepts,
                                    List<Project> structureProjects,
                                    List<Project> fiipsProjects,
                                    List<WorkConcept> allWorkConcepts,
                                    FormMapping formMapping)
        {
            InitializeComponent();
            this.currentFiscalYear = GetFiscalYear();
            this.userAccount = userAccount;
            this.project = project;
            this.database = database;
            this.primaryWorkConcepts = primaryWorkConcepts;
            this.fiipsWorkConcepts = fiipsWorkConcepts;
            this.structureProjects = structureProjects;
            this.fiipsProjects = fiipsProjects;
            this.allWorkConcepts = database.GetAllWorkConcepts();
            this.formMapping = formMapping;
            originalProjectStatus = project.Status;
            previousAdvanceableYear = project.AdvanceableFiscalYear;
            previousFiscalYear = project.FiscalYear;

            if (fileManager == null)
            {
                fileManager = new FileManager(database);
            }

            PopulateWorkConcepts();
            PopulateChangeJustifications();
            PopulatePrecertificationLiaisons();
            PopulateCertificationLiaisons();
            RefreshForm(project);
        }*/

        public FormStructureProject(UserAccount userAccount, DatabaseService database, Project project,
                                    FormMapping formMapping)
        {
            InitializeComponent();
            this.currentFiscalYear = GetFiscalYear();
            this.userAccount = userAccount;
            this.project = project;
            this.primaryWorkConcepts = database.GetPrimaryWorkConcepts();
            this.fiipsWorkConcepts = database.GetFiipsWorkConcepts();
            this.structureProjects = database.GetProjectsInSct();
            this.fiipsProjects = database.GetProjectsInFiips();
            this.allWorkConcepts = database.GetAllWorkConcepts();
            this.formMapping = formMapping;
            originalProjectStatus = project.Status;
            dataServ = database;

            if (fileManager == null)
            {
                fileManager = new FileManagerService(database);
            }

            PopulateWorkConcepts();
            PopulateChangeJustifications();
            PopulatePrecertificationLiaisons();
            PopulateCertificationLiaisons();
            certificationDirectory = database.GetCertificationDirectory();
            bosCdTemplate = database.GetBosCdTemplate();
            tempDirectory = database.GetTempDirectory();
            RefreshForm(project);
            AddOnChangeHandlerToInputControls(this);
        }

        private void AddOnChangeHandlerToInputControls(Control ctrl)
        {
            foreach (Control subctrl in ctrl.Controls)
            {
                if (subctrl is TextBox)
                {
                    if (!((TextBox)subctrl).Name.ToLower().Equals("textboxstructureprojectfosprojectid"))
                    {
                        ((TextBox)subctrl).KeyDown += new KeyEventHandler(InputControls_OnChange);
                    }
                }
                else
                {
                    if (subctrl.Controls.Count > 0)
                        this.AddOnChangeHandlerToInputControls(subctrl);
                }


                /*
                if (subctrl is TextBox)
                    ((TextBox)subctrl).TextChanged +=
                        new EventHandler(InputControls_OnChange);
                else if (subctrl is CheckBox)
                    ((CheckBox)subctrl).CheckedChanged +=
                        new EventHandler(InputControls_OnChange);
                else if (subctrl is RadioButton)
                    ((RadioButton)subctrl).CheckedChanged +=
                        new EventHandler(InputControls_OnChange);
                else if (subctrl is ListBox)
                    ((ListBox)subctrl).SelectedIndexChanged +=
                        new EventHandler(InputControls_OnChange);
                else if (subctrl is ComboBox)
                    ((ComboBox)subctrl).SelectedIndexChanged +=
                        new EventHandler(InputControls_OnChange);
                else
                {
                    if (subctrl.Controls.Count > 0)
                        this.AddOnChangeHandlerToInputControls(subctrl);
                }*/
            }
        }

        internal int GetProjectDbId()
        {
            return project.ProjectDbId;
        }

        private void InputControls_OnChange(object sender, EventArgs e)
        {
            // Do something to indicate the form is dirty like:
            // this.formIsDirty = true;
            this.isDirty = true;
            savedTransaction = false;
        }

        private void PopulateWorkConcepts()
        {
            if (primaryWorkConcepts == null)
            {
                primaryWorkConcepts = dataServ.GetPrimaryWorkConcepts();
            }

            if (secondaryWorkConcepts == null || secondaryWorkConcepts.Count() == 0)
            {
                secondaryWorkConcepts = dataServ.GetSecondaryWorkConcepts();
            }

            foreach (WorkConcept pwc in primaryWorkConcepts)
            {
                if (!pwc.WorkConceptCode.Equals("00") && !pwc.WorkConceptCode.Equals("01"))
                {
                    string workConceptDisplay = String.Format("({0}) {1}", pwc.WorkConceptCode, pwc.WorkConceptDescription);
                    dgvcCertifiedWorkConcept.Items.Add(workConceptDisplay);
                }
            }

            //dgvcCertifiedWorkConcept.Items.Add("----------");
            dgvcCertifiedWorkConcept.Items.Add(String.Format("({0}) {1}", "EV", "EVALUATE FOR SECONDARY WORK CONCEPTS"));
            //dgvcCertifiedWorkConcept.Items.Add("----------");

            foreach (WorkConcept wc in secondaryWorkConcepts)
            {
                //if (!pwc.WorkConceptCode.Equals("00") && !pwc.WorkConceptCode.Equals("01"))
                {
                    string workConceptDisplay = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                    dgvcCertifiedWorkConcept.Items.Add(workConceptDisplay);
                }
            }
        }

        private void PopulateSecondaryWorkConcepts()
        {
            var secondaries = dataServ.GetSecondaryWorkConcepts();
            dgvcSecondaryWorkConcept.Items.Add("");

            foreach (WorkConcept wc in secondaries)
            {
                if (!wc.WorkConceptCode.Equals("90"))
                {
                    string workConceptDisplay = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                    dgvcSecondaryWorkConcept.Items.Add(workConceptDisplay);
                }
            }
        }

        private bool IsWorkConceptListed(string workConceptCode)
        {
            bool found = false;

            foreach (var item in dgvcCertifiedWorkConcept.Items)
            {
                //if (item)
            }
            return found;
        }

        private void PopulatePrecertificationLiaisons()
        {
            comboBoxPrecertifier.Items.Add("");

            foreach (var l in dataServ.GetPrecertificationLiaisons())
            {
                comboBoxPrecertifier.Items.Add(String.Format("{0} {1} ({2})", l.FirstName, l.LastName, l.UserDbId));
            }
        }

        private void PopulateCertificationLiaisons()
        {
            comboBoxCertifier.Items.Add("");

            foreach (var l in dataServ.GetCertificationLiaisons())
            {
                comboBoxCertifier.Items.Add(String.Format("{0} {1} ({2})", l.FirstName, l.LastName, l.UserDbId));
            }
        }

        private void PopulateChangeJustifications()
        {
            dgvcChangeReason.Items.Add("");
            dgvcChangeReason.Items.Add("(00) Structural");
            dgvcChangeReason.Items.Add("(01) ProximityToOtherWork");
            dgvcChangeReason.Items.Add("(02) LaneClosureRestriction");
            dgvcChangeReason.Items.Add("(03) ExpansionDevelopment");
            dgvcChangeReason.Items.Add("(04) LccaSecondaryWorkConcepts");
            dgvcChangeReason.Items.Add("(05) LccaSharedTrafficControlCosts");
            dgvcChangeReason.Items.Add("(06) LccaSharedMobilizationCosts");
            dgvcChangeReason.Items.Add("(07) LccaOther");
            dgvcChangeReason.Items.Add("(50) Other");
            comboBoxPrecertificationReasonCategory.Items.Add("");
            comboBoxPrecertificationReasonCategory.Items.Add("(00) Structural");
            comboBoxPrecertificationReasonCategory.Items.Add("(01) ProximityToOtherWork");
            comboBoxPrecertificationReasonCategory.Items.Add("(02) LaneClosureRestriction");
            comboBoxPrecertificationReasonCategory.Items.Add("(03) ExpansionDevelopment");
            comboBoxPrecertificationReasonCategory.Items.Add("(04) LccaSecondaryWorkConcepts");
            comboBoxPrecertificationReasonCategory.Items.Add("(05) LccaSharedTrafficControlCosts");
            comboBoxPrecertificationReasonCategory.Items.Add("(06) LccaSharedMobilizationCosts");
            comboBoxPrecertificationReasonCategory.Items.Add("(07) LccaOther");
            comboBoxPrecertificationReasonCategory.Items.Add("(10) Secondary Work Concept");
            comboBoxPrecertificationReasonCategory.Items.Add("(15) Evaluate for Secondary");
            comboBoxPrecertificationReasonCategory.Items.Add("(20) Ancillary Structure");
            comboBoxPrecertificationReasonCategory.Items.Add("(50) Other");
        }

        public void RefreshForm(WorkConcept wc, bool updateCost = true, bool addWorkConcept = false)
        {
            if ((project.Status == StructuresProgramType.ProjectStatus.QuasiCertified || project.Status == StructuresProgramType.ProjectStatus.Certified) && 
                (wc.Status == StructuresProgramType.WorkConceptStatus.Eligible || wc.Status == StructuresProgramType.WorkConceptStatus.Proposed
                || wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate)
                && (project.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification
                || project.UserAction == StructuresProgramType.ProjectUserAction.BosCertified
                || project.UserAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified))
            {
                MessageBox.Show(String.Format("Project has been certified."), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (project.InPrecertification && addWorkConcept &&
                (wc.Status == StructuresProgramType.WorkConceptStatus.Eligible || wc.Status == StructuresProgramType.WorkConceptStatus.Proposed
                || wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate))
            {
                MessageBox.Show(String.Format("Can't add work concept to project because project's in precertification."), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (project.InCertification && addWorkConcept && 
                (wc.Status == StructuresProgramType.WorkConceptStatus.Eligible || wc.Status == StructuresProgramType.WorkConceptStatus.Proposed
                || wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate))
            {
                MessageBox.Show(String.Format("Can't add work concept to project because project's in certification."), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Work Concept's in the project already
            if (workConceptsAdded.Any(c => c.WorkConceptDbId == wc.WorkConceptDbId))
            {
                if (project.Status != StructuresProgramType.ProjectStatus.Certified
                    && project.Status != StructuresProgramType.ProjectStatus.QuasiCertified
                    && project.Status != StructuresProgramType.ProjectStatus.Fiips
                    && !project.InPrecertification && !project.InCertification
                    && project.UserAction != StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment
                    && project.UserAction != StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment)
                {
                    MessageBox.Show("Can't add work concept to project because it's already in the project.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                workConceptsAdded.Add(wc);
                
                if (addWorkConcept)
                {
                    //changesMade = true;
                    isDirty = true;
                }

                if (project.Status == StructuresProgramType.ProjectStatus.New
                    && (textBoxStructureProjectFiscalYear.Text.Equals("") ||
                        textBoxStructureProjectFiscalYear.Text.Equals("0")))
                {
                    textBoxStructureProjectFiscalYear.Text = wc.FiscalYear.ToString();
                    project.FiscalYear = wc.FiscalYear;
                    textBoxStructureProjectCost.Text = "0";
                }

                if (updateCost)
                {
                    try
                    {
                        textBoxStructureProjectCost.Text = textBoxStructureProjectCost.Text.Replace(",", "");
                        textBoxStructureProjectCost.Text = (Convert.ToInt32(textBoxStructureProjectCost.Text) + wc.Cost).ToString();
                        textBoxStructureProjectCost.Text = String.Format("{0:n0}", Convert.ToInt32(textBoxStructureProjectCost.Text));
                    }
                    catch { }
                }

                // New project or existing str project that hasn't attained 'certified' status (it's unapproved or precertified)
                if ((project.Status != StructuresProgramType.ProjectStatus.Fiips 
                    && project.Status != StructuresProgramType.ProjectStatus.Certified
                    && project.Status != StructuresProgramType.ProjectStatus.QuasiCertified
                    && wc.Status != StructuresProgramType.WorkConceptStatus.Certified && wc.Status != StructuresProgramType.WorkConceptStatus.Quasicertified)
                    || wc.Status == StructuresProgramType.WorkConceptStatus.Proposed)
                {
                    /*
                    if (wc.FromProposedList)
                    {
                        if (wc.Status == StructuresProgramType.WorkConceptStatus.Proposed)
                        {
                            if (!wc.CertifiedWorkConceptCode.Equals("EV"))
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else
                            {
                                
                                wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                            }
                        }
                        //wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                    }*/
                    //if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                    {

                    }
                    if (secondaryWorkConcepts.Where(swc => swc.WorkConceptCode.Equals(wc.CertifiedWorkConceptCode)).Count() > 0
                        || wc.CertifiedWorkConceptCode.Equals("EV")
                        || wc.StructureId.StartsWith("C") || wc.StructureId.StartsWith("R")
                        || wc.StructureId.StartsWith("N") || wc.StructureId.StartsWith("M")
                        || wc.StructureId.StartsWith("S"))
                    {
                        if (wc.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.None)
                        {
                            wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                            wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.Accept;
                            //wc.PrecertificationDecisionDateTime = DateTime.Now;
                            //wc.PrecertificationDecisionInternalComments = "";

                            if (wc.StructureId.StartsWith("C") || wc.StructureId.StartsWith("R")
                                || wc.StructureId.StartsWith("N") || wc.StructureId.StartsWith("M")
                                || wc.StructureId.StartsWith("S"))
                            {
                                wc.PrecertificationDecisionReasonCategory = "(20) Ancillary Structure";
                                wc.PrecertificationDecisionReasonExplanation = "This is an Ancillary Structure (non-bridge), and the work concept has been automatically Pre-certified. BOS did not review the work concepts to verify they meet BOS asset management philosophy. Regional Bridge Maintenance staff are responsible for recommending and incorporating Ancillary Structure work.";
                            }
                            else if (wc.CertifiedWorkConceptCode.Equals("EV"))
                            {
                                wc.PrecertificationDecisionReasonCategory = "(15) Evaluate for Secondary";
                                wc.PrecertificationDecisionReasonExplanation = "This is a request from the region for BOS to evaluate the structure for secondary work. This work concept has been automatically Pre-certified. If this project has an earliest PSE after 7/1/2025, BOS will identify secondary work concepts when fully certifying the project. If this project has an earliest PSE before 7/1/2025, the region should work with regional Bridge Maintenance staff to re-submit with the most applicable 'secondary' work concept selected.";
                            }
                            else
                            {
                                wc.PrecertificationDecisionReasonCategory = "(10) Secondary Work Concept";
                                wc.PrecertificationDecisionReasonExplanation = "This is a 'secondary' work concept, and the work concept has been automatically Pre-certified. BOS did not review the work concepts to verify they meet BOS asset management philosophy. If this project has an earliest PSE after 7/1/2025, BOS will identify additional secondary work concepts when fully certifying the project. If this project has an earliest PSE before 7/1/2025, the region should work with regional Bridge Maintenance staff to enter notes concerning additional secondary work to be completed on this project.";
                            }
                        }
                    }
                    else if (wc.FromProposedList)
                    {
                        if (wc.Status == StructuresProgramType.WorkConceptStatus.Proposed)
                        {
                            if (!wc.CertifiedWorkConceptCode.Equals("EV"))
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                            }
                        }
                        //wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                    }
                    else if (!wc.Evaluate && wc.WorkConceptCode.Equals(wc.CertifiedWorkConceptCode))
                    {
                        if (wc.Status == StructuresProgramType.WorkConceptStatus.Eligible)
                        {
                            wc.EarlierFiscalYear = allWorkConcepts.Where(e => e.WorkConceptCode.Equals(wc.WorkConceptCode)).First().EarlierFiscalYear;
                            wc.LaterFiscalYear = allWorkConcepts.Where(e => e.WorkConceptCode.Equals(wc.WorkConceptCode)).First().LaterFiscalYear;

                            if (IsWorkConceptPrecertified(wc, Convert.ToInt32(textBoxStructureProjectFiscalYear.Text)))
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                            }
                            else
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                        }
                    }
                }
                
                DataGridView dgv = dataGridViewProjectWorkConcepts;
                dgv.Rows.Add();

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcWorkConceptHistory"].Value = "History";
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcStructureId"].Value = wc.StructureId;

                if (project.ProjectDbId != 0 && wc.WorkConceptDbId != 0)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcWorkConceptHistory"].Value = "History";
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcReviewDetails"].Value = wc.StructureId;
                string workConceptDisplay = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcWorkConcept"].Value = workConceptDisplay;

                

                string certifiedWorkConceptDisplay = String.Format("({0}) {1}", wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription);
                if (wc.Evaluate && !wc.FromProposedList)
                {
                    certifiedWorkConceptDisplay = String.Format("({0}) {1}", "EV", "EVALUATE FOR SECONDARY WORK CONCEPTS");
                    //dgvcCertifiedWorkConcept.Items.Add(evaluate);
                }

                if (!dataServ.IsWorkConceptPrimary(wc.CertifiedWorkConceptCode))
                {
                    //dgvcCertifiedWorkConcept.Items.Add(certifiedWorkConceptDisplay);
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConcept"].Value = certifiedWorkConceptDisplay;

                if (wc.Evaluate && !wc.FromProposedList)
                {
                    //dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConcept"].ReadOnly = true;
                }

                if (wc.FromProposedList)
                {
                    //dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConcept"].ReadOnly = true;
                }

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified 
                    || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                {
                    /*
                    if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified ||
                        wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                    }*/

                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                }
                else
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Value = wc.Status.ToString();

                    if (wc.Status == StructuresProgramType.WorkConceptStatus.Fiips)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.LightBlue;
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified || wc.CertifiedWorkConceptCode.Equals("EV"))
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;
                    }
                }

                if (wc.FiscalYear != 0)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcWorkConceptFiscalYear"].Value = wc.FiscalYear;
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcChangeReason"].Value = wc.ChangeJustifications;
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcNotes"].Value = wc.ChangeJustificationNotes;

                if (wc.FromFiips)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcNotes"].Value = wc.DotProgram + "; " + wc.FiipsDescription;
                }
                else if (wc.FromProposedList)
                {
                    if (String.IsNullOrEmpty(wc.ChangeJustificationNotes))
                    {
                        wc.ChangeJustificationNotes = dataServ.GetRegionNotes(wc);
                        
                        //wc.ChangeJustificationNotes = String.Format("Justification Category: {0}\r\nJustification Explanation: {1}", wc.ReasonCategory, wc.Notes);
                    }

                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcNotes"].Value = wc.ChangeJustificationNotes;
                }
                
                if (!String.IsNullOrEmpty(wc.PrecertificationDecision.ToString()) && wc.PrecertificationDecision != StructuresProgramType.PrecertificatioReviewDecision.None)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcPrecertificationDecision"].Value = wc.PrecertificationDecision.ToString();
                }

                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcWorkConceptDbId"].Value = wc.WorkConceptDbId;

                if (wc.Cost > 0)
                {
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCost"].Value = String.Format("{0:n0}", wc.Cost);
                }

                if (project.Status != StructuresProgramType.ProjectStatus.Fiips)
                {
                    /*
                    if (project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                    {
                        checkBoxUnapproved.Checked = false;
                        //textBoxUnapproved.Visible = false;
                        checkBoxPrecertified.Checked = false;
                        //textBoxPrecertified.Visible = false;
                        checkBoxCertified.Checked = true;
                        //textBoxCertified.Visible = true;
                        linkLabelBosCd.Visible = true;
                    }*/

                    if (project.InCertification)
                    {
                        checkBoxUnapproved.Checked = false;
                        checkBoxPrecertified.Checked = false;

                        if (project.Status == StructuresProgramType.ProjectStatus.Unapproved)
                        {
                            checkBoxUnapproved.Checked = true;
                        }
                        else
                        {
                            checkBoxPrecertified.Checked = true;
                        }
                    }
                    else
                    {
                        linkLabelBosCd.Visible = false;
                        //|| w.Status == StructuresProgramType.WorkConceptStatus.Evaluate)
                        if (workConceptsAdded.Where(w => (w.Status == StructuresProgramType.WorkConceptStatus.Unapproved
                                                                )).Count() > 0
                                                            )
                        {
                            checkBoxUnapproved.Checked = true;
                            checkBoxPrecertified.Checked = false;
                            checkBoxCertified.Checked = false;
                        }
                        else if (workConceptsAdded.All(w => w.Status == StructuresProgramType.WorkConceptStatus.Certified ||
                                    w.Status == StructuresProgramType.WorkConceptStatus.Quasicertified))
                        {
                            checkBoxUnapproved.Checked = false;
                            checkBoxPrecertified.Checked = false;
                            checkBoxCertified.Checked = true;
                            linkLabelBosCd.Visible = true;
                        }
                        else
                        {
                            checkBoxUnapproved.Checked = false;
                            checkBoxPrecertified.Checked = true;
                            checkBoxCertified.Checked = false;
                        }
                    }

                    textBoxUnapproved.Visible = false;
                    textBoxPrecertified.Visible = false;
                    textBoxCertified.Visible = false;

                    if (checkBoxUnapproved.Checked)
                    {
                        //textBoxUnapproved.Visible = true;
                        groupBoxProjectStatus.BackColor = Color.Red;
                    }
                    else if (checkBoxPrecertified.Checked)
                    {
                        //textBoxPrecertified.Visible = true;
                        groupBoxProjectStatus.BackColor = Color.Yellow;
                    }
                    else if (checkBoxCertified.Checked)
                    {
                        //textBoxCertified.Visible = true;
                        groupBoxProjectStatus.BackColor = Color.Green;
                    }

                    /*
                    if (addWorkConcept)
                    {
                        if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length > 0)
                        {
                            textBoxStructureProjectAdvanceableFiscalYear.Focus();
                        }
                    }*/
                }

                if (addWorkConcept)
                {
                    if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length > 0)
                    {
                        textBoxStructureProjectAdvanceableFiscalYear.Focus();
                    }
                    else
                    {
                        textBoxStructureProjectAdvanceableFiscalYear.Focus();
                    }

                    textBoxStructureProjectDescription.Focus();
                }
            }
        }

        private int GetFiscalYear()
        {
            return controller.GetFiscalYear();
        }

        private bool IsWorkConceptPrecertified(WorkConcept wc, int projectFiscalYear)
        {
            return controller.IsWorkConceptPrecertified(wc, projectFiscalYear);
        }

        public void RefreshForm(Project project)
        {
            this.isDirty = false;
            this.savedTransaction = false;
            this.project = project;
            splitContainerWorkConceptsReview.Panel2Collapsed = true;
            UpdateProjectInfo(project);
            workConceptsAdded.Clear();
            dataGridViewProjectWorkConcepts.Rows.Clear();
            
            foreach (WorkConcept wc in project.WorkConcepts)
            {
                WorkConcept originalWorkConcept = new WorkConcept(wc);
                originalWorkConcepts.Add(originalWorkConcept);
                WorkConcept workConcept = new WorkConcept(wc);
                RefreshForm(workConcept, false);
            }

            try
            {
                dataGridViewProjectWorkConcepts.CurrentCell.Selected = false;
            }
            catch { }
        }

        private void UpdateProjectInfo(Project project, bool updateBoxTree = true)
        {
            associatedProject = null;
            groupBoxStructureProjectInfo.Visible = false;
            groupBoxProjectStatus.Visible = false;
            #region Structures, Fiips, Proj Files
            // Structures
            groupBoxStructureProjectInfo.Enabled = true;
            textBoxStructureProjectDbId.Text = "";
            textBoxStructureProjectFiscalYear.Text = "";
            textBoxStructureProjectFiscalYear.ReadOnly = false;
            textBoxStructureProjectAdvanceableFiscalYear.Text = "";
            textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = false;
            textBoxStructureProjectCost.Text = "";
            textBoxStructureProjectCost.ReadOnly = false;
            textBoxAcceptablePseDateStart.Text = "";
            textBoxAcceptablePseDateStart.ReadOnly = false;
            textBoxAcceptablePseDateEnd.Text = "";
            textBoxAcceptablePseDateEnd.ReadOnly = false;
            comboBoxStructureProjectImprovementConcept.SelectedIndex = 0;
            textBoxStructureProjectDescription.Text = "";
            textBoxStructureProjectDescription.ReadOnly = false;
            textBoxStructureProjectNotes.Text = "";
            textBoxStructureProjectNotes.ReadOnly = false;
            textBoxNotificationRecipients.Text = "";
            textBoxNotificationRecipients.ReadOnly = false;
            textBoxProjectHistory.Text = "";
            textBoxProjectHistory.ReadOnly = true;
            tabPagePrecertification.Text = "Precertification";
            tabPageCertification.Text = "Certification";
            tabPageInspection.Text = @"Inspection & Plans";
            tabControlProject.SelectedTab = tabPageProjectInfo;

            // Fiips
            groupBoxFiipsProjectInfo.Enabled = true;
            textBoxStructureProjectFosProjectId.Text = "";
            textBoxStructureProjectFosProjectId.ReadOnly = false;
            textBoxStructureProjectFosProjectId.BackColor = SystemColors.Window;
            textBoxFiipsFiscalYear.Text = "";
            textBoxFiipsCost.Text = "";
            comboBoxStructureProjectFiipsImprovementConcept.SelectedIndex = 0;
            textBoxLifecycleStage.Text = "";
            textBoxStructureProjectFiipsDescription.Text = "";
            buttonUpdateFiipsData.Enabled = true;

            if (userAccount.IsRegionalMaintenanceEngineer || userAccount.IsSuperRead || userAccount.IsRegionalRead)
            {
                buttonUpdateFiipsData.Enabled = false;
                textBoxStructureProjectFosProjectId.ReadOnly = true;
                textBoxStructureProjectFiscalYear.ReadOnly = true;
                textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = true;
                textBoxStructureProjectCost.ReadOnly = true;
                textBoxStructureProjectDescription.ReadOnly = true;
                textBoxStructureProjectNotes.ReadOnly = true;
                textBoxNotificationRecipients.ReadOnly = true;
            }

            if (!userAccount.IsPrecertificationLiaison && !userAccount.IsPrecertificationSupervisor &&
                !userAccount.IsCertificationLiaison && !userAccount.IsCertificationSupervisor)
            {
                textBoxAcceptablePseDateStart.ReadOnly = true;
                textBoxAcceptablePseDateEnd.ReadOnly = true;
            }
            else if (project.Status == StructuresProgramType.ProjectStatus.Certified ||
                        project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
            {
                textBoxAcceptablePseDateStart.ReadOnly = true;
                textBoxAcceptablePseDateEnd.ReadOnly = true;
            }

            // Buttons
            buttonMapStructureProject.Enabled = true;
            buttonMapFiipsProject.Enabled = true;
            buttonCompareProjects.Enabled = false;

            // Precertification, Certification, Inspection Tabs
            groupBoxPrecertification.Visible = false;
            groupBoxCertification.Visible = false;
            groupBoxStructureInspection.Visible = false;
            #endregion Structures, Fiips, Proj Files

            #region Project Create/Edit
            buttonSaveProjectDescription.Enabled = false;
            groupBoxProjectEdit.Visible = false;
            buttonSaveProject.Enabled = false;
            buttonSubmitProjectForPrecertification.Enabled = false;
            buttonDeleteProject.Enabled = false;
            buttonDeleteWorkConceptFromProject.Enabled = false;
            buttonRequestRecertification.Enabled = false;
            buttonUnlockProject.Enabled = false;
            dataGridViewProjectWorkConcepts.ReadOnly = false;

            // Below items currently not in use
            buttonAddNewStructure.Visible = false;
            buttonSubmitProjectForCertification.Visible = false;
            //buttonRequestRecertification.Visible = false;
            //groupBoxAdvancedCertification.Visible = false;
            //groupBoxBosReview.Visible = false;
            #endregion Project Create/Edit

            #region Precertification Review
            groupBoxProjectReview.Visible = false;
            comboBoxPrecertifier.Enabled = false;
            buttonSavePrecertifier.Enabled = false;
            buttonAcceptPrecertification.Enabled = false;
            buttonTransitionallyCertify.Enabled = false;
            buttonRejectPrecertification.Enabled = false;
            #endregion Precertification Review

            #region Precertification Structure-specific Review
            comboBoxPrecertificationDecision.Enabled = false;
            comboBoxPrecertificationReasonCategory.Enabled = false;
            textBoxPrecertificationReasonExplanation.ReadOnly = true;
            buttonSaveStructurePrecertification.Enabled = false;
            labelPrecertificationInternalComments.Visible = false;
            textBoxPrecertificationInternalComments.Visible = false;
            #endregion Precertification Structure-specific Review

            #region Certification Review
            // Project Info Tab
            groupBoxProjectCertification.Visible = false;
            comboBoxCertifier.Enabled = false;
            buttonSaveCertifier.Enabled = false;
            buttonSubmitCertification.Enabled = false;
            buttonApproveCertification.Enabled = false;
            buttonRejectCertification.Enabled = false;
            // Certification/Structure Tab
            buttonSaveStructureCertification.Enabled = false;
            buttonCertifyStructure.Enabled = false;
            comboBoxNewWorkConcept.Enabled = false;
            buttonDenyStructureCertification.Enabled = false;
            dgvPrimaryElements.Enabled = false;
            dgvSecondaryElements.Enabled = false;
            buttonGenerateSecondaryWorkConceptsTable.Enabled = false;
            buttonDeleteSecondaryWorkConceptsTable.Enabled = false;
            groupBoxCertificationAdditionalInfo.Enabled = false;
            textBoxAdditionalPrimaryWorkConceptComments.Enabled = false;
            textBoxAdditionalSecondaryWorkConceptComments.Enabled = false;
            textBoxCertificationGeneralComments.Enabled = false;
            buttonGrantRecertification.Enabled = false;
            buttonRejectRecertification.Enabled = false;
            #endregion Certification Review

            // Data Grid column visibility
            foreach (DataGridViewColumn column in dataGridViewProjectWorkConcepts.Columns)
            {
                column.Visible = false;
            }

            dataGridViewProjectWorkConcepts.Columns["dgvcWorkConcept"].HeaderText = "Eligible Work Concept";
            dataGridViewProjectWorkConcepts.Columns["dgvcWorkConceptFiscalYear"].HeaderText = "Eligible Fy";

            if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
            {
                groupBoxStructureProjectInfo.Text = "Corresponding Structures Project";
                groupBoxFiipsProjectInfo.Text = "Fiips Project";
                tabControlProject.TabPages.Remove(tabControlProject.TabPages["tabPagePrecertification"]);
                tabControlProject.TabPages.Remove(tabControlProject.TabPages["tabPageCertification"]);
                //tabControlProject.TabPages.Remove(tabControlProject.TabPages["tabPageCertification"]);
                tabControlProject.TabPages.Remove(tabControlProject.TabPages["tabPageInspection"]);
                tabControlProject.TabPages.Remove(tabControlProject.TabPages["tabPageProjectStatus"]);
                this.Text = String.Format("Fiips Project: Construction Id {0}", warehouseDatabase.FormatConstructionId(project.FosProjectId));
                tabPageProjectStatus.Text = "Fiips";
                tabPageProjectStatus.BackColor = Color.Blue;
                textBoxStructureProjectFiscalYear.ReadOnly = true;
                textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = true;
                textBoxAcceptablePseDateStart.ReadOnly = true;
                textBoxAcceptablePseDateEnd.ReadOnly = true;
                textBoxStructureProjectCost.Enabled = false;
                groupBoxProjectFilesInBox.Visible = false;
                //buttonCloseReviewPanel.Visible = false;
                UpdateProject(project);
               
                // If there's an associated structures project with this Fiips project, update the UI
                try
                {
                    var matches = structureProjects.Where(s => s.FosProjectId.Equals(project.FosProjectId)).OrderByDescending(p => p.UserActionDateTime);

                    if (matches.Count() > 1)
                    {
                        int previousCount = 0;

                        foreach (var match in matches)
                        {
                            if (match.WorkConcepts.Count() > previousCount)
                            {
                                associatedProject = match;
                            }

                            previousCount = match.WorkConcepts.Count();
                        }
                    }
                    else
                    {
                        associatedProject = matches.First();
                    }
                }
                catch { }

                // Buttons 
                if (associatedProject != null)
                {
                    UpdateProject(associatedProject);
                    buttonMapStructureProject.Enabled = true;
                    groupBoxProjectStatus.Visible = true;
                    //groupBoxProjectFiles.Visible = true;
                }
                else
                {
                    //MessageBox.Show(String.Format("This Fiips project with construction id {0} does not have a corresponding structures project.", project.FosProjectId), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    buttonMapStructureProject.Enabled = false;
                    //groupBoxProjectFiles.Visible = false;
                }

                /*
                if (userAccount.IsAdministrator || userAccount.IsSuperUser
                        || userAccount.IsRegionalProgrammer || userAccount.IsRegionalMaintenanceEngineer)
                {
                    buttonUpdateFiipsData.Enabled = true;
                }
                */

                // Data Grid column visibility
                dataGridViewProjectWorkConcepts.ReadOnly = true;
                dataGridViewProjectWorkConcepts.Columns["dgvcStructureId"].Visible = true;
                dataGridViewProjectWorkConcepts.Columns["dgvcWorkConcept"].Visible = true;
                dataGridViewProjectWorkConcepts.Columns["dgvcWorkConcept"].HeaderText = "Fiips Work Concept";
                dataGridViewProjectWorkConcepts.Columns["dgvcCertifiedWorkConceptStatus"].Visible = false;
                dataGridViewProjectWorkConcepts.Columns["dgvcWorkConceptFiscalYear"].Visible = true;
                dataGridViewProjectWorkConcepts.Columns["dgvcWorkConceptFiscalYear"].HeaderText = "Fiips Fy";
                dataGridViewProjectWorkConcepts.Columns["dgvcNotes"].Visible = true;
            }
            else // Structures Project
            {
                groupBoxStructureProjectInfo.Visible = true;
                groupBoxStructureProjectInfo.Text = "Structures Project";
                groupBoxFiipsProjectInfo.Text = "Corresponding FIIPS Project";
                groupBoxProjectStatus.Visible = true;
                #region Structures, Proj Files
                // If there's an associated project with this structures project, update the UI
                try
                {
                    textBoxStructureProjectFosProjectId.Text = project.FosProjectId;

                    if (project.FosProjectId.Length == 8)
                    {
                        try
                        {
                            associatedProject = fiipsProjects.Where(s => s.FosProjectId.Equals(project.FosProjectId)).First();
                        }
                        catch
                        {
                            Dw.FiipsProject tempFiipsProject = warehouseDatabase.GetFiipsProject(project.FosProjectId);

                            if (tempFiipsProject != null)
                            {
                                associatedProject = new Project();
                                associatedProject.Status = StructuresProgramType.ProjectStatus.Fiips;
                                associatedProject.FosProjectId = tempFiipsProject.ConstructionId;
                                associatedProject.DesignId = tempFiipsProject.DesignId;
                                associatedProject.LifecycleStageCode = tempFiipsProject.LifecycleStageCode;
                                associatedProject.FiipsImprovementConcept = tempFiipsProject.PlanningProjectConceptCode;
                                associatedProject.LetDate = tempFiipsProject.LetDate;
                                associatedProject.FiscalYear = tempFiipsProject.LetDate.Month > 6 && tempFiipsProject.LetDate.Month <= 12 ? tempFiipsProject.LetDate.Year + 1 : tempFiipsProject.LetDate.Year;
                                associatedProject.PseDate = tempFiipsProject.PseDate;
                                associatedProject.EpseDate = tempFiipsProject.EarliestPseDate;
                            }
                        }
                    }

                    if (associatedProject != null)
                    {
                        UpdateProject(associatedProject);
                        buttonMapFiipsProject.Enabled = true;
                    }
                    else
                    {
                        if (project.FosProjectId.Length > 0)
                        {
                            //textBoxStructureProjectFosProjectId.BackColor = Color.Red;
                        }

                        if ((project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification
                                    && project.Status == StructuresProgramType.ProjectStatus.Precertified)
                                    || project.InCertification
                                    || (project.Status == StructuresProgramType.ProjectStatus.Precertified && project.PrecertifyDate.Year != 1))
                        {
                            //MessageBox.Show(String.Format("This structures project {0} does not have a corresponding Fiips project.", project.ProjectDbId), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        buttonMapFiipsProject.Enabled = false;
                    }
                }
                catch { }

                textBoxStructureProjectCost.Enabled = true;
                textBoxStructureProjectDbId.Text = project.ProjectDbId.ToString();
                textBoxStructureProjectFiscalYear.Text = project.FiscalYear.ToString();
                textBoxStructureProjectAdvanceableFiscalYear.Text 
                    = project.AdvanceableFiscalYear != 0 ? project.AdvanceableFiscalYear.ToString() : "";
                //textBoxAcceptablePseDateStart.Text = project.AcceptablePseDateStart.Year != 1 ? project.AcceptablePseDateStart.ToString("MM/dd/yyyy") : String.Format("01/01/{0}", (project.FiscalYear - 1).ToString());
                //textBoxAcceptablePseDateEnd.Text = project.AcceptablePseDateEnd.Year != 1 ? project.AcceptablePseDateEnd.ToString("MM/dd/yyyy") : String.Format("06/30/{0}", (project.FiscalYear - 1).ToString());
                textBoxAcceptablePseDateStart.Text = project.AcceptablePseDateStart.Year != 1 ? project.AcceptablePseDateStart.ToString("MM/dd/yyyy") : "";
                textBoxAcceptablePseDateEnd.Text = project.AcceptablePseDateEnd.Year != 1 ? project.AcceptablePseDateEnd.ToString("MM/dd/yyyy") : "";
                textBoxStructureProjectFosProjectId.ReadOnly = false;

                if (userAccount.IsRegionalMaintenanceEngineer || userAccount.IsSuperRead || userAccount.IsRegionalRead)
                {
                    textBoxStructureProjectFosProjectId.ReadOnly = true;
                    textBoxStructureProjectFiscalYear.ReadOnly = true;
                    textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = true;
                    textBoxStructureProjectCost.ReadOnly = true;
                    textBoxStructureProjectDescription.ReadOnly = true;
                    textBoxStructureProjectNotes.ReadOnly = true;
                    textBoxNotificationRecipients.ReadOnly = true;
                }

                textBoxStructureProjectDescription.Text = project.Description;
                textBoxStructureProjectNotes.Text = project.Notes;
                textBoxNotificationRecipients.Text = project.NotificationRecipients;
                //buttonCloseReviewPanel.Visible = true;

                if (project.StructuresCost == 0)
                {
                    textBoxStructureProjectCost.Text = project.StructuresCost.ToString();
                }
                else if (project.StructuresCost > 0)
                {
                    textBoxStructureProjectCost.Text = String.Format("{0:n0}", project.StructuresCost);
                }

                if (project.Status == StructuresProgramType.ProjectStatus.New)
                {
                    this.Text = String.Format("Structures Project Status: New");
                    comboBoxStructureProjectImprovementConcept.SelectedIndex = 1;
                    buttonMapStructureProject.Enabled = false;
                }
                else
                {
                    string workFlowStatus = dataServ.GetWorkflowStatus(project);

                    /*
                    switch (project.UserAction)
                    {
                        case StructuresProgramType.ProjectUserAction.CreateProject:
                        case StructuresProgramType.ProjectUserAction.SavedProject:
                            workFlowStatus = @"Saved/Not Submitted";
                            break;
                        case StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification:
                            if (project.Status == StructuresProgramType.ProjectStatus.Precertified)
                            {
                                workFlowStatus = @"Precertified/Pending Certification";
                            }
                            else
                            {
                                workFlowStatus = @"Submitted/Pending Precertification";
                            }
                            break;
                        case StructuresProgramType.ProjectUserAction.Precertification:
                            workFlowStatus = @"Precertification in Progress";
                            break;
                        case StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification:
                            workFlowStatus = @"Precertified/Pending Certification";
                            break;
                        case StructuresProgramType.ProjectUserAction.BosRejectedPrecertification:
                            workFlowStatus = @"Precertification Rejected/Modification Required";
                            break;
                        case StructuresProgramType.ProjectUserAction.Certification:
                            workFlowStatus = @"Certification in Progress";
                            break;
                        case StructuresProgramType.ProjectUserAction.BosCertified:
                            workFlowStatus = @"Certified/Well Done!";
                            buttonDeleteFile.Enabled = false;
                            break;
                        case StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment:
                            workFlowStatus = @"Precertification liaison unassigned";
                            break;
                        case StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment:
                            workFlowStatus = @"Certification liaison unassigned";
                            break;
                    }*/

                    string formText = String.Format("Structures Project: Str Proj Id {0}", project.ProjectDbId);
                    this.Text = formText;
                    tabPageProjectStatus.Text = "Project History & Status: " + workFlowStatus;
                    
                    switch (project.Status)
                    {
                        case StructuresProgramType.ProjectStatus.Unapproved:
                            tabPageProjectStatus.BackColor = Color.Red;
                            break;
                        case StructuresProgramType.ProjectStatus.Precertified:
                            tabPageProjectStatus.BackColor = Color.Yellow;
                            break;
                        case StructuresProgramType.ProjectStatus.Certified:
                        case StructuresProgramType.ProjectStatus.QuasiCertified:
                            tabPageProjectStatus.BackColor = Color.Green;
                            break;
                        default:
                            tabPageProjectStatus.BackColor = SystemColors.Control;
                            break;
                    }

                    comboBoxStructureProjectImprovementConcept.SelectedItem = project.StructuresConcept;
                }

                // Project History
                if (project.IsQuasicertified && String.IsNullOrEmpty(project.History))
                {
                    textBoxProjectHistory.Text = "Reviewed and transitionally certified.";
                }
                else
                {
                    // Loop over history
                    textBoxProjectHistory.Text = project.History;
                }
                #endregion Structures, Project Files

                // NOT IN USE: Advanced Certification
                /*
                checkBoxRequestAdvancedCertification.Checked = project.RequestAdvancedCertification;

                if (project.RequestAdvancedCertification && project.AdvancedCertificationDate.Year >= currentFiscalYear)
                {
                    if (project.AdvancedCertificationDate < dateTimePickerAdvancedCertification.MinDate)
                    {
                        dateTimePickerAdvancedCertification.MinDate = project.AdvancedCertificationDate;
                    }

                    dateTimePickerAdvancedCertification.Value = project.AdvancedCertificationDate.Date;
                }
                */

                // Data Grid column visibility
                foreach (DataGridViewColumn column in dataGridViewProjectWorkConcepts.Columns)
                {
                    if (!column.Name.Equals("dgvcWorkConceptDbId") && !column.Name.Equals("dgvcPrecertificationDecision")) 
                    {
                        column.Visible = true;
                    }
                }

                string precertificationStatus = dataServ.GetLastPrecertificationOrCertification(project.ProjectDbId, "precertification");
                string certificationStatus = dataServ.GetLastPrecertificationOrCertification(project.ProjectDbId, "certification");
                labelPrecertifier.Text = precertificationStatus;
                labelCertifier.Text = certificationStatus;

                if (project.PrecertificationLiaisonUserDbId != 0)
                {
                    string liaison = project.PrecertificationLiaisonUserFullName + " (" + project.PrecertificationLiaisonUserDbId + ")";
                    comboBoxPrecertifier.SelectedItem = liaison;
                    /*
                    for (int i = 0; i < comboBoxPrecertifier.Items.Count; i++)
                    {
                        string l = comboBoxPrecertifier.Items[i].ToString();
                    }*/
                }

                if (project.CertificationLiaisonUserDbId != 0)
                {
                    string liaison = project.CertificationLiaisonUserFullName + " (" + project.CertificationLiaisonUserDbId + ")";
                    comboBoxCertifier.SelectedItem = liaison;
                }

                /*
                textBoxStructureProjectFiscalYear.ReadOnly = false;
                textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = false;
                comboBoxStructureProjectImprovementConcept.Enabled = true;
                textBoxStructureProjectCost.ReadOnly = false;
                textBoxStructureProjectDescription.ReadOnly = false;
                textBoxStructureProjectNotes.ReadOnly = false;
                textBoxProjectHistory.ReadOnly = true;*/
                dataGridViewProjectWorkConcepts.ReadOnly = false;
                dataGridViewProjectWorkConcepts.Columns["dgvcChangeReason"].Visible = true;
                dataGridViewProjectWorkConcepts.Columns["dgvcNotes"].Visible = true;
                //dataGridViewProjectWorkConcepts.Columns["dgvcPrecertificationDecision"].Visible = true;

                if (userAccount.IsRegionalProgrammer
                                    || userAccount.IsSuperUser || userAccount.IsAdministrator
                                    || userAccount.IsCertificationLiaison
                                    || userAccount.IsPrecertificationLiaison
                                    || userAccount.IsCertificationSupervisor
                                    || userAccount.IsPrecertificationSupervisor)
                {
                    buttonUpdateFiipsData.Enabled = true;
                    groupBoxProjectEdit.Visible = true;

                    if (project.ProjectDbId > 0)
                    //&& project.Status != StructuresProgramType.ProjectStatus.Certified
                    //&& project.Status != StructuresProgramType.ProjectStatus.QuasiCertified)
                    {
                        buttonSaveProjectDescription.Enabled = true;
                    }
                }

                if (userAccount.IsPrecertificationLiaison || userAccount.IsPrecertificationSupervisor
                                || userAccount.IsCertificationLiaison || userAccount.IsCertificationSupervisor
                                || userAccount.IsSuperUser || userAccount.IsAdministrator)
                {
                    labelPrecertificationInternalComments.Visible = true;
                    textBoxPrecertificationInternalComments.Visible = true;
                }
                else
                {
                    labelPrecertificationInternalComments.Visible = false;
                    textBoxPrecertificationInternalComments.Visible = false;
                }

                if ((project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                    && (project.UserAction == StructuresProgramType.ProjectUserAction.BosCertified
                        || project.UserAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified
                        || project.UserAction == StructuresProgramType.ProjectUserAction.RequestRecertification
                        )
                )
                {
                    textBoxStructureProjectFiscalYear.ReadOnly = true;
                    textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = true;
                    textBoxAcceptablePseDateStart.ReadOnly = true;
                    textBoxAcceptablePseDateEnd.ReadOnly = true;
                    comboBoxStructureProjectImprovementConcept.Enabled = false;
                    //textBoxStructureProjectCost.ReadOnly = true;
                    //textBoxStructureProjectDescription.ReadOnly = true;
                    //textBoxStructureProjectNotes.ReadOnly = true;
                    //textBoxNotificationRecipients.ReadOnly = true;
                    textBoxProjectHistory.ReadOnly = true;
                    dataGridViewProjectWorkConcepts.ReadOnly = true;
                    //dataGridViewProjectWorkConcepts.Columns["dgvcChangeReason"].Visible = false;
                    //dataGridViewProjectWorkConcepts.Columns["dgvcNotes"].Visible = false;
                    //dataGridViewProjectWorkConcepts.Columns["dgvcPrecertificationDecision"].Visible = false;

                    if ((project.UserAction == StructuresProgramType.ProjectUserAction.BosCertified
                        || project.UserAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified) && project.ProjectDbId > 999)
                    {
                        if (userAccount.IsRegionalProgrammer
                            || userAccount.IsSuperUser || userAccount.IsAdministrator)
                        {
                            groupBoxProjectEdit.Visible = true;
                            buttonRequestRecertification.Enabled = true;
                        }
                    }
                    else if (project.UserAction == StructuresProgramType.ProjectUserAction.RequestRecertification && project.ProjectDbId > 999)
                    {
                        if (userAccount.IsPrecertificationLiaison || userAccount.IsCertificationLiaison
                            || userAccount.IsPrecertificationSupervisor || userAccount.IsCertificationSupervisor
                            || userAccount.IsSuperUser || userAccount.IsAdministrator)
                        {
                            groupBoxProjectCertification.Visible = true;
                            buttonGrantRecertification.Enabled = true;
                            buttonRejectRecertification.Enabled = true;
                        }
                    }
                }
                else
                {
                    switch (project.Status)
                    {
                        /*
                        case StructuresProgramType.ProjectStatus.Certified:
                        case StructuresProgramType.ProjectStatus.QuasiCertified:
                            textBoxStructureProjectFiscalYear.ReadOnly = true;
                            textBoxStructureProjectAdvanceableFiscalYear.ReadOnly = true;
                            comboBoxStructureProjectImprovementConcept.Enabled = false;
                            textBoxStructureProjectCost.ReadOnly = true;
                            textBoxStructureProjectDescription.ReadOnly = true;
                            textBoxStructureProjectNotes.ReadOnly = true;
                            textBoxProjectHistory.ReadOnly = true;
                            dataGridViewProjectWorkConcepts.ReadOnly = true;
                            dataGridViewProjectWorkConcepts.Columns["dgvcChangeReason"].Visible = false;
                            dataGridViewProjectWorkConcepts.Columns["dgvcNotes"].Visible = false;
                            dataGridViewProjectWorkConcepts.Columns["dgvcPrecertificationDecision"].Visible = false;

                            if ((project.UserAction == StructuresProgramType.ProjectUserAction.BosCertified
                                || project.UserAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified) && project.ProjectDbId > 999)
                            {
                                if (userAccount.IsRegionalProgrammer
                                    || userAccount.IsSuperUser || userAccount.IsAdministrator)
                                {
                                    groupBoxProjectEdit.Visible = true;
                                    buttonRequestRecertification.Enabled = true;
                                }
                            }
                            else if (project.UserAction == StructuresProgramType.ProjectUserAction.RequestRecertification && project.ProjectDbId > 999)
                            {
                                if (userAccount.IsPrecertificationLiaison || userAccount.IsCertificationLiaison
                                    || userAccount.IsPrecertificationSupervisor || userAccount.IsCertificationSupervisor
                                    || userAccount.IsSuperUser || userAccount.IsAdministrator)
                                {
                                    groupBoxProjectCertification.Visible = true;
                                    buttonGrantRecertification.Enabled = true;
                                    buttonRejectRecertification.Enabled = true;
                                }
                            }

                            break;*/
                        case StructuresProgramType.ProjectStatus.New:
                            if (userAccount.IsRegionalProgrammer
                                    || userAccount.IsSuperUser || userAccount.IsAdministrator)
                            {
                                groupBoxProjectEdit.Visible = true;
                                buttonSaveProject.Enabled = true;
                                buttonSubmitProjectForPrecertification.Enabled = true;
                                buttonDeleteWorkConceptFromProject.Enabled = true;
                            }

                            dataGridViewProjectWorkConcepts.Columns["dgvcPrecertificationDecision"].Visible = false;
                            dataGridViewProjectWorkConcepts.Columns["dgvcReviewDetails"].Visible = false;
                            break;
                        default:
                            // Project Create/Edit
                            if (userAccount.IsRegionalProgrammer
                                    || userAccount.IsSuperUser || userAccount.IsAdministrator
                                    || userAccount.IsCertificationLiaison
                                    || userAccount.IsPrecertificationLiaison
                                    || userAccount.IsCertificationSupervisor
                                    || userAccount.IsPrecertificationSupervisor)
                            {
                                buttonUpdateFiipsData.Enabled = true;
                                groupBoxProjectEdit.Visible = true;

                                if (project.ProjectDbId > 0)
                                    //&& project.Status != StructuresProgramType.ProjectStatus.Certified
                                    //&& project.Status != StructuresProgramType.ProjectStatus.QuasiCertified)
                                {
                                    buttonSaveProjectDescription.Enabled = true;
                                }

                                if (project.Locked)
                                {
                                    dataGridViewProjectWorkConcepts.ReadOnly = true;
                                    //buttonWithdrawCertification.Enabled = true;

                                    if (userAccount.IsSuperUser || userAccount.IsAdministrator)
                                    {
                                        buttonUnlockProject.Enabled = true;
                                    }
                                }
                                else
                                {
                                    if (project.UserAction != StructuresProgramType.ProjectUserAction.RejectRecertification)
                                    {
                                        buttonSaveProject.Enabled = true;
                                        buttonSubmitProjectForPrecertification.Enabled = true;
                                        buttonDeleteProject.Enabled = true;
                                        buttonDeleteWorkConceptFromProject.Enabled = true;
                                    }
                                    else
                                    {
                                        if (project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                                        {
                                            if (userAccount.IsRegionalProgrammer
                                                || userAccount.IsSuperUser || userAccount.IsAdministrator)
                                            {
                                                groupBoxProjectEdit.Visible = true;
                                                buttonRequestRecertification.Enabled = true;
                                            }
                                        }
                                    }
                                }
                            } // end if (userAccount.IsRegionalProgrammer || userAccount.IsRegionalMaintenanceEngineer
                              // || userAccount.IsSuperUser || userAccount.IsAdministrator || userAccount.IsOmniscient)

                            // Precertification info
                            groupBoxProjectReview.Visible = true;

                            /*
                            if (project.InPrecertification)
                            {
                                precertificationStatus = "In Precertification by " + project.PrecertificationLiaisonUserFullName;
                            }
                            else if (project.PrecertifyDate.Year != 1)
                            {
                                precertificationStatus = String.Format("Precertified on {0} by {1}", project.PrecertifyDate, project.PrecertificationLiaisonUserFullName);
                            }
                            /*
                            else if (project.PrecertifyDate.Year == 1) // Grab the last project precertification event from the database
                            {

                            }*/
                            /*
                            else if (project.UserAction == StructuresProgramType.ProjectUserAction.BosRejectedPrecertification)
                            {
                                precertificationStatus = String.Format("Rejected for precertification by {0} on {1}", project.PrecertificationLiaisonUserFullName, project.UserActionDateTime);
                            }
                            else if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification)
                            {
                                if (project.Status == StructuresProgramType.ProjectStatus.Precertified)
                                {
                                    precertificationStatus = String.Format("Auto-Precertified on {0}", project.UserActionDateTime);
                                }
                                else if (project.Status == StructuresProgramType.ProjectStatus.Unapproved)
                                {
                                    precertificationStatus = String.Format("Auto-Unapproved on {0}", project.UserActionDateTime);
                                }
                            }*/

                            if (!project.InPrecertification)
                            {
                                comboBoxPrecertificationDecision.Enabled = false;
                                comboBoxPrecertificationReasonCategory.Enabled = false;
                                textBoxPrecertificationReasonExplanation.ReadOnly = true;
                                textBoxPrecertificationReasonExplanation.Enabled = false;
                                labelPrecertificationInternalComments.Visible = false;
                                //textBoxPrecertificationInternalComments.Enabled = false;
                            }

                            labelPrecertifier.Text = precertificationStatus;

                            if (userAccount.IsPrecertificationSupervisor || userAccount.IsPrecertificationLiaison)
                            {
                                if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification ||
                                    project.InPrecertification || project.UserAction == StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment)
                                {
                                    comboBoxPrecertifier.Enabled = true;
                                    buttonSavePrecertifier.Enabled = true;
                                }

                                buttonSaveStructurePrecertification.Enabled = true;
                                textBoxPrecertificationInternalComments.Enabled = true;

                                if (project.InPrecertification)
                                {
                                    comboBoxPrecertificationDecision.Enabled = true;
                                    comboBoxPrecertificationReasonCategory.Enabled = true;
                                    textBoxPrecertificationReasonExplanation.ReadOnly = false;
                                    textBoxPrecertificationReasonExplanation.Enabled = true;
                                    UpdateProjectPrecertificationAcceptReject();
                                }

                                if ((project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification || project.UserAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification) && (project.Status == StructuresProgramType.ProjectStatus.Precertified || project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified))
                                {
                                    buttonTransitionallyCertify.Enabled = true;
                                }
                            }

                            if (userAccount.IsPrecertificationLiaison || userAccount.IsPrecertificationSupervisor
                                || userAccount.IsCertificationLiaison || userAccount.IsCertificationSupervisor
                                || userAccount.IsSuperUser || userAccount.IsAdministrator)
                            {
                                labelPrecertificationInternalComments.Visible = true;
                                textBoxPrecertificationInternalComments.Visible = true;
                            }
                            else
                            {
                                labelPrecertificationInternalComments.Visible = false;
                                textBoxPrecertificationInternalComments.Visible = false;
                            }
                            
                            groupBoxProjectCertification.Visible = true;

                            if (project.InCertification || project.UserAction == StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment)
                            {
                                /*
                                if (project.InCertification)
                                {
                                    certificationStatus = "In Certification by " + project.CertificationLiaisonUserFullName;
                                }
                                else if (project.UserAction == StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment)
                                {
                                    certificationStatus = "Certification liaison was unassigned";
                                }*/

                                //project.UserAction == StructuresProgramType.ProjectUserAction.Certification && 
                                if (project.WorkConcepts.All(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                                    && (userAccount.IsCertificationLiaison || userAccount.IsCertificationSupervisor || userAccount.IsAdministrator))
                                {
                                    buttonSubmitCertification.Enabled = true;
                                }

                                if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification
                                    && project.WorkConcepts.All(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                                    && (userAccount.IsCertificationSupervisor || userAccount.IsAdministrator))
                                {
                                    buttonApproveCertification.Enabled = true;
                                }

                                if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection
                                    && project.WorkConcepts.Any(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                                    && (userAccount.IsCertificationSupervisor || userAccount.IsAdministrator || userAccount.IsCertificationLiaison))
                                {
                                    buttonRejectCertification.Enabled = true;
                                }
                            }
                            else if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                            {
                                if (String.IsNullOrEmpty(project.History))
                                {
                                    certificationStatus = "Transitionally certified by BOS";
                                }
                                else
                                {
                                    // Grab the last certification record
                                }
                            }
                            else if (project.Status == StructuresProgramType.ProjectStatus.Certified)
                            {
                                // Grab the last certification record
                            }
                            /*
                            else if (project.CertifyDate.Year != 1)
                            {
                                certificationStatus = String.Format("Certified on {0} by {1}", project.CertifyDate, project.CertificationLiaisonUserFullName);
                            }*/


                            labelCertifier.Text = certificationStatus;

                            /*
                            if (userAccount.IsCertificationSupervisor && 
                                (project.Status == StructuresProgramType.ProjectStatus.Precertified || project.Status == StructuresProgramType.ProjectStatus.Certified 
                                    || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified) 
                                && !project.InPrecertification && project.UserAction != StructuresProgramType.ProjectUserAction.RejectRecertification)*/
                            if ((userAccount.IsCertificationSupervisor
                                && project.Status == StructuresProgramType.ProjectStatus.Precertified
                                && !project.InPrecertification) 
                                || (userAccount.IsCertificationSupervisor && project.InCertification)
                                || (userAccount.IsCertificationSupervisor && project.UserAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification && (project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified))
                                )
                            {
                                comboBoxCertifier.Enabled = true;
                                buttonSaveCertifier.Enabled = true;
                            }

                            if ((userAccount.IsCertificationSupervisor || userAccount.IsCertificationLiaison) && project.InCertification)
                            {
                                buttonSaveStructureCertification.Enabled = true;
                                buttonCertifyStructure.Enabled = true;
                                comboBoxNewWorkConcept.Enabled = true;
                                buttonDenyStructureCertification.Enabled = true;
                                dgvPrimaryElements.Enabled = true;
                                dgvSecondaryElements.Enabled = true;
                                buttonGenerateSecondaryWorkConceptsTable.Enabled = true;
                                buttonDeleteSecondaryWorkConceptsTable.Enabled = true;
                                groupBoxCertificationAdditionalInfo.Enabled = true;
                                textBoxAdditionalPrimaryWorkConceptComments.Enabled = true;
                                textBoxAdditionalSecondaryWorkConceptComments.Enabled = true;
                                textBoxCertificationGeneralComments.Enabled = true;
                            }

                            break;
                    }
                }

                /*
                if (project.Status == StructuresProgramType.ProjectStatus.Certified)
                {
                    textBoxStructureProjectFiscalYear.ReadOnly = true;
                    comboBoxStructureProjectImprovementConcept.Enabled = false;
                    textBoxStructureProjectCost.ReadOnly = true;
                    textBoxStructureProjectDescription.ReadOnly = true;
                    textBoxStructureProjectNotes.ReadOnly = true;
                    textBoxProjectHistory.ReadOnly = true;
                    dataGridViewProjectWorkConcepts.ReadOnly = true;
                    dataGridViewProjectWorkConcepts.Columns["dgvcChangeReason"].Visible = false;
                    dataGridViewProjectWorkConcepts.Columns["dgvcNotes"].Visible = false;
                    dataGridViewProjectWorkConcepts.Columns["dgvcPrecertificationDecision"].Visible = false;
                }
                else //New, Unapproved, or Precertified
                {
                    if (project.Status != StructuresProgramType.ProjectStatus.New && (userAccount.IsAdministrator || userAccount.IsSuperUser
                        || userAccount.IsRegionalProgrammer || userAccount.IsRegionalMaintenanceEngineer))
                    {
                        buttonUpdateFiipsData.Enabled = true;
                    }


                }*/
            } // End - Structures Project

            if (project != null && associatedProject != null)
            {
                buttonCompareProjects.Enabled = true;
            }

            if (updateBoxTree && dataServ.EnableBox() && project.Status != StructuresProgramType.ProjectStatus.Fiips)
            {
                groupBoxProjectFilesInBox.Visible = true;

                if (!String.IsNullOrEmpty(project.BoxId))
                {
                    FileManagerService.ROOT_FOLDER = project.BoxId;
                    Task<Item> boxItem = fileManager.UpdateProjectFileTree(project, tvFiles, picLoading);
                }
            }
        }

        internal string FormatConstructionId(string constructionId)
        {
            return controller.FormatConstructionId(constructionId);
        }

        private void UpdateProject(Project project)
        {
            if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
            {
                //groupBoxStructureProjectInfo.Visible = false;
                textBoxStructureProjectFosProjectId.Text = project.FosProjectId;
                textBoxStructureProjectFosProjectId.ReadOnly = true;
                textBoxStructureProjectFosDesignId.Text = project.DesignId.Equals("0") ? "" : project.DesignId;
                //textBoxStructureProjectFosDesignId.ReadOnly = true;
                textBoxFiipsFiscalYear.Text = project.FiscalYear.ToString();
                linkLabelBosCd.Visible = false;

                try
                {
                    if (project.PseDate.Year != 1)
                    {
                        textBoxFiipsPseDate.Text = project.PseDate.ToShortDateString();
                    }
                    else
                    {
                        textBoxFiipsPseDate.Text = "";
                    }
                }
                catch { }

                try
                {
                    if (project.EpseDate.Year != 1)
                    {
                        textBoxFiipsEarliestPseDate.Text = project.EpseDate.ToShortDateString();
                    }
                    else
                    {
                        textBoxFiipsEarliestPseDate.Text = "";
                    }
                }
                catch { }

                //textBoxFiipsCost.Text = project.FiipsCost.ToString();
                textBoxFiipsCost.Text = String.Format("{0:n0}", project.FiipsCost);
                comboBoxStructureProjectFiipsImprovementConcept.Text = project.FiipsImprovementConcept;
                textBoxLifecycleStage.Text = project.LifecycleStageCode;
                textBoxStructureProjectFiipsDescription.Text = project.Description;
            }
            else
            {
                //groupBoxStructureProjectInfo.Visible = true;
                textBoxStructureProjectDbId.Text = project.ProjectDbId.ToString();

                if (project.StructuresCost > 0)
                {
                    textBoxStructureProjectCost.Text = String.Format("{0:n0}", project.StructuresCost);
                }

                //textBoxStructureProjectDbId.Text = project.ProjectDbId.ToString();
                textBoxStructureProjectFiscalYear.Text = project.FiscalYear.ToString();
                textBoxStructureProjectAdvanceableFiscalYear.Text 
                    = project.AdvanceableFiscalYear != 0 ? project.AdvanceableFiscalYear.ToString() : "";
                textBoxStructureProjectDescription.Text = project.Description;
                textBoxStructureProjectNotes.Text = project.Notes;
                textBoxNotificationRecipients.Text = project.NotificationRecipients;
                linkLabelBosCd.Visible = false;

                switch (project.Status)
                {
                    case StructuresProgramType.ProjectStatus.Certified:
                    case StructuresProgramType.ProjectStatus.QuasiCertified:
                        checkBoxCertified.Checked = true;
                        checkBoxPrecertified.Checked = false;
                        checkBoxUnapproved.Checked = false;
                        linkLabelBosCd.Visible = true;
                        break;
                    case StructuresProgramType.ProjectStatus.Precertified:
                        checkBoxCertified.Checked = false;
                        checkBoxPrecertified.Checked = true;
                        checkBoxUnapproved.Checked = false;
                        break;
                    case StructuresProgramType.ProjectStatus.Unapproved:
                        checkBoxCertified.Checked = false;
                        checkBoxPrecertified.Checked = false;
                        checkBoxUnapproved.Checked = true;
                        break;
                }

                textBoxUnapproved.Visible = false;
                textBoxPrecertified.Visible = false;
                textBoxCertified.Visible = false;

                if (checkBoxUnapproved.Checked)
                {
                    //textBoxUnapproved.Visible = true;
                    groupBoxProjectStatus.BackColor = Color.Red;
                }
                else if (checkBoxPrecertified.Checked)
                {
                    //textBoxPrecertified.Visible = true;
                    groupBoxProjectStatus.BackColor = Color.Yellow;
                }
                else if (checkBoxCertified.Checked)
                {
                    //textBoxCertified.Visible = true;
                    groupBoxProjectStatus.BackColor = Color.Green;
                }

                if (project.StructuresCost > 0)
                {
                    //textBoxStructureProjectCost.Text = project.StructuresCost.ToString();
                }

                comboBoxStructureProjectImprovementConcept.SelectedItem = project.StructuresConcept;
                
                /*
                if (project.IsQuasicertified)
                {
                    textBoxProjectHistory.Text = "Reviewed and certified (without BOSCD) as part of the transitional implementation of the BOS certification process.";
                }
                else
                {
                    textBoxProjectHistory.Text = project.History;
                }*/
            }
        }

        private void UpdateGridVisibility(bool visible)
        {
            dataGridViewProjectWorkConcepts.Columns[2].Visible = visible;
            dataGridViewProjectWorkConcepts.Columns[4].Visible = false;
            dataGridViewProjectWorkConcepts.Columns[6].Visible = visible;
            dataGridViewProjectWorkConcepts.Columns[8].Visible = visible;
        }

        private void dataGridViewProjectWorkConcepts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;
            axAcroPDF1.Visible = false;

            if (rowIndex >= 0 && columnIndex >= 0)
            {
                string columnName = senderGrid.Columns[columnIndex].Name;
                string structureId = "";
                Dw.Structure dwStructure = null;
                int workConceptDbId = 0;

                try
                {
                    structureId = senderGrid.Rows[rowIndex].Cells["dgvcStructureId"].Value.ToString().Trim();
                }
                catch { }

                string workConceptStatus = senderGrid.Rows[rowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value.ToString().Trim();
                string workConceptDescription = senderGrid.Rows[rowIndex].Cells["dgvcWorkConcept"].Value.ToString().Trim();
                string certifiedWorkConcept = senderGrid.Rows[rowIndex].Cells["dgvcCertifiedWorkConcept"].Value.ToString().Trim();

                if (workConceptDescription.StartsWith("(PR)"))
                {
                    workConceptStatus = "PROPOSE";
                }

                try
                {
                    workConceptDbId = Convert.ToInt32(senderGrid.Rows[rowIndex].Cells["dgvcWorkConceptDbId"].Value);
                }
                catch { }

                switch (columnName.ToUpper())
                {
                    case "DGVCWORKCONCEPTHISTORY":
                        OpenWorkConceptHistory(structureId, workConceptDbId, project);
                        break;
                    case "DGVCSTRUCTUREID":
                        if (!String.IsNullOrEmpty(structureId))
                        {
                            formMapping.OpenStructureWindow(structureId);
                        }

                        break;
                    case "DGVCCERTIFIEDWORKCONCEPTSTATUS":
                        if (workConceptStatus.Equals("BOSCD"))
                        {
                            DateTime dateTime = DateTime.Now;
                            string xfdfFilePath = Path.Combine(certificationDirectory, String.Format("{0}-{1}.xfdf", structureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
                            string pdfFilePath = Path.Combine(certificationDirectory, "boscd.pdf");
                            string xfdfFilePathInTempDir = Path.Combine(tempDirectory, String.Format("{0}-{1}.xfdf", structureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
                            string pdfFilePathInTempDir = Path.Combine(tempDirectory, "boscd.pdf");

                            if (!Directory.Exists(tempDirectory))
                            {
                                Directory.CreateDirectory(tempDirectory);
                            }

                            string newPdfFilePath = Path.Combine(tempDirectory, String.Format("{0}-{1}.pdf", structureId, dateTime.ToString("yyyy-MM-dd-hhmmsstt")));
                            string signatureFilePath = Path.Combine(certificationDirectory, "billdreher.jpg");
                            Structure sptStructure = dataServ.GetSptStructure(structureId);
                            ReportWriterService.CreateStructureCd(xfdfFilePathInTempDir, pdfFilePath, newPdfFilePath, signatureFilePath, project,
                                                                sptStructure,
                                                                project.WorkConcepts.Where(w => w.WorkConceptDbId == workConceptDbId).First(),
                                                                dateTime,
                                                                associatedProject,
                                                                true);
                        }
                        break;
                    case "DGVCREVIEWDETAILS":
                        if (senderGrid.Rows[rowIndex].Cells["dgvcReviewDetails"].Value.ToString().Trim().Length > 0)
                        {
                            groupBoxPrecertification.Visible = true;
                            groupBoxCertification.Visible = true;
                            groupBoxStructureInspection.Visible = true;
                            string formattedStructureId = Utility.FormatStructureId(structureId);
                            tabPagePrecertification.Text = String.Format("Precertification: {0}", formattedStructureId);
                            tabPageCertification.Text = String.Format("Certification: {0}", formattedStructureId);
                            tabPageInspection.Text = String.Format("Inspection & Plans: {0}", formattedStructureId);
                            groupBoxStructureInspection.Text = String.Format("Structure Id: {0}", formattedStructureId);
                            labelWorkConceptStatus.Text = String.Format("{0}", workConceptStatus);
                            textBoxCurrentWorkConceptDbId.Text = workConceptDbId.ToString();
                            textBoxCurrentStructureId.Text = structureId;

                            if (structures.Where(s => s != null && s.StructureId.Equals(structureId)).Count() > 0)
                            {
                                dwStructure = structures.Where(s => s.StructureId.Equals(structureId)).First();
                            }
                            else
                            {
                                try
                                {
                                    dwStructure = warehouseDatabase.GetStructure(structureId, true, false);
                                    structures.Add(dwStructure);
                                }
                                catch
                                {
                                    MessageBox.Show("Unable to retrieve structure data for " + structureId + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }

                            linkLabelStructureId.Text = structureId;
                            labelFeatureOnUnder.Text = "";
                            labelLastInspectionDate.Text = "";
                            labelLastInspectionFilePath.Text = "";

                            if (dwStructure != null)
                            {
                                string featureOnUnder = dwStructure.ServiceFeatureOn;

                                if (!String.IsNullOrEmpty(dwStructure.ServiceFeatureUnder))
                                {
                                    featureOnUnder += " over " + dwStructure.ServiceFeatureUnder;
                                }

                                labelFeatureOnUnder.Text = featureOnUnder;
                                string lastInspectionFilePath = dwStructure.LastInspection.InspectionFilePath;
                                string lastInspectionDate = dwStructure.LastInspection.InspectionDate.ToString("yyyy/MM/dd");
                                labelLastInspectionDate.Text = "Last Inspection: " + lastInspectionDate;
                                labelLastInspectionFilePath.Text = lastInspectionFilePath;
                                //OpenInspection(lastInspectionFilePath);

                                List<DateTime> inspectionDates = warehouseDatabase.GetInspectionDates(structureId);
                                comboBoxInspectionDate.Items.Clear();

                                foreach (DateTime inspectionDate in inspectionDates)
                                {
                                    int result = DateTime.Compare(inspectionDate, new DateTime(2014, 3, 10));

                                    if (result >= 0)
                                    {
                                        comboBoxInspectionDate.Items.Add(inspectionDate.ToString("yyyy/MM/dd"));
                                    }
                                }

                                if (comboBoxInspectionDate.Items.Count > 0)
                                {
                                    comboBoxInspectionDate.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Unable to retrieve structure data for " + structureId + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }

                            // Bridge Plans
                            List<int> plansFolders = warehouseDatabase.GetPlansFolders(structureId);
                            comboBoxPlansYear.Items.Clear();

                            foreach (var plansFolder in plansFolders)
                            {
                                comboBoxPlansYear.Items.Add(plansFolder);
                            }

                            if (comboBoxPlansYear.Items.Count > 0)
                            {
                                comboBoxPlansYear.SelectedIndex = 0;
                                buttonViewPlans.Enabled = true;
                            }
                            else
                            {
                                buttonViewPlans.Enabled = false;
                            }

                            // Populate Precertification
                            //groupBoxPrecertification.Text = String.Format("Precertification  {0}", structureId);
                            var wcs = workConceptsAdded.Where(w => w.WorkConceptDbId == workConceptDbId && w.StructureId.Equals(structureId));
                            currentWorkConcept = wcs.Count() > 0 ? wcs.First() : null;
                            comboBoxPrecertificationDecision.SelectedItem = (int)currentWorkConcept.PrecertificationDecision == 0 ? "" : currentWorkConcept.PrecertificationDecision.ToString();
                            comboBoxPrecertificationReasonCategory.SelectedItem = currentWorkConcept.PrecertificationDecisionReasonCategory;
                            textBoxPrecertificationReasonExplanation.Text = currentWorkConcept.PrecertificationDecisionReasonExplanation;
                            textBoxPrecertificationInternalComments.Text = currentWorkConcept.PrecertificationDecisionInternalComments;
                            labelWorkConceptInPrecertification.Text = String.Format("Work Concept To Be Certified: ({0}) {1}", currentWorkConcept.CertifiedWorkConceptCode, currentWorkConcept.CertifiedWorkConceptDescription);
                            labelLastPrecertificationId.Text = currentWorkConcept.ProjectWorkConceptHistoryDbId.ToString();
                            labelPrecertificationLastAction.Text = currentWorkConcept.PrecertificationDecisionDateTime.ToString("MM/dd/yyyy");

                            if (currentWorkConcept.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.None
                                || !project.InPrecertification)
                            {
                                var wcHistory = dataServ.GetProjectWorkConceptHistory(structureId, workConceptDbId, project);
                                //&& !String.IsNullOrEmpty(h.PrecertificationDecisionInternalComments)
                                var lastPrecertifications = wcHistory.Where(h => (h.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.Accept
                                                                            || h.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.Reject
                                                                            || h.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.AutoAccept
                                                                            || (h.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.None
                                                                                    && (!String.IsNullOrEmpty(h.PrecertificationDecisionReasonExplanation) || !String.IsNullOrEmpty(h.PrecertificationDecisionInternalComments))
                                                                                ) 
                                                                            )).OrderByDescending(h => h.PrecertificationDecisionDateTime);

                                if (lastPrecertifications.Count() > 0)
                                {
                                    var lastPrecertification = lastPrecertifications.First();
                                    labelLastPrecertificationId.Text = lastPrecertification.ProjectWorkConceptHistoryDbId.ToString();
                                    comboBoxPrecertificationDecision.SelectedItem = (int)lastPrecertification.PrecertificationDecision == 0 ? "" : lastPrecertification.PrecertificationDecision.ToString();
                                    comboBoxPrecertificationReasonCategory.SelectedItem = lastPrecertification.PrecertificationDecisionReasonCategory;
                                    textBoxPrecertificationReasonExplanation.Text = lastPrecertification.PrecertificationDecisionReasonExplanation;
                                    textBoxPrecertificationInternalComments.Text = lastPrecertification.PrecertificationDecisionInternalComments;
                                    labelLastPrecertificationId.Text = currentWorkConcept.ProjectWorkConceptHistoryDbId.ToString();
                                    labelPrecertificationLastAction.Text = currentWorkConcept.PrecertificationDecisionDateTime.ToString("MM/dd/yyyy");
                                    //labelPrecertificationLastAction.Text = lastPrecertification.PrecertificationDecisionDateTime.ToString("MM/dd/yyyy");
                                }
                            }
                            // Populate Certification
                            //groupBoxCertification.Text = String.Format("Certification: {0}", structureId);
                            labelWorkConceptInCertification.Text = labelWorkConceptInPrecertification.Text;
                                //String.Format("Wc:({0}){1}     Cert Wc:({2}){3}", currentWorkConcept.WorkConceptCode, currentWorkConcept.WorkConceptDescription, currentWorkConcept.CertifiedWorkConceptCode, currentWorkConcept.CertifiedWorkConceptDescription);
                            //labelCertificationLastAction.Text = "None";

                            if (currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Precertified && currentWorkConcept.CertificationDateTime.Year > 1)
                            {
                                labelCertificationLastAction.Text = "Saved on " + currentWorkConcept.CertificationDateTime.ToString();
                            }
                            else if (currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Certified)
                            {
                                labelCertificationLastAction.Text = "Certified on " + currentWorkConcept.CertificationDateTime.ToString();
                            }
                            else if (currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                            {
                                labelCertificationLastAction.Text = "Rejected on " + currentWorkConcept.CertificationDateTime.ToString();
                            }

                            if (currentWorkConcept.WorkConceptCode.Equals("EV")
                                || currentWorkConcept.WorkConceptCode.Equals("PR"))
                            {
                                comboBoxNewWorkConcept.Items.Add(String.Format("({0}) {1}", "PR", "SELECT WORK CONCEPT..."));
                                comboBoxNewWorkConcept.Items.Add("----------");

                                foreach (WorkConcept pwc in primaryWorkConcepts)
                                {
                                    string workConceptDisplay = String.Format("({0}) {1}", pwc.WorkConceptCode, pwc.WorkConceptDescription);

                                    if (!pwc.WorkConceptCode.Equals("00") && !pwc.WorkConceptCode.Equals("01"))
                                    {
                                        comboBoxNewWorkConcept.Items.Add(workConceptDisplay);
                                    }
                                }

                                comboBoxNewWorkConcept.Items.Add("----------");
                                //comboBoxNewWorkConcept.Items.Add(String.Format("({0}) {1}", "EV", "EVALUATE FOR SECONDARY WORK CONCEPTS"));
                                //comboBoxNewWorkConcept.Items.Add("----------");

                                foreach (WorkConcept swc in secondaryWorkConcepts)
                                {
                                    string workConceptDisplay = String.Format("({0}) {1}", swc.WorkConceptCode, swc.WorkConceptDescription);
                                    comboBoxNewWorkConcept.Items.Add(workConceptDisplay);
                                }
                            }
                            else
                            {
                                comboBoxNewWorkConcept.Enabled = false;
                            }
                            
                            RenderPrimaryElementsGrid(structureId);

                            if (dgvcSecondaryWorkConcept.Items.Count == 0)
                            {
                                PopulateSecondaryWorkConcepts();
                            }

                            RenderSecondaryElementsGrid(structureId);
                            textBoxAdditionalPrimaryWorkConceptComments.Text = currentWorkConcept.CertificationPrimaryWorkTypeComments;
                            textBoxAdditionalSecondaryWorkConceptComments.Text = currentWorkConcept.CertificationSecondaryWorkTypeComments;
                            textBoxCertificationGeneralComments.Text = currentWorkConcept.CertificationAdditionalComments;
                            textBoxWorkConceptEstimatedConstructionCost.Text = currentWorkConcept.EstimatedConstructionCost != 0 ? currentWorkConcept.EstimatedConstructionCost.ToString() : "";
                            textBoxWorkConceptEstimatedDesignLevelOfEffort.Text = currentWorkConcept.EstimatedDesignLevelOfEffort != 0 ? currentWorkConcept.EstimatedDesignLevelOfEffort.ToString() : "";
                            radioButtonToBeDeterminedDesignResourcing.Checked = true;

                            switch (currentWorkConcept.DesignResourcing)
                            {
                                case "TBD":
                                    radioButtonToBeDeterminedDesignResourcing.Checked = true;
                                    break;
                                case "BOS":
                                    radioButtonInHouseDesignResourcing.Checked = true;
                                    break;
                                case "Consultant":
                                    radioButtonConsultantDesignResourcing.Checked = true;
                                    break;
                            }
                        }

                        break;
                }
            }
        }

        private bool LoadInspection(string inspectionFilePath)
        {
            try
            {
                axAcroPDF1.LoadFile(inspectionFilePath);
                axAcroPDF1.setShowToolbar(true);
                axAcroPDF1.setView("FitH");
                axAcroPDF1.setZoom(100);
                axAcroPDF1.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load inspection", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                axAcroPDF1.Visible = false;
            }

            return axAcroPDF1.Visible;
        }

        private void RenderPrimaryElementsGrid(string structureId)
        {
            dgvPrimaryElements.Rows.Clear();
            DataGridView dgv = dgvPrimaryElements;
            int rowCounter = 0;
           
            foreach (var ewc in project.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == currentWorkConcept.ProjectWorkConceptHistoryDbId 
                                                                                        && el.StructureId.Equals(currentWorkConcept.StructureId)
                                                                                        && el.WorkConceptLevel.ToUpper().Equals("PRIMARY")
                                                                                        && el.CertificationDateTime == currentWorkConcept.CertificationDateTime).OrderBy(el => el.ElementNumber))
            {
                dgv.Rows.Add();
                //int lastRow = dgv.Rows.GetLastRow(0);
                dgv.Rows[rowCounter].Cells["dgvcPrimaryElementNumber"].Value = ewc.ElementNumber;

                if (String.IsNullOrEmpty(ewc.ElementName))
                {
                    ewc.ElementName = warehouseDatabase.GetElementName(ewc.ElementNumber);
                }

                dgv.Rows[rowCounter].Cells["dgvcPrimaryElementName"].Value = ewc.ElementName;
                dgv.Rows[rowCounter].Cells["dgvcPrimaryWorkConcept"].Value = String.Format("({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                dgv.Rows[rowCounter].Cells["dgvcPrimaryComments"].Value = ewc.Comments;
                rowCounter++;
            }

            try
            {
                dgvPrimaryElements.CurrentCell.Selected = false;
            }
            catch { }
        }

        private void GenerateSecondaryWorkConceptsTable(string structureId)
        {
            var ewcs = project.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == currentWorkConcept.ProjectWorkConceptHistoryDbId
                                                                                        && el.StructureId.Equals(currentWorkConcept.StructureId)
                                                                                        && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                        && el.CertificationDateTime == currentWorkConcept.CertificationDateTime).OrderBy(el => el.ElementNumber).ToList();

            if (ewcs.Count > 0)
            {
                DialogResult dr = MessageBox.Show(String.Format("The secondary work concepts table already has elements. Continue with generating elements from the last inspection?"), "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                {
                    return;
                }
            }

            var lastInspection = warehouseDatabase.GetLastInspection(structureId, true);

            if (lastInspection != null)
            {
                int rowCounter = 0;
                DataGridView dgv = dgvSecondaryElements;
                int existingRowCounter = dgv.RowCount;
                rowCounter = dgv.RowCount - 1;

                foreach (var element in lastInspection.Elements)
                {
                    dgv.Rows.Add();
                    dgv.Rows[rowCounter].Cells["dgvcSecondaryElementNumber"].Value = element.ElementNumber;
                    dgv.Rows[rowCounter].Cells["dgvcSecondaryElementName"].Value = element.ElementName;
                    dgv.Rows[rowCounter].Cells["dgvcSecondaryWorkConcept"].Value = "";
                    dgv.Rows[rowCounter].Cells["dgvcSecondaryComments"].Value = "";
                    rowCounter++;
                }
            }
            else
            {
                MessageBox.Show(String.Format("Unable to retrieve the last inspection for {0}.", Utility.FormatStructureId(structureId)), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RenderSecondaryElementsGrid(string structureId)
        {
            dgvSecondaryElements.Rows.Clear();
            DataGridView dgv = dgvSecondaryElements;
            var ewcs = project.CertifiedElementWorkConceptCombinations.Where(el => el.ProjectWorkConceptHistoryDbId == currentWorkConcept.ProjectWorkConceptHistoryDbId
                                                                                        && el.StructureId.Equals(currentWorkConcept.StructureId)
                                                                                        && el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                        && el.CertificationDateTime == currentWorkConcept.CertificationDateTime).OrderBy(el => el.ElementNumber).ToList();
            int rowCounter = 0;

            foreach (var ewc in ewcs)
            {
                dgv.Rows.Add();
                dgv.Rows[rowCounter].Cells["dgvcSecondaryElementNumber"].Value = ewc.ElementNumber;

                if (String.IsNullOrEmpty(ewc.ElementName))
                {
                    ewc.ElementName = warehouseDatabase.GetElementName(ewc.ElementNumber);
                }

                dgv.Rows[rowCounter].Cells["dgvcSecondaryElementName"].Value = ewc.ElementName;
                if (!String.IsNullOrEmpty(ewc.WorkConceptCode))
                {
                    dgv.Rows[rowCounter].Cells["dgvcSecondaryWorkConcept"].Value = String.Format("({0}) {1}", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                }
                dgv.Rows[rowCounter].Cells["dgvcSecondaryComments"].Value = ewc.Comments;
                rowCounter++;
            }

            /*
            if (ewcs.Count() == 0)
            {
                var lastInspection = warehouseDatabase.GetLastInspection(structureId, true);
                rowCounter = 0;

                if (lastInspection != null && lastInspection.Elements != null)
                {
                    foreach (var element in lastInspection.Elements)
                    {
                        dgv.Rows.Add();
                        //int lastRow = dgv.Rows.GetLastRow(0);
                        dgv.Rows[rowCounter].Cells["dgvcSecondaryElementNumber"].Value = element.ElementNumber;
                        dgv.Rows[rowCounter].Cells["dgvcSecondaryElementName"].Value = element.ElementName;
                        dgv.Rows[rowCounter].Cells["dgvcSecondaryWorkConcept"].Value = "";
                        dgv.Rows[rowCounter].Cells["dgvcSecondaryComments"].Value = "";

                        rowCounter++;
                    }
                }
                else
                {
                    MessageBox.Show("Unable to retrieve the last inspection for " + structureId + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }*/

            try
            {
                dgvSecondaryElements.CurrentCell.Selected = false;
            }
            catch { }
        }

        private void buttonDeleteWorkConceptFromProject_Click(object sender, EventArgs e)
        {
            if (project.UserAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification)
            {
                DialogResult dr = MessageBox.Show(String.Format("BOS has precertified this project. Are you sure you want delete work concept(s)?"), "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                {
                    return;
                }
            }

            foreach (DataGridViewRow selectedRow in dataGridViewProjectWorkConcepts.SelectedRows)
            {
                //structureId = senderGrid.Rows[rowIndex].Cells["dgvcStructureId"].Value.ToString().Trim();
                int workConceptDbId = Convert.ToInt32(selectedRow.Cells["dgvcWorkConceptDbId"].Value);
                string structureId = selectedRow.Cells["dgvcStructureId"].Value.ToString().Trim();
                WorkConcept w = workConceptsAdded.Where(wc => wc.WorkConceptDbId == workConceptDbId && wc.StructureId.Equals(structureId)).First();
                w.ProjectDbId = 0;
                textBoxStructureProjectCost.Text = (Convert.ToInt32(textBoxStructureProjectCost.Text.Replace(",", "")) - w.Cost).ToString();
                workConceptsAdded.RemoveAll(wc => wc.WorkConceptDbId == workConceptDbId && wc.StructureId.Equals(structureId));
                dataGridViewProjectWorkConcepts.Rows.RemoveAt(selectedRow.Index);
                UpdateProjectStatus();

                /*
                if (workConceptsAdded.Where(wc => (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)).Count() == 0)
                {
                    checkBoxUnapproved.Checked = false;
                    checkBoxPrecertified.Checked = true;
                    checkBoxCertified.Checked = false;
                }
                else
                {
                    checkBoxUnapproved.Checked = true;
                    checkBoxPrecertified.Checked = false;
                    checkBoxCertified.Checked = false;
                }

                if (checkBoxUnapproved.Checked)
                {
                    groupBoxProjectStatus.BackColor = Color.Red;
                }
                else if (checkBoxPrecertified.Checked)
                {
                    groupBoxProjectStatus.BackColor = Color.Yellow;
                }
                else if (checkBoxCertified.Checked)
                {
                    groupBoxProjectStatus.BackColor = Color.Green;
                }*/
            }
        }

        private void dataGridViewProjectWorkConcepts_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            var dgv = (DataGridView)sender;
            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            if (columnIndex >= 0)
            {
                // || dgv.CurrentCell.ColumnIndex == 2
                if (dgv.Columns[columnIndex].Name.Equals("dgvcCertifiedWorkConcept"))
                {
                    int workConceptDbId = Convert.ToInt32(dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcWorkConceptDbId"].Value);
                    WorkConcept wc = workConceptsAdded.Where(w => w.WorkConceptDbId == workConceptDbId).First();
                    WorkConcept originalWc = null;
                    string originalWorkConceptDisplay = "";

                    if (project.ProjectDbId != 0)
                    {
                        try
                        {
                            originalWc = project.WorkConcepts.Where(w => w.WorkConceptDbId == workConceptDbId).First();
                            originalWorkConceptDisplay = String.Format("({0}) {1}", originalWc.CertifiedWorkConceptCode, originalWc.CertifiedWorkConceptDescription);
                        }
                        catch { }
                    }

                    if (originalWc != null && (originalWc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified 
                                                || originalWc.Status == StructuresProgramType.WorkConceptStatus.Certified))
                    {
                        if (dgv.CurrentCell.EditedFormattedValue.ToString().Equals(originalWorkConceptDisplay))
                        {
                            wc.Status = originalWc.Status;
                            wc.CertifiedWorkConceptCode = originalWc.CertifiedWorkConceptCode;
                            wc.CertifiedWorkConceptDescription = originalWc.CertifiedWorkConceptDescription;
                            wc.PrecertificationDecision = originalWc.PrecertificationDecision;
                            wc.PrecertificationDecisionDateTime = originalWc.PrecertificationDecisionDateTime;
                            wc.PrecertificationDecisionInternalComments = originalWc.PrecertificationDecisionInternalComments;
                            wc.PrecertificationDecisionReasonCategory = originalWc.PrecertificationDecisionReasonCategory;
                            wc.PrecertificationDecisionReasonExplanation = originalWc.PrecertificationDecisionReasonExplanation;
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value = originalWc.Status.ToString();
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcChangeReason"].Value = originalWc.ChangeJustifications;
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcPrecertificationDecision"].Value = originalWc.PrecertificationDecision.ToString();

                            DetermineProjectStatus(project.FiscalYear);
                        }
                        else
                        {
                            wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            wc.CertifiedWorkConceptCode = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2);
                            wc.CertifiedWorkConceptDescription = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(5, dgv.CurrentCell.EditedFormattedValue.ToString().Length - 5).Trim();
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value = wc.Status.ToString();
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcChangeReason"].Value = "";
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcPrecertificationDecision"].Value = "";
                            checkBoxUnapproved.Checked = true;
                            groupBoxProjectStatus.BackColor = Color.Red;
                            linkLabelBosCd.Visible = false;
                            checkBoxPrecertified.Checked = false;
                            checkBoxCertified.Checked = false;

                            //if (project.ProjectDbId != 0)
                            {
                                wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.None;
                                wc.PrecertificationDecisionDateTime = new DateTime(1, 1, 1, 0, 0, 0);
                                wc.PrecertificationDecisionInternalComments = "";
                                wc.PrecertificationDecisionReasonCategory = "";
                                wc.PrecertificationDecisionReasonExplanation = "";
                            }
                        }
                    }
                    else if (secondaryWorkConcepts.Where(swc => swc.WorkConceptCode.Equals(dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2))).Count() > 0
                        || dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2).Equals("EV")
                        || wc.StructureId.StartsWith("C") || wc.StructureId.StartsWith("R")
                        || wc.StructureId.StartsWith("N") || wc.StructureId.StartsWith("M")
                        || wc.StructureId.StartsWith("S"))
                    {
                        //wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                        wc.CertifiedWorkConceptCode = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2);
                        wc.CertifiedWorkConceptDescription = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(5, dgv.CurrentCell.EditedFormattedValue.ToString().Length - 5).Trim();
                        wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value = wc.Status.ToString();
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcChangeReason"].Value = "";
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcPrecertificationDecision"].Value = "";
                        wc.ChangeJustifications = "";
                        wc.ChangeJustificationNotes = "";

                        if (workConceptsAdded.All(w => w.Status == StructuresProgramType.WorkConceptStatus.Certified || w.Status == StructuresProgramType.WorkConceptStatus.Quasicertified))
                        {
                            checkBoxUnapproved.Checked = false;
                            checkBoxPrecertified.Checked = false;
                            groupBoxProjectStatus.BackColor = Color.Green;
                            checkBoxCertified.Checked = true;
                            linkLabelBosCd.Visible = true;
                        }
                        else if (workConceptsAdded.Where(w => w.Status == StructuresProgramType.WorkConceptStatus.Unapproved).Count() == 0)
                        {
                            checkBoxUnapproved.Checked = false;
                            checkBoxPrecertified.Checked = true;
                            groupBoxProjectStatus.BackColor = Color.Yellow;
                            checkBoxCertified.Checked = false;
                            linkLabelBosCd.Visible = false;
                        }
                        else
                        {
                            checkBoxUnapproved.Checked = true;
                            checkBoxPrecertified.Checked = false;
                            groupBoxProjectStatus.BackColor = Color.Red;
                            checkBoxCertified.Checked = false;
                            linkLabelBosCd.Visible = false;
                        }

                        //comboBoxPrecertificationReasonCategory.Items.Add("(10) Secondary Work Concept");
                        //comboBoxPrecertificationReasonCategory.Items.Add("(15) Evaluate for Secondary");
                        //comboBoxPrecertificationReasonCategory.Items.Add("(20) Ancillary Structure");

                        //Redo...
                        {
                            wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.Accept;
                            wc.PrecertificationDecisionDateTime = DateTime.Now;
                            wc.PrecertificationDecisionInternalComments = "";

                            if (wc.StructureId.StartsWith("C") || wc.StructureId.StartsWith("R")
                                || wc.StructureId.StartsWith("N") || wc.StructureId.StartsWith("M")
                                || wc.StructureId.StartsWith("S"))
                            {
                                wc.PrecertificationDecisionReasonCategory = "(20) Ancillary Structure";
                                wc.PrecertificationDecisionReasonExplanation = "This is an Ancillary Structure (non-bridge), and the work concept has been automatically Pre-certified. BOS did not review the work concepts to verify they meet BOS asset management philosophy. Regional Bridge Maintenance staff are responsible for recommending and incorporating Ancillary Structure work.";
                            }
                            else if (dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2).Equals("EV"))
                            {
                                wc.PrecertificationDecisionReasonCategory = "(15) Evaluate for Secondary";
                                wc.PrecertificationDecisionReasonExplanation = "This is a request from the region for BOS to evaluate the structure for secondary work. This work concept has been automatically Pre-certified. If this project has an earliest PSE after 7/1/2025, BOS will identify secondary work concepts when fully certifying the project. If this project has an earliest PSE before 7/1/2025, the region should work with regional Bridge Maintenance staff to re-submit with the most applicable 'secondary' work concept selected.";
                            }
                            else
                            {
                                wc.PrecertificationDecisionReasonCategory = "(10) Secondary Work Concept";
                                wc.PrecertificationDecisionReasonExplanation = "This is a 'secondary' work concept, and the work concept has been automatically Pre-certified. BOS did not review the work concepts to verify they meet BOS asset management philosophy. If this project has an earliest PSE after 7/1/2025, BOS will identify additional secondary work concepts when fully certifying the project. If this project has an earliest PSE before 7/1/2025, the region should work with regional Bridge Maintenance staff to enter notes concerning additional secondary work to be completed on this project.";
                            }
                        }
                    }
                    else if (project.ProjectDbId != 0 && originalWc != null && dgv.CurrentCell.EditedFormattedValue.ToString().Equals(originalWorkConceptDisplay))
                    {
                        wc.Status = originalWc.Status;
                        wc.CertifiedWorkConceptCode = originalWc.CertifiedWorkConceptCode;
                        wc.CertifiedWorkConceptDescription = originalWc.CertifiedWorkConceptDescription;
                        wc.PrecertificationDecision = originalWc.PrecertificationDecision;
                        wc.PrecertificationDecisionDateTime = originalWc.PrecertificationDecisionDateTime;
                        wc.PrecertificationDecisionInternalComments = originalWc.PrecertificationDecisionInternalComments;
                        wc.PrecertificationDecisionReasonCategory = originalWc.PrecertificationDecisionReasonCategory;
                        wc.PrecertificationDecisionReasonExplanation = originalWc.PrecertificationDecisionReasonExplanation;
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value = originalWc.Status.ToString();
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcChangeReason"].Value = originalWc.ChangeJustifications;
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcPrecertificationDecision"].Value = originalWc.PrecertificationDecision.ToString();

                        DetermineProjectStatus(project.FiscalYear);
                    }
                    
                    // Eligible and Certified work concepts don't match -> Scope Change
                    else if (!dgv.CurrentCell.EditedFormattedValue.ToString().Equals(dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcWorkConcept"].Value.ToString()))
                    {
                        wc.CertifiedWorkConceptCode = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2);
                        wc.CertifiedWorkConceptDescription = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(5, dgv.CurrentCell.EditedFormattedValue.ToString().Length - 5).Trim();
                        wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value = wc.Status.ToString();
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcChangeReason"].Value = "";
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcPrecertificationDecision"].Value = "";
                        checkBoxUnapproved.Checked = true;
                        groupBoxProjectStatus.BackColor = Color.Red;
                        linkLabelBosCd.Visible = false;
                        checkBoxPrecertified.Checked = false;
                        checkBoxCertified.Checked = false;

                        //if (project.ProjectDbId != 0)
                        {
                            wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.None;
                            wc.PrecertificationDecisionDateTime = new DateTime(1, 1, 1, 0, 0, 0);
                            wc.PrecertificationDecisionInternalComments = "";
                            wc.PrecertificationDecisionReasonCategory = "";
                            wc.PrecertificationDecisionReasonExplanation = "";
                        }
                    }
                    else // Eligible and Certified work concepts match -> NO Scope Change
                    {
                        wc.CertifiedWorkConceptCode = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(1, 2);
                        wc.CertifiedWorkConceptDescription = dgv.CurrentCell.EditedFormattedValue.ToString().Substring(5, dgv.CurrentCell.EditedFormattedValue.ToString().Length - 5).Trim();

                        if (wc.CertifiedWorkConceptCode.Equals("EV"))
                        {
                            wc.Status = StructuresProgramType.WorkConceptStatus.Evaluate;
                            //dgv.Rows[dgv.CurrentCell.RowIndex].Cells[3].Value = wc.Status.ToString();
                        }
                        else if (IsWorkConceptPrecertified(wc, Convert.ToInt32(textBoxStructureProjectFiscalYear.Text)))
                        {
                            wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                        }
                        else
                        {
                            wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                        }

                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Value = wc.Status.ToString();
                        dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcPrecertificationDecision"].Value = "";

                        if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                        {
                            checkBoxUnapproved.Checked = true;
                            checkBoxPrecertified.Checked = false;
                            checkBoxCertified.Checked = false;
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                            groupBoxProjectStatus.BackColor = Color.Red;
                            linkLabelBosCd.Visible = false;
                        }
                        else // Precertified or Certified
                        {
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcChangeReason"].Value = "";
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcNotes"].Value = "";
                            wc.ChangeJustifications = "";
                            wc.ChangeJustificationNotes = "";
                            dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;

                            if (workConceptsAdded.All(w => w.Status == StructuresProgramType.WorkConceptStatus.Certified || w.Status == StructuresProgramType.WorkConceptStatus.Quasicertified))
                            {
                                checkBoxUnapproved.Checked = false;
                                checkBoxPrecertified.Checked = false;
                                groupBoxProjectStatus.BackColor = Color.Green;
                                checkBoxCertified.Checked = true;
                                linkLabelBosCd.Visible = true;
                            }
                            else if (workConceptsAdded.Where(w => w.Status == StructuresProgramType.WorkConceptStatus.Unapproved).Count() == 0)
                            {
                                checkBoxUnapproved.Checked = false;
                                checkBoxPrecertified.Checked = true;
                                groupBoxProjectStatus.BackColor = Color.Yellow;
                                checkBoxCertified.Checked = false;
                                linkLabelBosCd.Visible = false;
                            }
                        }

                        //if (project.ProjectDbId != 0)
                        {
                            wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.None;
                            wc.PrecertificationDecisionDateTime = new DateTime(1, 1, 1, 0, 0, 0);
                            wc.PrecertificationDecisionInternalComments = "";
                            wc.PrecertificationDecisionReasonCategory = "";
                            wc.PrecertificationDecisionReasonExplanation = "";
                        }
                    }
                }
                else if (dgv.Columns[columnIndex].Name.Equals("dgvcChangeReason"))
                {
                    //if (!dgv.CurrentCell.EditedFormattedValue.ToString().Equals("")) //|| dgv.CurrentCell.ColumnIndex == 5
                    {
                        int workConceptDbId = Convert.ToInt32(dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcWorkConceptDbId"].Value);
                        WorkConcept wc = workConceptsAdded.Where(w => w.WorkConceptDbId == workConceptDbId).First();
                        wc.ChangeJustifications = dgv.CurrentCell.EditedFormattedValue.ToString();
                    }
                }
            }
        }

        private void dataGridViewProjectWorkConcepts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //this.isDirty = true;
        }

        List<WorkConcept> GetUnapprovedWorkConceptsWithoutJustification()
        {
            return controller.GetUnapprovedWorkConceptsWithoutJustification(workConceptsAdded);
        }

        private async void SaveProject(object sender, EventArgs e)
        {
            if (workConceptsAdded.Count() == 0)
            {
                MessageBox.Show("Project has no work concepts.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var button = (Button)sender;

            if (button.Text.Equals("Save"))
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.SavedProject;
            }
            else if (button.Text.Equals("Submit"))
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification;
            }
            else if (button.Text.Equals("Submit for Certification"))
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification;
            }

            if (project.UserAction == StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification)
            {
                DialogResult dr = MessageBox.Show(String.Format("BOS has precertified this project. Saving a precertified project rescinds its precertified status. Continue with save?\r\nNOTE: To update project info or FIIPS association without affecting status, use the other Save buttons."), "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dr == DialogResult.No)
                {
                    return;
                }
            }

            int projectFiscalYear = 0;
            bool validProjectFiscalYear = true;
            bool validAdvanceableProjectFiscalYear = true;
            
            if (textBoxStructureProjectFiscalYear.Text.Trim().Length > 0)
            {
                try
                {
                    projectFiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());

                    if (projectFiscalYear < currentFiscalYear)
                    {
                        validProjectFiscalYear = false;
                        MessageBox.Show("Enter project fiscal year >= " + (currentFiscalYear).ToString(), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length > 0)
                    {
                        validAdvanceableProjectFiscalYear = ValidateAdvanceableProjectFiscalYear();

                        if (!validAdvanceableProjectFiscalYear)
                        {
                            MessageBox.Show(String.Format("Enter an advanceable year <= {0} and >= {1}", projectFiscalYear, currentFiscalYear), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch
                {
                    validProjectFiscalYear = false;
                    MessageBox.Show("Enter project fiscal year >= " + (currentFiscalYear).ToString(), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (validProjectFiscalYear && validAdvanceableProjectFiscalYear && workConceptsAdded.Count() > 0)
            {
                ToggleProjectEditButtons(false);
                int unapprovedWorkConceptsWithoutJustificationCount = GetUnapprovedWorkConceptsWithoutJustification().Count();

                if (unapprovedWorkConceptsWithoutJustificationCount > 0)
                {
                    string message = String.Format("Please provide justification and notes for {0} 'Unapproved' work concept(s).", unapprovedWorkConceptsWithoutJustificationCount);
                    MessageBox.Show(message);
                    ToggleProjectEditButtons(true);
                    return;
                }

                project.CurrentFiscalYear = currentFiscalYear;
                project.FiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());
                project.ProjectYear = currentProjectYear + (project.FiscalYear - currentFiscalYear);
                project.StructuresConcept = comboBoxStructureProjectImprovementConcept.SelectedItem.ToString();
               

                if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length > 0)
                {
                    project.AdvanceableFiscalYear = Convert.ToInt32(textBoxStructureProjectAdvanceableFiscalYear.Text.Trim());
                }
                else
                {
                    project.AdvanceableFiscalYear = 0;
                }
                
                try
                {
                    project.StructuresCost = Convert.ToInt32(textBoxStructureProjectCost.Text.Trim().Replace(",",""));
                    textBoxStructureProjectCost.Text = String.Format("{0:n0}", project.StructuresCost);
                }
                catch { }

                /*
                try
                {
                    project.AcceptablePseDateStart = Convert.ToDateTime(textBoxAcceptablePseDateStart.Text.Trim());
                }
                catch
                {
                    if (project.AdvanceableFiscalYear != 0)
                    {
                        project.AcceptablePseDateStart = database.CalculateAcceptablePseDateStart(project.AdvanceableFiscalYear);
                    }
                    else
                    {
                        project.AcceptablePseDateStart = database.CalculateAcceptablePseDateStart(project.FiscalYear);
                    }
                }

                try
                {
                    project.AcceptablePseDateEnd = Convert.ToDateTime(textBoxAcceptablePseDateEnd.Text.Trim());
                }
                catch
                {
                    project.AcceptablePseDateEnd = database.CalculateAcceptablePseDateEnd(project.FiscalYear);
                }*/
                if (button.Text.Equals("Submit"))
                {
                    if (String.IsNullOrEmpty(textBoxAcceptablePseDateStart.Text.Trim()))
                    {
                        if (project.AdvanceableFiscalYear != 0)
                        {
                            textBoxAcceptablePseDateStart.Text = dataServ.CalculateAcceptablePseDateStart(project.AdvanceableFiscalYear).ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            textBoxAcceptablePseDateStart.Text = dataServ.CalculateAcceptablePseDateStart(project.FiscalYear).ToString("MM/dd/yyyy");
                        }

                        project.AcceptablePseDateStart = Convert.ToDateTime(textBoxAcceptablePseDateStart.Text.Trim());
                    }

                    if (String.IsNullOrEmpty(textBoxAcceptablePseDateEnd.Text.Trim()))
                    {
                        textBoxAcceptablePseDateEnd.Text = dataServ.CalculateAcceptablePseDateEnd(project.FiscalYear).ToString("MM/dd/yyyy");
                        project.AcceptablePseDateEnd = Convert.ToDateTime(textBoxAcceptablePseDateEnd.Text.Trim());
                    }
                }

                if (checkBoxUnapproved.Checked)
                {
                    project.Status = StructuresProgramType.ProjectStatus.Unapproved;
                }
                else if (checkBoxPrecertified.Checked)
                {
                    project.Status = StructuresProgramType.ProjectStatus.Precertified;

                    
                }
                else if (checkBoxCertified.Checked)
                {
                    project.Status = StructuresProgramType.ProjectStatus.Certified;
                }

                project.Description = textBoxStructureProjectDescription.Text.Trim();
                project.Notes = textBoxStructureProjectNotes.Text.Trim();
                project.NotificationRecipients = textBoxNotificationRecipients.Text.Trim();
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.FosProjectId = textBoxStructureProjectFosProjectId.Text.Trim();
                project.InPrecertification = false;
                project.PrecertifyDate = new DateTime(1, 1, 1, 0, 0, 0);
                string action = "";
                project.UserActionDateTime = DateTime.Now;
                project.FosProjectId = textBoxStructureProjectFosProjectId.Text.Trim();

                try
                {
                    project.FiipsImprovementConcept = comboBoxStructureProjectFiipsImprovementConcept.SelectedText;
                }
                catch { }

                try
                {
                    project.FiipsDescription = textBoxStructureProjectFiipsDescription.Text.Trim();
                }
                catch { }

                project.WorkConcepts = workConceptsAdded;
                int numStructures = 0;
                string currentStructureId = "";

                foreach (var wc in project.WorkConcepts.OrderBy(el => el.StructureId))
                {
                    if (!wc.StructureId.Equals(currentStructureId))
                    {
                        numStructures++;
                    }

                    wc.ProjectDbId = project.ProjectDbId;
                    wc.WorkConceptTimeStamp = project.UserActionDateTime;
                    wc.StructureProjectFiscalYear = project.FiscalYear;
                    wc.StructuresConcept = project.StructuresConcept;
                    wc.CurrentFiscalYear = currentFiscalYear;

                    if (wc.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.None)
                    {
                        //wc.PrecertificationDecision = StructuresProgramType.PrecertificatioReviewDecision.None;
                        wc.PrecertificationDecisionDateTime = new DateTime(1, 1, 1, 0, 0, 0);
                        wc.PrecertificationDecisionReasonCategory = "";
                        wc.PrecertificationDecisionReasonExplanation = "";
                        wc.PrecertificationDecisionInternalComments = "";
                    }

                    if (wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate)
                    {
                        wc.FiscalYear = project.FiscalYear;
                        wc.StructureProjectFiscalYear = project.FiscalYear;
                    }

                    currentStructureId = wc.StructureId;
                }

                project.NumberOfStructures = numStructures;

                try
                {
                    project.Region = project.WorkConcepts[0].Region;
                }
                catch
                {
                    MessageBox.Show("Unable to save project.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                project.RelatedProjects = "";
                
                if (project.ProjectDbId == 0) // New project
                {
                    // Insert new project
                    action = "Add";
                    dataServ.InsertProject(project);
                    // Update in-memory structureProjects collection
                    structureProjects.Add(project);
                }
                else // Existing project
                {
                    action = "Update";
                    var history = dataServ.GetProjectUserActionHistory(project.ProjectDbId).ToList();
                    bool skipPrecertification = false;

                    try
                    {
                        if (history[0] == StructuresProgramType.ProjectUserAction.BosRejectedCertification
                            && history[1] == StructuresProgramType.ProjectUserAction.BosPrecertified)
                        {
                            skipPrecertification = true;
                        }
                    }
                    catch { }

                    dataServ.InsertProject(project);
                }

                foreach (WorkConcept wc in project.WorkConcepts)
                {
                    wc.ProjectDbId = project.ProjectDbId;
                }

                // Box
                if (dataServ.EnableBox())
                {
                    try
                    {
                        await fileManager.UpdateBoxCertificationDirectory(project);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Box error: " + ex.Message, "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                formMapping.EditProject(project, action);
                project.UserDbIds.Add(userAccount.UserDbId);
                //Email.ComposeMessage(project, userAccount, database.GetEmailAddresses(project.UserDbIds), database.GetApplicationMode(), Path.Combine(database.GetMyDirectory(), "bos.jpg"), database);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                UpdateProjectInfo(project);
                ToggleProjectEditButtons(true);
                /*
                if (button.Text.Equals("Save"))
                {
                    UpdateProjectInfo(project);
                    ToggleProjectEditButtons(true);
                }
                else if (button.Text.Equals("Submit"))
                {
                    this.Close();
                }*/
            }
        }

        private void ClearForm()
        {
            textBoxStructureProjectCost.Text = "";
            textBoxStructureProjectFiscalYear.Text = "";
            textBoxStructureProjectDescription.Text = "";
            textBoxStructureProjectNotes.Text = "";
            textBoxNotificationRecipients.Text = "";
            textBoxStructureProjectDbId.Text = "0";
            checkBoxPrecertified.Checked = false;
            comboBoxStructureProjectImprovementConcept.SelectedItem = null;
            textBoxStructureProjectFosProjectId.Text = "XXXXXXXX";
            comboBoxStructureProjectFiipsImprovementConcept.SelectedItem = null;
            textBoxStructureProjectFiipsDescription.Text = "";
            workConceptsAdded = new List<WorkConcept>();
            dataGridViewProjectWorkConcepts.Rows.Clear();
        }

        private void ToggleProjectEditButtons(bool enabled)
        {
            buttonSaveProjectDescription.Enabled = enabled;
            buttonSaveProject.Enabled = enabled;
            buttonSubmitProjectForPrecertification.Enabled = enabled;
            buttonDeleteProject.Enabled = enabled;
            buttonDeleteWorkConceptFromProject.Enabled = enabled;
            pictureBoxProjectEdit.Visible = !enabled;
        }

        private void buttonSaveProject_Click(object sender, EventArgs e)
        {
            savedTransaction = true;
            SaveProject(sender, e);
        }

        private bool ValidateAdvanceableProjectFiscalYear()
        {
            int advanceableProjectFiscalYear = 0;
            bool validAdvanceableProjectFiscalYear = true;

            if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length > 0)
            {
                try
                {
                    advanceableProjectFiscalYear = Convert.ToInt32(textBoxStructureProjectAdvanceableFiscalYear.Text.Trim());
                    int projectFiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());

                    if (advanceableProjectFiscalYear > projectFiscalYear || advanceableProjectFiscalYear < currentFiscalYear)
                    {
                        validAdvanceableProjectFiscalYear = false;
                    }
                }
                catch
                {
                    validAdvanceableProjectFiscalYear = false;
                }
            }

            return validAdvanceableProjectFiscalYear;
        }

        private bool ValidateProjectFiscalYear()
        {
            int projectFiscalYear = 0;
            bool validProjectFiscalYear = true;

            if (textBoxStructureProjectFiscalYear.Text.Trim().Length > 0)
            {
                try
                {
                    projectFiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());

                    if (projectFiscalYear < currentFiscalYear)
                    {
                        validProjectFiscalYear = false;
                    }
                }
                catch
                {
                    validProjectFiscalYear = false;
                }
            }
            else
            {
                validProjectFiscalYear = false;
            }

            return validProjectFiscalYear;
        }

        private void textBoxStructureProjectFiscalYear_Validating(object sender, CancelEventArgs e)
        {
            if (project.Status != StructuresProgramType.ProjectStatus.Fiips && !textBoxStructureProjectFiscalYear.ReadOnly)
            {
                try
                {
                    if (Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim()) == project.FiscalYear)
                    {
                        return;
                    }
                }
                catch { }

                bool validProjectFiscalYear = ValidateProjectFiscalYear();

                if (!validProjectFiscalYear)
                {
                    MessageBox.Show("Enter project fiscal year >= " + (currentFiscalYear).ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
                else
                {
                    changesMade = true;
                    int projectFiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());
                    DetermineProjectStatus(projectFiscalYear);
                    int advanceableFy = 0;

                    try
                    {
                        advanceableFy = Convert.ToInt32(textBoxStructureProjectAdvanceableFiscalYear.Text.Trim());
                        DetermineProjectStatus(advanceableFy, true);
                    }
                    catch { }

                    /*
                    if (String.IsNullOrEmpty(textBoxAcceptablePseDateStart.Text.Trim()))
                    {
                        if (advanceableFy != 0)
                        {
                            textBoxAcceptablePseDateStart.Text = database.CalculateAcceptablePseDateStart(advanceableFy).ToString("MM/dd/yyyy");
                        }
                        else
                        {
                            textBoxAcceptablePseDateStart.Text = database.CalculateAcceptablePseDateStart(projectFiscalYear).ToString("MM/dd/yyyy");
                        }
                    }

                    if (String.IsNullOrEmpty(textBoxAcceptablePseDateEnd.Text.Trim()))
                    {
                        textBoxAcceptablePseDateEnd.Text = database.CalculateAcceptablePseDateEnd(projectFiscalYear).ToString("MM/dd/yyyy");
                    }*/
                }
            }
        }

        private void DetermineProjectStatus(int projectFiscalYear, bool isAdvanceable = false)
        {
            var workConceptsToEvaluate = workConceptsAdded;

            if (isAdvanceable)
            {
                workConceptsToEvaluate = workConceptsAdded.Where(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Precertified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified
                                                                    || wc.Status == StructuresProgramType.WorkConceptStatus.Certified).ToList();
            }

            foreach (WorkConcept wc in workConceptsToEvaluate)
            {
                WorkConcept originalWc = null;

                try
                {
                    originalWc = project.WorkConcepts.Where(w => w.WorkConceptDbId == wc.WorkConceptDbId).First();
                }
                catch { }

                /*
                if (originalWc != null && originalWc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                {
                    wc.Status = originalWc.Status;
                }*/
                if (originalWc != null && (originalWc.Status == StructuresProgramType.WorkConceptStatus.Certified || originalWc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified))
                {
                    if (originalWc.FromProposedList || !originalWc.WorkConceptCode.Equals(originalWc.CertifiedWorkConceptCode))
                    {
                        if (secondaryWorkConcepts.Where(swc => swc.WorkConceptCode.Equals(wc.CertifiedWorkConceptCode)).Count() > 0
                        || wc.CertifiedWorkConceptCode.Equals("EV")
                        || wc.StructureId.StartsWith("C") || wc.StructureId.StartsWith("R")
                        || wc.StructureId.StartsWith("N") || wc.StructureId.StartsWith("M")
                        || wc.StructureId.StartsWith("S"))
                        {
                            if (project.FiscalYear == projectFiscalYear)
                            {
                                wc.Status = originalWc.Status;
                            }
                            else
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                            }
                        }
                        else if (isAdvanceable)
                        {
                            if (project.AdvanceableFiscalYear != projectFiscalYear && project.FiscalYear != projectFiscalYear)
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else
                            {
                                wc.Status = originalWc.Status;
                            }
                        }
                        else
                        {
                            if (project.FiscalYear != projectFiscalYear)
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else
                            {
                                wc.Status = originalWc.Status;
                            }
                        }
                    }
                    else if (wc.CertifiedWorkConceptCode.Equals(originalWc.CertifiedWorkConceptCode))
                    {
                        if (isAdvanceable)
                        {
                            /*
                            if (project.AdvanceableFiscalYear != projectFiscalYear)
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else if (!IsWorkConceptPrecertified(wc, projectFiscalYear) && originalWc.Status != StructuresProgramType.WorkConceptStatus.Certified
                                        && originalWc.Status != StructuresProgramType.WorkConceptStatus.Quasicertified)
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else
                            {
                                wc.Status = originalWc.Status;
                            }*/
                            if (project.AdvanceableFiscalYear == projectFiscalYear)
                            {
                                wc.Status = originalWc.Status;
                            }
                            else
                            {
                                if (IsWorkConceptPrecertified(wc, projectFiscalYear))
                                {
                                    wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                                }
                                else
                                {
                                    wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                                }
                            }
                        }
                        else
                        {
                            /*
                            if (project.FiscalYear != project.FiscalYear)
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else if (!IsWorkConceptPrecertified(wc, projectFiscalYear) && originalWc.Status != StructuresProgramType.WorkConceptStatus.Certified
                                        && originalWc.Status != StructuresProgramType.WorkConceptStatus.Quasicertified)
                            {
                                wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                            }
                            else
                            {
                                wc.Status = originalWc.Status;
                            }*/
                            if (project.FiscalYear == projectFiscalYear)
                            {
                                wc.Status = originalWc.Status;
                            }
                            else
                            {
                                if (IsWorkConceptPrecertified(wc, projectFiscalYear))
                                {
                                    wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                                }
                                else
                                {
                                    wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                                }
                            }
                        }
                    }
                    else
                    {
                        wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                    }
                    /*
                    if ((wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                            )
                    {
                        if (!IsWorkConceptPrecertified(wc, projectFiscalYear))
                        {
                            wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                        }
                        else
                        {
                            wc.Status = originalWc.Status;
                        }
                    }*/
                }
                else if (secondaryWorkConcepts.Where(swc => swc.WorkConceptCode.Equals(wc.CertifiedWorkConceptCode)).Count() > 0
                        || wc.CertifiedWorkConceptCode.Equals("EV")
                        || wc.StructureId.StartsWith("C") || wc.StructureId.StartsWith("R")
                        || wc.StructureId.StartsWith("N") || wc.StructureId.StartsWith("M")
                        || wc.StructureId.StartsWith("S"))
                {
                    continue;
                }
                /*
                else if (wc.FromProposedList)
                {
                    if (wc.CertifiedWorkConceptCode.Equals("EV"))
                    {
                        wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                    }
                    else
                    {
                        wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                    }

                    try
                    {
                        var origWc = project.WorkConcepts.Where(w => w.StructureId.Equals(wc.StructureId) && w.WorkConceptDbId == wc.WorkConceptDbId).First();
                        if (project.FiscalYear == projectFiscalYear)
                        {
                            //wc.Status = origWc.Status;
                        }
                    }
                    catch { }
                }*/
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate || wc.CertifiedWorkConceptCode.Equals("EV"))
                {
                    wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                }
                else if (wc.WorkConceptCode.Equals(wc.CertifiedWorkConceptCode))
                {
                    if (IsWorkConceptPrecertified(wc, projectFiscalYear))
                    {
                        wc.Status = StructuresProgramType.WorkConceptStatus.Precertified;
                    }
                    else
                    {
                        wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
                    }

                    try
                    {
                        var origWc = project.WorkConcepts.Where(w => w.StructureId.Equals(wc.StructureId) && w.WorkConceptDbId == wc.WorkConceptDbId).First();
                        if (project.FiscalYear == projectFiscalYear)
                        {
                            //wc.Status = origWc.Status;
                        }
                    }
                    catch { }
                }
                else if (!wc.WorkConceptCode.Equals(wc.CertifiedWorkConceptCode))
                {
                    wc.Status = StructuresProgramType.WorkConceptStatus.Unapproved;

                    try
                    {
                        var origWc = project.WorkConcepts.Where(w => w.StructureId.Equals(wc.StructureId) && w.WorkConceptDbId == wc.WorkConceptDbId).First();
                        if (project.FiscalYear == projectFiscalYear)
                        {
                            //wc.Status = origWc.Status;
                        }
                    }
                    catch { }
                }


                //UpdateProjectStatus();

                foreach (DataGridViewRow row in dataGridViewProjectWorkConcepts.Rows)
                {
                    if (Convert.ToInt32(row.Cells["dgvcWorkConceptDbId"].Value) == wc.WorkConceptDbId)
                    {
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Value = wc.Status.ToString();

                        if (project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                        {
                            if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                            {
                                row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                            }
                        }
                        else if (wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Transitionally Certified";
                        }


                        if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                        }
                        else if (wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified || wc.Status == StructuresProgramType.WorkConceptStatus.Certified)
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                        }
                        else
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;
                        }
                        break;
                    }
                }
            }

            UpdateProjectStatus();

            /*
            if (workConceptsAdded.Where(w => ((w.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                                                || (w.Status == StructuresProgramType.WorkConceptStatus.Evaluate))).Count() == 0)
            {
                checkBoxUnapproved.Checked = false;
                checkBoxPrecertified.Checked = true;
                checkBoxCertified.Checked = false;
            }
            else
            {
                checkBoxUnapproved.Checked = true;
                checkBoxPrecertified.Checked = false;
                checkBoxCertified.Checked = false;
            }*/
        }


        private void buttonSubmitProject_Click(object sender, EventArgs e)
        {
            savedTransaction = true;
            SaveProject(sender, e);
        }

        private void dataGridViewProjectWorkConcepts_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = (DataGridView)sender;
            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            if (rowIndex >= 0 && columnIndex >= 0)
            {
                if (dgv.Columns[columnIndex].Name.Equals("dgvcNotes"))
                {
                    int workConceptDbId = Convert.ToInt32(dgv.Rows[dgv.CurrentCell.RowIndex].Cells["dgvcWorkConceptDbId"].Value);
                    WorkConcept wc = workConceptsAdded.Where(w => w.WorkConceptDbId == workConceptDbId).First();
                    wc.ChangeJustificationNotes = dgv.CurrentCell.EditedFormattedValue.ToString();
                }
            }
        }

        private void checkBoxRequestAdvancedCertification_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                if (ValidateProjectFiscalYear())
                {
                    //dateTimePickerAdvancedCertification.Enabled = true;
                    SetAdvancedCertificationDates();
                }
            }
            else
            {
                //dateTimePickerAdvancedCertification.Enabled = false;
            }
        }

        private void SetAdvancedCertificationDates()
        {
            int projectFiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());
            DateTime latestDate = DateTime.Now.AddYears(2);
            DateTime earliestDate = DateTime.Now.AddMonths(6);
            //dateTimePickerAdvancedCertification.MinDate = earliestDate.Date;
            //dateTimePickerAdvancedCertification.MaxDate = latestDate.Date;
        }

        private void buttonSubmitProjectForCertification_Click(object sender, EventArgs e)
        {
            SaveProject(sender, e);
        }

        private void buttonCompareProjects_Click(object sender, EventArgs e)
        {
            MessageBox.Show("To be implemented. Will generate a comparison report between the linked Structures Project and Fiips Project.");
        }

        private void UpdateAssociatedConstructionId(string constructionId)
        {
            Project fiipsProject = null;
            if (fiipsProjects.Where(s => s.FosProjectId.Equals(constructionId)).Count() > 0)
            {
                fiipsProject = fiipsProjects.Where(s => s.FosProjectId.Equals(constructionId)).First();
            }
            else
            {
                //Dw.FiipsProject tempFiipsProject = warehouseDatabase.GetFiipsProject(project.FosProjectId);
                Dw.FiipsProject tempFiipsProject = warehouseDatabase.GetFiipsProject(constructionId);

                if (tempFiipsProject != null)
                {
                    fiipsProject = new Project();
                    fiipsProject.Status = StructuresProgramType.ProjectStatus.Fiips;
                    fiipsProject.FosProjectId = tempFiipsProject.ConstructionId;
                    fiipsProject.DesignId = tempFiipsProject.DesignId;
                    fiipsProject.LifecycleStageCode = tempFiipsProject.LifecycleStageCode;
                    fiipsProject.FiipsImprovementConcept = tempFiipsProject.PlanningProjectConceptCode;
                    fiipsProject.LetDate = tempFiipsProject.LetDate;
                    fiipsProject.FiscalYear = tempFiipsProject.LetDate.Month > 6 && tempFiipsProject.LetDate.Month <= 12 ? tempFiipsProject.LetDate.Year + 1 : tempFiipsProject.LetDate.Year;
                    fiipsProject.PseDate = tempFiipsProject.PseDate;
                    fiipsProject.EpseDate = tempFiipsProject.EarliestPseDate;
                }
            }

            if (fiipsProject != null)
            {
                UpdateProject(fiipsProject);
                buttonMapFiipsProject.Enabled = true;
                project.FosProjectId = fiipsProject.FosProjectId;
                project.FiipsImprovementConcept = fiipsProject.FiipsImprovementConcept;
                project.FiipsDescription = fiipsProject.FiipsDescription;
                project.FiipsCost = fiipsProject.FiipsCost;
                project.LifecycleStageCode = fiipsProject.LifecycleStageCode;
                project.PseDate = fiipsProject.PseDate;
                project.EpseDate = fiipsProject.EpseDate;
                dataServ.UpdateProjectFosProjectId(project.ProjectHistoryDbId, constructionId);
            }
            else
            {
                if (MessageBox.Show(String.Format("Unable to find a Fiips project with Construction Id {0}. Update database anyway?", constructionId), "SCT", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    dataServ.UpdateProjectFosProjectId(project.ProjectHistoryDbId, constructionId);
                    project.FosProjectId = constructionId;
                }
            }
        }

        private void buttonUpdateFiipsData_Click(object sender, EventArgs e)
        {
            string constructionId = textBoxStructureProjectFosProjectId.Text.Trim();

            if (constructionId.Length != 8)
            {
                MessageBox.Show("Invalid Construction Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateAssociatedConstructionId(constructionId);
        }

        private void buttonMapStructureProject_Click(object sender, EventArgs e)
        {
            int projectDbId = 0;

            try
            {
                projectDbId = Convert.ToInt32(textBoxStructureProjectDbId.Text.Trim());
            }
            catch { }

            if (projectDbId != 0)
            {
                formMapping.SearchProject(projectDbId);
            }
        }

        private void buttonMapFiipsProject_Click(object sender, EventArgs e)
        {
            string constructionId = textBoxStructureProjectFosProjectId.Text.Trim();

            if (constructionId.Length == 8)
            {
                formMapping.SearchProject(constructionId);
            }
            else
            {
                MessageBox.Show("Invalid Construction Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDeleteProject_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete the project?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.Deactivated;
                project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                dataServ.DeleteProject(Convert.ToInt32(textBoxStructureProjectDbId.Text));
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                formMapping.EditProject(project, "Delete");
                this.Close();
            }
        }

        private void linkLabelBosCd_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabelBosCd.Enabled = false;
            List<Structure> structures = new List<Structure>();

            foreach (var workConcept in project.WorkConcepts.OrderBy(wc => wc.StructureId))
            {
                try
                {
                    structures.Add(dataServ.GetSptStructure(workConcept.StructureId));
                }
                catch { }
            }

            ReportWriterService.CreateProjectCd((DatabaseService)dataServ, project, structures, DateTime.Now, associatedProject);

            /*
            if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
            {
                ReportWriter.CreateProjectCd(associatedProject, structures, DateTime.Now, project);
            }
            else
            {
                ReportWriter.CreateProjectCd(project, structures, DateTime.Now, associatedProject);
            }*/

            linkLabelBosCd.Enabled = true;
        }

        private void buttonAddNewStructure_Click(object sender, EventArgs e)
        {

        }

        private void buttonUploadProjectFiles_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialogInProject.ShowDialog();
        }

        private void FormStructureProject_Load(object sender, EventArgs e)
        {
            splitContainerWorkConceptsReview.Panel2Collapsed = true;
            splitContainerPrimaryWorkConcept.SplitterDistance = Convert.ToInt32(this.Width / 1.5);
            splitContainerSecondaryWorkConcepts.SplitterDistance = Convert.ToInt32(this.Width / 1.5);
            splitContainerPlus.SplitterDistance = Convert.ToInt32(this.Width / 1.5);
        }

        private void dataGridViewProjectWorkConcepts_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {

        }

        private void buttonWithdrawCertification_Click(object sender, EventArgs e)
        {
            string subject = String.Format("Str Proj {0}- {1} by {2}", project.ProjectDbId, "Withdraw Review Request", project.UserFullName);

            if (!dataServ.GetApplicationMode().Equals("PROD"))
            {
                subject += String.Format(" in {0} Environment", dataServ.GetApplicationMode());
            }

            string message = String.Format("Str Proj {0} in {1}", project.ProjectDbId, project.FiscalYear);
            message += String.Format("</br>Project Status: {0}", project.Status.ToString());
            message += String.Format("</br>Action: {0}", "Withdraw Review Request by " + project.UserFullName + " on " + DateTime.Now);
            string[] to = new string[] { "joshua.dietsche@dot.wi.gov", "philip.meinel@dot.wi.gov", "ryan.bowers@dot.wi.gov", "joseph.barut@dot.wi.gov" };
            
            if (dataServ.GetApplicationMode().Equals("DEV"))
            {
                to = new string[] { "joseph.barut@dot.wi.gov" };
            }

            message += String.Format("</br></br>{0} Work Concepts", project.WorkConcepts.Count);

            foreach (var wc in project.WorkConcepts)
            {
                message += String.Format("</br>{0}, ({1}){2}, {3}", wc.StructureId, wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription, wc.Status.ToString());
            }

            EmailService.ComposeMessage(subject, message, to, null, null, new string[] { "joseph.barut@dot.wi.gov" });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"\\mad00fph\n4public\bos\wispt-test\certmockup.xlsm");
        }

        private void buttonSavePrecertifier_Click(object sender, EventArgs e)
        {
            string selection = "";
            savedTransaction = true;
            try
            {
                selection = comboBoxPrecertifier.SelectedItem.ToString();
            }
            catch { }

            //string fullName = "";
            int precertificationLiaisonUserDbId = 0;

            if (!String.IsNullOrEmpty(selection))
            {
                precertificationLiaisonUserDbId = GetLiaisonUserDbId(selection);

                if (String.IsNullOrEmpty(textBoxAcceptablePseDateStart.Text.Trim()))
                {
                    if (project.AdvanceableFiscalYear != 0)
                    {
                        textBoxAcceptablePseDateStart.Text = dataServ.CalculateAcceptablePseDateStart(project.AdvanceableFiscalYear).ToString("MM/dd/yyyy");
                    }
                    else
                    {
                        textBoxAcceptablePseDateStart.Text = dataServ.CalculateAcceptablePseDateStart(project.FiscalYear).ToString("MM/dd/yyyy");
                    }

                    project.AcceptablePseDateStart = Convert.ToDateTime(textBoxAcceptablePseDateStart.Text.Trim());
                }

                if (String.IsNullOrEmpty(textBoxAcceptablePseDateEnd.Text.Trim()))
                {
                    textBoxAcceptablePseDateEnd.Text = dataServ.CalculateAcceptablePseDateEnd(project.FiscalYear).ToString("MM/dd/yyyy");
                    project.AcceptablePseDateEnd = Convert.ToDateTime(textBoxAcceptablePseDateEnd.Text.Trim());
                }

                dataServ.SaveCertifier(precertificationLiaisonUserDbId, "Precertification", project, workConceptsAdded, userAccount);
                UpdateProjectInfo(project);
                // Parse selection
                /*
                precertificationLiaisonUserDbId = GetLiaisonUserDbId(selection);
                UserAccount liaisonUserAccount = database.GetPrecertificationLiaisons().Where(l => l.UserDbId == precertificationLiaisonUserDbId).First();
                fullName = liaisonUserAccount.FirstName + " " + liaisonUserAccount.LastName;
                project.Locked = true;
                project.InPrecertification = true;
                project.PrecertificationLiaisonUserDbId = precertificationLiaisonUserDbId;
                project.PrecertificationLiaisonUserFullName = fullName;
                //project.Status = StructuresProgramType.ProjectStatus.InPrecertification;
                //project.UserDbId = precertificationLiaisonUserDbId;
                project.UserDbId = userAccount.UserDbId;
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserDbIds.Add(precertificationLiaisonUserDbId);
                project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                project.UserAction = StructuresProgramType.ProjectUserAction.Precertification;
                database.UpdatePrecertifier(project, workConceptsAdded);
                project.WorkConcepts = workConceptsAdded;
                project.History = database.GetProjectHistory(project.ProjectDbId);
                UpdateProjectInfo(project);
                //formMapping.EditProject(project, "Update");
                Email.ComposeMessage(project, userAccount, database.GetEmailAddresses(project.UserDbIds), database.GetApplicationMode(), Path.Combine(database.GetMyDirectory(), "bos.jpg"), database);*/
            }
            else
            {
                // If assigned to a liaison and gets unassigned
                if (project.PrecertificationLiaisonUserDbId != 0)
                {
                    dataServ.SaveCertifier(0, "Precertification", project, workConceptsAdded, userAccount);
                    UpdateProjectInfo(project);
                    /*
                    UserAccount formerLiaisonUserAccount = database.GetPrecertificationLiaisons().Where(l => l.UserDbId == project.PrecertificationLiaisonUserDbId).First();
                    fullName = formerLiaisonUserAccount.FirstName + " " + formerLiaisonUserAccount.LastName;
                    project.Locked = false;
                    project.InPrecertification = false;
                    project.UserDbId = userAccount.UserDbId;
                    project.UserDbIds.Add(userAccount.UserDbId);
                    project.UserDbIds.Add(precertificationLiaisonUserDbId);
                    project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                    project.UserAction = StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment;
                    project.PrecertificationLiaisonUserDbId = 0;
                    project.PrecertificationLiaisonUserFullName = "";
                    database.UpdatePrecertifier(project, workConceptsAdded);
                    project.WorkConcepts = workConceptsAdded;
                    project.History = database.GetProjectHistory(project.ProjectDbId);
                    UpdateProjectInfo(project);
                    Email.ComposeMessage(project, userAccount, database.GetEmailAddresses(project.UserDbIds), database.GetApplicationMode(), Path.Combine(database.GetMyDirectory(), "bos.jpg"), database);
                */
                }
            }
        }

        // Liaison string format: firstName lastName (userdbid)
        private int GetLiaisonUserDbId(string liaison)
        {
            return controller.GetLiaisonUserDbId(liaison);
        }

        private void buttonSaveStructurePrecertification_Click(object sender, EventArgs e)
        {
            string decision = comboBoxPrecertificationDecision.SelectedItem.ToString();

            if (!project.InPrecertification && !decision.Equals(StructuresProgramType.PrecertificatioReviewDecision.None))
            {
                string internalComments = textBoxPrecertificationInternalComments.Text.Trim();
                int projectWorkConceptHistoryDbId = 0;
                Int32.TryParse(labelLastPrecertificationId.Text, out projectWorkConceptHistoryDbId);

                if (projectWorkConceptHistoryDbId != 0)
                {
                    currentWorkConcept.PrecertificationDecisionInternalComments = internalComments;
                    dataServ.UpdateWorkConceptPrecertificationInternalComments(internalComments, projectWorkConceptHistoryDbId);
                    tabControlProject.SelectedTab = tabPageProjectInfo;
                }
                //currentWorkConcept.PrecertificationDecisionInternalComments = internalComments;
            }
            else
            {
                StructuresProgramType.WorkConceptStatus workConceptStatus = StructuresProgramType.WorkConceptStatus.Unapproved;

                switch (labelWorkConceptStatus.Text.ToUpper())
                {
                    case "PRECERTIFIED":
                        workConceptStatus = StructuresProgramType.WorkConceptStatus.Precertified;
                        break;
                    case "PROPOSE":
                        workConceptStatus = StructuresProgramType.WorkConceptStatus.Proposed;
                        break;
                }

                // Unapproved or proposed work concept requires Accept or Reject decision
                if (String.IsNullOrEmpty(decision) && (workConceptStatus == StructuresProgramType.WorkConceptStatus.Unapproved
                                                        || workConceptStatus == StructuresProgramType.WorkConceptStatus.Proposed))
                {
                    MessageBox.Show("You must 'Accept' or 'Reject' an unapproved or a proposed work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string reasonCategory = "";
                string reasonExplanation = textBoxPrecertificationReasonExplanation.Text.Trim();
                string internalComments = textBoxPrecertificationInternalComments.Text.Trim();

                if (workConceptStatus == StructuresProgramType.WorkConceptStatus.Unapproved || workConceptStatus == StructuresProgramType.WorkConceptStatus.Proposed
                    || !String.IsNullOrEmpty(decision))
                {
                    try
                    {
                        reasonCategory = comboBoxPrecertificationReasonCategory.SelectedItem.ToString();
                    }
                    catch
                    { }

                    if (String.IsNullOrEmpty(reasonCategory))
                    {
                        MessageBox.Show("You must provide a 'Reason Category' for any precertification decision.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (String.IsNullOrEmpty(reasonExplanation))
                    {
                        MessageBox.Show("You must provide a 'Reason Explanation' for any precertification decision.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                int currentWorkConceptDbId = Convert.ToInt32(textBoxCurrentWorkConceptDbId.Text);
                string currentStructureId = textBoxCurrentStructureId.Text;
                var workConcepts = workConceptsAdded.Where(w => w.WorkConceptDbId == currentWorkConceptDbId && w.StructureId.Equals(currentStructureId));
                var currentWorkConcept = workConcepts.Count() > 0 ? workConcepts.First() : null;

                if (workConceptStatus == StructuresProgramType.WorkConceptStatus.Unapproved
                    || workConceptStatus == StructuresProgramType.WorkConceptStatus.Proposed
                    || !String.IsNullOrEmpty(decision))
                {
                    currentWorkConcept.PrecertificationDecision = decision.Equals("Accept") ? StructuresProgramType.PrecertificatioReviewDecision.Accept : StructuresProgramType.PrecertificatioReviewDecision.Reject;
                    currentWorkConcept.Status = decision.Equals("Accept") ? StructuresProgramType.WorkConceptStatus.Precertified : StructuresProgramType.WorkConceptStatus.Unapproved;
                }

                currentWorkConcept.PrecertificationDecisionReasonCategory = reasonCategory;
                currentWorkConcept.PrecertificationDecisionReasonExplanation = reasonExplanation;
                currentWorkConcept.PrecertificationDecisionInternalComments = internalComments;

                // Update the grid
                foreach (DataGridViewRow row in dataGridViewProjectWorkConcepts.Rows)
                {
                    if (Convert.ToInt32(row.Cells["dgvcWorkConceptDbId"].Value) == currentWorkConceptDbId
                        && row.Cells["dgvcStructureId"].Value.ToString().Equals(currentStructureId))
                    {
                        //row.Cells["dgvcCertifiedWorkConceptStatus"].Value = currentWorkConcept.Status.ToString();
                        row.Cells["dgvcPrecertificationDecision"].Value =
                            currentWorkConcept.PrecertificationDecision != StructuresProgramType.PrecertificatioReviewDecision.None ?
                            currentWorkConcept.PrecertificationDecision.ToString() : "";

                        if (currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Certified || currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                        }
                        else if (currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Precertified || currentWorkConcept.Evaluate)
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Precertified";

                            if (currentWorkConcept.Evaluate && project.Status == StructuresProgramType.ProjectStatus.Certified)
                            {
                                row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                                row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                            }
                        }
                        else if (currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                        {
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                            row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Unapproved";
                        }

                        break;
                    }
                }

                // Update project status
                UpdateProjectStatus();
                project.WorkConcepts = workConceptsAdded;

                // Update the database
                dataServ.UpdateWorkConceptPrecertification(project, currentWorkConcept);

                // Update Project Info
                UpdateProjectInfo(project);

                // Update Precertification Accept or Reject
                UpdateProjectPrecertificationAcceptReject();

                tabControlProject.SelectedTab = tabPageProjectInfo;

                // Update Navigation
                formMapping.EditProject(project, "Update");
            }

            savedTransaction = true;
        }

        private void UpdateProjectPrecertificationAcceptReject()
        {
            /*
            if (project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification && project.Status == StructuresProgramType.ProjectStatus.Precertified)
            {
                buttonTransitionallyCertify.Enabled = true;
            }*/

            if (project.InPrecertification)
            {
                /*
                if (project.WorkConcepts.Where(w => w.Status != StructuresProgramType.WorkConceptStatus.Unapproved).Count() == 0)
                {

                }*/
                if (project.Status == StructuresProgramType.ProjectStatus.Precertified || project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                {
                    buttonAcceptPrecertification.Enabled = true;
                    buttonTransitionallyCertify.Enabled = true;
                }
                else
                {
                    bool enableReject = false;
                    var wcs = project.WorkConcepts.Where(w => w.Status == StructuresProgramType.WorkConceptStatus.Unapproved);
                                                            //|| w.Status == StructuresProgramType.WorkConceptStatus.Evaluate);
                    if (wcs.Count() > 0)
                    {
                        enableReject = true;
                    }

                    //foreach (var wc in wcs)
                    //{
                        /*
                        if (wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate)
                        {
                            if (wc.PrecertificationDecision != StructuresProgramType.PrecertificatioReviewDecision.Reject)
                            {
                                enableReject = false;
                            }
                        }
                        else*/

                        /*
                        {
                            if (wc.PrecertificationDecision != StructuresProgramType.PrecertificatioReviewDecision.Reject)
                            {
                                enableReject = false;
                            }
                        }*/
                    //}

                    buttonRejectPrecertification.Enabled = enableReject;
                }
            }
        }

        private void UpdateProjectStatus()
        {
            //|| (w.Status == StructuresProgramType.WorkConceptStatus.Evaluate)
            if (!project.InCertification)
            {
                if (workConceptsAdded.All(w => w.Status == StructuresProgramType.WorkConceptStatus.Certified || w.Status == StructuresProgramType.WorkConceptStatus.Quasicertified))
                {
                    checkBoxUnapproved.Checked = false;
                    checkBoxPrecertified.Checked = false;
                    checkBoxCertified.Checked = true;
                }
                else if (workConceptsAdded.Where(w => ((w.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                                                   )).Count() > 0)
                {
                    checkBoxUnapproved.Checked = true;
                    checkBoxPrecertified.Checked = false;
                    checkBoxCertified.Checked = false;
                }
                else
                {
                    checkBoxUnapproved.Checked = false;
                    checkBoxPrecertified.Checked = true;
                    checkBoxCertified.Checked = false;
                }

                if (checkBoxUnapproved.Checked)
                {
                    project.Status = StructuresProgramType.ProjectStatus.Unapproved;
                    groupBoxProjectStatus.BackColor = Color.Red;
                    linkLabelBosCd.Visible = false;
                }
                else if (checkBoxPrecertified.Checked)
                {
                    project.Status = StructuresProgramType.ProjectStatus.Precertified;
                    groupBoxProjectStatus.BackColor = Color.Yellow;
                    linkLabelBosCd.Visible = false;
                }
                else if (checkBoxCertified.Checked)
                {
                    project.Status = StructuresProgramType.ProjectStatus.Certified;
                    groupBoxProjectStatus.BackColor = Color.Green;
                    linkLabelBosCd.Visible = true;
                }
            }
        }

        private void textBoxStructureProjectAdvanceableFiscalYear_Validating(object sender, CancelEventArgs e)
        {
            if (project.Status != StructuresProgramType.ProjectStatus.Fiips && !textBoxStructureProjectAdvanceableFiscalYear.ReadOnly)
            {
                try
                {
                    if (project.AdvanceableFiscalYear > 0 && Convert.ToInt32(textBoxStructureProjectAdvanceableFiscalYear.Text.Trim()) == project.AdvanceableFiscalYear)
                    {
                        return;
                    }
                }
                catch { }

                if (ValidateProjectFiscalYear())
                {
                    bool validAdvanceableProjectFiscalYear = ValidateAdvanceableProjectFiscalYear();
                    int projectFiscalYear = Convert.ToInt32(textBoxStructureProjectFiscalYear.Text.Trim());

                    if (!validAdvanceableProjectFiscalYear)
                    {
                        MessageBox.Show(String.Format("Enter an advanceable year <= {0} and >= {1}", projectFiscalYear, currentFiscalYear), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                    else if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length > 0)
                    {
                        changesMade = true;
                        try
                        {
                            DetermineProjectStatus(projectFiscalYear);

                            int advanceableProjectFiscalYear = Convert.ToInt32(textBoxStructureProjectAdvanceableFiscalYear.Text.Trim());
                            DetermineProjectStatus(advanceableProjectFiscalYear, true);
                            //DetermineProjectStatus(projectFiscalYear);
                            //textBoxAcceptablePseDateStart.Text = database.CalculateAcceptablePseDateStart(advanceableProjectFiscalYear).ToString("MM/dd/yyyy");
                        }
                        catch
                        {
                            MessageBox.Show(String.Format("Enter an advanceable year <= {0} and >= {1}", projectFiscalYear, currentFiscalYear), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                    else if (textBoxStructureProjectAdvanceableFiscalYear.Text.Trim().Length == 0)
                    {
                        DetermineProjectStatus(projectFiscalYear);
                        //textBoxAcceptablePseDateStart.Text = database.CalculateAcceptablePseDateStart(projectFiscalYear).ToString("MM/dd/yyyy");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a valid project fiscal year first.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonAcceptPrecertification_Click(object sender, EventArgs e)
        {
            UpdateProjectPrecertification(StructuresProgramType.PrecertificatioReviewDecision.Accept);
        }

        private void buttonRejectPrecertification_Click(object sender, EventArgs e)
        {
            UpdateProjectPrecertification(StructuresProgramType.PrecertificatioReviewDecision.Reject);
        }

        private void UpdateProjectPrecertification(StructuresProgramType.PrecertificatioReviewDecision decision)
        {
            savedTransaction = true;
            // Update project object
            project.InPrecertification = false;
            project.Locked = false;
            project.UserDbId = userAccount.UserDbId;
            project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
            project.UserDbIds.Add(userAccount.UserDbId);

            if (decision == StructuresProgramType.PrecertificatioReviewDecision.Accept)
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification;
            }
            else
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.BosRejectedPrecertification;
            }

            // Update database
            dataServ.UpdateProjectCertification(project, decision);
            project.History = dataServ.GetProjectHistory(project.ProjectDbId);
         
            // Update Nav and Map & Email
            formMapping.EditProject(project, "Update");
            //Email.ComposeMessage(project, userAccount, database.GetEmailAddresses(project.UserDbIds), database.GetApplicationMode(), Path.Combine(database.GetMyDirectory(), "bos.jpg"), database);
            EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
            this.Close();

            // TODO: Email
            // Who gets the email?
            /*
            UserAccount submitter = database.GetPrecertificationSubmitter(project.ProjectDbId);
            string subject = String.Format("Str Proj {0}- {1} by {2}", project.ProjectDbId, project.UserAction, project.UserFullName);

            if (!database.GetApplicationMode().Equals("PROD"))
            {
                subject += String.Format(" in {0} Environment", database.GetApplicationMode());
            }

            string message = String.Format("Str Proj {0} in {1}", project.ProjectDbId, project.FiscalYear);
            message += String.Format("</br>Project Status: {0}", project.Status.ToString());
            message += String.Format("</br>Action: {0}", project.UserAction + " by " + project.UserFullName + " on " + project.UserActionDateTime);
            string[] to = new string[1];
            to[0] = submitter.EmailAddress;
            message += String.Format("</br></br># of Work Concepts: ", project.WorkConcepts.Count);

            foreach (var wc in project.WorkConcepts)
            {
                message += String.Format("</br>{0}, ({1}){2}, {3}", wc.StructureId, wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription, wc.Status.ToString());
            }*/

            //Email.ComposeMessage(subject, message, to);
            //Email.ComposeMessage(project, userAccount, database.GetEmailAddresses(project.UserDbIds), database.GetApplicationMode(), Path.Combine(database.GetMyDirectory(), "bos.jpg"));
            //this.Close();
        }

        private void OpenWorkConceptHistory(string structureId, int workConceptDbId, Project project)
        {
            var wcs = dataServ.GetProjectWorkConceptHistory(structureId, workConceptDbId, project);

            if (wcs.Count() == 0)
            {
                MessageBox.Show("No project history", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string excelFilePath = ExcelReporterService.WriteStructureCertificationHistory(project, wcs, userAccount, (DatabaseService)dataServ);

                if (System.IO.File.Exists(excelFilePath))
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = excelFilePath;
                    Process.Start(psi);
                }
            }
        }

        private void linkLabelHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (project.ProjectDbId != 0 && textBoxCurrentStructureId.Text.Length > 0 && textBoxCurrentWorkConceptDbId.Text.Length > 0)
            {
                OpenWorkConceptHistory(textBoxCurrentStructureId.Text, Convert.ToInt32(textBoxCurrentWorkConceptDbId.Text), project);
                /*
                var wcs = database.GetProjectWorkConceptHistory(textBoxCurrentStructureId.Text, Convert.ToInt32(textBoxCurrentWorkConceptDbId.Text), project);

                if (wcs.Count() == 0)
                {
                    MessageBox.Show("No precertification history", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string excelFilePath = ExcelReporter.WriteStructureCertificationHistory(project, wcs, userAccount, database);

                    if (System.IO.File.Exists(excelFilePath))
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = excelFilePath;
                        Process.Start(psi);
                    }
                }*/
            }
        }

        private async void tvFiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes[0].Text == FileManagerService.LOADING)
            {
                picLoading.Visible = true;

                try
                {
                    await fileManager.UpdateFolder(e.Node, project, userAccount);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Box error: {0}", ex.Message), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    picLoading.Visible = false;
                }

                //picLoading.Visible = false;
            }
        }

        private void tvFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is null)
            {
                //ToggleItemBtns(false);
                //ToggleFolderBtns(true);
                ToggleFileBtns(false, userAccount);
            }
            else
            {
                //ToggleItemBtns(true);
                if (((Item)e.Node.Tag).Type is ItemType.folder)
                {
                    //ToggleFolderBtns(true);
                    ToggleFileBtns(false, userAccount);
                }
                else
                {
                    //ToggleFolderBtns(true);
                    ToggleFileBtns(true, userAccount);
                }
            }
        }

        private void DisableFileBtns()
        {
            buttonAddFile.Enabled = false;
            buttonDeleteFile.Enabled = false;
            buttonOpen.Enabled = false;
            buttonDownload.Enabled = false;
        }

        private void ToggleFileBtns(bool isEnabled, UserAccount userAccount)
        {
            buttonRefresCertificationDirectory.Enabled = true;

            if (userAccount.IsRegionalRead || userAccount.IsSuperRead)
            {
                buttonAddFile.Enabled = false;
                buttonDeleteFile.Enabled = false;
                buttonOpen.Enabled = false;
                buttonDownload.Enabled = false;
            }
            else
            {
                buttonAddFile.Enabled = !isEnabled;
                buttonDeleteFile.Enabled = isEnabled;
                buttonOpen.Enabled = isEnabled;
                buttonDownload.Enabled = isEnabled;
            }
        }

        private async void openFileDialogInProject_FileOk(object sender, CancelEventArgs e)
        {
            picLoading.Visible = true;
            buttonAddFile.Enabled = false;

            try
            {
                if (tvFiles.SelectedNode == null || (tvFiles.SelectedNode.Tag == null) && tvFiles.SelectedNode.Parent == null)
                {
                    await BOS.Box.File.UploadAsync(openFileDialogInProject.FileNames, FileManagerService.ROOT_FOLDER);
                    //fileManager.UpdateTree(picLoading, tvFiles);
                    await fileManager.UpdateProjectFileTree(project, tvFiles, picLoading);
                }
                else if (tvFiles.SelectedNode.Tag == null && tvFiles.SelectedNode.Parent != null)
                {
                    string parentId = (tvFiles.SelectedNode.Parent.Tag as Item).Id;
                    await BOS.Box.File.UploadAsync(openFileDialogInProject.FileNames, parentId);
                    await fileManager.UpdateFolder(tvFiles.SelectedNode.Parent, project, userAccount);
                }
                else
                {
                    string parentId = (tvFiles.SelectedNode.Tag as Item).Id;
                    await BOS.Box.File.UploadAsync(openFileDialogInProject.FileNames, parentId);
                    await fileManager.UpdateFolder(tvFiles.SelectedNode, project, userAccount);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error with adding file(s): {0}", ex.InnerException), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            picLoading.Visible = false;
            buttonAddFile.Enabled = true;
        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            openFileDialogInProject.ShowDialog();
        }

        private async void buttonDeleteFile_Click(object sender, EventArgs e)
        {
            TreeNode parentNode = null;

            try
            {
                parentNode = tvFiles.SelectedNode.Parent;
            }
            catch { }

            Item delItem = (Item)tvFiles.SelectedNode.Tag;
            DialogResult result = MessageBox.Show("Are you sure you want " +
                "to delete " + delItem.Name + "?", "Confirm Delete",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                picLoading.Visible = true;
                buttonDeleteFile.Enabled = false;
                await delItem.DeleteAsync();
                if (tvFiles.SelectedNode.Parent == null)//Checks to see if parent is root folder
                {
                    fileManager.UpdateProjectFileTree(project, tvFiles, picLoading);//Refreshes Root
                }
                else
                {
                    await fileManager.UpdateFolder(tvFiles.SelectedNode.Parent, project, userAccount);
                }
                picLoading.Visible = false;
                tvFiles.SelectedNode = null;
                tvFiles.SelectedNode = tvFiles.Nodes[0];
                //tvFiles.Select();

                /*
                if (parentNode != null)
                {
                    tvFiles.SelectedNode = tvFiles.Nodes[0];
                    //tvFiles.Select();
                }*/
                //DisableFileBtns();
            }
        }

        private async void buttonOpen_Click(object sender, EventArgs e)
        {
            Item openItem = (Item)tvFiles.SelectedNode.Tag;
            picLoading.Visible = true;
            buttonOpen.Enabled = false;
            string url = await BOS.Box.File.PreviewAsync(openItem.Id, true);
            Process.Start(url);
            buttonOpen.Enabled = true;
            picLoading.Visible = false;
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            Item file = tvFiles.SelectedNode.Tag as Item;
            saveFileDialog1.DefaultExt = System.IO.Path.GetExtension(file.Name);
            saveFileDialog1.FileName = file.Name;
            saveFileDialog1.ShowDialog();
        }

        private async void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            using (var fileStream = System.IO.File.Create(saveFileDialog1.FileName))
            {
                Item file = tvFiles.SelectedNode.Tag as Item;
                var dl = await BOS.Box.File.DownloadAsync(file.Id);
                dl.CopyTo(fileStream);
            }
        }

        private async void buttonRefresCertificationDirectory_Click(object sender, EventArgs e)
        {
            await fileManager.UpdateProjectFileTree(project, tvFiles, picLoading);
        }

        private void linkLabelStructureId_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string structureId = linkLabelStructureId.Text;
            formMapping.OpenStructureWindow(structureId);
        }

        private void SaveCertifier(int certificationLiaisonUserDbId, Project project, List<WorkConcept> workConcepts)
        {
            if (certificationLiaisonUserDbId != 0)
            {
                UserAccount liaisonUserAccount = dataServ.GetCertificationLiaisons().Where(l => l.UserDbId == certificationLiaisonUserDbId).First();
                string fullName = liaisonUserAccount.FirstName + " " + liaisonUserAccount.LastName;
                project.Locked = true;
                project.InPrecertification = false;
                project.InCertification = true;
                project.CertificationLiaisonUserDbId = certificationLiaisonUserDbId;
                project.CertificationLiaisonUserFullName = fullName;
                project.UserDbId = userAccount.UserDbId;
                project.UserDbIds.Add(certificationLiaisonUserDbId);
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                project.UserAction = StructuresProgramType.ProjectUserAction.Certification;
                //dataServ.UpdateCertifier(project, workConceptsAdded);
                //project.WorkConcepts = workConceptsAdded;
                dataServ.UpdateCertifier(project, workConcepts);
                project.WorkConcepts = workConcepts;
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                UpdateProjectInfo(project);
                //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
            }
            else // Getting unassigned
            {
                if (project.CertificationLiaisonUserDbId != 0)
                {
                    UserAccount formerLiaisonUserAccount = dataServ.GetCertificationLiaisons().Where(l => l.UserDbId == project.CertificationLiaisonUserDbId).First();
                    string fullName = formerLiaisonUserAccount.FirstName + " " + formerLiaisonUserAccount.LastName;
                    project.Locked = false;
                    project.InPrecertification = false;
                    project.InCertification = false;
                    project.UserDbId = userAccount.UserDbId;
                    project.UserDbIds.Add(certificationLiaisonUserDbId);
                    project.UserDbIds.Add(userAccount.UserDbId);
                    project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                    project.UserAction = StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment;
                    project.CertificationLiaisonUserDbId = 0;
                    project.CertificationLiaisonUserFullName = "";
                    //dataServ.UpdateCertifier(project, workConceptsAdded);
                    //project.WorkConcepts = workConceptsAdded;
                    dataServ.UpdateCertifier(project, workConcepts);
                    project.WorkConcepts = workConcepts;
                    project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                    UpdateProjectInfo(project);
                    //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                    EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                }
            }
        }

        private void buttonSaveCertifier_Click(object sender, EventArgs e)
        {
            bool missingFiipsProject = String.IsNullOrEmpty(project.FosProjectId) ? true : false;
            DialogResult result = DialogResult.Yes;
            DialogResult result2 = DialogResult.Yes;

            if (missingFiipsProject)
            {
                result2 = MessageBox.Show("This structures project is missing a corresponding FIIPS project. Notify the region about it?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result2 == DialogResult.Yes)
                {
                    //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ, true);
                    EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ, true);
                }

                result = MessageBox.Show("Proceed with assigning a certification liaison?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            savedTransaction = true;
            string selection = "";
            int certificationLiaisonUserDbId = 0;
            bool initialAssignment = true;

            try
            {
                selection = comboBoxCertifier.SelectedItem.ToString();
            }
            catch { }

            //string fullName = "";
            if (!String.IsNullOrEmpty(selection))
            {
                // Parse selection
                certificationLiaisonUserDbId = GetLiaisonUserDbId(selection);

                if (project.CertificationLiaisonUserDbId == certificationLiaisonUserDbId)
                {
                    return;
                }

                if (project.CertificationLiaisonUserDbId != 0)
                {
                    initialAssignment = false;
                }

                //dataServ.SaveCertifier(certificationLiaisonUserDbId, "Certification", project, workConceptsAdded, userAccount, initialAssignment);
                dataServ.SaveCertifier(certificationLiaisonUserDbId, "Certification", project, workConceptsAdded, userAccount);
                UpdateProjectInfo(project);
                //Email.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                //SaveCertifier(certificationLiaisonUserDbId, project, workConceptsAdded);

                /*
                UserAccount liaisonUserAccount = dataServ.GetCertificationLiaisons().Where(l => l.UserDbId == certificationLiaisonUserDbId).First();
                fullName = liaisonUserAccount.FirstName + " " + liaisonUserAccount.LastName;
                project.Locked = true;
                project.InPrecertification = false;
                project.InCertification = true;
                project.CertificationLiaisonUserDbId = certificationLiaisonUserDbId;
                project.CertificationLiaisonUserFullName = fullName;
                project.UserDbId = userAccount.UserDbId;
                project.UserDbIds.Add(certificationLiaisonUserDbId);
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                project.UserAction = StructuresProgramType.ProjectUserAction.Certification;
                dataServ.UpdateCertifier(project, workConceptsAdded);
                project.WorkConcepts = workConceptsAdded;
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                UpdateProjectInfo(project);
                Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                */
            }
            else
            {
                // If already assigned to a liaison and gets unassigned
                if (project.CertificationLiaisonUserDbId != 0)
                {
                    //SaveCertifier(0, project, workConceptsAdded);
                    dataServ.SaveCertifier(0, "Certification", project, workConceptsAdded, userAccount);
                    UpdateProjectInfo(project);
                    /*
                    project.Locked = false;
                    project.InCertification = false;
                    project.UserAction = StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment;
                    project.CertificationLiaisonUserDbId = 0;
                    project.CertificationLiaisonUserFullName = "";
                    dataServ.UpdateCertifier(project, workConceptsAdded);
                    UpdateProjectInfo(project);*/

                    /*
                    UserAccount formerLiaisonUserAccount = dataServ.GetCertificationLiaisons().Where(l => l.UserDbId == project.CertificationLiaisonUserDbId).First();
                    fullName = formerLiaisonUserAccount.FirstName + " " + formerLiaisonUserAccount.LastName;
                    project.Locked = false;
                    project.InPrecertification = false;
                    project.InCertification = false;
                    project.UserDbId = userAccount.UserDbId;
                    project.UserDbIds.Add(certificationLiaisonUserDbId);
                    project.UserDbIds.Add(userAccount.UserDbId);
                    project.UserFullName = String.Format("{0} {1}", userAccount.FirstName, userAccount.LastName);
                    project.UserAction = StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment;
                    project.CertificationLiaisonUserDbId = 0;
                    project.CertificationLiaisonUserFullName = "";
                    dataServ.UpdateCertifier(project, workConceptsAdded);
                    project.WorkConcepts = workConceptsAdded;
                    project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                    UpdateProjectInfo(project);
                    Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);*/
                }
            }
        }

        private void dgvPrimaryElements_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            return;
        }

        private void dgvPrimaryElements_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            return;
        }

        private void dgvSecondaryElements_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void buttonViewPlans_Click(object sender, EventArgs e)
        {
            string plansFolderPath = warehouseDatabase.GetPlansFolderPath(linkLabelStructureId.Text, comboBoxPlansYear.SelectedItem.ToString());

            try
            {
                Process.Start(plansFolderPath);
            }
            catch
            {
                MessageBox.Show("Unable to open bridge plans for year " + comboBoxPlansYear.SelectedItem.ToString() + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSaveStructureCertification_Click(object sender, EventArgs e)
        {
            savedTransaction = true;
            UpdateProjectElementWorkConcept(sender, e, "save");
        }

        private void buttonCertifyStructure_Click(object sender, EventArgs e)
        {
            savedTransaction = true;
            UpdateProjectElementWorkConcept(sender, e, "certify");
        }

        private void UpdateProjectElementWorkConcept(object sender, EventArgs e, string action)
        {
            /*
            if (action.Equals("save") && currentWorkConcept.Status == StructuresProgramType.WorkConceptStatus.Certified)
            {
                DialogResult result = MessageBox.Show("Work concept's already certified; saving resets its status to precertified. Continue saving?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }
            else if (action.Equals("reject"))
            {
                DialogResult result = MessageBox.Show("Rejecting the work concept?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }
            else if (action.Equals("certify") && radioButtonToBeDeterminedDesignResourcing.Checked)
            {
                MessageBox.Show("You must select In-House or Consultant for design resourcing to certify the work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            /*
            if (action.Equals("certify") && radioButtonToBeDeterminedDesignResourcing.Checked)
            {
                MessageBox.Show("You must select In-House or Consultant for design resourcing to certify the work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/


            List<ElementWorkConcept> ewcs = new List<ElementWorkConcept>();
            int primaryEwcsCount = 0;

            foreach (DataGridViewRow row in dgvPrimaryElements.Rows)
            {
                if (row.Cells["dgvcPrimaryElementNumber"].Value != null)
                {
                    ElementWorkConcept ewc = new ElementWorkConcept();
                    ewc.StructureId = currentWorkConcept.StructureId;
                    ewc.ProjectWorkConceptHistoryDbId = currentWorkConcept.ProjectWorkConceptHistoryDbId;
                    ewc.ElementNumber = Convert.ToInt32(row.Cells["dgvcPrimaryElementNumber"].Value);
                    ewc.ElementName = row.Cells["dgvcPrimaryElementName"].Value.ToString();
                    ewc.WorkConceptCode = currentWorkConcept.WorkConceptCode;
                    ewc.WorkConceptDescription = currentWorkConcept.WorkConceptDescription;
                    ewc.WorkConceptLevel = "Primary";

                    try
                    {
                        ewc.Comments = row.Cells["dgvcPrimaryComments"].Value.ToString();
                    }
                    catch { }

                    ewcs.Add(ewc);
                    primaryEwcsCount++;
                }
            }

            foreach (DataGridViewRow row in dgvSecondaryElements.Rows)
            {
                bool saveRow = true;
                
                if (!action.Equals("save"))
                {
                    if (row.Cells["dgvcSecondaryElementNumber"].Value != null)
                    {
                        saveRow = (row.Cells["dgvcSecondaryComments"].Value != null && !row.Cells["dgvcSecondaryComments"].Value.Equals("")) ||
                                    (row.Cells["dgvcSecondaryWorkConcept"].Value != null && !row.Cells["dgvcSecondaryWorkConcept"].Value.Equals("")) ? true : false;
                    }
                }

                if (row.Cells["dgvcSecondaryElementNumber"].Value != null 
                    && saveRow)
                {
                    ElementWorkConcept ewc = new ElementWorkConcept();
                    ewc.StructureId = currentWorkConcept.StructureId;
                    ewc.ProjectWorkConceptHistoryDbId = currentWorkConcept.ProjectWorkConceptHistoryDbId;
                    ewc.ElementNumber = Convert.ToInt32(row.Cells["dgvcSecondaryElementNumber"].Value);
                    ewc.ElementName = row.Cells["dgvcSecondaryElementName"].Value.ToString();
                    
                    try
                    {
                        var secondaryWorkConceptFullDescription = row.Cells["dgvcSecondaryWorkConcept"].Value.ToString();
                        string[] parsed = ParseWorkConceptFullDescription(secondaryWorkConceptFullDescription);
                        var secondaryWorkConceptCode = parsed[0];
                        var secondaryWorkConceptDescription = parsed[1];
                        ewc.WorkConceptCode = secondaryWorkConceptCode;
                        ewc.WorkConceptDescription = secondaryWorkConceptDescription;
                    }
                    catch { }

                    ewc.WorkConceptLevel = "Secondary";

                    try
                    {
                        ewc.Comments = row.Cells["dgvcSecondaryComments"].Value.ToString();
                    }
                    catch { }

                    ewcs.Add(ewc);
                }
            }

            var workConcepts = workConceptsAdded.Where(w => w.WorkConceptDbId == currentWorkConcept.WorkConceptDbId && w.StructureId.Equals(currentWorkConcept.StructureId));
            var workConcept = workConcepts.First();

            if (action.Equals("certify"))
            {
                workConcept.Status = StructuresProgramType.WorkConceptStatus.Certified;
            }
            else if (action.Equals("reject"))
            {
                workConcept.Status = StructuresProgramType.WorkConceptStatus.Unapproved;
            }
            else if (workConcept.Status == StructuresProgramType.WorkConceptStatus.Precertified)
            {
                workConcept.Status = StructuresProgramType.WorkConceptStatus.Precertified;
            }

            bool updateWorkConceptToBeCertified = false;

            if (workConcept.WorkConceptCode.Equals("EV") 
                || workConcept.WorkConceptCode.Equals("PR"))
            {
                try
                {
                    string selectedWorkConcept = comboBoxNewWorkConcept.SelectedItem.ToString().ToUpper();

                    if (!action.Equals("reject") && !selectedWorkConcept.Contains("SELECT WORK CONCEPT") && !selectedWorkConcept.Contains("----------"))
                    {
                        string[] newWorkConcept = ParseWorkConceptFullDescription(selectedWorkConcept);
                        string workConceptCode = newWorkConcept[0];
                        string workConceptDescription = newWorkConcept[1];
                        workConcept.CertifiedWorkConceptCode = workConceptCode;
                        workConcept.CertifiedWorkConceptDescription = workConceptDescription;
                        updateWorkConceptToBeCertified = true;
                    }
                }
                catch { }
            }

            int matches
                = project.CertifiedElementWorkConceptCombinations
                .Where(el => el.ProjectWorkConceptHistoryDbId == workConcept.ProjectWorkConceptHistoryDbId
                        && el.StructureId.Equals(workConcept.StructureId)
                        && el.CertificationDateTime == workConcept.CertificationDateTime).Count();
            project.CertifiedElementWorkConceptCombinations.RemoveAll(
                el => el.ProjectWorkConceptHistoryDbId == workConcept.ProjectWorkConceptHistoryDbId
                && el.StructureId.Equals(workConcept.StructureId)
                && el.CertificationDateTime == workConcept.CertificationDateTime);
            //project.CertifiedElementWorkConceptCombinations.AddRange(ewcs);
            workConcept.CertificationDecision = action.Equals("reject") ? "Reject" : "Accept";
            workConcept.CertificationPrimaryWorkTypeComments = textBoxAdditionalPrimaryWorkConceptComments.Text.Trim();
            workConcept.CertificationSecondaryWorkTypeComments = textBoxAdditionalSecondaryWorkConceptComments.Text.Trim();
            workConcept.CertificationAdditionalComments = textBoxCertificationGeneralComments.Text.Trim();
            string cost = textBoxWorkConceptEstimatedConstructionCost.Text.Replace(",", "").Trim();

            if (!String.IsNullOrEmpty(cost))
            {
                try
                {
                    workConcept.EstimatedConstructionCost = Convert.ToInt32(cost);
                }
                catch
                { }
            }

            string loe = textBoxWorkConceptEstimatedDesignLevelOfEffort.Text.Replace(",", "").Trim();

            if (!String.IsNullOrEmpty(loe))
            {
                try
                {
                    workConcept.EstimatedDesignLevelOfEffort = Convert.ToInt32(loe);
                }
                catch
                { }
            }

            if (radioButtonToBeDeterminedDesignResourcing.Checked)
            {
                workConcept.DesignResourcing = "TBD";
            }
            else if (radioButtonInHouseDesignResourcing.Checked)
            {
                workConcept.DesignResourcing = "BOS";
            }
            else if (radioButtonConsultantDesignResourcing.Checked)
            {
                workConcept.DesignResourcing = "Consultant";
            }

            // Update the grid
            foreach (DataGridViewRow row in dataGridViewProjectWorkConcepts.Rows)
            {
                if (Convert.ToInt32(row.Cells["dgvcWorkConceptDbId"].Value) == workConcept.WorkConceptDbId
                    && row.Cells["dgvcStructureId"].Value.ToString().Equals(workConcept.StructureId))
                {
                    //row.Cells["dgvcCertifiedWorkConceptStatus"].Value = workConcept.Status.ToString();
                    if (updateWorkConceptToBeCertified)
                    {
                        //row.Cells["dgvcCertifiedWorkConcept"].Value = workConcept.CertifiedWorkConceptDescription;
                    }

                    if (workConcept.Status == StructuresProgramType.WorkConceptStatus.Certified)
                    {
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                    }
                    else if (workConcept.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                    {
                        //row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Transitionally Certified";
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Certified";
                    }
                    else if (workConcept.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                    {
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Unapproved";
                        /*
                        if (workConcept.Status == StructuresProgramType.WorkConceptStatus.Precertified || workConcept.CertifiedWorkConceptCode.Equals("EV"))
                        {
                            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;

                            if (wc.CertifiedWorkConceptCode.Equals("EV") && project.Status == StructuresProgramType.ProjectStatus.Certified)
                            {
                                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Green;
                            }
                        }
                        else
                        {
                            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Red;
                        }*/
                    }
                    else
                    {
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Style.BackColor = Color.Yellow;
                        row.Cells["dgvcCertifiedWorkConceptStatus"].Value = "Precertified";
                    }

                    break;
                }
            }

            //UpdateProjectStatus();
            project.WorkConcepts = workConceptsAdded;

            // Update the dataServ
            //dataServ.UpdateWorkConceptCertification(ewcs, workConcept, updateWorkConceptToBeCertified);
            dataServ.UpdateWorkConceptCertification(ewcs, workConcept);
            // Update project element work concept combinations
            project.CertifiedElementWorkConceptCombinations.AddRange(dataServ.GetElementWorkConceptPairings(workConcept.StructureId, workConcept.ProjectWorkConceptHistoryDbId, workConcept.CertificationDateTime));

            // Update Project Info
            UpdateProjectInfo(project, false);
            //tabControlProject.SelectedTab = tabPageProjectInfo;

            // Update Navigation
            formMapping.EditProject(project, "Update");
            //formMapping.SearchProject(project.ProjectDbId);
        }

        private void dgvPrimaryElements_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var dgv = (DataGridView)sender;
            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            if (columnIndex >= 0 && rowIndex >= 0)
            {
                if (dgv.Columns[columnIndex].Name.ToLower().Equals("dgvcprimaryelementnumber"))
                {
                    var currentCell = dgv.CurrentCell;

                    if (!String.IsNullOrEmpty(currentCell.EditedFormattedValue.ToString()))
                    {
                        int elementNumber = 0;

                        try
                        {
                            elementNumber = Convert.ToInt32(dgv.CurrentCell.EditedFormattedValue);
                            string elementName = warehouseDatabase.GetElementName(elementNumber);

                            if (!String.IsNullOrEmpty(elementName))
                            {
                                dgv.Rows[rowIndex].Cells["dgvcPrimaryElementName"].Value = elementName;
                            }
                            else
                            {
                                MessageBox.Show("Enter a valid element number.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                dgv.Rows[rowIndex].Cells["dgvcPrimaryElementName"].Value = "";
                                e.Cancel = true;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Enter a valid element number.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private void dgvSecondaryElements_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var dgv = (DataGridView)sender;
            int columnIndex = dgv.CurrentCell.ColumnIndex;
            int rowIndex = dgv.CurrentCell.RowIndex;

            if (columnIndex >= 0 && rowIndex >= 0)
            {
                if (dgv.Columns[columnIndex].Name.ToLower().Equals("dgvcsecondaryelementnumber"))
                {
                    var currentCell = dgv.CurrentCell;

                    if (!String.IsNullOrEmpty(currentCell.EditedFormattedValue.ToString()))
                    {
                        int elementNumber = 0;

                        try
                        {
                            elementNumber = Convert.ToInt32(dgv.CurrentCell.EditedFormattedValue);
                            string elementName = warehouseDatabase.GetElementName(elementNumber);

                            if (!String.IsNullOrEmpty(elementName))
                            {
                                dgv.Rows[rowIndex].Cells["dgvcSecondaryElementName"].Value = elementName;
                            }
                            else
                            {
                                MessageBox.Show("Enter a valid element number.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                dgv.Rows[rowIndex].Cells["dgvcSecondaryElementName"].Value = "";
                                e.Cancel = true;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Enter a valid element number.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.Cancel = true;
                        }
                    }
                }
            }
        }

        private string[] ParseWorkConceptFullDescription(string workConcept)
        {
            return controller.ParseWorkConceptFullDescription(workConcept);
        }

        private void textBoxWorkConceptEstimatedConstructionCost_Validating(object sender, CancelEventArgs e)
        {
            string cost = textBoxWorkConceptEstimatedConstructionCost.Text.Replace(",", "").Trim();
            int constructionCost = 0;

            if (!String.IsNullOrEmpty(cost))
            {
                try
                {
                    constructionCost = Convert.ToInt32(cost);
                }
                catch
                {
                    MessageBox.Show("Enter a valid construction cost.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void textBoxWorkConceptEstimatedDesignLevelOfEffort_Validating(object sender, CancelEventArgs e)
        {
            string loe = textBoxWorkConceptEstimatedDesignLevelOfEffort.Text.Replace(",", "").Trim();
            int levelOfEffort = 0;

            if (!String.IsNullOrEmpty(loe))
            {
                try
                {
                    levelOfEffort = Convert.ToInt32(loe);
                }
                catch
                {
                    MessageBox.Show("Enter a valid level of effort.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
        }

        private void buttonSubmitCertification_Click(object sender, EventArgs e)
        {
            DialogResult result = new DialogResult();

            if (project.WorkConcepts.All(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Certified
                                            || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified))
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification;
                result = MessageBox.Show("Submitting project for certification approval?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else if (project.WorkConcepts.Any(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved))
            {
                project.UserAction = StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection;
                result = MessageBox.Show("Submitting project for rejection approval?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                savedTransaction = true;
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserDbIds.Add(userAccount.UserDbId);
                dataServ.CertifyProject(project, project.UserAction);
                workConceptsAdded = project.WorkConcepts;
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                formMapping.EditProject(project, "Update");
                //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                this.Close();
                //UpdateProjectInfo(project);
                //tabControlProject.SelectedTab = tabPageProjectInfo;

                /*
                string subject = String.Format("Str Proj {0}- {1} by {2}", project.ProjectDbId, project.UserAction, project.UserFullName);

                if (!dataServ.GetApplicationMode().Equals("PROD"))
                {
                    subject += String.Format(" in {0} Environment", dataServ.GetApplicationMode());
                }

                string message = String.Format("Str Proj {0} in {1}", project.ProjectDbId, project.FiscalYear);
                message += String.Format("</br>Project Status: In Certification");
                message += String.Format("</br>Action: {0}", project.UserAction + " by " + project.UserFullName + " on " + project.UserActionDateTime);
                string[] to = new string[] { "laura.shadewald@dot.wi.gov", "aaron.bonk@dot.wi.gov" };

                if (dataServ.GetApplicationMode().Equals("DEV"))
                {
                    to = new string[] { "joseph.barut@dot.wi.gov" };
                }

                message += String.Format("</br></br># of Work Concepts: ", project.WorkConcepts.Count);

                foreach (var wc in project.WorkConcepts)
                {
                    message += String.Format("</br>{0}, ({1}){2}, {3}", wc.StructureId, wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription, wc.Status.ToString());
                }

                Email.ComposeMessage(subject, message, to, null, null, new string[] { "joseph.barut@dot.wi.gov" });
                this.Close();*/
            }
            else
            {
                return;
            }
        }

        internal void UpdateCertification(StructuresProgramType.ProjectUserAction userAction)
        {
            DialogResult result = DialogResult.No; 

            if (userAction == StructuresProgramType.ProjectUserAction.BosCertified)
            {
                result = MessageBox.Show("Approving project for full certification?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else if (userAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification)
            {
                result = MessageBox.Show("Rejecting project for full certification?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else if (userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
            {
                //result = MessageBox.Show("Transitionally certifying project?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                result = DialogResult.Yes;
            }
            else if (userAction == StructuresProgramType.ProjectUserAction.RequestRecertification)
            {
                result = MessageBox.Show("Requesting project recertification?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else if (userAction == StructuresProgramType.ProjectUserAction.GrantRecertification)
            {
                result = MessageBox.Show("Granting project recertification request?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else if (userAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
            {
                result = MessageBox.Show("Rejecting project recertification request?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }

            if (result == DialogResult.Yes)
            {
                savedTransaction = true;
                project.Locked = false;
                project.InCertification = false;
                project.InPrecertification = false;
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserAction = userAction;

                if (userAction == StructuresProgramType.ProjectUserAction.BosTransitionallyCertified)
                {
                    foreach (WorkConcept wc in project.WorkConcepts)
                    {
                        wc.Status = StructuresProgramType.WorkConceptStatus.Certified;
                    }
                }

                dataServ.CertifyProject(project, userAction);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                formMapping.EditProject(project, "Update");
                this.Close();
            }
            else
            {
                return;
            }
        }

        private void buttonApproveCertification_Click(object sender, EventArgs e)
        {
            UpdateCertification(StructuresProgramType.ProjectUserAction.BosCertified);
            /*
            DialogResult result = MessageBox.Show("Approving project for full certification?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                project.Locked = false;
                project.InCertification = false;
                project.UserAction = StructuresProgramType.ProjectUserAction.BosCertified;
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserDbIds.Add(userAccount.UserDbId);
                dataServ.CertifyProject(project, StructuresProgramType.ProjectUserAction.BosCertified);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"));
                formMapping.EditProject(project, "Update");
                this.Close();
            }
            else
            {
                return;
            }*/
        }

        private void linkLabelStructureCosts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://wisconsindot.gov/Pages/doing-bus/eng-consultants/cnslt-rsrces/strct/struc-costs.aspx");
        }

        private void linkLabelSimilarProjectsTool_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"\\mad00fph\n4public\BPD\Estimating User Group\Similar Projects\similar-projects-links.xlsm");
        }

        private void linkLabelBridgeManualBridgeRehabilitation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://wisconsindot.gov/dtsdManuals/strct/manuals/bridge/ch40.pdf");
        }

        private void linkLabelBridgeManualBridgePreservation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://wisconsindot.gov/dtsdManuals/strct/manuals/bridge/ch42.pdf");
        }

        private void linkLabelBridgeManualAssetManagement_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://wisconsindot.gov/dtsdManuals/strct/manuals/bridge/ch41.pdf");
        }

        private void linkLabelStructureDesignHoursWorksheet_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"https://wigov.sharepoint.com/sites/dot-dtsd/mydtsd/tpms/documents/ScopWDotStructdgnhrsapr2013updt.pdf");
        }

        private void buttonUnlockProject_Click(object sender, EventArgs e)
        {}

        private void buttonOpenInspection_Click(object sender, EventArgs e)
        {
            //buttonOpenInspection.Enabled = false;
            //pictureBoxInspection.Visible = true;
            //await Task.Run(() => GetMonitoringReport(startFy, endFy, regions, includeState, includeLocal));
            //Dw.Inspection lastInspection = await Task.Run(() => warehouseDatabase.GetLastInspection(labelStructureId.Text));
            bool isLoaded = LoadInspection(labelLastInspectionFilePath.Text);
            //pictureBoxInspection.Visible = false;
            //buttonOpenInspection.Enabled = true;
        }

        private void OpenInspection(string inspectionFilePath)
        {
            bool isLoaded = LoadInspection(inspectionFilePath);
        }

        private void buttonDenyStructureCertification_Click(object sender, EventArgs e)
        {
            savedTransaction = true;
            UpdateProjectElementWorkConcept(sender, e, "reject");
        }

        private void buttonRejectCertification_Click(object sender, EventArgs e)
        {
            UpdateCertification(StructuresProgramType.ProjectUserAction.BosRejectedCertification);
            /*
            DialogResult result = MessageBox.Show("Rejecting project for full certification?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                project.Locked = false;
                project.InCertification = false;
                project.UserAction = StructuresProgramType.ProjectUserAction.BosRejectedCertification;
                dataServ.CertifyProject(project, StructuresProgramType.ProjectUserAction.BosRejectedCertification);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                string subject = String.Format("Str Proj {0}- {1} by {2}", project.ProjectDbId, project.UserAction, project.UserFullName);

                if (!dataServ.GetApplicationMode().Equals("PROD"))
                {
                    subject += String.Format(" in {0} Environment", dataServ.GetApplicationMode());
                }

                string message = String.Format("Str Proj {0} in {1}", project.ProjectDbId, project.FiscalYear);
                message += String.Format("</br>Project Status: {0}", project.Status.ToString());
                message += String.Format("</br>Action: {0}", project.UserAction + " by " + project.UserFullName + " on " + project.UserActionDateTime);
                string[] to = new string[] { "laura.shadewald@dot.wi.gov", "aaron.bonk@dot.wi.gov" };

                if (dataServ.GetApplicationMode().Equals("DEV"))
                {
                    to = new string[] { "joseph.barut@dot.wi.gov" };
                }

                message += String.Format("</br></br># of Work Concepts: ", project.WorkConcepts.Count);

                foreach (var wc in project.WorkConcepts)
                {
                    message += String.Format("</br>{0}, ({1}){2}, {3}", wc.StructureId, wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription, wc.Status.ToString());
                }

                Email.ComposeMessage(subject, message, to, null, null, new string[] { "joseph.barut@dot.wi.gov" });
                formMapping.EditProject(project, "Update");
                this.Close();
            }
            else
            {
                return;
            }*/
        }

        private void buttonTransitionallyCertify_Click(object sender, EventArgs e)
        {
            /*
            foreach (var wc in project.WorkConcepts)
            {
                wc.Status = StructuresProgramType.WorkConceptStatus.Certified;
            }*/

            bool missingFiipsProject = String.IsNullOrEmpty(project.FosProjectId) ? true : false;
            DialogResult result = DialogResult.Yes;
            DialogResult result2 = DialogResult.Yes;

            if (missingFiipsProject)
            {
                result2 = MessageBox.Show("This structures project is missing a corresponding FIIPS project. Notify the region about it?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result2 == DialogResult.Yes)
                {
                    //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ, true, "precertification");
                    EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ, true, "precertification");
                }

                result = MessageBox.Show("Proceed with transitionally certifying this project?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            UpdateCertification(StructuresProgramType.ProjectUserAction.BosTransitionallyCertified);
        }

        private void buttonGenerateSecondaryWorkConceptsTable_Click(object sender, EventArgs e)
        {
            GenerateSecondaryWorkConceptsTable(currentWorkConcept.StructureId);
        }

        private void buttonDeleteSecondaryWorkConceptsTable_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(String.Format("Are you sure you want to delete the table?"), "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                dgvSecondaryElements.Rows.Clear();
            }
        }

        private void buttonRequestRecertification_Click(object sender, EventArgs e)
        {
            OpenRequestRecertification(project, "request");
            UpdateProjectInfo(project);
            //UpdateCertification(StructuresProgramType.ProjectUserAction.RequestRecertification);
        }

        private void OpenRequestRecertification(Project project, string action)
        {
            /*
            if (formRequestRecertification != null)
            {
                formRequestRecertification.Close();
            }*/
            
            formRequestRecertification = new FormRequestRecertification(project, this, (DatabaseService)dataServ, userAccount, action);
            formRequestRecertification.StartPosition = FormStartPosition.CenterParent;
            formRequestRecertification.ShowDialog();
            /*
            formProposeWorkConcept = new FormProposeWorkConcept(workConcept, primaryWorkConcepts, secondaryWorkConcepts, justifications, formMainController);
            formProposeWorkConcept.StartPosition = FormStartPosition.Manual;
            formProposeWorkConcept.Location = new Point(gMapControlStructuresMap.Width / 2, gMapControlStructuresMap.Height / 2);
            formProposeWorkConcept.Show();*/
        }

        private void buttonGrantRecertification_Click(object sender, EventArgs e)
        {
            //UpdateCertification(StructuresProgramType.ProjectUserAction.GrantRecertification);
        }

        private void buttonRejectRecertification_Click(object sender, EventArgs e)
        {

        }

        private void buttonGrantRecertification_Click_1(object sender, EventArgs e)
        {
            OpenRequestRecertification(project, "grant");
            UpdateProjectInfo(project);
        }

        private void buttonRejectRecertification_Click_1(object sender, EventArgs e)
        {
            OpenRequestRecertification(project, "reject");
            UpdateProjectInfo(project);
        }

        private void FormStructureProject_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!userAccount.IsRegionalRead && !userAccount.IsSuperRead && !savedTransaction && isDirty)
            {
                DialogResult result = MessageBox.Show("Save changes?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    project.Status = originalProjectStatus;
                }
            }
        }

        private void textBoxStructureProjectDescription_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!project.Description.Equals(textBoxStructureProjectDescription.Text.Trim()))
                {
                    changesMade = true;
                }
            }
            catch { }
        }

        private void textBoxStructureProjectNotes_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!project.Notes.Equals(textBoxStructureProjectNotes.Text.Trim()))
                {
                    changesMade = true;
                }
            }
            catch { }
        }

        private void textBoxStructureProjectCost_TextChanged(object sender, EventArgs e)
        {
            if (!project.StructuresCost.ToString().Equals(textBoxStructureProjectCost.Text.Trim().Replace(",", "")))
            {
                changesMade = true;
            }
        }

        private void textBoxNotificationRecipients_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!project.NotificationRecipients.Equals(textBoxNotificationRecipients.Text.Trim()))
                {
                    changesMade = true;
                }
            }
            catch { }
        }

        public static void ShowFormStructureProject(FormStructureProject formStructureProject, FormWindowState windowState = FormWindowState.Maximized)
        {
            formStructureProject.TopMost = false;
            formStructureProject.StartPosition = FormStartPosition.CenterScreen;
            formStructureProject.WindowState = FormWindowState.Normal;
            formStructureProject.Show();
        }

        private void buttonSaveProjectDescription_Click(object sender, EventArgs e)
        {
            try
            {
                project.StructuresCost = Convert.ToInt32(textBoxStructureProjectCost.Text.Trim().Replace(",", ""));
                textBoxStructureProjectCost.Text = String.Format("{0:n0}", project.StructuresCost);
            }
            catch { }

            try
            {
                project.AcceptablePseDateStart = Convert.ToDateTime(textBoxAcceptablePseDateStart.Text.Trim());
            }
            catch
            {
                if (project.AdvanceableFiscalYear != 0)
                {
                    project.AcceptablePseDateStart = dataServ.CalculateAcceptablePseDateStart(project.AdvanceableFiscalYear);
                }
                else
                {
                    project.AcceptablePseDateStart = dataServ.CalculateAcceptablePseDateStart(project.FiscalYear);
                }
            }

            try
            {
                project.AcceptablePseDateEnd = Convert.ToDateTime(textBoxAcceptablePseDateEnd.Text.Trim());
            }
            catch
            {
                project.AcceptablePseDateEnd = dataServ.CalculateAcceptablePseDateEnd(project.FiscalYear);
            }

            project.Description = textBoxStructureProjectDescription.Text.Trim();
            project.Notes = textBoxStructureProjectNotes.Text.Trim();
            project.NotificationRecipients = textBoxNotificationRecipients.Text.Trim();
            project.FosProjectId = textBoxStructureProjectFosProjectId.Text.Trim();
            project.UserDbIds.Add(userAccount.UserDbId);
            dataServ.UpdateProjectWhileInPrecertificationOrCertification(project);
            savedTransaction = true;
        }

        private void comboBoxInspectionDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DateTime inspectionDate = Convert.ToDateTime(comboBox.SelectedItem);
            inspection = warehouseDatabase.GetInspection(linkLabelStructureId.Text, inspectionDate);
            textBoxInspectionType.Text = "";

            if (inspection != null)
            {
                int counter = 0;

                foreach (var inspType in inspection.InspectionTypes)
                {
                    if (counter == 0)
                    {
                        textBoxInspectionType.Text = inspType.InspectionTypeDescription;
                    }
                    else
                    {
                        textBoxInspectionType.Text += ", " + inspType.InspectionTypeDescription;
                    }

                    counter++;
                }

                /*
                if (inspection.IsLastInspection)
                {
                    textBoxLastInspectionDate.Text = inspection.InspectionDate.ToString("MM/dd/yyyy");
                    textBoxSuperstructureRating.Text = inspection.SuperstructureRating;
                    textBoxSubstructureRating.Text = inspection.SubstructureRating;
                    textBoxDeckRating.Text = inspection.DeckRating;
                    textBoxCulvertRating.Text = inspection.CulvertRating;
                    linkLabelLastInspection.Text = inspection.InspectionDate.ToString("yyyy/MM/dd");
                    string lastInspectionFilePath = inspection.InspectionFilePath;
                    textBoxLastInspectionPath.Text = lastInspectionFilePath;

                    if (File.Exists(inspection.CoverPhotoFilePath))
                    {
                        pictureBoxCoverPhoto.Load(inspection.CoverPhotoFilePath);
                    }
                }*/

                if (System.IO.File.Exists(inspection.InspectionFilePath))
                {
                    try
                    {
                        axAcroPDF1.LoadFile(inspection.InspectionFilePath);
                        axAcroPDF1.setShowToolbar(true);
                        axAcroPDF1.setView("FitH");
                        axAcroPDF1.setZoom(100);
                        //axAcroPDF1.
                        //axAcroPDF1.gotoFirstPage();
                        axAcroPDF1.Visible = true;
                        //labelLastInspectionDate.Text = "Last Inspection: " + lastInspectionDate;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(String.Format("Unable to load the {0} inspection.", inspectionDate.ToString("yyyy/MM/dd")), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        axAcroPDF1.Visible = false;
                    }
                }
                else
                {
                    MessageBox.Show(String.Format("Unable to load the {0} inspection.", inspectionDate.ToString("yyyy/MM/dd")), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    axAcroPDF1.Visible = false;
                }
            }
            else
            {
                MessageBox.Show(String.Format("Unable to load the {0} inspection.", inspectionDate.ToString("yyyy/MM/dd")), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                axAcroPDF1.Visible = false;
            }
        }

        private void dataGridViewProjectWorkConcepts_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.isDirty = true;
        }

        private void textBoxAcceptablePseDateStart_Validating(object sender, CancelEventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxAcceptablePseDateStart.Text.Trim()) && !isDateValid(textBoxAcceptablePseDateStart.Text.Trim()))
            {
                MessageBox.Show("Enter a date in the required format.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private bool isDateValid(string date)
        {
            return controller.isDateValid(date);
        }

        private void textBoxAcceptablePseDateEnd_Validating(object sender, CancelEventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxAcceptablePseDateEnd.Text.Trim()) && !isDateValid(textBoxAcceptablePseDateEnd.Text.Trim()))
            {
                MessageBox.Show("Enter a date in the required format.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void groupBoxProjectStatus_Enter(object sender, EventArgs e)
        {

        }
    }
}
