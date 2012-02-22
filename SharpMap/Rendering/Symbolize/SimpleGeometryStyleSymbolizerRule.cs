using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Styles;

namespace SharpMap.Rendering.Symbolize
{
    /// <summary>
    /// Simple symbolizer rule that just always returns the <see cref="GeometryStyle"/>
    /// </summary>
    public class SimpleGeometryStyleSymbolizerRule : GeometrySymbolizerRule
    {
        private GeometryStyle _geometryStyle;

        public SimpleGeometryStyleSymbolizerRule(GeometryStyle style)
        {
            _geometryStyle = style;
        }

        ///<summary>
        /// The geometry style to apply
        ///</summary>
        public GeometryStyle GeometryStyle
        {
            get { return _geometryStyle; }
            set
            {
                if (_geometryStyle == value)
                    return;
                if (_geometryStyle != null && value != null && _geometryStyle.Equals(value))
                    return;

                _geometryStyle = value;
                OnPropertyChanged("GeometryStyle");
            }
        }

        public override bool EvaluateStyle(IFeatureDataRecord record, RenderPhase phase, out GeometryStyle style)
        {
            // always return the geometry style
            style = GeometryStyle;
            return true;
        }
    }
}