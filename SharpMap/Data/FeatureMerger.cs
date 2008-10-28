// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection.Emit;
using System.Reflection;
using GeoAPI.Geometries;
using SharpMap.Data;
using System.Collections;

namespace SharpMap.Data
{
    /// <summary>
    /// Represents a merger of two <see cref="FeatureDataTable"/> instances.
    /// </summary>
    internal sealed class FeatureMerger
    {
        #region Nested types
        private delegate Object CreateMergerDelegate(DataTable target, Boolean preserveChanges, MissingSchemaAction action);
        private delegate DataTable MergeSchemaDelegate(Object merger, DataTable source);
        private delegate Object GetKeyIndexDelegate(DataTable table);
        private delegate Object GetDataKeyFromUniqueConstraintDelegate(UniqueConstraint constraint);
        private delegate Object CreateEmptyDataKeyDelegate();
        private delegate Object CreateDataKeyDelegate(DataColumn[] columns, Boolean copyColumns);
        private delegate DataColumn[] GetDataKeyColumnReferenceDelegate(Object dataKey);
        private delegate Object DataKeyGetSortIndex(Object dataKey, DataViewRowState rowState);
        private delegate Boolean GetDataKeyHasValue(Object dataKey);
        private delegate DataColumn CloneDataColumn(DataColumn dataKey);
        #endregion

        #region Static fields
        private static readonly CreateMergerDelegate _createMerger;
        private static readonly MergeSchemaDelegate _mergeSchema;
        private static readonly GetKeyIndexDelegate _getKeyIndex;
        private static readonly GetDataKeyColumnReferenceDelegate _getDataKeyColumnReference;
        private static readonly CreateDataKeyDelegate _createDataKey;
        private static readonly CreateEmptyDataKeyDelegate _createEmptyDataKey;
        private static readonly GetDataKeyFromUniqueConstraintDelegate _getDataKeyFromUniqueConstraint;
        private static readonly DataKeyGetSortIndex _dataKeyGetSortIndex;
        private static readonly GetDataKeyHasValue _getDataKeyHasValue;
        private static readonly CloneDataColumn _cloneDataColumn;
        #endregion

        #region Static constructor
        static FeatureMerger()
        {
            _createMerger = generateCreateMergerDelegate();
            _mergeSchema = generateMergeSchemaDelegate();
            _getKeyIndex = generateGetKeyIndexDelegate();
            _getDataKeyColumnReference = generateGetDataKeyColumnReferenceDelegate();
            _createDataKey = generateCreateDataKeyDelegate();
            _createEmptyDataKey = generateCreateEmptyDataKeyDelegate();
            _getDataKeyFromUniqueConstraint = generateGetDataKeyFromUniqueConstraintDelegate();
            _dataKeyGetSortIndex = generateDataKeyGetSortIndexDelegate();
            _getDataKeyHasValue = generateGetDataKeyHasValueDelegate();
            _cloneDataColumn = generateCloneDataColumnDelegate();
        }
        #endregion

        #region Instance fields
        private readonly FeatureDataSet _targetDataSet;
        private readonly FeatureDataTable _targetDataTable;
        private readonly Object _innerMerger;
        private readonly SchemaMergeAction _mergeAction;
        private readonly Boolean _preserveChanges;
        private readonly Boolean _isTableStandalone;
        private Boolean _ignoreNamespaceForTableLookup;
        private ColumnMapper _columnMap;
        #endregion

        #region Object constructor
        internal FeatureMerger(FeatureDataSet target, Boolean preserveChanges, SchemaMergeAction mergeAction)
        {
            throw new NotImplementedException();
        }

        internal FeatureMerger(FeatureDataTable target, Boolean preserveChanges, SchemaMergeAction mergeAction)
        {
            //if ((SchemaMergeAction.CoerceTypes & mergeAction) != SchemaMergeAction.None)
            //{
            //    throw new NotImplementedException("SchemaMergeAction.ConvertTypes is currently not supported.");
            //}

            if ((SchemaMergeAction.KeyByType & mergeAction) != SchemaMergeAction.None)
            {
                throw new NotImplementedException("SchemaMergeAction.KeyByType is currently not supported.");
            }

            if ((SchemaMergeAction.CaseInsensitive & mergeAction) != SchemaMergeAction.None)
            {
                throw new NotImplementedException("SchemaMergeAction.CaseInsensitive is currently not supported.");
            }

            _isTableStandalone = true;
            _targetDataTable = target;
            _preserveChanges = preserveChanges;
            _mergeAction = mergeAction;
            _innerMerger = createInnerMerger(target, preserveChanges, mergeAction);
        }
        #endregion

        internal void MergeFeature(IFeatureDataRecord record, IGeometryFactory factory)
        {
            if (record == null) throw new ArgumentNullException("record");

            Boolean checkForTarget = _targetDataTable.Rows.Count > 0 &&
                                    (_targetDataTable.PrimaryKey.Length > 0);

            mergeSchemaIfNeeded(record, factory);

            FeatureDataRow targetFeature = null;

            if (checkForTarget && record.HasOid)
            {
                targetFeature = _targetDataTable.Find(record.GetOid());
            }

            mergeFeature(_targetDataTable, record, targetFeature, _columnMap, PreserveChanges);
        }

