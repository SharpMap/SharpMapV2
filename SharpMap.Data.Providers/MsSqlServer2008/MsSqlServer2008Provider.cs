/*
 *  The attached / following is part of SharpMap.Data.Providers.MsSqlServer2008
 *  SharpMap.Data.Providers.MsSqlServer2008 is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
using System.Globalization;
using System.Text;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.MsSqlServer2008;
using SharpMap.Data.Providers.MsSqlServer2008.Expressions;
using SharpMap.Expressions;
using SharpMap.Utilities.SridUtility;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = GeoAPI.DataStructures.Caster;
#else
using Processor = GeoAPI.DataStructures.Processor;
using Caster = GeoAPI.DataStructures.Caster;
using Enumerable = GeoAPI.DataStructures.Enumerable;
using System.ComponentModel;
#endif

namespace SharpMap.Data.Providers
{
    public enum SqlServer2008ExtentsMode
    {
        /// <summary>
        /// Requires no additional components but can be very slow for large datasets
        /// </summary>
        QueryIndividualFeatures = 0,
        /// <summary>
        /// Requires SqlSpatialTools be installed on the db server
        /// </summary>
        UseSqlSpatialTools = 1,

        /// <summary>
        /// Requires no additional components but does require additional columns in the form of [GeomColumnName]_Envelope_MinX, [GeomColumnName]_Envelope_MinY, [GeomColumnName]_Envelope_MaxX, [GeomColumnName]_Envelope_MaxY
        /// Initial tests seem to show this is the fastest.
        /// </summary>
        UseEnvelopeColumns = 2
    }

    public class MsSqlServer2008Provider<TOid>
        : SpatialDbProviderBase<TOid>
    {
        public MsSqlServer2008Provider(IGeometryFactory geometryFactory,
                                       String connectionString,
                                       String tableName)
            : this(geometryFactory, connectionString, null, tableName, null, null)
        {
        }

        public MsSqlServer2008Provider(IGeometryFactory geometryFactory,
                                       String connectionString,
                                       String tableSchema,
                                       String tableName,
                                       String oidColumn,
                                       String geometryColumn)
            : base(new SqlServerDbUtility(),
                   geometryFactory,
                   connectionString,
                   tableSchema,
                   tableName,
                   oidColumn,
                   geometryColumn)
        {
        }

        public override String GeometryColumnConversionFormatString
        {
            get { return "{0}.STAsBinary()"; }
        }

        public override string GeomFromWkbFormatString
        {
            get
            {
                return String.Format("geometry::STGeomFromWKB({0}, {1}).MakeValid()", "{0}",
                                     SridInt.HasValue ? SridInt : 0);
            }
        }

        public override IExtents GetExtents()
        {
            bool withNoLock =
                GetProviderPropertyValue<WithNoLockExpression, bool>(
                    DefaultProviderProperties == null ? null : DefaultProviderProperties.ProviderProperties.Collection,
                    false);

            SqlServer2008ExtentsMode server2008ExtentsCalculationMode =
                GetProviderPropertyValue<MsSqlServer2008ExtentsModeExpression, SqlServer2008ExtentsMode>(
                    DefaultProviderProperties == null ? null : DefaultProviderProperties.ProviderProperties.Collection,
                    SqlServer2008ExtentsMode.QueryIndividualFeatures);

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.Connection = conn;
                switch (server2008ExtentsCalculationMode)
                {
                    case SqlServer2008ExtentsMode.UseSqlSpatialTools:
                        {
                            cmd.CommandText =
                                string.Format(
                                    @"
    declare @envelope Geometry
    select @envelope = dbo.GeometryEnvelopeAggregate({0}) from {1}.{2} {3}
    select 
        @envelope.STPointN(2).STX as MinX, 
        @envelope.STPointN(2).STY as MinY, 
        @envelope.STPointN(4).STX as MaxX, 
        @envelope.STPointN(4).STY as MaxY",
                                    GeometryColumn, TableSchema, Table,
                                    withNoLock ? "WITH(NOLOCK)" : "");
                            break;
                        }
                    case SqlServer2008ExtentsMode.UseEnvelopeColumns:
                        {
                            cmd.CommandText = string.Format(
                                "SELECT MIN({0}_Envelope_MinX), MIN({0}_Envelope_MinY), MAX({0}_Envelope_MaxX), MAX({0}_Envelope_MaxY) FROM {1}.{2} {3}",
                                GeometryColumn, TableSchema, Table,
                                withNoLock ? "WITH(NOLOCK)" : "");
                            break;
                        }
                    default:
                        {
                            cmd.CommandText =
                                string.Format(
                                    @"
    select 
	    Min(Geom.STEnvelope().STPointN(1).STX)as MinX, 
	    Min(Geom.STEnvelope().STPointN(1).STY) as MinY,  
	    Max(Geom.STEnvelope().STPointN(3).STX) as MaxX, 
	    Max(Geom.STEnvelope().STPointN(3).STY) as MaxY FROM {0}.{1} {2}",
                                    TableSchema, Table, withNoLock ? "WITH(NOLOCK)" : "");
                            break;
                        }
                }

                cmd.CommandType = CommandType.Text;
                conn.Open();
                using (IDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        if (r.IsDBNull(0) || r.IsDBNull(1) || r.IsDBNull(2) || r.IsDBNull(3))
                            return GeometryFactory.CreateExtents();

                        double xmin = r.GetDouble(0);
                        double ymin = r.GetDouble(1);
                        double xmax = r.GetDouble(2);
                        double ymax = r.GetDouble(3);
                        return GeometryFactory.CreateExtents2D(xmin, ymin, xmax, ymax);
                    }
                }
            }

            return GeometryFactory.CreateExtents();
        }

        protected override ExpressionTreeToSqlCompilerBase<TOid> CreateSqlCompiler(Expression expression)
        {
            return new MsSqlServer2008ExpressionTreeToSqlCompiler<TOid>(this, expression);
        }

        protected override DataTable BuildSchemaTable(bool withGeometryColumn)
        {
            try
            {
                DataTable dt = base.BuildSchemaTable(withGeometryColumn);
                //Remove envelope columns from the table
                for (int i = dt.Columns.Count - 1; i > -1; i--)
                {
                    DataColumn c = dt.Columns[i];
                    if (c.ColumnName.StartsWith(string.Format("{0}_Envelope", GeometryColumn)))
                        dt.Columns.RemoveAt(i);
                }

                return dt;
            }
            catch (Exception ex)
            {
                throw new SchemaTableBuildException(ex);
                //jd: it took ages to work out this exception :(  hopefully it has saved you some time.
            }
        }

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                                    ExpressionTreeToSqlCompilerBase<TOid> compiler)
        {
            int pageNumber = GetProviderPropertyValue<DataPageNumberExpression, int>(properties, -1);
            int pageSize = GetProviderPropertyValue<DataPageSizeExpression, int>(properties, 0);

            if (pageSize > 0 && pageNumber > -1)
                return GenerateSelectSql(properties, compiler, pageSize, pageNumber);

            //string orderByCols = String.Join(",",
            //                                 Enumerable.ToArray(Processor.Select(
            //                                                        GetProviderPropertyValue
            //                                                            <OrderByCollectionExpression,
            //                                                            CollectionExpression<OrderByExpression>>(
            //                                                            properties,
            //                                                            new CollectionExpression<OrderByExpression>(
            //                                                                new OrderByExpression[] { })),
            //                                                        delegate(OrderByExpression o)
            //                                                        {
            //                                                            return "[" +
            //                                                                   o.PropertyNameExpression.PropertyName +
            //                                                                   "] " +
            //                                                                   (o.Direction == SortOrder.Ascending
            //                                                                        ? "ASC"
            //                                                                        : "DESC");
            //                                                        })));


            string orderByClause = string.IsNullOrEmpty(compiler.OrderByClause) ? "" : " ORDER BY " + compiler.OrderByClause;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                           FormatColumnNames(true, true,
                                                                             compiler.ProjectedColumns.Count > 0
                                                                                 ? compiler.ProjectedColumns
                                                                                 : SelectAllColumnNames()
                                                               )));

            return string.Format(" {0} SELECT {1}  FROM {2}{6} {3} {4} {5} {7}",
                                 compiler.SqlParamDeclarations,
                                 mainQueryColumns,
                                 QualifiedTableName,
                                 compiler.SqlJoinClauses,
                                 string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                 compiler.SqlWhereClause,
                                 GetWithClause(properties),
                                 orderByClause);
        }

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                                    ExpressionTreeToSqlCompilerBase<TOid> compiler, int pageSize,
                                                    int pageNumber)
        {
            //string orderByCols = String.Join(",",
            //                                 Enumerable.ToArray(Processor.Select(
            //                                                        GetProviderPropertyValue
            //                                                            <OrderByCollectionExpression,
            //                                                            CollectionExpression<OrderByExpression>>(
            //                                                            properties,
            //                                                            new CollectionExpression<OrderByExpression>(
            //                                                                new OrderByExpression[] { })),
            //                                                        delegate(OrderByExpression o)
            //                                                        {
            //                                                            return "[" +
            //                                                                   o.PropertyNameExpression.PropertyName +
            //                                                                   "] " +
            //                                                                   (o.Direction == SortOrder.Ascending
            //                                                                        ? "ASC"
            //                                                                        : "DESC");
            //                                                        })));


            string orderByCols = string.IsNullOrEmpty(compiler.OrderByClause) ? OidColumn : compiler.OrderByClause;

            int startRecord = (pageNumber * pageSize) + 1;
            int endRecord = (pageNumber + 1) * pageSize;

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


            return string.Format(
                @" {0};
WITH CTE(rownumber, {8}) 
    AS 
    (   SELECT ROW_NUMBER() OVER(ORDER BY {7}) AS rownumber, {1}  
        FROM {2}{6} 
        {3} {4} {5} 
    ) 
SELECT {8} 
FROM CTE 
WHERE rownumber BETWEEN {9} AND {10} ",
                compiler.SqlParamDeclarations,
                mainQueryColumns,
                QualifiedTableName,
                compiler.SqlJoinClauses,
                string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                compiler.SqlWhereClause,
                GetWithClause(properties),
                orderByCols,
                subQueryColumns,
                compiler.CreateParameter(startRecord).ParameterName,
                compiler.CreateParameter(endRecord).ParameterName);
        }

        protected string GetWithClause(IEnumerable<ProviderPropertyExpression> properties)
        {
            bool withNoLock = GetProviderPropertyValue<WithNoLockExpression, bool>(properties, false);

            IEnumerable<string> indexNames = GetProviderPropertyValue<IndexNamesExpression, IEnumerable<string>>(
                properties, new string[] { });


            bool forceIndex = Enumerable.Count(indexNames) > 0 &&
                              GetProviderPropertyValue<ForceIndexExpression, bool>(properties, false);

            if (!withNoLock && !forceIndex)
                return "";

            if (withNoLock && !forceIndex)
                return " WITH(NOLOCK) ";

            if (forceIndex && !withNoLock)
                return string.Format(" WITH(INDEX({0})) ", string.Join(",", Enumerable.ToArray(indexNames)));

            return string.Format(" WITH(NOLOCK,INDEX({0})) ", string.Join(",", Enumerable.ToArray(indexNames)));
        }

        //protected override DataTable BuildSchemaTable()
        //{
        //    DataTable dt = base.BuildSchemaTable(true);
        //    dt.Columns[GeometryColumn].DataType = typeof(byte[]);
        //    //the natural return type is the native sql Geometry we need to override this to avoid a schema merge exception
        //    return dt;
        //}

        public void RebuildSpatialIndex()
        {
            RebuildSpatialIndex(SqlServer2008SpatialIndexGridDensity.Low, SqlServer2008SpatialIndexGridDensity.Low,
                                SqlServer2008SpatialIndexGridDensity.Medium,
                                SqlServer2008SpatialIndexGridDensity.High);
        }

        public void RebuildSpatialIndex(SqlServer2008SpatialIndexGridDensity level1,
                                        SqlServer2008SpatialIndexGridDensity level2,
                                        SqlServer2008SpatialIndexGridDensity level3,
                                        SqlServer2008SpatialIndexGridDensity level4)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                conn.Open();
                RebuildSpatialIndex(conn, level1, level2, level3, level4);
            }
        }

        protected internal void RebuildSpatialIndex(IDbConnection conn, SqlServer2008SpatialIndexGridDensity level1,
                                                    SqlServer2008SpatialIndexGridDensity level2,
                                                    SqlServer2008SpatialIndexGridDensity level3,
                                                    SqlServer2008SpatialIndexGridDensity level4)
        {
            Func<SqlServer2008SpatialIndexGridDensity, string> dlgtName =
                delegate(SqlServer2008SpatialIndexGridDensity o)
                {
                    switch (o)
                    {
                        case SqlServer2008SpatialIndexGridDensity.Low:
                            return "LOW";
                        case SqlServer2008SpatialIndexGridDensity.Medium:
                            return "MEDIUM";
                        default:
                            return "HIGH";
                    }
                };

            IExtents2D ext = GetExtents() as IExtents2D;

            StringBuilder sb = new StringBuilder();

            string ndxName = string.Format("[sidx_{0}_{1}]", Table, GeometryColumn);

            sb.AppendFormat(
                @"IF EXISTS(SELECT * FROM sys.indexes where name='{0}' and object_id = object_id('{1}'))
BEGIN
DROP INDEX {0} ON {1}
END
",
                ndxName, QualifiedTableName);

            sb.AppendFormat(CultureInfo.InvariantCulture,
                            @"CREATE SPATIAL INDEX {0}
   ON {2}({1})
   USING GEOMETRY_GRID
   WITH (
    BOUNDING_BOX = ( xmin={3}, ymin={4}, xmax={5}, ymax={6} ),
    GRIDS = ({7}, {8}, {9}, {10}));
",
                            ndxName, GeometryColumn, QualifiedTableName, ext.Min[Ordinates.X], ext.Min[Ordinates.Y],
                            ext.Max[Ordinates.X], ext.Max[Ordinates.Y], dlgtName(level1), dlgtName(level2),
                            dlgtName(level3),
                            dlgtName(level4));

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sb.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateIndex(string indexName, IEnumerable<string> columnNames)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                conn.Open();
                CreateIndex(conn, indexName, columnNames);
            }
        }

        public void FixGeometries()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                string sql = string.Format("UPDATE {0} Set {1} = {1}.MakeValid()", QualifiedTableName, GeometryColumn);
                conn.Open();
                using (IDbCommand cmd = DbUtility.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected internal void CreateIndex(IDbConnection conn, string indexName, IEnumerable<string> columnNames)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
                @"IF EXISTS (SELECT * FROM sys.indexes WHERE name = '{0}' and object_id = object_id('{1}'))
BEGIN
    DROP INDEX [{0}] on {1}
END
",
                indexName, QualifiedTableName);

            sb.AppendFormat(
                @"CREATE  INDEX [{0}] ON {1}({2})", indexName, QualifiedTableName,
                string.Join(",", Enumerable.ToArray(columnNames)));


            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sb.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }

        public void CreateEnvelopeColumns()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                conn.Open();
                //using (IDbTransaction tran = conn.BeginTransaction())
                //{
                CreateEnvelopeColumns(conn);
                //}
            }
        }

        protected internal void CreateEnvelopeColumns(IDbConnection conn)
        {
            string minX = string.Format("{0}_Envelope_MinX", GeometryColumn),
                   minY = string.Format("{0}_Envelope_MinY", GeometryColumn),
                   maxX = string.Format("{0}_Envelope_MaxX", GeometryColumn),
                   maxY = string.Format("{0}_Envelope_MaxY", GeometryColumn);

            StringBuilder sb = new StringBuilder();

            Action<string> dlgtDrop = delegate(string colName)
                                          {
                                              sb.AppendFormat(
                                                  @"IF EXISTS(SELECT * FROM sys.columns where name = '{0}' and object_id = object_id('{1}'))
BEGIN
ALTER TABLE {1} DROP COLUMN [{0}]
END
",
                                                  colName, QualifiedTableName);
                                          };

            dlgtDrop(minX);
            dlgtDrop(minY);
            dlgtDrop(maxX);
            dlgtDrop(maxY);

            sb.AppendFormat(
                @"
                ALTER TABLE {0} ADD", QualifiedTableName);

            Action<string, int, string> dlgtCreate = delegate(string colName, int coordIndex, string selector)
                                                         {
                                                             sb.AppendFormat(CultureInfo.InvariantCulture,
                                                                             @"
     [{0}] AS {1}.STEnvelope().STPointN({2}).{3} PERSISTED
",
                                                                             colName,
                                                                             GeometryColumn, coordIndex,
                                                                             selector);
                                                         };

            dlgtCreate(minX, 1, "STX");
            sb.AppendLine(",");
            dlgtCreate(minY, 1, "STY");
            sb.AppendLine(",");
            dlgtCreate(maxX, 3, "STX");
            sb.AppendLine(",");
            dlgtCreate(maxY, 3, "STY");


            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sb.ToString();
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }

            CreateIndex(conn, string.Format("ndx_{0}_{1}", Table, minX), new[] { minX });
            CreateIndex(conn, string.Format("ndx_{0}_{1}", Table, minY), new[] { minY });
            CreateIndex(conn, string.Format("ndx_{0}_{1}", Table, maxX), new[] { maxX });
            CreateIndex(conn, string.Format("ndx_{0}_{1}", Table, maxY), new[] { maxY });
        }


        public static MsSqlServer2008Provider<TOid> Create(string connectionString, IGeometryFactory geometryFactory,
                                                           string schema, string tableName, FeatureDataTable model)
        {
            return CreateTableHelper.Create<TOid>(connectionString, geometryFactory, schema, tableName, model);
        }

        public static void CreateGeometryColumnsTable(IDbConnection conn, string schema)
        {
            CreateTableHelper.CreateGeometryColumnsTable(conn, schema);
        }

        public void RegisterInGeometryColumnsTable()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                if (!DatabaseHasGeometryColumnsTable(conn, TableSchema))
                    CreateGeometryColumnsTable(conn, TableSchema);
                RegisterInGeometryColumnsTable(conn, DbUtility, TableSchema, Table, GeometryColumn, 2,
                                               SridInt.HasValue ? SridInt.Value : 0, "GEOMETRY");
            }
        }

        public void DropSridConstraint()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                DropSridConstraint(conn, TableSchema, Table, GeometryColumn);
            }
        }

        public void CreateSridConstraint()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                CreateSridConstraint(conn, TableSchema, Table, GeometryColumn, SridInt.HasValue ? SridInt.Value : 0);
            }
        }

        public static void RegisterInGeometryColumnsTable(IDbConnection conn, IDbUtility dbUtility, string schema,
                                                          string tableName, string geometryColumn, int coordDimension,
                                                          int srid, string geometryType)
        {
            CreateTableHelper.RegisterInGeometryColumns(conn, dbUtility, schema, tableName, geometryColumn,
                                                        coordDimension, srid, geometryType);
        }

        public static void CreateSridConstraint(IDbConnection conn, string schema, string tableName,
                                                string geometryColumn, int srid)
        {
            CreateTableHelper.CreateSridConstraint(conn, schema, tableName, geometryColumn, srid);
        }

        public static void DropSridConstraint(IDbConnection conn, string schema, string tableName, string geometryColumn)
        {
            CreateTableHelper.DropSridConstraint(conn, schema, tableName, geometryColumn);
        }

        public static bool DatabaseHasGeometryColumnsTable(IDbConnection conn, string schema)
        {
            return CreateTableHelper.CheckIfObjectExists(conn, schema, "Geometry_Columns");
        }

        public static void UnregsiterInGeometryColumnsTable(IDbConnection conn, IDbUtility dbUtility, string schema,
                                                          string tableName)
        {
            if (CreateTableHelper.CheckIfObjectExists(conn, "dbo", "geometry_columns"))
                CreateTableHelper.UnregisterInGeometryColumns(conn, dbUtility, schema, tableName);
        }

        public static void DropTable(IDbConnection conn, IDbUtility dbUtility, string schema,
                                                          string tableName)
        {
            UnregsiterInGeometryColumnsTable(conn, dbUtility, schema, tableName);
            CreateTableHelper.DropTable(conn, dbUtility, schema, tableName);
        }

        //TODO: add a set of strategies for reading srid
        protected override void ReadSpatialReference(out ICoordinateSystem cs, out string srid)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (IDbCommand cmd = DbUtility.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        string.Format(
                            @"
DECLARE @found bit 
set @found = 0; 
IF EXISTS(select * from sys.objects where object_id = object_id(@p1 + '.Geometry_Columns'))
    BEGIN
    IF  EXISTS(SELECT * FROM [{1}].[Geometry_Columns] WHERE F_Table_Catalog = @p0 AND F_Table_Schema = @p1 AND F_Table_Name = @p2 AND F_Geometry_Column = @p3)
        BEGIN
            SELECT DISTINCT SRID FROM [{1}].[Geometry_Columns] WHERE F_Table_Catalog = @p0 AND F_Table_Schema = @p1 AND F_Table_Name = @p2 AND F_Geometry_Column = @p3 
            SET @found = 1
        END
    END
IF @found = 0
    BEGIN
        SELECT DISTINCT [{0}].STSrid FROM [{1}].[{2}] 
    END",
                            GeometryColumn, TableSchema, Table);
                    cmd.Parameters.Add(DbUtility.CreateParameter("p0", conn.Database, ParameterDirection.Input));
                    cmd.Parameters.Add(DbUtility.CreateParameter("p1", TableSchema, ParameterDirection.Input));
                    cmd.Parameters.Add(DbUtility.CreateParameter("p2", Table, ParameterDirection.Input));
                    cmd.Parameters.Add(DbUtility.CreateParameter("p3", GeometryColumn, ParameterDirection.Input));

                    conn.Open();
                    using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            if (reader.RecordsAffected > 1)
                                throw new Exception(
                                    string.Format(
                                        "The Sql Server Table [{0}].[{1}] contains geometries in multiple Srids.",
                                        TableSchema, Table));

                            object v = reader[0];
                            if (v is int)
                            {
                                int isrid = (int)v;
                                cs = SridMap.DefaultInstance.Process(isrid, default(ICoordinateSystem));
                                srid = !Equals(cs, default(ICoordinateSystem))
                                           ? SridMap.DefaultInstance.Process(cs, "")
                                           : "";

                                return;
                            }
                        }
                    }
                }
            }
            cs = default(ICoordinateSystem);
            srid = "";
        }

        #region Nested type: SchemaTableBuildException

        public class SchemaTableBuildException : Exception
        {
            public SchemaTableBuildException(Exception ex)
                : base(
                    "An error occured while attempting to get the schema of the database table. Ensure that the Microsoft.SqlServer.Types assembly is installed in the GAC or bin directory of the host machine and check the inner exception.",
                    ex)
            {
            }
        }

        #endregion
    }

    public enum SqlServer2008SpatialIndexGridDensity
    {
        Low,
        Medium,
        High
    }

    public class InvalidDatabaseConfigurationException : Exception
    {
        public InvalidDatabaseConfigurationException(string message)
            : base(message)
        {
        }
    }

    internal static class CreateTableHelper
    {
        internal static bool EnsureDbIsSpatiallyEnabled(IDbConnection connection)
        {
            return CheckIfObjectExists(connection, "sys", "spatial_reference_systems");
        }


        internal static bool CheckIfObjectExists(IDbConnection connection, string schema, string objectName)
        {
            IDbCommand cmd = connection.CreateCommand();
            cmd.CommandText =
                string.Format(
                    @"SELECT 
CASE WHEN object_id('[{0}].[{1}]') IS NULL THEN 0
ELSE 1
END
	", schema, objectName);
            cmd.CommandType = CommandType.Text;
            EnsureOpenConnection(cmd);
            return (int)cmd.ExecuteScalar() == 1;
        }

        internal static bool CheckProviderCompatibility<TOid>(IDbConnection connection,
                                                              IGeometryFactory geometryFactory,
                                                              string schema,
                                                              string tableName,
                                                              FeatureDataTable model,
                                                              out MsSqlServer2008Provider<TOid> provider)
        {
            MsSqlServer2008Provider<TOid> p = new MsSqlServer2008Provider<TOid>(geometryFactory,
                                                                                connection.ConnectionString,
                                                                                schema,
                                                                                tableName,
                                                                                model.PrimaryKey[0].ColumnName,
                                                                                "Geom");

            FeatureDataTable fdt = p.CreateNewTable();

            if (FeatureDataTableModelIsCompatible(fdt, model))
            {
                provider = p;
                return true;
            }

            provider = null;
            return false;
        }

        internal static bool FeatureDataTableModelIsCompatible(FeatureDataTable a, FeatureDataTable b)
        {
            //TODO: need to handle case where envelope columns are the only difference
            if (a.Columns.Count != b.Columns.Count)
                return false;

            foreach (DataColumn clm in a.Columns)
            {
                if (!b.Columns.Contains(clm.ColumnName))
                    return false;

                DataColumn clm2 = b.Columns[clm.ColumnName];

                if (!DataColumnsCompatible(clm, clm2))
                    return false;
            }
            return true;
        }

        internal static bool DataColumnsCompatible(DataColumn a, DataColumn b)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(b.DataType);
            return tc.CanConvertTo(a.DataType);
        }

        internal static MsSqlServer2008Provider<TOid> Create<TOid>(string connectionString,
                                                                   IGeometryFactory geometryFactory,
                                                                   string schema,
                                                                   string tableName,
                                                                   FeatureDataTable model)
        {
            using (IDbConnection conn = new SqlServerDbUtility().CreateConnection(connectionString))
            {
                //using (IDbTransaction tran = conn.BeginTransaction())
                //{
                //    try
                //    {
                if (!EnsureDbIsSpatiallyEnabled(conn))
                    throw new InvalidDatabaseConfigurationException(
                        "Database does not contain spatial components.");

                if (CheckIfObjectExists(conn, schema, tableName))
                {
                    MsSqlServer2008Provider<TOid> provider;
                    if (CheckProviderCompatibility(conn, geometryFactory, schema, tableName, model, out provider))
                        return provider;

                    throw new IncompatibleSchemaException(
                        "The table already exists in the database but has an incompatible schema.");
                }
                string oidColumn, geometryColumn;
                CreateDatabaseObjects(conn, schema, tableName, model, out oidColumn, out geometryColumn);

                MsSqlServer2008Provider<TOid> prov = new MsSqlServer2008Provider<TOid>(geometryFactory,
                                                                                       connectionString, schema,
                                                                                       tableName, oidColumn,
                                                                                       geometryColumn);


                //    tran.Commit();
                return prov;
                //}
                //catch
                //{
                //    tran.Rollback();
                //    throw;
                //}
                //}
            }
        }

        internal static void CreateDatabaseObjects(IDbConnection conn, string schema, string tableName,
                                                   FeatureDataTable model, out string oidColumn,
                                                   out string geometryColumn)
        {
            SqlServerDbUtility util = new SqlServerDbUtility();
            string oidCol = model.PrimaryKey[0].ColumnName, geomCol = "Geom";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("CREATE TABLE  [{0}].[{1}] (", schema, tableName);
            foreach (DataColumn column in model.Columns)
            {
                sb.AppendFormat("[{0}] {1},\n", column.ColumnName, SqlServerDbUtility.GetFullTypeString(column.DataType));
            }

            sb.Append("\n[Geom] geometry,");
            sb.AppendFormat("CONSTRAINT [pk_{0}_{1}] PRIMARY KEY CLUSTERED([{1}])", tableName, oidCol);
            sb.AppendLine(")");

            oidColumn = oidCol;
            geometryColumn = geomCol;

            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sb.ToString();

            ExecuteNoQuery(cmd);
        }


        internal static void CreateGeometryColumnsTable(IDbConnection conn, string schema)
        {
            string sql = string.Format(
                @"CREATE TABLE [{0}].[Geometry_Columns](
    [F_Table_Catalog] [varchar](255) NOT NULL,
    [F_Table_Schema] [varchar](20) NOT NULL,
    [F_Table_Name] [varchar](255) NOT NULL,
    [F_Geometry_Column] [varchar](255) NOT NULL,
    [Coord_Dimension] [int] NOT NULL,
    [SRID] [int] NOT NULL,
    [Geometry_Type] [varchar](20) NULL,
    [DataSourceValid]  AS (case when object_id([F_Table_Schema] + '.' + [F_Table_Name]) IS NOT NULL then 1 else 0 end),
 CONSTRAINT [PK_Geometry_Columns] PRIMARY KEY CLUSTERED 
(
    [F_Table_Catalog] ASC,
    [F_Table_Schema] ASC,
    [F_Table_Name] ASC,
    [F_Geometry_Column] ASC
) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[Geometry_Columns] ADD  CONSTRAINT [DF_Geometry_Columns_Geometry_Type]  DEFAULT ('Geometry') FOR [Geometry_Type]
",
                schema);

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                ExecuteNoQuery(cmd);
            }
        }

        internal static void UnregisterInGeometryColumns(IDbConnection conn, IDbUtility dbUtility, string schema,
                                                       string tableName)
        {
            string sql = string.Format(
                @"DELETE FROM [{0}].[Geometry_Columns] 
            WHERE 
                F_Table_Catalog = @pCatalog 
                AND  F_Table_Schema = @pSchema 
                AND F_Table_Name = @pTable 
                ", schema);

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(dbUtility.CreateParameter("pCatalog", conn.Database, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pSchema", schema, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pTable", tableName, ParameterDirection.Input));
                ExecuteNoQuery(cmd);
            }
        }

        internal static void RegisterInGeometryColumns(IDbConnection conn, IDbUtility dbUtility, string schema,
                                                       string tableName, string geometryColumnName, int coordDimension,
                                                       int srid, string geometryType)
        {
            string sql = string.Format(
                @"IF EXISTS(SELECT * FROM [{0}].[Geometry_Columns] 
            WHERE 
                F_Table_Catalog = @pCatalog 
                AND  F_Table_Schema = @pSchema 
                AND F_Table_Name = @pTable 
                AND F_Geometry_Column = @pGeomColumn)
	BEGIN
		UPDATE [{0}].Geometry_Columns 
			SET 
				Coord_Dimension = @pCoordDimension,
				SRID = @pSrid,
				Geometry_Type = @pGeometryType 
			WHERE 
				F_Table_Catalog = @pCatalog 
				AND F_Table_Schema = @pSchema  
				AND F_Table_Name = @pTable 
				AND F_Geometry_Column = @pGeomColumn 

	END
ELSE
	BEGIN
		INSERT INTO [{0}].[Geometry_Columns](
                        F_Table_Catalog
                        , F_Table_Schema
                        , F_Table_Name
                        , F_Geometry_Column
                        , Coord_Dimension
                        , SRID
                        , Geometry_Type)
		Values(
                @pCatalog
                , @pSchema
                , @pTable
                , @pGeomColumn
                , @pCoordDimension
                , @pSrid
                , @pGeometryType)           
	END",
                schema);

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add(dbUtility.CreateParameter("pCatalog", conn.Database, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pSchema", schema, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pTable", tableName, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pGeomColumn", geometryColumnName, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pCoordDimension", coordDimension, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pSrid", srid, ParameterDirection.Input));
                cmd.Parameters.Add(dbUtility.CreateParameter("pGeometryType", geometryType, ParameterDirection.Input));

                ExecuteNoQuery(cmd);
            }
        }

        internal static void CreateSridConstraint(IDbConnection conn, string schema, string tableName,
                                                  string geometryColumnName, int srid)
        {
            DropSridConstraint(conn, schema, tableName, geometryColumnName);

            string name = string.Format("ck_{0}_{1}_{2}_STSrid", schema, tableName, geometryColumnName);

            string sql = string.Format(
                @"ALTER TABLE [{0}].[{1}] ADD CONSTRAINT {2}
	CHECK ([{3}].STSrid = {4})", schema, tableName, name,
                geometryColumnName, srid);

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                ExecuteNoQuery(cmd);
            }
        }

        internal static void DropSridConstraint(IDbConnection conn, string schema, string tableName,
                                                string geometryColumnName)
        {
            string name = string.Format("ck_{0}_{1}_{2}_STSrid", schema, tableName, geometryColumnName);

            string sql = string.Format(
                @"IF EXISTS(SELECT * FROM sys.check_constraints where object_id = object_id('{0}'))
    BEGIN
        ALTER TABLE [{1}].[{2}]
        DROP CONSTRAINT [{0}]
    END",
                name, schema, tableName);

            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                ExecuteNoQuery(cmd);
            }
        }

        internal static void ExecuteNoQuery(IDbCommand cmd)
        {
            EnsureOpenConnection(cmd);
            cmd.ExecuteNonQuery();
        }

        internal static void EnsureOpenConnection(IDbCommand cmd)
        {
            if (cmd.Connection.State == ConnectionState.Closed || cmd.Connection.State == ConnectionState.Broken)
                cmd.Connection.Open();
        }

        internal static void DropTable(IDbConnection conn, IDbUtility dbUtility, string schema, string tableName)
        {
            if (CheckIfObjectExists(conn, schema, tableName))
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("DROP TABLE [{0}].[{1}]", schema, tableName);
                    cmd.CommandType = CommandType.Text;
                    ExecuteNoQuery(cmd);
                }
            }
        }
    }


    public class IncompatibleSchemaException : Exception
    {
        public IncompatibleSchemaException(string message)
            : base(message)
        {
        }
    }
}