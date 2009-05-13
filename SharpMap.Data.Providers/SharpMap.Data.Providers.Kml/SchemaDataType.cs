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
    [XmlRoot("SchemaData", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class SchemaDataType : AbstractObjectType
    {
        private object[] schemaDataExtensionField;

        private string schemaUrlField;
        private SimpleDataType[] simpleDataField;

        /// <remarks/>
        [XmlElement("SimpleData")]
        public SimpleDataType[] SimpleData
        {
            get { return simpleDataField; }
            set { simpleDataField = value; }
        }

        /// <remarks/>
        [XmlElement("SchemaDataExtension")]
        public object[] SchemaDataExtension
        {
            get { return schemaDataExtensionField; }
            set { schemaDataExtensionField = value; }
        }

        /// <remarks/>
        [XmlAttribute(DataType = "anyURI")]
        public string schemaUrl
        {
            get { return schemaUrlField; }
            set { schemaUrlField = value; }
        }
    }
}