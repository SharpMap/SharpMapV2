using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (ListStyleType))]
    [XmlInclude(typeof (BalloonStyleType))]
    [XmlInclude(typeof (AbstractColorStyleType))]
    [XmlInclude(typeof (PolyStyleType))]
    [XmlInclude(typeof (LineStyleType))]
    [XmlInclude(typeof (LabelStyleType))]
    [XmlInclude(typeof (IconStyleType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractSubStyleType : AbstractObjectType
    {
        private AbstractObjectType[] abstractSubStyleObjectExtensionGroupField;
        private string[] abstractSubStyleSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractSubStyleSimpleExtensionGroup")]
        public string[] AbstractSubStyleSimpleExtensionGroup
        {
            get { return abstractSubStyleSimpleExtensionGroupField; }
            set { abstractSubStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractSubStyleObjectExtensionGroup")]
        public AbstractObjectType[] AbstractSubStyleObjectExtensionGroup
        {
            get { return abstractSubStyleObjectExtensionGroupField; }
            set { abstractSubStyleObjectExtensionGroupField = value; }
        }
    }
}