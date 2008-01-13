// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing
{
    /// <summary>
    /// Abstract representation of a node in an 
    /// <see cref="ISpatialIndex{TBounds,TItem}"/> or a 
    /// <see cref="IUpdatableSpatialIndex{TBounds,TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">Type of the index entry.</typeparam>
    public abstract class SpatialIndexNode<TItem> : ISpatialIndexNode<IExtents, TItem>
    {
        private readonly Func<TItem, IExtents> _bounder;
        //private UInt32? _nodeId;
        private IExtents _bounds;
        private readonly List<TItem> _items = new List<TItem>();
        private ISpatialIndex<IExtents, TItem> _index;

        protected SpatialIndexNode(Func<TItem, IExtents> bounder)
        {
            _bounder = bounder;
        }

        protected Func<TItem, IExtents> Bounder
        {
            get
            {
                return _bounder;
            }
        }

        /// <summary>
        /// Gets the extents for this node, which minimally bounds all items.
        /// </summary>
        public IExtents Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }

        public abstract Boolean IsLeaf { get; }

        ///// <summary>
        ///// Node identifier.
        ///// </summary>
        //public UInt32? NodeId
        //{
        //    get { return _nodeId; }
        //    protected set { _nodeId = value; }
        //}

        /// <summary>
        /// Gets a reference to the containing spatial index.
        /// </summary>
        public ISpatialIndex<IExtents, TItem> Index
        {
            get { return _index; }
            protected set { _index = value; }
        }

        /// <summary>
        /// Gets the list of items in the index node.
        /// </summary>
        public IEnumerable<TItem> Items
        {
            get { return _items; }
        }

        public abstract IEnumerable<ISpatialIndexNode<IExtents, TItem>> Children { get; }

        public abstract void AddChildren(IEnumerable<ISpatialIndexNode<IExtents, TItem>> children);

        public abstract void AddChild(ISpatialIndexNode<IExtents, TItem> child);

        /// <summary>
        /// Adds item to <see cref="Items"/> list and expands node's
        /// <see cref="ISpatialIndexNode{TBounds, TItem}.Bounds">bounds</see> 
        /// to contain the <paramref name="item"/>'s bounds.
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItem(TItem item)
        {
            Boolean cancel;
            OnItemAdding(item, out cancel);

            if (cancel)
            {
                return;
            }

            _items.Add(item);
            Bounds = Bounds.Union(_bounder(item));

            OnItemAdded(item);
        }

        /// <summary>
        /// Adds <paramref name="items">items</paramref> to 
        /// <see cref="Items"/> list and expands node's
        /// <see cref="IExtents">extents</see> to contain all 
        /// <paramref name="items">items'</paramref>
        /// bounding boxes.
        /// </summary>
        /// <param name="items">Items to add.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="items"/> is null.
        /// </exception>
        public void AddItems(IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (TItem item in items)
            {
                Boolean cancel;
                OnItemAdding(item, out cancel);

                if (cancel)
                {
                    continue;
                }

                _items.Add(item);
                Bounds = Bounds.Union(_bounder(item));
                OnItemAdded(item);
            }
        }

        /// <summary>
        /// Removes an item and adjusts 
        /// <see cref="ISpatialIndexNode{TBounds, TItem}.Bounds">bounds</see> 
        /// to accomodate new set of items.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the item was found and removed, 
        /// <see langword="false"/> if not found or not removed.
        /// </returns>
        public virtual Boolean RemoveItem(TItem item)
        {
            Boolean cancel;

            OnItemRemoving(item, out cancel);

            if (cancel)
            {
                return false;
            }

            Boolean removed = _items.Remove(item);

            if (removed)
            {
                OnItemRemoved(item);
            }

            IExtents itemBounds = _bounder(item);

            if (removed && Bounds.Borders(itemBounds))
            {
                Bounds = null;

                foreach (TItem keptItem in _items)
                {
                    Bounds = Bounds.Union(_bounder(keptItem));
                }
            }

            return removed;
        }

        public abstract Boolean RemoveChild(ISpatialIndexNode<IExtents, TItem> child);


        /// <summary>
        /// Removes all contained items and sets the 
        /// <see cref="ISpatialIndexNode{TBounds, TItem}.Bounds">bounds</see> 
        /// to <see langword="null"/>.
        /// </summary>
        public void Clear()
        {
            Boolean cancel;
            OnClearing(out cancel);
            Bounds = null;
            OnCleared();
        }

        ///// <summary>
        ///// Gets the bounding box for the <paramref name="item">item</paramref>.
        ///// </summary>
        ///// <param name="item">The item to retrieve the bounding box for.</param>
        ///// <returns>The bounding box of the given <paramref name="item">item</paramref>.</returns>
        //protected abstract IExtents GetItemBounds(TItem item);

        protected virtual void OnClearing(out Boolean cancel)
        {
            _items.Clear();
            cancel = false;
        }

        protected virtual void OnCleared() { }

        protected virtual void OnItemAdding(TItem entry, out Boolean cancel)
        {
            cancel = false;
        }

        protected virtual void OnItemAdded(TItem entry) { }

        protected virtual void OnItemRemoving(TItem entry, out Boolean cancel)
        {
            cancel = false;
        }

        protected virtual void OnItemRemoved(TItem entry) { }

        #region ISpatialIndexNode<IExtents,TItem> Members


        public int ChildCount
        {
            get { throw new NotImplementedException(); }
        }

        public int ItemCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsPrunable
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasItems
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasChildren
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<TItem> Query(IExtents query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TItem> Query(IExtents query, Predicate<TItem> predicate)
        {
            throw new NotImplementedException();
        }

        public int TotalItems
        {
            get { throw new NotImplementedException(); }
        }

        public int TotalNodes
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IBoundable<IExtents> Members


        public bool Intersects(IExtents bounds)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}