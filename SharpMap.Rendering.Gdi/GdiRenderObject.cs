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
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiColorMatrix = System.Drawing.Imaging.ColorMatrix;
using System.Text;

using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// Holds GDI objects together for later drawing on an image surface.
    /// </summary>
    public struct GdiRenderObject
    {
        public GdiRenderObject(GraphicsPath path, Brush fill, Brush highlightFill, Brush selectFill, Pen outline, Pen highlightOutline, Pen selectOutline)
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
        public readonly GdiRenderObjectType Type;
        public readonly Bitmap Symbol;
        public readonly Matrix AffineTransform;
        public readonly GdiColorMatrix ColorTransform;
        public readonly GraphicsPath GdiPath;
        public readonly Brush Fill;
        public readonly Brush HighlightFill;
        public readonly Brush SelectFill;
        public readonly Pen Outline;
        public readonly Pen HightlightOutline;
        public readonly Pen SelectOutline;

        public GdiRenderObjectState State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}
