// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "PostalRouteType", Namespace = Declarations.SchemaVersion), Serializable]
    public class PostalRouteType
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<PostalRouteName> _postalRouteName;
        [XmlIgnore] private PostalRouteNumber _postalRouteNumber;
        [XmlIgnore] private PostBox _postBox;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_addressLine == null) _addressLine = new List<AddressLine>();
                return _addressLine;
            }
            set { _addressLine = value; }
        }

        [XmlElement(Type = typeof (PostalRouteName), ElementName = "PostalRouteName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalRouteName> PostalRouteName
        {
            get
            {
                if (_postalRouteName == null) _postalRouteName = new List<PostalRouteName>();
                return _postalRouteName;
            }
            set { _postalRouteName = value; }
        }

        [XmlElement(Type = typeof (PostalRouteNumber), ElementName = "PostalRouteNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteNumber PostalRouteNumber
        {
            get { return _postalRouteNumber; }
            set { _postalRouteNumber = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get { return _postBox; }
            set { _postBox = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (PostalRouteName _c in PostalRouteName) _c.MakeSchemaCompliant();
            PostalRouteNumber.MakeSchemaCompliant();
        }
    }
}