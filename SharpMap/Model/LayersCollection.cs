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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using SharpMap.Layers;

namespace SharpMap
{
    /// <summary>
    /// Implementation of <see cref="IList{T}"/> which provides notification when layers are added
    /// or removed.
    /// </summary>
    public class LayersCollection : IModelCollection<ILayer>
    {
        private List<ILayer> _layers = new List<ILayer>();

        public LayersCollection() { }

        public LayersCollection(IEnumerable<ILayer> layers)
        {
            _layers.AddRange(layers);
        }

        public ILayer Find(Predicate<ILayer> predicate)
        {
            return _layers.Find(predicate);
		}

		public IEnumerable<ILayer> FindAll(Predicate<ILayer> predicate)
		{
			return _layers.FindAll(predicate);
		}

        public void AddLayers(IEnumerable<ILayer> layers)
        {
            if (layers == null)
                throw new ArgumentNullException("layers");

            _layers.AddRange(layers);

            onAddedLayers(layers);
        }

        public bool ContainsAny(IEnumerable<ILayer> value)
        {
            return _layers.Exists(delegate(ILayer test) { return test == value; });
        }

        public event EventHandler<ModelCollectionChangedEventArgs<ILayer>> CollectionChanged;

        #region IList Implementation
        public int Add(ILayer layer)
        {
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}

            _layers.Add(layer);

            onAddedLayers(new ILayer[] { layer });

            return _layers.Count - 1;
        }

        public void Clear()
        {
            List<ILayer> layers = new List<ILayer>(this);

            _layers.Clear();

            onRemovedLayers(layers);
        }

        public int Count
        {
            get { return _layers.Count; }
        }

        public bool Contains(ILayer item)
        {
            return _layers.Contains(item);
        }

		public int IndexOf(ILayer item)
        {
			return _layers.IndexOf(item);
        }

        public void Insert(int index, ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            _layers.Insert(index, layer);
        }

        public void Remove(ILayer layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException("layer");
            }

            _layers.Remove(layer);

            onRemovedLayers(new ILayer[] { layer });
        }

        public void RemoveAt(int index)
        {
            ILayer layer = this[index];
            _layers.RemoveAt(index);

            onRemovedLayers(new ILayer[] { layer });
        }

        public ILayer this[int index]
        {
            get { return _layers[index]; }
            set { _layers[index] = value; }
        }

        public ILayer this[string layerName]
        {
            get
            {
                return _layers.Find(delegate(ILayer layer)
                    {
                        return String.Compare(layer.LayerName, layerName, StringComparison.CurrentCultureIgnoreCase) == 0;
                    });
            }
            set
            {
                ILayer layer = _layers.Find(delegate(ILayer candidateLayer)
                    {
                        return String.Compare(candidateLayer.LayerName, layerName, StringComparison.CurrentCultureIgnoreCase) == 0;
                    });

                int index = this.IndexOf(layer);
                _layers[index] = value;
            }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void CopyTo(ILayer[] array, int arrayIndex)
        {
            _layers.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ILayer> GetEnumerator()
        {
            return _layers.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection<ILayer> Members

        void ICollection<ILayer>.Add(ILayer item)
        {
            this.Add(item);
        }

        bool ICollection<ILayer>.Remove(ILayer item)
        {
            return _layers.Remove(item);
        }

        #endregion

        private void onAddedLayers(IEnumerable<ILayer> layers)
        {
            EventHandler<ModelCollectionChangedEventArgs<ILayer>> e = CollectionChanged;

            if (e != null)
            {
                ModelCollectionChangedEventArgs<ILayer> args = new ModelCollectionChangedEventArgs<ILayer>(layers, CollectionChangeAction.Add);
                e(this, args);
            }
        }

        private void onRemovedLayers(IEnumerable<ILayer> layers)
        {
            EventHandler<ModelCollectionChangedEventArgs<ILayer>> e = CollectionChanged;

            if (e != null)
            {
                ModelCollectionChangedEventArgs<ILayer> args = new ModelCollectionChangedEventArgs<ILayer>(layers, CollectionChangeAction.Remove);
                e(this, args);
            }
        }
    }
}
