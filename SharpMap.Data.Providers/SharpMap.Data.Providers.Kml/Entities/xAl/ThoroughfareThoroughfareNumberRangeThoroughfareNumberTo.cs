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
using System.Xml.Serialization;

namespace SharpMap.Entities.xAl
{
    /// <remarks/>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class ThoroughfareThoroughfareNumberRangeThoroughfareNumberTo : xAlElementBase
    {
        private ThoroughfareNumber[] thoroughfareNumberField;
        private ThoroughfareNumberPrefix[] thoroughfareNumberPrefixField;

        private ThoroughfareNumberSuffix[] thoroughfareNumberSuffixField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public xAlTypedElementBase[] AddressLine { get; set; }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberPrefix")]
        public ThoroughfareNumberPrefix[] ThoroughfareNumberPrefix
        {
            get { return thoroughfareNumberPrefixField; }
            set { thoroughfareNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumber")]
        public ThoroughfareNumber[] ThoroughfareNumber
        {
            get { return thoroughfareNumberField; }
            set { thoroughfareNumberField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberSuffix")]
        public ThoroughfareNumberSuffix[] ThoroughfareNumberSuffix
        {
            get { return thoroughfareNumberSuffixField; }
            set { thoroughfareNumberSuffixField = value; }
        }
    }
}