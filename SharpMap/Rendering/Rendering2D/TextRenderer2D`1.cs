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
using System.Collections.Generic;
using NPack;
using NPack.Interfaces;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rendering2D
{
    public abstract class TextRenderer2D<TRenderObject> : Renderer2D, ITextRenderer2D<TRenderObject>
    {
        private StyleTextRenderingHint _textRenderingHint;

        protected TextRenderer2D() : this(StyleTextRenderingHint.SystemDefault) { }

        protected TextRenderer2D(StyleTextRenderingHint textRenderingHint)
        {
            _textRenderingHint = textRenderingHint;
        }
        #region ITextRenderer2D<TRenderObject> Members

        public abstract IEnumerable<TRenderObject> RenderText(String text, StyleFont font, Rectangle2D layoutRectangle,
                                                              Path2D flowPath, StyleBrush fontBrush, Matrix2D transform);

        #endregion

        #region ITextRenderer<Point2D,Size2D,Rectangle2D,TRenderObject> Members

        public StyleTextRenderingHint TextRenderingHint
        {
            get { return _textRenderingHint; }
            set { _textRenderingHint = value; }
        }

        public abstract Size2D MeasureString(String text, StyleFont font);

        public IEnumerable<TRenderObject> RenderText(String text, StyleFont font, Point2D location, StyleBrush fontBrush)
        {
            Rectangle2D layoutRectangle = new Rectangle2D(location, MeasureString(text, font));
            return RenderText(text, font, layoutRectangle, null, fontBrush, null);
        }

        #endregion

        #region ITextRenderer<Point2D,Size2D,Rectangle2D> Explicit Members

        IEnumerable ITextRenderer<Point2D, Size2D, Rectangle2D>.RenderText(
            String text, StyleFont font, Rectangle2D layoutRectangle, Path<Point2D, Rectangle2D> flowPath, 
            StyleBrush fontBrush, IMatrix<DoubleComponent> transform)
        {
            if (!(flowPath is Path2D))
            {
                throw new ArgumentException("Parameter must be a Path2D instance.", "flowPath");
            }

            if (!(transform is Matrix2D))
            {
                throw new ArgumentException("Parameter must be a Matrix2D instance.", "transform");
            }

            return RenderText(text, font, layoutRectangle, flowPath as Path2D, fontBrush, transform as Matrix2D);
        }

        IEnumerable ITextRenderer<Point2D, Size2D, Rectangle2D>.RenderText(String text, StyleFont font, Point2D location, StyleBrush fontBrush)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}