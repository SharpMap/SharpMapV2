/*
 *	This file is part of SharpMap
 *  SharpMap is free software. This file © 2008 Newgrove Consultants Limited, 
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
using System.IO;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Presentation.AspNet
{
    public interface IWebMapRenderer : IDisposable
    {
        WebMapView MapView { get; set; }
        double Dpi { get; }
        IRasterRenderer2D CreateRasterRenderer();
        IVectorRenderer2D CreateVectorRenderer();
        ITextRenderer2D CreateTextRenderer();
        Type GeometryRendererType { get; }
        Type LabelRendererType { get; }
        Type GetRenderObjectType();
        void ClearRenderQueue();
        void EnqueueRenderObject(object o);

        event EventHandler RenderDone;
        Stream Render(WebMapView mapView, out string mimeType);
    }

    public interface IWebMapRenderer<TOutputFormat> : IWebMapRenderer
    {
        new TOutputFormat Render(out string mimeType);
    }
}