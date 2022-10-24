using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using Dw = Wisdot.Bos.Dw;

namespace WiSam.StructuresProgram
{
    class FormLoginController
    {
        private FormLogin formLogin;
        private Database database;
        private static IDatabaseService dataServ = new DatabaseService();
        private Dw.Database warehouseDatabase = new Dw.Database();

        public FormLoginController(FormLogin formLogin)
        {
            this.formLogin = formLogin;
            database = new Database("WISAMS");
        }

        internal bool ConnectToDataWarehouse()
        {
            return dataServ.OpenDatabaseConnection("WISAMS");
        }

        public bool DoLogin(string userName, string userPassword)
        {
            if (!AuthenticateUser(userName, userPassword))
            {
                return false;
            }
            else
            {
                FormHelp formHelp = null;
                FormLogin formLogin = null;

                foreach (Form form in Application.OpenForms)
                {
                    if (form.Name.Equals("FormHelp"))
                    {
                        formHelp = (FormHelp)form;
                    }

                    if (form.Name.Equals("FormLogin"))
                    {
                        formLogin = (FormLogin)form;
                    }
                }

                FormMain formMain = new FormMain(dataServ.GetUserAccount(userName, userPassword), formLogin, formHelp);
                FormHelpHide();
                //database.CloseDatabaseConnection("WISAMS");
                formMain.TopMost = false;
                formMain.StartPosition = FormStartPosition.CenterScreen;
                formMain.Show();
                formMain.WindowState = FormWindowState.Maximized;
                return true;
            }
        }

        public void Do(string action)
        {
            switch (action)
            {
                case "FormHelpShow":
                    FormHelpShow();
                    break;
                case "Exit":
                    formLogin.Close();
                    break;
                default:
                    MessageBox.Show("Invalid action.");
                    break;
            }
        }

        private bool AuthenticateUser(string userName, string userPassword)
        {
            return dataServ.AuthenticateUser(userName, userPassword);
        }

        private void FormHelpShow()
        {
            bool formHelpIsOpen = false;
            FormHelp formHelp = null;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals("FormHelp"))
                {
                    formHelpIsOpen = true;
                    formHelp = (FormHelp)form;
                }
            }

            if (!formHelpIsOpen)
            {
                formHelp = new FormHelp();
            }

            formHelp.WindowState = FormWindowState.Normal;
            formHelp.Show();
        }

        private void FormHelpHide()
        {
            bool formHelpIsOpen = false;
            FormHelp formHelp = null;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals("FormHelp"))
                {
                    formHelpIsOpen = true;
                    formHelp = (FormHelp)form;
                }
            }

            if (formHelpIsOpen)
            {
                formHelp.Hide();
            }
        }
    }
}
