// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
	/// Utility class for transforming between world and view coordinates.
	/// </summary>
	public static class Transform2D
	{
		/// <summary>
		/// Transforms from world coordinate system to view coordinates.
		/// </summary>
        /// <param name="worldPoint">Point in world coordinate system.</param>
        /// <param name="mapViewPort">Map view port reference.</param>
		/// <returns>Point in view coordinates.</returns>
        /// <remarks>
        /// NOTE: This method DOES NOT take the <see cref="MapViewPort2D".MapTransform>MapTransform</see> of the <see cref="MapViewPort2D"/>.
        /// </remarks>
        public static ViewPoint2D WorldToView(GeoPoint worldPoint, MapViewPort2D mapViewPort)
		{
            double heightUnit = (mapViewPort.WorldWidth * mapViewPort.ViewSize.Height) / mapViewPort.ViewSize.Width;
            double left = mapViewPort.GeoCenter.X - mapViewPort.WorldWidth * 0.5;
            double top = mapViewPort.GeoCenter.Y + heightUnit * 0.5 * mapViewPort.PixelAspectRatio;
			double x = (float)((worldPoint.X - left) / mapViewPort.PixelWidth);
			double y = (float)((top - worldPoint.Y) / mapViewPort.PixelHeight);

            ViewPoint2D result = new ViewPoint2D(x, y);
			return result;
        }

        /// <summary>
        /// Transforms a <see cref="BoundingBox"/> rectangle from world coordinate system to view coordinates.
        /// </summary>
        /// <param name="worldBounds">Rectangle in world coordinate system.</param>
        /// <param name="mapViewPort">Map view port reference.</param>
        /// <returns>Rectangle in view coordinates.</returns>
        /// <remarks>
        /// NOTE: This method DOES NOT take the <see cref="MapViewPort2D".MapTransform>MapTransform</see> of the <see cref="MapViewPort2D"/>.
        /// </remarks>
        public static ViewRectangle2D WorldToView(BoundingBox worldBounds, MapViewPort2D mapViewPort)
        {
            ViewPoint2D lowerLeft = WorldToView(worldBounds.Min, mapViewPort);
            ViewPoint2D upperRight = WorldToView(worldBounds.Max, mapViewPort);
            return ViewRectangle2D.FromLTRB(lowerLeft.X, upperRight.Y, upperRight.X, lowerLeft.Y);
        }

		/// <summary>
		/// Transforms a point from view coordinates to world coordinate system.
		/// </summary>
        /// <param name="viewPoint">Point in view coordinates.</param>
        /// <param name="mapViewPort">Map view port reference.</param>
        /// <returns>Point in world coordinate system.</returns>
        /// <remarks>
        /// NOTE: This method DOES NOT take the <see cref="MapViewPort2D".MapTransform>MapTransform</see> of the <see cref="MapViewPort2D"/>.
        /// </remarks>
        public static GeoPoint ViewToWorld(ViewPoint2D viewPoint, MapViewPort2D mapViewPort)
		{
			BoundingBox env = mapViewPort.ViewEnvelope;
			return new GeoPoint(env.Min.X + viewPoint.X * mapViewPort.PixelWidth, env.Max.Y - viewPoint.Y * mapViewPort.PixelHeight);
		}

        /// <summary>
        /// Transforms a rectangle from view coordinates to world coordinate system.
        /// </summary>
        /// <param name="rectangle">Rectangle in view coordinates.</param>
        /// <param name="mapViewPort">Map view port reference.</param>
        /// <returns>Rectangle in world coordinate system.</returns>
        /// <remarks>
        /// NOTE: This method DOES NOT take the <see cref="MapViewPort2D".MapTransform>MapTransform</see> of the <see cref="MapViewPort2D"/>.
        /// </remarks>
        public static BoundingBox ViewToWorld(ViewRectangle2D rectangle, MapViewPort2D mapViewPort)
        {
            GeoPoint lowerLeft = ViewToWorld(new ViewPoint2D(rectangle.Left, rectangle.Bottom), mapViewPort);
            GeoPoint upperRight = ViewToWorld(new ViewPoint2D(rectangle.Right, rectangle.Top), mapViewPort);
            return new BoundingBox(lowerLeft, upperRight);
        }
	}
}
