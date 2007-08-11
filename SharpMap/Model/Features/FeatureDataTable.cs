using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SharpMap.Geometries;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Threading;
using SharpMap.Indexing.RTree;
using SharpMap.Indexing;
using SharpMap.Data;
using System.Data.Common;

namespace SharpMap
{
    /// <summary>
    /// Represents one feature table of in-memory spatial data. 
    /// </summary>
#if !DEBUG_STEPINTO
	[System.Diagnostics.DebuggerStepThrough()]
#endif
    [Serializable()]
    public class FeatureDataTable : DataTable, IEnumerable<FeatureDataRow>
    {
        #region Nested Types
        private delegate FeatureDataView GetDefaultViewDelegate(FeatureDataTable table);

        internal sealed class LoadFeaturesAdapter : DataAdapter
        {
            internal LoadFeaturesAdapter() { }

            public new int Fill(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
            {
                return base.Fill(dataTables, dataReader, startRecord, maxRecords);
            }
        }
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
        }

        /// <summary>
        /// Intitalizes a new instance of the FeatureDataTable class and
        /// copies the name and structure of the given <paramref name="table"/>.
        /// </summary>
        /// <param name="table"></param>
        public FeatureDataTable(DataTable table)
            : base(table.TableName)
        {
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
        /// Gets the number of feature rows in the feature table.
        /// </summary>
        [Browsable(false)]
        public int FeatureCount
        {
            get { return base.Rows.Count; }
        }

        public new FeatureDataSet DataSet
        {
            get { return (FeatureDataSet)base.DataSet; }
        }

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
                    defaultView = Interlocked.CompareExchange<FeatureDataView>(ref baseDefaultView, defaultView, null);

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
        /// Gets the feature data row at the specified index
        /// </summary>
        /// <param name="index">row index</param>
        /// <returns>FeatureDataRow</returns>
        public FeatureDataRow this[int index]
        {
            get { return base.Rows[index] as FeatureDataRow; }
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

        #region Methods
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

        public new FeatureDataTable GetChanges()
        {
            FeatureDataTable changes = Clone();
            FeatureDataRow row = null;

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

        public override void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
        {
            if (!(reader is IFeatureDataReader))
            {
                throw new NotSupportedException("Only IFeatureDataReader instances are supported to load from.");
            }

            base.Load(reader as IFeatureDataReader, loadOption, errorHandler);
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
            
            adapter.Fill(new DataTable[] { this }, reader, 0, 0);

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
        #endregion

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
            base.OnRowChanged(e);

            Debug.Assert(e.Row is FeatureDataRow);

            if (e.Action == DataRowAction.Add)
            {
                FeatureDataRow r = e.Row as FeatureDataRow;
                if (r.Geometry != null)
                {
                    _envelope = _envelope.Join(r.Geometry.GetBoundingBox());
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
        #endregion

        #region Private static helper methods
        private static GetDefaultViewDelegate generateGetDefaultViewDelegate()
        {
            DynamicMethod get_DefaultViewMethod = new DynamicMethod("get_DefaultView_DynamicMethod",
                typeof(FeatureDataView), new Type[] { typeof(FeatureDataTable) }, typeof(DataTable));

            ILGenerator il = get_DefaultViewMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, typeof(DataTable).GetField("defaultView", BindingFlags.Instance | BindingFlags.NonPublic));
            il.Emit(OpCodes.Ret);

            return get_DefaultViewMethod.CreateDelegate(typeof(GetDefaultViewDelegate))
                as GetDefaultViewDelegate;
        }
        #endregion

        #region Private helper methods

        private void initializeSpatialIndex()
        {
            IIndexRestructureStrategy restructureStrategy = new NullRestructuringStrategy();
            RestructuringHuristic restructureHeuristic = new RestructuringHuristic(RestructureOpportunity.Default, 4.0);
            IEntryInsertStrategy<RTreeIndexEntry<FeatureDataRow>> insertStrategy = new GuttmanQuadraticInsert<FeatureDataRow>();
            INodeSplitStrategy nodeSplitStrategy = new GuttmanQuadraticSplit<FeatureDataRow>();
            _rTreeIndex = new SelfOptimizingDynamicSpatialIndex<FeatureDataRow>(restructureStrategy,
                restructureHeuristic, insertStrategy, nodeSplitStrategy, new DynamicRTreeBalanceHeuristic());
        }
        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
