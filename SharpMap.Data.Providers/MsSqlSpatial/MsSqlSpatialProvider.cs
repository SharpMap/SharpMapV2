/*
 *  The attached / following is part of SharpMap.Data.Providers.MsSqlSpatial
 *  SharpMap.Data.Providers.MsSqlSpatial is free software © 2008 Newgrove Consultants Limited, 
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
using GeoAPI.CoordinateSystems;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.MsSqlSpatial;
using SharpMap.Expressions;
using SharpMap.Utilities.SridUtility;

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
    public class MsSqlSpatialProvider
        : SpatialDbProviderBase<long>
    {
        private string _spatialSchema = "ST";

        public MsSqlSpatialProvider(IGeometryFactory geometryFactory, string connectionString, string tableName)
            : this(geometryFactory, connectionString, string.Empty, string.Empty, tableName, string.Empty, string.Empty)
        {
        }

        public MsSqlSpatialProvider(IGeometryFactory geometryFactory, string connectionString, string spatialSchema,
                                    string tableSchema, string tableName, string oidColumn, string geometryColumn)
            : base(
                new SqlServerDbUtility(), geometryFactory, connectionString, tableSchema, tableName, oidColumn,
                geometryColumn)
        {
            if (!string.IsNullOrEmpty(spatialSchema))
                SpatialSchema = spatialSchema;
        }

        public string SpatialSchema
        {
            get { return _spatialSchema; }
            set { _spatialSchema = value; }
        }

        public override string GeometryColumnConversionFormatString
        {
            get { return SpatialSchema + ".AsBinary({0})"; }
        }

        public override string GeomFromWkbFormatString
        {
            get
            {
                return string.Format("{0}.GeomFromWKB({1},{2})", SpatialSchema, "{0}",
                                     ParseSrid(Srid).HasValue ? ParseSrid(Srid).Value : -1);
            }
        }

        //protected override DataTable BuildSchemaTable()
        //{
        //    return base.BuildSchemaTable(true);
        //}

        public override IExtents GetExtents()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText =
                    string.Format(
                        "SELECT MIN({0}_Envelope_MinX), MIN({0}_Envelope_MinY), MAX({0}_Envelope_MaxX), MAX({0}_Envelope_MaxY) FROM {1}.{2} {3}",
                        GeometryColumn, TableSchema, Table,
                        GetWithClause(DefaultProviderProperties == null
                                          ? null
                                          : DefaultProviderProperties.ProviderProperties.Collection));
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
                return GeometryFactory.CreateExtents();
            }
        }

        protected string GetWithClause(IEnumerable<ProviderPropertyExpression> providerPropertyExpressions)
        {
            if (providerPropertyExpressions == null)
                return "";

            bool withNoLock =
                GetProviderPropertyValue<WithNoLockExpression, bool>(providerPropertyExpressions, false);

            return withNoLock ? " WITH(NOLOCK) " : "";
        }

        protected override ExpressionTreeToSqlCompilerBase<long> CreateSqlCompiler(Expression expression)
        {
            return new MsSqlSpatialExpressionTreeToSqlCompiler(this, expression);
        }

        protected override string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                                    ExpressionTreeToSqlCompilerBase<long> compiler)
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
                                                                    delegate(OrderByExpression o)
                                                                        {
                                                                            return "[" +
                                                                                   o.PropertyNameExpression.PropertyName +
                                                                                   "] " +
                                                                                   (o.Direction == SortOrder.Ascending
                                                                                        ? "ASC"
                                                                                        : "DESC");
                                                                        })));

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
                                                    ExpressionTreeToSqlCompilerBase<long> compiler, int pageSize,
                                                    int pageNumber)
        {
            string orderByCols = String.Join(",",
                                             Enumerable.ToArray(
                                                 Processor.Select(
                                                     GetProviderPropertyValue
                                                         <OrderByCollectionExpression,
                                                         CollectionExpression<OrderByExpression>>(
                                                         properties,
                                                         new CollectionExpression<OrderByExpression>(
                                                             new[] { new OrderByExpression(OidColumn), })),
                                                     delegate(OrderByExpression o)
                                                         {
                                                             return "[" + o.PropertyNameExpression.PropertyName + "] " +
                                                                    (o.Direction == SortOrder.Ascending ? "ASC" : "DESC");
                                                         })));

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


        public static MsSqlSpatialProvider Create(string connectionString, string schema, string tableName,
                                                  FeatureDataTable model)
        {
            throw new NotImplementedException();
        }

        protected override void ReadSpatialReference(out ICoordinateSystem cs, out string srid)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (IDbCommand cmd = DbUtility.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText =
                        "SELECT TOP 1 SRID from ST.GEOMETRY_COLUMNS WHERE F_TABLE_SCHEMA = @pschema and F_TABLE_NAME = @ptablename";

                    cmd.Parameters.Add(DbUtility.CreateParameter("pschema", TableSchema, ParameterDirection.Input));
                    cmd.Parameters.Add(DbUtility.CreateParameter("ptablename", Table, ParameterDirection.Input));
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result is int)
                    {
                        int isrid = (int)result;
                        cs = SridMap.DefaultInstance.Process(isrid, (ICoordinateSystem)null);
                        srid = !Equals(cs, default(ICoordinateSystem)) ? SridMap.DefaultInstance.Process(cs, "") : "";
                        return;
                    }
                }
            }
            cs = default(ICoordinateSystem);
            srid = "";
        }
    }
}