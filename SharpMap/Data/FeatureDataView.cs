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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using SharpMap.Geometries;
using SharpMap.Query;

namespace SharpMap.Data
{
    /// <summary>
    /// Provides a data-bindable view of feature data stored in a 
    /// <see cref="FeatureDataTable"/>
    /// and can provide sorted, filtered, searchable and editable access to that data.
    /// </summary>
    public class FeatureDataView : DataView, IEnumerable<FeatureDataRow>
    {
        #region Nested types

        private delegate void SetDataViewManagerDelegate(FeatureDataView view, DataViewManager dataViewManager);

        private delegate void SetLockedDelegate(DataView view, bool locked);

        private delegate void SetIndex2Delegate(
            FeatureDataView view, string newSort, DataViewRowState dataViewRowState, object expression, bool fireEvent);

        #endregion

        #region Type fields

        private static readonly SetDataViewManagerDelegate _setDataViewManager;
        private static readonly SetLockedDelegate _setLocked;
        private static readonly SetIndex2Delegate _setIndex2;

        #endregion

        #region Static constructor

        static FeatureDataView()
        {
            // Create method to set DataViewManager
            _setDataViewManager = GenerateSetDataViewManagerDelegate();

            // Create method to set locked status
            _setLocked = GenerateSetLockedDelegate();

            // Create a delegate to the SetIndex2 method
            _setIndex2 = GenerateSetIndex2Delegate();
        }

        #endregion

        #region Instance fields
        //private Geometry _geometryFilter = Point.Empty;
        //private readonly SpatialQueryType _queryType;
        //private readonly ArrayList _oidFilter = new ArrayList();
        private FeatureSpatialQuery _viewDefinition;
        private bool _reindexingEnabled = true;
        private bool _shouldReindex = false;
        #endregion

        #region Object constructors

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/>.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        public FeatureDataView(FeatureDataTable table)
            : this(table, null, null, DataViewRowState.CurrentRows) { }

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/> having the specified geometry filter, 
        /// sort order and row state filter.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        /// <param name="intersectionFilter">
        /// Geometry used in intersection test to filter feature table rows.
        /// </param>
        /// <param name="sort">Sort expression to order view by.</param>
        /// <param name="rowState">Filter on the state of the rows to view.</param>
        public FeatureDataView(FeatureDataTable table, Geometry intersectionFilter,
                               string sort, DataViewRowState rowState)
            : this(table, intersectionFilter, SpatialQueryType.Intersects, sort, rowState) { }

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/> having the specified geometry filter, 
        /// sort order and row state filter.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        /// <param name="intersectionFilter">
        /// Geometry used in intersection test to filter feature table rows.
        /// </param>
        /// <param name="sort">Sort expression to order view by.</param>
        /// <param name="rowState">Filter on the state of the rows to view.</param>
        /// <param name="emptyGeometryFilterIsExclusive">
        /// Value indicating that an empty <see cref="GeometryFilter"/> excludes rows 
        /// (<see langword="true"/>) or includes them (<see langword="false"/>).
        /// </param>
        public FeatureDataView(FeatureDataTable table, Geometry intersectionFilter,
                               string sort, DataViewRowState rowState, bool emptyGeometryFilterIsExclusive)
            : this(table, intersectionFilter, SpatialQueryType.Intersects, sort, rowState, emptyGeometryFilterIsExclusive) { }

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/> having the specified geometry filter, 
        /// sort order and row state filter.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        /// <param name="query">
        /// Geometry used in building view to filter feature table rows.
        /// </param>
        /// <param name="queryType">
        /// Type of spatial relation which <paramref name="query"/> has to features.
        /// </param>
        /// <param name="sort">Sort expression to order view by.</param>
        /// <param name="rowState">Filter on the state of the rows to view.</param>
        public FeatureDataView(FeatureDataTable table, Geometry query, SpatialQueryType queryType,
                               string sort, DataViewRowState rowState)
            : this(table, query, queryType, sort, rowState, false) { }


