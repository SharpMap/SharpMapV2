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
    [XmlRoot(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0", IsNullable = false)]
    public class PostBox
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private FirmType firmField;
        private string indicatorField;
        private PostalCode postalCodeField;
        private PostBoxPostBoxNumberExtension postBoxNumberExtensionField;

        private PostBoxPostBoxNumber postBoxNumberField;

        private PostBoxPostBoxNumberPrefix postBoxNumberPrefixField;

        private PostBoxPostBoxNumberSuffix postBoxNumberSuffixField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumber PostBoxNumber
        {
            get { return postBoxNumberField; }
            set { postBoxNumberField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumberPrefix PostBoxNumberPrefix
        {
            get { return postBoxNumberPrefixField; }
            set { postBoxNumberPrefixField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumberSuffix PostBoxNumberSuffix
        {
            get { return postBoxNumberSuffixField; }
            set { postBoxNumberSuffixField = value; }
        }

        /// <remarks/>
        public PostBoxPostBoxNumberExtension PostBoxNumberExtension
        {
            get { return postBoxNumberExtensionField; }
            set { postBoxNumberExtensionField = value; }
        }

        /// <remarks/>
        public FirmType Firm
        {
            get { return firmField; }
            set { firmField = value; }
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