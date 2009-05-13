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
    [XmlRoot("Data", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class DataType : AbstractObjectType
    {
        private object[] dataExtensionField;
        private string displayNameField;

        private string nameField;
        private string valueField;

        /// <remarks/>
        public string displayName
        {
            get { return displayNameField; }
            set { displayNameField = value; }
        }

        /// <remarks/>
        public string value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <remarks/>
        [XmlElement("DataExtension")]
        public object[] DataExtension
        {
            get { return dataExtensionField; }
            set { dataExtensionField = value; }
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