using System;

namespace SharpMap.Data.Providers.GdalPreview
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseRasterShaderFunction
    {
        public Double Minimum { get; set; }
        public Double Maximum { get; set; }

        public abstract void Initialize();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inValue"></param>
        /// <param name="outRed"></param>
        /// <param name="outGreen"></param>
        /// <param name="outBlue"></param>
        public abstract Boolean Shade(double inValue, ref int outRed, ref int outGreen, ref int outBlue);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inRed"></param>
        /// <param name="inGreen"></param>
        /// <param name="inBlue"></param>
        /// <param name="outRed"></param>
        /// <param name="outGreen"></param>
        /// <param name="outBlue"></param>
        public abstract Boolean Shade(double inRed, double inGreen, double inBlue, ref int outRed, ref int outGreen, ref int outBlue);
    }
}
