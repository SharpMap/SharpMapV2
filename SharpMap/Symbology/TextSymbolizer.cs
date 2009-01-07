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

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "TextSymbolizerType")]
    [XmlRoot("TextSymbolizer", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class TextSymbolizer
    {
        private GeometryPropertyNameExpression _geometry;
        private ParameterValue _label;
        private Font _font;
        private LabelPlacement _labelPlacement;
        private Halo _halo;
        private Fill _fill;

        public GeometryPropertyNameExpression Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }

        public ParameterValue Label
        {
            get { return _label; }
            set { _label = value; }
        }
        
        // ck - xsd.exe appears to have generated this, but Symbolizer.xsd calls for 
        //      a "Font" element of type "FontType". Don't know exactly why xsd.exe gen'd 
        //      this... but I changed it to Font.
        //[XmlArrayItem("SvgParameter", IsNullable = false)]
        public Font Font
        {
            get { return _font; }
            set { _font = value; }
        }

        public LabelPlacement LabelPlacement
        {
            get { return _labelPlacement; }
            set { _labelPlacement = value; }
        }

        public Halo Halo
        {
            get { return _halo; }
            set { _halo = value; }
        }

        public Fill Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }
    }
}