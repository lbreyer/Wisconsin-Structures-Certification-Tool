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
    class FormMappingController
    {
        private static IMappingService serv = new MappingService();

        public int GetFiscalYear()
        {
            return serv.GetFiscalYear();
        }
        public void ColorWorkConceptNode(TreeNode node, StructuresProgramType.WorkConceptStatus status)
        {
            serv.ColorWorkConceptNode(node, status);
        }
        public void RemoveWorkConceptRecursive(TreeNode treeNode, WorkConcept wc, GMapControl gMapControlStructuresMap)
        {
            serv.RemoveWorkConceptRecursive(treeNode, wc, gMapControlStructuresMap);
        }
        public void EditWorkConcept(WorkConcept wc, string action, TreeView treeViewMapping, GMapControl gMapControlStructuresMap)
        {
            serv.EditWorkConcept(wc, action, treeViewMapping, gMapControlStructuresMap);
        }
        public void DeleteProjectMarkerAndRoutes(Project project, GMapControl gMapControlStructuresMap)
        {
            serv.DeleteProjectMarkerAndRoutes(project, gMapControlStructuresMap);
        }
        public string FormatStructureId(string structureId)
        {
            return serv.FormatStructureId(structureId);
        }
        public string FormatConstructionId(string constructionId)
        {
            return serv.FormatConstructionId(constructionId);
        }
        public void ColorProjectNode(TreeNode node, StructuresProgramType.ProjectStatus status)
        {
            serv.ColorProjectNode(node, status);
        }
        public void RenderTreeWorkConcepts(TreeNode parentNode, List<WorkConcept> workConcepts, bool isChecked = false)
        {
            serv.RenderTreeWorkConcepts(parentNode, workConcepts, isChecked);
        }
        public void AddMarkersToLayer(GMapOverlay layer, List<WorkConcept> workConcepts, GMarkerGoogleType markerType, bool isVisible)
        {
            serv.AddMarkersToLayer(layer, workConcepts, markerType, isVisible);
        }
        public WorkConcept GetWorkConceptInstances(string structureId, List<Project> structureProjects)
        {
            return serv.GetWorkConceptInstances(structureId, structureProjects);
        }
        public void ExpandNodeRecursive(TreeNode treeNode)
        {
            serv.ExpandNodeRecursive(treeNode);
        }
        public void FindProjectRecursive(TreeNode treeNode, int structuresProjectId, List<Project> projectHits, List<int> projectIdHits, List<Project> fiipsProjects, List<string> fiipsIdHits)
        {
            serv.FindProjectRecursive(treeNode, structuresProjectId, projectHits, projectIdHits, fiipsProjects, fiipsIdHits);
        }
        public void FindProjectWithGivenUserActionRecursive(TreeNode treeNode, StructuresProgramType.ProjectUserAction userAction, List<int> projectIdHits)
        {
            serv.FindProjectWithGivenUserActionRecursive(treeNode, userAction, projectIdHits);
        }
        public void FindProjectWithGivenStatusRecursive(TreeNode treeNode, StructuresProgramType.ProjectStatus projectStatus, List<int> projectIdHits, bool fromExcel = false)
        {
            serv.FindProjectWithGivenStatusRecursive(treeNode, projectStatus, projectIdHits, fromExcel);
        }
        public void FindFosRecursive(TreeNode treeNode, string fosId, List<Project> projectHits, List<string> fiipsIdHits, List<int> projectIdHits)
        {
            serv.FindFosRecursive(treeNode, fosId, projectHits, fiipsIdHits, projectIdHits);
        }
        public void FindStructuresInMultipleStructuresProjects(TreeNode treeNode, List<Project> structureProjects, List<string> structureIdHits)
        {
            serv.FindStructuresInMultipleStructuresProjects(treeNode, structureProjects, structureIdHits);
        }
        public void FindStructureRecursive(TreeNode treeNode, string structureId, List<string> structureIdHits)
        {
            serv.FindStructureRecursive(treeNode, structureId, structureIdHits);
        }
        public float ConvertRadiansToDegrees(double radians)
        {
            return serv.ConvertRadiansToDegrees(radians);
        }
        public void ToggleProjectRoute(GMapMarker item, bool toggleOn, GMapControl gMapControlStructuresMap)
        {
            serv.ToggleProjectRoute(item, toggleOn, gMapControlStructuresMap);
        }
        public PointLatLng GetLocation(Point pnt, GMapControl gMapControlStructuresMap)
        {
            return serv.GetLocation(pnt, gMapControlStructuresMap);
        }
        public bool CheckDist(double min, double max, GeoCoordinate geo, GeoLocation geoLocation)
        {
            return serv.CheckDist(min, max, geo, geoLocation);
        }

    }
}
