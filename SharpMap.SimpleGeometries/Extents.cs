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
using System.Globalization;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NPack;
using NPack.Interfaces;

namespace SharpMap.SimpleGeometries
{
    /// <summary>
    /// Bounding extents type with Double precision.
    /// </summary>
    /// <remarks>
    /// The <see cref="Extents"/> represents a 2D extents whose sides 
    /// are parallel to the two axes of the coordinate system.
    /// </remarks>
    [Serializable]
    public struct Extents : IExtents2D, IEquatable<Extents>
    {
        private static readonly Extents _empty;

        /// <summary>
        /// Gets an empty BoundingBox.
        /// </summary>
        public static Extents Empty
        {
            get { return _empty; }
        }

        private readonly GeometryFactory _factory;
        private Double _xMin, _yMin, _xMax, _yMax;
        private Boolean _hasValue;

        #region Constructors

        /// <summary>
        /// Initializes a bounding extents.
        /// </summary>
        /// <remarks>
        /// In case min values are larger than max values, the parameters 
        /// will be swapped to ensure correct min/max boundary.
        /// </remarks>
        /// <param name="minX">left</param>
        /// <param name="minY">bottom</param>
        /// <param name="maxX">right</param>
        /// <param name="maxY">top</param>
        internal Extents(GeometryFactory factory, Double minX, Double minY, Double maxX, Double maxY)
        {
            _factory = factory;
            _xMin = minX;
            _yMin = minY;
            _xMax = maxX;
            _yMax = maxY;
            _hasValue = true;
            checkMinMax();
        }

        /// <summary>
        /// Initializes a bounding extents.
        /// </summary>
        /// <param name="lowerLeft">Lower left corner.</param>
        /// <param name="upperRight">Upper right corner.</param>
        internal Extents(GeometryFactory factory, ICoordinate2D lowerLeft, ICoordinate2D upperRight)
            : this(factory, 0, 0, 0, 0)
        {
            if (lowerLeft == null || lowerLeft.IsEmpty
                || upperRight == null || upperRight.IsEmpty)
            {
                _hasValue = false;
                return;
            }

            _xMin = lowerLeft.X;
            _yMin = lowerLeft.Y;
            _xMax = upperRight.X;
            _yMax = upperRight.Y;
            checkMinMax();
        }

        /// <summary>
        /// Initializes a bounding extents.
        /// </summary>
        /// <param name="extents">Extents instances to join.</param>
        internal Extents(GeometryFactory factory, params Extents[] extents)
            : this(factory, 0, 0, 0, 0)
        {
            _hasValue = false;

            if (extents == null || extents.Length == 0)
            {
                return;
            }

            foreach (Extents extent in extents)
            {
                ExpandToInclude(extent);
            }
        }

        internal Extents(GeometryFactory factory, params IExtents[] extents)
            : this(factory, 0, 0, 0, 0)
        {
            _hasValue = false;

            if (extents == null || extents.Length == 0)
            {
                return;
            }

            foreach (IExtents extent in extents)
            {
                ExpandToInclude(extent);
            }
        }

        internal Extents(GeometryFactory factory)
            : this(factory, 0, 0, 0, 0)
        {
            _hasValue = false;
        }

        /// <summary>
        /// Initializes a new BoundingBox based on the bounds from a set of geometries.
        /// </summary>
        /// <param name="objects">List of <see cref="Geometry"/> objects to compute the BoundingBox for.</param>
        internal Extents(GeometryFactory factory, IEnumerable<Geometry> objects)
            : this(factory, 0, 0, 0, 0)
        {
            _hasValue = false;

            if (objects == null)
            {
                return;
            }

            checkMinMax();

            foreach (Geometry g in objects)
            {
                ExpandToInclude(g);
            }
        }

        /// <summary>
        /// Initializes a new BoundingBox based on the bounds from a set of bounding boxes.
        /// </summary>
        /// <param name="objects">list of <see cref="Extents"/> objects to compute the BoundingBox for.</param>
        internal Extents(GeometryFactory factory, IEnumerable<Extents> objects)
            : this(factory, 0, 0, 0, 0)
        {
            _hasValue = false;

            if (objects == null)
            {
                return;
            }

            foreach (Extents extents in objects)
            {
                ExpandToInclude(extents);
            }
        }

        #endregion Constructors

        #region Metrics Properties

        /// <summary>
        /// Gets or sets the lower left corner.
        /// </summary>
        public ICoordinate Min
        {
            get
            {
                return IsEmpty ? null : _factory.CoordinateFactory.Create(_xMin, _yMin);
            }
        }

        /// <summary>
        /// Gets or sets the upper right corner.
        /// </summary>
        public ICoordinate Max
        {
            get
            {
                return IsEmpty ? null : _factory.CoordinateFactory.Create(_xMax, _yMax);
            }
            private set
            {
                if (value == null)
                {
                    IsEmpty = true;
                    return;
                }

                _xMax = value[Ordinates.X];
                _yMax = value[Ordinates.Y];
            }
        }

        /// <summary>
        /// Gets the lower left corner.
        /// </summary>
        public Point LowerLeft
        {
            get { return new Point(Min); }
        }

        /// <summary>
        /// Gets the lower right corner.
        /// </summary>
        public Point LowerRight
        {
            get
            {
                if (IsEmpty)
                {
                    return _factory.CreatePoint() as Point;
                }

                return (Point)_factory.CreatePoint2D(_xMax, _yMin);
            }
        }

        /// <summary>
        /// Gets the upper left corner.
        /// </summary>
        public Point UpperLeft
        {
            get
            {
                if (IsEmpty)
                {
                    return _factory.CreatePoint() as Point;
                }


                return (Point)_factory.CreatePoint2D(_xMin, _yMax);
            }
        }

        /// <summary>
        /// Gets the upper right corner.
        /// </summary>
        public Point UpperRight
        {
            get { return new Point(Max); }
        }

        /// <summary>
        /// Returns true if BoundingBox is empty, false otherwise.
        /// </summary>
        public Boolean IsEmpty
        {
            get { return !_hasValue; }
            private set
            {
                if (value)
                {
                    _xMin = _yMin = _xMax = _yMax = 0;
                }

                _hasValue = !value;
            }
        }

