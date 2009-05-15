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
    [XmlRoot(ElementName = "PostOffice", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class PostOffice
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private string _indicator;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private PostalRouteType _postalRoute;
        [XmlIgnore] private PostBox _postBox;
        [XmlIgnore] private List<PostOfficeName> _postOfficeName;
        [XmlIgnore] private PostOfficeNumber _postOfficeNumber;
        [XmlIgnore] private string _type;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _indicator; }
            set { _indicator = value; }
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

        [XmlElement(Type = typeof (PostOfficeName), ElementName = "PostOfficeName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostOfficeName> PostOfficeName
        {
            get
            {
                if (_postOfficeName == null) _postOfficeName = new List<PostOfficeName>();
                return _postOfficeName;
            }
            set { _postOfficeName = value; }
        }

        [XmlElement(Type = typeof (PostOfficeNumber), ElementName = "PostOfficeNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOfficeNumber PostOfficeNumber
        {
            get { return _postOfficeNumber; }
            set { _postOfficeNumber = value; }
        }

        [XmlElement(Type = typeof (PostalRouteType), ElementName = "PostalRoute", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalRouteType PostalRoute
        {
            get { return _postalRoute; }
            set { _postalRoute = value; }
        }

        [XmlElement(Type = typeof (PostBox), ElementName = "PostBox", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public PostBox PostBox
        {
            get { return _postBox; }
            set { _postBox = value; }
        }

        [XmlElement(Type = typeof (PostalCode), ElementName = "PostalCode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalCode PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}