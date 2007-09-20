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
        /// Gets or sets a <see cref="StyleTextRenderingHint"/> to control how rendered text appears.
        /// </summary>
        StyleTextRenderingHint TextRenderingHint { get; set; }

        /// <summary>
        /// Measures the <typeparamref name="TSize"/> of a string in the given <paramref name="font"/>.
        /// </summary>
        /// <param name="text">The string to measure.</param>
        /// <param name="font">The font to use to draw the string.</param>
        /// <returns>A measurement of the string.</returns>
        TSize MeasureString(string text, StyleFont font);

        /// <summary>
        /// Renders a label.
        /// </summary>
        /// <param name="label">Label to render.</param>
        /// <returns>A <typeparamref name="TRenderObject"/> used to draw the label.</returns>
        TRenderObject RenderLabel(ILabel<TPoint, TRectangle, GraphicsPath<TPoint, TRectangle>> label);

        /// <summary>
        /// Renders a label.
        /// </summary>
        /// <param name="text">The label text.</param>
        /// <param name="location">The location in graphical display coordinates to draw the label at.</param>
        /// <param name="font">The font to use to draw the label.</param>
        /// <param name="foreColor">The color to use to draw the label.</param>
        /// <returns>A <typeparamref name="TRenderObject"/> used to draw the label.</returns>
        TRenderObject RenderLabel(string text, TPoint location, StyleFont font, StyleColor foreColor);

        /// <summary>
        /// Renders a label.
        /// </summary>
        /// <param name="text">The label text.</param>
        /// <param name="location">
        /// The location in graphical display coordinates to draw the label at.
        /// </param>
        /// <param name="offset">
        /// An offset to apply to the <paramref name="location"/> in graphical display coordinates.
        /// </param>
        /// <param name="font">The font to use to draw the label.</param>
        /// <param name="foreColor">The color to use to draw the label.</param>
        /// <param name="backColor">The color to use behind the label.</param>
        /// <param name="halo">
        /// A <see cref="StylePen"/> instance to draw an outline around the label with.
        /// </param>
        /// <param name="rotation">An amount to rotate the label by.</param>
        /// <returns>A <typeparamref name="TRenderObject"/> used to draw the label.</returns>
        TRenderObject RenderLabel(string text, TPoint location, TPoint offset, StyleFont font, 
            StyleColor foreColor, StyleBrush backColor, StylePen halo, float rotation);
    }
}
