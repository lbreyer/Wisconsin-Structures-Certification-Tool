using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IEmailRepository
    {
        string EmailMessage(Project project, UserAccount userAccount, string applicationMode, string imagePath, DatabaseService database, bool missingFiipsProject = false, string workflowPhase = "certification");
        string ComposeMessage(Project project, UserAccount userAccount, string toList, string applicationMode, string imagePath, DatabaseService database, bool missingFiipsProject = false, string workflowPhase = "certification");
        bool ComposeMessage(string subject, string message, string[] to,
            [Optional] string[] attachments, [Optional] string[] cc, [Optional] string[] bcc,
            [Optional] MailPriority mailPriority);
        bool IsNullOrEmpty(Array array);
    }
}
