using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    class CaiRow
    {
        public string StructureId { get; set; }
        public int WorkActionYear { get; set; }
        public int Age { get; set; }
        public double CaiValue { get; set; }
        public string NbiDeck { get; set; }
        public string NbiSup { get; set; }
        public string NbiSub { get; set; }
        public string NbiCulv { get; set; }
        public string Quantities { get; set; }
    }
}
