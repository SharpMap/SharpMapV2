// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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

//using System.Drawing;
//using GdiPointF = System.Drawing.PointF;

using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Presentation;
using SharpMap.Rendering;

namespace SharpMap.Utilities
{
	/// <summary>
	/// Class for transforming between world and image coordinate
	/// </summary>
	public static class Transform2D
	{
		/// <summary>
		/// Transforms from world coordinate system (WCS) to image coordinates
		/// NOTE: This method DOES NOT take the MapTransform property into account (use SharpMap.Map.MapToWorld instead)
		/// </summary>
		/// <param name="p">Point in WCS</param>
		/// <param name="map">Map reference</param>
		/// <returns>Point in image coordinates</returns>
        public static ViewPoint2D WorldToMap(GeoPoint p, MapViewPort2D mapViewPort)
		{
            double heightUnit = (mapViewPort.Zoom * mapViewPort.ViewSize.Height) / mapViewPort.ViewSize.Width;
			double left = mapViewPort.GeoCenter.X - mapViewPort.Zoom * 0.5;
            double top = mapViewPort.GeoCenter.Y + heightUnit * 0.5 * mapViewPort.PixelAspectRatio;
			double x = (float)((p.X - left) / mapViewPort.PixelWidth);
			double y = (float)((top - p.Y) / mapViewPort.PixelHeight);

            ViewPoint2D result = new ViewPoint2D(x, y);
			return result;
		}

		/// <summary>
		/// Transforms from image coordinates to world coordinate system (WCS).
		/// NOTE: This method DOES NOT take the MapTransform property into account (use SharpMap.Map.MapToWorld instead)
		/// </summary>
		/// <param name="p">Point in image coordinate system</param>
		/// <param name="map">Map reference</param>
		/// <returns>Point in WCS</returns>
        public static GeoPoint MapToWorld(ViewPoint2D p, MapViewPort2D mapViewPort)
		{
			BoundingBox env = mapViewPort.ViewEnvelope;
			return new GeoPoint(env.Min.X + p.X * mapViewPort.PixelWidth, env.Max.Y - p.Y * mapViewPort.PixelHeight);
		}

        public static ViewRectangle2D WorldToMap(BoundingBox box, MapViewPort2D mapViewPort)
        {
            ViewPoint2D lowerLeft = WorldToMap(box.Min, mapViewPort);
            ViewPoint2D upperRight = WorldToMap(box.Max, mapViewPort);
            return ViewRectangle2D.FromLTRB(lowerLeft.X, upperRight.Y, upperRight.X, lowerLeft.Y);
        }

        public static BoundingBox MapToWorld(ViewRectangle2D rectangle, MapViewPort2D mapViewPort)
        {
            GeoPoint lowerLeft = MapToWorld(new ViewPoint2D(rectangle.Left, rectangle.Bottom), mapViewPort);
            GeoPoint upperRight = MapToWorld(new ViewPoint2D(rectangle.Right, rectangle.Top), mapViewPort);
            return new BoundingBox(lowerLeft, upperRight);
        }
	}
}
