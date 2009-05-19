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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TimeClockType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimeClockType : TimeReferenceSystemType
    {
        [XmlIgnore] private List<TimeCalendarPropertyType> _dateBasis;
        [XmlIgnore] private StringOrRefType _referenceEvent;
        [XmlIgnore] private DateTime _referenceTime;
        [XmlIgnore] private DateTime _utcReference;
        [XmlIgnore] public bool ReferenceTimeSpecified;
        [XmlIgnore] public bool UtcReferenceSpecified;

        public TimeClockType()
        {
            ReferenceTime = DateTime.Now;
            UtcReference = DateTime.Now;
        }

        [XmlElement(Type = typeof (TimeCalendarPropertyType), ElementName = "dateBasis", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<TimeCalendarPropertyType> DateBasis
        {
            get
            {
                if (_dateBasis == null)
                {
                    _dateBasis = new List<TimeCalendarPropertyType>();
                }
                return _dateBasis;
            }
            set { _dateBasis = value; }
        }

        [XmlElement(Type = typeof (StringOrRefType), ElementName = "referenceEvent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public StringOrRefType ReferenceEvent
        {
            get { return _referenceEvent; }
            set { _referenceEvent = value; }
        }

        [XmlElement(ElementName = "referenceTime", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "time"
            , Namespace = "http://www.opengis.net/gml/3.2")]
        public DateTime ReferenceTime
        {
            get { return _referenceTime; }
            set
            {
                _referenceTime = value;
                ReferenceTimeSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime ReferenceTimeUtc
        {
            get { return _referenceTime.ToUniversalTime(); }
            set
            {
                _referenceTime = value.ToLocalTime();
                ReferenceTimeSpecified = true;
            }
        }

        [XmlElement(ElementName = "utcReference", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "time",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public DateTime UtcReference
        {
            get { return _utcReference; }
            set
            {
                _utcReference = value;
                UtcReferenceSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime UtcReferenceUtc
        {
            get { return _utcReference.ToUniversalTime(); }
            set
            {
                _utcReference = value.ToLocalTime();
                UtcReferenceSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            ReferenceEvent.MakeSchemaCompliant();
        }
    }
}