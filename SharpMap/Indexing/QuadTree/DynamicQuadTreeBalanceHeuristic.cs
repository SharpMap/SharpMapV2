using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.QuadTree
{
    public class DynamicQuadTreeBalanceHeuristic : IndexBalanceHeuristic
    {
        public DynamicQuadTreeBalanceHeuristic()
            : base(4, 4, UInt16.MaxValue) { }

        public override int TargetNodeCount
        {
            get { return 4; }
        }
    }
}
