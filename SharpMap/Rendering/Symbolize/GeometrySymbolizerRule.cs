using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public abstract class GeometrySymbolizerRule : SymbolizerRule, IGeometrySymbolizerRule
    {

        public void Symbolize(IFeatureDataRecord obj, RenderPhase phase, Matrix2D transform, IGeometryRasterizer rasterizer)
        {
            if (!Enabled)
                return;

            GeometryStyle style;
            if (EvaluateStyle(obj, phase, out style))
                rasterizer.Rasterize(obj.Geometry, style, transform);
        }

        public override bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out IStyle style)
        {
            GeometryStyle s;
            bool ret = EvaluateStyle(record, phase, out s);
            style = s;
            return ret;
        }
     
        public abstract bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out GeometryStyle style);
        

    }
}