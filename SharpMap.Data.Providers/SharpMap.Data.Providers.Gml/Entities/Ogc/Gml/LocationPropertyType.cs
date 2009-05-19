// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "LocationPropertyType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (AbstractGeometricPrimitiveType)), XmlInclude(typeof (GeometricComplexType)),
     XmlInclude(typeof (GridType)), XmlInclude(typeof (AbstractGeometricAggregateType))]
    public class LocationPropertyType
    {
        [XmlIgnore] private AbstractGeometry _abstractGeometry;
        [XmlIgnore] private Actuate _actuate;
        [XmlIgnore] private string _arcrole;
        [XmlIgnore] private string _href;
        [XmlIgnore] private LocationKeyWord _locationKeyWord;
        [XmlIgnore] private LocationString _locationString;
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] private string _null;
        [XmlIgnore] private string _remoteSchema;
        [XmlIgnore] private string _role;
        [XmlIgnore] private Show _show;
        [XmlIgnore] private string _title;
        [XmlIgnore] private string _type;
        [XmlIgnore] public bool ActuateSpecified;
        [XmlIgnore] public bool ShowSpecified;

        public LocationPropertyType()
        {
            Type = "simple";
            Null = string.Empty;
        }

        [XmlElement(Type = typeof (AbstractGeometry), ElementName = "AbstractGeometry", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public AbstractGeometry AbstractGeometry
        {
            get { return _abstractGeometry; }
            set { _abstractGeometry = value; }
        }

        [XmlAttribute(AttributeName = "actuate")]
        public Actuate Actuate
        {
            get { return _actuate; }
            set
            {
                _actuate = value;
                ActuateSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "arcrole", DataType = "anyURI")]
        public string Arcrole
        {
            get { return _arcrole; }
            set { _arcrole = value; }
        }

        [XmlAttribute(AttributeName = "href", DataType = "anyURI")]
        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        [XmlElement(Type = typeof (LocationKeyWord), ElementName = "LocationKeyWord", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LocationKeyWord LocationKeyWord
        {
            get { return _locationKeyWord; }
            set { _locationKeyWord = value; }
        }

        [XmlElement(Type = typeof (LocationString), ElementName = "LocationString", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public LocationString LocationString
        {
            get { return _locationString; }
            set { _locationString = value; }
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        [XmlElement(ElementName = "Null", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public string Null
        {
            get { return _null; }
            set { _null = value; }
        }

        [XmlAttribute(AttributeName = "remoteSchema", DataType = "anyURI")]
        public string RemoteSchema
        {
            get { return _remoteSchema; }
            set { _remoteSchema = value; }
        }

        [XmlAttribute(AttributeName = "role", DataType = "anyURI")]
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        [XmlAttribute(AttributeName = "show")]
        public Show Show
        {
            get { return _show; }
            set
            {
                _show = value;
                ShowSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "title", DataType = "string")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [XmlAttribute(AttributeName = "type", DataType = "string")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            AbstractGeometry.MakeSchemaCompliant();
            LocationKeyWord.MakeSchemaCompliant();
            LocationString.MakeSchemaCompliant();
        }
    }
}