using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class MappingService : IMappingService
    {
        private static IMappingRepository repo = new MappingRepository();

        public void AddMarkersToLayer(GMapOverlay layer, List<WorkConcept> workConcepts, GMarkerGoogleType markerType, bool isVisible)
        {
            repo.AddMarkersToLayer(layer, workConcepts, markerType, isVisible);
        }

        public bool CheckDist(double min, double max, GeoCoordinate geo, GeoLocation geoLocation)
        {
            return repo.CheckDist(min, max, geo, geoLocation);
        }

        public void ColorProjectNode(TreeNode node, StructuresProgramType.ProjectStatus status)
        {
            repo.ColorProjectNode(node, status);
        }

        public void ColorWorkConceptNode(TreeNode node, StructuresProgramType.WorkConceptStatus status)
        {
            repo.ColorWorkConceptNode(node, status);
        }

        public float ConvertRadiansToDegrees(double radians)
        {
            return repo.ConvertRadiansToDegrees(radians);
        }

        public void DeleteProjectMarkerAndRoutes(Project project, GMapControl gMapControlStructuresMap)
        {
            repo.DeleteProjectMarkerAndRoutes(project, gMapControlStructuresMap);
        }

        public void EditWorkConcept(WorkConcept wc, string action, TreeView treeViewMapping, GMapControl gMapControlStructuresMap)
        {
            repo.EditWorkConcept(wc, action, treeViewMapping, gMapControlStructuresMap);
        }

        public void ExpandNodeRecursive(TreeNode treeNode)
        {
            repo.ExpandNodeRecursive(treeNode);
        }

        public void FindFosRecursive(TreeNode treeNode, string fosId, List<Project> projectHits, List<string> fiipsIdHits, List<int> projectIdHits)
        {
            repo.FindFosRecursive(treeNode, fosId, projectHits, fiipsIdHits, projectIdHits);
        }

        public void FindProjectRecursive(TreeNode treeNode, int structuresProjectId, List<Project> projectHits, List<int> projectIdHits, List<Project> fiipsProjects, List<string> fiipsIdHits)
        {
            repo.FindProjectRecursive(treeNode, structuresProjectId, projectHits, projectIdHits, fiipsProjects, fiipsIdHits);
        }

        public void FindProjectWithGivenStatusRecursive(TreeNode treeNode, StructuresProgramType.ProjectStatus projectStatus, List<int> projectIdHits, bool fromExcel = false)
        {
            repo.FindProjectWithGivenStatusRecursive(treeNode, projectStatus, projectIdHits, fromExcel);
        }

        public void FindProjectWithGivenUserActionRecursive(TreeNode treeNode, StructuresProgramType.ProjectUserAction userAction, List<int> projectIdHits)
        {
            repo.FindProjectWithGivenUserActionRecursive(treeNode, userAction, projectIdHits);
        }

        public void FindStructureRecursive(TreeNode treeNode, string structureId, List<string> structureIdHits)
        {
            repo.FindStructureRecursive(treeNode, structureId, structureIdHits);
        }

        public void FindStructuresInMultipleStructuresProjects(TreeNode treeNode, List<Project> structureProjects, List<string> structureIdHits)
        {
            repo.FindStructuresInMultipleStructuresProjects(treeNode, structureProjects, structureIdHits);
        }

        public string FormatConstructionId(string constructionId)
        {
            return repo.FormatConstructionId(constructionId);
        }

        public string FormatStructureId(string structureId)
        {
            return repo.FormatStructureId(structureId);
        }

        public int GetFiscalYear()
        {
            return repo.GetFiscalYear();
        }

        public PointLatLng GetLocation(Point pnt, GMapControl gMapControlStructuresMap)
        {
            return repo.GetLocation(pnt, gMapControlStructuresMap);
        }

        public WorkConcept GetWorkConceptInstances(string structureId, List<Project> structureProjects)
        {
            return repo.GetWorkConceptInstances(structureId, structureProjects);
        }

        public void RemoveWorkConceptRecursive(TreeNode treeNode, WorkConcept wc, GMapControl gMapControlStructuresMap)
        {
            repo.RemoveWorkConceptRecursive(treeNode, wc, gMapControlStructuresMap);
        }

        public void RenderTreeWorkConcepts(TreeNode parentNode, List<WorkConcept> workConcepts, bool isChecked = false)
        {
            repo.RenderTreeWorkConcepts(parentNode, workConcepts, isChecked);
        }

        public void ToggleProjectRoute(GMapMarker item, bool toggleOn, GMapControl gMapControlStructuresMap)
        {
            repo.ToggleProjectRoute(item, toggleOn, gMapControlStructuresMap);
        }
    }
}
