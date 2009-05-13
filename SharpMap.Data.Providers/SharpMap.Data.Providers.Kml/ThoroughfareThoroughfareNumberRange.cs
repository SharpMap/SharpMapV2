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
    public class ThoroughfareThoroughfareNumberRange
    {
        private AddressLine[] addressLineField;
        private XmlAttribute[] anyAttrField;
        private string codeField;

        private string indicatorField;

        private ThoroughfareThoroughfareNumberRangeIndicatorOccurrence indicatorOccurrenceField;

        private bool indicatorOccurrenceFieldSpecified;

        private ThoroughfareThoroughfareNumberRangeNumberRangeOccurrence numberRangeOccurrenceField;

        private bool numberRangeOccurrenceFieldSpecified;
        private ThoroughfareThoroughfareNumberRangeRangeType rangeTypeField;

        private bool rangeTypeFieldSpecified;
        private string separatorField;
        private ThoroughfareThoroughfareNumberRangeThoroughfareNumberFrom thoroughfareNumberFromField;

        private ThoroughfareThoroughfareNumberRangeThoroughfareNumberTo thoroughfareNumberToField;

        private string typeField;

        /// <remarks/>
        [XmlElement("AddressLine")]
        public AddressLine[] AddressLine
        {
            get { return addressLineField; }
            set { addressLineField = value; }
        }

        /// <remarks/>
        public ThoroughfareThoroughfareNumberRangeThoroughfareNumberFrom ThoroughfareNumberFrom
        {
            get { return thoroughfareNumberFromField; }
            set { thoroughfareNumberFromField = value; }
        }

        /// <remarks/>
        public ThoroughfareThoroughfareNumberRangeThoroughfareNumberTo ThoroughfareNumberTo
        {
            get { return thoroughfareNumberToField; }
            set { thoroughfareNumberToField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public ThoroughfareThoroughfareNumberRangeRangeType RangeType
        {
            get { return rangeTypeField; }
            set { rangeTypeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool RangeTypeSpecified
        {
            get { return rangeTypeFieldSpecified; }
            set { rangeTypeFieldSpecified = value; }
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
        public string Separator
        {
            get { return separatorField; }
            set { separatorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public ThoroughfareThoroughfareNumberRangeIndicatorOccurrence IndicatorOccurrence
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
        public ThoroughfareThoroughfareNumberRangeNumberRangeOccurrence NumberRangeOccurrence
        {
            get { return numberRangeOccurrenceField; }
            set { numberRangeOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberRangeOccurrenceSpecified
        {
            get { return numberRangeOccurrenceFieldSpecified; }
            set { numberRangeOccurrenceFieldSpecified = value; }
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
    }
}