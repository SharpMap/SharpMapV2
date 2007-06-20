using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public interface IGeometryRenderer<TViewPoint, TViewSize, TViewRectange, TRenderObject> : IRenderer<TViewPoint, TViewSize, TViewRectange, TRenderObject>
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectange : IViewMatrix
    {
    }
}
