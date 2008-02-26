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
using System.Collections.Generic;
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing.QuadTree
{
    public class QuadTree<TItem> : QuadTreeNode<TItem>, ISpatialIndex<IExtents, TItem>
        where TItem : IBoundable<IExtents>
    {
        public QuadTree(IGeometryFactory geoFactory)
            : base(geoFactory.CreateExtents()) { }

        /// <summary>
        /// Adds a spatial item with an extent specified by the given
        /// <paramref name="bounds"/> to the index.
        /// </summary>
        public void Insert(IExtents bounds, TItem item)
        {
            throw new NotImplementedException();
        }

        /// <summary> 
        /// Queries the index for all items whose extents intersect the given search <c>Envelope</c> 
        /// Note that some kinds of indexes may also return objects which do not in fact
        /// intersect the query envelope.
        /// </summary>
        /// <param name="bounds">The envelope to query for.</param>
        /// <returns>A list of the items found by the query.</returns>
        public IEnumerable<TItem> Query(IExtents bounds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the index for all items whose extents intersect the given search 
        /// <see cref="IExtents{TCoordinate}" />, and applies an <see cref="Action{T}" /> to them.
        /// Note that some kinds of indexes may also return objects which do not in fact
        /// intersect the query envelope.
        /// </summary>
        /// <param name="bounds">The envelope to query for.</param>
        /// <param name="predicate">A predicate delegate to apply to the items found.</param>
        public IEnumerable<TItem> Query(IExtents bounds, Predicate<TItem> predicate)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region ISpatialIndex<IExtents,TItem> Members

        public void Insert(TItem item)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}