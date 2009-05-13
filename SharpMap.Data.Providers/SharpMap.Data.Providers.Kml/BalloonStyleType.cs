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
    [XmlRoot("BalloonStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class BalloonStyleType : AbstractSubStyleType
    {
        private AbstractObjectType[] balloonStyleObjectExtensionGroupField;
        private string[] balloonStyleSimpleExtensionGroupField;
        private displayModeEnumType displayModeField;

        private bool displayModeFieldSpecified;
        private ItemChoiceType itemElementNameField;
        private byte[] itemField;

        private byte[] textColorField;

        private string textField;

        public BalloonStyleType()
        {
            displayModeField = displayModeEnumType.@default;
        }

        /// <remarks/>
        [XmlElement("bgColor", typeof (byte[]), DataType = "hexBinary")]
        [XmlElement("color", typeof (byte[]), DataType = "hexBinary")]
        [XmlChoiceIdentifier("ItemElementName")]
        public byte[] Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public ItemChoiceType ItemElementName
        {
            get { return itemElementNameField; }
            set { itemElementNameField = value; }
        }

        /// <remarks/>
        // CODEGEN Warning: 'default' attribute on items of type 'hexBinary' is not supported in this version of the .Net Framework.  Ignoring default='ff000000' attribute.
        [XmlElement(DataType = "hexBinary")]
        public byte[] textColor
        {
            get { return textColorField; }
            set { textColorField = value; }
        }

        /// <remarks/>
        public string text
        {
            get { return textField; }
            set { textField = value; }
        }

        /// <remarks/>
        public displayModeEnumType displayMode
        {
            get { return displayModeField; }
            set { displayModeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool displayModeSpecified
        {
            get { return displayModeFieldSpecified; }
            set { displayModeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("BalloonStyleSimpleExtensionGroup")]
        public string[] BalloonStyleSimpleExtensionGroup
        {
            get { return balloonStyleSimpleExtensionGroupField; }
            set { balloonStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("BalloonStyleObjectExtensionGroup")]
        public AbstractObjectType[] BalloonStyleObjectExtensionGroup
        {
            get { return balloonStyleObjectExtensionGroupField; }
            set { balloonStyleObjectExtensionGroupField = value; }
        }
    }
}