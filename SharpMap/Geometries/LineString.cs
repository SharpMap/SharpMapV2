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
using System.ComponentModel;

namespace SharpMap.Geometries
{
	/// <summary>
	/// A LineString is a Curve with linear interpolation between points. 
	/// Each consecutive pair of points defines a line segment.
	/// </summary>
	[Serializable]
	public class LineString : Curve
	{
        private readonly BindingList<Point> _vertices = new BindingList<Point>();
	    private BoundingBox _bounds = BoundingBox.Empty;

		/// <summary>
		/// Initializes an instance of a LineString from a set of vertices.
		/// </summary>
		/// <param name="vertices"></param>
		public LineString(IEnumerable<Point> vertices)
		{
            _vertices.ListChanged += handleVertexesChanged;

			foreach (Point p in vertices)
			{
				_vertices.Add(p.Clone() as Point);
			}
		}

		/// <summary>
		/// Initializes an instance of a LineString.
		/// </summary>
		public LineString() : this(new Point[0])
		{
		}

		/// <summary>
		/// Gets or sets the collection of vertices in this Geometry.
		/// </summary>
		public virtual IList<Point> Vertices
		{
			get { return _vertices; }
		}

		#region OpenGIS Methods

		/// <summary>
		/// Returns the specified point N in this Linestring.
		/// </summary>
		/// <remarks>This method is supplied as part of the OpenGIS Simple Features Specification</remarks>
		/// <param name="N"></param>
		/// <returns></returns>
		public Point Point(int N)
		{
			return _vertices[N];
		}

		/// <summary>
		/// The number of points in this LineString.
		/// </summary>
		/// <remarks>This method is supplied as part of the OpenGIS Simple Features Specification</remarks>
		public virtual int NumPoints
		{
			get { return _vertices.Count; }
		}

		#endregion

		#region "Inherited methods from abstract class Geometry"

		/// <summary>
		/// Checks whether this instance is spatially equal to the LineString 'l'
		/// </summary>
		/// <param name="l">LineString to compare to</param>
		/// <returns>true of the objects are spatially equal</returns>
		public bool Equals(LineString l)
		{
			if (ReferenceEquals(l, null))
			{
				return false;
			}
			if (l.Vertices.Count != Vertices.Count)
			{
				return false;
			}
			for (int i = 0; i < l.Vertices.Count; i++)
			{
				if (!l.Vertices[i].Equals(Vertices[i]))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
		/// in hashing algorithms and data structures like a hash table.
		/// </summary>
		/// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
		public override int GetHashCode()
		{
			int hash = 0;

			for (int i = 0; i < Vertices.Count; i++)
			{
				hash = hash ^ Vertices[i].GetHashCode();
			}

			return hash;
		}

		/// <summary>
		/// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
		/// </summary>
		/// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
		public override bool IsEmpty()
		{
			return _vertices == null || _vertices.Count == 0;
		}

		/// <summary>
		///  Returns 'true' if this Geometry has no anomalous geometric points, such as self
		/// intersection or self tangency. The description of each instantiable geometric class will include the specific
		/// conditions that cause an instance of that class to be classified as not simple.
		/// </summary>
		/// <returns>true if the geometry is simple</returns>
		public override bool IsSimple()
		{
			List<Point> verts = new List<Point>(_vertices.Count);
			for (int i = 0; i < _vertices.Count; i++)
				if (!verts.Exists(delegate(Point p) { return p.Equals(_vertices[i]); }))
					verts.Add(_vertices[i]);
			return (verts.Count == _vertices.Count - (IsClosed ? 1 : 0));
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
#warning fake the union using a GeometryCollection
		    return FakeSpatialOperations.Union(this, geom);
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

		#endregion

		/// <summary>
		/// Returns the vertex where this Geometry begins.
		/// </summary>
		/// <returns>First vertex in this <see cref="LineString"/>.</returns>
		public override Point StartPoint
		{
			get
			{
				if (_vertices.Count == 0)
				{
					throw new InvalidOperationException("No starting point found: LineString has no vertices.");
				}

				return _vertices[0];
			}
		}

		/// <summary>
		/// Gets the vertex where this Geometry ends.
		/// </summary>
		/// <returns>Last vertex in this <see cref="LineString"/>.</returns>
		public override Point EndPoint
		{
			get
			{
				if (_vertices.Count == 0)
				{
					throw new InvalidOperationException("No endpoint found: LineString has no vertices.");
				}

				return _vertices[_vertices.Count - 1];
			}
		}

		/// <summary>
		/// Returns true if this <see cref="LineString"/> is closed and simple.
		/// </summary>
		public override bool IsRing
		{
			get { return (IsClosed && IsSimple()); }
		}

		/// <summary>
		/// The length of this <see cref="LineString"/>, as measured in the spatial reference 
		/// system of this LineString.
		/// </summary>
		public override double Length
		{
			get
			{
				if (Vertices.Count < 2)
				{
					return 0;
				}

				double sum = 0;

				for (int i = 1; i < Vertices.Count; i++)
				{
					sum += Vertices[i].Distance(Vertices[i - 1]);
				}

				return sum;
			}
		}

		/// <summary>
		/// The position of a point on the line, parameterised by length.
		/// </summary>
		/// <param name="t">Distance down the line.</param>
		/// <returns>Point at line at distance t from <see cref="StartPoint"/></returns>
		public override Point Value(double t)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// The minimum bounding box for this Geometry.
		/// </summary>
		/// <returns>BoundingBox for this geometry.</returns>
		public override BoundingBox GetBoundingBox()
		{
			BoundingBox bbox = BoundingBox.Empty;

			if (Vertices == null || Vertices.Count == 0)
			{
				return bbox;
			}

			foreach (Point p in Vertices)
			{
				bbox.ExpandToInclude(p);
			}

			return bbox;
		}

        public override IEnumerable<Point> GetVertices()
        {
            foreach (Point point in _vertices)
            {
                yield return point;
            }
        }

		#region ICloneable Members

		/// <summary>
		/// Creates a deep copy of the LineString.
		/// </summary>
		/// <returns>A copy of the LineString instance.</returns>
		public override Geometry Clone()
		{
			LineString l = new LineString();

			for (int i = 0; i < _vertices.Count; i++)
			{
				l.Vertices.Add(_vertices[i].Clone() as Point);
			}

			return l;
		}

		#endregion

        private void handleVertexesChanged(object sender, ListChangedEventArgs args)
        {
            switch (args.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    _bounds.ExpandToInclude(_vertices[args.NewIndex]);
                    break;
                case ListChangedType.ItemDeleted:
                case ListChangedType.Reset:
                    _bounds = BoundingBox.Empty;
                    foreach (Point point in _vertices)
                    {
                        _bounds.ExpandToInclude(point);
                    }
                    break;
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemMoved:
                case ListChangedType.PropertyDescriptorAdded:
                case ListChangedType.PropertyDescriptorChanged:
                case ListChangedType.PropertyDescriptorDeleted:
                default:
                    break;
            }
        }
	}
}