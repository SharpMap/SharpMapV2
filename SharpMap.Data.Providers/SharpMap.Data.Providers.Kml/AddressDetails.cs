using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
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