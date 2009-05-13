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
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class PremisePremiseNumberRange
    {
        private string indicatorField;

        private PremisePremiseNumberRangeIndicatorOccurence indicatorOccurenceField;

        private bool indicatorOccurenceFieldSpecified;

        private PremisePremiseNumberRangeNumberRangeOccurence numberRangeOccurenceField;

        private bool numberRangeOccurenceFieldSpecified;
        private PremisePremiseNumberRangePremiseNumberRangeFrom premiseNumberRangeFromField;

        private PremisePremiseNumberRangePremiseNumberRangeTo premiseNumberRangeToField;

        private string rangeTypeField;
        private string separatorField;

        private string typeField;

        /// <remarks/>
        public PremisePremiseNumberRangePremiseNumberRangeFrom PremiseNumberRangeFrom
        {
            get { return premiseNumberRangeFromField; }
            set { premiseNumberRangeFromField = value; }
        }

        /// <remarks/>
        public PremisePremiseNumberRangePremiseNumberRangeTo PremiseNumberRangeTo
        {
            get { return premiseNumberRangeToField; }
            set { premiseNumberRangeToField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string RangeType
        {
            get { return rangeTypeField; }
            set { rangeTypeField = value; }
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
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public PremisePremiseNumberRangeIndicatorOccurence IndicatorOccurence
        {
            get { return indicatorOccurenceField; }
            set { indicatorOccurenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool IndicatorOccurenceSpecified
        {
            get { return indicatorOccurenceFieldSpecified; }
            set { indicatorOccurenceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public PremisePremiseNumberRangeNumberRangeOccurence NumberRangeOccurence
        {
            get { return numberRangeOccurenceField; }
            set { numberRangeOccurenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberRangeOccurenceSpecified
        {
            get { return numberRangeOccurenceFieldSpecified; }
            set { numberRangeOccurenceFieldSpecified = value; }
        }
    }
}