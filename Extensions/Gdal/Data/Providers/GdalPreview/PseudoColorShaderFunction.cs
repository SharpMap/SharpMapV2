using System;

namespace SharpMap.Data.Providers.GdalPreview
{
    public class PseudoColorShaderFunction : BaseRasterShaderFunction
    {
        private readonly double[] _breakpoints = new double[2];
        private double _interval;
        private double _two55ByRange;

        #region BaseRasterShaderFunction Member

        public override void Initialize()
        {
            _interval = (Maximum - Minimum) / 3d;
            _breakpoints[0] = Minimum + _interval;
            _breakpoints[1] = _breakpoints[0] + _interval;
            _two55ByRange = 255d / (Maximum - Minimum);
        }

        public override bool Shade(double inValue, ref int outRed, ref int outGreen, ref int outBlue)
        {
            if (inValue < Minimum) inValue = Minimum;
            if (inValue > Maximum) inValue = Maximum;

            if ( inValue < _breakpoints[0])
            {
                outRed = 0;
                outGreen = (Int32)((_two55ByRange * (inValue - Minimum)) * 3d);
                outBlue = 255;
                return true;
            }

            if (inValue < _breakpoints[1])
            {
                //Double inner = 
                outRed = (Int32)((_two55ByRange * (inValue - _breakpoints[0])) * 3d);
                outGreen = 255;
                outBlue = 255 - outRed;
                return true;
            }

            outRed = 255;
            outGreen = (Int32)(255 - ((_two55ByRange * (inValue - _breakpoints[1])) * 3d));
            outBlue = 0;
            return true;
        }

        public override bool Shade(double inRed, double inGreen, double inBlue, ref int outRed, ref int outGreen, ref int outBlue)
        {
            return false;
        }

        #endregion
    }
}
