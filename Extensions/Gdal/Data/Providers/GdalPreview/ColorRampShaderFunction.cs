using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpMap.Styles;
using GdalBand = OSGeo.GDAL.Band;
using GdalColorEntry = OSGeo.GDAL.ColorEntry;
using GdalColorTable = OSGeo.GDAL.ColorTable;
using GdalPaletteInterp = OSGeo.GDAL.PaletteInterp;

namespace SharpMap.Data.Providers.GdalPreview
{
    public enum ColorRampShaderMethod
    {
        Interpolated,
        Discrete,
        Exact
    }

    public class ColorRampShaderFunction : BaseRasterShaderFunction
    {
        public static ColorRampShaderFunction CreateFromGdalColorTable(GdalColorTable colorTable)
        {
            if (colorTable == null)
                throw new ArgumentNullException("colorTable");

            Int32 numColorEntries = colorTable.GetCount();
            GdalPaletteInterp paletteInterp = colorTable.GetPaletteInterpretation();
            ColorRampShaderFunction shader = new ColorRampShaderFunction {Method = ColorRampShaderMethod.Interpolated};
            List<ColorRampItem> colorRamp = new List<ColorRampItem>(numColorEntries);
            for (int i = 0; i < numColorEntries; i++ )
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
                                            Label = string.Format("Color{0}",i),
                                            Value = i
                                        };
                colorRamp.Add(cri);
            }
            shader.ColorRamp = colorRamp;
            return shader;
        }
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
        public struct ColorRampItem : IComparable<ColorRampItem>
        {
            public String Label;
            public Double Value;
            public StyleColor Color;

            #region Implementation of IComparable<ColorRampItem>

            public int CompareTo(ColorRampItem other)
            {
                return Value.CompareTo(other.Value);
            }

            #endregion
        }
        #endregion


        private const Double DifferenceThreshold = 0.0000001d;
        private const Int32 ColorCacheSize = 1024;

        private ColorRampShaderMethod _method;
        private List<ColorRampItem> _colorRampItemList;
        private Int32 _currentColorRampItemIndex;

        private readonly Dictionary<Double, StyleColor> _colorCache =
            new Dictionary<double, StyleColor>(ColorCacheSize);

        public ColorRampShaderMethod Method
        {
            get { return _method; }
            set
            {
                _colorCache.Clear();
                _method = value;
            }
        }

        public List<ColorRampItem> ColorRamp
        {
            get { return _colorRampItemList; }
            set { _colorRampItemList = value; }
        }

        #region Implementation of BaseRasterShaderFunction

        public override void Initialize()
        {
            //nothing to do atm
            return;
        }
        public override bool Shade(double inValue, ref int outRed, ref int outGreen, ref int outBlue)
        {
            StyleColor color;
            if(_colorCache.TryGetValue(inValue, out color))
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

        public override bool Shade(double inRed, double inGreen, double inBlue, 
            ref int outRed, ref int outGreen, ref int outBlue)
        {
            outRed = 0;
            outGreen = 0;
            outBlue = 0;
            return false;
        }

        #endregion

        private Boolean DiscreteColor( double value, ref int outRed, ref int outGreen, ref int  outBlue)
        {
            Int32 colorRampItemCount = _colorRampItemList.Count;
            if ( colorRampItemCount <= 0 )
                return false;

            ColorRampItem colorRampItem;
            while(_currentColorRampItemIndex >= 0 && 
                  _currentColorRampItemIndex < colorRampItemCount)
            {
                colorRampItem = _colorRampItemList[_currentColorRampItemIndex];
                Double tinyDiff = Math.Abs(value - colorRampItem.Value);
                if ( _currentColorRampItemIndex != 0 && value <= _colorRampItemList[_currentColorRampItemIndex - 1].Value)
                {
                    _currentColorRampItemIndex--;
                }
                else if( value <= colorRampItem.Value || tinyDiff <= DifferenceThreshold)
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

        private Boolean ExactColor( double value, ref int outRed, ref int outGreen, ref int  outBlue)
        {
            int colorRampItemCount = _colorRampItemList.Count;
            if (colorRampItemCount <= 0)
            {
                return false;
            }

            ColorRampItem colorRampItem;
            while (_currentColorRampItemIndex >= 0 && _currentColorRampItemIndex < colorRampItemCount)
            {
                //Start searching from the last index - assumtion is that neighboring pixels tend to be similar values
                colorRampItem = _colorRampItemList[_currentColorRampItemIndex];
                double tinyDiff = Math.Abs(value - colorRampItem.Value);
                if (value == colorRampItem.Value || tinyDiff <= DifferenceThreshold)
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
            Int32 colorRampItemCount = _colorRampItemList.Count;
            if (colorRampItemCount <= 0)
            {
                return false;
            }

            ColorRampItem colorRampItem;
            while (_currentColorRampItemIndex >= 0 && _currentColorRampItemIndex < colorRampItemCount)
            {
                //Start searching from the last index - assumtion is that neighboring pixels tend to be similar values
                colorRampItem = _colorRampItemList[_currentColorRampItemIndex];
                Double tinyDiff = Math.Abs(value - colorRampItem.Value);
                //If the previous entry is less, then search closer to the top of the list (assumes mColorRampItemList is sorted)
                if (_currentColorRampItemIndex != 0 && value <= _colorRampItemList[_currentColorRampItemIndex - 1].Value)
                {
                    _currentColorRampItemIndex--;
                }
                else if (_currentColorRampItemIndex != 0 &&
                         (value <= colorRampItem.Value || tinyDiff <= DifferenceThreshold))
                {
                    ColorRampItem previousColorRampItem = _colorRampItemList[_currentColorRampItemIndex - 1];
                    Double currentRampRange = colorRampItem.Value - previousColorRampItem.Value; //difference between two consecutive entry values
                    Double offsetInRange = value - previousColorRampItem.Value; //difference between the previous entry value and value

                    outRed = (int) (previousColorRampItem.Color.R +
                                    (( (colorRampItem.Color.R - previousColorRampItem.Color.R)/
                                      currentRampRange)*offsetInRange));
                    outGreen = (int) ( previousColorRampItem.Color.G +
                                      (( (colorRampItem.Color.G - previousColorRampItem.Color.G)/
                                        currentRampRange)*offsetInRange));
                    outBlue = (int) ( previousColorRampItem.Color.B +
                                     (( (colorRampItem.Color.G - previousColorRampItem.Color.B)/
                                       currentRampRange)*offsetInRange));
                    if (ColorCacheSize >= _colorCache.Count)
                    {
                        StyleColor newColor = new StyleColor(outBlue, outGreen, outRed, 0);
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
                        StyleColor newColor = new StyleColor(outBlue, outGreen, outRed, 0);
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
