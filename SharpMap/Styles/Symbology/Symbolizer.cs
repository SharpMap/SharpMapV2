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

namespace SharpMap.Styles.Symbology
{
    [XmlInclude(typeof (RasterSymbolizer))]
    [XmlInclude(typeof (TextSymbolizer))]
    [XmlInclude(typeof (PointSymbolizer))]
    [XmlInclude(typeof (PolygonSymbolizer))]
    [XmlInclude(typeof (LineSymbolizer))]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "SymbolizerType")]
    public abstract class Symbolizer
    {
        private String nameField;
        private Description descriptionField;
        private BaseSymbolizerType baseSymbolizerField;
        private OgcSymbologyEncodingVersion versionField;
        private bool versionFieldSpecified;
        private String uomField;

        public String Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        public Description Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        public BaseSymbolizerType BaseSymbolizer
        {
            get { return baseSymbolizerField; }
            set { baseSymbolizerField = value; }
        }

        [XmlAttribute(AttributeName = "version")]
        public OgcSymbologyEncodingVersion Version
        {
            get { return versionField; }
            set { versionField = value; }
        }

        [XmlIgnore]
        public Boolean VersionSpecified
        {
            get { return versionFieldSpecified; }
            set { versionFieldSpecified = value; }
        }

        [XmlAttribute(DataType = "anyURI", AttributeName = "uom")]
        public String Uom
        {
            get { return uomField; }
            set { uomField = value; }
        }
    }
}