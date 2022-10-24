using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class PriorityIndexFactor
    {
        public int PriorityIndexFactorKey { get; set; }
        public int PriorityIndexCategoryKey { get; set; }
        public string PriorityIndexCategoryName { get; set; }
        public string PriorityIndexFactorId { get; set; }
        public string PriorityIndexFactorDesc { get; set; }
        public float PriorityIndexFactorWeight { get; set; }
        public string PriorityIndexFactorType { get; set; }
        public string IndexValueFormula { get; set; }
        public int Year { get; set; }
        public bool IsCurrentRow { get; set; }
        public DateTime RowEffectiveDate { get; set; }
        public DateTime RowExpirationDate { get; set; }
        public DateTime RowLastUpdateDate { get; set; }
        public string Notes { get; set; }
        public string FieldValue { get; set; }
        public float IndexValue { get; set; }
        public float Score { get; set; }
    }
}
