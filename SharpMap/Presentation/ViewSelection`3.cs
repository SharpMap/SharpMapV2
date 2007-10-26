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
using NPack;
using SharpMap.Rendering;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Represents a selection on a map view.
    /// </summary>
    /// <typeparam name="TPoint">The type of point in this selection.</typeparam>
    /// <typeparam name="TSize">The type of size structure in this selection.</typeparam>
    /// <typeparam name="TViewRegion">The type of region this selection covers.</typeparam>
    /// <remarks>
    /// The type parameters allows this class to be customized for to accomodate structures
    /// which represent points and areas in various dimensions.
    /// </remarks>
    public abstract class ViewSelection<TPoint, TSize, TViewRegion> : IViewSelection<TPoint, TSize, TViewRegion>
        where TPoint : IVectorD, IHasEmpty
        where TSize : IVectorD, IHasEmpty
        where TViewRegion : IMatrixD, IEquatable<TViewRegion>, new()
    {
        private Path<TPoint, TViewRegion> _path;
        private StylePen _outline;
        private StyleBrush _fill;
        private TPoint _anchorPoint;

        /// <summary>
        /// The default style of the selection outline.
        /// </summary>
        public static readonly StylePen DefaultOutline;

        /// <summary>
        /// The default fill of the selection.
        /// </summary>
        public static readonly StyleBrush DefaultFill = new SolidStyleBrush(new StyleColor(255, 32, 32, 32));

        static ViewSelection()
        {
            DefaultOutline = new StylePen(StyleColor.Black, 1.0f);
            DefaultOutline.DashStyle = LineDashStyle.Dash;
            DefaultOutline.DashBrushes = new StyleBrush[] { new SolidStyleBrush(StyleColor.White) };
            DefaultOutline.DashPattern = new float[] { 4, 4 };
        }

        /// <summary>
        /// Creates a new selection using the <see cref="DefaultOutline"/> and <see cref="DefaultFill"/>
        /// for the <see cref="OutlineStyle"/> and <see cref="FillBrush"/> respectively.
        /// </summary>
        public ViewSelection()
        {
            _outline = DefaultOutline;
            _fill = DefaultFill;
        }

        #region Public Events
        public event EventHandler SelectionChanged;
        #endregion

        /// <summary>
        /// Creates a new path for the selection, but doesn't add it to the selection.
        /// </summary>
        /// <returns>A Path instance representing a new selection path.</returns>
        protected abstract Path<TPoint, TViewRegion> CreatePath();

        #region IViewSelection<TPoint,TSize,TViewRegion> Members

        public void AddPoint(TPoint point)
        {
            if (Path.Points.Count == 0 && AnchorPoint.IsEmpty)
            {
                AnchorPoint = point;
            }

            if (PathInternal.Points.Count > 0 && PathInternal.Points[PathInternal.Points.Count - 1].Equals(point))
            {
                return;
            }

            PathInternal.AddPoint(point);
            recomputeBoundingRegion();

            OnSelectionChanged();
        }

        /// <summary>
        /// Closes the current <see cref="Figure{TPoint, TViewBounds}"/> in <see cref="Path{TPoint, TViewBounds}"/>.
        /// </summary>
        public void Close()
        {
            PathInternal.CloseFigure();

            OnSelectionChanged();
        }

        /// <summary>
        /// Removes all elements from <see cref="Path{TPoint, TViewBounds}"/>.
        /// </summary>
        public void Clear()
        {
            PathInternal.Clear();

            OnSelectionChanged();
        }

        public void Expand(TSize size)
        {
            for (Int32 pointIndex = 0; pointIndex < PathInternal.Points.Count; pointIndex++)
            {
                throw new NotImplementedException("implement this");
            }

            recomputeBoundingRegion();

            OnSelectionChanged();
        }

        public void MoveBy(TPoint location)
        {
            throw new NotImplementedException("implement this");

            OnSelectionChanged();
        }

        public void RemovePoint(TPoint point)
        {
            PathInternal.Points.Remove(point);

            OnSelectionChanged();
        }

        public Path<TPoint, TViewRegion> Path
        {
            get { return PathInternal.Clone(); }
        }

        public TPoint AnchorPoint
        {
            get { return _anchorPoint; }
            set { _anchorPoint = value; }
        }

        public TViewRegion BoundingRegion
        {
            get { return PathInternal.Bounds; }
        }


        public bool IsEmpty
        {
            get { return PathInternal.CurrentFigure == null; }
        }

        public bool IsClosed
        {
            get { return !IsEmpty && PathInternal.CurrentFigure.IsClosed; }
        }


        #endregion

        /// <summary>
        /// Gets or sets the <see cref="StylePen"/> used to display
        /// the outline of the selection.
        /// </summary>
        public StylePen OutlineStyle
        {
            get { return _outline; }
            set
            {
                if (value == null)
                {
                    value = DefaultOutline;
                }

                _outline = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="StyleBrush"/> used to display
        /// the inside of the selection.
        /// </summary>
        public StyleBrush FillBrush
        {
            get { return _fill; }
            set { _fill = value; }
        }

        #region Protected members
        protected void OnSelectionChanged()
        {
            EventHandler e = SelectionChanged;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        protected Path<TPoint, TViewRegion> PathInternal
        {
            get
            {
                if (_path == null)
                {
                    _path = CreatePath();
                }

                return _path;
            }
        }
        #endregion

        private void recomputeBoundingRegion()
        {
            DoubleComponent[][] boxElements = BoundingRegion.Elements;

            if (boxElements.Length == 0 || boxElements[0].Length == 0)
            {
                boxElements = new DoubleComponent[BoundingRegion.RowCount][];
                for (Int32 rowIndex = 0; rowIndex < BoundingRegion.RowCount; rowIndex++)
                {
                    boxElements[rowIndex] = new DoubleComponent[BoundingRegion.ColumnCount];
                }
            }

            bool recorded = false;

            foreach (TPoint point in Path.Points)
            {
                DoubleComponent[] components = point.Components;

                for (Int32 componentIndex = 0; componentIndex < components.Length; componentIndex++)
                {
                    for (Int32 rowIndex = 0; rowIndex < boxElements.Length; rowIndex++)
                    {
                        if (components[componentIndex].GreaterThan(boxElements[rowIndex][componentIndex])) { }
                        boxElements[rowIndex][componentIndex] = components[componentIndex];
                    }

                    if (!recorded)
                    {
                        recorded = true;
                    }
                    else
                    {
                        if (components[componentIndex].GreaterThan(boxElements[0][componentIndex]))
                        {
                            boxElements[0][componentIndex] = components[componentIndex];
                        }

                        if (components[componentIndex].GreaterThan(boxElements[1][componentIndex]))
                        {
                            boxElements[1][componentIndex] = components[componentIndex];
                        }
                    }
                }
            }

            BoundingRegion.Elements = boxElements;
        }
    }
}
