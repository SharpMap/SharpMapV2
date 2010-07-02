using System.IO;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Data.Providers
{
    public class BrutileRasterRecord : IRasterRecord
    {
        private Stream _tileImage;
        private Rectangle2D _viewBounds;
        //private Rectangle2D _rasterBounds;
        private Matrix2D _toViewTransform;

        internal BrutileRasterRecord(Stream tileImage, IExtents extents)
        {
            _tileImage = tileImage;
            _viewBounds = new Rectangle2D(extents.Min[Ordinates.X], extents.Max[Ordinates.Y],
                                          extents.Max[Ordinates.X], extents.Min[Ordinates.Y]);
        }

        #region Implementation of IRasterRecord

        public Stream GetImage(IExtents viewPort, Matrix2D toViewTransform)
        {
            _toViewTransform = toViewTransform;
            return _tileImage;
        }

        public Rectangle2D ViewBounds
        {
            get
            {
                Point2D pt = _viewBounds.UpperLeft;
                Size2D sz = _viewBounds.Size;
                return new Rectangle2D(_toViewTransform.TransformVector(pt.X, pt.Y), 
                    new Size2D((double)_toViewTransform[0, 0] * sz.Width+1, (double)_toViewTransform[1, 1] * sz.Height+1));
            }
        }

        public Rectangle2D RasterBounds
        {
            get { return Rectangle2D.Empty; }
        }

        #endregion
    }
        
}
