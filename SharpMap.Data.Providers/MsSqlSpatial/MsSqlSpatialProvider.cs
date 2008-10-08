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
                        BuildWithStatement(DefaultProviderProperties.ProviderProperties.Collection));
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

        protected string BuildWithStatement(IEnumerable<ProviderPropertyExpression> providerPropertyExpressions)
        {
            bool withNoLock =
                GetProviderPropertyValue<WithNoLockExpression, bool>(providerPropertyExpressions, false);

            return withNoLock ? " WITH(NOLOCK) " : "";
        }

        protected override IEnumerable<string> SelectAllColumnNames()
        {
            foreach (DataColumn col in GetSchemaTable().Columns)
            {
                yield return string.Equals(col.ColumnName, GeometryColumn, StringComparison.InvariantCultureIgnoreCase)
                                 ? String.Format(GeometryColumnConversionFormatString + " AS {1}", col.ColumnName,
                                                 GeometryColumn)
                                 : string.Format("{0}.{1}", Table, col.ColumnName);
            }
        }

        protected override ExpressionTreeToSqlCompilerBase CreateSqlCompiler(Expression expression)
        {
            bool withNoLock =
                GetProviderPropertyValue<WithNoLockExpression, bool>(
                    DefaultProviderProperties.ProviderProperties.Collection,
                    false);


            return new MsSqlSpatialExpressionTreeToSqlCompiler(DbUtility, withNoLock, SelectAllColumnNames,
                                                               GeometryColumnConversionFormatString, expression,
                                                               SpatialSchema, TableSchema, Table,
                                                               OidColumn, GeometryColumn, Srid);
        }

        protected override string GenerateSql(IList<ProviderPropertyExpression> properties,
                                              ExpressionTreeToSqlCompilerBase compiler)
        {
            return string.Format(" {0} SELECT {1}  FROM {2}{6} {3} {4} {5}",
                                 compiler.SqlParamDeclarations,
                                 string.IsNullOrEmpty(compiler.SqlColumns)
                                     ? string.Join(",", Enumerable.ToArray(SelectAllColumnNames()))
                                     : compiler.SqlColumns,
                                 compiler.QualifiedTableName,
                                 compiler.SqlJoinClauses,
                                 string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                 compiler.SqlWhereClause,
                                 BuildWithStatement(DefaultProviderProperties.ProviderProperties.Collection));
        }
    }
}