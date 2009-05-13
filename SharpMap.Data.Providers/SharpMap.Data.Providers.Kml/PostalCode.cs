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
    [XmlRoot(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0", IsNullable = false)]
    public class PostalCode
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private PostalCodePostalCodeNumberExtension[] postalCodeNumberExtensionField;
        private PostalCodePostalCodeNumber[] postalCodeNumberField;

        private PostalCodePostTown postTownField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("PostalCodeNumber")]
        public PostalCodePostalCodeNumber[] PostalCodeNumber
        {
            get { return postalCodeNumberField; }
            set { postalCodeNumberField = value; }
        }

        /// <remarks/>
        [XmlElement("PostalCodeNumberExtension")]
        public PostalCodePostalCodeNumberExtension[] PostalCodeNumberExtension
        {
            get { return postalCodeNumberExtensionField; }
            set { postalCodeNumberExtensionField = value; }
        }

        /// <remarks/>
        public PostalCodePostTown PostTown
        {
            get { return postTownField; }
            set { postTownField = value; }
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
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
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