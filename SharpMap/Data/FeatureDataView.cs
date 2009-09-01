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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using GeoAPI.Geometries;
using SharpMap.Expressions;

#if DOTNET35
using Enumerable = System.Linq.Enumerable;
using Caster = System.Linq.Enumerable;
#else
using Enumerable = GeoAPI.DataStructures.Enumerable;
using Caster = GeoAPI.DataStructures.Caster;
#endif




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

        private delegate void SetLockedDelegate(DataView view, Boolean locked);

        private delegate void SetIndex2Delegate(
            FeatureDataView view, String newSort, DataViewRowState dataViewRowState, Object expression, Boolean fireEvent);

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
        private FeatureQueryExpression _viewDefinition;
        private Boolean _reindexingEnabled = true;
        private Boolean _shouldReindex;
        private IExtents _extents;
        private Boolean _isViewDefinitionExlcusive;
        #endregion

        #region Object constructors

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/>.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        public FeatureDataView(FeatureDataTable table)
            : this(table, (FeatureQueryExpression)null, "", DataViewRowState.CurrentRows) { }

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
        public FeatureDataView(FeatureDataTable table,
                               IGeometry intersectionFilter,
                               String sort,
                               DataViewRowState rowState)
            : this(table, intersectionFilter, SpatialOperation.Intersects, sort, rowState) { }

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/> having the specified geometry filter, 
        /// sort order and row state filter.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        /// <param name="query">
        /// Geometry used in building view to filter feature table rows.
        /// </param>
        /// <param name="op">
        /// Type of spatial relation which <paramref name="query"/> has to features.
        /// </param>
        /// <param name="sort">Sort expression to order view by.</param>
        /// <param name="rowState">Filter on the state of the rows to view.</param>
        public FeatureDataView(FeatureDataTable table,
                               IGeometry query,
                               SpatialOperation op,
                               String sort,
                               DataViewRowState rowState)
            : this(table, new FeatureQueryExpression(query, op, table), sort, rowState) { }

        /// <summary>
        /// Creates a new <see cref="FeatureDataView"/> on the given
        /// <see cref="FeatureDataTable"/> having the specified geometry filter, 
        /// sort order and row state filter.
        /// </summary>
        /// <param name="table">Table to create view on.</param>
        /// <param name="definition">
        /// Spatial expression used in building view to filter feature table rows.
        /// </param>
        /// <param name="sort">Sort expression to order view by.</param>
        /// <param name="rowState">Filter on the state of the rows to view.</param>
        public FeatureDataView(FeatureDataTable table,
                               FeatureQueryExpression definition,
                               String sort,
                               DataViewRowState rowState)
            : base(
                table,
                "",
                String.IsNullOrEmpty(sort)
                            ? table.PrimaryKey.Length == 1
                                    ? table.PrimaryKey[0].ColumnName
                                    : ""
                            : sort,
                rowState)
        {
            _viewDefinition = definition;

            // This call rebuilds the index which was just built with 
            // the call into the base constructor, which may be a performance
            // hit. A more robust solution would be to just recreate the 
            // behavior of the base constructor here, so we can create the 
            // underlying index once.
            setFilterPredicate();
        }

        internal FeatureDataView(FeatureDataTable table, Boolean locked)
            : this(table)
        {
            // The DataView is locked when it is created as a default
            // view for a table.
            _setLocked(this, locked);
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating that the <see cref="ViewDefinition"/>
        /// filter is inclusive of the <see cref="Table"/>'s rows (<see langword="true"/>),
        /// or exclusive (<see langword="false"/>).
        /// </summary>
        public Boolean IsViewDefinitionExclusive
        {
            get { return _isViewDefinitionExlcusive; }
            set
            {
                if (value == _isViewDefinitionExlcusive)
                {
                    return;
                }

                _isViewDefinitionExlcusive = value;
                resetInternal();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="SpatialBinaryExpression"/> instance used
        /// to filter the table data based on a spatial relation.
        /// </summary>
        public SpatialBinaryExpression SpatialFilter
        {
            get
            {
                return _viewDefinition == null || _viewDefinition.SpatialPredicate == null
                            ? null
                            : _viewDefinition.SpatialPredicate.Clone() as SpatialBinaryExpression;
            }
            set
            {
                if (_viewDefinition == null)
                {
                    if (value != null)
                    {
                        ViewDefinition = new FeatureQueryExpression(new AllAttributesExpression(), value, null);
                    }
                }
                else
                {
                    // short-circuit if it is an equal query, since we don't want to have to 
                    // recompute all the indexes for nothing
                    if (Equals(_viewDefinition.SpatialPredicate, value))
                    {
                        return;
                    }

                    ViewDefinition = new FeatureQueryExpression(_viewDefinition, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a set of feature identifiers (oids) used
        /// to include or exclude features from the view. 
        /// </summary>
        /// <remarks>
        /// A value of <see langword="null"/> clears the filter, and allows
        /// all features regardless of oid value.
        /// </remarks>
        public OidCollectionExpression OidFilter
        {
            get
            {
                return _viewDefinition == null || _viewDefinition.OidPredicate == null
                            ? null
                            : _viewDefinition.OidPredicate.Clone() as OidCollectionExpression;
            }
            set
            {
                if (_viewDefinition == null)
                {
                    if (value != null)
                    {
                        ViewDefinition = new FeatureQueryExpression(new AllAttributesExpression(), value, null);
                    }
                }
                else
                {
                    if (Equals(_viewDefinition.OidPredicate, value))
                    {
                        return;
                    }

                    PredicateExpression predicate = _viewDefinition.Predicate == null
                                                        ? (PredicateExpression)value
                                                        : new BinaryExpression(value,
                                                                               BinaryOperator.And,
                                                                               _viewDefinition.SpatialPredicate);

                    ViewDefinition = new FeatureQueryExpression(_viewDefinition.Projection,
                                                                predicate, null);
                }
            }
        }

        /// <summary>
        /// Gets or sets a set of feature identifiers (oids) used
        /// to include or exclude features from the view. 
        /// </summary>
        /// <remarks>
        /// A value of <see langword="null"/> clears the filter, and allows
        /// all features regardless of oid value.
        /// </remarks>
        public AttributeBinaryExpression AttributeFilter
        {
            get
            {
                return _viewDefinition == null || _viewDefinition.SingleAttributePredicate == null
                            ? null
                            : _viewDefinition.SingleAttributePredicate.Clone() as AttributeBinaryExpression;
            }
            set
            {
                if (Equals(_viewDefinition.SingleAttributePredicate, value))
                {
                    return;
                }

                // TODO: create new predicate based on existing predicate and new attribute filter
                throw new NotImplementedException("Filtering by attribute value not yet supported.");
            }
        }

        /// <summary>
        /// Gets the bounds which this view covers as an <see cref="IExtents"/>.
        /// </summary>
        public IExtents Extents
        {
            get
            {
                return _extents;
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

        /// <summary>
        /// Gets or sets a <see cref="FeatureQueryExpression"/> which defines the 
        /// set of columns as well as which rows are included in the view based on the 
        /// underlying <see cref="Table"/>.
        /// </summary>
        public FeatureQueryExpression ViewDefinition
        {
            get
            {
                return _viewDefinition == null
                    ? null
                    : _viewDefinition.Clone() as FeatureQueryExpression;
            }
            set
            {
                if (Equals(_viewDefinition, value))
                {
                    return;
                }

                _viewDefinition = value;

                //// NOTE: changed Point.Empty to null
                //IGeometry missingGeometry;
                //ArrayList missingOids = new ArrayList();

                //if (!_viewDefinition.QueryGeometry.IsEmpty &&
                //    !Table.Contains(_viewDefinition.QueryGeometry))
                //{
                //    missingGeometry = _viewDefinition.QueryGeometry; //.Difference(Table.LoadedRegion);
                //}
                //else
                //{
                //    missingGeometry = Table.GeometryFactory.CreatePoint();
                //}

                //foreach (Object oid in value.Oids)
                //{
                //    FeatureDataRow feature = Table.Find(oid);

                //    if (feature == null || !feature.IsFullyLoaded)
                //    {
                //        missingOids.Add(oid);
                //    }
                //}

                //if (!missingGeometry.IsEmpty || missingOids.Count > 0)
                //{
                //    FeatureQueryExpression notFound = new FeatureQueryExpression(missingGeometry,
                //                                                                 value.QueryType,
                //                                                                 missingOids);
                //    Table.NotifyFeaturesNotFound(notFound);
                //}

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

        /// <summary>
        /// Gets or sets a value which enables (<see langword="true"/>)
        /// or disables (<see langword="false"/>) the automatic indexing of the view
        /// based on the <see cref="ViewDefinition"/> used.
        /// </summary>
        /// <remarks>
        /// If <see cref="AutoIndexingEnabled"/> is set to <see langword="false"/>
        /// and a new <see cref="ViewDefinition"/> is set, the new columnset and/or 
        /// rowset will not be computed until 
        /// </remarks>
        public Boolean AutoIndexingEnabled
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

        /// <summary>
        /// Causes the internal index to be rebuilt, and the current columnset and rowset
        /// to be updated, even if <see cref="AutoIndexingEnabled"/> is <see langword="false"/>.
        /// </summary>
        public void ForceIndexRebuild()
        {
            Reset();
        }

        /// <summary>
        /// RowFilter expressions not supported at this time. 
        /// Use the ViewDefinition property instead.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override String RowFilter
        {
            // TODO: parse/serialize ViewDefinition?
            get
            {
                throw new NotSupportedException("RowFilter expressions not supported " +
                                                "at this time. Use the ViewDefinition property " +
                                                "instead.");
            }
            set
            {
                throw new NotSupportedException("RowFilter expressions not supported " +
                                                "at this time. Use the ViewDefinition property " +
                                                "instead.");
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

        protected override void ColumnCollectionChanged(Object sender, CollectionChangeEventArgs e)
        {
            base.ColumnCollectionChanged(sender, e);
        }

        protected override void IndexListChanged(Object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    FeatureDataRow feature = this[e.NewIndex].Row as FeatureDataRow;
                    Debug.Assert(feature != null);
                    if (_extents == null)
                    {
                        _extents = feature.Extents;
                    }
                    else
                    {
                        _extents.ExpandToInclude(feature.Extents);
                    }
                    break;
                case ListChangedType.Reset:
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemDeleted:
                    recomputeExtents();
                    break;
                default:
                    break;
            }

            base.IndexListChanged(sender, e);
        }

#if DEBUG
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }
#endif

        protected override void UpdateIndex(Boolean force)
        {
            if (AutoIndexingEnabled)
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

        internal void SetIndex2(String newSort, DataViewRowState dataViewRowState,
                                Object dataExpression, Boolean fireEvent)
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
            Object iFilter = createRowPredicateFilter(isRowInView);
            SetIndex2(Sort, RowStateFilter, iFilter, true);
        }

        private Boolean isRowInView(DataRow row)
        {
            FeatureDataRow feature = row as FeatureDataRow;

            if (feature == null)
            {
                return false;
            }

            Boolean matches = inGeometryFilter(feature) &&
                              inOidFilter(feature) &&
                              inAttributeFilter(feature);

            return _isViewDefinitionExlcusive
                       ? !matches
                       : matches;
        }

        private Boolean inGeometryFilter(FeatureDataRow feature)
        {
            if (_viewDefinition == null || _viewDefinition.SpatialPredicate == null)
            {
                return true;
            }

            SpatialBinaryExpression spatialQueryExpression = _viewDefinition.SpatialPredicate;
            SpatialOperation op = spatialQueryExpression.Op;

            GeometryExpression geometryExpression
                = spatialQueryExpression.SpatialExpression as GeometryExpression;
            ExtentsExpression extentsExpression
                = spatialQueryExpression.SpatialExpression as ExtentsExpression;

            if (!SpatialExpression.IsNullOrEmpty(geometryExpression))
            {
                return SpatialBinaryExpression.IsMatch(op,
                                                       spatialQueryExpression.IsSpatialExpressionLeft,
                                                       geometryExpression.Geometry,
                                                       feature.Geometry);
            }

            if (!SpatialExpression.IsNullOrEmpty(extentsExpression))
            {
                return SpatialBinaryExpression.IsMatch(op,
                                                       spatialQueryExpression.IsSpatialExpressionLeft,
                                                       extentsExpression.Extents,
                                                       feature.Extents);
            }

            return true;
        }

        private Boolean inOidFilter(FeatureDataRow feature)
        {
            if (!feature.HasOid)
            {
                return false;
            }

            if (_viewDefinition == null || _viewDefinition.OidPredicate == null)
            {
                return true;
            }

            // NOTE: This will get to be a performance problem due to the 
            // poor structuring of the OID values. Consider creating a local,
            // sorted index where a binary search can be performed.
            IEnumerable oids = _viewDefinition.OidPredicate.OidValues;

            Int32 count = 0;

            if (Enumerable.FirstOrDefault(Caster.Cast<object>(oids)) == null) //jd:added explicit type param to allow compiliation in net3.5
            {
                return true;
            }

            Object featureOid = feature.GetOid();

            Debug.Assert(featureOid != null);

            foreach (Object oid in oids)
            {
                if (featureOid.Equals(oid))
                {
                    return true;
                }
            }

            return false;
        }

        private Boolean inAttributeFilter(FeatureDataRow feature)
        {
            // TODO: convert the AttributePredicate to a DataExpression and set it here
            return true;
        }

        private void resetInternal()
        {
            // TODO: Perhaps resetting the entire index is a bit drastic... 
            // Reconsider how to enlist and retire rows incrementally, 
            // perhaps doing RTree diffs.
            if (AutoIndexingEnabled)
            {
                Reset();
            }
            else
            {
                _shouldReindex = true;
            }
        }

        private void recomputeExtents()
        {
            IEnumerable<FeatureDataRow> features = this;

            _extents = null;

            foreach (FeatureDataRow feature in features)
            {
                if (_extents == null)
                {
                    _extents = feature.Extents;
                }
                else
                {
                    _extents.ExpandToInclude(feature.Extents);
                }
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
                .GetConstructor(new Type[] { typeof(Object), typeof(IntPtr) });
            IntPtr setIndex2Pointer = setIndex2Info.MethodHandle.GetFunctionPointer();
            return (SetIndex2Delegate)setIndexDelegateCtor.Invoke(new Object[] { null, setIndex2Pointer });
        }

        private static SetLockedDelegate GenerateSetLockedDelegate()
        {
            // Use LCG to create a set accessor to the DataView.locked field
            DynamicMethod setLockedMethod = new DynamicMethod("set_locked_DynamicMethod",
                                                              null, new Type[] { typeof(DataView), typeof(Boolean) },
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

        private static Object createRowPredicateFilter(Predicate<DataRow> filter)
        {
#warning Reflection on internal type breaks in CLR versions less than v2.0.50727.1378
            // This is the only way we have to take control of what predicate is used to filter the DataView.
            // Unfortunately, this type is only available in the v2.0 CLR which ships with .Net v3.5 Beta 2 (v2.0.50727.1378)
            // Currently, the only two choices to provided spatially filtered views are to implement 
            // System.ComponentModel.IBindingListView or to rely on v3.5.
            String rowPredicateFilterTypeName =
                "System.Data.DataView+RowPredicateFilter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            Type rowPredicateFilterType = Type.GetType(rowPredicateFilterTypeName);
            Debug.Assert(rowPredicateFilterType != null,
                         "Can't find the type System.Data.DataView+RowPredicateFilter. " +
                         "This is probably because you are not running the v2.0 CLR which ships with .Net v3.5 Beta 2.");
            Object[] args = new Object[] { filter };
            Object rowPredicateFilter = Activator.CreateInstance(rowPredicateFilterType,
                                                                 BindingFlags.Instance | BindingFlags.NonPublic,
                                                                 null, args, null);

            return rowPredicateFilter;
        }

        #endregion
    }
}