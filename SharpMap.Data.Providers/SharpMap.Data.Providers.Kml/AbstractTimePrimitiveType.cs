using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (TimeSpanType))]
    [XmlInclude(typeof (TimeStampType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractTimePrimitiveType : AbstractObjectType
    {
        private AbstractObjectType[] abstractTimePrimitiveObjectExtensionGroupField;
        private string[] abstractTimePrimitiveSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractTimePrimitiveSimpleExtensionGroup")]
        public string[] AbstractTimePrimitiveSimpleExtensionGroup
        {
            get { return abstractTimePrimitiveSimpleExtensionGroupField; }
            set { abstractTimePrimitiveSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractTimePrimitiveObjectExtensionGroup")]
        public AbstractObjectType[] AbstractTimePrimitiveObjectExtensionGroup
        {
            get { return abstractTimePrimitiveObjectExtensionGroupField; }
            set { abstractTimePrimitiveObjectExtensionGroupField = value; }
        }
    }
}