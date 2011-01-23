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
using GdiPath = System.Drawing.Drawing2D.GraphicsPath;
namespace SharpMap.Rendering.Gdi
{
    /// <summary>
    /// Holds Gdi objects together for later drawing on an image surface.
    /// </summary>
    public abstract class GdiRenderObject
    {
        public readonly RenderState State;

        protected GdiRenderObject(RenderState state)
        {
            State = state;
        }

        public abstract void ToGraphicsDevice(Graphics g);

        protected void AdjustAndDrawImage(Graphics g, GdiMatrix affineTransform, RectangleF location, GdiColorMatrix colorMatrix, Bitmap raster)
        {
            GdiMatrix save = null;
            if (!affineTransform.IsIdentity)
            {
                save = g.Transform.Clone();
                GdiMatrix transform = g.Transform;
                RectangleF rectLocation = location;
                Single offsetX = rectLocation.Left + rectLocation.Width / 2;
                Single offsetY = rectLocation.Top - rectLocation.Width / 2;
                transform.Translate(-offsetX, -offsetY);
                transform.Multiply(affineTransform, MatrixOrder.Append);
                transform.Translate(offsetX, offsetY, MatrixOrder.Append);
                g.Transform = transform;
            }

            //ImageAttributes ia = new ImageAttributes();
            //    ia.SetColorMatrix(colorMatrix);
            //
            //GraphicsUnit graphicsUnit = GraphicsUnit.Pixel;
            g.DrawImage(raster, location);

            if (save != null) g.Transform = save;
        }
        
    }

    public class GdiVectorRenderObject : GdiRenderObject
    {
        public GdiVectorRenderObject(RenderState renderState, GraphicsPath path, Pen line, Pen outline, Brush fill)
            :base(renderState)
        {
            Path = path;
            Line = line;
            Outline = outline;
            Fill = fill;
        }

        public readonly GraphicsPath Path;
        public readonly Pen Line;
        public readonly Pen Outline;
        public readonly Brush Fill;

        public override void ToGraphicsDevice(Graphics g)
        {
            if (Fill != null)
            {
                g.FillPath(Fill, Path);
                if (Outline != null)
                    g.DrawPath(Outline, Path);
                return;
            }

            if (Outline != null)
                g.DrawPath(Outline, Path);
            g.DrawPath(Line, Path);
        }
    }

    public class GdiPointRenderObject : GdiRenderObject
    {
        public GdiPointRenderObject(RenderState renderState, RectangleF bounds, GdiMatrix transform, GdiColorMatrix colorMatrix, Bitmap symbol)
            : base(renderState)
        {
            Bounds = bounds;
            Transform = transform;
            ColorMatrix = colorMatrix;
            Symbol = symbol;
        }

        //public readonly Point Point;
        public readonly RectangleF Bounds;
        public readonly GdiMatrix Transform;
        public readonly GdiColorMatrix ColorMatrix;
        public readonly Bitmap Symbol;

        public override void ToGraphicsDevice(Graphics g)
        {
            AdjustAndDrawImage(g, Transform, Bounds, ColorMatrix, Symbol);
        }
    }

    public class GdiRasterRenderObject : GdiRenderObject
    {
        public GdiRasterRenderObject(RenderState renderState, Bitmap raster, Rectangle bounds, GdiMatrix transform, GdiColorMatrix colorMatrix)
            : base(renderState)
        {
            Raster = raster;
            Bounds = bounds;
            Transform = transform;
            ColorMatrix = colorMatrix;
        }

        public readonly Bitmap Raster;
        public readonly GdiMatrix Transform;
        public readonly Rectangle Bounds;
        public readonly GdiColorMatrix ColorMatrix;

        public override void ToGraphicsDevice(Graphics g)
        {
            AdjustAndDrawImage(g, Transform, Bounds, ColorMatrix, Raster);
        }
    }

    public abstract class GdiTextRenderObject<T> : GdiRenderObject
    {
        protected GdiTextRenderObject(RenderState renderState, string text, Font font, Brush fontBrush, T location)
            : base(renderState)
        {
            Font = font;
            Text = text;
            Location = location;
        }

        public readonly String Text;
        public readonly Font Font;
        public readonly Brush FontBrush;
        public readonly T Location;

        //public abstract void ToGraphicsDevice(Graphics g);

    }

    public class GdiTextRenderObjectRect : GdiTextRenderObject<RectangleF>
    {
        internal GdiTextRenderObjectRect(RenderState renderState, string text, Font font, Brush fontBrush, RectangleF location) 
            : base(renderState, text, font, fontBrush, location)
        {
        }

        /// <summary>
        /// Without this change, labels render upside down and don't all scale readably.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object to use for GDI+ operations</param>
        /// <returns></returns>
        private RectangleF AdjustForLabel(Graphics g)
        {
            // this transform goes from the underlying coordinates to 
            // screen coordinates, but for some reason renders text upside down
            // we cannot just scale by 1, -1 because offsets are affected also
            GdiMatrix m = g.Transform;
            // used to scale text size for the current zoom level
            float scale = Math.Abs(m.Elements[0]);

            // get the bounds of the label in the underlying coordinate space
            Point ll = new Point((Int32)Location.X, (Int32)Location.Y);
            Point ur = new Point((Int32)(Location.X + Location.Width),
                                 (Int32)(Location.Y + Location.Height));

            Point[] transformedPoints1 =
                {
                    new Point((Int32) Location.X, (Int32) Location.Y),
                    new Point((Int32) (Location.X + Location.Width),
                              (Int32) (Location.Y + Location.Height))
                };

            // get the label bounds transformed into screen coordinates
            // note that if we just render this as-is the label is upside down
            m.TransformPoints(transformedPoints1);

            // for labels, we're going to use an identity matrix and screen coordinates
            GdiMatrix newM = new GdiMatrix();
            g.Transform = newM;

            Int32 pixelWidth = ur.X - ll.X;
            Int32 pixelHeight = ur.Y - ll.Y;

            // if we're scaling text, then x,y position will get multiplied by our 
            // scale, so adjust for it here so that we can use actual pixel x,y
            // Also center our label on the coordinate instead of putting the label origin on the coordinate
            RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale,
                                                  (transformedPoints1[0].Y / scale) - pixelHeight,
                                                  pixelWidth,
                                                  pixelHeight);
            //RectangleF newBounds = new RectangleF(transformedPoints1[0].X / scale - (pixelWidth / 2), transformedPoints1[0].Y / scale - (pixelHeight / 2), pixelWidth, pixelHeight);

            return newBounds;
        }

        public override void ToGraphicsDevice(Graphics g)
        {
            RectangleF newLocation = AdjustForLabel(g);
            g.DrawString(Text, Font, FontBrush, newLocation);
        }
    }

    /*        
    public class GdiRenderObject
    {
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
     */
}