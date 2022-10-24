using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dw = Wisdot.Bos.Dw;

namespace WiSam.StructuresProgram
{
    public partial class FormLogin : Form
    {
        private FormLoginController formLoginController;
        private string windowsUserName = "";

        public FormLogin()
        {
            InitializeComponent();
            formLoginController = new FormLoginController(this);

            if (!formLoginController.ConnectToDataWarehouse())
            {
                MessageBox.Show("Unable to connect to the Structures Data Warehouse. Please inform BOS.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonLogin.Enabled = false;
            }
            else
            {
                SetupForm();
            }
        }

        private void SetupForm()
        {
            // Move later to controller?
            windowsUserName = Environment.UserName.ToLower();
            textBoxUserName.Text = windowsUserName;
            textBoxPassword.Text = windowsUserName;
            comboBoxDatabase.SelectedIndex = 0;
        }

        private void buttonOpenHelp_Click(object sender, EventArgs e)
        {
            formLoginController.Do("FormHelpShow");
        }

        private void Login()
        {
            ToggleControls(false);
            FormLoggingIn formLoggingIn = new FormLoggingIn();
            formLoggingIn.StartPosition = this.StartPosition;
            formLoggingIn.Show();

            if (formLoginController.DoLogin(textBoxUserName.Text.Trim(), textBoxPassword.Text.Trim()))
            {
                formLoggingIn.Close();
                Hide();
            }
            else
            {
                formLoggingIn.Close();
                MessageBox.Show("Invalid username or password.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ToggleControls(true);
            }
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void ToggleControls(bool enabled)
        {
            buttonLogin.Enabled = enabled;
            buttonExitApplication.Enabled = enabled;
            buttonEmailUs.Enabled = enabled;
            //pictureBoxProjectEdit.Visible = !enabled;
        }

        private void buttonExitApplication_Click(object sender, EventArgs e)
        {
            formLoginController.Do("Exit");
        }

        private void buttonEmailUs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:dotdtsdstructuresprogram@dot.wi.gov");
        }

        /*
        private void FormLogin_KeyDown(object sender, KeyEventArgs e)
        {

        }
        */

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Login();
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBoxPassword;
        }
    }
}
