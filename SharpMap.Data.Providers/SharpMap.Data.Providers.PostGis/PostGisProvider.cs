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
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using GeoAPI.Geometries;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using Npgsql;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.PostGis;
using SharpMap.Expressions;
using SharpMap.Utilities.SridUtility;

#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Processor = GeoAPI.DataStructures.Processor;
using Caster = GeoAPI.DataStructures.Caster;
using Enumerable = GeoAPI.DataStructures.Enumerable;
#endif

namespace SharpMap.Data.Providers
{
    public class PostGisProvider<TOid>
        : SpatialDbProviderBase<TOid>, ISpatialDbProvider<PostGisDbUtility>
    {
        #region Static Properties

        static PostGisProvider()
        {
            AddDerivedProperties(typeof (PostGisProvider<TOid>));
        }

        #endregion

        /// <summary>
        /// Spatialite tables only accept geometries specified for the geometry column
        /// Look for entry in geometry_columns table of sqlite-db file
        /// </summary>
        private OgcGeometryType _validGeometryType = OgcGeometryType.Geometry;


        public PostGisProvider(IGeometryFactory geometryFactory, string connectionString, string tableName)
            : this(
                geometryFactory, connectionString, "public", tableName, "poid",
                getGeometryColumnName(connectionString, "public", tableName))
        {
        }

        public PostGisProvider( IGeometryFactory geometryFactory, string connectionString,
            string tableSchema, string tableName, string oidColumn, string geometryColumn )
            : this( geometryFactory, connectionString, tableSchema, tableName, oidColumn, geometryColumn, null )
        { }

        public PostGisProvider(IGeometryFactory geometryFactory, string connectionString,
                               string tableSchema, string tableName, string oidColumn, string geometryColumn,
                               ICoordinateTransformationFactory coordinateTransformationFactory)
            : base(
                new PostGisDbUtility(), geometryFactory, connectionString, tableSchema,
                tableName,
                oidColumn,
                geometryColumn,
                coordinateTransformationFactory)
        {
            using (var cn = (NpgsqlConnection)DbUtility.CreateConnection(connectionString))
            {

                try
                {

                    cn.Open();

                    if ( !PostGisProviderStatic.Has_X_Privilege( cn, "table", "\"public\".\"geometry_columns\"", "SELECT" ) )
                        throw new PostGisException( "Insufficient rights to access table \"public\".\"geometry_columns\"!" );

                    if ( !PostGisProviderStatic.Has_X_Privilege( cn, "table", string.Format( "\"{0}\".\"{1}\"", tableSchema, tableName ), "SELECT" ) )
                        throw new PostGisException( string.Format( "Insufficient rights to access table \"{0}\".\"{1}\"!", tableSchema, tableName ) );

                    var cmd = (NpgsqlCommand)DbUtility.CreateCommand();
                    cmd.Connection = cn;
                    cmd.CommandText =
  @"SELECT x.""type""
    FROM ""public"".""geometry_columns"" AS x
    WHERE (x.""f_table_schema""=:p0 AND x.""f_table_name""=:p1 AND x.""f_geometry_column""=:p2);";
                    cmd.Parameters.Add( DbUtility.CreateParameter( "p0", tableSchema, ParameterDirection.Input ) );
                    cmd.Parameters.Add( DbUtility.CreateParameter( "p1", tableName, ParameterDirection.Input ) );
                    cmd.Parameters.Add( DbUtility.CreateParameter( "p2", geometryColumn, ParameterDirection.Input ) );

                    var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    if ( dr.HasRows )
                    {
                        dr.Read();
                        //valid geometry type
                        _validGeometryType = parseGeometryType( dr.GetString( 0 ) );

                    }
                    else
                    {
                        _validGeometryType = OgcGeometryType.Geometry;
                    }
                }
                catch ( Exception )
                {
                    _validGeometryType = OgcGeometryType.Unknown;
                }
            }
        }

        public override string GeometryColumnConversionFormatString
        {
            get { return "ST_AsBinary({0})::bytea"; }
        }

        public override string GeomFromWkbFormatString
        {
            get
            {
                return string.Format("ST_GeomFromWKB({0}, {1})", "{0}",
                    SridInt.HasValue ? SridInt : PostGisProviderStatic.DefaultSridInt );
            }
        }

        public override string QualifiedTableName
        {
            get
            {
                if (String.IsNullOrEmpty(Table))
                    throw new PostGisException("Table name mustnot be null or emtpy");

                if (String.IsNullOrEmpty(TableSchema))
                    return String.Format("\"{0}\"", Table);
                else
                    return String.Format("{0}.\"{1}\"", TableSchema, Table);
            }
        }

        #region ISpatialDbProvider<PostGisDbUtility> Members

        public new PostGisDbUtility DbUtility
        {
            get { return (PostGisDbUtility) base.DbUtility; }
        }

        #endregion

        /// <summary>
        /// Determines whether Postgres database is spatially enabled
        /// </summary>
        /// <param name="connectionString">Connection String to access SqLite database file</param>
        /// <returns>
        /// <value>true</value> if it is,
        /// <value>false</value> if it isn't.
        /// </returns>
        private static Boolean IsSpatiallyEnabled(String connectionString)
        {
            String postGisVersion = null;
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();

                postGisVersion = (String)new NpgsqlCommand("SELECT postgis_version();", conn).ExecuteScalar();

                conn.Close();
            }

            return !String.IsNullOrEmpty(postGisVersion);
        }

