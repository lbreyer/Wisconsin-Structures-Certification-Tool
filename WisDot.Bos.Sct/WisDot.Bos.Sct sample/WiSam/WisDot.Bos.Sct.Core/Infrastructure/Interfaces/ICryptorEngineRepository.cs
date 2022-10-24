using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface ICryptorEngineRepository
    {
        string Encrypt(string toEncrypt, bool useHashing);
        string Decrypt(string cipherString, bool useHashing);
    }
}
