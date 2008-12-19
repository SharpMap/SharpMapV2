using GeoAPI.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Presentation.AspNet.Demo.GeoJson
{
    public class GeoJsonRequestConfig : IMapRequestConfig
    {
        #region IMapRequestConfig Members

        public string CacheKey { get; set; }

        public string MimeType
        {
            get { return "application/json"; }
        }

        public IExtents2D RealWorldBounds { get; set; }

        public Size2D OutputSize { get; set; }

        public void ConfigureMap(Map map)
        {
        }

        public void ConfigureMapView(IMapView2D mapView)
        {
            mapView.ViewSize = OutputSize;
            if (RealWorldBounds == null)
                mapView.ZoomToExtents();
            else
                mapView.ZoomToWorldBounds(RealWorldBounds);
        }

        #endregion

        #region IMapRequestConfig Members

        public string BaseSrid
        {
            get; set;
        }

        #endregion
    }
}