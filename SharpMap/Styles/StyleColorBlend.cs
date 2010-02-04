// Portions copyright 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Styles
{
    /// <summary>
    /// Defines arrays of colors and positions used for interpolating color blending 
    /// in a multicolor gradient.
    /// </summary>
    /// <seealso cref="GradientTheme2D"/>
    public class StyleColorBlend
    {
        private StyleColor[] _colors;
        private Single[] _positions;
        private Single _minimum = Single.NaN;
        private Single _maximum = Single.NaN;

        internal StyleColorBlend() { }

        /// <summary>
        /// Gets or sets an array of colors that represents the colors to use at 
        /// corresponding positions along a gradient.
        /// </summary>
        /// <value>An array of <see cref="StyleColor"/> structures that 
        /// represents the colors to use at corresponding positions along a gradient.
        /// </value>
        /// <remarks>
        /// This property is an array of <see cref="StyleColor"/> structures 
        /// that represents the colors to use at corresponding positions along a gradient. 
        /// Along with the Positions property, this property defines a multicolor gradient.
        /// </remarks>
        public StyleColor[] Colors
        {
            get { return _colors; }
            set { _colors = value; }
        }

        /// <summary>
        /// Gets or sets the positions along a gradient line.
        /// </summary>
        /// <value>An array of values that specify percentages of distance along the 
        /// gradient line.</value>
        /// <remarks>
        /// <para>The elements of this array specify percentages of distance along the 
        /// gradient line. For example, an element value of 0.2f specifies that this 
        /// point is 20 percent of the total distance from the starting point. 
        /// The elements in this array are represented by Single values between 0.0f and 1.0f, 
        /// and the first element of the array must be 0.0f and the last element must be 1.0f.</para>
        /// <pre>Along with the Colors property, this property defines a multicolor gradient.</pre>
        /// </remarks>
        public Single[] Positions
        {
            get { return _positions; }
            set
            {
                _positions = value;
                if (value == null)
                    _minimum = _maximum = float.NaN;
                else
                {
                    _minimum = value[0];
                    _maximum = value[value.GetUpperBound(0)];
                }

            }
        }

        /// <summary>
        /// Initializes a new instance of the ColorBlend class.
        /// </summary>
        /// <param name="colors">An array of Color structures that represents the 
        /// colors to use at corresponding positions along a gradient.</param>
        /// <param name="positions">An array of values that specify percentages of 
        /// distance along the gradient line.</param>
        public StyleColorBlend(StyleColor[] colors, Single[] positions)
        {
            _colors = colors;
            Positions = positions;
        }

        /// <summary>
        /// Gets the color from the scale at position 'pos'.
        /// </summary>
        /// <remarks>If the position is outside the scale [0..1] only the fractional part
        /// is used (in other words the scale restarts for each integer-part).</remarks>
        /// <param name="pos">Position on scale between 0.0f and 1.0f</param>
        /// <returns>Color on scale</returns>
        public StyleColor GetColor(Single pos)
        {
            if (_colors.Length != _positions.Length)
            {
                throw (new ArgumentException("Colors and Positions arrays must be of equal length"));
            }

            if (_colors.Length < 2)
            {
                throw (new ArgumentException("At least two colors must be defined in the ColorBlend"));
            }
            /*
            if (_positions[0] != 0f)
            {
                throw (new ArgumentException("First position value must be 0.0f"));
            }

            if (_positions[_positions.Length - 1] != 1f)
            {
                throw (new ArgumentException("Last position value must be 1.0f"));
            }

            if (pos > 1 || pos < 0)
            {
                pos -= (Single)Math.Floor(pos);
            }
            */
            Int32 i = 1;

            while (i < _positions.Length && _positions[i] < pos)
                i++;

            Single frac = (pos - _positions[i - 1]) / (_positions[i] - _positions[i - 1]);
            frac = Math.Max(0f, frac);
            frac = Math.Min(1f, frac);
            Int32 r = (Int32)Math.Round((_colors[i - 1].R * (1 - frac) + _colors[i].R * frac));
            Int32 g = (Int32)Math.Round((_colors[i - 1].G * (1 - frac) + _colors[i].G * frac));
            Int32 b = (Int32)Math.Round((_colors[i - 1].B * (1 - frac) + _colors[i].B * frac));
            Int32 a = (Int32)Math.Round((_colors[i - 1].A * (1 - frac) + _colors[i].A * frac));

            return StyleColor.FromBgra(b, g, r, a);
        }

        ///// <summary>
        ///// Converts the color blend to a gradient brush
        ///// </summary>
        ///// <param name="rectangle"></param>
        ///// <param name="angle"></param>
        ///// <returns></returns>
        //public LinearGradientBrush ToBrush(ViewRectangle rectangle, Single angle)
        //{
        //    LinearGradientBrush br = new LinearGradientBrush(rectangle, Color.Black, Color.Black, angle, true);
        //    System.Drawing.Drawing2D.ColorBlend cb = new System.Drawing.Drawing2D.ColorBlend();
        //    cb.Colors = _colors;
        //    cb.Positions = _positions;
        //    br.InterpolationColors = cb;
        //    return br;
        //}

        #region Predefined color scales

        /// <summary>
        /// Gets a linear gradient scale with seven colours making a rainbow from red to violet.
        /// </summary>
        /// <remarks>
        /// Colors span the following with an interval of 1/6:
        /// { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet }
        /// </remarks>
        public static StyleColorBlend Rainbow7
        {
            get
            {
                StyleColorBlend cb = new StyleColorBlend();
                cb._positions = new Single[7];
                for (Int32 i = 1; i < 7; i++)
                    cb.Positions[i] = i / 6f;
                cb.Colors = new StyleColor[] { StyleColor.Red, StyleColor.Orange, StyleColor.Yellow, StyleColor.Green, StyleColor.Blue, StyleColor.Indigo, StyleColor.Violet };
                return cb;
            }
        }

        /// <summary>
        /// Gets a linear gradient scale with five colours making a rainbow from red to blue.
        /// </summary>
        /// <remarks>
        /// Colors span the following with an interval of 0.25:
        /// { Color.Red, Color.Yellow, Color.Green, Color.Cyan, Color.Blue }
        /// </remarks>
        public static StyleColorBlend Rainbow5
        {
            get
            {
                return new StyleColorBlend(
                    new StyleColor[] { StyleColor.Red, StyleColor.Yellow, StyleColor.Green, StyleColor.Cyan, StyleColor.Blue },
                    new Single[] { 0f, 0.25f, 0.5f, 0.75f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from black to white
        /// </summary>
        public static StyleColorBlend BlackToWhite
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Black, StyleColor.White }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from white to black
        /// </summary>
        public static StyleColorBlend WhiteToBlack
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.White, StyleColor.Black }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from red to green
        /// </summary>
        public static StyleColorBlend RedToGreen
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Red, StyleColor.Green }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from green to red
        /// </summary>
        public static StyleColorBlend GreenToRed
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Green, StyleColor.Red }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from blue to green
        /// </summary>
        public static StyleColorBlend BlueToGreen
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Blue, StyleColor.Green }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from green to blue
        /// </summary>
        public static StyleColorBlend GreenToBlue
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Green, StyleColor.Blue }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from red to blue
        /// </summary>
        public static StyleColorBlend RedToBlue
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Red, StyleColor.Blue }, new Single[] { 0f, 1f });
            }
        }

        /// <summary>
        /// Gets a linear gradient scale from blue to red
        /// </summary>
        public static StyleColorBlend BlueToRed
        {
            get
            {
                return new StyleColorBlend(new StyleColor[] { StyleColor.Blue, StyleColor.Red }, new Single[] { 0f, 1f });
            }
        }

        #endregion

        #region Constructor helpers

        /// <summary>
        /// Creates a linear gradient scale from two colors
        /// </summary>
        /// <param name="fromColor"></param>
        /// <param name="toColor"></param>
        /// <returns></returns>
        public static StyleColorBlend TwoColors(StyleColor fromColor, StyleColor toColor)
        {
            return new StyleColorBlend(new StyleColor[] { fromColor, toColor }, new Single[] { 0f, 1f });
        }

        /// <summary>
        /// Creates a linear gradient scale from three colors
        /// </summary>
        public static StyleColorBlend ThreeColors(StyleColor fromColor, StyleColor middleColor, StyleColor toColor)
        {
            return new StyleColorBlend(new StyleColor[] { fromColor, middleColor, toColor },
                new Single[] { 0f, 0.5f, 1f });
        }

        #endregion
    }
}
