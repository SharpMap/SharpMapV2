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
using GeoAPI.Geometries;
using GeoAPI.Utilities;
using GeoAPI.Coordinates;
using NPack;
using NPack.Interfaces;

namespace SharpMap.SimpleGeometries
{
    /// <summary>
    /// A LineString is a Curve with linear interpolation between points. 
    /// Each consecutive pair of points defines a line segment.
    /// </summary>
    [Serializable]
    public class LineString : Curve, ILineString
    {
        private readonly List<ICoordinate> _vertices = new List<ICoordinate>();

        /// <summary>
        /// Initializes an instance of a LineString from a set of vertices.
        /// </summary>
        /// <param name="vertices"></param>
        public LineString(IEnumerable<IPoint> vertices)
        {
            foreach (IPoint p in vertices)
            {
                _vertices.Add(p.Coordinate);
            }
        }

        /// <summary>
        /// Initializes an instance of a LineString from a set of vertices.
        /// </summary>
        /// <param name="vertices"></param>
        public LineString(IEnumerable<ICoordinate> vertices)
        {
            foreach (ICoordinate p in vertices)
            {
                _vertices.Add(p);
            }
        }

        /// <summary>
        /// Initializes an instance of a LineString.
        /// </summary>
        public LineString()
            : this(new ICoordinate[0])
        {
        }

        #region OpenGIS Methods

        /// <summary>
        /// Returns the specified point N in this Linestring.
        /// </summary>
        /// <remarks>This method is supplied as part of the OpenGIS Simple Features Specification</remarks>
        /// <param name="index"></param>
        /// <returns></returns>
        public IPoint GetPoint(Int32 index)
        {
            return new Point(Coordinates[index]);
        }

        /// <summary>
        /// The number of points in this LineString.
        /// </summary>
        /// <remarks>This method is supplied as part of the OpenGIS Simple Features Specification</remarks>
        public override Int32 PointCount
        {
            get { return _vertices == null ? 0 : _vertices.Count; }
        }

        #endregion

        #region "Inherited methods from abstract class Geometry"

