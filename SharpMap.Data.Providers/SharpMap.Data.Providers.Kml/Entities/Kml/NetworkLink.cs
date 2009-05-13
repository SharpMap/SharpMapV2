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

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [Serializable]
    [XmlType(TypeName = "NetworkLinkType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("NetworkLink", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class NetworkLink : FeatureBase
    {
        private bool flyToViewField;

        private bool flyToViewFieldSpecified;

        private UrlType itemElementNameField;
        private LinkType itemField;

        private KmlObjectBase[] networkLinkObjectExtensionGroupField;
        private string[] networkLinkSimpleExtensionGroupField;
        private bool refreshVisibilityField;

        private bool refreshVisibilityFieldSpecified;

        public NetworkLink()
        {
            refreshVisibilityField = false;
            flyToViewField = false;
        }

        /// <remarks/>
        public bool refreshVisibility
        {
            get { return refreshVisibilityField; }
            set { refreshVisibilityField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool refreshVisibilitySpecified
        {
            get { return refreshVisibilityFieldSpecified; }
            set { refreshVisibilityFieldSpecified = value; }
        }

        /// <remarks/>
        public bool flyToView
        {
            get { return flyToViewField; }
            set { flyToViewField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool flyToViewSpecified
        {
            get { return flyToViewFieldSpecified; }
            set { flyToViewFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("Link", typeof (LinkType))]
        [XmlElement("Url", typeof (LinkType))]
        [XmlChoiceIdentifier("ItemElementName")]
        public LinkType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public UrlType ItemElementName
        {
            get { return itemElementNameField; }
            set { itemElementNameField = value; }
        }

        /// <remarks/>
        [XmlElement("NetworkLinkSimpleExtensionGroup")]
        public string[] NetworkLinkSimpleExtensionGroup
        {
            get { return networkLinkSimpleExtensionGroupField; }
            set { networkLinkSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("NetworkLinkObjectExtensionGroup")]
        public KmlObjectBase[] NetworkLinkObjectExtensionGroup
        {
            get { return networkLinkObjectExtensionGroupField; }
            set { networkLinkObjectExtensionGroupField = value; }
        }
    }
}