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
using SharpMap.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// An entry in an <see cref="RTree"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    public struct RTreeIndexEntry<TValue>
    {
        private TValue _value;
        private BoundingBox _boundingBox;

        /// <summary>
        /// Creates a new instance of an RTreeIndexEntry.
        /// </summary>
        /// <param name="value">The value to store in the index.</param>
        /// <param name="box">The extents of the <paramref name="value"/>.</param>
        public RTreeIndexEntry(TValue value, BoundingBox box)
        {
            _value = value;
            _boundingBox = box;
        }

        /// <summary>
        /// The value being indexed.
        /// </summary>
        public TValue Value
        {
            get { return _value; }
            internal set { _value = value; }
        }

        /// <summary>
        /// The extents of the <see cref="Value"/>.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get { return _boundingBox; }
            internal set { _boundingBox = value; }
        }

        public override String ToString()
        {
            return String.Format("[{0}] Value: {1}; BoundingBox: {2}", 
                GetType(), Value, BoundingBox);
        }
    }
}