using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (LinkType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public class BasicLinkType : AbstractObjectType
    {
        private AbstractObjectType[] basicLinkObjectExtensionGroupField;
        private string[] basicLinkSimpleExtensionGroupField;
        private string hrefField;

        /// <remarks/>
        public string href
        {
            get { return hrefField; }
            set { hrefField = value; }
        }

        /// <remarks/>
        [XmlElement("BasicLinkSimpleExtensionGroup")]
        public string[] BasicLinkSimpleExtensionGroup
        {
            get { return basicLinkSimpleExtensionGroupField; }
            set { basicLinkSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("BasicLinkObjectExtensionGroup")]
        public AbstractObjectType[] BasicLinkObjectExtensionGroup
        {
            get { return basicLinkObjectExtensionGroupField; }
            set { basicLinkObjectExtensionGroupField = value; }
        }
    }
}