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
using SharpMap.Rendering.Rendering2D;

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
            while (_renderQueue.Count > 0)
            {
                sb.Append(_renderQueue.Dequeue().Json);
                if (_renderQueue.Count > 0)
                    sb.Append(",");
            }
            sb.Append("]");
            sb.Append("}");
            return sb.ToString();
        }

        public IRasterRenderer2D CreateRasterRenderer()
        {
            return new GeoJsonRasterRenderer();
        }

        public IVectorRenderer2D CreateVectorRenderer()
        {
            return new GeoJsonVectorRenderer();
        }

        public ITextRenderer2D CreateTextRenderer()
        {
            return new GeoJsonTextRenderer();
        }

        public double Dpi
        {
            get { return double.NaN; }
        }

        public Type GetRenderObjectType()
        {
            return typeof (GeoJsonRenderObject);
        }

        public void ClearRenderQueue()
        {
            _renderQueue.Clear();
        }

        public void EnqueueRenderObject(object o)
        {
            _renderQueue.Enqueue((GeoJsonRenderObject) o);
        }

        public event EventHandler RenderDone;

        Stream IWebMapRenderer.Render(WebMapView mapView, out string mimeType)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            sw.Write(Render(mapView, out mimeType));
            sw.Flush();

            ms.Position = 0;
            return ms;
        }

        public WebMapView MapView { get; set; }

        public void Dispose()
        {
        }

        public Type GeometryRendererType
        {
            get { return typeof (GeoJsonGeometryRenderer<>); }
        }

        public Type LabelRendererType
        {
            get { return typeof (BasicLabelRenderer2D<>); }
        }

        #endregion
    }
}