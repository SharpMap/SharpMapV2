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
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.SpatiaLite2;
using SharpMap.Expressions;
using SharpMap.Utilities;
using SortOrder=System.Data.SqlClient.SortOrder;
//using System.Data.SQLite;

namespace MapViewer.DataSource
{
    public partial class SpatialLite : UserControl, ICreateDataProvider
    {
        private DataSet datasetTableAndColumns;
        private int oidcolumn = -1;

        public SpatialLite()
        {
            InitializeComponent();
            cbTables.MouseDown += cbTables_MouseDown;
        }

        private string ServerConnectionString
        {
            get
            {
                string scs = string.Format("Data Source={0};", tbPath.Text);
                if (!string.IsNullOrEmpty(tbPassword.Text))
                    scs += string.Format("Password={0};", tbPassword.Text);
                return scs;
            }
        }

        #region ICreateDataProvider Members

        public IFeatureProvider GetProvider()
        {
            if (EnsureTables())
            {
                string conn = ServerConnectionString;

                DataRowView drv = (DataRowView) cbTables.SelectedItem;
                string schema = (string) drv["Schema"];
                string tableName = (string) drv["TableName"];
                ;
                string geomColumn = (string) drv["GeometryColumn"];
                int coordDimension = (int) (long) drv["Dimension"];
                string srid = drv["SRID"].ToString();
                string spatialReference = drv["SpatialReference"] == DBNull.Value
                                              ?
                                                  ""
                                              :
                                                  (string) drv["SpatialReference"];

                GeometryServices gs = new GeometryServices();
                IGeometryFactory gf = gs[srid]; //, coordDimension];
                //if (!string.IsNullOrEmpty(spatialReference))
                //    gf.SpatialReference = gs.CoordinateSystemFactory.CreateFromWkt(spatialReference);

                string oidColumnName = (String) dgvColumns.Rows[oidcolumn].Cells[2].Value;

                List<String> columns = new List<string>();
                //List<OrderByExpression> orderby = new List<OrderByExpression>();
                foreach (DataGridViewRow dgvr in dgvColumns.Rows)
                {
                    if ((bool) dgvr.Cells["Include"].Value) columns.Add((String) dgvr.Cells[2].Value);
                    //if (dgvr.Cells["SortOrder"].Value != null)
                    //    orderby.Add(new OrderByExpression((String) dgvr.Cells[1].Value,
                    //                                      (SortOrder) dgvr.Cells["SortOrder"].Value));
                }

                if (!columns.Contains(oidColumnName))
                    columns.Insert(0, oidColumnName);
                columns.Insert(1, geomColumn);

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

                IFeatureProvider prov =
                    new SpatiaLite2Provider(gf, conn, schema, tableName, oidColumnName, geomColumn,
                                            gs.CoordinateTransformationFactory) {DefaultProviderProperties = ppe};

                //jd commented temporarily to get a build
                //((ISpatialDbProvider)prov).DefinitionQuery =
                //    new ProviderQueryExpression(ppe, ape, null);

                return prov;
            }
            return null;
        }

        public string ProviderName
        {
            get { return (string) ((DataRowView) cbTables.SelectedItem)["Label"]; }
        }

        #endregion

        private bool EnsureConnection()
        {
            bool ok = true;
            StringBuilder message = new StringBuilder();
            if (string.IsNullOrEmpty(tbPath.Text))
            {
                message.AppendLine("Please specifiy path to SpatiaLite database file");
                ok = false;
            }

            if (!ok)
            {
                MessageBox.Show(message.ToString(), "Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ok;
            }

            try
            {
                ok = SpatiaLite2Provider.IsSpatiallyEnabled(ServerConnectionString);
            }
            catch (SpatiaLite2Exception ex)
            {
                ok = false;
                message.AppendFormat("Error connection to server {0}", ex.Message);
            }

            if (!ok)
                MessageBox.Show(message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return ok;
        }

        private bool EnsureTables()
        {
            if (cbTables.Items.Count == 0)
            {
                return GetTables();
            }
            return true;
        }

        private bool GetTables()
        {
            if (!EnsureConnection())
                return false;

            datasetTableAndColumns =
                SpatiaLite2Provider.GetSpatiallyEnabledTables(ServerConnectionString);

            if (datasetTableAndColumns == null) return false;

            cbTables.Enabled = true;
            cbTables.DataSource = datasetTableAndColumns.Tables[0];
            cbTables.DisplayMember = "Label";
            cbTables.ValueMember = "TableName";

            return true;
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
                tbPath.Text = ofd.FileName;
        }

        private void cbTables_MouseDown(object sender, EventArgs e)
        {
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
            if (dgvColumns.Columns.Count < 6)
            {
                DataGridViewComboBoxColumn dgvc = new DataGridViewComboBoxColumn();
                dgvc.Name = "SortOrder";

                dgvc.DataSource = Enum.GetNames(typeof (SortOrder));
                    //new List<String>(GeoAPI.DataStructures.Enumerable.ToArray<String> Enum.GetNames(SortOrder));
                dgvColumns.Columns.Add(dgvc);

                dgvColumns.Columns[0].Visible = false;
                dgvColumns.Columns[1].ReadOnly = true;
                dgvColumns.Columns[2].ReadOnly = true;
                dgvColumns.Columns[3].ReadOnly = false;
                dgvColumns.Columns[4].Visible = false;
                dgvColumns.Columns[5].ReadOnly = false;
            }

            setOidColumn(-1);
            foreach (DataGridViewRow dgvr in dgvColumns.Rows)
                if ((bool) dgvr.Cells[4].Value)
                {
                    setOidColumn(dgvr.Index);
                    break;
                }

            dgvColumns.Enabled = true;

            lbSRID.Text = string.Format("SRID: {0}", ((DataRowView) cbTables.SelectedItem)["SRID"]);
        }

        private void setOidColumn(int rowindex)
        {
            if (oidcolumn == rowindex) return;
            Font fnt = dgvColumns.DefaultCellStyle.Font;

            if (oidcolumn >= 0)
            {
                dgvColumns.Rows[oidcolumn].Cells[2].Style.Font =
                    new Font(fnt.FontFamily.Name, fnt.SizeInPoints, FontStyle.Bold);

                dgvColumns.Rows[oidcolumn].Cells[5].Value = false;
            }

            if (rowindex >= 0)
            {
                dgvColumns.Rows[rowindex].Cells[2].Style.Font =
                    new Font(fnt.FontFamily.Name, fnt.SizeInPoints, FontStyle.Regular);
                ;
                dgvColumns.Rows[rowindex].Cells[5].Value = true;
            }
            oidcolumn = rowindex;
        }

        private void cbTables_MouseDown(object sender, MouseEventArgs e)
        {
            GetTables();
        }
    }
}