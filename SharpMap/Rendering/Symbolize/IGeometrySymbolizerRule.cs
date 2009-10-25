using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public interface IGeometrySymbolizerRule
        : ISymbolizerRule
    {
        void Symbolize(IFeatureDataRecord obj, RenderPhase renderPhase, Matrix2D transform, IGeometryRasterizer rasterizer);
        bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out GeometryStyle style);
    }
}