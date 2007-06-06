using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.QuadTree
{
    public class DynamicQuadTree<TValue> : QuadTree<TValue>, IUpdatableSpatialIndex<TValue>
    {
        private IEntryInsertStrategy<TValue> _insertStrategy;
        private INodeSplitStrategy _nodeSplitStrategy;
        private DynamicQuadTreeBalanceHeuristic _heuristic;

        /// <summary>
        /// Creates a new updatable QuadTree instance
        /// </summary>
        /// <param name="insertStrategy">Strategy used to insert new entries into the index.</param>
        /// <param name="nodeSplitStrategy">Strategy used to split nodes when they are full.</param>
        /// <param name="heuristic">Heuristics used to build the index and keep it balanced.</param>
        public DynamicQuadTree(IEntryInsertStrategy<TValue> insertStrategy, INodeSplitStrategy nodeSplitStrategy, DynamicQuadTreeBalanceHeuristic heuristic)
            : base()
        {
            _insertStrategy = insertStrategy;
            _nodeSplitStrategy = nodeSplitStrategy;
            _heuristic = heuristic;
        }

        /// <summary>
        /// Huristic used to create and balance the tree.
        /// </summary>
        public DynamicQuadTreeBalanceHeuristic Heuristic
        {
            get { return _heuristic; }
        }

        /// <summary>
        /// The strategy used to insert new entries into the index.
        /// </summary>
        public IEntryInsertStrategy<TValue> EntryInsertStrategy
        {
            get { return _insertStrategy; }
        }

        #region IUpdatableSpatialIndex<TValue> Members

        public void Insert(TValue entry)
        {
            ISpatialIndexNode newSiblingFromSplit;
            
            EntryInsertStrategy.InsertEntry(entry, this, _nodeSplitStrategy, Heuristic, out newSiblingFromSplit);

            if (newSiblingFromSplit != null)
	        {
                Items.Add(newSiblingFromSplit as QuadTreeNode<TValue>);
	        }
        }

        #endregion
    }
}
