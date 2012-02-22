using System.Collections.Generic;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Symbolize
{
    /// <summary>
    /// Interface for all symbolizers that can symbolize geometry feature data
    /// </summary>
    public interface IGeometrySymbolizer : ISymbolizer
    {
        /// <summary>
        /// Gets a collection of <see cref="ISymbolizerRule"/>s.
        /// </summary>
        ICollection<IGeometrySymbolizerRule> Rules { get; }
        
        /// <summary>
        /// Function to symbolize a series of <paramref name="records"/> at the provided <paramref name="renderPhase"/> applying the provided <paramref name="transform"/> and using the <paramref name="rasterizer"/>
        /// </summary>
        /// <param name="records">The records to symbolize</param>
        /// <param name="renderPhase">The rendering phase</param>
        /// <param name="transform">the Affine transformation</param>
        /// <param name="rasterizer">The rasterizer to use.</param>
        void Symbolize(IEnumerable<IFeatureDataRecord> records, RenderPhase renderPhase, Matrix2D transform, IGeometryRasterizer rasterizer);
    }
}