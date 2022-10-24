using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class EmailService
    {
        private static IEmailRepository repo = new EmailRepository();

        public static string EmailMessage(Project project, UserAccount userAccount, string applicationMode, string imagePath, DatabaseService database, bool missingFiipsProject = false, string workflowPhase = "certification")
        {
            return repo.EmailMessage(project, userAccount, applicationMode, imagePath, database, missingFiipsProject, workflowPhase);
        }

        public static string ComposeMessage(Project project, UserAccount userAccount, string toList, string applicationMode, string imagePath, DatabaseService database, bool missingFiipsProject = false, string workflowPhase = "certification")
        {
            return repo.ComposeMessage(project, userAccount, toList, applicationMode, imagePath, database, missingFiipsProject, workflowPhase);
        }

        public static bool ComposeMessage(string subject, string message, string[] to, [Optional] string[] attachments, [Optional] string[] cc, [Optional] string[] bcc, [Optional] MailPriority mailPriority)
        {
            return repo.ComposeMessage(subject, message, to, attachments, cc, bcc, mailPriority);
        }

        public static bool IsNullOrEmpty(Array array)
        {
            return repo.IsNullOrEmpty(array);
        }
    }
}
