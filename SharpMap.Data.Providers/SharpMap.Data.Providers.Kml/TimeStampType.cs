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
    [XmlRoot("TimeStamp", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class TimeStampType : AbstractTimePrimitiveType
    {
        private AbstractObjectType[] timeStampObjectExtensionGroupField;
        private string[] timeStampSimpleExtensionGroupField;
        private string whenField;

        /// <remarks/>
        public string when
        {
            get { return whenField; }
            set { whenField = value; }
        }

        /// <remarks/>
        [XmlElement("TimeStampSimpleExtensionGroup")]
        public string[] TimeStampSimpleExtensionGroup
        {
            get { return timeStampSimpleExtensionGroupField; }
            set { timeStampSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("TimeStampObjectExtensionGroup")]
        public AbstractObjectType[] TimeStampObjectExtensionGroup
        {
            get { return timeStampObjectExtensionGroupField; }
            set { timeStampObjectExtensionGroupField = value; }
        }
    }
}