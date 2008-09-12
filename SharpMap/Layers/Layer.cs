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
using System.Collections.Generic;
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
    public abstract class Layer : CustomTypeDescriptor, ILayer, ICloneable
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
        /// Gets a <see cref="PropertyDescriptor"/> for 
        /// <see cref="Layer"/>'s <see cref="AsyncQuery"/> property.
        /// </summary>
        public static PropertyDescriptor AsyncQueryProperty
        {
            get { return _layerTypeProperties.Find("AsyncQuery", false); }
        }

        /// <summary>
        /// Gets a  <see cref="PropertyDescriptor"/> for 
        /// <see cref="Layer"/>'s <see cref="CoordinateTransformation"/> property.
        /// </summary>
        public static PropertyDescriptor CoordinateTransformationProperty
        {
            get { return _layerTypeProperties.Find("CoordinateTransformation", false); }
        }

        /// <summary>
        /// Gets a  <see cref="PropertyDescriptor"/> for 
        /// <see cref="Layer"/>'s <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor EnabledProperty
        {
            get { return _layerTypeProperties.Find("Enabled", false); }
        }

        /// <summary>
        /// Gets a  <see cref="PropertyDescriptor"/> for 
        /// <see cref="Layer"/>'s <see cref="Extents"/> property.
        /// </summary>
        public static PropertyDescriptor ExtentsProperty
        {
            get { return _layerTypeProperties.Find("Extents", false); }
        }

        /// <summary>
        /// Gets a  <see cref="PropertyDescriptor"/> for 
        /// <see cref="Layer"/>'s <see cref="LayerName"/> property.
        /// </summary>
        public static PropertyDescriptor LayerNameProperty
        {
            get { return _layerTypeProperties.Find("LayerName", false); }
        }

        /// <summary>
        /// Gets a  <see cref="PropertyDescriptor"/> for 
        /// <see cref="Layer"/>'s <see cref="Style"/> property.
        /// </summary>
        public static PropertyDescriptor StyleProperty
        {
            get { return _layerTypeProperties.Find("Style", false); }
        }

        #endregion

        #region Instance fields

        private readonly ILayer _parent;
        private ICoordinateTransformation _coordinateTransform;
        private String _layerName;
        private IStyle _style;
        private Boolean _disposed;
        private readonly IAsyncProvider _dataSource;
        private Boolean _asyncQuery;
        //private Boolean _handleFeaturesNotFoundEvent = true;
        private IGeometry _loadedRegion;
        private PropertyDescriptorCollection _instanceProperties;
        private IGeometryFactory _geoFactory;
        private IQueryCache _cache;
        private IAsyncResult _loadAsyncResult;
        private readonly Object _loadCompletionSync = new Object();
        private Dictionary<PropertyDescriptor, Object> _propertyValues;
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
            : this(layerName, style, dataSource, null, null) { }

        protected Layer(String layerName,
                        IStyle style,
                        IProvider dataSource,
                        IGeometryFactory geometryFactory,
                        ILayer parent)
            : base(parent)
        {
            if (layerName == null) throw new ArgumentNullException("layerName");
            if (dataSource == null) throw new ArgumentNullException("dataSource");

            _layerName = layerName;
            _parent = parent;

            if (parent != null)
            {
                _asyncQuery = parent.AsyncQuery;
                _geoFactory = geometryFactory;
                _coordinateTransform = parent.CoordinateTransformation;
                dataSource = parent.DataSource as IAsyncProvider ??
                             CreateAsyncProvider(parent.DataSource);
                _loadedRegion = parent.LoadedRegion;
                style = parent.Style;
            }

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

        public void SetPropertyValue<TValue>(PropertyDescriptor property, TValue value)
        {
            checkSetValueType<TValue>(property);
            checkPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public void SetPropertyValue(PropertyDescriptor property, Object value)
        {
            checkPropertyParameter(property);
            SetPropertyValueInternal(property, value);
        }

        public virtual Boolean HasProperty(PropertyDescriptor property)
        {
            PropertyDescriptorCollection properties = _instanceProperties ?? _layerTypeProperties;
            return properties.Contains(property) || (_propertyValues != null &&
                                                     _propertyValues.ContainsKey(property));
        }

        public override Object GetPropertyOwner(PropertyDescriptor pd)
        {
            return base.GetPropertyOwner(pd) ?? (HasProperty(pd) ? this : null);
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            if (_instanceProperties != null)
            {
                return _instanceProperties;
            }

            PropertyDescriptorCollection parentProperties = base.GetProperties();

            return parentProperties != PropertyDescriptorCollection.Empty
                       ? parentProperties
                       : _layerTypeProperties;
        }

        public Int32 AddProperty(PropertyDescriptor property)
        {
            ensureInstanceProperties();
            return _instanceProperties.Add(property);
        }

        public Int32 AddProperty<TValue>(PropertyDescriptor property, TValue value)
        {
            ensureInstanceProperties();
            Int32 index = _instanceProperties.Add(property);
            SetPropertyValueInternal(property, value);
            return index;
        }

        public TValue GetPropertyValue<TValue>(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal<TValue>(property);
        }

        public Object GetPropertyValue(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            return GetPropertyValueInternal(property);
        }

        #region ILayer Members
        /// <summary>
        /// Gets or sets a value indicating that data is obtained asynchronously.
        /// </summary>
        public Boolean AsyncQuery
        {
            get
            {
                return _asyncQuery;
            }
            set
            {
                checkParent();

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
                checkParent();

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
            get
            {
                return Style.Enabled;
            }
            set
            {
                checkParent();

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
            set
            {
                checkParent();
                _geoFactory = value;
            }
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
            protected set
            {
                checkParent();
                _loadedRegion = value;
            }
        }

        public void LoadIntersectingLayerData(IExtents region)
        {
            if (region == null) throw new ArgumentNullException("region");

            SpatialBinaryExpression exp = new SpatialBinaryExpression(new ExtentsExpression(region),
                                                                      SpatialOperation.Intersects,
                                                                      new LayerExpression(this));

            QueryExpression query = GetQueryFromSpatialBinaryExpression(exp);

            LoadLayerData(query);
        }

        public void LoadIntersectingLayerData(IGeometry region)
        {
            if (region == null) throw new ArgumentNullException("region");

            SpatialBinaryExpression exp = new SpatialBinaryExpression(new GeometryExpression(region),
                                                                      SpatialOperation.Intersects,
                                                                      new LayerExpression(this));

            QueryExpression query = GetQueryFromSpatialBinaryExpression(exp);

            LoadLayerData(query);
        }

        public void LoadLayerData(QueryExpression query)
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

        public void LoadLayerDataAsync(QueryExpression query)
        {
            AsyncCallback callback = completeLoadLayerData;

            _loadAsyncResult = _dataSource.BeginExecuteQuery(query, callback);
        }

        public abstract IEnumerable Select(Expression query);

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
            get
            {
                if (_style == null)
                {
                    _style = CreateStyle();
                }

                return _style;
            }
            set
            {
                checkParent();

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
        // DESIGN_NOTE: ProcessLoadResults(IEnumerable results)
        protected abstract void ProcessLoadResults(Object results);

        // DESIGN_NOTE: AddLoadedResults(Expression expression, IEnumerable results)
        protected void AddLoadedResults(Expression expression, Object results)
        {
            // BUG: this will fail when we have a single result and it implements IEnumerable
            // To solve this, we should always wrap the result in an IEnumerable wrapper
            IEnumerable enumerable = results as IEnumerable;

            if (enumerable == null)
            {
                QueryCache.AddExpressionResult(expression, results);   
            }
            else
            {
                QueryCache.AddExpressionResults(expression, enumerable);   
            }
        }

        public IQueryCache QueryCache
        {
            get { return _cache; }
            set
            {
                checkParent();
                _cache = value;
            }
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
        /// Clones the layer.
        /// </summary>
        /// <returns>A memberwise-duplicated layer instance.</returns>
        public virtual Object Clone()
        {
            return MemberwiseClone();
        }

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

        #region Protected virtual members

        protected virtual TValue GetPropertyValueInternal<TValue>(PropertyDescriptor property)
        {
            checkGetValueType<TValue>(property);

            return (TValue)GetPropertyValueInternal(property);
        }

        protected virtual Object GetPropertyValueInternal(PropertyDescriptor property)
        {
            if (_layerTypeProperties.Contains(property))
            {
                String propertyName = property.Name;

                if (LayerNameProperty.Name.Equals(propertyName))
                {
                    return _layerName;
                }

                if (CoordinateTransformationProperty.Name.Equals(propertyName))
                {
                    return CoordinateTransformation;
                }

                if (AsyncQueryProperty.Name.Equals(propertyName))
                {
                    return AsyncQuery;
                }

                if (EnabledProperty.Name.Equals(propertyName))
                {
                    return Enabled;
                }

                if (StyleProperty.Name.Equals(propertyName))
                {
                    return Style;
                }
            }

            if (_instanceProperties.Contains(property))
            {
                Object value;

                return _propertyValues != null && _propertyValues.TryGetValue(property, out value)
                           ? value
                           : null;
            }

            throw new InvalidOperationException("Property doesn't exist on this layer: " +
                                                property.Name);
        }

        protected abstract QueryExpression GetQueryFromSpatialBinaryExpression(SpatialBinaryExpression exp);

        protected virtual void SetPropertyValueInternal<TValue>(PropertyDescriptor property, TValue value)
        {
            checkSetValueType<TValue>(property);

            SetPropertyValueInternal(property, (Object)value);
        }

        protected virtual void SetPropertyValueInternal(PropertyDescriptor property, Object value)
        {
            if (_layerTypeProperties.Contains(property))
            {
                String propertyName = property.Name;

                if (LayerNameProperty.Name.Equals(propertyName))
                {
                    _layerName = value as String;
                }
                else if (CoordinateTransformationProperty.Name.Equals(propertyName))
                {
                    CoordinateTransformation = value as ICoordinateTransformation;
                }
                else if (AsyncQueryProperty.Name.Equals(propertyName))
                {
                    AsyncQuery = (Boolean)value;
                }
                else if (EnabledProperty.Name.Equals(propertyName))
                {
                    Enabled = (Boolean)value;
                }
                else if (StyleProperty.Name.Equals(propertyName))
                {
                    Style = value as Style;
                }
            }
            else if (_instanceProperties.Contains(property))
            {
                if (_propertyValues == null)
                {
                    _propertyValues = new Dictionary<PropertyDescriptor, Object>();
                }

                _propertyValues[property] = value;
            }
            else
            {
                throw new InvalidOperationException("Property doesn't exist on this layer: " +
                                                    property.Name);
            }
        }

        #endregion

        #region Private helper methods

        private static void checkGetValueType<TValue>(PropertyDescriptor property)
        {
            if (!typeof(TValue).IsAssignableFrom(property.PropertyType))
            {
                throw new ArgumentException("The type of the property isn't " +
                                            "assignable to the value variable.");
            }
        }

        private static void checkSetValueType<TValue>(PropertyDescriptor property)
        {
            if (!property.PropertyType.IsAssignableFrom(typeof(TValue)))
            {
                throw new ArgumentException("The type of the value isn't " +
                                            "assignable to the property.");
            }
        }

        private void ensureInstanceProperties()
        {
            if (_instanceProperties != null)
            {
                return;
            }

            PropertyDescriptor[] propArray = new PropertyDescriptor[_layerTypeProperties.Count];
            _layerTypeProperties.CopyTo(propArray, 0);
            _instanceProperties = new PropertyDescriptorCollection(propArray, false);
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
            if (!Monitor.TryEnter(_loadCompletionSync))
            {
                return;
            }

            if (asyncResult.IsCompleted)
            {
                Monitor.Exit(_loadCompletionSync);
                return;
            }

            Object results = _dataSource.EndExecuteQuery(asyncResult);
            endLoadInternal(asyncResult.AsyncState as Expression, results);
            Monitor.Exit(_loadCompletionSync);
        }

        // DESIGN_NOTE: this should probably change to endLoadInternal(Expression expression, IEnumerable results)
        private void endLoadInternal(Expression expression, Object results)
        {
            ProcessLoadResults(results);

            if (expression != null)
            {
                AddLoadedResults(expression, results);
            }

            _loadAsyncResult = null;
            _loadedRegion = null;
            OnLayerDataLoaded(expression, results);
        }

        private void checkParent()
        {
            if (_parent != null)
            {
                throw new InvalidOperationException("A child layer cannot have properties " +
                                                    "set directly. Set properties on the " +
                                                    "parent layer: " + _parent.LayerName);
            }
        }

        private void checkPropertyParameter(PropertyDescriptor property)
        {
            if (property == null) { throw new ArgumentNullException("property"); }

            if (!HasProperty(property))
            {
                throw new InvalidOperationException("Property doesn't exist for layer " +
                                                    LayerName);
            }

            if (property.IsReadOnly)
            {
                throw new InvalidOperationException(String.Format("Property {0} is read only.",
                                                                  property.Name));
            }
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
