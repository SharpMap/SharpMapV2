// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using GeoAPI.IO.WellKnownBinary;
using GeoAPI.IO.WellKnownText;
using NPack.Interfaces;
using NPack;
using GeoAPI.Operations.Buffer;

namespace SharpMap.SimpleGeometries
{
    /// <summary>
    /// Represents an geometrical entity in a defined Cartesian space.
    /// The root class of the Geometry object Model hierarchy.
    /// <see cref="Geometry"/> is an abstract (non-instantiable) class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The instantiable subclasses of <see cref="Geometry"/> 
    /// defined in the specification are restricted to 0, 
    /// 1 and 2 dimensional geometric objects that exist in 
    /// two-dimensional Cartesian space (R<sup>2</sup>).
    /// </para>
    /// <para>
    /// All instantiable geometry classes described in this 
    /// specification are defined so that valid instances of a
    /// geometry class are topologically closed 
    /// (i.e. all defined geometries include their boundary).
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class Geometry : IGeometry, IEquatable<Geometry>, IVertexStream<Point, DoubleComponent>
    {
        private ICoordinateSystem _spatialReference;
        private Int32? _srid;
        private Tolerance _tolerance = Tolerance.Global;
        private Extents? _extents;
        private GeometryFactory _factory;
        private ICoordinateSequence _coordinates;
        private Object _userData;

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override Int32 GetHashCode()
        {
            Int32 hashCode = GetType().GetHashCode();

            foreach (Point point in GetVertexes())
            {
                hashCode ^= point.X.GetHashCode() ^ point.Y.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Returns a WellKnownText representation of the <see cref="Geometry"/>
        /// </summary>
        /// <returns>Well-known text</returns>
        public override String ToString()
        {
            return AsText();
        }

        /// <summary>
        /// Gets or sets the spatial reference system associated 
        /// with the <see cref="Geometry"/>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Geometry"/> may not have had a spatial 
        /// reference system defined for
        /// it, in which case SpatialReference will be <see langword="null"/>.
        /// </remarks>
        public ICoordinateSystem SpatialReference
        {
            get { return _spatialReference; }
            set { _spatialReference = value; }
        }

        /// <summary>
        /// Gets or sets the tolerance used in comparisons with a 
        /// <see cref="Geometry"/> instance.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Tolerance.Global"/>. 
        /// If the value of this property is explicitly set,
        /// that value is used, on an instance by instance basis, 
        /// until it is set to null, which will
        /// allow the Geometry instance to participate 
        /// in the global setting.
        /// </remarks>
        public Tolerance Tolerance
        {
            get
            {
                return _tolerance;
            }
            set { _tolerance = value; }
        }

        // The following are methods that should be implemented on a
        // geometry Object according to the OpenGIS Simple Features Specification


        #region IGeometry Members

        public abstract Dimensions BoundaryDimension { get; }

        public abstract IPoint Centroid { get; }

        IGeometry IGeometry.Clone()
        {
            return Clone();
        }

        public ICoordinateSequence Coordinates
        {
            get { return _coordinates; }
            internal set { _coordinates = value; }
        }

        public IGeometryFactory Factory
        {
            get { return _factory; }
            internal set { _factory = (GeometryFactory)value; }
        }

        public abstract OgcGeometryType GeometryType { get; }

        public string GeometryTypeName
        {
            get { return GeometryType.ToString(); }
        }

        public Boolean IsRectangle
        {
            get { throw new NotImplementedException(); }
        }

        public Boolean IsValid
        {
            get { throw new NotImplementedException(); }
        }

        public void Normalize()
        {
            throw new NotImplementedException();
        }

        public abstract Int32 PointCount { get; }

        public IPrecisionModel PrecisionModel
        {
            get { throw new NotSupportedException(); }
        }

        public Int32? Srid
        {
            get { return _srid; }
        }

        public Object UserData
        {
            get
            {
                return _userData;
            }
            set
            {
                _userData = value;
            }
        }

        /// <summary>
        /// The inherent dimension of this <see cref="Geometry"/> object, 
        /// which must be less than or equal to the coordinate dimension.
        /// </summary>
        /// <remarks>
        /// This specification is restricted to geometries in 
        /// two-dimensional coordinate space.
        /// </remarks>
        public abstract Dimensions Dimension { get; }

        /// <summary>
        /// The minimum bounding box for this <see cref="Geometry"/>, 
        /// returned as a <see cref="Geometry"/>.
        /// </summary>
        /// <remarks>
        /// The envelope is actually the <see cref="IExtents"/> converted into a 
        /// polygon. The polygon is defined by the corner points of the bounding 
        /// box ((MINX, MINY), (MAXX, MINY), (MAXX, MAXY), (MINX, MAXY), (MINX, MINY)).
        /// </remarks>
        /// <seealso cref="Extents"/>
        public IGeometry Envelope
        {
            get
            {
                Extents box = ExtentsInternal;
                IEnumerable<ICoordinate> coordinates = getBoundCoordinates(box);

                if (_factory == null)
                {
                    throw new InvalidOperationException(
                        "No IGeometryFactory has been set for this geometry.");
                }

                Polygon envelope = FactoryInternal.CreatePolygon(coordinates) as Polygon;
                return envelope;
            }
        }

        private IEnumerable<ICoordinate> getBoundCoordinates(Extents box)
        {
            yield return Factory.CoordinateFactory.Create(box.XMin, box.YMin);    //minx miny
            yield return Factory.CoordinateFactory.Create(box.XMax, box.YMin);   //maxx miny
            yield return Factory.CoordinateFactory.Create(box.XMax, box.YMax);      //maxx maxy
            yield return Factory.CoordinateFactory.Create(box.XMin, box.YMax);       //minx maxy
            yield return Factory.CoordinateFactory.Create(box.XMin, box.YMin);    //close ring
        }

        /// <summary>
        /// Exports this <see cref="Geometry"/> to a specific 
        /// well-known text representation of <see cref="Geometry"/>.
        /// </summary>
        public String AsText()
        {
            return WktWriter.ToWkt(this);
        }

        /// <summary>
        /// Exports this <see cref="Geometry"/> to a specific 
        /// well-known binary representation of <see cref="Geometry"/>.
        /// </summary>
        public Byte[] AsBinary()
        {
            return WkbWriter.ToWkb(this);
        }

        /// <summary>
        /// The minimum bounding box for this <see cref="Geometry"/>, returned as an <see cref="IExtents"/>.
        /// </summary>
        /// <returns></returns>
        public abstract IExtents Extents { get; }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> is the empty geometry . If true, then this
        /// <see cref="Geometry"/> represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        public abstract Boolean IsEmpty { get; }

        /// <summary>
        ///  Returns <see langword="true" /> if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        public abstract Boolean IsSimple { get; }

        #endregion

        #region ISpatialRelation Members

        public Boolean Contains(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> ‘spatially contains’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Contains(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Contains(this, g);
        }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> ‘spatially crosses’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Crosses(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Crosses(this, g);
        }

        public Boolean Covers(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean Covers(IGeometry g)
        {
            throw new NotImplementedException();
        }

        public Boolean Crosses(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns <see langword="true" /> if this Geometry is ‘spatially disjoint’ from another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Disjoint(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Disjoint(this, g);
        }

        public Boolean Disjoint(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean Equals(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        #region IEquatable<IGeometry> Members

        public Boolean Equals(IGeometry other)
        {
            if (ReferenceEquals(other, this))
            {
                return true;
            }

            if (GeometryType != other.GeometryType)
            {
                return false;
            }

            if (!ExtentsInternal.Equals(other.Extents))
            {
                return false;
            }

            return EqualsInternal(other);
        }

        #endregion

        protected abstract Boolean EqualsInternal(IGeometry other);

        #region IEquatable<Geometry> Members

        /// <summary>
        /// Returns <see langword="true" /> if this Geometry is 'spatially equal' to another Geometry.
        /// </summary>
        public virtual Boolean Equals(Geometry other)
        {
            return BoundingBoxSpatialRelations.Equals(this, other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        /// Returns <see langword="true" /> if this Geometry is 'spatially equal' to another Geometry.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        /// <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false</returns>
        public override Boolean Equals(Object obj)
        {
            Geometry g = obj as Geometry;

            if (ReferenceEquals(g, null))
            {
                return false;
            }
            else
            {
                return Equals(g);
            }
        }

        /// <summary>
        /// Returns <see langword="true" /> if <paramref name="g1"/> is 'spatially equal' to 
        /// <paramref name="g2"/>.
        /// </summary>
        /// <param name="g1">First geometry to compare.</param>
        /// <param name="g2">Second geometry to compare.</param>
        /// <returns>
        /// True if the two <see cref="Geometry"/> instances are equal, 
        /// false otherwise.
        /// </returns>
        public static Boolean operator ==(Geometry g1, Geometry g2)
        {
            if (ReferenceEquals(g1, g2))
            {
                return true;
            }

            if (!ReferenceEquals(g1, null))
            {
                return g1.Equals(g2);
            }
            else
            {
                return g2.Equals(g1);
            }
        }

        /// <summary>
        /// Returns <see langword="true" /> if <paramref name="g1"/> is not 
        /// 'spatially equal' to <paramref name="g2"/>.
        /// </summary>
        /// <param name="g1">First geometry to compare.</param>
        /// <param name="g2">Second geometry to compare.</param>
        /// <returns>
        /// True if the two <see cref="Geometry"/> instances are not equal, 
        /// false otherwise.
        /// </returns>
        public static Boolean operator !=(Geometry g1, Geometry g2)
        {
            if (ReferenceEquals(g1, g2))
            {
                return false;
            }

            if (!ReferenceEquals(g1, null))
            {
                return !g1.Equals(g2);
            }
            else
            {
                return !g2.Equals(g1);
            }
        }

        #endregion

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> ‘spatially intersects’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Intersects(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Intersects(this, g);
        }

        public Boolean Intersects(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean IsCoveredBy(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean IsCoveredBy(IGeometry g)
        {
            throw new NotImplementedException();
        }

        public Boolean IsWithinDistance(IGeometry g, Double distance, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean IsWithinDistance(IGeometry g, Double distance)
        {
            throw new NotImplementedException();
        }

        public Boolean Overlaps(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> 'spatially overlaps' another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Overlaps(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Overlaps(this, g);
        }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> is spatially related to 
        /// another <see cref="Geometry"/>, by testing
        /// for intersections between the Interior, Boundary and Exterior of the two geometries
        /// as specified by the values in the intersectionPatternMatrix
        /// </summary>
        /// <param name="other"><see cref="Geometry"/> to relate to</param>
        /// <param name="intersectionPattern">Intersection Pattern</param>
        /// <returns>True if spatially related</returns>
        public Boolean Relate(IGeometry other, String intersectionPattern)
        {
            throw new NotImplementedException();
        }

        public IntersectionMatrix Relate(IGeometry g)
        {
            throw new NotImplementedException();
        }

        public Boolean Relate(IGeometry g, string intersectionPattern, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean Relate(IGeometry g, IntersectionMatrix intersectionPattern, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean Relate(IGeometry g, IntersectionMatrix intersectionPattern)
        {
            throw new NotImplementedException();
        }

        public Boolean Touches(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> ‘spatially touches’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Touches(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Touches(this, g);
        }

        /// <summary>
        /// Returns <see langword="true" /> if this <see cref="Geometry"/> is ‘spatially within’ another <see cref="Geometry"/>.
        /// </summary>
        public virtual Boolean Within(IGeometry geom)
        {
            Geometry g = checkParameterType(geom);

            return BoundingBoxSpatialRelations.Within(this, g);
        }

        public Boolean Within(IGeometry g, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region "Methods that support Spatial Analysis"

        /// <summary>
        /// Returns the closure of the combinatorial boundary of this <see cref="Geometry"/>. The
        /// combinatorial boundary is defined as described in section 3.12.3.2 of [1]. Because the result of this function
        /// is a closure, and hence topologically closed, the resulting boundary can be represented using
        /// representational geometry primitives
        /// </summary>
        public abstract IGeometry Boundary { get; }

        public IGeometry Buffer(Double distance, Int32 quadrantSegments, GeoAPI.Operations.Buffer.BufferStyle endCapStyle)
        {
            throw new NotImplementedException();
        }

        public IGeometry Buffer(Double distance, BufferStyle endCapStyle)
        {
            throw new NotImplementedException();
        }

        public IGeometry Buffer(Double distance, Int32 quadrantSegments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents all points 
        /// whose distance from this Geometry
        /// is less than or equal to distance. 
        /// Calculations are in the Spatial Reference
        /// System of this Geometry.
        /// </summary>
        /// <param name="d">Buffer distance</param>
        public abstract IGeometry Buffer(Double d);

        /// <summary>
        /// Returns a geometry that represents 
        /// the convex hull of this Geometry.
        /// </summary>
        public abstract IGeometry ConvexHull();

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// difference of this Geometry with another Geometry.
        /// </summary>
        public abstract IGeometry Difference(IGeometry geometry);

        /// <summary>
        /// Returns the shortest distance between any 
        /// two points in the two geometries
        /// as calculated in the spatial reference 
        /// system of this Geometry.
        /// </summary>
        public abstract Double Distance(IGeometry geometry);

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// intersection of this <see cref="Geometry"/>
        /// with the given <paramref name="geometry"/>.
        /// </summary>
        public abstract IGeometry Intersection(IGeometry geometry);

        /// <summary>
        /// Returns a geometry that represents the point set 
        /// symmetric difference of this Geometry with another Geometry.
        /// </summary>
        public abstract IGeometry SymmetricDifference(IGeometry geometry);

        /// <summary>
        /// Returns a geometry that represents the point set union 
        /// of this Geometry with another Geometry.
        /// </summary>
        public abstract IGeometry Union(IGeometry geometry);

        #endregion

        /// <summary>
        /// Creates a deep copy of the Geometry instance.
        /// </summary>
        /// <returns>Copy of Geometry</returns>
        public abstract Geometry Clone();

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable Members

        public Int32 CompareTo(Object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected internal virtual Extents ExtentsInternal
        {
            get
            {
                if (_extents == null)
                {
                    _extents = (Extents)Extents;
                }

                return _extents.Value;
            }
        }

        protected internal GeometryFactory FactoryInternal
        {
            get { return _factory; }
        }

        ///// <summary>
        ///// Creates a <see cref="Geometry"/> based on a Well-Known Text String
        ///// </summary>
        ///// <param name="wkt">Well-Known Text</param>
        ///// <returns></returns>
        //public static Geometry FromText(String wkt)
        //{
        //    return GeometryFromWkt.Parse(wkt);
        //}

        ///// <summary>
        ///// Creates a <see cref="Geometry"/> based on a Well-Known Binary Byte array
        ///// </summary>
        ///// <param name="wkb">Well-Known Binary</param>
        ///// <returns></returns>
        //public static Geometry FromWkb(Byte[] wkb)
        //{
        //    return GeometryFromWkb.Parse(wkb);
        //}

        #region IVertexStream<Point,DoubleComponent> Members

        public abstract IEnumerable<Point> GetVertexes(ITransformMatrix<DoubleComponent> transform);

        public abstract IEnumerable<Point> GetVertexes();

        #endregion


        private static Geometry checkParameterType(IGeometry geom)
        {
            if (!(geom is Geometry))
            {
                throw new ArgumentException(
                    "Parameter must be a SharpMap.SimpleGeometries.Geometry instance");
            }

            return geom as Geometry;
        }
    }
}