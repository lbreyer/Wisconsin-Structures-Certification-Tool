using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;

namespace WiSam.StructuresProgram
{
    public class FormStructureProjectController
    {
        private static IStructureProjectService serv = new StructureProjectService();
        public int GetFiscalYear()
        {
            return serv.GetFiscalYear();
        }

        public bool IsWorkConceptPrecertified(WorkConcept wc, int projectFiscalYear)
        {
            return serv.IsWorkConceptPrecertified(wc, projectFiscalYear);
        }

        public string FormatConstructionId(string constructionId)
        {
            return serv.FormatConstructionId(constructionId);
        }
        public List<WorkConcept> GetUnapprovedWorkConceptsWithoutJustification(List<WorkConcept> workConceptsAdded)
        {
            return serv.GetUnapprovedWorkConceptsWithoutJustification(workConceptsAdded);
        }
        public int GetLiaisonUserDbId(string liaison)
        {
            return serv.GetLiaisonUserDbId(liaison);
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            return serv.ParseWorkConceptFullDescription(workConcept);
        }

        public bool isDateValid(string date)
        {
            return serv.isDateValid(date);
        }

    }
}
