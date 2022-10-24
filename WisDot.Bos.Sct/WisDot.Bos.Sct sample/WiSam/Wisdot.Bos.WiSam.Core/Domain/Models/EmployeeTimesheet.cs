using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class EmployeeTimesheet
    {
        public int EmployeeId { get; set; }
        public string StructureId { get; set; }
        public string ProjectId { get; set; }
        public int ActivityCode { get; set; }
        public DateTime WeekEndingDate { get; set; }
        public int WorkNumber { get; set; }
        public float TotalHours { get; set; }
        public string ActivityDescription { get; set; }
        public int ActivityCategoryId { get; set; }
        public string ActivityCategoryDescription { get; set; }
        public int MonthWeekEndingDate { get; set; }
        public int YearWeekEndingDate { get; set; }
        public int ExcelRowNumber { get; set; }
    }
}
