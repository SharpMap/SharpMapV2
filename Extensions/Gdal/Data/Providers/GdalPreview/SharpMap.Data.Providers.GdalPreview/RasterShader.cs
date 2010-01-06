using System;

namespace SharpMap.Data.Providers.GdalPreview
{
    public class RasterShader
    {
        private Double _minimum;
        private Double _maximum;

        private BaseRasterShaderFunction _shaderFunction;

        public Double Minimum
        {
            get { return _minimum; }
            set
            { 
                _minimum = value;
                if (_shaderFunction != null)
                {
                    _shaderFunction.Minimum = value;
                    _shaderFunction.Initialize();
                }
            }
        }

        public Double Maximum
        {
            get { return _maximum; }
            set
            {
                _maximum = value;
                if (_shaderFunction != null)
                {
                    _shaderFunction.Maximum = value;
                    _shaderFunction.Initialize();
                }
            }
        }

        public BaseRasterShaderFunction RasterShaderFunction
        {
            get { return _shaderFunction; }
            set { _shaderFunction = value; }
        }

        public Boolean Shade(double inValue, ref int outRed, ref int outGreen, ref int outBlue)
        {
            if (_shaderFunction != null)
                return _shaderFunction.Shade(inValue, ref outRed, ref outGreen, ref outBlue);
            return false;
        }

        Boolean Shade(double inRed, double inGreen, double inBlue, ref int outRed, ref int outGreen, ref int outBlue)
        {
            if (_shaderFunction != null)
                return _shaderFunction.Shade(inRed, inBlue, inGreen, ref outRed, ref outBlue, ref outGreen);
            return false;
        }
    }
}
