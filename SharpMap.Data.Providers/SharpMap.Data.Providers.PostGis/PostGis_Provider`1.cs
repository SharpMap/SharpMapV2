#define EXPLAIN
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
#if DEBUG
using System.Diagnostics;
#endif
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using GeoAPI.Geometries;
using GeoAPI.DataStructures;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.PostGis;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{

    public class PostGis_Provider<TOid>
        : SpatialDbProviderBase<TOid>, ISpatialDbProvider<PostGis_Utility>
    {

        #region Static Properties

        static PostGis_Provider()
        {
            AddDerivedProperties(typeof(PostGis_Provider<TOid>));
        }


        #endregion

        /// <summary>
        /// Spatialite tables only accept geometries specified for the geometry column
        /// Look for entry in geometry_columns table of sqlite-db file
        /// </summary>
        private OgcGeometryType _validGeometryType = OgcGeometryType.Geometry;

        /// <summary>
        /// Determines whether Postgres database is spatially enabled
        /// </summary>
        /// <param name="connectionString">Connection String to access SqLite database file</param>
        /// <returns>
        /// <value>true</value> if it is,
        /// <value>false</value> if it isn't.
        /// </returns>
        static Boolean IsSpatiallyEnabled(String connectionString)
        {
            Boolean result = false;
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                Object postGisVersion =
                    new NpgsqlCommand("SELECT postgis_version();", conn).ExecuteScalar();
            }

            return result;
        }


        public PostGis_Provider(IGeometryFactory geometryFactory, string connectionString, string tableName)
            : this(geometryFactory, connectionString, "public", tableName, "poid", getGeometryColumnName(connectionString, "public", tableName))
        {
        }

        private static String getGeometryColumnName(string connectionString, String schemaName, String tableName)
        {
            String columnName = PostGis_ProviderStatic.DefaultGeometryColumnName;
            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                try
                {
                    columnName = (String)new NpgsqlCommand(
                        String.Format("SELECT f_geometry_column FROM {0}.geometry_columns WHERE (f_table_name='{1}') LIMIT 1;", schemaName, tableName.ToLower()),
                        cn).ExecuteScalar();

                }
                catch
                {
                    columnName = "";
                }
            }

            return columnName;

        }

        public PostGis_Provider(IGeometryFactory geometryFactory, string connectionString,
                                  string tableSchema, string tableName, string oidColumn, string geometryColumn)
            : base(
                    new PostGis_Utility(), geometryFactory, connectionString, tableSchema,
                    tableName,
                    oidColumn,
                    geometryColumn)
        {

            using (NpgsqlConnection cn = new NpgsqlConnection(connectionString))
            {
                cn.Open();
                try
                {
                    String selectClause = string.Format("SELECT type, srid FROM {0}.geometry_columns WHERE (f_table_name='{1}' AND f_geometry_column='{2}')",
                      tableSchema, tableName, geometryColumn);
                    NpgsqlDataReader dr = new NpgsqlCommand(selectClause, cn).ExecuteReader();
                    if (dr.HasRows)
                    {

                        dr.Read();
                        //valid geometry type
                        _validGeometryType = parseGeometryType(dr.GetString(0));

                        //Srid
                        Srid = dr.GetInt32(1).ToString();
                        if (geometryFactory.Srid == null)
                            geometryFactory.Srid = Srid;
                        else
                        {
                            //geometryFactory.SpatialReference
                        }

                    }
                    else
                    {
                        dr.Close();
                        selectClause = string.Format("SELECT ST_SRID({0}) FROM {1}.{2} LIMIT 1;", geometryColumn, tableSchema, tableName);
                        Srid = ((Int32)new NpgsqlCommand(selectClause, cn).ExecuteScalar()).ToString();
                        _validGeometryType = OgcGeometryType.Geometry;
                    }
                }
                catch (Exception)
                {
                    _validGeometryType = OgcGeometryType.Unknown;
                }
            }
        }

        private OgcGeometryType parseGeometryType(String geometryString)
        {
            if (String.IsNullOrEmpty(geometryString))
                throw new PostGis_Exception("Geometry type not specified!");

            switch (geometryString.ToUpper())
            {
                case "GEOMETRY": return OgcGeometryType.Geometry;
                case "POINT": return OgcGeometryType.Point;
                case "LINESTRING": return OgcGeometryType.LineString;
                case "POLYGON": return OgcGeometryType.Polygon;
                case "MULTIPOINT": return OgcGeometryType.MultiPoint;
                case "MULTILINESTRING": return OgcGeometryType.MultiLineString;
                case "MULTIPOLYGON": return OgcGeometryType.MultiPolygon;
                case "GEOMETRYCOLLECTION": return OgcGeometryType.GeometryCollection;
                default:
                    throw new PostGis_Exception(string.Format("Invalid geometry type '{0}'", geometryString));
            }
        }

        public override string GeometryColumnConversionFormatString
        {
            get { return "ST_AsBinary({0})::bytea"; }
        }

        public override string GeomFromWkbFormatString
        {
            get { return string.Format("ST_GeomFromWKB({0}, {1})", "{0}", Srid == null ? PostGis_ProviderStatic.DefaultSrid : Srid); }
        }

        public override IExtents GetExtents()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                conn.Open();
                cmd.Connection = conn;
                /*cmd.CommandText = String.Format(
                    "SELECT " + 
                        "st_xmin(x.ext) AS xmin, st_ymin(x.ext) AS ymin, " +
                        "st_xmax(x.ext) AS xmax, st_ymax(x.ext) AS ymax " +
                    "FROM "+
                        "(SELECT st_box3d( st_estimated_extent('{0}', '{1}', '{2}') ) AS ext ) as x;", 
                    TableSchema, Table, GeometryColumn);

                 */
                cmd.CommandText = String.Format(
                    "SELECT " +
                        "st_xmin(x.ext) AS xmin, st_ymin(x.ext) AS ymin, " +
                        "st_xmax(x.ext) AS xmax, st_ymax(x.ext) AS ymax " +
                    "FROM " +
                        "(SELECT st_extent( \"{2}\" ) as ext FROM {0}.\"{1}\") as x;",
                    TableSchema, Table, GeometryColumn);
                cmd.CommandType = CommandType.Text;

                using (NpgsqlDataReader r = (NpgsqlDataReader)cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        return GeometryFactory.CreateExtents2D(
                            r.GetDouble(0), r.GetDouble(1),
                            r.GetDouble(2), r.GetDouble(3));
                    }
                }
                return GeometryFactory.CreateExtents();
            }
        }

        public override void Insert(IEnumerable<FeatureDataRow> features)
        {
            base.Insert(features);
            using (NpgsqlConnection cn = new NpgsqlConnection(ConnectionString))
            {
                cn.Open();
                new NpgsqlCommand(String.Format("VACUUM ANALYZE {0};", QualifiedTableName), cn).ExecuteNonQuery();
            }
        }

        public override string QualifiedTableName
        {
            get
            {
                if (String.IsNullOrEmpty(Table))
                    throw new PostGis_Exception("Table name mustnot be null or emtpy");

                if (String.IsNullOrEmpty(TableSchema))
                    return String.Format("\"{0}\"", Table);
                else
                    return String.Format("{0}.\"{1}\"", TableSchema, Table);
            }
        }

        public override DataTable GetSchemaTable(Boolean withGeometryColumn)
        {
            DataTable dt = null;
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (NpgsqlCommand cmd = new NpgsqlCommand(string.Format("SELECT * FROM {0};", QualifiedTableName), conn))
                {
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.FillSchema(ds, SchemaType.Source);
                    dt = ds.Tables["Table"];
                }

                //dt.Columns[OidColumn].DataType = typeof(Int64);
                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    if (dt.Columns[i].ColumnName == GeometryColumn)
                        dt.Columns[i].DataType = typeof(Byte[]);
                    //replaceObjectDataColumn(dt, dt.Columns[i]);

                }

                if (!withGeometryColumn)
                {
                    dt.Columns.Remove(GeometryColumn);
                }
            }
            return dt;
        }

        private void replaceObjectDataColumn(DataTable dt, DataColumn dcOld)
        {
            int index = dcOld.Ordinal;
            dt.Columns.Remove(dcOld);

            DataColumn dcNew = new DataColumn(dcOld.ColumnName, typeof(System.Byte[]), dcOld.Expression, dcOld.ColumnMapping);
            dcNew.AutoIncrement = dcOld.AutoIncrement;
            dcNew.AutoIncrementSeed = dcOld.AutoIncrementSeed;
            dcNew.AutoIncrementStep = dcOld.AutoIncrementStep;
            dcNew.Caption = dcOld.Caption;
            dcNew.DefaultValue = dcOld.DefaultValue;
            foreach (object prop in dcOld.ExtendedProperties)
                dcNew.ExtendedProperties.Add(prop.GetHashCode(), prop);
            dcNew.MaxLength = dcOld.MaxLength;
            dcNew.Prefix = dcOld.Prefix;
            dcNew.ReadOnly = dcOld.ReadOnly;
            dcNew.Unique = dcOld.Unique;

            dt.Columns.Add(dcNew);
            dcNew.SetOrdinal(index);

            dcOld.Dispose();
            dcOld = null;
        }

        protected override ExpressionTreeToSqlCompilerBase<TOid> CreateSqlCompiler(Expression expression)
        {
            return new PostGis_ExpressionTreeToSqlCompiler<TOid>(this, expression);
        }

        public void Vacuum()
        {
            using (NpgsqlConnection cn = new NpgsqlConnection(this.ConnectionString))
            {
                try
                {
                    new NpgsqlCommand(
                        String.Format("VACUUM ANALYZE {0};", QualifiedTableName),
                        cn).ExecuteNonQuery();
                }
                finally
                { }
            }
        }

        #region Private helpers for Insert and Update

        protected override string InsertClause(IDbCommand cmd)
        {
            var sets = new List<string>();

            //Columnnames
            DataColumnCollection dcc = GetSchemaTable(false).Columns;
            foreach (DataColumn dc in dcc)
                sets.Add(string.Format(" \"{0}\"", dc.ColumnName));

            String columNames = string.Format("({0}, \"{1}\")", String.Join(",", sets.ToArray()).Trim(), GeometryColumn);
            sets.Clear();

            //Parameter
            foreach (DataColumn dc in dcc)
            {
                IDataParameter param = null;
                sets.Add(string.Format(" :{0}", ParamForColumn(dc, out param)));
                cmd.Parameters.Add(param);
            }

            //Geometry
            sets.Add(string.Format("{0}", string.Format(GeomFromWkbFormatString, ":PGeo")));
            //cmd.Parameters.Add(DbUtility.CreateParameterByType("PGeo", toDbType(typeof(byte[])), ParameterDirection.Input));
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("PGeo", ParameterDirection.Input));
            return String.Format("{0} VALUES({1})", columNames, string.Join(",", sets.ToArray()).Trim());
        }

        protected override string UpdateClause(IDbCommand cmd)
        {
            var sets = new List<string>();
            //Attribute
            foreach (DataColumn dc in GetSchemaTable(false).Columns)
            {
                IDataParameter param = null;
                sets.Add(string.Format(" \"{0}\"=:{1}", dc.ColumnName, ParamForColumn(dc, out param)));
                cmd.Parameters.Add(param);
            }
            //Geometry
            sets.Add(
                string.Format("\"{0}\"={1}",
                              GeometryColumn,
                              string.Format(GeomFromWkbFormatString, ":PGeo")));
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("PGeo", ParameterDirection.Input));

            return String.Join(",", sets.ToArray()).Trim();
        }

        protected override string WhereClause(IDbCommand cmd)
        {
            cmd.Parameters.Add(DbUtility.CreateParameter<UInt32>(":POldOid", ParameterDirection.Input));
            return string.Format("{0}=:POldOid", OidColumn);
        }

        protected override String ParamForColumn(DataColumn dc, out IDataParameter param)
        {
            String paramName = ParamForColumn(dc);
            param = DbUtility.CreateParameter(paramName, dc.DataType, ParameterDirection.Input);
            return paramName;
        }

        protected override String ParamForColumn(DataColumn dc)
        {
            return string.Format("P{0}", dc.Ordinal);
        }

        #endregion

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                              ExpressionTreeToSqlCompilerBase<TOid> compiler)
        {
            int pageNumber = GetProviderPropertyValue<DataPageNumberExpression, int>(properties, -1);
            int pageSize = GetProviderPropertyValue<DataPageSizeExpression, int>(properties, 0);

            string sql = "";
            if (pageSize > 0 && pageNumber > -1)
                sql = GenerateSelectSql(properties, compiler, pageSize, pageNumber);
            else
            {
                string orderByCols = String.Join(",",
                                                 Enumerable.ToArray(Processor.Transform(
                                                     GetProviderPropertyValue<OrderByCollectionExpression, CollectionExpression<OrderByExpression>>(
                                                         properties, new CollectionExpression<OrderByExpression>(new OrderByExpression[] { })), o => o.ToString())));


                string orderByClause = string.IsNullOrEmpty(orderByCols) ? "" : " ORDER BY " + orderByCols;

                string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                               FormatColumnNames(true, true,
                                                                                 compiler.ProjectedColumns.Count > 0
                                                                                     ? compiler.ProjectedColumns
                                                                                     : SelectAllColumnNames())));

                sql = String.Format("SELECT {0} FROM {1} {2} {3} {4} {5}",
                                     mainQueryColumns,
                                     QualifiedTableName,
                                     compiler.SqlJoinClauses,
                                     string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                     compiler.SqlWhereClause,
                                     orderByClause);
            }
#if DEBUG && EXPLAIN
            if (sql.StartsWith("SELECT"))
            {
                using (NpgsqlConnection cn = new NpgsqlConnection(ConnectionString))
                {
                    cn.Open();
                    NpgsqlCommand cm = new NpgsqlCommand(String.Format("EXPLAIN ANALYZE {0}", sql), cn);
                    foreach (IDataParameter par in compiler.ParameterCache.Values)
                        cm.Parameters.Add(par);

                    Debug.WriteLine("");
                    NpgsqlDataReader dr = cm.ExecuteReader();
                    if (dr.HasRows)
                        while (dr.Read()) Debug.WriteLine(dr.GetString(0));
                    Debug.WriteLine("");
                }
            }
#endif
            return sql;
        }

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                              ExpressionTreeToSqlCompilerBase<TOid> compiler, int pageSize, int pageNumber)
        {
            string orderByCols = String.Join(",",
                                             Enumerable.ToArray(Processor.Transform(
                                                 GetProviderPropertyValue<OrderByCollectionExpression, CollectionExpression<OrderByExpression>>(
                                                     properties, new CollectionExpression<OrderByExpression>(new OrderByExpression[] { })), o => o.ToString())));


            orderByCols = string.IsNullOrEmpty(orderByCols) ? OidColumn : orderByCols;

            Int64 startRecord = (pageNumber * pageSize) + 1;
            Int64 endRecord = (pageNumber + 2) * pageSize; //sequence fn seems to be called twice

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                           FormatColumnNames(true, true,
                                                                             compiler.ProjectedColumns.Count > 0
                                                                                 ? compiler.ProjectedColumns
                                                                                 : SelectAllColumnNames())));

            //string subQueryColumns = string.Join(",", Enumerable.ToArray(compiler.ProjectedColumns.Count > 0
            //                             ? FormatColumnNames(false, false, compiler.ProjectedColumns)
            //                             : SelectAllColumnNames(false, false)));

            string sqlWhereClause = String.Format(@" WHERE (nextval('sharpmap') BETWEEN {0} AND {1})",
                    compiler.CreateParameter(startRecord).ParameterName,
                    compiler.CreateParameter(endRecord).ParameterName);

            if (!String.IsNullOrEmpty(compiler.SqlWhereClause))
                sqlWhereClause += " AND " + compiler.SqlWhereClause;

            return string.Format(
@"DROP SEQUENCE IF EXISTS ""sharpmap"";
CREATE TEMPORARY SEQUENCE ""sharpmap"" INCREMENT BY 1;
SELECT {0} FROM {1} {2} {3} ORDER BY {4};",
                mainQueryColumns,
                QualifiedTableName,
                compiler.SqlJoinClauses,
                sqlWhereClause,
                orderByCols
                );

        }

        public override IEnumerable<string> FormatColumnNames(bool formatGeometryColumn,
                                                                bool qualifyColumnNames, IEnumerable<string> names)
        {
            foreach (string col in names)
            {
                yield return
                    formatGeometryColumn &&
                    String.Equals(col, GeometryColumn, StringComparison.InvariantCultureIgnoreCase)
                    ? String.Format(GeometryColumnConversionFormatString + " AS \"{1}\"", qualifyColumnNames ? QualifyColumnName(col) : col, GeometryColumn)
                        : qualifyColumnNames
                              ? QualifyColumnName(col)
                              : string.Format("{0}", col);
            }
        }

        public override string QualifyColumnName(string column)
        {
            if (String.IsNullOrEmpty(column))
                throw new ArgumentNullException("column");

            return String.Format("\"{0}\".\"{1}\"", Table, column);
        }



        #region ISpatialDbProvider<PostGis_Utility> Members

        public new PostGis_Utility DbUtility
        {
            get { return (PostGis_Utility)base.DbUtility; }
        }

        #endregion
    }
}