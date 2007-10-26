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

// Initially copied from Paint.Net under the following MIT license:
//
// Paint.NET
// Copyright (C) dotPDN LLC, Rick Brewster, Chris Crosetto, Tom Jackson, Michael Kelsey, Brandon Ortiz, 
// Craig Taylor, Chris Trevino, and Luke Walker.
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.
// MIT License: http://www.opensource.org/licenses/mit-license.php
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial 
// portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
// LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SharpMap.Styles
{
    /// <summary>
    /// Represents a 32-bit color in BGRA (blue, green, red, alpha) format.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
    public struct StyleColor : IEquatable<StyleColor>
    {
        #region Private fields

        [FieldOffset(0)]
        private byte _b;
        [FieldOffset(1)]
        private byte _g;
        [FieldOffset(2)]
        private byte _r;
        [FieldOffset(3)]
        private byte _a;

        [FieldOffset(0)]
        private uint _bgra;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="StyleColor"/> from a <see cref="UInt32"/>
        /// value where the highest order byte is the blue, the next highest is green,
        /// the next is red and the lowest order byte is the alpha value.
        /// </summary>
        /// <param name="bgra">
        /// The blue, green, red, alpha encoded <see cref="UInt32"/> value to create
        /// the <see cref="StyleColor"/> from.
        /// </param>
        public StyleColor(uint bgra)
        {
            _b = _g = _r = _a = 0;
            _bgra = bgra;
        }

        /// <summary>
        /// Creates a new <see cref="StyleColor"/> from individual color components.
        /// </summary>
        /// <param name="b">The blue value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="r">The red value.</param>
        /// <param name="a">The alpha value.</param>
        /// <remarks>
        /// If any of the parameters are outside the range [0 - 255] the value
        /// is clamped on the nearest value within that range.
        /// </remarks>
        public StyleColor(Int32 b, Int32 g, Int32 r, Int32 a)
        {
            _bgra = 0;
            _b = clampToByte(b);
            _g = clampToByte(g);
            _r = clampToByte(r);
            _a = clampToByte(a);
        }

        #endregion

        public override String ToString()
        {
            return "[StyleColor] " + (LookupColorName(this) 
                ?? String.Format("B = {0}; G = {1}; R = {2}; A = {3}", B, G, R, A));
        }

        /// <summary>
        /// Generates a new <see cref="StyleColor"/> from a <see cref="UInt32"/>
        /// value where the highest order byte is the blue, the next highest is green,
        /// the next is red and the lowest order byte is the alpha value.
        /// </summary>
        /// <param name="bgra">
        /// The blue, green, red, alpha encoded <see cref="UInt32"/> value to create
        /// a <see cref="StyleColor"/> from.
        /// </param>
        /// <returns>
        /// A <see cref="StyleColor"/> with the same value as <paramref name="bgra"/>.
        /// </returns>
        public static StyleColor FromBgra(uint bgra)
        {
            return new StyleColor(bgra);
        }

        /// <summary>
        /// Generates a new <see cref="StyleColor"/> from individual color components.
        /// </summary>
        /// <param name="b">The blue value.</param>
        /// <param name="g">The green value.</param>
        /// <param name="r">The red value.</param>
        /// <param name="a">The alpha value.</param>
        /// <returns>
        /// A <see cref="StyleColor"/> instance with the specified color
        /// component values.
        /// </returns>
        /// <remarks>
        /// If any of the parameters are outside the range [0 - 255] the value
        /// is clamped on the nearest value within that range.
        /// </remarks>
        public static StyleColor FromBgra(Int32 b, Int32 g, Int32 r, Int32 a)
        {
            return new StyleColor(b, g, r, a);
        }

        /// <summary> 
        /// Creates a color using HSB.
        /// </summary> 
        /// <remarks>
        /// Adapted from the algorithm in "Computer Graphics: Principles and Practice in C", 
        /// ISBN: 978-0201848403
        /// </remarks>
        /// <param name="hue">The hue value.</param> 
        /// <param name="saturation">The saturation value.</param> 
        /// <param name="brightness">The brightness value.</param> 
        /// <returns>A <see cref="StyleColor"/> structure containing the equivalent RGBA values</returns> 
        public static StyleColor FromHsb(Double hue, Double saturation, Double brightness)
        {
            StyleColor c = new StyleColor();
            c.setByHsb(hue, saturation, brightness);
            return c;
        }

        /// <summary>
        /// Gets the color value as a BGRA-encoded <see cref="UInt32"/>.
        /// </summary>
        public UInt32 Bgra
        {
            get { return _bgra; }
        }

        /// <summary>
        /// Gets the blue component of this color.
        /// </summary>
        public Byte B
        {
            get { return _b; }
        }

        /// <summary>
        /// Gets the green component of this color.
        /// </summary>
        public Byte G
        {
            get { return _g; }
        }

        /// <summary>
        /// Gets the red component of this color.
        /// </summary>
        public Byte R
        {
            get { return _r; }
        }

        /// <summary>
        /// Gets the alpha component of this color.
        /// </summary>
        public Byte A
        {
            get { return _a; }
        }

        /// <summary>
        /// Value of pure red luminance - 0.2126.
        /// </summary>
        /// <remarks>
        /// This value is correct for modern monitors' phosphors. 
        /// See ITU-R BT.709 for definition.
        /// </remarks>
        public const Double RedLuminanceFactor = 0.2126;

        /// <summary>
        /// Value of pure green luminance - 0.7152.
        /// </summary>
        /// <remarks>
        /// This value is correct for modern monitors' phosphors. 
        /// See ITU-R BT.709 for definition.
        /// </remarks>
        public const Double GreenLuminanceFactor = 0.7152;

        /// <summary>
        /// Value of pure blue luminance - 0.0722.
        /// </summary>
        /// <remarks>
        /// This value is correct for modern monitors' phosphors. 
        /// See ITU-R BT.709 for definition.
        /// </remarks>
        public const Double BlueLuminanceFactor = 0.0722;

        /// <summary>
        /// Gets the luminance of the color based on the values 
        /// of the red, green, and blue components. 
        /// </summary>
        /// <remarks>
        /// <see cref="A">Alpha</see>
        /// is not used in the computation, since the resulting luminance depends on 
        /// what is blended with the alpha.
        /// </remarks>
        /// <returns>A value in the range [0 to 1] (inclusive).</returns>
        public Double Luminance
        {
            get
            {
                return ((BlueLuminanceFactor * B)
                        + (GreenLuminanceFactor * G)
                        + (RedLuminanceFactor * R)) / 255.0;
            }
        }

        /// <summary>
        /// Gets the hue of the color based on the values
        /// of the red, green and blue components.
        /// </summary>
        /// <returns>A value in the range [0 to 360] (inclusive).</returns>
        public Double Hue
        {
            get
            {
                if (R == G && G == B)
                {
                    return 0.0;
                }

                Double redFactor = R / 255.0;
                Double greenFactor = G / 255.0;
                Double blueFactor = B / 255.0;
                Double largestFactor = redFactor;
                Double smallestFactor = redFactor;

                if (greenFactor > largestFactor)
                {
                    largestFactor = greenFactor;
                }

                if (blueFactor > largestFactor)
                {
                    largestFactor = blueFactor;
                }

                if (greenFactor < smallestFactor)
                {
                    smallestFactor = greenFactor;
                }

                if (blueFactor < smallestFactor)
                {
                    smallestFactor = blueFactor;
                }

                Double majorFactorDifference = largestFactor - smallestFactor;
                Double hue = 0.0;

                if (redFactor == largestFactor)
                {
                    hue = (greenFactor - blueFactor) / majorFactorDifference;
                }
                else if (greenFactor == largestFactor)
                {
                    hue = 2.0 + ((blueFactor - redFactor) / majorFactorDifference);
                }
                else if (blueFactor == largestFactor)
                {
                    hue = 4.0 + ((redFactor - greenFactor) / majorFactorDifference);
                }

                hue *= 60.0;

                if (hue < 0.0)
                {
                    hue += 360.0;
                }

                return hue;
            }
        }

        /// <summary>
        /// Gets the saturation of the color based on the values
        /// of the red, green and blue components.
        /// </summary>
        /// <returns>A value in the range [0 to 1] (inclusive).</returns>
        public Double Saturation
        {
            get
            {
                Double redFactor = R / 255.0;
                Double greenFactor = G / 255.0;
                Double blueFactor = B / 255.0;
                Double saturation = 0.0;

                Double largestFactor = redFactor;
                Double smallestFactor = redFactor;

                if (greenFactor > largestFactor) largestFactor = greenFactor;
                if (blueFactor > largestFactor) largestFactor = blueFactor;
                if (greenFactor < smallestFactor) smallestFactor = greenFactor;
                if (blueFactor < smallestFactor) smallestFactor = blueFactor;

                if (largestFactor == smallestFactor)
                {
                    return saturation;
                }

                Double average = (largestFactor + smallestFactor) / 2.0;

                if (average <= 0.5)
                {
                    return ((largestFactor - smallestFactor) / (largestFactor + smallestFactor));
                }

                return ((largestFactor - smallestFactor) / ((2.0 - largestFactor) - smallestFactor));
            }
        }

        /// <summary>
        /// Gets a value indicating the relative brightness of this color.
        /// </summary>
        /// <remarks>
        /// The human eye perceives different colors as having different brightness. 
        /// Yellow, for example, is brighter than blue. This method computes the relative
        /// brightness of a given color.
        /// </remarks>
        public Double Brightness
        {
            get
            {
                // Normalize color vector (to between 0 and 1)
                Double redFactor = R / 255.0;
                Double greenFactor = G / 255.0;
                Double blueFactor = B / 255.0;

                Double largestFactor = redFactor;
                Double smallestFactor = redFactor;

                // Find the largest and smallest components
                if (greenFactor > largestFactor) largestFactor = greenFactor;
                if (blueFactor > largestFactor) largestFactor = blueFactor;
                if (greenFactor < smallestFactor) smallestFactor = greenFactor;
                if (blueFactor < smallestFactor) smallestFactor = blueFactor;

                // Return the midpoint as the brightness
                return ((largestFactor + smallestFactor) / 2f);
            }
        }

        /// <summary>
        /// Evenly interpolates between two <see cref="StyleColor"/> values
        /// in the RGB color space.
        /// </summary>
        /// <param name="color1">The first color to interpolate between</param>
        /// <param name="color2">The second color to interpolate between</param>
        /// <returns>A <see cref="StyleColor"/> on the midpoint of the distance between 
        /// <paramref name="color1"/> and <paramref name="color2"/>.</returns>
        public static StyleColor Interpolate(StyleColor color1, StyleColor color2)
        {
            return Interpolate(color1, color2, 50);
        }

        /// <summary>
        /// Interpolates between two <see cref="StyleColor"/> values
        /// in the RGB color space.
        /// </summary>
        /// <param name="color1">The first color to interpolate between</param>
        /// <param name="color2">The second color to interpolate between</param>
        /// <param name="blendFactor">The percentage of the distance between 
        /// <paramref name="color1"/> and <paramref name="color2"/> to return.</param>
        /// <returns>A <see cref="StyleColor"/> which is <paramref name="blendFactor"/> percent
        /// of the distance between <paramref name="color1"/> and <paramref name="color2"/>.
        /// </returns>
        /// <remarks>If <paramref name="blendFactor"/> is less than 0 or more than 100,
        /// the value used is 0 or 100 respectively.</remarks>
        public static StyleColor Interpolate(StyleColor color1, StyleColor color2, Double blendFactor)
        {
            // Clamp values
            if (blendFactor < 0) blendFactor = 0;
            if (blendFactor > 100) blendFactor = 100;

            blendFactor /= 100;

            // Compute interpolated value based on the linear distance
            // between 0 and 1
            if (blendFactor == 1)
            {
                return color2;
            }
            else if (blendFactor == 0)
            {
                return color1;
            }
            else
            {
                Double r = Math.Abs(color1.R - color2.R) * blendFactor + color1.R;
                Double g = Math.Abs(color1.G - color2.G) * blendFactor + color1.G;
                Double b = Math.Abs(color1.B - color2.B) * blendFactor + color1.B;
                Double a = Math.Abs(color1.A - color2.A) * blendFactor + color1.A;

                if (r > 255) r = 255;
                if (g > 255) g = 255;
                if (b > 255) b = 255;
                if (a > 255) a = 255;

                return new StyleColor((Int32)b, (Int32)g, (Int32)r, (Int32)a);
            }
        }

        #region Equality Computation

        #region IEquatable<StyleColor> Members

        /// <summary>
        /// Compares two <see cref="StyleColor"/> instances to determine if they are equal.
        /// </summary>
        public bool Equals(StyleColor other)
        {
            return this == other;
        }

        #endregion

        /// <summary>
        /// Compares two <see cref="StyleColor"/> instances to determine if they are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is StyleColor && ((StyleColor)obj).Bgra == Bgra)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if two <see cref="StyleColor"/> instances are equal, false if they are not equal.
        /// </summary>
        public static bool operator ==(StyleColor color1, StyleColor color2)
        {
            return color1.Bgra == color2.Bgra;
        }

        /// <summary>
        /// Returns true if two <see cref="StyleColor"/> instances are not equal, false if they are equal.
        /// </summary>
        public static bool operator !=(StyleColor color1, StyleColor color2)
        {
            return color1.Bgra != color2.Bgra;
        }

        #endregion

        #region Conversion Operators

        /// <summary>
        /// Casts a <see cref="StyleColor"/> to a <see cref="UInt32"/>.
        /// </summary>
        public static explicit operator UInt32(StyleColor color)
        {
            return color.Bgra;
        }

        /// <summary>
        /// Casts a <see cref="UInt32"/> to a <see cref="StyleColor"/>.
        /// </summary>
        public static explicit operator StyleColor(UInt32 value)
        {
            return new StyleColor(value);
        }

        #endregion

        #region GetHashCode

        /// <summary>
        /// Returns a hash code for this color value.
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                return (Int32)Bgra;
            }
        }

        #endregion

        #region Predefined Color Properties

        /// <summary>
        /// Gets a transparent color (BRGA = 255, 255, 255, 0).
        /// </summary>
        /// <remarks>
        /// The alpha component is 0, making this color completely transparent.
        /// </remarks>
        public static StyleColor Transparent
        {
            get { return FromBgra(255, 255, 255, 0); }
        }

        /// <summary>
        /// Gets a bluish-white (BRGA = 255, 248, 240, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(240, 248, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor AliceBlue
        {
            get { return FromBgra(255, 248, 240, 255); }
        }

        /// <summary>
        /// (BRGA = 215, 235, 250, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(250, 235, 215); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor AntiqueWhite
        {
            get { return FromBgra(215, 235, 250, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 255, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 255, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Aqua
        {
            get { return FromBgra(255, 255, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 212, 255, 127, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(127, 255, 212); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Aquamarine
        {
            get { return FromBgra(212, 255, 127, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 255, 240, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(240, 255, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Azure
        {
            get { return FromBgra(255, 255, 240, 255); }
        }

        /// <summary>
        /// (BRGA = 220, 245, 245, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(245, 245, 220); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Beige
        {
            get { return FromBgra(220, 245, 245, 255); }
        }

        /// <summary>
        /// (BRGA = 196, 228, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 228, 196); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Bisque
        {
            get { return FromBgra(196, 228, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 0, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 0, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Black
        {
            get { return FromBgra(0, 0, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 205, 235, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 235, 205); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor BlanchedAlmond
        {
            get { return FromBgra(205, 235, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 0, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 0, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Blue
        {
            get { return FromBgra(255, 0, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 226, 43, 138, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(138, 43, 226); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor BlueViolet
        {
            get { return FromBgra(226, 43, 138, 255); }
        }

        /// <summary>
        /// (BRGA = 42, 42, 165, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(165, 42, 42); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Brown
        {
            get { return FromBgra(42, 42, 165, 255); }
        }

        /// <summary>
        /// (BRGA = 135, 184, 222, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(222, 184, 135); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor BurlyWood
        {
            get { return FromBgra(135, 184, 222, 255); }
        }

        /// <summary>
        /// (BRGA = 160, 158, 95, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(95, 158, 160); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor CadetBlue
        {
            get { return FromBgra(160, 158, 95, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 255, 127, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(127, 255, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Chartreuse
        {
            get { return FromBgra(0, 255, 127, 255); }
        }

        /// <summary>
        /// (BRGA = 30, 105, 210, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(210, 105, 30); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Chocolate
        {
            get { return FromBgra(30, 105, 210, 255); }
        }

        /// <summary>
        /// (BRGA = 80, 127, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 127, 80); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Coral
        {
            get { return FromBgra(80, 127, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 237, 149, 100, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(100, 149, 237); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor CornflowerBlue
        {
            get { return FromBgra(237, 149, 100, 255); }
        }

        /// <summary>
        /// (BRGA = 220, 248, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 248, 220); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Cornsilk
        {
            get { return FromBgra(220, 248, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 60, 20, 220, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(220, 20, 60); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Crimson
        {
            get { return FromBgra(60, 20, 220, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 255, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 255, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Cyan
        {
            get { return FromBgra(255, 255, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 139, 0, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 0, 139); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkBlue
        {
            get { return FromBgra(139, 0, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 139, 139, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 139, 139); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkCyan
        {
            get { return FromBgra(139, 139, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 11, 134, 184, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(184, 134, 11); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkGoldenrod
        {
            get { return FromBgra(11, 134, 184, 255); }
        }

        /// <summary>
        /// (BRGA = 169, 169, 169, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(169, 169, 169); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkGray
        {
            get { return FromBgra(169, 169, 169, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 100, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 100, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkGreen
        {
            get { return FromBgra(0, 100, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 107, 183, 189, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(189, 183, 107); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkKhaki
        {
            get { return FromBgra(107, 183, 189, 255); }
        }

        /// <summary>
        /// (BRGA = 139, 0, 139, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(139, 0, 139); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkMagenta
        {
            get { return FromBgra(139, 0, 139, 255); }
        }

        /// <summary>
        /// (BRGA = 47, 107, 85, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(85, 107, 47); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkOliveGreen
        {
            get { return FromBgra(47, 107, 85, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 140, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 140, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkOrange
        {
            get { return FromBgra(0, 140, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 204, 50, 153, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(153, 50, 204); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkOrchid
        {
            get { return FromBgra(204, 50, 153, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 0, 139, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(139, 0, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkRed
        {
            get { return FromBgra(0, 0, 139, 255); }
        }

        /// <summary>
        /// (BRGA = 122, 150, 233, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(233, 150, 122); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkSalmon
        {
            get { return FromBgra(122, 150, 233, 255); }
        }

        /// <summary>
        /// (BRGA = 139, 188, 143, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(143, 188, 139); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkSeaGreen
        {
            get { return FromBgra(139, 188, 143, 255); }
        }

        /// <summary>
        /// (BRGA = 139, 61, 72, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(72, 61, 139); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkSlateBlue
        {
            get { return FromBgra(139, 61, 72, 255); }
        }

        /// <summary>
        /// (BRGA = 79, 79, 47, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(47, 79, 79); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkSlateGray
        {
            get { return FromBgra(79, 79, 47, 255); }
        }

        /// <summary>
        /// (BRGA = 209, 206, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 206, 209); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkTurquoise
        {
            get { return FromBgra(209, 206, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 211, 0, 148, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(148, 0, 211); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DarkViolet
        {
            get { return FromBgra(211, 0, 148, 255); }
        }

        /// <summary>
        /// (BRGA = 147, 20, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 20, 147); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DeepPink
        {
            get { return FromBgra(147, 20, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 191, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 191, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DeepSkyBlue
        {
            get { return FromBgra(255, 191, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 105, 105, 105, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(105, 105, 105); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DimGray
        {
            get { return FromBgra(105, 105, 105, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 144, 30, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(30, 144, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor DodgerBlue
        {
            get { return FromBgra(255, 144, 30, 255); }
        }

        /// <summary>
        /// (BRGA = 34, 34, 178, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(178, 34, 34); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Firebrick
        {
            get { return FromBgra(34, 34, 178, 255); }
        }

        /// <summary>
        /// (BRGA = 240, 250, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 250, 240); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor FloralWhite
        {
            get { return FromBgra(240, 250, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 34, 139, 34, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(34, 139, 34); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor ForestGreen
        {
            get { return FromBgra(34, 139, 34, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 0, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 0, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Fuchsia
        {
            get { return FromBgra(255, 0, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 220, 220, 220, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(220, 220, 220); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Gainsboro
        {
            get { return FromBgra(220, 220, 220, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 248, 248, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(248, 248, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor GhostWhite
        {
            get { return FromBgra(255, 248, 248, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 215, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 215, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Gold
        {
            get { return FromBgra(0, 215, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 32, 165, 218, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(218, 165, 32); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Goldenrod
        {
            get { return FromBgra(32, 165, 218, 255); }
        }

        /// <summary>
        /// (BRGA = 128, 128, 128, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(128, 128, 128); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Gray
        {
            get { return FromBgra(128, 128, 128, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 128, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 128, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Green
        {
            get { return FromBgra(0, 128, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 47, 255, 173, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(173, 255, 47); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor GreenYellow
        {
            get { return FromBgra(47, 255, 173, 255); }
        }

        /// <summary>
        /// (BRGA = 240, 255, 240, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(240, 255, 240); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Honeydew
        {
            get { return FromBgra(240, 255, 240, 255); }
        }

        /// <summary>
        /// (BRGA = 180, 105, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 105, 180); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor HotPink
        {
            get { return FromBgra(180, 105, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 92, 92, 205, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(205, 92, 92); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor IndianRed
        {
            get { return FromBgra(92, 92, 205, 255); }
        }

        /// <summary>
        /// (BRGA = 130, 0, 75, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(75, 0, 130); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Indigo
        {
            get { return FromBgra(130, 0, 75, 255); }
        }

        /// <summary>
        /// (BRGA = 240, 255, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 255, 240); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Ivory
        {
            get { return FromBgra(240, 255, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 140, 230, 240, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(240, 230, 140); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Khaki
        {
            get { return FromBgra(140, 230, 240, 255); }
        }

        /// <summary>
        /// (BRGA = 250, 230, 230, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(230, 230, 250); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Lavender
        {
            get { return FromBgra(250, 230, 230, 255); }
        }

        /// <summary>
        /// (BRGA = 245, 240, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 240, 245); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LavenderBlush
        {
            get { return FromBgra(245, 240, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 252, 124, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 124, 252); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LawnGreen
        {
            get { return FromBgra(0, 252, 124, 255); }
        }

        /// <summary>
        /// (BRGA = 205, 250, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 250, 205); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LemonChiffon
        {
            get { return FromBgra(205, 250, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 230, 216, 173, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(173, 216, 230); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightBlue
        {
            get { return FromBgra(230, 216, 173, 255); }
        }

        /// <summary>
        /// (BRGA = 128, 128, 240, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(240, 128, 128); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightCoral
        {
            get { return FromBgra(128, 128, 240, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 255, 224, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(224, 255, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightCyan
        {
            get { return FromBgra(255, 255, 224, 255); }
        }

        /// <summary>
        /// (BRGA = 210, 250, 250, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(250, 250, 210); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightGoldenrodYellow
        {
            get { return FromBgra(210, 250, 250, 255); }
        }

        /// <summary>
        /// (BRGA = 144, 238, 144, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(144, 238, 144); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightGreen
        {
            get { return FromBgra(144, 238, 144, 255); }
        }

        /// <summary>
        /// (BRGA = 211, 211, 211, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(211, 211, 211); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightGray
        {
            get { return FromBgra(211, 211, 211, 255); }
        }

        /// <summary>
        /// (BRGA = 193, 182, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 182, 193); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightPink
        {
            get { return FromBgra(193, 182, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 122, 160, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 160, 122); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightSalmon
        {
            get { return FromBgra(122, 160, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 170, 178, 32, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(32, 178, 170); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightSeaGreen
        {
            get { return FromBgra(170, 178, 32, 255); }
        }

        /// <summary>
        /// (BRGA = 250, 206, 135, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(135, 206, 250); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightSkyBlue
        {
            get { return FromBgra(250, 206, 135, 255); }
        }

        /// <summary>
        /// (BRGA = 153, 136, 119, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(119, 136, 153); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightSlateGray
        {
            get { return FromBgra(153, 136, 119, 255); }
        }

        /// <summary>
        /// (BRGA = 222, 196, 176, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(176, 196, 222); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightSteelBlue
        {
            get { return FromBgra(222, 196, 176, 255); }
        }

        /// <summary>
        /// (BRGA = 224, 255, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 255, 224); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LightYellow
        {
            get { return FromBgra(224, 255, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 255, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 255, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Lime
        {
            get { return FromBgra(0, 255, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 50, 205, 50, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(50, 205, 50); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor LimeGreen
        {
            get { return FromBgra(50, 205, 50, 255); }
        }

        /// <summary>
        /// (BRGA = 230, 240, 250, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(250, 240, 230); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Linen
        {
            get { return FromBgra(230, 240, 250, 255); }
        }

        /// <summary>
        /// (BRGA = 255, 0, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 0, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Magenta
        {
            get { return FromBgra(255, 0, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 0, 128, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(128, 0, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Maroon
        {
            get { return FromBgra(0, 0, 128, 255); }
        }

        /// <summary>
        /// (BRGA = 170, 205, 102, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(102, 205, 170); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumAquamarine
        {
            get { return FromBgra(170, 205, 102, 255); }
        }

        /// <summary>
        /// (BRGA = 205, 0, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 0, 205); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumBlue
        {
            get { return FromBgra(205, 0, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 211, 85, 186, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(186, 85, 211); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumOrchid
        {
            get { return FromBgra(211, 85, 186, 255); }
        }

        /// <summary>
        /// (BRGA = 219, 112, 147, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(147, 112, 219); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumPurple
        {
            get { return FromBgra(219, 112, 147, 255); }
        }

        /// <summary>
        /// (BRGA = 113, 179, 60, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(60, 179, 113); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumSeaGreen
        {
            get { return FromBgra(113, 179, 60, 255); }
        }

        /// <summary>
        /// (BRGA = 238, 104, 123, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(123, 104, 238); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumSlateBlue
        {
            get { return FromBgra(238, 104, 123, 255); }
        }

        /// <summary>
        /// (BRGA = 154, 250, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 250, 154); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumSpringGreen
        {
            get { return FromBgra(154, 250, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 204, 209, 72, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(72, 209, 204); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumTurquoise
        {
            get { return FromBgra(204, 209, 72, 255); }
        }

        /// <summary>
        /// (BRGA = 133, 21, 199, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(199, 21, 133); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MediumVioletRed
        {
            get { return FromBgra(133, 21, 199, 255); }
        }

        /// <summary>
        /// (BRGA = 112, 25, 25, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(25, 25, 112); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MidnightBlue
        {
            get { return FromBgra(112, 25, 25, 255); }
        }

        /// <summary>
        /// (BRGA = 250, 255, 245, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(245, 255, 250); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MintCream
        {
            get { return FromBgra(250, 255, 245, 255); }
        }

        /// <summary>
        /// (BRGA = 225, 228, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 228, 225); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor MistyRose
        {
            get { return FromBgra(225, 228, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 181, 228, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 228, 181); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Moccasin
        {
            get { return FromBgra(181, 228, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 173, 222, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 222, 173); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor NavajoWhite
        {
            get { return FromBgra(173, 222, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 128, 0, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 0, 128); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Navy
        {
            get { return FromBgra(128, 0, 0, 255); }
        }

        /// <summary>
        /// (BRGA = 230, 245, 253, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(253, 245, 230); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor OldLace
        {
            get { return FromBgra(230, 245, 253, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 128, 128, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(128, 128, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Olive
        {
            get { return FromBgra(0, 128, 128, 255); }
        }

        /// <summary>
        /// (BRGA = 35, 142, 107, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(107, 142, 35); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor OliveDrab
        {
            get { return FromBgra(35, 142, 107, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 165, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 165, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Orange
        {
            get { return FromBgra(0, 165, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 69, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 69, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor OrangeRed
        {
            get { return FromBgra(0, 69, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 214, 112, 218, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(218, 112, 214); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Orchid
        {
            get { return FromBgra(214, 112, 218, 255); }
        }

        /// <summary>
        /// (BRGA = 170, 232, 238, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(238, 232, 170); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PaleGoldenrod
        {
            get { return FromBgra(170, 232, 238, 255); }
        }

        /// <summary>
        /// (BRGA = 152, 251, 152, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(152, 251, 152); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PaleGreen
        {
            get { return FromBgra(152, 251, 152, 255); }
        }

        /// <summary>
        /// (BRGA = 238, 238, 175, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(175, 238, 238); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PaleTurquoise
        {
            get { return FromBgra(238, 238, 175, 255); }
        }

        /// <summary>
        /// (BRGA = 147, 112, 219, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(219, 112, 147); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PaleVioletRed
        {
            get { return FromBgra(147, 112, 219, 255); }
        }

        /// <summary>
        /// (BRGA = 213, 239, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 239, 213); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PapayaWhip
        {
            get { return FromBgra(213, 239, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 185, 218, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 218, 185); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PeachPuff
        {
            get { return FromBgra(185, 218, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 63, 133, 205, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(205, 133, 63); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Peru
        {
            get { return FromBgra(63, 133, 205, 255); }
        }

        /// <summary>
        /// (BRGA = 203, 192, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 192, 203); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Pink
        {
            get { return FromBgra(203, 192, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 221, 160, 221, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(221, 160, 221); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Plum
        {
            get { return FromBgra(221, 160, 221, 255); }
        }

        /// <summary>
        /// (BRGA = 230, 224, 176, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(176, 224, 230); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor PowderBlue
        {
            get { return FromBgra(230, 224, 176, 255); }
        }

        /// <summary>
        /// (BRGA = 128, 0, 128, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(128, 0, 128); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Purple
        {
            get { return FromBgra(128, 0, 128, 255); }
        }

        /// <summary>
        /// (BRGA = 0, 0, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 0, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Red
        {
            get { return FromBgra(0, 0, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 143, 143, 188, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(188, 143, 143); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor RosyBrown
        {
            get { return FromBgra(143, 143, 188, 255); }
        }

        /// <summary>
        /// (BRGA = 225, 105, 65, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(65, 105, 225); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor RoyalBlue
        {
            get { return FromBgra(225, 105, 65, 255); }
        }

        /// <summary>
        /// (BRGA = 19, 69, 139, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(139, 69, 19); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SaddleBrown
        {
            get { return FromBgra(19, 69, 139, 255); }
        }

        /// <summary>
        /// (BRGA = 114, 128, 250, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(250, 128, 114); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Salmon
        {
            get { return FromBgra(114, 128, 250, 255); }
        }

        /// <summary>
        /// (BRGA = 96, 164, 244, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(244, 164, 96); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SandyBrown
        {
            get { return FromBgra(96, 164, 244, 255); }
        }

        /// <summary>
        /// (BRGA = 87, 139, 46, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(46, 139, 87); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SeaGreen
        {
            get { return FromBgra(87, 139, 46, 255); }
        }

        /// <summary>
        /// (BRGA = 238, 245, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 245, 238); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SeaShell
        {
            get { return FromBgra(238, 245, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 45, 82, 160, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(160, 82, 45); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Sienna
        {
            get { return FromBgra(45, 82, 160, 255); }
        }

        /// <summary>
        /// (BRGA = 192, 192, 192, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(192, 192, 192); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Silver
        {
            get { return FromBgra(192, 192, 192, 255); }
        }

        /// <summary>
        /// (BRGA = 235, 206, 135, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(135, 206, 235); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SkyBlue
        {
            get { return FromBgra(235, 206, 135, 255); }
        }

        /// <summary>
        /// (BRGA = 205, 90, 106, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(106, 90, 205); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SlateBlue
        {
            get { return FromBgra(205, 90, 106, 255); }
        }

        /// <summary>
        /// (BRGA = 144, 128, 112, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(112, 128, 144); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SlateGray
        {
            get { return FromBgra(144, 128, 112, 255); }
        }

        /// <summary>
        /// (BRGA = 250, 250, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 250, 250); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Snow
        {
            get { return FromBgra(250, 250, 255, 255); }
        }

        /// <summary>
        /// (BRGA = 127, 255, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 255, 127); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SpringGreen
        {
            get { return FromBgra(127, 255, 0, 255); }
        }

        /// <summary>
        /// Gets a greyish pastel blue (BRGA = 180, 130, 70, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(70, 130, 180); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor SteelBlue
        {
            get { return FromBgra(180, 130, 70, 255); }
        }

        /// <summary>
        /// Gets a light brown (BRGA = 140, 180, 210, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(210, 180, 140); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Tan
        {
            get { return FromBgra(140, 180, 210, 255); }
        }

        /// <summary>
        /// Gets a blueish-green (BRGA = 128, 128, 0, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(0, 128, 128); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Teal
        {
            get { return FromBgra(128, 128, 0, 255); }
        }

        /// <summary>
        /// Gets a medium pastel purple (BRGA = 216, 191, 216, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(216, 191, 216); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Thistle
        {
            get { return FromBgra(216, 191, 216, 255); }
        }

        /// <summary>
        /// Gets a bright orangish-red (BRGA = 71, 99, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 99, 71); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Tomato
        {
            get { return FromBgra(71, 99, 255, 255); }
        }

        /// <summary>
        /// Gets a bright greenish-blue (BRGA = 208, 224, 64, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(64, 224, 208); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Turquoise
        {
            get { return FromBgra(208, 224, 64, 255); }
        }

        /// <summary>
        /// Gets a bright pinkish-purple (BRGA = 238, 130, 238, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(238, 130, 238); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Violet
        {
            get { return FromBgra(238, 130, 238, 255); }
        }

        /// <summary>
        /// Gets a light tan (BRGA = 179, 222, 245, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(245, 222, 179); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Wheat
        {
            get { return FromBgra(179, 222, 245, 255); }
        }

        /// <summary>
        /// Gets a pure white (BRGA = 255, 255, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 255, 255); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor White
        {
            get { return FromBgra(255, 255, 255, 255); }
        }

        /// <summary>
        /// Gets a very light whitish-grey (BRGA = 245, 245, 245, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(245, 245, 245); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor WhiteSmoke
        {
            get { return FromBgra(245, 245, 245, 255); }
        }

        /// <summary>
        /// Gets a pure yellow (BRGA = 0, 255, 255, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 255, 0); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor Yellow
        {
            get { return FromBgra(0, 255, 255, 255); }
        }

        /// <summary>
        /// Gets a bright yellow-green (BRGA = 50, 205, 154, 255).
        /// </summary>
        /// <remarks>
        /// Here is an example: 
        /// <div style="width: 100%; height: 25px; background-color: rgb(255, 154, 205); border: solid 1px black;"></div>
        /// </remarks>
        public static StyleColor YellowGreen
        {
            get { return FromBgra(50, 205, 154, 255); }
        }

        #endregion

        #region Zero

        /// <summary>
        /// Gets a zero value as a StyleColor.
        /// </summary>
        public static StyleColor Zero
        {
            get { return (StyleColor)0; }
        }

        #endregion

        #region Color name lookup by value

        private static readonly object _colorNameLookupSync = new object();
        private static readonly Dictionary<StyleColor, String> _colorNameLookup 
            = new Dictionary<StyleColor, String>();

        /// <summary>
        /// Gets the name of a color, if one exists.
        /// </summary>
        /// <param name="color">The color value to lookup.</param>
        /// <returns>The name of the color if one exists, otherwise null.</returns>
        public static String LookupColorName(StyleColor color)
        {
            if (_colorNameLookup.Count == 0)
            {
                lock (_colorNameLookupSync)
                {
                    if (_colorNameLookup.Count == 0)
                    {
                        Type colorType = typeof(StyleColor);
                        PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.Public);

                        foreach (PropertyInfo pi in propInfos)
                        {
                            if (pi.PropertyType == colorType)
                            {
                                _colorNameLookup[(StyleColor)pi.GetValue(null, null)] = pi.Name;
                            }
                        }
                    }
                }
            }

            String name;
            _colorNameLookup.TryGetValue(color, out name);
            return name;
        }

        #endregion

        #region Predefined Colors Dictionary

        private static readonly object _predefinedColorsSync = new object();
        private static readonly Dictionary<String, StyleColor> _predefinedColors = new Dictionary<String, StyleColor>();

        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.Dictionary{String, StyleColor}"/> 
        /// which indexes a StyleColor value by its name.
        /// </summary>
        public static Dictionary<String, StyleColor> PredefinedColors
        {
            get
            {
                if (_predefinedColors.Count == 0)
                {
                    lock (_predefinedColorsSync)
                    {
                        if (_predefinedColors.Count == 0)
                        {
                            Type colorType = typeof(StyleColor);
                            PropertyInfo[] propInfos =
                                colorType.GetProperties(BindingFlags.Static | BindingFlags.Public);

                            foreach (PropertyInfo pi in propInfos)
                            {
                                if (pi.PropertyType == colorType)
                                {
                                    _predefinedColors.Add(pi.Name, (StyleColor)pi.GetValue(null, null));
                                }
                            }
                        }
                    }
                }

                return new Dictionary<String, StyleColor>(_predefinedColors);
            }
        }

        #endregion

        #region Private helper methods

        private static byte clampToByte(Int32 value)
        {
            return value > 255 ? (byte)255 : value < 0 ? (byte)0 : (byte)value;
        }

        private void setByHsb(Double h, Double s, Double v)
        {
            _r = 0;
            _g = 0;
            _b = 0;
            _a = 255;

            Double r, g, b;

            if (v == 0)
            {
                return;
            }

            if (s == 0)
            {
                r = g = b = v;
            }
            else
            {
                Double temp1, temp2;
                h = h % 360;
                temp2 = ((v <= 0.5) ? v * (1.0 + s) : v + s - (v * s));
                temp1 = 2.0 * v - temp2;

                Double[] t3 = new Double[] { h + 1.0 / 3.0, h, h - 1.0 / 3.0 };
                Double[] clr = new Double[] { 0, 0, 0 };

                for (Int32 i = 0; i < 3; i++)
                {
                    if (t3[i] < 0)
                    {
                        t3[i] += 1.0;
                    }

                    if (t3[i] > 1)
                    {
                        t3[i] -= 1.0;
                    }

                    if (6.0 * t3[i] < 1.0)
                    {
                        clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                    }
                    else if (2.0 * t3[i] < 1.0)
                    {
                        clr[i] = temp2;
                    }
                    else if (3.0 * t3[i] < 2.0)
                    {
                        clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                    }
                    else
                    {
                        clr[i] = temp1;
                    }
                }

                r = clr[0];
                g = clr[1];
                b = clr[2];
            }

            _b = (byte)(255 * b);
            _g = (byte)(255 * g);
            _r = (byte)(255 * r);
        }

        #endregion
    }
}