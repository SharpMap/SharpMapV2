using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Text;
using System.Windows.Forms;
using GeoAPI.Geometries;
using IBM.Data.DB2;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Utilities;

namespace MapViewer.DataSource
{
    public partial class DB2SpatialExtender : UserControl, ICreateDataProvider
    {
        private DataSet _datasetTables;
        private string lastDbQueried;

        public DB2SpatialExtender()
        {
            InitializeComponent();
            cbDataBases.MouseDown += cbDataBases_MouseDown;
        }

        protected string ServerConnectionString
        {
            get
            {
                return string.Format(
                    "{0};UId={1};Password={2};",
                    tbServer.Text,
                    tbUName.Text,
                    tbPassword.Text);
            }
        }

        private bool EnsureTables()
        {
            if (cbTables.Items.Count == 0)
            {
                if (!EnsureDataBases())
                    return false;

                return GetTables();
            }
            return true;
        }

        private bool EnsureDataBases()
        {
            if (cbDataBases.Items.Count == 0)
            {
                if (!EnsureConnection())
                    return false;
                return GetDataBases();
            }
            return true;
        }

        private bool EnsureConnection()
        {
            bool ok = true;
            StringBuilder message = new StringBuilder();
            if (string.IsNullOrEmpty(tbServer.Text))
            {
                message.AppendLine("Please specifiy a Server and Port to connect to");
                ok = false;
            }
            else
            {
                if (!tbServer.Text.ToLower().Contains("server="))
                {
                    message.Append("Please specifiy name or ip of the server!");
                    ok = false;
                }
            }

            if (string.IsNullOrEmpty(tbUName.Text))
            {
                message.AppendLine("Please submit your username on DB2 Database server.");
                ok = false;
            }
            if (!ok)
            {
                MessageBox.Show(message.ToString(), "Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ok;
            }

            if (!ok)
                MessageBox.Show(message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return ok;
        }

        private string findServer(string connectionString)
        {
            if (connectionString.Length == 0)
                return "";

            string[] parts = connectionString.Split(';');

            foreach (string elem in parts)
                if (elem.ToLower().StartsWith("server="))
                    return elem;
            return "";
        }

        private string parseHost(string connectionString)
        {
            string serverElem = findServer(connectionString);
            if (serverElem.Length == 0)
                return "";
            serverElem = serverElem.Substring(serverElem.IndexOf('=') + 1);
            string[] parts = serverElem.Split(':');

            return parts[0];
        }

        private string parsePort(string connectionString)
        {
            string serverElem = findServer(connectionString);
            if (serverElem.Length == 0)
                return "";
            serverElem = serverElem.Substring(serverElem.IndexOf('=') + 1);
            string[] parts = serverElem.Split(':');

            return parts[1];
        }

        private bool GetDataBases()
        {
            DB2DataSourceEnumerator db2dbenum = new DB2DataSourceEnumerator();

            //DataTable dtDataBases = db2dbenum.GetDataSources();
            DataTable dtDataBases = db2dbenum.GetDataSources(false);
            //parseHost(ServerConnectionString),
            //parsePort(ServerConnectionString),
            //"", //tbUName.Text,
            //""); //tbPassword.Text);
            string hostName = parseHost(ServerConnectionString);

            //local database server?
            bool localDbServer = string.Compare(hostName, Environment.MachineName, true) == 0;
            if (!localDbServer)
                localDbServer = string.Compare(hostName, "localhost", true) == 0;
            if (!localDbServer)
            {
                localDbServer = string.Compare(hostName,
                                               Dns.GetHostAddresses(Dns.GetHostName())[0].ToString()) == 0;
            }

            List<string> lst = new List<string>();
            foreach (DataRow dr in dtDataBases.Rows)
            {
                if ((localDbServer && dr["ServerName"] == DBNull.Value) ||
                    ((string) dr["ServerName"] == hostName))
                {
                    string cn = ServerConnectionString + "Database=" + dr["DatabaseAlias"];
                    if (DB2SpatialExtenderProviderStatic.IsSpatiallyEnabled(cn))
                    {
                        lst.Add((string) dr["DatabaseAlias"]);
                    }
                }
            }
            cbDataBases.DataSource = lst;
            return true;
        }

        private bool GetTables()
        {
            if (!EnsureDataBases())
            {
                lastDbQueried = "";
                return false;
            }

            if (cbDataBases.Text == lastDbQueried) return true;
            lastDbQueried = cbDataBases.Text;

            string serverConnectionString = ServerConnectionString + string.Format("Database={0}", cbDataBases.Text);
            using (DB2Connection cn = new DB2Connection(serverConnectionString))
            {
                cn.Open();
                DB2Command cmd = new DB2Command(string.Format(
                                                    @"
SELECT  x.table_schema AS ""Schema"", 
		x.table_name AS ""TableName"",
        z0.COLNAME AS ""OID"",
        z1.TYPENAME AS ""OIDType"",
		x.column_name AS ""GeometryColumn"",
		2 as ""Dimension"",
	    x.table_name || '.[' || x.column_name || '] (' || x.type_name || CAST(')' AS VARCHAR(10))  AS ""Label"",
		y.organization || ':' || cast( y.organization_coordsys_id AS CHAR(10)) AS ""SRID"",
		y.definition as ""SpatialReference""
FROM    {0}.st_geometry_columns AS x 
            LEFT JOIN {0}.st_spatial_reference_systems as y on x.srs_id=y.srs_id
            LEFT JOIN SYSCAT.KEYCOLUSE AS z0 ON (z0.TABSCHEMA =x.table_schema AND z0.TABNAME = x.table_name)
            LEFT JOIN SYSCAT.COLUMNS   AS z1 ON (z1.TABSCHEMA =x.table_schema AND z1.TABNAME = x.table_name AND z0.colname=z1.colname);",
                                                    DB2SpatialExtenderProviderStatic.DefaultSpatialSchema), cn);

                DB2DataAdapter da = new DB2DataAdapter(cmd);
                _datasetTables = new DataSet();
                da.Fill(_datasetTables, "Tables");
            }

            cbTables.Enabled = true;
            cbTables.DataSource = _datasetTables.Tables[0];
            cbTables.DisplayMember = "Label";
            cbTables.ValueMember = "TableName";

            return true;
        }

        private void cbDataBases_MouseDown(object sender, MouseEventArgs e)
        {
            EnsureDataBases();
        }

        private void cbDataBases_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetTables();
        }

        private void chkUsername_CheckedChanged(object sender, EventArgs e)
        {
            tbUName.Enabled = chkUsername.Checked;
            if (!chkUsername.Checked)
                tbUName.Text = Environment.UserName;
        }

        private void cbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbSRID.Text = string.Format("SRID: {0}", ((DataRowView) cbTables.SelectedItem)["SRID"]);
        }

