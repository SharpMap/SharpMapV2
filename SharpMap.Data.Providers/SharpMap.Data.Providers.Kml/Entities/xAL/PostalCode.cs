// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<PostalCodeNumber> _PostalCodeNumber;
        [XmlIgnore] private List<PostalCodeNumberExtension> _PostalCodeNumberExtension;
        [XmlIgnore] private PostTown _PostTown;
        [XmlIgnore] private string _Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_AddressLine == null) _AddressLine = new List<AddressLine>();
                return _AddressLine;
            }
            set { _AddressLine = value; }
        }

        [XmlElement(Type = typeof (PostalCodeNumber), ElementName = "PostalCodeNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalCodeNumber> PostalCodeNumber
        {
            get
            {
                if (_PostalCodeNumber == null) _PostalCodeNumber = new List<PostalCodeNumber>();
                return _PostalCodeNumber;
            }
            set { _PostalCodeNumber = value; }
        }

        [XmlElement(Type = typeof (PostalCodeNumberExtension), ElementName = "PostalCodeNumberExtension",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalCodeNumberExtension> PostalCodeNumberExtension
        {
            get
            {
                if (_PostalCodeNumberExtension == null)
                    _PostalCodeNumberExtension = new List<PostalCodeNumberExtension>();
                return _PostalCodeNumberExtension;
            }
            set { _PostalCodeNumberExtension = value; }
        }

        [XmlElement(Type = typeof (PostTown), ElementName = "PostTown", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostTown PostTown
        {
            get { return _PostTown; }
            set { _PostTown = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}