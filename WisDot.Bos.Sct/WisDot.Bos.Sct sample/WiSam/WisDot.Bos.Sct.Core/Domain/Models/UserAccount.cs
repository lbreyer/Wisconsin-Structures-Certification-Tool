using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class UserAccount
    {
        public int UserDbId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DotLogin { get; set; }
        public string Office { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> RoleTypes { get; set; }
        public bool IsRegionalProgrammer { get; set; }
        public bool IsRegionalMaintenanceEngineer { get; set; }
        public bool IsRegionalRead { get; set; }
        public bool IsSuperRead { get; set; }
        public bool IsSuperUser { get; set; }
        public bool IsAdministrator { get; set; }
        public bool IsOmniscient { get; set; }
        public bool IsCertificationLiaison { get; set; }
        public bool IsCertificationSupervisor { get; set; }
        public bool IsPrecertificationLiaison { get; set; }
        public bool IsPrecertificationSupervisor { get; set; }
    }
}
