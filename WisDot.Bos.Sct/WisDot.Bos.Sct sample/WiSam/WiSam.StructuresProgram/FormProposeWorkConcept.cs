using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WiSam.StructuresProgram
{
    public partial class FormProposeWorkConcept : Form
    {
        private WorkConcept workConcept;
        private List<WorkConcept> primaryWorkConcepts;
        private List<WorkConcept> secondaryWorkConcepts;
        private List<string> justifications;
        private FormMainController formMainController;

        internal FormProposeWorkConcept(WorkConcept workConcept, List<WorkConcept> primaryWorkConcepts, List<WorkConcept> secondaryWorkConcepts, List<string> justifications, FormMainController formMainController)
        {
            InitializeComponent();
            this.workConcept = workConcept;
            this.primaryWorkConcepts = primaryWorkConcepts;
            this.secondaryWorkConcepts = secondaryWorkConcepts;
            this.justifications = justifications;
            this.formMainController = formMainController;
        }

        private void FormProposeWorkConcept_Load(object sender, EventArgs e)
        {
            textBoxNewWorkConceptStructureId.Text = workConcept.StructureId;
            PopulateWorkConcepts();
            string certifiedWorkConceptCode = String.Format("({0})", workConcept.CertifiedWorkConceptCode);

            if (certifiedWorkConceptCode.Equals("01"))
            {
                textBoxNewWorkConceptStructureId.ReadOnly = false;
                comboBoxNewWorkConcept.Enabled = false;
            }

            textBoxNewWorkConceptFiscalYear.Text = workConcept.FiscalYear != 0 ? workConcept.FiscalYear.ToString() : "";

            foreach (var item in comboBoxNewWorkConcept.Items)
            {
                if (item.ToString().Contains(certifiedWorkConceptCode))
                {
                    comboBoxNewWorkConcept.SelectedItem = item;
                    break;
                }
            }

            PopulateJustifications();
            
            if (String.IsNullOrEmpty(workConcept.ReasonCategory))
            {
                comboBoxNewWorkConceptReasonCategory.SelectedIndex = 0;
            }
            else
            {
                foreach (var item in comboBoxNewWorkConceptReasonCategory.Items)
                {
                    if (item.ToString().Equals(workConcept.ReasonCategory.ToUpper()))
                    {
                        comboBoxNewWorkConceptReasonCategory.SelectedItem = item;
                        break;
                    }
                }
            }

            textBoxNewWorkConceptReasonExplanation.Text = workConcept.Notes;
        }

        private void PopulateWorkConcepts()
        {
            if (workConcept.CertifiedWorkConceptCode.Equals("01"))
            {
                comboBoxNewWorkConcept.Items.Add(String.Format("({0}) {1}", "01", workConcept.CertifiedWorkConceptDescription));
            }
            else
            {
                comboBoxNewWorkConcept.Items.Add(String.Format("({0}) {1}", "PR", "SELECT WORK CONCEPT..."));
                comboBoxNewWorkConcept.Items.Add("----------");

                foreach (WorkConcept pwc in primaryWorkConcepts)
                {
                    string workConceptDisplay = String.Format("({0}) {1}", pwc.WorkConceptCode, pwc.WorkConceptDescription);

                    if (!pwc.WorkConceptCode.Equals("00") && !pwc.WorkConceptCode.Equals("01"))
                    {
                        comboBoxNewWorkConcept.Items.Add(workConceptDisplay);
                    }
                }

                comboBoxNewWorkConcept.Items.Add("----------");
                comboBoxNewWorkConcept.Items.Add(String.Format("({0}) {1}", "EV", "EVALUATE FOR SECONDARY WORK CONCEPTS"));
                comboBoxNewWorkConcept.Items.Add("----------");

                foreach (WorkConcept swc in secondaryWorkConcepts)
                {
                    string workConceptDisplay = String.Format("({0}) {1}", swc.WorkConceptCode, swc.WorkConceptDescription);
                    comboBoxNewWorkConcept.Items.Add(workConceptDisplay);
                }
            }
        }

        private void PopulateJustifications()
        {
            comboBoxNewWorkConceptReasonCategory.Items.Add("SELECT JUSTIFICATION...");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(00) Structural");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(01) ProximityToOtherWork");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(02) LaneClosureRestriction");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(03) ExpansionDevelopment");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(04) LccaSecondaryWorkConcepts");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(05) LccaSharedTrafficControlCosts");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(06) LccaSharedMobilizationCosts");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(07) LccaOther");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(50) Other");
            /* comboBoxNewWorkConceptReasonCategory.Items.Add("(00) Structural");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(01) ProximityToOtherWork");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(02) LaneClosureRestriction");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(03) ExpansionDevelopment");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(04) LccaSecondaryWorkConcepts");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(05) LccaSharedTrafficControlCosts");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(06) LccaSharedMobilizationCosts");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(07) LccaOther");
            comboBoxNewWorkConceptReasonCategory.Items.Add("(50) Other");*/

            /*
            foreach (var j in justifications)
            {
                comboBoxNewWorkConceptReasonCategory.Items.Add(j.ToUpper());
            }*/
        }

        private void buttonAddNewWorkConcept_Click(object sender, EventArgs e)
        {
            string structureId = textBoxNewWorkConceptStructureId.Text.Trim();
            string selectedWorkConcept = comboBoxNewWorkConcept.SelectedItem.ToString().ToUpper();
            string selectedJustification = comboBoxNewWorkConceptReasonCategory.SelectedItem.ToString();
            string explanation = textBoxNewWorkConceptReasonExplanation.Text.Trim();
            int fiscalYear = 0;
            bool validFiscalYear = false;

            if (selectedWorkConcept.Contains("SELECT WORK CONCEPT") || selectedWorkConcept.Contains("----------"))
            {
                MessageBox.Show("Select work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                fiscalYear = Convert.ToInt32(textBoxNewWorkConceptFiscalYear.Text.Trim());
                validFiscalYear = formMainController.ValidateFiscalYear(fiscalYear);
            }
            catch
            { }

            if (!validFiscalYear)
            {
                MessageBox.Show(String.Format("Enter an Fy >= {0} but <= {1}", formMainController.GetWorkConceptStartFiscalYear(),
                                               formMainController.GetWorkConceptEndFiscalYear()), "SCT",
                                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (selectedJustification.Contains("SELECT JUSTIFICATION"))
            {
                MessageBox.Show("Select justification category for the work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (explanation.Length == 0)
            {
                MessageBox.Show("Enter justification explanation for the work concept.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int projectYear = formMainController.GetProjectYearBasedOnFiscalYear(fiscalYear);

            if (formMainController.DoesStructureHaveAnEligibleWorkConcept(structureId))
            {
                DialogResult result = MessageBox.Show(String.Format("{0} already has an eligible work concept. Continue?", structureId), "SCT", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }
            }

            string[] workConcept = formMainController.ParseWorkConceptFullDescription(selectedWorkConcept);
            string workConceptCode = workConcept[0];
            string workConceptDescription = workConcept[1];
            formMainController.DoProposedWorkConceptAdd(0, structureId, workConceptCode, workConceptDescription, fiscalYear, projectYear,
                selectedJustification, explanation);
            this.Close();
        }

        private void buttonClearProposedWorkConceptFields_Click(object sender, EventArgs e)
        {
            // Structure Id's only updatable if new structure
            if (workConcept.CertifiedWorkConceptCode.Equals("01"))
            {
                textBoxNewWorkConceptStructureId.Text = "";
            }

            comboBoxNewWorkConcept.SelectedIndex = 0;
            textBoxNewWorkConceptFiscalYear.Text = "";
            comboBoxNewWorkConceptReasonCategory.SelectedIndex = 0;
            textBoxNewWorkConceptReasonExplanation.Text = "";
        }
    }
}
