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
        [XmlIgnore] private string _Indicator;
        [XmlIgnore] private Occurrence _IndicatorOccurence;

        [XmlIgnore] public bool _IndicatorOccurenceSpecified;
        [XmlIgnore] private NumberOccurrence _NumberRangeOccurence;

        [XmlIgnore] public bool _NumberRangeOccurenceSpecified;
        [XmlIgnore] private PremiseNumberRangeFrom _PremiseNumberRangeFrom;
        [XmlIgnore] private PremiseNumberRangeTo _PremiseNumberRangeTo;
        [XmlIgnore] private string _RangeType;
        [XmlIgnore] private string _Separator;
        [XmlIgnore] private string _Type;

        [XmlAttribute(AttributeName = "RangeType")]
        public string RangeType
        {
            get { return _RangeType; }
            set { _RangeType = value; }
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

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurence")]
        public Occurrence IndicatorOccurence
        {
            get { return _IndicatorOccurence; }
            set
            {
                _IndicatorOccurence = value;
                _IndicatorOccurenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberRangeOccurence")]
        public NumberOccurrence NumberRangeOccurence
        {
            get { return _NumberRangeOccurence; }
            set
            {
                _NumberRangeOccurence = value;
                _NumberRangeOccurenceSpecified = true;
            }
        }

        [XmlElement(Type = typeof (PremiseNumberRangeFrom), ElementName = "PremiseNumberRangeFrom", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRangeFrom PremiseNumberRangeFrom
        {
            get { return _PremiseNumberRangeFrom; }
            set { _PremiseNumberRangeFrom = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberRangeTo), ElementName = "PremiseNumberRangeTo", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PremiseNumberRangeTo PremiseNumberRangeTo
        {
            get { return _PremiseNumberRangeTo; }
            set { _PremiseNumberRangeTo = value; }
        }

        public void MakeSchemaCompliant()
        {
            PremiseNumberRangeFrom.MakeSchemaCompliant();
            PremiseNumberRangeTo.MakeSchemaCompliant();
        }
    }
}