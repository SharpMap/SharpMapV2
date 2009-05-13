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
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class AddressDetailsCountry
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private AddressDetailsCountryCountryNameCode[] countryNameCodeField;

        private CountryName[] countryNameField;

        private object itemField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("CountryNameCode")]
        public AddressDetailsCountryCountryNameCode[] CountryNameCode
        {
            get { return countryNameCodeField; }
            set { countryNameCodeField = value; }
        }

        /// <remarks/>
        [XmlElement("CountryName")]
        public CountryName[] CountryName
        {
            get { return countryNameField; }
            set { countryNameField = value; }
        }

        /// <remarks/>
        [XmlElement("AdministrativeArea", typeof (AdministrativeArea))]
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
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }
    }
}