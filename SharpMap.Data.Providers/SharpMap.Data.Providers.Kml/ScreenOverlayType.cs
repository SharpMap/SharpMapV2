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
    [XmlRoot("ScreenOverlay", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ScreenOverlayType : AbstractOverlayType
    {
        private vec2Type overlayXYField;

        private double rotationField;

        private bool rotationFieldSpecified;
        private vec2Type rotationXYField;

        private AbstractObjectType[] screenOverlayObjectExtensionGroupField;
        private string[] screenOverlaySimpleExtensionGroupField;
        private vec2Type screenXYField;
        private vec2Type sizeField;

        public ScreenOverlayType()
        {
            rotationField = 0;
        }

        /// <remarks/>
        public vec2Type overlayXY
        {
            get { return overlayXYField; }
            set { overlayXYField = value; }
        }

        /// <remarks/>
        public vec2Type screenXY
        {
            get { return screenXYField; }
            set { screenXYField = value; }
        }

        /// <remarks/>
        public vec2Type rotationXY
        {
            get { return rotationXYField; }
            set { rotationXYField = value; }
        }

        /// <remarks/>
        public vec2Type size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        public double rotation
        {
            get { return rotationField; }
            set { rotationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rotationSpecified
        {
            get { return rotationFieldSpecified; }
            set { rotationFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ScreenOverlaySimpleExtensionGroup")]
        public string[] ScreenOverlaySimpleExtensionGroup
        {
            get { return screenOverlaySimpleExtensionGroupField; }
            set { screenOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ScreenOverlayObjectExtensionGroup")]
        public AbstractObjectType[] ScreenOverlayObjectExtensionGroup
        {
            get { return screenOverlayObjectExtensionGroupField; }
            set { screenOverlayObjectExtensionGroupField = value; }
        }
    }
}