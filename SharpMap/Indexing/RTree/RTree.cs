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
using System.Threading;
using SharpMap.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// Implements an R-Tree spatial index.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    public abstract class RTree<TValue> : ISearchableSpatialIndex<RTreeIndexEntry<TValue>>, IDisposable

    {
        private ISpatialIndexNode _root;
        private Boolean _disposed;
        private Int64 _nextNodeId = 0;

        #region Object Construction/Destruction

        /// <summary>
        /// Creates a new instance of an RTree index.
        /// </summary>
        public RTree()
        {
            initIndex();
        }

        ~RTree()
        {
            Dispose(false);
        }

        #region Dispose Pattern

        /// <summary>
        /// Disposes the index.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(Boolean disposing) {}

        protected Boolean IsDisposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Root node in the R-Tree
        /// </summary>
        public ISpatialIndexNode Root
        {
            get { return _root; }
            protected set { _root = value; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clears the index of all entries.
        /// </summary>
        public void Clear()
        {
            initIndex();
        }

        /// <summary>
        /// Searches the tree and looks for intersections with the <see cref="BoundingBox"/> <paramref name="box"/>.
        /// </summary>
        /// <param name="box">BoundingBox to intersect the index with.</param>
        public virtual IEnumerable<RTreeIndexEntry<TValue>> Search(BoundingBox box)
        {
            List<RTreeIndexEntry<TValue>> objectlist = new List<RTreeIndexEntry<TValue>>();
            IntersectTreeRecursive(box, Root, objectlist);
            return objectlist;
        }

        /// <summary>
        /// Searches the tree and looks for intersections with the
        /// given <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry">Geometry to intersect the index with.</param>
        public virtual IEnumerable<RTreeIndexEntry<TValue>> Search(Geometry geometry)
        {
            if (geometry == null || geometry.IsEmpty())
            {
                return new RTreeIndexEntry<TValue>[0];
            }

            return Search(geometry.GetBoundingBox());
        }

        #endregion

        #region Protected / Internal methods

        /// <summary>
        /// Creates a new index branch node with an empty bounding box.
        /// </summary>
        /// <returns>A new branch node for the R-Tree.</returns>
        protected internal virtual RTreeBranchNode<TValue> CreateBranchNode()
        {
            RTreeBranchNode<TValue> node = new RTreeBranchNode<TValue>(this);
            node.NodeId = getNewNodeId();
            return node;
        }

        /// <summary>
        /// Creates a new index branch node with the given bounding box.
        /// </summary>
        /// <returns>A new branch node for the R-Tree.</returns>
        protected internal virtual RTreeBranchNode<TValue> CreateBranchNode(BoundingBox boundingBox)
        {
            RTreeBranchNode<TValue> node = CreateBranchNode();
            node.BoundingBox = boundingBox;
            return node;
        }

        /// <summary>
        /// Creates a new index leaf node with an empty bounding box.
        /// </summary>
        /// <returns>A new leaf node for the R-Tree.</returns>
        protected internal virtual RTreeLeafNode<TValue> CreateLeafNode()
        {
            RTreeLeafNode<TValue> node = new RTreeLeafNode<TValue>(this);
            node.NodeId = getNewNodeId();
            return node;
        }

        /// <summary>
        /// Creates a new index leaf node with the given bounding box.
        /// </summary>
        /// <returns>A new leaf node for the R-Tree.</returns>
        protected internal virtual RTreeLeafNode<TValue> CreateLeafNode(BoundingBox boundingBox)
        {
            RTreeLeafNode<TValue> node = CreateLeafNode();
            node.BoundingBox = boundingBox;
            return node;
        }

        /// <summary>
        /// Recursive function that traverses the tree and looks for intersections 
        /// with a given <see cref="BoundingBox">region</see>.
        /// </summary>
        /// <remarks>
        /// Nothing is added to the list if <paramref name="box"/> equals 
        /// <see cref="BoundingBox.Empty"/>.
        /// </remarks>
        /// <param name="box">Boundingbox to intersect with</param>
        /// <param name="node">Node to search from</param>
        /// <param name="list">List of found intersections</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="node"/> 
        /// is null or if <paramref name="list"/> is null.
        /// </exception>
        protected static void IntersectTreeRecursive(BoundingBox box, ISpatialIndexNode node,
                                              List<RTreeIndexEntry<TValue>> list)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (list == null) throw new ArgumentNullException("list");

            if (box == BoundingBox.Empty)
            {
                return;
            }

            if (node is RTreeLeafNode<TValue>) //Leaf has been reached
            {
                foreach (RTreeIndexEntry<TValue> entry in (node as RTreeLeafNode<TValue>).Items)
                {
                    if (entry.BoundingBox.Intersects(box))
                    {
                        list.Add(entry);
                    }
                }
            }
            else if (node is RTreeBranchNode<TValue>)
            {
                if (box.Intersects(node.BoundingBox))
                {
                    foreach (ISpatialIndexNode child in (node as RTreeBranchNode<TValue>).Items)
                    {
                        IntersectTreeRecursive(box, child, list);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Unknown node type: " + node.GetType());
            }
        }

        #endregion

        #region Private helper methods

        private void initIndex()
        {
            Root = CreateLeafNode();
        }

        private UInt32 getNewNodeId()
        {
            return (UInt32) Interlocked.Increment(ref _nextNodeId);
        }

        #endregion
    }
}