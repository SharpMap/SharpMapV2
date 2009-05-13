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
    [XmlRoot("SimpleField", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class SimpleFieldType
    {
        private string displayNameField;
        private string nameField;

        private object[] simpleFieldExtensionField;

        private string typeField;

        /// <remarks/>
        public string displayName
        {
            get { return displayNameField; }
            set { displayNameField = value; }
        }

        /// <remarks/>
        [XmlElement("SimpleFieldExtension")]
        public object[] SimpleFieldExtension
        {
            get { return simpleFieldExtensionField; }
            set { simpleFieldExtensionField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }
}