using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    /// <summary>
    /// Interface for symbolizer rules that can symbolize geometries
    /// </summary>
    public interface IGeometrySymbolizerRule
        : ISymbolizerRule
    {
        /// <summary>
        /// Function to symbolize the provided <paramref name="record"/> during the provided <paramref name="renderPhase"/> using the
        /// provided <paramref name="transform"/> and <paramref name="rasterizer"/>.
        /// </summary>
        /// <param name="record">The feature data record</param>
        /// <param name="renderPhase">The rendering phase</param>
        /// <param name="transform">The affine tranformation</param>
        /// <param name="rasterizer">The <see cref="IGeometryRasterizer"/></param>
        void Symbolize(IFeatureDataRecord record, RenderPhase renderPhase, Matrix2D transform, IGeometryRasterizer rasterizer);
        
        /// <summary>
        /// Function to evaluate the necessary <see cref="GeometryStyle"/>
        /// </summary>
        /// <param name="record">The feature data record</param>
        /// <param name="phase">The render phase</param>
        /// <param name="style">The <see cref="GeometryStyle"/></param>
        /// <returns><c>true</c> if the <see cref="GeometryStyle"/> could be evaluated.</returns>
        bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out GeometryStyle style);
    }
}