        /// <summary>
        /// Gets the left boundary.
        /// </summary>
        public Double Left
        {
            get { return _xMin; }
            private set { _xMin = value; }
        }

        /// <summary>
        /// Gets the right boundary.
        /// </summary>
        public Double Right
        {
            get { return _xMax; }
            private set { _xMax = value; }
        }

        /// <summary>
        /// Gets the top boundary.
        /// </summary>
        public Double Top
        {
            get { return _yMax; }
            private set { _yMax = value; }
        }

        /// <summary>
        /// Gets the bottom boundary.
        /// </summary>
        public Double Bottom
        {
            get { return _yMin; }
            private set { _yMin = value; }
        }

        /// <summary>
        /// Returns the width of the bounding extents.
        /// </summary>
        /// <returns>
        /// Width of this <see cref="Extents"/>. 
        /// Returns <see cref="Double.NaN"/> if <see cref="IsEmpty"/> is true.
        /// </returns>
        public Double Width
        {
            get
            {
                if (IsEmpty)
                {
                    return Double.NaN;
                }

                return Math.Abs(_xMax - _xMin);
            }
        }

        /// <summary>
        /// Returns the height of the bounding extents.
        /// </summary>
        /// <returns>
        /// Height of this <see cref="Extents"/>. 
        /// Returns <see cref="Double.NaN"/> if <see cref="IsEmpty"/> is true.
        /// </returns>
        public Double Height
        {
            get
            {
                if (IsEmpty)
                {
                    return Double.NaN;
                }

                return Math.Abs(_yMax - _yMin);
            }
        }

        #endregion

        #region Spatial Relationships

        #region Borders

        /// <summary>
        /// Determines if two boxes share, at least partially, a common border, within the 
        /// <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="extents">The extents to check.</param>
        /// <returns>
        /// True if <paramref name="extents"/> shares a commons border, false if not, 
        /// or if either or both are empty.
        /// </returns>
        public Boolean Borders(Extents extents)
        {
            return Borders(extents, Tolerance.Global);
        }

        /// <summary>
        /// Determines if two boxes share, at least partially, a common border.
        /// </summary>
        /// <param name="extents">The extents to check.</param>
        /// <param name="tolerance">The tolerance to use in comparing.</param>
        /// <returns>
        /// True if <paramref name="extents"/> shares a commons border, false if not, or if either or both are empty.
        /// </returns>
        public Boolean Borders(Extents extents, Tolerance tolerance)
        {
            return Left == extents.Left || Bottom == extents.Bottom || Right == extents.Right || Top == extents.Top;
        }

        #endregion Borders

        #region Contains

