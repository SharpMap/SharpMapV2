/*
 *  The attached / following is part of SharpMap.Data.Providers.Db
 *  SharpMap.Data.Providers.Db is free software © 2008 Newgrove Consultants Limited, 
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Database;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db
{
    public abstract class SpatialDbProviderBase<TOid>
        : ProviderBase, IWritableFeatureProvider<TOid>
    {
        static SpatialDbProviderBase()
        {
            AddDerivedProperties(typeof(SpatialDbProviderBase<TOid>));
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="ConnectionString"/> property.
        /// </summary>
        public static PropertyDescriptor ConnectionStringProperty
        {
            get { return ProviderStaticProperties.Find("ConnectionString", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="DefinitionQuery"/> property.
        /// </summary>
        public static PropertyDescriptor DefinitionQueryProperty
        {
            get { return ProviderStaticProperties.Find("DefinitionQuery", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="GeometryColumn"/> property.
        /// </summary>
        public static PropertyDescriptor GeometryColumnProperty
        {
            get { return ProviderStaticProperties.Find("GeometryColumn", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="GeometryColumnConversionFormatString"/> property.
        /// </summary>
        public static PropertyDescriptor GeometryColumnConversionFormatStringProperty
        {
            get { return ProviderStaticProperties.Find("GeometryColumnConversionFormatString", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="Locale"/> property.
        /// </summary>
        public static PropertyDescriptor LocaleProperty
        {
            get { return ProviderStaticProperties.Find("Locale", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="OidColumn"/> property.
        /// </summary>
        public static PropertyDescriptor OidColumnProperty
        {
            get { return ProviderStaticProperties.Find("OidColumn", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="Table"/> property.
        /// </summary>
        public static PropertyDescriptor TableProperty
        {
            get { return ProviderStaticProperties.Find("Table", false); }
        }

        /// <summary>
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="SpatialDbProviderBase{TOid}"/>'s <see cref="TableSchema"/> property.
        /// </summary>
        public static PropertyDescriptor TableSchemaProperty
        {
            get { return ProviderStaticProperties.Find("TableSchema", false); }
        }

        private String _geometryColumn = "Wkb_Geometry";
        private String _oidColumn = "Oid";
        private String _tableSchema = "dbo";

        protected SpatialDbProviderBase(IDbUtility dbUtility,
                                        IGeometryFactory geometryFactory,
                                        String connectionString,
                                        String tableSchema,
                                        String tableName,
                                        String oidColumn,
                                        String geometryColumn)
        {
            DbUtility = dbUtility;
            GeometryFactory = geometryFactory;
            SpatialReference = GeometryFactory == null ? null : GeometryFactory.SpatialReference;
            Srid = GeometryFactory == null ? null : GeometryFactory.Srid;

            if (!String.IsNullOrEmpty(connectionString))
            {
                ConnectionString = connectionString;
            }

            if (!String.IsNullOrEmpty(tableSchema))
            {
                TableSchema = tableSchema;
            }

            if (!String.IsNullOrEmpty(oidColumn))
            {
                OidColumn = oidColumn;
            }

            if (!String.IsNullOrEmpty(geometryColumn))
            {
                GeometryColumn = geometryColumn;
            }

            if (!String.IsNullOrEmpty(tableName))
            {
                Table = tableName;
            }
        }

        public String ConnectionString { get; set; }

        public String Table { get; set; }

        public String TableSchema
        {
            get { return _tableSchema; }
            set { _tableSchema = value; }
        }

        public String GeometryColumn
        {
            get { return _geometryColumn; }
            set { _geometryColumn = value; }
        }

        /// <summary>
        /// Name of column that contains the Object ID.
        /// </summary>
        public String OidColumn
        {
            get { return _oidColumn; }
            set { _oidColumn = value; }
        }

        /// <summary>
        /// Definition query used for limiting dataset (WHERE clause)
        /// </summary>
        public PredicateExpression DefinitionQuery { get; set; }

        protected IDbUtility DbUtility { get; set; }
        public abstract String GeometryColumnConversionFormatString { get; }

        #region IWritableFeatureProvider<TOid> Members

        public void Insert(FeatureDataRow<TOid> feature)
        {
            Insert(new[] {feature});
        }

        public void Insert(IEnumerable<FeatureDataRow<TOid>> features)
        {
            throw new NotImplementedException();
        }

        public void Update(FeatureDataRow<TOid> feature)
        {
            Update(new[] {feature});
        }

        public void Update(IEnumerable<FeatureDataRow<TOid>> features)
        {
            throw new NotImplementedException();
        }

        public void Delete(FeatureDataRow<TOid> feature)
        {
            Delete(new[] {feature});
        }

        public void Delete(IEnumerable<FeatureDataRow<TOid>> features)
        {
            var featureIds = new List<TOid>();
            foreach (var fdr in features)
            {
                featureIds.Add(fdr.Id);
            }

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (IDbCommand cmd = DbUtility.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        String.Format(
                            "DELETE FROM {0}.{1} WHERE {2} in ({3})",
                            TableSchema,
                            Table,
                            OidColumn,
                            String.Join(",",
                                        Enumerable.ToArray(Enumerable.Transform<TOid, String>(featureIds,
                                                                                              DbUtility.FormatValue)))
                            );
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<TOid> ExecuteOidQuery(SpatialBinaryExpression query)
        {
            foreach (IFeatureDataRecord fdr in
                ExecuteFeatureDataReader(
                    PrepareCommand(new FeatureQueryExpression(new AttributesProjectionExpression(new[] {OidColumn}),
                                                              null, query))))
            {
                yield return (TOid) fdr.GetOid();
            }
        }

        public IGeometry GetGeometryByOid(TOid oid)
        {
            //var exp
            //    = new FeatureQueryExpression(
            //        new AttributesProjectionExpression(
            //            new[]
            //                {
            //                    String.Format(GeometryColumnConversionFormatString, GeometryColumn)
            //                }),
            //        new AttributeBinaryExpression(new PropertyNameExpression(OidColumn), BinaryOperator.Equals,
            //                                      new LiteralExpression<TOid>(oid)),
            //        null);

            //using (IFeatureDataReader reader = ExecuteFeatureDataReader(PrepareCommand(exp)))
            //{
            //    foreach (IFeatureDataRecord fdr in reader)
            //        return fdr.Geometry;
            //}
            return null;
        }

        public IFeatureDataRecord GetFeatureByOid(TOid oid)
        {
            //var exp
            //    = new FeatureQueryExpression(
            //        new AttributesProjectionExpression(SelectAllColumnNames()),
            //        new AttributeBinaryExpression(new PropertyNameExpression(OidColumn), BinaryOperator.Equals,
            //                                      new LiteralExpression<TOid>(oid)),
            //        null);

            //using (IFeatureDataReader reader = ExecuteFeatureDataReader(PrepareCommand(exp)))
            //{
            //    foreach (IFeatureDataRecord fdr in reader)
            //        return fdr;
            //}
            return null;
        }

        public void SetTableSchema(FeatureDataTable<TOid> table)
        {
            SetTableSchema(table, SchemaMergeAction.Add | SchemaMergeAction.Key);
        }

        public void SetTableSchema(FeatureDataTable<TOid> table, SchemaMergeAction schemaAction)
        {
            DataTable dt = GetSchemaTable();

            table.Merge(dt);
        }

        public FeatureDataTable CreateNewTable()
        {
            var tbl = new FeatureDataTable<TOid>(OidColumn, GeometryFactory);
            SetTableSchema(tbl, SchemaMergeAction.Add | SchemaMergeAction.Key);
            return tbl;
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query)
        {
            return ExecuteFeatureQuery(query, FeatureQueryExecutionOptions.FullFeature);
        }

        public IFeatureDataReader ExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            return ExecuteFeatureDataReader(PrepareCommand(query));
        }

        public IGeometryFactory GeometryFactory { get; set; }

        public Int32 GetFeatureCount()
        {
            var attrs = new AttributesProjectionExpression(new[] {"Count(*)"});

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (
                    IDbCommand cmd =
                        PrepareCommand(DefinitionQuery == null
                                           ? (Expression) attrs
                                           : new QueryExpression(attrs, DefinitionQuery)))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    return (Int32) cmd.ExecuteScalar();
                }
            }
        }

        // Note: maybe need to cache the results of this? It could get called pretty often
        public DataTable GetSchemaTable()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (IDbCommand cmd = DbUtility.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM {0} ", Table);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    IDbDataAdapter da = DbUtility.CreateAdapter(cmd);
                    var ds = new DataSet();
                    da.FillSchema(ds, SchemaType.Source);
                    return ds.Tables["Table"];
                }
            }
        }


        public CultureInfo Locale
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public void SetTableSchema(FeatureDataTable table)
        {
            table.Merge(GetSchemaTable());
        }

        public override String ConnectionId
        {
            get { return ConnectionString; }
        }

        public override Object ExecuteQuery(Expression query)
        {
            return ExecuteFeatureDataReader(PrepareCommand(query));
        }

        #endregion

        protected override void Dispose(Boolean disposing)
        {
            if (!IsDisposed)
            {
                if (IsOpen)
                {
                    Close();
                }
            }
        }

        protected virtual IEnumerable<String> SelectAllColumnNames()
        {
            foreach (DataColumn col in GetSchemaTable().Columns)
            {
                yield return String.Equals(col.ColumnName, 
                                           GeometryColumn, 
                                           StringComparison.InvariantCultureIgnoreCase)
                                 ? String.Format(GeometryColumnConversionFormatString + " AS {1}", 
                                                 col.ColumnName,
                                                 GeometryColumn)
                                 : col.ColumnName;
            }
        }

        protected virtual IFeatureDataReader ExecuteFeatureDataReader(IDbCommand cmd)
        {
            Debug.WriteLine(String.Format("executing sql : {0}", cmd.CommandText));
            IDbConnection conn = DbUtility.CreateConnection(ConnectionString);
            cmd.Connection = conn;
            conn.Open();
            return new SpatialDbFeatureDataReader(GeometryFactory, cmd.ExecuteReader(CommandBehavior.CloseConnection),
                                                  GeometryColumn, OidColumn);
        }

        protected virtual IDbCommand PrepareCommand(Expression query)
        {
            Expression exp = query;

            if (DefinitionQuery != null)
            {
                exp = new BinaryExpression(query, BinaryOperator.And, DefinitionQuery);
            }

            ExpressionTreeToSqlCompilerBase compiler = CreateSqlCompiler(exp);

            String sql = String.Format(" {0} SELECT {1} FROM {2} {3} {4} {5}",
                                       compiler.SqlParamDeclarations,
                                       String.IsNullOrEmpty(compiler.SqlColumns)
                                           ? String.Join(",", Enumerable.ToArray(SelectAllColumnNames()))
                                           : compiler.SqlColumns,
                                       compiler.QualifiedTableName,
                                       compiler.SqlJoinClauses,
                                       String.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                       compiler.SqlWhereClause);

            IDbCommand cmd = DbUtility.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            foreach (IDataParameter p in compiler.ParameterCache.Values)
            {
                cmd.Parameters.Add(p);
            }

            return cmd;
        }

        protected abstract ExpressionTreeToSqlCompilerBase CreateSqlCompiler(Expression expression);
    }
}
