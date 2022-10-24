using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Principal;
using System.Runtime.InteropServices;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class FileUtility
    {
        // For impersonation
        private const int LOGON32_PROVIDER_DEFAULT = 0;
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private WindowsImpersonationContext impersonationContext = null;
        private IntPtr userHandle = IntPtr.Zero;
        private IDatabaseService dataServ;
        private ImpersonationUser impersonationUser;

        // global variables and method have to do with elevation of priviledge
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
                                                int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        // closes open handes returned by LogonUser
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public FileUtility(DatabaseService database)
        {
            dataServ = database;
            impersonationUser = dataServ.GetImpersonationUser();
        }

        public void CreateDir(string path, bool impersonate)
        {
            try
            {
                if (impersonate)
                {
                    bool loggedOn = LogonUser(impersonationUser.ImpersonationUserId,
                                                impersonationUser.ImpersonationDomain,
                                                impersonationUser.ImpersonationPassword,
                                                LOGON32_LOGON_INTERACTIVE,
                                                LOGON32_PROVIDER_DEFAULT,
                                                ref userHandle);

                    if (!loggedOn)
                    {
                        System.Windows.Forms.MessageBox.Show("Unable to create directory");
                        return;
                    }

                    impersonationContext = WindowsIdentity.Impersonate(userHandle);
                }

                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(String.Format("Error: {0}", ex.Message));
            }
            finally
            {
                if (impersonate)
                {
                    CleanUpImpersonation();
                }
            }
        }

        private void CleanUpImpersonation()
        {
            // Clean up
            if (impersonationContext != null)
            {
                impersonationContext.Undo();
            }

            if (userHandle != IntPtr.Zero)
            {
                CloseHandle(userHandle);
            }
        }
    }
}
