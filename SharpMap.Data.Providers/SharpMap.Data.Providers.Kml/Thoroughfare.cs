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
    public class Thoroughfare
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private ThoroughfareDependentThoroughfare dependentThoroughfareField;
        private string dependentThoroughfaresConnectorField;
        private ThoroughfareDependentThoroughfares dependentThoroughfaresField;

        private bool dependentThoroughfaresFieldSpecified;

        private string dependentThoroughfaresIndicatorField;
        private string dependentThoroughfaresTypeField;
        private object itemField;

        private object[] itemsField;
        private ThoroughfareLeadingTypeType thoroughfareLeadingTypeField;

        private ThoroughfareNameType[] thoroughfareNameField;

        private ThoroughfareNumberPrefix[] thoroughfareNumberPrefixField;

        private ThoroughfareNumberSuffix[] thoroughfareNumberSuffixField;
        private ThoroughfarePostDirectionType thoroughfarePostDirectionField;

        private ThoroughfarePreDirectionType thoroughfarePreDirectionField;

        private ThoroughfareTrailingTypeType thoroughfareTrailingTypeField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumber", typeof (ThoroughfareNumber))]
        [XmlElement("ThoroughfareNumberRange", typeof (ThoroughfareThoroughfareNumberRange))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberPrefix")]
        public ThoroughfareNumberPrefix[] ThoroughfareNumberPrefix
        {
            get { return thoroughfareNumberPrefixField; }
            set { thoroughfareNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberSuffix")]
        public ThoroughfareNumberSuffix[] ThoroughfareNumberSuffix
        {
            get { return thoroughfareNumberSuffixField; }
            set { thoroughfareNumberSuffixField = value; }
        }

        /// <remarks/>
        public ThoroughfarePreDirectionType ThoroughfarePreDirection
        {
            get { return thoroughfarePreDirectionField; }
            set { thoroughfarePreDirectionField = value; }
        }

        /// <remarks/>
        public ThoroughfareLeadingTypeType ThoroughfareLeadingType
        {
            get { return thoroughfareLeadingTypeField; }
            set { thoroughfareLeadingTypeField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareName")]
        public ThoroughfareNameType[] ThoroughfareName
        {
            get { return thoroughfareNameField; }
            set { thoroughfareNameField = value; }
        }

        /// <remarks/>
        public ThoroughfareTrailingTypeType ThoroughfareTrailingType
        {
            get { return thoroughfareTrailingTypeField; }
            set { thoroughfareTrailingTypeField = value; }
        }

        /// <remarks/>
        public ThoroughfarePostDirectionType ThoroughfarePostDirection
        {
            get { return thoroughfarePostDirectionField; }
            set { thoroughfarePostDirectionField = value; }
        }

        /// <remarks/>
        public ThoroughfareDependentThoroughfare DependentThoroughfare
        {
            get { return dependentThoroughfareField; }
            set { dependentThoroughfareField = value; }
        }

        /// <remarks/>
        [XmlElement("DependentLocality", typeof (DependentLocalityType))]
        [XmlElement("Firm", typeof (FirmType))]
        [XmlElement("PostalCode", typeof (PostalCode))]
        [XmlElement("Premise", typeof (Premise))]
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
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public ThoroughfareDependentThoroughfares DependentThoroughfares
        {
            get { return dependentThoroughfaresField; }
            set { dependentThoroughfaresField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool DependentThoroughfaresSpecified
        {
            get { return dependentThoroughfaresFieldSpecified; }
            set { dependentThoroughfaresFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string DependentThoroughfaresIndicator
        {
            get { return dependentThoroughfaresIndicatorField; }
            set { dependentThoroughfaresIndicatorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string DependentThoroughfaresConnector
        {
            get { return dependentThoroughfaresConnectorField; }
            set { dependentThoroughfaresConnectorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string DependentThoroughfaresType
        {
            get { return dependentThoroughfaresTypeField; }
            set { dependentThoroughfaresTypeField = value; }
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