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
    public abstract class GraphicsFigure<TViewPoint, TViewBounds> : ICloneable, IEnumerable<TViewPoint>, IEquatable<GraphicsFigure<TViewPoint, TViewBounds>>
        where TViewPoint : IViewVector
        where TViewBounds : IViewMatrix
    {
        private List<TViewPoint> _points = new List<TViewPoint>();
        private bool _isClosed;

        public GraphicsFigure(IEnumerable<TViewPoint> points)
            : this(points, false) { }

        public GraphicsFigure(IEnumerable<TViewPoint> points, bool isClosed)
        {
            _points.AddRange(points);
            _isClosed = isClosed;
        }

        public bool IsClosed
        {
            get { return _isClosed; }
            protected set { _isClosed = value; }
        }

        public TViewBounds Bounds
        {
            get { return ComputeBounds(); }
        }

        public IList<TViewPoint> Points
        {
            get { return _points; }
        }

        protected abstract TViewBounds ComputeBounds();
        protected abstract GraphicsFigure<TViewPoint, TViewBounds> CreateFigure(IEnumerable<TViewPoint> points, bool isClosed);

        public GraphicsFigure<TViewPoint, TViewBounds> Clone()
        {
            List<TViewPoint> pointsCopy = new List<TViewPoint>(_points);
            GraphicsFigure<TViewPoint, TViewBounds> path = CreateFigure(pointsCopy, IsClosed);
            return path;
        }

        public override bool Equals(object obj)
        {
            GraphicsFigure<TViewPoint, TViewBounds> other = obj as GraphicsFigure<TViewPoint, TViewBounds>;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 86848163;
                foreach (TViewPoint p in Points)
                    hash ^= p.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}] {1} {2} points", GetType(), Points.Count, typeof(TViewPoint).Name);
        }

        #region IEnumerable<TViewPoint> Members

        public IEnumerator<TViewPoint> GetEnumerator()
        {
            foreach (TViewPoint p in _points)
                yield return p;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEquatable<GraphicsPath<TViewPoint>> Members

        public bool Equals(GraphicsFigure<TViewPoint, TViewBounds> other)
        {
            if (other == null)
                return false;

            if (other.Points.Count != this.Points.Count)
                return false;

            for (int pointIndex = 0; pointIndex < other.Points.Count; pointIndex++)
            {
                if (!this.Points[pointIndex].Equals(other.Points[pointIndex]))
                    return false;
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
