using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("ExtendedData", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ExtendedDataType
    {
        private XmlElement[] anyField;
        private DataType[] dataField;

        private SchemaDataType[] schemaDataField;

        /// <remarks/>
        [XmlElement("Data")]
        public DataType[] Data
        {
            get { return dataField; }
            set { dataField = value; }
        }

        /// <remarks/>
        [XmlElement("SchemaData")]
        public SchemaDataType[] SchemaData
        {
            get { return schemaDataField; }
            set { schemaDataField = value; }
        }

        /// <remarks/>
        [XmlAnyElement]
        public XmlElement[] Any
        {
            get { return anyField; }
            set { anyField = value; }
        }
    }
}