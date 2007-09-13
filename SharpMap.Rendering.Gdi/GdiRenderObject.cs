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

using System.Drawing;
using System.Drawing.Drawing2D;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// Holds Gdi objects together for later drawing on an image surface.
    /// </summary>
    public struct GdiRenderObject
    {
        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="path">A GraphicsPath to draw.</param>
        /// <param name="fill">
        /// The brush used to fill the path when the state is <see cref="GdiRenderObjectState.Normal"/>.
        /// </param>
        /// <param name="highlightFill">
        /// The brush used to fill the path when the state is <see cref="GdiRenderObjectState.Hover"/>.
        /// </param>
        /// <param name="selectFill">
        /// The brush used to fill the path when the state is <see cref="GdiRenderObjectState.Selected"/>.
        /// </param>
        /// <param name="outline">
        /// The pen used to outline the path when the state is <see cref="GdiRenderObjectState.Normal"/>.
        /// </param>
        /// <param name="highlightOutline">
        /// The pen used to outline the path when the state is <see cref="GdiRenderObjectState.Hover"/>.
        /// </param>
        /// <param name="selectOutline">
        /// The pen used to outline the path when the state is <see cref="GdiRenderObjectState.Selected"/>.
        /// </param>
        public GdiRenderObject(GraphicsPath path, Brush fill, Brush highlightFill, Brush selectFill, Pen outline,
                               Pen highlightOutline, Pen selectOutline)
        {
            Type = GdiRenderObjectType.Path;
            _state = GdiRenderObjectState.Normal;
            GdiPath = path;
            Fill = fill;
            HighlightFill = highlightFill;
            SelectFill = selectFill;
            Outline = outline;
            HightlightOutline = highlightOutline;
            SelectOutline = selectOutline;

            Symbol = null;
            AffineTransform = null;
            ColorTransform = null;
        }

        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="symbol">The symbol to draw.</param>
        /// <param name="transform">The affine transform applied to the symbol before drawing.</param>
        /// <param name="colorTransform">The color transform applied to the symbol before drawing.</param>
        public GdiRenderObject(Bitmap symbol, Matrix transform, GdiColorMatrix colorTransform)
        {
            Type = GdiRenderObjectType.Symbol;
            _state = GdiRenderObjectState.Normal;
            Symbol = symbol;
            AffineTransform = transform;
            ColorTransform = colorTransform;

            GdiPath = null;
            Fill = null;
            HighlightFill = null;
            SelectFill = null;
            Outline = null;
            HightlightOutline = null;
            SelectOutline = null;
        }

        private GdiRenderObjectState _state;

        /// <summary>
        /// The type of the render object: either a path or a symbol.
        /// </summary>
        public readonly GdiRenderObjectType Type;

        /// <summary>
        /// The symbol to draw when <see cref="Type"/> is <see cref="GdiRenderObjectType.Symbol"/>
        /// </summary>
        public readonly Bitmap Symbol;

        /// <summary>
        /// The affine transform applied to the symbol before drawing.
        /// </summary>
        public readonly Matrix AffineTransform;

        /// <summary>
        /// The color transform applied to the symbol before drawing.
        /// </summary>
        public readonly GdiColorMatrix ColorTransform;

        /// <summary>
        /// The path to draw when <see cref="Type"/> is <see cref="GdiRenderObjectType.Path"/>
        /// </summary>
        public readonly GraphicsPath GdiPath;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="GdiRenderObjectState.Normal"/>.
        /// </summary>
        public readonly Brush Fill;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="GdiRenderObjectState.Hover"/>.
        /// </summary>
        public readonly Brush HighlightFill;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="GdiRenderObjectState.Selected"/>.
        /// </summary>
        public readonly Brush SelectFill;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="GdiRenderObjectState.Normal"/>.
        /// </summary>
        public readonly Pen Outline;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="GdiRenderObjectState.Hover"/>.
        /// </summary>
        public readonly Pen HightlightOutline;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="GdiRenderObjectState.Selected"/>.
        /// </summary>
        public readonly Pen SelectOutline;

        /// <summary>
        /// Gets or sets the render object state of visual appearance.
        /// </summary>
        public GdiRenderObjectState State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}