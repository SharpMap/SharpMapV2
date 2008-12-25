// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Xml.Serialization;
using GeoAPI.DataStructures;
using GeoAPI.DataStructures.Collections.Generic;
using SharpMap.Data;
using SharpMap.Expressions;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "StrokeType")]
    [XmlRoot("Stroke", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Stroke
    {
        private Object _fillOrStroke;
        private readonly HybridDictionary<String, SvgParameter> _svgParameterMap 
            = new HybridDictionary<String, SvgParameter>();

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("GraphicFill", typeof (GraphicFill))]
        [XmlElement("GraphicStroke", typeof (GraphicStroke))]
        public Object GraphicFillOrGraphicStroke
        {
            get { return _fillOrStroke; }
            set { _fillOrStroke = value; }
        }

        [XmlIgnore]
        public GraphicFill GraphicFill
        {
            get { return _fillOrStroke as GraphicFill; }
        }

        [XmlIgnore]
        public GraphicStroke GraphicStroke
        {
            get { return _fillOrStroke as GraphicStroke; }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("SvgParameter")]
        public SvgParameter[] SvgParameter
        {
            get { return Enumerable.ToArray(_svgParameterMap.Values); }
            set
            {
                foreach (SvgParameter parameter in value)
                {
                    _svgParameterMap[parameter.Name] = parameter;
                }
            }
        }

        public StyleColor GetColor(IFeatureDataRecord feature)
        {
            Expression expression = _svgParameterMap["stroke"].Expression;
            String result = feature.EvaluateFor<String>();
            return result == null ? StyleColor.Transparent : StyleColor.FromArgbString(result);
        }

        public Single GetOpacity(IFeatureDataRecord feature)
        {
            return feature.Evaluate<Single>(_svgParameterMap["stroke-opacity"]);
        }

        public Single GetWidth(IFeatureDataRecord feature)
        {
            return feature.Evaluate<Single>(_svgParameterMap["stroke-width"]);
        }

        public LineJoin GetLineJoin(IFeatureDataRecord feature)
        {
            String result = feature.Evaluate<String>(_svgParameterMap["stroke-linejoin"]);

            if (String.Equals(result, LineJoin.Miter.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return LineJoin.Miter;
            }

            if (String.Equals(result, LineJoin.Round.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return LineJoin.Round;
            }

            if (String.Equals(result, LineJoin.Bevel.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return LineJoin.Bevel;
            }

            return LineJoin.Miter;
        }

        public LineCap GetLineCap(IFeatureDataRecord feature)
        {
            String result = feature.Evaluate<String>(_svgParameterMap["stroke-linecap"]);

            if (String.Equals(result, LineCap.Butt.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return LineCap.Butt;
            }

            if (String.Equals(result, LineCap.Round.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return LineCap.Round;
            }

            if (String.Equals(result, LineCap.Square.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return LineCap.Square;
            }

            return LineCap.Butt;
        }

        public Single[] GetDashArray(IFeatureDataRecord feature)
        {
            return feature.Evaluate<Single[]>(_svgParameterMap["stroke-dasharray"]);
        }

        public Single GetDashOffset(IFeatureDataRecord feature)
        {
            return feature.Evaluate<Single>(_svgParameterMap["stroke-dashoffset"]);
        }
    }
}