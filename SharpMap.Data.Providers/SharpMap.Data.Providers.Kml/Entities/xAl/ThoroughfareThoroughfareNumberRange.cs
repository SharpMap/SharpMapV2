// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAl
{
    /// <remarks/>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class ThoroughfareThoroughfareNumberRange
    {
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
        public xAlTypedElementBase[] AddressLine { get; set; }

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