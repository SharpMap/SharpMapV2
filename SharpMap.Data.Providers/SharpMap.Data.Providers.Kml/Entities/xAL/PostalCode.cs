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
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private List<PostalCodeNumber> __PostalCodeNumber;
        [XmlIgnore] private List<PostalCodeNumberExtension> __PostalCodeNumberExtension;
        [XmlIgnore] private PostTown __PostTown;
        [XmlIgnore] private string __Type;
        [XmlAnyElement] public XmlElement[] Any;

        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (__AddressLine == null) __AddressLine = new List<AddressLine>();
                return __AddressLine;
            }
            set { __AddressLine = value; }
        }

        [XmlElement(Type = typeof (PostalCodeNumber), ElementName = "PostalCodeNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalCodeNumber> PostalCodeNumber
        {
            get
            {
                if (__PostalCodeNumber == null) __PostalCodeNumber = new List<PostalCodeNumber>();
                return __PostalCodeNumber;
            }
            set { __PostalCodeNumber = value; }
        }

        [XmlElement(Type = typeof (PostalCodeNumberExtension), ElementName = "PostalCodeNumberExtension",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PostalCodeNumberExtension> PostalCodeNumberExtension
        {
            get
            {
                if (__PostalCodeNumberExtension == null)
                    __PostalCodeNumberExtension = new List<PostalCodeNumberExtension>();
                return __PostalCodeNumberExtension;
            }
            set { __PostalCodeNumberExtension = value; }
        }

        [XmlElement(Type = typeof (PostTown), ElementName = "PostTown", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostTown PostTown
        {
            get
            {
                if (__PostTown == null) __PostTown = new PostTown();
                return __PostTown;
            }
            set { __PostTown = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}