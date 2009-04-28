/*
 *  The attached / following is part of SharpMap.Data.Providers.PostGis
 *  SharpMap.Data.Providers.PostGis is free software © 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2008
 *  
 *  This work is based on SharpMap.Data.Providers.Db by John Diss for 
 *  Newgrove Consultants Ltd, www.newgrove.com
 *  
 *  Other than that, this spatial data provider requires:
 *  - SharpMap by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/SharpMap
 *    
 *  - GeoAPI.Net by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/GeoApi
 *    
 *  - .Net Data Provider for IBM DB2 Database Server by IBM.
 *    http://www-01.ibm.com/software/data/db2/9/download.html
 *    
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using GeoAPI.Geometries;
using IBM.Data.DB2;

namespace SharpMap.Data.Providers
{
    public static class DB2SpatialExtenderProviderStatic
    {
        //private static readonly System.Globalization.CultureInfo

        /// <summary>
        /// Name of spatial schema used with DB2SpatialExtender
        /// </summary>
        public const String DefaultSpatialSchema = "DB2GSE";

        /// <summary>
        /// Name used when no geometry column is supplied with constructor of PostGisProvider(TOid)
        /// </summary>
        public static String DefaultGeometryColumnName = "XGeometryX";

        /// <summary>
        /// Srid assumed when no Srid is mentioned
        /// </summary>
        public static String DefaultSrid = "0"; //"2000000000";

        /// <summary>
        /// Creates a spatially enabled database on the via connectionString specified Database;
        /// </summary>
        /// <typeparam name="TOid"></typeparam>
        /// <param name="featureDataTable"></param>
        /// <param name="connectionString"></param>
        public static void CreateDataTable<TOid>(FeatureDataTable featureDataTable, String connectionString)
        {
            CreateDataTable<TOid>(featureDataTable, featureDataTable.TableName, connectionString);
        }

        public static void CreateDataTable<TOid>(FeatureDataTable featureDataTable, String tableName,
                                                 String connectionString)
        {
            CreateDataTable<TOid>(featureDataTable, tableName, connectionString, DefaultGeometryColumnName);
        }

        public static void CreateDataTable<TOid>(FeatureDataTable featureDataTable, String tableName,
                                                 String connectionString, String geometryColumnName)
        {
            CreateDataTable<TOid>(featureDataTable, tableName, connectionString, geometryColumnName,
                                  OgcGeometryType.Geometry,
                                  -1, -1, -1);
        }

        public static void CreateDataTable<TOid>(
            FeatureDataTable featureDataTable,
            String tableName,
            String connectionString,
            String geometryColumnName,
            OgcGeometryType geometryType,
            Double smallRasterSize, Double mediumRasterSize, Double largeRasterSize
            )
        {
            DB2Connection conn = new DB2Connection(connectionString);
            if (conn.State == ConnectionState.Closed) conn.Open();

            string srid = featureDataTable.GeometryFactory.Srid ?? DefaultSrid;
            if (conn != null)
            {
                try
                {
                    string createTableClause = string.Format("CREATE TABLE \"{0}\".\"{1}\" ({2});",
                                                             DefaultSpatialSchema,
                                                             tableName,
                                                             ColumnsClause(featureDataTable.Columns,
                                                                           featureDataTable.Constraints));

                    new DB2Command(createTableClause, conn).ExecuteNonQuery();

                    new DB2Command(string.Format("ALTER TABLE \"{0}\".\"{1}\" ADD COLUMN \"{2}\" \"{0}\".{3};",
                                                 DefaultSpatialSchema, tableName, geometryColumnName,
                                                 ToDb2GeometryType(geometryType)), conn).ExecuteNonQuery();

                    Object db2crsname = new DB2Command(String.Format(
                                                           @"SELECT DISTINCT SPATIAL_REF_SYS.CS_NAME
   FROM ""{0}"".""SPATIAL_REF_SYS"" AS SPATIAL_REF_SYS
   WHERE SPATIAL_REF_SYS.AUTH_SRID = {1};",
                                                           DefaultSpatialSchema,
                                                           srid),
                                                       conn).ExecuteScalar();

                    if (db2crsname == null || db2crsname == DBNull.Value || ((String) db2crsname).Length == 0)
                        db2crsname = "DEFAULT_SRS";

                    //register spatial column
                    DB2Command cmd =
                        new DB2Command(String.Format("CALL \"{0}\".ST_REGISTER_SPATIAL_COLUMN(?,?,?,?,?,?);",
                                                     DefaultSpatialSchema),
                                       conn);
                    //cmd.CommandType = CommandType.StoredProcedure;

                    //input parameters
                    //DB2Parameter par;
                    DB2Parameter par1 = cmd.Parameters.Add(new DB2Parameter("@P1", DB2Type.VarChar, 130));
                    par1.Value = "\"" + DefaultSpatialSchema + "\"";
                    DB2Parameter par2 = cmd.Parameters.Add(new DB2Parameter("@P2", DB2Type.VarChar, 130));
                    par2.Value = "\"" + tableName + "\"";
                    DB2Parameter par3 = cmd.Parameters.Add(new DB2Parameter("@P3", DB2Type.VarChar, 130));
                    par3.Value = "\"" + geometryColumnName + "\"";
                    DB2Parameter par4 = cmd.Parameters.Add(new DB2Parameter("@P4", DB2Type.VarChar, 130));
                    par4.Value = "\"" + db2crsname + "\"";

                    //output parameters
                    DB2Parameter par5 = cmd.Parameters.Add(new DB2Parameter("@P5", DB2Type.Integer, 4));
                    par5.Direction = ParameterDirection.Output;
                    DB2Parameter par6 = cmd.Parameters.Add(new DB2Parameter("@P6", DB2Type.VarChar, 1024));
                    par6.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    if (smallRasterSize != -1d)
                    {
                        //adding spatial index
                        new DB2Command(
                            String.Format(
                                "CREATE INDEX \"{0}\".\"idx_{1}_{2}\" ON \"{0}\".\"{1}\"(\"{2}\") EXTEND USING \"{0}\".({3}, {4}, {5});",
                                DefaultSpatialSchema,
                                tableName,
                                geometryColumnName,
                                smallRasterSize, mediumRasterSize, largeRasterSize),
                            conn).ExecuteNonQuery();
                    }
                }
                catch (DB2Exception ex)
                {
                    Trace.Write(ex.Message);
                    //throw new DB2SpatialExtenderException(string.Format("Cannot create geometry column with type of '{0}'", geometryType.ToString()));
                }
                catch
                {
                }
            }
            conn.Close();
            conn = null;

            DB2SpatialExtenderProvider<TOid> prov = new DB2SpatialExtenderProvider<TOid>(
                featureDataTable.GeometryFactory, connectionString, DefaultSpatialSchema, tableName,
                featureDataTable.Columns[0].ColumnName, geometryColumnName);

            prov.Insert(featureDataTable);

            return;
        }

        internal static object ToDb2GeometryType(OgcGeometryType geometryType)
        {
            switch (geometryType)
            {
                case OgcGeometryType.Geometry:
                case OgcGeometryType.Point:
                case OgcGeometryType.LineString:
                case OgcGeometryType.Polygon:
                case OgcGeometryType.MultiLineString:
                case OgcGeometryType.MultiPoint:
                case OgcGeometryType.MultiPolygon:
                case OgcGeometryType.GeometryCollection:
                    return string.Format("ST_{0}", geometryType.ToString().ToUpper());
                default:
                    throw new ArgumentException(String.Format("Invalid geometry type: {0}", geometryType));
            }
        }

        private static String ColumnsClause(DataColumnCollection dcc, ConstraintCollection ccc)
        {
            string[] columns = new String[dcc.Count];

            Int32 index = 0;
            foreach (DataColumn dc in dcc)
            {
                String columnName = dc.ColumnName;

                columns[index++] = string.Format(" \"{0}\" {1}{2}",
                                                 columnName, DB2SpatialExtenderDbUtility.GetTypeString(dc.DataType),
                                                 dc.DefaultValue == DBNull.Value
                                                     ?
                                                         ""
                                                     :
                                                         String.Format(" DEFAULT {0}",
                                                                       String.Format(CultureInfo.InvariantCulture, "{0}",
                                                                                     dc.DefaultValue)));
                ;
                //dc.DataType.
                if (index == 1) columns[0] = columns[0] + " NOT NULL";
            }
            index = 0;

            string[] constraints = new String[ccc.Count];
            foreach (Constraint c in ccc)
            {
                UniqueConstraint uc = c as UniqueConstraint;
                if (uc != null)
                {
                    if (uc.IsPrimaryKey)
                    {
                        constraints[index++] = String.Format(", CONSTRAINT \"{0}\" PRIMARY KEY ({1})",
                                                             uc.ConstraintName,
                                                             ColumnNamesToCommaSeparatedString(uc.Columns));
                    }
                    else
                    {
                        constraints[index++] = String.Format(", CONSTRAINT \"{0}\" UNIQUE ({1})",
                                                             uc.ConstraintName,
                                                             ColumnNamesToCommaSeparatedString(uc.Columns));
                    }
                }

                //ForeignKeyConstraint fc = c as ForeignKeyConstraint;
                //if (fc != null)
                //{
                //    constraints[index++] = String.Format(" CONSTRAINT \"{0}\" FOREIGN KEY ({1}) REFERENCES {2} ({3}) MATCH FULL ON UPDATE {4} ON DELETE {5}",
                //        fc.ConstraintName,
                //        ColumnNamesToCommaSeparatedString(fc.Columns),
                //        fc.RelatedTable.TableName,
                //        ColumnNamesToCommaSeparatedString(fc.RelatedColumns),
                //        ruleToAction(fc.UpdateRule),
                //        ruleToAction(fc.DeleteRule)
                //    );

                //}
            }

            String constraintsClause = "";
            if (index > 0)
            {
                Array.Resize(ref constraints, index);
                constraintsClause = String.Join(String.Empty, constraints);
            }
            return String.Join(",", columns) + constraintsClause;
        }

        private static String ruleToAction(Rule rule)
        {
            switch (rule)
            {
                case Rule.Cascade:
                    return "CASCADE";
                case Rule.SetDefault:
                    return "SET DEFAULT";
                case Rule.SetNull:
                    return "SET NULL";
                case Rule.None:
                default:
                    return "NO ACTION";
            }
        }

        private static String OrdinalsToCommaSeparatedString(IEnumerable<DataColumn> dcc)
        {
            return OrdinalsToCommaSeparatedString(String.Empty, dcc);
        }

        private static String OrdinalsToCommaSeparatedString(String prefix, IEnumerable dcc)
        {
            String ret = "";
            foreach (DataColumn t in dcc)
                ret += String.Format(", {0}{1}", prefix, t.Ordinal);

            if (ret.Length > 0)
                ret = ret.Substring(2);

            return ret;
        }

        private static String ColumnNamesToCommaSeparatedString(IEnumerable<DataColumn> dcc)
        {
            return ColumnNamesToCommaSeparatedString(String.Empty, dcc);
        }

        private static String ColumnNamesToCommaSeparatedString(String prefix, IEnumerable<DataColumn> dcc)
        {
            String ret = "";
            foreach (DataColumn t in dcc)
            {
                String columnName = t.ColumnName;
                if (columnName == "oid") columnName = "poid";
                ret += String.Format(", \"{0}\"", columnName);
            }
            if (ret.Length > 0)
                ret = ret.Substring(2);

            return ret;
        }

        /// <summary>
        /// Determines whether DB2 database is spatially enabled
        /// </summary>
        /// <param name="connectionString">Connection String to access DB2 Database</param>
        /// <returns>
        /// <value>true</value> if it is,
        /// <value>false</value> if it isn't.
        /// </returns>
        public static Boolean IsSpatiallyEnabled(String connectionString)
        {
            Boolean result = false;
            try
            {
                using (DB2Connection conn = new DB2Connection(connectionString))
                {
                    conn.Open();

                    object spatialSchema = new DB2Command(
                        String.Format("SELECT COUNT(*) FROM SYSCAT.SCHEMATA WHERE (SCHEMANAME='{0}');",
                                      DefaultSpatialSchema),
                        conn).ExecuteScalar();

                    result = ((int) spatialSchema == 1);
                }
            }
            catch
            {
            }
            return result;
        }
    }
}