        /// <summary>
        /// Returns true if this instance contains the <see cref="Extents"/>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="extents"><see cref="Extents"/></param>
        /// <returns>
        /// True this <see cref="Extents"/> contains the 
        /// <paramref name="extents">argument</paramref>.
        /// </returns>
        public Boolean Contains(IExtents extents)
        {
            return Contains(extents, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this instance contains the <see cref="Extents"/>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>
        /// </summary>
        /// <param name="extents"><see cref="Extents"/></param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True this <see cref="Extents"/> contains the 
        /// <paramref name="extents">argument</paramref>.
        /// </returns>
        public Boolean Contains(IExtents extents, Tolerance tolerance)
        {
            IExtents2D extents2D = extents as IExtents2D;

            if (extents2D == null)
            {
                throw new ArgumentException(
                    "Parameter must be a non-null IExtent2D instance.");
            }

            if (IsEmpty || extents2D.IsEmpty)
            {
                return false;
            }

            return tolerance.LessOrEqual(Left, extents2D.Left) &&
                   tolerance.GreaterOrEqual(Top, extents2D.Top) &&
                   tolerance.GreaterOrEqual(Right, extents2D.Right) &&
                   tolerance.LessOrEqual(Bottom, extents2D.Bottom);
        }

        /// <summary>
        /// Checks whether a <see cref="Point"/> borders or lies within the bounding extents, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="p"><see cref="Point"/></param>
        /// <returns>
        /// True if <paramref name="p">the point</paramref> borders or is 
        /// within this <see cref="Extents"/>.
        /// </returns>
        public Boolean Contains(Point p)
        {
            return Contains(p, Tolerance.Global);
        }

        /// <summary>
        /// Checks whether a <see cref="Point"/> borders or lies within the bounding extents, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="p"><see cref="Point"/></param>
        /// <param name="tolerance"><see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if <paramref name="p">the point</paramref> 
        /// borders or is within this <see cref="Extents"/>.
        /// </returns>
        public Boolean Contains(Point p, Tolerance tolerance)
        {
            if (p == null || IsEmpty || p.IsEmpty)
            {
                return false;
            }

            if (tolerance.Less(Right, p.X))
            {
                return false;
            }
            if (tolerance.Greater(Left, p.X))
            {
                return false;
            }
            if (tolerance.Less(Top, p.Y))
            {
                return false;
            }
            if (tolerance.Greater(Bottom, p.Y))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if this instance contains the <paramref name="geometry"/>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="geometry">
        /// Geometry to test if this <see cref="Extents"/> contains.
        /// </param>
        /// <returns>
        /// True if this <see cref="Extents"/> contains the 
        /// <paramref name="geometry">geometry</paramref>.
        /// </returns>
        public Boolean Contains(Geometry geometry)
        {
            return Contains(geometry, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this instance contains the <paramref name="geometry">, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="geometry">Geometry to test if this <see cref="Extents"/> contains.</param>
        /// <param name="tolerance"><see cref="Tolerance"/> to use to compare values.</param>
        /// <returns>True if this <see cref="Extents"/> contains the 
        /// <paramref name="geometry">geometry</paramref>.</returns>
        public Boolean Contains(Geometry geometry, Tolerance tolerance)
        {
            if (geometry == null)
            {
                return false;
            }

            if (geometry is Point)
            {
                return Contains(geometry as Point, tolerance);
            }

#warning Bounding extents intersection is incorrect here...
            // TODO: Replace bounding extents intersection with actual geometric intersection when NTS implemented
            if (geometry == null)
            {
                return false;
            }

            return Contains(geometry.Extents, tolerance);
        }

        #endregion Contains

        #region Intersects

        /// <summary>
        /// Determines whether the <see cref="Extents"/> instance intersects the 
        /// <paramref name="extents">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="extents"><see cref="Extents"/> to check intersection with.
        /// </param>
        /// <returns>
        /// True if the <paramref name="extents">argument</paramref> 
        /// touches this <see cref="Extents"/> instance in any way.
        /// </returns>
        public Boolean Intersects(IExtents extents)
        {
            return Intersects(extents, Tolerance.Global);
        }

        /// <summary>
        /// Determines whether the <see cref="Extents"/> instance intersects the
        /// <paramref name="extents">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="extents">
        /// <see cref="Extents"/> to check intersection with.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if the <paramref name="extents">argument</paramref> touches 
        /// this <see cref="Extents"/> instance in any way.
        /// </returns>
        public Boolean Intersects(IExtents extents, Tolerance tolerance)
        {
            return Touches(extents, tolerance);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance intersects 
        /// the <paramref name="geometry">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="geometry">
        /// <see cref="Geometry"/> to check intersection with.
        /// </param>
        /// <returns>
        /// True if this BoundingBox intersects the 
        /// <paramref name="geometry">geometry</paramref>.
        /// </returns>
        public Boolean Intersects(Geometry geometry)
        {
            return Intersects(geometry, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance intersects the 
        /// <paramref name="geometry">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="geometry">
        /// <see cref="Geometry"/> to check intersection with.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if this BoundingBox intersects the 
        /// <paramref name="geometry">geometry</paramref>.
        /// </returns>
        public Boolean Intersects(Geometry geometry, Tolerance tolerance)
        {
            return Touches(geometry, tolerance);
        }

        #endregion Intersects

        #region Overlaps

        /// <summary>
        /// Returns true if this <see cref="Extents"/> overlaps the passed 
        /// <paramref name="b">BoundingBox</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Extents"/> can touch and not overlap. If the passed 
        /// <paramref name="b">bounding extents</paramref> and 
        /// this <see cref="Extents"/> share a common edge or a common point, 
        /// the <see cref="Touches"/> method will
        /// return true, but <see cref="Overlaps"/> will return false.
        /// </remarks>
        /// <param name="b">
        /// <see cref="Extents"/> to test if it overlaps this <see cref="Extents"/>.
        /// </param>
        /// <returns>
        /// True if the <paramref name="b">bounding extents</paramref> overlaps.
        /// </returns>
        public Boolean Overlaps(IExtents b)
        {
            return Overlaps(b, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> overlaps the passed 
        /// <paramref name="b">BoundingBox</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Extents"/> can touch and not overlap. 
        /// If the passed <paramref name="b">bounding extents</paramref> and 
        /// this <see cref="Extents"/> share a common edge or a common point, 
        /// the <see cref="Touches"/> method will
        /// return true, but <see cref="Overlaps"/> will return false.
        /// </remarks>
        /// <param name="b">
        /// <see cref="Extents"/> to test if it overlaps this <see cref="Extents"/>.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if the <paramref name="b">bounding extents</paramref> overlaps.
        /// </returns>
        public Boolean Overlaps(IExtents b, Tolerance tolerance)
        {
            if (IsEmpty || b.IsEmpty)
            {
                return false;
            }

            return Contains(b) ||
                   !(tolerance.GreaterOrEqual(b.GetMin(Ordinates.X), Right) ||
                     tolerance.LessOrEqual(b.GetMax(Ordinates.X), Left) ||
                     tolerance.LessOrEqual(b.GetMax(Ordinates.Y), Bottom) ||
                     tolerance.GreaterOrEqual(b.GetMin(Ordinates.Y), Top));
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> overlaps the 
        /// <paramref name="p">point</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Point"/> can touch and not overlap. 
        /// If the <paramref name="p">point</paramref> and 
        /// the <see cref="Extents"/> share a common point, 
        /// the <see cref="Touches"/> method will
        /// return true, but <see cref="Overlaps"/> will return false. 
        /// For <see cref="Point">points</see>,
        /// <see cref="Contains"/> will return true if Overlaps returns true.
        /// </remarks>
        /// <param name="p">
        /// <see cref="Point"/> to test if it overlaps this <see cref="Extents"/>.
        /// </param>
        /// <returns>
        /// True if the <paramref name="p">point</paramref> overlaps.
        /// </returns>
        public Boolean Overlaps(Point p)
        {
            return Overlaps(p, Tolerance.Global);
        }


        /// <summary>
        /// Returns true if this <see cref="Extents"/> overlaps the 
        /// <paramref name="p">point</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Point"/> can touch and not overlap. If the 
        /// <paramref name="p">point</paramref> and 
        /// the <see cref="Extents"/> share a common point, 
        /// the <see cref="Touches"/> method will
        /// return true, but <see cref="Overlaps"/> will return false. 
        /// For <see cref="Point">points</see>,
        /// <see cref="Contains"/> will return true if Overlaps returns true.
        /// </remarks>
        /// <param name="p">
        /// <see cref="Point"/> to test if it overlaps this 
        /// <see cref="Extents"/>.</param>
        /// <param name="tolerance"><see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if the <paramref name="p">point</paramref> overlaps.
        /// </returns>
        public Boolean Overlaps(Point p, Tolerance tolerance)
        {
            if (p == null || p.IsEmpty)
            {
                return false;
            }

            return Overlaps(p.Extents);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> overlaps the 
        /// <paramref name="geometry">geometry</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Geometry"/> can touch and not overlap. 
        /// If the <paramref name="geometry">geometry</paramref> and 
        /// the <see cref="Extents"/> share a common edge or a point, 
        /// the <see cref="Touches"/> method will
        /// return true, but <see cref="Overlaps"/> will return false. 
        /// For <see cref="Point">points</see>,
        /// <see cref="Contains"/> will return true if Overlaps returns true.
        /// </remarks>
        /// <param name="geometry">
        /// <see cref="Geometry"/> to test if it overlaps this <see cref="Extents"/>.
        /// </param>
        /// <returns>
        /// True if the <paramref name="geometry">geometry</paramref> overlaps.
        /// </returns>
        public Boolean Overlaps(Geometry geometry)
        {
            return Overlaps(geometry, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> overlaps the 
        /// <paramref name="geometry">geometry</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <remarks>
        /// A <see cref="Geometry"/> can touch and not overlap. 
        /// If the <paramref name="geometry">geometry</paramref> and 
        /// the <see cref="Extents"/> share a common edge or a point, 
        /// the <see cref="Touches"/> method will
        /// return true, but <see cref="Overlaps"/> will return false. 
        /// For <see cref="Point">points</see>,
        /// <see cref="Contains"/> will return true if Overlaps returns true.
        /// </remarks>
        /// <param name="geometry">
        /// <see cref="Geometry"/> to test if it overlaps this <see cref="Extents"/>.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if the <paramref name="geometry">geometry</paramref> overlaps.
        /// </returns>
        public Boolean Overlaps(Geometry geometry, Tolerance tolerance)
        {
            if (geometry == null)
            {
                return false;
            }

            if (geometry is Point)
            {
                return Overlaps(geometry as Point, tolerance);
            }

#warning Bounding extents intersection is incorrect here...
            // TODO: Replace bounding extents intersection with actual geometric intersection when NTS implemented

            return Overlaps(geometry.Extents, tolerance);
        }

        #endregion Overlaps

        #region Touches

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance touches the 
        /// <paramref name="extents">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="extents">
        /// <see cref="Extents"/> to check if this BoundingBox touches.
        /// </param>
        /// <returns>
        /// True if <paramref name="extents"/> touches.
        /// </returns>
        public Boolean Touches(IExtents extents)
        {
            return Touches(extents, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance touches the 
        /// <paramref name="extents">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="extents">
        /// <see cref="Extents"/> to check if this BoundingBox touches.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if <paramref name="extents"/> touches.
        /// </returns>
        public Boolean Touches(IExtents extents, Tolerance tolerance)
        {
            IExtents2D extents2D = extents as IExtents2D;

            if (extents2D == null)
            {
                throw new ArgumentException(
                    "Parameter must be a non-null IExtent2D instance.");
            }

            if (IsEmpty || extents2D.IsEmpty)
            {
                return false;
            }

            return !(tolerance.Greater(extents2D.Left, Right) ||
                     tolerance.Less(extents2D.Right, Left) ||
                     tolerance.Less(extents2D.Top, Bottom) ||
                     tolerance.Greater(extents2D.Bottom, Top));
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance touches the
        /// <paramref name="p">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="p">
        /// <see cref="Point"/> to check if this BoundingBox instance touches.
        /// </param>
        /// <returns>
        /// True if the <paramref name="p">point</paramref> touches.
        /// </returns>
        public Boolean Touches(Point p)
        {
            return Touches(p, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance touches the 
        /// <paramref name="p">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="p">
        /// <see cref="Point"/> to check if this BoundingBox instance touches.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if the <paramref name="p">point</paramref> touches.
        /// </returns>
        public Boolean Touches(Point p, Tolerance tolerance)
        {
            if (p == null || p.IsEmpty)
            {
                return false;
            }

            return Touches(p.Extents, tolerance);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance touches the 
        /// <paramref name="geometry">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="geometry">
        /// <see cref="Geometry"/> to test if it touches this <see cref="Extents"/>.
        /// </param>
        /// <returns>
        /// True if the <paramref name="geometry">geometry</paramref> touches.
        /// </returns>
        public Boolean Touches(Geometry geometry)
        {
            return Touches(geometry, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance touches the 
        /// <paramref name="geometry">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="geometry">
        /// <see cref="Geometry"/> to test if it touches this <see cref="Extents"/>.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if the <paramref name="geometry">geometry</paramref> touches.
        /// </returns>
        public Boolean Touches(Geometry geometry, Tolerance tolerance)
        {
            if (geometry is Point)
            {
                return Touches(geometry as Point, tolerance);
            }

#warning Bounding extents intersection is incorrect here...
            // TODO: Replace bounding extents intersection with actual geometric intersection when NTS implemented
            if (geometry == null)
            {
                return false;
            }

            return Touches(geometry.Extents, tolerance);
        }

        #endregion Touches

        #region Within

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance is 
        /// completely within (does not border) the <paramref name="extents">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="extents"><see cref="Extents"/></param>
        /// <returns>True if the BoundingBox contains <paramref name="extents"/>.</returns>
        public Boolean Within(IExtents extents)
        {
            return Within(extents, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance is 
        /// completely within (does not border) the <paramref name="extents">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="extents"><see cref="Extents"/></param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>True if the BoundingBox contains <paramref name="extents"/>.</returns>
        public Boolean Within(IExtents extents, Tolerance tolerance)
        {
            if (IsEmpty || extents.IsEmpty)
            {
                return false;
            }

            return tolerance.LessOrEqual(Left, extents.GetMin(Ordinates.X)) &&
                   tolerance.GreaterOrEqual(Top, extents.GetMax(Ordinates.Y)) &&
                   tolerance.GreaterOrEqual(Right, extents.GetMax(Ordinates.X)) &&
                   tolerance.LessOrEqual(Bottom, extents.GetMin(Ordinates.Y));
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> 
        /// instance is completely within (does not border) 
        /// the <paramref name="p">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="p">
        /// Point to test if it is within this <see cref="Extents"/> instance.
        /// </param>
        /// <returns>
        /// True if this <see cref="Extents"/> is 
        /// within the <paramref name="p">point</paramref>.
        /// </returns>
        public Boolean Within(Point p)
        {
            return Within(p, Tolerance.Global);
        }


        /// <summary>
        /// Returns true if this <see cref="Extents"/> 
        /// instance is completely within (does not border) 
        /// the <paramref name="p">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="p">
        /// Point to test if it is within this <see cref="Extents"/> instance.
        /// </param>
        /// <param name="tolerance">
        /// <see cref="Tolerance"/> to use to compare values.
        /// </param>
        /// <returns>
        /// True if this <see cref="Extents"/> is within the 
        /// <paramref name="p">point</paramref>.
        /// </returns>
        public Boolean Within(Point p, Tolerance tolerance)
        {
            if (p == null || IsEmpty || p.IsEmpty)
            {
                return false;
            }

            if (tolerance.LessOrEqual(Right, p.X))
            {
                return false;
            }
            if (tolerance.GreaterOrEqual(Left, p.X))
            {
                return false;
            }
            if (tolerance.LessOrEqual(Top, p.Y))
            {
                return false;
            }
            if (tolerance.GreaterOrEqual(Bottom, p.Y))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance is completely within 
        /// (does not border) the <paramref name="geometry">argument</paramref>, 
        /// within the <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="geometry">Geometry to test if it is within this <see cref="Extents"/> instance.</param>
        /// <returns>True if this <see cref="Extents"/> is within the 
        /// <paramref name="geometry">geometry</paramref>.</returns>
        public Boolean Within(Geometry geometry)
        {
            return Within(geometry, Tolerance.Global);
        }

        /// <summary>
        /// Returns true if this <see cref="Extents"/> instance is completely within 
        /// (does not border) the <paramref name="geometry">argument</paramref>, 
        /// within the <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="geometry">Geometry to test if it is within this <see cref="Extents"/> instance.</param>
        /// <param name="tolerance"><see cref="Tolerance"/> to use to compare values.</param>
        /// <returns>True if this <see cref="Extents"/> is within the 
        /// <paramref name="geometry">geometry</paramref>.</returns>
        public Boolean Within(Geometry geometry, Tolerance tolerance)
        {
            if (geometry == null)
            {
                return false;
            }

            if (geometry is Point)
            {
                return Contains(geometry as Point, tolerance);
            }

#warning Bounding extents intersection is incorrect here...
            // TODO: Replace bounding extents intersection with actual geometric intersection when NTS implemented

            return Within(geometry.Extents, tolerance);
        }

        #endregion Within

        #endregion

        #region GetArea and GetIntersectingArea

        /// <summary>
        /// Returns the area of the <see cref="Extents"/>.
        /// </summary>
        /// <returns>Area of extents</returns>
        public Double GetArea()
        {
            return Width * Height;
        }

        /// <summary>
        /// Gets the intersecting area between two boundingboxes
        /// </summary>
        /// <param name="r">BoundingBox</param>
        /// <returns>Area</returns>
        public Double GetIntersectingArea(Extents r)
        {
            //UInt32 cIndex;
            //for (cIndex = 0; cIndex < 2; cIndex++)
            //    if (Min[cIndex] > r.Max[cIndex] || Max[cIndex] < r.Min[cIndex]) return 0.0;

            if (!r.Intersects(this))
            {
                return 0.0;
            }

            //Double ret = 1.0;
            //Double f1, f2;

            //for (cIndex = 0; cIndex < 2; cIndex++)
            //{
            //    f1 = Math.Max(Min[cIndex], r.Min[cIndex]);
            //    f2 = Math.Min(Max[cIndex], r.Max[cIndex]);
            //    ret *= f2 - f1;
            //}

            return
                (Math.Min(r.Right, Right) - Math.Max(r.Left, Left)) * (Math.Min(r.Top, Top) - Math.Max(r.Bottom, Bottom));
        }

        #endregion

        #region Join

        /// <summary>
        /// Computes the joined BoundingBox of this instance and another BoundingBox.
        /// </summary>
        /// <param name="extents">BoundingBox to join with</param>
        /// <returns>A new BoundingBox containing both BoundingBox instances.</returns>
        public Extents Join(Extents extents)
        {
            if (extents == Empty)
            {
                return this;
            }
            else if (this == Empty)
            {
                return extents;
            }
            else
            {
                return new Extents(
                    _factory,
                    Math.Min(Left, extents.Left),
                    Math.Min(Bottom, extents.Bottom),
                    Math.Max(Right, extents.Right),
                    Math.Max(Top, extents.Top));
            }
        }

        /// <summary>
        /// Computes the joined BoundingBox of two BoundingBox instances.
        /// </summary>
        /// <param name="box1"></param>
        /// <param name="box2"></param>
        /// <returns></returns>
        public static Extents Join(Extents box1, Extents box2)
        {
            return box1.Join(box2);
        }

        /// <summary>
        /// Computes the joined <see cref="Extents"/> of an array of BoundingBox instances.
        /// </summary>
        /// <param name="boxes">BoundingBox instances to join.</param>
        /// <returns>Combined BoundingBox.</returns>
        public static Extents Join(GeometryFactory factory, params Extents[] boxes)
        {
            return new Extents(factory, boxes);
        }

        #endregion

        #region Grow and Shrink

        /// <summary>
        /// Returns a <see cref="Extents"/> with a size decreased over this BoundingBox by the given 
        /// amount in all directions.
        /// </summary>
        /// <param name="amount">Amount to decrease in all directions.</param>
        public Extents Shrink(Double amount)
        {
            return Grow(-amount);
        }

        /// <summary>
        /// Returns a <see cref="Extents"/> with a size decreased over this BoundingBox by the 
        /// given amount in horizontal and vertical directions.
        /// </summary>
        /// <param name="amountInX">Amount to decrease in horizontal direction.</param>
        /// <param name="amountInY">Amount to decrease in vertical direction.</param>
        public Extents Shrink(Double amountInX, Double amountInY)
        {
            return Grow(-amountInX, -amountInY);
        }

        /// <summary>
        /// Returns a <see cref="Extents"/> with a size decreased over this BoundingBox by the given 
        /// amount in each specific direction.
        /// </summary>
        /// <param name="amountBottom">Amount to decrease the bottom edge by.</param>
        /// <param name="amountLeft">Amount to decrease the left edge by.</param>
        /// <param name="amountRight">Amount to decrease the right edge by.</param>
        /// <param name="amountTop">Amount to decrease the top edge by.</param>
        public Extents Shrink(Double amountTop, Double amountRight, Double amountBottom, Double amountLeft)
        {
            return Grow(-amountTop, -amountRight, -amountBottom, -amountLeft);
        }

        /// <summary>
        /// Returns a <see cref="Extents"/> with a size increased over this BoundingBox by the given 
        /// amount in all directions.
        /// </summary>
        /// <param name="amount">Amount to increase in all directions.</param>
        public Extents Grow(Double amount)
        {
            return Grow(amount, amount);
        }

        /// <summary>
        /// Returns a <see cref="Extents"/> with a size increased over this BoundingBox by the 
        /// given amount in horizontal and vertical directions.
        /// </summary>
        /// <param name="amountInX">Amount to increase in horizontal direction.</param>
        /// <param name="amountInY">Amount to increase in vertical direction.</param>
        public Extents Grow(Double amountInX, Double amountInY)
        {
            return Grow(amountInY, amountInX, amountInY, amountInX);
        }

        /// <summary>
        /// Returns a <see cref="Extents"/> with a size increased over this BoundingBox by the given 
        /// amount in each specific direction.
        /// </summary>
        /// <param name="amountBottom">Amount to increase the bottom edge by.</param>
        /// <param name="amountLeft">Amount to increase the left edge by.</param>
        /// <param name="amountRight">Amount to increase the right edge by.</param>
        /// <param name="amountTop">Amount to increase the top edge by.</param>
        public Extents Grow(Double amountTop, Double amountRight, Double amountBottom, Double amountLeft)
        {
            Extents extents = this; // make a copy
            extents.Left -= amountLeft;
            extents.Bottom -= amountBottom;
            extents.Right += amountRight;
            extents.Top += amountTop;
            extents.checkMinMax();
            return extents;
        }

        #endregion

        #region Offset

        /// <summary>
        /// Moves/translates the <see cref="Extents"/> along the the specified vector.
        /// </summary>
        /// <param name="vector">Offset vector.</param>
        public void Offset(Point vector)
        {
            _xMin += vector.X;
            _xMax += vector.X;
            _yMin += vector.Y;
            _yMax += vector.Y;
        }

        #endregion Offset

        #region ExpandToInclude
        /// <summary>
        /// Expands the <see cref="Extents"/> instance to contain the space contained by <paramref name="extents"/>.
        /// </summary>
        /// <param name="extent"><see cref="Extents"/> to enlarge extents to contain.</param>
        public void ExpandToInclude(IExtents extent)
        {
            IExtents2D extent2D = extent as IExtents2D;

            if (extent2D == null)
            {
                throw new ArgumentException(
                    "Parameter must be a non-null IExtent2D instance.");
            }

            if (extent2D.Left < Left || IsEmpty)
            {
                Left = extent2D.Left;
            }
            if (extent2D.Bottom < Bottom || IsEmpty)
            {
                Bottom = extent2D.Bottom;
            }
            if (extent2D.Right > Right || IsEmpty)
            {
                Right = extent2D.Right;
            }
            if (extent2D.Top > Top || IsEmpty)
            {
                Top = extent2D.Top;
            }

            if (IsEmpty)
            {
                IsEmpty = extent.IsEmpty;
            }
        }

        /// <summary>
        /// Expands the <see cref="Extents"/> instance to contain the space 
        /// contained by <paramref name="extent"/>.
        /// </summary>
        /// <param name="extent"><see cref="Extents"/> to enlarge extents to contain.</param>
        public void ExpandToInclude(Extents extent)
        {
            if (extent.Left < Left || IsEmpty)
            {
                Left = extent.Left;
            }
            if (extent.Bottom < Bottom || IsEmpty)
            {
                Bottom = extent.Bottom;
            }
            if (extent.Right > Right || IsEmpty)
            {
                Right = extent.Right;
            }
            if (extent.Top > Top || IsEmpty)
            {
                Top = extent.Top;
            }

            // TODO: check if this is the right behavior... expanding by an empty makes the target empty?
            if (IsEmpty)
            {
                IsEmpty = extent.IsEmpty;
            }
        }

        /// <summary>
        /// Expands the <see cref="Extents"/> instance to contain geometry <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry"><see cref="Geometry"/> to enlarge extents to contain.</param>
        public void ExpandToInclude(Geometry geometry)
        {
            ExpandToInclude(geometry.Extents);
        }

        /// <summary>
        /// Expands the <see cref="Extents"/> instance to contain geometry <paramref name="coordinate"/>.
        /// </summary>
        /// <param name="coordinate"><see cref="ICoordinate"/> to enlarge extents to contain.</param>
        public void ExpandToInclude(ICoordinate coordinate)
        {
            if (coordinate == null)
            {
                return;
            }

            Double x = coordinate[Ordinates.X];
            Double y = coordinate[Ordinates.Y];

            if (x < Left || IsEmpty)
            {
                Left = x;
            }

            if (y < Bottom || IsEmpty)
            {
                Bottom = y;
            }

            if (x > Right || IsEmpty)
            {
                Right = x;
            }

            if (y > Top || IsEmpty)
            {
                Top = y;
            }

            // TODO: check if this is the right behavior... expanding by an empty makes the target empty?
            if (IsEmpty)
            {
                IsEmpty = coordinate.IsEmpty;
            }
        }

        #endregion ExpandToInclude

        #region Split

        /// <summary>
        /// Splits the BoundingBox where it intersects with the <paramref name="point"/>.
        /// </summary>
        /// <param name="point">Point to perform split at.</param>
        /// <remarks>
        /// A BoundingBox instance may be split into 1, 2 or 4 new BoundingBox instances by a point
        /// depending on if the point is outside, on the edge of, or inside the BoundingBox respectively.
        /// </remarks>
        /// <returns>An enumeration of BoundingBox instances subdivided by the point.</returns>
        public IEnumerable<Extents> Split(Point point)
        {
            if (!Contains(point))
            {
                return new Extents[0];
            }

            List<Extents> splits = new List<Extents>(4);

            Extents b1 = new Extents(_factory, Left, point.Y, point.X, Top);
            Extents b2 = new Extents(_factory, point.X, point.Y, Right, Top);
            Extents b3 = new Extents(_factory, Left, Bottom, point.X, point.Y);
            Extents b4 = new Extents(_factory, point.X, Bottom, Right, point.Y);

            if (b1.GetArea() > 0)
            {
                splits.Add(b1);
            }

            if (b2.GetArea() > 0)
            {
                splits.Add(b2);
            }

            if (b3.GetArea() > 0)
            {
                splits.Add(b3);
            }

            if (b4.GetArea() > 0)
            {
                splits.Add(b4);
            }

            return splits;
        }

        #endregion

        #region Distance

        /// <summary> 
        /// Computes the minimum distance between this and another <see cref="Extents"/>.
        /// The distance between overlapping bounding boxes is 0.  Otherwise, the
        /// distance is the Euclidean distance between the closest points.
        /// </summary>
        /// <param name="extents">BoundingBox to calculate distance to.</param>
        /// <returns>The distance between this and another <see cref="Extents"/>. 
        /// Returns <see cref="Double.NaN"/> if either <see cref="Extents"/>'s 
        /// <see cref="IsEmpty"/> property is true.</returns>
        public Double Distance(Extents extents)
        {
            if (IsEmpty || extents.IsEmpty)
                return Double.NaN;

            Double ret = 0.0;

            if (Contains(extents))
            {
                return ret;
            }

            ret += extents.Right < Left ? Math.Pow(Left - extents.Right, 2) : Math.Pow(extents.Left - Right, 2);
            ret += extents.Top < Bottom ? Math.Pow(Bottom - extents.Top, 2) : Math.Pow(extents.Bottom - Top, 2);

            //for (UInt32 cIndex = 0; cIndex < 2; cIndex++)
            //{
            //    if (p[cIndex] < Min[cIndex]) ret += Math.Pow(Min[cIndex] - p[cIndex], 2.0);
            //    else if (p[cIndex] > Max[cIndex]) ret += Math.Pow(p[cIndex] - Max[cIndex], 2.0);
            //}

            return Math.Sqrt(ret);
        }

        /// <summary>
        /// Computes the minimum distance between this BoundingBox and a <see cref="Point"/>.
        /// </summary>
        /// <param name="p"><see cref="Point"/> to calculate distance to.</param>
        /// <returns>Minimum distance.</returns>
        public Double Distance(Point p)
        {
            return Distance(p.Extents);
        }

        #endregion

        #region GetCentroid

        /// <summary>
        /// Returns the center of the BoundingBox.
        /// </summary>
        public Point GetCentroid()
        {
            if (IsEmpty)
            {
                return _factory.CreatePoint() as Point;
            }

            return (Point)LowerLeft.Add(UpperRight).Multiply(0.5);
        }

        #endregion

        #region ToGeometry

        /// <summary>
        /// Computes a <see cref="Geometry"/> with the same vertices
        /// as the BoundingBox instance.
        /// </summary>
        /// <returns>
        /// A <see cref="Polygon"/> with the exact same shape and 
        /// area as the BoundingBox, if the BoundingBox is not empty,
        /// or <see cref="Point.Empty"/> if it is.
        /// </returns>
        public Geometry ToGeometry()
        {
            if (IsEmpty)
            {
                return _factory.CreatePoint() as Geometry;
            }

            ICoordinate[] vertices = new ICoordinate[]
                                         {
                                              LowerLeft.Coordinate, 
                                              UpperLeft.Coordinate, 
                                              UpperRight.Coordinate, 
                                              LowerRight.Coordinate, 
                                              LowerLeft.Coordinate
                                         };

            Polygon p = (Polygon)_factory.CreatePolygon(vertices);
            return p;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance, within the 
        /// <see cref="Tolerance.Global">global tolerance</see>.
        /// </summary>
        /// <param name="other"><see cref="Extents"/> to compare to.</param>
        /// <returns>True if equal within <see cref="Tolerance.Global"/>.</returns>
        public Boolean Equals(Extents other)
        {
            return Equals(other, Tolerance.Global);
        }

        /// <summary>
        /// Checks whether the values of this instance is equal to the values of another instance, within the 
        /// <paramref name="tolerance">given tolerance</paramref>.
        /// </summary>
        /// <param name="other"><see cref="Extents"/> to compare to.</param>
        /// <param name="tolerance">The <see cref="Tolerance"/> to use to compare.</param>
        /// <returns>True if equal within <paramref name="tolerance"/>.</returns>
        public Boolean Equals(Extents other, Tolerance tolerance)
        {
            // Check empty
            if (IsEmpty && other.IsEmpty)
            {
                return true;
            }

            if (IsEmpty || other.IsEmpty)
            {
                return false;
            }

            return tolerance.Equal(Left, other.Left) &&
                   tolerance.Equal(Right, other.Right) &&
                   tolerance.Equal(Top, other.Top) &&
                   tolerance.Equal(Bottom, other.Bottom);
        }

        #endregion Equality

        #region Object Overrides

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is Extents))
            {
                return false;
            }

            Extents extents = (Extents)obj;
            return Equals(extents);
        }

        /// <summary>
        /// Returns a hash code for the specified object
        /// </summary>
        /// <returns>A hash code for the specified object</returns>
        public override Int32 GetHashCode()
        {
            return (Int32)(_xMin + _yMin + _xMax + _yMax);
        }

        /// <summary>
        /// Returns a String representation of the <see cref="Extents"/> as "(MinX, MinY) (MaxX, MaxY)".
        /// </summary>
        /// <returns>Lower Left: (MinX, MinY) Upper Right: (MaxX, MaxY).</returns>
        public override String ToString()
        {
            if (this == Empty)
            {
                return "[BoundingBox] Empty";
            }

            return
                String.Format(CultureInfo.CurrentCulture,
                              "[BoundingBox] Lower Left: ({0:N}, {1:N}) Upper Right: ({2:N}, {3:N})", Left, Bottom,
                              Right, Top);
        }

        #endregion Object Overrides

        #region Value Operators

        public static Boolean operator ==(Extents box1, Extents box2)
        {
            return box1.Equals(box2);
        }

        public static Boolean operator !=(Extents box1, Extents box2)
        {
            return !box1.Equals(box2);
        }

        #endregion

        #region Intersection Static Method

        /// <summary>
        /// Returns the intersection of two <see cref="Extents"/> instances as a BoundingBox.
        /// </summary>
        /// <param name="b1">The first <see cref="Extents"/> to intersect.</param>
        /// <param name="b2">The second BoundingBox to intersect.</param>
        /// <returns>The BoundingBox which represents the shared area between <paramref name="b1"/> and <paramref name="b2"/>.</returns>
        public static Extents Intersection(GeometryFactory factory, Extents b1, Extents b2)
        {
            if (!b1.Intersects(b2))
            {
                return Empty;
            }

            return new Extents(
                factory,
                b1.Left < b2.Left ? b2.Left : b1.Left,
                b1.Bottom < b2.Bottom ? b2.Bottom : b1.Bottom,
                b1.Right > b2.Right ? b2.Right : b1.Right,
                b1.Top > b2.Top ? b2.Top : b1.Top);
        }

        #endregion Intersection Static Method

        #region Private Helper Methods

        /// <summary>
        /// Checks whether min values are actually smaller than max values and in that case swaps them.
        /// </summary>
        /// <returns>True if the bounding was changed, false otherwise.</returns>
        private void checkMinMax()
        {
            if (_xMin > _xMax)
            {
                Double tmp = _xMin;
                _xMin = _xMax;
                _xMax = tmp;
            }

            if (_yMin > _yMax)
            {
                Double tmp = _yMin;
                _yMin = _yMax;
                _yMax = tmp;
            }
        }

        #endregion Private Helper Methods

        #region IExtents Members

        public Boolean Borders(IExtents other, Tolerance tolerance)
        {
            Extents otherExtents = getAsExtents(other);
            return Borders(otherExtents, tolerance);
        }

        public Boolean Borders(IExtents other)
        {
            Extents otherExtents = getAsExtents(other);
            return Borders(otherExtents);
        }

        public ICoordinate Center
        {
            get
            {
                return IsEmpty
                    ? null
                    : (Min.Add(Max)).Divide(2);
            }
        }

        public Boolean Contains(ICoordinate other, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public Boolean Contains(Tolerance tolerance, params Double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public Boolean Contains(ICoordinate other)
        {
            throw new NotImplementedException();
        }

        public Boolean Contains(params Double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public Double Distance(IExtents extents)
        {
            return Distance(getAsExtents(extents));
        }

        public void ExpandToInclude(IGeometry other)
        {
            Extents otherExtents = getAsExtents(other.Extents);
            ExpandToInclude(otherExtents);
        }

        public void ExpandToInclude(params Double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public Double GetMax(Ordinates ordinate)
        {
            return Max[ordinate];
        }

        public Double GetMin(Ordinates ordinate)
        {
            return Min[ordinate];
        }

        public Double GetSize(params Ordinates[] axes)
        {
            if (axes.Length == 1)
            {
                return GetSize(axes[0]);
            }
            else if (axes.Length == 2)
            {
                return GetSize(axes[0], axes[1]);
            }

            throw new NotSupportedException(
                "The axes vector can only have 1 or 2 components, " +
                "since this is a 2D extents.");
        }

        public Double GetSize(Ordinates axis1, Ordinates axis2, Ordinates axis3)
        {
            throw new NotSupportedException(
                "The axes vector can only have 1 or 2 components, " +
                "since this is a 2D extents.");
        }

        public Double GetSize(Ordinates axis1, Ordinates axis2)
        {
            // The dot product of the axes
            return (GetMax(axis1) - GetMin(axis1))
                * (GetMax(axis2) - GetMin(axis2));
        }

        public Double GetSize(Ordinates axis)
        {
            return GetMax(axis) - GetMin(axis);
        }

        public IExtents Intersection(IExtents extents)
        {
            return Intersection(_factory, this, getAsExtents(extents));
        }

        public Boolean Intersects(Tolerance tolerance, params Double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public Boolean Intersects(params Double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public Boolean Overlaps(ICoordinate other)
        {
            throw new NotImplementedException();
        }

        public Boolean Overlaps(params Double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public void Scale(Double factor, Ordinates axis)
        {
            throw new NotImplementedException();
        }

        public void Scale(Double factor)
        {
            _xMin *= factor;
            _xMax *= factor;
            _yMin *= factor;
            _yMax *= factor;
        }

        public void Scale(params Double[] vector)
        {
            throw new NotImplementedException();
        }

        public void SetToEmpty()
        {
            throw new NotImplementedException();
        }

        IGeometry IExtents.ToGeometry()
        {
            return ToGeometry();
        }

        public void Transform(IMatrix<DoubleComponent> transformMatrix)
        {
            throw new NotImplementedException();
        }

        public void Translate(params Double[] vector)
        {
            throw new NotImplementedException();
        }

        public void TranslateRelativeToWidth(params Double[] vector)
        {
            if (vector.Length > 2)
            {
                throw new ArgumentOutOfRangeException("vector", vector.Length,
                    "This extents only has 2 dimensions, "+
                    "so only 2 or fewer translation values can be accepted.");
            }

            if (vector.Length == 0)
            {
                return;
            }

            Double xOffset = vector[0] * Width;
            Double yOffset = 0;

            if (vector.Length == 2)
            {
                yOffset = vector[1] * Height;
            }

            _xMin += xOffset;
            _xMax += xOffset;

            _yMin += yOffset;
            _yMax += yOffset;
        }

        public IExtents Union(IExtents box)
        {
            return Join(this, getAsExtents(box));
        }

        public IExtents Union(IPoint point)
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

        #region IEquatable<IExtents> Members

        public Boolean Equals(IExtents other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            // value type copies and boxes for a deep copy
            return this;
        }

        #endregion

        private Extents getAsExtents(IExtents other)
        {
            Extents otherExtents;

            if (other is Extents)
            {
                otherExtents = (Extents)other;
            }
            else
            {
                otherExtents = new Extents(_factory,
                                           (ICoordinate2D)other.Min,
                                           (ICoordinate2D)other.Max);
            }

            return otherExtents;
        }
    }
}