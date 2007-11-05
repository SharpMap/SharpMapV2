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
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Styles
{
    /// <summary>
    /// A brush which fills with a linearly interpolated gradient between
    /// two or more colors.
    /// </summary>
    public class LinearGradientStyleBrush : StyleBrush
    {
        private StyleColorBlend _colorBlend;
        private StyleColor _startColor = StyleColor.Black;
        private StyleColor _endColor = StyleColor.White;
        private IMatrixD _transform;

        /// <summary>
        /// Gets or sets a color blend object used to balance 
        /// how the colors are interpolated across the gradient.
        /// </summary>
        public StyleColorBlend ColorBlend
        {
            get { return _colorBlend; }
            set { _colorBlend = value; }
        }

        /// <summary>
        /// Gets or sets the starting color of the gradient.
        /// </summary>
        public StyleColor StartColor
        {
            get { return _startColor; }
            set { _startColor = value; }
        }

        /// <summary>
        /// Gets or sets the ending color of the gradient.
        /// </summary>
        public StyleColor EndColor
        {
            get { return _endColor; }
            set { _endColor = value; }
        }

        /// <summary>
        /// Gets or sets the matrix used to transform the linear interpolation
        /// of the blend.
        /// </summary>
        public IMatrixD Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        /// <summary>
        /// Returns a string description of the <see cref="LinearGradientStyleBrush"/>.
        /// </summary>
        /// <returns>A string which describes the brush.</returns>
        public override string ToString()
        {
            return String.Format("LinearGradientStyleBrush - Base Color: {0}; Start: {1}; End: {2}; Blend: {3}", 
                Color, StartColor, EndColor, ColorBlend);
        }
    }
}