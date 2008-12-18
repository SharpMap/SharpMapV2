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
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "CoverageStyleType")]
    [XmlRoot("CoverageStyle", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class CoverageStyle
    {
        private string _name;
        private Description _description;
        private string _coverageName;
        private string[] _semanticTypeIdentifier;
        private object[] _items;
        private OgcSymbologyEncodingVersion _version;
        private bool _versionSpecified;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string CoverageName
        {
            get { return _coverageName; }
            set { _coverageName = value; }
        }

        [XmlElement("SemanticTypeIdentifier")]
        public string[] SemanticTypeIdentifier
        {
            get { return _semanticTypeIdentifier; }
            set { _semanticTypeIdentifier = value; }
        }

        [XmlElement("OnlineResource", typeof (OnlineResource))]
        [XmlElement("Rule", typeof (Rule))]
        public object[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        [XmlAttribute(AttributeName = "version")]
        public OgcSymbologyEncodingVersion Version
        {
            get { return _version; }
            set { _version = value; }
        }

        [XmlIgnore]
        public bool VersionSpecified
        {
            get { return _versionSpecified; }
            set { _versionSpecified = value; }
        }
    }
}