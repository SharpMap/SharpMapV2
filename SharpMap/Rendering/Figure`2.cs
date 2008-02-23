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
using NPack;
using NPack.Interfaces;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Represents a graphical figure, which is a portion of a 
    /// <see cref="Path{TPoint,TViewBounds}"/>.
    /// </summary>
    /// <typeparam name="TPoint">Type of point to use in the figure.</typeparam>
    /// <typeparam name="TViewBounds">Type of rectilinear shape to bound this figure.</typeparam>
    public abstract class Figure<TPoint, TViewBounds>
        : ICloneable, IEnumerable<TPoint>, IEquatable<Figure<TPoint, TViewBounds>>
        where TPoint : IVectorD
        where TViewBounds : IMatrixD
    {
        private readonly List<TPoint> _points = new List<TPoint>();
        private Boolean _isClosed;
        private TViewBounds _bounds;

        #region Object Construction

        /// <summary>
        /// Creates a new open <see cref="Figure{TPoint, TViewBounds}"/> 
        /// from the given points.
        /// </summary>
        /// <param name="points">The points from which to create the figure.</param>
        public Figure(IEnumerable<TPoint> points)
            : this(points, false)
        {
        }

        /// <summary>
        /// Creates a new <see cref="Figure{TPoint, TViewBounds}"/> 
        /// from the given points.
        /// </summary>
        /// <param name="points">The points from which to create the figure.</param>
        /// <param name="isClosed">True to close the path, false to keep it open.</param>
        public Figure(IEnumerable<TPoint> points, Boolean isClosed)
        {
            _points.AddRange(points);
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
                              typeof (TPoint).Name, IsClosed);
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

                foreach (TPoint p in Points)
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
            Figure<TPoint, TViewBounds> other = obj as Figure<TPoint, TViewBounds>;
            return Equals(other);
        }

        #region IEquatable<Path<TPoint>> Members

        public Boolean Equals(Figure<TPoint, TViewBounds> other)
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
        public Figure<TPoint, TViewBounds> Clone()
        {
            Figure<TPoint, TViewBounds> figure = CreateFigure(Points, IsClosed);
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
        public TViewBounds Bounds
        {
            get
            {
                if (ReferenceEquals(_bounds, null) || _bounds.Equals(EmptyBounds))
                {
                    _bounds = ComputeBounds();
                }

                return _bounds;
            }
        }

        /// <summary>
        /// Gets a value indicating an empty bounds shape.
        /// </summary>
        protected abstract TViewBounds EmptyBounds { get; }

        /// <summary>
        /// Gets true if the figure is closed, false if open.
        /// </summary>
        public Boolean IsClosed
        {
            get { return _isClosed; }
            protected set { _isClosed = value; }
        }

        /// <summary>
        /// A list of the points in this figure.
        /// </summary>
        public IList<TPoint> Points
        {
            get { return _points.AsReadOnly(); }
        }

        #endregion

        #region Public methods

        public void Add(TPoint point)
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
        /// A <typeparamref name="TViewBounds"/> instance describing a minimally bounding 
        /// rectilinear space which contains the figure.
        /// </returns>
        protected abstract TViewBounds ComputeBounds();

        /// <summary>
        /// Creates a new figure with the given points, either open or closed.
        /// </summary>
        /// <param name="points">Points to use in sequence to create the figure.</param>
        /// <param name="isClosed">True if the figure is closed, false otherwise.</param>
        /// <returns>A new Figure instance.</returns>
        protected abstract Figure<TPoint, TViewBounds> CreateFigure(IEnumerable<TPoint> points,
                                                                                Boolean isClosed);

        #endregion

        internal List<TPoint> PointsInternal
        {
            get { return _points; }
        }

        internal void TransformPoints(ITransformMatrix<DoubleComponent> transform)
        {
            for (Int32 i = 0; i < _points.Count; i++)
            {
                TPoint point = _points[i];

                DoubleComponent[] pointComponents = point.Components;
                Array.Resize(ref pointComponents, pointComponents.Length + 1);
                pointComponents[pointComponents.Length - 1] = 1;
                transform.TransformVector(pointComponents);
                Array.Resize(ref pointComponents, pointComponents.Length - 1);
                point.Components = pointComponents;
                _points[i] = point;
            }
        }

        #region IEnumerable<TPoint> Members

        public IEnumerator<TPoint> GetEnumerator()
        {
            foreach (TPoint p in _points)
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