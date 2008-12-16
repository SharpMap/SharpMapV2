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
 *  - Npgsql - .Net Data Provider for Postgresql Provider by 
 *    Josh Cooley,Francisco Figueiredo jr. and others, 
 *    released under this license http://npgsql.projects.postgresql.org/license.html
 *    http://npgsql.projects.postgresql.org/
 *    
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using GeoAPI.Geometries;
using Npgsql;
using SharpMap.Data.Providers.PostGis;

namespace SharpMap.Data.Providers
{
    internal static class PostGisProviderStatic
    {
        //private static readonly System.Globalization.CultureInfo

        /// <summary>
        /// Name used when no geometry column is supplied with constructor of PostGisProvider(TOid)
        /// </summary>
        public static String DefaultGeometryColumnName = "XGeometryX";

        /// <summary>
        /// Srid assumed when no Srid is mentioned
        /// </summary>
        public static String DefaultSrid = "EPSG:4326";

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
            CreateDataTable<TOid>(featureDataTable, "public", tableName, connectionString, DefaultGeometryColumnName);
        }

        public static void CreateDataTable<TOid>(FeatureDataTable featureDataTable, String schemaName, String tableName,
                                                 String connectionString, String geometryColumnName)
        {
            CreateDataTable<TOid>(featureDataTable, schemaName, tableName, connectionString, geometryColumnName,
                                  OgcGeometryType.Geometry);
        }

        public static void CreateDataTable<TOid>(
            FeatureDataTable featureDataTable,
            String schemaName,
            String tableName,
            String connectionString,
            String geometryColumnName,
            OgcGeometryType geometryType)
        {
            var conn = new NpgsqlConnection(connectionString);
            if (conn.State == ConnectionState.Closed) conn.Open();

            string srid = featureDataTable.GeometryFactory.Srid == null
                              ? DefaultSrid
                              : featureDataTable.GeometryFactory.Srid;
            if (conn != null)
            {
                try
                {
                    string createTableClause = string.Format("CREATE TABLE {0}.\"{1}\" ({2}) WITH(OIDS=TRUE);",
                                                             schemaName,
                                                             tableName,
                                                             ColumnsClause(featureDataTable.Columns,
                                                                           featureDataTable.Constraints));

                    new NpgsqlCommand(createTableClause, conn).ExecuteNonQuery();

                    String addGeometryColumnClause = String.Format("('{0}', '{1}', {2}, '{3}', {4})",
                                                                   tableName,
                                                                   geometryColumnName,
                                                                   srid,
                                                                   geometryType.ToString().ToUpper(),
                                                                   2);

                    //adding spatial column
                    new NpgsqlCommand(String.Format("SELECT AddGeometryColumn {0};",
                                                    addGeometryColumnClause),
                                      conn).ExecuteNonQuery();

                    //adding GIST index
                    new NpgsqlCommand(String.Format("CREATE INDEX index_{0}_{1} ON {2}.\"{3}\" USING gist(\"{4}\");",
                                                    tableName,
                                                    geometryColumnName,
                                                    schemaName,
                                                    tableName,
                                                    geometryColumnName),
                                      conn).ExecuteNonQuery();
                }
                catch (NpgsqlException ex)
                {
                    Trace.Write(ex.Message);
                    throw new PostGisException(string.Format("Cannot create geometry column with type of '{0}'",
                                                             geometryType));
                }
                catch
                {
                }
            }
            conn.Close();
            conn = null;

            var prov = new PostGisProvider<TOid>(
                featureDataTable.GeometryFactory, connectionString, schemaName, tableName,
                featureDataTable.Columns[0].ColumnName, geometryColumnName);

            prov.Insert(featureDataTable);

            return;
        }

        private static String ColumnsClause(DataColumnCollection dcc, ConstraintCollection ccc)
        {
            var columns = new String[dcc.Count];

            Int32 index = 0;
            foreach (DataColumn dc in dcc)
            {
                String columnName = dc.ColumnName;
                if (columnName == "oid") columnName = "poid";

                columns[index++] = string.Format(" \"{0}\" {1}{2}",
                                                 columnName, PostGisDbUtility.GetTypeString(dc.DataType),
                                                 dc.DefaultValue == DBNull.Value
                                                     ?
                                                         ""
                                                     :
                                                         String.Format(" DEFAULT {0}",
                                                                       String.Format(CultureInfo.InvariantCulture, "{0}",
                                                                                     dc.DefaultValue)));
                ;
            }
            index = 0;

            var constraints = new String[ccc.Count];
            foreach (Constraint c in ccc)
            {
                var uc = c as UniqueConstraint;
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
                //Other Constraints are not supported by SqLite
                var fc = c as ForeignKeyConstraint;
                if (fc != null)
                {
                    constraints[index++] =
                        String.Format(
                            " CONSTRAINT \"{0}\" FOREIGN KEY ({1}) REFERENCES {2} ({3}) MATCH FULL ON UPDATE {4} ON DELETE {5}",
                            fc.ConstraintName,
                            ColumnNamesToCommaSeparatedString(fc.Columns),
                            fc.RelatedTable.TableName,
                            ColumnNamesToCommaSeparatedString(fc.RelatedColumns),
                            ruleToAction(fc.UpdateRule),
                            ruleToAction(fc.DeleteRule)
                            );
                }
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
    }
}