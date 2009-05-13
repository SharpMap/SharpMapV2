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
    public class SubPremiseType
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private BuildingNameType[] buildingNameField;

        private FirmType firmField;
        private object[] itemsField;

        private MailStopType mailStopField;

        private PostalCode postalCodeField;

        private SubPremiseType subPremiseField;
        private SubPremiseTypeSubPremiseName[] subPremiseNameField;
        private SubPremiseTypeSubPremiseNumberPrefix[] subPremiseNumberPrefixField;

        private SubPremiseTypeSubPremiseNumberSuffix[] subPremiseNumberSuffixField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("SubPremiseName")]
        public SubPremiseTypeSubPremiseName[] SubPremiseName
        {
            get { return subPremiseNameField; }
            set { subPremiseNameField = value; }
        }

        /// <remarks/>
        [XmlElement("SubPremiseLocation", typeof (SubPremiseTypeSubPremiseLocation))]
        [XmlElement("SubPremiseNumber", typeof (SubPremiseTypeSubPremiseNumber))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("SubPremiseNumberPrefix")]
        public SubPremiseTypeSubPremiseNumberPrefix[] SubPremiseNumberPrefix
        {
            get { return subPremiseNumberPrefixField; }
            set { subPremiseNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("SubPremiseNumberSuffix")]
        public SubPremiseTypeSubPremiseNumberSuffix[] SubPremiseNumberSuffix
        {
            get { return subPremiseNumberSuffixField; }
            set { subPremiseNumberSuffixField = value; }
        }

        /// <remarks/>
        [XmlElement("BuildingName")]
        public BuildingNameType[] BuildingName
        {
            get { return buildingNameField; }
            set { buildingNameField = value; }
        }

        /// <remarks/>
        public FirmType Firm
        {
            get { return firmField; }
            set { firmField = value; }
        }

        /// <remarks/>
        public MailStopType MailStop
        {
            get { return mailStopField; }
            set { mailStopField = value; }
        }

        /// <remarks/>
        public PostalCode PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public SubPremiseType SubPremise
        {
            get { return subPremiseField; }
            set { subPremiseField = value; }
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