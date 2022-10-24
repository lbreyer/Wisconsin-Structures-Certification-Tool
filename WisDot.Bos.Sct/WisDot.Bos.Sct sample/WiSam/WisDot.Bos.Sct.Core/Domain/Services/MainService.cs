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
using BOS.Box;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Data.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class MainService : IMainService
    {
        private static IMainRepository repo = new MainRepository();
        private static IMainQuery query = new MainQuery();

        public void AddWorkConceptToEligibilityGrid(DataGridView dgv, WorkConcept wc)
        {
            repo.AddWorkConceptToEligibilityGrid(dgv, wc);
        }

        public int DoFindFiipsProject(string fosProjectIdToFind, DataGridView dataGridViewProjectsList)
        {
            return repo.DoFindFiipsProject(fosProjectIdToFind, dataGridViewProjectsList);
        }

        public int DoFindStructure(string structureIdToFind, DataGridView dgvi)
        {
            return repo.DoFindStructure(structureIdToFind, dgvi);
        }

        public int DoFindStructureInProjects(string structureIdToFind, DataGridView dataGridViewProjectsList, List<Project> existingFiipsProjects, List<Project> existingStructureProjects)
        {
            return repo.DoFindStructureInProjects(structureIdToFind, dataGridViewProjectsList, existingFiipsProjects, existingStructureProjects);
        }

        public int DoFindStructuresProject(int structuresProjectIdToFind, DataGridView dataGridViewProjectsList)
        {
            return repo.DoFindStructuresProject(structuresProjectIdToFind, dataGridViewProjectsList);
        }

        public List<int> FilterProjectsListGrid(StructuresProgramType.ProjectStatus status, bool show, DataGridView dataGridViewProjectsList)
        {
            return repo.FilterProjectsListGrid(status, show, dataGridViewProjectsList);
        }

        public string FormatConstructionId(string constructionId)
        {
            return repo.FormatConstructionId(constructionId);
        }

        public int GetCurrentFiscalYear()
        {
            return repo.GetCurrentFiscalYear();
        }

        public string GetRandomExcelFileName(string baseDir)
        {
            return repo.GetRandomExcelFileName(baseDir);
        }

        public bool IsFormOpen(string formName)
        {
            return repo.IsFormOpen(formName);
        }

        public bool IsProposedWorkConceptInAProject(int workConceptDbId, string structureId, List<Project> existingStructureProjects)
        {
            return repo.IsProposedWorkConceptInAProject(workConceptDbId, structureId, existingStructureProjects);
        }

        public bool IsStructureInHsi(string structureId, UserAccount userAccount = null)
        {
            return repo.IsStructureInHsi(structureId, userAccount);
        }

        public void OpenExcelFile(string filePath)
        {
            query.OpenExcelFile(filePath);
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            return repo.ParseWorkConceptFullDescription(workConcept);
        }

        public void RenderAllWorkConceptsGrid(List<WorkConcept> wcs, DataGridView dgv, string color = "")
        {
            repo.RenderAllWorkConceptsGrid(wcs, dgv, color);
        }

        public bool ValidateFiscalYear(int fiscalYear, int workConceptStartFiscalYear, int workConceptEndFiscalYear)
        {
            return repo.ValidateFiscalYear(fiscalYear, workConceptStartFiscalYear, workConceptEndFiscalYear);
        }

        public bool ValidateStructureId(string structureId, UserAccount userAccount)
        {
            return repo.ValidateStructureId(structureId, userAccount);
        }
    }
}
