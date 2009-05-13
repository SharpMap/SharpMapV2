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
    public class SubPremiseTypeSubPremiseName
    {
        private XmlAttribute[] anyAttrField;
        private string codeField;
        private string[] textField;
        private string typeField;

        private SubPremiseTypeSubPremiseNameTypeOccurrence typeOccurrenceField;

        private bool typeOccurrenceFieldSpecified;

        /// <remarks/>
        [XmlAttribute]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public SubPremiseTypeSubPremiseNameTypeOccurrence TypeOccurrence
        {
            get { return typeOccurrenceField; }
            set { typeOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool TypeOccurrenceSpecified
        {
            get { return typeOccurrenceFieldSpecified; }
            set { typeOccurrenceFieldSpecified = value; }
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