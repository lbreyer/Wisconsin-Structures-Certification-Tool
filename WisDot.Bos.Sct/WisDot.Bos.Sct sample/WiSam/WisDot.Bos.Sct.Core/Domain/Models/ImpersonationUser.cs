using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class ImpersonationUser
    {
        //private static string impersonationDomain = ConfigurationManager.AppSettings["ImpersonationDomain"];
        //private static string impersonationUserId = ConfigurationManager.AppSettings["ImpersonationUser"];
        //private static string encryptedImpersonationPassword = ConfigurationManager.AppSettings["ImpersonationPassword"];
        //private static string decryptedImpersonationPassword = CryptorEngine.Encrypt(encryptedImpersonationPassword, true);
        public string ImpersonationDomain { get; set; }
        public string ImpersonationUserId { get; set; }
        public string ImpersonationPassword { get; set; }

        public ImpersonationUser()
        {
            Initialize();
        }

        public void Initialize()
        {
            ImpersonationDomain = "";
            ImpersonationUserId = "";
            ImpersonationPassword = "";
        }
    }
}
