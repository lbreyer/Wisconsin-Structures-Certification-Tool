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
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using System.IO;
using Dw = Wisdot.Bos.Dw;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Infrastructure;

namespace WiSam.StructuresProgram
{
    public partial class FormStructure : Form
    {
        private UserAccount userAccount;
        //private WiSamEntities.Structure structure;
        //private string baseUrl = "https://www.google.com/maps/search/?api=1&query=";
        //private string structureUrl = "";
        private string otherBaseUrl = "http://maps.google.com/maps?q=";
        private string otherStructureUrl = "";
        //private float latitude;
        //private float longitude;
        private string structureId;
        //private string workConceptCode;
        //private string workConceptDescription;
        //private int fiscalYear;
        //private int projectYear;
        //private List<WorkConcept> workConcepts = new List<WorkConcept>();
        private List<WorkConcept> eligibleProposedWorkConceptsForGivenStructure = new List<WorkConcept>();
        private List<WorkConcept> eligibleWorkConcepts;
        private List<WorkConcept> workConceptsInStructuresProjectsForGivenStructure = new List<WorkConcept>();
        private List<Project> structuresProjectsForGivenStructure = new List<Project>();
        //private List<WorkConcept> selectedWorkConcepts;
        //private List<WorkConcept> quasicertifiedWorkConcepts;
        //private List<WorkConcept> precertifiedApprovedWorkConcepts;
        //private List<WorkConcept> precertifiedUnapprovedWorkConcepts;
        //private List<WorkConcept> certifiedWorkConcepts;
        private List<WorkConcept> fiipsWorkConcepts;
        private List<Project> structureProjects;
        private List<Project> fiipsProjects;
        private List<WorkConcept> primaryWorkConcepts;
        private List<WorkConcept> allWorkConcepts;
        private FormMapping formMapping;
        private Dw.Database warehouseDatabase = new Dw.Database();
        private List<int> plansFolders = new List<int>();
        private List<DateTime> inspectionDates = new List<DateTime>();
        private Dw.Inspection inspection;
        private List<Dw.Structure> structures = new List<Dw.Structure>();
        IStructureService structServ = new StructureService();
        IDatabaseService dataServ = new DatabaseService();

        public FormStructure(UserAccount userAccount, DatabaseService dataServ, string structureId, FormMapping formMapping)
        {
            InitializeComponent();
            this.userAccount = userAccount;
            this.dataServ = dataServ;
            this.structureId = structureId;
            this.formMapping = formMapping;
            eligibleWorkConcepts = dataServ.GetEligibleWorkConcepts();
            //eligibleWorkConcepts = ewcs;
            fiipsWorkConcepts = dataServ.GetFiipsWorkConcepts();
            structureProjects = dataServ.GetProjectsInSct();
            fiipsProjects = dataServ.GetProjectsInFiips();
            PopulateForm();
        }

        private void ClearFields()
        {
            labelStructureId.Text = "";
            labelFeatureOnUnder.Text = "";
            textBoxRegion.Text = "";
            textBoxCounty.Text = "";
            textBoxFeatureOn.Text = "";
            textBoxFeatureUnder.Text = "";
            textBoxStructureType.Text = "";
            textBoxLatitude.Text = "";
            textBoxLongitude.Text = "";
            textBoxNumSpans.Text = "";
            textBoxTotalLengthSpans.Text = "";
            textBoxOwner.Text = "";
            textBoxMunicipality.Text = "";
            textBoxConstructionHistory.Clear();
            textBoxLastInspectionPath.Text = "";
            linkLabelLastInspection.Text = "";
            otherStructureUrl = "";
            pictureBoxCoverPhoto.Image = null;
            comboBoxPlansYear.Items.Clear();
            comboBoxInspectionDate.Items.Clear();
            textBoxInspectionType.Text = "";
        }
        
        private void PopulatePlansYear()
        {
            plansFolders = warehouseDatabase.GetPlansFolders(structureId);

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
        }

