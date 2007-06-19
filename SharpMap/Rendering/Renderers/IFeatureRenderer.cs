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

using SharpMap.Data;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer of feature data.
    /// </summary>
    /// <typeparam name="TViewPoint">Type of point vector used by the graphical display coordinate system.</typeparam>
    /// <typeparam name="TViewSize">Type of size vector used by the graphical display coordinate system.</typeparam>
    /// <typeparam name="TViewRectangle">Type of rectangle matrix used by the graphical display coordinate system.</typeparam>
    /// <typeparam name="TRenderObject">Type of object used by the graphical display coordinate system to render spatial items.</typeparam>
    public interface IFeatureRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> : IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject>
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectangle : IViewMatrix
	{
		/// <summary>
		/// Renders the attributes and/or spatial data in the <paramref name="feature"/>.
		/// </summary>
		/// <param name="feature">A <see cref="FeatureDataRow"/> instance with spatial data.</param>
		/// <returns>An enumeration of <typeparamref name="TRenderObject"/> instances used to draw the spatial data.</returns>
		IEnumerable<TRenderObject> RenderFeature(FeatureDataRow feature, IStyle style);

        /// <summary>
        /// Renders the attributes and/or spatial data in the <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">A <see cref="FeatureDataRow"/> instance with spatial data.</param>
		/// <param name="style">Style used to render the feature, overriding theme. 
		/// Use null if no style is desired or to use <see cref="IFeatureRenderer.Theme"/>.</param>
        /// <returns>An enumeration of <typeparamref name="TRenderObject"/> instances used to draw the spatial data.</returns>
        IEnumerable<TRenderObject> RenderFeature(FeatureDataRow feature, IStyle style);

		/// <summary>
		/// Gets the theme by which to compute styles to apply to rendered features.
		/// </summary>
        ITheme Theme { get; }
    }
}
