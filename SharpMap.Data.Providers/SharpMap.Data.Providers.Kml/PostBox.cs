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
    public class PostBox
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private FirmType firmField;
        private string indicatorField;
        private PostalCode postalCodeField;
        private PostBoxPostBoxNumberExtension postBoxNumberExtensionField;

        private PostBoxPostBoxNumber postBoxNumberField;

        private PostBoxPostBoxNumberPrefix postBoxNumberPrefixField;

        private PostBoxPostBoxNumberSuffix postBoxNumberSuffixField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumber PostBoxNumber
        {
            get { return postBoxNumberField; }
            set { postBoxNumberField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumberPrefix PostBoxNumberPrefix
        {
            get { return postBoxNumberPrefixField; }
            set { postBoxNumberPrefixField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumberSuffix PostBoxNumberSuffix
        {
            get { return postBoxNumberSuffixField; }
            set { postBoxNumberSuffixField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumberExtension PostBoxNumberExtension
        {
            get { return postBoxNumberExtensionField; }
            set { postBoxNumberExtensionField = value; }
        }

        /// <remarks/>
        public FirmType Firm
        {
            get { return firmField; }
            set { firmField = value; }
        }

        /// <remarks/>
        public PostalCode PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
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
        [XmlAttribute]
        public string Indicator
        {
            get { return indicatorField; }
            set { indicatorField = value; }
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