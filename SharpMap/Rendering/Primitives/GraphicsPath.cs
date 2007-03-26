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
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public abstract class GraphicsPath<TViewPoint, TViewBounds> : ICloneable, IEnumerable<GraphicsFigure<TViewPoint, TViewBounds>>, IEquatable<GraphicsPath<TViewPoint, TViewBounds>>
        where TViewPoint : IViewVector
        where TViewBounds : IViewMatrix
    {
        private List<GraphicsFigure<TViewPoint, TViewBounds>> _figures = new List<GraphicsFigure<TViewPoint, TViewBounds>>();
        private int _currentFigureIndex;

        public GraphicsPath() { }
        
        public GraphicsPath(IEnumerable<TViewPoint> points)
            : this(points, false) { }

        public GraphicsPath(IEnumerable<TViewPoint> points, bool isClosed)
        {
            GraphicsFigure<TViewPoint, TViewBounds> figure = CreateFigure(points, isClosed);
            Figures.Add(figure);
            _currentFigureIndex = 0;
        }

        public GraphicsPath(GraphicsFigure<TViewPoint, TViewBounds> figure)
            : this(new GraphicsFigure<TViewPoint, TViewBounds>[] { figure }) { }

        public GraphicsPath(IEnumerable<GraphicsFigure<TViewPoint, TViewBounds>> figures)
        {
            _figures.AddRange(figures);
            if (_figures.Count == 0)
                _currentFigureIndex = -1;
            else
                _currentFigureIndex = 0;
        }

        public IList<GraphicsFigure<TViewPoint, TViewBounds>> Figures
        {
            get { return _figures; }
        }

        public IList<TViewPoint> Points
        {
            get { return Figures[CurrentFigure].Points; }
        }

        public int CurrentFigure
        {
            get { return _currentFigureIndex; }
            set
            {
                if (_currentFigureIndex < 0 || _currentFigureIndex >= Figures.Count)
                    throw new ArgumentOutOfRangeException("CurrentFigure", value, "CurentFigure must be the index of an existing figure in this path.");

                _currentFigureIndex = value;
            }
        }

        public TViewBounds Bounds
        {
            get { return ComputeBounds(); }
        }

        /// <summary>
        /// Creates a <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with a single figure.
        /// </summary>
        /// <param name="points">Points in the path.</param>
        /// <param name="isClosed">True if the path wraps from the last point to the first point, false if it is open.</param>
        /// <returns>A <see cref="GraphicsPath{TViewPoint, TViewBounds}"/> with one figure consisting of the points in <paramref name="points"/>.</returns>
        protected abstract GraphicsPath<TViewPoint, TViewBounds> CreatePath(IEnumerable<TViewPoint> points, bool isClosed);
        protected abstract GraphicsPath<TViewPoint, TViewBounds> CreatePath(IEnumerable<GraphicsFigure<TViewPoint, TViewBounds>> figures);
        protected abstract GraphicsFigure<TViewPoint, TViewBounds> CreateFigure(IEnumerable<TViewPoint> points, bool isClosed);

        /// <summary>
        /// Computes a minimum bounding box for this path.
        /// </summary>
        /// <returns>A <typeparamref name="TViewBounds"/> instance which is the minimum bounding 
        /// n-dimensional rectilinear shape encompassing this path.</returns>
        protected abstract TViewBounds ComputeBounds();

        public void NewFigure(IEnumerable<TViewPoint> points, bool closeFigure)
        {
            Figures.Add(CreateFigure(points, closeFigure));
            _currentFigureIndex = Figures.Count - 1;
        }

        /// <summary>
        /// Clones this <see cref="GraphicsPath{TViewPoint, TViewBounds}"/>
        /// </summary>
        /// <returns>A new GraphicsPath with identical figures.</returns>
        public GraphicsPath<TViewPoint, TViewBounds> Clone()
        {
            List<GraphicsFigure<TViewPoint, TViewBounds>> figuresCopy = new List<GraphicsFigure<TViewPoint, TViewBounds>>(Figures.Count);

            foreach (GraphicsFigure<TViewPoint, TViewBounds> figure in Figures)
                figuresCopy.Add(figure.Clone());

            GraphicsPath<TViewPoint, TViewBounds> path = CreatePath(figuresCopy);
            return path;
        }

        public override bool Equals(object obj)
        {
            GraphicsPath<TViewPoint, TViewBounds> other = obj as GraphicsPath<TViewPoint, TViewBounds>;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 19638952;
                foreach (GraphicsFigure<TViewPoint, TViewBounds> figure in _figures)
                    hash ^= figure.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format("GraphicsPath - {0} figures of {1} points; Bounds: {2}", Figures.Count, typeof(TViewPoint).Name, ComputeBounds());
        }

        #region IEnumerable<GraphicsFigure<TViewPoint,TViewBounds>> Members

        public IEnumerator<GraphicsFigure<TViewPoint, TViewBounds>> GetEnumerator()
        {
            foreach (GraphicsFigure<TViewPoint, TViewBounds> figure in Figures)
                yield return figure;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEquatable<GraphicsPath<TViewPoint>> Members

        public bool Equals(GraphicsPath<TViewPoint, TViewBounds> other)
        {
            if (other == null)
                return false;

            if (other.Figures.Count != this.Figures.Count)
                return false;

            unchecked
            {
                for (int figureIndex = 0; figureIndex < other.Figures.Count; figureIndex++)
                {
                    if (!this.Figures[figureIndex].Equals(other.Figures[figureIndex]))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
