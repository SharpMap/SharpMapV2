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
    [XmlRoot("PolyStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PolyStyleType : AbstractColorStyleType
    {
        private bool fillField;

        private bool fillFieldSpecified;

        private bool outlineField;

        private bool outlineFieldSpecified;

        private AbstractObjectType[] polyStyleObjectExtensionGroupField;
        private string[] polyStyleSimpleExtensionGroupField;

        public PolyStyleType()
        {
            fillField = true;
            outlineField = true;
        }

        /// <remarks/>
        public bool fill
        {
            get { return fillField; }
            set { fillField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool fillSpecified
        {
            get { return fillFieldSpecified; }
            set { fillFieldSpecified = value; }
        }

        /// <remarks/>
        public bool outline
        {
            get { return outlineField; }
            set { outlineField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool outlineSpecified
        {
            get { return outlineFieldSpecified; }
            set { outlineFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("PolyStyleSimpleExtensionGroup")]
        public string[] PolyStyleSimpleExtensionGroup
        {
            get { return polyStyleSimpleExtensionGroupField; }
            set { polyStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PolyStyleObjectExtensionGroup")]
        public AbstractObjectType[] PolyStyleObjectExtensionGroup
        {
            get { return polyStyleObjectExtensionGroupField; }
            set { polyStyleObjectExtensionGroupField = value; }
        }
    }
}