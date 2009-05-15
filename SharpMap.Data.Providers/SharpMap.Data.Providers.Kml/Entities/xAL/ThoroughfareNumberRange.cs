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
        [XmlIgnore] private List<AddressLine> __AddressLine;
        [XmlIgnore] private string __Code;
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private Occurrence __IndicatorOccurrence;

        [XmlIgnore] public bool __IndicatorOccurrenceSpecified;
        [XmlIgnore] private NumberOccurrence __NumberRangeOccurrence;

        [XmlIgnore] public bool __NumberRangeOccurrenceSpecified;
        [XmlIgnore] private RangeType __RangeType;

        [XmlIgnore] public bool __RangeTypeSpecified;
        [XmlIgnore] private string __Separator;
        [XmlIgnore] private ThoroughfareNumberFrom __ThoroughfareNumberFrom;
        [XmlIgnore] private ThoroughfareNumberTo __ThoroughfareNumberTo;
        [XmlIgnore] private string __Type;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "RangeType")]
        public RangeType RangeType
        {
            get { return __RangeType; }
            set
            {
                __RangeType = value;
                __RangeTypeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return __Indicator; }
            set { __Indicator = value; }
        }

        [XmlAttribute(AttributeName = "Separator")]
        public string Separator
        {
            get { return __Separator; }
            set { __Separator = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurrence")]
        public Occurrence IndicatorOccurrence
        {
            get { return __IndicatorOccurrence; }
            set
            {
                __IndicatorOccurrence = value;
                __IndicatorOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberRangeOccurrence")]
        public NumberOccurrence NumberRangeOccurrence
        {
            get { return __NumberRangeOccurrence; }
            set
            {
                __NumberRangeOccurrence = value;
                __NumberRangeOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return __Code; }
            set { __Code = value; }
        }

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (__AddressLine == null) __AddressLine = new List<AddressLine>();
                return __AddressLine;
            }
            set { __AddressLine = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberFrom), ElementName = "ThoroughfareNumberFrom", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareNumberFrom ThoroughfareNumberFrom
        {
            get
            {
                
                return __ThoroughfareNumberFrom;
            }
            set { __ThoroughfareNumberFrom = value; }
        }

        [XmlElement(Type = typeof (ThoroughfareNumberTo), ElementName = "ThoroughfareNumberTo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ThoroughfareNumberTo ThoroughfareNumberTo
        {
            get
            {
                
                return __ThoroughfareNumberTo;
            }
            set { __ThoroughfareNumberTo = value; }
        }

        public void MakeSchemaCompliant()
        {
            ThoroughfareNumberFrom.MakeSchemaCompliant();
            ThoroughfareNumberTo.MakeSchemaCompliant();
        }
    }
}