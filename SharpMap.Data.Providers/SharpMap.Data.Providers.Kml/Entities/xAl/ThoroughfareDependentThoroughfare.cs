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