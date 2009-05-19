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
    [Serializable, XmlType(TypeName = "GeometricComplexPropertyType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class GeometricComplexPropertyType
    {
        [XmlIgnore] private Actuate _actuate;
        [XmlIgnore] private string _arcrole;
        [XmlIgnore] private CompositeCurve _compositeCurve;
        [XmlIgnore] private CompositeSolid _compositeSolid;
        [XmlIgnore] private CompositeSurface _compositeSurface;
        [XmlIgnore] private GeometricComplex _geometricComplex;
        [XmlIgnore] private string _href;
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] private string _remoteSchema;
        [XmlIgnore] private string _role;
        [XmlIgnore] private Show _show;
        [XmlIgnore] private string _title;
        [XmlIgnore] private string _type;
        [XmlIgnore] public bool ActuateSpecified;
        [XmlIgnore] public bool OwnsSpecified;
        [XmlIgnore] public bool ShowSpecified;

        public GeometricComplexPropertyType()
        {
            Owns = false;
            Type = "simple";
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

        [XmlElement(Type = typeof (CompositeCurve), ElementName = "CompositeCurve", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CompositeCurve CompositeCurve
        {
            get { return _compositeCurve; }
            set { _compositeCurve = value; }
        }

        [XmlElement(Type = typeof (CompositeSolid), ElementName = "CompositeSolid", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CompositeSolid CompositeSolid
        {
            get { return _compositeSolid; }
            set { _compositeSolid = value; }
        }

        [XmlElement(Type = typeof (CompositeSurface), ElementName = "CompositeSurface", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public CompositeSurface CompositeSurface
        {
            get { return _compositeSurface; }
            set { _compositeSurface = value; }
        }

        [XmlElement(Type = typeof (GeometricComplex), ElementName = "GeometricComplex", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public GeometricComplex GeometricComplex
        {
            get { return _geometricComplex; }
            set { _geometricComplex = value; }
        }

        [XmlAttribute(AttributeName = "href", DataType = "anyURI")]
        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        [XmlAttribute(AttributeName = "owns", DataType = "boolean")]
        public bool Owns
        {
            get { return _owns; }
            set
            {
                _owns = value;
                OwnsSpecified = true;
            }
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
            GeometricComplex.MakeSchemaCompliant();
            CompositeCurve.MakeSchemaCompliant();
            CompositeSurface.MakeSchemaCompliant();
            CompositeSolid.MakeSchemaCompliant();
        }
    }
}