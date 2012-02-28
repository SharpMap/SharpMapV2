using System;
using System.Collections.Generic;

namespace SharpMap.Styles.Shader
{
    /// <summary>
    /// Color ramp shader methods
    /// </summary>
    public enum ColorRampShaderMethod
    {
        /// <summary>
        /// Interpolation of the color value
        /// </summary>
        Interpolated,
        /// <summary>
        /// Discrete color value
        /// </summary>
        Discrete,
        /// <summary>
        /// Exact color value
        /// </summary>
        Exact
    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class ColorRampShader : RasterShader
    {
        /*
        public static ColorRampShader CreateFromGdalColorTable(GdalColorTable colorTable)
        {
            if (colorTable == null)
                throw new ArgumentNullException("colorTable");

            Int32 numColorEntries = colorTable.GetCount();
            GdalPaletteInterp paletteInterp = colorTable.GetPaletteInterpretation();
            ColorRampShaderFunction shader = new ColorRampShaderFunction { Method = ColorRampShaderMethod.Interpolated };
            List<ColorRampItem> colorRamp = new List<ColorRampItem>(numColorEntries);
            for (int i = 0; i < numColorEntries; i++)
            {
                GdalColorEntry ci = colorTable.GetColorEntry(i);
                StyleColor color;
                switch (paletteInterp)
                {
                    case GdalPaletteInterp.GPI_Gray:
                        color = StyleColor.FromBgra(ci.c1, ci.c1, ci.c1, 255);
                        break;

                    case GdalPaletteInterp.GPI_CMYK:
                        color = StyleColor.FromCmyk(ci.c1, ci.c2, ci.c3, ci.c4);
                        break;

                    case GdalPaletteInterp.GPI_HLS:
                        color = StyleColor.FromHls(ci.c1, ci.c2, ci.c3, ci.c4);
                        break;

                    default:
                        color = StyleColor.FromBgra(ci.c3, ci.c2, ci.c1, ci.c4);
                        break;
                }

                ColorRampItem cri = new ColorRampItem
                {
                    Color = color,
                    Label = string.Format("Color{0}", i),
                    Value = i
                };
                colorRamp.Add(cri);
            }
            shader.ColorRamp = colorRamp;
            return shader;
        }
         */
        /*
        private static Double ScaleColorComponent(Int16 inVal, Int16 inMin, Int32 inMax, Double minVal, Double maxVal)
        {
            Debug.Assert(inMin <= inVal && inVal <= inMax);
            Debug.Assert(minVal < maxVal);

            Double step = ((Double) inMax - inMin)/(maxVal - minVal);
            return minVal + step*((Double) inVal - (Double) inMin);
        }
        */

        #region nested types

        /// <summary>
        /// Color ramp item
        /// </summary>
        [Serializable]
        public struct ColorRampItem : IComparable<ColorRampItem>
        {
            /// <summary>
            /// Label for this item
            /// </summary>
            public String Label;

            /// <summary>
            /// The value
            /// </summary>
            public Double Value;

            /// <summary>
            /// The assigned Color
            /// </summary>
            public StyleColor Color;

            #region Implementation of IComparable<ColorRampItem>

            public int CompareTo(ColorRampItem other)
            {
                return Value.CompareTo(other.Value);
            }

            #endregion Implementation of IComparable<ColorRampItem>
        }

        #endregion nested types

        private const Double DifferenceThreshold = 0.0000001d;
        private const Int32 ColorCacheSize = 1024;

        private ColorRampShaderMethod _method;
        private List<ColorRampItem> _colorRampItemList;
        private Int32 _currentColorRampItemIndex;

        private readonly Dictionary<Double, StyleColor> _colorCache =
            new Dictionary<double, StyleColor>(ColorCacheSize);

        /// <summary>
        /// Gets or sets the color ramp shader method
        /// </summary>
        public ColorRampShaderMethod Method
        {
            get { return _method; }
            set
            {
                _colorCache.Clear();
                _method = value;
            }
        }

        /// <summary>
        /// Gets or sets the color ramp
        /// </summary>
        public List<ColorRampItem> ColorRamp
        {
            get { return _colorRampItemList; }
            set { _colorRampItemList = value; }
        }

        #region Implementation of BaseRasterShaderFunction

        public override void Initialize()
        {
            _colorRampItemList.Sort();
        }

        public override bool Shade(double inValue, ref int outRed, ref int outGreen, ref int outBlue)
        {
            StyleColor color;
            if (_colorCache.TryGetValue(inValue, out color))
            {
                outRed = color.R;
                outGreen = color.G;
                outBlue = color.B;
                return true;
            }

            if (_currentColorRampItemIndex < 0)
                _currentColorRampItemIndex = 0;
            else if (_currentColorRampItemIndex >= _colorRampItemList.Count)
                _currentColorRampItemIndex = _colorRampItemList.Count - 1;

            switch (Method)
            {
                case ColorRampShaderMethod.Exact:
                    return ExactColor(inValue, ref outRed, ref outGreen, ref outBlue);
                case ColorRampShaderMethod.Discrete:
                    return DiscreteColor(inValue, ref outRed, ref outGreen, ref outBlue);
                case ColorRampShaderMethod.Interpolated:
                    return InterpolatedColor(inValue, ref outRed, ref outGreen, ref outBlue);
            }
            throw new Exception("Should never reach here!");
        }

        /*
        public override bool Shade(double inRed, double inGreen, double inBlue,
            ref int outRed, ref int outGreen, ref int outBlue)
        {
            outRed = 0;
            outGreen = 0;
            outBlue = 0;
            return false;
        }
        */

        #endregion Implementation of BaseRasterShaderFunction

        private Boolean DiscreteColor(double value, ref int outRed, ref int outGreen, ref int outBlue)
        {
            var colorRampItemCount = _colorRampItemList.Count;
            if (colorRampItemCount <= 0)
                return false;

            while (_currentColorRampItemIndex >= 0 &&
                  _currentColorRampItemIndex < colorRampItemCount)
            {
                var colorRampItem = _colorRampItemList[_currentColorRampItemIndex];
                var tinyDiff = Math.Abs(value - colorRampItem.Value);
                if (_currentColorRampItemIndex != 0 && value <= _colorRampItemList[_currentColorRampItemIndex - 1].Value)
                {
                    _currentColorRampItemIndex--;
                }
                else if (value <= colorRampItem.Value || tinyDiff <= DifferenceThreshold)
                {
                    outRed = colorRampItem.Color.R;
                    outGreen = colorRampItem.Color.G;
                    outBlue = colorRampItem.Color.B;
                    if (ColorCacheSize > _colorCache.Count)
                        _colorCache.Add(value, colorRampItem.Color);
                    return true;
                }
                else
                {
                    _currentColorRampItemIndex++;
                }
            }
            return true;
        }

        private Boolean ExactColor(double value, ref int outRed, ref int outGreen, ref int outBlue)
        {
            var colorRampItemCount = _colorRampItemList.Count;
            if (colorRampItemCount <= 0)
            {
                return false;
            }

            while (_currentColorRampItemIndex >= 0 && _currentColorRampItemIndex < colorRampItemCount)
            {
                //Start searching from the last index - assumtion is that neighboring pixels tend to be similar values
                var colorRampItem = _colorRampItemList[_currentColorRampItemIndex];
                var tinyDiff = Math.Abs(value - colorRampItem.Value);
                if (tinyDiff <= DifferenceThreshold)
                {
                    outRed = colorRampItem.Color.R;
                    outGreen = colorRampItem.Color.G;
                    outBlue = colorRampItem.Color.B;
                    //Cache the shaded value
                    if (ColorCacheSize >= _colorCache.Count)
                    {
                        _colorCache.Add(value, colorRampItem.Color);
                    }
                    return true;
                }
                //pixel value sits between ramp entries so bail
                if (_currentColorRampItemIndex != colorRampItemCount - 1 &&
                    value > colorRampItem.Value &&
                    value < _colorRampItemList[_currentColorRampItemIndex + 1].Value)
                {
                    return false;
                }
                //Search deeper into the color ramp list
                if (value > colorRampItem.Value)
                {
                    _currentColorRampItemIndex++;
                }
                //Search back toward the begining of the list
                else
                {
                    _currentColorRampItemIndex--;
                }
            }

            return false; // value not found
        }

        private Boolean InterpolatedColor(double value, ref int outRed, ref int outGreen, ref int outBlue)
        {
            var colorRampItemCount = _colorRampItemList.Count;
            if (colorRampItemCount <= 0)
            {
                return false;
            }

            while (_currentColorRampItemIndex >= 0 && _currentColorRampItemIndex < colorRampItemCount)
            {
                //Start searching from the last index - assumtion is that neighboring pixels tend to be similar values
                var colorRampItem = _colorRampItemList[_currentColorRampItemIndex];
                var tinyDiff = Math.Abs(value - colorRampItem.Value);

                //If the previous entry is less, then search closer to the top of the list (assumes _colorRampItemList is sorted)
                if (_currentColorRampItemIndex != 0 && value <= _colorRampItemList[_currentColorRampItemIndex - 1].Value)
                {
                    _currentColorRampItemIndex--;
                }
                else if (_currentColorRampItemIndex != 0 &&
                         (value <= colorRampItem.Value || tinyDiff <= DifferenceThreshold))
                {
                    var previousColorRampItem = _colorRampItemList[_currentColorRampItemIndex - 1];
                    var currentRampRange = colorRampItem.Value - previousColorRampItem.Value; //difference between two consecutive entry values
                    var offsetInRange = value - previousColorRampItem.Value; //difference between the previous entry value and value

                    var prev = previousColorRampItem.Color;
                    var curr = colorRampItem.Color;

                    outRed = (int)(prev.R + (((curr.R - prev.R) / currentRampRange) * offsetInRange));
                    outGreen = (int)(prev.G + (((curr.G - prev.G) / currentRampRange) * offsetInRange));
                    outBlue = (int)(prev.B + (((curr.G - prev.B) / currentRampRange) * offsetInRange));
                    if (ColorCacheSize >= _colorCache.Count)
                    {
                        var newColor = new StyleColor(outBlue, outGreen, outRed, 0);
                        _colorCache.Add(value, newColor);
                    }
                    return true;
                }
                else if (_currentColorRampItemIndex == 0 && value <= colorRampItem.Value)
                {
                    //ColorRampItem myPreviousColorRampItem = _colorRampItemList[_currentColorRampItemIndex - 1];
                    //currentRampRange = colorRampItem.Value - myPreviousColorRampItem.Value;
                    //offsetInRange = value - myPreviousColorRampItem.Value;

                    outRed = colorRampItem.Color.R;
                    outGreen = colorRampItem.Color.G;
                    outBlue = colorRampItem.Color.B;
                    if (ColorCacheSize >= _colorCache.Count)
                    {
                        var newColor = new StyleColor(outBlue, outGreen, outRed, 0);
                        _colorCache.Add(value, newColor);
                    }
                    return true;
                }
                //Search deeper into the color ramp list
                else if (value > colorRampItem.Value)
                {
                    _currentColorRampItemIndex++;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }
    }
}