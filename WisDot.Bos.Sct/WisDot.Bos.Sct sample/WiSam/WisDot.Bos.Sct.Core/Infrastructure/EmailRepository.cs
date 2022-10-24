using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class EmailRepository : IEmailRepository
    {
        private static IDatabaseService dataServ = new DatabaseService();

        public string ComposeMessage(Project project, UserAccount userAccount, string toList, string applicationMode, string imagePath, DatabaseService database, bool missingFiipsProject = false, string workflowPhase = "certification")
        {
            string message = "";
            string subject = String.Format("Structures Certification Tool Notification: Structures Project {0}", project.ProjectDbId);

            if (!applicationMode.Equals("PROD"))
            {
                subject += String.Format(" in {0} Environment", applicationMode);
            }

            string userAction = dataServ.GetWorkflowStatus(project);
            string[] to = new string[1];
            to[0] = toList;
            string[] ccList = new string[] { "najoua.ksontini@dot.wi.gov", "laura.shadewald@dot.wi.gov", "aaron.bonk@dot.wi.gov",
                                                "emily.kuehne@dot.wi.gov", "dominique.bechle@dot.wi.gov", "jonathon.resheske@dot.wi.gov",
                                                "ruth.coisman@dot.wi.gov",
                                                "joshua.dietsche@dot.wi.gov", "philip.meinel@dot.wi.gov", "ryan.bowers@dot.wi.gov", "joseph.barut@dot.wi.gov" };
            string[] bccList = new string[] { "scot.becker@dot.wi.gov" };

            if (applicationMode.Equals("DEV"))
            {
                to = new string[] { "joseph.barut@dot.wi.gov" };
                ccList = to;
                bccList = to;
            }
            else if (applicationMode.Equals("TEST"))
            {
                to = new string[] { "DOTDTSDStructuresProgram@dot.wi.gov" };
            }


            string projectColor = "green";
            string projectTextColor = "white";

            if (project.Status == StructuresProgramType.ProjectStatus.Precertified)
            {
                projectColor = "yellow";
                projectTextColor = "black";
            }
            else if (project.Status == StructuresProgramType.ProjectStatus.Unapproved)
            {
                projectColor = "red";
            }

            //message = String.Format("Dear {0} {1},</br></br>", userAccount.FirstName, userAccount.LastName);
            message = String.Format("To Whom It May Concern,</br></br>");

            if (missingFiipsProject)
            {
                if (workflowPhase.Equals("certification"))
                {
                    message += String.Format("This is a notification that this structures project {0} requires a corresponding FIIPS project.</br>",
                                                project.ProjectDbId);
                }
                else if (workflowPhase.Equals("precertification"))
                {
                    message += String.Format("This is a notification that this structures project {0} requires a corresponding FIIPS project.</br>",
                                               project.ProjectDbId);
                }
            }
            else
            {
                message += String.Format("This is an auto notification of a transaction in a structures project {0} listed below.</br></br>", project.ProjectDbId);
                message += String.Format("<b>Transaction:</b> {0}</br>", userAction);
            }

            message += String.Format("<b>Structures Project:</b> {0}</br>", project.ProjectDbId);
            message += String.Format("<b>Project Description:</b> {0}</br>", project.Description);
            message += String.Format("<b>Project Fy:</b> {0}</br>", project.FiscalYear);
            message += String.Format("<b>Advanceable Project Fy:</b> {0}</br>", project.AdvanceableFiscalYear > 0 ? project.AdvanceableFiscalYear.ToString() : "");
            message += String.Format("<b>Project Status:</b> <tt style='background-color:{0};color:{1}'>{2}</tt></br>", projectColor, projectTextColor, project.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : project.Status.ToString());
            message += String.Format("<b>Corresponding FIIPS Project Construction ID:</b> {0}</br>", !String.IsNullOrEmpty(project.FosProjectId) ? project.FosProjectId : "Unassigned");
            message += String.Format("</br></br><b>Project Work Concepts ({0})</b>", project.WorkConcepts.Count());
            message += String.Format("</br><table style='width:100%;border-width:1px;padding:3px;border-style:solid;text-align:left'><tr><td><b>Structure Id</b></td><td><b>Work Concept To Be Certified</b></td>");
            message += "<td><b>Work Concept Status</b></td></tr>";

            foreach (var wc in project.WorkConcepts)
            {
                string color = "green";
                string textColor = "white";

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified)
                {
                    color = "yellow";
                    textColor = "black";
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                {
                    color = "red";
                }

                message += String.Format("<tr>");
                message += String.Format("<td><a href='#{0}'>{1}</a></td>", wc.StructureId, wc.StructureId);
                // message += String.Format("<td>{0}</td>", wc.WorkConceptDescription);
                message += String.Format("<td>({0}) {1}</td>", wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription);
                //message += String.Format("<td>{0}</td>", wc.FiscalYear);
                message += String.Format("<td style='background-color:{0};color:{1}'>{2}</td>", color, textColor, wc.Status.ToString());
                message += String.Format("</tr>");
            }

            message += "</table></br></br>";

            foreach (var wc in project.WorkConcepts)
            {
                string color = "green";
                string textColor = "white";

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified)
                {
                    color = "yellow";
                    textColor = "black";
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                {
                    color = "red";
                }

                message += String.Format("<b id = '{0}'>{1}</b>", wc.StructureId, wc.StructureId);
                message += String.Format("</br>");
                message += String.Format("<tt style='background-color:{0};color:{1}'>Status: {2}</tt>", color, textColor, wc.Status);
                message += String.Format("</br>");
                message += String.Format("Change Justification Category: {0}", Utility.TranslateWorkConceptJustifications(wc.ChangeJustifications));
                message += String.Format("</br>");
                message += String.Format("Change Justification Notes: {0}", Utility.TranslateWorkConceptJustifications(wc.ChangeJustificationNotes));
                message += String.Format("</br>");

                if (wc.PrecertificationDecision != StructuresProgramType.PrecertificatioReviewDecision.None || !String.IsNullOrEmpty(wc.PrecertificationDecisionReasonExplanation))
                {
                    string precertificationDetails = "";
                    precertificationDetails = String.Format("Liaison: {0}</br>Decision: {1}</br>Explanation: {2}", project.PrecertificationLiaisonUserFullName, wc.PrecertificationDecision, wc.PrecertificationDecisionReasonExplanation);
                    message += String.Format("<p>Precertification:</br>{0}</p>", precertificationDetails);
                }

                if (project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                {
                    string certificationDetails = "";

                    if (project.FromExcel)
                    {
                        certificationDetails = "Manually Transitionally Certified";
                    }
                    else if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                    {
                        certificationDetails = "Transitionally Certified";
                    }
                    else
                    {
                        string secondaryWorkConcepts = "";

                        foreach (var ewc in project.CertifiedElementWorkConceptCombinations.Where(el => el.WorkConceptLevel.ToUpper().Equals("SECONDARY")))
                        {
                            secondaryWorkConcepts += String.Format("({0}) {1}; ", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                        }

                        certificationDetails = String.Format("Liaison: {0}" +
                            "</br>Primary Work Concept Comments: {1}" +
                            "</br>Secondary Work Concepts: {2}" +
                            "</br>Secondary Work Concepts Comments: {3}" +
                            "</br>Additional Comments: {4}", project.CertificationLiaisonUserFullName,
                            wc.CertificationPrimaryWorkTypeComments,
                            secondaryWorkConcepts,
                            wc.CertificationSecondaryWorkTypeComments,
                            wc.CertificationAdditionalComments);
                    }

                    message += String.Format("<p>Certification:</br>{0}</p>", certificationDetails);
                }

                message += String.Format("</br></br>");
            }

            message += "Thank you,</br>WisDOT Bureau of Structures</br></br>";
            message += String.Format("<img src='{0}'>", imagePath);




            EmailService.ComposeMessage(subject, message, to, null, ccList, bccList);
            return message;
        }

        public bool ComposeMessage(string subject, string message, string[] to, [Optional] string[] attachments, [Optional] string[] cc, [Optional] string[] bcc, [Optional] MailPriority mailPriority)
        {
            /* Default is a successful sending of the email, but might want to change default logic to be false (instead of true) */
            bool successfulSend = true;

            /* In App.Config files (there are 3 app.config files in this entire project) */
            string from = ConfigurationManager.AppSettings.GetValues("EmailAddress")[0];

            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();

            /* The following lines are requirements either from BITS or SMTP */
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "mailx.dot.state.wi.us";
            mail.From = new MailAddress(from);

            if (!IsNullOrEmpty(to) && !to[0].Equals(""))
            {
                foreach (string recipient in to)
                {
                    mail.To.Add(recipient);
                }
            }
            else
            {
                successfulSend = false;
            }

            if (!IsNullOrEmpty(attachments) && !attachments[0].Equals(""))
            {
                foreach (string file in attachments)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
            }

            mail.CC.Add(from);
            if (!IsNullOrEmpty(cc) && !cc[0].Equals(""))
            {
                foreach (string person in cc)
                {
                    mail.CC.Add(person);
                }
            }

            if (!IsNullOrEmpty(bcc) && !bcc[0].Equals(""))
            {
                foreach (string person in bcc)
                {
                    mail.Bcc.Add(person);
                }
            }

            if (!string.IsNullOrWhiteSpace(subject) && !string.IsNullOrWhiteSpace(message))
            {
                mail.Subject = subject;
                mail.Body = message;
            }
            else
            {
                successfulSend = false;
            }

            mail.IsBodyHtml = true;

            /* Making sure that email has all the parameters needed to send correctly, required params are To, Subject, Body */
            try
            {
                if (successfulSend)
                    client.Send(mail);
            }
            catch (Exception e)
            {
                successfulSend = false;
            }

            mail.Dispose();
            client.Dispose();

            /* Return true on successful sending of email */
            return successfulSend;
        }

        public string EmailMessage(Project project, UserAccount userAccount, string applicationMode, string imagePath, DatabaseService database, bool missingFiipsProject = false, string workflowPhase = "certification")
        {
            string message = "";
            string subject = String.Format("Structures Certification Tool Notification: Structures Project {0}", project.ProjectDbId);

            if (!applicationMode.Equals("PROD"))
            {
                subject += String.Format(" in {0} Environment", applicationMode);
            }

            string userAction = database.GetWorkflowStatus(project);
            string regionalTransactors = database.GetEmailAddressesRegionalTransactors(project.UserDbIds);
            string[] toList = new string[] { "" };

            if (!String.IsNullOrEmpty(regionalTransactors))
            {
                toList = new string[] { regionalTransactors };
            }
            else
            {
                toList = new string[] { database.GetEmailAddress(project.UserDbId) };
            }

            string[] ccList = new string[] { "DOTDTSDStructuresProgram@dot.wi.gov" };
            string[] bccList = new string[] { "joseph.barut@dot.wi.gov; scot.becker@dot.wi.gov" };
            string cc = "";

            switch (project.UserAction)
            {
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForPrecertification:
                    cc = database.GetPrecertificationLiaisonsEmailAddresses();

                    if (project.Status == StructuresProgramType.ProjectStatus.Precertified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified
                        || project.Status == StructuresProgramType.ProjectStatus.Certified)
                    {
                        cc = String.Format("{0},{1},{2}", cc, database.GetCertificationLiaisonsEmailAddresses(), database.GetCertificationSupervisorsEmailAddresses());
                    }

                    if (!String.IsNullOrEmpty(project.NotificationRecipients))
                    {
                        cc = String.Format("{0},{1}", cc, project.NotificationRecipients);
                    }

                    if (project.History.ToLower().Contains("rejected for certification"))
                    {
                        cc = String.Format("{0},{1},{2}", cc, database.GetCertificationLiaisonsEmailAddresses(), database.GetCertificationLiaisonsEmailAddresses());
                    }

                    ccList = new string[] { cc };
                    break;
                case StructuresProgramType.ProjectUserAction.Deactivated:
                    cc = database.GetPrecertificationLiaisonsEmailAddresses();
                    ccList = new string[] { cc };
                    break;
                case StructuresProgramType.ProjectUserAction.Precertification:
                case StructuresProgramType.ProjectUserAction.BosRejectedPrecertification:
                    cc = database.GetPrecertificationLiaisonsEmailAddresses();

                    if (!String.IsNullOrEmpty(project.NotificationRecipients))
                    {
                        cc = String.Format("{0},{1}", cc, project.NotificationRecipients);
                    }

                    if (project.History.ToLower().Contains("rejected for certification"))
                    {
                        cc = String.Format("{0},{1},{2}", cc, database.GetCertificationLiaisonsEmailAddresses(), database.GetCertificationLiaisonsEmailAddresses());
                    }

                    ccList = new string[] { cc };
                    break;
                case StructuresProgramType.ProjectUserAction.BosAcceptedPrecertification:
                case StructuresProgramType.ProjectUserAction.BosTransitionallyCertified:
                    cc = String.Format("{0},{1},{2}", database.GetPrecertificationLiaisonsEmailAddresses(),
                                                        database.GetCertificationLiaisonsEmailAddresses(),
                                                        database.GetCertificationSupervisorsEmailAddresses());

                    if (!String.IsNullOrEmpty(project.NotificationRecipients))
                    {
                        cc = String.Format("{0},{1}", cc, project.NotificationRecipients);
                    }


                    ccList = new string[] { cc };
                    break;
                case StructuresProgramType.ProjectUserAction.Certification:
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification:
                case StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection:
                    string toListString = String.Format("{0},{1},{2}", database.GetCertificationSupervisorsEmailAddresses(), database.GetEmailAddress(project.CertificationLiaisonUserDbId), !String.IsNullOrEmpty(regionalTransactors) ? regionalTransactors : database.GetEmailAddress(project.UserDbId));
                    toList = new string[] { toListString };
                    break;
                case StructuresProgramType.ProjectUserAction.BosCertified:
                case StructuresProgramType.ProjectUserAction.BosRejectedCertification:
                    cc = String.Format("{0},{1},{2}", database.GetPrecertificationLiaisonsEmailAddresses(),
                                        database.GetCertificationSupervisorsEmailAddresses(),
                                        database.GetEmailAddress(project.CertificationLiaisonUserDbId));

                    if (!String.IsNullOrEmpty(project.NotificationRecipients))
                    {
                        cc = String.Format("{0},{1}", cc, project.NotificationRecipients);
                    }

                    ccList = new string[] { cc };
                    break;
                case StructuresProgramType.ProjectUserAction.RequestRecertification:
                    if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                    {
                        cc = String.Format("{0}", database.GetPrecertificationLiaisonsEmailAddresses());
                    }
                    else if (project.Status == StructuresProgramType.ProjectStatus.Certified)
                    {
                        cc = String.Format("{0}", database.GetCertificationSupervisorsEmailAddresses());
                    }

                    if (!String.IsNullOrEmpty(project.NotificationRecipients))
                    {
                        cc = String.Format("{0},{1}", cc, project.NotificationRecipients);
                    }

                    if (project.History.ToLower().Contains("rejected for certification"))
                    {
                        cc = String.Format("{0},{1},{2}", cc, database.GetCertificationLiaisonsEmailAddresses(), database.GetCertificationLiaisonsEmailAddresses());
                    }

                    ccList = new string[] { cc };
                    break;
                case StructuresProgramType.ProjectUserAction.RejectRecertification:
                case StructuresProgramType.ProjectUserAction.GrantRecertification:
                    cc = String.Format("{0},{1}", database.GetPrecertificationLiaisonsEmailAddresses(), database.GetCertificationSupervisorsEmailAddresses());

                    if (!String.IsNullOrEmpty(project.NotificationRecipients))
                    {
                        cc = String.Format("{0},{1}", cc, project.NotificationRecipients);
                    }

                    if (project.History.ToLower().Contains("rejected for certification"))
                    {
                        cc = String.Format("{0},{1}", cc, database.GetCertificationLiaisonsEmailAddresses());
                    }

                    ccList = new string[] { cc };
                    break;
            }

            if (applicationMode.Equals("DEV"))
            {
                toList = new string[] { "joseph.barut@dot.wi.gov" };
                ccList = toList;

                if (!String.IsNullOrEmpty(project.NotificationRecipients))
                {
                    ccList = new string[] { project.NotificationRecipients };
                }

                bccList = toList;
            }
            else if (applicationMode.Equals("TEST"))
            {
                toList = new string[] { "DOTDTSDStructuresProgram@dot.wi.gov" };
                ccList = new string[] { "najoua.ksontini@dot.wi.gov", "laura.shadewald@dot.wi.gov", "aaron.bonk@dot.wi.gov",
                                                "emily.kuehne@dot.wi.gov", "dominique.bechle@dot.wi.gov", "jonathon.resheske@dot.wi.gov",
                                                "ruth.coisman@dot.wi.gov",
                                                "joshua.dietsche@dot.wi.gov", "philip.meinel@dot.wi.gov", "ryan.bowers@dot.wi.gov", "joseph.barut@dot.wi.gov" };
            }

            if (missingFiipsProject)
            {
                ccList = new string[] { database.GetEmailAddress(project.UserDbId) };
            }

            string projectColor = "green";
            string projectTextColor = "white";

            if (project.Status == StructuresProgramType.ProjectStatus.Precertified)
            {
                projectColor = "yellow";
                projectTextColor = "black";
            }
            else if (project.Status == StructuresProgramType.ProjectStatus.Unapproved)
            {
                projectColor = "red";
            }

            message = String.Format("To Whom It May Concern,</br></br>");

            if (missingFiipsProject)
            {
                message += String.Format("This is a notification that this structures project {0} requires a corresponding FIIPS project.</br>",
                                                project.ProjectDbId);
            }
            else
            {
                message += String.Format("This is an auto notification of a transaction in a structures project {0} listed below.</br></br>", project.ProjectDbId);
                if (project.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
                {
                    message += String.Format("<font color='red'><b>Transaction: {0}</b></font></br>", userAction);
                }
                else
                {
                    message += String.Format("<b>Transaction:</b> {0}</br>", userAction);
                }
            }

            if (project.UserAction == StructuresProgramType.ProjectUserAction.RequestRecertification
                || project.UserAction == StructuresProgramType.ProjectUserAction.GrantRecertification
                || project.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
            {
                message += String.Format("<b>Reason for Recertification Request:</b> {0}</br>", project.RecertificationReason);

                if (project.UserAction == StructuresProgramType.ProjectUserAction.GrantRecertification
                        || project.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
                {
                    string recertificationDecision = "Granted";

                    if (project.UserAction == StructuresProgramType.ProjectUserAction.RejectRecertification)
                    {
                        recertificationDecision = "Rejected";
                    }

                    message += String.Format("<b>BOS Decison & Comments:</b> {0}; {1}</br>", recertificationDecision, project.RecertificationComments);
                }
            }

            message += String.Format("<b>Structures Project:</b> {0}</br>", project.ProjectDbId);
            message += String.Format("<b>Project Description:</b> {0}</br>", project.Description);
            message += String.Format("<b>Project Fy:</b> {0}</br>", project.FiscalYear);
            message += String.Format("<b>Advanceable Project Fy:</b> {0}</br>", project.AdvanceableFiscalYear > 0 ? project.AdvanceableFiscalYear.ToString() : "");
            message += String.Format("<b>Project Status:</b> <tt style='background-color:{0};color:{1}'>{2}</tt></br>", projectColor, projectTextColor, project.Status == StructuresProgramType.ProjectStatus.QuasiCertified ? "Transitionally Certified" : project.Status.ToString());
            message += String.Format("<b>Corresponding FIIPS Project Construction ID:</b> {0}</br>", !String.IsNullOrEmpty(project.FosProjectId) ? project.FosProjectId : "Unassigned");
            message += String.Format("</br></br><b>Project Work Concepts ({0})</b>", project.WorkConcepts.Count());
            message += String.Format("</br><table style='width:100%;border-width:1px;padding:3px;border-style:solid;text-align:left'><tr><td><b>Structure Id</b></td><td><b>Work Concept To Be Certified</b></td>");
            message += "<td><b>Work Concept Status</b></td></tr>";

            foreach (var wc in project.WorkConcepts)
            {
                string color = "green";
                string textColor = "white";

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified)
                {
                    color = "yellow";
                    textColor = "black";
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                {
                    color = "red";
                }

                message += String.Format("<tr>");
                message += String.Format("<td><a href='#{0}'>{1}</a></td>", wc.StructureId, wc.StructureId);
                // message += String.Format("<td>{0}</td>", wc.WorkConceptDescription);
                message += String.Format("<td>({0}) {1}</td>", wc.CertifiedWorkConceptCode, wc.CertifiedWorkConceptDescription);
                //message += String.Format("<td>{0}</td>", wc.FiscalYear);
                message += String.Format("<td style='background-color:{0};color:{1}'>{2}</td>", color, textColor, wc.Status.ToString());
                message += String.Format("</tr>");
            }

            message += "</table></br></br>";

            foreach (var wc in project.WorkConcepts)
            {
                string color = "green";
                string textColor = "white";

                if (wc.Status == StructuresProgramType.WorkConceptStatus.Precertified)
                {
                    color = "yellow";
                    textColor = "black";
                }
                else if (wc.Status == StructuresProgramType.WorkConceptStatus.Unapproved)
                {
                    color = "red";
                }

                message += String.Format("<b id = '{0}'>{1}</b>", wc.StructureId, wc.StructureId);
                message += String.Format("</br>");
                message += String.Format("<tt style='background-color:{0};color:{1}'>{2}</tt>", color, textColor, wc.Status.ToString());
                message += String.Format("</br>");
                message += String.Format("Change Justification Category: {0}", Utility.TranslateWorkConceptJustifications(wc.ChangeJustifications));
                message += String.Format("</br>");
                message += String.Format("Change Justification Notes: {0}", Utility.TranslateWorkConceptJustifications(wc.ChangeJustificationNotes));
                message += String.Format("</br>");

                if (wc.PrecertificationDecision != StructuresProgramType.PrecertificatioReviewDecision.None || !String.IsNullOrEmpty(wc.PrecertificationDecisionReasonExplanation))
                {
                    string precertificationDetails = "";
                    precertificationDetails = String.Format("Liaison: {0}</br>Decision: {1}</br>Explanation: {2}", project.PrecertificationLiaisonUserFullName, wc.PrecertificationDecision, wc.PrecertificationDecisionReasonExplanation);
                    message += String.Format("<p>Precertification:</br>{0}</p>", precertificationDetails);
                }


                if (project.Status == StructuresProgramType.ProjectStatus.Certified || project.Status == StructuresProgramType.ProjectStatus.QuasiCertified
                    || project.Status == StructuresProgramType.ProjectStatus.Rejected
                    || project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForRejection
                    || project.UserAction == StructuresProgramType.ProjectUserAction.SubmittedProjectForCertification)
                {
                    string certificationDetails = "";

                    if (project.FromExcel)
                    {
                        certificationDetails = "Manually Transitionally Certified";
                    }
                    else if (project.Status == StructuresProgramType.ProjectStatus.QuasiCertified)
                    {
                        certificationDetails = "Transitionally Certified";
                    }
                    else
                    {
                        string secondaryWorkConcepts = "";

                        foreach (var ewc in project.CertifiedElementWorkConceptCombinations.Where(el => el.WorkConceptLevel.ToUpper().Equals("SECONDARY")
                                                                                                    && el.StructureId.Equals(wc.StructureId)))
                        {
                            secondaryWorkConcepts += String.Format("({0}) {1}; ", ewc.WorkConceptCode, ewc.WorkConceptDescription);
                        }

                        certificationDetails = String.Format("Liaison: {0}" +
                            "</br>Primary Work Concept Comments: {1}" +
                            "</br>Secondary Work Concepts: {2}" +
                            "</br>Secondary Work Concepts Comments: {3}" +
                            "</br>Additional Comments: {4}", project.CertificationLiaisonUserFullName,
                            wc.CertificationPrimaryWorkTypeComments,
                            secondaryWorkConcepts,
                            wc.CertificationSecondaryWorkTypeComments,
                            wc.CertificationAdditionalComments);
                    }

                    message += String.Format("<p>Certification:</br>{0}</p>", certificationDetails);
                }

                message += String.Format("</br></br>");
            }

            message += "Thank you,</br>WisDOT Bureau of Structures</br></br>";
            message += String.Format("<img src='{0}'>", imagePath);
            ComposeMessage(subject, message, toList, null, ccList, bccList);
            return message;
        }

        public bool IsNullOrEmpty(Array array)
        {
            return (array == null || array.Length == 0);
        }
    }
}
