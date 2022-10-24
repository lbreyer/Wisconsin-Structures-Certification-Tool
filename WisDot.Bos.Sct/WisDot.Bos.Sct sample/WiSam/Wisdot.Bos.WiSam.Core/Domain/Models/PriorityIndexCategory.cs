using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class PriorityIndexCategory
    {
        public int PriorityIndexCategoryKey { get; set; }
        public string PriorityIndexCategoryName { get; set; }
        public string PriorityIndexCategoryDesc { get; set; }
        public float PriorityIndexMaxValue { get; set; }
        public int Year { get; set; }
        public bool IsCurrentRow { get; set; }
        public DateTime RowEffectiveDate { get; set; }
        public DateTime RowExpirationDate { get; set; }
        public DateTime RowLastUpdateDate { get; set; }
        public string Notes { get; set; }
        public float Score { get; set; }
    }
}
