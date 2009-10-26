/*
 *	This file is part of SharpMap.Rendering.Web.GeoJson
 *  SharpMap.Rendering.Web.GeoJson is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpMap.Presentation.AspNet;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.GeoJson;
using SharpMap.Rendering.Rasterize;

namespace SharpMap.Rendering.Web
{
    public class GeoJsonRenderer : IWebMapRenderer<string>
    {
        private readonly Queue<GeoJsonRenderObject> _renderQueue = new Queue<GeoJsonRenderObject>();

        #region IWebMapRenderer<string> Members

        public string Render(WebMapView mapView, out string mimeType)
        {
            mimeType = "application/json";

            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            sb.Append(JsonUtility.FormatJsonAttribute("type", "FeatureCollection"));
            sb.Append(",");

            sb.Append("\"features\":");
            sb.Append("[");

            StringBuilder rendered = MapView.RenderedData as StringBuilder;
            if (rendered.Length > 0)
                rendered.Remove(rendered.Length - 1, 1);
            sb.Append(rendered.ToString());

            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        public double Dpi
        {
            get { return double.NaN; }
        }

        Stream IWebMapRenderer.Render(WebMapView mapView, out string mimeType)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            sw.Write(Render(mapView, out mimeType));
            sw.Flush();

            ms.Position = 0;
            return ms;
        }

        private WebMapView _mapView;
        public WebMapView MapView
        {
            get { return _mapView; }
            set
            {
                _mapView = value;
                RasterizeSurface = new GeoJsonRasterizeSurface(value);
            }
        }

        public void Dispose()
        {
        }

        private IRasterizeSurface _rasterizeSurface;

        public IRasterizeSurface RasterizeSurface
        {
            get { return _rasterizeSurface; }
            protected set { _rasterizeSurface = value; }
        }

        #endregion


        public event EventHandler RenderDone;
    }
}