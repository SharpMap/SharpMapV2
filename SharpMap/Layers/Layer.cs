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
using System.ComponentModel;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using SharpMap.Data;
using GeoAPI.Geometries;
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
        private static readonly PropertyDescriptorCollection _properties;

        static Layer()
        {
            _properties = TypeDescriptor.GetProperties(typeof(Layer));
        }

        #region PropertyDescriptors

        // This pattern reminds me of DependencyProperties in WPF...

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="AsyncQuery"/> property.
        /// </summary>
        public static PropertyDescriptor AsyncQueryProperty
        {
            get { return _properties.Find("AsyncQuery", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor CoordinateTransformationProperty
        {
            get { return _properties.Find("CoordinateTransformation", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor EnabledProperty
        {
            get { return _properties.Find("Enabled", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Extents"/> property.
        /// </summary>
        public static PropertyDescriptor ExtentsProperty
        {
            get { return _properties.Find("Extents", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="ShouldHandleDataCacheMissEvent"/> 
        /// property.
        /// </summary>
        public static PropertyDescriptor ShouldHandleDataCacheMissEventProperty
        {
            get { return _properties.Find("ShouldHandleDataCacheMissEvent", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="LayerName"/> property.
        /// </summary>
        public static PropertyDescriptor LayerNameProperty
        {
            get { return _properties.Find("LayerName", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Style"/> property.
        /// </summary>
        public static PropertyDescriptor StyleProperty
        {
            get { return _properties.Find("Style", false); }
        }
        #endregion

        #region Instance fields
        private ICoordinateTransformation _coordinateTransform;
        private String _layerName;
        private IStyle _style;
        private Boolean _disposed;
        private readonly ILayerProvider _dataSource;
        private Boolean _asyncQuery = false;
        private Boolean _handleFeaturesNotFoundEvent = true;
        #endregion

        #region Object Creation / Disposal

        /// <summary>
        /// Creates a new Layer instance with the given data source.
        /// </summary>
        /// <param name="dataSource">
        /// The <see cref="ILayerProvider"/> which provides the data 
        /// for the layer.
        /// </param>
        protected Layer(ILayerProvider dataSource) :
            this(String.Empty, null, dataSource)
        {
        }

        /// <summary>
        /// Creates a new Layer instance identified by the given name and
        /// with the given data source.
        /// </summary>
        /// <param name="layerName">
        /// The name to uniquely identify the layer by.
        /// </param>
        /// <param name="dataSource">
        /// The <see cref="ILayerProvider"/> which provides the data 
        /// for the layer.
        /// </param>
        protected Layer(String layerName, ILayerProvider dataSource) :
            this(layerName, null, dataSource)
        {
        }


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
        /// The <see cref="ILayerProvider"/> which provides the data 
        /// for the layer.
        /// </param>
        protected Layer(String layerName, IStyle style, ILayerProvider dataSource)
        {
            LayerName = layerName;
            _dataSource = dataSource;
            Style = style;
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

#if !EXPERIMENTAL
                throw new NotImplementedException("AsyncQuery not implemented in this release.");
#else
                _asyncQuery = value;
                OnAsyncQueryChanged();
#endif
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

        /// <summary>
        /// Gets the data source used to create this layer.
        /// </summary>
        public ILayerProvider DataSource
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
        public Boolean Enabled
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
        /// A <see cref="BoundingBox"/> defining the extent of 
        /// all data available to the layer.
        /// </returns>
        public BoundingBox Extents
        {
            get
            {
                BoundingBox fullExtents = DataSource.GetExtents();

                if (CoordinateTransformation != null)
                {
                    return GeometryTransform.TransformBox(
                        fullExtents, CoordinateTransformation.MathTransform);
                }
                else
                {
                    return fullExtents;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value which causes the layer to handle
        /// an event from a data store indicating that the data is not cached
        /// and must be read from <see cref="DataSource"/>.
        /// </summary>
        public Boolean ShouldHandleFeaturesNotFoundEvent
        {
            get { return _handleFeaturesNotFoundEvent; }
            set
            {
                if (_handleFeaturesNotFoundEvent == value)
                {
                    return;
                }

                _handleFeaturesNotFoundEvent = value;

                OnShouldHandleDataCacheMissEventChanged();
            }
        }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public String LayerName
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

        /// <summary>
        /// Gets the spatial reference ID of the layer data source, if one is set.
        /// </summary>
        public virtual Int32? Srid
        {
            get
            {
                if (DataSource == null)
                {
                    throw new InvalidOperationException(
                        "DataSource property is null on layer '" + LayerName + "'");
                }

                return DataSource.Srid;
            }
        }

        /// <summary>
        /// Gets or sets the style for the layer.
        /// </summary>
        public IStyle Style
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

        public Boolean IsVisibleWhen(Predicate<ILayer> condition)
        {
            return condition(this);
        }
        #endregion

        #region Protected members

        protected void AddLoadedRegion(BoundingBox region)
        {
            AddLoadedRegion(region.ToGeometry());
        }

        protected void AddLoadedRegion(Geometry region)
        {
            if (region == null)
            {
                return;
            }

            if (LoadedRegion == null)
            {
                LoadedRegion = region;
            }
            else
            {
                LoadedRegion = LoadedRegion.Union(region);
                LoadedRegion = FakeSpatialOperations.SimplifyRegion(LoadedRegion);
            }
        }

        protected virtual IStyle CreateStyle()
        {
            return new Style();
        }

        public abstract Geometry LoadedRegion { get; protected set; }

        public virtual void LoadIntersectingLayerData(BoundingBox region)
        {
            SpatialExpression query = new SpatialExpression(region.ToGeometry(), SpatialExpressionType.Intersects);
            LoadLayerData(query);
        }

        public virtual void LoadIntersectingLayerData(Geometry region)
        {
            SpatialExpression query = new SpatialExpression(region, SpatialExpressionType.Intersects);
            LoadLayerData(query);
        }

        public virtual void LoadLayerData(SpatialExpression query)
        {
            AddLoadedRegion(query.QueryRegion);
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

        protected virtual void OnShouldHandleDataCacheMissEventChanged()
        {
            OnPropertyChanged(ShouldHandleDataCacheMissEventProperty.Name);
        }

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
        public abstract object Clone();

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
    }
}