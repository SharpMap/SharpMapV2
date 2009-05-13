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
    [XmlRoot("IconStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class IconStyleType : AbstractColorStyleType
    {
        private double headingField;

        private bool headingFieldSpecified;

        private vec2Type hotSpotField;
        private BasicLinkType iconField;

        private AbstractObjectType[] iconStyleObjectExtensionGroupField;
        private string[] iconStyleSimpleExtensionGroupField;
        private double scaleField;

        private bool scaleFieldSpecified;

        public IconStyleType()
        {
            scaleField = 1;
            headingField = 0;
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
        public double heading
        {
            get { return headingField; }
            set { headingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool headingSpecified
        {
            get { return headingFieldSpecified; }
            set { headingFieldSpecified = value; }
        }

        /// <remarks/>
        public BasicLinkType Icon
        {
            get { return iconField; }
            set { iconField = value; }
        }

        /// <remarks/>
        public vec2Type hotSpot
        {
            get { return hotSpotField; }
            set { hotSpotField = value; }
        }

        /// <remarks/>
        [XmlElement("IconStyleSimpleExtensionGroup")]
        public string[] IconStyleSimpleExtensionGroup
        {
            get { return iconStyleSimpleExtensionGroupField; }
            set { iconStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("IconStyleObjectExtensionGroup")]
        public AbstractObjectType[] IconStyleObjectExtensionGroup
        {
            get { return iconStyleObjectExtensionGroupField; }
            set { iconStyleObjectExtensionGroupField = value; }
        }
    }
}