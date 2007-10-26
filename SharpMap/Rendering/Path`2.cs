// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
    /// Represents a series of figures of connected points in an 
    /// n-dimensional vector space, which is set by generic parameters.
    /// </summary>
    /// <typeparam name="TPoint">Type of point used in the path.</typeparam>
    /// <typeparam name="TViewBounds">Type of rectilinear used to bound the path.</typeparam>
    public abstract class Path<TPoint, TViewBounds> : ICloneable,
                                                      IEnumerable<Figure<TPoint, TViewBounds>>,
                                                      IEquatable<Path<TPoint, TViewBounds>>
        where TPoint : IVectorD
        where TViewBounds : IMatrixD, IEquatable<TViewBounds>
    {
        private readonly List<Figure<TPoint, TViewBounds>> _figures =
            new List<Figure<TPoint, TViewBounds>>();

        private Int32 _currentFigureIndex;
        private TViewBounds _bounds;

        /// <summary>
        /// Creates a new, empty <see cref="Path{TPoint, TViewBounds}"/>.
        /// </summary>
        protected Path() {}

        /// <summary>
        /// Creates a new, open <see cref="Path{TPoint, TViewBounds}"/> 
        /// with the given points.
        /// </summary>
        /// <param name="points">Points to add to the path in sequence.</param>
        protected Path(IEnumerable<TPoint> points)
            : this(points, false) {}

        /// <summary>
        /// Creates a new <see cref="Path{TPoint, TViewBounds}"/> 
        /// with the given points, as closed or open.
        /// </summary>
        /// <param name="points">
        /// Points to add to the path in sequence.
        /// </param>
        /// <param name="isClosed">
        /// True to create a closed path, false for an open path.
        /// </param>
        protected Path(IEnumerable<TPoint> points, Boolean isClosed)
        {
            Figure<TPoint, TViewBounds> figure = CreateFigure(points, isClosed);
            _figures.Add(figure);
            _currentFigureIndex = 0;
        }

        /// <summary>
        /// Creates a new <see cref="Path{TPoint, TViewBounds}"/> with the given 
        /// <see cref="Figure{TPoint, TRectangle}"/> instance.
        /// </summary>
        /// <param name="figure">A figure to create the path from.</param>
        protected Path(Figure<TPoint, TViewBounds> figure)
            : this(new Figure<TPoint, TViewBounds>[] {figure}) {}

        /// <summary>
        /// Creates a new <see cref="Path{TPoint, TViewBounds}"/> with the given 
        /// <see cref="Path{TPoint, TViewBounds}"/> instances.
        /// </summary>
        /// <param name="figures">An enumeration of figures to create the path from.</param>
        protected Path(IEnumerable<Figure<TPoint, TViewBounds>> figures)
        {
            _figures.AddRange(figures);

            if (_figures.Count == 0)
            {
                _currentFigureIndex = -1;
            }
            else
            {
                _currentFigureIndex = 0;
            }
        }

        /// <summary>
        /// Provides a String representation of the 
        /// <see cref="Path{TPoint, TViewBounds}"/>.
        /// </summary>
        /// <returns>A String which describes the <see cref="Path{TPoint, TViewBounds}"/></returns>
        public override String ToString()
        {
            return String.Format("[{0}] {1} figure{2} of {3} points; Bounds: {4}",
                                 GetType(), _figures.Count, (_figures.Count > 1 ? "s" : ""), typeof (TPoint).Name,
                                 ComputeBounds());
        }

        /// <summary>
        /// Returns the hash code for the instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code suitable for use in hash tables.
        /// </returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                Int32 hash = 19638952;
                foreach (Figure<TPoint, TViewBounds> figure in _figures)
                {
                    hash ^= figure.GetHashCode();
                    hash ^= figure.IsClosed ? 729487 : 9245;
                }

                return hash;
            }
        }

        #region Equality Computation

        /// <summary>
        /// Returns true if two <see cref="Path{TPoint, TViewBounds}"/> 
        /// instances are figure-for-figure, point-for-point identical.
        /// </summary>
        /// <remarks>
        /// Casts <paramref name="obj"/> to a <see cref="Path{TPoint, TViewBounds}"/> 
        /// and calls <see cref="Equals(Path{TPoint, TViewBounds})"/> with the result.
        /// </remarks>
        /// <param name="obj">The value to compare.</param>
        /// <returns>True if the parameter is a path which is figure-for-figure, point-for-point equal.</returns>
        public override Boolean Equals(object obj)
        {
            Path<TPoint, TViewBounds> other = obj as Path<TPoint, TViewBounds>;
            return Equals(other);
        }

        #region IEquatable<Path<TPoint, TViewBounds>> Members

        /// <summary>
        /// Returns true if two <see cref="Path{TPoint, TViewBounds}"/> instances are figure-for-figure, 
        /// point-for-point identical.
        /// </summary>
        /// <param name="other">The <see cref="Path{TPoint, TViewBounds}"/> to compare.</param>
        /// <returns>True if the two paths are figure-for-figure, point-for-point equal.</returns>
        public Boolean Equals(Path<TPoint, TViewBounds> other)
        {
            if (other == null)
            {
                return false;
            }

            if (other._figures.Count != _figures.Count)
            {
                return false;
            }

            unchecked
            {
                for (Int32 figureIndex = 0; figureIndex < other._figures.Count; figureIndex++)
                {
                    if (!_figures[figureIndex].Equals(other._figures[figureIndex]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets a list of the <see cref="Figure{TPoint, TViewBounds}"/> in this path.
        /// </summary>
        public IList<Figure<TPoint, TViewBounds>> Figures
        {
            get { return _figures.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the points in the <see cref="CurrentFigure"/>.
        /// </summary>
        public IList<TPoint> Points
        {
            get
            {
                if (CurrentFigure == null)
                {
                    return new TPoint[] {};
                }

                return CurrentFigure.Points;
            }
        }

        /// <summary>
        /// Gets or sets the current figure in the path.
        /// </summary>
        public Figure<TPoint, TViewBounds> CurrentFigure
        {
            get
            {
                if (_figures.Count == 0)
                {
                    return null;
                }

                return _figures[_currentFigureIndex];
            }
            set
            {
                Int32 index = _figures.IndexOf(value);

                if (index < 0)
                {
                    throw new InvalidOperationException("CurrentFigure must be an existing figure in this path.");
                }

                _currentFigureIndex = index;
            }
        }

        /// <summary>
        /// Gets the bounds of the path.
        /// </summary>
        public TViewBounds Bounds
        {
            get
            {
                if (EmptyBounds.Equals(_bounds))
                {
                    _bounds = ComputeBounds();
                }

                return _bounds;
            }
        }

        /// <summary>
        /// Creates a <see cref="Path{TPoint, TViewBounds}"/> with the given figures.
        /// </summary>
        /// <param name="figures">Figures used in creating the path.</param>
        /// <returns>A new path with the given <paramref name="figures"/>.</returns>
        protected abstract Path<TPoint, TViewBounds> CreatePath(
            IEnumerable<Figure<TPoint, TViewBounds>> figures);

        /// <summary>
        /// Creates a new <see cref="Figure{TPoint, TViewBounds}"/> 
        /// with the given points, either open or closed.
        /// </summary>
        /// <param name="points">Points in the figure.</param>
        /// <param name="isClosed">
        /// True if the figure wraps from the last point to the first point, 
        /// false if it is open.
        /// </param>
        /// <returns>A new figure with the given points and open or closed condition.</returns>
        protected abstract Figure<TPoint, TViewBounds> CreateFigure(IEnumerable<TPoint> points,
                                                                    Boolean isClosed);

        /// <summary>
        /// Computes a minimum bounding box for this path.
        /// </summary>
        /// <returns>
        /// A <typeparamref name="TViewBounds"/> instance which is the minimum 
        /// bounding n-dimensional rectilinear shape encompassing this path.
        /// </returns>
        protected abstract TViewBounds ComputeBounds();

        /// <summary>
        /// Gets a value indicating an empty bounds shape.
        /// </summary>
        protected abstract TViewBounds EmptyBounds { get; }

        /// <summary>
        /// Creates a new Figure from the given <paramref name="points"/>, 
        /// either closed or open, and adds it to this path.
        /// </summary>
        /// <remarks>
        /// Also sets the <see cref="CurrentFigure"/> to the added figure.
        /// </remarks>
        /// <param name="points">
        /// Points to make the Figure from.
        /// </param>
        /// <param name="closeFigure">True to close the figure, false to keep it open.</param>
        public void NewFigure(IEnumerable<TPoint> points, Boolean closeFigure)
        {
            _figures.Add(CreateFigure(points, closeFigure));
            _currentFigureIndex = _figures.Count - 1;
            _bounds = EmptyBounds;
        }

        /// <summary>
        /// Clones this <see cref="Path{TPoint, TViewBounds}"/>.
        /// </summary>
        /// <returns>A new Path with identical figures.</returns>
        public Path<TPoint, TViewBounds> Clone()
        {
            List<Figure<TPoint, TViewBounds>> figuresCopy =
                new List<Figure<TPoint, TViewBounds>>(_figures.Count);

            foreach (Figure<TPoint, TViewBounds> figure in _figures)
            {
                figuresCopy.Add(figure.Clone());
            }

            Path<TPoint, TViewBounds> path = CreatePath(figuresCopy);
            return path;
        }

        /// <summary>
        /// Adds a point to the current figure, creating one if the path has no figures.
        /// </summary>
        /// <param name="point">The <typeparamref name="TPoint"/> to add.</param>
        public void AddPoint(TPoint point)
        {
            if (CurrentFigure == null)
            {
                NewFigure(new TPoint[] {point}, false);
            }
            else
            {
                _figures[_currentFigureIndex].Add(point);
            }
        }

        /// <summary>
        /// Removes all elements from <see cref="Path{TPoint, TViewBounds}"/>.
        /// </summary>
        public void Clear()
        {
            _figures.Clear();
        }

        /// <summary>
        /// Closes the current <see cref="Figure{TPoint, TViewBounds}"/>.
        /// </summary>
        public void CloseFigure()
        {
            CurrentFigure.Close();
        }

        public void TransformPoints(ITransformMatrix<DoubleComponent> transform)
        {
            foreach (Figure<TPoint, TViewBounds> figure in _figures)
            {
                figure.TransformPoints(transform);
            }
        }

        #region IEnumerable<Figure<TPoint,TViewBounds>> Members

        /// <summary>
        /// Enumerates the figures in this path.
        /// </summary>
        /// <returns>An enumerator over this path's <see cref="Figure{TPoint, TViewBounds}">figures</see>.</returns>
        public IEnumerator<Figure<TPoint, TViewBounds>> GetEnumerator()
        {
            foreach (Figure<TPoint, TViewBounds> figure in _figures)
            {
                yield return figure;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}