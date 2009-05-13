using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (PolyStyleType))]
    [XmlInclude(typeof (LineStyleType))]
    [XmlInclude(typeof (LabelStyleType))]
    [XmlInclude(typeof (IconStyleType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractColorStyleType : AbstractSubStyleType
    {
        private AbstractObjectType[] abstractColorStyleObjectExtensionGroupField;
        private string[] abstractColorStyleSimpleExtensionGroupField;
        private byte[] colorField;

        private colorModeEnumType colorModeField;

        private bool colorModeFieldSpecified;

        public AbstractColorStyleType()
        {
            colorModeField = colorModeEnumType.normal;
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
        public colorModeEnumType colorMode
        {
            get { return colorModeField; }
            set { colorModeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool colorModeSpecified
        {
            get { return colorModeFieldSpecified; }
            set { colorModeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractColorStyleSimpleExtensionGroup")]
        public string[] AbstractColorStyleSimpleExtensionGroup
        {
            get { return abstractColorStyleSimpleExtensionGroupField; }
            set { abstractColorStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractColorStyleObjectExtensionGroup")]
        public AbstractObjectType[] AbstractColorStyleObjectExtensionGroup
        {
            get { return abstractColorStyleObjectExtensionGroupField; }
            set { abstractColorStyleObjectExtensionGroupField = value; }
        }
    }
}