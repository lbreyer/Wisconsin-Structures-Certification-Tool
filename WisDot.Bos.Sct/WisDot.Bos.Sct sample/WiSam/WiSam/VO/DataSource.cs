using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiSam
{
    public abstract class DataSource
    {
        public string DatasourceName { get; set; }
        public string DatasourceConStr { get; set; }
    }
}
