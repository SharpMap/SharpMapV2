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
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// A leaf node in an R-Tree index.
    /// </summary>
    /// <typeparam name="TItem">The type of the value used in the entries.</typeparam>
    public class RTreeLeafNode<TItem> : SpatialIndexNode<TItem>

    {
        internal RTreeLeafNode(ISpatialIndex<IExtents, TItem> index)
        {
            Index = index;
        }

        /// <summary>
        /// The node's unique id.
        /// </summary>
        /// <remarks>
        /// The scope of the uniqueness is the index to which the node belongs.
        /// </remarks>
        public new UInt32? NodeId
        {
            get { return base.NodeId; }
            protected internal set { base.NodeId = value; }
        }

        /// <summary>
        /// Gets the BoundingBox for this node, which minimally bounds all items.
        /// </summary>
        public new IExtents Bounds
        {
            get { return base.Bounds; }
            protected internal set { base.Bounds = value; }
        }

        public override String ToString()
        {
            return String.Format("Leaf NodeId: {0}; BoundingBox: {1}; Entries: {2}", NodeId, Bounds, Items.Count);
        }

        /// <summary>
        /// Gets the bounding box for the <paramref name="item">entry</paramref>.
        /// </summary>
        /// <param name="item">The entry to retrieve the bounding box for.</param>
        /// <returns>
        /// The bounding box of the given <paramref name="item">entry</paramref>.
        /// </returns>
        protected override IExtents GetItemBounds(RTreeIndexEntry<TItem> item)
        {
            return item.Bounds;
        }
    }
}