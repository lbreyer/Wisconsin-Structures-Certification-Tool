using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiSam
{
    public enum Datasources
    {
        HsiProduction,
        HsiAcceptance,
        Fiips,
        WiSam,
    }

    public abstract class DataSourceFactory
    {
        /*
        public DataSourceFactory()
        {
            this.OpenConnection(Datasources.HsiProduction);
        }*/

        public abstract DataSource OpenConnection(Datasources ds);
    }
}
