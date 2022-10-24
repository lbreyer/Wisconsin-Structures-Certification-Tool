using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class UserActivity
    {
        public int UserLogDbId { get; set; }
        public int UserDbId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Activity { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public string DotLogin { get; set; }
        public string Office { get; set; }
        public string EmailAddress { get; set; }
    }
}
