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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "PremiseNumberRange", Namespace = Declarations.SchemaVersion), Serializable]
    public class PremiseNumberRange
    {
        [XmlIgnore] private string _indicator;
        [XmlIgnore] private Occurrence _indicatorOccurence;

        [XmlIgnore] public bool _indicatorOccurenceSpecified;
        [XmlIgnore] private NumberOccurrence _numberRangeOccurence;

        [XmlIgnore] public bool _numberRangeOccurenceSpecified;
        [XmlIgnore] private PremiseNumberRangeFrom _premiseNumberRangeFrom;
        [XmlIgnore] private PremiseNumberRangeTo _premiseNumberRangeTo;
        [XmlIgnore] private string _rangeType;
        [XmlIgnore] private string _separator;
        [XmlIgnore] private string _type;

        [XmlAttribute(AttributeName = "RangeType")]
        public string RangeType
        {
            get { return _rangeType; }
            set { _rangeType = value; }
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

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurence")]
        public Occurrence IndicatorOccurence
        {
            get { return _indicatorOccurence; }
            set
            {
                _indicatorOccurence = value;
                _indicatorOccurenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberRangeOccurence")]
        public NumberOccurrence NumberRangeOccurence
        {
            get { return _numberRangeOccurence; }
            set
            {
                _numberRangeOccurence = value;
                _numberRangeOccurenceSpecified = true;
            }
        }

        [XmlElement(Type = typeof (PremiseNumberRangeFrom), ElementName = "PremiseNumberRangeFrom", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRangeFrom PremiseNumberRangeFrom
        {
            get { return _premiseNumberRangeFrom; }
            set { _premiseNumberRangeFrom = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberRangeTo), ElementName = "PremiseNumberRangeTo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRangeTo PremiseNumberRangeTo
        {
            get { return _premiseNumberRangeTo; }
            set { _premiseNumberRangeTo = value; }
        }

        public void MakeSchemaCompliant()
        {
            PremiseNumberRangeFrom.MakeSchemaCompliant();
            PremiseNumberRangeTo.MakeSchemaCompliant();
        }
    }
}