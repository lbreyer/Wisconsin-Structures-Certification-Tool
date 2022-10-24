using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WisDot.Bos.Sct.Core.Domain.Services.Interfaces
{
    public interface IStructureProjectService
    {
        int GetFiscalYear();
        bool IsWorkConceptPrecertified(WorkConcept wc, int projectFiscalYear);
        string FormatConstructionId(string constructionId);
        List<WorkConcept> GetUnapprovedWorkConceptsWithoutJustification(List<WorkConcept> workConceptsAdded);
        int GetLiaisonUserDbId(string liaison);
        string[] ParseWorkConceptFullDescription(string workConcept);
        bool isDateValid(string date);

    }
}
