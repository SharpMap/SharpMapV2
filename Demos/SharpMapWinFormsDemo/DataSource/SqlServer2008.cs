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
using System.Windows.Forms;
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Utilities;

namespace MapViewer.DataSource
{
    internal partial class SqlServer2008 : MsSqlSpatial
    {
        public SqlServer2008()
        {
            InitializeComponent();
        }

        #region ICreateDataProvider Members

        public override IFeatureProvider GetProvider()
        {
            if (!EnsureTables())
                return null;

            string oidColumn, oidType, geometryColumn, schema, tableName;

            string[] prts = ((string) cbTable.SelectedItem).Split('|');
            oidColumn = prts[3];
            oidType = prts[2];
            geometryColumn = prts[1];
            prts = prts[0].Split('.');
            schema = prts[0];
            tableName = prts[1];
            IGeometryFactory f = new GeometryServices().DefaultGeometryFactory;

            string conn = ServerConnectionString;
            conn += string.Format("initial catalog={0};", cbDataBases.SelectedItem);

            switch (oidType)
            {
                case "bigint":
                    return new MsSqlServer2008Provider<long>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
                case "decimal":
                    return new MsSqlServer2008Provider<decimal>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
                case "int":
                case "smallint":
                    return new MsSqlServer2008Provider<int>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
                case "float":
                case "numeric":
                case "real":
                    return new MsSqlServer2008Provider<double>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
                case "tinyint":
                    return new MsSqlServer2008Provider<byte>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
                case "uniqueidentifier":
                    return new MsSqlServer2008Provider<Guid>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
                case "nvarchar":
                case "varchar":
                    return new MsSqlServer2008Provider<string>(f, conn, schema, tableName, oidColumn, geometryColumn) { ValidatesGeometry = true };
            }
            return null;
        }

        #endregion

        protected override bool GetTables()
        {
            if (!EnsureDataBases())
                return false;


            string conn = ServerConnectionString;

            conn += string.Format("initial catalog={0};", cbDataBases.SelectedItem);

            using (SqlConnection c = new SqlConnection(conn))
            {
                using (SqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText =
                        @"
SELECT keys.COLUMN_NAME as OIDColumn,types.name as OIDType,sch.name as SchemaName,  tbls.[name] as TableName, 
(select top 1 name from sys.columns where system_type_id = 240 and object_id = tbls.object_id) as GeometryColumn  
FROM sys.tables tbls 
inner join sys.columns cols on cols.object_id = tbls.object_id  
inner join sys.types types on types.system_type_id = cols.system_type_id
inner join sys.schemas sch on sch.schema_id = tbls.schema_id 
inner join  INFORMATION_SCHEMA.KEY_COLUMN_USAGE keys on keys.TABLE_NAME = tbls.name and Keys.COLUMN_NAME = cols.name
where tbls.object_id in (select object_id from sys.columns where system_type_id = 240)
order by tbls.name";
                    cmd.CommandType = CommandType.Text;


                    try
                    {
                        c.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            List<string> lst = new List<string>();
                            while (dr.Read())
                                lst.Add(string.Format("{0}.{1}|{2}|{3}|{4}", dr["schemaName"], dr["TableName"],
                                                      dr["GeometryColumn"], dr["OIDType"], dr["OIDColumn"]));

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
    }
}