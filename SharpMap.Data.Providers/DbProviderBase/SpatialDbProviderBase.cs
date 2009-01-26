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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db.Expressions;
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
#endif

namespace SharpMap.Data.Providers.Db
{
    public abstract class SpatialDbProviderBase<TOid>
        : FeatureProviderBase, IWritableFeatureProvider<TOid>, ISpatialDbProvider
    {
        private readonly int? _geometryFactorySridInt;
        protected int? SridInt
        {
            get
            {
                return _geometryFactorySridInt;
            }
        }

        public ICoordinateTransformationFactory CoordinateTransformationFactory { get; set; }

        private static readonly Dictionary<TableCacheKey, DataTable> _cachedSchemas =
            new Dictionary<TableCacheKey, DataTable>();

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
            : this(dbUtility, geometryFactory, connectionString, tableSchema, tableName, oidColumn, geometryColumn, null)
        {

        }

        protected SpatialDbProviderBase(IDbUtility dbUtility,
                                        IGeometryFactory geometryFactory,
                                        String connectionString,
                                        String tableSchema,
                                        String tableName,
                                        String oidColumn,
                                        String geometryColumn, ICoordinateTransformationFactory coordinateTransformationFactory)
        {
            DbUtility = dbUtility;
            GeometryFactory = geometryFactory.Clone();
            OriginalSpatialReference = GeometryFactory.SpatialReference;
            OriginalSrid = GeometryFactory.Srid;

            if (geometryFactory.SpatialReference != null)
                _geometryFactorySridInt = SridMap.DefaultInstance.Process(geometryFactory.SpatialReference, (int?)null);

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

            CoordinateTransformationFactory = coordinateTransformationFactory;

            ICoordinateSystem cs;
            string srid;

            ReadSpatialReference(out cs, out srid);

            OriginalSpatialReference = cs;
            OriginalSrid = srid;
            GeometryFactory.SpatialReference = SpatialReference;
            GeometryFactory.Srid = Srid;
        }

        protected abstract void ReadSpatialReference(out ICoordinateSystem cs, out string srid);

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

        #region ISpatialDbProvider Members

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

        public IDbUtility DbUtility { get; protected set; }
        public abstract String GeometryColumnConversionFormatString { get; }
        public abstract String GeomFromWkbFormatString { get; }

        public virtual String QualifiedTableName
        {
            get { return QualifyTableName(TableSchema, Table); }
        }


        public ProviderPropertiesExpression DefaultProviderProperties { get; set; }

        #endregion

        #region IWritableFeatureProvider<TOid> Members

        public void Insert(FeatureDataRow<TOid> feature)
        {
            Insert((IEnumerable<FeatureDataRow<TOid>>)new[] { feature });
        }

        public virtual void Insert(IEnumerable<FeatureDataRow<TOid>> features)
        {
            OgcGeometryType geometryType = OgcGeometryType.Unknown;

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (IDbTransaction tran = conn.BeginTransaction())
                {
                    IDbCommand cmd = DbUtility.CreateCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = tran;

                    cmd.CommandText = string.Format(
                        "INSERT INTO {0} {1};", QualifiedTableName, InsertClause(cmd));

                    foreach (FeatureDataRow<TOid> row in features)
                    {
                        cmd.Parameters.Clear();

                        for (int i = 0; i < row.FieldCount; i++)
                        {
                            cmd.Parameters.Add(DbUtility.CreateParameter(string.Format("P{0}", i), row[i],
                                                                         ParameterDirection.Input));
                        }

                        cmd.Parameters.Add(DbUtility.CreateParameter("PGeo", row.Geometry.AsBinary(),
                                                                     ParameterDirection.Input));

                        //for (int i = 0; i < cmd.Parameters.Count - 1; i++)
                        //    ((IDataParameter)cmd.Parameters[i]).Value = row[i];
                        //byte[] geomBinary = row.Geometry.AsBinary();
                        //((IDataParameter)cmd.Parameters["@PGeo"]).Value = geomBinary;

                        if (geometryType == OgcGeometryType.Unknown)
                            geometryType = row.Geometry.GeometryType;

                        if (row.Geometry.GeometryType == geometryType)
                            cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

            }
        }

        public void Update(FeatureDataRow<TOid> feature)
        {
            Update((IEnumerable<FeatureDataRow<TOid>>)new[] { feature });
        }

        public virtual void Update(IEnumerable<FeatureDataRow<TOid>> features)
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

        public void Delete(FeatureDataRow<TOid> feature)
        {
            Delete((IEnumerable<FeatureDataRow<TOid>>)new[] { feature });
        }

        public virtual void Delete(IEnumerable<FeatureDataRow<TOid>> features)
        {
            var featureIds = new List<TOid>();
            foreach (var fdr in features)
            {
                featureIds.Add(fdr.Id);
            }

            ExpressionTreeToSqlCompilerBase<TOid> compiler = CreateSqlCompiler(null);
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
                                            Processor.Select(featureIds,
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
                    PrepareSelectCommand(
                        new FeatureQueryExpression(new AttributesProjectionExpression(new[] { OidColumn }),
                                                   (AttributeBinaryExpression)null, query))))
            {
                yield return (TOid)fdr.GetOid();
            }
        }

        public IGeometry GetGeometryByOid(TOid oid)
        {
            var exp
                = new FeatureQueryExpression(
                    new AttributesProjectionExpression(
                        new[]
                            {
                                GeometryColumn
                            }), null, null, new OidCollectionExpression(new[] { oid }));


            using (IFeatureDataReader reader = ExecuteFeatureQuery(exp))
            {
                foreach (IFeatureDataRecord fdr in reader)
                    return fdr.Geometry.Clone();
            }
            return null;
        }

        public IFeatureDataRecord GetFeatureByOid(TOid oid)
        {
            var exp
                = new FeatureQueryExpression(
                    new AllAttributesExpression(), null, null, new OidCollectionExpression(new[] { oid }));

            using (IFeatureDataReader reader = ExecuteFeatureQuery(exp))
            {
                FeatureDataTable fdt = new FeatureDataTable<TOid>("features", OidColumn, GeometryFactory);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    if (name != OidColumn)
                    {
                        fdt.Columns.Add(name, reader.GetFieldType(i));
                    }
                }

                foreach (IFeatureDataRecord fdr in reader)
                {
                    FeatureDataRow row = fdt.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string name = reader.GetName(i);
                        row[name] = reader[i];
                    }
                    if ((fdr as SpatialDbFeatureDataReader).HasGeometry)
                        row.Geometry = reader.Geometry;

                    fdt.AddRow(row);
                    return row;
                }
            }
            return null;
        }

        public void SetTableSchema(FeatureDataTable<TOid> table)
        {
            SetTableSchema(table, SchemaMergeAction.AddAll | SchemaMergeAction.Key);
        }

        public void SetTableSchema(FeatureDataTable<TOid> table, SchemaMergeAction schemaAction)
        {
            DataTable dt = GetSchemaTable();

            table.Merge(dt);
        }

        public override FeatureDataTable CreateNewTable()
        {
            IGeometryFactory fact;
            if (CoordinateTransformation == null)
                fact = GeometryFactory;
            else
            {
                fact = GeometryFactory.Clone();
                fact.SpatialReference = CoordinateTransformation.Target;
                fact.Srid = SridMap.DefaultInstance.Process(fact.SpatialReference, "");
            }

            var tbl = new FeatureDataTable<TOid>(OidColumn, fact);
            SetTableSchema(tbl, SchemaMergeAction.AddAll | SchemaMergeAction.Key);
            return tbl;
        }

        protected override IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query)
        {
            return ExecuteFeatureQuery(query, FeatureQueryExecutionOptions.FullFeature);
        }

        protected override IFeatureDataReader InternalExecuteFeatureQuery(FeatureQueryExpression query, FeatureQueryExecutionOptions options)
        {
            return ExecuteFeatureDataReader(PrepareSelectCommand(query));
        }

        public override Int32 GetFeatureCount()
        {
            var attrs = new AttributesProjectionExpression(new[] { "Count(*)" });

            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            {
                using (
                    IDbCommand cmd =
                        PrepareSelectCommand(DefinitionQuery == null
                                                 ? (Expression)attrs
                                                 : new QueryExpression(attrs, DefinitionQuery)))
                {
                    cmd.Connection = conn;
                    conn.Open();
                    return (Int32)cmd.ExecuteScalar();
                }
            }
        }

        public override DataTable GetSchemaTable()
        {
            return GetSchemaTable(false);
        }


        public override CultureInfo Locale
        {
            get { return CultureInfo.InvariantCulture; }
        }

        public override void SetTableSchema(FeatureDataTable table)
        {
            table.Merge(GetSchemaTable());
        }

        public override String ConnectionId
        {
            get { return ConnectionString; }
        }

        public override Object ExecuteQuery(Expression query)
        {
            return ExecuteFeatureDataReader(PrepareSelectCommand(query));
        }

        public IExtents GetExtentsByOid(TOid oid)
        {
            var query =
                new FeatureQueryExpression(new AttributesProjectionExpression(new[] { GeometryColumn }),
                                           null, null, new OidCollectionExpression(new[] { oid }));
            using (IFeatureDataReader fdr = ExecuteFeatureQuery(query))
            {
                while (fdr.Read())
                {
                    return (IExtents)fdr.Geometry.Extents.Clone();
                }
            }
            return GeometryFactory.CreateExtents();
        }

        public void Insert(FeatureDataRow feature)
        {
            Insert((FeatureDataRow<TOid>)feature);
        }

        public void Insert(IEnumerable<FeatureDataRow> features)
        {
            Insert(Caster.Downcast<FeatureDataRow<TOid>, FeatureDataRow>(features));
        }

        public void Update(FeatureDataRow feature)
        {
            Update((FeatureDataRow<TOid>)feature);
        }

        public void Update(IEnumerable<FeatureDataRow> features)
        {
            Update(Caster.Downcast<FeatureDataRow<TOid>, FeatureDataRow>(features));
        }

        public void Delete(FeatureDataRow feature)
        {
            Delete((FeatureDataRow<TOid>)feature);
        }

        public void Delete(IEnumerable<FeatureDataRow> features)
        {
            Delete(Caster.Downcast<FeatureDataRow<TOid>, FeatureDataRow>(features));
        }

        #endregion

        protected virtual string QualifyTableName(string schema, string table)
        {
            if (string.IsNullOrEmpty(schema))
                return string.Format("[{0}]", table);

            return string.Format("[{0}].[{1}]", schema, table);
        }

        public DataTable GetSchemaTable(bool forceCreateNew)
        {
            if (forceCreateNew)
                RemoveCachedSchema();

            var key = new TableCacheKey(GetType(), ConnectionString, TableSchema, Table);
            DataTable tbl;
            if (!_cachedSchemas.TryGetValue(key, out tbl))
            {
                DataTable tb = BuildSchemaTable(); ///this may take some time
                lock (_cachedSchemas)
                    if (!_cachedSchemas.TryGetValue(key, out tbl))
                    //check again in case a matching schema has been created
                    {
                        _cachedSchemas.Add(key, tb);
                        tbl = tb;
                    }
            }

            return tbl.Clone();
        }

        protected void RemoveCachedSchema()
        {
            var key = new TableCacheKey(GetType(), ConnectionString, TableSchema, Table);
            lock (_cachedSchemas)
            {
                if (_cachedSchemas.ContainsKey(key))
                {
                    _cachedSchemas.Remove(key);
                }
            }
        }

        protected virtual DataTable BuildSchemaTable()
        {
            return BuildSchemaTable(false);
        }

        public virtual string QualifyColumnName(string columnName)
        {
            GuardValueNotNull(Table, "Table");
            GuardValueNotNull(columnName, "ColumnName");

            return string.Format("{0}.[{1}]", QualifiedTableName, columnName);
        }

        protected void GuardValueNotNull<T>(T value, string name)
        {
            if (Equals(value, default(T)) || (typeof(T) == typeof(string)
                                              && string.IsNullOrEmpty(value as string)))
                throw new InvalidOperationException(string.Format("{0} cannot be null.", name));
        }

        public static IEnumerable<ProviderPropertyExpression> MergeProviderProperties(
            IEnumerable<ProviderPropertyExpression> primary, IEnumerable<ProviderPropertyExpression> secondary)
        {
            if (Equals(null, secondary))
                return primary;
            if (Equals(null, primary))
                return secondary;

            var list = new List<ProviderPropertyExpression>();

            Func<ProviderPropertyExpression, ProviderPropertyExpression, bool> predicate
                = (prop1, prop2) => prop1.GetType() == prop2.GetType();

            foreach (ProviderPropertyExpression prop in primary)
                AddToList(list, prop, predicate);

            foreach (ProviderPropertyExpression prop in secondary)
                AddToList(list, prop, predicate);

            return list;
        }

        private static void AddToList<T>(IList<T> storage, T item, Func<T, T, bool> filter)
        {
            if (Enumerable.Count(Processor.Where(storage, i => filter(i, item))) == 0)
                storage.Add(item);
        }

        protected virtual DataTable BuildSchemaTable(Boolean withGeometryColumn)
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.CommandText = string.Format("SELECT * FROM {0} ", QualifiedTableName);
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                IDbDataAdapter da = DbUtility.CreateAdapter(cmd);
                var ds = new DataSet();
                da.FillSchema(ds, SchemaType.Source);
                DataTable dt = ds.Tables[0];

                //remove geometry column from schema
                if (!withGeometryColumn)
                {
                    dt.Columns.Remove(GeometryColumn);
                }

                return dt;
            }
        }


        public void Update(DataRowCollection features)
        {
            Update(IEnumerableOfDataRowCollection(features));
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

        public virtual IEnumerable<string> SelectAllColumnNames()
        {
            yield return GeometryColumn;

            foreach (DataColumn c in GetSchemaTable().Columns)
                yield return c.ColumnName;
        }

        public virtual IEnumerable<string> FormatColumnNames(bool formatGeometryColumn,
                                                             bool qualifyColumnNames, IEnumerable<string> names)
        {
            foreach (string col in names)
            {
                yield return
                    formatGeometryColumn &&
                    String.Equals(col, GeometryColumn, StringComparison.InvariantCultureIgnoreCase)
                        ? String.Format(GeometryColumnConversionFormatString + " AS {1}", col, col)
                        : qualifyColumnNames
                              ? String.Format("{0}.[{1}]", QualifiedTableName, col)
                              : string.Format("[{0}]", col);
            }
        }

        protected virtual IFeatureDataReader ExecuteFeatureDataReader(IDbCommand cmd)
        {
            Debug.WriteLine(String.Format("executing sql : {0}", cmd.CommandText));
            IDbConnection conn = DbUtility.CreateConnection(ConnectionString);
            cmd.Connection = conn;
            if (conn.State == ConnectionState.Closed) conn.Open();
            return new SpatialDbFeatureDataReader(GeometryFactory, cmd.ExecuteReader(CommandBehavior.CloseConnection),
                                                  GeometryColumn, OidColumn) { CoordinateTransformation = CoordinateTransformation };
        }


        protected virtual IDbCommand PrepareSelectCommand(Expression query)
        {
            Expression exp = ExpressionMerger.MergeExpressions(query, DefinitionQuery);

            ExpressionTreeToSqlCompilerBase<TOid> compiler = CreateSqlCompiler(exp);

            var props =
                new List<ProviderPropertyExpression>(
                    MergeProviderProperties(
                        compiler.ProviderProperties,
                        DefaultProviderProperties == null
                            ? null
                            : DefaultProviderProperties.ProviderProperties.Collection));

            IDbCommand cmd = DbUtility.CreateCommand();
            cmd.CommandText = GenerateSelectSql(props, compiler);
            cmd.CommandType = CommandType.Text;

            foreach (IDataParameter p in compiler.ParameterCache.Values)
            {
                cmd.Parameters.Add(p);
            }

            return cmd;
        }


        public int? ParseSrid(string sridText)
        {
            return SridMap.DefaultInstance.Process(sridText, (int?)null);
        }


        protected virtual string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
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
                                                                    o => o.ToString())));

            string orderByClause = string.IsNullOrEmpty(orderByCols) ? "" : " ORDER BY " + orderByCols;

            string mainQueryColumns = string.Join(",", Enumerable.ToArray(
                                                           FormatColumnNames(true, true,
                                                                             compiler.ProjectedColumns.Count > 0
                                                                                 ? compiler.ProjectedColumns
                                                                                 : SelectAllColumnNames()
                                                               )));


            return String.Format(" {0} SELECT {1} FROM {2} {3} {4} {5} {6}",
                                 compiler.SqlParamDeclarations,
                                 mainQueryColumns,
                                 QualifiedTableName,
                                 compiler.SqlJoinClauses,
                                 String.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                 compiler.SqlWhereClause,
                                 orderByClause);
        }

        protected abstract string GenerateSelectSql(IList<ProviderPropertyExpression> properties,
                                                    ExpressionTreeToSqlCompilerBase<TOid> compiler, int pageSize,
                                                    int pageNumber);


        public static TValue GetProviderPropertyValue<TExpression, TValue>(
            IEnumerable<ProviderPropertyExpression> expressions, TValue defaultValue)
            where TExpression : ProviderPropertyExpression<TValue>
        {
            if (expressions == null)
                return defaultValue;

            foreach (ProviderPropertyExpression propertyExpression in expressions)
                if (propertyExpression is TExpression)
                    return ((TExpression)propertyExpression).PropertyValueExpression.Value;
            return defaultValue;
        }

        protected abstract ExpressionTreeToSqlCompilerBase<TOid> CreateSqlCompiler(Expression expression);

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

        #region Private helpers for Insert and Update

        protected virtual string InsertClause(IDbCommand cmd)
        {
            var sets = new List<string>();

            //Columnnames
            DataColumnCollection dcc = GetSchemaTable().Columns;
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
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("@PGeo", ParameterDirection.Input));

            return String.Format("{0} VALUES({1})", columNames, string.Join(",", sets.ToArray()).Trim());
        }

        protected virtual string UpdateClause(IDbCommand cmd)
        {
            var sets = new List<string>();
            //Attribute
            foreach (DataColumn dc in GetSchemaTable().Columns)
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
            cmd.Parameters.Add(DbUtility.CreateParameter<byte[]>("@PGeo", ParameterDirection.Input));

            return String.Join(",", sets.ToArray()).Trim();
        }

        protected virtual string WhereClause(IDbCommand cmd)
        {
            cmd.Parameters.Add(DbUtility.CreateParameter<TOid>("@POldOid", ParameterDirection.Input));
            return string.Format("{0}=@POldOid", OidColumn);
        }

        protected virtual String ParamForColumn(DataColumn dc, out IDataParameter param)
        {
            String paramName = ParamForColumn(dc);
            param = DbUtility.CreateParameter(paramName, dc.DataType, ParameterDirection.Input);
            return paramName;
        }

        protected virtual String ParamForColumn(DataColumn dc)
        {
            return string.Format("@P{0}", dc.Ordinal);
        }

        #endregion

        #region Nested type: TableCacheKey

        private struct TableCacheKey
        {
            private readonly string _connectionString;
            private readonly Type _providerType;
            private readonly string _schema;

            private readonly string _tableName;

            public TableCacheKey(Type providerType, string connectionString, string schema, string tblName)
            {
                _providerType = providerType;
                _tableName = tblName;
                _schema = schema;
                _connectionString = connectionString;
            }

            public string TableName
            {
                get { return _tableName; }
            }

            public string ConnectionString
            {
                get { return _connectionString; }
            }

            public string Schema
            {
                get { return _schema; }
            }

            public Type ProviderType
            {
                get { return _providerType; }
            }

            public override int GetHashCode()
            {
                return _providerType.GetHashCode() ^ _connectionString.ToLower().GetHashCode() ^
                       _schema.ToLower().GetHashCode() ^
                       _tableName.ToLower().GetHashCode();
            }
        }

        #endregion
    }
}