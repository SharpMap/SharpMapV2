// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAl
{
    /// <remarks/>
    [Serializable]
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