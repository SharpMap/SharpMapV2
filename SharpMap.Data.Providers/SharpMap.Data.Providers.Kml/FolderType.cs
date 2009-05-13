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
    [XmlRoot("Folder", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class FolderType : AbstractContainerType
    {
        private AbstractObjectType[] folderObjectExtensionGroupField;
        private string[] folderSimpleExtensionGroupField;
        private AbstractFeatureType[] itemsField;

        /// <remarks/>
        [XmlElement("NetworkLink", typeof (NetworkLinkType))]
        [XmlElement("Placemark", typeof (PlacemarkType))]
        public AbstractFeatureType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("FolderSimpleExtensionGroup")]
        public string[] FolderSimpleExtensionGroup
        {
            get { return folderSimpleExtensionGroupField; }
            set { folderSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("FolderObjectExtensionGroup")]
        public AbstractObjectType[] FolderObjectExtensionGroup
        {
            get { return folderObjectExtensionGroupField; }
            set { folderObjectExtensionGroupField = value; }
        }
    }
}