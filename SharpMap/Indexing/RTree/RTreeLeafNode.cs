using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// A leaf node in an R-Tree index.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    public class RTreeLeafNode<TValue> : SpatialIndexNode<RTreeIndexEntry<TValue>, RTreeIndexEntry<TValue>>
        
    {
        internal RTreeLeafNode(ISearchableSpatialIndex<RTreeIndexEntry<TValue>> index)
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
        public new BoundingBox BoundingBox
        {
            get { return base.BoundingBox; }
            protected internal set { base.BoundingBox = value; }
        }

        public override string ToString()
        {
            return String.Format("Leaf NodeId: {0}; BoundingBox: {1}; Entries: {2}", NodeId, BoundingBox, Items.Count);
        }

        /// <summary>
        /// Gets the bounding box for the <paramref name="item">entry</paramref>.
        /// </summary>
        /// <param name="item">The entry to retrieve the bounding box for.</param>
        /// <returns>The bounding box of the given <paramref name="item">entry</paramref>.</returns>
        protected override BoundingBox GetItemBoundingBox(RTreeIndexEntry<TValue> item)
        {
            return item.BoundingBox;
        }
    }
}