        public FeatureDataView(FeatureDataTable table, Geometry query, SpatialQueryType queryType,
                               string sort, DataViewRowState rowState, bool emptyGeometryFilterIsExclusive)
            : this(table, new FeatureSpatialQuery(query, queryType, null), sort, rowState, emptyGeometryFilterIsExclusive)
        { }

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/> having the specified geometry filter, 
        /// sort order and row state filter.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        /// <param name="query">
        /// Geometry used in building view to filter feature table rows.
        /// </param>
        /// <param name="queryType">
        /// Type of spatial relation which <paramref name="query"/> has to features.
        /// </param>
        /// <param name="sort">Sort expression to order view by.</param>
        /// <param name="rowState">Filter on the state of the rows to view.</param>
        /// <param name="emptyGeometryFilterIsExclusive">
        /// Value indicating that an empty <see cref="GeometryFilter"/> excludes rows 
        /// (<see langword="true"/>) or includes them (<see langword="false"/>).
        /// </param>
        public FeatureDataView(FeatureDataTable table, FeatureSpatialQuery definition,
                               string sort, DataViewRowState rowState, bool emptyGeometryFilterIsExclusive)
            : base(
                table, "",
                String.IsNullOrEmpty(sort) ? table.PrimaryKey.Length == 1 ? table.PrimaryKey[0].ColumnName : "" : sort,
                rowState)
        {
            if (definition == null) throw new ArgumentNullException("definition");

            // TODO: Support all query types in FeatureDataView
            if (definition.QueryType != SpatialQueryType.Intersects &&
                definition.QueryType != SpatialQueryType.Disjoint)
            {
                throw new NotSupportedException(
                    "Only query types of SpatialQueryType.Intersects and " +
                    "SpatialQueryType.Disjoint are currently supported.");
            }

            _viewDefinition = definition;

            // This call rebuilds the index which was just built with 
            // the call into the base constructor, which may be a performance
            // hit. A more robust solution would be to just recreate the 
            // behavior of the base constructor here, so we can create the 
            // underlying index once.
            setFilterPredicate();
        }

        internal FeatureDataView(FeatureDataTable table, bool locked)
            : this(table)
        {
            // The DataView is locked when it is created as a default
            // view for a table.
            _setLocked(this, locked);
        }

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="Geometry"/> instance used
        /// to filter the table data based on intersection.
        /// </summary>
        public Geometry GeometryFilter
        {
            get
            {
                return _viewDefinition.QueryRegion == null
                  ? Point.Empty
                  : _viewDefinition.QueryRegion.Clone();
            }
            set
            {
                value = value ?? Point.Empty;

                if (_viewDefinition.QueryRegion == value)
                {
                    return;
                }

                ViewDefinition = new FeatureSpatialQuery(value,
                    GeometryFilterType, _viewDefinition.Oids);
            }
        }

        /// <summary>
        /// Gets the type of spatial relation which <see cref="GeometryFilter"/>
        /// has with features to include them in the view.
        /// </summary>
        public SpatialQueryType GeometryFilterType
        {
            get { return _viewDefinition.QueryType; }
        }

        /// <summary>
        /// Gets or sets a set of feature identifiers (oids) used
        /// to include or exclude features from the view. 
        /// </summary>
        /// <remarks>
        /// A value of <see langword="null"/> clears the filter, and allows
        /// all features regardless of oid value.
        /// </remarks>
        public IEnumerable OidFilter
        {
            get
            {
                return _viewDefinition.Oids;
            }
            set
            {
                if (_viewDefinition.Oids == value)
                {
                    return;
                }

                ViewDefinition = new FeatureSpatialQuery(_viewDefinition.QueryRegion,
                                                          _viewDefinition.QueryType, value);
            }
        }

        /// <summary>
        /// Gets the bounds which this view covers as a <see cref="BoundingBox"/>.
        /// </summary>
        public BoundingBox Extents
        {
            get
            {
                if (_viewDefinition.QueryRegion == null)
                {
                    return BoundingBox.Empty;
                }

                return _viewDefinition.QueryRegion.GetBoundingBox();
            }
            //set
            //{
            //    if (value == BoundingBox.Empty)
            //    {
            //        GeometryFilter = null;
            //    }
            //    GeometryFilter = value.ToGeometry();
            //}
        }

