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
using System.IO;
using System.Text;

using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Interface to a graphical renderer.
    /// </summary>
    /// <typeparam name="TViewPoint">Type of point vector used by the graphical display coordinate system.</typeparam>
    /// <typeparam name="TViewSize">Type of size vector used by the graphical display coordinate system.</typeparam>
    /// <typeparam name="TViewRectangle">Type of rectangle matrix used by the graphical display coordinate system.</typeparam>
    /// <typeparam name="TRenderObject">Type of object used by the graphical display coordinate system to render spatial items.</typeparam>
    public interface IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> : IDisposable
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectangle : IViewMatrix
    {
        /// <summary>
        /// Gets or sets a <see cref="Style"/> used to render objects.
        /// </summary>
        IStyle Style { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Theme"/> used to render objects.
        /// </summary>
        ITheme Theme { get; set; }

        /// <summary>
        /// Gets or sets a matrix used to transform world coordinates to graphical display coordinates.
        /// </summary>
        IViewMatrix ViewTransform { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="StyleRenderingMode"/> value used to render objects.
        /// </summary>
        StyleRenderingMode StyleRenderingMode { get; set; }
    }
}
