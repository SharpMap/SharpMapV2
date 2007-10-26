// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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
using SharpMap.CoordinateSystems;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// Class for holding a group of layers.
    /// </summary>
    /// <remarks>
    /// A <see cref="LayerGroup"/> is useful for grouping a set of layers,
    /// for instance a set of image tiles, or a feature layer and a label layer, 
    /// and expose them as a single layer.
    /// </remarks>
    public class LayerGroup : ILayer, ICloneable
    {
        private List<ILayer> _layers = new List<ILayer>();
        private Boolean _disposed;

        #region Object Creation / Disposal

        /// <summary>
        /// Initializes a new group layer.
        /// </summary>
        /// <param name="layername">Name of the layer group.</param>
        public LayerGroup(String layername)
        {
            LayerName = layername;
        }

        #region Dispose Pattern

        ~LayerGroup()
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
            if (IsDisposed)
            {
                return;
            }

            foreach (Layer layer in Layers)
            {
                if (layer is IDisposable)
                {
                    ((IDisposable) layer).Dispose();
                }
            }

            Layers.Clear();
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

        /// <summary>
        /// Sublayers in the group
        /// </summary>
        public IList<ILayer> Layers
        {
            get { return _layers; }
            set { _layers = new List<ILayer>(value); }
        }

        /// <summary>
        /// Returns a layer by its name
        /// </summary>
        /// <param name="name">Name of layer</param>
        /// <returns>Layer</returns>
        public ILayer GetLayerByName(String name)
        {
            return
                _layers.Find(
                    delegate(ILayer layer) { return String.Compare(layer.LayerName, name, StringComparison.CurrentCultureIgnoreCase) == 0; });
        }

        #region ILayer Members

        public ICoordinateSystem CoordinateSystem
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public ILayerProvider DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public Boolean Enabled
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>
        /// Bounding box corresponding to the extent of the features in the layer.
        /// </returns>
        public BoundingBox Extents
        {
            get
            {
                BoundingBox bbox = BoundingBox.Empty;

                if (Layers.Count == 0)
                {
                    return bbox;
                }

                _layers.ForEach(delegate(ILayer layer) { bbox.ExpandToInclude(layer.Extents); });

                return bbox;
            }
        }

        public String LayerName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Int32? Srid
        {
            get { throw new NotImplementedException(); }
        }

        public IStyle Style
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        
        public Boolean IsVisibleWhen(Predicate<ILayer> condition)
        {
            return condition(this);
        }
        #endregion

        public ILayer Clone()
        {
            throw new NotImplementedException();
        }

        #region ICloneable Members

        /// <summary>
        /// Clones the layer.
        /// </summary>
        /// <returns>A deep-copy of the layer.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ILayer Members

        public Boolean AsyncQuery
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public event EventHandler LayerDataAvailable;

        #endregion
    }
}