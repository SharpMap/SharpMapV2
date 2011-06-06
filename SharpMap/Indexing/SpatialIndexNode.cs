// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
        where TItem : IBoundable<IExtents>
    {
        //private readonly Func<TItem, IExtents> _bounder;
        //private UInt32? _nodeId;
        //private readonly IExtents _emptyBounds;
        private IExtents _bounds;
        private IList<TItem> _items;
        private ISpatialIndex<IExtents, TItem> _index;
        private Int32 _level;

        public SpatialIndexNode(Int32 level)
        {
            _level = level;
        }

        //protected SpatialIndexNode(IExtents emptyBounds)
        //{
        //    //_bounder = bounder;
        //    _emptyBounds = emptyBounds;
        //}

        //protected Func<TItem, IExtents> Bounder
        //{
        //    get
        //    {
        //        return _bounder;
        //    }
        //}

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
            get
            {
                if (_items == null)
                {
                    yield break;
                }

                foreach (TItem item in _items)
                {
                    yield return item;
                }
            }
        }

        public abstract IEnumerable<ISpatialIndexNode<IExtents, TItem>> SubNodes { get; }

        protected abstract void addSubNodes(IEnumerable<ISpatialIndexNode<IExtents, TItem>> children);

        protected abstract void addSubNode(ISpatialIndexNode<IExtents, TItem> child);

        /// <summary>
        /// Adds item to <see cref="Items"/> list and expands node's
        /// <see cref="ISpatialIndexNode{TBounds, TItem}.Bounds">bounds</see> 
        /// to contain the <paramref name="item"/>'s bounds.
        /// </summary>
        /// <param name="item">Item to add</param>
        protected void addItem(TItem item)
        {
            Boolean cancel;
            OnItemAdding(item, out cancel);

            if (cancel)
            {
                return;
            }

            ensureList();

            _items.Add(item);

            if (Bounds == null || Bounds.IsEmpty)
            {
                Bounds = item.Bounds.Clone() as IExtents;
            }
            else
            {
                Bounds.ExpandToInclude(item.Bounds);
            }

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
        protected void addItems(IEnumerable<TItem> items)
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

                ensureList();

                _items.Add(item);

                if (Bounds == null || Bounds.IsEmpty)
                {
                    Bounds = item.Bounds.Clone() as IExtents;
                }
                else
                {
                    Bounds.ExpandToInclude(item.Bounds);
                }

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
        protected virtual Boolean removeItem(TItem item)
        {
            Boolean cancel;

            OnItemRemoving(item, out cancel);

            if (cancel)
            {
                return false;
            }

            if (_items == null)
            {
                return false;
            }

            Boolean removed = _items.Remove(item);

            if (removed)
            {
                OnItemRemoved(item);
            }

            IExtents itemBounds = item.Bounds;

            if (removed && Bounds.Intersects(itemBounds))
            {
                Bounds = null;

                foreach (TItem keptItem in _items)
                {
                    if (Bounds == null || Bounds.IsEmpty)
                    {
                        Bounds = keptItem.Bounds.Clone() as IExtents;
                    }
                    else
                    {
                        Bounds.ExpandToInclude(keptItem.Bounds);
                    }
                }
            }

            return removed;
        }

        protected abstract Boolean removeSubNode(ISpatialIndexNode<IExtents, TItem> child);

        /// <summary>
        /// Removes all contained items and sets the 
        /// <see cref="ISpatialIndexNode{TBounds, TItem}.Bounds">bounds</see> 
        /// to <see langword="null"/>.
        /// </summary>
        public void Clear()
        {
            Boolean cancel;
            OnClearing(out cancel);

            if (cancel)
            {
                return;
            }

            if (_items != null)
            _items.Clear();

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

        public abstract Int32 SubNodeCount { get; }

        public Int32 ItemCount
        {
            get { return _items == null ? 0 : _items.Count; }
        }

        public Boolean IsEmpty
        {
            get
            {
                if (ItemCount > 0)
                {
                    return false;
                }

                if (HasSubNodes)
                {
                    foreach (ISpatialIndexNode<IExtents, TItem> child in SubNodes)
                    {
                        if (!child.IsEmpty)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }

        public Boolean IsPrunable
        {
            get { return !HasSubNodes && !HasItems; }
        }

        public Boolean HasItems
        {
            get { return ItemCount > 0; }
        }

        public Boolean HasSubNodes
        {
            get { return SubNodeCount > 0; }
        }

        public IEnumerable<TItem> Query(IExtents query)
        {
            if (HasItems)
            {
                foreach (TItem item in _items)
                {
                    if (item.Intersects(query))
                    {
                        yield return item;
                    }
                }
            }

            if (HasSubNodes)
            {
                foreach (ISpatialIndexNode<IExtents, TItem> child in SubNodes)
                {
                    foreach (TItem item in child.Query(query))
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<TItem> Query(IExtents query, Predicate<TItem> predicate)
        {
            foreach (TItem item in Query(query))
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<TResult> Query<TResult>(IExtents query, Func<TItem, TResult> selector)
        {
            foreach (TItem item in Query(query))
            {
                yield return selector(item);
            }
        }

        public Int32 TotalItemCount
        {
            get
            {
                Int32 total = ItemCount;

                if (HasSubNodes)
                {
                    foreach (ISpatialIndexNode<IExtents, TItem> child in SubNodes)
                    {
                        total += child.TotalItemCount;
                    }
                }

                return total;
            }
        }

        public Int32 TotalNodeCount
        {
            get
            {
                Int32 total = SubNodeCount;

                if (HasSubNodes)
                {
                    foreach (ISpatialIndexNode<IExtents, TItem> child in SubNodes)
                    {
                        total += child.TotalNodeCount;
                    }
                }

                return total;
            }
        }

        #endregion

        protected virtual IList<TItem> CreateItemList()
        {
            return new List<TItem>();
        }

        #region IBoundable<IExtents> Members


        public Boolean Intersects(IExtents bounds)
        {
            return bounds != null && _bounds != null && _bounds.Intersects(bounds);
        }

        #endregion

        private void ensureList()
        {
            if (_items == null)
            {
                _items = CreateItemList();
            }
        }


        #region ISpatialIndexNode<IExtents,TItem> Members

        public void Add(IBoundable<IExtents> child)
        {
            if (child is ISpatialIndexNode<IExtents, TItem>)
                addSubNode((ISpatialIndexNode<IExtents, TItem>)child);
            else if (child is TItem)
                addItem((TItem)child);
            else
                throw new ArgumentException();
        }

        public void AddRange(IEnumerable<IBoundable<IExtents>> children)
        {
            foreach (IBoundable<IExtents> child in children)
            {
                Add(child);
            }
        }

        public IEnumerable<IBoundable<IExtents>> ChildBoundables
        {
            get
            {
                foreach (TItem item in Items)
                {
                    yield return item;
                }
                foreach (ISpatialIndexNode<IExtents, TItem> node in SubNodes)
                {
                    yield return node;
                }
            }
        }

        public bool Remove(IBoundable<IExtents> child)
        {
            if (child is ISpatialIndexNode<IExtents, TItem>)
                return removeSubNode(child as ISpatialIndexNode<IExtents, TItem>);
            if (child is TItem)
                return removeItem((TItem)child);
            throw new ArgumentException();
        }

        public bool RemoveRange(IEnumerable<IBoundable<IExtents>> children)
        {
            foreach (IBoundable<IExtents> child in children)
            {
                Remove(child);
            }
            return true;//hack
        }

        #endregion


        public void InsertRange(IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                addItem(item);
            }
        }

        public Int32 Level
        {
            get
            {
                return _level;
            }
        }
    }
}