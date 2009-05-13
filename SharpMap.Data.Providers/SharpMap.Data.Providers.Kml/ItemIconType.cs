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
    [XmlRoot("ItemIcon", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ItemIconType : AbstractObjectType
    {
        private string hrefField;

        private AbstractObjectType[] itemIconObjectExtensionGroupField;
        private string[] itemIconSimpleExtensionGroupField;
        private string stateField;

        /// <remarks/>
        public string state
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string href
        {
            get { return hrefField; }
            set { hrefField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemIconSimpleExtensionGroup")]
        public string[] ItemIconSimpleExtensionGroup
        {
            get { return itemIconSimpleExtensionGroupField; }
            set { itemIconSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemIconObjectExtensionGroup")]
        public AbstractObjectType[] ItemIconObjectExtensionGroup
        {
            get { return itemIconObjectExtensionGroupField; }
            set { itemIconObjectExtensionGroupField = value; }
        }
    }
}