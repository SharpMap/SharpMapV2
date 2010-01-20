using System;
using System.Drawing;
using GeoAPI.Coordinates;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Utilities;

namespace SharpMap.Data.Providers.GdalPreview
{
    /// <summary>
    /// Interface for Computing the images to render
    /// </summary>
    public abstract class BaseGdalPreview
    {
        #region Nested Types
        /// <summary>
        /// 
        /// </summary>
        protected class GdalReadParameter
        {
            public Int32 X1;
            public Int32 Y1;
            public Int32 X2;
            public Int32 Y2;
            public Int32 SourceWidth { get { return X2 - X1; } }
            public Int32 SourceHeight { get { return Y2 - Y1; } }
            public Int32 ResultWidth;
            public Int32 ResultHeight;
            public Rectangle2D ViewPort;
        }
        #endregion


        protected GdalRasterProvider _provider;

        /// <summary>
        /// Function that creates the Bitmap to render
        /// </summary>
        /// <param name="viewPort">Viewport</param>
        /// <param name="toViewTransform">Transformation Matrix to Device coordinates</param>
        /// <param name="viewBounds">Bounds of intersection of Raster extent an Viewport in Device coordinates</param>
        /// <param name="rasterBounds">Bounds of Raster, always starting with 0,0</param>
        /// <returns>Bitmap to render</returns>
        public abstract Bitmap GetPreview(IExtents viewPort, Matrix2D toViewTransform, 
            out Rectangle2D viewBounds,out Rectangle2D rasterBounds);

        public virtual void Initialize(GdalRasterProvider provider)
        {
            _provider = provider;
            _provider.ReadFile();
        }

        public abstract String Name { get; }

        protected virtual GdalReadParameter GetRasterViewPort(IExtents viewPort, Matrix2D toViewTransform)
        {
            //Intersection of viewport with raster extents
            IExtents rasterExtents = viewPort.Intersection(_provider.GetExtents());
            if (rasterExtents.IsEmpty)
                return null;

            IGeometryFactory geoFactory;
            if (_provider.CoordinateTransformation != null)
                geoFactory = new GeometryServices()[_provider.OriginalSpatialReference.Wkt];
            else
                geoFactory = _provider.GeometryFactory;

            IExtents shownRasterExtents;
            // put display bounds into current projection
            ICoordinateTransformation coordinateTransformation = _provider.CoordinateTransformation;
            if (coordinateTransformation != null)
                shownRasterExtents = coordinateTransformation.InverseTransform(rasterExtents, geoFactory);
            else
                shownRasterExtents = rasterExtents;

            Double[] geoTrans = new double[6];
            _provider.Dataset.GetGeoTransform(geoTrans);
            GeoTransform geoTransform = new GeoTransform(_provider.GeometryFactory, geoTrans);
            Double x1 = Double.MaxValue;
            Double y1 = Double.MaxValue;
            Double x2 = Double.MinValue;
            Double y2 = Double.MinValue;
            Double count = 0;
            ICoordinateSequence sre = shownRasterExtents.ToGeometry().Coordinates;
            foreach (ICoordinate c in sre)
            {
                Int32 ctx, cty;
                geoTransform.GroundToImage(c, out ctx, out cty);
                x1 = ctx < x1 ? ctx : x1;
                y1 = cty < y1 ? cty : y1;
                x2 = ctx > x2 ? ctx : x2;
                y2 = cty > y2 ? cty : y2;
                if (++count == 4) break;
            }

            // stay within image
            Size2D imageSize = _provider.Size;
            if (x2 > imageSize.Width) x2 = imageSize.Width;
            if (y2 > imageSize.Height) y2 = imageSize.Height;
            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;

            Int32 displayImageWidth = (int)(x2 - x1);
            Int32 displayImageHeight = (int)(y2 - y1);

            // convert ground coordinates to map coordinates to figure out where to place the bitmap
            Point2D bitmapBR = toViewTransform.TransformVector(rasterExtents.Max[Ordinates.X], rasterExtents.Min[Ordinates.Y]) + Point2D.One;
            Point2D bitmapTL = toViewTransform.TransformVector(rasterExtents.Min[Ordinates.X], rasterExtents.Max[Ordinates.Y]);

            Int32 bitmapWidth = (Int32)(bitmapBR.X - bitmapTL.X);
            Int32 bitmapHeight = Math.Abs((Int32)(bitmapBR.Y - bitmapTL.Y));

            // check to see if image is on its side
            if (bitmapWidth > bitmapHeight && displayImageWidth < displayImageHeight)
            {
                displayImageWidth = bitmapHeight;
                displayImageHeight = bitmapWidth;
            }
            else
            {
                displayImageWidth = bitmapWidth;
                displayImageHeight = bitmapHeight;
            }

            // 0 pixels in length or height, nothing to display
            if (bitmapWidth < 1 || bitmapHeight < 1)
                return null;

            GdalReadParameter readParameter = new GdalReadParameter
                {
                    X1 = (Int32) x1,
                    Y1 = (Int32) y1,
                    X2 = (Int32) x2,
                    Y2 = (Int32) y2,
                    ResultWidth = displayImageWidth,
                    ResultHeight = displayImageHeight,
                    ViewPort = new Rectangle2D(bitmapTL, new Size2D(displayImageWidth, displayImageHeight))
                };

            return readParameter;
        }
    }
}
