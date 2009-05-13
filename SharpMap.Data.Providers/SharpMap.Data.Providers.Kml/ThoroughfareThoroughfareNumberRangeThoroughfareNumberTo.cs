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
    public class ThoroughfareThoroughfareNumberRangeThoroughfareNumberTo
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private string codeField;
        private string[] textField;

        private ThoroughfareNumber[] thoroughfareNumberField;
        private ThoroughfareNumberPrefix[] thoroughfareNumberPrefixField;

        private ThoroughfareNumberSuffix[] thoroughfareNumberSuffixField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberPrefix")]
        public ThoroughfareNumberPrefix[] ThoroughfareNumberPrefix
        {
            get { return thoroughfareNumberPrefixField; }
            set { thoroughfareNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumber")]
        public ThoroughfareNumber[] ThoroughfareNumber
        {
            get { return thoroughfareNumberField; }
            set { thoroughfareNumberField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberSuffix")]
        public ThoroughfareNumberSuffix[] ThoroughfareNumberSuffix
        {
            get { return thoroughfareNumberSuffixField; }
            set { thoroughfareNumberSuffixField = value; }
        }

        /// <remarks/>
        [XmlText]
        public string[] Text
        {
            get { return textField; }
            set { textField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
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