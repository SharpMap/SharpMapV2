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
    public class PostalCodePostTown
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;

        private PostalCodePostTownPostTownName[] postTownNameField;

        private PostalCodePostTownPostTownSuffix postTownSuffixField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("PostTownName")]
        public PostalCodePostTownPostTownName[] PostTownName
        {
            get { return postTownNameField; }
            set { postTownNameField = value; }
        }

        /// <remarks/>
        public PostalCodePostTownPostTownSuffix PostTownSuffix
        {
            get { return postTownSuffixField; }
            set { postTownSuffixField = value; }
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