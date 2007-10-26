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
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Encapsulates a label glyph on a map.
    /// </summary>
    /// <typeparam name="TPoint">Type of point.</typeparam>
    /// <typeparam name="TSize">Type of size.</typeparam>
    /// <typeparam name="TRectangle">Type of rectangle.</typeparam>
    /// <typeparam name="TPath">Type of graphics path.</typeparam>
    public interface ILabel<TPoint, TSize, TRectangle, TPath>
        where TPoint : IVectorD
        where TSize : IVectorD
		where TRectangle : IMatrixD, IEquatable<TRectangle>
		where TPath : Path<TPoint, TRectangle>
	{
        /// <summary>
        /// Gets or sets the label collision buffer.
        /// </summary>
        TSize CollisionBuffer { get; set; }

        /// <summary>
        /// Gets or sets a path along which label runs.
        /// </summary>
        TPath FlowPath { get; set; }

        /// <summary>
        /// Gets or sets the label font.
        /// </summary>
        StyleFont Font { get; set; }

        /// <summary>
        /// Gets or sets the label's world location.
		/// </summary>
        TPoint Location { get; set; }

        /// <summary>
        /// Gets or sets the label's offset within the <see cref="CollisionBuffer"/>.
        /// </summary>
        TPoint Offset { get; set; }

        /// <summary>
        /// Gets or sets a priority value used in label layout.
        /// </summary>
        Int32 Priority { get; set; }

        /// <summary>
        /// Gets or sets the label's rotation.
		/// </summary>
		Single Rotation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SharpMap.Styles.LabelStyle"/> of this label.
        /// </summary>
        LabelStyle Style { get; set; }

        /// <summary>
        /// Gets or sets the text of the label.
        /// </summary>
        String Text { get; set; }

        IMatrixD Transform { get; set; }
	}
}
