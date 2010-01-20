using System.IO;
using GeoAPI.Geometries;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Data.Providers
{
    public class GdalRasterRecord : IRasterRecord
    {
        private readonly GdalRasterProvider _provider;
        private Rectangle2D _viewBounds;
        private Rectangle2D _rasterBounds;

        internal GdalRasterRecord(GdalRasterProvider provider, IExtents extents)
        {
            _provider = provider;
        }

        #region Implementation of IRasterRecord

        public Stream GetImage(IExtents viewPort, Matrix2D toViewTransform)
        {
            return _provider.GetPreview(viewPort, toViewTransform, out _viewBounds, out _rasterBounds);
        }

        public Rectangle2D ViewBounds
        {
            get { return _viewBounds; }
        }

        public Rectangle2D RasterBounds
        {
            get { return _rasterBounds; }
        }

        #endregion
    }

    
}