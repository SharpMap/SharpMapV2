// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "ThoroughfareNumberRange", Namespace = Declarations.SchemaVersion), Serializable]
    public class ThoroughfareNumberRange
    {
        [XmlIgnore] private List<AddressLine> _addressLine;
        [XmlIgnore] private string _code;
        [XmlIgnore] private string _indicator;
        [XmlIgnore] private Occurrence _indicatorOccurrence;

        [XmlIgnore] public bool _indicatorOccurrenceSpecified;
        [XmlIgnore] private NumberOccurrence _numberRangeOccurrence;

        [XmlIgnore] public bool _numberRangeOccurrenceSpecified;
        [XmlIgnore] private RangeType _rangeType;

        [XmlIgnore] public bool _rangeTypeSpecified;
        [XmlIgnore] private string _separator;
        [XmlIgnore] private ThoroughfareNumberFrom _thoroughfareNumberFrom;
        [XmlIgnore] private ThoroughfareNumberTo _thoroughfareNumberTo;
        [XmlIgnore] private string _type;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "RangeType")]
        public RangeType RangeType
        {
            get { return _rangeType; }
            set
            {
                _rangeType = value;
                _rangeTypeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _indicator; }
            set { _indicator = value; }
        }

        [XmlAttribute(AttributeName = "Separator")]
        public string Separator
        {
            get { return _separator; }
            set { _separator = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurrence")]
        public Occurrence IndicatorOccurrence
        {
            get { return _indicatorOccurrence; }
            set
            {
                _indicatorOccurrence = value;
                _indicatorOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberRangeOccurrence")]
        public NumberOccurrence NumberRangeOccurrence
        {
            get { return _numberRangeOccurrence; }
            set
            {
                _numberRangeOccurrence = value;
                _numberRangeOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_addressLine == null) _addressLine = new List<AddressLine>();
                return _addressLine;
            }
            set { _addressLine = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberFrom), ElementName = "ThoroughfareNumberFrom", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareNumberFrom ThoroughfareNumberFrom
        {
            get { return _thoroughfareNumberFrom; }
            set { _thoroughfareNumberFrom = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberTo), ElementName = "ThoroughfareNumberTo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareNumberTo ThoroughfareNumberTo
        {
            get { return _thoroughfareNumberTo; }
            set { _thoroughfareNumberTo = value; }
        }

        public void MakeSchemaCompliant()
        {
            ThoroughfareNumberFrom.MakeSchemaCompliant();
            ThoroughfareNumberTo.MakeSchemaCompliant();
        }
    }
}