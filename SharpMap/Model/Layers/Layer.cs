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
using System.ComponentModel;
using System.Text;

using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Rendering;
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
		private ICoordinateSystem _coordinateSystem;
        private ICoordinateTransformation _coordinateTransform;
        private string _layerName;
        private int _srid = -1;
        private IStyle _style;
		private bool _disposed;
        private BoundingBox _visibleRegion;
        private IProvider _dataSource;

        #region Object Creation / Disposal
        protected Layer(IProvider dataSource)
        {
            _dataSource = dataSource;
        }

        #region Dispose Pattern
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
			return this.LayerName;
		}
		#endregion

        #region ILayer Members

        /// <summary>
        /// Gets the coordinate system of the layer.
        /// </summary>
        public ICoordinateSystem CoordinateSystem
        {
            get { return _coordinateSystem; }
            private set { _coordinateSystem = value; }
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
        /// the value of <see cref="Style.Enabled"/>. 
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
		/// Gets the full extent of the layer.
		/// </summary>
		/// <returns>
        /// <see cref="BoundingBox"/> corresponding to the 
        /// extent of the features in the layer.
        /// </returns>
        public abstract BoundingBox Envelope { get; }

        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        public string LayerName
        {
            get { return _layerName; }
            set 
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentException("LayerName must not be null or empty.");
                _layerName = value;
                OnPropertyChanged("LayerName");
            }
        }

        /// <summary>
        /// The spatial reference ID.
        /// </summary>
        public virtual int Srid
        {
            get { return _srid; }
            set 
            { 
                _srid = value;
                OnPropertyChanged("Srid");
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
                bool cancel = false;
                OnVisibleRegionChanging(value, ref cancel);
                _visibleRegion = value;
                OnVisibleRegionChanged();
                OnPropertyChanged("VisibleRegion");
            }
        }

        protected virtual void OnVisibleRegionChanged()
        {
        }

        protected abstract void OnVisibleRegionChanging(BoundingBox value, ref bool cancel);
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
    }
}
