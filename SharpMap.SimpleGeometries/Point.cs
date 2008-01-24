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
using GeoAPI.Geometries;
using NPack;
using NPack.Interfaces;
using GeoAPI.Coordinates;

namespace SharpMap.SimpleGeometries
{
    /// <summary>
    /// A Point is a 0-dimensional geometry and represents a single location in 2D coordinate space. 
    /// A Point has an x coordinate value and a y-coordinate value. 
    /// The boundary of a Point is the empty set.
    /// </summary>
    [Serializable]
    public class Point : Geometry, IPoint, IComparable<Point>, IVector<DoubleComponent>
    {
        private static readonly Point _empty = new Point();
        private static readonly Point _zero = new Point(0, 0);
        private static readonly Point _one = new Point(1, 1);

        private Double _x = 0.0;
        private Double _y = 0.0;
        private Boolean _hasValue = false;

        /// <summary>
        /// Initializes a new Point
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        public Point(Double x, Double y)
        {
            _x = x;
            _y = y;
            SetNotEmpty();
        }

        /// <summary>
        /// Initializes a new Point
        /// </summary>
        /// <param name="coordinate">The coordinate used to define the point.</param>
        public Point(ICoordinate coordinate)
        {
            _x = coordinate[Ordinates.X];
            _y = coordinate[Ordinates.Y];
            SetNotEmpty();
        }

        /// <summary>
        /// Initializes a new empty Point
        /// </summary>
        public Point() { }

        /// <summary>
        /// Returns a point based on degrees, minutes and seconds notation.
        /// For western or southern coordinates, add minus '-' in front of all longitude and/or latitude values
        /// </summary>
        /// <param name="longDegrees">Longitude degrees</param>
        /// <param name="longMinutes">Longitude minutes</param>
        /// <param name="longSeconds">Longitude seconds</param>
        /// <param name="latDegrees">Latitude degrees</param>
        /// <param name="latMinutes">Latitude minutes</param>
        /// <param name="latSeconds">Latitude seconds</param>
        /// <returns>Point</returns>
        public static Point FromDMS(Double longDegrees, Double longMinutes, Double longSeconds,
            Double latDegrees, Double latMinutes, Double latSeconds)
        {
            Double x = longDegrees + longMinutes / 60 + longSeconds / 3600;
            Double y = latDegrees + latMinutes / 60 + latSeconds / 3600;
            return new Point(x, y);
        }

        /// <summary>
        /// Gets an empty (uninitialized) point.
        /// </summary>
        /// <remarks>
        /// Returns a new empty point. If checking if a point is empty, especially in a loop, use <see cref="IsEmpty"/>
        /// since it doesn't create a new object.
        /// </remarks>
        public static Point Empty
        {
            get { return _empty.Clone() as Point; }
        }

        /// <summary>
        /// Gets a point representing (0, 0).
        /// </summary>
        /// <remarks>
        /// Returns a new point set to (0, 0). If checking if a point is zero, especially in a loop, use the <see cref="X"/>
        /// and <see cref="Y"/> properties, since these operations don't create a new object.
        /// </remarks>
        public static Point Zero
        {
            get { return _zero.Clone() as Point; }
        }

        /// <summary>
        /// Returns a 2D <see cref="Point"/> instance from this <see cref="Point"/>.
        /// </summary>
        /// <remarks>
        /// This method, which implements an OGC standard, behaves the same as <see cref="Clone"/> in 
        /// returning an exact copy of a point.
        /// </remarks>
        /// <returns><see cref="Point"/></returns>
        public Point AsPoint()
        {
            return new Point(_x, _y);
        }

