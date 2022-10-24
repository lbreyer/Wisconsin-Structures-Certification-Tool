using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IMainRepository
    {
        bool ValidateStructureId(string structureId, UserAccount userAccount);
        bool IsStructureInHsi(string structureId, UserAccount userAccount = null);
        bool IsFormOpen(string formName);
        List<int> FilterProjectsListGrid(StructuresProgramType.ProjectStatus status, bool show, DataGridView dataGridViewProjectsList);
        bool ValidateFiscalYear(int fiscalYear, int workConceptStartFiscalYear, int workConceptEndFiscalYear);
        int DoFindStructuresProject(int structuresProjectIdToFind, DataGridView dataGridViewProjectsList);
        int DoFindFiipsProject(string fosProjectIdToFind, DataGridView dataGridViewProjectsList);
        int DoFindStructureInProjects(string structureIdToFind, DataGridView dataGridViewProjectsList, List<Project> existingFiipsProjects, List<Project> existingStructureProjects);
        int DoFindStructure(string structureIdToFind, DataGridView dgvi);
        string FormatConstructionId(string constructionId);
        bool IsProposedWorkConceptInAProject(int workConceptDbId, string structureId, List<Project> existingStructureProjects);
        string[] ParseWorkConceptFullDescription(string workConcept);
        int GetCurrentFiscalYear();
        void RenderAllWorkConceptsGrid(List<WorkConcept> wcs, DataGridView dgv, string color = "");
        void AddWorkConceptToEligibilityGrid(DataGridView dgv, WorkConcept wc);
        string GetRandomExcelFileName(string baseDir);

    }
}
