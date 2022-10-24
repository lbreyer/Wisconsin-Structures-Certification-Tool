using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class ComboboxValue
    {
        public string Code { get; set; }
        public string Value { get; set; }

        public ComboboxValue(string code, string value)
        {
            Code = code;
            Value = value;
        }
    }
}
