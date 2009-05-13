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
    [XmlRoot("Alias", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class AliasType : AbstractObjectType
    {
        private AbstractObjectType[] aliasObjectExtensionGroupField;
        private string[] aliasSimpleExtensionGroupField;
        private string sourceHrefField;
        private string targetHrefField;

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string targetHref
        {
            get { return targetHrefField; }
            set { targetHrefField = value; }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string sourceHref
        {
            get { return sourceHrefField; }
            set { sourceHrefField = value; }
        }

        /// <remarks/>
        [XmlElement("AliasSimpleExtensionGroup")]
        public string[] AliasSimpleExtensionGroup
        {
            get { return aliasSimpleExtensionGroupField; }
            set { aliasSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AliasObjectExtensionGroup")]
        public AbstractObjectType[] AliasObjectExtensionGroup
        {
            get { return aliasObjectExtensionGroupField; }
            set { aliasObjectExtensionGroupField = value; }
        }
    }
}