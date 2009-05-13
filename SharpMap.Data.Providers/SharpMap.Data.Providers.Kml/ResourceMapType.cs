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
    [XmlRoot("ResourceMap", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ResourceMapType : AbstractObjectType
    {
        private AliasType[] aliasField;

        private AbstractObjectType[] resourceMapObjectExtensionGroupField;
        private string[] resourceMapSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("Alias")]
        public AliasType[] Alias
        {
            get { return aliasField; }
            set { aliasField = value; }
        }

        /// <remarks/>
        [XmlElement("ResourceMapSimpleExtensionGroup")]
        public string[] ResourceMapSimpleExtensionGroup
        {
            get { return resourceMapSimpleExtensionGroupField; }
            set { resourceMapSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ResourceMapObjectExtensionGroup")]
        public AbstractObjectType[] ResourceMapObjectExtensionGroup
        {
            get { return resourceMapObjectExtensionGroupField; }
            set { resourceMapObjectExtensionGroupField = value; }
        }
    }
}