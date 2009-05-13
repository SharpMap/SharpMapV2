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