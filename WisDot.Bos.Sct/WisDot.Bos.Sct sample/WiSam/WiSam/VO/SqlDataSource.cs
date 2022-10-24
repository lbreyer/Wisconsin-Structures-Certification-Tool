using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;

namespace WiSam
{
    public class SqlDataSource : DataSource
    {
        public SqlConnection Connection { get; set; }
    }
}
