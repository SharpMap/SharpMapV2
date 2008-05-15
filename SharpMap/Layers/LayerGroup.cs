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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using GeoAPI.CoordinateSystems.Transformations;
using SharpMap.Data;
using SharpMap.Expressions;
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
    public class LayerGroup : Layer, IList<ILayer>
    {
        private static readonly PropertyDescriptorCollection _layerGroupTypeProperties;

        static LayerGroup()
        {
            _layerGroupTypeProperties = TypeDescriptor.GetProperties(typeof(LayerGroup));
        }

        public static PropertyDescriptor ShowChildrenProperty
        {
            get { return _layerGroupTypeProperties.Find("ShowChildren", false); }
        }

        private readonly List<ILayer> _layers = new List<ILayer>();
        private readonly ILayer _master;

        #region Object Creation / Disposal

        /// <summary>
        /// Initializes a new group layer.
        /// </summary>
        public LayerGroup(IProvider dataSource) : base(dataSource) { }

        /// <summary>
        /// Initializes a new group layer.
        /// </summary>
        /// <param name="layers">An enumeration of member layers.</param>
        /// <param name="master">
        /// A <see cref="ILayer"/> which is the master layer for the group, from which the group
        /// gets the data source.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Parameter <paramref name="layers"/> contains layers and <paramref name="master"/>
        /// is not a member of the enumeration.
        /// </exception>
        public LayerGroup(IEnumerable<ILayer> layers, ILayer master) : base(master.DataSource)
        {
            _layers.AddRange(layers);
            _master = master;

            if (_layers.Count > 0 && !_layers.Contains(master))
            {
                throw new ArgumentException("The layers list must contain the master layer");
            }
        }

        #region Dispose Pattern

        ~LayerGroup()
        {
            Dispose(false);
        }

        protected override void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            foreach (ILayer layer in this)
            {
                if (layer != null)
                {
                    layer.Dispose();
                }
            }

            _layers.Clear();
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns a layer by its name
        /// </summary>
        /// <param name="name">Name of layer</param>
        /// <returns>Layer</returns>
        public ILayer GetLayerByName(String name)
        {
            Predicate<ILayer> find =
                delegate(ILayer layer)
                {
                    return String.Compare(layer.LayerName,
                                          name,
                                          StringComparison.CurrentCultureIgnoreCase) == 0;
                };

            return _layers.Find(find);
        }

        public ILayer MasterLayer
        {
            get
            {
                return _master;
            }
            //set
            //{
            //    if (_layers.Count > 0 && !Contains(value))
            //    {
            //        throw new ArgumentException("Master layer must be a member of the LayerGroup");
            //    }

            //    _master = value;
            //}
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
                    if (layer != MasterLayer && layer.Enabled)
                    {
                        return true;
                    }
                }

                return false;
            }
            set
            {
                Boolean valChanged = false;

                foreach (ILayer layer in _layers)
                {
                    if (layer != MasterLayer && layer.Enabled != value)
                    {
                        layer.Enabled = value;
                        valChanged = true;
                    }
                }

                if (valChanged)
                {
                    OnPropertyChanged(ShowChildrenProperty.Name);
                }
            }
        }

        // Replace the following by implementing a generic property
        // set mechanism which delegates to the Master layer
        // then traps the resulting property change notification
        // and sets the same property on the children...

        ///// <summary>
        ///// Whether we should show anything other than the master
        ///// </summary>
        //public Boolean AreFeaturesSelectable
        //{
        //    get
        //    {
        //        return MasterLayer.AreFeaturesSelectable;
        //    }
        //    set
        //    {
        //        if (value != MasterLayer.AreFeaturesSelectable)
        //        {
        //            MasterLayer.AreFeaturesSelectable = value;

        //            // this double check is here because some layers may not have an impl for 
        //            // that property setter, so the set may not have actually done anything
        //            if (value == MasterLayer.AreFeaturesSelectable)
        //            {
        //                OnPropertyChanged(Layer.AreFeaturesSelectableProperty.Name);
        //            }
        //        }
        //    }
        //}

        public ILayer this[String layerName]
        {
            get
            {
                return GetLayerByName(layerName);
            }
        }

        public ILayer this[Int32 index]
        {
            get { return _layers[index]; }
        }

        #region IList<ILayer> Members

        public void Add(ILayer layer)
        {
            _layers.Add(layer);
        }

        public void Clear()
        {
            _layers.Clear();
        }

        public Boolean Contains(ILayer item)
        {
            return _layers.Contains(item);
        }

        public void CopyTo(ILayer[] array, Int32 arrayIndex)
        {
            _layers.CopyTo(array, arrayIndex);
        }

        public Boolean Remove(ILayer item)
        {
            return _layers.Remove(item);
        }

        public Int32 Count
        {
            get { return _layers.Count; }
        }

        public Boolean IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(ILayer item)
        {
            throw new NotImplementedException();
        }

        public void Insert(Int32 index, ILayer item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(Int32 index)
        {
            throw new NotImplementedException();
        }

        ILayer IList<ILayer>.this[Int32 index]
        {
            get { return this[index]; }
            set { throw new NotSupportedException(); }
        }

        public IEnumerator<ILayer> GetEnumerator()
        {
            foreach (ILayer layer in _layers)
            {
                yield return layer;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Layer overrides

        public override ICoordinateTransformation CoordinateTransformation
        {
            get
            {
                return base.CoordinateTransformation ?? MasterLayer.CoordinateTransformation;
            }
            set
            {
                base.CoordinateTransformation = value;
            }
        }

        public override Boolean Enabled
        {
            get
            {
                // Layer group state is checked in Style property
                return base.Enabled;
            }
            set
            {
                // Layer group state is checked in Style property

                if (base.Enabled == value)
                {
                    return;
                }

                MasterLayer.Enabled = value;

                OnPropertyChanged(EnabledProperty.Name);
            }
        }

        public override String LayerName
        {
            get
            {
                checkState();
                return base.LayerName ?? MasterLayer.LayerName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override IStyle Style
        {
            get
            {
                checkState();
                return MasterLayer.Style;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override Object Clone()
        {
            return new LayerGroup(_layers, MasterLayer);
        }

        protected override void ProcessLoadResults(Object results)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable Select(Expression query)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private helper methods

        private void checkState()
        {
            if (MasterLayer == null)
            {
                throw new InvalidOperationException("MasterLayer is not set yet");
            }
        } 
        #endregion
    }
}