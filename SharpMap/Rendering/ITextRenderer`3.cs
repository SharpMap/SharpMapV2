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
using SharpMap.Styles;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Rendering
{
    public interface ITextRenderer<TPoint, TSize, TRectangle> : IRenderer
        where TPoint : IVectorD
        where TSize : IVectorD
        where TRectangle : IMatrixD, IEquatable<TRectangle>
    {
        /// <summary>
        /// Gets or sets a <see cref="StyleTextRenderingHint"/> to control how rendered text appears.
        /// </summary>
        StyleTextRenderingHint TextRenderingHint { get; set; }

        /// <summary>
        /// Measures the <typeparamref name="TSize"/> of a String in the given <paramref name="font"/>.
        /// </summary>
        /// <param name="text">The String to measure.</param>
        /// <param name="font">The font to use to draw the String.</param>
        /// <returns>A measurement of the String.</returns>
        TSize MeasureString(String text, StyleFont font);

        /// <summary>
        /// Renders a text String.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to use to draw the text.</param>
        /// <param name="location">The location in view coordinates to render the text at.</param>
        /// <param name="fontBrush">The brush to use to paint the text.</param>
        /// <returns>
        /// A set of object instances representing the rendered text.
        /// </returns>
        IEnumerable RenderText(String text, StyleFont font, TPoint location, StyleBrush fontBrush);

        /// <summary>
        /// Renders a text String.
        /// </summary>
        /// <param name="text">The text to render.</param>
        /// <param name="font">The font to use to draw the text.</param>
        /// <param name="layoutRectangle">
        /// The region in view coordinates to draw the text in.
        /// </param>
        /// <param name="flowPath">
        /// A path which the text is rendered to flow on.
        /// </param>
        /// <param name="fontBrush">
        /// The brush to use to paint the text.
        /// </param>
        /// <param name="transform">
        /// A transform to modify the location, rotation or skew of the text.
        /// </param>
        /// <returns>
        /// A set of object instances representing the rendered text.
        /// </returns>
        IEnumerable RenderText(String text, StyleFont font, TRectangle layoutRectangle,
                                              Path<TPoint, TRectangle> flowPath, StyleBrush fontBrush, 
                                              IMatrixD transform);
    }
}