        public FeatureSpatialQuery ViewDefinition
        {
            get { return _viewDefinition.Clone(); }
            set
            {
                if (_viewDefinition == value)
                {
                    return;
                }

                _viewDefinition = value;

                Geometry missingGeometry = null;
                ArrayList missingOids = new ArrayList();

                if (!_viewDefinition.QueryRegion.IsEmpty() 
                    && !Table.Envelope.Contains(_viewDefinition.QueryRegion))
                {
                    missingGeometry = Table.Envelope.Difference(_viewDefinition.QueryRegion);
                }

                if (value.Oids != null)
                {
                    foreach (object oid in value.Oids)
                    {
                        FeatureDataRow feature = Table.Find(oid);

                        if (feature == null || !feature.IsFullyLoaded)
                        {
                            missingOids.Add(oid);
                        }
                    }
                }

                Table.NotifyFeaturesNotFound(missingGeometry, missingOids);
                resetInternal();
            }
        }

        #region DataView overrides and shadows

        public override DataRowView AddNew()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the DataViewManager which is managing this view's settings.
        /// </summary>
        public new FeatureDataViewManager DataViewManager
        {
            get { return base.DataViewManager as FeatureDataViewManager; }
        }

        public bool ReindexingEnabled
        {
            get { return _reindexingEnabled; }
            set
            {
                if (_reindexingEnabled == value)
                {
                    return;
                }

                _reindexingEnabled = value;

                if (_shouldReindex)
                {
                    _shouldReindex = false;
                    Reset();
                }
            }
        }

        public override string RowFilter
        {
            get { return ""; }
            set
            {
                throw new NotSupportedException(
                    "RowFilter expressions not supported at this time.");
            }
        }

        /// <summary>
        /// Gets or sets the table for which this view filters and / or sorts rows.
        /// </summary>
        public new FeatureDataTable Table
        {
            get { return base.Table as FeatureDataTable; }
            set { base.Table = value; }
        }

        // TODO: Please don't optimize the following apparently useless overrides out, 
        // I need to figure out what to do with them [ck]

        protected override void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            base.ColumnCollectionChanged(sender, e);
        }

