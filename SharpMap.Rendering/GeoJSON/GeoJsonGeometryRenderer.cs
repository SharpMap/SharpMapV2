/*
 *	This file is part of SharpMap.Rendering.GeoJson
 *  SharpMap.Rendering.GeoJson is free software © 2008 Newgrove Consultants Limited, 
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
using System.Text;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.GeoJson
{
    /// <remarks>
    /// TRenderObject is always GeoJsonRenderObject. It is left out of the definition to simplify the Renderer creation in MapPresenter2D
    /// </remarks>
    public class GeoJsonGeometryRenderer<TRenderObject>
        : FeatureRenderer2D<GeoJsonGeometryStyle, TRenderObject>
    {
        public GeoJsonGeometryRenderer(VectorRenderer2D<TRenderObject> vectorRenderer)
            : base(vectorRenderer)
        {
            if (typeof (TRenderObject) != typeof (GeoJsonRenderObject))
                throw new ArgumentException("TRenderObject must be of type GeoJsonRenderObject");
        }

        protected override IEnumerable<TRenderObject> DoRenderFeature(IFeatureDataRecord feature,
                                                                      GeoJsonGeometryStyle style,
                                                                      RenderState state,
                                                                      ILayer layer)
        {
            if (feature == null) throw new ArgumentNullException("feature");
            if (style == null) throw new ArgumentNullException("style");

            if (feature.Geometry == null)
            {
                throw new InvalidOperationException("Feature must have a geometry to be rendered.");
            }


            StringBuilder sb = new StringBuilder();

            GeoJsonFeatureWriter.WriteFeature(sb, style, feature);
            yield return (TRenderObject) (object) (new GeoJsonRenderObject(sb.ToString()));
                /* oh dear - i feel very dirty now */
        }
    }
}