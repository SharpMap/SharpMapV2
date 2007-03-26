using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpMap.Styles
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct StyleColor
    {
        [FieldOffset(0)] private byte _b;
        [FieldOffset(1)] private byte _g;
        [FieldOffset(2)] private byte _r;
        [FieldOffset(3)] private byte _a;

        [FieldOffset(0)] private uint _bgra;

        public StyleColor(uint bgra)
        {
            _b = _g = _r = _a = 0;
            _bgra = bgra;
        }

        public StyleColor(int b, int g, int r, int a)
        {
            _bgra = 0;
            _b = clampToByte(b);
            _g = clampToByte(g);
            _r = clampToByte(r);
            _a = clampToByte(a);
        }

        private static byte clampToByte(int value)
        {
            return value > 255 ? (byte)255 : value < 0 ? (byte)0 : (byte)value;
        }

        public static StyleColor FromBgra(uint bgra)
        {
            return new StyleColor(bgra);
        }

        public static StyleColor FromBgra(int b, int g, int r, int a)
        {
            return new StyleColor(b, g, r, a);
        }

        /// <summary> 
        /// Creates a color using HSB
        /// </summary> 
        /// <remarks>Adapted from the algoritm in "Computer Graphics: Principles and Practice in C", ISBN: 978-0201848403</remarks>
        /// <param name="hue">The hue value.</param> 
        /// <param name="saturation">The saturation value.</param> 
        /// <param name="brightness">The brightness value.</param> 
        /// <returns>A <see cref="Color"/> structure containing the equivalent RGBA values</returns> 
        public static StyleColor FromHsb(double hue, double saturation, double brightness)
        {
            StyleColor c = new StyleColor();
            c.setByHsb(hue, saturation, brightness);
            return c;
        }

        public UInt32 Bgra
        {
            get { return _bgra; }
        }

        public Byte B
        {
            get { return _b; }
        }

        public Byte G
        {
            get { return _g; }
        }

        public Byte R
        {
            get { return _a; }
        }

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
        public const double RedLuminanceFactor = 0.2126;

        /// <summary>
        /// Value of pure green luminance - 0.7152.
        /// </summary>
        /// <remarks>
        /// This value is correct for modern monitors' phosphors. 
        /// See ITU-R BT.709 for definition.
        /// </remarks>
        public const double GreenLuminanceFactor = 0.7152;

        /// <summary>
        /// Value of pure blue luminance - 0.0722.
        /// </summary>
        /// <remarks>
        /// This value is correct for modern monitors' phosphors. 
        /// See ITU-R BT.709 for definition.
        /// </remarks>
        public const double BlueLuminanceFactor = 0.0722;

        /// <summary>
        /// Gets the luminance of the color based on the values 
        /// of the red, green, and blue components. <see cref="A">Alpha</see>
        /// is not used in the computation, since the resulting luminance depends on 
        /// what is blended with the alpha.
        /// </summary>
        /// <returns>A value in the range [0 to 1] (inclusive).</returns>
        public double Luminance
        {
            get
            {
                return ((BlueLuminanceFactor * (double)B)
                    + (GreenLuminanceFactor * (double)G)
                    + (RedLuminanceFactor * (double)R)) / 255.0;
            }
        }

        public double Hue
        {
            get
            {
                if (R == G && G == B)
                    return 0.0;

                double redFactor = ((double)R) / 255.0;
                double greenFactor = ((double)G) / 255.0;
                double blueFactor = ((double)B) / 255.0;
                double largestFactor = redFactor;
                double smallestFactor = redFactor;

                if (greenFactor > largestFactor)
                    largestFactor = greenFactor;

                if (blueFactor > largestFactor)
                    largestFactor = blueFactor;

                if (greenFactor < smallestFactor)
                    smallestFactor = greenFactor;

                if (blueFactor < smallestFactor)
                    smallestFactor = blueFactor;

                double majorFactorDifference = largestFactor - smallestFactor;
                double hue = 0.0;

                if (redFactor == largestFactor)
                    hue = (greenFactor - blueFactor) / majorFactorDifference;
                else if (greenFactor == largestFactor)
                    hue = 2.0 + ((blueFactor - redFactor) / majorFactorDifference);
                else if (blueFactor == largestFactor)
                    hue = 4.0 + ((redFactor - greenFactor) / majorFactorDifference);

                hue *= 60.0;

                if (hue < 0.0)
                    hue += 360.0;

                return hue;
            }
        }

        public double Saturation
        {
            get
            {
                double redFactor = ((double)R) / 255.0;
                double greenFactor = ((double)G) / 255.0;
                double blueFactor = ((double)B) / 255.0;
                double saturation = 0.0;

                double largestFactor = redFactor;
                double smallestFactor = redFactor;
                
                if (greenFactor > largestFactor)
                    largestFactor = greenFactor;

                if (blueFactor > largestFactor)
                    largestFactor = blueFactor;

                if (greenFactor < smallestFactor)
                    smallestFactor = greenFactor;

                if (blueFactor < smallestFactor)
                    smallestFactor = blueFactor;

                if (largestFactor == smallestFactor)
                    return saturation;

                double average = (largestFactor + smallestFactor) / 2.0;
                
                if (average <= 0.5)
                {
                    return ((largestFactor - smallestFactor) / (largestFactor + smallestFactor));
                }

                return ((largestFactor - smallestFactor) / ((2.0 - largestFactor) - smallestFactor));
            }
        }

        public double Brightness
        {
            get
            {
                double redFactor = ((double)R) / 255.0;
                double greenFactor = ((double)G) / 255.0;
                double blueFactor = ((double)B) / 255.0;
                double largestFactor = redFactor;
                double smallestFactor = redFactor;

                if (greenFactor > largestFactor)
                    largestFactor = greenFactor;

                if (blueFactor > largestFactor)
                    largestFactor = blueFactor;

                if (greenFactor < smallestFactor)
                    smallestFactor = greenFactor;

                if (blueFactor < smallestFactor)
                    smallestFactor = blueFactor;

                return ((largestFactor + smallestFactor) / 2f);
            }
        }

        /// <summary>
        /// Evenly interpolates between two <see cref="Color"/> values
        /// in the RGB color space.
        /// </summary>
        /// <param name="color1">The first color to interpolate between</param>
        /// <param name="color2">The second color to interpolate between</param>
        /// <returns>A <see cref="Color"/> on the midpoint of the distance between 
        /// <paramref name="color1"/> and <paramref name="color2"/>.</returns>
        public static StyleColor Interpolate(StyleColor color1, StyleColor color2)
        {
            return Interpolate(color1, color2, 50);
        }

        /// <summary>
        /// Interpolates between two <see cref="Color"/> values
        /// in the RGB color space.
        /// </summary>
        /// <param name="color1">The first color to interpolate between</param>
        /// <param name="color2">The second color to interpolate between</param>
        /// <param name="blendFactor">The percentage of the distance between 
        /// <paramref name="color1"/> and <paramref name="color2"/> to return.</param>
        /// <returns>A <see cref="Color"/> which is <paramref name="blendFactor"/> percent
        /// of the distance between <paramref name="color1"/> and <paramref name="color2"/>.
        /// </returns>
        /// <remarks>If <paramref name="blendFactor"/> is less than 0 or more than 100,
        /// the value used is 0 or 100 respectively.</remarks>
        public static StyleColor Interpolate(StyleColor color1, StyleColor color2, double blendFactor)
        {
            if (blendFactor < 0)
                blendFactor = 0;
            if (blendFactor > 100)
                blendFactor = 100;

            blendFactor /= 100;

            if (blendFactor == 1)
                return color2;
            else if (blendFactor == 0)
                return color1;
            else
            {
                double r = Math.Abs(color1.R - color2.R) * blendFactor + color1.R;
                double g = Math.Abs(color1.G - color2.G) * blendFactor + color1.G;
                double b = Math.Abs(color1.B - color2.B) * blendFactor + color1.B;
                double a = Math.Abs(color1.A - color2.A) * blendFactor + color1.A;

                if (r > 255) r = 255;
                if (g > 255) g = 255;
                if (b > 255) b = 255;
                if (a > 255) a = 255;

                return new StyleColor((int)b, (int)g, (int)r, (int)a);
            }
        }

        /// <summary>
        /// Returns true if two <see cref="Color"/> instances are equal, false if they are not equal.
        /// </summary>
        public static bool operator ==(StyleColor color1, StyleColor color2)
        {
            return color1.Bgra == color2.Bgra;
        }

        /// <summary>
        /// Returns true if two <see cref="Color"/> instances are not equal, false if they are equal.
        /// </summary>
        public static bool operator !=(StyleColor color1, StyleColor color2)
        {
            return color1.Bgra != color2.Bgra;
        }

        /// <summary>
        /// Casts a <see cref="Color"/> to a <see cref="UInt32"/>.
        /// </summary>
        public static explicit operator UInt32(StyleColor color)
        {
            return color.Bgra;
        }

        /// <summary>
        /// Casts a <see cref="UInt32"/> to a <see cref="Color"/>.
        /// </summary>
        public static explicit operator StyleColor(UInt32 value)
        {
            return new StyleColor(value);
        }

        /// <summary>
        /// Compares two Color instance to determine if they are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is StyleColor && ((StyleColor)obj).Bgra == this.Bgra)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns a hash code for this color value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked { return (int)Bgra; }
        }

        private void setByHsb(double h, double s, double v)
        {
            _r = 0;
            _g = 0;
            _b = 0;
            _a = 255;

            double temp1, temp2;
            double r = 0, g = 0, b = 0;

            if (v == 0)
                return;

            if (s == 0)
            {
                r = g = b = v;
            }
            else
            {
                h = h % 360;
                temp2 = ((v <= 0.5) ? v * (1.0 + s) : v + s - (v * s));
                temp1 = 2.0 * v - temp2;

                double[] t3 = new double[] { h + 1.0 / 3.0, h, h - 1.0 / 3.0 };
                double[] clr = new double[] { 0, 0, 0 };

                for (int i = 0; i < 3; i++)
                {
                    if (t3[i] < 0)
                        t3[i] += 1.0;

                    if (t3[i] > 1)
                        t3[i] -= 1.0;

                    if (6.0 * t3[i] < 1.0)
                        clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                    else if (2.0 * t3[i] < 1.0)
                        clr[i] = temp2;
                    else if (3.0 * t3[i] < 2.0)
                        clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                    else
                        clr[i] = temp1;
                }

                r = clr[0];
                g = clr[1];
                b = clr[2];
            }

            _b = (byte)(255 * b);
            _g = (byte)(255 * g);
            _r = (byte)(255 * r);
        }

        public static StyleColor Transparent
        {
            get { return StyleColor.FromBgra(255, 255, 255, 0); }
        }

        public static StyleColor AliceBlue
        {
            get
            {
                return StyleColor.FromBgra(255, 248, 240, 255);
            }
        }

        public static StyleColor AntiqueWhite
        {
            get
            {
                return StyleColor.FromBgra(215, 235, 250, 255);
            }
        }

        public static StyleColor Aqua
        {
            get
            {
                return StyleColor.FromBgra(255, 255, 0, 255);
            }
        }

        public static StyleColor Aquamarine
        {
            get
            {
                return StyleColor.FromBgra(212, 255, 127, 255);
            }
        }

        public static StyleColor Azure
        {
            get
            {
                return StyleColor.FromBgra(255, 255, 240, 255);
            }
        }

        public static StyleColor Beige
        {
            get
            {
                return StyleColor.FromBgra(220, 245, 245, 255);
            }
        }

        public static StyleColor Bisque
        {
            get
            {
                return StyleColor.FromBgra(196, 228, 255, 255);
            }
        }

        public static StyleColor Black
        {
            get
            {
                return StyleColor.FromBgra(0, 0, 0, 255);
            }
        }

        public static StyleColor BlanchedAlmond
        {
            get
            {
                return StyleColor.FromBgra(205, 235, 255, 255);
            }
        }

        public static StyleColor Blue
        {
            get
            {
                return StyleColor.FromBgra(255, 0, 0, 255);
            }
        }

        public static StyleColor BlueViolet
        {
            get
            {
                return StyleColor.FromBgra(226, 43, 138, 255);
            }
        }

        public static StyleColor Brown
        {
            get
            {
                return StyleColor.FromBgra(42, 42, 165, 255);
            }
        }

        public static StyleColor BurlyWood
        {
            get
            {
                return StyleColor.FromBgra(135, 184, 222, 255);
            }
        }

        public static StyleColor CadetBlue
        {
            get
            {
                return StyleColor.FromBgra(160, 158, 95, 255);
            }
        }

        public static StyleColor Chartreuse
        {
            get
            {
                return StyleColor.FromBgra(0, 255, 127, 255);
            }
        }

        public static StyleColor Chocolate
        {
            get
            {
                return StyleColor.FromBgra(30, 105, 210, 255);
            }
        }

        public static StyleColor Coral
        {
            get
            {
                return StyleColor.FromBgra(80, 127, 255, 255);
            }
        }

        public static StyleColor CornflowerBlue
        {
            get
            {
                return StyleColor.FromBgra(237, 149, 100, 255);
            }
        }

        public static StyleColor Cornsilk
        {
            get
            {
                return StyleColor.FromBgra(220, 248, 255, 255);
            }
        }

        public static StyleColor Crimson
        {
            get
            {
                return StyleColor.FromBgra(60, 20, 220, 255);
            }
        }

        public static StyleColor Cyan
        {
            get
            {
                return StyleColor.FromBgra(255, 255, 0, 255);
            }
        }

        public static StyleColor DarkBlue
        {
            get
            {
                return StyleColor.FromBgra(139, 0, 0, 255);
            }
        }

        public static StyleColor DarkCyan
        {
            get
            {
                return StyleColor.FromBgra(139, 139, 0, 255);
            }
        }

        public static StyleColor DarkGoldenrod
        {
            get
            {
                return StyleColor.FromBgra(11, 134, 184, 255);
            }
        }

        public static StyleColor DarkGray
        {
            get
            {
                return StyleColor.FromBgra(169, 169, 169, 255);
            }
        }

        public static StyleColor DarkGreen
        {
            get
            {
                return StyleColor.FromBgra(0, 100, 0, 255);
            }
        }

        public static StyleColor DarkKhaki
        {
            get
            {
                return StyleColor.FromBgra(107, 183, 189, 255);
            }
        }

        public static StyleColor DarkMagenta
        {
            get
            {
                return StyleColor.FromBgra(139, 0, 139, 255);
            }
        }

        public static StyleColor DarkOliveGreen
        {
            get
            {
                return StyleColor.FromBgra(47, 107, 85, 255);
            }
        }

        public static StyleColor DarkOrange
        {
            get
            {
                return StyleColor.FromBgra(0, 140, 255, 255);
            }
        }

        public static StyleColor DarkOrchid
        {
            get
            {
                return StyleColor.FromBgra(204, 50, 153, 255);
            }
        }

        public static StyleColor DarkRed
        {
            get
            {
                return StyleColor.FromBgra(0, 0, 139, 255);
            }
        }

        public static StyleColor DarkSalmon
        {
            get
            {
                return StyleColor.FromBgra(122, 150, 233, 255);
            }
        }

        public static StyleColor DarkSeaGreen
        {
            get
            {
                return StyleColor.FromBgra(139, 188, 143, 255);
            }
        }

        public static StyleColor DarkSlateBlue
        {
            get
            {
                return StyleColor.FromBgra(139, 61, 72, 255);
            }
        }

        public static StyleColor DarkSlateGray
        {
            get
            {
                return StyleColor.FromBgra(79, 79, 47, 255);
            }
        }

        public static StyleColor DarkTurquoise
        {
            get
            {
                return StyleColor.FromBgra(209, 206, 0, 255);
            }
        }

        public static StyleColor DarkViolet
        {
            get
            {
                return StyleColor.FromBgra(211, 0, 148, 255);
            }
        }

        public static StyleColor DeepPink
        {
            get
            {
                return StyleColor.FromBgra(147, 20, 255, 255);
            }
        }

        public static StyleColor DeepSkyBlue
        {
            get
            {
                return StyleColor.FromBgra(255, 191, 0, 255);
            }
        }

        public static StyleColor DimGray
        {
            get
            {
                return StyleColor.FromBgra(105, 105, 105, 255);
            }
        }

        public static StyleColor DodgerBlue
        {
            get
            {
                return StyleColor.FromBgra(255, 144, 30, 255);
            }
        }

        public static StyleColor Firebrick
        {
            get
            {
                return StyleColor.FromBgra(34, 34, 178, 255);
            }
        }

        public static StyleColor FloralWhite
        {
            get
            {
                return StyleColor.FromBgra(240, 250, 255, 255);
            }
        }

        public static StyleColor ForestGreen
        {
            get
            {
                return StyleColor.FromBgra(34, 139, 34, 255);
            }
        }

        public static StyleColor Fuchsia
        {
            get
            {
                return StyleColor.FromBgra(255, 0, 255, 255);
            }
        }

        public static StyleColor Gainsboro
        {
            get
            {
                return StyleColor.FromBgra(220, 220, 220, 255);
            }
        }

        public static StyleColor GhostWhite
        {
            get
            {
                return StyleColor.FromBgra(255, 248, 248, 255);
            }
        }

        public static StyleColor Gold
        {
            get
            {
                return StyleColor.FromBgra(0, 215, 255, 255);
            }
        }

        public static StyleColor Goldenrod
        {
            get
            {
                return StyleColor.FromBgra(32, 165, 218, 255);
            }
        }

        public static StyleColor Gray
        {
            get
            {
                return StyleColor.FromBgra(128, 128, 128, 255);
            }
        }

        public static StyleColor Green
        {
            get
            {
                return StyleColor.FromBgra(0, 128, 0, 255);
            }
        }

        public static StyleColor GreenYellow
        {
            get
            {
                return StyleColor.FromBgra(47, 255, 173, 255);
            }
        }

        public static StyleColor Honeydew
        {
            get
            {
                return StyleColor.FromBgra(240, 255, 240, 255);
            }
        }

        public static StyleColor HotPink
        {
            get
            {
                return StyleColor.FromBgra(180, 105, 255, 255);
            }
        }

        public static StyleColor IndianRed
        {
            get
            {
                return StyleColor.FromBgra(92, 92, 205, 255);
            }
        }

        public static StyleColor Indigo
        {
            get
            {
                return StyleColor.FromBgra(130, 0, 75, 255);
            }
        }

        public static StyleColor Ivory
        {
            get
            {
                return StyleColor.FromBgra(240, 255, 255, 255);
            }
        }

        public static StyleColor Khaki
        {
            get
            {
                return StyleColor.FromBgra(140, 230, 240, 255);
            }
        }

        public static StyleColor Lavender
        {
            get
            {
                return StyleColor.FromBgra(250, 230, 230, 255);
            }
        }

        public static StyleColor LavenderBlush
        {
            get
            {
                return StyleColor.FromBgra(245, 240, 255, 255);
            }
        }

        public static StyleColor LawnGreen
        {
            get
            {
                return StyleColor.FromBgra(0, 252, 124, 255);
            }
        }

        public static StyleColor LemonChiffon
        {
            get
            {
                return StyleColor.FromBgra(205, 250, 255, 255);
            }
        }

        public static StyleColor LightBlue
        {
            get
            {
                return StyleColor.FromBgra(230, 216, 173, 255);
            }
        }

        public static StyleColor LightCoral
        {
            get
            {
                return StyleColor.FromBgra(128, 128, 240, 255);
            }
        }

        public static StyleColor LightCyan
        {
            get
            {
                return StyleColor.FromBgra(255, 255, 224, 255);
            }
        }

        public static StyleColor LightGoldenrodYellow
        {
            get
            {
                return StyleColor.FromBgra(210, 250, 250, 255);
            }
        }

        public static StyleColor LightGreen
        {
            get
            {
                return StyleColor.FromBgra(144, 238, 144, 255);
            }
        }

        public static StyleColor LightGray
        {
            get
            {
                return StyleColor.FromBgra(211, 211, 211, 255);
            }
        }

        public static StyleColor LightPink
        {
            get
            {
                return StyleColor.FromBgra(193, 182, 255, 255);
            }
        }

        public static StyleColor LightSalmon
        {
            get
            {
                return StyleColor.FromBgra(122, 160, 255, 255);
            }
        }

        public static StyleColor LightSeaGreen
        {
            get
            {
                return StyleColor.FromBgra(170, 178, 32, 255);
            }
        }

        public static StyleColor LightSkyBlue
        {
            get
            {
                return StyleColor.FromBgra(250, 206, 135, 255);
            }
        }

        public static StyleColor LightSlateGray
        {
            get
            {
                return StyleColor.FromBgra(153, 136, 119, 255);
            }
        }

        public static StyleColor LightSteelBlue
        {
            get
            {
                return StyleColor.FromBgra(222, 196, 176, 255);
            }
        }

        public static StyleColor LightYellow
        {
            get
            {
                return StyleColor.FromBgra(224, 255, 255, 255);
            }
        }

        public static StyleColor Lime
        {
            get
            {
                return StyleColor.FromBgra(0, 255, 0, 255);
            }
        }

        public static StyleColor LimeGreen
        {
            get
            {
                return StyleColor.FromBgra(50, 205, 50, 255);
            }
        }

        public static StyleColor Linen
        {
            get
            {
                return StyleColor.FromBgra(230, 240, 250, 255);
            }
        }

        public static StyleColor Magenta
        {
            get
            {
                return StyleColor.FromBgra(255, 0, 255, 255);
            }
        }

        public static StyleColor Maroon
        {
            get
            {
                return StyleColor.FromBgra(0, 0, 128, 255);
            }
        }

        public static StyleColor MediumAquamarine
        {
            get
            {
                return StyleColor.FromBgra(170, 205, 102, 255);
            }
        }

        public static StyleColor MediumBlue
        {
            get
            {
                return StyleColor.FromBgra(205, 0, 0, 255);
            }
        }

        public static StyleColor MediumOrchid
        {
            get
            {
                return StyleColor.FromBgra(211, 85, 186, 255);
            }
        }

        public static StyleColor MediumPurple
        {
            get
            {
                return StyleColor.FromBgra(219, 112, 147, 255);
            }
        }

        public static StyleColor MediumSeaGreen
        {
            get
            {
                return StyleColor.FromBgra(113, 179, 60, 255);
            }
        }

        public static StyleColor MediumSlateBlue
        {
            get
            {
                return StyleColor.FromBgra(238, 104, 123, 255);
            }
        }

        public static StyleColor MediumSpringGreen
        {
            get
            {
                return StyleColor.FromBgra(154, 250, 0, 255);
            }
        }

        public static StyleColor MediumTurquoise
        {
            get
            {
                return StyleColor.FromBgra(204, 209, 72, 255);
            }
        }

        public static StyleColor MediumVioletRed
        {
            get
            {
                return StyleColor.FromBgra(133, 21, 199, 255);
            }
        }

        public static StyleColor MidnightBlue
        {
            get
            {
                return StyleColor.FromBgra(112, 25, 25, 255);
            }
        }

        public static StyleColor MintCream
        {
            get
            {
                return StyleColor.FromBgra(250, 255, 245, 255);
            }
        }

        public static StyleColor MistyRose
        {
            get
            {
                return StyleColor.FromBgra(225, 228, 255, 255);
            }
        }

        public static StyleColor Moccasin
        {
            get
            {
                return StyleColor.FromBgra(181, 228, 255, 255);
            }
        }

        public static StyleColor NavajoWhite
        {
            get
            {
                return StyleColor.FromBgra(173, 222, 255, 255);
            }
        }

        public static StyleColor Navy
        {
            get
            {
                return StyleColor.FromBgra(128, 0, 0, 255);
            }
        }

        public static StyleColor OldLace
        {
            get
            {
                return StyleColor.FromBgra(230, 245, 253, 255);
            }
        }

        public static StyleColor Olive
        {
            get
            {
                return StyleColor.FromBgra(0, 128, 128, 255);
            }
        }

        public static StyleColor OliveDrab
        {
            get
            {
                return StyleColor.FromBgra(35, 142, 107, 255);
            }
        }

        public static StyleColor Orange
        {
            get
            {
                return StyleColor.FromBgra(0, 165, 255, 255);
            }
        }

        public static StyleColor OrangeRed
        {
            get
            {
                return StyleColor.FromBgra(0, 69, 255, 255);
            }
        }

        public static StyleColor Orchid
        {
            get
            {
                return StyleColor.FromBgra(214, 112, 218, 255);
            }
        }

        public static StyleColor PaleGoldenrod
        {
            get
            {
                return StyleColor.FromBgra(170, 232, 238, 255);
            }
        }

        public static StyleColor PaleGreen
        {
            get
            {
                return StyleColor.FromBgra(152, 251, 152, 255);
            }
        }

        public static StyleColor PaleTurquoise
        {
            get
            {
                return StyleColor.FromBgra(238, 238, 175, 255);
            }
        }

        public static StyleColor PaleVioletRed
        {
            get
            {
                return StyleColor.FromBgra(147, 112, 219, 255);
            }
        }

        public static StyleColor PapayaWhip
        {
            get
            {
                return StyleColor.FromBgra(213, 239, 255, 255);
            }
        }

        public static StyleColor PeachPuff
        {
            get
            {
                return StyleColor.FromBgra(185, 218, 255, 255);
            }
        }

        public static StyleColor Peru
        {
            get
            {
                return StyleColor.FromBgra(63, 133, 205, 255);
            }
        }

        public static StyleColor Pink
        {
            get
            {
                return StyleColor.FromBgra(203, 192, 255, 255);
            }
        }

        public static StyleColor Plum
        {
            get
            {
                return StyleColor.FromBgra(221, 160, 221, 255);
            }
        }

        public static StyleColor PowderBlue
        {
            get
            {
                return StyleColor.FromBgra(230, 224, 176, 255);
            }
        }

        public static StyleColor Purple
        {
            get
            {
                return StyleColor.FromBgra(128, 0, 128, 255);
            }
        }

        public static StyleColor Red
        {
            get
            {
                return StyleColor.FromBgra(0, 0, 255, 255);
            }
        }

        public static StyleColor RosyBrown
        {
            get
            {
                return StyleColor.FromBgra(143, 143, 188, 255);
            }
        }

        public static StyleColor RoyalBlue
        {
            get
            {
                return StyleColor.FromBgra(225, 105, 65, 255);
            }
        }

        public static StyleColor SaddleBrown
        {
            get
            {
                return StyleColor.FromBgra(19, 69, 139, 255);
            }
        }

        public static StyleColor Salmon
        {
            get
            {
                return StyleColor.FromBgra(114, 128, 250, 255);
            }
        }

        public static StyleColor SandyBrown
        {
            get
            {
                return StyleColor.FromBgra(96, 164, 244, 255);
            }
        }

        public static StyleColor SeaGreen
        {
            get
            {
                return StyleColor.FromBgra(87, 139, 46, 255);
            }
        }

        public static StyleColor SeaShell
        {
            get
            {
                return StyleColor.FromBgra(238, 245, 255, 255);
            }
        }

        public static StyleColor Sienna
        {
            get
            {
                return StyleColor.FromBgra(45, 82, 160, 255);
            }
        }

        public static StyleColor Silver
        {
            get
            {
                return StyleColor.FromBgra(192, 192, 192, 255);
            }
        }

        public static StyleColor SkyBlue
        {
            get
            {
                return StyleColor.FromBgra(235, 206, 135, 255);
            }
        }

        public static StyleColor SlateBlue
        {
            get
            {
                return StyleColor.FromBgra(205, 90, 106, 255);
            }
        }

        public static StyleColor SlateGray
        {
            get
            {
                return StyleColor.FromBgra(144, 128, 112, 255);
            }
        }

        public static StyleColor Snow
        {
            get
            {
                return StyleColor.FromBgra(250, 250, 255, 255);
            }
        }

        public static StyleColor SpringGreen
        {
            get
            {
                return StyleColor.FromBgra(127, 255, 0, 255);
            }
        }

        public static StyleColor SteelBlue
        {
            get
            {
                return StyleColor.FromBgra(180, 130, 70, 255);
            }
        }

        public static StyleColor Tan
        {
            get
            {
                return StyleColor.FromBgra(140, 180, 210, 255);
            }
        }

        public static StyleColor Teal
        {
            get
            {
                return StyleColor.FromBgra(128, 128, 0, 255);
            }
        }

        public static StyleColor Thistle
        {
            get
            {
                return StyleColor.FromBgra(216, 191, 216, 255);
            }
        }

        public static StyleColor Tomato
        {
            get
            {
                return StyleColor.FromBgra(71, 99, 255, 255);
            }
        }

        public static StyleColor Turquoise
        {
            get
            {
                return StyleColor.FromBgra(208, 224, 64, 255);
            }
        }

        public static StyleColor Violet
        {
            get
            {
                return StyleColor.FromBgra(238, 130, 238, 255);
            }
        }

        public static StyleColor Wheat
        {
            get
            {
                return StyleColor.FromBgra(179, 222, 245, 255);
            }
        }

        public static StyleColor White
        {
            get
            {
                return StyleColor.FromBgra(255, 255, 255, 255);
            }
        }

        public static StyleColor WhiteSmoke
        {
            get
            {
                return StyleColor.FromBgra(245, 245, 245, 255);
            }
        }

        public static StyleColor Yellow
        {
            get
            {
                return StyleColor.FromBgra(0, 255, 255, 255);
            }
        }

        public static StyleColor YellowGreen
        {
            get
            {
                return StyleColor.FromBgra(50, 205, 154, 255);
            }
        }

        public static StyleColor Zero
        {
            get
            {
                return (StyleColor)0;
            }
        }

        private static Dictionary<string, StyleColor> _predefinedColors;

        /// <summary>
        /// Gets a <see cref="Dictionary{string, Color}"/> that contains replicas of those 
        /// predefined in System.Drawing.Color.
        /// </summary>
        public static Dictionary<string, StyleColor> PredefinedColors
        {
            get
            {
                if (_predefinedColors != null)
                {
                    Type colorType = typeof(StyleColor);
                    PropertyInfo[] propInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.Public);
                    Dictionary<string, StyleColor> colors = new Dictionary<string, StyleColor>();

                    foreach (PropertyInfo pi in propInfos)
                        if (pi.PropertyType == colorType)
                            colors.Add(pi.Name, (StyleColor)pi.GetValue(null, null));
                }

                return new Dictionary<string, StyleColor>(_predefinedColors);
            }
        }
    }
}