        protected override void IndexListChanged(object sender, ListChangedEventArgs e)
        {
            base.IndexListChanged(sender, e);
        }

        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }

        protected override void UpdateIndex(bool force)
        {
            if (!ReindexingEnabled)
            {
                return;
            }
            else
            {
                base.UpdateIndex(force);
            }
        }

        #endregion

        #region IEnumerable<FeatureDataRow> Members

        IEnumerator<FeatureDataRow> IEnumerable<FeatureDataRow>.GetEnumerator()
        {
            IEnumerator e = GetEnumerator();

            while (e.MoveNext())
            {
                DataRowView rowView = e.Current as DataRowView;
                Debug.Assert(rowView != null);
                FeatureDataRow feature = rowView.Row as FeatureDataRow;
                yield return feature;
            }
        }

        #endregion

        #region DataView internals access methods

        internal void SetDataViewManager(FeatureDataViewManager featureDataViewManager)
        {
            // Call the delegate we wired up to bypass the normally inaccessible 
            // base class method
            _setDataViewManager(this, featureDataViewManager);
        }

        internal void SetIndex2(string newSort, DataViewRowState dataViewRowState,
                                object dataExpression, bool fireEvent)
        {
            // Call the delegate we wired up to bypass the normally inaccessible 
            // base class method
            _setIndex2(this, newSort, dataViewRowState, dataExpression, fireEvent);
        }

        #endregion

        #region Private instance helper methods

        private void setFilterPredicate()
        {
            // TODO: rethink how the view is filtered... perhaps we could bypass SetIndex2 and create
            // the System.Data.Index directly with rows returned from the SharpMap.Indexing.RTree
            object iFilter = createRowPredicateFilter(isRowInView);
            SetIndex2(Sort, RowStateFilter, iFilter, true);
        }

        private bool isRowInView(DataRow row)
        {
            FeatureDataRow feature = row as FeatureDataRow;

            if (feature == null)
            {
                return false;
            }

            return inGeometryFilter(feature)
                   && inOidFilter(feature)
                   && inAttributeFilter();
        }

        private bool inGeometryFilter(FeatureDataRow feature)
        {
            return (_viewDefinition.QueryRegion == Point.Empty &&
                _viewDefinition.QueryType == SpatialQueryType.Disjoint) ||
                _viewDefinition.QueryRegion.Intersects(feature.Geometry);
        }

        private bool inOidFilter(FeatureDataRow feature)
        {
            if (!feature.HasOid)
            {
                return false;
            }

            if (_viewDefinition.Oids == null)
            {
                return true;
            }

            object featureOid = feature.GetOid();

            foreach (object oid in _viewDefinition.Oids)
            {
                if (featureOid == oid)
                {
                    return true;
                }
            }

            return false;
        }

        private bool inAttributeFilter()
        {
            // TODO: perhaps this is where we can execute the DataExpression filter
            return true;
        }

        private void resetInternal()
        {
            // TODO: Perhaps resetting the entire index is a bit drastic... 
            // Reconsider how to enlist and retire rows incrementally, 
            // perhaps doing RTree diffs.
            if (ReindexingEnabled)
            {
                Reset();
            }
            else
            {
                _shouldReindex = true;
            }
        }
        #endregion

        #region Private static helper methods

        private static SetIndex2Delegate GenerateSetIndex2Delegate()
        {
            // TODO: check if this could be redone with a DynamicMethod,
            // using System.Object in place of the inaccessible System.Data.DataExpression,
            // relying on delegate covariance.
            MethodInfo setIndex2Info = typeof(DataView).GetMethod(
                "SetIndex2", BindingFlags.NonPublic | BindingFlags.Instance);
            ConstructorInfo setIndexDelegateCtor = typeof(SetIndex2Delegate)
                .GetConstructor(new Type[] { typeof(object), typeof(IntPtr) });
            IntPtr setIndex2Pointer = setIndex2Info.MethodHandle.GetFunctionPointer();
            return (SetIndex2Delegate)setIndexDelegateCtor.Invoke(new Object[] { null, setIndex2Pointer });
        }

        private static SetLockedDelegate GenerateSetLockedDelegate()
        {
            // Use LCG to create a set accessor to the DataView.locked field
            DynamicMethod setLockedMethod = new DynamicMethod("set_locked_DynamicMethod",
                                                              null, new Type[] { typeof(DataView), typeof(bool) },
                                                              typeof(DataView));

            ILGenerator il = setLockedMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            FieldInfo lockedField = typeof(DataView).GetField("locked", BindingFlags.Instance | BindingFlags.NonPublic);
            il.Emit(OpCodes.Stfld, lockedField);
            il.Emit(OpCodes.Ret);

            return setLockedMethod.CreateDelegate(typeof(SetLockedDelegate)) as SetLockedDelegate;
        }

        private static SetDataViewManagerDelegate GenerateSetDataViewManagerDelegate()
        {
            // Use LCG to create a delegate to the internal DataView.SetDataViewManager method
            DynamicMethod setDataViewManagerMethod = new DynamicMethod("set_DataViewManager_DynamicMethod",
                                                                       null,
                                                                       new Type[]
                                                                           {
                                                                               typeof (FeatureDataView),
                                                                               typeof (DataViewManager)
                                                                           }, typeof(DataView));

            ILGenerator il = setDataViewManagerMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            MethodInfo setDataViewManagerInfo = typeof(DataView).GetMethod("SetDataViewManager",
                                                                            BindingFlags.Instance |
                                                                            BindingFlags.NonPublic, null,
                                                                            new Type[] { typeof(DataViewManager) }, null);
            il.Emit(OpCodes.Call, setDataViewManagerInfo);

            return setDataViewManagerMethod.CreateDelegate(typeof(SetDataViewManagerDelegate))
                   as SetDataViewManagerDelegate;
        }

        private static object createRowPredicateFilter(Predicate<DataRow> filter)
        {
#warning Reflection on internal type breaks in CLR versions less than v2.0.50727.1378
            // This is the only way we have to take control of what predicate is used to filter the DataView.
            // Unfortunately, this type is only available in the v2.0 CLR which ships with .Net v3.5 Beta 2 (v2.0.50727.1378)
            // Currently, the only two choices to provided spatially filtered views are to implement 
            // System.ComponentModel.IBindingListView or to rely on v3.5.
            string rowPredicateFilterTypeName =
                "System.Data.DataView+RowPredicateFilter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            Type rowPredicateFilterType = Type.GetType(rowPredicateFilterTypeName);
            Debug.Assert(rowPredicateFilterType != null,
                         "Can't find the type System.Data.DataView+RowPredicateFilter. " +
                         "This is probably because you are not running the v2.0 CLR which ships with .Net v3.5 Beta 2.");
            object[] args = new object[] { filter };
            object rowPredicateFilter = Activator.CreateInstance(rowPredicateFilterType,
                                                                 BindingFlags.Instance | BindingFlags.NonPublic,
                                                                 null, args, null);

            return rowPredicateFilter;
        }

        #endregion
    }
}