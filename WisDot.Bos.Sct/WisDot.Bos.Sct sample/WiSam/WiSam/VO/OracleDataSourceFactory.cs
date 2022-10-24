using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Oracle.DataAccess.Client;
using System.Configuration;
using System.Data;

namespace WiSam
{
    public class OracleDataSourceFactory : DataSourceFactory
    {
        public override DataSource OpenConnection(Datasources ds)
        {
            OracleDataSource ods = new OracleDataSource();
            switch (ds)
            {
                case Datasources.Fiips:
                    ods.DatasourceName = ConfigurationManager.ConnectionStrings["FiipsProdOra"].Name;
                    ods.DatasourceConStr = ConfigurationManager.ConnectionStrings["FiipsProdOra"].ConnectionString;
                    ods.Connection = new OracleConnection(ods.DatasourceConStr);
                    break;
                case Datasources.HsiAcceptance:
                    ods.DatasourceName = ConfigurationManager.ConnectionStrings["HsiAccOra"].Name;
                    ods.DatasourceConStr = ConfigurationManager.ConnectionStrings["HsiAccOra"].ConnectionString;
                    ods.Connection = new OracleConnection(ods.DatasourceConStr);
                    break;
                case Datasources.HsiProduction:
                    ods.DatasourceName = ConfigurationManager.ConnectionStrings["HsiProdOra"].Name;
                    ods.DatasourceConStr = ConfigurationManager.ConnectionStrings["HsiProdOra"].ConnectionString;
                    ods.Connection = new OracleConnection(ods.DatasourceConStr);
                    break;
            }
            if (ods.Connection.State == ConnectionState.Closed || ods.Connection.State == ConnectionState.Broken)
                ods.Connection.Open();
            return ods;
        }
    }
}
