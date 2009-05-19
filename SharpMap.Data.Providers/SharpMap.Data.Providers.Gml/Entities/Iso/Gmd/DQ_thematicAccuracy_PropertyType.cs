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
using SharpMap.Entities.Ogc.Gml;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlInclude(typeof (DQ_nonQuantitativeAttributeAccuracy_Type)),
     XmlInclude(typeof (DQ_thematicClassificationCorrectness_Type)),
     XmlType(TypeName = "DQ_thematicAccuracy_PropertyType", Namespace = "http://www.isotc211.org/2005/gmd"),
     XmlInclude(typeof (DQ_quantitativeAttributeAccuracy_Type))]
    public class DQ_thematicAccuracy_PropertyType
    {
        [XmlIgnore] private AbstractDQ_thematicAccuracy _abstractDQ_ThematicAccuracy;
        [XmlIgnore] private Actuate _actuate;
        [XmlIgnore] private string _arcrole;
        [XmlIgnore] private string _href;
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] private string _role;
        [XmlIgnore] private Show _show;
        [XmlIgnore] private string _title;
        [XmlIgnore] private string _type;
        [XmlIgnore] private string _uuidref;
        [XmlIgnore] public bool ActuateSpecified;
        [XmlIgnore] public bool ShowSpecified;

        public DQ_thematicAccuracy_PropertyType()
        {
            Type = "simple";
        }

        [XmlElement(Type = typeof (AbstractDQ_thematicAccuracy), ElementName = "AbstractDQ_thematicAccuracy",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public AbstractDQ_thematicAccuracy AbstractDQ_thematicAccuracy
        {
            get { return _abstractDQ_ThematicAccuracy; }
            set { _abstractDQ_ThematicAccuracy = value; }
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

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
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

        [XmlAttribute(AttributeName = "uuidref", DataType = "string")]
        public string Uuidref
        {
            get { return _uuidref; }
            set { _uuidref = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            AbstractDQ_thematicAccuracy.MakeSchemaCompliant();
        }
    }
}