        internal void MergeFeatures(FeatureDataTable source)
        {
            Boolean enforceConstraints = false;

            if (!_isTableStandalone)
            {
                // If the source table's DataSet and the target tables' DataSet are 
                // the same, there is nothing to do
                if (source.DataSet == _targetDataSet)
                {
                    return;
                }

                // Store off the current state of constraint enforcement for the dataset
                // so we can restore them later
                enforceConstraints = _targetDataSet.EnforceConstraints;
                _targetDataSet.EnforceConstraints = false;
            }
            else
            {
                // If the source table and the target tables are 
                // the same, there is nothing to do
                if (source == _targetDataTable)
                {
                    return;
                }

                _targetDataTable.SuspendEnforceConstraints = true;
            }

            if (_targetDataSet != null)
            {
                if ((source.DataSet == null) ||
                    (source.DataSet.Namespace != _targetDataSet.Namespace))
                {
                    _ignoreNamespaceForTableLookup = true;
                }
            }
            else if (((_targetDataTable.DataSet == null) || (source.DataSet == null)) ||
                     (source.DataSet.Namespace != _targetDataTable.DataSet.Namespace))
            {
                _ignoreNamespaceForTableLookup = true;
            }

            MergeSchema(source);
            mergeFeatureData(source);

            FeatureDataTable targetTable = _targetDataTable ?? (_ignoreNamespaceForTableLookup
                                                                    ? _targetDataSet.Tables[source.TableName]
                                                                    : _targetDataSet.Tables[source.TableName, source.Namespace]);

            if (targetTable != null)
            {
                targetTable.EvaluateExpressions();
            }

            if (!_isTableStandalone)
            {
                _targetDataSet.EnforceConstraints = enforceConstraints;
            }
            else
            {
                _targetDataTable.SuspendEnforceConstraints = false;

                try
                {
                    if (_targetDataTable.EnforceConstraints)
                    {
                        _targetDataTable.EnableConstraints();
                    }
                }
                catch (ConstraintException)
                {
                    if (_targetDataTable.DataSet != null)
                    {
                        _targetDataTable.DataSet.EnforceConstraints = false;
                    }

                    throw;
                }
            }
        }

        // TODO: MergeFeatures(FeatureDataTable) and MergeFeatures(IEnumerable<IFeatureDataRecord>) seem to overlap... try refactor
        internal void MergeFeatures(IEnumerable<IFeatureDataRecord> sourceFeatures,
                                    IGeometryFactory factory)
        {
            if (sourceFeatures == null) throw new ArgumentNullException("sourceFeatures");

            _targetDataTable.SuspendIndexEvents();

            Boolean checkedSchema = false;
            Boolean checkForTarget = _targetDataTable.Rows.Count > 0 &&
                                     (_targetDataTable.PrimaryKey.Length > 0);

            try
            {
                foreach (IFeatureDataRecord srcFeature in sourceFeatures)
                {
                    if (!checkedSchema)
                    {
                        checkedSchema = true;

                        mergeSchemaIfNeeded(srcFeature, factory);
                    }

                    FeatureDataRow targetFeature = null;

                    if (checkForTarget && srcFeature.HasOid)
                    {
                        targetFeature = _targetDataTable.Find(srcFeature.GetOid());
                    }

                    mergeFeature(_targetDataTable, srcFeature, targetFeature, _columnMap, PreserveChanges);
                }
            }
            finally
            {
                _targetDataTable.RestoreIndexEvents(true);
            }
        }

        internal void MergeSchema(DataTable source)
        {
            try
            {
                _mergeSchema(_innerMerger, source);
            }
            catch (NullReferenceException ex)
            {
                if (source == null) throw;

                if (source.PrimaryKey.Length > 0 && _targetDataTable.PrimaryKey.Length > 0)
                {
                    throw new DataException("Possible incompatible keys for merging schema", ex);
                }
            }
        }

        internal Boolean PreserveChanges
        {
            get { return _preserveChanges; }
        }

        internal SchemaMergeAction SchemaMergeAction
        {
            get { return _mergeAction; }
        }

        #region Private helper methods
        private void mergeSchemaIfNeeded(IFeatureDataRecord record, IGeometryFactory factory)
        {
            if ((SchemaMergeAction & SchemaMergeAction.AddAll) != SchemaMergeAction.None)
            {
                // HACK: Parsing and setting the schema should be less clunky here.
                //       We probably need to just do the schema merge ourselves without 
                //       having to rely on System.Data.Merger

                FeatureDataTable schemaModel = record is FeatureDataRow
                                                   ? (record as FeatureDataRow).Table
                                                   : createModelFromFeature(record, factory);

                MergeSchema(schemaModel);

                Int32 sourceCount = schemaModel.Columns.Count;
                Int32 targetCount = _targetDataTable.Columns.Count;

                if (targetCount != sourceCount)
                {
                    _columnMap = generateColumnMapper(schemaModel);
                    return;

                }

                for (Int32 i = 0; i < _targetDataTable.Columns.Count; i++)
                {
                    if (_targetDataTable.Columns[i].Ordinal != schemaModel.Columns[i].Ordinal)
                    {
                        _columnMap = generateColumnMapper(schemaModel);
                        return;
                    }
                }
            }

            _columnMap = generateDefaultColumnMapper();
        }

