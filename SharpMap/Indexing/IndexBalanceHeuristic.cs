using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing
{
    /// <summary>
    /// Heuristics used for tree generation and maintenance.
    /// </summary>
    public abstract class IndexBalanceHeuristic
    {
        private int _nodeItemMin;
        private int _nodeItemMax;
        private uint _maxTreeDepth;

        protected IndexBalanceHeuristic(int nodeItemMinimumCount, int nodeItemMaximumCount, uint maxTreeDepth)
        {
            _nodeItemMin = nodeItemMinimumCount;
            _nodeItemMax = nodeItemMaximumCount;
            _maxTreeDepth = maxTreeDepth;
        }

        /// <summary>
        /// Minimum number of index entries in a node before it is a candiate for splitting
        /// the node.
        /// </summary>
        public virtual int NodeItemMinimumCount
        {
            get { return _nodeItemMin; }
        }

        /// <summary>
        /// Number of index entries in a node to target. More than this will cause a split
        /// if <see cref="MaxTreeDepth"/> is not reached.
        /// </summary>
        public virtual int NodeItemMaximumCount
        {
            get { return _nodeItemMax; }
        }

        /// <summary>
        /// The maximum depth of the tree including the root.
        /// </summary>
        public virtual uint MaxTreeDepth
        {
            get { return _maxTreeDepth; }
        }

        /// <summary>
        /// The target number of nodes for a split node.
        /// </summary>
        public abstract int TargetNodeCount { get; }
    }
}
