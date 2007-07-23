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

using SharpMap.Rendering;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Represents a selection on a map view.
    /// </summary>
    /// <typeparam name="TViewPoint">The type of point in this selection.</typeparam>
    /// <typeparam name="TViewSize">The type of size structure in this selection.</typeparam>
    /// <typeparam name="TViewRegion">The type of region this selection covers.</typeparam>
    public interface IViewSelection<TViewPoint, TViewSize, TViewRegion>
        where TViewPoint : IVectorD
        where TViewSize : IVectorD
        where TViewRegion : IMatrixD, IEquatable<TViewRegion>
    {
        /// <summary>
        /// Adds a point to the selection.
        /// </summary>
        /// <param name="point">A point to add.</param>
        void AddPoint(TViewPoint point);

        /// <summary>
        /// Expands the selection by given size. If the size is negative, it contracts the selection.
        /// </summary>
        /// <param name="size">Amount to expand the selection by.</param>
        void Expand(TViewSize size);

        /// <summary>
        /// Moves the selection by the given offset.
        /// </summary>
        /// <param name="location">Point vector to move selection by.</param>
        void MoveBy(TViewPoint offset);

        /// <summary>
        /// Removes a point from the selection.
        /// </summary>
        /// <param name="point">Point to remove.</param>
        void RemovePoint(TViewPoint point);

        /// <summary>
        /// Path which represents the outline of the selection on the view surface.
        /// </summary>
        GraphicsPath<TViewPoint, TViewRegion> Path { get; }

        /// <summary>
        /// Point around which selection is transformed.
        /// </summary>
        TViewPoint AnchorPoint { get; set; }

        /// <summary>
        /// A minimum bounding box for the selection.
        /// </summary>
        TViewRegion BoundingRegion { get; }
    }
}
