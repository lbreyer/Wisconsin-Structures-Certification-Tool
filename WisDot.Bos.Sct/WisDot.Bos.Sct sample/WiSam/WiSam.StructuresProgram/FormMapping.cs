using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Device.Location;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using ClosedXML.Excel;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using Dw = Wisdot.Bos.Dw;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;

namespace WiSam.StructuresProgram
{
    public partial class FormMapping : Form
    {
        private UserAccount userAccount;
        private List<WorkConcept> eligibleWorkConcepts;
        private List<WorkConcept> selectedWorkConcepts;
        private List<WorkConcept> quasicertifiedWorkConcepts;
        private List<WorkConcept> precertifiedApprovedWorkConcepts;
        private List<WorkConcept> precertifiedUnapprovedWorkConcepts;
        private List<WorkConcept> certifiedWorkConcepts;
        private List<WorkConcept> fiipsWorkConcepts;
        private List<Project> structureProjects;
        private List<Project> fiipsProjects;
        //private Project project;
        private int startYear;
        private int endYear;
        private List<GMapOverlay> layers;
        //private GMapOverlay selectedWorkConceptsLayer;
        private GMapOverlay nearMeLayer;
        private GMapOverlay projectsLayer;
        private GMapOverlay fiipsLayer;
        private GMapOverlay structureLayer;
        private object sender;
        private IDatabaseService dataServ;
        private List<WorkConcept> primaryWorkConcepts;
        private List<WorkConcept> secondaryWorkConcepts;
        private List<string> justifications;
        private List<WorkConcept> allWorkConcepts;
        internal FormMainController formMainController;
        private GMapMarker mousedOverMarker = null;
        private List<int> projectIdHits = new List<int>();
        private List<string> structureIdHits = new List<string>();
        private List<string> fiipsIdHits = new List<string>();
        private List<Project> projectHits = new List<Project>();
        private ExcelReporterService excelReporter = new ExcelReporterService();
        private static Excel.Application xlsApp;
        private static Excel.Workbooks xlsBooks;

        //43.0731, -89.4012
        private double startingLatitude = 43.7844;
        private double startingLongitude = -88.7879;
        //private int nearMeWorkConceptCounter = 0;
        private int currentFiscalYear = 0;

        // User area selection
        //PointLatLng start;
        //PointLatLng end;
        //bool selecting = false;
        private int userMarkerCount = 0;
        private FormLoggingIn formLoading = new FormLoggingIn();
        private FormProposeWorkConcept formProposeWorkConcept;
        private TreeNode startingNode = null;

        private static FormMappingController controller = new FormMappingController();

        internal FormMapping(UserAccount userAccount, int startYear, int endYear,
            List<Project> fiipsProjects, List<Project> structureProjects,
            List<WorkConcept> certifiedWorkConcepts, List<WorkConcept> precertifiedApprovedWorkConcepts,
            List<WorkConcept> precertifiedUnapprovedWorkConcepts,
            List<WorkConcept> quasicertifiedWorkConcepts, List<WorkConcept> eligibleWorkConcepts,
            List<WorkConcept> selectedWorkConcepts, List<WorkConcept> fiipsWorkConcepts,
            object sender, DatabaseService database, List<WorkConcept> primaryWorkConcepts,
            List<WorkConcept> allWorkConcepts, FormMainController formMainController)
        {
            InitializeComponent();
            this.userAccount = userAccount;
            this.eligibleWorkConcepts = eligibleWorkConcepts;
            this.selectedWorkConcepts = selectedWorkConcepts;
            this.quasicertifiedWorkConcepts = quasicertifiedWorkConcepts;
            this.precertifiedUnapprovedWorkConcepts = precertifiedUnapprovedWorkConcepts;
            this.precertifiedApprovedWorkConcepts = precertifiedApprovedWorkConcepts;
            this.certifiedWorkConcepts = certifiedWorkConcepts;
            this.fiipsWorkConcepts = fiipsWorkConcepts;
            this.structureProjects = structureProjects;
            this.fiipsProjects = fiipsProjects;
            this.startYear = startYear;
            this.endYear = endYear;
            this.currentFiscalYear = GetFiscalYear();
            this.sender = sender;
            dataServ = database;
            this.primaryWorkConcepts = primaryWorkConcepts;
            this.allWorkConcepts = allWorkConcepts;
            this.formMainController = formMainController;

            if (secondaryWorkConcepts == null || secondaryWorkConcepts.Count() == 0)
            {
                secondaryWorkConcepts = database.GetSecondaryWorkConcepts();
            }

            if (justifications == null || justifications.Count() == 0)
            {
                justifications = database.GetProposedWorkConceptJustifications();
            }

            // Create map layers
            layers = new List<GMapOverlay>();
            fiipsLayer = new GMapOverlay("Fiips");
            fiipsLayer.IsVisibile = true;
            layers.Add(fiipsLayer);
            gMapControlStructuresMap.Overlays.Add(fiipsLayer);

            nearMeLayer = new GMapOverlay("NearMe");
            nearMeLayer.IsVisibile = true;
            layers.Add(nearMeLayer);
            gMapControlStructuresMap.Overlays.Add(nearMeLayer);

            projectsLayer = new GMapOverlay("Projects");
            projectsLayer.IsVisibile = true;
            layers.Add(projectsLayer);
            gMapControlStructuresMap.Overlays.Add(projectsLayer);

            structureLayer = new GMapOverlay("Structure");
            structureLayer.IsVisibile = true;
            layers.Add(structureLayer);
            gMapControlStructuresMap.Overlays.Add(structureLayer);

            RefreshForm(startYear, endYear, fiipsProjects, structureProjects,
                certifiedWorkConcepts, precertifiedApprovedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                quasicertifiedWorkConcepts, eligibleWorkConcepts, selectedWorkConcepts, fiipsWorkConcepts, sender);
        }

        private int GetFiscalYear()
        {
            return controller.GetFiscalYear();
        }

        public void RefreshForm(int startYear, int endYear,
            List<Project> fiipsProjects, List<Project> structureProjects,
            List<WorkConcept> certifiedWorkConcepts, List<WorkConcept> precertifiedApprovedWorkConcepts,
            List<WorkConcept> precertifiedUnapprovedWorkConcepts,
            List<WorkConcept> quasicertifiedWorkConcepts, List<WorkConcept> eligibleWorkConcepts,
            List<WorkConcept> selectedWorkConcepts, List<WorkConcept> fiipsWorkConcepts,
            object sender)
        {
            this.eligibleWorkConcepts = eligibleWorkConcepts;
            this.selectedWorkConcepts = selectedWorkConcepts;
            this.quasicertifiedWorkConcepts = quasicertifiedWorkConcepts;
            this.precertifiedUnapprovedWorkConcepts = precertifiedUnapprovedWorkConcepts;
            this.precertifiedApprovedWorkConcepts = precertifiedApprovedWorkConcepts;
            this.certifiedWorkConcepts = certifiedWorkConcepts;
            this.fiipsWorkConcepts = fiipsWorkConcepts;
            this.structureProjects = structureProjects;
            this.fiipsProjects = fiipsProjects;
            this.startYear = startYear;
            this.endYear = endYear;
            this.sender = sender;
            gMapControlStructuresMap.ShowCenter = true;
            //gMapControlStructuresMap.MapScaleInfoEnabled = true;


            //gMapControlStructuresMap.DragButton = MouseButtons.Left;
           
            // Clear markers for each overlay
            foreach (var o in gMapControlStructuresMap.Overlays)
            {
                o.Markers.Clear();
            }

            if (selectedWorkConcepts.Count == 0) // Center map in Madison
            {
                gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(startingLatitude, startingLongitude);
            }

            RenderTree();
            PopulateWorkConceptsFilter();
            comboBoxMapProviders.SelectedIndex = 0;

            if (userAccount.IsSuperUser || userAccount.IsSuperRead)
            {
                textBoxOffice.Text = "any";
            }
            else
            {
                textBoxOffice.Text = userAccount.Office.Substring(0, 1);
            }
        }

        internal TreeView GetTreeViewMap()
        {
            return treeViewMapping;
        }

