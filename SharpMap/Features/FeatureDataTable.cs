// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Indexing;
using SharpMap.Indexing.RTree;

namespace SharpMap.Features
{
    /// <summary>
    /// Represents one feature table of in-memory spatial data. 
    /// </summary>
#if !DEBUG_STEPINTO
	[System.Diagnostics.DebuggerStepThrough()]
#endif

    [Serializable]
    public class FeatureDataTable : DataTable, IEnumerable<FeatureDataRow>
    {
        #region Nested Types

        private delegate FeatureDataView GetDefaultViewDelegate(FeatureDataTable table);

        #endregion

        #region Type Fields

        private static readonly GetDefaultViewDelegate _getDefaultView;

        #endregion

        #region Static Constructors

        static FeatureDataTable()
        {
            _getDefaultView = generateGetDefaultViewDelegate();
        }

        #endregion

        #region Object Fields

        private BoundingBox _envelope;
        private SelfOptimizingDynamicSpatialIndex<FeatureDataRow> _rTreeIndex;

        #endregion

        #region Object Constructors

        /// <summary>
        /// Initializes a new instance of the FeatureDataTable class with no arguments.
        /// </summary>
        public FeatureDataTable()
        {
            Constraints.CollectionChanged += OnConstraintsChanged;
        }

        /// <summary>
        /// Initializes a new instance of the FeatureDataTable class with the given
        /// table name.
        /// </summary>
        public FeatureDataTable(string tableName)
            : base(tableName)
        {
            Constraints.CollectionChanged += OnConstraintsChanged;
        }