        private void PopulateInspectionDate()
        {
            inspectionDates = warehouseDatabase.GetInspectionDates(structureId);

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

        private void PopulateForm()
        {
            ClearFields();
            RenderWorkConceptsGrid();
            RenderSctHistoryGrid();
            this.Text = "Structure: " + warehouseDatabase.FormatStructureId (structureId);
            labelStructureId.Text = structureId;
            PopulatePlansYear();
            PopulateInspectionDate();
            Dw.Structure dwStructure = null;

            if (structures.Where(s => s.StructureId.Equals(structureId)).Count() > 0)
            {
                dwStructure = structures.Where(s => s.StructureId.Equals(structureId)).First();
            }
            else
            {
                dwStructure = warehouseDatabase.GetStructure(structureId, false, true);
                structures.Add(dwStructure);
            }

            if (dwStructure != null)
            {
                string featureOnUnder = dwStructure.ServiceFeatureOn;

                if (!String.IsNullOrEmpty(dwStructure.ServiceFeatureUnder))
                {
                    featureOnUnder += " over " + dwStructure.ServiceFeatureUnder;
                }

                labelFeatureOnUnder.Text = featureOnUnder;
                textBoxRegion.Text = dwStructure.GeoLocation.Region;
                textBoxCounty.Text = dwStructure.GeoLocation.County;
                textBoxFeatureOn.Text = dwStructure.ServiceFeatureOn;
                textBoxFeatureUnder.Text = dwStructure.ServiceFeatureUnder;
                textBoxStructureType.Text = dwStructure.StructureType;
                textBoxTotalLengthSpans.Text = dwStructure.StructureLength > 0 ? dwStructure.StructureLength.ToString() : "";
                textBoxLatitude.Text = dwStructure.GeoLocation.StartLatitude.ToString();
                textBoxLongitude.Text = dwStructure.GeoLocation.StartLongitude.ToString();
                otherStructureUrl = String.Format("{0}{1},{2}", otherBaseUrl, dwStructure.GeoLocation.StartLatitude.ToString(), dwStructure.GeoLocation.StartLongitude.ToString());

                if (gMapControlSingleStructure.Overlays.Count() > 0)
                {
                    gMapControlSingleStructure.Overlays.RemoveAt(0);
                }

                gMapControlSingleStructure.ShowCenter = false;
                gMapControlSingleStructure.Position = new GMap.NET.PointLatLng(dwStructure.GeoLocation.StartLatitude, dwStructure.GeoLocation.StartLongitude);
                GMapOverlay markers = new GMapOverlay("markers");
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(dwStructure.GeoLocation.StartLatitude, dwStructure.GeoLocation.StartLongitude), GMarkerGoogleType.black_small);
                marker.ToolTipText = String.Format("\r\n{0}", warehouseDatabase.FormatStructureId(structureId));
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTip.TextPadding = new Size(10, 10);
                marker.ToolTip.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                markers.Markers.Add(marker);
                gMapControlSingleStructure.Overlays.Add(markers);

                if (dwStructure is Dw.Bridge)
                {
                    Dw.Bridge bridge = (Dw.Bridge)dwStructure;
                    textBoxNumSpans.Text = bridge.NumberOfSpans.ToString();
                    Dw.Span mainSpan = null;

                    if (bridge.Spans.Where(span => span.MainSpan).Count() > 0)
                    {
                        mainSpan = bridge.Spans.Where(span => span.MainSpan).First();
                    }

                    if (mainSpan != null)
                    {
                        textBoxStructureType.Text = mainSpan.ConfigurationType;
                    }
                }
                else if (dwStructure is Dw.Culvert)
                {
                    Dw.Culvert bridge = (Dw.Culvert)dwStructure;
                    textBoxNumSpans.Text = bridge.NumberOfSpans.ToString();
                    Dw.Span mainSpan = null;

                    if (bridge.Spans.Where(span => span.MainSpan).Count() > 0)
                    {
                        mainSpan = bridge.Spans.Where(span => span.MainSpan).First();
                    }

                    if (mainSpan != null)
                    {
                        textBoxStructureType.Text = mainSpan.ConfigurationType;
                    }
                }

                textBoxOwner.Text = dwStructure.OwnerAgency;
                textBoxMunicipality.Text = dwStructure.GeoLocation.Municipality;
                textBoxConstructionHistory.Text = dwStructure.ConstructionHistory;
            }
            else
            {
                MessageBox.Show("Unable to retrieve data for " + structureId + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private string StructureIdToFolderName(string structureId)
        {
            return structServ.StructureIdToFolderName(structureId);
        }

        public static void ShowFormStructure(FormStructure formStructure, FormWindowState windowState = FormWindowState.Maximized)
        {
            formStructure.TopMost = false;
            formStructure.StartPosition = FormStartPosition.CenterScreen;
            formStructure.WindowState = FormWindowState.Normal;
            formStructure.Show();
        }

        private void RenderSctHistoryGrid()
        {
            DataGridView dgv = dataGridViewSctHistory;
            dgv.Rows.Clear();
            int rowCount = 0;
            List<Project> deletedProjects = dataServ.GetDeletedProjectsForStructure(structureId);
            List<Project> deletedWorkConcepts = dataServ.GetDeletedWorkConceptsForStructure(structureId);

            if (deletedProjects.Count > 0)
            {
                foreach (var project in deletedProjects)
                {
                    if (structuresProjectsForGivenStructure.Where(p => p.ProjectDbId == project.ProjectDbId).Count() == 0)
                    {
                        structuresProjectsForGivenStructure.Add(project);
                    }
                }
            }

            if (deletedWorkConcepts.Count > 0)
            {
                foreach (var project in deletedWorkConcepts)
                {
                    if (structuresProjectsForGivenStructure.Where(p => p.ProjectDbId == project.ProjectDbId).Count() == 0)
                    {
                        structuresProjectsForGivenStructure.Add(project);
                    }
                }
            }

            foreach (var project in structuresProjectsForGivenStructure)
            {
                dgv.Rows.Add();
                dgv.Rows[rowCount].Cells["SctProjectId"].Value = project.ProjectDbId;
                dgv.Rows[rowCount].Cells["LastTransactionDate"].Value = project.UserActionDateTime.ToString("MM/dd/yyyy");
                string lastStatus = dataServ.GetWorkflowTransaction(project.UserAction);

                /*
                if (project.UserAction == StructuresProgramType.ProjectUserAction.DeletedWorkConcept)
                {
                    lastStatus = StructuresProgramType.ProjectUserAction.DeletedWorkConcept.ToString();
                }
                else if (project.Status == StructuresProgramType.ProjectStatus.Deleted || project.UserAction == StructuresProgramType.ProjectUserAction.Deactivated)
                {
                    lastStatus = StructuresProgramType.ProjectUserAction.DeletedProject.ToString();
                }
                else
                {
                    lastStatus = project.UserAction.ToString();
                }*/

                dgv.Rows[rowCount].Cells["LastTransaction"].Value = lastStatus;
                WorkConcept wc = null;

                if (project.Status == StructuresProgramType.ProjectStatus.Deleted || project.UserAction == StructuresProgramType.ProjectUserAction.DeletedWorkConcept)
                {
                    if (project.WorkConcepts.Count > 0)
                    {
                        wc = project.WorkConcepts.First();
                    }
                }
                else
                {
                    if (workConceptsInStructuresProjectsForGivenStructure.Where(w => w.ProjectDbId == project.ProjectDbId).Count() > 0)
                    {
                        wc = workConceptsInStructuresProjectsForGivenStructure.Where(w => w.ProjectDbId == project.ProjectDbId).First();
                    }
                }

                if (wc == null)
                {
                    continue;
                }

                dgv.Rows[rowCount].Cells["EligibleWorkConcept"].Value = wc.WorkConceptDescription.ToUpper();
                dgv.Rows[rowCount].Cells["EligibleYear"].Value = wc.FiscalYear;
                dgv.Rows[rowCount].Cells["WorkConceptToBeCertified"].Value = wc.CertifiedWorkConceptDescription.ToUpper();
                dgv.Rows[rowCount].Cells["ProjectYear"].Value = project.FiscalYear;
                dgv.Rows[rowCount].Cells["AdvanceableProjectYear"].Value = project.AdvanceableFiscalYear != 0 ? project.AdvanceableFiscalYear.ToString() : "";
                dgv.Rows[rowCount].Cells["ChangeJustification"].Value = wc.ChangeJustifications;
                var regionNotes = wc.ChangeJustificationNotes;
                if (String.IsNullOrEmpty(wc.ChangeJustificationNotes))
                {
                    regionNotes = dataServ.GetRegionNotes(wc);
                }
                
                dgv.Rows[rowCount].Cells["RegionNotes"].Value = regionNotes;
                string precertificationLiaison = wc.ProjectPrecertificationLiaisonUserFullName;
                string precertificationDecision = wc.PrecertificationDecision.ToString();
                string precertificationReasonCategory = wc.PrecertificationDecisionReasonCategory;
                string precertificationDecisionReasonExplanation = wc.PrecertificationDecisionReasonExplanation;
                WorkConcept precertificationWorkConcept = dataServ.GetStructurePrecertification(wc);

                if (precertificationWorkConcept != null)
                {
                    dgv.Rows[rowCount].Cells["PrecertificationLiaison"].Value = precertificationWorkConcept.ProjectPrecertificationLiaisonUserFullName;
                    dgv.Rows[rowCount].Cells["PrecertificationDecision"].Value = precertificationWorkConcept.PrecertificationDecision == StructuresProgramType.PrecertificatioReviewDecision.None ? "" : precertificationWorkConcept.PrecertificationDecision.ToString();
                    dgv.Rows[rowCount].Cells["PrecertificationReasonCategory"].Value = precertificationWorkConcept.PrecertificationDecisionReasonCategory;
                    dgv.Rows[rowCount].Cells["PrecertificationReasonExplanation"].Value = precertificationWorkConcept.PrecertificationDecisionReasonExplanation;
                }

                WorkConcept certificationWorkConcept = dataServ.GetStructureCertification(wc);

                if (certificationWorkConcept != null)
                {
                    dgv.Rows[rowCount].Cells["CertificationLiaison"].Value = certificationWorkConcept.ProjectCertificationLiaisonUserFullName;
                    dgv.Rows[rowCount].Cells["CertificationPrimaryWorkConceptComments"].Value = certificationWorkConcept.CertificationPrimaryWorkTypeComments;
                    dgv.Rows[rowCount].Cells["CertificationSecondaryWorkConceptsComments"].Value = certificationWorkConcept.CertificationSecondaryWorkTypeComments;
                    dgv.Rows[rowCount].Cells["BosCdComments"].Value = certificationWorkConcept.CertificationAdditionalComments;
                }

                Project recertProject = dataServ.GetProjectRecertification(project.ProjectDbId);

                if (recertProject != null)
                {
                    dgv.Rows[rowCount].Cells["ReasonForRecertificationRequest"].Value = recertProject.RecertificationReason;
                    dgv.Rows[rowCount].Cells["BosRecertificationDecision"].Value = recertProject.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification ||
                        recertProject.UserAction == StructuresProgramType.ProjectUserAction.GrantRecertification ? recertProject.UserAction.ToString() : "";
                    dgv.Rows[rowCount].Cells["BosRecertificationComments"].Value = recertProject.RecertificationComments;
                }

                /*
                dgv.Rows[rowCount].Cells["ReasonForRecertificationRequest"].Value = project.RecertificationReason;
                dgv.Rows[rowCount].Cells["BosRecertificationDecision"].Value = project.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification || 
                    project.UserAction == StructuresProgramType.ProjectUserAction.GrantRecertification ? project.UserAction.ToString() : "";
                dgv.Rows[rowCount].Cells["BosRecertificationComments"].Value = project.RecertificationComments;*/
                dgv.Rows[rowCount].Cells["FiipsConstructionId"].Value = project.FosProjectId;

                if (!String.IsNullOrEmpty(project.FosProjectId))
                {
                    Project fiipsProject = null;
                    
                    if (fiipsProjects.Where(fp => fp.FosProjectId.Equals(project.FosProjectId)).Count() > 0)
                    {
                        fiipsProject = fiipsProjects.Where(fp => fp.FosProjectId.Equals(project.FosProjectId)).First();
                    }

                    if (fiipsProject != null)
                    {
                        dgv.Rows[rowCount].Cells["FiipsConcept"].Value = fiipsProject.FiipsImprovementConcept;
                        dgv.Rows[rowCount].Cells["Pse"].Value = fiipsProject.PseDate.Year != 1 ? fiipsProject.PseDate.ToString("MM/dd/yyyy") : "";
                        dgv.Rows[rowCount].Cells["Epse"].Value = fiipsProject.EpseDate.Year != 1 ? fiipsProject.EpseDate.ToString("MM/dd/yyyy") : "";
                    }
                    else
                    {
                        Dw.FiipsProject tempFiipsProject = warehouseDatabase.GetFiipsProject(project.FosProjectId);

                        if (tempFiipsProject != null)
                        {
                            dgv.Rows[rowCount].Cells["FiipsConcept"].Value = tempFiipsProject.PlanningProjectConceptCode;
                            dgv.Rows[rowCount].Cells["Pse"].Value = tempFiipsProject.PseDate.Year != 1 ? tempFiipsProject.PseDate.ToString("MM/dd/yyyy") : "";
                            dgv.Rows[rowCount].Cells["Epse"].Value = tempFiipsProject.EarliestPseDate.Year != 1 ? tempFiipsProject.EarliestPseDate.ToString("MM/dd/yyyy") : "";
                        }
                    }
                }

                rowCount++;
            }

            foreach (Project project in deletedProjects)
            {
                structuresProjectsForGivenStructure.Remove(project);
            }

            foreach (Project project in deletedWorkConcepts)
            {
                structuresProjectsForGivenStructure.Remove(project);
            }
        }

        private void RenderWorkConceptsGrid()
        {
            DataGridView dgvEligible = dataGridViewStructureEligibleWorkConcepts;
            dgvEligible.Rows.Clear();
            DataGridView dgvStructuresProjects = dataGridViewStructuresProjects;
            dgvStructuresProjects.Rows.Clear();
            DataGridView dgvFiipsProjects = dataGridViewFiipsProjects;
            dgvFiipsProjects.Rows.Clear();
            dgvEligible.Columns["AddtoProj"].Visible = false;
            dgvEligible.Columns["dgvcDeleteProposedWorkConcept"].Visible = false;

            if (userAccount.IsAdministrator || userAccount.IsRegionalProgrammer || userAccount.IsSuperUser)
            {
                dgvEligible.Columns["AddtoProj"].Visible = true;
            }


            //List<int> eligibleWorkConceptIds = new List<int>();
            var eligibles = dataServ.GetEligibleWorkConcepts().Where(wc => wc.StructureId.Equals(structureId))
                            .GroupBy(g => new { g.WorkConceptCode, g.FiscalYear }).Select(g => g.First()).ToList();
            eligibleProposedWorkConceptsForGivenStructure = eligibles;
            //foreach (var wc in workConcepts.Where(wc => (wc.FromEligibilityList || wc.Evaluate || wc.FromProposedList) && wc.ProjectDbId == 0))
            foreach (var wc in eligibles)
            {
                //if (!eligibleWorkConceptIds.Contains(wc.WorkConceptDbId))
                {
                    //eligibleWorkConceptIds.Add(wc.WorkConceptDbId);
                    dgvEligible.Rows.Add();
                    dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Empty;
                    dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;

                    if (wc.WorkConceptCode.Equals("PR"))
                    {
                        dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[1].Value = wc.CertifiedWorkConceptDescription;

                        if (userAccount.IsAdministrator || userAccount.IsRegionalProgrammer || userAccount.IsSuperUser)
                        {
                            dgvEligible.Columns["dgvcDeleteProposedWorkConcept"].Visible = true;
                        }
                    }
                    else
                    {
                        dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[1].Value = wc.WorkConceptDescription;
                    }

                    dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[2].Value = wc.WorkConceptDbId;
                    dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
                    dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[4].Value = wc.ProjectDbId;

                    if (wc.WorkConceptCode.Equals("PR"))
                    {
                        dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].Cells[5].Value = String.Format("Reason Cat: {0}, Notes: {1}", wc.ReasonCategory, wc.Notes);
                        dgvEligible.Rows[dgvEligible.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.DarkOrange;
                    }
                }
            }

            try
            {
                dgvEligible.CurrentCell.Selected = false;
            }
            catch { }

            if (dgvEligible.Rows.Count == 0)
            {
                labelWorkConcepts.Text = String.Format("Work Concepts{0}{1}{2}{3}", Environment.NewLine, "This structure has no eligible or proposed work concepts", Environment.NewLine, "in the current 11-year window.");
                panelWorkConcepts.Height = 58;
            }
            else
            {
                labelWorkConcepts.Text = "Work Concepts";
                panelWorkConcepts.Height = 24;
            }

            var projects = dataServ.GetProjectsInSct().Where(p => p.UserAction != StructuresProgramType.ProjectUserAction.Deactivated && p.WorkConcepts.Any(wc => wc.StructureId.Equals(structureId)))
                            .GroupBy(g => g.ProjectDbId).Select(g => g.First()).ToList();
            structuresProjectsForGivenStructure = projects;
            var workConceptsInStructuresProjects = new List<WorkConcept>();
            workConceptsInStructuresProjectsForGivenStructure = workConceptsInStructuresProjects;

            foreach (var project in projects)
            {
                try
                {
                    workConceptsInStructuresProjects.AddRange(project.WorkConcepts.Where(wc => wc.StructureId.Equals(structureId)));
                }
                catch { }
            }
            //foreach (var wc in workConcepts.Where(wc => wc.Status != StructuresProgramType.WorkConceptStatus.Fiips
                                                    //&& wc.ProjectDbId != 0))
            foreach (var wc in workConceptsInStructuresProjects)
            {
                dgvStructuresProjects.Rows.Add();
                dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;

                if (wc.FromProposedList)
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[1].Value = wc.CertifiedWorkConceptDescription;
                }
                else
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[1].Value = wc.CertifiedWorkConceptDescription;
                }

                dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[2].Value = wc.WorkConceptDbId;
                //dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
                dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[3].Value = wc.StructureProjectFiscalYear;
                dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[4].Value = wc.ProjectDbId;

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Red;
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified)
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Yellow;
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified)
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[5].Value = "Transitionally Certified";
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightGreen;
                }
                else if (wc.Evaluate)
                {
                    dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Yellow;

                    if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                    {
                        dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Red;
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                    {
                        dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].Cells[5].Value = "Transitionally Certified";
                        dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified)
                    {
                        dgvStructuresProjects.Rows[dgvStructuresProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightGreen;
                    }
                }
            }

