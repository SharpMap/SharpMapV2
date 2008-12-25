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
using System.Collections;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using NPack;
using NPack.Interfaces;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Represents a graphical figure, which is a portion of a 
    /// <see cref="Path{TCoordinate}"/>.
    /// </summary>
    /// <typeparam name="TCoordinate">Type of point to use in the figure.</typeparam>
    public class Figure<TCoordinate> : ICloneable,
                                       IEnumerable<TCoordinate>,
                                       IEquatable<Figure<TCoordinate>>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        private readonly ICoordinateSequence<TCoordinate> _points;
        private Boolean _isClosed;
        private Rectangle<TCoordinate> _bounds;

        #region Object Construction

        /// <summary>
        /// Creates a new open <see cref="Figure{TCoordinate, TBounds}"/> 
        /// from the given points.
        /// </summary>
        /// <param name="points">The points from which to create the figure.</param>
        public Figure(IEnumerable<TCoordinate> points)
            : this(points, false) { }

        /// <summary>
        /// Creates a new <see cref="Figure{TCoordinate, TBounds}"/> 
        /// from the given points.
        /// </summary>
        /// <param name="points">The points from which to create the figure.</param>
        /// <param name="isClosed">True to close the path, false to keep it open.</param>
        public Figure(IEnumerable<TCoordinate> points, Boolean isClosed)
        {
            foreach (TCoordinate point in points)
            {
                _points.Add(point);
            }

            IsClosed = isClosed;
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a String representation of this <see cref="Figure"/>.
        /// </summary>
        public override String ToString()
        {
            return
                String.Format("[{0}] Number of {2} points: {1}; Closed: {3}", GetType(), Points.Count,
                              typeof(TCoordinate).Name, IsClosed);
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Returns a value to use in hash sets.
        /// </summary>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hash = 86848163;

                foreach (TCoordinate p in Points)
                {
                    hash ^= p.GetHashCode();
                }

                return hash;
            }
        }

        #endregion

        #region Equality Computation

        public override Boolean Equals(Object obj)
        {
            Figure<TCoordinate> other = obj as Figure<TCoordinate>;
            return Equals(other);
        }

        #region IEquatable<Path<TCoordinate>> Members

        public Boolean Equals(Figure<TCoordinate> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Points.Count != Points.Count)
            {
                return false;
            }

            if (other.IsClosed != IsClosed)
            {
                return false;
            }

            for (Int32 pointIndex = 0; pointIndex < other.Points.Count; pointIndex++)
            {
                if (!Points[pointIndex].Equals(other.Points[pointIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #endregion

        #region Cloning

        /// <summary>
        /// Creates an exact copy of this figure.
        /// </summary>
        /// <returns>A point-by-point copy of this figure.</returns>
        public Figure<TCoordinate> Clone()
        {
            Figure<TCoordinate> figure = CreateFigure(Points, IsClosed);
            return figure;
        }

        #region ICloneable Members

        Object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the bounds of this figure.
        /// </summary>
        public Rectangle<TCoordinate> Bounds
        {
            get
            {
                if (_bounds.IsEmpty)
                {
                    _bounds = ComputeBounds();
                }

                return _bounds;
            }
        }

        /// <summary>
        /// Gets true if the figure is closed, false if open.
        /// </summary>
        public Boolean IsClosed
        {
            get { return _isClosed; }
            protected set { _isClosed = value; }
        }

        /// <summary>
        /// Gets the points in this figure.
        /// </summary>
        public ICoordinateSequence<TCoordinate> Points
        {
            get { return _points.Freeze(); }
        }

        #endregion

        #region Public methods

        public void Add(TCoordinate point)
        {
            _points.Add(point);
        }

        /// <summary>
        /// Marks the figure as closed.
        /// </summary>
        public void Close()
        {
            IsClosed = true;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Computes the minimum bounding rectilinear shape that contains this figure.
        /// </summary>
        /// <returns>
        /// A <see name="Rectangle{TCoordinate}"/> instance describing a minimally bounding 
        /// rectilinear space which contains the figure.
        /// </returns>
        protected Rectangle<TCoordinate> ComputeBounds()
        {
            return new Rectangle<TCoordinate>(_points.Minimum, _points.Maximum);
        }

        /// <summary>
        /// Creates a new figure with the given points, either open or closed.
        /// </summary>
        /// <param name="points">Points to use in sequence to create the figure.</param>
        /// <param name="isClosed">True if the figure is closed, false otherwise.</param>
        /// <returns>A new Figure instance.</returns>
        protected Figure<TCoordinate> CreateFigure(IEnumerable<TCoordinate> points,
                                                   Boolean isClosed)
        {
            return new Figure<TCoordinate>(_points, isClosed);
        }

        #endregion

        internal void TransformPoints(ITransformMatrix<DoubleComponent> transform)
        {
            for (Int32 i = 0; i < _points.Count; i++)
            {
                TCoordinate point = _points[i];

                DoubleComponent[] pointComponents = point.Components;
                Array.Resize(ref pointComponents, pointComponents.Length + 1);
                pointComponents[pointComponents.Length - 1] = 1;
                transform.TransformVector(pointComponents);
                Array.Resize(ref pointComponents, pointComponents.Length - 1);
                point.Components = pointComponents;
                _points[i] = point;
            }
        }

        #region IEnumerable<TCoordinate> Members

        public IEnumerator<TCoordinate> GetEnumerator()
        {
            foreach (TCoordinate p in _points)
            {
                yield return p;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}