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
    public class DynamicQuadTree<TItem> : QuadTree<TItem>, IUpdatableSpatialIndex<IExtents, TItem>
    {
        private readonly IItemInsertStrategy<IExtents, TItem> _insertStrategy;
        private readonly INodeSplitStrategy<IExtents, TItem> _nodeSplitStrategy;
        private readonly DynamicQuadTreeBalanceHeuristic _heuristic;

        /// <summary>
        /// Creates a new updatable QuadTree instance
        /// </summary>
        /// <param name="insertStrategy">Strategy used to insert new entries into the index.</param>
        /// <param name="nodeSplitStrategy">Strategy used to split nodes when they are full.</param>
        /// <param name="heuristic">Heuristics used to build the index and keep it balanced.</param>
        public DynamicQuadTree(IItemInsertStrategy<IExtents, TItem> insertStrategy, INodeSplitStrategy<IExtents, TItem> nodeSplitStrategy, DynamicQuadTreeBalanceHeuristic heuristic, Func<TItem, IExtents> bounder)
            :base(bounder)
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

        #region IUpdatableSpatialIndex<IExtents,TItem> Members

        public bool Remove(IExtents bounds, TItem item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
