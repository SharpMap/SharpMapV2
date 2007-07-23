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

using SharpMap.Rendering;
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using NPack;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Represents a selection on a map view.
    /// </summary>
    /// <typeparam name="TViewPoint">The type of point in this selection.</typeparam>
    /// <typeparam name="TViewSize">The type of size structure in this selection.</typeparam>
    /// <typeparam name="TViewRegion">The type of region this selection covers.</typeparam>
    /// <remarks>
    /// The type parameters allows this class to be customized for to accomodate structures
    /// which represent points and areas in various dimensions.
    /// </remarks>
    public abstract class ViewSelection<TViewPoint, TViewSize, TViewRegion> : IViewSelection<TViewPoint, TViewSize, TViewRegion>
        where TViewPoint : IVectorD, IHasEmpty
        where TViewSize : IVectorD, IHasEmpty
        where TViewRegion : IMatrixD, IEquatable<TViewRegion>, new()
    {
        private GraphicsPath<TViewPoint, TViewRegion> _path;
        private StylePen _outline;
        private StyleBrush _fill;
        private TViewPoint _anchorPoint;
        private TViewRegion _boundingRegion = new TViewRegion();

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
            _path = CreatePath();
            _outline = DefaultOutline;
            _fill = DefaultFill;
        }

        /// <summary>
        /// Creates a new path for the selection, but doesn't add it to the selection.
        /// </summary>
        /// <returns>A GraphicsPath instance representing a new selection path.</returns>
        protected abstract GraphicsPath<TViewPoint, TViewRegion> CreatePath();

        #region IViewSelection<TViewPoint,TViewSize,TViewRegion> Members
        public void AddPoint(TViewPoint point)
        {
            if (Path.Points.Count == 0 && AnchorPoint.IsEmpty)
            {
                AnchorPoint = point;
            }

            Path.CurrentFigure.Add(point);
            recomputeBoundingRegion();
        }

        public void Expand(TViewSize size)
        {
            for (int pointIndex = 0; pointIndex < Path.Points.Count; pointIndex++)
            {
                throw new NotImplementedException("implement this");
            }

            recomputeBoundingRegion();
        }

        public void MoveBy(TViewPoint location)
        {
            throw new NotImplementedException("implement this");
        }

        public void RemovePoint(TViewPoint point)
        {
            _path.Points.Remove(point);
        }

        public GraphicsPath<TViewPoint, TViewRegion> Path
        {
            get { return _path.Clone(); }
        }

        public TViewPoint AnchorPoint
        {
            get { return _anchorPoint; }
            set { _anchorPoint = value; }
        }

        public TViewRegion BoundingRegion
        {
            get { return _boundingRegion; }
        }

        #endregion

        public StylePen OutlineStyle
        {
            get { return _outline; }
            set
            {
                if (value == null)
                    value = DefaultOutline;

                _outline = value;
            }
        }

        public StyleBrush FillBrush
        {
            get { return _fill; }
            private set { _fill = value; }
        }

        private void recomputeBoundingRegion()
        {
            DoubleComponent[][] boxElements = BoundingRegion.Elements;
            bool recorded = false;

            foreach (TViewPoint point in Path.Points)
            {
                DoubleComponent[] components = point.Components;

                for (int componentIndex = 0; componentIndex < components.Length; componentIndex++)
                {
                    for (int rowIndex = 0; rowIndex < boxElements.Length; rowIndex++)
                    {
                        if (components[componentIndex].GreaterThan(boxElements[rowIndex][componentIndex]))
                        {

                        }
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
