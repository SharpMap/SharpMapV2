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

/*
 * Portions by Felix Obermaier
 * Ingenieurgruppe IVV GmbH & Co. KG
 * www.ivv-aachen.de
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
        private String _geometryColumn = "Wkb_Geometry";
        private String _oidColumn = "Oid";
        private String _tableSchema = "dbo";

        static SpatialDbProviderBase()
        {
            AddDerivedProperties(typeof(SpatialDbProviderBase<TOid>));
        }

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
        public abstract String GeomFromWkbFormatString { get; }

        protected virtual String QualifiedTableName
        {
            get
            {
                if (string.IsNullOrEmpty(TableSchema))
                    return string.Format("[{0}]", Table);

                return string.Format("[{0}].[{1}]", TableSchema, Table);
            }
        }

        public ProviderPropertiesExpression DefaultProviderProperties { get; set; }

        #region IWritableFeatureProvider<TOid> Members

        public void Insert(FeatureDataRow<TOid> feature)
        {
            Insert((IEnumerable<FeatureDataRow<TOid>>)new[] { feature });
        }

        public void Insert(IEnumerable<FeatureDataRow<TOid>> features)
        {
            Insert((IEnumerable<FeatureDataRow>)features);
        }

        public void Update(FeatureDataRow<TOid> feature)
        {
            Update((IEnumerable<FeatureDataRow<TOid>>)new[] { feature });
        }

        public void Update(IEnumerable<FeatureDataRow<TOid>> features)
        {
            Update((IEnumerable<FeatureDataRow>)features);
        }

        public void Delete(FeatureDataRow<TOid> feature)
        {
            Delete(new[] { feature });
        }

        public void Delete(IEnumerable<FeatureDataRow<TOid>> features)
        {
            var featureIds = new List<TOid>();
            foreach (var fdr in features)
            {
                featureIds.Add(fdr.Id);
            }

            ExpressionTreeToSqlCompilerBase compiler = CreateSqlCompiler(null);
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (IDbCommand cmd = DbUtility.CreateCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        String.Format(
                            "DELETE FROM {0} WHERE {1} in ({2})",
                            QualifiedTableName,
                            OidColumn,
                            String.Join(",",
                                        Enumerable.ToArray(
                                            Processor.Transform(featureIds,
                                                                   delegate(TOid o)
                                                                       {
                                                                           return
                                                                               compiler.CreateParameter(o).ParameterName;
                                                                       })))
                            );
                    conn.Open();
                    foreach (IDataParameter p in compiler.ParameterCache.Values)
                        cmd.Parameters.Add(p); 

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<TOid> ExecuteOidQuery(SpatialBinaryExpression query)
        {
            foreach (IFeatureDataRecord fdr in
                ExecuteFeatureDataReader(
                    PrepareCommand(new FeatureQueryExpression(new AttributesProjectionExpression(new[] { OidColumn }),
                                                              null, query))))
            {
                yield return (TOid)fdr.GetOid();
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
            var attrs = new AttributesProjectionExpression(new[] { "Count(*)" });

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (
                    IDbCommand cmd =
                        PrepareCommand(DefinitionQuery == null
                                           ? (Expression)attrs
                                           : new QueryExpression(attrs, DefinitionQuery)))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    return (Int32)cmd.ExecuteScalar();
                }
            }
        }

        // Note: maybe need to cache the results of this? It could get called pretty often
        /// <remarks >maybe need to cache the results of this? It could get called pretty often</remarks>
        public virtual DataTable GetSchemaTable()
        {
            return GetSchemaTable(false);
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

        protected virtual IEnumerable<ProviderPropertyExpression> MergeProviderProperties(
            IEnumerable<ProviderPropertyExpression> other)
        {
            if (Equals(null, DefaultProviderProperties))
                return other;
            if (Equals(null, other))
                return DefaultProviderProperties.ProviderProperties.Collection;

            var list = new List<ProviderPropertyExpression>();

            Func<ProviderPropertyExpression, ProviderPropertyExpression, bool> predicate
                = (prop1, prop2) => prop1.GetType() == prop2.GetType();

            foreach (ProviderPropertyExpression prop in other)
                AddToList(list, prop, predicate);

            foreach (ProviderPropertyExpression prop in DefaultProviderProperties.ProviderProperties.Collection)
                AddToList(list, prop, predicate);

            return list;
        }

        private void AddToList<T>(IList<T> storage, T item, Func<T, T, bool> filter)
        {
            if (Enumerable.Count(Enumerable.Select(storage, i => filter(i, item))) == 0)
                storage.Add(item);
        }

        public virtual DataTable GetSchemaTable(Boolean withGeometryColumn)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT * FROM {0} ", QualifiedTableName);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                //conn.Open();
                IDbDataAdapter da = DbUtility.CreateAdapter(cmd);
                var ds = new DataSet();
                da.FillSchema(ds, SchemaType.Source);
                //conn.Close();
                DataTable dt = ds.Tables["Table"];

                //remove geometry column from schema
                if (!withGeometryColumn)
                {
                    dt.Columns.Remove(GeometryColumn);
                    //Int32 ordinal = dt.Columns[GeometryColumn].Ordinal;
                    //dt.Columns.Remove(ordinal);
                    //for (Int32 index = ordinal; index < dt.Rows.Count; index++)
                    //  dt.Columns[ordinal].Ordinal = ordinal;
                }

                return dt;
            }
        }

        public virtual void Insert(IEnumerable<FeatureDataRow> features)
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

        public void Update(DataRowCollection features)
        {
            Update(IEnumerableOfDataRowCollection(features));
        }

        public virtual void Update(IEnumerable<FeatureDataRow> features)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                {
                    Boolean success = true;

                    using (IDbTransaction tran = conn.BeginTransaction())
                    {
                        IDbCommand cmd = DbUtility.CreateCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = tran;

                        cmd.CommandText = string.Format(
                            "UPDATE {0} SET {1} WHERE ({2});", QualifiedTableName,
                            UpdateClause(cmd),
                            WhereClause(cmd));

                        foreach (FeatureDataRow row in features)
                        {
                            for (int i = 0; i < cmd.Parameters.Count - 2; i++)
                                ((IDataParameter)cmd.Parameters[i]).Value = row[i];

                            ((IDataParameter)cmd.Parameters["@PGeo"]).Value = row.Geometry.AsBinary();
                            ((IDataParameter)cmd.Parameters["@POldOid"]).Value =
                                row[OidColumn, DataRowVersion.Original];

                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                                success = false;
                                break;
                            }
                        }

                        if (success) tran.Commit();
                    }
                }
            }
        }

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
                                 : String.Format("[{0}]", col.ColumnName);
            }
        }

        protected virtual IFeatureDataReader ExecuteFeatureDataReader(IDbCommand cmd)
        {
            Debug.WriteLine(String.Format("executing sql : {0}", cmd.CommandText));
            IDbConnection conn = DbUtility.CreateConnection(ConnectionString);
            cmd.Connection = conn;
            if (conn.State == ConnectionState.Closed) conn.Open();
            return new SpatialDbFeatureDataReader(GeometryFactory, cmd.ExecuteReader(CommandBehavior.CloseConnection),
                                                  GeometryColumn, OidColumn);
        }

        protected virtual Expression MergeQueries(Expression expr1, Expression expr2)
        {
            if (Equals(null, expr1))
                return expr2;
            if (Equals(null, expr2))
                return expr1;

            if (expr1 is QueryExpression && expr2 is QueryExpression)
                return new BinaryExpression(expr1, BinaryOperator.And, expr2);

            throw new NotImplementedException(string.Format("No merge routine for Expressions of Types {0} and {1}",
                                                            expr1.GetType(),
                                                            expr2.GetType()));
        }

        protected virtual IDbCommand PrepareCommand(Expression query)
        {
            Expression exp = query;

            MergeQueries(query, DefinitionQuery);

            ExpressionTreeToSqlCompilerBase compiler = CreateSqlCompiler(exp);

            var props =
                new List<ProviderPropertyExpression>(MergeProviderProperties(compiler.ProviderProperties));

            IDbCommand cmd = DbUtility.CreateCommand();
            cmd.CommandText = GenerateSql(props, compiler);
            cmd.CommandType = CommandType.Text;

            foreach (IDataParameter p in compiler.ParameterCache.Values)
            {
                cmd.Parameters.Add(p);
            }

            return cmd;
        }

        /* TODO: Add order by from ProviderPropertyExpression */

        protected virtual string GenerateSql(IList<ProviderPropertyExpression> properties,
                                             ExpressionTreeToSqlCompilerBase compiler)
        {
            return String.Format(" {0} SELECT {1} FROM {2} {3} {4} {5}",
                                 compiler.SqlParamDeclarations,
                                 String.IsNullOrEmpty(compiler.SqlColumns)
                                     ? String.Join(",", Enumerable.ToArray(SelectAllColumnNames()))
                                     : compiler.SqlColumns,
                                 compiler.QualifiedTableName,
                                 compiler.SqlJoinClauses,
                                 String.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                 compiler.SqlWhereClause);
        }

        protected TValue GetProviderPropertyValue<TExpression, TValue>(
            IEnumerable<ProviderPropertyExpression> expressions, TValue defaultValue)
            where TExpression : ProviderPropertyExpression<TValue>
        {
            foreach (ProviderPropertyExpression propertyExpression in expressions)
                if (propertyExpression is TExpression)
                    return ((TExpression)propertyExpression).PropertyValueExpression.Value;
            return defaultValue;
        }

        protected abstract ExpressionTreeToSqlCompilerBase CreateSqlCompiler(Expression expression);

        /// <summary>
        /// Converts DataRowCollection to IEnumerable of FeatureDataRow
        /// </summary>
        /// <param name="features"></param>
        /// <returns>IEnumerable</returns>
        private static IEnumerable<FeatureDataRow> IEnumerableOfDataRowCollection(DataRowCollection features)
        {
            if (features == null) throw new ArgumentNullException("features");

            foreach (FeatureDataRow fdr in features)
            {
                var fdrTOid = fdr as FeatureDataRow<TOid>;
                if (fdrTOid != null) yield return fdrTOid;
            }
        }

        protected virtual DbType toDbType(Type type)
        {
            switch (type.FullName)
            {
                case "System.String":
                    return DbType.String;

                case "System.Boolean":
                    return DbType.Boolean;
                case "System.Guid":
                    return DbType.Guid;

                case "System.Byte":
                    return DbType.Byte;
                case "System.SByte":
                    return DbType.AnsiString;

                case "System.Int16":
                    return DbType.Int16;
                case "System.Int32":
                    return DbType.Int32;
                case "System.Int64":
                    return DbType.Int64;

                case "System.UInt16":
                    return DbType.UInt16;
                case "System.UInt32":
                    return DbType.UInt32;
                case "System.UInt64":
                    return DbType.UInt64;

                case "System.Single":
                    return DbType.Single;
                case "System.Double":
                    return DbType.Double;

                case "System.Decimal":
                    return DbType.Decimal;
                case "System.Currency":
                    return DbType.Currency;

                case "System.Object":
                case "System.Byte[]":
                    return DbType.Object;

                case "System.DateTime":
                    return DbType.DateTime;

                default:
                    throw new ArgumentException(string.Format("Do not know how to transorm '{0}' to DbType!",
                                                              type.FullName));
            }
        }

        #region Private helpers for Insert and Update

        protected virtual string InsertClause(IDbCommand cmd)
        {
            var sets = new List<string>();

            //Columnnames
            DataColumnCollection dcc = GetSchemaTable(false).Columns;
            foreach (DataColumn dc in dcc)
                sets.Add(string.Format(" [{0}]", dc.ColumnName));

            String columNames = string.Format("({0}, [{1}])", String.Join(",", sets.ToArray()).Trim(), GeometryColumn);
            sets.Clear();

            //Parameter
            foreach (DataColumn dc in dcc)
            {
                IDataParameter param = null;
                sets.Add(string.Format(" {0}", ParamForColumn(dc, out param)));
                cmd.Parameters.Add(param);
            }

            //Geometry
            sets.Add(string.Format("{0}", string.Format(GeomFromWkbFormatString, "@PGeo")));
            cmd.Parameters.Add(DbUtility.CreateParameter("@PGeo", toDbType(typeof(byte[])), ParameterDirection.Input));

            return String.Format("{0} VALUES({1})", columNames, string.Join(",", sets.ToArray()).Trim());
        }

        protected virtual string UpdateClause(IDbCommand cmd)
        {
            var sets = new List<string>();
            //Attribute
            foreach (DataColumn dc in GetSchemaTable(false).Columns)
            {
                IDataParameter param = null;
                sets.Add(string.Format(" [{0}]={1}", dc.ColumnName, ParamForColumn(dc, out param)));
                cmd.Parameters.Add(param);
            }
            //Geometry
            sets.Add(
                string.Format("[{0}]={1}",
                              GeometryColumn,
                              string.Format(GeomFromWkbFormatString, "@PGeo")));
            cmd.Parameters.Add(DbUtility.CreateParameter("@PGeo", toDbType(typeof(byte[])), ParameterDirection.Input));

            return String.Join(",", sets.ToArray()).Trim();
        }

        protected virtual string WhereClause(IDbCommand cmd)
        {
            cmd.Parameters.Add(DbUtility.CreateParameter("@POldOid", toDbType(default(TOid).GetType()),
                                                         ParameterDirection.Input));
            return string.Format("{0}=@POldOid", OidColumn);
        }

        protected virtual String ParamForColumn(DataColumn dc, out IDataParameter param)
        {
            String paramName = ParamForColumn(dc);
            param = DbUtility.CreateParameter(paramName, toDbType(dc.DataType), ParameterDirection.Input);
            return paramName;
        }

        protected virtual String ParamForColumn(DataColumn dc)
        {
            return string.Format("@P{0}", dc.Ordinal);
        }

        #endregion
    }
}