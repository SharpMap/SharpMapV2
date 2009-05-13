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
    public class PostOffice
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private string indicatorField;

        private object[] itemsField;
        private PostalCode postalCodeField;

        private PostalRouteType postalRouteField;

        private PostBox postBoxField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("PostOfficeName", typeof (PostOfficePostOfficeName))]
        [XmlElement("PostOfficeNumber", typeof (PostOfficePostOfficeNumber))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        public PostalRouteType PostalRoute
        {
            get { return postalRouteField; }
            set { postalRouteField = value; }
        }

        /// <remarks/>
        public PostBox PostBox
        {
            get { return postBoxField; }
            set { postBoxField = value; }
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