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
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class MappingRepository : IMappingRepository
    {
        private static IDatabaseService dataServ = new DatabaseService();

        public void AddMarkersToLayer(GMapOverlay layer, List<WorkConcept> workConcepts, GMarkerGoogleType markerType, bool isVisible)
        {
            int counter = 0;

            foreach (WorkConcept workConcept in workConcepts)
            {
                counter++;

                if (workConcept.GeoLocation == null || workConcept.GeoLocation.LatitudeDecimal == 0 || workConcept.GeoLocation.LongitudeDecimal == 0)
                {
                    var gl = dataServ.GetStructureLatLong(workConcept.StructureId);
                    workConcept.GeoLocation = gl;
                }

                switch (workConcept.Status)
                {
                    case StructuresProgramType.WorkConceptStatus.Eligible:
                        markerType = GMarkerGoogleType.white_small;
                        break;
                    case StructuresProgramType.WorkConceptStatus.Unapproved:
                        markerType = GMarkerGoogleType.red_small;
                        break;
                    case StructuresProgramType.WorkConceptStatus.Precertified:
                        markerType = GMarkerGoogleType.yellow_small;
                        break;
                    case StructuresProgramType.WorkConceptStatus.Quasicertified:
                        markerType = GMarkerGoogleType.green_small;
                        break;
                    case StructuresProgramType.WorkConceptStatus.Certified:
                        markerType = GMarkerGoogleType.green_small;
                        break;
                    case StructuresProgramType.WorkConceptStatus.Fiips:
                        markerType = GMarkerGoogleType.blue_dot;
                        break;
                    case StructuresProgramType.WorkConceptStatus.Proposed:
                        if (workConcept.WorkConceptDbId <= 0)
                        {
                            markerType = GMarkerGoogleType.black_small;
                        }
                        else
                        {
                            markerType = GMarkerGoogleType.orange_small;
                        }
                        break;
                    case StructuresProgramType.WorkConceptStatus.Evaluate:
                        //markerType = (GMarkerGoogleType)28;
                        markerType = GMarkerGoogleType.yellow_small;
                        /*
                        if (workConcept.StructureId.StartsWith("B") || workConcept.StructureId.StartsWith("P"))
                        {
                            markerType = (GMarkerGoogleType)28;
                        }
                        else if (workConcept.StructureId.StartsWith("C"))
                        {
                            markerType = (GMarkerGoogleType)6;
                        }
                        else
                        {
                            markerType = (GMarkerGoogleType)22;
                        }*/
                        break;
                    default:
                        markerType = GMarkerGoogleType.black_small;
                        break;
                }

                string currentStructureId = FormatStructureId(workConcept.StructureId);
                GMapMarker marker = new GMarkerGoogle(new PointLatLng(workConcept.GeoLocation.LatitudeDecimal, workConcept.GeoLocation.LongitudeDecimal),
                                                        markerType); ;
                marker.Tag = workConcept;

                /*
                if (workConcept.Evaluate || workConcept.Status == StructuresProgramType.WorkConceptStatus.Proposed)
                {
                    marker.ToolTipText = String.Format("\n{0}\n({1}) {2}", workConcept.StructureId, workConcept.WorkConceptCode, workConcept.CertifiedWorkConceptDescription);
                }*/
                if (workConcept.FromFiips)
                {
                    marker.ToolTipText = String.Format("\n{0}\n({1}) {2}\nCONSTR ID: {3}\nFY: {4}", currentStructureId, workConcept.WorkConceptCode, workConcept.WorkConceptDescription,
                                                        FormatConstructionId(workConcept.FosProjectId), workConcept.FiscalYear);
                }
                else if (workConcept.FromProposedList)
                {
                    if (workConcept.WorkConceptDbId > 0)
                    {
                        marker.ToolTipText = String.Format("\n{0}\n({1}) {2}\nSTR PROJ ID: {3}\nFY: {4}", currentStructureId, "PR", workConcept.CertifiedWorkConceptDescription,
                                                            workConcept.ProjectDbId != 0 ? workConcept.ProjectDbId.ToString() : "", workConcept.FiscalYear);
                    }
                    else
                    {
                        marker.ToolTipText = String.Format("\n{0}", currentStructureId);
                    }
                }
                else
                {
                    marker.ToolTipText = String.Format("\n{0}\n({1}) {2}\nSTR PROJ ID: {3}\nFY: {4}", currentStructureId, workConcept.WorkConceptCode, workConcept.WorkConceptDescription,
                                                        workConcept.ProjectDbId != 0 ? workConcept.ProjectDbId.ToString() : "",
                                                        workConcept.Status == StructuresProgramType.WorkConceptStatus.Eligible || workConcept.Status == StructuresProgramType.WorkConceptStatus.Quasicertified || workConcept.Status == StructuresProgramType.WorkConceptStatus.Fiips ? workConcept.FiscalYear : workConcept.StructureProjectFiscalYear);
                }

                marker.ToolTip.TextPadding = new Size(10, 10);
                marker.ToolTip.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                marker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                marker.IsVisible = isVisible;
                layer.Markers.Add(marker);
            }
        }

        public bool CheckDist(double min, double max, GeoCoordinate geo, GeoLocation geoLocation)
        {
            min *= 1609.344;
            max *= 1609.344;

            double dist = geo.GetDistanceTo(new GeoCoordinate(geoLocation.LatitudeDecimal, geoLocation.LongitudeDecimal));
            if (dist >= min && dist <= max) return true;
            return false;
        }

        public void ColorProjectNode(TreeNode node, StructuresProgramType.ProjectStatus status)
        {
            switch (status)
            {
                case StructuresProgramType.ProjectStatus.Unapproved:
                    node.ForeColor = Color.White;
                    node.BackColor = Color.Red;
                    break;
                case StructuresProgramType.ProjectStatus.Precertified:
                    node.BackColor = Color.Yellow;
                    break;
                case StructuresProgramType.ProjectStatus.InPrecertification:
                    node.BackColor = Color.Orange;
                    node.ForeColor = Color.White;
                    break;
                case StructuresProgramType.ProjectStatus.Certified:
                case StructuresProgramType.ProjectStatus.QuasiCertified:
                    node.ForeColor = Color.White;
                    node.BackColor = Color.Green;
                    break;
                case StructuresProgramType.ProjectStatus.Fiips:
                    node.ForeColor = Color.White;
                    node.BackColor = Color.Blue;
                    break;
            }
        }

        public void ColorWorkConceptNode(TreeNode node, StructuresProgramType.WorkConceptStatus status)
        {
            switch (status)
            {
                case StructuresProgramType.WorkConceptStatus.Unapproved:
                    node.ForeColor = Color.White;
                    node.BackColor = Color.Red;
                    break;
                case StructuresProgramType.WorkConceptStatus.Precertified:
                    node.BackColor = Color.Yellow;
                    break;
                case StructuresProgramType.WorkConceptStatus.Certified:
                case StructuresProgramType.WorkConceptStatus.Quasicertified:
                    node.ForeColor = Color.White;
                    node.BackColor = Color.Green;
                    break;
                /*
            case StructuresProgramType.WorkConceptStatus.Proposed:
                node.ForeColor = Color.White;
                node.BackColor = Color.Black;
                break;*/
                case StructuresProgramType.WorkConceptStatus.Evaluate:
                    //node.ForeColor = Color.White;
                    node.BackColor = Color.Yellow;
                    break;
                case StructuresProgramType.WorkConceptStatus.Fiips:
                    node.ForeColor = Color.White;
                    node.BackColor = Color.Blue;
                    break;
            }
        }

        public float ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return Convert.ToSingle(degrees);
        }

        public void DeleteProjectMarkerAndRoutes(Project project, GMapControl gMapControlStructuresMap)
        {
            GMapOverlay olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Projects")).First();
            GMapMarker projectMarker = null;

            try
            {
                projectMarker = olay.Markers.Where(x => x.Tag != null && ((Project)x.Tag).ProjectDbId == project.ProjectDbId).First();
                olay.Markers.Remove(projectMarker);
            }
            catch { }

            List<GMapRoute> routesToRemove = new List<GMapRoute>();

            foreach (var route in olay.Routes.Where(r => ((WorkConcept)r.Tag).ProjectDbId == project.ProjectDbId))
            {
                routesToRemove.Add(route);
            }

            foreach (var route in routesToRemove)
            {
                olay.Routes.Remove(route);
            }
        }

        public void EditWorkConcept(WorkConcept wc, string action, TreeView treeViewMapping, GMapControl gMapControlStructuresMap)
        {
            if (action.Equals("Add"))
            {
                try
                {
                    var startingBNode = treeViewMapping.Nodes["WorkConcepts"]
                        .Nodes.Cast<TreeNode>().Where(tn => tn.Text.Equals(wc.FiscalYear.ToString()));

                    if (startingBNode.Count() > 0)
                    {
                        var cNode = startingBNode.First().Nodes["WorkConceptsEligible"];
                        TreeNode dNode = new TreeNode(String.Format("{0} : (PR) {1}", wc.StructureId, wc.CertifiedWorkConceptDescription));
                        dNode.Tag = wc as WorkConcept;
                        dNode.Checked = true;
                        cNode.Nodes.Add(dNode);

                        if (wc.WorkConceptCode.Equals("PR"))
                        {
                            if (wc.ProjectDbId == 0)
                            {
                                dNode.BackColor = Color.DarkOrange;
                            }
                            else
                            {
                                ColorWorkConceptNode(dNode, wc.Status);
                            }
                        }
                        else
                        {
                            ColorWorkConceptNode(dNode, wc.Status);
                        }
                    }
                }
                catch { }
            }
            else if (action.Equals("Delete"))
            {
                foreach (TreeNode tn in treeViewMapping.Nodes)
                {
                    if (tn.Nodes != null)
                    {
                        RemoveWorkConceptRecursive(tn, wc, gMapControlStructuresMap);
                    }
                }
            }
        }

        public void ExpandNodeRecursive(TreeNode treeNode)
        {
            treeNode.Expand();

            if (treeNode.Parent != null)
            {
                ExpandNodeRecursive(treeNode.Parent);
            }
        }

        public void FindFosRecursive(TreeNode treeNode, string fosId, List<Project> projectHits, List<string> fiipsIdHits, List<int> projectIdHits)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Tag is Project)
                {
                    Project project = (Project)tn.Tag;

                    if (project.FosProjectId.Equals(fosId))
                    {
                        tn.Checked = true;
                        tn.Expand();
                        ExpandNodeRecursive(tn);
                        projectHits.Add(project);

                        if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                        {
                            if (!fiipsIdHits.Contains(project.FosProjectId))
                            {
                                fiipsIdHits.Add(project.FosProjectId);
                            }
                        }
                        else
                        {
                            if (!projectIdHits.Contains(project.ProjectDbId))
                            {
                                projectIdHits.Add(project.ProjectDbId);
                            }
                        }
                    }
                }
                else
                {
                    FindFosRecursive(tn, fosId, projectHits, fiipsIdHits, projectIdHits);
                }
            }
        }

        public void FindProjectRecursive(TreeNode treeNode, int structuresProjectId, List<Project> projectHits, List<int> projectIdHits, List<Project> fiipsProjects, List<string> fiipsIdHits)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Tag is Project)
                {
                    Project project = (Project)tn.Tag;

                    if (project.Status != StructuresProgramType.ProjectStatus.Fiips && project.ProjectDbId.Equals(structuresProjectId))
                    {
                        tn.Checked = true;
                        projectHits.Add(project);

                        if (!projectIdHits.Contains(structuresProjectId))
                        {
                            projectIdHits.Add(structuresProjectId);
                        }

                        if (!String.IsNullOrEmpty(project.FosProjectId))
                        {
                            Project associatedFiipsProject = null;

                            try
                            {
                                associatedFiipsProject = fiipsProjects.Where(f => f.FosProjectId.Equals(project.FosProjectId)).First();
                                projectHits.Add(associatedFiipsProject);
                                fiipsIdHits.Add(associatedFiipsProject.FosProjectId);
                            }
                            catch { }
                        }

                        tn.Expand();
                        ExpandNodeRecursive(tn);
                    }
                }
                else
                {
                    FindProjectRecursive(tn, structuresProjectId, projectHits, projectIdHits, fiipsProjects, fiipsIdHits);
                }
            }
        }

        public void FindProjectWithGivenStatusRecursive(TreeNode treeNode, StructuresProgramType.ProjectStatus projectStatus, List<int> projectIdHits, bool fromExcel = false)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Tag is Project)
                {
                    Project project = (Project)tn.Tag;
                    bool isAHit = false;

                    switch (projectStatus)
                    {
                        case StructuresProgramType.ProjectStatus.PendingSubmittal:
                            if (project.UserAction == StructuresProgramType.ProjectUserAction.SavedProject)
                            {
                                isAHit = true;
                            }
                            break;
                        case StructuresProgramType.ProjectStatus.PendingPrecertification:
                            if ((project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification &&
                                    project.Status == StructuresProgramType.ProjectStatus.Unapproved)
                                    || project.InPrecertification
                                    || (project.UserAction == StructuresProgramType.ProjectUserAction.UndoPrecertificationLiaisonAssignment && project.Status == StructuresProgramType.ProjectStatus.Unapproved))
                            {
                                isAHit = true;
                            }
                            break;
                        case StructuresProgramType.ProjectStatus.PendingCertification:
                            if ((project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification
                                    && project.Status == StructuresProgramType.ProjectStatus.Precertified)
                                    || project.InCertification
                                    || (project.Status == StructuresProgramType.ProjectStatus.Precertified && project.PrecertifyDate.Year != 1)
                                    || (project.UserAction == StructuresProgramType.ProjectUserAction.UndoCertificationLiaisonAssignment && project.Status == StructuresProgramType.ProjectStatus.Precertified))
                            {
                                isAHit = true;
                            }
                            break;
                        case StructuresProgramType.ProjectStatus.QuasiCertified:
                            if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified && project.IsQuasicertified)
                            {


                                if (fromExcel && project.FromExcel)
                                {
                                    isAHit = true;
                                }
                                else if (!fromExcel && !project.FromExcel)
                                {
                                    isAHit = true;
                                }
                            }
                            break;
                        case StructuresProgramType.ProjectStatus.Certified:
                            if (project.Status == StructuresProgramType.ProjectStatus.Certified && !project.IsQuasicertified)
                            {
                                isAHit = true;
                            }
                            break;
                        case StructuresProgramType.ProjectStatus.Rejected:
                            if (project.UserAction == StructuresProgramType.ProjectUserAction.BosRejectedPrecertification || project.UserAction == StructuresProgramType.ProjectUserAction.BosRejectedCertification)
                            {
                                isAHit = true;
                            }
                            break;
                    }

                    if (isAHit)
                    {
                        tn.Checked = true;

                        if (!projectIdHits.Contains(project.ProjectDbId))
                        {
                            projectIdHits.Add(project.ProjectDbId);
                            //textBoxHitCount.Text = (Convert.ToInt32(textBoxHitCount.Text) + 1).ToString();
                        }

                        tn.Expand();
                        ExpandNodeRecursive(tn);
                    }
                }
                else
                {
                    FindProjectWithGivenStatusRecursive(tn, projectStatus, projectIdHits, fromExcel);
                }
            }
        }

        public void FindProjectWithGivenUserActionRecursive(TreeNode treeNode, StructuresProgramType.ProjectUserAction userAction, List<int> projectIdHits)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Tag is Project)
                {
                    Project project = (Project)tn.Tag;
                    bool isAHit = false;

                    if (project.UserAction == userAction)
                    {
                        isAHit = true;
                    }

                    if (isAHit)
                    {
                        tn.Checked = true;

                        if (!projectIdHits.Contains(project.ProjectDbId))
                        {
                            projectIdHits.Add(project.ProjectDbId);
                            //textBoxHitCount.Text = (Convert.ToInt32(textBoxHitCount.Text) + 1).ToString();
                        }

                        tn.Expand();
                        ExpandNodeRecursive(tn);
                    }
                }
                else
                {
                    FindProjectWithGivenUserActionRecursive(tn, userAction, projectIdHits);
                }
            }
        }

        public void FindStructureRecursive(TreeNode treeNode, string structureId, List<string> structureIdHits)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Tag is WorkConcept)
                {
                    WorkConcept wc = (WorkConcept)tn.Tag;

                    if (wc.StructureId.Equals(structureId) || (wc.PlannedStructureId != null && wc.PlannedStructureId.Equals(structureId)))
                    {
                        tn.Checked = true;

                        if (!structureIdHits.Contains(wc.StructureId))
                        {
                            structureIdHits.Add(wc.StructureId);
                        }
                        //textBoxHitCount.Text = (Convert.ToInt32(textBoxHitCount.Text) + 1).ToString();
                        ExpandNodeRecursive(tn);
                    }
                }
                else
                {
                    FindStructureRecursive(tn, structureId, structureIdHits);
                }
            }
        }

        public void FindStructuresInMultipleStructuresProjects(TreeNode treeNode, List<Project> structureProjects, List<string> structureIdHits)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Tag is WorkConcept)
                {
                    WorkConcept wc = (WorkConcept)tn.Tag;

                    if (wc.Status != StructuresProgramType.WorkConceptStatus.Fiips && wc.ProjectDbId != 0)
                    {
                        List<Project> projects = structureProjects.Where(sp => sp.WorkConcepts.Any(s => s.StructureId.Equals(wc.StructureId))).ToList();

                        if (projects.Count > 1)
                        {
                            tn.Checked = true;

                            if (!structureIdHits.Contains(wc.StructureId))
                            {
                                structureIdHits.Add(wc.StructureId);
                            }
                        }
                    }
                }
                else
                {
                    FindStructuresInMultipleStructuresProjects(tn, structureProjects, structureIdHits);
                }
            }
        }

        public string FormatConstructionId(string constructionId)
        {
            string formattedId = constructionId;

            try
            {
                formattedId = String.Format("{0}-{1}-{2}", formattedId.Substring(0, 4), formattedId.Substring(4, 2), formattedId.Substring(6, 2));
            }
            catch { }

            return formattedId;
        }

        public string FormatStructureId(string structureId)
        {
            string formattedId = structureId.Trim();

            if (formattedId.Length == 7)
            {
                try
                {
                    formattedId = String.Format("{0}-{1}-{2}", formattedId.Substring(0, 1), formattedId.Substring(1, 2), formattedId.Substring(3, 4));
                }
                catch { }
            }
            else if (formattedId.Length == 11)
            {
                try
                {
                    formattedId = String.Format("{0}-{1}-{2}-{3}", formattedId.Substring(0, 1), formattedId.Substring(1, 2), formattedId.Substring(3, 4), formattedId.Substring(7, 4));
                }
                catch { }
            }

            return formattedId;
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

        public PointLatLng GetLocation(Point pnt, GMapControl gMapControlStructuresMap)
        {
            double lat = gMapControlStructuresMap.ViewArea.Top - (double)pnt.Y / (double)gMapControlStructuresMap.Height * gMapControlStructuresMap.ViewArea.HeightLat;
            double lon = gMapControlStructuresMap.ViewArea.Left + (double)pnt.X / (double)gMapControlStructuresMap.Width * gMapControlStructuresMap.ViewArea.WidthLng;
            return new PointLatLng(lat, lon);
        }

        public WorkConcept GetWorkConceptInstances(string structureId, List<Project> structureProjects)
        {
            WorkConcept workConcept = null;

            foreach (Project project in structureProjects)
            {
                var matches = project.WorkConcepts.Where(wc => wc.StructureId.Equals(structureId));

                if (matches.Count() > 0)
                {
                    workConcept = matches.First();
                    break;
                }
            }

            return workConcept;
        }

        public void RemoveWorkConceptRecursive(TreeNode treeNode, WorkConcept wc, GMapControl gMapControlStructuresMap)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn != null)
                {
                    if (tn.Tag is WorkConcept)
                    {
                        WorkConcept workConcept = (WorkConcept)tn.Tag;

                        if (workConcept.WorkConceptDbId == wc.WorkConceptDbId && workConcept.StructureId.Equals(wc.StructureId))
                        {
                            tn.Remove();

                            // Remove marker
                            GMapOverlay olay = null;

                            switch (wc.Status)
                            {
                                //case StructuresProgramType.WorkConceptStatus.Fiips:
                                //olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Fiips")).First();
                                //break;
                                case StructuresProgramType.WorkConceptStatus.Evaluate:
                                    olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("NearMe")).First();
                                    break;
                                default:
                                    olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Structure")).First();
                                    break;
                            }

                            var markers = olay.Markers.Where(m => ((WorkConcept)m.Tag).WorkConceptDbId == wc.WorkConceptDbId
                                                                && ((WorkConcept)m.Tag).StructureId.Equals(wc.StructureId)
                                                                && ((WorkConcept)m.Tag).FromProposedList == wc.FromProposedList
                                                            );

                            foreach (var marker in markers)
                            {
                                try
                                {
                                    olay.Markers.Remove(marker);
                                }
                                catch { }
                            }
                        }
                    }
                    else
                    {
                        RemoveWorkConceptRecursive(tn, wc, gMapControlStructuresMap);
                    }
                }
            }
        }

        public void RenderTreeWorkConcepts(TreeNode parentNode, List<WorkConcept> workConcepts, bool isChecked = false)
        {
            foreach (WorkConcept wc in workConcepts)
            {
                if (!wc.WorkConceptDescription.Equals(""))
                {
                    //TreeNode tn = new TreeNode(String.Format("{0}-{1}", wc.StructureId, wc.WorkConceptDescription));
                    string currentStructureId = FormatStructureId(wc.StructureId);
                    TreeNode tn = new TreeNode(String.Format("{0} : {1}", currentStructureId, wc.CertifiedWorkConceptDescription));

                    if (wc.WorkConceptCode.Equals("01")) // New structure
                    {
                        tn = new TreeNode(String.Format("{0} : {1}", FormatStructureId(wc.PlannedStructureId), wc.WorkConceptDescription));
                    }
                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Fiips)
                    {
                        tn = new TreeNode(String.Format("{0} : {1}", currentStructureId, wc.WorkConceptDescription));
                    }
                    else if (wc.WorkConceptCode.Equals("PR"))
                    {
                        if (wc.WorkConceptDbId > 0)
                        {
                            tn = new TreeNode(String.Format("{0} : ({1}) {2}", currentStructureId, wc.WorkConceptCode, wc.CertifiedWorkConceptDescription));
                        }
                        else
                        {
                            tn = new TreeNode(String.Format("{0}", currentStructureId));
                        }
                    }
                    /*
                    else if (wc.WorkConceptCode.Equals("EV"))
                    {
                        tn = new TreeNode(String.Format("{0}", wc.StructureId));
                    }*/

                    tn.Tag = wc as WorkConcept;
                    tn.Checked = isChecked;
                    parentNode.Nodes.Add(tn);

                    if (wc.WorkConceptCode.StartsWith("PR"))
                    {
                        //tn.ForeColor = Color.White;
                        if (parentNode.Tag is Project)
                        {
                            ColorWorkConceptNode(tn, wc.Status);
                        }
                        else
                        {
                            if (wc.WorkConceptDbId > 0)
                            {
                                tn.BackColor = Color.DarkOrange;
                            }
                            else
                            {
                                tn.BackColor = Color.Black;
                                tn.ForeColor = Color.White;
                            }
                        }
                    }
                    else
                    {
                        ColorWorkConceptNode(tn, wc.Status);
                    }
                }
            }
        }

        public void ToggleProjectRoute(GMapMarker item, bool toggleOn, GMapControl gMapControlStructuresMap)
        {
            Project project = (Project)item.Tag;

            if (project.IsProjectRouteOn && !toggleOn)
            {
                return;
            }

            GMapOverlay olay = gMapControlStructuresMap.Overlays.Where(o => o.Id.Equals("Projects")).First();

            if (project.GeoLocation != null
                && project.GeoLocation.LatitudeDecimal != 0
                && project.GeoLocation.LongitudeDecimal != 0)
            {
                foreach (var wc in project.WorkConcepts)
                {
                    if (wc.GeoLocation != null)
                    {
                        List<PointLatLng> points = new List<PointLatLng>();
                        points.Add(new PointLatLng(project.GeoLocation.LatitudeDecimal, project.GeoLocation.LongitudeDecimal));
                        points.Add(new PointLatLng(wc.GeoLocation.LatitudeDecimal, wc.GeoLocation.LongitudeDecimal));
                        GMapRoute route = new GMapRoute(points, wc.WorkConceptDbId.ToString());

                        if (toggleOn)
                        {
                            if (!olay.Routes.Any(r => r.Name.Equals(wc.WorkConceptDbId.ToString())))
                            {
                                route.Tag = wc;

                                if (project.Status == StructuresProgramType.ProjectStatus.Fiips)
                                {
                                    route.Stroke = new Pen(Color.Blue, 3);
                                }
                                else
                                {
                                    Color color = Color.Black;
                                    int width = 3;

                                    if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                                    {
                                        color = Color.Red;
                                    }
                                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified)
                                    {
                                        color = Color.Yellow;
                                        width = 5;
                                    }
                                    else if (wc.Status == StructuresProgramType.WorkConceptStatus.Certified || wc.IsQuasicertified)
                                    {
                                        color = Color.Green;
                                    }
                                    else if (wc.Evaluate)
                                    {
                                        color = Color.Purple;
                                    }

                                    route.Stroke = new Pen(color, width);
                                }

                                olay.Routes.Add(route);
                            }
                        }
                        else
                        {
                            try
                            {
                                if (olay.Routes.Where(r => r.Name.Equals(wc.WorkConceptDbId.ToString())).Count() > 0)
                                {
                                    GMapRoute routeToRemove = olay.Routes.Where(r => r.Name.Equals(wc.WorkConceptDbId.ToString())).First();
                                    olay.Routes.Remove(routeToRemove);
                                }
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
            }
        }
    }
}
