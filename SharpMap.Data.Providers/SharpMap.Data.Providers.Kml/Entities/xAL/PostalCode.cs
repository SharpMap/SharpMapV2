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
    [XmlRoot(ElementName = "PostalCode", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    public class PostalCode
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<PostalCodeNumber> _postalCodeNumber;
        [XmlIgnore] private List<PostalCodeNumberExtension> _postalCodeNumberExtension;
        [XmlIgnore] private PostTown _postTown;
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

        [XmlElement(Type = typeof (PostalCodeNumber), ElementName = "PostalCodeNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalCodeNumber> PostalCodeNumber
        {
            get
            {
                if (_postalCodeNumber == null) _postalCodeNumber = new List<PostalCodeNumber>();
                return _postalCodeNumber;
            }
            set { _postalCodeNumber = value; }
        }

        [XmlElement(Type = typeof (PostalCodeNumberExtension), ElementName = "PostalCodeNumberExtension",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalCodeNumberExtension> PostalCodeNumberExtension
        {
            get
            {
                if (_postalCodeNumberExtension == null)
                    _postalCodeNumberExtension = new List<PostalCodeNumberExtension>();
                return _postalCodeNumberExtension;
            }
            set { _postalCodeNumberExtension = value; }
        }

        [XmlElement(Type = typeof (PostTown), ElementName = "PostTown", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostTown PostTown
        {
            get { return _postTown; }
            set { _postTown = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}