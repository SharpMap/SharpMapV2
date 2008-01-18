// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.SimpleGeometries
{
	/// <summary>
	/// A MultiPoint is a 0 dimensional geometric collection. 
	/// The elements of a MultiPoint are restricted to Points. 
	/// The points are not connected or ordered.
	/// </summary>
	public class MultiPoint : GeometryCollection<Point>
	{
		/// <summary>
		/// Initializes a new MultiPoint collection
		/// </summary>
		public MultiPoint() { }

		public MultiPoint(Int32 initialCapacity)
            : base(initialCapacity) { }

		/// <summary>
		/// Gets or sets the MultiPoint collection
		/// </summary>
		public IList<Point> Points
		{
			get { return Collection; }
		}

		/// <summary>
		///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
		/// </summary>
		public override Int32 Dimension
		{
			get { return 0; }
		}

		/// <summary>
		/// The boundary of a MultiPoint is the empty set (null).
		/// </summary>
		/// <returns></returns>
		public override Geometry Boundary()
		{
			return null;
		}

		/// <summary>
		/// Creates a copy of this geometry.
		/// </summary>
		/// <returns>Copy of the MultiPoint.</returns>
		public override Geometry Clone()
		{
			MultiPoint multiPoint = new MultiPoint();

			foreach (Point p in Points)
			{
				multiPoint.Points.Add(p.Clone() as Point);
			}

			return multiPoint;
		}
	}
}