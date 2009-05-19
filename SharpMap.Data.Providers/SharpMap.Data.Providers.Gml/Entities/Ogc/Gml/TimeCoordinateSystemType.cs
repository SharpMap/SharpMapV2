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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TimeCoordinateSystemType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimeCoordinateSystemType : TimeReferenceSystemType
    {
        [XmlIgnore] private TimeIntervalLengthType _interval;
        [XmlIgnore] private DateTime _origin;
        [XmlIgnore] private TimePositionType _originPosition;
        [XmlIgnore] public bool OriginSpecified;

        public TimeCoordinateSystemType()
        {
            Origin = DateTime.Now;
        }

        [XmlElement(Type = typeof (TimeIntervalLengthType), ElementName = "interval", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimeIntervalLengthType Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        [XmlElement(ElementName = "origin", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public DateTime Origin
        {
            get { return _origin; }
            set
            {
                _origin = value;
                OriginSpecified = true;
            }
        }

        [XmlElement(Type = typeof (TimePositionType), ElementName = "originPosition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimePositionType OriginPosition
        {
            get { return _originPosition; }
            set { _originPosition = value; }
        }

        [XmlIgnore]
        public DateTime OriginUtc
        {
            get { return _origin.ToUniversalTime(); }
            set
            {
                _origin = value.ToLocalTime();
                OriginSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            OriginPosition.MakeSchemaCompliant();
            Interval.MakeSchemaCompliant();
        }
    }
}