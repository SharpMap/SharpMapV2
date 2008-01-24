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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using NPack;
using NPack.Interfaces;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap.Tools;
using SharpMap.Utilities;

namespace SharpMap
{
    /// <summary>
    /// A map is a collection of <see cref="Layer">layers</see> 
    /// composed into a single frame of spatial reference.
    /// </summary>
    [DesignTimeVisible(false)]
    public class Map : INotifyPropertyChanged, IDisposable
    {
        private static readonly PropertyDescriptorCollection _properties;

        static Map()
        {
            _properties = TypeDescriptor.GetProperties(typeof (Map));
        }

        #region PropertyDescriptors

        /// <summary>
        /// Gets a PropertyDescriptor for the Map's <see cref="ActiveTool"/> property.
        /// </summary>
        public static PropertyDescriptor ActiveToolProperty
        {
            get { return _properties.Find("ActiveTool", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for the Map's <see cref="Extents"/> property.
        /// </summary>
        public static PropertyDescriptor ExtentsProperty
        {
            get { return _properties.Find("Extents", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for the Map's <see cref="SpatialReference"/> property.
        /// </summary>
        public static PropertyDescriptor SpatialReferenceProperty
        {
            get { return _properties.Find("SpatialReference", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for the Map's <see cref="SelectedLayers"/> property.
        /// </summary>
        public static PropertyDescriptor SelectedLayersProperty
        {
            get { return _properties.Find("SelectedLayers", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for the Map's <see cref="Title"/> property.
        /// </summary>
        public static PropertyDescriptor TitleProperty
        {
            get { return _properties.Find("Title", false); }
        }

        #endregion

        #region Nested types

        #region LayerCollection type

        /// <summary>
        /// Represents an ordered collection of layers of geospatial features
        ///  which are composed into a map.
        /// </summary>
        public class LayerCollection : BindingList<ILayer>, ITypedList
        {
            private readonly Map _map;
            private Boolean? _sortedAscending = null;
            private readonly object _collectionChangeSync = new object();
            private PropertyDescriptor _sortProperty;
            private static readonly PropertyDescriptorCollection _layerProperties;
            internal readonly object LayersChangeSync = new object();

            static LayerCollection()
            {
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(ILayer));
				PropertyDescriptor[] propsArray = new PropertyDescriptor[props.Count];
                props.CopyTo(propsArray, 0);

                _layerProperties = new PropertyDescriptorCollection(propsArray, true);
            }

            internal LayerCollection(Map map)
            {
                _map = map;
                base.AllowNew = false;
            }

            internal void AddRange(IEnumerable<ILayer> layers)
            {
                RaiseListChangedEvents = false;

                foreach (ILayer layer in layers)
                {
                    Add(layer);
                }

                RaiseListChangedEvents = true;
                ResetBindings();
            }

            internal Boolean Exists(Predicate<ILayer> predicate)
            {
                foreach (ILayer layer in this)
                {
                    if (predicate(layer))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Gets a layer by its name, or <see langword="null"/> if the layer isn't found.
            /// </summary>
            /// <remarks>
            /// Performs culture-specific, case-insensitive search.
            /// </remarks>
            /// <param name="layerName">Name of layer.</param>
            /// <returns>
            /// Layer with <see cref="ILayer.LayerName"/> of <paramref name="layerName"/>.
            /// </returns>
            public ILayer this[String layerName]
            {
                get { return _map.GetLayerByName(layerName); }
            }

            #region AddNew support

            protected override object AddNewCore()
            {
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Adding new layers through LayerCollection is not supported. Always gets
            /// <see langword="false"/> and throws an exception if set.
            /// </summary>
            /// <exception cref="NotSupportedException">Thrown if set.</exception>
            public new Boolean AllowNew
            {
                get { return false; }
                set { throw new NotSupportedException(); }
            }

            protected override void OnAddingNew(AddingNewEventArgs e)
            {
                base.OnAddingNew(e);
            }

            public override void CancelNew(Int32 itemIndex)
            {
                base.CancelNew(itemIndex);
            }

            public override void EndNew(Int32 itemIndex)
            {
                base.EndNew(itemIndex);
            }

            #endregion

            #region Sorting support

            protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
            {
                lock (_collectionChangeSync)
                {
                    try
                    {
                        RaiseListChangedEvents = false;

                        if (prop.Name == "LayerName")
                        {
                            Int32 sortDirValue = (direction == ListSortDirection.Ascending ? 1 : -1);
                            Comparison<ILayer> comparison = delegate(ILayer lhs, ILayer rhs)
                                                            {
                                                                return sortDirValue*
                                                                       String.Compare(lhs.LayerName, rhs.LayerName,
                                                                                      StringComparison.
                                                                                          CurrentCultureIgnoreCase);
                                                            };

                            QuickSort.Sort(this, comparison);
                        }

                        _sortedAscending = (direction == ListSortDirection.Ascending);
                        _sortProperty = prop;
                    }
                    finally
                    {
                        RaiseListChangedEvents = true;
                        ResetBindings();
                    }
                }
            }

            protected override void RemoveSortCore()
            {
                _sortedAscending = null;
            }

            protected override Boolean SupportsSortingCore
            {
                get { return true; }
            }

            protected override Boolean IsSortedCore
            {
                get { return _sortedAscending.HasValue; }
            }

            protected override ListSortDirection SortDirectionCore
            {
                get
                {
                    if (!_sortedAscending.HasValue)
                    {
                        throw new InvalidOperationException("List is not sorted.");
                    }

                    return ((Boolean) _sortedAscending)
                               ? ListSortDirection.Ascending
                               : ListSortDirection.Descending;
                }
            }

            protected override PropertyDescriptor SortPropertyCore
            {
                get { return _sortProperty; }
            }

            #endregion

            #region Searching support

            protected override Int32 FindCore(PropertyDescriptor prop, object key)
            {
                if (prop.Name == "LayerName")
                {
                    String layerName = key as String;

                    if (String.IsNullOrEmpty(layerName))
                    {
                        throw new ArgumentException(
                            "Layer name must be a non-null, non-empty String.");
                    }

                    foreach (ILayer layer in this)
                    {
                        if (String.Compare(layerName, layer.LayerName, StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            return IndexOf(layer);
                        }
                    }

                    return -1;
                }
                else
                {
                    throw new NotSupportedException(
                        "Only sorting on the layer name is currently supported.");
                }
            }

            protected override Boolean SupportsSearchingCore
            {
                get { return true; }
            }

            #endregion

            protected override void ClearItems()
            {
                base.ClearItems();
            }

            protected override void InsertItem(Int32 index, ILayer item)
            {
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(Int32 index)
            {
                // This defines the missing "OnDeleting" functionality:
                // having a ListChangedEventArgs.NewIndex == -1 and
                // the index of the item pending removal to be 
                // ListChangedEventArgs.OldIndex.
                ListChangedEventArgs args
                    = new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, index);

                OnListChanged(args);

                base.RemoveItem(index);
            }

            protected override void SetItem(Int32 index, ILayer item)
            {
                base.SetItem(index, item);
            }

            protected override void OnListChanged(ListChangedEventArgs e)
            {
                base.OnListChanged(e);
            }

            protected override Boolean SupportsChangeNotificationCore
            {
                get { return true; }
            }

            #region ITypedList Members

            public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            {
                if (listAccessors != null)
                {
                    throw new NotSupportedException(
                        "Child lists not supported in LayersCollection.");
                }

                return _layerProperties;
            }

            /// <summary>
            /// Returns the name of the list.
            /// </summary>
            /// <param name="listAccessors">
            /// An array of <see cref="PropertyDescriptor"/> objects, 
            /// for which the list name is returned. This can be <see langword="null"/>.
            /// </param>
            /// <returns>The name of the list.</returns>
            /// <remarks>
            /// From the MSDN docs: This method is used only in the design-time framework 
            /// and by the obsolete DataGrid control.
            /// </remarks>
            public String GetListName(PropertyDescriptor[] listAccessors)
            {
                return "LayerCollection";
            }

            #endregion
        }

        #endregion LayerCollection

        #endregion Nested Types

        #region Fields

        private readonly object _activeToolSync = new object();
        private readonly IGeometryFactory _geoFactory;
        private readonly LayerCollection _layers;
        private readonly FeatureDataSet _featureDataSet;
        private readonly List<ILayer> _selectedLayers = new List<ILayer>();
        private IExtents _extents;
        private MapTool _activeTool = StandardMapTools2D.None;
        private ICoordinateSystem _spatialReference;
        private Boolean _disposed;
        private readonly String _defaultName;

        #endregion

        #region Object Creation / Disposal

        /// <summary>
        /// Creates a new instance of a Map with a title describing 
        /// when the map was created.
        /// </summary>
        public Map(IGeometryFactory geoFactory)
            : this("Map created " + DateTime.Now.ToShortDateString(), geoFactory)
        {
            _defaultName = _featureDataSet.DataSetName;
        }

        /// <summary>
        /// Creates a new instance of a Map with the given title.
        /// </summary>
        public Map(String title, IGeometryFactory geoFactory)
        {
            _geoFactory = geoFactory;
            _layers = new LayerCollection(this);
            _layers.ListChanged += handleLayersChanged;
            _featureDataSet = new FeatureDataSet(title);
        }

        #region Dispose Pattern

        ~Map()
        {
            Dispose(false);
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources deterministically.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                _disposed = true;
                GC.SuppressFinalize(this);

                EventHandler e = Disposed;
                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        /// <summary>
        /// Disposes the map object and all layers.
        /// </summary>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                foreach (ILayer layer in Layers)
                {
                    if (layer != null)
                    {
                        layer.Dispose();
                    }
                }

                _layers.Clear();
            }
        }

        /// <summary>
        /// Gets whether this layer is disposed, and no longer accessible.
        /// </summary>
        public Boolean IsDisposed
        {
            get { return _disposed; }
        }

        /// <summary>
        /// Event fired when the layer is disposed.
        /// </summary>
        public event EventHandler Disposed;

        #endregion

        #endregion

        #region Events

        ///// <summary>
        ///// Event fired when layers have been added to the map.
        ///// </summary>
        //public event EventHandler<LayersChangedEventArgs> LayersChanged;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Adds the given layer to the map, ordering it under all other layers.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        public void AddLayer(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            checkForDuplicateLayerName(layer);

            lock (Layers.LayersChangeSync)
            {
                _layers.Add(layer);
            }
        }

        /// <summary>
        /// Adds the given set of layers to the map, 
        /// ordering each one in turn under all other layers.
        /// </summary>
        /// <param name="layers">The set of layers to add.</param>
        public void AddLayers(IEnumerable<ILayer> layers)
        {
            if (layers == null)
            {
                throw new ArgumentNullException("layers");
            }

            List<String> layerNames = new List<String>(16);

            foreach (ILayer layer in layers)
            {
                if (layer == null)
                {
                    throw new ArgumentException("One of the layers is null.");
                }
                checkForDuplicateLayerName(layer);
                layerNames.Add(layer.LayerName);
            }

            for (Int32 i = 0; i < layerNames.Count; i++)
            {
                for (Int32 j = i + 1; j < layerNames.Count; j++)
                {
                    if (String.Compare(layerNames[i], layerNames[j], StringComparison.CurrentCultureIgnoreCase) == 0)
                    {
                        throw new ArgumentException("Layers to be added contain a duplicate name: " + layerNames[i]);
                    }
                }
            }

            lock (Layers.LayersChangeSync)
            {
                _layers.AddRange(layers);
            }
        }

        /// <summary>
        /// Removes all the layers from the map.
        /// </summary>
        public void ClearLayers()
        {
            _layers.Clear();
        }

        /// <summary>
        /// Disables the given layer so it is not visible and doesn't participate in
        /// spatial queries.
        /// </summary>
        /// <param name="index">The index of the layer to disable.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to 
        /// Layers.Count.
        /// </exception>
        public void DisableLayer(Int32 index)
        {
            if (index < 0 || index >= Layers.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            lock (Layers.LayersChangeSync)
            {
                changeLayerEnabled(Layers[index], false);
            }
        }

        /// <summary>
        /// Disables the given layer so it is not visible and doesn't participate in
        /// spatial queries.
        /// </summary>
        /// <param name="name">Name of layer to disable.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/> or empty.
        /// </exception>
        public void DisableLayer(String name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            lock (Layers.LayersChangeSync)
            {
                changeLayerEnabled(GetLayerByName(name), false);
            }
        }

        /// <summary>
        /// Disables the given layer so it is not visible and doesn't participate in
        /// spatial queries.
        /// </summary>
        /// <param name="layer">Layer to disable.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="layer"/> is <see langword="null"/>.
        /// </exception>
        public void DisableLayer(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            lock (Layers.LayersChangeSync)
            {
                changeLayerEnabled(layer, false);
            }
        }

        /// <summary>
        /// Enables the given layer so it is visible and participates in
        /// spatial queries.
        /// </summary>
        /// <param name="index">Index of the layer to enable.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> is less than 0 or greater than or equal to 
        /// Layers.Count.
        /// </exception>
        public void EnableLayer(Int32 index)
        {
            if (index < 0 || index >= Layers.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            lock (Layers.LayersChangeSync)
            {
                changeLayerEnabled(Layers[index], false);
            }
        }

        /// <summary>
        /// Enables the given layer so it is visible and participates in
        /// spatial queries.
        /// </summary>
        /// <param name="name">Name of layer to enable.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="name"/> is <see langword="null"/> or empty.
        /// </exception>
        public void EnableLayer(String name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            lock (Layers.LayersChangeSync)
            {
                changeLayerEnabled(GetLayerByName(name), true);
            }
        }

        /// <summary>
        /// Enables the given layer so it is visible and participates in
        /// spatial queries.
        /// </summary>
        /// <param name="layer">Layer to enable.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="layer"/> is <see langword="null"/>.
        /// </exception>
        public void EnableLayer(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            lock (Layers.LayersChangeSync)
            {
                changeLayerEnabled(layer, true);
            }
        }

        /// <summary>
        /// Gets the extents of the map based on the extents of all the layers 
        /// in the layers collection.
        /// </summary>
        /// <returns>Full map extents.</returns>
        public IExtents Extents
        {
            get { return _extents; }
        }

        /// <summary>
        /// Returns an enumerable set of all layers containing the String 
        /// <paramref name="layerNamePart"/>  in the <see cref="ILayer.LayerName"/> property.
        /// </summary>
        /// <param name="layerNamePart">Part of the layer name to search for.</param>
        /// <returns>IEnumerable{ILayer} of all layers with <see cref="ILayer.LayerName"/> 
        /// containing <paramref name="layerNamePart"/>.</returns>
        public IEnumerable<ILayer> FindLayers(String layerNamePart)
        {
            lock (Layers.LayersChangeSync)
            {
                layerNamePart = layerNamePart.ToLower();
                foreach (ILayer layer in Layers)
                {
                    String layerName = layer.LayerName.ToLower(CultureInfo.CurrentCulture);

                    if (layerName.Contains(layerNamePart))
                    {
                        yield return layer;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the active tool for map interaction.
        /// </summary>
        /// <typeparam name="TMapView">Type of Map view in use.</typeparam>
        /// <typeparam name="TPoint">Type of vertex used to define a point.</typeparam>
        /// <returns>The currently active MapTool for the map.</returns>
        public MapTool<TMapView, TPoint> GetActiveTool<TMapView, TPoint>()
            where TPoint : IVector<DoubleComponent>
        {
            return ActiveTool as MapTool<TMapView, TPoint>;
        }

        /// <summary>
        /// Returns a layer by its name, or <see langword="null"/> if the layer isn't found.
        /// </summary>
        /// <remarks>
        /// Performs culture-specific, case-insensitive search.
        /// </remarks>
        /// <param name="name">Name of layer.</param>
        /// <returns>
        /// Layer with <see cref="ILayer.LayerName"/> of <paramref name="name"/>.
        /// </returns>
        public ILayer GetLayerByName(String name)
        {
            lock (Layers.LayersChangeSync)
            {
                Int32 index = (_layers as IBindingList).Find(Layer.LayerNameProperty, name);

                if (index < 0)
                {
                	foreach (ILayer layer in _layers)
                	{
                		if(layer is LayerGroup)
                		{
                			ILayer groupMember = (layer as LayerGroup)[name];

                			if(groupMember != null)
                			{
                				return groupMember;
                			}
                		}
                	}
                    return null;
                }

                return _layers[index];
            }
        }

        /// <summary>
        /// Removes a layer from the map.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        public void RemoveLayer(ILayer layer)
        {
            if (layer != null)
            {
                lock (Layers.LayersChangeSync)
                {
                    _layers.Remove(layer);
                }
            }
        }

        /// <summary>
        /// Removes a layer from the map using the given layer name.
        /// </summary>
        /// <param name="name">The name of the layer to remove.</param>
        public void RemoveLayer(String name)
        {
            lock (Layers.LayersChangeSync)
            {
                ILayer layer = GetLayerByName(name);
                RemoveLayer(layer);
            }
        }

        /// <summary>
        /// Selects a layer, using the given index, to be the target of action on the map.
        /// </summary>
        /// <param name="index">The index of the layer to select.</param>
        public void SelectLayer(Int32 index)
        {
            lock (Layers.LayersChangeSync)
            {
                SelectLayers(new Int32[] {index});
            }
        }

        /// <summary>
        /// Selects a layer, using the given name, to be the target of action on the map.
        /// </summary>
        /// <param name="name">The name of the layer to select.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="name"/> is <see langword="null"/> or empty.
        /// </exception>
        public void SelectLayer(String name)
        {
            lock (Layers.LayersChangeSync)
            {
                SelectLayers(new String[] {name});
            }
        }

        /// <summary>
        /// Selects a layer to be the target of action on the map.
        /// </summary>
        /// <param name="layer">The layer to select.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="layer"/> is <see langword="null"/> or empty.
        /// </exception>
        public void SelectLayer(ILayer layer)
        {
            lock (Layers.LayersChangeSync)
            {
                SelectLayers(new ILayer[] {layer});
            }
        }

        /// <summary>
        /// Selects a set of layers, using the given index set, 
        /// to be the targets of action on the map.
        /// </summary>
        /// <param name="indexes">The set of indexes of the layers to select.</param>
        public void SelectLayers(IEnumerable<Int32> indexes)
        {
            if (indexes == null)
            {
                throw new ArgumentNullException("indexes");
            }

            lock (Layers.LayersChangeSync)
            {
                Converter<IEnumerable<Int32>, IEnumerable<ILayer>> layerGenerator = layersGenerator;
                selectLayersInternal(layerGenerator(indexes));
            }
        }

        /// <summary>
        /// Selects a set of layers, using the given set of names, 
        /// to be the targets of action on the map.
        /// </summary>
        /// <param name="layerNames">The set of names of layers to select.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="layerNames"/> is <see langword="null"/>.
        /// </exception>
        public void SelectLayers(IEnumerable<String> layerNames)
        {
            if (layerNames == null)
            {
                throw new ArgumentNullException("layerNames");
            }

            lock (Layers.LayersChangeSync)
            {
                Converter<IEnumerable<String>, IEnumerable<ILayer>> layerGenerator = layersGenerator;
                selectLayersInternal(layerGenerator(layerNames));
            }
        }

        /// <summary>
        /// Selects a set of layers to be the targets of action on the map.
        /// </summary>
        /// <param name="layers">The set of layers to select.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="layers"/> is <see langword="null"/>.
        /// </exception>
        public void SelectLayers(IEnumerable<ILayer> layers)
        {
            if (layers == null)
            {
                throw new ArgumentNullException("layers");
            }

            lock (Layers.LayersChangeSync)
            {
                selectLayersInternal(layers);
            }
        }

        public void SetLayerStyle(Int32 index, Style style)
        {
            if (index < 0 || index >= Layers.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            lock (Layers.LayersChangeSync)
            {
                setLayerStyleInternal(Layers[index], style);
            }
        }

        public void SetLayerStyle(String name, Style style)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            lock (Layers.LayersChangeSync)
            {
                setLayerStyleInternal(GetLayerByName(name), style);
            }
        }

        public void SetLayerStyle(ILayer layer, Style style)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            lock (Layers.LayersChangeSync)
            {
                setLayerStyleInternal(layer, style);
            }
        }

        /// <summary>
        /// Deselects a layer given by its index from being 
        /// the target of action on the map.
        /// </summary>
        /// <param name="index">The index of the layer to deselect.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <paramref name="index"/> is less than 0 or greater than or equal
        /// to Layers.Count.
        /// </exception>
        public void DeselectLayer(Int32 index)
        {
            DeselectLayers(new Int32[] {index});
        }

        /// <summary>
        /// Deselects a layer given by its name from being 
        /// the target of action on the map.
        /// </summary>
        /// <param name="name">The name of the layer to deselect.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        public void DeselectLayer(String name)
        {
            DeselectLayers(new String[] {name});
        }

        /// <summary>
        /// Deselects a layer from being 
        /// the target of action on the map.
        /// </summary>
        /// <param name="layer">The layer to deselect.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="layer"/> is <see langword="null"/>.
        /// </exception>
        public void DeselectLayer(ILayer layer)
        {
            DeselectLayers(new ILayer[] {layer});
        }

        /// <summary>
        /// Deselects a set of layers given by their index 
        /// from being the targets of action on the map.
        /// </summary>
        /// <param name="indexes">A set of indexes of layers to deselect.</param>
        /// <exception cref="ArgumentException">
        /// If <paramref name="indexes"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If any item in <paramref name="indexes"/> is less than 0 or greater than or equal
        /// to Layers.Count.
        /// </exception>
        public void DeselectLayers(IEnumerable<Int32> indexes)
        {
            if (indexes == null)
            {
                throw new ArgumentNullException("indexes");
            }

            lock (Layers.LayersChangeSync)
            {
                Converter<IEnumerable<Int32>, IEnumerable<ILayer>> layerGenerator
                    = layersGenerator;

                unselectLayersInternal(layerGenerator(indexes));
            }
        }

        /// <summary>
        /// Deselects a set of layers given by their names 
        /// from being the targets of action on the map.
        /// </summary>
        /// <param name="layerNames">A set of names of layers to deselect.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="layerNames"/> is <see langword="null"/>.
        /// </exception>
        public void DeselectLayers(IEnumerable<String> layerNames)
        {
            if (layerNames == null)
            {
                throw new ArgumentNullException("layerNames");
            }

            lock (Layers.LayersChangeSync)
            {
                unselectLayersInternal(layersGenerator(layerNames));
            }
        }

        /// <summary>
        /// Deselects a set of layers from being the targets of action on the map.
        /// </summary>
        /// <param name="layers">The set of layers to deselect.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="layers"/> is <see langword="null"/>.
        /// </exception>
        public void DeselectLayers(IEnumerable<ILayer> layers)
        {
            if (layers == null)
            {
                throw new ArgumentNullException("layers");
            }

            lock (Layers.LayersChangeSync)
            {
                unselectLayersInternal(layers);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently active tool used to
        /// interact with the map.
        /// </summary>
        public MapTool ActiveTool
        {
            get
            {
                lock (_activeToolSync)
                {
                    return _activeTool;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value == _activeTool)
                {
                    return;
                }

                lock (_activeToolSync)
                {
                    _activeTool = value;
                    onActiveToolChanged();
                }
            }
        }

        /// <summary>
        /// Gets center of map in world coordinates.
        /// </summary>
        public ICoordinate Center
        {
            get { return _extents.Center; }
        }

        public IGeometryFactory GeometryFactory
        {
            get { return _geoFactory; }
        }

        /// <summary>
        /// Gets a collection of layers. 
        /// </summary>
        /// <remarks>
        /// The first layer in the list is drawn first, the last one on top.
        /// </remarks>
        public LayerCollection Layers
        {
            get { return _layers; }
        }

        /// <summary>
        /// Gets or sets the name of the map.
        /// </summary>
        public String Title
        {
            get { return _featureDataSet.DataSetName; }
            set
            {
                if (value == _featureDataSet.DataSetName)
                {
                    return;
                }

                _featureDataSet.DataSetName = value ?? _defaultName;

                onNameChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of layers which are
        /// selected.
        /// </summary>
        public ReadOnlyCollection<ILayer> SelectedLayers
        {
            get
            {
                lock (Layers.LayersChangeSync)
                {
                    return _selectedLayers.AsReadOnly();
                }
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                foreach (ILayer layer in value)
                {
                    if (!Layers.Contains(layer))
                    {
                        throw new ArgumentException(
                            "The set of layers contains a layer {0} which is not " +
                            "currently part of the map. Please add the layer to " +
                            "the map before selecting it.");
                    }
                }

                lock (Layers.LayersChangeSync)
                {
                    _selectedLayers.Clear();
                    _selectedLayers.AddRange(value);
                    onSelectedLayersChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the spatial reference for the entire map.
        /// </summary>
        public ICoordinateSystem SpatialReference
        {
            get { return _spatialReference; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (value == _spatialReference)
                {
                    return;
                }

                _spatialReference = value;
                onSpatialReferenceChanged();
            }
        }

        /// <summary>
        /// Gets a FeatureDataSet containing all the loaded 
        /// features in all the enabled layers in the map.
        /// </summary>
        public FeatureDataSet Features
        {
            get { throw new NotImplementedException(); }
        }

        ///// <summary>
        ///// Gets or sets the current visible envelope of the map.
        ///// </summary>
        //public BoundingBox VisibleRegion
        //{
        //    get { return _visibleEnvelope; }
        //    set
        //    {
        //        if (_visibleEnvelope == value)
        //        {
        //            return;
        //        }

        //        _visibleEnvelope = value;

        //        foreach (ILayer layer in Layers)
        //        {
        //            layer.VisibleRegion = value;
        //        }

        //        onVisibleRegionChanged();
        //    }
        //}

        #endregion

        #region Private helper methods

        #region Event Generators

        private void onActiveToolChanged()
        {
            raisePropertyChanged(ActiveToolProperty.Name);
        }

        private void onSpatialReferenceChanged()
        {
            raisePropertyChanged(SpatialReferenceProperty.Name);
        }

        private void onSelectedLayersChanged()
        {
            raisePropertyChanged(SelectedLayersProperty.Name);
        }

        private void onNameChanged()
        {
            raisePropertyChanged(TitleProperty.Name);
        }

        private void raisePropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;

            if (e != null)
            {
                e(null, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void checkForDuplicateLayerName(ILayer layer)
        {
            Predicate<ILayer> namesMatch =
                delegate(ILayer match)
                {
                    return String.Compare(match.LayerName, layer.LayerName,
                                          StringComparison.CurrentCultureIgnoreCase) == 0;
                };

            if (_layers.Exists(namesMatch))
            {
                throw new DuplicateLayerException(layer.LayerName);
            }
        }

        private void handleLayersChanged(object sender, ListChangedEventArgs args)
        {
            switch (args.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                {
                    ILayer layer = _layers[args.NewIndex];
                    _extents.ExpandToInclude(layer.Extents);
                }
                    break;
                case ListChangedType.ItemChanged:
                    if (args.PropertyDescriptor.Name == Layer.ExtentsProperty.Name)
                    {
                        ILayer layer = _layers[args.NewIndex];
                        _extents.ExpandToInclude(layer.Extents);
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    recomputeExtents();
                    break;
                case ListChangedType.Reset:
                    recomputeExtents();
                    break;
                case ListChangedType.ItemMoved:
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                default:
                    break;
            }
        }

        private void recomputeExtents()
        {
            IExtents extents = null;

            foreach (ILayer layer in Layers)
            {
                if (layer.Enabled)
                {
                    if (extents == null)
                    {
                        extents = layer.Extents;
                    }
                    else
                    {
                        extents.ExpandToInclude(layer.Extents);
                    }
                }
            }

            _extents = extents;
        }

        private static void changeLayerEnabled(ILayer layer, Boolean enabled)
        {
            layer.Style.Enabled = enabled;
        }

        private static void setLayerStyleInternal(ILayer layer, IStyle style)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            if (style == null)
            {
                throw new ArgumentNullException("style");
            }

            layer.Style = style;
        }

        private void selectLayersInternal(IEnumerable<ILayer> layers)
        {
            checkLayersExist();

            foreach (ILayer layer in layers)
            {
                Predicate<ILayer> findDuplicate = delegate(ILayer match)
                                                  {
                                                      return String.Compare(layer.LayerName, match.LayerName,
                                                                            StringComparison.CurrentCultureIgnoreCase) ==
                                                             0;
                                                  };

                if (!_selectedLayers.Exists(findDuplicate))
                {
                    _selectedLayers.Add(layer);
                }
            }

            onSelectedLayersChanged();
        }

        private void unselectLayersInternal(IEnumerable<ILayer> layers)
        {
            checkLayersExist();

            List<ILayer> removeLayers = layers is List<ILayer>
                                            ? layers as List<ILayer>
                                            : new List<ILayer>(layers);

            Predicate<ILayer> removeMatch = delegate(ILayer match) { return removeLayers.Contains(match); };
            _selectedLayers.RemoveAll(removeMatch);

            onSelectedLayersChanged();
        }

        private IEnumerable<ILayer> layersGenerator(IEnumerable<Int32> layerIndexes)
        {
            foreach (Int32 index in layerIndexes)
            {
                if (index < 0 || index >= _layers.Count)
                {
                    throw new ArgumentOutOfRangeException("layerIndexes", index,
                                                          String.Format("Layer index must be between 0 and {0}",
                                                                        _layers.Count));
                }

                yield return _layers[index];
            }
        }

        private IEnumerable<ILayer> layersGenerator(IEnumerable<String> layerNames)
        {
            foreach (String name in layerNames)
            {
                if (String.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Layer name must not be null or empty.", "layerNames");
                }

                ILayer layer = GetLayerByName(name);

                if (layer != null)
                {
                    yield return layer;
                }
            }
        }

        private void checkLayersExist()
        {
            if (_layers.Count == 0)
            {
                throw new InvalidOperationException(
                    "No layers are present in the map, so layer operation cannot be performed");
            }
        }

        #endregion
    }
}
