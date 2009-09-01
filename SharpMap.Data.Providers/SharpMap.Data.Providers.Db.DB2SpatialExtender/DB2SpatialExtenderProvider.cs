//#define EXPLAIN
/*
 *  The attached / following is part of SharpMap.Data.Providers.DB2_SpatialExtender
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
 *  - DB2 .NET Data Provider from IBM
 *    http://www-01.ibm.com/software/data/db2/windows/dotnet.html
 *    
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using GeoAPI.CoordinateSystems;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using IBM.Data.DB2;
using IBM.Data.DB2Types;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Expressions;
using SharpMap.Utilities;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else

#endif

namespace SharpMap.Data.Providers
{
    public class DB2SpatialExtenderProvider<TOid>
        : SpatialDbProviderBase<TOid>, ISpatialDbProvider<DB2SpatialExtenderDbUtility>
    {
        #region Static Properties

        static DB2SpatialExtenderProvider()
        {
            AddDerivedProperties(typeof (DB2SpatialExtenderProvider<TOid>));
        }

        #endregion

        #region DB2_SpatialExtender_IndexTypes enum

        /// <summary>
        /// 
        /// </summary>
        public enum DB2_SpatialExtender_IndexTypes
        {
            None = 0,
            PlanarRasterIndex = 1,
            GeodeticVoronoiIndex = 2 // requires DB2 Enterprise Server
        }

        #endregion

        /// <summary>
        /// Spatialite tables only accept geometries specified for the geometry column
        /// Look for entry in geometry_columns table of sqlite-db file
        /// </summary>
        private readonly OgcGeometryType _validGeometryType = OgcGeometryType.Geometry;


        private Int32 _dataPageNumber;
        private Int32 _dataPageSize;

        private Int32 _db2SrsId = -1;
        private String _db2SrsName = "DEFAULT_SRS";

        public DB2SpatialExtenderProvider(IGeometryFactory geometryFactory, string connectionString, string tableName)
            : this(
                geometryFactory, connectionString, DB2SpatialExtenderProviderStatic.DefaultSpatialSchema, tableName,
                "OID",
                getGeometryColumnName(connectionString, DB2SpatialExtenderProviderStatic.DefaultSpatialSchema, tableName)
                )
        {
        }

        public DB2SpatialExtenderProvider(IGeometryFactory geometryFactory, string connectionString,
                                          string tableSchema, string tableName, string oidColumn, string geometryColumn)
            : base(
                new DB2SpatialExtenderDbUtility(), geometryFactory, connectionString, tableSchema,
                tableName,
                oidColumn,
                geometryColumn)
        {
            using (DB2Connection cn = new DB2Connection(connectionString))
            {
                cn.Open();
                cn.CacheData = true;
                try
                {
                    String selectClause =
                        string.Format(
                            @"
SELECT SGC.TYPE_NAME, SGC.SRS_ID
    FROM {0}.ST_GEOMETRY_COLUMNS AS SGC
    WHERE (SGC.TABLE_SCHEMA='{1}' AND SGC.TABLE_NAME='{2}' AND SGC.COLUMN_NAME='{3}')",
                            DB2SpatialExtenderProviderStatic.DefaultSpatialSchema,
                            tableSchema,
                            tableName,
                            geometryColumn);

                    DB2DataReader dr = new DB2Command(selectClause, cn).ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        //valid geometry type
                        _validGeometryType = parseGeometryType(dr.GetString(0));

                        //SrsId
                        DB2SrsId = dr.GetDB2Int32(1).Value;
                        if (!OriginalSpatialReference.EqualParams(GeometryFactory.SpatialReference))
                            GeometryFactory.SpatialReference = OriginalSpatialReference;
                        //OriginalSrid = dr[1].ToString();
                        if (geometryFactory.Srid == null)
                            geometryFactory.Srid = OriginalSrid;
                        else
                        {
                            //geometryFactory.SpatialReference
                        }
                    }
                    else
                    {
                        dr.Close();
                        selectClause =
                            string.Format(@"
SELECT {0}.ST_SRID({1})
    FROM {2} AS {3} 
    WHERE ({4}=MIN({4}));",
                                          DB2SpatialExtenderProviderStatic.DefaultSpatialSchema,
                                          QualifyColumnName(geometryColumn),
                                          QualifyTableName(tableSchema, tableName),
                                          tableName,
                                          QualifyColumnName(oidColumn));

                        OriginalSrid = ((Int32) new DB2Command(selectClause, cn).ExecuteScalar()).ToString();
                        _validGeometryType = OgcGeometryType.Geometry;
                    }
                }
                catch (Exception)
                {
                    _validGeometryType = OgcGeometryType.Unknown;
                }
            }
        }

        public Int32 DB2SrsId
        {
            get
            {
                if (_db2SrsId < 0)
                {
                    if (!String.IsNullOrEmpty(OriginalSrid))
                    {
                        using (DB2Connection cn = new DB2Connection(ConnectionString))
                        {
                            cn.Open();
                            DB2DataReader dr = new DB2Command(String.Format(
                                                                  @"SELECT SRS.SRS_ID, SRS.SRS_NAME
FROM DB2GSE.ST_SPATIAL_REFERENCE_SYSTEMS AS SRS
WHERE SRS.ORGANIZATION = {0} AND SRS.ORGANIZATION_COORDSYS_ID = {1};",
                                                                  SpatialReference.Authority,
                                                                  SpatialReference.AuthorityCode), cn).ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                _db2SrsId = dr.GetInt32(0);
                                _db2SrsName = dr.GetString(1);
                            }
                        }
                    }
                }

                return _db2SrsId > 0 ? _db2SrsId : 0;
            }

            private set
            {
                using (DB2Connection cn = new DB2Connection(ConnectionString))
                {
                    cn.Open();
                    DB2DataReader dr = new DB2Command(String.Format(
                                                          @"SELECT SRS.ORGANIZATION, SRS.ORGANIZATION_COORDSYS_ID, SRS.DEFINITION
   FROM {0}.ST_SPATIAL_REFERENCE_SYSTEMS AS SRS
   WHERE SRS.SRS_ID = {1};",
                                                          DB2SpatialExtenderProviderStatic.DefaultSpatialSchema,
                                                          value), cn).ExecuteReader(CommandBehavior.CloseConnection);
                    if (dr.HasRows)
                    {
                        dr.Read();

                        GeometryServices gserv = new GeometryServices();
                        if (!dr.IsDBNull(2))
                        {
                            OriginalSpatialReference = gserv.CoordinateSystemFactory.CreateFromWkt(dr.GetString(2));
                        }
                        else if (!(dr.IsDBNull(0) || dr.IsDBNull(1)))
                        {
                            OriginalSpatialReference = new GeometryServices()
                                [string.Format("{0}:{1}", dr.GetString(0).ToUpper(), dr.GetInt32(1))].SpatialReference;
                        }
                        GeometryFactory = gserv[OriginalSrid];

                        _db2SrsId = value;
                    }
                    else
                        throw new ArgumentException("value");
                }
            }
        }

        public String DB2SrsName
        {
            get { return _db2SrsId > -1 ? _db2SrsName : "DEFAULT_SRS"; }
        }

        public override string GeometryColumnConversionFormatString
        {
            get
            {
                return String.Format("{0}.ST_ASBINARY({1})",
                                     DB2SpatialExtenderProviderStatic.DefaultSpatialSchema, "{0}");
            }
        }

        public override string GeomFromWkbFormatString
        {
            get
            {
                //return string.Format("\"{2}\".ST_GEOMFROMWKB({0}, {1})", 
                //"{0}", 
                //Srid == null ? DB2SpatialExtenderProviderStatic.DefaultSrid : Srid, 
                //TableSchema);
                return string.Format("{2}.ST_GEOMFROMWKB(CAST({0} AS BLOB(2G)), CAST({1} AS INTEGER))",
                                     "{0}",
                                     DB2SrsId,
                                     DB2SpatialExtenderProviderStatic.DefaultSpatialSchema);
            }
        }

        public override string QualifiedTableName
        {
            get
            {
                if (String.IsNullOrEmpty(Table))
                    throw new DB2SpatialExtenderException("Table name mustnot be null or emtpy");

                if (String.IsNullOrEmpty(TableSchema))
                    return String.Format("\"{0}\"", Table);
                else
                    return String.Format("\"{0}\".\"{1}\"", TableSchema, Table);
            }
        }

        #region ISpatialDbProvider<DB2SpatialExtenderDbUtility> Members

        public new DB2SpatialExtenderDbUtility DbUtility
        {
            get { return (DB2SpatialExtenderDbUtility) base.DbUtility; }
        }

        #endregion

        private static String getGeometryColumnName(string connectionString, String schemaName, String tableName)
        {
            String columnName = DB2SpatialExtenderProviderStatic.DefaultGeometryColumnName;
            using (DB2Connection cn = new DB2Connection(connectionString))
            {
                cn.Open();
                try
                {
                    columnName = (String) new DB2Command(
                                              String.Format(
                                                  @"
SELECT COLS.COLUMN_NAME 
    FROM {0}.ST_GEOMETRY_COLUMNS AS COLS
    WHERE (COLS.TABLE_SCHEMA = '{1}' AND COLS.TABLE_NAME = '{2}');",
                                                  DB2SpatialExtenderProviderStatic.DefaultSpatialSchema,
                                                  schemaName,
                                                  tableName),
                                              cn).ExecuteScalar();
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
                throw new DB2SpatialExtenderException("Geometry type not specified!");

            switch (geometryString.ToUpper())
            {
                case "ST_GEOMETRY":
                    return OgcGeometryType.Geometry;
                case "ST_POINT":
                    return OgcGeometryType.Point;
                case "ST_LINESTRING":
                    return OgcGeometryType.LineString;
                case "ST_POLYGON":
                    return OgcGeometryType.Polygon;
                case "ST_MULTIPOINT":
                    return OgcGeometryType.MultiPoint;
                case "ST_MULTILINESTRING":
                    return OgcGeometryType.MultiLineString;
                case "ST_MULTIPOLYGON":
                    return OgcGeometryType.MultiPolygon;
                case "ST_GEOMETRYCOLLECTION":
                    return OgcGeometryType.GeometryCollection;
                default:
                    throw new DB2SpatialExtenderException(string.Format("Invalid geometry type '{0}'", geometryString));
            }
        }

        public override IExtents GetExtents()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                conn.Open();
                cmd.Connection = conn;

                cmd.CommandText = String.Format(
                    "SELECT " +
                    "min({0}.ST_MINX(tbl.\"{3}\")) AS xmin, min({0}.ST_MINY(tbl.\"{3}\")) AS ymin, " +
                    "max({0}.ST_MAXX(tbl.\"{3}\")) AS xmax, max({0}.ST_MAXY(tbl.\"{3}\")) AS ymax " +
                    "FROM \"{1}\".\"{2}\" AS tbl;",
                    DB2SpatialExtenderProviderStatic.DefaultSpatialSchema,
                    TableSchema,
                    Table,
                    GeometryColumn);
                cmd.CommandType = CommandType.Text;

                using (DB2DataReader r = (DB2DataReader) cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        if (r.IsDBNull(0) || r.IsDBNull(1) || r.IsDBNull(2) || r.IsDBNull(3))
                            return GeometryFactory.CreateExtents();

                        double xmin = r.GetDouble(0);
                        double ymin = r.GetDouble(1);
                        double xmax = r.GetDouble(2);
                        double ymax = r.GetDouble(3);

                        IExtents ext = GeometryFactory.CreateExtents2D(
                            r.GetDouble(0), r.GetDouble(1),
                            r.GetDouble(2), r.GetDouble(3));
                        return ext;
                    }
                }
                return GeometryFactory.CreateExtents();
            }
        }

        protected override DataTable BuildSchemaTable()
        {
            return BuildSchemaTable(false);
        }

        protected override DataTable BuildSchemaTable(Boolean withGeometryColumn)
        {
            DataTable dt = null;
            using (DB2Connection conn = new DB2Connection(ConnectionString))
            {
                conn.Open();
                conn.CacheData = true;

                using (DB2Command cmd = new DB2Command(string.Format("SELECT * FROM {0};", QualifiedTableName), conn))
                {
                    DB2DataAdapter da = new DB2DataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.FillSchema(ds, SchemaType.Source);
                    dt = ds.Tables["Table"];
                }

                if (!withGeometryColumn)
                {
                    dt.Columns.Remove(GeometryColumn);
                }
            }
            return dt;
        }

        protected override ExpressionTreeToSqlCompilerBase<TOid> CreateSqlCompiler(Expression expression)
        {
            return new DB2SpatialExtenderExpressionTreeToSqlCompiler<TOid>(this, expression);
        }

        public virtual void Insert(IEnumerable<FeatureDataRow> features)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                {
                    using (IDbTransaction tran = conn.BeginTransaction())
                    {
                        IDbCommand cmd = DbUtility.CreateCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = tran;

                        cmd.CommandText = string.Format(
                            "INSERT INTO {0} {1};", QualifiedTableName, InsertClause(cmd));

                        String cmdText = cmd.CommandText;

                        foreach (FeatureDataRow row in features)
                        {
                            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
                                ((IDataParameter) cmd.Parameters[i]).Value = row[i];

                            DB2Blob blob = new DB2Blob(row.Geometry.AsBinary());
                            ((IDataParameter) cmd.Parameters["@PGeo"]).Value = blob; //val; //row.Geometry.AsBinary();

                            if (row.Geometry.GeometryType == _validGeometryType
                                ||
                                _validGeometryType == OgcGeometryType.Geometry)
                                cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                }
                //new DB2Command(String.Format("VACUUM ANALYZE {0};", QualifiedTableName), (DB2Connection)conn).ExecuteNonQuery();
            }
        }

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                                    ExpressionTreeToSqlCompilerBase<TOid> compiler)
        {
            _dataPageNumber = GetProviderPropertyValue<DataPageNumberExpression, int>(properties, -1);
            _dataPageSize = GetProviderPropertyValue<DataPageSizeExpression, int>(properties, 0);

            string sql = "";

            //string orderByCols = String.Join(",",
            //                                 Enumerable.ToArray(Processor.Select(
            //                                                        GetProviderPropertyValue
            //                                                            <OrderByCollectionExpression,
            //                                                            CollectionExpression<OrderByExpression>>(
            //                                                            properties,
            //                                                            new CollectionExpression<OrderByExpression>(
            //                                                                new OrderByExpression[] {})),
            //                                                        delegate(OrderByExpression o) { return o.ToString("\"{0}\""); })));


            string orderByClause = string.IsNullOrEmpty(compiler.OrderByClause) ? "" : " ORDER BY " + compiler.OrderByClause;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                           FormatColumnNames(true, true,
                                                                             compiler.ProjectedColumns.Count > 0
                                                                                 ? compiler.ProjectedColumns
                                                                                 : SelectAllColumnNames())));

            sql = String.Format("\nSELECT {0}\nFROM {1}\n{2}\n{3} {4}\n{5};",
                                mainQueryColumns,
                                QualifiedTableName + " AS \"" + Table + "\"",
                                compiler.SqlJoinClauses,
                                string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                compiler.SqlWhereClause,
                                orderByClause);
            //}
#if DEBUG && EXPLAIN
            if (sql.StartsWith("SELECT"))
            {
                using (DB2Connection cn = new DB2Connection(ConnectionString))
                {
                    cn.Open();
                    DB2Command cm = new DB2Command(String.Format("EXPLAIN ANALYZE {0}", sql), cn);
                    foreach (IDataParameter par in compiler.ParameterCache.Values)
                        cm.Parameters.Add(par);

                    Debug.WriteLine("");
                    DB2DataReader dr = cm.ExecuteReader();
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
            return "";
            ///throw new NotImplementedException();
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

        protected override IFeatureDataReader ExecuteFeatureDataReader(IDbCommand cmd)
        {
            Debug.WriteLine(String.Format("executing sql : {0}", cmd.CommandText));
            IDbConnection conn = DbUtility.CreateConnection(ConnectionString);
            cmd.Connection = conn;
            if (conn.State == ConnectionState.Closed) conn.Open();

            DB2DataReader dr;
            if (_dataPageSize == 0 || _dataPageNumber == -1)
                dr = ((DB2Command) cmd).ExecuteReader(CommandBehavior.CloseConnection);
            else
                dr = ((DB2Command) cmd).ExecutePageReader((_dataPageNumber)*_dataPageSize, _dataPageSize);

            return new DB2SpatialExtenderFeatureDataReader(GeometryFactory, dr, GeometryColumn, OidColumn)
                       {CoordinateTransformation = CoordinateTransformation};
        }

        protected override void ReadSpatialReference(out ICoordinateSystem cs, out string srid)
        {
            throw new NotImplementedException();
        }

        #region Private helpers for Insert and Update

        protected override string InsertClause(IDbCommand cmd)
        {
            List<string> sets = new List<string>();

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
                sets.Add(string.Format("{0}", ParamForColumn(dc, out param)));

                DB2Parameter par = param as DB2Parameter;
                switch (par.DB2Type)
                {
                    case DB2Type.VarChar:
                        sets[sets.Count - 1] = string.Format("CAST({0} AS VARCHAR(250))", par.ParameterName);
                        break;
                    default:
                        break;
                }
                cmd.Parameters.Add(param);
            }

            //Geometry
            sets.Add(string.Format(GeomFromWkbFormatString, "@PGeo"));
            //    string.Format("CAST(@PGeo AS {0}.{1})", 
            //        TableSchema, 
            //        DB2SpatialExtenderProviderStatic.ToDb2GeometryType(_validGeometryType))
            //)));

            //cmd.Parameters.Add(DbUtility.CreateParameterByType("PGeo", toDbType(typeof(byte[])), ParameterDirection.Input));
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("@PGeo", ParameterDirection.Input));
            return String.Format("{0} VALUES({1})", columNames, string.Join(",", sets.ToArray()).Trim());
        }

        protected override string UpdateClause(IDbCommand cmd)
        {
            List<string> sets = new List<string>();
            //Attribute
            foreach (DataColumn dc in GetSchemaTable().Columns)
            {
                IDataParameter param = null;
                sets.Add(string.Format(" \"{0}\"={1}", dc.ColumnName, ParamForColumn(dc, out param)));
                cmd.Parameters.Add(param);
            }
            //Geometry
            sets.Add(
                string.Format("\"{0}\"={1}",
                              GeometryColumn,
                              string.Format(GeomFromWkbFormatString, "@PGeo")));
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("@PGeo", ParameterDirection.Input));

            return String.Join(",", sets.ToArray()).Trim();
        }

        protected override string WhereClause(IDbCommand cmd)
        {
            cmd.Parameters.Add(DbUtility.CreateParameter<UInt32>("@POldOid", ParameterDirection.Input));
            return string.Format("{0}=@POldOid", OidColumn);
        }

        protected override String ParamForColumn(DataColumn dc, out IDataParameter param)
        {
            String paramName = ParamForColumn(dc);
            param = DbUtility.CreateParameter(paramName, dc.DataType, ParameterDirection.Input);
            return paramName;
        }

        protected override String ParamForColumn(DataColumn dc)
        {
            return string.Format("@P{0}", dc.Ordinal);
        }

        #endregion
    }
}