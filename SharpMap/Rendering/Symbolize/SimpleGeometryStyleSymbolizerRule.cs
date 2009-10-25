using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public class SimpleGeometryStyleSymbolizerRule : GeometrySymbolizerRule
    {
        private GeometryStyle _geometryStyle;

        public GeometryStyle GeometryStyle
        {
            get { return _geometryStyle; }
            set { _geometryStyle = value; }
        }

        public override bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out GeometryStyle style)
        {
            style = GeometryStyle;
            return true;
        }
    }
}