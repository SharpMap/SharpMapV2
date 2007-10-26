// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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

namespace SharpMap.Geometries
{
	/// <summary>
	/// A generalized GeometryCollection which allows any geometry type to be used.
	/// </summary>
	/// <remarks>
    /// <para>
	/// All the elements in a GeometryCollection must be in the same 
    /// spatial reference system. This is also the spatial
	/// reference for the GeometryCollection.
    /// </para>
    /// <para>
	/// GeometryCollection places no other constraints on its elements. 
    /// Subclasses of GeometryCollection may
	/// restrict membership based on dimension or
    /// other topological constraints, such as degree of spatial overlap 
    /// between elements.
    /// </para>
	/// </remarks>
    public class GeometryCollection : GeometryCollection<Geometry>
	{

        /// <summary>
        /// Initializes a new GeometryCollection.
        /// </summary>
        public GeometryCollection() { }

		/// <summary>
		/// Initializes a new GeometryCollection with the given initial capacity.
		/// </summary>
		public GeometryCollection(Int32 initialCapacity)
            : base(initialCapacity) { }
	}
}