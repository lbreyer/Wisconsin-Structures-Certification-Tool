using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisDot.Bos.Sct.Core.Domain.Models
{
    public class ProjectHistoryRecord
    {
        public int ProjectDbId { get; set; }
        public int ProjectHistoryDbId { get; set; }
        public int ProjectFiscalYear { get; set; }
        public string UserAction { get; set; }
        public DateTime UserActionDateTime { get; set; }
        public string UserFullName { get; set; }
        public string Status { get; set; }
        public string RecertificationReason { get; set; }
        public string RecertificationComments { get; set; }

        public ProjectHistoryRecord()
        {
            Initialize();
        }

        private void Initialize()
        {
            this.UserAction = "";
            this.UserFullName = "";
            this.Status = "";
            this.RecertificationReason = "";
            this.RecertificationComments = "";
        }
    }
}
