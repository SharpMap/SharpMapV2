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
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Data;
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
#warning LayerGroup should inherit from Layer
    public class LayerGroup : ILayer, ICloneable
    {
        private List<ILayer> _layers = new List<ILayer>();
        private Boolean _disposed;
        private ILayer _master;

        #region Object Creation / Disposal

        /// <summary>
        /// Initializes a new group layer.
        /// </summary>
        public LayerGroup()
        {
        }

        /// <summary>
        /// Initializes a new group layer.
        /// </summary>
        /// <param name="layers">The list of member layers</param>
        /// <param name="master">A reference to the member of 'layers' which is to be the master layer ('layers' must contain 'master')</param>
        public LayerGroup(List<ILayer> layers, ILayer master)
        {
            _layers = layers;
            _master = master;
            if (layers != null && !layers.Contains(master))
            {
                throw new ArgumentException("The layers list must contain the master layer");
            }
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
                    ((IDisposable)layer).Dispose();
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
            get
            {
                return _layers;
            }
            set
            {
                _layers = new List<ILayer>(value);
                _master = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public ILayer this[String layerName]
        {
            get
            {
                return GetLayerByName(layerName);
            }
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
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.CoordinateSystem;
            }
        }

        public ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.CoordinateTransformation;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ILayerProvider DataSource
        {
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.DataSource;
            }
        }

        public Boolean Enabled
        {
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.Enabled;
            }
            set
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                if (MasterLayer.Enabled == value)
                {
                    return;
                }
                MasterLayer.Enabled = value;

                OnPropertyChanged("Enabled");
            }
        }

        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>
        /// An <see cref="IExtents"/> corresponding to the extent of the 
        /// data in the layer.
        /// </returns>
        public IExtents Extents
        {
            get
            {
                // Changed to null from BoundingBox.Empty
                IExtents bbox = null;

                if (Layers.Count == 0)
                {
                    return bbox;
                }

                _layers.ForEach(
                        delegate(ILayer layer)
                        {
                            if (bbox == null)
                            {
                                bbox = layer.Extents.Clone() as IExtents;
                            }
                            else
                            {
                                bbox.ExpandToInclude(layer.Extents);
                            }
                        }
                    );

                return bbox;
            }
        }

        public String LayerName
        {
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.LayerName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Int32? Srid
        {
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.Srid;
            }
        }

        public IStyle Style
        {
            get
            {
                if (MasterLayer == null)
                {
                    throw new NullReferenceException("MasterLayer is not set yet");
                }

                return MasterLayer.Style;
            }
            set
            {
                throw new NotImplementedException();
            }
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
        Object ICloneable.Clone()
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

        public ILayer MasterLayer
        {
            get
            {
                return _master;
            }
            set
            {
                if (Layers != null && !Layers.Contains(value))
                {
                    throw new ArgumentException("Master layer must be a member of the LayerGroup");
                }

                _master = value;
            }
        }

        /// <summary>
        /// Whether we should show anything other than the master
        /// </summary>
        public Boolean ShowChildren
        {
            get
            {
                // Yes, if we find even one enabled child we say the children are all visible
                foreach (ILayer layer in _layers)
                {
                    if (layer != MasterLayer)
                    {
                        if (layer.Enabled)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            set
            {
                Boolean valChanged = false;

                foreach (ILayer layer in _layers)
                {
                    if (layer != MasterLayer)
                    {
                        if (layer.Enabled != value)
                        {
                            layer.Enabled = value;
                            valChanged = true;
                        }
                    }
                }

                if (valChanged)
                {
                    OnPropertyChanged(Layer.ShowChildrenProperty.Name);
                }
            }
        }

        /// <summary>
        /// Whether we should show anything other than the master
        /// </summary>
        public Boolean AreFeaturesSelectable
        {
            get
            {
                return MasterLayer.AreFeaturesSelectable;
            }
            set
            {
                if (value != MasterLayer.AreFeaturesSelectable)
                {
                    MasterLayer.AreFeaturesSelectable = value;

                    // this double check is here because some layers may not have an impl for 
                    // that property setter, so the set may not have actually done anything
                    if (value == MasterLayer.AreFeaturesSelectable)
                    {
                        OnPropertyChanged(Layer.AreFeaturesSelectableProperty.Name);
                    }
                }
            }
        }

        public event EventHandler LayerDataAvailable;

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