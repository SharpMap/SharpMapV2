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
    [XmlRoot("Pair", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class PairType : AbstractObjectType
    {
        private AbstractStyleSelectorType itemField;
        private styleStateEnumType keyField;

        private bool keyFieldSpecified;

        private AbstractObjectType[] pairObjectExtensionGroupField;
        private string[] pairSimpleExtensionGroupField;
        private string styleUrlField;

        public PairType()
        {
            keyField = styleStateEnumType.normal;
        }

        /// <remarks/>
        public styleStateEnumType key
        {
            get { return keyField; }
            set { keyField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool keySpecified
        {
            get { return keyFieldSpecified; }
            set { keyFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(DataType = "anyURI")]
        public string styleUrl
        {
            get { return styleUrlField; }
            set { styleUrlField = value; }
        }

        /// <remarks/>
        [XmlElement("Style", typeof (StyleType))]
        [XmlElement("StyleMap", typeof (StyleMapType))]
        public AbstractStyleSelectorType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("PairSimpleExtensionGroup")]
        public string[] PairSimpleExtensionGroup
        {
            get { return pairSimpleExtensionGroupField; }
            set { pairSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("PairObjectExtensionGroup")]
        public AbstractObjectType[] PairObjectExtensionGroup
        {
            get { return pairObjectExtensionGroupField; }
            set { pairObjectExtensionGroupField = value; }
        }
    }
}