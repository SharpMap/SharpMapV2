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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "PremiseNumberRange", Namespace = Declarations.SchemaVersion), Serializable]
    public class PremiseNumberRange
    {
        [XmlIgnore] private string __Indicator;
        [XmlIgnore] private Occurrence __IndicatorOccurence;

        [XmlIgnore] public bool __IndicatorOccurenceSpecified;
        [XmlIgnore] private NumberOccurrence __NumberRangeOccurence;

        [XmlIgnore] public bool __NumberRangeOccurenceSpecified;
        [XmlIgnore] private PremiseNumberRangeFrom __PremiseNumberRangeFrom;
        [XmlIgnore] private PremiseNumberRangeTo __PremiseNumberRangeTo;
        [XmlIgnore] private string __RangeType;
        [XmlIgnore] private string __Separator;
        [XmlIgnore] private string __Type;

        [XmlAttribute(AttributeName = "RangeType")]
        public string RangeType
        {
            get { return __RangeType; }
            set { __RangeType = value; }
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

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return __Type; }
            set { __Type = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurence")]
        public Occurrence IndicatorOccurence
        {
            get { return __IndicatorOccurence; }
            set
            {
                __IndicatorOccurence = value;
                __IndicatorOccurenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberRangeOccurence")]
        public NumberOccurrence NumberRangeOccurence
        {
            get { return __NumberRangeOccurence; }
            set
            {
                __NumberRangeOccurence = value;
                __NumberRangeOccurenceSpecified = true;
            }
        }

        [XmlElement(Type = typeof (PremiseNumberRangeFrom), ElementName = "PremiseNumberRangeFrom", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRangeFrom PremiseNumberRangeFrom
        {
            get
            {
                if (__PremiseNumberRangeFrom == null) __PremiseNumberRangeFrom = new PremiseNumberRangeFrom();
                return __PremiseNumberRangeFrom;
            }
            set { __PremiseNumberRangeFrom = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberRangeTo), ElementName = "PremiseNumberRangeTo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRangeTo PremiseNumberRangeTo
        {
            get
            {
                if (__PremiseNumberRangeTo == null) __PremiseNumberRangeTo = new PremiseNumberRangeTo();
                return __PremiseNumberRangeTo;
            }
            set { __PremiseNumberRangeTo = value; }
        }

        public void MakeSchemaCompliant()
        {
            PremiseNumberRangeFrom.MakeSchemaCompliant();
            PremiseNumberRangeTo.MakeSchemaCompliant();
        }
    }
}