        /// <summary>
        /// Gets or sets the X coordinate of the point
        /// </summary>
        public Double X
        {
            get
            {
                if (!IsEmpty)
                {
                    return _x;
                }
                else
                {
                    throw new InvalidOperationException("Point is empty");
                }
            }
            set
            {
                _x = value;
                SetNotEmpty();
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the point
        /// </summary>
        public Double Y
        {
            get
            {
                if (!IsEmpty)
                {
                    return _y;
                }
                else
                {
                    throw new InvalidOperationException("Point is empty");
                }
            }
            set
            {
                _y = value;
                SetNotEmpty();
            }
        }

        /// <summary>
        /// Returns part of coordinate. Index 0 = X, Index 1 = Y
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual Double this[UInt32 index]
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException("Point is empty.");
                }
                else if (index == 0)
                {
                    return X;
                }
                else if (index == 1)
                {
                    return Y;
                }
                else
                {
                    throw (new ArgumentOutOfRangeException("index", "Point index out of bounds."));
                }
            }
            set
            {
                if (index == 0)
                {
                    X = value;
                }
                else if (index == 1)
                {
                    Y = value;
                }
                else
                {
                    throw (new ArgumentOutOfRangeException("index", "Point index out of bounds."));
                }

                SetNotEmpty();
            }
        }

        #region IPoint Members

        public ICoordinate Coordinate
        {
            get { throw new NotImplementedException(); }
        }

        public Double this[Ordinates ordinate]
        {
            get
            {
                switch (ordinate)
                {
                    case Ordinates.X:
                        return X;
                    case Ordinates.Y:
                        return Y;
                    case Ordinates.Z:
                    case Ordinates.M:
                    default:
                        return Double.NaN;
                }
            }
        }

        public Int32 OrdinateCount
        {
            get { return ComponentCount; }
        }

        #endregion

        #region Operators
        /// <summary>
        /// Vector + Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns></returns>
        public static Point operator +(Point v1, Point v2)
        {
            return new Point(v1.X + v2.X, v1.Y + v2.Y);
        }


        /// <summary>
        /// Vector - Vector
        /// </summary>
        /// <param name="v1">Vector</param>
        /// <param name="v2">Vector</param>
        /// <returns>Cross product</returns>
        public static Point operator -(Point v1, Point v2)
        {
            return new Point(v1.X - v2.X, v1.Y - v2.Y);
        }

        /// <summary>
        /// Vector * Scalar
        /// </summary>
        /// <param name="m">Vector</param>
        /// <param name="d">Scalar (Double)</param>
        /// <returns></returns>
        public static Point operator *(Point m, Double d)
        {
            return new Point(m.X * d, m.Y * d);
        }

        #endregion

        public override Int32 PointCount
        {
            get { return 1; }
        }

        #region "Inherited methods from abstract class Geometry"

