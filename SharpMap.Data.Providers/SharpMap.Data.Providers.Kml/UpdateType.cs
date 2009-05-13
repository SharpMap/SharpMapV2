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
    [XmlRoot("Update", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class UpdateType
    {
        private object[] itemsField;
        private string targetHrefField;

        private object[] updateExtensionGroupField;

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string targetHref
        {
            get { return targetHrefField; }
            set { targetHrefField = value; }
        }

        /// <remarks/>
        [XmlElement("Change", typeof (ChangeType))]
        [XmlElement("Create", typeof (CreateType))]
        [XmlElement("Delete", typeof (DeleteType))]
        [XmlElement("UpdateOpExtensionGroup", typeof (object))]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("UpdateExtensionGroup")]
        public object[] UpdateExtensionGroup
        {
            get { return updateExtensionGroupField; }
            set { updateExtensionGroupField = value; }
        }
    }
}