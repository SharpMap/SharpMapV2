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
    /// Enumerates the states of the GDI render object
    /// </summary>
    public enum GdiRenderObjectState : byte
    {
        Unknown = 0,
        Normal,
        Hover,
        Selected
    }

    /// <summary>
    /// Enumerates the type of GDI render object we have
    /// </summary>
    public enum GdiRenderObjectType : byte
    {
        Unknown = 0,
        Path,
        Symbol
    }

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
