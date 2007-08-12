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
using System.Collections.Generic;

namespace SharpMap.Geometries
{
	/// <summary>
	/// A MultiLineString is a MultiCurve whose elements are LineStrings.
	/// </summary>
	[Serializable]
	public class MultiLineString : MultiCurve
	{
		private readonly List<LineString> _lineStrings;

		/// <summary>
		/// Initializes an instance of a MultiLineString
		/// </summary>
		public MultiLineString()
			: this(8)
		{
		}

		public MultiLineString(int initialCapacity)
		{
			_lineStrings = new List<LineString>(initialCapacity);
		}

		/// <summary>
		/// Collection of <see cref="LineString">LineStrings</see> in the <see cref="MultiLineString"/>
		/// </summary>
		public List<LineString> LineStrings
		{
			get { return _lineStrings; }
		}

		/// <summary>
		/// Returns an indexed geometry in the collection
		/// </summary>
		/// <param name="index">Geometry index</param>
		/// <returns>Geometry at index</returns>
		public new LineString this[int index]
		{
			get { return _lineStrings[index]; }
		}

		/// <summary>
		/// Returns true if all LineStrings in this MultiLineString is closed (StartPoint=EndPoint for each LineString in this MultiLineString)
		/// </summary>
		public override bool IsClosed
		{
			get
			{
				foreach (LineString lineString in _lineStrings)
					if (!lineString.IsClosed)
						return false;

				return true;
			}
		}

		/// <summary>
		/// The length of this MultiLineString which is equal to the sum of the lengths of the element LineStrings.
		/// </summary>
		public override double Length
		{
			get
			{
				double l = 0;

				foreach (LineString lineString in _lineStrings)
					l += lineString.Length;

				return l;
			}
		}

		/// <summary>
		/// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
		/// </summary>
		/// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
		public override bool IsEmpty()
		{
			if (_lineStrings == null || _lineStrings.Count == 0)
				return true;

			foreach (LineString lineString in _lineStrings)
				if (!lineString.IsEmpty())
					return false;

			return true;
		}

		/// <summary>
		///  Returns 'true' if this Geometry has no anomalous geometric points, such as self
		/// intersection or self tangency. The description of each instantiable geometric class will include the specific
		/// conditions that cause an instance of that class to be classified as not simple.
		/// </summary>
		/// <returns>true if the geometry is simple</returns>
		public override bool IsSimple()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the closure of the combinatorial boundary of this Geometry. The
		/// combinatorial boundary is defined as described in section 3.12.3.2 of [1]. Because the result of this function
		/// is a closure, and hence topologically closed, the resulting boundary can be represented using
		/// representational geometry primitives
		/// </summary>
		/// <returns>Closure of the combinatorial boundary of this Geometry</returns>
		public override Geometry Boundary()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the shortest distance between any two points in the two geometries
		/// as calculated in the spatial reference system of this Geometry.
		/// </summary>
		/// <param name="geom">Geometry to calculate distance to</param>
		/// <returns>Shortest distance between any two points in the two geometries</returns>
		public override double Distance(Geometry geom)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a geometry that represents all points whose distance from this Geometry
		/// is less than or equal to distance. Calculations are in the Spatial Reference
		/// System of this Geometry.
		/// </summary>
		/// <param name="d">Buffer distance</param>
		/// <returns>Buffer around geometry</returns>
		public override Geometry Buffer(double d)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Geometry—Returns a geometry that represents the convex hull of this Geometry.
		/// </summary>
		/// <returns>The convex hull</returns>
		public override Geometry ConvexHull()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a geometry that represents the point set intersection of this Geometry
		/// with anotherGeometry.
		/// </summary>
		/// <param name="geom">Geometry to intersect with</param>
		/// <returns>Returns a geometry that represents the point set intersection of this Geometry with anotherGeometry.</returns>
		public override Geometry Intersection(Geometry geom)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a geometry that represents the point set union of this Geometry with anotherGeometry.
		/// </summary>
		/// <param name="geom">Geometry to union with</param>
		/// <returns>Unioned geometry</returns>
		public override Geometry Union(Geometry geom)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a geometry that represents the point set difference of this Geometry with anotherGeometry.
		/// </summary>
		/// <param name="geom">Geometry to compare to</param>
		/// <returns>Geometry</returns>
		public override Geometry Difference(Geometry geom)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a geometry that represents the point set symmetric difference of this Geometry with anotherGeometry.
		/// </summary>
		/// <param name="geom">Geometry to compare to</param>
		/// <returns>Geometry</returns>
		public override Geometry SymDifference(Geometry geom)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the number of geometries in the collection.
		/// </summary>
		public override int NumGeometries
		{
			get { return _lineStrings.Count; }
		}

		/// <summary>
		/// Returns an indexed geometry in the collection
		/// </summary>
		/// <param name="N">Geometry index</param>
		/// <returns>Geometry at index N</returns>
		public override Geometry Geometry(int N)
		{
			return _lineStrings[N];
		}

		/// <summary>
		/// The minimum bounding box for this Geometry.
		/// </summary>
		/// <returns></returns>
		public override BoundingBox GetBoundingBox()
		{
			BoundingBox bbox = BoundingBox.Empty;

			if (_lineStrings == null || _lineStrings.Count == 0)
			{
				return bbox;
			}

			foreach (LineString lineString in _lineStrings)
			{
				bbox.ExpandToInclude(lineString.GetBoundingBox());
			}

			return bbox;
		}

		/// <summary>
		/// Return a copy of this geometry
		/// </summary>
		/// <returns>Copy of Geometry</returns>
		public new MultiLineString Clone()
		{
			MultiLineString geoms = new MultiLineString();

			foreach (LineString lineString in _lineStrings)
			{
				geoms.LineStrings.Add(lineString.Clone() as LineString);
			}

			return geoms;
		}

		#region IEnumerable<Geometry> Members

		/// <summary>
		/// Gets an enumerator for enumerating the geometries in the GeometryCollection
		/// </summary>
		/// <returns></returns>
		public override IEnumerator<Geometry> GetEnumerator()
		{
			foreach (LineString l in _lineStrings)
			{
				yield return l;
			}
		}

		#endregion
	}
}