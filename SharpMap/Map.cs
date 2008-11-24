// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Diagnostics;
using GeoAPI.Geometries;
using NPack;
using NPack.Interfaces;
using SharpMap.Data;
using SharpMap.Expressions;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.Tools;

namespace SharpMap
{
    /// <summary>
    /// A map is a collection of <see cref="Layer">layers</see> 
    /// composed into a single frame of spatial reference, and a set
    /// of <see cref="IMapTool"/>s for interacting with them.
    /// </summary>
    [DesignTimeVisible(false)]
    public class Map : INotifyPropertyChanged, IDisposable
    {
        private static readonly PropertyDescriptorCollection _properties;

        static Map()
        {
            _properties = TypeDescriptor.GetProperties(typeof(Map));
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

        #region Fields

        private IGeometryFactory _geoFactory;
        private readonly LayerCollection _layers;
        private readonly FeatureDataSet _featureDataSet;
        private readonly List<ILayer> _selectedLayers = new List<ILayer>();
        private IExtents _extents;
        private readonly IPoint _emptyPoint;
        // 3D_UNSAFE: - change this initialization
        private IMapTool _activeTool = StandardMapView2DMapTools.None;

        // I18N_UNSAFE
        private IMapToolSet _mapTools = new MapToolSet("All Tools");
        private ICoordinateSystem _spatialReference;
        private Boolean _disposed;
        private readonly String _defaultName;
        private ICoordinateTransformationFactory _coordTransformFactory;

        #endregion

        #region Object Creation / Disposal
        /// <summary>
        /// Creates a new instance of a <see cref="Map"/> with a title describing 
        /// when the map was created.
        /// </summary>
        public Map(IGeometryFactory geoFactory)
            : this(geoFactory, null) { }

        /// <summary>
        /// Creates a new instance of a <see cref="Map"/> with a title describing 
        /// when the map was created.
        /// </summary>
        public Map(IGeometryFactory geoFactory, ICoordinateTransformationFactory coordTransformFactory)
            // I18N_UNSAFE
            : this("Map created " + DateTime.Now.ToShortDateString(), geoFactory, coordTransformFactory)
        {
            _defaultName = _featureDataSet.DataSetName;
        }

        /// <summary>
        /// Creates a new instance of a Map with the given title.
        /// </summary>
        public Map(String title, IGeometryFactory geoFactory, ICoordinateTransformationFactory coordTransformFactory)
        {
            _geoFactory = geoFactory;
            _coordTransformFactory = coordTransformFactory;
            _emptyPoint = _geoFactory.CreatePoint();
            _layers = new LayerCollection(this);
            _layers.ListChanged += handleLayersChanged;
            _featureDataSet = new FeatureDataSet(title, geoFactory);

            // TODO: tool configuration should come from a config file and / or reflection
            IMapTool[] mapTools = new IMapTool[]
                    {
                        StandardMapView2DMapTools.Pan, 
                        StandardMapView2DMapTools.Query, 
                        StandardMapView2DMapTools.ZoomIn,
                        StandardMapView2DMapTools.ZoomOut
                    };

            // I18N_UNSAFE
            Tools = new MapToolSet("Standard Map View Tools", mapTools);
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
            if (IsDisposed)
            {
                return;
            }

            Dispose(true);
            _disposed = true;
            GC.SuppressFinalize(this);

            OnDisposed();
        }

        #endregion

        /// <summary>
        /// Disposes the map object and all layers.
        /// </summary>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                _layers.ListChanged -= handleLayersChanged;

                foreach (ILayer layer in Layers)
                {
                    if (layer != null)
                    {
                        layer.Dispose();
                    }
                }

                _layers.Clear();
                unwireTools(Tools);
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

        public void AddLayer(IProvider provider)
        {
            throw new NotImplementedException("Possible future method...");
        }

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
            get
            {
                if (_extents == null)
                {
                    _extents = _geoFactory.CreateExtents();

                    foreach (ILayer layer in _layers)
                    {
                        _extents.ExpandToInclude(layer.Extents);
                    }
                }

                return _extents;
            }
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
                        if (layer is LayerGroup)
                        {
                            ILayer groupMember = (layer as LayerGroup)[name];

                            if (groupMember != null)
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
                SelectLayers(new Int32[] { index });
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
                SelectLayers(new String[] { name });
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
                SelectLayers(new ILayer[] { layer });
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
            DeselectLayers(new Int32[] { index });
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
            DeselectLayers(new String[] { name });
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
            DeselectLayers(new ILayer[] { layer });
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

        public void SelectFeatures(ICoordinate min, ICoordinate max)
        {
            IExtents extents = _geoFactory.CreateExtents(min, max);
            FeatureQueryExpression query = FeatureQueryExpression.Intersects(extents);
            List<IFeatureLayer> layersWithSelection = new List<IFeatureLayer>();

            foreach (IFeatureLayer layer in _layers)
            {
                if (layer == null || !layer.Enabled || !layer.AreFeaturesSelectable)
                {
                    continue;
                }

                layer.Features.SuspendIndexEvents();
                layer.Select(query);
                layersWithSelection.Add(layer);
            }

            foreach (IFeatureLayer layer in layersWithSelection)
            {
                layer.Features.RestoreIndexEvents(false);
            }
        }

        #endregion

        #region Properties

        public IMapToolSet Tools
        {
            get { return _mapTools; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (ReferenceEquals(_mapTools, value))
                {
                    return;
                }

                if (_mapTools != null)
                {
                    unwireTools(_mapTools);
                }

                _mapTools = value;

                wireupTools(_mapTools);
            }
        }

        /// <summary>
        /// Gets or sets the currently active tool used to
        /// interact with the map.
        /// </summary>
        public IMapTool ActiveTool
        {
            get
            {
                return _activeTool;
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

                _activeTool = value;
                onActiveToolChanged();
            }
        }

        /// <summary>
        /// Gets center of map in world coordinates.
        /// </summary>
        public ICoordinate Center
        {
            get { return _extents == null ? _emptyPoint.Coordinate : _extents.Center; }
        }

        public IGeometryFactory GeometryFactory
        {
            get { return _geoFactory; }
        }

        public ICoordinateTransformationFactory CoordinateTransformFactory
        {
            get { return _coordTransformFactory; }
            set { _coordTransformFactory = value; }
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
                        // I18N_UNSAFE
                        throw new ArgumentException("The set of layers contains a layer {0} which " +
                                                    "is not currently part of the map. " +
                                                    "Please add the layer to the map " +
                                                    "before selecting it.");
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
        /// Gets a <see cref="FeatureDataSet"/> containing all the loaded 
        /// features in all the enabled layers in the map.
        /// </summary>
        public FeatureDataSet Features
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Protected virtual members

        protected virtual void OnDisposed()
        {
            EventHandler e = Disposed;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Private helper methods

        #region Event Generators

        private void onActiveToolChanged()
        {
            raisePropertyChanged(ActiveToolProperty.Name);
        }

        private void onSpatialReferenceChanged()
        {
            _geoFactory = _geoFactory.Clone();
            _geoFactory.SpatialReference = SpatialReference;

            foreach (ILayer layer in _layers)
            {
                if (!layer.SpatialReference.EqualParams(SpatialReference))
                {
                    if (layer.CoordinateTransformation != null)
                    {
                        // TODO: do we ever need to support multiple transforms?
                        throw new InvalidOperationException("The layer already has a coordinate transform.");
                    }

                    layer.CoordinateTransformation
                        = CoordinateTransformFactory.CreateFromCoordinateSystems(layer.SpatialReference,
                                                                                 SpatialReference);
                }
            }

            _extents = null;

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

        private void handleToolsChanged(Object sender, MapToolSetChangedEventArgs args)
        {
            switch (args.Change)
            {
                case MapToolSetChange.ToolAdded:
                    wireupTool(args.Tool);
                    break;
                case MapToolSetChange.ToolRemoved:
                    unwireTool(args.Tool);
                    break;
                default:
                    throw Assert.ShouldNeverReachHereException();
            }
        }

        private void handleToolSelectedChanged(Object sender, EventArgs args)
        {
            IMapTool selected = sender as IMapTool;
            Assert.IsNotNull(selected);
            ActiveTool = selected;
        }

        private void handleLayersChanged(Object sender, ListChangedEventArgs args)
        {
            switch (args.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        ILayer layer = _layers[args.NewIndex];

                        if (_extents == null)
                        {
                            _extents = _geoFactory.CreateExtents(layer.Extents);
                        }
                        else
                        {
                            _extents.ExpandToInclude(layer.Extents);
                        }
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
                const StringComparison ignoreCase = StringComparison.CurrentCultureIgnoreCase;
                Predicate<ILayer> findDuplicate = delegate(ILayer match)
                                                  {
                                                      return String.Compare(layer.LayerName,
                                                                            match.LayerName,
                                                                            ignoreCase) == 0;
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
                throw new InvalidOperationException("No layers are present in the map, " +
                                                    "so layer operation cannot be performed");
            }
        }

        private void wireupTools(IMapToolSet mapTools)
        {
            foreach (IMapTool mapTool in mapTools)
            {
                IMapToolSet set = mapTool as IMapToolSet;

                if (set != null)
                {
                    mapTools.ToolAdded += handleToolsChanged;
                    mapTools.ToolRemoved += handleToolsChanged;
                    wireupTools(set);
                }
                else
                {
                    wireupTool(mapTool);
                }
            }
        }

        private void unwireTools(IMapToolSet mapTools)
        {
            foreach (IMapTool mapTool in mapTools)
            {
                IMapToolSet set = mapTool as IMapToolSet;

                if (set != null)
                {
                    mapTools.ToolAdded -= handleToolsChanged;
                    mapTools.ToolRemoved -= handleToolsChanged;
                    unwireTools(set);
                }
                else
                {
                    unwireTool(mapTool);
                }
            }
        }

        private void wireupTool(IMapTool mapTool)
        {
            mapTool.SelectedChanged += handleToolSelectedChanged;
        }

        private void unwireTool(IMapTool mapTool)
        {
            mapTool.SelectedChanged -= handleToolSelectedChanged;
        }

        #endregion
    }
}
