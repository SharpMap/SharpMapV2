using System.Collections.Generic;
using System.ComponentModel;
using SharpMap.Data;
using SharpMap.Presentation;
using SharpMap.Rendering.Rasterize;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Symbolize
{
    /// <summary>
    /// Geometry symbolizer class
    /// </summary>
    public class GeometrySymbolizer : Symbolizer, IGeometrySymbolizer
    {
        private readonly BindingList<IGeometrySymbolizerRule> _rules = new BindingList<IGeometrySymbolizerRule>();


        ///<summary>
        /// Creates an instance of this class
        ///</summary>
        public GeometrySymbolizer()
        {
            _rules.ListChanged += delegate { OnPropertyChanged("Rules"); };
        }

        #region IGeometrySymbolizer Members

        public ICollection<IGeometrySymbolizerRule> Rules
        {
            get { return _rules; }
        }

        public void Symbolize(IEnumerable<IFeatureDataRecord> records, RenderPhase renderPhase, Matrix2D transform,
                              IGeometryRasterizer rasterizer)
        {
            if (!Enabled)
                return;

            foreach (IGeometrySymbolizerRule rule in Rules)
            {
                if (rule.Enabled)
                    foreach (IFeatureDataRecord artifact in records)
                        rule.Symbolize(artifact, renderPhase, transform, rasterizer);
            }
        }

        #endregion
    }
}