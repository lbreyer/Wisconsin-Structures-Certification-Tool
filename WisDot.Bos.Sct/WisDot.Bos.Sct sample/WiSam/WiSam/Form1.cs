using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WiSam
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open FIIPS DB connection
           // DbConnection fiipsConn = new DbConnection(Databases.Fiips);
            //fiipsConn.OpenConnectionOracle(Databases.Fiips);

            // Open WiSAM DB connection
            //DbConnection samConn = new DbConnection(Databases.WiSam);
            //samConn.OpenConnectionSql();
            
            DataSourceFactory[] dsfs = new DataSourceFactory[2];
            dsfs[0] = new OracleDataSourceFactory();
            DataSource hsiAcc = dsfs[0].OpenConnection(Datasources.HsiAcceptance);
            DataSource fiips = dsfs[0].OpenConnection(Datasources.Fiips);
            OracleDataSource oraHsi = hsiAcc as OracleDataSource;
            SqlDataSource sqlFiips = fiips as SqlDataSource;

            oraHsi = hsiAcc as OracleDataSource;
            sqlFiips = fiips as SqlDataSource;
            
            
            //DataSource hsiProd = dsfs[0].OpenConnection(Datasources.HsiProduction);
           
            
            
        }
    }
}
