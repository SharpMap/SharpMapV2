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
using System.Diagnostics;
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
            _dataKeyGetSortIndex = generateDataKeyGetSortIndex();
            _getDataKeyHasValue = generateGetDataKeyHasValue();
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
        #endregion

        #region Object constructor
        internal FeatureMerger(FeatureDataSet target, Boolean preserveChanges, SchemaMergeAction mergeAction)
        {
            throw new NotImplementedException();
        }

        internal FeatureMerger(FeatureDataTable target, Boolean preserveChanges, SchemaMergeAction mergeAction)
        {
            if ((SchemaMergeAction.ConvertTypes & mergeAction) != SchemaMergeAction.None)
            {
                throw new NotImplementedException("SchemaMergeAction.ConvertTypes is currently not supported.");
            }

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

            Boolean checkForTarget = _targetDataTable.Rows.Count > 0 && (_targetDataTable.PrimaryKey.Length > 0);

            mergeSchemaIfNeeded(record, factory);

            FeatureDataRow targetFeature = null;

            if (checkForTarget && record.HasOid)
            {
                targetFeature = _targetDataTable.Find(record.GetOid());
            }

            mergeFeature(_targetDataTable, record, targetFeature, PreserveChanges);
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
                if ((source.DataSet == null) || (source.DataSet.Namespace != _targetDataSet.Namespace))
                {
                    _ignoreNamespaceForTableLookup = true;
                }
            }
            else if (((_targetDataTable.DataSet == null) || (source.DataSet == null)) || (source.DataSet.Namespace != _targetDataTable.DataSet.Namespace))
            {
                _ignoreNamespaceForTableLookup = true;
            }

            MergeSchema(source);
            mergeFeatureData(source);

            FeatureDataTable targetTable = _targetDataTable;

            if (targetTable == null)
            {
                if (_ignoreNamespaceForTableLookup)
                {
                    targetTable = _targetDataSet.Tables[source.TableName];
                }
                else
                {
                    targetTable = _targetDataSet.Tables[source.TableName, source.Namespace];
                }
            }

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
        internal void MergeFeatures(IEnumerable<IFeatureDataRecord> sourceFeatures, IGeometryFactory factory)
        {
            if (sourceFeatures == null) throw new ArgumentNullException("sourceFeatures");

            _targetDataTable.SuspendIndexEvents();

            Boolean checkedSchema = false;
            Boolean checkForTarget = _targetDataTable.Rows.Count > 0 && (_targetDataTable.PrimaryKey.Length > 0);

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

                    mergeFeature(_targetDataTable, srcFeature, targetFeature, PreserveChanges);
                }
            }
            finally
            {
                _targetDataTable.RestoreIndexEvents(true);
            }
        }

        internal void MergeSchema(FeatureDataTable source)
        {
            try
            {
                _mergeSchema(_innerMerger, source);
            }
            catch (NullReferenceException ex)
            {
                if (source == null)
                    throw;
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
            if ((SchemaMergeAction & SchemaMergeAction.Add) != SchemaMergeAction.None)
            {
                // HACK: Parsing and setting the schema should be less clunky here.
                //       We probably need to just do the schema merge ourselves without 
                //       having to rely on System.Data.Merger
                FeatureDataTable schemaModel;

                if (record is FeatureDataRow)
                {
                    schemaModel = (record as FeatureDataRow).Table;
                }
                else
                {
                    schemaModel = createModelFromFeature(record, factory);
                }

                MergeSchema(schemaModel);
            }
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

            mergeExtendedProperties(source.ExtendedProperties, target.ExtendedProperties);
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

        private void mergeExtendedProperties(IDictionary src, IDictionary dst)
        {
            if (SchemaMergeAction != SchemaMergeAction.None)
            {
                foreach (DictionaryEntry entry in src)
                {
                    if (!PreserveChanges || (!dst.Contains(entry.Key) || dst[entry.Key] == null))
                    {
                        dst[entry.Key] = entry.Value;
                    }
                }
            }
        }
        #endregion

        #region Private static helpers
        private static FeatureDataTable createModelFromFeature(IFeatureDataRecord srcFeature, IGeometryFactory factory)
        {
            FeatureDataTable schemaModel = new FeatureDataTable(factory);
            for (Int32 fieldIndex = 0; fieldIndex < srcFeature.FieldCount; fieldIndex++)
            {
                schemaModel.Columns.Add(srcFeature.GetName(fieldIndex),
                                        srcFeature.GetFieldType(fieldIndex));
            }

            return schemaModel;
        }

        private static void mergeFeature(FeatureDataTable target, IFeatureDataRecord srcFeature, FeatureDataRow targetFeature, Boolean preserveChanges)
        {
            if (targetFeature == null)
            {
                targetFeature = target.NewRow();
                setFeatureRowFromIFeatureDataRecord(srcFeature, targetFeature);
                target.AddRow(targetFeature);
            }
            else
            {
                if (preserveChanges && targetFeature.RowState == DataRowState.Modified)
                {
                    throw new NotImplementedException("Merging updates to original features state not yet implemented.");
                }
                else
                {
                    setFeatureRowFromIFeatureDataRecord(srcFeature, targetFeature);
                }
            }
        }

        private static void setFeatureRowFromIFeatureDataRecord(IFeatureDataRecord srcFeature, FeatureDataRow targetFeature)
        {
            for (Int32 i = 0; i < srcFeature.FieldCount; i++)
            {
                String colName = srcFeature.GetName(i);
                targetFeature[colName] = srcFeature.GetValue(i);
            }

            targetFeature.Geometry = srcFeature.Geometry;

            targetFeature.IsFullyLoaded = targetFeature.IsFullyLoaded || srcFeature.IsFullyLoaded;
        }

        private static Object createInnerMerger(DataTable target, Boolean preserveChanges, SchemaMergeAction schemaMergeAction)
        {
            MissingSchemaAction missingSchemaAction = MissingSchemaAction.Error;

            if ((Int32)(schemaMergeAction & SchemaMergeAction.Add) != 0)
            {
                missingSchemaAction = MissingSchemaAction.Add;
            }

            if ((Int32)(schemaMergeAction & SchemaMergeAction.Key) != 0)
            {
                missingSchemaAction = MissingSchemaAction.AddWithKey;
            }

            return _createMerger(target, preserveChanges, missingSchemaAction);
        }

        private static Object getKeyIndex(FeatureDataTable table)
        {
            return _getKeyIndex(table);
        }

        private static CreateMergerDelegate generateCreateMergerDelegate()
        {
            Type[] ctorParams = new Type[] { typeof(DataTable), typeof(Boolean), typeof(MissingSchemaAction) };

            DynamicMethod createMergerMethod = new DynamicMethod("Merger_Create",
                                                                 MethodAttributes.Public | MethodAttributes.Static,
                                                                 CallingConventions.Standard,
                                                                 typeof(Object),
                                                                 ctorParams,
                                                                 AdoNetInternalTypes.MergerType,
                                                                 false);

            ILGenerator il = createMergerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            ConstructorInfo ctor = AdoNetInternalTypes.MergerType
                .GetConstructor(flags, null, ctorParams, null);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Isinst, typeof(Object));
            il.Emit(OpCodes.Ret);

            CreateMergerDelegate d = (CreateMergerDelegate)createMergerMethod.CreateDelegate(typeof(CreateMergerDelegate));
            return d;
        }

        private static MergeSchemaDelegate generateMergeSchemaDelegate()
        {
            DynamicMethod mergeSchemaMethod = new DynamicMethod("Merger_MergeSchema",
                                                                MethodAttributes.Public | MethodAttributes.Static,
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
                                                          BindingFlags.Instance | BindingFlags.NonPublic);
            il.Emit(OpCodes.Call, mergeSchemaInfo);
            il.Emit(OpCodes.Ret);

            MergeSchemaDelegate d = (MergeSchemaDelegate)mergeSchemaMethod.CreateDelegate(typeof(MergeSchemaDelegate));
            return d;
        }

        private static GetKeyIndexDelegate generateGetKeyIndexDelegate()
        {
            //table.primaryKey.Key.GetSortIndex(DataViewRowState.OriginalRows | DataViewRowState.Added);
            return null;
        }

        private static GetDataKeyFromUniqueConstraintDelegate generateGetDataKeyFromUniqueConstraintDelegate()
        {
            DynamicMethod getDataKeyFromUniqueConstraintMethod = new DynamicMethod("FeatureMerger_GetDataKeyFromUniqueConstraint",
                                                                MethodAttributes.Public | MethodAttributes.Static,
                                                                CallingConventions.Standard,
                                                                typeof(Object),
                                                                new Type[] { typeof(UniqueConstraint) },
                                                                typeof(UniqueConstraint),
                                                                false);

            ILGenerator il = getDataKeyFromUniqueConstraintMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo keyInfo = typeof(UniqueConstraint).GetProperty("Key", flags);
            il.Emit(OpCodes.Call, keyInfo.GetGetMethod(true));
            il.Emit(OpCodes.Box, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ret);

            GetDataKeyFromUniqueConstraintDelegate d = getDataKeyFromUniqueConstraintMethod
                .CreateDelegate(typeof(GetDataKeyFromUniqueConstraintDelegate)) as GetDataKeyFromUniqueConstraintDelegate;

            return d;
        }

        private static CreateEmptyDataKeyDelegate generateCreateEmptyDataKeyDelegate()
        {
            DynamicMethod createEmptyDataKeyMethod = new DynamicMethod("FeatureMerger_CreateEmptyDataKey",
                                                                 MethodAttributes.Public | MethodAttributes.Static,
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

            CreateEmptyDataKeyDelegate d = createEmptyDataKeyMethod
                .CreateDelegate(typeof(CreateEmptyDataKeyDelegate)) as CreateEmptyDataKeyDelegate;
            
            return d;
        }

        private static CreateDataKeyDelegate generateCreateDataKeyDelegate()
        {
            DynamicMethod createDataKeyMethod = new DynamicMethod("FeatureMerger_CreateDataKey",
                                                                 MethodAttributes.Public | MethodAttributes.Static,
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
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            ConstructorInfo ctor = AdoNetInternalTypes.DataKeyType
                .GetConstructor(flags, null, new Type[] { typeof(DataColumn[]), typeof(Boolean) }, null);
            il.Emit(OpCodes.Call, ctor);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Box, AdoNetInternalTypes.DataKeyType);
            il.Emit(OpCodes.Ret);

            CreateDataKeyDelegate d = createDataKeyMethod
                .CreateDelegate(typeof(CreateDataKeyDelegate)) as CreateDataKeyDelegate;

            return d;
        }

        private static GetDataKeyColumnReferenceDelegate generateGetDataKeyColumnReferenceDelegate()
        {
            DynamicMethod getDataKeyColumnReferenceMethod = new DynamicMethod("FeatureMerger_GetDataKeyColumnReference",
                                                                MethodAttributes.Public | MethodAttributes.Static,
                                                                CallingConventions.Standard,
                                                                typeof(DataColumn[]),
                                                                new Type[] { typeof(Object) },
                                                                AdoNetInternalTypes.DataKeyType,
                                                                false);

            ILGenerator il = getDataKeyColumnReferenceMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Unbox_Any, AdoNetInternalTypes.DataKeyType);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo columnsReferenceInfo = AdoNetInternalTypes.DataKeyType
                .GetProperty("ColumnsReference", flags);
            MethodInfo getMethod = columnsReferenceInfo.GetGetMethod(true);
            il.Emit(OpCodes.Call, getMethod);
            il.Emit(OpCodes.Isinst, typeof(Object));
            il.Emit(OpCodes.Ret);

            GetDataKeyColumnReferenceDelegate d = getDataKeyColumnReferenceMethod
                .CreateDelegate(typeof(GetDataKeyColumnReferenceDelegate)) as GetDataKeyColumnReferenceDelegate;

            return d;
        }

        private static DataKeyGetSortIndex generateDataKeyGetSortIndex()
        {
            DynamicMethod dataKeyGetSortIndex = new DynamicMethod("FeatureMerger_DataKeyGetSortIndex_DynamicMethod",
                                                                MethodAttributes.Public | MethodAttributes.Static,
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
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo getSortIndexInfo = AdoNetInternalTypes.DataKeyType
                .GetMethod("GetSortIndex", flags, null, new Type[] { typeof(DataViewRowState)}, null);
            il.Emit(OpCodes.Ldloca_S, localKey);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Call, getSortIndexInfo);
            il.Emit(OpCodes.Isinst, typeof(Object));
            il.Emit(OpCodes.Ret);

            DataKeyGetSortIndex d = dataKeyGetSortIndex
                .CreateDelegate(typeof(DataKeyGetSortIndex)) as DataKeyGetSortIndex;

            return d;
        }

        private static GetDataKeyHasValue generateGetDataKeyHasValue()
        {
            DynamicMethod getDataKeyHasValue = new DynamicMethod("FeatureMerger_DataKey_get_HasValue_DynamicMethod",
                                                                MethodAttributes.Public | MethodAttributes.Static,
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
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo hasValueInfo = AdoNetInternalTypes.DataKeyType
                .GetProperty("HasValue", flags, null, typeof(Boolean), Type.EmptyTypes, null);
            il.Emit(OpCodes.Ldloca_S, localKey);
            il.Emit(OpCodes.Call, hasValueInfo.GetGetMethod(true));
            il.Emit(OpCodes.Ret);

            GetDataKeyHasValue d = getDataKeyHasValue
                .CreateDelegate(typeof(GetDataKeyHasValue)) as GetDataKeyHasValue;

            return d;
        }
        #endregion
    }
}