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
    [XmlRoot("Document", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class DocumentType : AbstractContainerType
    {
        private AbstractObjectType[] documentObjectExtensionGroupField;
        private string[] documentSimpleExtensionGroupField;
        private AbstractFeatureType[] itemsField;
        private SchemaType[] schemaField;

        /// <remarks/>
        [XmlElement("Schema")]
        public SchemaType[] Schema
        {
            get { return schemaField; }
            set { schemaField = value; }
        }

        /// <remarks/>
        [XmlElement("NetworkLink", typeof (NetworkLinkType))]
        [XmlElement("Placemark", typeof (PlacemarkType))]
        public AbstractFeatureType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("DocumentSimpleExtensionGroup")]
        public string[] DocumentSimpleExtensionGroup
        {
            get { return documentSimpleExtensionGroupField; }
            set { documentSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("DocumentObjectExtensionGroup")]
        public AbstractObjectType[] DocumentObjectExtensionGroup
        {
            get { return documentObjectExtensionGroupField; }
            set { documentObjectExtensionGroupField = value; }
        }
    }
}