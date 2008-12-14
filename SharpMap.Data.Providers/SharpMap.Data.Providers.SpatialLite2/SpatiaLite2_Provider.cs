#define EXPLAIN
/*
 *  The attached / following is part of SharpMap.Data.Providers.SpatiaLite2
 *  SharpMap.Data.Providers.SpatiaLite2 is free software � 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
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
 *  - SqLite, dedicated to public domain
 *    http://www.sqlite.org
 *  - SpatiaLite-2.2 by Alessandro Furieri released under a disjunctive tri-license:
 *    - 'Mozilla Public License, version 1.1 or later' OR
 *    - 'GNU General Public License, version 2.0 or later' 
 *    - 'GNU Lesser General Public License, version 2.1 or later' <--- this is the one
 *    http://www.gaia-gis.it/spatialite-2.2/index.html
 *    
 *  - SQLite ADO.NET 2.0 Provider by Robert Simpson, dedicated to public domain
 *    http://sourceforge.net/projects/sqlite-dotnet2
 *    
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using GeoAPI.Geometries;
using Proj4Utility;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.SpatiaLite2;
using SharpMap.Expressions;
#if DEBUG
using System.Diagnostics;
#endif

#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Processor = GeoAPI.DataStructures.Processor;
using Enumerable = GeoAPI.DataStructures.Enumerable;
using Caster = GeoAPI.DataStructures.Caster;
#endif

namespace SharpMap.Data.Providers
{

    /// <summary>
    /// Enumeration of spatial indices valid for SQLite
    /// </summary>
    public enum SpatiaLite2_IndexType
    {
        /// <summary>
        /// No valid spatial Index
        /// </summary>
        None = 0,

        /// <summary>
        /// RTree Index
        /// </summary>
        RTree = 1,

        /// <summary>
        /// In-Memory cache of Minimum Bounding Rectangles (MBR)
        /// </summary>
        MBRCache = 2
    }

    public class SpatiaLite2_Provider
        : SpatialDbProviderBase<Int64>, ISpatialDbProvider<SpatiaLite2_Utility>
    {

        #region Static Properties

        static SpatiaLite2_Provider()
        {
            //AddDerivedProperties(typeof(SpatialDbProviderBase<Int64>));
            AddDerivedProperties(typeof(SpatiaLite2_Provider));
        }
        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatiaLiteProvider2_2"/>'s <see cref="DefaultSpatiaLiteIndexType"/> property.
        /// </summary>
        public static PropertyDescriptor DefaultSpatiaLiteIndexTypeProperty
        {
            get { return ProviderStaticProperties.Find("DefaultSpatiaLiteIndexType", true); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatiaLiteProvider2_2"/>'s <see cref="DefaultSRID"/> property.
        /// </summary>
        public static PropertyDescriptor DefaultSRIDProperty
        {
            get { return ProviderStaticProperties.Find("DefaultSRID", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatiaLiteProvider2_2"/>'s <see cref="DefaultGeometryColumnName"/> property.
        /// </summary>
        public static PropertyDescriptor DefaultGeometryColumnNameProperty
        {
            get { return ProviderStaticProperties.Find("DefaultGeometryColumnName", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatiaLiteProvider2_2"/>'s <see cref="DefaultGeometryColumnName"/> property.
        /// </summary>
        public static PropertyDescriptor DefaultOIDColumnNameProperty
        {
            get { return ProviderStaticProperties.Find("DefaultOIDColumnNameProperty", false); }
        }

        #endregion
        /// <summary>
        /// SpatiaLite does not accept geometries without a valid SRID
        /// Used in SpatiaLite constructor if not SRID is specified.
        /// (e.g. 4326: 'WGS 84', '+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs')
        /// </summary>
        public static string DefaultSrid = "4326";

        /// <summary>
        /// Default name of the geometry column. Used in SpatiaLite constructor
        /// if no geometry column is specified.
        /// </summary>
        public static String DefaultGeometryColumnName = "XGeometryX";

        /// <summary>
        /// Default name of PrimaryKey column. Used in SpatialLite constructor
        /// if not primary key column name is specified.
        /// </summary>
        public static String DefaultOidColumnName = "OID";

        /// <summary>
        /// Default SpatiaLiteIndexType. Used in SpatialLite constructor
        /// if not spatial index type is specified.
        /// </summary>
        public static SpatiaLite2_IndexType DefaultSpatiaLiteIndexType = SpatiaLite2_IndexType.RTree;
        private SpatiaLite2_IndexType _spatialLiteIndexType;

        /// <summary>
        /// Spatialite tables only accept geometries specified for the geometry column
        /// Look for entry in geometry_columns table of sqlite-db file
        /// </summary>
        private OgcGeometryType _validGeometryType = OgcGeometryType.Geometry;

        /// <summary>
        /// Determines whether SqLite database file is spatially enabled
        /// </summary>
        /// <param name="connectionString">Connection String to access SqLite database file</param>
        /// <returns>
        /// <value>true</value> if it is,
        /// <value>false</value> if it isn't.
        /// </returns>
        static public Boolean IsSpatiallyEnabled(String connectionString)
        {
            Boolean result = false;
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                Int64 numTables =
                    (Int64)new SQLiteCommand(String.Format(
                        "SELECT COUNT(*) FROM [{0}].[sqlite_master] " +
                        "WHERE([tbl_name]='spatial_ref_sys' OR [tbl_name]='geometry_columns');", "main")
                        , conn).ExecuteScalar();

                if (numTables >= 2)
                {
                    Int64 numRefSys =
                        (Int64)new SQLiteCommand("SELECT COUNT(*) FROM spatial_ref_sys;", conn).ExecuteScalar();

                    result = (numRefSys > 0);
                }
            }
            return result;
        }

        public static DataSet GetSpatiallyEnabledTables(String connectionString)
        {
            DataSet ds = null;

            using (SQLiteConnection cn = new SQLiteConnection(connectionString))
            {
                cn.Open();
                createSQLiteInformationSchema(cn);

                SQLiteCommand cm = cn.CreateCommand();
                cm.CommandText = @"
SELECT	'main' AS [Schema],
        x.f_table_name AS [TableName], 
        x.f_geometry_column AS [GeometryColumn], 
        x.coord_dimension AS [Dimension], 
	    x.f_table_name || '.' || x.f_geometry_column || ' (' || x.type || ')' AS [Label],
        x.auth_name || ':' || x.auth_srid AS [SRID],
        y.srtext as [SpatialReference]
FROM [main].[geometry_columns] AS x LEFT JOIN [main].[spatial_ref_sys] as y on x.srid=y.srid;
TYPES [varchar], [varchar], [varchar], [bool];
SELECT  x.table_name as [TableName],
        x.column_name as [ColumnName],
        x.data_type as [DataType],
        -1 AS [Include],
        CASE
            WHEN (x.PK = 1) THEN -1
            ELSE 0
        END AS [PK]
FROM    information_schema_columns AS x 
	        LEFT JOIN (SELECT z.f_table_name FROM geometry_columns AS z) AS cte ON cte.f_table_name=x.table_name
	        LEFT JOIN geometry_columns AS y ON y.f_table_name=x.table_name
WHERE cte.f_table_name=x.table_name AND ( RTRIM(x.column_name) <> RTRIM(y.f_geometry_column))
ORDER BY [TableName], x.ordinal_position;";

                SQLiteDataAdapter da = new SQLiteDataAdapter(cm);
                ds = new DataSet();
                da.Fill(ds);
#if DEBUG
                Debug.Assert(ds.Tables.Count == 2);
#endif
            }
            return ds;
        }

        private static void createSQLiteInformationSchema(SQLiteConnection connection)
        {
            new SQLiteCommand(
@"CREATE TEMP TABLE information_schema_columns (
    table_name text, 
    ordinal_position integer,
    column_name text,
    data_type text,
    pk integer);", connection).ExecuteNonQuery();

            SQLiteCommand insert = new SQLiteCommand(
                "INSERT INTO information_schema_columns (table_name, ordinal_position, column_name, data_type, pk) VALUES(@P1, @P4, @P2, @P3, @P5);",
                connection);

            insert.Parameters.Add(new SQLiteParameter("@P1", DbType.String));
            insert.Parameters.Add(new SQLiteParameter("@P2", DbType.String));
            insert.Parameters.Add(new SQLiteParameter("@P3", DbType.String));
            insert.Parameters.Add(new SQLiteParameter("@P4", DbType.Int64));
            insert.Parameters.Add(new SQLiteParameter("@P5", DbType.Int64));


            //Pragma
            using (SQLiteConnection cn_pragma = (SQLiteConnection)connection.Clone())
            {
                SQLiteCommand pragma = new SQLiteCommand("PRAGMA table_info('@P1');", cn_pragma);
                //pragma.CommandType = CommandType.StoredProcedure;
                pragma.Parameters.Add("P1", DbType.String);

                //itereate throuhg sqlite_master
                using (SQLiteConnection cn_master = (SQLiteConnection)connection.Clone())
                {
                    //cn_master.Open();
                    string select = @"
SELECT tbl_name 
FROM sqlite_master 
WHERE type='table' AND NOT( name like 'cache_%' ) AND NOT( name like 'sqlite%' ) AND NOT( name like 'index_%' ) AND NOT (name in ('spatial_ref_sys', 'geometry_columns'));";
                    SQLiteDataReader dr = new SQLiteCommand(
                        select, cn_master
                        ).ExecuteReader(CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        //Get Column Info and ...
                        String tableName = dr.GetString(0).Trim();
                        pragma = new SQLiteCommand(string.Format("PRAGMA table_info('{0}');", tableName), cn_pragma); //.Parameters[0].Value = dr.GetString(0);
                        using (SQLiteDataReader drts = pragma.ExecuteReader())
                        {
                            while (drts.Read())
                            {
                                //... insert it into temp table
                                insert.Parameters[0].Value = tableName; ;
                                insert.Parameters[1].Value = drts.GetString(1);
                                insert.Parameters[2].Value = drts.GetString(2);
                                insert.Parameters[3].Value = drts.GetInt64(0);
                                insert.Parameters[4].Value = drts.GetInt64(5);
                                insert.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public SpatiaLite2_Provider(IGeometryFactory geometryFactory, string connectionString, string tableName)
            : this(geometryFactory, connectionString, "main", tableName, DefaultOidColumnName, DefaultGeometryColumnName)
        {
        }

        public SpatiaLite2_Provider(IGeometryFactory geometryFactory, string connectionString,
                                  string tableSchema, string tableName, string oidColumn, string geometryColumn)
            : base(
                    new SpatiaLite2_Utility(), geometryFactory, connectionString, tableSchema, tableName, oidColumn,
                    geometryColumn)
        {
            using (SQLiteConnection cn = new SQLiteConnection(connectionString))
            {
                cn.Open();
                try
                {
                    String selectClause = string.Format(
@"SELECT x.type, y.auth_name || ':' || y.auth_srid, x.spatial_index_enabled 
    FROM {0}.[geometry_columns] AS x
    LEFT JOIN {0}.[spatial_ref_sys] on x.srid = y.srid 
    WHERE (f_table_name='{1}' AND f_geometry_column='{2}')",
                      tableSchema, tableName, geometryColumn);
                    SQLiteDataReader dr = new SQLiteCommand(selectClause, cn).ExecuteReader();
                    if (dr.HasRows)
                    {

                        dr.Read();

                        //valid geometry type
                        _validGeometryType = parseGeometryType(dr.GetString(0));

                        //srid
                        if (String.Compare(OriginalSrid , dr.GetString(1), true) != 0)
                            throw new SpatiaLite2_Exception("Spatial Reference System does not match that of assigned geometry factory!");

                        //SpatiaLite Index
                        switch (dr.GetInt64(2))
                        {
                            case 0:
                                throw new SpatiaLite2_Exception("Spatial index type must not be 'None'");
                                //_spatialLiteIndexType = SpatiaLite2_IndexType.None;
                                //break;
                            case 1:
                                _spatialLiteIndexType = SpatiaLite2_IndexType.RTree;
                                break;
                            case 2:
                                _spatialLiteIndexType = SpatiaLite2_IndexType.MBRCache;
                                break;
                            default:
                                throw new SpatiaLite2_Exception("Unknown SpatiaLite index type.");
                        }
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
                throw new SpatiaLite2_Exception("Geometry type not specified!");

            switch (geometryString.ToUpper())
            {
                case "POINT": return OgcGeometryType.Point;
                case "LINESTRING": return OgcGeometryType.LineString;
                case "POLYGON": return OgcGeometryType.Polygon;
                case "MULTIPOINT": return OgcGeometryType.MultiPoint;
                case "MULTILINESTRING": return OgcGeometryType.MultiLineString;
                case "MULTIPOLYGON": return OgcGeometryType.MultiPolygon;
                case "GEOMETRYCOLLECTION": return OgcGeometryType.GeometryCollection;
                default:
                    throw new SpatiaLite2_Exception(string.Format("Invalid geometry type '{0}'", geometryString));
            }
        }

        public override string GeometryColumnConversionFormatString
        {
            get { return "AsBinary({0})"; }
        }

        public override string GeomFromWkbFormatString
        {
            get { return string.Format("GeomFromWKB({0},{1})", "{0}", Srid == null ? DefaultSrid : Srid); }
        }

        public override IExtents GetExtents()
        {
            //ensure spatial index
            if (SpatiaLiteIndexType == SpatiaLite2_IndexType.None)
                SpatiaLiteIndexType = DefaultSpatiaLiteIndexType;

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.Connection = conn;
                switch (SpatiaLiteIndexType)
                {
                    case SpatiaLite2_IndexType.RTree:
                        cmd.CommandText =
                        string.Format(
                            "SELECT MIN(xmin) as xmin, MIN(ymin) as ymin, MAX(xmax) as xmax, MAX(ymax) as ymax FROM idx_{0}_{1}",
                            Table, GeometryColumn);
                        break;

                    case SpatiaLite2_IndexType.MBRCache:
                        cmd.CommandText = string.Format(
                            "SELECT MIN(MbrMinX({0})) as xmin, MIN(MbrMinY({0})) as ymin, MAX(MbrMaxX({0})) as xmax, MAX(MbrMaxY({0})) as maxy from {1};",
                            GeometryColumn, QualifiedTableName);
                        break;
                }
                cmd.CommandType = CommandType.Text;
                Double xmin, ymin, xmax, ymax;

                using (IDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        if (r.IsDBNull(0) || r.IsDBNull(1) || r.IsDBNull(2) || r.IsDBNull(3))
                            return GeometryFactory.CreateExtents();

                        xmin = r.GetDouble(0);// - 0.000000000001;
                        ymin = r.GetDouble(1);// - 0.000000000001;
                        xmax = r.GetDouble(2);// + 0.000000000001;
                        ymax = r.GetDouble(3);// + 0.000000000001;
                        return GeometryFactory.CreateExtents2D(xmin, ymin, xmax, ymax);
                    }
                }
                return GeometryFactory.CreateExtents();
            }
        }

        public SpatiaLite2_IndexType SpatiaLiteIndexType
        {
            get
            {
                return _spatialLiteIndexType;
                //Int64 retVal = 0;
                //using (SQLiteConnection cn = (SQLiteConnection)DbUtility.CreateConnection(ConnectionString))
                //{
                //    //cn.Open();
                //    Object ret = new SQLiteCommand(string.Format("SELECT spatial_index_enabled FROM geometry_columns WHERE (f_table_name='{0}' AND f_geometry_column='{1}');", Table, GeometryColumn), cn).ExecuteScalar();
                //    if (ret != null) retVal = (long)ret;
                //}
                //switch(retVal)
                //{
                //    case 0: return SpatiaLite2_IndexType.None;
                //    case 1: return SpatiaLite2_IndexType.RTree;
                //    case 2: return SpatiaLite2_IndexType.MBRCache;
                //    default:
                //        throw new SpatiaLite2_Exception("Unknown spatial index type");
                //}
            }
            set
            {
                if (value == SpatiaLite2_IndexType.None) return;

                Object ret = 0;
                long retVal = 0;
                if (_spatialLiteIndexType != value)
                {
                    using (SQLiteConnection cn = (SQLiteConnection)DbUtility.CreateConnection(ConnectionString))
                    {
                        //First disable current spatial index
                        ret = new SQLiteCommand(string.Format("SELECT DisableSpatialIndex( '{0}', '{1}' )", Table, GeometryColumn), cn).ExecuteScalar();

                        if (value == SpatiaLite2_IndexType.RTree)
                            ret = new SQLiteCommand(string.Format("SELECT CreateSpatialIndex( '{0}', '{1}' );", Table, GeometryColumn), cn).ExecuteScalar();
                        if (value == SpatiaLite2_IndexType.MBRCache)
                            ret = new SQLiteCommand(string.Format("SELECT CreateMBRCache( '{0}', '{1}' );", Table, GeometryColumn), cn).ExecuteScalar();

                        System.Diagnostics.Debug.Assert(ret != null);
                        retVal = (long)ret;
                        System.Diagnostics.Debug.Assert(retVal == 1);
                    }
                    _spatialLiteIndexType = value;
                }
            }
        }

        protected override DataTable BuildSchemaTable()
        {
            return BuildSchemaTable(false);
        }

        protected override DataTable BuildSchemaTable(Boolean withGeometryColumn)
        {
            DataTable dt = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(string.Format("SELECT * FROM {0} ", Table), conn))
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    da.FillSchema(ds, SchemaType.Source);
                    dt = ds.Tables["Table"];
                }

                for (int i = 0; i < dt.Columns.Count; i++)
                {

                    if (dt.Columns[i].DataType == typeof(System.Object))
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

        protected override ExpressionTreeToSqlCompilerBase<Int64> CreateSqlCompiler(Expression expression)
        {
            return new SpatiaLite2_ExpressionTreeToSqlCompiler(this,
                                                               expression,
                                                               _spatialLiteIndexType);
        }

        public void Vacuum()
        {
            using (SQLiteConnection cn = new SQLiteConnection(this.ConnectionString))
            {
                try
                {
                    new SQLiteCommand("SELECT VACUUM;", cn).ExecuteNonQuery();
                }
                finally
                { }
            }
        }



        //public override OgcGeometryType ValidGeometryType
        //{
        //    get { return _validGeometryType; }
        //    protected set
        //    {
        //        if (_validGeometryType == OgcGeometryType.Unknown)
        //            _validGeometryType = value;
        //    }
        //}

        private static SQLiteConnection initSpatialMetaData(String connectionString)
        {
            //Test whether database is spatially enabled
            if (IsSpatiallyEnabled(connectionString)) return new SQLiteConnection(connectionString);

            SQLiteConnection conn = new SQLiteConnection(connectionString);

            conn.Open();
            
            Object retVal = new SQLiteCommand("SELECT load_extension('libspatialite-2.dll');;", conn).ExecuteScalar();
            SQLiteTransaction tran = conn.BeginTransaction();

            new SQLiteCommand("SELECT InitSpatialMetaData();", conn).ExecuteNonQuery();

            new SQLiteCommand("ALTER TABLE spatial_ref_sys ADD COLUMN srtext text;", conn).ExecuteNonQuery();

            SQLiteCommand cmd = new SQLiteCommand(
              "INSERT OR REPLACE INTO spatial_ref_sys (srid, auth_name, auth_srid, ref_sys_name, proj4text, srtext) VALUES (@P1, @P2, @P3, @P4, @P5, @P6);", conn);
            cmd.Parameters.Add(new SQLiteParameter("@P1", DbType.Int64));
            cmd.Parameters.Add(new SQLiteParameter("@P2", DbType.String));
            cmd.Parameters.Add(new SQLiteParameter("@P3", DbType.Int64));
            cmd.Parameters.Add(new SQLiteParameter("@P4", DbType.String));
            cmd.Parameters.Add(new SQLiteParameter("@P5", DbType.String));
            cmd.Parameters.Add(new SQLiteParameter("@P6", DbType.String));

            SQLiteParameterCollection pars = cmd.Parameters;
            foreach (Proj4Reader.Proj4SpatialRefSys p4srs in Proj4Reader.GetSRIDs())
            {
                pars[0].Value = p4srs.Srid;
                pars[1].Value = p4srs.AuthorityName;
                pars[2].Value = p4srs.AuthoritySrid;
                pars[3].Value = p4srs.RefSysName;
                pars[4].Value = p4srs.Proj4Text;
                pars[5].Value = p4srs.SrText;

                cmd.ExecuteNonQuery();
            }

            tran.Commit();

            return conn;

        }

        public static void CreateDataTable(SharpMap.Data.FeatureDataTable featureDataTable, String connectionString)
        {
            CreateDataTable(featureDataTable, featureDataTable.TableName, connectionString);
        }

        public static void CreateDataTable(SharpMap.Data.FeatureDataTable featureDataTable, String tableName, String connectionString)
        {
            CreateDataTable(featureDataTable, tableName, connectionString, DefaultGeometryColumnName, DefaultSpatiaLiteIndexType);
        }

        public static void CreateDataTable(
            SharpMap.Data.FeatureDataTable featureDataTable,
            String tableName,
            String connectionString,
            String geometryColumnName,
            SpatiaLite2_IndexType spatialIndexType)
        {
            if (spatialIndexType == SpatiaLite2_IndexType.None)
                spatialIndexType = DefaultSpatiaLiteIndexType;

            OgcGeometryType geometryType;

            SQLiteConnection conn = initSpatialMetaData(connectionString);
            string srid = featureDataTable.GeometryFactory.Srid == null ? DefaultSrid : featureDataTable.GeometryFactory.Srid;
            if (conn != null)
            {

                string createTableClause = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});", tableName, ColumnsClause(featureDataTable.Columns, featureDataTable.Constraints));
                new SQLiteCommand(createTableClause, conn).ExecuteNonQuery();

                geometryType = featureDataTable[0].Geometry.GeometryType;
                //geometryType = OgcGeometryType.Geometry;

                String addGeometryColumnClause = String.Format("('{0}', '{1}', {2}, '{3}', {4})",
                  tableName,
                  geometryColumnName,
                  srid,
                  geometryType.ToString(),
                  2);

                if ((Int64)new SQLiteCommand(String.Format("SELECT RecoverGeometryColumn {0}", addGeometryColumnClause), conn).ExecuteScalar() == (Int64)0)
                    if ((Int64)new SQLiteCommand(String.Format("SELECT AddGeometryColumn {0};", addGeometryColumnClause), conn).ExecuteScalar() == (Int64)0)
                        throw new SpatiaLite2_Exception(string.Format("Cannot create geometry column with type of '{0}'", geometryType.ToString()));

                switch (spatialIndexType)
                {
                    case SpatiaLite2_IndexType.RTree:
                        if (new SQLiteCommand(String.Format("SELECT CreateSpatialIndex('{0}','{1}');",
                            tableName, geometryColumnName), conn).ExecuteScalar() == (object)0) throw new SpatiaLite2_Exception("Could not create RTree index");
                        break;

                    case SpatiaLite2_IndexType.MBRCache:
                        if (new SQLiteCommand(String.Format("SELECT CreateMbrCache('{0}','{1}');",
                            tableName, geometryColumnName), conn).ExecuteScalar() == (object)0) throw new SpatiaLite2_Exception("Could not create MbrCache");
                        break;
                }
            }
            conn.Close();
            conn = null;

            SpatiaLite2_Provider prov = new SpatiaLite2_Provider(
              featureDataTable.GeometryFactory, connectionString, "main", tableName,
              featureDataTable.Columns[0].ColumnName, geometryColumnName);
            prov.Insert(featureDataTable);

            return;

        }

        public override void Insert(IEnumerable<FeatureDataRow> features)
        {
            OgcGeometryType geometryType = OgcGeometryType.Unknown;

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

                        foreach (FeatureDataRow row in features)
                        {
                            for (int i = 0; i < cmd.Parameters.Count - 1; i++)
                                ((IDataParameter)cmd.Parameters[i]).Value = row[i];

                            ((IDataParameter)cmd.Parameters["@PGeo"]).Value = row.Geometry.AsBinary();
                            if (geometryType == OgcGeometryType.Unknown)
                                geometryType = row.Geometry.GeometryType;

                            if (row.Geometry.GeometryType == geometryType)
                                cmd.ExecuteNonQuery();
                        }
                        tran.Commit();
                    }
                }
            }
        }

        public override void Update(IEnumerable<FeatureDataRow> features)
        {
            //Update(features, "UPDATE OR IGNORE");
        }

        private static String ColumnsClause(DataColumnCollection dcc, ConstraintCollection ccc)
        {
            String[] columns = new String[dcc.Count];

            Int32 index = 0;
            foreach (DataColumn dc in dcc)
            {
                columns[index++] = string.Format(" [{0}] {1}", dc.ColumnName, SpatiaLite2_Utility.GetTypeString(dc.DataType));
            }
            index = 0;

            String[] constraints = new String[ccc.Count];
            foreach (Constraint c in ccc)
            {
                UniqueConstraint uc = c as UniqueConstraint;
                if (uc != null)
                {
                    if (uc.IsPrimaryKey)
                    {
                        constraints[index++] = String.Format(", PRIMARY KEY ({0}) ON CONFLICT IGNORE", ColumnNamesToCommaSeparatedString(uc.Columns));
                    }
                    else
                    {
                        constraints[index++] = String.Format(", UNIQUE ({0}) ON CONFLICT IGNORE", ColumnNamesToCommaSeparatedString(uc.Columns));
                    }
                }
                //Other Constraints are not supported by SqLite
            }

            String constraintsClause = "";
            if (index > 0)
            {
                Array.Resize<String>(ref constraints, index);
                constraintsClause = String.Join(String.Empty, constraints);
            }
            return String.Join(",", columns) + constraintsClause;

        }

        static String OrdinalsToCommaSeparatedString(IEnumerable<DataColumn> dcc)
        {
            return OrdinalsToCommaSeparatedString(String.Empty, dcc);
        }

        static String OrdinalsToCommaSeparatedString(String prefix, IEnumerable dcc)
        {
            String ret = "";
            foreach (DataColumn t in dcc)
                ret += String.Format(", {0}{1}", prefix, t.Ordinal);

            if (ret.Length > 0)
                ret = ret.Substring(2);

            return ret;
        }

        static String ColumnNamesToCommaSeparatedString(IEnumerable<DataColumn> dcc)
        {
            return ColumnNamesToCommaSeparatedString(String.Empty, dcc);
        }

        static String ColumnNamesToCommaSeparatedString(String prefix, IEnumerable<DataColumn> dcc)
        {
            String ret = "";
            foreach (DataColumn t in dcc)
                ret += String.Format(", [{0}]", t.ColumnName);

            if (ret.Length > 0)
                ret = ret.Substring(2);

            return ret;
        }

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties, ExpressionTreeToSqlCompilerBase<long> compiler)
        {
            int pageNumber = GetProviderPropertyValue<DataPageNumberExpression, int>(properties, -1);
            int pageSize = GetProviderPropertyValue<DataPageSizeExpression, int>(properties, 0);

            string sql = "";
            if (pageSize > 0 && pageNumber > -1)
                sql = GenerateSelectSql(properties, compiler, pageSize, pageNumber);
            else
            {

                string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                               FormatColumnNames(true, true,
                                                                                 compiler.ProjectedColumns.Count > 0
                                                                                     ? compiler.ProjectedColumns
                                                                                     : SelectAllColumnNames())));

                string orderByCols = String.Join(",",
                                                 Enumerable.ToArray(Processor.Select(
                                                     GetProviderPropertyValue<OrderByCollectionExpression, CollectionExpression<OrderByExpression>>(
                                                         properties, new CollectionExpression<OrderByExpression>(new OrderByExpression[] { })), o => o.ToString("[{0}]"))));

                string orderByClause = string.IsNullOrEmpty(orderByCols) ? "" : " ORDER BY " + orderByCols;

                sql =
                 String.Format("SELECT {0} FROM {1} {2} {3} {4} {5}",
                                           mainQueryColumns,
                                           Table, //QualifiedTableName,
                                           compiler.SqlJoinClauses,
                                           String.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                     compiler.SqlWhereClause,
                                     orderByClause);
            }
#if DEBUG && EXPLAIN
            using (SQLiteConnection cn = (SQLiteConnection)DbUtility.CreateConnection(ConnectionString))
            {
                if (cn.State == ConnectionState.Closed) cn.Open();
                SQLiteCommand cm = new SQLiteCommand(String.Format("EXPLAIN {0}", sql), cn);
                foreach (IDataParameter par in compiler.ParameterCache.Values)
                    cm.Parameters.Add(par);

                Debug.WriteLine("");
                SQLiteDataReader dr = cm.ExecuteReader();
                if (dr.HasRows)
                    while (dr.Read())
                    {
                        Debug.WriteLine(String.Format("{0}; {1}; {2}; {3}; {4}; {5}; {6}; {7}",
                            dr.GetValue(0), dr.GetValue(1), dr.GetValue(2), dr.GetValue(3),
                            dr.GetValue(4), dr.GetValue(5), dr.GetValue(6), dr.GetValue(7)));
                    }
                Debug.WriteLine("");
            }
#endif
            return sql;
        }
        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties, ExpressionTreeToSqlCompilerBase<long> compiler, int pageSize, int pageNumber)
        {
            string orderByCols = String.Join(",",
                                  Enumerable.ToArray(Processor.Select(
                                      GetProviderPropertyValue<OrderByCollectionExpression, CollectionExpression<OrderByExpression>>(
                                          properties, new CollectionExpression<OrderByExpression>(new OrderByExpression[] { })), o => o.ToString("[{0}]"))));

            string orderByClause = string.IsNullOrEmpty(orderByCols) ? "ROWID" : " ORDER BY " + orderByCols;

            int startRecord = (pageNumber * pageSize);
            int endRecord = (pageNumber + 1) * pageSize - 1;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                           FormatColumnNames(true, true,
                                                                             compiler.ProjectedColumns.Count > 0
                                                                                 ? compiler.ProjectedColumns
                                                                                 : SelectAllColumnNames()
                                                               )));

            string subQueryColumns = string.Join(",", Enumerable.ToArray(
                                                          FormatColumnNames(false, false,
                                                                            compiler.ProjectedColumns.Count > 0
                                                                                ? compiler.ProjectedColumns
                                                                                : SelectAllColumnNames()
                                                              )));


            String sql = String.Format(
            "CREATE TEMP TABLE tmp AS SELECT {0} FROM {1} {2} {3} {4} {5};" +
            "SELECT {6} FROM tmp WHERE ROWID BETWEEN {7} AND {8};",
                mainQueryColumns,
                QualifiedTableName,
                compiler.SqlJoinClauses,
                String.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ", compiler.SqlWhereClause,
                orderByClause,
                subQueryColumns,
                compiler.CreateParameter(startRecord).ParameterName,
                compiler.CreateParameter(endRecord).ParameterName);

            return sql;
        }

        #region ISpatialDbProvider<SpatiaLite2_Utility> Members

        public new SpatiaLite2_Utility DbUtility
        {
            get { return (SpatiaLite2_Utility)base.DbUtility; }
        }

        #endregion
    }
}
