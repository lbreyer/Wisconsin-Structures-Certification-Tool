using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiSam.StructuresProgram
{
    public partial class FormLoggingIn : Form
    {
        public FormLoggingIn()
        {
            InitializeComponent();
        }


        private void FormLoggingIn_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                //e.Cancel = true;
                //this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
