// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing.QuadTree
{
    public class LinearQuadTreeEntryInsertStrategy<TItem> : IItemInsertStrategy<IExtents, TItem>
        where TItem : IBoundable<IExtents>
    {
        #region IEntryInsertStrategy<TItem> Members

        public void Insert(IExtents bounds, TItem entry, ISpatialIndexNode<IExtents, TItem> node, INodeSplitStrategy<IExtents, TItem> nodeSplitStrategy, IndexBalanceHeuristic heuristic, out ISpatialIndexNode<IExtents, TItem> newSiblingFromSplit)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (!(node is QuadTreeNode<TItem>))
            {
                throw new ArgumentException("Parameter 'node' must be of type QuadTreeNode<TItem>", "node");
            }

            QuadTreeNode<TItem> quadTreeNode = node as QuadTreeNode<TItem>;

            insertEntryRecursive(quadTreeNode, entry, nodeSplitStrategy, heuristic, out newSiblingFromSplit);

            if (newSiblingFromSplit != null)
            {
                if (quadTreeNode.ItemCount == 4)
                {
                    throw new QuadTreeIndexInsertOverflowException();
                }

                quadTreeNode.Add(newSiblingFromSplit);
            }
        }

        #endregion

        private void insertEntryRecursive(QuadTreeNode<TItem> quadTreeNode, TItem entry, INodeSplitStrategy<IExtents, TItem> nodeSplitStrategy, IndexBalanceHeuristic heuristic, out ISpatialIndexNode<IExtents, TItem> newSiblingFromSplit)
        {
            throw new NotImplementedException();
        }

        #region IItemInsertStrategy<IExtents,TItem> Members


        public ISpatialIndexNodeFactory<IExtents, TItem> NodeFactory
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
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
