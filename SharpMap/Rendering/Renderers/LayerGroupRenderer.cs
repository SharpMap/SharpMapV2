using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;
using SharpMap.Geometries;
using SharpMap.Layers;
using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public class LayerGroupRenderer : BaseFeatureRenderer2D<Style, object>
    {
        protected override IEnumerable<PositionedRenderObject2D<object>> DoRenderFeature(FeatureDataRow feature, IRenderContext renderContext)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
