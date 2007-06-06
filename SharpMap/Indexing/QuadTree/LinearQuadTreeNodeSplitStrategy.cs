using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.QuadTree
{
    public class LinearQuadTreeNodeSplitStrategy<TValue> : INodeSplitStrategy
    {
        #region INodeSplitStrategy Members

        public ISpatialIndexNode SplitNode(ISpatialIndexNode node, IndexBalanceHeuristic heuristic)
        {
            if (node == null)
            {
                return null;
            }

            QuadTreeNode<TValue> quadTreeNode = node as QuadTreeNode<TValue>;
            DynamicQuadTreeBalanceHeuristic quadTreeHeuristic = heuristic as DynamicQuadTreeBalanceHeuristic;

            throw new NotImplementedException();
        }

        #endregion
    }
}
