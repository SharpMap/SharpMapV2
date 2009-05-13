using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("NetworkLink", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class NetworkLinkType : AbstractFeatureType
    {
        private bool flyToViewField;

        private bool flyToViewFieldSpecified;

        private ItemChoiceType1 itemElementNameField;
        private LinkType itemField;

        private AbstractObjectType[] networkLinkObjectExtensionGroupField;
        private string[] networkLinkSimpleExtensionGroupField;
        private bool refreshVisibilityField;

        private bool refreshVisibilityFieldSpecified;

        public NetworkLinkType()
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
        public ItemChoiceType1 ItemElementName
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
        public AbstractObjectType[] NetworkLinkObjectExtensionGroup
        {
            get { return networkLinkObjectExtensionGroupField; }
            set { networkLinkObjectExtensionGroupField = value; }
        }
    }
}