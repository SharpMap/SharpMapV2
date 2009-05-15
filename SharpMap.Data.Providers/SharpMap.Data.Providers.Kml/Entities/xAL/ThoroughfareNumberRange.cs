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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "ThoroughfareNumberRange", Namespace = Declarations.SchemaVersion), Serializable]
    public class ThoroughfareNumberRange
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private string _Code;
        [XmlIgnore] private string _Indicator;
        [XmlIgnore] private Occurrence _IndicatorOccurrence;

        [XmlIgnore] public bool _IndicatorOccurrenceSpecified;
        [XmlIgnore] private NumberOccurrence _NumberRangeOccurrence;

        [XmlIgnore] public bool _NumberRangeOccurrenceSpecified;
        [XmlIgnore] private RangeType _RangeType;

        [XmlIgnore] public bool _RangeTypeSpecified;
        [XmlIgnore] private string _Separator;
        [XmlIgnore] private ThoroughfareNumberFrom _ThoroughfareNumberFrom;
        [XmlIgnore] private ThoroughfareNumberTo _ThoroughfareNumberTo;
        [XmlIgnore] private string _Type;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "RangeType")]
        public RangeType RangeType
        {
            get { return _RangeType; }
            set
            {
                _RangeType = value;
                _RangeTypeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _Indicator; }
            set { _Indicator = value; }
        }

        [XmlAttribute(AttributeName = "Separator")]
        public string Separator
        {
            get { return _Separator; }
            set { _Separator = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurrence")]
        public Occurrence IndicatorOccurrence
        {
            get { return _IndicatorOccurrence; }
            set
            {
                _IndicatorOccurrence = value;
                _IndicatorOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberRangeOccurrence")]
        public NumberOccurrence NumberRangeOccurrence
        {
            get { return _NumberRangeOccurrence; }
            set
            {
                _NumberRangeOccurrence = value;
                _NumberRangeOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_AddressLine == null) _AddressLine = new List<AddressLine>();
                return _AddressLine;
            }
            set { _AddressLine = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberFrom), ElementName = "ThoroughfareNumberFrom", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareNumberFrom ThoroughfareNumberFrom
        {
            get { return _ThoroughfareNumberFrom; }
            set { _ThoroughfareNumberFrom = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberTo), ElementName = "ThoroughfareNumberTo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareNumberTo ThoroughfareNumberTo
        {
            get { return _ThoroughfareNumberTo; }
            set { _ThoroughfareNumberTo = value; }
        }

        public void MakeSchemaCompliant()
        {
            ThoroughfareNumberFrom.MakeSchemaCompliant();
            ThoroughfareNumberTo.MakeSchemaCompliant();
        }
    }
}