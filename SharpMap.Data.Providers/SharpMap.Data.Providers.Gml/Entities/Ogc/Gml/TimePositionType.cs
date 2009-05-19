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
    [Serializable, XmlType(TypeName = "TimePositionType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimePositionType
    {
        [XmlIgnore] private string _calendarEraName;
        [XmlIgnore] private string _frame;
        [XmlIgnore] private TimeIndeterminateValueType _indeterminatePosition;
        [XmlIgnore] private DateTime _value;
        [XmlIgnore] public bool IndeterminatePositionSpecified;
        [XmlIgnore] public bool ValueSpecified;

        public TimePositionType()
        {
            Frame = "#ISO-8601";
        }

        [XmlAttribute(AttributeName = "calendarEraName", DataType = "string")]
        public string CalendarEraName
        {
            get { return _calendarEraName; }
            set { _calendarEraName = value; }
        }

        [XmlAttribute(AttributeName = "frame", DataType = "anyURI")]
        public string Frame
        {
            get { return _frame; }
            set { _frame = value; }
        }

        [XmlAttribute(AttributeName = "indeterminatePosition")]
        public TimeIndeterminateValueType IndeterminatePosition
        {
            get { return _indeterminatePosition; }
            set
            {
                _indeterminatePosition = value;
                IndeterminatePositionSpecified = true;
            }
        }

        [XmlText(DataType = "dateTime")]
        public DateTime Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime ValueUtc
        {
            get { return _value.ToUniversalTime(); }
            set
            {
                _value = value.ToLocalTime();
                ValueSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}