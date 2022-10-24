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

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IMappingRepository
    {
        int GetFiscalYear();
        void ColorWorkConceptNode(TreeNode node, StructuresProgramType.WorkConceptStatus status);
        void RemoveWorkConceptRecursive(TreeNode treeNode, WorkConcept wc, GMapControl gMapControlStructuresMap);
        void EditWorkConcept(WorkConcept wc, string action, TreeView treeViewMapping, GMapControl gMapControlStructuresMap);
        void DeleteProjectMarkerAndRoutes(Project project, GMapControl gMapControlStructuresMap);
        string FormatStructureId(string structureId);
        string FormatConstructionId(string constructionId);
        void ColorProjectNode(TreeNode node, StructuresProgramType.ProjectStatus status);
        void RenderTreeWorkConcepts(TreeNode parentNode, List<WorkConcept> workConcepts, bool isChecked = false);
        void AddMarkersToLayer(GMapOverlay layer, List<WorkConcept> workConcepts, GMarkerGoogleType markerType, bool isVisible);
        WorkConcept GetWorkConceptInstances(string structureId, List<Project> structureProjects);
        void ExpandNodeRecursive(TreeNode treeNode);
        void FindProjectRecursive(TreeNode treeNode, int structuresProjectId, List<Project> projectHits, List<int> projectIdHits, List<Project> fiipsProjects, List<string> fiipsIdHits);
        void FindProjectWithGivenUserActionRecursive(TreeNode treeNode, StructuresProgramType.ProjectUserAction userAction, List<int> projectIdHits);
        void FindProjectWithGivenStatusRecursive(TreeNode treeNode, StructuresProgramType.ProjectStatus projectStatus, List<int> projectIdHits, bool fromExcel = false);
        void FindFosRecursive(TreeNode treeNode, string fosId, List<Project> projectHits, List<string> fiipsIdHits, List<int> projectIdHits);
        void FindStructuresInMultipleStructuresProjects(TreeNode treeNode, List<Project> structureProjects, List<string> structureIdHits);
        void FindStructureRecursive(TreeNode treeNode, string structureId, List<string> structureIdHits);
        float ConvertRadiansToDegrees(double radians);
        void ToggleProjectRoute(GMapMarker item, bool toggleOn, GMapControl gMapControlStructuresMap);
        PointLatLng GetLocation(Point pnt, GMapControl gMapControlStructuresMap);
        bool CheckDist(double min, double max,
            GeoCoordinate geo, GeoLocation geoLocation);

    }
}