        private class ColumnMapper
        {
            private readonly Dictionary<Int32, Int32> _columnMap = new Dictionary<Int32, Int32>();
            private readonly Object[] _sourceValues;
            private readonly Object[] _targetValues;

            public ColumnMapper(Int32 count)
            {
                _sourceValues = new Object[count];
            }

            public ColumnMapper(Int32 sourceColumnCount, Int32 targetColumnCount)
            {
                _sourceValues = new Object[sourceColumnCount];
                _targetValues = new Object[targetColumnCount];
            }

            public void AddMapping(Int32 from, Int32 to)
            {
                _columnMap[from] = to;
            }

            public Object[] SourceValues
            {
                get { return _sourceValues; }
            }

            public Object[] TargetValues
            {
                get { return _targetValues ?? _sourceValues; }
            }

            private Boolean IsStraightMapping
            {
                get { return _targetValues == null; }
            }

            public Object[] Map()
            {
                if (!IsStraightMapping)
                {
                    for (Int32 sourceIndex = 0; sourceIndex < _sourceValues.Length; sourceIndex++)
                    {
                        Int32 targetIndex;

                        if (_columnMap.TryGetValue(sourceIndex, out targetIndex))
                        {
                            _targetValues[targetIndex] = _sourceValues[sourceIndex];
                        }
                    }
                }

                return TargetValues;
            }
        }

        private ColumnMapper generateDefaultColumnMapper()
        {
            Int32 count = _targetDataTable.Columns.Count;

            return new ColumnMapper(count);
        }

        private ColumnMapper generateColumnMapper(FeatureDataTable schemaModel)
        {
            Int32 targetCount = _targetDataTable.Columns.Count;
            Int32 sourceCount = schemaModel.Columns.Count;

            ColumnMapper mapper = new ColumnMapper(sourceCount, targetCount);

            foreach (DataColumn column in schemaModel.Columns)
            {
                mapper.AddMapping(column.Ordinal, _targetDataTable.Columns[column.ColumnName].Ordinal);
            }

            return mapper;
        }

        private void mergeFeatureData(FeatureDataTable source)
        {
            _targetDataTable.MergingData = true;

            try
            {
                mergeFeatureTables(source, _targetDataTable);
            }
            finally
            {
                _targetDataTable.MergingData = false;
            }
        }

        private void mergeFeatureTables(FeatureDataTable source, FeatureDataTable target)
        {
            Boolean isTargetEmpty = target.Rows.Count == 0;

            if (source.Rows.Count > 0)
            {
                Object primaryKeyIndex = null;
                Object srcLookupKey = createEmptyDataKey();
                target.SuspendIndexEvents();

                try
                {
                    if (!isTargetEmpty && (target.PrimaryKeyConstraint != null))
                    {
                        srcLookupKey = getSourceKeyOrTargetKeyForSource(source, target);

                        if (getDataKeyHasValue(srcLookupKey))
                        {
                            Object rowLookupKey = getDataKeyFromUniqueConstraint(target.PrimaryKeyConstraint);
                            const DataViewRowState rowState = DataViewRowState.OriginalRows |
                                                              DataViewRowState.Added;
                            primaryKeyIndex = getDataKeySortIndex(rowLookupKey, rowState);
                        }
                    }

                    foreach (FeatureDataRow sourceRow in source.Rows)
                    {
                        FeatureDataRow targetRow = null;

                        if (primaryKeyIndex != null)
                        {
                            targetRow = target.FindMergeTarget(sourceRow,
                                                               srcLookupKey,
                                                               primaryKeyIndex);
                        }

                        //mergeFeature(target, sourceRow, targetRow, PreserveChanges);
                        FeatureDataRow mergedRow = target.MergeRowInternal(sourceRow,
                                                                           targetRow,
                                                                           PreserveChanges,
                                                                           primaryKeyIndex);

                        // If we are adding the geometry to the row or if the geometry changed
                        // then we set it here.
                        if (mergedRow.Geometry == null ||
                            mergedRow.Geometry.EqualsExact(sourceRow.Geometry))
                        {
                            mergedRow.Geometry = sourceRow.Geometry;
                        }

                        mergedRow.IsFullyLoaded = mergedRow.IsFullyLoaded || sourceRow.IsFullyLoaded;
                    }
                }
                finally
                {
                    target.RestoreIndexEvents(true);
                }
            }

            mergeExtendedProperties(source.ExtendedProperties, target.ExtendedProperties, SchemaMergeAction, PreserveChanges);
        }

        private static Object getDataKeySortIndex(Object rowLookupKey, DataViewRowState rowStateFilter)
        {
            return _dataKeyGetSortIndex(rowLookupKey, rowStateFilter);
        }

