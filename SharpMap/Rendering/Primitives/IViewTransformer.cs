// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Rendering
{
    public interface IViewTransformer
    {
        IViewVector TransformToView(GeoPoint point);
        IEnumerable<IViewVector> TransformToView(IEnumerable<GeoPoint> points);
        GeoPoint ViewToWorld(IViewVector viewVector);
        BoundingBox ViewToWorld(IViewMatrix viewMatrix);
        IViewVector WorldToView(GeoPoint geoPoint);
        IViewMatrix WorldToView(BoundingBox bounds);
    }

    public interface IViewTransformer<TViewPoint, TViewRectangle> : IViewTransformer
        where TViewPoint : IViewVector
        where TViewRectangle : IViewMatrix
    {
        new TViewPoint TransformToView(GeoPoint point);
        new IEnumerable<TViewPoint> TransformToView(IEnumerable<GeoPoint> points);
        GeoPoint ViewToWorld(TViewPoint viewVector);
        BoundingBox ViewToWorld(TViewRectangle viewMatrix);
        new TViewPoint WorldToView(GeoPoint geoPoint);
        new TViewRectangle WorldToView(BoundingBox bounds);
    }
}