        #region ICreateDataProvider Member

        public IEnumerable<IFeatureProvider> GetProviders()
        {
            if (EnsureTables())
            {
                string conn = ServerConnectionString;

                conn += string.Format("Database={0};", cbDataBases.SelectedItem);

                DataRowView drv = (DataRowView) cbTables.SelectedItem;
                string schema = (string) drv["Schema"];
                string tableName = (string) drv["TableName"];
                string oidColumnName = (string) drv["OID"];
                string geomColumn = (string) drv["GeometryColumn"];
                int coordDimension = (int) drv["Dimension"];
                string srid = (string) drv["SRID"]; //((int)).ToString();
                string spatialReference = (string) drv["SpatialReference"];

                IGeometryFactory gf = null;
                GeometryServices gserv = new GeometryServices();
                if (spatialReference == "UNSPECIFIED" || spatialReference == "")
                {
                    gf = gserv[srid];
                }
                else
                {
                    gf = gserv[spatialReference];
                    if (string.IsNullOrEmpty(gf.Srid))
                        gf.Srid = srid;
                }


                IFeatureProvider prov = null;

                switch ((String) drv["OIDType"])
                {
                    case "BIGINT":
                        prov = new DB2SpatialExtenderProvider<long>(gf, conn, schema, tableName, oidColumnName,
                                                                    geomColumn);
                        break;
                    case "INTEGER":
                        prov = new DB2SpatialExtenderProvider<int>(gf, conn, schema, tableName, oidColumnName,
                                                                   geomColumn);
                        break;
                    case "VARCHAR":
                    case "CLOB":
                        prov = new DB2SpatialExtenderProvider<string>(gf, conn, schema, tableName, oidColumnName,
                                                                      geomColumn);
                        break;
                    case "REAL":
                        prov = new DB2SpatialExtenderProvider<Single>(gf, conn, schema, tableName, oidColumnName,
                                                                      geomColumn);
                        break;
                    case "DOUBLE":
                        prov = new DB2SpatialExtenderProvider<Double>(gf, conn, schema, tableName, oidColumnName,
                                                                      geomColumn);
                        break;
                    default:
                        yield break;
                }

                //prov.CoordinateTransformation = gserv.CoordinateTransformationFactory.CreateFromCoordinateSystems(
                //    gf.SpatialReference, 
                //    gserv["EPSG:31467"].SpatialReference);

                //jd commented temporarily to get a build
                //((ISpatialDbProvider)prov).DefinitionQuery =
                //    new ProviderQueryExpression(ppe, ape, null);

                yield return prov;
            }
        }

        public IEnumerable<string> ProviderNames
        {
            get { yield return "IBM DB2 SpatialExtender"; }
        }

        #endregion
    }
}