        /// <summary>
        /// Checks whether this instance is spatially equal to the LineString 'l'
        /// </summary>
        /// <param name="l">LineString to compare to</param>
        /// <returns>true of the objects are spatially equal</returns>
        public Boolean Equals(LineString l)
        {
            if (ReferenceEquals(l, null))
            {
                return false;
            }
            if (l.PointCount != PointCount)
            {
                return false;
            }
            for (Int32 i = 0; i < l.PointCount; i++)
            {
                if (!l._vertices[i].Equals(_vertices[i]))
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
        public override Int32 GetHashCode()
        {
            Int32 hash = 0;

            for (Int32 i = 0; i < PointCount; i++)
            {
                hash = hash ^ Coordinates[i].GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override Boolean IsEmpty
        {
            get
            {
                return _vertices == null || _vertices.Count == 0;
            }
        }

        /// <summary>
        ///  Returns 'true' if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public override Boolean IsSimple
        {
            get
            {
                List<ICoordinate> verts = new List<ICoordinate>(_vertices.Count);

                for (Int32 i = 0; i < _vertices.Count; i++)
                {
                    if (!verts.Exists(delegate(ICoordinate p) { return p.Equals(_vertices[i]); }))
                    {
                        verts.Add(Coordinates[i]);
                    }
                }

                return (verts.Count == _vertices.Count - (IsClosed ? 1 : 0));
            }
        }

        /// <summary>
        /// Returns the closure of the combinatorial boundary of this Geometry. The
        /// combinatorial boundary is defined as described in section 3.12.3.2 of [1]. Because the result of this function
        /// is a closure, and hence topologically closed, the resulting boundary can be represented using
        /// representational geometry primitives
        /// </summary>
        /// <returns>Closure of the combinatorial boundary of this Geometry</returns>
        public override IGeometry Boundary
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Dimensions BoundaryDimension
        {
            get { return Dimensions.Point; }
        }

        public override IPoint Centroid
        {
            get { return FactoryInternal.CreatePoint(ExtentsInternal.Center); }
        }

        /// <summary>
        /// Returns the shortest distance between any two points in the two geometries
        /// as calculated in the spatial reference system of this Geometry.
        /// </summary>
        /// <param name="geom">Geometry to calculate distance to</param>
        /// <returns>Shortest distance between any two points in the two geometries</returns>
        public override Double Distance(IGeometry geom)
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
        public override IGeometry Buffer(Double d)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>The convex hull</returns>
        public override IGeometry ConvexHull()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set intersection of this Geometry
        /// with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to intersect with</param>
        /// <returns>Returns a geometry that represents the point set intersection of this Geometry with anotherGeometry.</returns>
        public override IGeometry Intersection(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to union with</param>
        /// <returns>Unioned geometry</returns>
        public override IGeometry Union(IGeometry geom)
        {
#warning fake the union using a GeometryCollection
            return FactoryInternal.SpatialOps.Union(this, geom as Geometry);
        }

        /// <summary>
        /// Returns a geometry that represents the point set difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override IGeometry Difference(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override IGeometry SymmetricDifference(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Returns the vertex where this Geometry begins.
        /// </summary>
        /// <returns>First vertex in this <see cref="LineString"/>.</returns>
        public override IPoint StartPoint
        {
            get
            {
                if (_vertices.Count == 0)
                {
                    throw new InvalidOperationException("No starting point found: LineString has no vertices.");
                }

                return new Point(Coordinates[0]);
            }
        }

        /// <summary>
        /// Gets the vertex where this Geometry ends.
        /// </summary>
        /// <returns>Last vertex in this <see cref="LineString"/>.</returns>
        public override IPoint EndPoint
        {
            get
            {
                if (_vertices.Count == 0)
                {
                    throw new InvalidOperationException("No endpoint found: LineString has no vertices.");
                }

                return new Point(Coordinates[PointCount - 1]);
            }
        }

        /// <summary>
        /// Returns true if this <see cref="LineString"/> is closed and simple.
        /// </summary>
        public override Boolean IsRing
        {
            get { return (IsClosed && IsSimple); }
        }

        /// <summary>
        /// The length of this <see cref="LineString"/>, as measured in the spatial reference 
        /// system of this LineString.
        /// </summary>
        public override Double Length
        {
            get
            {
                if (PointCount < 2)
                {
                    return 0;
                }

                Double sum = 0;

                for (Int32 i = 1; i < PointCount; i++)
                {
                    sum += Coordinates[i].Distance(Coordinates[i - 1]);
                }

                return sum;
            }
        }

        /// <summary>
        /// The position of a point on the line, parameterised by length.
        /// </summary>
        /// <param name="t">Distance down the line.</param>
        /// <returns>Point at line at distance t from <see cref="StartPoint"/></returns>
        public override IPoint Value(Double t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns>BoundingBox for this geometry.</returns>
        public override IExtents Extents
        {
            get
            {
                Extents bbox = new Extents(FactoryInternal);

                if (PointCount == 0)
                {
                    return bbox;
                }

                foreach (ICoordinate p in _vertices)
                {
                    bbox.ExpandToInclude(p);
                }

                return bbox;
            }
        }

        public override IEnumerable<Point> GetVertexes()
        {
            foreach (ICoordinate coordinate in _vertices)
            {
                yield return (Point)FactoryInternal.CreatePoint(coordinate);
            }
        }

        public override IEnumerable<Point> GetVertexes(ITransformMatrix<DoubleComponent> transform)
        {
            throw new NotImplementedException();
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a deep copy of the LineString.
        /// </summary>
        /// <returns>A copy of the LineString instance.</returns>
        public override Geometry Clone()
        {
            return FactoryInternal.CreateLineString(_vertices) as Geometry;
        }

        #endregion

        #region ILineString Members

        public ILineString Reverse()
        {
            return new LineString(Slice.Reverse(_vertices));
        }

        #endregion

        public override OgcGeometryType GeometryType
        {
            get { return OgcGeometryType.LineString; }
        }
    }
}