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
    [XmlRoot("kml", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class KmlType
    {
        private string hintField;
        private AbstractFeatureType itemField;

        private AbstractObjectType[] kmlObjectExtensionGroupField;
        private string[] kmlSimpleExtensionGroupField;
        private NetworkLinkControlType networkLinkControlField;

        /// <remarks/>
        public NetworkLinkControlType NetworkLinkControl
        {
            get { return networkLinkControlField; }
            set { networkLinkControlField = value; }
        }

        /// <remarks/>
        [XmlElement("NetworkLink", typeof (NetworkLinkType))]
        [XmlElement("Placemark", typeof (PlacemarkType))]
        public AbstractFeatureType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("KmlSimpleExtensionGroup")]
        public string[] KmlSimpleExtensionGroup
        {
            get { return kmlSimpleExtensionGroupField; }
            set { kmlSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("KmlObjectExtensionGroup")]
        public AbstractObjectType[] KmlObjectExtensionGroup
        {
            get { return kmlObjectExtensionGroupField; }
            set { kmlObjectExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string hint
        {
            get { return hintField; }
            set { hintField = value; }
        }
    }
}