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
    public class ThoroughfareDependentThoroughfare
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private ThoroughfareLeadingTypeType thoroughfareLeadingTypeField;

        private ThoroughfareNameType[] thoroughfareNameField;

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