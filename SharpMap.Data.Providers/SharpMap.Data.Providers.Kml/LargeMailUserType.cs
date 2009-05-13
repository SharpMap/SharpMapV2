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
    public class LargeMailUserType
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private BuildingNameType[] buildingNameField;

        private Department departmentField;
        private LargeMailUserTypeLargeMailUserIdentifier largeMailUserIdentifierField;
        private LargeMailUserTypeLargeMailUserName[] largeMailUserNameField;
        private PostalCode postalCodeField;

        private PostBox postBoxField;

        private Thoroughfare thoroughfareField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("LargeMailUserName")]
        public LargeMailUserTypeLargeMailUserName[] LargeMailUserName
        {
            get { return largeMailUserNameField; }
            set { largeMailUserNameField = value; }
        }

        /// <remarks/>
        public LargeMailUserTypeLargeMailUserIdentifier LargeMailUserIdentifier
        {
            get { return largeMailUserIdentifierField; }
            set { largeMailUserIdentifierField = value; }
        }

        /// <remarks/>
        [XmlElement("BuildingName")]
        public BuildingNameType[] BuildingName
        {
            get { return buildingNameField; }
            set { buildingNameField = value; }
        }

        /// <remarks/>
        public Department Department
        {
            get { return departmentField; }
            set { departmentField = value; }
        }

        /// <remarks/>
        public PostBox PostBox
        {
            get { return postBoxField; }
            set { postBoxField = value; }
        }

        /// <remarks/>
        public Thoroughfare Thoroughfare
        {
            get { return thoroughfareField; }
            set { thoroughfareField = value; }
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
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }
    }
}