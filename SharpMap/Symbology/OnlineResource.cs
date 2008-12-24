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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "OnlineResourceType")]
    [XmlRoot("OnlineResource", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class OnlineResource
    {
        private String _type;
        private String _href;
        private String _role;
        private String _arcRole;
        private String _title;
        private Show _show;
        private Boolean _showSpecified;
        private Actuate _actuate;
        private Boolean _actuateSpecified;

        public OnlineResource()
        {
            _type = "simple";
        }

        [XmlAttribute(AttributeName = "type", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink")]
        public String Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "href", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink", DataType = "anyURI")]
        public String Href
        {
            get { return _href; }
            set { _href = value; }
        }

        [XmlAttribute(AttributeName = "role", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink", DataType = "anyURI")]
        public String Role
        {
            get { return _role; }
            set { _role = value; }
        }

        [XmlAttribute(AttributeName = "arcrole", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink", DataType = "anyURI")]
        public String ArcRole
        {
            get { return _arcRole; }
            set { _arcRole = value; }
        }

        [XmlAttribute(AttributeName = "title", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink")]
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [XmlAttribute(AttributeName = "show", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink")]
        public Show Show
        {
            get { return _show; }
            set { _show = value; }
        }

        [XmlIgnore]
        public Boolean ShowSpecified
        {
            get { return _showSpecified; }
            set { _showSpecified = value; }
        }

        [XmlAttribute(AttributeName = "actuate", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink")]
        public Actuate Actuate
        {
            get { return _actuate; }
            set { _actuate = value; }
        }

        [XmlIgnore]
        public Boolean ActuateSpecified
        {
            get { return _actuateSpecified; }
            set { _actuateSpecified = value; }
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/1999/xlink")]
    public enum Show
    {
        [XmlEnum(Name = "new")] 
        New,

        [XmlEnum(Name = "replace")] 
        Replace,

        [XmlEnum(Name = "embed")] 
        Embed,

        [XmlEnum(Name = "other")] 
        Other,

        [XmlEnum(Name = "none")] 
        None,
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://www.w3.org/1999/xlink")]
    public enum Actuate
    {
        [XmlEnum(Name = "onLoad")] 
        OnLoad,

        [XmlEnum(Name = "onRequest")] 
        OnRequest,

        [XmlEnum(Name = "other")] 
        Other,

        [XmlEnum(Name = "none")] 
        None,
    }
}