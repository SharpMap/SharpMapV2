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
    [XmlType(Namespace = "http://www.opengis.net/se")]
    [XmlRoot("Mark", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Mark
    {
        private object[] _parameters;
        private MarkType[] _parameterTypes;
        private Fill _fill;
        private Stroke _stroke;

        [XmlElement("Format", typeof (string))]
        [XmlElement("InlineContent", typeof (InlineContent))]
        [XmlElement("MarkIndex", typeof (string), DataType = "integer")]
        [XmlElement("OnlineResource", typeof (OnlineResource))]
        [XmlElement("WellKnownName", typeof (string))]
        [XmlChoiceIdentifier("ParameterTypes")]
        public object[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        [XmlElement("ParameterTypes")]
        [XmlIgnore]
        public MarkType[] ParameterTypes
        {
            get { return _parameterTypes; }
            set { _parameterTypes = value; }
        }

        public Fill Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }

        public Stroke Stroke
        {
            get { return _stroke; }
            set { _stroke = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", IncludeInSchema = false)]
    public enum MarkType
    {
        Format,

        InlineContent,

        MarkIndex,

        OnlineResource,

        WellKnownName,
    }
}