        private void PopulateWorkConceptsFilter()
        {
            comboBoxFilterWorkConcepts.Items.Add("ANY");

            foreach (WorkConcept wc in allWorkConcepts)
            {
                if (!wc.WorkConceptCode.Equals("00"))
                {
                    string workConceptDisplay = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                    comboBoxFilterWorkConcepts.Items.Add(workConceptDisplay);
                }
            }

            comboBoxFilterWorkConcepts.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="action"></param>
        internal void EditWorkConcept(WorkConcept wc, string action)
        {
            controller.EditWorkConcept(wc, action, treeViewMapping, gMapControlStructuresMap);
        }
        
        internal void EditProject(Project project, string action)
        {
            //GMapOverlay olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Structure")).First();
            //GMapOverlay nearMeOlay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("NearMe")).First();
            var projectMarkers = projectsLayer.Markers.Where(x => ((Project)x.Tag).ProjectDbId == project.ProjectDbId).ToList();

            foreach (var projectMarker in projectMarkers)
            {
                //projectsLayer.Markers.Remove(projectMarker);
            }

            foreach (var wc in project.WorkConcepts)
            {
                // Remove markers associated with the Work Concept
                try
                {
                    foreach (GMapOverlay o in gMapControlStructuresMap.Overlays)
                    {
                        try
                        {
                            var markers = o.Markers.Where(x => ((WorkConcept)x.Tag).WorkConceptDbId == wc.WorkConceptDbId).ToList();

                            foreach (var marker in markers)
                            {
                                try
                                {
                                    o.Markers.Remove(marker);
                                }
                                catch { }

                                /*
                                try
                                {
                                    nearMeOlay.Markers.Remove(marker);
                                }
                                catch { }*/
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                catch { }
            }



            switch (action)
            {
                case "Add":
                    UpdateTreeForProject(project);
                    break;
                case "Update":
                    DeleteProjectMarkerAndRoutes(project);
                    DeleteTreeForProject(project);
                    UpdateTreeForProject(project);
                    break;
                case "Delete":
                    DeleteProjectMarkerAndRoutes(project);
                    DeleteTreeForProject(project);
                    break;
                case "Deactivate":

                    break;
            }
        }

        private void DeleteProjectMarkerAndRoutes(Project project)
        {
            controller.DeleteProjectMarkerAndRoutes(project, gMapControlStructuresMap);
        }

        private void DeleteTreeForProject(Project project)
        {
            int projectDbId = project.ProjectDbId;

            if (treeViewMapping.Nodes["MyProjects"].Nodes != null)
            {
                foreach (TreeNode projectNode in treeViewMapping.Nodes["MyProjects"].Nodes)
                {
                    if (projectNode.Tag != null && projectNode.Tag is Project)
                    {
                        if (((Project)projectNode.Tag).ProjectDbId == projectDbId)
                        {
                            try
                            {
                                projectNode.Nodes.Clear();
                            }
                            catch { }

                            try
                            {
                                projectNode.Remove();
                            }
                            catch { }

                            break;
                        }
                    }
                }
            }

            if (treeViewMapping.Nodes["Projects"].Nodes != null)
            {
                foreach (TreeNode fyNode in treeViewMapping.Nodes["Projects"].Nodes)
                {
                    if (fyNode.Nodes["ProjectsStr"].Nodes != null)
                    {
                        foreach (TreeNode projectNode in fyNode.Nodes["ProjectsStr"].Nodes)
                        {
                            if (projectNode.Tag != null && projectNode.Tag is Project)
                            {
                                if (((Project)projectNode.Tag).ProjectDbId == projectDbId)
                                {
                                    try
                                    {
                                        projectNode.Nodes.Clear();
                                    }
                                    catch { }

                                    try
                                    {
                                        projectNode.Remove();
                                    }
                                    catch { }

                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateTreeForProject(Project project)
        {
            // FY->Structure Projects tree node
            var bNode = treeViewMapping.Nodes["Projects"].Nodes.Cast<TreeNode>().Where(tn => tn.Text.Equals(project.FiscalYear.ToString())).First();
            var cNode = bNode.Nodes["ProjectsStr"];

            // Add to FY->Structure Projects tree node
            TreeNode dNode = new TreeNode(String.Format("Str Proj Id {0}{1}", project.ProjectDbId, project.FosProjectId.Length > 0 ? " : Cnst Id " + FormatConstructionId(project.FosProjectId) : ""));
            dNode.Tag = project;
            cNode.Nodes.Add(dNode);
            ColorProjectNode(dNode, project.Status);
            RenderTreeWorkConcepts(dNode, project.WorkConcepts, false);
            dNode.Checked = true;

            // Add to My Projects tree node
            TreeNode dNodeMyProjects = new TreeNode(String.Format("Str Proj Id {0}{1}", project.ProjectDbId, project.FosProjectId.Length > 0 ? " : Cnst Id " + FormatConstructionId(project.FosProjectId) : ""));
            dNodeMyProjects.Tag = project;
            treeViewMapping.Nodes["MyProjects"].Nodes.Add(dNodeMyProjects);
            ColorProjectNode(dNodeMyProjects, project.Status);
            RenderTreeWorkConcepts(dNodeMyProjects, project.WorkConcepts, false);
            dNodeMyProjects.Checked = true;
            treeViewMapping.Nodes["MyProjects"].Expand();
            dNodeMyProjects.Expand();
        }

        private string FormatStructureId(string structureId)
        {
            return controller.FormatStructureId(structureId);
        }

        private string FormatConstructionId(string constructionId)
        {
            return controller.FormatConstructionId(constructionId);
        }

        private void RenderTree()
        {
            treeViewMapping.Nodes.Clear();
            TreeNode aNode;

            if (selectedWorkConcepts.Count > 0)
            {
                aNode = new TreeNode("Selections");
                aNode.Tag = "Selections";
                aNode.Checked = false;
                treeViewMapping.Nodes.Add(aNode);
                RenderTreeWorkConcepts(aNode, selectedWorkConcepts, false);
                aNode.Expand();
            }

            aNode = new TreeNode("My Projects");
            aNode.Tag = "MyProjects";
            aNode.Name = "MyProjects";
            treeViewMapping.Nodes.Add(aNode);
            List<Project> myProjects = new List<Project>();
            //myProjects.AddRange(structureProjects.Where(sp => sp.UserDbIds != null && sp.UserDbIds.Contains(userAccount.UserDbId)));
            myProjects.AddRange(structureProjects.Where(sp => sp.History.Contains(userAccount.LastName)));

            if (userAccount.IsSuperUser)
            {
                var submittedProjects = structureProjects
                                        .Where(sp => (
                                                        sp.UserAction == StructuresProgramType.ProjectUserAction.SavedProject
                                                        || sp.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification
                                                        || sp.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification
                                                        || sp.UserAction == StructuresProgramType.ProjectUserAction.CreateProject
                                                     )
                                                     && !myProjects.Any(mp => mp.ProjectDbId == sp.ProjectDbId)
                                              );

                if (submittedProjects != null)
                {
                    myProjects.AddRange(submittedProjects);
                }
            }

            myProjects = myProjects.OrderBy(mp => (int)mp.Status).ThenBy(mp => mp.FiscalYear).ToList();

            foreach (Project project in myProjects)
            {
                TreeNode bNode = new TreeNode(String.Format("Str Proj Id {0}{1}", project.ProjectDbId, project.FosProjectId.Length > 0 ? " : Cnst Id " + FormatConstructionId(project.FosProjectId) : ""));
                aNode.Nodes.Add(bNode);
                bNode.Tag = project;
                ColorProjectNode(bNode, project.Status);
                RenderTreeWorkConcepts(bNode, project.WorkConcepts, false);
            }

            aNode = new TreeNode("Projects");
            aNode.Tag = "Projects";
            aNode.Name = "Projects";
            treeViewMapping.Nodes.Add(aNode);

            for (int i = startYear; i <= endYear; i++)
            {
                TreeNode bNode = new TreeNode(String.Format("{0}", i));
                bNode.Tag = "ProjectsFy";
                bNode.Name = "ProjectsFy";
                aNode.Nodes.Add(bNode);

                TreeNode cNode = new TreeNode("Str");
                cNode.Tag = "ProjectsStr";
                cNode.Name = "ProjectsStr";
                bNode.Nodes.Add(cNode);

                // Loop through Structure projects
                foreach (Project project in structureProjects.Where(e => e.FiscalYear == i))
                {
                    TreeNode dNode = new TreeNode(String.Format("Str Proj Id {0}{1}", project.ProjectDbId, project.FosProjectId.Length > 0 ? " : Cnst Id " + FormatConstructionId(project.FosProjectId) : ""));
                    dNode.Tag = project;
                    cNode.Nodes.Add(dNode);
                    ColorProjectNode(dNode, project.Status);
                    RenderTreeWorkConcepts(dNode, project.WorkConcepts, false);
                }

                cNode = new TreeNode("Fiips");
                cNode.Tag = "ProjectsFiips";
                bNode.Nodes.Add(cNode);

                // Loop through Fiips projects
                foreach (Project project in fiipsProjects.Where(e => e.FiscalYear == i))
                {
                    TreeNode dNode = new TreeNode(String.Format("Cnst Id {0}", FormatConstructionId(project.FosProjectId)));
                    //dNode.Tag = "ProjectDbId" + project.FosProjectId;
                    dNode.Tag = project;
                    cNode.Nodes.Add(dNode);
                    ColorProjectNode(dNode, project.Status);
                    RenderTreeWorkConcepts(dNode, project.WorkConcepts, false);
                }
            }

            aNode = new TreeNode("Work Concepts");
            aNode.Tag = "WorkConcepts";
            aNode.Name = "WorkConcepts";
            treeViewMapping.Nodes.Add(aNode);

            for (int i = startYear; i <= endYear; i++)
            {
                TreeNode bNode = new TreeNode(String.Format("{0}", i));
                bNode.Tag = "WorkConceptsFy";
                aNode.Nodes.Add(bNode);

                TreeNode cNode = new TreeNode("Eligible");
                cNode.Tag = "WorkConceptsEligible";
                cNode.Name = "WorkConceptsEligible";

                if (i != currentFiscalYear)
                {
                    bNode.Nodes.Add(cNode);
                    //RenderTreeWorkConcepts(cNode, eligibleWorkConcepts.Where(e => e.FiscalYear == i && quasicertifiedWorkConcepts.Where(w => w.StructureId.Equals(e.StructureId)).Count() == 0).ToList());
                    RenderTreeWorkConcepts(cNode, eligibleWorkConcepts.Where(e => e.FiscalYear == i).ToList());
                }
                /*
                cNode = new TreeNode("Quasi-Certified");
                cNode.Tag = "WorkConceptsQuasiCertified";
                bNode.Nodes.Add(cNode);
                RenderTreeWorkConcepts(cNode, quasicertifiedWorkConcepts.Where(e => e.FiscalYear == i).ToList());
                */

                /*
                cNode = new TreeNode("Unapproved");
                cNode.Tag = "WorkConceptsUnapproved";
                cNode.Name = "WorkConceptsUnapproved";
                bNode.Nodes.Add(cNode);
                RenderTreeWorkConcepts(cNode, precertifiedUnapprovedWorkConcepts.Where(e => e.StructureProjectFiscalYear == i).ToList());

                cNode = new TreeNode("Precertified");
                cNode.Tag = "WorkConceptsPrecertified";
                cNode.Name = "WorkConceptsPrecertified";
                bNode.Nodes.Add(cNode);
                RenderTreeWorkConcepts(cNode, precertifiedApprovedWorkConcepts.Where(e => e.StructureProjectFiscalYear == i).ToList());

                cNode = new TreeNode("Certified");
                cNode.Tag = "WorkConceptsCertified";
                cNode.Name = "WorkConceptsCertified";
                bNode.Nodes.Add(cNode);
                RenderTreeWorkConcepts(cNode, quasicertifiedWorkConcepts.Where(e => e.FiscalYear == i).ToList());
                RenderTreeWorkConcepts(cNode, certifiedWorkConcepts.Where(e => e.StructureProjectFiscalYear == i).ToList());
                */

                cNode = new TreeNode("Fiips");
                cNode.Tag = "WorkConceptsFiips";
                cNode.Name = "WorkConceptsFiips";
                bNode.Nodes.Add(cNode);
                RenderTreeWorkConcepts(cNode, fiipsWorkConcepts.Where(e => e.FiscalYear == i).ToList());
            }

            aNode = new TreeNode("Search By Distance Searches");
            aNode.Tag = "NearMes";
            aNode.Name = "NearMes";
            treeViewMapping.Nodes.Add(aNode);
        }

        private void ColorProjectNode(TreeNode node, StructuresProgramType.ProjectStatus status)
        {
            controller.ColorProjectNode(node, status);
        }

        //private void ColorWorkConceptNodes(List<WorkConcept>)

        private void ColorWorkConceptNode(TreeNode node, StructuresProgramType.WorkConceptStatus status)
        {
            controller.ColorWorkConceptNode(node, status);
        }

        internal void RenderTreeWorkConcepts(TreeNode parentNode, List<WorkConcept> workConcepts, bool isChecked = false)
        {
            controller.RenderTreeWorkConcepts(parentNode, workConcepts, isChecked);
        }

        internal void AddMarkersToLayer(GMapOverlay layer, List<WorkConcept> workConcepts, GMarkerGoogleType markerType, bool isVisible)
        {
            controller.AddMarkersToLayer(layer, workConcepts, markerType, isVisible);
        }

        private void gMapControlStructuresMap_Load(object sender, EventArgs e)
        {
            gMapControlStructuresMap.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControlStructuresMap.DisableFocusOnMouseEnter = true;
            gMapControlStructuresMap.DragButton = MouseButtons.Left;
            //gMapControlStructuresMap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            //GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
        }

        private void checkBoxTooltip_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            MarkerTooltipMode tooltipMode = MarkerTooltipMode.OnMouseOver;

            if (checkBox.Checked)
            {
                tooltipMode = MarkerTooltipMode.Always;
            }

            foreach (var o in gMapControlStructuresMap.Overlays)
            {
                foreach (var marker in o.Markers)
                {
                    marker.ToolTipMode = tooltipMode;
                }
            }

            gMapControlStructuresMap.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormScot formScot = new FormScot();
            formScot.TopMost = false;
            formScot.Show();
        }

        private void listViewMappedWorkConcepts_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            /*
            var listView = (ListView)sender;
            int itemIndex = e.Item.Index;
            gMapControlStructuresMap.Overlays[itemIndex].IsVisibile = e.Item.Checked;*/
        }

        private bool IsFormOpen(string formName)
        {
            bool isFormOpen = false;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals(formName))
                {
                    isFormOpen = true;
                    break;
                }
            }

            return isFormOpen;
        }

        private void OpenForm(Form form)
        {
            bool isFormOpen = false;

            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name.Equals(form.Name))
                {
                    isFormOpen = true;
                    form = openForm;
                    break;
                }
            }

            if (isFormOpen)
            {
                form.Show();
            }
            else
            {
                form = new Form();
                form.Show();
            }
        }

        private void gMapControlStructuresMap_OnMarkerDoubleClick(GMapMarker item, MouseEventArgs e)
        {
            return;
            /*
            WorkConcept wc = (WorkConcept)item.Tag;

            if (wc.FromEligibilityList || wc.Evaluate || wc.FromProposedList)
            {
                FormStructureProject formStructureProject = null;

                try
                {
                    formStructureProject = (FormStructureProject)Utility.GetForm("FormStructureProject");
                }
                catch { }

                if (formStructureProject == null)
                {
                    formStructureProject = new FormStructureProject(userAccount, dataServ, wc, primaryWorkConcepts, eligibleWorkConcepts,
                                                                    quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                    precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                    fiipsWorkConcepts, structureProjects, fiipsProjects,
                                                                    allWorkConcepts, this);
                }
                else
                {
                    // IMPORTANT to make a new copy
                    WorkConcept newWc = new WorkConcept(wc);
                    formStructureProject.RefreshForm(newWc, true, true);
                }

                Utility.ShowFormStructureProject(formStructureProject);
            }*/
        }

        private WorkConcept GetWorkConceptInstances(string structureId)
        {
            return controller.GetWorkConceptInstances(structureId, structureProjects);
        }

        private void treeViewMapping_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            
            if (!checkBoxClearMapStatus.Checked && startingNode == null)
            {
                startingNode = tn;
                var tag = startingNode.Tag.ToString();
                OpenLoading();
            }
            
            foreach (TreeNode cn in tn.Nodes)
            {
                cn.Checked = tn.Checked;
            }

            // Map clearing in progress so just unchecking tree nodes without affecting the map
            if (checkBoxClearMapStatus.Checked)
            {
                return;
            }

            if (tn.Tag is WorkConcept)
            {
                WorkConcept wc = (WorkConcept)tn.Tag;
                GMapOverlay olay;
                GMapMarker marker;

                try
                {
                    /*
                    switch (wc.Status)
                    {
                        case StructuresProgramType.WorkConceptStatus.Fiips:
                            olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Fiips")).First();
                            break;
                        case StructuresProgramType.WorkConceptStatus.Evaluate:
                        case StructuresProgramType.WorkConceptStatus.Proposed:
                            olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("NearMe")).First();
                            break;
                        default:
                            olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Structure")).First();
                            break;
                    }*/

                    if (wc.Status == StructuresProgramType.WorkConceptStatus.Fiips)
                    {
                        olay = fiipsLayer;
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate || (wc.Status == StructuresProgramType.WorkConceptStatus.Proposed && wc.WorkConceptDbId <= 0))
                    {
                        olay = nearMeLayer;
                    }
                    else
                    {
                        if (wc.StructureId.Equals("B090063"))
                        {
                            var stop = "here";
                        }
                        olay = structureLayer;

                        if (tn.Parent.Tag.ToString().ToUpper().Contains("SEARCHBYDISTANCE"))
                        {
                            olay = nearMeLayer;
                        }
                    }

                    if (olay.Markers.Where(x => x.Tag != null && !x.Tag.ToString().StartsWith("SCT Marker") && ((WorkConcept)x.Tag).WorkConceptDbId == wc.WorkConceptDbId 
                                            && ((WorkConcept)x.Tag).ProjectDbId != wc.ProjectDbId).Count() == 0)
                    {
                        List<WorkConcept> wcs = new List<WorkConcept>();

                        if (wc.FromEligibilityList || (wc.FromProposedList && wc.WorkConceptDbId <= 0))
                        {
                            WorkConcept match = GetWorkConceptInstances(wc.StructureId);

                            if (match != null)
                            {
                                wcs.Add(match);
                                wc = match;
                            }
                            else
                            {
                                wcs.Add(wc);
                            }
                        }
                        else
                        {
                            wcs.Add(wc);
                        }

                        AddMarkersToLayer(olay, wcs, wc.MapMarkerType, false);
                    }

                    try
                    {

                        marker = olay.Markers.First(x => x.Tag != null && !x.Tag.ToString().StartsWith("SCT Marker") && ((wc.WorkConceptDbId == -1 && ((WorkConcept)x.Tag).StructureId.Equals(wc.StructureId)) || (((WorkConcept)x.Tag).WorkConceptDbId == wc.WorkConceptDbId && wc.WorkConceptDbId != -1)));
                        marker.IsVisible = tn.Checked;                        /*
                        var multipleMarkers = olay.Markers.Where(x => x.Tag != null 
                                                                    && !x.Tag.ToString().StartsWith("SCT Marker") 
                                                                    && ((wc.WorkConceptDbId == -1 && ((WorkConcept)x.Tag).StructureId.Equals(wc.StructureId)) 
                                                                            || (((WorkConcept)x.Tag).WorkConceptDbId == wc.WorkConceptDbId && wc.WorkConceptDbId != -1)
                                                                       )
                                                                    && wc.Status == ((WorkConcept)x.Tag).Status
                                                   );*/
                        //multipleMarkers.First().IsVisible = tn.Checked;
                        //var firstMarker = multipleMarkers.First();
                       //firstMarker = new GMarkerGoogle(firstMarker.Position, GMarkerGoogleType.pink);
                        //firstMarker.IsVisible = tn.Checked;

                        if (tn.Checked)
                        {
                            if (wc.GeoLocation != null && checkBoxDizzyEffect.Checked)
                            {
                                gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(wc.GeoLocation.LatitudeDecimal, wc.GeoLocation.LongitudeDecimal);
                            }
                        }
                    }
                    catch (Exception ex) // No marker placed
                    {
                        if (tn.Checked && checkBoxMappingError.Checked)
                        {
                            MessageBox.Show("Unable to map " + wc.StructureId + ".", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                { }
            }
            else if (tn.Tag is Project)
            {
                Project project = (Project)tn.Tag;
                GMapOverlay olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Projects")).First();
                List<GMapMarker> projectMarkers = new List<GMapMarker>();
                GMapMarker projectMarker = null;

                if (project.Status != StructuresProgramType.ProjectStatus.Fiips) // Structures Project
                {
                    try
                    {
                        projectMarker = olay.Markers.Where(x => x.Tag != null && ((Project)x.Tag).ProjectDbId == project.ProjectDbId).First();
                    }
                    catch { }
                }
                else // Fiips Project
                {
                    try
                    {
                        projectMarker = olay.Markers.Where(x => x.Tag != null
                                                                && ((Project)x.Tag).FosProjectId == project.FosProjectId
                                                                && ((Project)x.Tag).Status == StructuresProgramType.ProjectStatus.Fiips
                                                            ).First();
                    }
                    catch { }
                }

                if (projectMarker == null)
                {
                    int counter = 0;
                    float sumLats = 0;
                    float sumLongs = 0;
                    string currentStructureId = "";
                    string previousStructureId = "";
                    float projectLat = 0;
                    float projectLong = 0;

                    if (project.GeoLocation != null && project.GeoLocation.LatitudeDecimal != Double.NaN && project.GeoLocation.LongitudeDecimal != Double.NaN && project.GeoLocation.LatitudeDecimal != 0 && project.GeoLocation.LongitudeDecimal != 0)
                    {
                        try
                        {
                            projectLat = project.GeoLocation.LatitudeDecimal;
                            projectLong = project.GeoLocation.LongitudeDecimal;
                        }
                        catch { }
                    }
                    else
                    {
                        foreach (var wc in project.WorkConcepts.OrderBy(w => w.StructureId))
                        {
                            currentStructureId = wc.StructureId;

                            if (!currentStructureId.Equals(previousStructureId))
                            {
                                if (wc.GeoLocation == null || wc.GeoLocation.LatitudeDecimal == 0 || wc.GeoLocation.LongitudeDecimal == 0)
                                {
                                    var gl = dataServ.GetStructureLatLong(wc.StructureId);
                                    wc.GeoLocation = gl;
                                }

                                if (wc.GeoLocation != null)
                                {
                                    if (wc.GeoLocation.LatitudeDecimal != 0 || wc.GeoLocation.LongitudeDecimal != 0)
                                    {
                                        counter++;
                                        sumLats += wc.GeoLocation.LatitudeDecimal;
                                        sumLongs += wc.GeoLocation.LongitudeDecimal;
                                    }
                                }
                            }

                            previousStructureId = currentStructureId;
                        }

                        try
                        {
                            if (counter != 0)
                            {
                                projectLat = sumLats / counter;
                            }
                        }
                        catch { }

                        try
                        {
                            if (counter != 0)
                            {
                                projectLong = sumLongs / counter;
                            }
                        }
                        catch { }

                        project.GeoLocation.LatitudeDecimal = projectLat;
                        project.GeoLocation.LongitudeDecimal = projectLong;
                    }

                    if (project.NumberOfStructures >= 1) // Create a project marker if there's at least 1 work concept
                    {
                        
                        //GMarkerGoogleType markerType = new GMarkerGoogleType();
                        // Default to Fiips project marker offset longitudinally
                        //projectMarker = new GMarkerGoogle(new PointLatLng(averageLat, averageLong - 0.00005), new Bitmap(WiSam.StructuresProgram.Properties.Resources.Map_Marker_Ball_Left_Dark_Blue_48));
                        

                        switch (project.Status)
                        {
                            case StructuresProgramType.ProjectStatus.Unapproved:
                                //markerType = GMarkerGoogleType.red_pushpin;
                                projectMarker = new GMarkerGoogle(new PointLatLng(projectLat, projectLong), new Bitmap(WiSam.StructuresProgram.Properties.Resources.Map_Marker_Ball_Right_Red_64));
                                break;
                            case StructuresProgramType.ProjectStatus.Precertified:
                                //markerType = GMarkerGoogleType.yellow_pushpin;
                                projectMarker = new GMarkerGoogle(new PointLatLng(projectLat, projectLong), new Bitmap(WiSam.StructuresProgram.Properties.Resources.Map_Marker_Ball_Right_Yellow_64));
                                break;
                            case StructuresProgramType.ProjectStatus.Certified:
                            case StructuresProgramType.ProjectStatus.QuasiCertified:
                                //markerType = GMarkerGoogleType.green_pushpin;
                                projectMarker = new GMarkerGoogle(new PointLatLng(projectLat, projectLong), new Bitmap(WiSam.StructuresProgram.Properties.Resources.Map_Marker_Ball_Right_Chartreuse_64));
                                break;
                            default:
                                //markerType = GMarkerGoogleType.blue_pushpin;
                                projectMarker = new GMarkerGoogle(new PointLatLng(projectLat, projectLong), new Bitmap(WiSam.StructuresProgram.Properties.Resources.Map_Marker_Ball_Left_Dark_Blue_48));
                                break;
                        }
                        
                        projectMarker.Tag = project;
                        
                        if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                        {
                            string structuresProjectId = "";

                            try
                            {
                                var strProject = structureProjects.Where(p => p.FosProjectId.Equals(project.FosProjectId)).First();
                                structuresProjectId = strProject.ProjectDbId.ToString();
                            }
                            catch { }

                            projectMarker.ToolTipText = String.Format("\nFIIPS PROJECT\nCONSTR ID: {0}\nFY: {1}\n# STR: {2}\nSTR PROJ ID: {3}", 
                                                                        FormatConstructionId(project.FosProjectId), project.FiscalYear, project.NumberOfStructures, structuresProjectId);
                        }
                        else
                        {
                            projectMarker.ToolTipText = String.Format("\nSTRUCTURES PROJECT\nSTR PROJ ID: {0}\nFY: {1}\n# STR: {2}\nCONSTR ID: {3}", 
                                                                        project.ProjectDbId, project.FiscalYear, project.NumberOfStructures,
                                                                        String.IsNullOrEmpty(project.FosProjectId) ? "" : FormatConstructionId(project.FosProjectId));
                        }

                        projectMarker.ToolTip.TextPadding = new Size(10, 10);
                        projectMarker.ToolTip.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        projectMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        olay.Markers.Add(projectMarker);

                        // Drawing "route" lines between project center and work concepts on "markerenter" event
                        /*
                        if (counter > 1)
                        {
                            foreach (var wc in project.WorkConcepts)
                            {
                                if (wc.GeoLocation != null)
                                {
                                    List<PointLatLng> points = new List<PointLatLng>();
                                    points.Add(new PointLatLng(averageLat, averageLong));
                                    points.Add(new PointLatLng(wc.GeoLocation.latitude, wc.GeoLocation.longitude));
                                    GMapRoute route = new GMapRoute(points, project.ProjectDbId.ToString());
                                    route.Tag = wc;

                                    if (markerType == GMarkerGoogleType.blue_pushpin)
                                    {
                                        route.Stroke = new Pen(Color.Blue, 3);
                                    }
                                    else
                                    {
                                        route.Stroke = new Pen(Color.HotPink, 3);
                                    }

                                    olay.Routes.Add(route);
                                }
                            }
                        }*/
                    }
                }

                if (projectMarker != null)
                {
                    projectMarker.IsVisible = tn.Checked;

                    foreach (WorkConcept wc in project.WorkConcepts)
                    {
                        GMapRoute route = null;

                        try
                        {
                            route = olay.Routes.Where(x => x.Tag != null && ((WorkConcept)x.Tag).WorkConceptDbId == wc.WorkConceptDbId).First();
                        }
                        catch { }

                        if (route != null)
                        {
                            route.IsVisible = tn.Checked;
                        }
                    }

                    if (tn.Checked && checkBoxDizzyEffect.Checked)
                    {
                        gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(projectMarker.Position.Lat, projectMarker.Position.Lng);
                    }
                }
            }
            else // Not a work concept and not a project
            {}

            if (startingNode == tn)
            {
                CloseLoading();
                startingNode = null;
            }
        }

        private void gMapControlLegend_Load(object sender, EventArgs e)
        {
            //gMapControlLegend.DisableFocusOnMouseEnter = true;
           
            /*
            gMapControlLegend.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            GMapOverlay markers = new GMapOverlay("markers");
            GMapMarker marker = new GMarkerGoogle(
                new PointLatLng(8.5000, -123.5000),
                new Bitmap(WiSam.StructuresProgram.Properties.Resources.disk));
            marker.ToolTipText = "Elig";
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markers.Markers.Add(marker);

            
                       gMapControlLegend.Position = new GMap.NET.PointLatLng(8.7832, -124.5085);
                       gMapControlLegend.ShowCenter = true;

                       GMapOverlay markers = new GMapOverlay("markers");
                       GMapMarker marker = new GMarkerGoogle(
                           new PointLatLng(8.5000, -123.5000),
                           GMarkerGoogleType.white_small);
                       marker.ToolTipText = "Elig";
                       marker.ToolTipMode = MarkerTooltipMode.Always;
                       markers.Markers.Add(marker);

                       marker = new GMarkerGoogle(
                           new PointLatLng(8.5000, -123.3000),
                           GMarkerGoogleType.red_small);
                       marker.ToolTipText = "Unaprvd";
                       marker.ToolTipMode = MarkerTooltipMode.Always;
                       markers.Markers.Add(marker);

                       marker = new GMarkerGoogle(
                           new PointLatLng(8.5000, -123.0000),
                           GMarkerGoogleType.yellow_small);
                       marker.ToolTipText = "Precrtfd";
                       marker.ToolTipMode = MarkerTooltipMode.Always;
                       markers.Markers.Add(marker);

                       marker = new GMarkerGoogle(
                           new PointLatLng(8.5000, -122.7000),
                           GMarkerGoogleType.green_small);
                       marker.ToolTipText = "Crtfd";
                       marker.ToolTipMode = MarkerTooltipMode.Always;
                       markers.Markers.Add(marker);
                       */
            /*
            marker = new GMarkerGoogle(
                new PointLatLng(8.5000, -122.4000),
                GMarkerGoogleType.green_pushpin);
            marker.ToolTipText = "Certified";
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markers.Markers.Add(marker);*/

            /*
            marker = new GMarkerGoogle(
               new PointLatLng(8.5000, -122.1000),
               GMarkerGoogleType.blue_dot);
            marker.ToolTipText = "Fiips";
            marker.ToolTipMode = MarkerTooltipMode.Always;
            markers.Markers.Add(marker);

            gMapControlLegend.Overlays.Add(markers);*/
        }

        private void treeViewMapping_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            TreeNode tn = treeViewMapping.GetNodeAt(e.X, e.Y);
            treeViewMapping.SelectedNode = tn;

            if (tn != null)
            {
                if (tn.Tag is Project)
                {
                    Project p = (Project)tn.Tag;
                    tn.ContextMenuStrip = contextMenuStripForNavTree;
                    contextMenuStripForNavTree.Items["toolStripMenuItemDelete"].Visible = false;
                    //contextMenuStripForNavTree.Show();
                }
                else if (tn.Tag is WorkConcept)
                {
                    WorkConcept wc = (WorkConcept)tn.Tag;
                    tn.ContextMenuStrip = contextMenuStripForNavTree;

                    // Show Delete only if WC is proposed and user has permission
                    if (!wc.FromProposedList)
                    {
                        contextMenuStripForNavTree.Items["toolStripMenuItemDelete"].Visible = false;
                    }
                    else
                    {
                        if (userAccount.IsRegionalProgrammer || userAccount.IsSuperUser || userAccount.IsAdministrator)
                        {
                            contextMenuStripForNavTree.Items["toolStripMenuItemDelete"].Visible = true;
                        }
                    }

                    //contextMenuStripForNavTree.Show();
                }
            }
        }

        private void HandleMapOnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (item.Tag is Project)
            {
                Project project = (Project)item.Tag;
                OpenStructureProject(project);
            }
            else if (item.Tag is WorkConcept)
            {
                WorkConcept wc = (WorkConcept)item.Tag;
                OpenStructureWindow(wc);
            }
        }

        /*
        private void OpenStructureProject(WorkConcept wc)
        {
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
                formStructureProject = new FormStructureProject(userAccount, dataServ, wc, primaryWorkConcepts, eligibleWorkConcepts,
                                                                quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                fiipsWorkConcepts, structureProjects, fiipsProjects, allWorkConcepts,
                                                                this, treeViewMapping);
            }

            //formStructureProject.BringToFront();
            formStructureProject.TopMost = false;
            formStructureProject.StartPosition = FormStartPosition.CenterScreen;
            formStructureProject.WindowState = FormWindowState.Normal;
            formStructureProject.Show();
        }*/

        internal void OpenStructureProject(Project project)
        {
            FormStructureProject formStructureProject = null;
            
            try
            {
                formStructureProject = (FormStructureProject)Utility.GetForm("FormStructureProject");
            }
            catch { }

            if (formStructureProject == null)
            {
                /*
                formStructureProject = new FormStructureProject(userAccount, dataServ, project, primaryWorkConcepts, eligibleWorkConcepts,
                                                                quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                fiipsWorkConcepts, structureProjects, fiipsProjects, allWorkConcepts, this);*/
                formStructureProject = new FormStructureProject(userAccount, (DatabaseService)dataServ, project, this);
            }
            else
            {
                if (formStructureProject.Text.ToUpper().Contains("FIIPS"))
                {
                    formStructureProject.Close();
                    /*
                    formStructureProject = new FormStructureProject(userAccount, dataServ, project, primaryWorkConcepts, eligibleWorkConcepts,
                                                                quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                fiipsWorkConcepts, structureProjects, fiipsProjects, allWorkConcepts, this);*/
                    formStructureProject = new FormStructureProject(userAccount, (DatabaseService)dataServ, project, this);
                }
                else
                {
                    if (formStructureProject.isDirty && !formStructureProject.savedTransaction)
                    {
                        DialogResult result = MessageBox.Show("A project window that has unsaved changes is currently open. Continue with opening another project?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            formStructureProject.RefreshForm(project);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        if (formStructureProject.GetProjectDbId() != project.ProjectDbId)
                        {
                            formStructureProject.RefreshForm(project);
                        }
                    }
                }
            }

            FormStructureProject.ShowFormStructureProject(formStructureProject);

            if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
            {
                textBoxSearchByDistanceId.Text = project.FosProjectId;
            }
            else
            {
                textBoxSearchByDistanceId.Text = project.ProjectDbId.ToString();
            }

            try
            {
                textBoxSearchByDistanceLatitude.Text = project.GeoLocation.LatitudeDecimal.ToString();
            }
            catch { }

            try
            {
                textBoxSearchByDistanceLongitude.Text = project.GeoLocation.LongitudeDecimal.ToString();
            }
            catch { }

            if (checkBoxCenterOnSelected.Checked && project.GeoLocation != null && project.GeoLocation.LatitudeDecimal != 0 && project.GeoLocation.LongitudeDecimal != 0)
            {
                gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(project.GeoLocation.LatitudeDecimal, project.GeoLocation.LongitudeDecimal);
            }
        }

        private void OpenAddWorkConceptToProject(WorkConcept workConcept)
        {
            FormAddWorkConceptToProject formAddWorkConceptToProject = null;
            bool formAddWorkConceptToProjectIsOpen = false;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals("FormAddWorkConceptToProject"))
                {
                    formAddWorkConceptToProjectIsOpen = true;
                    formAddWorkConceptToProject = (FormAddWorkConceptToProject)form;
                    break;
                }
            }

            if (formAddWorkConceptToProjectIsOpen)
            {
                formAddWorkConceptToProject.RefreshForm(workConcept);
            }
            else
            {
                formAddWorkConceptToProject = new FormAddWorkConceptToProject(workConcept, this, primaryWorkConcepts);
            }

            formAddWorkConceptToProject.TopMost = false; ;
            formAddWorkConceptToProject.WindowState = FormWindowState.Normal;
            formAddWorkConceptToProject.StartPosition = FormStartPosition.CenterScreen;
            formAddWorkConceptToProject.Show();
        }

        private void CloseStructureWindows()
        {
            List<Form> openStructureWindows = Utility.GetForms("FormStructure");

            foreach (var openStructureWindow in openStructureWindows)
            {
                openStructureWindow.Close();
            }
        }

        internal void OpenStructureWindow(string structureId)
        {
            if (!checkBoxAllowMultipleStructureWindows.Checked)
            {
                CloseStructureWindows();
            }

            FormStructure formStructure = new FormStructure(userAccount, (DatabaseService)dataServ, structureId, this);
            FormStructure.ShowFormStructure(formStructure);

            if (checkBoxCenterOnSelected.Checked)
            {
                GeoLocation gl = dataServ.GetStructureLatLong(structureId);

                if (gl != null || gl.LatitudeDecimal != 0 || gl.LatitudeDecimal != 0)
                {
                    gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(gl.LatitudeDecimal, gl.LongitudeDecimal);
                }
            }
        }

        internal void OpenStructureWindow(WorkConcept wc)
        {
            if (!checkBoxAllowMultipleStructureWindows.Checked)
            {
                CloseStructureWindows();
            }

            FormStructure formStructure = new FormStructure(userAccount, (DatabaseService)dataServ, wc.StructureId, this);
            FormStructure.ShowFormStructure(formStructure);

            if (checkBoxCenterOnSelected.Checked)
            {
                gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(wc.GeoLocation.LatitudeDecimal, wc.GeoLocation.LongitudeDecimal);
            }
        }

        /*
        internal void OpenStructure(WorkConcept wc)
        {
            // Get all work concepts for the given structure
            List<WorkConcept> workConcepts = new List<WorkConcept>();

            if (wc.FromEligibilityList)
            {
                workConcepts.AddRange(eligibleWorkConcepts.Where(el => el.StructureId.Equals(wc.StructureId) && el.FromEligibilityList));
            }
            else if (wc.FromProposedList)
            {
                workConcepts.AddRange(eligibleWorkConcepts.Where(el => el.StructureId.Equals(wc.StructureId) && el.FromProposedList));
            }
            else if (wc.FromFiips)
            {
                workConcepts.AddRange(eligibleWorkConcepts.Where(el => el.StructureId.Equals(wc.StructureId) && el.FromEligibilityList));
                workConcepts.AddRange(eligibleWorkConcepts.Where(el => el.StructureId.Equals(wc.StructureId) && el.FromProposedList));
            }

            if (wc.ProjectDbId == 0 && wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate)
            {
                workConcepts.Add(wc);
            }

            // Get work concepts for given structure already in structure projects
            var existingProjects = structureProjects.Where(sp => sp.WorkConcepts.Any(c => c.StructureId.Equals(wc.StructureId))).ToList();

            foreach (var ep in existingProjects)
            {
                workConcepts.AddRange(ep.WorkConcepts.Where(w => w.StructureId.Equals(wc.StructureId)));
            }

            // Add Fiips
            workConcepts.AddRange(fiipsWorkConcepts.Where(el => el.StructureId == wc.StructureId));
            OpenStructureForm(wc, workConcepts);

            // Update Near ME controls
            textBoxSearchByDistanceId.Text = wc.StructureId;

            if (wc.GeoLocation == null && wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0)
            {
                //wc.GeoLocation = dataServ.GetStructureGeoLocation(wc.StructureId);
                wc.GeoLocation = dataServ.GetStructureLatLong(wc.StructureId);
            }

            textBoxSearchByDistanceLatitude.Text = wc.GeoLocation.LatitudeDecimal.ToString();
            textBoxSearchByDistanceLongitude.Text = wc.GeoLocation.LongitudeDecimal.ToString();
        }*/

        /*
        private void OpenStructureForm(WorkConcept wc, List<WorkConcept> workConcepts)
        {
            WiSamEntities.Structure structure = null;
            FormStructure formStructure = null;

            try
            {
                formStructure = (FormStructure)Utility.GetForm("FormStructure");
            }
            catch { }

            if (formStructure == null)
            {
                formStructure = new FormStructure(userAccount, structure, wc.StructureId, workConcepts,
                                                    dataServ, primaryWorkConcepts, eligibleWorkConcepts,
                                                    quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                    precertifiedApprovedWorkConcepts, certifiedWorkConcepts, fiipsWorkConcepts,
                                                    structureProjects, fiipsProjects, allWorkConcepts,
                                                    this);
            }
            else
            {
                formStructure.SetStructure(structure, wc.StructureId, workConcepts, dataServ);
            }

            Utility.ShowFormStructure(formStructure);

            if (checkBoxCenterOnSelected.Checked)
            {
                gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(wc.GeoLocation.LatitudeDecimal, wc.GeoLocation.LongitudeDecimal);
            }
        }*/


        /*
        private void gMapControlStructuresMap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            WorkConcept wc = null;
            Project project = null;

            if (item.Tag is WorkConcept)
            {
                var w = true;
            }
            else if (item.Tag is Project)
            {
                var p = true;
            }

            // Determine whether marker is a Project or a Work Concept
            try
            {
                project = (Project)item.Tag;
            }
            catch { }

            if (project != null)
            {
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
                    formStructureProject.RefreshForm(project);
                }
                else
                {
                    formStructureProject = new FormStructureProject(userAccount, dataServ, project, primaryWorkConcepts, eligibleWorkConcepts,
                                                                    quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                    precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                    fiipsWorkConcepts, structureProjects, fiipsProjects, allWorkConcepts, this);
                }

                formStructureProject.TopMost = true;
                formStructureProject.StartPosition = FormStartPosition.CenterScreen;
                formStructureProject.WindowState = FormWindowState.Normal;
                formStructureProject.Show();

                if (checkBoxCenterOnSelected.Checked && project.GeoLocation != null && project.GeoLocation.latitude != 0 && project.GeoLocation.longitude != 0)
                {
                    gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(project.GeoLocation.latitude, project.GeoLocation.longitude);
                }

                return;
            }

            wc = (WorkConcept)item.Tag;
            List<WorkConcept> workConcepts = new List<WorkConcept>(); // for given structure

            if (
                wc.ProjectDbId == 0 && 
                    (wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate 
                    || wc.Status == StructuresProgramType.WorkConceptStatus.Propose)
               )
            {
                workConcepts.Add(wc);
            }

            workConcepts.AddRange(eligibleWorkConcepts.Where(el => el.StructureId.Equals(wc.StructureId)));
            List<int> projectsWithStructureWorkConcept = new List<int>();

            foreach (var esp in structureProjects)
            {
                foreach (var workConcept in esp.WorkConcepts)
                {
                    if (workConcept.StructureId.Equals(wc.StructureId))
                    {
                        workConcepts.Add(workConcept);

                        if (workConcept.WorkConceptCode.Equals(wc.WorkConceptCode))
                        {
                            projectsWithStructureWorkConcept.Add(esp.ProjectDbId);
                        }
                    }
                }
            }

            workConcepts = workConcepts.OrderBy(el => el.WorkConceptTimeStamp).ToList();
            workConcepts.AddRange(fiipsWorkConcepts.Where(el => el.StructureId == wc.StructureId).ToList());

            {
                textBoxStructureId.Text = wc.StructureId;
                textBoxLat.Text = wc.GeoLocation.latitude.ToString();
                textBoxLongitude.Text = wc.GeoLocation.longitude.ToString();
                bool formStructureIsOpen = false;
                FormStructure formStructure = null;

                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name.Equals("FormStructure"))
                    {
                        formStructureIsOpen = true;
                        formStructure = (FormStructure)form;
                    }
                }

                WiSamEntities.Structure structure = null;
                structure = dataServ.GetStructure(wc.StructureId);

                if (structure == null && checkBoxMappingError.Checked)
                {
                    MessageBox.Show("Unable to retrieve structure data for " + wc.StructureId + ".");
                }

                if (!formStructureIsOpen)
                {
                    formStructure = new FormStructure(userAccount, structure, wc.StructureId, workConcepts,
                                                        dataServ, primaryWorkConcepts, eligibleWorkConcepts,
                                                        quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                        precertifiedApprovedWorkConcepts, certifiedWorkConcepts, fiipsWorkConcepts,
                                                        structureProjects, fiipsProjects, allWorkConcepts,
                                                        this);
                }
                else
                {
                    formStructure.SetStructure(structure, wc.StructureId, workConcepts);
                }

                formStructure.TopMost = true; ;
                formStructure.WindowState = FormWindowState.Normal;
                formStructure.StartPosition = FormStartPosition.CenterScreen;
                formStructure.Show();

                if (checkBoxCenterOnSelected.Checked)
                {
                    gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(wc.GeoLocation.latitude, wc.GeoLocation.longitude);
                }
            }

            if (checkBoxSingleClickWorkConceptAdd.Checked)
            {
                
                //if ((wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate || wc.Status == StructuresProgramType.WorkConceptStatus.Eligible)
                        //&& workConcepts.Where(el => el.Status == StructuresProgramType.WorkConceptStatus.Unapproved
                                                    //|| el.Status == StructuresProgramType.WorkConceptStatus.Precertified
                                                    //|| el.Status == StructuresProgramType.WorkConceptStatus.Quasicertified
                                                    //|| el.Status == StructuresProgramType.WorkConceptStatus.Certified).Count() == 0)
                                                    
                if (wc.Status == StructuresProgramType.WorkConceptStatus.Evaluate || wc.Status == StructuresProgramType.WorkConceptStatus.Eligible)
                {
                    if (projectsWithStructureWorkConcept.Count() > 0)
                    {
                        string projectsList = "";
                        int counter = 0;

                        foreach (var p in projectsWithStructureWorkConcept.Distinct())
                        {
                            if (counter > 1)
                            {
                                projectsList += ",";
                            }

                            projectsList += p.ToString();
                            counter++;
                        }

                        DialogResult dialogResult = 
                            MessageBox.Show(String.Format("This work concept for this structure is already in project(s): {0}", projectsList),
                                "Alert", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (dialogResult == DialogResult.Yes)
                        {
                            gMapControlStructuresMap_OnMarkerDoubleClick(item, e);
                        }
                    }
                    else
                    {
                        gMapControlStructuresMap_OnMarkerDoubleClick(item, e);
                    }
                }
            }
        }
        */

        private void gMapControlStructuresMap_MouseClick(object sender, MouseEventArgs e)
        {
            var here = 0;
        }

        private void gMapControlStructuresMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var here = 0;
        }

        private void SetToolTipProperties(ToolTip toolTip, bool isBalloon, int autoPopDelay)
        {
            toolTip.IsBalloon = isBalloon;
            toolTip.AutoPopDelay = autoPopDelay;
        }

        private void FormMapping_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            splitContainer1.SplitterDistance = groupBoxMappingOptions.Height - 30;
            splitContainerStructuresMapping.SplitterDistance = pictureBoxSearchByDistance.Location.X + pictureBoxSearchByDistance.Width + 30;
            splitContainer2.SplitterDistance = buttonFindProjectsManuallyTransitionallyCertified.Location.Y + 64;
            checkedListBoxOwners.SetItemChecked(0, true);
            checkedListBoxStructureTypes.SetItemChecked(0, true);
            checkedListBoxStructureTypes.SetItemChecked(1, true);
            checkedListBoxDotPrograms.SetItemChecked(0, true);
            ToolTip idToolTip = new ToolTip();
            SetToolTipProperties(idToolTip, true, 10000);
            idToolTip.SetToolTip(pictureBoxSearchById, "Search work concepts and projects based on Structure Id, Construction Id or Structures Project Id.");
            ToolTip distanceToolTip = new ToolTip();
            SetToolTipProperties(distanceToolTip, true, 10000);
            string line1 = "Search structures and their work concepts and projects based on their distance from a given location.";
            string line2 = "Given location can be a structure, a structures project, a FIIPS project, or a user marker.";
            string line3 = "In addition to structures work concepts and projects, search can include Fiips.";
            string searchByDistanceText = String.Format("{0}{1}{2}{3}{4}", line1, Environment.NewLine, line2, Environment.NewLine, line3);
            distanceToolTip.SetToolTip(pictureBoxSearchByDistance, searchByDistanceText);

            // Search by certification status tool tips
            ToolTip certificationToolTip = new ToolTip();
            SetToolTipProperties(certificationToolTip, true, 10000);
            certificationToolTip.SetToolTip(pictureBoxSearchByCertificationStatus, "Search structures projects based on their status in the certification process.");
            ToolTip savedToolTip = new ToolTip();
            SetToolTipProperties(savedToolTip, true, 10000);
            string savedNotSubmittedText = String.Format("Project's created, but not submitted to BOS for review.{0}(Ball in Court: Region)", Environment.NewLine);
            savedToolTip.SetToolTip(buttonFindProjectsPendingSubmittal, savedNotSubmittedText);
            ToolTip submittedToolTip = new ToolTip();
            SetToolTipProperties(submittedToolTip, true, 10000);
            string submittedText = String.Format("Project's submitted to BOS for precertification review.{0}(Ball in Court: BOS)", Environment.NewLine);
            submittedToolTip.SetToolTip(buttonFindProjectsPendingPrecertification, submittedText);
            ToolTip precertifiedToolTip = new ToolTip();
            SetToolTipProperties(precertifiedToolTip, true, 10000);
            string precertifiedText = String.Format("Project's submitted and precertified; its certification is pending.{0}(Ball in Court: BOS)", Environment.NewLine);
            precertifiedToolTip.SetToolTip(buttonFindProjectsPendingCertification, precertifiedText);
            ToolTip rejectedToolTip = new ToolTip();
            SetToolTipProperties(rejectedToolTip, true, 10000);
            string rejectedText = String.Format("Project's been rejected for precertification or certification; it requires modification by the region.{0}(Ball in Court: Region)", Environment.NewLine);
            rejectedToolTip.SetToolTip(buttonFindRejectedProjects, rejectedText);
            ToolTip certifiedToolTip = new ToolTip();
            SetToolTipProperties(certifiedToolTip, true, 10000);
            string certifiedText = String.Format("Project's certified through the full certification process.{0}Coordination with BOS is complete unless there are changes to scope or timing.", Environment.NewLine);
            precertifiedToolTip.SetToolTip(buttonFindProjectsCertified, certifiedText);
            ToolTip transitionallyCertifiedToolTip = new ToolTip();
            SetToolTipProperties(transitionallyCertifiedToolTip, true, 15000);
            string transitionallyCertifiedText = String.Format("Project's certified through an abridged 'transitional' certification process because of the proximity{0}of its let date to the implementation date of the BOS structures management initiative.{1}Coordination with BOS is complete unless there are changes to scope or timing.", Environment.NewLine, Environment.NewLine);
            transitionallyCertifiedToolTip.SetToolTip(buttonFindProjectsTransitionallyCertified, transitionallyCertifiedText);
        }

        private void ToggleSearchButtons(bool enabled)
        {
            buttonFindStructureId.Enabled = enabled;
            buttonFindFosId.Enabled = enabled;
            buttonFindProjectId.Enabled = enabled;
            buttonCenterMap.Enabled = enabled;
            buttonFindNearMe.Enabled = enabled;
            buttonClearMarkers.Enabled = enabled;
            
        }

        private void FindStructureId(string structureId, bool externalSearch = false)
        {
            if (!checkBoxRetainHits.Checked)
            {
                checkBoxClearMapStatus.Checked = true;
                ClearAllMarkers();
                checkBoxClearMapStatus.Checked = false;
            }

            OpenLoading();
            ToggleMainSearchButtons(false);
            structureIdHits = new List<string>();
            ClearSearchResults();

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                startingNode = tn;
                FindStructureRecursive(tn, structureId);
            }

            if (structureIdHits.Count > 0)
            {
                textBoxStructuresResultsNumberOfStructures.Text = structureIdHits.Count().ToString();
                //GeoLocation gl = dataServ.GetStructureGeoLocation(structureIdHits.First());
                GeoLocation gl = dataServ.GetStructureLatLong(structureIdHits.First());

                if (gl != null && gl.LatitudeDecimal != 0 && gl.LongitudeDecimal != 0)
                {
                    gMapControlStructuresMap.Position = new PointLatLng(gl.LatitudeDecimal, gl.LongitudeDecimal);
                }
                else
                {
                    MessageBox.Show(String.Format("Unable to map {0}", structureIdHits.First()), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                //GeoLocation gl = dataServ.GetStructureGeoLocation(structureId);
                GeoLocation gl = dataServ.GetStructureLatLong(structureId);

                if (gl != null && gl.LatitudeDecimal != 0 && gl.LongitudeDecimal != 0)
                {
                    TreeNode aNode = treeViewMapping.Nodes["NearMes"];
                    TreeNode bNode = new TreeNode(String.Format("Structures with No Work Concepts"));
                    bNode.Tag = String.Format("StructuresNoWorkConcepts");
                    bool bNodeExistsAlready = false;

                    foreach (TreeNode node in aNode.Nodes)
                    {
                        if (node.Tag.Equals(bNode.Tag))
                        {
                            bNodeExistsAlready = true;
                            bNode = node;
                            break;
                        }
                    }

                    if (!bNodeExistsAlready)
                    {
                        aNode.Nodes.Add(bNode);
                    }

                    WorkConcept wc = new WorkConcept();
                    wc.StructureId = structureId;
                    wc.WorkConceptDbId = -1;
                    wc.ProjectDbId = 0;
                    wc.MapMarkerType = GMarkerGoogleType.black_small;
                    wc.Status = StructuresProgramType.WorkConceptStatus.Proposed;
                    wc.GeoLocation = dataServ.GetStructureLatLong(structureId);
                    wc.FromEligibilityList = false;
                    wc.FromFiips = false;
                    wc.FromProposedList = true;
                    wc.WorkConceptCode = "PR";
                    wc.WorkConceptDescription = "?";
                    wc.Region = wc.GeoLocation.Region;
                    wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                    wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                    List<WorkConcept> workConceptsFound = new List<WorkConcept>() { wc };
                    RenderTreeWorkConcepts(bNode, workConceptsFound, false);
                    //startingNode = bNode;

                    foreach (TreeNode node in bNode.Nodes)
                    {
                        WorkConcept w = (WorkConcept)node.Tag;

                        if (w.StructureId.Equals(wc.StructureId))
                        {
                            node.Checked = true;
                        }
                    }

                    aNode.Expand();
                    bNode.Expand();

                    if (!externalSearch)
                    {
                        textBoxStructuresResultsNumberOfStructures.Text = "1";
                    }
                }
                else
                {
                    if (!externalSearch)
                    {
                        textBoxStructuresResultsNumberOfStructures.Text = "0";
                    }
                }
            }

            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void buttonFindStructureId_Click(object sender, EventArgs e)
        {
            string structureId = textBoxSearchByIdStructureId.Text.ToUpper().Trim();
            string translatedStructureId = Utility.TranslateStructureId(structureId);

            if (translatedStructureId.Length != 7 && translatedStructureId.Length != 11)
            {
                MessageBox.Show("Invalid Structure Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FindStructureId(translatedStructureId);
            }
        }

        private void ClearMarkers()
        {
            foreach (var o in gMapControlStructuresMap.Overlays)
            {
                foreach (var marker in o.Markers)
                {
                    marker.IsVisible = false;
                }
            }

            gMapControlStructuresMap.Refresh();
        }

        internal void ClearAllMarkers()
        {
            userMarkerCount = 0;

            foreach (var olay in gMapControlStructuresMap.Overlays)
            {
                olay.Markers.Clear();
            }

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                ResetTreeNodeRecursive(tn);
                tn.Collapse();
                tn.Checked = false;
            }
        }

        internal void ResetTree()
        {
            if (!checkBoxRetainHits.Checked)
            {
                foreach (TreeNode tn in treeViewMapping.Nodes)
                {
                    ResetTreeNodeRecursive(tn);
                    tn.Collapse();
                }
            }
        }

        private void ResetTreeNodeRecursive(TreeNode treeNode)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Checked)
                {
                    tn.Checked = false;
                }

                if (!(tn.Tag is WorkConcept))
                {
                    ResetTreeNodeRecursive(tn);
                }
            }
        }

        private void FindProjectRecursive(TreeNode treeNode, int structuresProjectId)
        {
            controller.FindProjectRecursive(treeNode, structuresProjectId, projectHits, projectIdHits, fiipsProjects, fiipsIdHits);
        }

        private void FindProjectWithGivenUserActionRecursive(TreeNode treeNode, StructuresProgramType.ProjectUserAction userAction)
        {
            controller.FindProjectWithGivenUserActionRecursive(treeNode, userAction, projectIdHits);
        }

        private void FindProjectWithGivenStatusRecursive(TreeNode treeNode, StructuresProgramType.ProjectStatus projectStatus, bool fromExcel = false)
        {
            controller.FindProjectWithGivenStatusRecursive(treeNode, projectStatus, projectIdHits, fromExcel);
        }

        private void FindFosRecursive(TreeNode treeNode, string fosId)
        {
            controller.FindFosRecursive(treeNode, fosId, projectHits, fiipsIdHits, projectIdHits);
        }

        private void RemoveWorkConceptRecursive(TreeNode treeNode, WorkConcept wc)
        {
            controller.RemoveWorkConceptRecursive(treeNode, wc, gMapControlStructuresMap);
        }

        private void FindStructuresInMultipleStructuresProjects(TreeNode treeNode)
        {
            controller.FindStructuresInMultipleStructuresProjects(treeNode, structureProjects, structureIdHits);
        }

        private void FindStructureRecursive(TreeNode treeNode, string structureId)
        {
            controller.FindStructureRecursive(treeNode, structureId, structureIdHits);
        }

        private void ExpandNodeRecursive(TreeNode treeNode)
        {
            controller.ExpandNodeRecursive(treeNode);
        }

        internal void SearchStructure(string structureId)
        {
            FindStructureId(structureId, true);
        }

        internal void SearchProject(string constructionId, bool updateResults = true)
        {
            FindFiipsProject(constructionId, updateResults);
        }

        internal void SearchProject(int structuresProjectId, bool updateResults = true)
        {
            FindStructuresProject(structuresProjectId);
            //textBoxHitCount.Text = "0";

            /*
            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                FindProjectRecursive(tn, projectId);
            }*/
        }

        private void FindFiipsProject(string constructionId, bool updateResults = true)
        {
            if (!checkBoxRetainHits.Checked)
            {
                checkBoxClearMapStatus.Checked = true;
                ClearAllMarkers();
                checkBoxClearMapStatus.Checked = false;
            }

            OpenLoading();
            ToggleMainSearchButtons(false);
            fiipsIdHits = new List<string>();
            projectIdHits = new List<int>();
            projectHits = new List<Project>();
            ClearSearchResults();

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                startingNode = tn;
                FindFosRecursive(tn, constructionId);
            }

            List<Project> structuresProjects = projectHits.Where(p => p.Status != StructuresProgramType.ProjectStatus.Fiips).GroupBy(p => p.ProjectDbId).Select(g => g.First()).ToList();
            List<Project> fiipsProjects = projectHits.Where(p => p.Status == StructuresProgramType.ProjectStatus.Fiips).GroupBy(p => p.FosProjectId).Select(g => g.First()).ToList();

            if (updateResults)
            {
                textBoxStructuresResultsNumberOfProjects.Text = structuresProjects.Count().ToString();
                textBoxStructuresResultsNumberOfStructures.Text = structuresProjects.Sum(p => p.NumberOfStructures).ToString();
                textBoxFiipsResultsNumberOfProjects.Text = fiipsProjects.Count().ToString();
                textBoxFiipsResultsNumberOfStructures.Text = fiipsProjects.Sum(p => p.NumberOfStructures).ToString();
            }

            if (fiipsProjects.Count() > 0)
            {
                Project p = fiipsProjects.First();

                try
                {
                    gMapControlStructuresMap.Position = new PointLatLng(p.GeoLocation.LatitudeDecimal, p.GeoLocation.LongitudeDecimal);
                }
                catch { }
            }

            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void buttonFindFosId_Click(object sender, EventArgs e)
        {
            string constructionId = textBoxSearchByIdFosId.Text.ToUpper().Trim().Replace("-", "");
            
            if (constructionId.Length != 8)
            {
                MessageBox.Show("Invalid Construction Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FindFiipsProject(constructionId);
            }
        }

        private void buttonResetMap_Click(object sender, EventArgs e)
        {
            gMapControlStructuresMap.Zoom = 8;
            gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(startingLatitude, startingLongitude);
        }

        /*
        private void gMapControlStructuresMap_MouseDoubleClick_1(object sender, MouseEventArgs e)
        {
            return; // Intentional to not handle this event
            GMapControl map = (GMapControl)sender;

            if (map.IsMouseOverMarker)
            {
                bool proceed = true;
                WorkConcept wc = null;

                for (int i = startYear; i <= endYear; i++)
                {
                    var o = map.Overlays.Where(el => el.Id.Equals(i.ToString())).First();

                    foreach (var m in o.Markers)
                    {
                        if (m.IsMouseOver && m.Tag is WorkConcept)
                        {
                            wc = (WorkConcept)m.Tag;
                            proceed = false;
                            break;
                        }
                    }

                    if (!proceed)
                    {
                        break;
                    }
                }

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
                    formStructureProject.RefreshForm(wc);
                }
                else
                {
                    formStructureProject = new FormStructureProject(userAccount, dataServ, wc, primaryWorkConcepts, eligibleWorkConcepts,
                                                                    quasicertifiedWorkConcepts, precertifiedUnapprovedWorkConcepts,
                                                                    precertifiedApprovedWorkConcepts, certifiedWorkConcepts,
                                                                    fiipsWorkConcepts, structureProjects, fiipsProjects, allWorkConcepts,
                                                                    this, treeViewMapping);
                }

                formStructureProject.TopMost = false;
                formStructureProject.WindowState = FormWindowState.Normal;
                formStructureProject.Show();
            }
        }*/

        private void buttonClearMarkers_Click(object sender, EventArgs e)
        {
            ClearSearchResults();
            checkBoxClearMapStatus.Checked = true;
            ClearAllMarkers();
            gMapControlStructuresMap.Position = new PointLatLng(startingLatitude, startingLongitude);
            checkBoxClearMapStatus.Checked = false;
        }

        private void comboBoxMapProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;

            if (comboBox.SelectedIndex == 0)
            {
                gMapControlStructuresMap.MapProvider = GoogleMapProvider.Instance;
            }
            else
            {
                gMapControlStructuresMap.MapProvider = BingHybridMapProvider.Instance;
            }
        }

        public static float ConvertRadiansToDegrees(double radians)
        {
            return controller.ConvertRadiansToDegrees(radians);
        }

        private void SearchByDistance(string id, StructuresProgramType.ObjectType objectType, float latitude, float longitude, double givenRadius = 0)
        {
            bool includeStateBridges = false;
            bool includeLocalBridges = false;

            if (checkedListBoxOwners.CheckedItems.Count == 0)
            {
                MessageBox.Show("Check at least State or Local.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                foreach (var item in checkedListBoxOwners.CheckedItems)
                {
                    if (item.Equals("State"))
                    {
                        includeStateBridges = true;
                    }
                    else if (item.Equals("Local"))
                    {
                        includeLocalBridges = true;
                    }
                }
            }

            string structureTypes = "'-1'";

            if (checkedListBoxStructureTypes.CheckedItems.Count == 0)
            {
                MessageBox.Show("Check at least one Structure Type.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                foreach (var item in checkedListBoxStructureTypes.CheckedItems)
                {
                    switch (item)
                    {
                        case "B":
                            structureTypes += ",'B'";
                            break;
                        case "P":
                            structureTypes += ",'P'";
                            break;
                        case "Culvert":
                            structureTypes += ",'C'";
                            break;
                        case "Sign":
                            structureTypes += ",'S'";
                            break;
                        case "Noise":
                            structureTypes += ",'N'";
                            break;
                        case "Retaining":
                            structureTypes += ",'R'";
                            break;
                        case "Monotube":
                            structureTypes += ",'G'";
                            break;
                        case "Hml":
                            structureTypes += ",'L'";
                            break;
                        case "Misc":
                            structureTypes += ",'M'";
                            break;
                    }
                }
            }

            float radius = 5;

            try
            {
                radius = Convert.ToSingle(textBoxWithinDistance.Text.Trim());

                if (radius == 0)
                {
                    radius = Convert.ToSingle(".001");
                }
            }
            catch { }

            if (givenRadius != 0)
            {
                radius = Convert.ToSingle(givenRadius);
            }

            OpenLoading();
            ToggleSearchButtons(false);
            ClearSearchResults();
            List<string> structuresFound = dataServ.FindStructuresNearMe(id, objectType, latitude, longitude, radius, structureTypes, includeStateBridges, includeLocalBridges, textBoxOffice.Text);
            List<WorkConcept> workConceptsFound = new List<WorkConcept>();
            List<Project> structuresProjectsFound = new List<Project>();
            List<WorkConcept> fiipsWorkConceptsFound = new List<WorkConcept>();
            List<Project> fiipsProjectsFound = new List<Project>();
            TreeNode aNode = treeViewMapping.Nodes["NearMes"];
            TreeNode bNode = new TreeNode(String.Format("{0} - {1} mi radius", id, radius));
            bNode.Tag = String.Format("SearchByDistance:{0}:{1}", objectType, id);
            aNode.Nodes.Add(bNode);

            foreach (string structureId in structuresFound.OrderBy(s => s))
            {
                var matchingWorkConcepts = new List<WorkConcept>();
                var matchingStructuresProjects =
                    (from project in structureProjects
                    where project.WorkConcepts.Any(wc => wc.StructureId.Equals(structureId))
                    select project).ToList();

                if (matchingStructuresProjects.Count > 0)
                {
                    structuresProjectsFound.AddRange(matchingStructuresProjects);
                    
                    foreach (var project in matchingStructuresProjects)
                    {
                        foreach (var wc in project.WorkConcepts.Where(c => c.StructureId.Equals(structureId)))
                        {
                            matchingWorkConcepts.Add(wc);
                        }
                    }
                }
                else
                {
                    var matchingProposed =
                        (from wc in eligibleWorkConcepts
                        where wc.StructureId.Equals(structureId)
                            && wc.FromProposedList
                        select wc).ToList();

                    if (matchingProposed.Count > 0)
                    {
                        foreach (var wc in matchingProposed)
                        {
                            matchingWorkConcepts.Add(wc);
                        }
                    }
                    else
                    {
                        var matchingEligible =
                            (from wc in eligibleWorkConcepts
                             where wc.StructureId.Equals(structureId)
                                 && wc.FromEligibilityList
                             select wc).ToList();

                        if (matchingEligible.Count > 0)
                        {
                            foreach (var wc in matchingEligible)
                            {
                                matchingWorkConcepts.Add(wc);
                            }
                        }
                        else
                        {
                            WorkConcept wc = new WorkConcept();
                            wc.StructureId = structureId;
                            wc.WorkConceptDbId = -1;
                            wc.ProjectDbId = 0;
                            wc.MapMarkerType = GMarkerGoogleType.black_small;
                            wc.Status = StructuresProgramType.WorkConceptStatus.Proposed;
                            wc.GeoLocation = dataServ.GetStructureLatLong(structureId);
                            wc.FromEligibilityList = false;
                            wc.FromFiips = false;
                            wc.FromProposedList = true;
                            wc.WorkConceptCode = "PR";
                            wc.WorkConceptDescription = "?";
                            wc.Region = wc.GeoLocation.Region;
                            wc.CertifiedWorkConceptCode = wc.WorkConceptCode;
                            wc.CertifiedWorkConceptDescription = wc.WorkConceptDescription;
                            matchingWorkConcepts.Add(wc);
                        }
                    }
                }

                workConceptsFound.AddRange(matchingWorkConcepts);
                var matchingFiipsProjects =
                   (from project in fiipsProjects
                    where project.WorkConcepts.Any(wc => wc.StructureId.Equals(structureId))
                    select project).ToList();

                if (matchingFiipsProjects.Count > 0)
                {
                    fiipsProjectsFound.AddRange(matchingFiipsProjects);

                    foreach (var project in matchingFiipsProjects)
                    {
                        foreach (var wc in project.WorkConcepts.Where(c => c.StructureId.Equals(structureId)))
                        {
                            fiipsWorkConceptsFound.Add(wc);
                        }
                    }
                }
            }

            // Add matching structure work concepts to the tree
            RenderTreeWorkConcepts(bNode, workConceptsFound, false);
            startingNode = bNode;

            foreach (TreeNode node in bNode.Nodes)
            {
                node.Checked = true;
            }
            //bNode.Checked = true;
            /*
            foreach (var wc in workConceptsFound)
            {
                foreach (TreeNode tn in bNode.Nodes)
                {
                    //startingNode = tn;
                    FindStructureRecursive(tn, wc.StructureId);
                }
            }*/
            aNode.Expand();

            textBoxStructuresResultsNumberOfStructures.Text = structuresFound.Count.ToString();
            List<Project> distinctStructuresProjectsFound = structuresProjectsFound.GroupBy(x => new { x.ProjectDbId, x.FosProjectId }).Select(g => g.First()).ToList();
            textBoxStructuresResultsNumberOfProjects.Text = distinctStructuresProjectsFound.Count().ToString();
            textBoxFiipsResultsNumberOfStructures.Text = fiipsWorkConceptsFound.GroupBy(w => w.StructureId).Select(g => g.First()).Count().ToString();
            List<Project> distinctFiipsProjectsFound = fiipsProjectsFound.GroupBy(f => f.FosProjectId).Select(g => g.First()).ToList();
            textBoxFiipsResultsNumberOfProjects.Text = distinctFiipsProjectsFound.Count.ToString();

            foreach (Project project in distinctStructuresProjectsFound)
            {
                TreeNode cNode = new TreeNode(String.Format("Str Proj Id {0}{1}", project.ProjectDbId, project.FosProjectId.Length > 0 ? " : Cnst Id " + FormatConstructionId(project.FosProjectId) : ""));
                cNode.Tag = project;
                bNode.Nodes.Add(cNode);
                ColorProjectNode(cNode, project.Status);
                RenderTreeWorkConcepts(cNode, project.WorkConcepts, false);
                cNode.Checked = checkBoxSearchByDistanceShowStructuresProjects.Checked;

                if (!cNode.Checked && objectType == StructuresProgramType.ObjectType.StructuresProject && project.ProjectDbId == Convert.ToInt32(id))
                {
                    cNode.Checked = true;
                }
            }

            RenderTreeWorkConcepts(bNode, fiipsWorkConceptsFound.GroupBy(f => f.WorkConceptDbId).Select(g => g.First()).ToList(), false);
           
            foreach (Project project in distinctFiipsProjectsFound)
            {
                TreeNode cNode = new TreeNode(String.Format("Cnst Id {0}", FormatConstructionId(project.FosProjectId)));
                cNode.Tag = project;
                bNode.Nodes.Add(cNode);
                ColorProjectNode(cNode, project.Status);
                RenderTreeWorkConcepts(cNode, project.WorkConcepts, false);
                cNode.Checked = checkBoxSearchByDistanceShowFiips.Checked;

                if (!cNode.Checked && objectType == StructuresProgramType.ObjectType.FiipsProject && project.FosProjectId.Equals(id))
                {
                    cNode.Checked = true;
                }
            }

            bNode.Expand();
            gMapControlStructuresMap.ZoomAndCenterMarkers("NearMe");
            float latPoint = 0;
            float longPoint = 0;

            try
            {
                latPoint = Convert.ToSingle(textBoxSearchByDistanceLatitude.Text);
            }
            catch { }

            try
            {
                longPoint = Convert.ToSingle(textBoxSearchByDistanceLongitude.Text);
            }
            catch { }

            if (latPoint != 0 && longPoint != 0)
            {
                gMapControlStructuresMap.Position = new PointLatLng(latPoint, longPoint);
            }

            ToggleSearchButtons(true);
            startingNode = null;
            CloseLoading();
        }

        private bool ValidateSearchByDistanceId()
        {
            bool isValid = true;
            string givenId = textBoxSearchByDistanceId.Text.Trim().ToUpper();
            string translatedStructureId = Utility.TranslateStructureId(givenId);
            string id = textBoxSearchByDistanceId.Text.Replace("-", "").Trim().ToUpper();
            float latitude = 0;
            float longitude = 0;

            if (id.Length == 0)
            {
                isValid = false;
            }
            else if (translatedStructureId.Length == 7 || translatedStructureId.Length == 11)
            {
                GeoLocation gl = dataServ.GetStructureLatLong(translatedStructureId);
                radioButtonSearchByStructureId.Checked = true;

                if (gl != null)
                {
                    latitude = gl.LatitudeDecimal;
                    longitude = gl.LongitudeDecimal;
                }
            }
            else if (id.Length == 8)
            {
                //var projects = fiipsLayer.Markers.Where(m => m.Tag != null && m.Tag is Project && ((Project)m.Tag).FosProjectId.Equals(id) && ((Project)m.Tag).GeoLocation != null);
                var projects = fiipsProjects.Where(p => p.FosProjectId.Equals(id) && p.GeoLocation != null);
                radioButtonSearchByFiipsId.Checked = true;

                if (projects.Count() > 0)
                {
                    var project = projects.First();
                    latitude = project.GeoLocation.LatitudeDecimal;
                    longitude = project.GeoLocation.LongitudeDecimal;
                }
            }
            else if (id.ToUpper().StartsWith("SCT MARKER"))
            {
                var markers = nearMeLayer.Markers.Where(m => m.Tag != null && m.Tag.ToString().ToUpper().Equals(id.ToUpper()));
                radioButtonSearchByUserMarker.Checked = true;

                if (markers.Count() > 0)
                {
                    var marker = markers.First();
                    latitude = Convert.ToSingle(marker.Position.Lat);
                    longitude = Convert.ToSingle(marker.Position.Lng);
                }
            }
            else // Structures Project Id
            {
                //var projects = projectsLayer.Markers.Where(m => m.Tag != null && m.Tag is Project && ((Project)m.Tag).ProjectDbId.ToString().Equals(id) && ((Project)m.Tag).GeoLocation != null);
                var projects = structureProjects.Where(p => p.ProjectDbId.ToString().Equals(id) && p.GeoLocation != null);
                radioButtonSearchByProjectId.Checked = true;

                if (projects.Count() > 0)
                {
                    var project = projects.First();
                    latitude = project.GeoLocation.LatitudeDecimal;
                    longitude = project.GeoLocation.LongitudeDecimal;
                }
            }

            if (latitude == 0 && longitude == 0)
            {
                isValid = false;
            }
            else
            {
                textBoxSearchByDistanceLatitude.Text = latitude.ToString();
                textBoxSearchByDistanceLongitude.Text = longitude.ToString();
            }

            return isValid;
        }

        private void FindNearMe()
        {
            string id = textBoxSearchByDistanceId.Text.Trim();

            if (!ValidateSearchByDistanceId())
            {
                MessageBox.Show(String.Format("Unable to locate {0} on the map.", id), "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var latitude = Convert.ToSingle(textBoxSearchByDistanceLatitude.Text);
            var longitude = Convert.ToSingle(textBoxSearchByDistanceLongitude.Text);
            var objectType = StructuresProgramType.ObjectType.WorkConcept;

            if (radioButtonSearchByStructureId.Checked)
            {
                objectType = StructuresProgramType.ObjectType.WorkConcept;
                SearchByDistance(Utility.TranslateStructureId(id), objectType, latitude, longitude);
            }
            else if (radioButtonSearchByFiipsId.Checked)
            {
                objectType = StructuresProgramType.ObjectType.FiipsProject;
                SearchByDistance(id, objectType, latitude, longitude);
            }
            else if (radioButtonSearchByProjectId.Checked)
            {
                objectType = StructuresProgramType.ObjectType.StructuresProject;
                SearchByDistance(id, objectType, latitude, longitude);
            }
            else if (radioButtonSearchByUserMarker.Checked)
            {
                objectType = StructuresProgramType.ObjectType.UserMarker;
                SearchByDistance(id, objectType, latitude, longitude);
            }
        }

        private void buttonFindNearMe_Click(object sender, EventArgs e)
        {
            FindNearMe();
        }

        private void buttonCenterMap_Click(object sender, EventArgs e)
        {
            float latPoint = 0;
            float longPoint = 0;

            try
            {
                latPoint = Convert.ToSingle(textBoxSearchByDistanceLatitude.Text);
            }
            catch { }

            try
            {
                longPoint = Convert.ToSingle(textBoxSearchByDistanceLongitude.Text);
            }
            catch { }

            if (latPoint != 0 && longPoint != 0)
            {
                gMapControlStructuresMap.Position = new PointLatLng(latPoint, longPoint);
            }
        }

        private void textBoxStructureId_KeyDown(object sender, KeyEventArgs e)
        {
            var here = 0;
        }

        private void textBoxStructureId_TextChanged(object sender, EventArgs e)
        {
            var here = 0;
        }

        private void textBoxStructureId_Leave(object sender, EventArgs e)
        {
            if (textBoxSearchByDistanceId.Text.Trim().Length > 0)
            {
                //textBoxSearchByDistanceLatitude.Text = "";
                //textBoxSearchByDistanceLongitude.Text = "";
            }
        }

        private void radioButtonMapChoice_Click(object sender, EventArgs e)
        {
            var checkedButton = groupBox5.Controls.OfType<RadioButton>()
                                      .FirstOrDefault(r => r.Checked);
            
            switch (checkedButton.Text)
            {
                case "Google":
                    gMapControlStructuresMap.MapProvider = GoogleMapProvider.Instance;
                    break;
                case "Hybrid":
                    gMapControlStructuresMap.MapProvider = BingHybridMapProvider.Instance;
                    break;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FormScot formScot = new FormScot();
            formScot.TopMost = false;
            formScot.WindowState = FormWindowState.Maximized;
            formScot.StartPosition = FormStartPosition.CenterScreen;
            formScot.Show();
        }

        private void FindStructuresProject(int structuresProjectId, bool updateResults = true)
        {
            if (!checkBoxRetainHits.Checked)
            {
                checkBoxClearMapStatus.Checked = true;
                ClearAllMarkers();
                checkBoxClearMapStatus.Checked = false;
            }

            OpenLoading();
            ToggleMainSearchButtons(false);
            fiipsIdHits = new List<string>();
            projectIdHits = new List<int>();
            projectHits = new List<Project>();
            ClearSearchResults();

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                startingNode = tn;
                FindProjectRecursive(tn, structuresProjectId);
            }

            List<Project> structuresProjects = projectHits.Where(p => p.Status != StructuresProgramType.ProjectStatus.Fiips).GroupBy(p => p.ProjectDbId).Select(g => g.First()).ToList();
            List<Project> fiipsProjects = projectHits.Where(p => p.Status == StructuresProgramType.ProjectStatus.Fiips).GroupBy(p => p.FosProjectId).Select(g => g.First()).ToList();

            if (updateResults)
            {
                textBoxStructuresResultsNumberOfProjects.Text = structuresProjects.Count().ToString();
                textBoxStructuresResultsNumberOfStructures.Text = structuresProjects.Sum(p => p.NumberOfStructures).ToString();
                textBoxFiipsResultsNumberOfProjects.Text = fiipsProjects.Count().ToString();
                textBoxFiipsResultsNumberOfStructures.Text = fiipsProjects.Sum(p => p.NumberOfStructures).ToString();
            }

            if (structuresProjects.Count() > 0)
            {
                Project p = structuresProjects.First();

                try
                {
                    gMapControlStructuresMap.Position = new PointLatLng(p.GeoLocation.LatitudeDecimal, p.GeoLocation.LongitudeDecimal);
                }
                catch { }
            }

            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void buttonFindProjectId_Click(object sender, EventArgs e)
        {
            int structuresProjectId = 0;

            try
            {
                structuresProjectId = Convert.ToInt32(textBoxSearchByIdProjectId.Text.ToUpper().Trim());
            }
            catch { }

            if (structuresProjectId == 0)
            {
                MessageBox.Show("Invalid Structures Project Id", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                FindStructuresProject(structuresProjectId);
            }
        }

        private void buttonZoomIn_Click(object sender, EventArgs e)
        {
            gMapControlStructuresMap.Zoom++;
        }

        private void buttonZoomOut_Click(object sender, EventArgs e)
        {
            gMapControlStructuresMap.Zoom--;
        }

        private void radioButtonMapChoice(object sender, EventArgs e)
        {

        }

        private void contextMenuStripForNavTree_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            var tn = treeViewMapping.SelectedNode;
            switch (e.ClickedItem.Text)
            {
                case "View":
                    if (tn.Tag is Project)
                    {
                        OpenStructureProject((Project)tn.Tag);
                    }
                    else if (tn.Tag is WorkConcept)
                    {
                        WorkConcept wc = (WorkConcept)tn.Tag;
                        textBoxSearchByDistanceId.Text = wc.StructureId;
                        //OpenStructure(wc);
                        OpenStructureWindow(wc);
                    }
                    break;
                case "NearMe":

                    break;
                case "Delete": // For proposed wc
                    if (tn.Tag is WorkConcept)
                    {
                        WorkConcept wc = (WorkConcept)tn.Tag;

                        if (formMainController.IsProposedWorkConceptInAProject(wc.WorkConceptDbId, wc.StructureId))
                        {
                            DialogResult dr = MessageBox.Show("You're about to delete a proposed work concept that's already in a project. Continue with the deletion?", "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (dr == DialogResult.Yes)
                            {
                                formMainController.DeleteProposedWorkConcept(wc.WorkConceptDbId, wc.StructureId, wc.FiscalYear, -1);
                            }
                        }
                        else
                        {
                            formMainController.DeleteProposedWorkConcept(wc.WorkConceptDbId, wc.StructureId, wc.FiscalYear, -1);
                        }
                    }
                    break;
            }
        }

        private void textBoxStructureId_KeyDown_1(object sender, KeyEventArgs e)
        {

        }

        private void gMapControlStructuresMap_OnMarkerEnter(GMapMarker item)
        {
            if (item.Tag is Project)
            {
                ToggleProjectRoute(item, true);
            }
        }

        private void gMapControlStructuresMap_OnMarkerLeave(GMapMarker item)
        {
            if (item.Tag is Project)
            {
                ToggleProjectRoute(item, false);
            }
        }

        private void ToggleProjectRoute(GMapMarker item, bool toggleOn)
        {
            controller.ToggleProjectRoute(item, toggleOn, gMapControlStructuresMap);
        }

        private void gMapControlStructuresMap_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            contextMenuStripForMapMarkers.Items["toolStripMenuItemToggleOnProjectLines"].Visible = false;
            contextMenuStripForMapMarkers.Items["toolStripMenuItemToggleOffProjectLines"].Visible = false;
            contextMenuStripForMapMarkers.Items["toolStripMenuItemCenterOnMarker"].Visible = false;
            contextMenuStripForMapMarkers.Items["toolStripMenuItemSearchByDistance"].Visible = false;
            contextMenuStripForMapMarkers.Items["toolStripMenuItemPlaceMarker"].Visible = false;
            contextMenuStripForMapMarkers.Items["toolStripMenuItemProposeWorkConcept"].Visible = false;
            */

            foreach (ToolStripItem item in contextMenuStripForMapMarkers.Items)
            {
                item.Visible = false;
            }

            // Handle only right-click moused over on a Project
            if (e.Button != MouseButtons.Right)
            {
                return;
            }

            if (gMapControlStructuresMap.IsMouseOverMarker)
            {
                mousedOverMarker = null;
                //contextMenuStripForMapMarkers.Items["toolStripMenuItemToggleOnProjectLines"].Visible = false;
                //contextMenuStripForMapMarkers.Items["toolStripMenuItemToggleOffProjectLines"].Visible = false;
                //contextMenuStripForMapMarkers.Items["toolStripMenuItemCenterOnMarker"].Visible = false;
                //contextMenuStripForMapMarkers.Items["toolStripMenuItemPlaceMarker"].Visible = false;

                List<GMapOverlay> overlays = new List<GMapOverlay>();
                overlays.Add(gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Structure")).First());
                overlays.Add(gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Projects")).First());
                overlays.Add(gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("NearMe")).First());
                overlays.Add(gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Fiips")).First());
                //gMapControlStructuresMap.Overlays
                foreach (var olay in overlays)
                {
                    if (olay.Markers.Any(m => m.IsMouseOver & m.IsVisible))
                    {
                        var markers = olay.Markers.Where(m => m.IsMouseOver && m.IsVisible);
                        var marker = markers.First();
                        mousedOverMarker = markers.First();
                        contextMenuStripForMapMarkers.Items["toolStripMenuItemCenterOnMarker"].Visible = true;
                        contextMenuStripForMapMarkers.Items["toolStripMenuItemSearchByDistance"].Visible = true;

                        if (marker.Tag is Project)
                        {
                            Project project = (Project)marker.Tag;

                            if (project.IsProjectRouteOn)
                            {
                                contextMenuStripForMapMarkers.Items["toolStripMenuItemToggleOffProjectLines"].Visible = true;
                            }
                            else
                            {
                                contextMenuStripForMapMarkers.Items["toolStripMenuItemToggleOnProjectLines"].Visible = true;
                            }

                            contextMenuStripForMapMarkers.Items["toolStripMenuItemAssignedPrecertificationLiaison"].Visible = true;
                            string precertificationLiaisonText = String.Format("Precertification Liaison: {0}", !String.IsNullOrEmpty(project.PrecertificationLiaisonUserFullName) ? project.PrecertificationLiaisonUserFullName : "Unassigned");
                            contextMenuStripForMapMarkers.Items["toolStripMenuItemAssignedPrecertificationLiaison"].Text = precertificationLiaisonText;
                            contextMenuStripForMapMarkers.Items["toolStripMenuItemAssignedCertificationLiaison"].Visible = true;
                            string certificationLiaisonText = String.Format("Certification Liaison: {0}", !String.IsNullOrEmpty(project.CertificationLiaisonUserFullName) ? project.CertificationLiaisonUserFullName : "Unassigned");
                            contextMenuStripForMapMarkers.Items["toolStripMenuItemAssignedCertificationLiaison"].Text = certificationLiaisonText;
                            //contextMenuStripForMapMarkers.Items["toolStripMenuItemAssignCertificationLiaison"].Visible = true;
                        }
                        else if (marker.Tag is WorkConcept)
                        {
                            var workConcept = (WorkConcept)marker.Tag;

                            if ((userAccount.IsRegionalProgrammer || userAccount.IsSuperUser || userAccount.IsAdministrator))
                            {
                                if ((workConcept.ProjectDbId == 0 && workConcept.WorkConceptCode.Equals("PR")))
                                {
                                    contextMenuStripForMapMarkers.Items["toolStripMenuItemProposeWorkConcept"].Visible = true;
                                }
                                else if (workConcept.FromFiips)
                                {
                                    var wcs = eligibleWorkConcepts.Where(wc => wc.StructureId.Equals(workConcept.StructureId));

                                    if (wcs.Count() == 0)
                                    {
                                        contextMenuStripForMapMarkers.Items["toolStripMenuItemProposeWorkConcept"].Visible = true;
                                    }
                                }
                            }
                        }
                        else
                        { }

                        break;
                    }
                }
            }
            else
            {
                //new Bitmap(WiSam.StructuresProgram.Properties.Resources.Map_Marker_Ball_Right_Chartreuse_64)
                var pointLatLong = GetLocation(e.Location);
                mousedOverMarker = new GMarkerGoogle(pointLatLong, new Bitmap(WiSam.StructuresProgram.Properties.Resources.LocationIcon));
                mousedOverMarker.Tag = "SCT Marker";
                contextMenuStripForMapMarkers.Items["toolStripMenuItemPlaceMarker"].Visible = true;
            }
        }

        private PointLatLng GetLocation(Point pnt)
        {
            return controller.GetLocation(pnt, gMapControlStructuresMap);
        }

        private void contextMenuStripForMapMarkers_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "Place SCT Marker":
                    tabControlSearchTypes.SelectedTab = tabControlSearchTypes.TabPages["tabPageSearchByDistance"];
                    userMarkerCount++;
                    mousedOverMarker.Tag = "SCT Marker " + userMarkerCount;
                    mousedOverMarker.ToolTipText = String.Format("SCT Marker {0}", userMarkerCount);
                    mousedOverMarker.ToolTip.TextPadding = new Size(10, 10);
                    mousedOverMarker.ToolTip.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                    mousedOverMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    textBoxSearchByDistanceId.Text = mousedOverMarker.Tag.ToString();
                    radioButtonSearchByUserMarker.Checked = true;
                    textBoxSearchByDistanceLatitude.Text = mousedOverMarker.Position.Lat.ToString();
                    textBoxSearchByDistanceLongitude.Text = mousedOverMarker.Position.Lng.ToString();
                    nearMeLayer.Markers.Add(mousedOverMarker);
                    break;
                case "Propose Work Concept":
                    if (mousedOverMarker != null && mousedOverMarker.Tag is WorkConcept)
                    {
                        OpenProposeWorkConcept((WorkConcept)mousedOverMarker.Tag);
                    }
                    break;
                case "Search By Distance":
                    tabControlSearchTypes.SelectedTab = tabControlSearchTypes.TabPages["tabPageSearchByDistance"];

                    if (mousedOverMarker != null)
                    {
                        if (mousedOverMarker.Tag is WorkConcept)
                        {
                            WorkConcept wc = (WorkConcept)mousedOverMarker.Tag;
                            textBoxSearchByDistanceId.Text = wc.StructureId;
                            radioButtonSearchByStructureId.Checked = true;

                            if (wc.GeoLocation != null && wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0)
                            {
                                textBoxSearchByDistanceLatitude.Text = wc.GeoLocation.LatitudeDecimal.ToString();
                                textBoxSearchByDistanceLongitude.Text = wc.GeoLocation.LongitudeDecimal.ToString();
                            }

                            SearchByDistance(wc.StructureId, StructuresProgramType.ObjectType.WorkConcept, wc.GeoLocation.LatitudeDecimal, wc.GeoLocation.LongitudeDecimal);
                        }
                        else if (mousedOverMarker.Tag is Project)
                        {
                            Project project = (Project)mousedOverMarker.Tag;

                            if (project.GeoLocation != null && project.GeoLocation.LatitudeDecimal != 0 && project.GeoLocation.LongitudeDecimal != 0)
                            {
                                textBoxSearchByDistanceLatitude.Text = project.GeoLocation.LatitudeDecimal.ToString();
                                textBoxSearchByDistanceLongitude.Text = project.GeoLocation.LongitudeDecimal.ToString();
                            }

                            if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                            {
                                textBoxSearchByDistanceId.Text = project.FosProjectId;
                                radioButtonSearchByFiipsId.Checked = true;
                                SearchByDistance(project.FosProjectId, StructuresProgramType.ObjectType.FiipsProject, project.GeoLocation.LatitudeDecimal, project.GeoLocation.LongitudeDecimal);
                            }
                            else
                            {
                                textBoxSearchByDistanceId.Text = project.ProjectDbId.ToString();
                                radioButtonSearchByProjectId.Checked = true;
                                SearchByDistance(project.ProjectDbId.ToString(), StructuresProgramType.ObjectType.StructuresProject, project.GeoLocation.LatitudeDecimal, project.GeoLocation.LongitudeDecimal);
                            }
                        }
                        else if (mousedOverMarker.Tag.ToString().StartsWith("SCT Marker"))
                        {
                            textBoxSearchByDistanceId.Text = mousedOverMarker.Tag.ToString();
                            radioButtonSearchByUserMarker.Checked = true;
                            textBoxSearchByDistanceLatitude.Text = mousedOverMarker.Position.Lat.ToString();
                            textBoxSearchByDistanceLongitude.Text = mousedOverMarker.Position.Lng.ToString();
                            SearchByDistance(mousedOverMarker.Tag.ToString(), StructuresProgramType.ObjectType.UserMarker, Convert.ToSingle(mousedOverMarker.Position.Lat), Convert.ToSingle(mousedOverMarker.Position.Lng));
                        }
                    }
                    
                    break;
                case "Center":
                    gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(mousedOverMarker.Position.Lat, mousedOverMarker.Position.Lng);
                    tabControlSearchTypes.SelectedTab = tabControlSearchTypes.TabPages["tabPageSearchByDistance"];

                    if (mousedOverMarker.Tag is WorkConcept)
                    {
                        WorkConcept wc = (WorkConcept)mousedOverMarker.Tag;
                        textBoxSearchByDistanceId.Text = wc.StructureId;
                        radioButtonSearchByStructureId.Checked = true;

                        if (wc.GeoLocation != null && wc.GeoLocation.LatitudeDecimal != 0 && wc.GeoLocation.LongitudeDecimal != 0)
                        {
                            textBoxSearchByDistanceLatitude.Text = wc.GeoLocation.LatitudeDecimal.ToString();
                            textBoxSearchByDistanceLongitude.Text = wc.GeoLocation.LongitudeDecimal.ToString();
                        }
                    }
                    else if (mousedOverMarker.Tag is Project)
                    {
                        Project project = (Project)mousedOverMarker.Tag;

                        if (project.GeoLocation != null && project.GeoLocation.LatitudeDecimal != 0 && project.GeoLocation.LongitudeDecimal != 0)
                        {
                            textBoxSearchByDistanceLatitude.Text = project.GeoLocation.LatitudeDecimal.ToString();
                            textBoxSearchByDistanceLongitude.Text = project.GeoLocation.LongitudeDecimal.ToString();
                        }

                        if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                        {
                            textBoxSearchByDistanceId.Text = project.FosProjectId;
                            radioButtonSearchByFiipsId.Checked = true;
                            //SearchByDistance(project.FosProjectId, StructuresProgramType.ObjectType.FiipsProject, project.GeoLocation.LatitudeDecimal, project.GeoLocation.LongitudeDecimal);
                        }
                        else
                        {
                            textBoxSearchByDistanceId.Text = project.ProjectDbId.ToString();
                            radioButtonSearchByProjectId.Checked = true;
                            //SearchByDistance(project.ProjectDbId.ToString(), StructuresProgramType.ObjectType.StructuresProject, project.GeoLocation.LatitudeDecimal, project.GeoLocation.LongitudeDecimal);
                        }
                    }
                    else if (mousedOverMarker.Tag.ToString().StartsWith("SCT Marker"))
                    {
                        textBoxSearchByDistanceId.Text = mousedOverMarker.Tag.ToString();
                        radioButtonSearchByUserMarker.Checked = true;
                        textBoxSearchByDistanceLatitude.Text = mousedOverMarker.Position.Lat.ToString();
                        textBoxSearchByDistanceLongitude.Text = mousedOverMarker.Position.Lng.ToString();
                        //SearchByDistance(mousedOverMarker.Tag.ToString(), StructuresProgramType.ObjectType.UserMarker, Convert.ToSingle(mousedOverMarker.Position.Lat), Convert.ToSingle(mousedOverMarker.Position.Lng));
                    }

                    break;
                case "Toggle On Project Lines":
                case "Toggle Off Project Lines":
                    if (mousedOverMarker.Tag is Project)
                    {
                        Project mousedOverProject = (Project)mousedOverMarker.Tag;
                        Project project = null;

                        if (mousedOverProject.Status == StructuresProgramType.ProjectStatus.Fiips)
                        {
                            try
                            {
                                project = fiipsProjects.Where(p => p.FosProjectId.Equals(mousedOverProject.FosProjectId)).First();
                            }
                            catch { }
                        }
                        else
                        {
                            try
                            {
                                project = structureProjects.Where(p => p.ProjectDbId == mousedOverProject.ProjectDbId).First();
                            }
                            catch { }
                        }

                        if (project != null)
                        {
                            if (e.ClickedItem.Text.Equals("Toggle On Project Lines"))
                            {
                                project.IsProjectRouteOn = true;
                                ToggleProjectRoute(mousedOverMarker, true);
                            }
                            else
                            {
                                project.IsProjectRouteOn = false;
                                ToggleProjectRoute(mousedOverMarker, false);
                            }
                        }
                    }
                    break;
            }
        }

        private void buttonFindProjectsPendingSubmittal_Click(object sender, EventArgs e)
        {
            //textBoxHitCount.Text = "0";
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.PendingSubmittal);
            //textBoxHitCount.Text = hits.ToString();
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private List<int> FindProjectsWithGivenUserAction(StructuresProgramType.ProjectUserAction userAction)
        {
            if (!checkBoxRetainHits.Checked)
            {
                checkBoxClearMapStatus.Checked = true;
                ClearAllMarkers();
                checkBoxClearMapStatus.Checked = false;
            }

            projectIdHits = new List<int>();

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                startingNode = tn;
                FindProjectWithGivenUserActionRecursive(tn, userAction);
                startingNode = null;
            }

            gMapControlStructuresMap.ZoomAndCenterMarkers("Structure");
            return projectIdHits;
        }

        private List<int> FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus projectStatus, bool fromExcel = false)
        {
            if (!checkBoxRetainHits.Checked)
            {
                checkBoxClearMapStatus.Checked = true;
                ClearAllMarkers();
                checkBoxClearMapStatus.Checked = false;
            }

            projectIdHits = new List<int>();

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                startingNode = tn;
                FindProjectWithGivenStatusRecursive(tn, projectStatus, fromExcel);
                startingNode = null;
            }

            gMapControlStructuresMap.ZoomAndCenterMarkers("Structure");
            return projectIdHits;
        }

        private void buttonFindProjectsPendingPrecertification_Click(object sender, EventArgs e)
        {
            //textBoxHitCount.Text = "0";
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.PendingPrecertification);
            //textBoxHitCount.Text = hitCount.ToString();
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void buttonFindProjectsPendingCertification_Click(object sender, EventArgs e)
        {
            //textBoxHitCount.Text = "0";
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.PendingCertification);
            //textBoxHitCount.Text = hitCount.ToString();
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void buttonFindProjectsCertified_Click(object sender, EventArgs e)
        {
            //textBoxHitCount.Text = "0";
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.Certified);
            //await Task.Run(() => FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.Certified));
            //textBoxHitCount.Text = hitCount.ToString();
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void ClearSearchResults()
        {
            textBoxStructuresResultsNumberOfStructures.Text = "";
            textBoxStructuresResultsNumberOfProjects.Text = "";
            textBoxFiipsResultsNumberOfProjects.Text = "";
            textBoxFiipsResultsNumberOfStructures.Text = "";
        }

        private void ToggleMainSearchButtons(bool status)
        {
            buttonFindStructureId.Enabled = status;
            buttonFindFosId.Enabled = status;
            buttonFindProjectId.Enabled = status;
            buttonFindProjectsCertified.Enabled = status;
            buttonFindProjectsPendingSubmittal.Enabled = status;
            buttonFindProjectsPendingPrecertification.Enabled = status;
            buttonFindProjectsPendingCertification.Enabled = status;
            buttonFindProjectsManuallyTransitionallyCertified.Enabled = status;
        }

        private void OpenProposeWorkConcept(WorkConcept workConcept)
        {
            if (formProposeWorkConcept != null)
            {
                formProposeWorkConcept.Close();
            }

            formProposeWorkConcept = new FormProposeWorkConcept(workConcept, primaryWorkConcepts, secondaryWorkConcepts, justifications, formMainController);
            formProposeWorkConcept.StartPosition = FormStartPosition.Manual;
            formProposeWorkConcept.Location = new Point(gMapControlStructuresMap.Width / 2, gMapControlStructuresMap.Height / 2);
            formProposeWorkConcept.Show();
        }

        private void OpenLoading()
        {
            formLoading = new FormLoggingIn();
            formLoading.StartPosition = FormStartPosition.Manual;
            formLoading.Location = new Point(gMapControlStructuresMap.Width / 2, gMapControlStructuresMap.Height / 2);
            formLoading.Show();
        }

        private void CloseLoading()
        {
            if (formLoading != null)
            {
                formLoading.Close();
            }
        }

        private void buttonFindProjectsTransitionallyCertified_Click(object sender, EventArgs e)
        {
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.QuasiCertified, true);
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void DisplaySearchByCertificationStatusResults(List<int> projectIds)
        {
            textBoxStructuresResultsNumberOfProjects.Text = projectIds.Count.ToString();
            List<string> structureIds = new List<string>();

            foreach (Project project in structureProjects.Where(p => projectIds.Contains(p.ProjectDbId)))
            {
                foreach (WorkConcept wc in project.WorkConcepts)
                {
                    if (!structureIds.Contains(wc.StructureId))
                    {
                        structureIds.Add(wc.StructureId);
                    }
                }
            }

            textBoxStructuresResultsNumberOfStructures.Text = structureIds.Count().ToString();
        }

        private void buttonCreateMapReport_Click(object sender, EventArgs e)
        {
            OpenLoading();
            buttonCreateMapReport.Enabled = false;
            string outputFilePath = formMainController.GetRandomExcelFileName(@"c:\temp");
            
            foreach (var olay in gMapControlStructuresMap.Overlays)
            {
                gMapControlStructuresMap.ZoomAndCenterMarkers(olay.Id);
            }

            Bitmap mapImage = new Bitmap(gMapControlStructuresMap.Width, gMapControlStructuresMap.Height);
            gMapControlStructuresMap.DrawToBitmap(mapImage, gMapControlStructuresMap.Bounds);
            string imageFilePath = outputFilePath.Replace(".xlsx", ".jpeg");
            mapImage.Save(imageFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            XLWorkbook wb = new XLWorkbook();
            List<string> sctColumnNames = new List<string>()
            {
                "Structure ID", "Work Concept to be Certified", "Work Concept Certification Status",
                "Work Concept Fy", "Structures Project ID", "Structures Project Certification Status",
                "Structures Project Last Transaction", "Structures Project Fy",
                "Precertification Liaison", "Certification Liaison",
                "Corridors 2030", "Feature On", "Feature Under", "Region", "County"
            };
            List<string> fiipsColumnNames = new List<string>()
            {
                "Construction ID", "Let FY", "Adv PSE", "LC Stage", "Structure ID", "Work Concept",
                "Corridors 2030", "Feature On", "Feature Under", "Region", "County"
            };

            int cc = 1; // Column counter
            int rc = 1; // Row counter
            var ws1 = wb.AddWorksheet("Map");
            var ws2 = wb.AddWorksheet("SCT Work Concepts");
            var ws3 = wb.AddWorksheet("FIIPS Work Concepts");
            ws1.AddPicture(imageFilePath).Placement = ClosedXML.Excel.Drawings.XLPicturePlacement.FreeFloating;

            foreach (var columnName in sctColumnNames)
            {
                ws2.Cell(rc, cc).Value = columnName;
                cc++;
            }

            cc = 1;
            rc = 1;

            foreach (var columnName in fiipsColumnNames)
            {
                ws3.Cell(rc, cc).Value = columnName;
                cc++;
            }

            // Grab the overlay
            //var structuresWorkConceptsOlay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Structure")).First();
            // var structuresProjectsOlay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Projects")).First();
            //var structuresWorkConceptsOlay = structureLayer;// = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Structure")).First();
            //var structuresProjectsOlay = projectsLayer;//  = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Projects")).First();
            //var nearMeOlay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("NearMe")).First();
            var markers = new List<GMapMarker>();
            markers.AddRange(structureLayer.Markers.Where(m => m.IsVisible && m.Tag != null && m.Tag is WorkConcept).ToList());
            markers.AddRange(nearMeLayer.Markers.Where(m => m.IsVisible && m.Tag != null && m.Tag is WorkConcept).ToList());
            rc = 2;

            foreach (var marker in markers.OrderBy(m => ((WorkConcept)m.Tag).StructureId))
            {
                WorkConcept wc = (WorkConcept)marker.Tag;
                Dw.Structure str = dataServ.GetWarehouseDatabase().GetStructure(wc.StructureId);

                if (str != null)
                {
                    ws2.Cell(rc, 11).Value = str.ProgrammingType;
                    ws2.Cell(rc, 12).Value = str.ServiceFeatureOn;
                    ws2.Cell(rc, 13).Value = str.ServiceFeatureUnder;

                    if (str.GeoLocation != null)
                    {
                        ws2.Cell(rc, 14).Value = str.GeoLocation.Region;
                        ws2.Cell(rc, 15).Value = str.GeoLocation.County;
                    }
                }

                ws2.Cell(rc, 1).Value = wc.StructureId;
                
                if (wc.FiscalYear != 0)
                {
                    ws2.Cell(rc, 2).Value = String.Format("({0}) {1}", wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription);
                }
                else
                {
                    ws2.Cell(rc, 2).Value = "";
                }

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Quasicertified)
                {
                    ws2.Cell(rc, 3).Value = "Certified";
                }
                else
                {
                    ws2.Cell(rc, 3).Value = wc.FiscalYear != 0 ? wc.Status.ToString() : "";
                }

                ws2.Cell(rc, 4).Value = wc.FiscalYear != 0 ? wc.FiscalYear.ToString() : "";
                ws2.Cell(rc, 5).Value = wc.ProjectDbId != 0 ? wc.ProjectDbId.ToString() : "";

                try
                {
                    // var projectMarker = structuresProjectsOlay.Markers.Where(m => ((Project)m.Tag).ProjectDbId == wc.ProjectDbId && ((Project)m.Tag).Status != StructuresProgramType.ProjectStatus.Fiips).First();
                    List<Project> projects = structureProjects.Where(sp => sp.WorkConcepts.Any(s => s.StructureId.Equals(wc.StructureId))).ToList();
                    Project project = structureProjects.Where(sp => sp.ProjectDbId == wc.ProjectDbId).First();

                    if (projects.Count > 1)
                    {
                        string projectIds = "";
                        string projectStatuses = "";
                        string projectWorkflows = "";
                      
                        foreach (Project proj in projects)
                        {
                            projectIds += String.Format("{0}; ", proj.ProjectDbId);
                            projectStatuses += String.Format("{0}; ", proj.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : proj.Status.ToString());
                            projectWorkflows += String.Format("{0}; ", dataServ.GetWorkflowStatus(proj));
                        }

                        ws2.Cell(rc, 5).Value = projectIds;
                        ws2.Cell(rc, 6).Value = projectStatuses;
                        ws2.Cell(rc, 7).Value = projectWorkflows;
                    }
                    else
                    {
                        if (project.IsQuasicertified && String.IsNullOrEmpty(project.History))
                        {
                            ws2.Cell(rc, 6).Value = "Transitionally Certified";
                            ws2.Cell(rc, 7).Value = "Transitionally Certified";
                        }
                        else
                        {
                            ws2.Cell(rc, 6).Value = project.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : project.Status.ToString();
                            string projectStatus = dataServ.GetWorkflowStatus(project);
                            ws2.Cell(rc, 7).Value = projectStatus;
                        }
                    }

                    ws2.Cell(rc, 8).Value = project.FiscalYear;
                    //ws2.Cell(rc, 9).Value = project.PrecertificationLiaisonUserFullName;
                    ws2.Cell(rc, 9).Value = dataServ.GetPrecertificationLiaison(project.ProjectDbId);
                    //ws2.Cell(rc, 10).Value = project.CertificationLiaisonUserFullName;
                    ws2.Cell(rc, 10).Value = dataServ.GetCertificationLiaison(project.ProjectDbId);
                }
                catch { }

                rc++;
            }

            rc = 2;
            var fiipsMarkers = fiipsLayer.Markers.Where(m => m.IsVisible && m.Tag != null && m.Tag is WorkConcept).ToList();

            foreach (var marker in fiipsMarkers.OrderBy(m => ((WorkConcept)m.Tag).FosProjectId))
            {
                WorkConcept wc = (WorkConcept)marker.Tag;
                Project proj = fiipsProjects.Where(fp => fp.FosProjectId.Equals(wc.FosProjectId)).First();
                ws3.Cell(rc, 1).Value = wc.FosProjectId;
                ws3.Cell(rc, 2).Value = proj.FiscalYear;
                ws3.Cell(rc, 3).Value = proj.EpseDate.Year != 1 ? proj.EpseDate.ToString("yyyy/MM/dd") : "";
                ws3.Cell(rc, 4).Value = proj.LifecycleStageCode;
                ws3.Cell(rc, 5).Value = wc.StructureId;
                ws3.Cell(rc, 6).Value = String.Format("({0}) {1}", wc.WorkConceptCode, wc.WorkConceptDescription);
                Dw.Structure str = dataServ.GetWarehouseDatabase().GetStructure(wc.StructureId);

                if (str != null)
                {
                    ws3.Cell(rc, 7).Value = str.ProgrammingType;
                    ws3.Cell(rc, 8).Value = str.ServiceFeatureOn;
                    ws3.Cell(rc, 9).Value = str.ServiceFeatureUnder;

                    if (str.GeoLocation != null)
                    {
                        ws3.Cell(rc, 10).Value = str.GeoLocation.Region;
                        ws3.Cell(rc, 11).Value = str.GeoLocation.County;
                    }
                }

                rc++;
            }

            wb.SaveAs(outputFilePath);
            wb.Dispose();

            try
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(outputFilePath);
            }
            catch { }

            buttonCreateMapReport.Enabled = true;
            CloseLoading();
        }

        private void gMapControlStructuresMap_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            if (selecting)
            {
                end = GetLocation(e.Location);
                List<PointLatLng> pts = new List<PointLatLng>()
                {
                    new PointLatLng(start.Lat, start.Lng),
                    new PointLatLng(start.Lat, end.Lng),
                    new PointLatLng(end.Lat, end.Lng),
                    new PointLatLng(end.Lat, start.Lng)
                };
                var select = new GMapPolygon(pts, "select")
                {
                    Fill = Brushes.Transparent,
                    Stroke = Pens.Crimson
                };
                
                nearMeLayer.Polygons.Clear();
                nearMeLayer.Polygons.Add(select);
            }*/
        }

        private void gMapControlStructuresMap_MouseUp(object sender, MouseEventArgs e)
        {
            /*
            if (!selecting)
            {
                return;
            }

            selecting = false;
            end = GetLocation(e.Location);
            */
        }

        private static bool CheckDist(double min, double max,
            GeoCoordinate geo, GeoLocation geoLocation)
        {
            return controller.CheckDist(min, max, geo, geoLocation);
        }

        private void textBoxSearchByIdStructureId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonFindStructureId_Click(sender, e);
            }
        }

        private void textBoxSearchByDistanceId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                FindNearMe();
            }
        }

