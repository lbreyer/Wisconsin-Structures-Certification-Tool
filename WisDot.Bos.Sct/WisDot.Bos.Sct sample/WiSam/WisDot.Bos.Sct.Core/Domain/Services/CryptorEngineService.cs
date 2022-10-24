using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class CryptorEngineService
    {
        private static CryptorEngineRepository repo = new CryptorEngineRepository();

        public static string Encrypt(string toEncrypt, bool useHashing)
        {
            return repo.Encrypt(toEncrypt, useHashing);
        }

        public static string Decrypt(string cipherString, bool useHashing)
        {
            return repo.Decrypt(cipherString, useHashing);
        }
    }
}
