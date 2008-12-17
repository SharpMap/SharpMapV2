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

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "OnlineResourceType")]
    [XmlRoot("OnlineResource", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class OnlineResource
    {
        private string _type;
        private string _href;
        private string _role;
        private string _arcRole;
        private string _title;
        private Show _show;
        private bool _showSpecified;
        private Actuate _actuate;
        private bool _actuateSpecified;

        public OnlineResource()
        {
            _type = "simple";
        }

        [XmlAttribute(AttributeName = "type", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "href", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink", DataType = "anyURI")]
        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        [XmlAttribute(AttributeName = "role", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink", DataType = "anyURI")]
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        [XmlAttribute(AttributeName = "arcrole", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink", DataType = "anyURI")]
        public string ArcRole
        {
            get { return _arcRole; }
            set { _arcRole = value; }
        }

        [XmlAttribute(AttributeName = "title", Form = XmlSchemaForm.Qualified, Namespace = "http://www.w3.org/1999/xlink")]
        public string Title
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
        public bool ShowSpecified
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
        public bool ActuateSpecified
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