// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TimeIntervalLengthType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimeIntervalLengthType
    {
        [XmlIgnore] private string _factor;
        [XmlIgnore] private string _radix;
        [XmlIgnore] private TimeUnitType _unit;
        [XmlIgnore] private decimal _value;
        [XmlIgnore] public bool UnitSpecified = true;
        [XmlIgnore] public bool ValueSpecified;

        [XmlAttribute(AttributeName = "factor", DataType = "integer")]
        public string Factor
        {
            get { return _factor; }
            set { _factor = value; }
        }

        [XmlAttribute(AttributeName = "radix", DataType = "positiveInteger")]
        public string Radix
        {
            get { return _radix; }
            set { _radix = value; }
        }

        [XmlAttribute(AttributeName = "unit")]
        public TimeUnitType Unit
        {
            get { return _unit; }
            set
            {
                _unit = value;
                UnitSpecified = true;
            }
        }

        [XmlText(typeof (decimal))]
        public decimal Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}