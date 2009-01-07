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
    [XmlInclude(typeof (RasterSymbolizer))]
    [XmlInclude(typeof (TextSymbolizer))]
    [XmlInclude(typeof (PointSymbolizer))]
    [XmlInclude(typeof (PolygonSymbolizer))]
    [XmlInclude(typeof (LineSymbolizer))]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "SymbolizerType")]
    public abstract class Symbolizer
    {
        private String _name;
        private Description _description;
        private BaseSymbolizer _baseSymbolizer;
        private OgcSymbologyEncodingVersion _version;
        private Boolean _versionSpecified;
        private String _uom;

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public BaseSymbolizer BaseSymbolizer
        {
            get { return _baseSymbolizer; }
            set { _baseSymbolizer = value; }
        }

        [XmlAttribute(AttributeName = "version")]
        public OgcSymbologyEncodingVersion Version
        {
            get { return _version; }
            set { _version = value; }
        }
        
        [XmlIgnore]
        public Boolean VersionSpecified
        {
            get { return _versionSpecified; }
            set { _versionSpecified = value; }
        }

        [XmlAttribute(DataType = "anyURI", AttributeName = "uom")]
        public String Uom
        {
            get { return _uom; }
            set { _uom = value; }
        }
    }
}