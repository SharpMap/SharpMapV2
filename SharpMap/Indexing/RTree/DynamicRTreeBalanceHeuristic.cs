using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.RTree
{
    public class DynamicRTreeBalanceHeuristic : IndexBalanceHeuristic
    {
        /// <summary>
        /// Creates a new DynamicRTreeBalanceHeuristic with optimum heuristic values.
        /// </summary>
        /// <remarks>
        /// Where did I find these so called "optimum" values? I wrote them down without the
        /// reference.... d'oh!
        /// TODO: find the reference for RTree insert heuristics.
        /// </remarks>
        public DynamicRTreeBalanceHeuristic()
            : base(4, 10, UInt16.MaxValue) { }

        public DynamicRTreeBalanceHeuristic(int nodeItemMinimumCount, int nodeItemMaximumCount, uint maxTreeDepth)
            : base(nodeItemMinimumCount, nodeItemMaximumCount, maxTreeDepth) { }

        public override int TargetNodeCount
        {
            get { return NodeItemMaximumCount - NodeItemMinimumCount + 1; }
        }
    }
}
