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
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.Db.Expressions;
using SharpMap.Data.Providers.MsSqlSpatial;
using SharpMap.Expressions;

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
            get { throw new NotImplementedException(); }
        }

        public override DataTable GetSchemaTable()
        {
            return base.GetSchemaTable(true);
        }

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
                        GetWithClause(DefaultProviderProperties.ProviderProperties.Collection));
                cmd.CommandType = CommandType.Text;
                double xmin, ymin, xmax, ymax;
                conn.Open();
                using (IDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        xmin = r.GetDouble(0);
                        ymin = r.GetDouble(1);
                        xmax = r.GetDouble(2);
                        ymax = r.GetDouble(3);
                        return GeometryFactory.CreateExtents2D(xmin, ymin, xmax, ymax);
                    }
                }
                return GeometryFactory.CreateExtents();
            }
        }

        protected string GetWithClause(IEnumerable<ProviderPropertyExpression> providerPropertyExpressions)
        {
            bool withNoLock =
                GetProviderPropertyValue<WithNoLockExpression, bool>(providerPropertyExpressions, false);

            return withNoLock ? " WITH(NOLOCK) " : "";
        }

        //protected override IEnumerable<string> SelectAllColumnNames(bool formatGeometryColumn, bool qualifyColumnNames)
        //{
        //    foreach (DataColumn col in GetSchemaTable().Columns)
        //    {
        //        yield return string.Equals(col.ColumnName, GeometryColumn, StringComparison.InvariantCultureIgnoreCase)
        //                        && formatGeometryColumn ? String.Format(GeometryColumnConversionFormatString + " AS {1}", col.ColumnName,
        //                                         GeometryColumn)
        //                                         : qualifyColumnNames ? string.Format("{0}.[{1}]", QualifiedTableName, col.ColumnName) : string.Format("[{0}]", col.ColumnName);
        //    }
        //}

        protected override ExpressionTreeToSqlCompilerBase<long> CreateSqlCompiler(Expression expression)
        {
            bool withNoLock =
                GetProviderPropertyValue<WithNoLockExpression, bool>(
                    DefaultProviderProperties.ProviderProperties.Collection,
                    false);


            return new MsSqlSpatialExpressionTreeToSqlCompiler(DbUtility, withNoLock, this,
                                                               GeometryColumnConversionFormatString, expression,
                                                               SpatialSchema, TableSchema, Table,
                                                               OidColumn, GeometryColumn, Srid);
        }

        protected override string GenerateSql(IList<ProviderPropertyExpression> properties,
                                              ExpressionTreeToSqlCompilerBase<long> compiler)
        {
            int pageNumber = GetProviderPropertyValue<DataPageNumberExpression, int>(properties, -1);
            int pageSize = GetProviderPropertyValue<DataPageSizeExpression, int>(properties, 0);

            if (pageSize > 0 && pageNumber > -1)
                return GenerateSql(properties, compiler, pageSize, pageNumber);


            string orderByCols = String.Join(",",
                                 Enumerable.ToArray(
                                     GetProviderPropertyValue<OrderByExpression, IEnumerable<string>>(
                                         properties, new string[] { })));

            string orderByClause = string.IsNullOrEmpty(orderByCols) ? "" : " ORDER BY " + orderByCols;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(compiler.ProjectedColumns.Count > 0
                              ? FormatColumnNames(true, true, compiler.ProjectedColumns)
                              : SelectAllColumnNames(true, true)));

            return string.Format(" {0} SELECT {1}  FROM {2}{6} {3} {4} {5} {7}",
                                 compiler.SqlParamDeclarations,
                                 mainQueryColumns,
                                 compiler.QualifiedTableName,
                                 compiler.SqlJoinClauses,
                                 string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                 compiler.SqlWhereClause,
                                 GetWithClause(properties),
                                 orderByClause);
        }

        protected override string GenerateSql(IList<ProviderPropertyExpression> properties, ExpressionTreeToSqlCompilerBase<long> compiler, int pageSize, int pageNumber)
        {


            string orderByCols = string.Join(",",
                                            Enumerable.ToArray(
                                                GetProviderPropertyValue<OrderByExpression, IEnumerable<string>>(
                                                    properties, new string[] { })));

            orderByCols = string.IsNullOrEmpty(orderByCols) ? OidColumn : orderByCols;
            int startRecord = (pageNumber * pageSize) + 1;
            int endRecord = (pageNumber + 1) * pageSize;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(compiler.ProjectedColumns.Count > 0
                             ? FormatColumnNames(true, true, compiler.ProjectedColumns)
                             : SelectAllColumnNames(true, true)));

            string subQueryColumns = string.Join(",", Enumerable.ToArray(compiler.ProjectedColumns.Count > 0
                                         ? FormatColumnNames(false, false, compiler.ProjectedColumns)
                                         : SelectAllColumnNames(false, false)));



            return string.Format(
@" {0};
WITH CTE(rownumber, {8}) 
    AS 
    (   SELECT ROW_NUMBER() OVER(ORDER BY {7} ASC) AS rownumber, {1}  
        FROM {2}{6} 
        {3} {4} {5} 
    ) 
SELECT {8} 
FROM CTE 
WHERE rownumber BETWEEN {9} AND {10} ",
                           compiler.SqlParamDeclarations,
                           mainQueryColumns,
                           compiler.QualifiedTableName,
                           compiler.SqlJoinClauses,
                           string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                           compiler.SqlWhereClause,
                           GetWithClause(properties),
                           orderByCols,
                           subQueryColumns,
                           compiler.CreateParameter(startRecord).ParameterName,
                           compiler.CreateParameter(endRecord).ParameterName);
        }
    }
}