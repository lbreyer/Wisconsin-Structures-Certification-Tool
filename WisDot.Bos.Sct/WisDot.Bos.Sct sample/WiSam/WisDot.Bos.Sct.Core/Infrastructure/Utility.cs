using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Data;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class Utility
    {
        public static Form GetForm(string formName)
        {
            Form form = null;

            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name.Equals(formName))
                {
                    form = openForm;
                    break;
                }
            }

            return form;
        }

        public static List<Form> GetForms(string formName)
        {
            List<Form> forms = new List<Form>();

            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm.Name.Equals(formName))
                {
                    forms.Add(openForm);
                }
            }

            return forms;
        }

        public static string StructureIdToFolderName(string structureId)
        {
            string folderName = structureId.Substring(0, 3) + "-";

            if (!structureId.Substring(3, 1).Equals("0"))
            {
                folderName += structureId.Substring(3, 1);
            }

            folderName += structureId.Substring(4, 3);

            if (structureId.Length == 11)
            {
                folderName += "-" + structureId.Substring(7, 4);
            }

            return folderName;
        }

        public static string FormatConstructionId(string constructionId)
        {
            string formattedId = constructionId;

            try
            {
                formattedId = String.Format("{0}-{1}-{2}", formattedId.Substring(0, 4), formattedId.Substring(4, 2), formattedId.Substring(6, 2));
            }
            catch { }

            return formattedId;
        }

        public static string FormatStructureId(string structureId)
        {
            string formattedId = structureId.Trim();

            if (formattedId.Length == 7)
            {
                try
                {
                    formattedId = String.Format("{0}-{1}-{2}", formattedId.Substring(0, 1), formattedId.Substring(1, 2), formattedId.Substring(3, 4));
                }
                catch { }
            }
            else if (formattedId.Length == 11)
            {
                try
                {
                    formattedId = String.Format("{0}-{1}-{2}-{3}", formattedId.Substring(0, 1), formattedId.Substring(1, 2), formattedId.Substring(3, 4), formattedId.Substring(7, 4));
                }
                catch { }
            }

            return formattedId;
        }

        public static string TranslateStructureId(string structureId)
        {
            string translatedStructureId = null;
            string givenStructureId = structureId.ToUpper().Trim();
            int numberOfDashes = structureId.Count(c => c == '-');
            string structureType = "";
            string countyNumber = "";
            string id = "";

            if (numberOfDashes > 0)
            {
                if (numberOfDashes == 1)
                {
                    string firstPart = givenStructureId.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    string secondPart = givenStructureId.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                    structureType = firstPart.Substring(0, 1);
                    countyNumber = firstPart.Substring(1).Trim();
                    id = secondPart;
                }
                else if (numberOfDashes == 2)
                {
                    string firstPart = givenStructureId.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    string secondPart = givenStructureId.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                    string thirdPart = givenStructureId.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[2].Trim();
                    structureType = firstPart.Trim();
                    countyNumber = secondPart.Trim();
                    id = thirdPart.Trim();
                }

                if (numberOfDashes == 1 || numberOfDashes == 2)
                {
                    if (countyNumber.Length == 1)
                    {
                        countyNumber = String.Format("0{0}", countyNumber);
                    }

                    if (id.Length == 1)
                    {
                        id = String.Format("000{0}", id);
                    }
                    else if (id.Length == 2)
                    {
                        id = String.Format("00{0}", id);
                    }
                    else if (id.Length == 3)
                    {
                        id = String.Format("0{0}", id);
                    }

                    translatedStructureId = String.Format("{0}{1}{2}", structureType, countyNumber, id);
                }
                else
                {
                    translatedStructureId = givenStructureId.Replace("-", "");
                }
            }
            else
            {
                translatedStructureId = givenStructureId;
            }

            return translatedStructureId;
        }

        public static string TranslateWorkConceptJustification(StructuresProgramType.WorkConceptChangeJustification j)
        {
            string justification = "";

            switch (j)
            {
                case StructuresProgramType.WorkConceptChangeJustification.ExpansionDevelopment:
                    justification = "Expansion Development";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.LaneClosureRestriction:
                    justification = "Lane Closure Restriction";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.LccaOther:
                    justification = "Life Cycle Cost Analysis - Other";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.LccaSecondaryWorkConcepts:
                    justification = "Life Cycle Cost Analysis - Secondary Work Concepts";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.LccaSharedMobilizationCosts:
                    justification = "Life Cycle Cost Analysis - Shared Mobilization Costs";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.LccaSharedTrafficControlCosts:
                    justification = "Life Cycle Cost Analysis - Shared Traffic Control Costs";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.Other:
                    justification = "Other";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.ProximityToOtherWork:
                    justification = "Proximity To Other Work";
                    break;
                case StructuresProgramType.WorkConceptChangeJustification.Structural:
                    justification = "Structural";
                    break;
            }

            return justification;
        }

        public static string TranslateWorkConceptJustifications(string text)
        {
            string translatedText =
                text.Replace(StructuresProgramType.WorkConceptChangeJustification.ExpansionDevelopment.ToString(), "Expansion Development")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.LaneClosureRestriction.ToString(), "Lane Closure Restriction")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.LccaOther.ToString(), "Life Cycle Cost Analysis - Other")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.LccaSecondaryWorkConcepts.ToString(), "Life Cycle Cost Analysis - Secondary Work Concepts")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.LccaSharedMobilizationCosts.ToString(), "Life Cycle Cost Analysis - Shared Mobilization Costs")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.LccaSharedTrafficControlCosts.ToString(), "Life Cycle Cost Analysis - Shared Traffic Control Costs")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.Other.ToString(), "Other")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.ProximityToOtherWork.ToString(), "Proximity To Other Work")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.ProximityToOtherWork.ToString().ToUpper(), "Proximity To Other Work")
                .Replace(StructuresProgramType.WorkConceptChangeJustification.Structural.ToString(), "Structural")
                .Replace("STRUCTURAL", "Structural")
                .Replace("EXPANSIONCAPACITYINCREASE", "Expansion Capacity Increase")
                .Replace("PROXIMITYTOOTHERWORK", "Proximity to other Work")
                .Replace("OTHER", "Other");
            string laneclosure = StructuresProgramType.WorkConceptChangeJustification.LaneClosureRestriction.ToString();
            return translatedText;
        }
    }
}
