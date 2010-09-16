/*
 *	This file is part of SharpMapMapViewer
 *  SharpMapMapViewer is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *  
 *  Also Felix Obermaier
 * 
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoAPI.Geometries;
using Npgsql;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Expressions;
using SharpMap.Utilities;
using SortOrder=System.Data.SqlClient.SortOrder;

namespace MapViewer.DataSource
{
    internal partial class PostGis : UserControl, ICreateDataProvider
    {
        private DataSet datasetTableAndColumns;
        private string lastDbQueried = "";
        private int oidcolumn = -1;

        public PostGis()
        {
            InitializeComponent();

            cbDataBases.MouseDown += cbDataBases_MouseDown;
            chkUsername.Checked = false;
        }

        protected string ServerConnectionString
        {
            get
            {
                return string.Format(
                    "{0};User Id={1};Password={2};",
                    tbServer.Text,
                    tbUName.Text,
                    tbPassword.Text);
            }
        }

        #region ICreateDataProvider Members

        public IEnumerable<IFeatureProvider> GetProviders()
        {
            if (EnsureTables())
            {
                string conn = ServerConnectionString;

                conn += string.Format("Database={0};", cbDataBases.SelectedItem);
                conn += "Enlist=true;";

                DataRowView drv = (DataRowView) cbTables.SelectedItem;
                string schema = (string) drv["Schema"];
                string tableName = (string) drv["TableName"];
                ;
                string geomColumn = (string) drv["GeometryColumn"];
                int coordDimension = (int) drv["Dimension"];
                string srid = (string) drv["SRID"]; //((int)).ToString();

                GeometryServices gs = new GeometryServices();
                IGeometryFactory gf = gs[srid]; //, coordDimension];
                //if (!string.IsNullOrEmpty(spatialReference))
                //    gf.SpatialReference = gs.CoordinateSystemFactory.CreateFromWkt(spatialReference);

                string oidColumnName = (String) dgvColumns.Rows[oidcolumn].Cells["ColumnName"].Value;

                List<String> columns = new List<string>();
                //List<OrderByExpression> orderby = new List<OrderByExpression>();
                foreach (DataGridViewRow dgvr in dgvColumns.Rows)
                {
                    if ((bool) dgvr.Cells["Include"].Value) columns.Add((String) dgvr.Cells["ColumnName"].Value);
                    //if (dgvr.Cells["SortOrder"].Value != null)
                    //    orderby.Add(new OrderByExpression((String) dgvr.Cells[1].Value,
                    //                                      (SortOrder) dgvr.Cells["SortOrder"].Value));
                }

                if (!columns.Contains(oidColumnName))
                    columns.Insert(0, oidColumnName);

                columns.Add(geomColumn);

                ProviderPropertiesExpression ppe = null;

                //if (orderby.Count == 0)
                //{
                //    ppe = new ProviderPropertiesExpression(
                //        new ProviderPropertyExpression[]
                //            {
                //                new AttributesCollectionExpression(columns)
                //            });
                //}
                //else
                //{
                    ppe = new ProviderPropertiesExpression(
                        new ProviderPropertyExpression[]
                            {
                                //new OrderByCollectionExpression(orderby),
                                new AttributesCollectionExpression(columns)
                            });
                //}

                IFeatureProvider prov = null;

                switch ((String) dgvColumns.Rows[oidcolumn].Cells["DataType"].Value)
                {
                    case "bigint":
                        prov = new PostGisProvider<long>(gf, conn, schema, tableName, oidColumnName, geomColumn,
                                                         gs.CoordinateTransformationFactory)
                                   {DefaultProviderProperties = ppe};
                        break;
                    case "integer":
                        prov = new PostGisProvider<int>(gf, conn, schema, tableName, oidColumnName, geomColumn,
                                                        gs.CoordinateTransformationFactory)
                                   {DefaultProviderProperties = ppe};
                        break;
                    case "character varying":
                    case "text":
                        prov = new PostGisProvider<string>(gf, conn, schema, tableName, oidColumnName, geomColumn,
                                                           gs.CoordinateTransformationFactory)
                                   {DefaultProviderProperties = ppe};
                        break;
                    case "uuid":
                        prov = new PostGisProvider<Guid>(gf, conn, schema, tableName, oidColumnName, geomColumn,
                                                         gs.CoordinateTransformationFactory)
                                   {DefaultProviderProperties = ppe};
                        break;
                    default:
                        yield break;
                }

                //jd commented temporarily to get a build
                //((ISpatialDbProvider)prov).DefinitionQuery =
                //    new ProviderQueryExpression(ppe, ape, null);

                yield return prov;
            }
        }

        public IEnumerable<string> ProviderNames
        {
            get { yield return (string) ((DataRowView) cbTables.SelectedItem)["Label"]; }
        }

        #endregion

        private void cbDataBases_MouseDown(object sender, MouseEventArgs e)
        {
            EnsureDataBases();
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
                if (!tbServer.Text.ToLower().Contains("port="))
                {
                    message.Append("Please specifiy port on which the server listens (usually: port=5432;)!");
                    ok = false;
                }
            }

            if (string.IsNullOrEmpty(tbUName.Text))
            {
                message.AppendLine("Please submit your username on Postgres Database server.");
                ok = false;
            }
            if (!ok)
            {
                MessageBox.Show(message.ToString(), "Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ok;
            }

            string serverConnectionString = ServerConnectionString + ";Database=postgres";
            using (NpgsqlConnection conn = new NpgsqlConnection(serverConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (NpgsqlException ex)
                {
                    ok = false;
                    message.AppendFormat("Error connection to server {0}", ex.Message);
                }
            }
            if (!ok)
                MessageBox.Show(message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return ok;
        }

        private bool GetDataBases()
        {
            string select = "select datname as name from pg_database where not(datistemplate) AND datallowconn;";

            string serverConnectionString = ServerConnectionString + "Database=postgres;";
            using (NpgsqlConnection cn = new NpgsqlConnection(serverConnectionString))
            {
                NpgsqlCommand cm = cn.CreateCommand();
                cm.CommandText = select;
                cm.CommandType = CommandType.Text;

                try
                {
                    List<string> lst = new List<string>();
                    cn.Open();
                    using (NpgsqlDataReader dr = cm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string tmpConnectionString = ServerConnectionString +
                                                         string.Format("Database={0};", dr.GetString(0));
                            try
                            {
                                using (NpgsqlConnection tmpCn = new NpgsqlConnection(tmpConnectionString))
                                {
                                    tmpCn.Open();
                                    NpgsqlCommand tmpCm = tmpCn.CreateCommand();
                                    tmpCm.CommandText = "postgis_version";
                                    tmpCm.CommandType = CommandType.StoredProcedure;
                                    object res = tmpCm.ExecuteScalar();

                                    lst.Add(dr.GetString(0));
                                }
                            }
                            catch (NpgsqlException ex)
                            {
                                Trace.Write(string.Format("Database '{0}' is not a postgis database!", dr.GetString(0)));
                            }
                            finally
                            {
                            }
                        }
                    }
                    cbDataBases.DataSource = lst;
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private void chkUsername_CheckedChanged(object sender, EventArgs e)
        {
            tbUName.Enabled = chkUsername.Checked;
            if (!chkUsername.Checked)
                tbUName.Text = Environment.UserName;
        }

        private void cbTables_DataSourceChanged(object sender, EventArgs e)
        {
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
            using (NpgsqlConnection cn = new NpgsqlConnection(serverConnectionString))
            {
                string select =
                    @"
CREATE OR REPLACE FUNCTION gis_table_and_columns()
  RETURNS SETOF refcursor AS
$BODY$
DECLARE 
  tbl refcursor;
  col refcursor;

BEGIN

OPEN tbl FOR
	SELECT  x.f_table_schema AS ""Schema"", 
		    x.f_table_name AS ""TableName"",
		    x.f_geometry_column AS ""GeometryColumn"",
		    x.coord_dimension as ""Dimension"",
	        x.f_table_name || $lit$.[$lit$ || x.f_geometry_column || $lit$] ($lit$ || x.type || $lit$)$lit$ AS ""Label"",
		    y.auth_name || $lit$:$lit$ || y.auth_srid AS ""SRID"",
		    y.srtext as ""SpatialReference""
    FROM    geometry_columns AS x LEFT JOIN spatial_ref_sys as y on x.srid=y.srid;
RETURN NEXT tbl;

OPEN col FOR
SELECT	x.table_schema as ""TableSchema"",
	    x.table_name as ""TableName"",
	    x.column_name as ""ColumnName"",
	    x.data_type as ""DataType"",
	    true AS ""Include"",
	    CASE
		    WHEN u.column_name = x.column_name then true
		    ELSE false
	    END AS ""PK"",
	    x.ordinal_position
FROM	information_schema.columns AS x 
LEFT JOIN (SELECT DISTINCT z.f_table_schema, z.f_table_name FROM geometry_columns AS z) AS cte ON (cte.f_table_schema = x.table_schema and cte.f_table_name=x.table_name)
LEFT JOIN information_schema.key_column_usage AS u ON u.table_name=x.table_name
WHERE   cte.f_table_name=x.table_name AND (RTRIM(x.data_type) <> 'USER-DEFINED')
UNION
    SELECT 	gc.f_table_schema as ""TableSchema"",
	        gc.f_table_name as ""TableName"",
	        CAST('oid' as character varying) as ""ColumnName"",
	        cast('bigint' as character varying) as ""DataType"",
	        true as ""Include"",
	        true as ""PK"",
	        cast(0 as integer) as ordinal_position
    FROM    pg_class as cls
    INNER JOIN pg_namespace as ns on ns.oid = cls.relnamespace
    INNER JOIN geometry_columns as gc on ns.nspname=gc.f_table_schema and cls.relname=gc.f_table_name
    WHERE cls.relkind='r' and cls.relhasoids
ORDER BY ""TableName"", ordinal_position;
RETURN NEXT col;
RETURN;
END;
$BODY$
  LANGUAGE 'plpgsql' VOLATILE
  COST 100
  ROWS 1000;
";

                //do function
                cn.Open();
                new NpgsqlCommand(select, cn).ExecuteNonQuery();

                NpgsqlCommand cm = cn.CreateCommand();
                cm.CommandText = "gis_table_and_columns";
                cm.CommandType = CommandType.StoredProcedure;
                cm.Transaction = cn.BeginTransaction();
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cm);
                datasetTableAndColumns = new DataSet();
                da.Fill(datasetTableAndColumns);
                cm.Transaction.Commit();

                new NpgsqlCommand("DROP FUNCTION gis_table_and_columns();", cn).ExecuteNonQuery();

                cbTables.Enabled = true;
                cbTables.DataSource = datasetTableAndColumns.Tables[0];
                cbTables.DisplayMember = "Label";
                cbTables.ValueMember = "TableName";
            }
            return true;
        }

        private void cbTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTables.SelectedIndex < 0)
            {
                dgvColumns.DataSource = null;
                return;
            }

            DataView dv = new DataView(
                datasetTableAndColumns.Tables[1],
                string.Format("TableName='{0}'", cbTables.SelectedValue.GetType() == typeof (String)
                                                     ?
                                                         cbTables.SelectedValue
                                                     :
                                                         datasetTableAndColumns.Tables[0].Rows[0][0]),
                "",
                DataViewRowState.CurrentRows);
            dv.AllowDelete = false;
            dv.AllowNew = false;

            dgvColumns.DataSource = dv;
            if (dgvColumns.Columns.Count < 8)
            {
                DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
                dgvc.Name = "SortOrder";

                dgvc.DataSource = Enum.GetNames(typeof (SortOrder));
                    //new List<String>(GeoAPI.DataStructures.Enumerable.ToArray<String> Enum.GetNames(SortOrder));
                dgvColumns.Columns.Add(dgvc);

                dgvColumns.Columns["TableSchema"].Visible = false; // TableName
                dgvColumns.Columns["TableName"].Visible = false; // TableName
                dgvColumns.Columns["ColumnName"].ReadOnly = true; // ColumnName
                dgvColumns.Columns["DataType"].ReadOnly = true; // DataType
                dgvColumns.Columns["Include"].ReadOnly = false; // Include
                dgvColumns.Columns["PK"].Visible = false; // PrimaryKey
                dgvColumns.Columns["ordinal_position"].Visible = false;
                dgvColumns.Columns["SortOrder"].ReadOnly = false; // SortOrder
            }

            oidcolumn = -2;
            setOidColumn(-1);
            foreach (DataGridViewRow dgvr in dgvColumns.Rows)
                if ((bool) dgvr.Cells["PK"].Value)
                {
                    setOidColumn(dgvr.Index);
                    break;
                }

            dgvColumns.Enabled = true;

            lbSRID.Text = string.Format("SRID: {0}", ((DataRowView) cbTables.SelectedItem)["SRID"]);
        }

        private void cbDataBases_DataSourceChanged(object sender, EventArgs e)
        {
            if (cbDataBases.DataSource == null)
            {
                cbTables.DataSource = null;
                dgvColumns.DataSource = null;
                datasetTableAndColumns.Dispose();
                datasetTableAndColumns = null;
                return;
            }
        }

        private void cbDataBases_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetTables();
        }

        private void dgvColumns_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvColumns.Columns[e.ColumnIndex].Name != "ColumnName") return;

            setOidColumn(e.RowIndex);
        }

        private void setOidColumn(int rowindex)
        {
            if (oidcolumn == rowindex) return;
            Font fnt = dgvColumns.DefaultCellStyle.Font;

            if (oidcolumn >= 0)
            {
                dgvColumns.Rows[oidcolumn].Cells["ColumnName"].Style.Font =
                    new Font(fnt.FontFamily.Name, fnt.SizeInPoints, FontStyle.Regular);

                dgvColumns.Rows[oidcolumn].Cells["PK"].Value = false;
            }

            if (rowindex >= 0)
            {
                dgvColumns.Rows[rowindex].Cells["ColumnName"].Style.Font =
                    new Font(fnt.FontFamily.Name, fnt.SizeInPoints, FontStyle.Bold);
                ;
                dgvColumns.Rows[rowindex].Cells["PK"].Value = true;
            }
            oidcolumn = rowindex;
        }
    }
}