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
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class DependentLocalityTypeDependentLocalityNumber
    {
        private XmlAttribute[] anyAttrField;
        private string codeField;
        private DependentLocalityTypeDependentLocalityNumberNameNumberOccurrence nameNumberOccurrenceField;

        private bool nameNumberOccurrenceFieldSpecified;

        private string[] textField;

        /// <remarks/>
        [XmlAttribute]
        public DependentLocalityTypeDependentLocalityNumberNameNumberOccurrence NameNumberOccurrence
        {
            get { return nameNumberOccurrenceField; }
            set { nameNumberOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NameNumberOccurrenceSpecified
        {
            get { return nameNumberOccurrenceFieldSpecified; }
            set { nameNumberOccurrenceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }

        /// <remarks/>
        [XmlAnyAttribute]
        public XmlAttribute[] AnyAttr
        {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }

        /// <remarks/>
        [XmlText]
        public string[] Text
        {
            get { return textField; }
            set { textField = value; }
        }
    }
}