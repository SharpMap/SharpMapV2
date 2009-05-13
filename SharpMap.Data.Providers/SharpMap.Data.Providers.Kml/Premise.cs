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
    public class Premise
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private BuildingNameType[] buildingNameField;

        private object[] items1Field;
        private object[] itemsField;

        private MailStopType mailStopField;

        private PostalCode postalCodeField;

        private Premise premise1Field;

        private string premiseDependencyField;

        private string premiseDependencyTypeField;
        private PremisePremiseName[] premiseNameField;
        private PremiseNumberPrefix[] premiseNumberPrefixField;

        private PremiseNumberSuffix[] premiseNumberSuffixField;

        private string premiseThoroughfareConnectorField;
        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseName")]
        public PremisePremiseName[] PremiseName
        {
            get { return premiseNameField; }
            set { premiseNameField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseLocation", typeof (PremisePremiseLocation))]
        [XmlElement("PremiseNumber", typeof (PremiseNumber))]
        [XmlElement("PremiseNumberRange", typeof (PremisePremiseNumberRange))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseNumberPrefix")]
        public PremiseNumberPrefix[] PremiseNumberPrefix
        {
            get { return premiseNumberPrefixField; }
            set { premiseNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("PremiseNumberSuffix")]
        public PremiseNumberSuffix[] PremiseNumberSuffix
        {
            get { return premiseNumberSuffixField; }
            set { premiseNumberSuffixField = value; }
        }

        /// <remarks/>
        [XmlElement("BuildingName")]
        public BuildingNameType[] BuildingName
        {
            get { return buildingNameField; }
            set { buildingNameField = value; }
        }

        /// <remarks/>
        [XmlElement("Firm", typeof (FirmType))]
        [XmlElement("SubPremise", typeof (SubPremiseType))]
        public object[] Items1
        {
            get { return items1Field; }
            set { items1Field = value; }
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
        [XmlElement("Premise")]
        public Premise Premise1
        {
            get { return premise1Field; }
            set { premise1Field = value; }
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
        public string PremiseDependency
        {
            get { return premiseDependencyField; }
            set { premiseDependencyField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string PremiseDependencyType
        {
            get { return premiseDependencyTypeField; }
            set { premiseDependencyTypeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string PremiseThoroughfareConnector
        {
            get { return premiseThoroughfareConnectorField; }
            set { premiseThoroughfareConnectorField = value; }
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