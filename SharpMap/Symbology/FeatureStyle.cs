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
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FeatureTypeStyleType")]
    [XmlRoot("FeatureTypeStyle", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class FeatureStyle : Style
    {
        private XmlQualifiedName _featureTypeName;
        private Boolean _areFeaturesSelectable;

        public XmlQualifiedName FeatureTypeName
        {
            get { return _featureTypeName; }
            set { _featureTypeName = value; }
        }

        [XmlIgnore]
        public Boolean AreFeaturesSelectable
        {
            get { return _areFeaturesSelectable; }
            set { _areFeaturesSelectable = value; }
        }
    }
}