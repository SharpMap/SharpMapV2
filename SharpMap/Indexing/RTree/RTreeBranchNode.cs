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
    /// A branch node in an R-Tree index.
    /// </summary>
    /// <typeparam name="TItem">The type of the value used in the entries.</typeparam>
    public class RTreeBranchNode<TItem> : SpatialIndexNode<TItem>
    {
        internal RTreeBranchNode(ISpatialIndex<IExtents, TItem> index, Func<TItem, IExtents> bounder)
            : base(bounder)
        {
            Index = index;
        }

        /// <summary>
        /// Gets the node's unique id.
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
        /// Gets the extents of the nodes accessible via this branch.
        /// </summary>
        public new IExtents Bounds
        {
            get { return base.Bounds; }
            protected internal set { base.Bounds = value; }
        }

        /// <summary>
        /// Recursively descends the branch to remove the given <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The index entry to remove.</param>
        public void Remove(TItem entry)
        {
            foreach (ISpatialIndexNode<IExtents, TItem> node in Items)
            {
                IExtents itemBounds = Bounder(entry);

                if (node.Bounds.Contains(itemBounds))
                {
                    if (node is RTreeBranchNode<TItem>)
                    {
                        RTreeBranchNode<TItem> branch = node as RTreeBranchNode<TItem>;
                        branch.Remove(entry);
                    }
                    else if (node is RTreeLeafNode<TItem>)
                    {
                        RTreeLeafNode<TItem> leaf = node as RTreeLeafNode<TItem>;
                        leaf.Remove(entry);
                    }

                    break;
                }
            }
        }

        public override String ToString()
        {
            return String.Format("[{0}] NodeId: {1}; BoundingBox: {2}; Children Count: {3}", 
                GetType(), NodeId, Bounds, Items.Count);
        }
    }
}