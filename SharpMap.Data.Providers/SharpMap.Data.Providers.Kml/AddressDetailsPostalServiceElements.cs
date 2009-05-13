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
    public class AddressDetailsPostalServiceElements
    {
        private AddressDetailsPostalServiceElementsAddressIdentifier[] addressIdentifierField;
        private AddressDetailsPostalServiceElementsAddressLatitudeDirection addressLatitudeDirectionField;
        private AddressDetailsPostalServiceElementsAddressLatitude addressLatitudeField;
        private AddressDetailsPostalServiceElementsAddressLongitudeDirection addressLongitudeDirectionField;
        private AddressDetailsPostalServiceElementsAddressLongitude addressLongitudeField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private AddressDetailsPostalServiceElementsBarcode barcodeField;

        private AddressDetailsPostalServiceElementsEndorsementLineCode endorsementLineCodeField;

        private AddressDetailsPostalServiceElementsKeyLineCode keyLineCodeField;

        private AddressDetailsPostalServiceElementsSortingCode sortingCodeField;

        private AddressDetailsPostalServiceElementsSupplementaryPostalServiceData[] supplementaryPostalServiceDataField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressIdentifier")]
        public AddressDetailsPostalServiceElementsAddressIdentifier[] AddressIdentifier
        {
            get { return addressIdentifierField; }
            set { addressIdentifierField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsEndorsementLineCode EndorsementLineCode
        {
            get { return endorsementLineCodeField; }
            set { endorsementLineCodeField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsKeyLineCode KeyLineCode
        {
            get { return keyLineCodeField; }
            set { keyLineCodeField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsBarcode Barcode
        {
            get { return barcodeField; }
            set { barcodeField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsSortingCode SortingCode
        {
            get { return sortingCodeField; }
            set { sortingCodeField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsAddressLatitude AddressLatitude
        {
            get { return addressLatitudeField; }
            set { addressLatitudeField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsAddressLatitudeDirection AddressLatitudeDirection
        {
            get { return addressLatitudeDirectionField; }
            set { addressLatitudeDirectionField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsAddressLongitude AddressLongitude
        {
            get { return addressLongitudeField; }
            set { addressLongitudeField = value; }
        }

        /// <remarks/>
        public AddressDetailsPostalServiceElementsAddressLongitudeDirection AddressLongitudeDirection
        {
            get { return addressLongitudeDirectionField; }
            set { addressLongitudeDirectionField = value; }
        }

        /// <remarks/>
        [XmlElement("SupplementaryPostalServiceData")]
        public AddressDetailsPostalServiceElementsSupplementaryPostalServiceData[] SupplementaryPostalServiceData
        {
            get { return supplementaryPostalServiceDataField; }
            set { supplementaryPostalServiceDataField = value; }
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