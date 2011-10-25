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
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Utilities;

namespace MapViewer.DataSource
{
    public partial class MsSqlSpatial : UserControl, ICreateDataProvider
    {
        public MsSqlSpatial()
        {
            InitializeComponent();

            cbDataBases.MouseDown += cbDataBases_MouseDown;
        }

        protected string ServerConnectionString
        {
            get
            {
                SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
                sb.DataSource = this.tbServer.Text;
                sb.IntegratedSecurity = this.rbTrusted.Checked;
                sb.UserID = this.tbUName.Text;
                sb.Password = this.tbPassword.Text;
                return sb.ToString();
            }
        }

        #region ICreateDataProvider Members

        public string ProviderName
        {
            get { return ((string) cbTable.SelectedItem).Split('|')[0]; }
        }

        public virtual IFeatureProvider GetProvider()
        {
            if (EnsureTables())
            {
                string conn = ServerConnectionString;

                conn += string.Format("initial catalog={0};", cbDataBases.SelectedItem);

                string[] prts = ((string) cbTable.SelectedItem).Split('|');

                string geomColumn = prts[1];

                prts = prts[0].Split('.');
                string schema = prts[0];
                string tableName = prts[1];


                return new MsSqlSpatialProvider(new GeometryServices().DefaultGeometryFactory, conn, "ST", schema,
                                                tableName, "oid", geomColumn);
            }
            return null;
        }

        #endregion

        private void cbDataBases_MouseDown(object sender, MouseEventArgs e)
        {
            EnsureDataBases();
        }

        protected bool EnsureDataBases()
        {
            if (cbDataBases.Items.Count == 0)
            {
                if (!EnsureConnection())
                    return false;
                return GetDataBases();
            }
            return true;
        }

        private bool GetDataBases()
        {
            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(this.ServerConnectionString);
            sb.InitialCatalog = "master";
            string conn = sb.ToString();

            using (SqlConnection c = new SqlConnection(conn))
            {
                using (SqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "Select [name] from sys.databases";
                    cmd.CommandType = CommandType.Text;


                    try
                    {
                        List<string> lst = new List<string>();
                        c.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                                lst.Add((string) dr["name"]);
                            cbDataBases.DataSource = lst;
                        }
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
        }

        protected bool EnsureTables()
        {
            if (cbTable.Items.Count == 0)
            {
                if (!EnsureDataBases())
                    return false;

                return GetTables();
            }
            return true;
        }

        protected virtual bool GetTables()
        {
            if (!EnsureDataBases())
                return false;


            SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(this.ServerConnectionString);
            sb.InitialCatalog = this.cbDataBases.SelectedItem.ToString();
            string conn = sb.ToString();

            using (SqlConnection c = new SqlConnection(conn))
            {
                using (SqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText =
                        @"
IF EXISTS(SELECT * FROM sys.tables where object_id = object_id('ST.GEOMETRY_COLUMNS'))
BEGIN
SELECT F_TABLE_SCHEMA as [schema], F_TABLE_NAME as [name], F_GEOMETRY_COLUMN as GeomColumn from ST.GEOMETRY_COLUMNS ORDER BY F_TABLE_NAME
END";
                    cmd.CommandType = CommandType.Text;


                    try
                    {
                        c.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            List<string> lst = new List<string>();
                            while (dr.Read())
                                lst.Add(string.Format("{0}.{1}|{2}", dr["schema"], dr["name"], dr["GeomColumn"]));

                            cbTable.DataSource = lst;
                        }
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
        }

        private bool EnsureConnection()
        {
            bool ok = true;
            StringBuilder message = new StringBuilder();
            if (string.IsNullOrEmpty(tbServer.Text))
            {
                ok = false;
                message.AppendLine("Please enter a Server");
            }

            if (rbSqlServer.Checked)
            {
                if (string.IsNullOrEmpty(tbUName.Text))
                {
                    ok = false;
                    message.AppendLine("Sql Server authentication mode is set, but you did not supply UName");
                }
            }
            if (!ok)
            {
                MessageBox.Show(message.ToString(), "Errors", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ok;
            }

            using (SqlConnection conn = new SqlConnection(ServerConnectionString))
            {
                try
                {
                    conn.Open();
                }
                catch (SqlException ex)
                {
                    ok = false;
                    message.AppendFormat("Error connection to server {0}", ex.Message);
                }
            }
            if (!ok)
                MessageBox.Show(message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return ok;
        }

        private void rbSqlServer_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideLogin(rbSqlServer.Checked);
        }

        private void rbTrusted_CheckedChanged(object sender, EventArgs e)
        {
            ShowHideLogin(rbSqlServer.Checked);
        }

        private void ShowHideLogin(bool p)
        {
            grpLogin.Enabled = p;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetTables();
        }
    }
}