        private static Boolean getDataKeyHasValue(Object srcKey)
        {
            return _getDataKeyHasValue(srcKey);
        }

        private static Object getSourceKeyOrTargetKeyForSource(FeatureDataTable source,
                                                               FeatureDataTable target)
        {
            // TODO: check if there is code which already does this.
            if (source.PrimaryKeyConstraint != null)
            {
                return getDataKeyFromUniqueConstraint(source.PrimaryKeyConstraint);
            }

            if (target.PrimaryKeyConstraint == null)
            {
                return createEmptyDataKey();
            }

            Object dataKey = getDataKeyFromUniqueConstraint(target.PrimaryKeyConstraint);
            DataColumn[] columnsReference = getDataKeyColumnReference(dataKey);
            DataColumn[] columns = new DataColumn[columnsReference.Length];

            for (Int32 i = 0; i < columnsReference.Length; i++)
            {
                // What about matching type? Does an exception get thrown previously if no match?
                // TODO: This will probably need to be changed to handle the SchemaMergeAction.KeyByType option
                columns[i] = source.Columns[columnsReference[i].ColumnName];
            }

            return createDataKey(columns, false);
        }

        private static DataColumn[] getDataKeyColumnReference(Object dataKey)
        {
            return _getDataKeyColumnReference(dataKey);
        }

        private static Object createDataKey(DataColumn[] columns, Boolean copyColumns)
        {
            return _createDataKey(columns, copyColumns);
        }

        private static Object createEmptyDataKey()
        {
            return _createEmptyDataKey();
        }

        private static Object getDataKeyFromUniqueConstraint(UniqueConstraint uniqueConstraint)
        {
            return _getDataKeyFromUniqueConstraint(uniqueConstraint);
        }

        private static void mergeExtendedProperties(IDictionary src, IDictionary dst, SchemaMergeAction mergeAction, Boolean preserveChanges)
        {
            if (mergeAction == SchemaMergeAction.None)
            {
                return;
            }

            foreach (DictionaryEntry entry in src)
            {
                if (!preserveChanges || (!dst.Contains(entry.Key) || dst[entry.Key] == null))
                {
                    dst[entry.Key] = entry.Value;
                }
            }
        }

        #endregion

        #region Private static helpers
        private static FeatureDataTable createModelFromFeature(IFeatureDataRecord srcFeature,
                                                               IGeometryFactory factory)
        {
            FeatureDataTable schemaModel = new FeatureDataTable(factory);

            for (Int32 fieldIndex = 0; fieldIndex < srcFeature.FieldCount; fieldIndex++)
            {
                schemaModel.Columns.Add(srcFeature.GetName(fieldIndex),
                                        srcFeature.GetFieldType(fieldIndex));
            }

            return schemaModel;
        }

        private static void mergeFeature(FeatureDataTable target,
                                         IFeatureDataRecord srcFeature,
                                         FeatureDataRow targetFeature,
                                         ColumnMapper columnMapper,
                                         Boolean preserveChanges)
        {
            if (targetFeature == null)
            {
                targetFeature = target.NewRow();
                setFeatureRowFromIFeatureDataRecord(srcFeature, targetFeature, columnMapper);
                target.AddRow(targetFeature);
            }
            else
            {
                if (preserveChanges && targetFeature.RowState == DataRowState.Modified)
                {
                    throw new NotImplementedException("Merging updates to original features " +
                                                      "state not yet implemented.");
                }

                setFeatureRowFromIFeatureDataRecord(srcFeature, targetFeature, columnMapper);
            }
        }

        // FIX_PERF
        private static void setFeatureRowFromIFeatureDataRecord(IFeatureDataRecord srcFeature,
                                                                FeatureDataRow targetFeature,
                                                                ColumnMapper columnMapper)
        {
            //for (Int32 i = 0; i < srcFeature.FieldCount; i++)
            //{
            //    String colName = srcFeature.GetName(i);
            //    targetFeature[colName] = srcFeature.GetValue(i);
            //}

            srcFeature.GetValues(columnMapper.SourceValues);
            targetFeature.ItemArray = columnMapper.Map();
            targetFeature.Geometry = srcFeature.Geometry;
            targetFeature.IsFullyLoaded = targetFeature.IsFullyLoaded || srcFeature.IsFullyLoaded;
        }

        private static Object createInnerMerger(DataTable target,
                                                Boolean preserveChanges,
                                                SchemaMergeAction schemaMergeAction)
        {
            MissingSchemaAction missingSchemaAction = MissingSchemaAction.Error;

            if ((Int32)(schemaMergeAction & SchemaMergeAction.AddAll) != 0)
            {
                missingSchemaAction = MissingSchemaAction.Add;
            }

            if ((Int32)(schemaMergeAction & SchemaMergeAction.Key) != 0)
            {
                missingSchemaAction = MissingSchemaAction.AddWithKey;
            }

            return _createMerger(target, preserveChanges, missingSchemaAction);
        }

        //private static Object getKeyIndex(FeatureDataTable table)
        //{
        //    return _getKeyIndex(table);
        //}

