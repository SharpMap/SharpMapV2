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
    public class Locality
    {
        private XmlAttribute[] anyAttrField;
        private XmlElement[] anyField;

        private DependentLocalityType dependentLocalityField;
        private string indicatorField;
        private object itemField;

        private PostalCode postalCodeField;
        private Premise premiseField;
        private Thoroughfare thoroughfareField;

        private string typeField;

        private string usageTypeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public xAlTypedElementBase[] AddressLine { get; set; }

        /// <remarks/>
        [XmlElement("LocalityName")]
        public xAlTypedElementBase[] LocalityName { get; set; }

        /// <remarks/>
        [XmlElement("LargeMailUser", typeof (LargeMailUserType))]
        [XmlElement("PostBox", typeof (PostBox))]
        [XmlElement("PostOffice", typeof (PostOffice))]
        [XmlElement("PostalRoute", typeof (PostalRouteType))]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        public Thoroughfare Thoroughfare
        {
            get { return thoroughfareField; }
            set { thoroughfareField = value; }
        }

        /// <remarks/>
        public Premise Premise
        {
            get { return premiseField; }
            set { premiseField = value; }
        }

        /// <remarks/>
        public DependentLocalityType DependentLocality
        {
            get { return dependentLocalityField; }
            set { dependentLocalityField = value; }
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
        public string UsageType
        {
            get { return usageTypeField; }
            set { usageTypeField = value; }
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