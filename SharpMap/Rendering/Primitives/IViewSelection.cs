using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public interface IViewSelection<TViewPoint, TViewSize, TViewRegion>
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRegion : IViewMatrix
    {
        void AddPoint(TViewPoint point);
        void Expand(TViewSize size);
        void MoveTo(TViewPoint location);
        void RemovePoint(TViewPoint point);
        GraphicsPath<TViewPoint, TViewRegion> Path { get; }
        TViewPoint AnchorPoint { get; set; }
        TViewRegion BoundingRegion { get; }
    }
}