        private const MethodAttributes PublicStaticMethod = MethodAttributes.Public |
                                                            MethodAttributes.Static;
        private const BindingFlags NonPublicInstance = BindingFlags.NonPublic |
                                                       BindingFlags.Instance;
        private static CreateMergerDelegate generateCreateMergerDelegate()
        {
            Type[] ctorParams = new Type[] { 
                                             typeof(DataTable), 
                                             typeof(Boolean), 
                                             typeof(MissingSchemaAction) 
                                           };

            DynamicMethod createMergerMethod = new DynamicMethod("Merger_Create",
                                                                 PublicStaticMethod,
                                                                 CallingConventions.Standard,
                                                                 typeof(Object),
                                                                 ctorParams,
                                                                 AdoNetInternalTypes.MergerType,
                                                                 false);

            ILGenerator il = createMergerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            ConstructorInfo ctor = AdoNetInternalTypes.MergerType.GetConstructor(NonPublicInstance,
                                                                                 null,
                                                                                 ctorParams,
                                                                                 null);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Isinst, typeof(Object));
            il.Emit(OpCodes.Ret);

            CreateMergerDelegate d
                = (CreateMergerDelegate)createMergerMethod.CreateDelegate(typeof(CreateMergerDelegate));
            return d;
        }

        private static MergeSchemaDelegate generateMergeSchemaDelegate()
        {
            DynamicMethod mergeSchemaMethod = new DynamicMethod("Merger_MergeSchema",
                                                                PublicStaticMethod,
                                                                CallingConventions.Standard,
                                                                typeof(DataTable),
                                                                new Type[] { typeof(Object), typeof(DataTable) },
                                                                AdoNetInternalTypes.MergerType,
                                                                false);

            ILGenerator il = mergeSchemaMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, AdoNetInternalTypes.MergerType);
            il.Emit(OpCodes.Ldarg_1);
            MethodInfo mergeSchemaInfo = AdoNetInternalTypes.MergerType.GetMethod("MergeSchema",
                                                                                  NonPublicInstance);
            il.Emit(OpCodes.Call, mergeSchemaInfo);
            il.Emit(OpCodes.Ret);

