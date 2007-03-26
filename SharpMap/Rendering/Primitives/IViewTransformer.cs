using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Rendering
{
    public interface IViewTransformer<TViewPoint, TViewRectangle>
    {
        TViewPoint TransformToView(GeoPoint point);
        IEnumerable<TViewPoint> TransformToView(IEnumerable<GeoPoint> points);
        GeoPoint ViewToWorld(TViewPoint p);
        BoundingBox ViewToWorld(TViewRectangle rect);
        TViewPoint WorldToView(GeoPoint p);
        TViewRectangle WorldToView(BoundingBox bbox);
    }
}
