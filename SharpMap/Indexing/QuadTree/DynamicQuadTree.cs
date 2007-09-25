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

        public void Remove(TValue entry)
        {
            // TODO: Implement quad-tree remove.
            throw new NotImplementedException("TODO: Implement quad-tree remove");
        }

        #endregion
    }
}