        /// <summary>
        /// Intitalizes a new instance of the FeatureDataTable class and
        /// copies the name and structure of the given <paramref name="table"/>.
        /// </summary>
        /// <param name="table"></param>
        public FeatureDataTable(DataTable table)
            : base(table.TableName)
        {
            Constraints.CollectionChanged += OnConstraintsChanged;

            //if (table.DataSet == null)
            //    throw new ArgumentException("Parameter 'table' must belong to a DataSet");

            if (table.DataSet == null || (table.CaseSensitive != table.DataSet.CaseSensitive))
            {
                CaseSensitive = table.CaseSensitive;
            }

            if (table.DataSet == null || (table.Locale.ToString() != table.DataSet.Locale.ToString()))
            {
                Locale = table.Locale;
            }

            if (table.DataSet == null || (table.Namespace != table.DataSet.Namespace))
            {
                Namespace = table.Namespace;
            }

            Prefix = table.Prefix;
            MinimumCapacity = table.MinimumCapacity;
            DisplayExpression = table.DisplayExpression;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs after a FeatureDataRow has been changed successfully. 
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowChanged;

        /// <summary>
        /// Occurs when a FeatureDataRow is changing. 
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowChanging;

        /// <summary>
        /// Occurs after a row in the table has been deleted.
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowDeleted;

        /// <summary>
        /// Occurs before a row in the table is about to be deleted.
        /// </summary>
        public event FeatureDataRowChangeEventHandler FeatureDataRowDeleting;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the containing FeatureDataSet.
        /// </summary>
        public new FeatureDataSet DataSet
        {
            get { return (FeatureDataSet)base.DataSet; }
        }

        /// <summary>
        /// Gets the default FeatureDataView for the table.
        /// </summary>
        public new FeatureDataView DefaultView
        {
            get
            {
                FeatureDataView defaultView = DefaultViewInternal;

                if (defaultView == null)
                {
                    if (DataSet != null)
                    {
                        defaultView = DataSet.DefaultViewManager.CreateDataView(this);
                    }
                    else
                    {
                        defaultView = new FeatureDataView(this, true);
                        // This call to SetIndex2 is actually performed in the DataView..ctor(DataTable)
                        // constructor, but for some reason is left out of the DataView..ctor(DataTable, bool)
                        // constructor. Since we call DataView..ctor(DataTable) in 
                        // FeatureDataView..ctor(FeatureDataTable, bool), we don't need this here.
                        //defaultView.SetIndex2("", DataViewRowState.CurrentRows, null, true);
                    }

                    FeatureDataView baseDefaultView = _getDefaultView(this);
                    defaultView = Interlocked.CompareExchange(ref baseDefaultView, defaultView, null);

                    if (defaultView == null)
                    {
                        defaultView = baseDefaultView;
                    }
                }

                return defaultView;
            }
        }

        /// <summary>
        /// Gets the full extents of all features in the feature table.
        /// </summary>
        public BoundingBox Envelope
        {
            get { return _envelope; }
        }

        /// <summary>
        /// Gets the number of feature rows in the feature table.
        /// </summary>
        [Browsable(false)]
        public int FeatureCount
        {
            get { return base.Rows.Count; }
        }

        /// <summary>
        /// Gets the feature data row at the specified index
        /// </summary>
        /// <param name="index">row index</param>
        /// <returns>FeatureDataRow</returns>
        public FeatureDataRow this[int index]
        {
            get { return base.Rows[index] as FeatureDataRow; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the table is spatially indexed.
        /// </summary>
        public bool IsSpatiallyIndexed
        {
            get { return _rTreeIndex != null; }
            set
            {
                if (value && _rTreeIndex == null)
                {
                    initializeSpatialIndex();
                }
                else if (!value && _rTreeIndex != null)
                {
                    _rTreeIndex.Dispose();
                    _rTreeIndex = null;
                }
            }
        }

        /// <summary>
        /// Gets the collection of rows that belong to this table.
        /// </summary>
        /// <exception cref="NotSupportedException">
        /// Thrown if this property is set.
        /// </exception>
        public new DataRowCollection Rows
        {
            get { return base.Rows; }
            set { throw new NotSupportedException(); }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a row to the FeatureDataTable
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(FeatureDataRow row)
        {
            base.Rows.Add(row);
        }

        /// <summary>
        /// Clones the structure of the FeatureDataTable, including all FeatureDataTable schemas and constraints. 
        /// </summary>
        /// <returns></returns>
        public new FeatureDataTable Clone()
        {
            FeatureDataTable cln = ((FeatureDataTable)(base.Clone()));
            return cln;
        }

        public FeatureDataRow Find(object key)
        {
            return Rows.Find(key) as FeatureDataRow;
        }

        public new FeatureDataTable GetChanges()
        {
            FeatureDataTable changes = Clone();
            FeatureDataRow row;

            for (int i = 0; i < Rows.Count; i++)
            {
                row = Rows[i] as FeatureDataRow;
                if (row.RowState != DataRowState.Unchanged)
                {
                    changes.ImportRow(row);
                }
            }

            if (changes.Rows.Count == 0)
            {
                return null;
            }

            return changes;
        }

        /// <summary>
        /// Returns an enumerator for enumering the rows of the FeatureDataTable
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<FeatureDataRow> GetEnumerator()
        {
            foreach (FeatureDataRow row in Rows)
            {
                yield return row;
            }
        }

        public void ImportRow(FeatureDataRow featureRow)
        {
            FeatureDataRow newRow = NewRow();
            copyRow(featureRow, newRow);
            AddRow(newRow);
        }

        public override void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
        {
            if (!(reader is IFeatureDataReader))
            {
                throw new NotSupportedException("Only IFeatureDataReader instances are supported to load from.");
            }

            Load(reader as IFeatureDataReader, loadOption, errorHandler);
        }

        public void Load(IFeatureDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            LoadFeaturesAdapter adapter = new LoadFeaturesAdapter();
            adapter.FillLoadOption = loadOption;
            adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;

            if (errorHandler != null)
            {
                adapter.FillError += errorHandler;
            }

            adapter.Fill(this, reader);

            if (!reader.IsClosed && !reader.NextResult())
            {
                reader.Close();
            }
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table.
        /// </summary>
        /// <returns></returns>
        public new FeatureDataRow NewRow()
        {
            return base.NewRow() as FeatureDataRow;
        }

        /// <summary>
        /// Removes the row from the table
        /// </summary>
        /// <param name="row">Row to remove</param>
        public void RemoveRow(FeatureDataRow row)
        {
            base.Rows.Remove(row);
        }

        public IEnumerable<FeatureDataRow> Select(BoundingBox bounds)
        {
            foreach (RTreeIndexEntry<FeatureDataRow> entry in _rTreeIndex.Search(bounds))
            {
                yield return entry.Value;
            }
        }

        public IEnumerable<FeatureDataRow> Select(Geometry geometry)
        {
            foreach (RTreeIndexEntry<FeatureDataRow> entry in _rTreeIndex.Search(geometry))
            {
                yield return entry.Value;
            }
        }

        #endregion

        protected virtual void OnConstraintsChanged(object sender, CollectionChangeEventArgs args)
        {
            if (args.Action == CollectionChangeAction.Add && args.Element is UniqueConstraint)
            {
                UniqueConstraint constraint = args.Element as UniqueConstraint;

                // If more than one column is added to the primary key, throw
                // an exception - we don't support it for now.
                if (constraint.IsPrimaryKey && constraint.Columns.Length > 1)
                {
                    throw new NotSupportedException("Compound primary keys not supported.");
                }
            }
        }

        internal void MergeFeature(IFeatureDataReader record)
        {
            // TODO: Reevaluate FeatureDataTable.MergeFeature in terms of DataTable.Merge
            // This function looks as if it duplicates DataTable.Merge
            // somewhat. However, since it's not a complete overlap, and
            // it isn't clear how to accomplish the operation with DataTable.Merge,
            // this method will be used for now...
            if (record == null) throw new ArgumentNullException("record");

            FeatureDataRow feature;

            // If the feature as an identifier, then we can merge it
            if (PrimaryKey.Length == 1)
            {
                string keyColumnName = PrimaryKey[0].ColumnName;
                object key = record[keyColumnName];
                feature = Find(key);

                if (feature != null)
                {
                    foreach (DataColumn column in Columns)
                    {
                        if (column != PrimaryKey[0])
                        {
                            if (feature[column] != record[column.ColumnName])
                            {
                                feature[column] = record[column.ColumnName];
                            }
                        }
                    }

                    // A matching feature was found and updated, so we can leave
                    return;
                }
            }

            // No match to 'record' was found in this table, add a new one
            feature = NewRow();
            object[] values = new object[Columns.Count];
            record.GetValues(values);
            feature.ItemArray = values;
            feature.Geometry = record.GetGeometry();
            AddRow(feature);
        }

        #region Protected Overrides

        protected override void OnTableCleared(DataTableClearEventArgs e)
        {
            base.OnTableCleared(e);

            _rTreeIndex.Clear();
        }

        /// <summary>
        /// Creates and returns a new instance of a FeatureDataTable.
        /// </summary>
        /// <returns>An empty FeatureDataTable.</returns>
        protected override DataTable CreateInstance()
        {
            return new FeatureDataTable();
        }

        /// <summary>
        /// Creates a new FeatureDataRow with the same schema as the table, 
        /// based on a datarow builder.
        /// </summary>
        /// <param name="builder">
        /// The DataRowBuilder instance to use to construct
        /// a new row.
        /// </param>
        /// <returns>A new DataRow using the schema in the DataRowBuilder.</returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new FeatureDataRow(builder);
        }

        /// <summary>
        /// Returns the FeatureDataRow type.
        /// </summary>
        /// <returns>The <see cref="Type"/> <see cref="FeatureDataRow"/>.</returns>
        protected override Type GetRowType()
        {
            return typeof(FeatureDataRow);
        }

        #endregion

        #region Event Generators

        /// <summary>
        /// Raises the FeatureDataRowChanged event. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanged(DataRowChangeEventArgs e)
        {
            Debug.Assert(e.Row is FeatureDataRow);

            if (e.Action == DataRowAction.Add
                || e.Action == DataRowAction.ChangeCurrentAndOriginal
                || e.Action == DataRowAction.Change)
            {
                FeatureDataRow r = e.Row as FeatureDataRow;

#warning: Does this never work, since r.Geometry isn't populated when rows are loaded?
                if (r.Geometry != null)
                {
                    _envelope = _envelope.Join(r.Geometry.GetBoundingBox());

                    if (IsSpatiallyIndexed)
                    {
                        _rTreeIndex.Insert(new RTreeIndexEntry<FeatureDataRow>(r, r.Geometry.GetBoundingBox()));
                    }
                }
            }
            else if (e.Action == DataRowAction.Delete)
            {
                throw new NotSupportedException("Can't subtract bounding box");
            }

            if ((FeatureDataRowChanged != null))
            {
                FeatureDataRowChanged(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }

            base.OnRowChanged(e);
        }

        /// <summary>
        /// Raises the FeatureDataRowChanging event. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowChanging(DataRowChangeEventArgs e)
        {
            base.OnRowChanging(e);

            if ((FeatureDataRowChanging != null))
            {
                FeatureDataRowChanging(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }

        /// <summary>
        /// Raises the FeatureDataRowDeleted event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleted(DataRowChangeEventArgs e)
        {
            base.OnRowDeleted(e);

            if ((FeatureDataRowDeleted != null))
            {
                FeatureDataRowDeleted(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }

        /// <summary>
        /// Raises the FeatureDataRowDeleting event. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowDeleting(DataRowChangeEventArgs e)
        {
            base.OnRowDeleting(e);
            if ((FeatureDataRowDeleting != null))
            {
                FeatureDataRowDeleting(this, new FeatureDataRowChangeEventArgs(((FeatureDataRow)(e.Row)), e.Action));
            }
        }

        #endregion

        #region Internal helper methods

        internal FeatureDataView DefaultViewInternal
        {
            get { return _getDefaultView(this); }
        }

        internal void RowGeometryChanged(FeatureDataRow row)
        {
            row.BeginEdit();
            row.EndEdit();
        }

        internal void CopyTableSchema(FeatureDataTable target)
        {
            target.Clear();
            target.Columns.Clear();
            DataColumn[] targetPrimaryKey = new DataColumn[PrimaryKey.Length];
            int keyIndex = 0;

            foreach (DataColumn column in Columns)
            {
                DataColumn newColumn = new DataColumn(column.ColumnName, column.DataType);
                newColumn.AllowDBNull = column.AllowDBNull;
                newColumn.AutoIncrement = column.AutoIncrement;
                newColumn.AutoIncrementSeed = column.AutoIncrementSeed;
                newColumn.AutoIncrementStep = column.AutoIncrementStep;
                newColumn.Caption = column.Caption;
                newColumn.ColumnMapping = column.ColumnMapping;
                newColumn.DateTimeMode = column.DateTimeMode;
                newColumn.DefaultValue = column.DefaultValue;
                newColumn.Expression = column.Expression;
                newColumn.MaxLength = column.MaxLength;
                newColumn.Namespace = column.Namespace;
                newColumn.Prefix = column.Prefix;
                newColumn.ReadOnly = column.ReadOnly;
                newColumn.Unique = column.Unique;

                foreach (DictionaryEntry entry in column.ExtendedProperties)
                {
                    newColumn.ExtendedProperties.Add(entry.Key, entry.Value);
                }

                target.Columns.Add(newColumn);

                // if this column is a key, add it to the target key columns array
                if (Array.Exists(PrimaryKey, delegate(DataColumn key) { return column == key; }))
                {
                    targetPrimaryKey[keyIndex] = newColumn;
                    keyIndex++;
                }
            }

            Debug.Assert(keyIndex == PrimaryKey.Length);
            target.PrimaryKey = targetPrimaryKey;
        }

        #endregion

        #region Private static helper methods

        private static GetDefaultViewDelegate generateGetDefaultViewDelegate()
        {
            DynamicMethod get_DefaultViewMethod = new DynamicMethod("get_DefaultView_DynamicMethod",
                                                                    typeof(FeatureDataView),
                                                                    new Type[] { typeof(FeatureDataTable) }, typeof(DataTable));

            ILGenerator il = get_DefaultViewMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeof(DataTable).GetField("defaultView", BindingFlags.Instance | BindingFlags.NonPublic));
            il.Emit(OpCodes.Ret);

            return get_DefaultViewMethod.CreateDelegate(typeof(GetDefaultViewDelegate))
                   as GetDefaultViewDelegate;
        }

        #endregion

        #region Private helper methods

        private static void copyRow(FeatureDataRow source, FeatureDataRow target)
        {
            if (source.Table.Columns.Count != target.Table.Columns.Count)
            {
                throw new InvalidOperationException("Can't import a feature row with a different number of columns.");
            }

            for (int columnIndex = 0; columnIndex < source.Table.Columns.Count; columnIndex++)
            {
                target[columnIndex] = source[columnIndex];
            }

            target.Geometry = source.Geometry == null
                ? null
                : source.Geometry.Clone();
        }

        private void initializeSpatialIndex()
        {
            IIndexRestructureStrategy restructureStrategy = new NullRestructuringStrategy();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.Default, 4.0);
            IEntryInsertStrategy<RTreeIndexEntry<FeatureDataRow>> insertStrategy = new GuttmanQuadraticInsert<FeatureDataRow>();
            INodeSplitStrategy nodeSplitStrategy = new GuttmanQuadraticSplit<FeatureDataRow>();
            _rTreeIndex = new SelfOptimizingDynamicSpatialIndex<FeatureDataRow>(restructureStrategy,
                                                                                restructureHeuristic, insertStrategy,
                                                                                nodeSplitStrategy,
                                                                                new DynamicRTreeBalanceHeuristic());
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}