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
        private xAlTypedElementBase addressLatitudeDirectionField;
        private xAlTypedElementBase addressLatitudeField;
        private xAlTypedElementBase addressLongitudeDirectionField;
        private xAlTypedElementBase addressLongitudeField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private xAlTypedElementBase barcodeField;

        private xAlTypedElementBase endorsementLineCodeField;

        private xAlTypedElementBase keyLineCodeField;

        private AddressDetailsPostalServiceElementsSortingCode sortingCodeField;

        private xAlTypedElementBase[] supplementaryPostalServiceDataField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressIdentifier")]
        public AddressDetailsPostalServiceElementsAddressIdentifier[] AddressIdentifier
        {
            get { return addressIdentifierField; }
            set { addressIdentifierField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase EndorsementLineCode
        {
            get { return endorsementLineCodeField; }
            set { endorsementLineCodeField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase KeyLineCode
        {
            get { return keyLineCodeField; }
            set { keyLineCodeField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase Barcode
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
        public xAlTypedElementBase AddressLatitude
        {
            get { return addressLatitudeField; }
            set { addressLatitudeField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase AddressLatitudeDirection
        {
            get { return addressLatitudeDirectionField; }
            set { addressLatitudeDirectionField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase AddressLongitude
        {
            get { return addressLongitudeField; }
            set { addressLongitudeField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase AddressLongitudeDirection
        {
            get { return addressLongitudeDirectionField; }
            set { addressLongitudeDirectionField = value; }
        }

        /// <remarks/>
        [XmlElement("SupplementaryPostalServiceData")]
        public xAlTypedElementBase[] SupplementaryPostalServiceData
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