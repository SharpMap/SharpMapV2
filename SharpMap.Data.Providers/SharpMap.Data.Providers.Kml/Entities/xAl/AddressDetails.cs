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
using System.Xml.Serialization;

namespace SharpMap.Entities.xAl
{
    /// <remarks/>
    [Serializable]
    [XmlType(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    [XmlRoot(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0", IsNullable = false)]
    public class AddressDetails
    {
        private string addressDetailsKeyField;
        private string addressTypeField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private string codeField;

        private string currentStatusField;
        private object itemField;
        private AddressDetailsPostalServiceElements postalServiceElementsField;
        private string usageField;

        private string validFromDateField;

        private string validToDateField;

        /// <remarks/>
        public AddressDetailsPostalServiceElements PostalServiceElements
        {
            get { return postalServiceElementsField; }
            set { postalServiceElementsField = value; }
        }

        /// <remarks/>
        [XmlElement("Address", typeof (AddressDetailsAddress))]
        [XmlElement("AddressLines", typeof (AddressLinesType))]
        [XmlElement("AdministrativeArea", typeof (AdministrativeArea))]
        [XmlElement("Country", typeof (AddressDetailsCountry))]
        [XmlElement("Locality", typeof (Locality))]
        [XmlElement("Thoroughfare", typeof (Thoroughfare))]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlAnyElement]
        public XmlElement[] Any
        {
            get { return anyField; }
            set { anyField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string AddressType
        {
            get { return addressTypeField; }
            set { addressTypeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string CurrentStatus
        {
            get { return currentStatusField; }
            set { currentStatusField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string ValidFromDate
        {
            get { return validFromDateField; }
            set { validFromDateField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string ValidToDate
        {
            get { return validToDateField; }
            set { validToDateField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Usage
        {
            get { return usageField; }
            set { usageField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string AddressDetailsKey
        {
            get { return addressDetailsKeyField; }
            set { addressDetailsKeyField = value; }
        }

        /// <remarks/>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }
    }
}