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
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "TrimType")]
    [XmlRoot("Trim", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class TrimExpression : SymbologyFunctionExpression
    {
        private ParameterValue _stringValue;
        private StripOffPositionType _stripOffPosition;
        private bool _stripOffPositionSpecified;
        private string _stripOffChar;

        [XmlElement(Order = 0)]
        public ParameterValue StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }

        [XmlAttribute(AttributeName = "stripOffPosition")]
        public StripOffPositionType StripOffPosition
        {
            get { return _stripOffPosition; }
            set { _stripOffPosition = value; }
        }

        [XmlIgnore]
        public bool StripOffPositionSpecified
        {
            get { return _stripOffPositionSpecified; }
            set { _stripOffPositionSpecified = value; }
        }

        [XmlAttribute(AttributeName = "stripOffChar")]
        public string StripOffChar
        {
            get { return _stripOffChar; }
            set { _stripOffChar = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "stripOffPositionType")]
    public enum StripOffPositionType
    {
        [XmlEnum(Name = "leading")] Leading,
        [XmlEnum(Name = "trailing")] Trailing,
        [XmlEnum(Name = "both")] Both,
    }
}