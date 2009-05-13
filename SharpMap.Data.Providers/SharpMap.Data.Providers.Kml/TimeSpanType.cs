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
    [XmlRoot("TimeSpan", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class TimeSpanType : AbstractTimePrimitiveType
    {
        private string beginField;

        private string endField;

        private AbstractObjectType[] timeSpanObjectExtensionGroupField;
        private string[] timeSpanSimpleExtensionGroupField;

        /// <remarks/>
        public string begin
        {
            get { return beginField; }
            set { beginField = value; }
        }

        /// <remarks/>
        public string end
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlElement("TimeSpanSimpleExtensionGroup")]
        public string[] TimeSpanSimpleExtensionGroup
        {
            get { return timeSpanSimpleExtensionGroupField; }
            set { timeSpanSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("TimeSpanObjectExtensionGroup")]
        public AbstractObjectType[] TimeSpanObjectExtensionGroup
        {
            get { return timeSpanObjectExtensionGroupField; }
            set { timeSpanObjectExtensionGroupField = value; }
        }
    }
}