            try
            {
                dgvStructuresProjects.CurrentCell.Selected = false;
            }
            catch { }

            if (dgvStructuresProjects.Rows.Count == 0)
            {
                labelStructuresProjects.Text = String.Format("Structures Projects{0}{1}{2}{3}", Environment.NewLine, "This structure has no certified work and", Environment.NewLine, "is not included in any structures projects.");
                panelStructuresProjects.Height = 58;
            }
            else
            {
                labelStructuresProjects.Text = "Structures Projects";
                panelStructuresProjects.Height = 24;
            }

            //List<int> fiipsWorkConceptIds = new List<int>();
            var fiipsProjects = dataServ.GetProjectsInFiips().Where(p => p.WorkConcepts.Any(wc => wc.StructureId.Equals(structureId)))
                                        .GroupBy(p => p.FosProjectId).Select(g => g.First()).ToList();
            var workConceptsInFiipsProjects = new List<WorkConcept>();

            foreach (var project in fiipsProjects)
            {
                try
                {
                    workConceptsInFiipsProjects.AddRange(project.WorkConcepts.Where(wc => wc.StructureId.Equals(structureId)));
                }
                catch { }
            }

            //foreach (var wc in workConcepts.Where(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Fiips))
            foreach (var wc in workConceptsInFiipsProjects.GroupBy(wc => new { wc.StructureId, wc.FosProjectId }).Select(g => g.First()))
            {
                //if (!fiipsWorkConceptIds.Contains(wc.WorkConceptDbId))
                {
                    //fiipsWorkConceptIds.Add(wc.WorkConceptDbId);
                    dgvFiipsProjects.Rows.Add();
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].Cells[1].Value = wc.WorkConceptDescription;
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].Cells[2].Value = wc.WorkConceptDbId;
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].Cells[4].Value = wc.FosProjectId;
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].Cells[5].Value = wc.FiipsDescription;
                    dgvFiipsProjects.Rows[dgvFiipsProjects.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                }
            }

            try
            {
                dgvFiipsProjects.CurrentCell.Selected = false;
            }
            catch { }

            if (dgvFiipsProjects.Rows.Count == 0)
            {
                labelFiipsProjects.Text = String.Format("Fiips Projects{0}{1}{2}{3}", Environment.NewLine, "This structure is not included in any Fiips projects", Environment.NewLine, "in the current 11-year window.");
                panelFiipsProjects.Height = 58;
            }
            else
            {
                labelFiipsProjects.Text = "Fiips Projects";
                panelFiipsProjects.Height = 24;
            }
        }

        private string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds)
        {
            return structServ.ConvertDegreesMinutesSecondsToDecimalDegrees(degreesMinutesSeconds);
        }

        private void FormStructure_Load(object sender, EventArgs e)
        {
            //textBoxStructureId.Text = structure.StructureId;
            //toolTipStructureWindow.SetToolTip(buttonZoomIn, "Zoom In");
            //toolTipStructureWindow.SetToolTip(buttonZoomOut, "Zoom Out");
            
        }

        private void SetToolTipProperties(ToolTip toolTip, bool isBalloon, int autoPopDelay)
        {
            structServ.SetToolTipProperties(toolTip, isBalloon, autoPopDelay);
        }

        private void buttonRefreshMap_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(otherStructureUrl);
        }

        private void gMapControlSingleStructure_Load(object sender, EventArgs e)
        {
            gMapControlSingleStructure.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControlSingleStructure.DisableFocusOnMouseEnter = true;
            gMapControlSingleStructure.DragButton = MouseButtons.Left;
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
        }

        private void dataGridViewStructureAllWorkConcepts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        internal void DoWorkConceptAdd(WorkConcept wc)
        {
            FormStructureProject formStructureProject = null;

            try
            {
                formStructureProject = (FormStructureProject)Utility.GetForm("FormStructureProject");
            }
            catch { }

            if (formStructureProject == null)
            {
                formStructureProject = new FormStructureProject(userAccount, (DatabaseService)dataServ, wc, formMapping);
            }
            else
            {
                if (formStructureProject.Text.ToUpper().Contains("FIIPS"))
                {
                    formStructureProject.Close();
                    formStructureProject = new FormStructureProject(userAccount, (DatabaseService)dataServ, wc, formMapping);
                }
                else
                {
                    // IMPORTANT to make a new copy
                    WorkConcept newWc = new WorkConcept(wc);
                    formStructureProject.RefreshForm(newWc, true, true);
                }
            }

            FormStructureProject.ShowFormStructureProject(formStructureProject);
        }

        private void dataGridViewStructureAllWorkConcepts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;

            if (rowIndex >= 0 && columnIndex >= 0)
            {
                string columnName = senderGrid.Columns[columnIndex].Name;
                
                switch (columnName.ToUpper())
                {
                    case "ADDTOPROJ":
                        string alert = null;
                        int numberOfCertifiedWorkConcepts = workConceptsInStructuresProjectsForGivenStructure.Where(wc => wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified).Count();

                        if (numberOfCertifiedWorkConcepts > 0)
                        {
                            alert = String.Format("Structure has {0} certified {1} already.", numberOfCertifiedWorkConcepts, numberOfCertifiedWorkConcepts == 1 ? "work concept" : "work concepts");
                        }

                        if (structuresProjectsForGivenStructure.Count() > 0)
                        {
                            if (alert == null)
                            {
                                alert = String.Format("{0} {1} this structure already.", structuresProjectsForGivenStructure.Count, structuresProjectsForGivenStructure.Count == 1 ? "structures project contains" : "structures projects contain");
                            }
                            else
                            {
                                alert = String.Format("{0} In addition, {1} {2} this structure already.", alert, structuresProjectsForGivenStructure.Count, structuresProjectsForGivenStructure.Count == 1 ? "structures project contains" : "structures projects contain");
                            }
                        }

                        if (alert != null)
                        {
                            DialogResult result = MessageBox.Show(String.Format("{0} Continue?", alert), "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.No)
                            {
                                return;
                            }
                        }

                        try
                        {
                            int workConceptDbId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[2].Value);
                            DoWorkConceptAdd(eligibleProposedWorkConceptsForGivenStructure.Where(wc => wc.WorkConceptDbId == workConceptDbId).First());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Error: {0}", ex.Message), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        break;
                    case "DGVCDELETEPROPOSEDWORKCONCEPT":
                        // Allow deletion only if Work Concept's Proposed and it's not in a Structures Project
                        try
                        {
                            int workConceptDbId = Convert.ToInt32(senderGrid.Rows[e.RowIndex].Cells[2].Value);
                            WorkConcept currentWorkConcept = null;

                            try
                            {
                                currentWorkConcept = workConceptsInStructuresProjectsForGivenStructure.Where(wc => wc.WorkConceptDbId == workConceptDbId).First();

                                if (currentWorkConcept.WorkConceptCode.Equals("PR"))
                                {
                                    if (currentWorkConcept.ProjectDbId != 0)
                                    {
                                        MessageBox.Show("Can't delete this proposed work concept because it's in a Structures Project.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        dataServ.DeactivateProposedWorkConcept(currentWorkConcept.WorkConceptDbId, currentWorkConcept.StructureId);
                                        //formMapping.OpenStructureWindow(currentWorkConcept.StructureId);
                                        WorkConcept proposedWorkConcept = null;

                                        try
                                        {
                                            proposedWorkConcept = eligibleWorkConcepts.Where(w => w.WorkConceptDbId == workConceptDbId &&
                                                                                                w.StructureId.Equals(structureId) && w.FromProposedList).First();
                                            formMapping.EditWorkConcept(proposedWorkConcept, "Delete");
                                            //eligibleWorkConcepts.Remove(proposedWorkConcept);
                                            dataServ.GetEligibleWorkConcepts().Remove(proposedWorkConcept);
                                        }
                                        catch { }

                                        formMapping.OpenStructureWindow(currentWorkConcept.StructureId);
                                    }
                                }
                                else
                                {

                                }
                            }
                            catch
                            {
                                
                                string structureId = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();
                                dataServ.DeactivateProposedWorkConcept(workConceptDbId, structureId);
                                //formMapping.OpenStructureWindow(structureId);
                                WorkConcept proposedWorkConcept = null;

                                try
                                {
                                    proposedWorkConcept = eligibleWorkConcepts.Where(w => w.WorkConceptDbId == workConceptDbId &&
                                                                                        w.StructureId.Equals(structureId) && w.FromProposedList).First();
                                    formMapping.EditWorkConcept(proposedWorkConcept, "Delete");
                                    //eligibleWorkConcepts.Remove(proposedWorkConcept);
                                    dataServ.GetEligibleWorkConcepts().Remove(proposedWorkConcept);
                                }
                                catch { }

                                formMapping.OpenStructureWindow(structureId);
                            }
                            //DoWorkConceptAdd(eligibleProposedWorkConceptsForGivenStructure.Where(wc => wc.WorkConceptDbId == workConceptDbId).First());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Error: {0}", ex.Message), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
            }
        }

        private void linkLabelLastInspection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(textBoxLastInspectionPath.Text))
            {
                try
                {
                    System.Diagnostics.Process.Start(textBoxLastInspectionPath.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to open last inspection. Error: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Unable to open last inspection.");
            }
        }

        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            gMapControlSingleStructure.Zoom++;
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            gMapControlSingleStructure.Zoom--;
        }

        private void radioButtonMapChoice_Click(object sender, EventArgs e)
        {
            var checkedButton = groupBoxMapping.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked);

            switch (checkedButton.Text)
            {
                case "Google":
                    gMapControlSingleStructure.MapProvider = GoogleMapProvider.Instance;
                    break;
                case "Hybrid":
                    gMapControlSingleStructure.MapProvider = BingHybridMapProvider.Instance;
                    break;
            }
        }

        private void buttonViewPlans_Click(object sender, EventArgs e)
        {
            string plansFolderPath = warehouseDatabase.GetPlansFolderPath(structureId, comboBoxPlansYear.SelectedItem.ToString());

            try
            {
                Process.Start(plansFolderPath);
            }
            catch
            {
                MessageBox.Show("Unable to open bridge plans for year " + comboBoxPlansYear.SelectedItem.ToString() + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxInspectionDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            DateTime inspectionDate = Convert.ToDateTime(comboBox.SelectedItem);
            inspection = warehouseDatabase.GetInspection(structureId, inspectionDate);
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

                if (!String.IsNullOrEmpty(inspection.SpecialReport))
                {
                    linkLabelInspectionSpecialReport.Enabled = true;
                }
                else
                {
                    linkLabelInspectionSpecialReport.Enabled = false;
                }

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
                }

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

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        { }

        private void dataGridViewstructuresProjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenProject(sender, e);
        }

        private void dataGridViewFiipsProjects_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            OpenProject(sender, e);
        }

        private void OpenProject(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            int rowIndex = e.RowIndex;
            int columnIndex = e.ColumnIndex;

            if (columnIndex >= 0 && rowIndex >= 0)
            {
                string columnName = senderGrid.Columns[columnIndex].Name;

                switch (columnName.ToLower())
                {
                    case "datagridviewlinkcolumnstructuresprojectid":
                        try
                        {
                            int structuresProjectId = Convert.ToInt32(senderGrid.Rows[rowIndex].Cells[columnIndex].Value);
                            Project project = structureProjects.Where(p => p.ProjectDbId == structuresProjectId).First();
                            formMapping.OpenStructureProject(project);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Error: {0}", ex.Message), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    case "datagridviewlinkcolumnconstructionid":
                        try
                        {
                            string constructionId = senderGrid.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                            Project project = fiipsProjects.Where(p => p.FosProjectId.Equals(constructionId)).First();
                            formMapping.OpenStructureProject(project);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Error: {0}", ex.Message), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
            }
        }

        private void linkLabelInspectionSpecialReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //string plansFolderPath = warehouseDatabase.GetInspectionSpecialReport(structureId, comboBoxPlansYear.SelectedItem.ToString());

            try
            {
                Process.Start(inspection.SpecialReport);
            }
            catch
            {
                MessageBox.Show("Unable to open Inspection Special Report folder.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
