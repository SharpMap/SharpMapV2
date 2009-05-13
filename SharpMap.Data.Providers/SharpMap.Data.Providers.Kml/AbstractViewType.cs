using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (CameraType))]
    [XmlInclude(typeof (LookAtType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractViewType : AbstractObjectType
    {
        private AbstractObjectType[] abstractViewObjectExtensionGroupField;
        private string[] abstractViewSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractViewSimpleExtensionGroup")]
        public string[] AbstractViewSimpleExtensionGroup
        {
            get { return abstractViewSimpleExtensionGroupField; }
            set { abstractViewSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractViewObjectExtensionGroup")]
        public AbstractObjectType[] AbstractViewObjectExtensionGroup
        {
            get { return abstractViewObjectExtensionGroupField; }
            set { abstractViewObjectExtensionGroupField = value; }
        }
    }
}