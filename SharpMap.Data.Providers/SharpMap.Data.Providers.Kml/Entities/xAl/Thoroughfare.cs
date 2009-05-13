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
    public class Thoroughfare
    {
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;
        private ThoroughfareDependentThoroughfare dependentThoroughfareField;
        private string dependentThoroughfaresConnectorField;
        private ThoroughfareDependentThoroughfares dependentThoroughfaresField;

        private bool dependentThoroughfaresFieldSpecified;

        private string dependentThoroughfaresIndicatorField;
        private string dependentThoroughfaresTypeField;
        private object itemField;

        private object[] itemsField;

        private ThoroughfareNumberPrefix[] thoroughfareNumberPrefixField;

        private ThoroughfareNumberSuffix[] thoroughfareNumberSuffixField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public xAlTypedElementBase[] AddressLine { get; set; }

        /// <remarks/>
        [XmlElement("ThoroughfareNumber", typeof (ThoroughfareNumber))]
        [XmlElement("ThoroughfareNumberRange", typeof (ThoroughfareThoroughfareNumberRange))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberPrefix")]
        public ThoroughfareNumberPrefix[] ThoroughfareNumberPrefix
        {
            get { return thoroughfareNumberPrefixField; }
            set { thoroughfareNumberPrefixField = value; }
        }

        /// <remarks/>
        [XmlElement("ThoroughfareNumberSuffix")]
        public ThoroughfareNumberSuffix[] ThoroughfareNumberSuffix
        {
            get { return thoroughfareNumberSuffixField; }
            set { thoroughfareNumberSuffixField = value; }
        }

        /// <remarks/>
        public xAlTypedElementBase ThoroughfarePreDirection { get; set; }

        /// <remarks/>
        public xAlTypedElementBase ThoroughfareLeadingType { get; set; }

        /// <remarks/>
        [XmlElement("ThoroughfareName")]
        public xAlTypedElementBase[] ThoroughfareName { get; set; }

        /// <remarks/>
        public xAlTypedElementBase ThoroughfareTrailingType { get; set; }

        /// <remarks/>
        public xAlTypedElementBase ThoroughfarePostDirection { get; set; }

        /// <remarks/>
        public ThoroughfareDependentThoroughfare DependentThoroughfare
        {
            get { return dependentThoroughfareField; }
            set { dependentThoroughfareField = value; }
        }

        /// <remarks/>
        [XmlElement("DependentLocality", typeof (DependentLocalityType))]
        [XmlElement("Firm", typeof (FirmType))]
        [XmlElement("PostalCode", typeof (PostalCode))]
        [XmlElement("Premise", typeof (Premise))]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
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
        public ThoroughfareDependentThoroughfares DependentThoroughfares
        {
            get { return dependentThoroughfaresField; }
            set { dependentThoroughfaresField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool DependentThoroughfaresSpecified
        {
            get { return dependentThoroughfaresFieldSpecified; }
            set { dependentThoroughfaresFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string DependentThoroughfaresIndicator
        {
            get { return dependentThoroughfaresIndicatorField; }
            set { dependentThoroughfaresIndicatorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string DependentThoroughfaresConnector
        {
            get { return dependentThoroughfaresConnectorField; }
            set { dependentThoroughfaresConnectorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string DependentThoroughfaresType
        {
            get { return dependentThoroughfaresTypeField; }
            set { dependentThoroughfaresTypeField = value; }
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