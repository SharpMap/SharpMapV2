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
        [XmlIgnore] private Address _Address;
        [XmlIgnore] private string _AddressDetailsKey;
        [XmlIgnore] private AddressLinesType _AddressLines;
        [XmlIgnore] private string _AddressType;
        [XmlIgnore] private AdministrativeArea _AdministrativeArea;
        [XmlIgnore] private string _Code;
        [XmlIgnore] private Country _Country;

        [XmlIgnore] private string _CurrentStatus;
        [XmlIgnore] private Locality _Locality;
        [XmlIgnore] private PostalServiceElements _PostalServiceElements;
        [XmlIgnore] private Thoroughfare _Thoroughfare;
        [XmlIgnore] private string _Usage;

        [XmlIgnore] private string _ValidFromDate;

        [XmlIgnore] private string _ValidToDate;
        [XmlAnyElement] public XmlElement[] Any;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "AddressType")]
        public string AddressType
        {
            get { return _AddressType; }
            set { _AddressType = value; }
        }

        [XmlAttribute(AttributeName = "CurrentStatus")]
        public string CurrentStatus
        {
            get { return _CurrentStatus; }
            set { _CurrentStatus = value; }
        }

        [XmlAttribute(AttributeName = "ValidFromDate")]
        public string ValidFromDate
        {
            get { return _ValidFromDate; }
            set { _ValidFromDate = value; }
        }

        [XmlAttribute(AttributeName = "ValidToDate")]
        public string ValidToDate
        {
            get { return _ValidToDate; }
            set { _ValidToDate = value; }
        }

        [XmlAttribute(AttributeName = "Usage")]
        public string Usage
        {
            get { return _Usage; }
            set { _Usage = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        [XmlAttribute(AttributeName = "AddressDetailsKey")]
        public string AddressDetailsKey
        {
            get { return _AddressDetailsKey; }
            set { _AddressDetailsKey = value; }
        }

        [XmlElement(Type = typeof (PostalServiceElements), ElementName = "PostalServiceElements", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PostalServiceElements PostalServiceElements
        {
            get { return _PostalServiceElements; }
            set { _PostalServiceElements = value; }
        }

        [XmlElement(Type = typeof (Address), ElementName = "Address", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Address Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        [XmlElement(Type = typeof (AddressLinesType), ElementName = "AddressLines", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AddressLinesType AddressLines
        {
            get { return _AddressLines; }
            set { _AddressLines = value; }
        }

        [XmlElement(Type = typeof (Country), ElementName = "Country", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public Country Country
        {
            get { return _Country; }
            set { _Country = value; }
        }

        [XmlElement(Type = typeof (AdministrativeArea), ElementName = "AdministrativeArea", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AdministrativeArea AdministrativeArea
        {
            get { return _AdministrativeArea; }
            set { _AdministrativeArea = value; }
        }

        [XmlElement(Type = typeof (Locality), ElementName = "Locality", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Locality Locality
        {
            get { return _Locality; }
            set { _Locality = value; }
        }

        [XmlElement(Type = typeof (Thoroughfare), ElementName = "Thoroughfare", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Thoroughfare Thoroughfare
        {
            get { return _Thoroughfare; }
            set { _Thoroughfare = value; }
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