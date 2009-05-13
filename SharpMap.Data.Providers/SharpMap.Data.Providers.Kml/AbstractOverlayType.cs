using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (PhotoOverlayType))]
    [XmlInclude(typeof (ScreenOverlayType))]
    [XmlInclude(typeof (GroundOverlayType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractOverlayType : AbstractFeatureType
    {
        private AbstractObjectType[] abstractOverlayObjectExtensionGroupField;
        private string[] abstractOverlaySimpleExtensionGroupField;
        private byte[] colorField;

        private int drawOrderField;

        private bool drawOrderFieldSpecified;

        private LinkType iconField;

        public AbstractOverlayType()
        {
            drawOrderField = 0;
        }

        /// <remarks/>
        // CODEGEN Warning: 'default' attribute on items of type 'hexBinary' is not supported in this version of the .Net Framework.  Ignoring default='ffffffff' attribute.
        [XmlElement(DataType = "hexBinary")]
        public byte[] color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public int drawOrder
        {
            get { return drawOrderField; }
            set { drawOrderField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool drawOrderSpecified
        {
            get { return drawOrderFieldSpecified; }
            set { drawOrderFieldSpecified = value; }
        }

        /// <remarks/>
        public LinkType Icon
        {
            get { return iconField; }
            set { iconField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractOverlaySimpleExtensionGroup")]
        public string[] AbstractOverlaySimpleExtensionGroup
        {
            get { return abstractOverlaySimpleExtensionGroupField; }
            set { abstractOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractOverlayObjectExtensionGroup")]
        public AbstractObjectType[] AbstractOverlayObjectExtensionGroup
        {
            get { return abstractOverlayObjectExtensionGroupField; }
            set { abstractOverlayObjectExtensionGroupField = value; }
        }
    }
}