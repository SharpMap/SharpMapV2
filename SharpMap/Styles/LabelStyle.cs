// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Styles
{
    /// <summary>
    /// Defines a style used for rendering labels.
    /// </summary>
    public class LabelStyle : Style
    {
        private HorizontalAlignment _horisontalAlignment;
        private VerticalAlignment _verticalAlignment;
        private StyleTextRenderingHint _textRenderingHint;
        private StyleRenderingMode _smoothingMode;
        private Size2D _collisionBuffer;
        private bool _collisionDetection;
        private Point2D _offset;
        private StylePen _halo;
        private StyleFont _font;
        private StyleBrush _foreground;
        private StyleBrush _background;

        /// <summary>
        /// Initializes a new <see cref="LabelStyle"/>.
        /// </summary>
        public LabelStyle()
            : this(new StyleFont(new StyleFontFamily("Arial"), new Size2D(12, 12), StyleFontStyle.Regular), new SolidStyleBrush(StyleColor.Black))
        {
        }

        public LabelStyle(StyleFont font, StyleBrush foreground)
            : this(font, foreground, new SolidStyleBrush(StyleColor.Transparent), 
                Point2D.Empty, Size2D.Empty, HorizontalAlignment.Left, VerticalAlignment.Middle)
        {
        }

        public LabelStyle(StyleFont font, StyleBrush foreground, StyleBrush background, Point2D offset, Size2D collisionBuffer, 
            HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            _font = font;
            _foreground = foreground;
            _background = background;
            _collisionBuffer = collisionBuffer;
            _collisionDetection = !_collisionBuffer.IsEmpty;
            _offset = offset;
            _horisontalAlignment = horizontalAlignment;
            _verticalAlignment = verticalAlignment;
        }

        /// <summary>
        /// Render whether smoothing (anti-aliasing) is applied to lines 
        /// and curves and the edges of filled areas.
        /// </summary>
        public StyleRenderingMode SmoothingMode
        {
            get { return _smoothingMode; }
            set { _smoothingMode = value; }
        }

        /// <summary>
        /// Specifies the quality of text rendering.
        /// </summary>
        public StyleTextRenderingHint TextRenderingHint
        {
            get { return _textRenderingHint; }
            set { _textRenderingHint = value; }
        }

        /// <summary>
        /// Font used to draw the label.
        /// </summary>
        public StyleFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Brush used to fill the font for the label.
        /// </summary>
        public StyleBrush Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }

        /// <summary>
        /// The background brush of the label. 
        /// </summary>
        /// <remarks>
        /// Set to transparent brush or null if background isn't needed.
        /// </remarks>
        public StyleBrush Background
        {
            get { return _background; }
            set { _background = value; }
        }

        /// <summary>
        /// Creates a halo around the text.
        /// </summary>
        public StylePen Halo
        {
            get { return _halo; }
            set { _halo = value; }
        }

        /// <summary>
        /// Specifies relative position of labels with respect to object's label
        /// <see cref="SharpMap.Rendering.ILabel{TPoint, TSize, TRectangle, TPath}.Location"/>.
        /// </summary>
        public Point2D Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Gets or sets whether collision detection is enabled for the labels.
        /// If set to true, label collision will be applied.
        /// </summary>
        public bool CollisionDetection
        {
            get { return _collisionDetection; }
            set { _collisionDetection = value; }
        }

        /// <summary>
        /// Distance around label where collision buffer is active.
        /// </summary>
        public Size2D CollisionBuffer
        {
            get { return _collisionBuffer; }
            set { _collisionBuffer = value; }
        }

        /// <summary>
        /// The horizontal alignment of the text in relation to the label 
        /// <see cref="SharpMap.Rendering.ILabel{TPoint, TSize, TRectangle, TPath}.Location"/>.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return _horisontalAlignment; }
            set { _horisontalAlignment = value; }
        }

        /// <summary>
        /// The vertical alignment of the text in relation to the label
        /// <see cref="SharpMap.Rendering.ILabel{TPoint, TSize, TRectangle, TPath}.Location"/>.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { _verticalAlignment = value; }
        }
    }
}