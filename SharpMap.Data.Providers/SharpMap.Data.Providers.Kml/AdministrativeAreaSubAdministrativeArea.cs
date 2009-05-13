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
    public class AdministrativeAreaSubAdministrativeArea
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;

        private XmlElement[] anyField;
        private string indicatorField;
        private object itemField;
        private AdministrativeAreaSubAdministrativeAreaSubAdministrativeAreaName[] subAdministrativeAreaNameField;

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
        [XmlElement("SubAdministrativeAreaName")]
        public AdministrativeAreaSubAdministrativeAreaSubAdministrativeAreaName[] SubAdministrativeAreaName
        {
            get { return subAdministrativeAreaNameField; }
            set { subAdministrativeAreaNameField = value; }
        }

        /// <remarks/>
        [XmlElement("Locality", typeof (Locality))]
        [XmlElement("PostOffice", typeof (PostOffice))]
        [XmlElement("PostalCode", typeof (PostalCode))]
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