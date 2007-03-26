using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing
{

    public interface INodeSplitStrategy
    {
        Node SplitNode(Node node, DynamicRTreeHeuristic huristic);
    }

    public interface IEntryInsertStrategy
    {
        void InsertEntry(RTreeIndexEntry entry, Node node, INodeSplitStrategy nodeSplitStrategy, DynamicRTreeHeuristic heuristic, out Node newSiblingFromSplit);
    }

    public interface IIndexRestructureStrategy
    {
        void RestructureNode(Node node);
    }
}
