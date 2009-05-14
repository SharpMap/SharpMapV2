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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlRoot(ElementName = "AddressDetails", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    [XmlType(TypeName = "AddressDetails", Namespace = Declarations.SchemaVersion)]
    public class AddressDetails
    {
        [XmlIgnore] private Address __Address;
        [XmlIgnore] private string __AddressDetailsKey;
        [XmlIgnore] private AddressLinesType __AddressLines;
        [XmlIgnore] private string __AddressType;
        [XmlIgnore] private AdministrativeArea __AdministrativeArea;
        [XmlIgnore] private string __Code;
        [XmlIgnore] private Country __Country;

        [XmlIgnore] private string __CurrentStatus;
        [XmlIgnore] private Locality __Locality;
        [XmlIgnore] private PostalServiceElements __PostalServiceElements;
        [XmlIgnore] private Thoroughfare __Thoroughfare;
        [XmlIgnore] private string __Usage;

        [XmlIgnore] private string __ValidFromDate;

        [XmlIgnore] private string __ValidToDate;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "AddressType")]
        public string AddressType
        {
            get { return __AddressType; }
            set { __AddressType = value; }
        }

        [XmlAttribute(AttributeName = "CurrentStatus")]
        public string CurrentStatus
        {
            get { return __CurrentStatus; }
            set { __CurrentStatus = value; }
        }

        [XmlAttribute(AttributeName = "ValidFromDate")]
        public string ValidFromDate
        {
            get { return __ValidFromDate; }
            set { __ValidFromDate = value; }
        }

        [XmlAttribute(AttributeName = "ValidToDate")]
        public string ValidToDate
        {
            get { return __ValidToDate; }
            set { __ValidToDate = value; }
        }

        [XmlAttribute(AttributeName = "Usage")]
        public string Usage
        {
            get { return __Usage; }
            set { __Usage = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return __Code; }
            set { __Code = value; }
        }

        [XmlAttribute(AttributeName = "AddressDetailsKey")]
        public string AddressDetailsKey
        {
            get { return __AddressDetailsKey; }
            set { __AddressDetailsKey = value; }
        }

        [XmlElement(Type = typeof (PostalServiceElements), ElementName = "PostalServiceElements", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalServiceElements PostalServiceElements
        {
            get
            {
                if (__PostalServiceElements == null) __PostalServiceElements = new PostalServiceElements();
                return __PostalServiceElements;
            }
            set { __PostalServiceElements = value; }
        }

        [XmlElement(Type = typeof (Address), ElementName = "Address", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Address Address
        {
            get
            {
                if (__Address == null) __Address = new Address();
                return __Address;
            }
            set { __Address = value; }
        }

        [XmlElement(Type = typeof (AddressLinesType), ElementName = "AddressLines", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLinesType AddressLines
        {
            get
            {
                if (__AddressLines == null) __AddressLines = new AddressLinesType();
                return __AddressLines;
            }
            set { __AddressLines = value; }
        }

        [XmlElement(Type = typeof (Country), ElementName = "Country", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Country Country
        {
            get
            {
                if (__Country == null) __Country = new Country();
                return __Country;
            }
            set { __Country = value; }
        }

        [XmlElement(Type = typeof (AdministrativeArea), ElementName = "AdministrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AdministrativeArea AdministrativeArea
        {
            get
            {
                if (__AdministrativeArea == null) __AdministrativeArea = new AdministrativeArea();
                return __AdministrativeArea;
            }
            set { __AdministrativeArea = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get
            {
                if (__Locality == null) __Locality = new Locality();
                return __Locality;
            }
            set { __Locality = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get
            {
                if (__Thoroughfare == null) __Thoroughfare = new Thoroughfare();
                return __Thoroughfare;
            }
            set { __Thoroughfare = value; }
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