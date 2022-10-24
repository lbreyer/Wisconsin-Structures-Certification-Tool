using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class StructureProjectService : IStructureProjectService
    {
        private static IStructureProjectRepository repo = new StructureProjectRepository();

        public string FormatConstructionId(string constructionId)
        {
            return repo.FormatConstructionId(constructionId);
        }

        public int GetFiscalYear()
        {
            return repo.GetFiscalYear();
        }

        public int GetLiaisonUserDbId(string liaison)
        {
            return repo.GetLiaisonUserDbId(liaison);
        }

        public List<WorkConcept> GetUnapprovedWorkConceptsWithoutJustification(List<WorkConcept> workConceptsAdded)
        {
            return repo.GetUnapprovedWorkConceptsWithoutJustification(workConceptsAdded);
        }

        public bool isDateValid(string date)
        {
            return repo.isDateValid(date);
        }

        public bool IsWorkConceptPrecertified(WorkConcept wc, int projectFiscalYear)
        {
            return repo.IsWorkConceptPrecertified(wc, projectFiscalYear);
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            return repo.ParseWorkConceptFullDescription(workConcept);
        }
    }
}
