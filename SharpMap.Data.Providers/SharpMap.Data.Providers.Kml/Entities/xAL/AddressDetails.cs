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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlRoot(ElementName = "AddressDetails", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    [XmlType(TypeName = "AddressDetails", Namespace = Declarations.SchemaVersion)]
    public class AddressDetails
    {
        [XmlIgnore] private Address _address;
        [XmlIgnore] private string _addressDetailsKey;
        [XmlIgnore] private AddressLinesType _addressLines;
        [XmlIgnore] private string _addressType;
        [XmlIgnore] private AdministrativeArea _administrativeArea;
        [XmlIgnore] private string _code;
        [XmlIgnore] private Country _country;

        [XmlIgnore] private string _currentStatus;
        [XmlIgnore] private Locality _locality;
        [XmlIgnore] private PostalServiceElements _postalServiceElements;
        [XmlIgnore] private Thoroughfare _thoroughfare;
        [XmlIgnore] private string _usage;

        [XmlIgnore] private string _validFromDate;

        [XmlIgnore] private string _validToDate;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "AddressType")]
        public string AddressType
        {
            get { return _addressType; }
            set { _addressType = value; }
        }

        [XmlAttribute(AttributeName = "CurrentStatus")]
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { _currentStatus = value; }
        }

        [XmlAttribute(AttributeName = "ValidFromDate")]
        public string ValidFromDate
        {
            get { return _validFromDate; }
            set { _validFromDate = value; }
        }

        [XmlAttribute(AttributeName = "ValidToDate")]
        public string ValidToDate
        {
            get { return _validToDate; }
            set { _validToDate = value; }
        }

        [XmlAttribute(AttributeName = "Usage")]
        public string Usage
        {
            get { return _usage; }
            set { _usage = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [XmlAttribute(AttributeName = "AddressDetailsKey")]
        public string AddressDetailsKey
        {
            get { return _addressDetailsKey; }
            set { _addressDetailsKey = value; }
        }

        [XmlElement(Type = typeof (PostalServiceElements), ElementName = "PostalServiceElements", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalServiceElements PostalServiceElements
        {
            get { return _postalServiceElements; }
            set { _postalServiceElements = value; }
        }

        [XmlElement(Type = typeof (Address), ElementName = "Address", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        [XmlElement(Type = typeof (AddressLinesType), ElementName = "AddressLines", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLinesType AddressLines
        {
            get { return _addressLines; }
            set { _addressLines = value; }
        }

        [XmlElement(Type = typeof (Country), ElementName = "Country", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Country Country
        {
            get { return _country; }
            set { _country = value; }
        }

        [XmlElement(Type = typeof (AdministrativeArea), ElementName = "AdministrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AdministrativeArea AdministrativeArea
        {
            get { return _administrativeArea; }
            set { _administrativeArea = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _thoroughfare; }
            set { _thoroughfare = value; }
        }

        public void MakeSchemaCompliant()
        {
            Address.MakeSchemaCompliant();
            AddressLines.MakeSchemaCompliant();
            Country.MakeSchemaCompliant();
            AdministrativeArea.MakeSchemaCompliant();
            Locality.MakeSchemaCompliant();
            Thoroughfare.MakeSchemaCompliant();
        }
    }
}