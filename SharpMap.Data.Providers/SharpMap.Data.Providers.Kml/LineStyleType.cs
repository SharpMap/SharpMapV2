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
    [XmlRoot("LineStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LineStyleType : AbstractColorStyleType
    {
        private AbstractObjectType[] lineStyleObjectExtensionGroupField;
        private string[] lineStyleSimpleExtensionGroupField;
        private double widthField;

        private bool widthFieldSpecified;

        public LineStyleType()
        {
            widthField = 1;
        }

        /// <remarks/>
        public double width
        {
            get { return widthField; }
            set { widthField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool widthSpecified
        {
            get { return widthFieldSpecified; }
            set { widthFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LineStyleSimpleExtensionGroup")]
        public string[] LineStyleSimpleExtensionGroup
        {
            get { return lineStyleSimpleExtensionGroupField; }
            set { lineStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LineStyleObjectExtensionGroup")]
        public AbstractObjectType[] LineStyleObjectExtensionGroup
        {
            get { return lineStyleObjectExtensionGroupField; }
            set { lineStyleObjectExtensionGroupField = value; }
        }
    }
}