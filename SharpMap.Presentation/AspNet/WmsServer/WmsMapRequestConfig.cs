/*
 *  The attached / following is part of SharpMap.Presentation.AspNet.WmsServer
 *  SharpMap.Presentation.AspNet.WmsServer is free software © 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using GeoAPI.CoordinateSystems;
using GeoAPI.Geometries;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using SharpMap.Utilities;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Presentation.AspNet.WmsServer
{
    public class WmsMapRequestConfig : IMapRequestConfig
    {



        public Capabilities.WmsServiceDescription ServiceDescription { get; set; }

        public WmsMode WmsMode { get; set; }
        public StyleColor BackgroundColor { get; set; }

        public ICollection<string> EnabledLayerNames { get; set; }

        public string Crs { get; set; }

        #region IMapRequestConfig Members

        public string CacheKey { get; set; }

        public string MimeType { get; internal set; }

        public IExtents2D RealWorldBounds { get; set; }

        public Size2D OutputSize { get; set; }

        public virtual void ConfigureMap(Map map)
        {
            if (WmsMode == WmsMode.Capabilites)
                return;

            ICoordinateSystem coordinateSystem = SridMap.DefaultInstance.Process(this.Crs, (ICoordinateSystem)null);
            map.SpatialReference = coordinateSystem;

            foreach (ILayer l in map.Layers)
                l.Enabled = false;

            foreach (string layerName in EnabledLayerNames)
            {
                ILayer l = map.GetLayerByName(layerName);
                if (l != null)
                    l.Enabled = true;
                else
                    throw new WmsException(WmsExceptionCode.LayerNotDefined,
                                           string.Format("Layer {0} is not defined.", layerName));
            }
        }

        private static readonly object lockobj = new object();

        public void ConfigureMapView(IMapView2D mapView)
        {
            mapView.ViewSize = OutputSize;
            mapView.BackgroundColor = BackgroundColor;
            mapView.ZoomToWorldBounds(RealWorldBounds);            
        }

        #endregion

        #region IMapRequestConfig Members

        public string BaseSrid
        {
            get;
            set;
        }

        #endregion
    }
}