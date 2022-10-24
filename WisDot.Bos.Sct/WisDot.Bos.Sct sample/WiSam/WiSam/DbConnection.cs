using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace WiSam
{
    public enum Databases
    {
        HsiProduction,
        HsiAcceptance,
        Fiips,
        WiSam,
    }

    public class DbConnection
    {
        private OracleConnection hsiProdOra;
        private OracleConnection hsiAccOra;
        private OracleConnection fiipsProdOra;
        private SqlConnection wiSamProdSql;

        public DbConnection(Databases db)
        {
            switch (db)
            {
                case Databases.HsiProduction:
                    hsiProdOra = new OracleConnection(ConfigurationManager.ConnectionStrings["HsiProdOra"].ConnectionString);
                    break;
                case Databases.HsiAcceptance:
                    hsiAccOra = new OracleConnection(ConfigurationManager.ConnectionStrings["HsiAccOra"].ConnectionString);
                    break;
                case Databases.Fiips:
                    fiipsProdOra = new OracleConnection(ConfigurationManager.ConnectionStrings["FiipsProdOra"].ConnectionString);
                    break;
                case Databases.WiSam:
                    wiSamProdSql = new SqlConnection(ConfigurationManager.ConnectionStrings["WiSamProdSql"].ConnectionString);
                    break;
            }
        }

        public SqlConnection OpenConnectionSql()
        {
            if (wiSamProdSql.State == ConnectionState.Closed || wiSamProdSql.State == ConnectionState.Broken)
            {
                wiSamProdSql.Open();
            }

            return wiSamProdSql;
        }

        public OracleConnection OpenConnectionOracle(Databases db)
        {
            switch (db)
            {
                case Databases.HsiProduction:
                    if (hsiProdOra.State == ConnectionState.Closed || hsiProdOra.State == ConnectionState.Broken)
                    {
                        hsiProdOra.Open();
                    }

                    return hsiProdOra;
                case Databases.HsiAcceptance:
                    if (hsiAccOra.State == ConnectionState.Closed || hsiAccOra.State == ConnectionState.Broken)
                    {
                        hsiAccOra.Open();
                    }

                    return hsiAccOra;
                case Databases.Fiips:
                    if (fiipsProdOra.State == ConnectionState.Closed || fiipsProdOra.State == ConnectionState.Broken)
                    {
                        fiipsProdOra.Open();
                    }

                    return fiipsProdOra;
                default:
                    return new OracleConnection();
            }
        }
    }
}
