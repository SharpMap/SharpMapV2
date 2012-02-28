namespace SharpMap.Styles
{
    /// <summary>
    /// Raster visualization methods
    /// </summary>
    public enum RasterVisualizationMethod
    {
        /// <summary>
        /// Visualize a single raster band using a grayscale
        /// </summary>
        SingleBandGray,

        /// <summary>
        /// Visualize a single raster band using a palette
        /// </summary>
        SingleBandColorPalette,

        /// <summary>
        /// Visualize a single raster band using a shader function
        /// </summary>
        SingleBandShader,

        /// <summary>
        /// Visualize a multi bands of a raster by converting RGB to colors.
        /// </summary>
        MultiBandColor,

        /// <summary>
        /// Visualize a multi bands of a raster by converting RGB to colors and applying a color matrix.
        /// </summary>
        MultiBandColorColorMatrix,

        /// <summary>
        /// specifies a custom visualization method
        /// </summary>
        Custom,
    }
}