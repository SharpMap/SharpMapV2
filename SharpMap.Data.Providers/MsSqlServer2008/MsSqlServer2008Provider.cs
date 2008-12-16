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
using System.Text;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.MsSqlServer2008;
using SharpMap.Data.Providers.MsSqlServer2008.Expressions;
using SharpMap.Expressions;
#if DOTNET35
using Processor = System.Linq.Enumerable;
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using SharpMap.Utilities.SridUtility;
using Processor = GeoAPI.DataStructures.Processor;
using Caster = GeoAPI.DataStructures.Caster;
using Enumerable = GeoAPI.DataStructures.Enumerable;
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
                return String.Format("geometry::STGeomFromWKB({0}, {1})", "{0}",
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
                return base.BuildSchemaTable(withGeometryColumn);
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

            string orderByCols = String.Join(",",
                                             Enumerable.ToArray(Processor.Select(
                                                                    GetProviderPropertyValue
                                                                        <OrderByCollectionExpression,
                                                                        CollectionExpression<OrderByExpression>>(
                                                                        properties,
                                                                        new CollectionExpression<OrderByExpression>(
                                                                            new OrderByExpression[] { })),
                                                                    o =>
                                                                    "[" + o.PropertyNameExpression.PropertyName + "] " +
                                                                    (o.Direction == SortOrder.Ascending ? "ASC" : "DESC"))));


            string orderByClause = string.IsNullOrEmpty(orderByCols) ? "" : " ORDER BY " + orderByCols;

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
            string orderByCols = String.Join(",",
                                             Enumerable.ToArray(Processor.Select(
                                                                    GetProviderPropertyValue
                                                                        <OrderByCollectionExpression,
                                                                        CollectionExpression<OrderByExpression>>(
                                                                        properties,
                                                                        new CollectionExpression<OrderByExpression>(
                                                                            new OrderByExpression[] { })),
                                                                    o =>
                                                                    "[" + o.PropertyNameExpression.PropertyName + "] " +
                                                                    (o.Direction == SortOrder.Ascending ? "ASC" : "DESC"))));


            orderByCols = string.IsNullOrEmpty(orderByCols) ? OidColumn : orderByCols;

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
            RebuildSpatialIndex(SpatialIndexGridDensity.Low, SpatialIndexGridDensity.Low, SpatialIndexGridDensity.Medium,
                                SpatialIndexGridDensity.High);
        }

        public void RebuildSpatialIndex(SpatialIndexGridDensity level1, SpatialIndexGridDensity level2, SpatialIndexGridDensity level3, SpatialIndexGridDensity level4)
        {

        }

        public void CreateIndex(string[] columnNames)
        {

        }

        public void CreateEnvelopeColumns()
        {
            string minX = string.Format("[{0}_Envelope_MinX]", GeometryColumn),
                miny = string.Format("[{0}_Envelope_MinY]", GeometryColumn),
                maxX = string.Format("[{0}_Envelope_MaxX]", GeometryColumn),
                maxY = string.Format("[{0}_Envelope_MaxY]", GeometryColumn);

            StringBuilder sb = new StringBuilder();

            //TODO:


            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (IDbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sb.ToString();
                    cmd.CommandType = CommandType.Text;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public static MsSqlServer2008Provider<TOid> Create(string connectionString, IGeometryFactory geometryFactory,
                                                           string schema, string tableName, FeatureDataTable model)
        {
            return CreateTableHelper.Create<TOid>(connectionString, geometryFactory, schema, tableName, model);
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

    public enum SpatialIndexGridDensity
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
            return (int)cmd.ExecuteScalar() == 1;
        }

        internal static bool CheckProviderCompatibility<TOid>(IDbConnection connection,
                                                              IGeometryFactory geometryFactory,
                                                              string schema,
                                                              string tableName,
                                                              FeatureDataTable model,
                                                              out MsSqlServer2008Provider<TOid> provider)
        {
            var p = new MsSqlServer2008Provider<TOid>(geometryFactory,
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
            if (a.DataType != b.DataType)
                return false;
            //TODO: cases where coercion is possible
            return true;
        }


        internal static MsSqlServer2008Provider<TOid> Create<TOid>(string connectionString,
                                                                   IGeometryFactory geometryFactory,
                                                                   string schema,
                                                                   string tableName,
                                                                   FeatureDataTable model)
        {
            using (IDbConnection conn = new SqlServerDbUtility().CreateConnection(connectionString))
            {
                conn.Open();
                if (!EnsureDbIsSpatiallyEnabled(conn))
                    throw new InvalidDatabaseConfigurationException("Database does not contain spatial components.");

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

                return new MsSqlServer2008Provider<TOid>(geometryFactory, connectionString, schema, tableName, oidColumn,
                                                         geometryColumn);
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

            cmd.ExecuteNonQuery();
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