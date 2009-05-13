using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class PremisePremiseNumberRangePremiseNumberRangeTo
    {
        private AddressLine[] addressLineField;

        private PremiseNumber[] premiseNumberField;
        private PremiseNumberPrefix[] premiseNumberPrefixField;

        private PremiseNumberSuffix[] premiseNumberSuffixField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseNumberPrefix")]
        public PremiseNumberPrefix[] PremiseNumberPrefix
        {
            get { return premiseNumberPrefixField; }
            set { premiseNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseNumber")]
        public PremiseNumber[] PremiseNumber
        {
            get { return premiseNumberField; }
            set { premiseNumberField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseNumberSuffix")]
        public PremiseNumberSuffix[] PremiseNumberSuffix
        {
            get { return premiseNumberSuffixField; }
            set { premiseNumberSuffixField = value; }
        }
    }
}