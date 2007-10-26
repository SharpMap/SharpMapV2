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
using System.Text;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Styles
{
    /// <summary>
    /// Represents a style for drawing lines.
    /// </summary>
    [Serializable]
    public class StylePen
    {
        #region Fields
        // This default (10.0f) is the same in both GDI+ and WPF 
        // for the MiterLimit value on the respective Pen objects.
        private Single _miterLimit = 10.0f;
        private StyleBrush _backgroundBrush;
        private Single _dashOffset;
        private LineDashCap _dashCap;
        private LineDashStyle _dashStyle;
        private Single[] _dashPattern;
        private StyleBrush[] _dashBrushes;
        private StyleLineCap _startCap;
        private StyleLineCap _endCap;
        private StyleLineJoin _lineJoin;
        private Matrix2D _transform = new Matrix2D();
        private Double _width;
        private Single[] _compoundArray;
        private StylePenAlignment _alignment;
        private Int32? _hashCode = null;
        #endregion

        #region Object Constructors
        /// <summary>
        /// Creates a new <see cref="StylePen"/> with the given solid
        /// <paramref name="color"/> and <paramref name="width"/>.
        /// </summary>
        /// <param name="color">Color of the pen.</param>
        /// <param name="width">Width of the pen.</param>
        public StylePen(StyleColor color, Double width)
            : this(new SolidStyleBrush(color), width) { }

        /// <summary>
        /// Creates a new pen with the given <see cref="StyleBrush"/>
        /// and <paramref name="width"/>.
        /// </summary>
        /// <param name="backgroundBrush">
        /// The StyleBrush which describes the color of the line.
        /// </param>
        /// <param name="width">The width of the line.</param>
        public StylePen(StyleBrush backgroundBrush, Double width)
        {
            _backgroundBrush = backgroundBrush;
            _width = width;
        }
        #endregion

        #region ToString
        public override String ToString()
        {
            return String.Format(
                "[StylePen] Width: {0}; Alignment: {2}; CompoundArray: {3}; MiterLimit: {4}; DashOffset: {5}; DashPattern: {6}; DashBrushes: {7}; DashStyle: {8}; StartCap: {9}; EndCap: {10}; DashCap: {11}; LineJoin: {12}; Transform: {13}; Background: {1};",
                Width, BackgroundBrush.ToString(), Alignment, printFloatArray(CompoundArray), MiterLimit, DashOffset, DashPattern, printBrushes(DashBrushes), DashStyle, StartCap, EndCap, DashCap, LineJoin, Transform);
        }
        #endregion

        #region GetHashCode
        public override Int32 GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                _hashCode = computeHashCode();
            }

            return (Int32)_hashCode;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the alignment of the pen.
        /// </summary>
        public StylePenAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                _alignment = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets an array of widths used to create a compound line.
        /// </summary>
        public Single[] CompoundArray
        {
            get { return _compoundArray; }
            set
            {
                _compoundArray = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets a value which limits the length of a miter at a line joint.
        /// </summary>
        /// <remarks>
        /// If the value is set to less than 1.0f, the value is clamped to 1.0f.
        /// </remarks>
        public Single MiterLimit
        {
            get { return _miterLimit; }
            set
            {
                if (value < 1.0f)
                {
                    value = 1.0f;
                }

                _miterLimit = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets a brush used to paint the line.
        /// </summary>
        public StyleBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set
            {
                _backgroundBrush = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets the offset of the start of the dash pattern.
        /// </summary>
        public Single DashOffset
        {
            get { return _dashOffset; }
            set
            {
                _dashOffset = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets an array of values used as widths in a dash pattern.
        /// </summary>
        public Single[] DashPattern
        {
            get { return _dashPattern; }
            set
            {
                _dashPattern = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets an array of brushes used to draw the dashes in a pen.
        /// </summary>
        public StyleBrush[] DashBrushes
        {
            get { return _dashBrushes; }
            set
            {
                _dashBrushes = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets a value used to compute the dash pattern.
        /// </summary>
        public LineDashStyle DashStyle
        {
            get { return _dashStyle; }
            set
            {
                _dashStyle = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of line terminator at the beginning
        /// of a line.
        /// </summary>
        public StyleLineCap StartCap
        {
            get { return _startCap; }
            set
            {
                _startCap = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of line terminator at the end
        /// of a line.
        /// </summary>
        public StyleLineCap EndCap
        {
            get { return _endCap; }
            set
            {
                _endCap = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of ending present at the start and end
        /// of a dash in the line.
        /// </summary>
        public LineDashCap DashCap
        {
            get { return _dashCap; }
            set
            {
                _dashCap = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets the type of joint drawn where a line contains a join.
        /// </summary>
        public StyleLineJoin LineJoin
        {
            get { return _lineJoin; }
            set
            {
                _lineJoin = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets a matrix transformation for drawing with the pen.
        /// </summary>
        /// <remarks>
        /// If set to null, a <see cref="Matrix2D.Identity"/> matrix will be used instead.
        /// </remarks>
        public Matrix2D Transform
        {
            get { return _transform; }
            set
            {
                if (value == null)
                {
                    value = new Matrix2D();
                }

                _transform = value;
                _hashCode = null;
            }
        }

        /// <summary>
        /// Gets or sets the width of the line drawn by this pen.
        /// </summary>
        public Double Width
        {
            get { return _width; }
            set
            {
                _width = value;
                _hashCode = null;
            }
        }
        #endregion

        #region Private helper methods

        private Int32 computeHashCode()
        {
            return Alignment.GetHashCode() ^
                   getSingleArrayHashCode(CompoundArray) ^
                   MiterLimit.GetHashCode() ^
                   BackgroundBrush.GetHashCode() ^
                   DashOffset.GetHashCode() ^
                   getSingleArrayHashCode(DashPattern) ^
                   getStyleBrushesArrayHashCode(DashBrushes) ^
                   DashStyle.GetHashCode() ^
                   StartCap.GetHashCode() ^
                   EndCap.GetHashCode() ^
                   DashCap.GetHashCode() ^
                   LineJoin.GetHashCode() ^
                   Transform.GetHashCode() ^
                   Width.GetHashCode() ^
                   -18133844;
        }

        private static String printBrushes(StyleBrush[] brushes)
        {
            if (brushes == null || brushes.Length == 0)
                return String.Empty;

            StringBuilder buffer = new StringBuilder();

            foreach (StyleBrush brush in brushes)
            {
                buffer.Append(brush.ToString());
                buffer.Append(", ");
            }

            buffer.Length -= 2;
            return buffer.ToString();
        }

        private static String printFloatArray(Single[] values)
        {
            if (values == null || values.Length == 0)
                return String.Empty;

            StringBuilder buffer = new StringBuilder();

            foreach (Single value in values)
            {
                buffer.AppendFormat("{0:N3}", value);
                buffer.Append(", ");
            }

            buffer.Length -= 2;
            return buffer.ToString();
        }

        private static Int32 getStyleBrushesArrayHashCode(StyleBrush[] brushes)
        {
            Int32 hashCode = -1720831040;

            if (brushes != null)
            {
                foreach (StyleBrush brush in brushes)
                {
                    hashCode ^= brush.GetHashCode();
                }
            }

            return hashCode;
        }

        private static Int32 getSingleArrayHashCode(Single[] values)
        {
            Int32 hashCode = 737226580;

            if (values != null)
            {
                foreach (Single value in values)
                {
                    hashCode ^= value.GetHashCode();
                }
            }

            return hashCode;
        }
        #endregion
    }
}
