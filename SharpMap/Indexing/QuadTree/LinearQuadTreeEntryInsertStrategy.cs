using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.QuadTree
{
    public class LinearQuadTreeEntryInsertStrategy<TValue> : IEntryInsertStrategy<TValue>
    {
        #region IEntryInsertStrategy<TValue> Members

        public void InsertEntry(TValue entry, ISpatialIndexNode node, INodeSplitStrategy nodeSplitStrategy, IndexBalanceHeuristic heuristic, out ISpatialIndexNode newSiblingFromSplit)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (!(node is QuadTreeNode<TValue>))
            {
                throw new ArgumentException("Parameter 'node' must be of type QuadTreeNode<TValue>", "node");
            }

            QuadTreeNode<TValue> quadTreeNode = node as QuadTreeNode<TValue>;

            insertEntryRecursive(quadTreeNode, entry, nodeSplitStrategy, heuristic, out newSiblingFromSplit);

            if (newSiblingFromSplit != null)
            {
                if (quadTreeNode.Items.Count == 4)
                {
                    throw new QuadTreeIndexInsertOverflowException();
                }

                quadTreeNode.Items.Add(newSiblingFromSplit as QuadTreeNode<TValue>);
            }
        }

        #endregion

        private void insertEntryRecursive(QuadTreeNode<TValue> quadTreeNode, TValue entry, INodeSplitStrategy nodeSplitStrategy, IndexBalanceHeuristic heuristic, out ISpatialIndexNode newSiblingFromSplit)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    [global::System.Serializable]
    public class QuadTreeIndexInsertOverflowException : Exception
    {
        public QuadTreeIndexInsertOverflowException() { }
        public QuadTreeIndexInsertOverflowException(string message) : base(message) { }
        public QuadTreeIndexInsertOverflowException(string message, Exception inner) : base(message, inner) { }
        protected QuadTreeIndexInsertOverflowException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
