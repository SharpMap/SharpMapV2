// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Text;
using System.Data;
using System.Reflection.Emit;
using System.Reflection;
using SharpMap.Data;
using System.Collections;

namespace SharpMap.Data
{
    internal sealed class FeatureMerger
    {
        #region Nested types
        private delegate Object CreateMergerDelegate(DataTable target, bool preserveChanges, MissingSchemaAction action);
        private delegate DataTable MergeSchemaDelegate(object merger, DataTable source);
        private delegate Object GetKeyIndexDelegate(DataTable table);
        #endregion

        #region Static fields
        private static readonly CreateMergerDelegate _createMerger;
        private static readonly MergeSchemaDelegate _mergeSchema;
        private static readonly GetKeyIndexDelegate _getKeyIndex;
        private static readonly RuntimeTypeHandle _adoMergerTypeHandle;
        #endregion

        #region Static constructor
        static FeatureMerger()
        {
            Type adoMergerType = Type.GetType("System.Data.Merger, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            _adoMergerTypeHandle = adoMergerType.TypeHandle;

            _createMerger = generateCreateMergerDelegate();
            _mergeSchema = generateMergeSchemaDelegate();
            _getKeyIndex = generateGetKeyIndexDelegate();
        }
        #endregion

        #region Instance fields
        private readonly FeatureDataTable _target;
        private readonly Object _innerMerger;
        private readonly SchemaMergeAction _mergeAction;
        private readonly bool _preserveChanges;
        #endregion

        #region Object constructor
        internal FeatureMerger(FeatureDataTable target, bool preserveChanges, SchemaMergeAction mergeAction)
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

            _target = target;
            _preserveChanges = preserveChanges;
            _mergeAction = mergeAction;
            _innerMerger = createInnerMerger(target, preserveChanges, mergeAction);
        }
        #endregion

        internal bool PreserveChanges
        {
            get { return _preserveChanges; }
        }

        internal SchemaMergeAction SchemaMergeAction
        {
            get { return _mergeAction; }
        }

        internal void MergeSchema(FeatureDataTable source)
        {
            _mergeSchema(_innerMerger, source);
        }

        internal void MergeFeatures(FeatureDataTable source)
        {
            throw new NotImplementedException();
            // TODO: call ado Merger.MergeTable(DataTable src)
        }

        internal void MergeFeature(IFeatureDataRecord record)
        {
            if (record == null) throw new ArgumentNullException("record");

            bool checkForTarget = false;

            if ((SchemaMergeAction & SchemaMergeAction.Add) != SchemaMergeAction.None)
            {
                // HACK: Parsing and setting the schema should be less clunky here.
                //      We probably need to just do the schema merge ourselves without having 
                //      to rely on System.Data.Merger
                FeatureDataTable schemaModel = createModelFromFeature(record);
                MergeSchema(schemaModel);
                checkForTarget = _target.Rows.Count > 0 && (_target.PrimaryKey.Length > 0);
            }

            FeatureDataRow targetFeature = null;

            if (checkForTarget && record.HasOid)
            {
                targetFeature = _target.Find(record.GetOid());
            }

            mergeFeature(_target, record, targetFeature, PreserveChanges);
        }

        internal void MergeFeatures(IEnumerable<IFeatureDataRecord> sourceFeatures)
        {
            if (sourceFeatures == null) throw new ArgumentNullException("sourceFeatures");

            _target.SuspendIndexEvents();

            bool checkedSchema = false;
            bool checkForTarget = false;

            try
            {
                foreach (IFeatureDataRecord srcFeature in sourceFeatures)
                {
                    if (!checkedSchema)
                    {
                        checkedSchema = true;

                        if ((SchemaMergeAction & SchemaMergeAction.Add) != SchemaMergeAction.None)
                        {
                            // HACK: Parsing and setting the schema should be less clunky here.
                            //      We probably need to just do the schema merge ourselves without having 
                            //      to rely on System.Data.Merger
                            FeatureDataTable schemaModel = createModelFromFeature(srcFeature);
                            MergeSchema(schemaModel);
                            checkForTarget = _target.Rows.Count > 0 && (_target.PrimaryKey.Length > 0);
                        }
                    }

                    FeatureDataRow targetFeature = null;

                    if (checkForTarget && srcFeature.HasOid)
                    {
                        targetFeature = _target.Find(srcFeature.GetOid());
                    }

                    mergeFeature(_target, srcFeature, targetFeature, PreserveChanges);
                }
            }
            finally
            {
                _target.RestoreIndexEvents(true);
            }
        }

        private static FeatureDataTable createModelFromFeature(IFeatureDataRecord srcFeature)
        {
            FeatureDataTable schemaModel = new FeatureDataTable();
            for (int fieldIndex = 0; fieldIndex < srcFeature.FieldCount; fieldIndex++)
            {
                schemaModel.Columns.Add(srcFeature.GetName(fieldIndex),
                                        srcFeature.GetFieldType(fieldIndex));
            }

            return schemaModel;
        }

        private static void mergeFeature(FeatureDataTable target, IFeatureDataRecord srcFeature, FeatureDataRow targetFeature, bool preserveChanges)
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
            for (int i = 0; i < srcFeature.FieldCount; i++)
            {
                string colName = srcFeature.GetName(i);
                targetFeature[colName] = srcFeature.GetValue(i);
            }

            targetFeature.Geometry = srcFeature.Geometry;

            targetFeature.IsFullyLoaded = targetFeature.IsFullyLoaded || srcFeature.IsFullyLoaded;
        }

        #region Private helper methods
        private static object createInnerMerger(DataTable target, bool preserveChanges, SchemaMergeAction schemaMergeAction)
        {
            MissingSchemaAction missingSchemaAction = MissingSchemaAction.Error;

            if ((int)(schemaMergeAction & SchemaMergeAction.Add) != 0)
            {
                missingSchemaAction = MissingSchemaAction.Add;
            }

            if ((int)(schemaMergeAction & SchemaMergeAction.Key) != 0)
            {
                missingSchemaAction = MissingSchemaAction.AddWithKey;
            }

            return _createMerger(target, preserveChanges, missingSchemaAction);
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

        private static object getKeyIndex(FeatureDataTable table)
        {
            return _getKeyIndex(table);
        }

        private static CreateMergerDelegate generateCreateMergerDelegate()
        {
            Type[] ctorParams = new Type[] { typeof(DataTable), typeof(bool), typeof(MissingSchemaAction) };

            DynamicMethod createMergerMethod = new DynamicMethod("Merger_Create",
                                                                 MethodAttributes.Public | MethodAttributes.Static,
                                                                 CallingConventions.Standard,
                                                                 typeof(Object),
                                                                 ctorParams,
                                                                 Type.GetTypeFromHandle(_adoMergerTypeHandle),
                                                                 false);

            Type adoMergerType = Type.GetTypeFromHandle(_adoMergerTypeHandle);
            ILGenerator il = createMergerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            ConstructorInfo ctor = adoMergerType
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, ctorParams, null);
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
                                                                new Type[] { typeof(object), typeof(DataTable) },
                                                                Type.GetTypeFromHandle(_adoMergerTypeHandle),
                                                                false);

            Type merger = Type.GetTypeFromHandle(_adoMergerTypeHandle);
            ILGenerator il = mergeSchemaMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, merger);
            il.Emit(OpCodes.Ldarg_1);
            MethodInfo mergeSchemaInfo = merger.GetMethod("MergeSchema",
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
        #endregion

    }
}