        private void textBoxSearchByIdFosId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonFindFosId_Click(sender, e);
            }
        }

        private void textBoxSearchByIdProjectId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonFindProjectId_Click(sender, e);
            }
        }

        private void buttonFindRejectedProjects_Click(object sender, EventArgs e)
        {
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.Rejected);
            //textBoxHitCount.Text = hitCount.ToString();
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
           
        }

        private void buttonFindStructuresInMultipleStructuresProjects_Click(object sender, EventArgs e)
        {
            if (!checkBoxRetainHits.Checked)
            {
                checkBoxClearMapStatus.Checked = true;
                ClearAllMarkers();
                checkBoxClearMapStatus.Checked = false;
            }

            OpenLoading();
            ToggleMainSearchButtons(false);
            structureIdHits = new List<string>();
            ClearSearchResults();

            foreach (TreeNode tn in treeViewMapping.Nodes)
            {
                startingNode = tn;
                FindStructuresInMultipleStructuresProjects(tn);
            }

            textBoxStructuresResultsNumberOfStructures.Text = structureIdHits.Count().ToString();

            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void contextMenuStripForMapMarkers_MouseDown(object sender, MouseEventArgs e)
        {
            var here = 0;
        }

        private void contextMenuStripForMapMarkers_Click(object sender, EventArgs e)
        {
            var here = 0;
        }

        private void button1_Click_3(object sender, EventArgs e)
        {
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenStatus(StructuresProgramType.ProjectStatus.QuasiCertified);
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }

        private void buttonFindTopUsers_Click(object sender, EventArgs e)
        {
            List<UserAccount> topUsers = dataServ.GetTopUsers();
            int counter = 0;
            double longitude = -87.46;
            double startingLatitude = 43.54;

            foreach (var user in topUsers)
            {
                if (counter >= 3)
                {
                    break;
                }

                Bitmap bitMap = WiSam.StructuresProgram.Properties.Resources.award_star_gold_1;

                if (counter == 1)
                {
                    bitMap = WiSam.StructuresProgram.Properties.Resources.award_star_silver_1;
                }
                else
                {
                    bitMap = WiSam.StructuresProgram.Properties.Resources.award_star_bronze_1;
                }

                GMapMarker marker = new GMarkerGoogle(new PointLatLng(startingLatitude - (counter * 0.22), longitude), new Bitmap(bitMap));
                //GMapMarker marker = new GMarkerGoogle(new PointLatLng(startingLatitude - (counter * 0.22), longitude), new Bitmap());
                marker.ToolTipText = String.Format("{0}{1}. {2} {3}", Environment.NewLine, counter + 1, user.FirstName, user.LastName);
                marker.ToolTipMode = MarkerTooltipMode.Always;
                marker.ToolTip.TextPadding = new Size(10, 10);
                marker.ToolTip.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                nearMeLayer.Markers.Add(marker);
                counter++;
            }

            gMapControlStructuresMap.Position = new GMap.NET.PointLatLng(startingLatitude, longitude);
        }

        private void buttonFindProjectsRecertificationRequested_Click(object sender, EventArgs e)
        {
            OpenLoading();
            ToggleMainSearchButtons(false);
            ClearSearchResults();
            List<int> hits = FindProjectsWithGivenUserAction(StructuresProgramType.ProjectUserAction.RequestRecertification);
            DisplaySearchByCertificationStatusResults(hits);
            ToggleMainSearchButtons(true);
            CloseLoading();
        }
    }
}
