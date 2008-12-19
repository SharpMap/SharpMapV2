/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using GeoAPI.Geometries;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Presentation.AspNet.Demo
{
    public class BasicMapRequestConfig
        : IMapRequestConfig
    {
        #region IMapRequestConfig Members

        public string CacheKey { get; set; }

        public string MimeType { get; set; }

        public IExtents2D RealWorldBounds { get; set; }

        public Size2D OutputSize { get; set; }

        public void ConfigureMapView(IMapView2D mapView)
        {
            mapView.ViewSize = OutputSize;
            if (RealWorldBounds == null)
                mapView.ZoomToExtents();
            else
                mapView.ZoomToWorldBounds(RealWorldBounds);
        }

        public void ConfigureMap(Map map)
        {
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