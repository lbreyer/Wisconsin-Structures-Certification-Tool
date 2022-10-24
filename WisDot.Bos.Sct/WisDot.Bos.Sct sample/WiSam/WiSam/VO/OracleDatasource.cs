using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Oracle.DataAccess.Client;

namespace WiSam
{
    public class OracleDataSource : DataSource
    {
        public OracleConnection Connection { get; set; }
    }
}
