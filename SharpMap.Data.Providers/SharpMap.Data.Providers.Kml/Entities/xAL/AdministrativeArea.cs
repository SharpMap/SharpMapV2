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
    [XmlRoot(ElementName = "AdministrativeArea", Namespace = Declarations.SchemaVersion, IsNullable = false),
     Serializable]
    public class AdministrativeArea
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private List<AdministrativeAreaName> _administrativeAreaName;
        [XmlIgnore] private string _indicator;
        [XmlIgnore] private Locality _locality;
        [XmlIgnore] private PostalCode _postalCode;
        [XmlIgnore] private PostOffice _postOffice;
        [XmlIgnore] private SubAdministrativeArea _subAdministrativeArea;
        [XmlIgnore] private string _type;

        [XmlIgnore] private string _usageType;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "UsageType")]
        public string UsageType
        {
            get { return _usageType; }
            set { _usageType = value; }
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

        [XmlElement(Type = typeof (AdministrativeAreaName), ElementName = "AdministrativeAreaName", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AdministrativeAreaName> AdministrativeAreaName
        {
            get
            {
                if (_administrativeAreaName == null) _administrativeAreaName = new List<AdministrativeAreaName>();
                return _administrativeAreaName;
            }
            set { _administrativeAreaName = value; }
        }

        [XmlElement(Type = typeof (SubAdministrativeArea), ElementName = "SubAdministrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SubAdministrativeArea SubAdministrativeArea
        {
            get { return _subAdministrativeArea; }
            set { _subAdministrativeArea = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }

        [XmlElement(Type = typeof (PostOffice), ElementName = "PostOffice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostOffice PostOffice
        {
            get { return _postOffice; }
            set { _postOffice = value; }
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
            Locality.MakeSchemaCompliant();
            PostOffice.MakeSchemaCompliant();
            PostalCode.MakeSchemaCompliant();
        }
    }
}