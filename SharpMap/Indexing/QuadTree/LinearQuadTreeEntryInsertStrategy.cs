// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

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
            throw new NotImplementedException();
        }
    }

    [global::System.Serializable]
    public class QuadTreeIndexInsertOverflowException : Exception
    {
        public QuadTreeIndexInsertOverflowException() { }
        public QuadTreeIndexInsertOverflowException(String message) : base(message) { }
        public QuadTreeIndexInsertOverflowException(String message, Exception inner) : base(message, inner) { }
        protected QuadTreeIndexInsertOverflowException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
