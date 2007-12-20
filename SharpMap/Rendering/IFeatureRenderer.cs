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

using System.Collections;
using SharpMap.Data;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.Layers;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer of feature data.
    /// </summary>
    public interface IFeatureRenderer : IRenderer
    {
        /// <summary>
        /// Gets or sets the default style if no style or theme information is provided.
        /// </summary>
        IStyle DefaultStyle { get; set; }

        /// <summary>
        /// Renders the attributes and/or spatial data in the <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">
        /// A <see cref="IFeatureDataRecord"/> instance with spatial data.
        /// </param>
        /// <returns>
        /// An enumeration of rendered objects used to draw the spatial data.
        /// </returns>
        IEnumerable RenderFeature(IFeatureDataRecord feature);

        /// <summary>
        /// Renders the attributes and/or spatial data in the <paramref name="feature"/>.
        /// </summary>
        /// <param name="feature">
        /// A <see cref="IFeatureDataRecord"/> instance with spatial data.
        /// </param>
        /// <param name="style">
        /// Style used to render the feature, overriding theme. 
        /// Use null if no style is desired or to use <see cref="Theme"/>.
        /// </param>
        /// <returns>
        /// An enumeration of rendered objects used to draw the spatial data.
        /// </returns>
        IEnumerable RenderFeature(IFeatureDataRecord feature, IStyle style, RenderState renderState, ILayer layer);

		void CleanUp();

        /// <summary>
        /// Gets or sets the theme by which to compute styles 
        /// to apply to rendered features.
        /// </summary>
        ITheme Theme { get; set; }
    }
}
