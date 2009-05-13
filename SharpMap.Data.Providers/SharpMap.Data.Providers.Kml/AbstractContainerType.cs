using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (FolderType))]
    [XmlInclude(typeof (DocumentType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractContainerType : AbstractFeatureType
    {
        private AbstractObjectType[] abstractContainerObjectExtensionGroupField;
        private string[] abstractContainerSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractContainerSimpleExtensionGroup")]
        public string[] AbstractContainerSimpleExtensionGroup
        {
            get { return abstractContainerSimpleExtensionGroupField; }
            set { abstractContainerSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractContainerObjectExtensionGroup")]
        public AbstractObjectType[] AbstractContainerObjectExtensionGroup
        {
            get { return abstractContainerObjectExtensionGroupField; }
            set { abstractContainerObjectExtensionGroupField = value; }
        }
    }
}