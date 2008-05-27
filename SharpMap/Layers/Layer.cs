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
using System.Collections;
using System.ComponentModel;
using System.Threading;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using SharpMap.Data;
using GeoAPI.Geometries;
using SharpMap.Data.Caching;
using SharpMap.Expressions;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// Abstract class for common layer properties and behavior.
    /// </summary>
    /// <remarks>
    /// Implement this class instead of the ILayer interface to 
    /// obtain basic layer functionality.
    /// </remarks>
    [Serializable]
    public abstract class Layer : ILayer, ICloneable
    {
        private static readonly PropertyDescriptorCollection _layerTypeProperties;

        static Layer()
        {
            _layerTypeProperties = TypeDescriptor.GetProperties(typeof(Layer));
        }

        #region PropertyDescriptors

        protected PropertyDescriptorCollection LayerTypeProperties
        {
            get { return _layerTypeProperties; }
        }

        // This pattern reminds me of DependencyProperties in WPF...

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="AsyncQuery"/> property.
        /// </summary>
        public static PropertyDescriptor AsyncQueryProperty
        {
            get { return _layerTypeProperties.Find("AsyncQuery", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor CoordinateTransformationProperty
        {
            get { return _layerTypeProperties.Find("CoordinateTransformation", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor EnabledProperty
        {
            get { return _layerTypeProperties.Find("Enabled", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Extents"/> property.
        /// </summary>
        public static PropertyDescriptor ExtentsProperty
        {
            get { return _layerTypeProperties.Find("Extents", false); }
        }

        ///// <summary>
        ///// Gets a PropertyDescriptor for Layer's <see cref="ShouldHandleFeaturesNotFoundEvent"/> 
        ///// property.
        ///// </summary>
        //public static PropertyDescriptor ShouldHandleFeaturesNotFoundEventProperty
        //{
        //    get { return _layerTypeProperties.Find("ShouldHandleFeaturesNotFoundEvent", false); }
        //}

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="LayerName"/> property.
        /// </summary>
        public static PropertyDescriptor LayerNameProperty
        {
            get { return _layerTypeProperties.Find("LayerName", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Style"/> property.
        /// </summary>
        public static PropertyDescriptor StyleProperty
        {
            get { return _layerTypeProperties.Find("Style", false); }
        }

        #endregion

        #region Instance fields
        private ICoordinateTransformation _coordinateTransform;
        private String _layerName;
        private IStyle _style;
        private Boolean _disposed;
        private readonly IAsyncProvider _dataSource;
        private Boolean _asyncQuery;
        //private Boolean _handleFeaturesNotFoundEvent = true;
        private IGeometry _loadedRegion;
        private PropertyDescriptorCollection _customProperties;
        private IGeometryFactory _geoFactory;
        private IQueryCache _cache;
        private IAsyncResult _loadAsyncResult;
        private readonly Object _loadCompletionSync = new Object();
        #endregion

        #region Object Creation / Disposal

        /// <summary>
        /// Creates a new Layer instance with the given data source.
        /// </summary>
        /// <param name="dataSource">
        /// The <see cref="IProvider"/> which provides the data 
        /// for the layer.
        /// </param>
        protected Layer(IProvider dataSource) :
            this(String.Empty, null, dataSource) { }

        /// <summary>
        /// Creates a new Layer instance identified by the given name and
        /// with the given data source.
        /// </summary>
        /// <param name="layerName">
        /// The name to uniquely identify the layer by.
        /// </param>
        /// <param name="dataSource">
        /// The <see cref="IProvider"/> which provides the data 
        /// for the layer.
        /// </param>
        protected Layer(String layerName, IProvider dataSource) :
            this(layerName, null, dataSource) { }

        /// <summary>
        /// Creates a new Layer instance identified by the given name, with
        /// symbology described by <paramref name="style"/> and
        /// with the given data source.
        /// </summary>
        /// <param name="layerName">
        /// The name to uniquely identify the layer by.
        /// </param>
        /// <param name="style">
        /// The symbology used to style the layer.
        /// </param>
        /// <param name="dataSource">
        /// The <see cref="IProvider"/> which provides the data 
        /// for the layer.
        /// </param>
        protected Layer(String layerName, IStyle style, IProvider dataSource)
        {
            if (layerName == null) throw new ArgumentNullException("layerName");
            if (dataSource == null) throw new ArgumentNullException("dataSource");

            _layerName = layerName;

            IAsyncProvider asyncProvider = dataSource as IAsyncProvider;
            _dataSource = asyncProvider ?? CreateAsyncProvider(dataSource);

            _style = style;
            // TODO: inject the cache type or instance...
            _cache = new NullQueryCache();
        }

        #region Dispose Pattern
        /// <summary>
        /// Releases resources if <see cref="Dispose"/> isn't called.
        /// </summary>
        ~Layer()
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
        /// Releases all resources, and removes from finalization 
        /// queue if <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">
        /// True if being called deterministically, false if being called from finalizer.
        /// </param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (_dataSource != null)
                {
                    _dataSource.Dispose();
                }
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

        #region ToString

        /// <summary>
        /// Returns the name of the layer.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return LayerName;
        }

        #endregion

        public PropertyDescriptorCollection Properties
        {
            get
            {
                return _customProperties ?? _layerTypeProperties;
            }
        }

        public Int32 AddProperty(PropertyDescriptor property)
        {
            initCustomPropsIfNeeded();
            return _customProperties.Add(property);
        }

        #region ILayer Members
        /// <summary>
        /// Gets or sets a value indicating that data is obtained asynchronously.
        /// </summary>
        public Boolean AsyncQuery
        {
            get { return _asyncQuery; }
            set
            {
                if (_asyncQuery == value)
                {
                    return;
                }

                _asyncQuery = value;
                OnAsyncQueryChanged();
            }
        }

        /// <summary>
        /// Gets the coordinate system of the layer.
        /// </summary>
        public ICoordinateSystem CoordinateSystem
        {
            get { return DataSource.SpatialReference; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICoordinateTransformation"/> 
        /// applied to this layer.
        /// </summary>
        public virtual ICoordinateTransformation CoordinateTransformation
        {
            get { return _coordinateTransform; }
            set
            {
                if (_coordinateTransform == value)
                {
                    return;
                }

                _coordinateTransform = value;
                OnCoordinateTransformationChanged();
            }
        }

        public event EventHandler<LayerDataLoadedEventArgs> DataLoaded;

        /// <summary>
        /// Gets the data source used to create this layer.
        /// </summary>
        public IProvider DataSource
        {
            get { return _dataSource; }
        }

        /// <summary>
        /// Gets or sets a value which indicates if the layer 
        /// is enabled (visible or able to participate in queries) or not.
        /// </summary>
        /// <remarks>
        /// This property is a convenience property which exposes 
        /// the value of <see cref="SharpMap.Styles.Style.Enabled"/>. 
        /// If setting this property and the Style property 
        /// value is null, a new <see cref="Style"/> 
        /// object is created and assigned to the Style property, 
        /// and then the Style.Enabled property is set.
        /// </remarks>
        public virtual Boolean Enabled
        {
            get { return Style.Enabled; }
            set
            {
                if (Style == null)
                {
                    Style = CreateStyle();
                }

                if (Enabled == value)
                {
                    return;
                }

                Style.Enabled = value;
                OnEnabledChanged();
            }
        }

        /// <summary>
        /// Gets the full extent of the data available to the layer.
        /// </summary>
        /// <returns>
        /// A <see cref="IExtents"/> defining the extent of 
        /// all data available to the layer.
        /// </returns>
        public IExtents Extents
        {
            get
            {
                IExtents fullExtents = DataSource.GetExtents();

                if (CoordinateTransformation != null)
                {
                    throw new NotImplementedException();

                    // TODO: This needs to be supported in GeoAPI
                    //return GeometryTransform.TransformBox(
                    //    fullExtents, CoordinateTransformation.MathTransform);
                }
                else
                {
                    return fullExtents;
                }
            }
        }

        public IGeometryFactory GeometryFactory
        {
            get { return _geoFactory; }
            set { _geoFactory = value; }
        }

        public Object GetProperty(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return property.GetValue(this);
        }

        public Boolean IsLoadingData
        {
            get { return _loadAsyncResult != null; }
        }

        public Boolean IsVisibleWhen(Predicate<ILayer> condition)
        {
            return condition(this);
        }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public virtual String LayerName
        {
            get { return _layerName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("LayerName must not be null or empty.");
                }

                if (_layerName == value)
                {
                    return;
                }

                _layerName = value;
                OnLayerNameChanged();
            }
        }

        public IGeometry LoadedRegion
        {
            get
            {
                if (_loadedRegion == null)
                {
                    computeLoadedRegion();
                }

                return _loadedRegion;
            }
            protected set { _loadedRegion = value; }
        }

        public void LoadIntersectingLayerData(IExtents region)
        {
            if (region == null) throw new ArgumentNullException("region");

            LoadIntersectingLayerData(region.ToGeometry());
        }

        public void LoadIntersectingLayerData(IGeometry region)
        {
            if (region == null) throw new ArgumentNullException("region");

            SpatialBinaryExpression query = new SpatialBinaryExpression(new SpatialExpression(region),
                                                                        SpatialOperation.Intersects,
                                                                        new LayerExpression(this));
            LoadLayerData(query);
        }

        public void LoadLayerData(SpatialBinaryExpression query)
        {
            if (_asyncQuery)
            {
                LoadLayerDataAsync(query);
            }
            else
            {
                _loadAsyncResult = _dataSource.BeginExecuteQuery(query, completeLoadLayerData);

                lock (_loadCompletionSync)
                {

                    Object results = _dataSource.EndExecuteQuery(_loadAsyncResult);
                    endLoadInternal(query, results);
                }
            }
        }

        public void LoadLayerDataAsync(SpatialBinaryExpression query)
        {
            AsyncCallback callback = completeLoadLayerData;

            _loadAsyncResult = _dataSource.BeginExecuteQuery(query, callback);
        }

        public abstract IEnumerable Select(Expression query);

        public void SetProperty(PropertyDescriptor property, Object value)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            property.SetValue(this, value);
        }

        /// <summary>
        /// Gets the spatial reference ID of the layer data source, if one is set.
        /// </summary>
        public virtual Int32? Srid
        {
            get
            {
                if (DataSource == null)
                {
                    throw new InvalidOperationException("DataSource property is null on layer '" +
                                                        LayerName + "'");
                }

                return DataSource.Srid;
            }
        }

        /// <summary>
        /// Gets or sets the style for the layer.
        /// </summary>
        public virtual IStyle Style
        {
            get { return _style; }
            set
            {
                if (_style == value)
                {
                    return;
                }

                _style = value;
                OnStyleChanged();
            }
        }

        #endregion

        #region Protected members

        protected abstract IAsyncProvider CreateAsyncProvider(IProvider dataSource);
        //{ return new Async<Feature/Raster>ProviderAdapter(dataSource); }

        /// <summary>
        /// Processes data from the <see cref="DataSource"/> which satisfies
        /// the <see cref="Expression"/> sent to <see cref="LoadLayerData"/>
        /// or <see cref="LoadLayerDataAsync"/>.
        /// </summary>
        /// <param name="results">
        /// The results from loading the layer data from the DataSoruce.
        /// </param>
        protected abstract void ProcessLoadResults(Object results);

        protected void AddLoadedResults(Expression expression, Object result)
        {
            QueryCache.AddExpressionResult(expression, result);
        }

        protected IQueryCache QueryCache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        protected virtual IStyle CreateStyle()
        {
            return new Style();
        }

        protected virtual void OnAsyncQueryChanged()
        {
            OnPropertyChanged(AsyncQueryProperty.Name);
        }

        protected virtual void OnCoordinateTransformationChanged()
        {
            OnPropertyChanged(CoordinateTransformationProperty.Name);
        }

        protected virtual void OnEnabledChanged()
        {
            OnPropertyChanged(EnabledProperty.Name);
        }

        protected virtual void OnLayerDataLoaded(Expression expression, Object result)
        {
            EventHandler<LayerDataLoadedEventArgs> e = DataLoaded;

            if (e != null)
            {
                e(this, new LayerDataLoadedEventArgs(expression, result));
            }
        }

        //protected virtual void OnShouldHandleDataCacheMissEventChanged()
        //{
        //    OnPropertyChanged(ShouldHandleFeaturesNotFoundEventProperty.Name);
        //}

        protected virtual void OnLayerNameChanged()
        {
            OnPropertyChanged(LayerNameProperty.Name);
        }

        protected virtual void OnStyleChanged()
        {
            OnPropertyChanged(StyleProperty.Name);
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the layer
        /// </summary>
        /// <returns>cloned object</returns>
        public abstract Object Clone();

        #endregion

        #region OnPropertyChanged

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property changed.</param>
        protected virtual void OnPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;

            if (e != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                e(this, args);
            }
        }

        #endregion

        #region Private helper methods

        private void initCustomPropsIfNeeded()
        {
            if (_customProperties != null)
            {
                return;
            }

            PropertyDescriptor[] propArray = new PropertyDescriptor[_layerTypeProperties.Count];
            _layerTypeProperties.CopyTo(propArray, 0);
            _customProperties = new PropertyDescriptorCollection(propArray, false);
        }

        private void computeLoadedRegion()
        {
            foreach (IGeometry geometry in _cache)
            {
                if (geometry == null)
                {
                    continue;
                }

                // TODO: this is probably too costly... need to perform a 
                // more intellegent merge, such as a binary merge or something
                _loadedRegion = _loadedRegion == null ? geometry : _loadedRegion.Union(geometry);
            }
        }

        private void completeLoadLayerData(IAsyncResult asyncResult)
        {
            // The lock is already held, so the calling thread will handle
            // the load synchronously
            if(!Monitor.TryEnter(_loadCompletionSync))
            {
                return;    
            }

            if (asyncResult.IsCompleted)
            {
                Monitor.Exit(_loadCompletionSync);
                return;
            }

            Object result = _dataSource.EndExecuteQuery(asyncResult);
            endLoadInternal(asyncResult.AsyncState as Expression, result);
            Monitor.Exit(_loadCompletionSync);
        }

        private void endLoadInternal(Expression expression, Object result)
        {
            ProcessLoadResults(result);

            if (expression != null)
            {
                AddLoadedResults(expression, result);
            }

            _loadAsyncResult = null;
            _loadedRegion = null;
            OnLayerDataLoaded(expression, result);
        }

        #endregion

        //private IGeometry mergeRegions(IGeometry a, IGeometry b)
        //{
        //    Boolean anyGeometryCollection = a.GeometryType == OgcGeometryType.GeometryCollection ||
        //                                    b.GeometryType == OgcGeometryType.GeometryCollection;
        //    return anyGeometryCollection
        //               ? GeometryFactory.CreateGeometryCollection(a, b)
        //               : a.Union(b);
        //}

        ///// <summary>
        ///// Gets or sets a value which causes the layer to handle
        ///// an event from a data store indicating that the data is not cached
        ///// and must be read from <see cref="DataSource"/>.
        ///// </summary>
        //public Boolean ShouldHandleFeaturesNotFoundEvent
        //{
        //    get { return _handleFeaturesNotFoundEvent; }
        //    set
        //    {
        //        if (_handleFeaturesNotFoundEvent == value)
        //        {
        //            return;
        //        }

        //        _handleFeaturesNotFoundEvent = value;

        //        OnShouldHandleDataCacheMissEventChanged();
        //    }
        //}
    }
}
