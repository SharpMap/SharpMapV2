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
    [XmlRoot("linkSnippet", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class SnippetType
    {
        private int maxLinesField;

        private string valueField;

        public SnippetType()
        {
            maxLinesField = 2;
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(2)]
        public int maxLines
        {
            get { return maxLinesField; }
            set { maxLinesField = value; }
        }

        /// <remarks/>
        [XmlText]
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }
}