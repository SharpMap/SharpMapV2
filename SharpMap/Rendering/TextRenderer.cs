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
using GeoAPI.Coordinates;
using NPack;
using NPack.Interfaces;
using SharpMap.Symbology;

namespace SharpMap.Rendering
{
    public abstract class TextRenderer<TCoordinate> : Renderer, ITextRenderer<TCoordinate>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        private TextRenderingHint _textRenderingHint;

        protected TextRenderer() : this(TextRenderingHint.SystemDefault) { }

        protected TextRenderer(TextRenderingHint textRenderingHint)
        {
            _textRenderingHint = textRenderingHint;
        }

        #region ITextRenderer<Point2D,Size2D,Rectangle2D,TRenderObject> Members

        public TextRenderingHint TextRenderingHint
        {
            get { return _textRenderingHint; }
            set { _textRenderingHint = value; }
        }

        public abstract Size<TCoordinate> MeasureString(String text, StyleFont font);
        public abstract void RenderText(IScene scene, string text, StyleFont font, TCoordinate location, Fill fill);
        public abstract void RenderText(IScene scene, string text, StyleFont font, Rectangle<TCoordinate> layoutRectangle, Path<TCoordinate> flowPath, Fill fill, IMatrix<DoubleComponent> transform);

        public void RenderText(IScene scene, String text, StyleFont font, TCoordinate location, IBrush fontBrush)
        {
            Rectangle<TCoordinate> layoutRectangle = new Rectangle<TCoordinate>(location, MeasureString(text, font));
            RenderText(scene, text, font, layoutRectangle, null, fontBrush, null);
        }

        #endregion
    }
}