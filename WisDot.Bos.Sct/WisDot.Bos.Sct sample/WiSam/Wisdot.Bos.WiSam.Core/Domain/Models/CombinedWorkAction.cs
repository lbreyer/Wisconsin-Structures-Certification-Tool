using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class CombinedWorkAction
    {
        public string MainWorkActionCode { get; set; }
        public string SecondaryWorkActionCode { get; set; }
        public string CombinedWorkActionCode { get; set; }
        public bool BypassRule { get; set; }
        public bool Active { get; set; }
    }
}
