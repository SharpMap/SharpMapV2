using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;

namespace SharpMap.Rendering
{
    public interface IFeatureRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> : IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject>
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectangle : IViewMatrix
    {
        IEnumerable<TRenderObject> RenderFeature(FeatureDataRow feature);
    }
}
