using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace WiSam
{
    public class SqlDataSourceFactory : DataSourceFactory
    {
        public override DataSource OpenConnection(Datasources ds)
        {
            SqlDataSource sds = new SqlDataSource();
            switch (ds)
            {
                case Datasources.WiSam:
                    sds.DatasourceName = ConfigurationManager.ConnectionStrings["WiSamProdSql"].Name;
                    sds.DatasourceConStr = ConfigurationManager.ConnectionStrings["WiSamProdSql"].ConnectionString;
                    sds.Connection = new SqlConnection(sds.DatasourceConStr);
                    break;
                default:
                    throw new ArgumentException("Invalid datasource.", "ds");
            }
            if (sds.Connection.State == ConnectionState.Closed || sds.Connection.State == ConnectionState.Broken)
                sds.Connection.Open();
            return sds;
        }
    }
}
