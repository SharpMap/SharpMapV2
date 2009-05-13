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
    [XmlRoot("LabelStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LabelStyleType : AbstractColorStyleType
    {
        private AbstractObjectType[] labelStyleObjectExtensionGroupField;
        private string[] labelStyleSimpleExtensionGroupField;
        private double scaleField;

        private bool scaleFieldSpecified;

        public LabelStyleType()
        {
            scaleField = 1;
        }

        /// <remarks/>
        public double scale
        {
            get { return scaleField; }
            set { scaleField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool scaleSpecified
        {
            get { return scaleFieldSpecified; }
            set { scaleFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("LabelStyleSimpleExtensionGroup")]
        public string[] LabelStyleSimpleExtensionGroup
        {
            get { return labelStyleSimpleExtensionGroupField; }
            set { labelStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LabelStyleObjectExtensionGroup")]
        public AbstractObjectType[] LabelStyleObjectExtensionGroup
        {
            get { return labelStyleObjectExtensionGroupField; }
            set { labelStyleObjectExtensionGroupField = value; }
        }
    }
}