using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Services;

namespace WiSam.StructuresProgram
{
    public partial class FormRequestRecertification : Form
    {
        Project project;
        FormStructureProject formStructureProject;
        UserAccount userAccount;
        string action;
        private IDatabaseService dataServ;

        public FormRequestRecertification(Project project, FormStructureProject formStructureProject, DatabaseService database, UserAccount userAccount, string action)
        {
            InitializeComponent();
            this.project = project;
            this.formStructureProject = formStructureProject;
            dataServ = database;
            this.userAccount = userAccount;
            this.action = action;
            buttonClearRecertificationReason.Enabled = false;
            buttonSubmitRecertificationRequest.Enabled = false;
            buttonGrantRecertificationRequest.Enabled = false;
            buttonRejectRecertificationRequest.Enabled = false;
            textBoxRecertificationReason.Text = project.RecertificationReason;
            textBoxRecertificationComments.Text = project.RecertificationComments;
            

            if (action.Equals("request"))
            {
                this.Text = "Request Recertification";
                buttonClearRecertificationReason.Enabled = true;
                buttonSubmitRecertificationRequest.Enabled = true;
                textBoxRecertificationReason.Enabled = true;
                textBoxRecertificationComments.ReadOnly = true;
            }
            else if (action.Equals("grant"))
            {
                this.Text = "Grant Recertification Request";
                textBoxRecertificationReason.ReadOnly = true;
                buttonGrantRecertificationRequest.Enabled = true;
                textBoxRecertificationComments.Enabled = true;
            }
            else if (action.Equals("reject"))
            {
                this.Text = "Reject Recertification Request";
                textBoxRecertificationReason.ReadOnly = true;
                buttonRejectRecertificationRequest.Enabled = true;
                textBoxRecertificationComments.Enabled = true;
            }
        }

        private void buttonSubmitRecertificationRequest_Click(object sender, EventArgs e)
        {
            if (textBoxRecertificationReason.Text.Trim().Length == 0)
            {
                MessageBox.Show("Enter reason for recertification request", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                project.Locked = false;
                project.InCertification = false;
                project.InPrecertification = false;
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserAction = StructuresProgramType.ProjectUserAction.RequestRecertification;
                project.RecertificationReason = textBoxRecertificationReason.Text.Trim();
                dataServ.CertifyProject(project, project.UserAction);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                this.Close();
            }
        }

        private void buttonGrantRecertificationRequest_Click(object sender, EventArgs e)
        {
            string comments = textBoxRecertificationComments.Text.Trim();

            if (comments.Length == 0)
            {
                MessageBox.Show("Enter comments for granting the request", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                project.Locked = false;
                project.InCertification = false;
                project.InPrecertification = false;
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserAction = StructuresProgramType.ProjectUserAction.GrantRecertification;
                project.RecertificationReason = textBoxRecertificationReason.Text.Trim();
                project.RecertificationComments = comments;
                dataServ.CertifyProject(project, project.UserAction);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                this.Close();
            }
        }

        private void buttonRejectRecertificationRequest_Click(object sender, EventArgs e)
        {
            string comments = textBoxRecertificationComments.Text.Trim();

            if (comments.Length == 0)
            {
                MessageBox.Show("Enter comments for rejecting the request", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                project.Locked = false;
                project.InCertification = false;
                project.InPrecertification = false;
                project.UserDbId = userAccount.UserDbId;
                project.UserFullName = userAccount.FirstName + " " + userAccount.LastName;
                project.UserDbIds.Add(userAccount.UserDbId);
                project.UserAction = StructuresProgramType.ProjectUserAction.RejectRecertification;
                project.RecertificationReason = textBoxRecertificationReason.Text.Trim();
                project.RecertificationComments = comments;
                dataServ.CertifyProject(project, project.UserAction);
                project.History = dataServ.GetProjectHistory(project.ProjectDbId);
                //Email.ComposeMessage(project, userAccount, dataServ.GetEmailAddresses(project.UserDbIds), dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), dataServ);
                EmailService.EmailMessage(project, userAccount, dataServ.GetApplicationMode(), Path.Combine(dataServ.GetMyDirectory(), "bos.jpg"), (DatabaseService)dataServ);
                this.Close();
            }
        }

        private void buttonClearRecertificationReason_Click(object sender, EventArgs e)
        {
            textBoxRecertificationReason.Clear();
        }
    }
}
