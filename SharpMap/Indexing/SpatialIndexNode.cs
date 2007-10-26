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
using SharpMap.Geometries;

namespace SharpMap.Indexing
{
    /// <summary>
    /// Abstract representation of a node in an 
    /// <see cref="ISearchableSpatialIndex{TEntry}"/> or a 
    /// <see cref="IUpdatableSpatialIndex{TEntry}"/>.
    /// </summary>
    /// <typeparam name="TItem">Type of the node's children items.</typeparam>
    /// <typeparam name="TEntry">Type of the index entry.</typeparam>
    public abstract class SpatialIndexNode<TItem, TEntry> : ISpatialIndexNode
    {
        private UInt32? _nodeId;
        private BoundingBox _boundingBox = BoundingBox.Empty;
        private readonly List<TItem> _items = new List<TItem>();
        private ISearchableSpatialIndex<TEntry> _index;

        /// <summary>
        /// Gets the BoundingBox for this node, which minimally bounds all items.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            protected set { _boundingBox = value; }
        }

        /// <summary>
        /// Node identifier.
        /// </summary>
        public UInt32? NodeId
        {
            get { return _nodeId; }
            protected set { _nodeId = value; }
        }

        /// <summary>
        /// Gets a reference to the containing spatial index.
        /// </summary>
        public ISearchableSpatialIndex<TEntry> Index
        {
            get { return _index; }
            protected set { _index = value; }
        }

        /// <summary>
        /// Gets the list of items in the index node.
        /// </summary>
        public IList<TItem> Items
        {
            get { return _items.AsReadOnly(); }
        }

        /// <summary>
        /// Adds item to <see cref="Items"/> list and expands node's
        /// <see cref="ISpatialIndexNode.BoundingBox">bounding box</see> to contain the
        /// <paramref name="item"/>'s BoundingBox.
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(TItem item)
        {
            Boolean cancel;
            OnItemAdding(item, out cancel);

            if (cancel)
            {
                return;
            }

            _items.Add(item);
            BoundingBox = BoundingBox.Join(GetItemBoundingBox(item));

            OnItemAdded(item);
        }

        /// <summary>
        /// Adds <paramref name="items">items</paramref> to 
        /// <see cref="Items"/> list and expands node's
        /// <see cref="BoundingBox">bounding box</see> to contain all 
        /// <paramref name="items">items'</paramref>
        /// bounding boxes.
        /// </summary>
        /// <param name="items">Items to add.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="items"/> is null.
        /// </exception>
        public void AddRange(IEnumerable<TItem> items)
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
                BoundingBox = BoundingBox.Join(GetItemBoundingBox(item));
                OnItemAdded(item);
            }
        }

        /// <summary>
        /// Removes an item and adjusts 
        /// <see cref="ISpatialIndexNode.BoundingBox">bounding box</see> 
        /// to accomodate new set of items.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the item was found and removed, 
        /// <see langword="false"/> if not found or not removed.
        /// </returns>
        public virtual Boolean Remove(TItem item)
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

            BoundingBox itemBounds = GetItemBoundingBox(item);

            if (removed && BoundingBox.Borders(itemBounds))
            {
                BoundingBox = BoundingBox.Empty;

                foreach (TItem keptItem in _items)
                {
                    BoundingBox = BoundingBox.Join(GetItemBoundingBox(keptItem));
                }
            }

            return removed;
        }

        /// <summary>
        /// Removes all contained items and sets the 
        /// <see cref="ISpatialIndexNode.BoundingBox">bounding box</see> 
        /// to <see cref="SharpMap.Geometries.BoundingBox.Empty"/>.
        /// </summary>
        public void Clear()
        {
            Boolean cancel;
            OnClearing(out cancel);
            BoundingBox = BoundingBox.Empty;
            OnCleared();
        }

        public override String ToString()
        {
            return String.Format("Node Id: {0}; BoundingBox: {1}", _nodeId, _boundingBox);
        }

        /// <summary>
        /// Gets the bounding box for the <paramref name="item">item</paramref>.
        /// </summary>
        /// <param name="item">The item to retrieve the bounding box for.</param>
        /// <returns>The bounding box of the given <paramref name="item">item</paramref>.</returns>
        protected abstract BoundingBox GetItemBoundingBox(TItem item);

        protected virtual void OnClearing(out Boolean cancel)
        {
            _items.Clear();
            cancel = false;
        }

        protected virtual void OnCleared() {}

        protected virtual void OnItemAdding(TItem entry, out Boolean cancel)
        {
            cancel = false;
        }

        protected virtual void OnItemAdded(TItem entry) {}

        protected virtual void OnItemRemoving(TItem entry, out Boolean cancel)
        {
            cancel = false;
        }

        protected virtual void OnItemRemoved(TItem entry) {}
    }
}