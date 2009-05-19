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
    [Serializable, XmlType(TypeName = "TimePeriodType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimePeriodType : AbstractTimeGeometricPrimitiveType
    {
        [XmlIgnore] private TimeInstantPropertyType _begin;
        [XmlIgnore] private TimePositionType _beginPosition;
        [XmlIgnore] private string _duration;
        [XmlIgnore] private TimeInstantPropertyType _end;
        [XmlIgnore] private TimePositionType _endPosition;
        [XmlIgnore] private TimeInterval _timeInterval;

        public TimePeriodType()
        {
            Duration = string.Empty;
        }

        [XmlElement(Type = typeof (TimeInstantPropertyType), ElementName = "begin", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimeInstantPropertyType Begin
        {
            get { return _begin; }
            set { _begin = value; }
        }

        [XmlElement(Type = typeof (TimePositionType), ElementName = "beginPosition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimePositionType BeginPosition
        {
            get { return _beginPosition; }
            set { _beginPosition = value; }
        }

        [XmlElement(ElementName = "duration", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "duration",
            Namespace = "http://www.opengis.net/gml/3.2")]
        public string Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        [XmlElement(Type = typeof (TimeInstantPropertyType), ElementName = "end", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimeInstantPropertyType End
        {
            get { return _end; }
            set { _end = value; }
        }

        [XmlElement(Type = typeof (TimePositionType), ElementName = "endPosition", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimePositionType EndPosition
        {
            get { return _endPosition; }
            set { _endPosition = value; }
        }

        [XmlElement(Type = typeof (TimeInterval), ElementName = "timeInterval", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public TimeInterval TimeInterval
        {
            get { return _timeInterval; }
            set { _timeInterval = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            BeginPosition.MakeSchemaCompliant();
            Begin.MakeSchemaCompliant();
            EndPosition.MakeSchemaCompliant();
            End.MakeSchemaCompliant();
            TimeInterval.MakeSchemaCompliant();
        }
    }
}