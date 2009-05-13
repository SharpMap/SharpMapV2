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
    [XmlRoot("Style", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class StyleType : AbstractStyleSelectorType
    {
        private BalloonStyleType balloonStyleField;
        private IconStyleType iconStyleField;

        private LabelStyleType labelStyleField;

        private LineStyleType lineStyleField;

        private ListStyleType listStyleField;
        private PolyStyleType polyStyleField;

        private AbstractObjectType[] styleObjectExtensionGroupField;
        private string[] styleSimpleExtensionGroupField;

        /// <remarks/>
        public IconStyleType IconStyle
        {
            get { return iconStyleField; }
            set { iconStyleField = value; }
        }

        /// <remarks/>
        public LabelStyleType LabelStyle
        {
            get { return labelStyleField; }
            set { labelStyleField = value; }
        }

        /// <remarks/>
        public LineStyleType LineStyle
        {
            get { return lineStyleField; }
            set { lineStyleField = value; }
        }

        /// <remarks/>
        public PolyStyleType PolyStyle
        {
            get { return polyStyleField; }
            set { polyStyleField = value; }
        }

        /// <remarks/>
        public BalloonStyleType BalloonStyle
        {
            get { return balloonStyleField; }
            set { balloonStyleField = value; }
        }

        /// <remarks/>
        public ListStyleType ListStyle
        {
            get { return listStyleField; }
            set { listStyleField = value; }
        }

        /// <remarks/>
        [XmlElement("StyleSimpleExtensionGroup")]
        public string[] StyleSimpleExtensionGroup
        {
            get { return styleSimpleExtensionGroupField; }
            set { styleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("StyleObjectExtensionGroup")]
        public AbstractObjectType[] StyleObjectExtensionGroup
        {
            get { return styleObjectExtensionGroupField; }
            set { styleObjectExtensionGroupField = value; }
        }
    }
}