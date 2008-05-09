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
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// Holds Gdi objects together for later drawing on an image surface.
    /// </summary>
    public struct GdiRenderObject
    {
        private RenderState _state;

        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="path">The path to draw.</param>
        /// <param name="fill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        /// <param name="line">
        /// The pen used to draw a line when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightLine">
        /// The pen used to draw a line when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectLine">
        /// The pen used to draw a line when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        /// <param name="outline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        public GdiRenderObject(GraphicsPath path, Brush fill, Brush highlightFill, Brush selectFill, 
                               Pen line, Pen highlightLine, Pen selectLine,
                               Pen outline, Pen highlightOutline, Pen selectOutline)
        {
			_state = RenderState.Normal;
            GdiPath = path;
            Fill = fill;
            HighlightFill = highlightFill;
            SelectFill = selectFill;
            Line = line;
            HighlightLine = highlightLine;
            SelectLine = selectLine;
            Outline = outline;
            HighlightOutline = highlightOutline;
            SelectOutline = selectOutline;

            Image = null;
            Bounds = RectangleF.Empty;
            AffineTransform = null;
            ColorTransform = null;

            Text = null;
            Font = null;
        }

        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="image">The symbol to draw.</param>
        /// <param name="imageBounds">The location and size to draw the symbol.</param>
        /// <param name="transform">The affine transform applied to the symbol before drawing.</param>
        /// <param name="colorTransform">The color transform applied to the symbol before drawing.</param>
        public GdiRenderObject(Bitmap image, RectangleF imageBounds, GdiMatrix transform, GdiColorMatrix colorTransform)
        {
			_state = RenderState.Normal;
            Image = image;
            Bounds = imageBounds;
            AffineTransform = transform;
            ColorTransform = colorTransform;

            GdiPath = null;
            Fill = null;
            HighlightFill = null;
            SelectFill = null;
            Line = null;
            HighlightLine = null;
            SelectLine = null;
            Outline = null;
            HighlightOutline = null;
            SelectOutline = null;

            Text = null;
            Font = null;
        }

        /// <summary>
        /// Creates a new GdiRenderObject instance.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="font">The font to use to render the text.</param>
        /// <param name="bounds">The location and size to draw the text.</param>
        /// <param name="fill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectFill">
        /// The brush used to fill the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        /// <param name="outline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Normal"/>.
        /// </param>
        /// <param name="highlightOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Highlighted"/>.
        /// </param>
        /// <param name="selectOutline">
        /// The pen used to outline the path when the state is <see cref="RenderState.Selected"/>.
        /// </param>
        public GdiRenderObject(String text, Font font, RectangleF bounds, 
                               Brush fill, Brush highlightFill, Brush selectFill,
                               Pen outline, Pen highlightOutline, Pen selectOutline)
        {
			_state = RenderState.Normal;
            GdiPath = null;

            Image = null;
            Bounds = RectangleF.Empty;
            AffineTransform = null;
            ColorTransform = null;

            Text = text;
            Font = font;
            Bounds = bounds;
            Fill = fill;
            HighlightFill = highlightFill;
            SelectFill = selectFill;
            Outline = outline;
            HighlightOutline = highlightOutline;
            SelectOutline = selectOutline;
            Line = null;
            HighlightLine = null;
            SelectLine = null;
        }

        public readonly String Text;
        public readonly Font Font;

        /// <summary>
        /// The symbol to draw if render object is a symbol.
        /// </summary>
        public readonly Bitmap Image;

        /// <summary>
        /// The location and size of the symbol or text.
        /// </summary>
        public readonly RectangleF Bounds;

        /// <summary>
        /// The affine transform applied to the symbol before drawing.
        /// </summary>
        public readonly GdiMatrix AffineTransform;

        /// <summary>
        /// The color transform applied to the symbol before drawing.
        /// </summary>
        public readonly GdiColorMatrix ColorTransform;

        /// <summary>
        /// The path to draw if render object is a path.
        /// </summary>
        public readonly GraphicsPath GdiPath;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="RenderState.Normal"/>.
        /// </summary>
        public readonly Brush Fill;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="RenderState.Highlighted"/>.
        /// </summary>
        public readonly Brush HighlightFill;

        /// <summary>
        /// The brush used to fill the path when the state is 
        /// <see cref="RenderState.Selected"/>.
        /// </summary>
        public readonly Brush SelectFill;

        /// <summary>
        /// The pen used to draw a line when the state is 
        /// <see cref="RenderState.Normal"/>.
        /// </summary>
        public readonly Pen Line;

        /// <summary>
        /// The pen used to draw a line when the state is 
        /// <see cref="RenderState.Highlighted"/>.
        /// </summary>
        public readonly Pen HighlightLine;

        /// <summary>
        /// The pen used to draw a line when the state is 
        /// <see cref="RenderState.Selected"/>.
        /// </summary>
        public readonly Pen SelectLine;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="RenderState.Normal"/>.
        /// </summary>
        public readonly Pen Outline;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="RenderState.Highlighted"/>.
        /// </summary>
        public readonly Pen HighlightOutline;

        /// <summary>
        /// The pen used to outline the path when the state is 
        /// <see cref="RenderState.Selected"/>.
        /// </summary>
        public readonly Pen SelectOutline;

        /// <summary>
        /// Gets or sets the render object state of visual appearance.
        /// </summary>
		public RenderState State
		{
			get { return _state; }
			set { _state = value; }
		}
	}
}