        /// <summary>
        /// Checks whether this instance is spatially equal to <paramref name="p"/>.
        /// </summary>
        /// <param name="p">Point to compare to</param>
        /// <returns>True if the points are either both empty or have the same coordinates, false otherwise.</returns>
        public virtual Boolean Equals(Point p)
        {
            if (ReferenceEquals(p, null))
            {
                return false;
            }

            if (IsEmpty && p.IsEmpty)
            {
                return true;
            }

            if (IsEmpty || p.IsEmpty)
            {
                return false;
            }

            return Tolerance.Equal(p.X, _x) && Tolerance.Equal(p.Y, _y);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. <see cref="GetHashCode"/> is suitable for use 
        /// in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>A hash code for the current <see cref="GetHashCode"/>.</returns>
        public override Int32 GetHashCode()
        {
            return _x.GetHashCode() ^ _y.GetHashCode() ^ IsEmpty.GetHashCode();
        }

        /// <summary>
        ///  The inherent dimension of this Geometry object, which must be less than or equal to the coordinate dimension.
        /// </summary>
        public override Dimensions Dimension
        {
            get { return Dimensions.Point; }
        }

        /// <summary>
        /// If true, then this Geometry represents the empty point set, Ø, for the coordinate space. 
        /// </summary>
        /// <returns>Returns 'true' if this Geometry is the empty geometry</returns>
        public override Boolean IsEmpty
        {
            get
            {
                return !_hasValue;
            }
        }

        /// <summary>
        /// Returns 'true' if this Geometry has no anomalous geometric points, such as self
        /// intersection or self tangency. The description of each instantiable geometric class will include the specific
        /// conditions that cause an instance of that class to be classified as not simple.
        /// </summary>
        /// <returns>true if the geometry is simple</returns>
        public override Boolean IsSimple
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// The boundary of a point is the empty set.
        /// </summary>
        /// <returns>null</returns>
        public override IGeometry Boundary
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the distance between this geometry instance and another geometry, as
        /// measured in the spatial reference system of this instance.
        /// </summary>
        /// <param name="geom"></param>
        /// <returns></returns>
        public override Double Distance(IGeometry geom)
        {
            if (geom is Point)
            {
                Point p = geom as Point;
                return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2));
            }
            else
            {
                throw new NotImplementedException("The method or operation is not implemented for this geometry type.");
            }
        }
        /// <summary>
        /// Returns the distance between this point and an <see cref="Extents"/>
        /// instance.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public Double Distance(Extents box)
        {
            return box.Distance(this);
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
        /// Geometry—Returns a geometry that represents the convex hull of this Geometry.
        /// </summary>
        /// <returns>The convex hull</returns>
        public override IGeometry ConvexHull()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set intersection of this Geometry
        /// with another Geometry.
        /// </summary>
        /// <param name="geom">Geometry to intersect with.</param>
        /// <returns>
        /// Returns a geometry that represents the point set intersection of 
        /// this Geometry with another Geometry.
        /// </returns>
        public override IGeometry Intersection(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a geometry that represents the point set union of this Geometry 
        /// with another Geometry.
        /// </summary>
        /// <param name="geometry">Geometry to union with.</param>
        /// <returns>Geometry union.</returns>
        public override IGeometry Union(IGeometry geometry)
        {
#warning broken implementation awaiting for replacement with NTS in Beta 2
            if (IsEmpty)
            {
                return geometry;
            }

            if (geometry.IsEmpty)
            {
                return this;
            }

            GeometryCollection union = new GeometryCollection();
            union.Add(this);
            return union;
        }

        /// <summary>
        /// Returns a geometry that represents the point set difference of this 
        /// Geometry with another Geometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override IGeometry Difference(IGeometry geom)
        {
            if (geom == null) throw new ArgumentNullException("geom");

            if (Equals(geom))
            {
                return Empty;
            }

            return geom;
        }

        /// <summary>
        /// Returns a geometry that represents the point set symmetric difference 
        /// of this Geometry with anotherGeometry.
        /// </summary>
        /// <param name="geom">Geometry to compare to</param>
        /// <returns>Geometry</returns>
        public override IGeometry SymmetricDifference(IGeometry geom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The minimum bounding box for this Geometry.
        /// </summary>
        /// <returns></returns>
        public override IExtents Extents
        {
            get
            {
                if (IsEmpty)
                {
                    return new Extents();
                }

                return new Extents(X, Y, X, Y);
            }
        }
        /// <summary>
        /// Checks whether this point touches an <see cref="Extents"/>
        /// </summary>
        /// <param name="box">box</param>
        /// <returns>true if they touch</returns>
        public Boolean Touches(Extents box)
        {
            return box.Touches(this);
        }

        /// <summary>
        /// Checks whether this point touches another <see cref="Geometry"/>
        /// </summary>
        /// <param name="geom">Geometry</param>
        /// <returns>true if they touch</returns>
        public override Boolean Touches(IGeometry geom)
        {
            if (geom is Point && Equals(geom))
            {
                return true;
            }

            throw new NotImplementedException("Touches not implemented for this feature type");
        }

        /// <summary>
        /// Checks whether this point intersects an <see cref="Extents"/>
        /// </summary>
        /// <param name="box">Box</param>
        /// <returns>True if they intersect</returns>
        public Boolean Intersects(Extents box)
        {
            return box.Contains(this);
        }

        /// <summary>
        /// Returns true if this instance contains 'geom'
        /// </summary>
        /// <param name="geom">Geometry</param>
        /// <returns>True if geom is contained by this instance</returns>
        public override Boolean Contains(IGeometry geom)
        {
            return false;
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of the Point.
        /// </summary>
        /// <returns>A copy of the Point instance.</returns>
        public override Geometry Clone()
        {
            if (IsEmpty)
            {
                return new Point();
            }

            return new Point(X, Y);
        }

        public override IEnumerable<Point> GetVertices()
        {
            yield return this;
        }

        #region IComparable<Point> Members

        /// <summary>
        /// Comparator used for ordering point first by ascending X, then by ascending Y.
        /// </summary>
        /// <param name="other">The <see cref="Point"/> to compare.</param>
        /// <returns>
        /// 0 if the points are spatially equal or both empty; 1 if <paramref name="other"/> is empty or
        /// if this point has a greater <see cref="X"/> value or equal X values and a greater <see cref="Y"/> value; 
        /// -1 if this point is empty or if <paramref name="other"/> has a greater <see cref="X"/> value or equal X values 
        /// and a greater <see cref="Y"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="other"/> is null.</exception>
        public virtual Int32 CompareTo(Point other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (Equals(other)) // This handles the case where both are empty. 
            {
                return 0;
            }
            else if (IsEmpty)
            {
                return -1;
            }
            else if (other.IsEmpty)
            {
                return 1;
            }
            else if (Tolerance.Less(X, other.X) || Tolerance.Equal(X, other.X) && Tolerance.Less(Y, other.Y))
            {
                return -1;
            }
            else if (Tolerance.Greater(X, other.X) || Tolerance.Equal(X, other.X) && Tolerance.Greater(Y, other.Y))
            {
                return 1;
            }

            throw new InvalidOperationException("Points cannot be compared.");
        }

        #endregion

        protected virtual void SetEmpty()
        {
            _x = _y = 0;
            _hasValue = false;
        }

        protected void SetNotEmpty()
        {
            _hasValue = true;
        }

        #region IEnumerable<DoubleComponent> Members

        public IEnumerator<DoubleComponent> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVector<DoubleComponent> Members

        IVector<DoubleComponent> IVector<DoubleComponent>.Clone()
        {
            throw new NotImplementedException();
        }

        public virtual Int32 ComponentCount
        {
            get { return 2; }
        }

        public virtual DoubleComponent[] Components
        {
            get
            {
                DoubleComponent[] components = new DoubleComponent[2];
                components[0] = _x;
                components[1] = _y;
                return components;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DoubleComponent this[Int32 index]
        {
            get
            {
                return this[(UInt32)index];
            }
            set
            {
                this[(UInt32) index] = (Double)value;
            }
        }

        #endregion

        #region IMatrix<DoubleComponent> Members

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Clone()
        {
            throw new NotImplementedException();
        }

        public Int32 ColumnCount
        {
            get { throw new NotImplementedException(); }
        }

        public Double Determinant
        {
            get { throw new NotImplementedException(); }
        }

        public DoubleComponent[][] Elements
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public MatrixFormat Format
        {
            get { throw new NotImplementedException(); }
        }

        public IMatrix<DoubleComponent> GetMatrix(Int32[] rowIndexes, Int32 j0, Int32 j1)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> Inverse
        {
            get { throw new NotSupportedException(); }
        }

        public Boolean IsInvertible
        {
            get { return false; }
        }

        public Boolean IsSingular
        {
            get { throw new NotSupportedException(); }
        }

        public Boolean IsSquare
        {
            get { return false; }
        }

        public Boolean IsSymmetrical
        {
            get { return false; }
        }

        public Int32 RowCount
        {
            get { throw new NotImplementedException(); }
        }

        public IMatrix<DoubleComponent> Transpose()
        {
            throw new NotSupportedException();
        }

        public DoubleComponent this[Int32 row, Int32 column]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region INegatable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> INegatable<IVector<DoubleComponent>>.Negative()
        {
            return new Point(-_x, -_y);
        }

        #endregion

        #region INegatable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> INegatable<IMatrix<DoubleComponent>>.Negative()
        {
            return new Point(-_x, -_y);
        }

        #endregion

        #region ISubtractable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> ISubtractable<IMatrix<DoubleComponent>>.Subtract(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasZero<IMatrix<DoubleComponent>>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IAddable<IMatrix<DoubleComponent>>.Add(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IDivisible<IMatrix<DoubleComponent>>.Divide(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasOne<IMatrix<DoubleComponent>>.One
        {
            get { return _one; }
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IMultipliable<IMatrix<DoubleComponent>>.Multiply(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IMatrix<DoubleComponent>> Members

        Boolean IEquatable<IMatrix<DoubleComponent>>.Equals(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Subtract(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IHasZero<IVector<DoubleComponent>>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region IAddable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Add(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Divide(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IHasOne<IVector<DoubleComponent>>.One
        {
            get { return _one; }
        }

        #endregion

        #region IMultipliable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Multiply(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVector<DoubleComponent> Members

        Int32 IVector<DoubleComponent>.ComponentCount
        {
            get { return (Int32)Dimension; }
        }

        DoubleComponent[] IVector<DoubleComponent>.Components
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        IVector<DoubleComponent> IVector<DoubleComponent>.Negative()
        {
            throw new NotImplementedException();
        }

        DoubleComponent IVector<DoubleComponent>.this[Int32 index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IMatrix<DoubleComponent> Members


        Int32 IMatrix<DoubleComponent>.ColumnCount
        {
            get { throw new NotImplementedException(); }
        }

        Double IMatrix<DoubleComponent>.Determinant
        {
            get { throw new NotImplementedException(); }
        }

        MatrixFormat IMatrix<DoubleComponent>.Format
        {
            get { throw new NotImplementedException(); }
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.GetMatrix(Int32[] rowIndexes, Int32 startColumn, Int32 endColumn)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsInvertible
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsSingular
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsSquare
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsSymmetrical
        {
            get { throw new NotImplementedException(); }
        }

        Int32 IMatrix<DoubleComponent>.RowCount
        {
            get { throw new NotImplementedException(); }
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Transpose()
        {
            throw new NotImplementedException();
        }

        DoubleComponent IMatrix<DoubleComponent>.this[Int32 row, Int32 column]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IComparable<IMatrix<DoubleComponent>> Members

        Int32 IComparable<IMatrix<DoubleComponent>>.CompareTo(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Abs()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrix<DoubleComponent>> Members

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.GreaterThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.GreaterThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.LessThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.LessThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Exp()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Log()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<DoubleComponent> Members

        IEnumerator<DoubleComponent> IEnumerable<DoubleComponent>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> ISubtractable<IVector<DoubleComponent>>.Subtract(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IAddable<IVector<DoubleComponent>>.Add(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IDivisible<IVector<DoubleComponent>>.Divide(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IMultipliable<IVector<DoubleComponent>>.Multiply(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IVector<DoubleComponent>> Members

        Boolean IBooleanComparable<IVector<DoubleComponent>>.GreaterThan(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVector<DoubleComponent>>.GreaterThanOrEqualTo(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVector<DoubleComponent>>.LessThan(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVector<DoubleComponent>>.LessThanOrEqualTo(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Exp()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Log()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IVector<DoubleComponent>> Members

        Boolean IEquatable<IVector<DoubleComponent>>.Equals(IVector<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IVector<DoubleComponent>> Members

        Int32 IComparable<IVector<DoubleComponent>>.CompareTo(IVector<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Double,IPoint> Members

        IPoint IComputable<Double, IPoint>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IPoint> Members

        IPoint IComputable<IPoint>.Abs()
        {
            throw new NotImplementedException();
        }

        IPoint IComputable<IPoint>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IPoint> Members

        IPoint INegatable<IPoint>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IPoint> Members

        public IPoint Subtract(IPoint b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IPoint> Members

        IPoint IHasZero<IPoint>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IPoint> Members

        public IPoint Add(IPoint b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IPoint> Members

        public IPoint Divide(IPoint b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IPoint> Members

        IPoint IHasOne<IPoint>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<IPoint> Members

        public IPoint Multiply(IPoint b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IPoint> Members

        public Boolean GreaterThan(IPoint value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(IPoint value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(IPoint value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(IPoint value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IPoint> Members

        IPoint IExponential<IPoint>.Exp()
        {
            throw new NotImplementedException();
        }

        IPoint IExponential<IPoint>.Log()
        {
            throw new NotImplementedException();
        }

        IPoint IExponential<IPoint>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IPoint IExponential<IPoint>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IPoint IExponential<IPoint>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IPoint> Members

        public IPoint Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,IPoint> Members

        public IPoint Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IComputable<Double, IVector<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IComputable<IVector<DoubleComponent>>.Abs()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IComputable<IVector<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IMultipliable<Double, IVector<DoubleComponent>>.Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IDivisible<Double, IVector<DoubleComponent>>.Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
