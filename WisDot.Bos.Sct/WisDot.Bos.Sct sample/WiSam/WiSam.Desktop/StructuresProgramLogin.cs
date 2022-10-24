using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WiSam.Desktop
{
    public partial class StructuresProgramLogin : Form
    {
        public StructuresProgramLogin()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            new StructuresProgramHelp().Show();
        }
    }
}
