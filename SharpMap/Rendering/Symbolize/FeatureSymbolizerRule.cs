using SharpMap.Data;
using SharpMap.Rendering.Rasterize;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    public abstract class FeatureSymbolizerRule : IFeatureSymbolizerRule
    {
        #region ISymbolizerRule Members

        public abstract bool Enabled { get; }

        public IFeatureSymbolizer Symbolizer { get; set; }

        public void Symbolize(IFeatureDataRecord obj, IFeatureRasterizer rasterizer)
        {
            if (!Enabled)
                return;

            IStyle style;
            if (EvaluateStyle(obj, out style))
                rasterizer.Rasterize(obj.Geometry, style);
        }

        public abstract bool EvaluateStyle(IFeatureDataRecord record, out IStyle style);

        #endregion

        #region ISymbolizerRule Members

        ISymbolizer ISymbolizerRule.Symbolizer
        {
            get { return Symbolizer; }
        }

        #endregion
    }
}