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
    [XmlType(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class PostalRouteType
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private object[] itemsField;

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
        [XmlElement("PostalRouteName", typeof (PostalRouteTypePostalRouteName))]
        [XmlElement("PostalRouteNumber", typeof (PostalRouteTypePostalRouteNumber))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        public PostBox PostBox
        {
            get { return postBoxField; }
            set { postBoxField = value; }
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