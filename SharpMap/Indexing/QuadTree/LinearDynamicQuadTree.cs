using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.QuadTree
{
    public class LinearDynamicQuadTree<TValue> : DynamicQuadTree<TValue>
    {
        public LinearDynamicQuadTree()
            : base(new LinearQuadTreeEntryInsertStrategy<TValue>(), new LinearQuadTreeNodeSplitStrategy<TValue>(), new DynamicQuadTreeBalanceHeuristic())
        {
        }
    }
}
