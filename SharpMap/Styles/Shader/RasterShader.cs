using System;

namespace SharpMap.Styles.Shader
{
    /// <summary>
    /// Base raster shader class
    /// </summary>
    [Serializable]
    public abstract class RasterShader
    {
        /// <summary>
        /// Gets or sets the minimum value
        /// </summary>
        public Double Minimum { get; set; }

        /// <summary>
        /// Gets or sets the maximum value
        /// </summary>
        public Double Maximum { get; set; }

        /// <summary>
        /// Initialization function
        /// </summary>
        public virtual void Initialize()
        { }

        /// <summary>
        /// Function to shade <paramref name="inValue"/> to the
        /// </summary>
        /// <param name="inValue">The value to shade</param>
        /// <param name="outRed">The red pixel value</param>
        /// <param name="outGreen">The green pixel value</param>
        /// <param name="outBlue">The blue pixel value</param>
        /// <returns><c>true</c> if shading was successful</returns>
        public abstract Boolean Shade(double inValue, ref int outRed, ref int outGreen, ref int outBlue);
    }
}