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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using System.Collections.Generic;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer of label data.
    /// </summary>
    /// <typeparam name="TPoint">
    /// Type of point vector used by the graphical display coordinate system.
    /// </typeparam>
    /// <typeparam name="TSize">
    /// Type of size vector used by the graphical display coordinate system.
    /// </typeparam>
    /// <typeparam name="TRectangle">
    /// Type of rectangle matrix used by the graphical display coordinate system.
    /// </typeparam>
    /// <typeparam name="TRenderObject">
    /// Type of object used by the graphical display coordinate system to render spatial items.
    /// </typeparam>
    public interface ILabelRenderer<TPoint, TSize, TRectangle, TRenderObject>
        where TPoint : IVectorD
		where TSize : IVectorD
        where TRectangle : IMatrixD, IEquatable<TRectangle>
    {
        /// <summary>
        /// Renders a label.
        /// </summary>
        /// <param name="label">Label to render.</param>
        /// <returns>A <typeparamref name="TRenderObject"/> used to draw the label.</returns>
        IEnumerable<TRenderObject> RenderLabel(ILabel<TPoint, TSize, TRectangle, Path<TPoint, TRectangle>> label);
    }
}
