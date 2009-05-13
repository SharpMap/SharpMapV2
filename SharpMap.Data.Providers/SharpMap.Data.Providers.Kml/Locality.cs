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
    public class Locality
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private DependentLocalityType dependentLocalityField;
        private string indicatorField;
        private object itemField;
        private LocalityLocalityName[] localityNameField;

        private PostalCode postalCodeField;
        private Premise premiseField;
        private Thoroughfare thoroughfareField;

        private string typeField;

        private string usageTypeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("LocalityName")]
        public LocalityLocalityName[] LocalityName
        {
            get { return localityNameField; }
            set { localityNameField = value; }
        }

        /// <remarks/>
        [XmlElement("LargeMailUser", typeof (LargeMailUserType))]
        [XmlElement("PostBox", typeof (PostBox))]
        [XmlElement("PostOffice", typeof (PostOffice))]
        [XmlElement("PostalRoute", typeof (PostalRouteType))]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public Thoroughfare Thoroughfare
        {
            get { return thoroughfareField; }
            set { thoroughfareField = value; }
        }

        /// <remarks/>
        public Premise Premise
        {
            get { return premiseField; }
            set { premiseField = value; }
        }

        /// <remarks/>
        public DependentLocalityType DependentLocality
        {
            get { return dependentLocalityField; }
            set { dependentLocalityField = value; }
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
        public string UsageType
        {
            get { return usageTypeField; }
            set { usageTypeField = value; }
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