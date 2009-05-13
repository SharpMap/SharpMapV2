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
    [XmlRoot("StyleMap", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class StyleMapType : AbstractStyleSelectorType
    {
        private PairType[] pairField;

        private AbstractObjectType[] styleMapObjectExtensionGroupField;
        private string[] styleMapSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("Pair")]
        public PairType[] Pair
        {
            get { return pairField; }
            set { pairField = value; }
        }

        /// <remarks/>
        [XmlElement("StyleMapSimpleExtensionGroup")]
        public string[] StyleMapSimpleExtensionGroup
        {
            get { return styleMapSimpleExtensionGroupField; }
            set { styleMapSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("StyleMapObjectExtensionGroup")]
        public AbstractObjectType[] StyleMapObjectExtensionGroup
        {
            get { return styleMapObjectExtensionGroupField; }
            set { styleMapObjectExtensionGroupField = value; }
        }
    }
}