using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (StyleMapType))]
    [XmlInclude(typeof (StyleType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractStyleSelectorType : AbstractObjectType
    {
        private AbstractObjectType[] abstractStyleSelectorObjectExtensionGroupField;
        private string[] abstractStyleSelectorSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractStyleSelectorSimpleExtensionGroup")]
        public string[] AbstractStyleSelectorSimpleExtensionGroup
        {
            get { return abstractStyleSelectorSimpleExtensionGroupField; }
            set { abstractStyleSelectorSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractStyleSelectorObjectExtensionGroup")]
        public AbstractObjectType[] AbstractStyleSelectorObjectExtensionGroup
        {
            get { return abstractStyleSelectorObjectExtensionGroupField; }
            set { abstractStyleSelectorObjectExtensionGroupField = value; }
        }
    }
}