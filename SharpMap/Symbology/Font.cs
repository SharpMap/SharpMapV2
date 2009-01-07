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
using SharpMap.Expressions;

namespace SharpMap.Symbology
{
    public enum FontStyle
    {
        Normal,
        Italic,
        Oblique
    }

    public enum FontWeight
    {
        Normal,
        Bold
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FontType")]
    [XmlRoot("Font", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Font : NameMappedParameterObject
    {
        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("SvgParameter")]
        public SvgParameter[] SvgParameter
        {
            get { return Enumerable.ToArray<SvgParameter>(ParameterMap.Values); }
            set
            {
                foreach (SvgParameter parameter in value)
                {
                    if (parameter != null)
                    {
                        ParameterMap[parameter.Name] = parameter;   
                    }
                }
            }
        }

        public String GetFamily(Evaluator evaluator)
        {
            return (String)evaluator.Evaluate((Expression)ParameterMap["font-family"]);
        }

        public FontStyle GetStyle(Evaluator evaluator)
        {
            return (FontStyle)Enum.Parse(typeof(FontStyle), (String)evaluator.Evaluate((Expression)ParameterMap["font-style"]));
        }

        public FontWeight GetWeight(Evaluator evaluator)
        {
            return (FontWeight)Enum.Parse(typeof(FontWeight), (String)evaluator.Evaluate((Expression)ParameterMap["font-weight"]));
        }

        public Single GetSize(Evaluator evaluator)
        {
            return Convert.ToSingle(evaluator.Evaluate((Expression)ParameterMap["font-size"]));
        }
    }
}