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
    [XmlType(Namespace = "http://www.w3.org/2005/Atom")]
    [XmlRoot("author", Namespace = "http://www.w3.org/2005/Atom", IsNullable = false)]
    public class atomPersonConstruct
    {
        private ItemsChoiceType[] itemsElementNameField;
        private string[] itemsField;

        /// <remarks/>
        [XmlElement("email", typeof (string))]
        [XmlElement("name", typeof (string))]
        [XmlElement("uri", typeof (string))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public string[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}