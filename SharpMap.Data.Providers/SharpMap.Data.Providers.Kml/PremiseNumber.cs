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
    [XmlRoot(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0", IsNullable = false)]
    public class PremiseNumber
    {
        private XmlAttribute[] anyAttrField;
        private string codeField;
        private string indicatorField;

        private PremiseNumberIndicatorOccurrence indicatorOccurrenceField;

        private bool indicatorOccurrenceFieldSpecified;
        private PremiseNumberNumberType numberTypeField;

        private bool numberTypeFieldSpecified;

        private PremiseNumberNumberTypeOccurrence numberTypeOccurrenceField;

        private bool numberTypeOccurrenceFieldSpecified;

        private string[] textField;
        private string typeField;

        /// <remarks/>
        [XmlAttribute]
        public PremiseNumberNumberType NumberType
        {
            get { return numberTypeField; }
            set { numberTypeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberTypeSpecified
        {
            get { return numberTypeFieldSpecified; }
            set { numberTypeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Indicator
        {
            get { return indicatorField; }
            set { indicatorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public PremiseNumberIndicatorOccurrence IndicatorOccurrence
        {
            get { return indicatorOccurrenceField; }
            set { indicatorOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool IndicatorOccurrenceSpecified
        {
            get { return indicatorOccurrenceFieldSpecified; }
            set { indicatorOccurrenceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public PremiseNumberNumberTypeOccurrence NumberTypeOccurrence
        {
            get { return numberTypeOccurrenceField; }
            set { numberTypeOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberTypeOccurrenceSpecified
        {
            get { return numberTypeOccurrenceFieldSpecified; }
            set { numberTypeOccurrenceFieldSpecified = value; }
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