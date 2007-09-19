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
using System.ComponentModel;
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;
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
            _properties = TypeDescriptor.GetProperties(typeof (Layer));
        }

        #region PropertyDescriptors

        // This pattern reminds me of DependencyProperties in WPF...

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="Enabled"/> property.
        /// </summary>
        public static PropertyDescriptor EnabledProperty
        {
            get { return _properties.Find("Enabled", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="LayerName"/> property.
        /// </summary>
        public static PropertyDescriptor LayerNameProperty
        {
            get { return _properties.Find("LayerName", false); }
        }

        /// <summary>
        /// Gets a PropertyDescriptor for Layer's <see cref="VisibleRegion"/> property.
        /// </summary>
        public static PropertyDescriptor VisibleRegionProperty
        {
            get { return _properties.Find("VisibleRegion", false); }
        } 
        #endregion

        #region Instance fields

        private ICoordinateTransformation _coordinateTransform;
        private string _layerName;
        private IStyle _style;
        private bool _disposed;
        private BoundingBox _visibleRegion;
        private readonly ILayerProvider _dataSource;
        private bool _asyncQuery = false;
        private MultiPolygon _loadedRegion;
        private readonly BackgroundWorker _dataQueryWorker = new BackgroundWorker();

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
        protected Layer(string layerName, ILayerProvider dataSource) :
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
        protected Layer(string layerName, IStyle style, ILayerProvider dataSource)
        {
            LayerName = layerName;
            _dataSource = dataSource;
            Style = style;

            _dataQueryWorker.RunWorkerCompleted += _dataQueryWorker_RunWorkerCompleted;
            _dataQueryWorker.DoWork += _dataQueryWorker_DoWork;
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
        protected virtual void Dispose(bool disposing)
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
        public bool IsDisposed
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
        public override string ToString()
        {
            return LayerName;
        }

        #endregion

        #region ILayer Members

        #region Events
        ///// <summary>
        ///// Event raised when layer data has been completely
        ///// loaded if <see cref="AsyncQuery"/> is true.
        ///// </summary>
        //public event EventHandler LayerDataAvailable; 
        #endregion
       
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating that data is obtained asynchronously.
        /// </summary>
        public bool AsyncQuery
        {
            get { return _asyncQuery; }
            set
            {
                _asyncQuery = value;
                OnPropertyChanged("AsyncQuery");
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
                _coordinateTransform = value;
                OnPropertyChanged("CoordinateTransformation");
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
        public bool Enabled
        {
            get { return Style.Enabled; }
            set
            {
                if (Style == null)
                {
                    Style = new Style();
                }

                Style.Enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        /// <summary>
        /// Gets the full extent of the data available to the layer.
        /// </summary>
        /// <returns>
        /// A <see cref="BoundingBox"/> defining the extent of 
        /// all data available to the layer.
        /// </returns>
        public BoundingBox Envelope
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
        /// Gets or sets the name of the layer.
        /// </summary>
        public string LayerName
        {
            get { return _layerName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("LayerName must not be null or empty.");
                }

                _layerName = value;
                OnPropertyChanged("LayerName");
            }
        }

        /// <summary>
        /// The spatial reference ID of the layer data source, if one is set.
        /// </summary>
        public virtual int? Srid
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
        /// The style for the layer.
        /// </summary>
        public IStyle Style
        {
            get { return _style; }
            set
            {
                _style = value;
                OnPropertyChanged("Style");
            }
        }

        /// <summary>
        /// Gets or sets the visible region for this layer.
        /// </summary>
        public BoundingBox VisibleRegion
        {
            get { return _visibleRegion; }
            set
            {
                if (value == VisibleRegion)
                {
                    return;
                }

                // TODO: reevaluate how VisibleRegion's BoundingBox.Within interacts 
                // with LoadedRegion once NTS is integrated
                if (!value.Within(LoadedRegion))
                {
                    // Since the visible region changed, and we don't have the data
                    // which covers this new region, we have to query for it.
                    //
                    // We can do it asynchronously, with a BackgroundWorker instance,
                    // or synchronously
                    if (AsyncQuery)
                    {
                        _dataQueryWorker.RunWorkerAsync(value);
                    }
                    else
                    {
                        LoadLayerDataForRegion(value);
                        SetVisibleRegionInternal(value);
                    }
                }
            }
        }

        #endregion
        protected virtual MultiPolygon LoadedRegion
        {
            get { return _loadedRegion; }
        }

        protected virtual void LoadLayerDataForRegion(BoundingBox region)
        {
            AddLoadedRegion(region.ToGeometry() as Polygon);
        }

        protected virtual void LoadLayerDataForRegion(Polygon region)
        {
            AddLoadedRegion(region);
        }

        protected void AddLoadedRegion(BoundingBox region)
        {
            AddLoadedRegion(region.ToGeometry() as Polygon);
        }

        protected void AddLoadedRegion(Polygon region)
        {
            if (region == null)
            {
                return;
            }

            if (_loadedRegion == null)
            {
                _loadedRegion = new MultiPolygon(32);
                _loadedRegion.Polygons.Add(region);
            }
            else
            {
                _loadedRegion.Polygons.Add(region);
                _loadedRegion = simplifyRegion(_loadedRegion);
            }
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
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler e = PropertyChanged;

            if (e != null)
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                e(this, args);
            }
        }

        #endregion

        #region Protected methods

        protected void SetVisibleRegionInternal(BoundingBox value)
        {
            _visibleRegion = value;
            OnVisibleRegionChanged();
            OnPropertyChanged("VisibleRegion");
        }

        protected virtual void OnVisibleRegionChanged() { }

        //protected virtual void OnLayerDataAvailable()
        //{
        //    EventHandler e = LayerDataAvailable;

        //    if (e != null)
        //    {
        //        e(this, EventArgs.Empty);
        //    }
        //}
        #endregion

        private void _dataQueryWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetVisibleRegionInternal((BoundingBox) e.Result);
        }

        private void _dataQueryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is BoundingBox)
            {
                BoundingBox bounds = (BoundingBox)e.Argument;
                LoadLayerDataForRegion(bounds);
            }
            else if (e.Argument is Polygon)
            {
                Polygon geometry = e.Argument as Polygon;
                LoadLayerDataForRegion(geometry);
            }

            e.Result = e.Argument;
        }

        private MultiPolygon simplifyRegion(MultiPolygon region)
        {
            // TODO: perform simplification...
            return region;
        }
    }
}