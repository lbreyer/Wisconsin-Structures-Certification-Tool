using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IDatabaseRepository
    {
        ImpersonationUser GetImpersonationUser();
        void PopulateProject(Project project, DataRow dr);
        List<Project> MigrateExcelProjects(List<Project> projects, List<WorkConcept> eligibles);
        DateTime CalculateAcceptablePseDateStart(int fiscalYear);
        DateTime CalculateAcceptablePseDateEnd(int fiscalYear);
        string GetWorkflowStatus(Project project);
        List<Project> GetFiipsProjects(int startFiscalYear, int endFiscalYear, string region = "any");
        string ConvertDegreesMinutesSecondsToDecimalDegrees(string degreesMinutesSeconds);
        Inspection GetLastInspection(string strId);
        string GetRegionComboCode(string region);
        List<string> GetProposedWorkConceptJustifications();
        string FormatEmailAddresses(string addresses);
        string[] ParseWorkConceptFullDescription(string workConcept);
        int GetFiscalYear();

    }
}
