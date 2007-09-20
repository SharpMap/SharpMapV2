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
        private HorizontalAlignmentEnum _horisontalAlignment;
        private VerticalAlignmentEnum _verticalAlignment;
        private StyleTextRenderingHint _textRenderingHint;
        private StyleRenderingMode _smoothingMode;
        private Size2D _collisionBuffer;
        private bool _collisionDetection;
        private Point2D _offset;
        private StylePen _halo;
        private StyleFont _font;
        private StyleColor _foreColor;
        private StyleBrush _backColor;

        /// <summary>
        /// Initializes a new LabelStyle
        /// </summary>
        public LabelStyle()
        {
            _font = new StyleFont("Times New Roman", new Size2D(12, 12));
            _offset = new Point2D(0, 0);
            _collisionDetection = false;
            _collisionBuffer = new Size2D(0, 0);
            _foreColor = StyleColor.Black;
            _horisontalAlignment = HorizontalAlignmentEnum.Center;
            _verticalAlignment = VerticalAlignmentEnum.Middle;
        }

        /// <summary>
        /// Render whether smoothing (antialiasing) is applied to lines 
        /// and curves and the edges of filled areas
        /// </summary>
        public StyleRenderingMode SmoothingMode
        {
            get { return _smoothingMode; }
            set { _smoothingMode = value; }
        }

        /// <summary>
        /// Specifies the quality of text rendering
        /// </summary>
        public StyleTextRenderingHint TextRenderingHint
        {
            get { return _textRenderingHint; }
            set { _textRenderingHint = value; }
        }

        /// <summary>
        /// Label Font
        /// </summary>
        public StyleFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        /// <summary>
        /// Font color
        /// </summary>
        public StyleColor ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }

        /// <summary>
        /// The background color of the label. Set to transparent brush or null if background isn't needed
        /// </summary>
        public StyleBrush BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        /// <summary>
        /// Creates a halo around the text
        /// </summary>
        public StylePen Halo
        {
            get { return _halo; }
            set { _halo = value; }
        }

        /// <summary>
        /// Specifies relative position of labels with respect to objects label point
        /// </summary>
        public Point2D Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        /// <summary>
        /// Gets or sets whether Collision Detection is enabled for the labels.
        /// If set to true, label collision will be tested.
        /// </summary>
        public bool CollisionDetection
        {
            get { return _collisionDetection; }
            set { _collisionDetection = value; }
        }

        /// <summary>
        /// Distance around label where collision buffer is active
        /// </summary>
        public Size2D CollisionBuffer
        {
            get { return _collisionBuffer; }
            set { _collisionBuffer = value; }
        }

        /// <summary>
        /// The horisontal alignment of the text in relation to the labelpoint
        /// </summary>
        public HorizontalAlignmentEnum HorizontalAlignment
        {
            get { return _horisontalAlignment; }
            set { _horisontalAlignment = value; }
        }

        /// <summary>
        /// The horisontal alignment of the text in relation to the labelpoint
        /// </summary>
        public VerticalAlignmentEnum VerticalAlignment
        {
            get { return _verticalAlignment; }
            set { _verticalAlignment = value; }
        }

        /// <summary>
        /// Label text alignment
        /// </summary>
        public enum HorizontalAlignmentEnum : short
        {
            /// <summary>
            /// Left oriented
            /// </summary>
            Left = 0,
            /// <summary>
            /// Right oriented
            /// </summary>
            Right = 2,
            /// <summary>
            /// Centered
            /// </summary>
            Center = 1
        }

        /// <summary>
        /// Label text alignment
        /// </summary>
        public enum VerticalAlignmentEnum : short
        {
            /// <summary>
            /// Left oriented
            /// </summary>
            Bottom = 0,
            /// <summary>
            /// Right oriented
            /// </summary>
            Top = 2,
            /// <summary>
            /// Centered
            /// </summary>
            Middle = 1
        }
    }
}