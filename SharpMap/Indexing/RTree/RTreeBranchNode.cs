using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// A branch node in an R-Tree index.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    public class RTreeBranchNode<TValue> : SpatialIndexNode<ISpatialIndexNode, RTreeIndexEntry<TValue>>
        
    {
        internal RTreeBranchNode(ISearchableSpatialIndex<RTreeIndexEntry<TValue>> index)
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

        public new BoundingBox BoundingBox
        {
            get { return base.BoundingBox; }
            protected internal set { base.BoundingBox = value; }
        }

        public override string ToString()
        {
            return String.Format("Branch NodeId: {0}; BoundingBox: {1}; Children: {2}", NodeId, BoundingBox, Items.Count);
        }

        /// <summary>
        /// Gets the bounding box for the <paramref name="item">child node</paramref>.
        /// </summary>
        /// <param name="item">The child node to retrieve the bounding box for.</param>
        /// <returns>The bounding box of the given <paramref name="item">child node</paramref>.</returns>
        protected override BoundingBox GetItemBoundingBox(ISpatialIndexNode item)
        {
            return item.BoundingBox;
        }
    }
}
