using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WiSam.StructuresProgram
{
    public partial class FormAddWorkConceptToProject : Form
    {
        WorkConcept wc = null;
        FormMapping formMapping = null;
        List<WorkConcept> primaryWorkConcepts = null;

        public FormAddWorkConceptToProject(WorkConcept wc, FormMapping formMapping, List<WorkConcept> primaryWorkConcepts)
        {
            InitializeComponent();
            this.wc = wc;
            groupBox1.Text = this.wc.StructureId;
            this.formMapping = formMapping;
            this.primaryWorkConcepts = primaryWorkConcepts;
            PopulateWorkConcepts();
            comboBoxWorkConcept.SelectedIndex = 0;

            if (wc.WorkConceptCode.Equals("EV"))
            {
                radioButtonEvaluate.Checked = true;
            }
            else if (wc.WorkConceptCode.Equals("PR"))
            {
                radioButtonEvaluate.Checked = true;
            }
        }

        public void RefreshForm(WorkConcept wc)
        {
            this.wc = wc;
            groupBox1.Text = this.wc.StructureId;
        }

        private void PopulateWorkConcepts()
        {
            foreach (WorkConcept pwc in primaryWorkConcepts)
            {
                if (!pwc.WorkConceptCode.Equals("01") && !pwc.WorkConceptCode.Equals("00"))
                {
                    string workConceptDisplay = String.Format("({0}) {1}", pwc.WorkConceptCode, pwc.WorkConceptDescription);
                    comboBoxWorkConcept.Items.Add(workConceptDisplay);
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonAddNewWorkConcept_Click(object sender, EventArgs e)
        {
            if (radioButtonRecommend.Checked)
            {
                wc.Evaluate = false;
                wc.WorkConceptCode = "PR";
                wc.WorkConceptDescription = "PROPOSE";
                wc.Status = StructuresProgramType.WorkConceptStatus.Proposed;
                ParseSelectedWorkConcept(wc);
            }

            formMapping.OpenStructureWindow(wc);
            this.Close();
        }

        private void ParseSelectedWorkConcept(WorkConcept wc)
        {
            string selectedWorkConcept = comboBoxWorkConcept.SelectedItem.ToString();
            wc.CertifiedWorkConceptCode = selectedWorkConcept.Substring(1, 2);
            wc.CertifiedWorkConceptDescription = selectedWorkConcept.Substring(5, selectedWorkConcept.Length - 5).Trim();
        }
    }
}