            MergeSchemaDelegate d
                = (MergeSchemaDelegate)mergeSchemaMethod.CreateDelegate(typeof(MergeSchemaDelegate));
            return d;
        }

        private static GetKeyIndexDelegate generateGetKeyIndexDelegate()
        {
            //table.primaryKey.Key.GetSortIndex(DataViewRowState.OriginalRows | DataViewRowState.Added);
            return null;
        }

        private static GetDataKeyFromUniqueConstraintDelegate generateGetDataKeyFromUniqueConstraintDelegate()
        {
            DynamicMethod getDataKeyFromUniqueConstraintMethod = new DynamicMethod(
                                                                "FeatureMerger_GetDataKeyFromUniqueConstraint",
                                                                PublicStaticMethod,
                                                                CallingConventions.Standard,
                                                                typeof(Object),
                                                                new Type[] { typeof(UniqueConstraint) },
                                                                typeof(UniqueConstraint),
                                                                false);

            ILGenerator il = getDataKeyFromUniqueConstraintMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            PropertyInfo keyInfo = typeof(UniqueConstraint).GetProperty("Key", NonPublicInstance);
            il.Emit(OpCodes.Call, keyInfo.GetGetMethod(true));
            il.Emit(OpCodes.Box, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ret);

            GetDataKeyFromUniqueConstraintDelegate d
                = getDataKeyFromUniqueConstraintMethod.CreateDelegate(typeof(GetDataKeyFromUniqueConstraintDelegate))
                    as GetDataKeyFromUniqueConstraintDelegate;

            return d;
        }

        private static CreateEmptyDataKeyDelegate generateCreateEmptyDataKeyDelegate()
        {
            DynamicMethod createEmptyDataKeyMethod = new DynamicMethod("FeatureMerger_CreateEmptyDataKey",
                                                                 PublicStaticMethod,
                                                                 CallingConventions.Standard,
                                                                 typeof(Object),
                                                                 null,
                                                                 AdoNetInternalTypes.DataKeyType,
                                                                 false);

            ILGenerator il = createEmptyDataKeyMethod.GetILGenerator();
            LocalBuilder newKeyLocal = il.DeclareLocal(AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ldloca_S, newKeyLocal);
            il.Emit(OpCodes.Initobj, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Box, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ret);

            CreateEmptyDataKeyDelegate d
                = createEmptyDataKeyMethod.CreateDelegate(typeof(CreateEmptyDataKeyDelegate))
                    as CreateEmptyDataKeyDelegate;

            return d;
        }

        private static CreateDataKeyDelegate generateCreateDataKeyDelegate()
        {
            DynamicMethod createDataKeyMethod = new DynamicMethod("FeatureMerger_CreateDataKey",
                                                                 PublicStaticMethod,
                                                                 CallingConventions.Standard,
                                                                 typeof(Object),
                                                                 new Type[] { typeof(DataColumn[]), typeof(Boolean) },
                                                                 AdoNetInternalTypes.DataKeyType,
                                                                 false);

            ILGenerator il = createDataKeyMethod.GetILGenerator();
            LocalBuilder newDataKey = il.DeclareLocal(AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ldloca_S, newDataKey);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            Type[] ctorParams = new Type[] { typeof(DataColumn[]), typeof(Boolean) };
            ConstructorInfo ctor = AdoNetInternalTypes.DataKeyType.GetConstructor(NonPublicInstance,
                                                                                  null,
                                                                                  ctorParams,
                                                                                  null);
            il.Emit(OpCodes.Call, ctor);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Box, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ret);

            CreateDataKeyDelegate d
                = createDataKeyMethod.CreateDelegate(typeof(CreateDataKeyDelegate))
                    as CreateDataKeyDelegate;

            return d;
        }

        private static GetDataKeyColumnReferenceDelegate generateGetDataKeyColumnReferenceDelegate()
        {
            DynamicMethod getDataKeyColumnReferenceMethod = new DynamicMethod(
                                                                "FeatureMerger_GetDataKeyColumnReference",
                                                                PublicStaticMethod,
                                                                CallingConventions.Standard,
                                                                typeof(DataColumn[]),
                                                                new Type[] { typeof(Object) },
                                                                AdoNetInternalTypes.DataKeyType,
                                                                false);

            ILGenerator il = getDataKeyColumnReferenceMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Unbox_Any, AdoNetInternalTypes.DataKeyType);
            PropertyInfo columnsReferenceInfo = AdoNetInternalTypes.DataKeyType.GetProperty("ColumnsReference",
                                                                                            NonPublicInstance);
            MethodInfo getMethod = columnsReferenceInfo.GetGetMethod(true);
            il.Emit(OpCodes.Call, getMethod);
            il.Emit(OpCodes.Isinst, typeof(Object));
            il.Emit(OpCodes.Ret);

            GetDataKeyColumnReferenceDelegate d
                = getDataKeyColumnReferenceMethod.CreateDelegate(typeof(GetDataKeyColumnReferenceDelegate))
                    as GetDataKeyColumnReferenceDelegate;

            return d;
        }

        private static DataKeyGetSortIndex generateDataKeyGetSortIndexDelegate()
        {
            DynamicMethod dataKeyGetSortIndex = new DynamicMethod("FeatureMerger_DataKeyGetSortIndex_DynamicMethod",
                                                                PublicStaticMethod,
                                                                CallingConventions.Standard,
                                                                typeof(Object),
                                                                new Type[] { typeof(Object), typeof(DataViewRowState) },
                                                                AdoNetInternalTypes.DataKeyType,
                                                                false);

            ILGenerator il = dataKeyGetSortIndex.GetILGenerator();
            LocalBuilder localKey = il.DeclareLocal(AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Unbox_Any, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Stloc_0);
            Type[] methodParams = new Type[] { typeof(DataViewRowState) };
            MethodInfo getSortIndexInfo = AdoNetInternalTypes.DataKeyType.GetMethod("GetSortIndex",
                                                                                    NonPublicInstance,
                                                                                    null,
                                                                                    methodParams,
                                                                                    null);
            il.Emit(OpCodes.Ldloca_S, localKey);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, getSortIndexInfo);
            il.Emit(OpCodes.Isinst, typeof(Object));
            il.Emit(OpCodes.Ret);

            DataKeyGetSortIndex d = dataKeyGetSortIndex.CreateDelegate(typeof(DataKeyGetSortIndex))
                as DataKeyGetSortIndex;

            return d;
        }

        private static GetDataKeyHasValue generateGetDataKeyHasValueDelegate()
        {
            DynamicMethod getDataKeyHasValue = new DynamicMethod(
                                                            "FeatureMerger_DataKey_get_HasValue_DynamicMethod",
                                                            PublicStaticMethod,
                                                            CallingConventions.Standard,
                                                            typeof(Boolean),
                                                            new Type[] { typeof(Object) },
                                                            AdoNetInternalTypes.DataKeyType,
                                                            false);

            ILGenerator il = getDataKeyHasValue.GetILGenerator();
            LocalBuilder localKey = il.DeclareLocal(AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Unbox_Any, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Stloc_0);
            PropertyInfo hasValueInfo = AdoNetInternalTypes.DataKeyType.GetProperty("HasValue",
                                                                                    NonPublicInstance,
                                                                                    null,
                                                                                    typeof(Boolean),
                                                                                    Type.EmptyTypes,
                                                                                    null);
            il.Emit(OpCodes.Ldloca_S, localKey);
            il.Emit(OpCodes.Call, hasValueInfo.GetGetMethod(true));
            il.Emit(OpCodes.Ret);

            GetDataKeyHasValue d = getDataKeyHasValue.CreateDelegate(typeof(GetDataKeyHasValue))
                as GetDataKeyHasValue;

            return d;
        }

        private static CloneDataColumn generateCloneDataColumnDelegate()
        {
            DynamicMethod cloneDataColumn = new DynamicMethod(
                                                            "DataColumn_Clone_DynamicMethod",
                                                            PublicStaticMethod,
                                                            CallingConventions.Standard,
                                                            typeof(DataColumn),
                                                            new Type[] { typeof(DataColumn) },
                                                            typeof(DataColumn),
                                                            false);

            ILGenerator il = cloneDataColumn.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            MethodInfo cloneInfo = typeof(DataColumn).GetMethod("Clone", NonPublicInstance);
            il.Emit(OpCodes.Call, cloneInfo);
            il.Emit(OpCodes.Ret);

            CloneDataColumn d = cloneDataColumn.CreateDelegate(typeof(CloneDataColumn))
                as CloneDataColumn;

            return d;
        }
        #endregion

        #region Schema Merge Experimental

        //private SchemaMap _schemaMap = new SchemaMap();

        //private delegate Object Converter(Object input);

        //private class SchemaMap
        //{
        //    private readonly List<SchemaMapColumnMatch> _columnMatches = new List<SchemaMapColumnMatch>();
        //    private readonly List<SchemaMapKeyMatch> _keyMatches = new List<SchemaMapKeyMatch>();

        //    public void AddColumnMatch(DataColumn src, DataColumn dst)
        //    {
        //        AddColumnMatch(src, dst, null);
        //    }

        //    public void AddColumnMatch(DataColumn src, DataColumn dst, Converter converter)
        //    {
        //        SchemaMapColumnMatch match = new SchemaMapColumnMatch(src, dst, converter);
        //        _columnMatches.Add(match);
        //    }

        //    public void AddKeyMatch(DataColumn src, DataColumn dst)
        //    {
        //        SchemaMapKeyMatch match = new SchemaMapKeyMatch(src, dst);
        //        _keyMatches.Add(match);
        //    }

        //    public SchemaMapColumnMatch GetColumnMatch(Int32 index)
        //    {
        //        return _columnMatches[index];
        //    }

        //    public SchemaMapKeyMatch GetKeyMatch(Int32 index)
        //    {
        //        return _keyMatches[index];
        //    }
        //}

        //private class SchemaMapKeyMatch
        //{
        //    private readonly DataColumn _srcColumn;
        //    private readonly DataColumn _dstColumn;

        //    public SchemaMapKeyMatch(DataColumn src, DataColumn dst)
        //    {
        //        _srcColumn = src;
        //        _dstColumn = dst;
        //    }

        //    public DataColumn Destination
        //    {
        //        get { return _dstColumn; }
        //    }

        //    public DataColumn Source
        //    {
        //        get { return _srcColumn; }
        //    }
        //}

        //private class SchemaMapColumnMatch
        //{
        //    private readonly DataColumn _srcColumn;
        //    private readonly DataColumn _dstColumn;
        //    private readonly Converter _converter;

        //    public SchemaMapColumnMatch(DataColumn src, DataColumn dst, Converter converter)
        //    {
        //        _srcColumn = src;
        //        _converter = converter;
        //        _dstColumn = dst;
        //    }

        //    public Converter Converter
        //    {
        //        get { return _converter; }
        //    }

        //    public DataColumn Destination
        //    {
        //        get { return _dstColumn; }
        //    }

        //    public DataColumn Source
        //    {
        //        get { return _srcColumn; }
        //    }
        //}

        //private static DataTable mergeSchema(DataTable source, DataSet targetDataSet, DataTable target, SchemaMergeAction mergeAction, Boolean preserveChanges)
        //{
        //    Boolean ignoreCase = ((Int32)(mergeAction & SchemaMergeAction.CaseInsensitive) != 0);
        //    DataTable targetTable = null;

        //    if (targetDataSet != null)
        //    {
        //        if (tablesContain(targetDataSet, ignoreCase, source.TableName))
        //        {
        //            targetTable = targetDataSet.Namespace != source.Namespace
        //                            ? targetDataSet.Tables[source.TableName]
        //                            : targetDataSet.Tables[source.TableName, source.Namespace];
        //        }
        //    }
        //    else
        //    {
        //        targetTable = target;
        //    }

        //    if (targetTable == null)    // This will be true when the targetDataSet doesn't have a table by the same name
        //    {
        //        throw new NotImplementedException("DataSet merges not yet implemented.");
        //    }
        //    else
        //    {
        //        if (mergeAction == SchemaMergeAction.None)
        //        {
        //            mergeExtendedProperties(source.ExtendedProperties, targetTable.ExtendedProperties, mergeAction, preserveChanges);
        //            return targetTable;
        //        }

        //        Int32 initialTargetCount = targetTable.Columns.Count;
        //        Boolean matchIfConvertible = (Int32)(mergeAction & SchemaMergeAction.MatchIfConvertible) != 0;
        //        Boolean includeKey = (Int32)(mergeAction & SchemaMergeAction.Key) != 0;
        //        Boolean add = (Int32)(mergeAction & SchemaMergeAction.AddAll) != 0;
        //        Boolean replace = (Int32)(mergeAction & SchemaMergeAction.Replace) != 0;

        //        for (int i = 0; i < source.Columns.Count; i++)
        //        {
        //            DataColumn src = source.Columns[i];
        //            DataColumn dest = columnsContain(targetTable, source, src.ColumnName, src.DataType, ignoreCase, matchIfConvertible, includeKey)
        //                                            ? targetTable.Columns[src.ColumnName]
        //                                            : null;
        //            if (dest == null)
        //            {
        //                // rematch using type coersion to System.Object to ensure match
        //                dest = columnsContain(targetTable, src.ColumnName, typeof(Object), ignoreCase, true, includeKey)
        //                                            ? targetTable.Columns[src.ColumnName]
        //                                            : null;

        //                if (dest != null && replace)
        //                {
        //                    // Find common type to convert to (widen for numeric and character data, cast up for objects)
        //                    // Add new column with found type
        //                    // Copy existing dest data to new column from old column
        //                    // Remove original destination column
        //                    throw new NotImplementedException("Schema replacement not yet implemented");
        //                }
        //            }

        //            if (dest == null)
        //            {
        //                if (add)
        //                {
        //                    dest = _cloneDataColumn(src);
        //                    targetTable.Columns.Add(dest);
        //                }
        //                else if (!isKey(source, src) || (includeKey && isKey(source, src)))
        //                {
        //                    // TODO: raise merge failed event
        //                    throw new DataException("Merge failed, can't match column: " + src.ColumnName);
        //                }
        //            }
        //            else
        //            {
        //                if (matchIfConvertible)
        //                {
        //                    throw new NotImplementedException("Schema matching on convertible types not yet implemented");
        //                }
        //                else if (dest.DataType != src.DataType ||
        //                        ((dest.DataType == typeof(DateTime)) &&
        //                        (dest.DateTimeMode != src.DateTimeMode) &&
        //                        ((dest.DateTimeMode & src.DateTimeMode) != DataSetDateTime.Unspecified)))
        //                {
        //                    throw new DataException("Merge failed, data type mismatch: " + src.ColumnName);
        //                }

        //                mergeExtendedProperties(src.ExtendedProperties, dest.ExtendedProperties, mergeAction, preserveChanges);
        //            }

        //            // Set expressions 
        //            if (targetDataSet != null)
        //            {
        //                throw new NotImplementedException("DataSet merges not yet implemented.");
        //                //for (int i = initialTargetCount; i < targetTable.Columns.Count; i++)
        //                //{
        //                //    targetTable.Columns[i].Expression = source.Columns[targetTable.Columns[i].ColumnName].Expression;
        //                //}
        //            }

        //            // 
        //            DataColumn[] targetKey = targetTable.PrimaryKey;
        //            DataColumn[] sourceKey = source.PrimaryKey;

        //            if (targetKey.Length != sourceKey.Length)
        //            {
        //                // special case when the target table does not have the PrimaryKey

        //                if (targetKey.Length == 0)
        //                {
        //                    DataColumn[] key = new DataColumn[sourceKey.Length];

        //                    for (Int32 i = 0; i < sourceKey.Length; i++)
        //                    {
        //                        key[i] = targetTable.Columns[sourceKey[i].ColumnName];
        //                    }

        //                    targetTable.PrimaryKey = key;
        //                }
        //                else if (sourceKey.Length != 0)
        //                {
        //                    targetDataSet.RaiseMergeFailed(targetTable, Res.GetString(Res.DataMerge_PrimaryKeyMismatch), missingSchemaAction);
        //                }
        //            }
        //            else
        //            {
        //                for (int i = 0; i < targetKey.Length; i++)
        //                {
        //                    if (String.Compare(targetKey[i].ColumnName, sourceKey[i].ColumnName, false, targetTable.Locale) != 0)
        //                    {
        //                        targetDataSet.RaiseMergeFailed(source,
        //                            Res.GetString(Res.DataMerge_PrimaryKeyColumnsMismatch, targetKey[i].ColumnName, sourceKey[i].ColumnName),
        //                            missingSchemaAction
        //                    );
        //                    }
        //                }
        //            }
        //        }

        //        mergeExtendedProperties(source.ExtendedProperties, targetTable.ExtendedProperties, mergeAction, preserveChanges);
        //    }

        //    return targetTable;
        //}

        //private static Boolean isKey(DataTable table, DataColumn column)
        //{
        //    return Array.Exists(table.PrimaryKey, delegate(DataColumn find) { return ReferenceEquals(find, column); });
        //}

        //private static Boolean tablesContain(DataSet targetDataSet, Boolean ignoreCase, String name)
        //{
        //    StringComparer comparer = StringComparer.Create(targetDataSet.Locale ?? CultureInfo.CurrentCulture, ignoreCase);

        //    foreach (DataTable table in targetDataSet.Tables)
        //    {
        //        if (comparer.Compare(table.TableName, name) == 0)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        //private static Boolean columnsContain(DataTable table, String columnName, Type columnType, Boolean ignoreCase, Boolean matchIfConvertible, Boolean includeKey)
        //{
        //    StringComparer comparer = StringComparer.Create(table.Locale ?? CultureInfo.CurrentCulture, ignoreCase);

        //    foreach (DataColumn column in table.Columns)
        //    {
        //        if (comparer.Compare(column.ColumnName, columnName) == 0)
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}
        #endregion
    }
}