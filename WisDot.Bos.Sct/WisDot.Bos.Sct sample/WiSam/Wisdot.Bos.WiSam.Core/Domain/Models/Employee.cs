using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wisdot.Bos.WiSam.Core.Domain.Models
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public float HourlyRate { get; set; }
        public List<EmployeeTimesheet> Timesheets { get; set; }

        public Employee()
        {
            Timesheets = new List<EmployeeTimesheet>();
        }
    }
}
