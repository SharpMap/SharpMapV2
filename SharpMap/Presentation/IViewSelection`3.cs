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
    public interface IViewSelection<TPoint, TSize, TViewRegion>
        where TPoint : IVectorD
        where TSize : IVectorD
        where TViewRegion : IMatrixD, IEquatable<TViewRegion>
    {
        /// <summary>
        /// Adds a point to the selection.
        /// </summary>
        /// <param name="point">A point to add.</param>
        void AddPoint(TPoint point);

        /// <summary>
        /// Expands the selection by given size. If the size is negative, it contracts the selection.
        /// </summary>
        /// <param name="size">Amount to expand the selection by.</param>
        void Expand(TSize size);

        /// <summary>
        /// Moves the selection by the given offset.
        /// </summary>
        /// <param name="offset">Point vector to move selection by.</param>
        void MoveBy(TPoint offset);

        /// <summary>
        /// Removes a point from the selection.
        /// </summary>
        /// <param name="point">Point to remove.</param>
        void RemovePoint(TPoint point);

        /// <summary>
        /// Path which represents the outline of the selection on the view surface.
        /// </summary>
        GraphicsPath<TPoint, TViewRegion> Path { get; }

        /// <summary>
        /// Point around which selection is transformed.
        /// </summary>
        TPoint AnchorPoint { get; set; }

        /// <summary>
        /// A minimum bounding box for the selection.
        /// </summary>
        TViewRegion BoundingRegion { get; }

        void Close();
        void Clear();
        bool IsEmpty { get; }
        bool IsClosed { get; }

        event EventHandler SelectionChanged;
    }
}
