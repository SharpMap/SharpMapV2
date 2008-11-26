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
using GeoAPI.Geometries;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.Rendering.GeoJson
{
    public class GeoJsonLabelRenderer : FeatureRenderer2D<GeoJsonLabelStyle, GeoJsonRenderObject>,
                                        ILabelRenderer<Point2D, Size2D, Rectangle2D, GeoJsonRenderObject>
    {
        public GeoJsonLabelRenderer(VectorRenderer2D<GeoJsonRenderObject> vectorRenderer)
            : base(vectorRenderer)
        {
        }

        #region ILabelRenderer<Point2D,Size2D,Rectangle2D,GeoJsonRenderObject> Members

        public IEnumerable<GeoJsonRenderObject> RenderLabel(
            ILabel<Point2D, Size2D, Rectangle2D, Path<Point2D, Rectangle2D>> label)
        {
            ///I dont think we need this for GeoJson

            throw new NotImplementedException();
        }

        #endregion

        protected override IEnumerable<GeoJsonRenderObject> DoRenderFeature(IFeatureDataRecord feature,
                                                                            GeoJsonLabelStyle style,
                                                                            RenderState state, ILayer layer)
        {
            var fdr = (FeatureDataRow)feature;

            string text = fdr.Evaluate(style.LabelExpression);


            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append(JsonUtility.FormatJsonAttribute("type", "Label"));
            sb.Append(",");
            sb.Append(JsonUtility.FormatJsonAttribute("id", feature.GetOid()));
            sb.Append(",");
            sb.Append(JsonUtility.FormatJsonAttribute("text", text));
            sb.Append(",");


            IGeometry geom;
            if (style.IncludeGeometryData)
                geom = style.PreProcessGeometries
                           ? style.GeometryPreProcessor(feature.Geometry)
                           : feature.Geometry;
            else
                geom = feature.Geometry.Centroid;

            sb.Append("\"shape\":");
            GeoJsonFeatureWriter.WriteGeometry(sb, geom, style.CoordinateNumberFormatString);
            sb.Append("}");


            yield return new GeoJsonRenderObject(sb.ToString());
        }
    }
}