        private static String getGeometryColumnName(string connectionString, String schemaName, String tableName)
        {
            String columnName = PostGisProviderStatic.DefaultGeometryColumnName;
            using (var cn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    cn.Open();

                    if ( !PostGisProviderStatic.Has_X_Privilege( cn, "table", "\"public\".\"geometry_columns\"", "SELECT" ) )
                        return null;
                    
                    var cmd = new NpgsqlCommand();
                    cmd.Connection = cn;
                    cmd.CommandText =
@"SELECT x.""f_geometry_column""
FROM ""public"".""geometry_columns"" AS x 
WHERE (x.""f_schema_name""=:p0 AND x.""f_table_name""=:p1)
LIMIT 1;";
                    cmd.Parameters.Add(new NpgsqlParameter("p0", schemaName));
                    cmd.Parameters.Add(new NpgsqlParameter("p1", tableName));

                    columnName = (String) cmd.ExecuteScalar();
                }
                catch
                {
                    columnName = "";
                }
            }

            return columnName;
        }

        private OgcGeometryType parseGeometryType(String geometryString)
        {
            if (String.IsNullOrEmpty(geometryString))
                throw new PostGisException("Geometry type not specified!");

            switch (geometryString.ToUpper())
            {
                case "GEOMETRY":
                    return OgcGeometryType.Geometry;
                case "POINT":
                    return OgcGeometryType.Point;
                case "LINESTRING":
                    return OgcGeometryType.LineString;
                case "POLYGON":
                    return OgcGeometryType.Polygon;
                case "MULTIPOINT":
                    return OgcGeometryType.MultiPoint;
                case "MULTILINESTRING":
                    return OgcGeometryType.MultiLineString;
                case "MULTIPOLYGON":
                    return OgcGeometryType.MultiPolygon;
                case "GEOMETRYCOLLECTION":
                    return OgcGeometryType.GeometryCollection;
                default:
                    throw new PostGisException(string.Format("Invalid geometry type '{0}'", geometryString));
            }
        }

        public override IExtents GetExtents()
        {
            Double xmin = 0, ymin = 0, xmax = 0, ymax = 0;
            Boolean isDbNull = true;

            using ( var conn = (NpgsqlConnection)DbUtility.CreateConnection( ConnectionString ) )
            {
                var cmd = (NpgsqlCommand)DbUtility.CreateCommand();

                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = String.Format(
                    "SELECT " +
                    "st_xmin(x.ext) AS xmin, st_ymin(x.ext) AS ymin, " +
                    "st_xmax(x.ext) AS xmax, st_ymax(x.ext) AS ymax " +
                    "FROM " +
                    "(SELECT st_extent( \"{2}\" ) as ext FROM {0}.\"{1}\") as x;",
                    TableSchema, Table, GeometryColumn );
                cmd.CommandType = CommandType.Text;

                var r = (NpgsqlDataReader)cmd.ExecuteReader();

                while ( r.Read() )
                {
                    if ( !( r.IsDBNull( 0 ) || r.IsDBNull( 1 ) || r.IsDBNull( 2 ) || r.IsDBNull( 3 ) ) )
                    {
                        xmin = r.GetDouble( 0 );
                        ymin = r.GetDouble( 1 );
                        xmax = r.GetDouble( 2 );
                        ymax = r.GetDouble( 3 );
                        isDbNull = false;
                    }
                }

                conn.Close();

            }

            return isDbNull
                ?
                    GeometryFactory.CreateExtents()
                :
                    GeometryFactory.CreateExtents2D( xmin, ymin, xmax, ymax );

        }

        public override void Insert(IEnumerable<FeatureDataRow<TOid>> features)
        {
            base.Insert(features);
            Vacuum();
        }

        protected override DataTable BuildSchemaTable()
        {
            return BuildSchemaTable(false);
        }

        protected override DataTable BuildSchemaTable(Boolean withGeometryColumn)
        {
            DataTable dt = null;
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();

                using (
                    var cmd = new NpgsqlCommand(string.Format("SELECT * FROM {0} LIMIT 1;", QualifiedTableName), conn))
                {
                    var da = new NpgsqlDataAdapter(cmd);
                    var ds = new DataSet();
                    da.FillSchema(ds, SchemaType.Source);
                    dt = ds.Tables["Table"];
                }

                conn.Close();
            }

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Columns[i].ColumnName == GeometryColumn)
                    dt.Columns[i].DataType = typeof (Byte[]);
            }

            if (!withGeometryColumn)
                dt.Columns.Remove(GeometryColumn);

            return dt;
        }

        protected override ExpressionTreeToSqlCompilerBase<TOid> CreateSqlCompiler(Expression expression)
        {
            return new PostGisExpressionTreeToSqlCompiler<TOid>(this, expression);
        }

        public void Vacuum()
        {
            using (var cn = new NpgsqlConnection(ConnectionString))
            {
                try
                {
                    cn.Open();
                    new NpgsqlCommand(
                         String.Format( "VACUUM ANALYZE {0};", QualifiedTableName ),
                         cn ).ExecuteNonQuery();
                }
                finally
                {
                    cn.Close();
                }
            }
        }

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
                                                 Enumerable.ToArray(Processor.Select(
                                                                        GetProviderPropertyValue
                                                                            <OrderByCollectionExpression,
                                                                            CollectionExpression<OrderByExpression>>(
                                                                            properties,
                                                                            new CollectionExpression<OrderByExpression>(
                                                                                new OrderByExpression[] {})),
                                                                        o => o.ToString("\"{0}\""))));


                string orderByClause = string.IsNullOrEmpty(orderByCols) ? "" : " ORDER BY " + orderByCols;

                string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                               FormatColumnNames(true, true,
                                                                                 compiler.ProjectedColumns.Count > 0
                                                                                     ? compiler.ProjectedColumns
                                                                                     : SelectAllColumnNames())));

                sql = String.Format("\nSELECT {0}\nFROM {1}\n{2}\n{3} {4}\n{5};",
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
                using (var cn = new NpgsqlConnection(ConnectionString))
                {
                    cn.Open();
                    var cm = new NpgsqlCommand(String.Format("EXPLAIN ANALYZE {0}", sql), cn);
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
                                                    ExpressionTreeToSqlCompilerBase<TOid> compiler, int pageSize,
                                                    int pageNumber)
        {
            string orderByCols = String.Join(",",
                                             Enumerable.ToArray(Processor.Select(
                                                                    GetProviderPropertyValue
                                                                        <OrderByCollectionExpression,
                                                                        CollectionExpression<OrderByExpression>>(
                                                                        properties,
                                                                        new CollectionExpression<OrderByExpression>(
                                                                            new OrderByExpression[] {})),
                                                                    o => o.ToString("\"{0}\""))));


            orderByCols = string.IsNullOrEmpty(orderByCols) ? QualifyColumnName(OidColumn) : orderByCols;

            Int64 startRecord = (pageNumber*pageSize) + 1;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                           FormatColumnNames(true, true,
                                                                             compiler.ProjectedColumns.Count > 0
                                                                                 ? compiler.ProjectedColumns
                                                                                 : SelectAllColumnNames())));

            string sqlWhereClause = String.Format(@" WHERE (nextval('sharpmap') > {0})",
                                                  compiler.CreateParameter(startRecord).ParameterName); //,

            if (!String.IsNullOrEmpty(compiler.SqlWhereClause))
                sqlWhereClause += " AND " + compiler.SqlWhereClause;

            return string.Format(
                @"
DROP SEQUENCE IF EXISTS ""sharpmap"";
CREATE TEMPORARY SEQUENCE ""sharpmap"" INCREMENT BY 1;
SELECT {0}
FROM {1}
{2}
{3}
ORDER BY {4}
LIMIT {5};",
                mainQueryColumns,
                QualifiedTableName,
                compiler.SqlJoinClauses,
                sqlWhereClause,
                orderByCols,
                compiler.CreateParameter(pageSize).ParameterName);
        }

        public override IEnumerable<string> FormatColumnNames(bool formatGeometryColumn,
                                                              bool qualifyColumnNames, IEnumerable<string> names)
        {
            foreach (string col in names)
            {
                yield return
                    formatGeometryColumn &&
                    String.Equals(col, GeometryColumn, StringComparison.InvariantCultureIgnoreCase)
                        ? String.Format(GeometryColumnConversionFormatString + " AS \"{1}\"",
                                        qualifyColumnNames ? QualifyColumnName(col) : col, GeometryColumn)
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

        #region Private helpers for Insert and Update

        protected override string InsertClause(IDbCommand cmd)
        {
            var sets = new List<string>();

            //Columnnames
            DataColumnCollection dcc = GetSchemaTable().Columns;
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
            //cmd.Parameters.Add(DbDbUtility.CreateParameterByType("PGeo", toDbType(typeof(byte[])), ParameterDirection.Input));
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("PGeo", ParameterDirection.Input));
            return String.Format("{0} VALUES({1})", columNames, string.Join(",", sets.ToArray()).Trim());
        }

        protected override string UpdateClause(IDbCommand cmd)
        {
            var sets = new List<string>();
            //Attribute
            foreach (DataColumn dc in GetSchemaTable().Columns)
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

        protected override void ReadSpatialReference(out ICoordinateSystem cs, out string srid)
        {
            using ( var conn = (NpgsqlConnection)DbUtility.CreateConnection( ConnectionString ) )
            {
                conn.Open();
                var cmd = (NpgsqlCommand)DbUtility.CreateCommand();

                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText =
//@"SELECT y.""auth_name"" || $L$:$L$ || y.""auth_srid"" FROM ""public"".""spatial_ref_sys"" AS y
@"SELECT y.""srtext"" FROM ""public"".""spatial_ref_sys"" AS y
INNER JOIN ""public"".""geometry_columns"" as x ON x.""srid""=y.""srid""
WHERE (x.""f_table_schema""=:p0 AND x.""f_table_name""=:p1 AND x.""f_geometry_column""=:p2)
LIMIT 1;";

                cmd.Parameters.Add( DbUtility.CreateParameter( "p0", TableSchema, ParameterDirection.Input ) );
                cmd.Parameters.Add( DbUtility.CreateParameter( "p1", Table, ParameterDirection.Input ) );
                cmd.Parameters.Add( DbUtility.CreateParameter( "p2", GeometryColumn, ParameterDirection.Input ) );

                object result = cmd.ExecuteScalar();
                if ( result is string )
                {
                    string ssrid = (string)result;
                    cs = SridMap.DefaultInstance.Process( ssrid, (ICoordinateSystem)null );
                    srid = !Equals( cs, default( ICoordinateSystem ) ) ? SridMap.DefaultInstance.Process( cs, "" ) : "";
                    return;
                }
                //close connection
                conn.Close();

            }
            cs = default( ICoordinateSystem );
            srid = "";
            
        }
    }
}