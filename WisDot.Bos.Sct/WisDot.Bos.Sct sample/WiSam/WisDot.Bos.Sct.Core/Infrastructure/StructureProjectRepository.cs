using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiSamEntities = Wisdot.Bos.WiSam.Core.Domain.Models;
using System.IO;
using System.Diagnostics;
using BOS.Box;
using System.Threading;
using Dw = Wisdot.Bos.Dw;
using System.Globalization;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class StructureProjectRepository : IStructureProjectRepository
    {
        private static IDatabaseService dataServ = new DatabaseService();
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

        public int GetFiscalYear()
        {
            int currentYear = DateTime.Now.Year;

            if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 7, 1)) >= 0)
            {
                currentYear = currentYear + 1;
            }

            return currentYear;
        }

        public int GetLiaisonUserDbId(string liaison)
        {
            string[] parts = liaison.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            return Convert.ToInt32(parts[1]);
        }

        public List<WorkConcept> GetUnapprovedWorkConceptsWithoutJustification(List<WorkConcept> workConceptsAdded)
        {
            List<WorkConcept> wcs = new List<WorkConcept>();

            foreach (WorkConcept wc in workConceptsAdded)
            {
                if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved && !wc.FromProposedList)
                {
                    if (wc.ChangeJustifications.Equals("") || wc.ChangeJustificationNotes.Trim().Equals(""))
                    {
                        wcs.Add(wc);
                    }
                }
            }

            return wcs;
        }

        public bool isDateValid(string date)
        {
            bool isValid = true;
            CultureInfo enUS = new CultureInfo("en-US");
            DateTime dateValue;

            if (DateTime.TryParseExact(date, "MM/dd/yyyy", enUS, System.Globalization.DateTimeStyles.None, out dateValue))
            {
            }
            else
            {
                isValid = false;
            }

            return isValid;
        }

        public bool IsWorkConceptPrecertified(WorkConcept wc, int projectFiscalYear)
        {
            bool isPrecertified = true;

            if (projectFiscalYear < wc.FiscalYear)
            {
                if (wc.EarlierFiscalYear != -99)
                {
                    if (wc.EarlierFiscalYear == -1) // Check WiSAMS DN scenario
                    {

                    }
                    else
                    {
                        if (projectFiscalYear < wc.FiscalYear - wc.EarlierFiscalYear)
                        {
                            isPrecertified = false;
                        }
                    }
                }
            }
            else if (projectFiscalYear > wc.FiscalYear)
            {
                if (wc.LaterFiscalYear != -99)
                {
                    if (wc.LaterFiscalYear == -1) // Check WiSAMS DN scenario
                    {

                    }
                    else
                    {
                        if (projectFiscalYear > wc.FiscalYear + wc.LaterFiscalYear)
                        {
                            isPrecertified = false;
                        }
                    }
                }
            }

            return isPrecertified;
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            string[] parsed = workConcept.Split(new string[] { "(", ")", ";" }, StringSplitOptions.RemoveEmptyEntries);
            parsed[1] = workConcept.Remove(0, parsed[0].Length + 2).Trim();
            return parsed;
        }
    }
}
