using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class Budget
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public float Amount { get; set; }

        //public Budget() { }

        public Budget(int startYear, int endYear, float amount)
        {
            StartYear = startYear;
            EndYear = endYear;
            Amount = amount;
        }
    }
}
