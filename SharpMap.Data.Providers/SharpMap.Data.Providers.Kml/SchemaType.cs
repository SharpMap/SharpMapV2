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
    [XmlRoot("Schema", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class SchemaType
    {
        private string idField;
        private string nameField;
        private object[] schemaExtensionField;
        private SimpleFieldType[] simpleFieldField;

        /// <remarks/>
        [XmlElement("SimpleField")]
        public SimpleFieldType[] SimpleField
        {
            get { return simpleFieldField; }
            set { simpleFieldField = value; }
        }

        /// <remarks/>
        [XmlElement("SchemaExtension")]
        public object[] SchemaExtension
        {
            get { return schemaExtensionField; }
            set { schemaExtensionField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "ID")]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }
    }
}