using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WiSam.Business
{
    public partial class Progress : Form
    {
        public Progress(int startYear, int endYear, string analysisType = "Constrained")
        {
            InitializeComponent();
            progressBar1.Minimum = 1;
            progressBar1.Maximum = endYear - startYear + 2;
            progressBar1.Step = 1;
            label1.Text = String.Format("Analysis Type: {0}, Analysis Window: {1}-{2}", analysisType, startYear, endYear);
        }

        public Progress(int numberOfStructures, string analysisType = "Unconstrained")
        {
            InitializeComponent();
            progressBar1.Minimum = 1;
            progressBar1.Maximum = numberOfStructures + 1;
            //progressBar1.Value = 0;
            progressBar1.Step = 1;
            label1.Text = String.Format("Analysis Type: {0}, Number of Structures to Analyze: {1}", analysisType, numberOfStructures);
        }

        public void UpdateProgress(int progress, int year, int numberOfYears, string action = "")
        {
            progressBar1.BeginInvoke(
                new Action(() =>
                {
                    if (progress > 0)
                    {
                        progressBar1.Value = progress + 1;
                        label2.Text = String.Format("{0} year {1}... ({2}/{3})", action, year, progress, numberOfYears);
                    }
                    else
                    {
                        label3.Text = year.ToString();

                        if (progress == -2)
                        {
                            pictureBox1.Visible = true;
                        }
                    }
                }
                ));
        }

        public void UpdateProgress(int progress, string structureId, int numberOfStructures)
        {
            progressBar1.BeginInvoke(
                new Action(() =>
                {
                    if (progress > 0)
                    {
                        progressBar1.Value = progress + 1;
                        label2.Text = String.Format("Analyzed {0}... ({1}/{2})", structureId, progress, numberOfStructures);
                    }
                    else
                    {
                        label3.Text = structureId;

                        if (progress == -2)
                        {
                            pictureBox1.Visible = true;
                        }
                    }
                }
                ));
        }
    }
}
