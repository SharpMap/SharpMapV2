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
    [Serializable, XmlType(TypeName = "TimeCalendarEraType", Namespace = Declarations.SchemaVersion)]
    public class TimeCalendarEraType : DefinitionType
    {
        [XmlIgnore] private TimePeriodPropertyType _epochOfUse;
        [XmlIgnore] private decimal _julianReference;
        [XmlIgnore] private DateTime _referenceDate;
        [XmlIgnore] private StringOrRefType _referenceEvent;
        [XmlIgnore] public bool JulianReferenceSpecified;
        [XmlIgnore] public bool ReferenceDateSpecified;

        public TimeCalendarEraType()
        {
            ReferenceDate = DateTime.Now;
            JulianReferenceSpecified = true;
        }

        [XmlElement(Type = typeof (TimePeriodPropertyType), ElementName = "epochOfUse", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimePeriodPropertyType EpochOfUse
        {
            get { return _epochOfUse; }
            set { _epochOfUse = value; }
        }

        [XmlElement(ElementName = "julianReference", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "decimal", Namespace = Declarations.SchemaVersion)]
        public decimal JulianReference
        {
            get { return _julianReference; }
            set
            {
                _julianReference = value;
                JulianReferenceSpecified = true;
            }
        }

        [XmlElement(ElementName = "referenceDate", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "date"
            , Namespace = Declarations.SchemaVersion)]
        public DateTime ReferenceDate
        {
            get { return _referenceDate; }
            set
            {
                _referenceDate = value;
                ReferenceDateSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime ReferenceDateUtc
        {
            get { return _referenceDate.ToUniversalTime(); }
            set
            {
                _referenceDate = value.ToLocalTime();
                ReferenceDateSpecified = true;
            }
        }

        [XmlElement(Type = typeof (StringOrRefType), ElementName = "referenceEvent", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public StringOrRefType ReferenceEvent
        {
            get { return _referenceEvent; }
            set { _referenceEvent = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            ReferenceEvent.MakeSchemaCompliant();
            EpochOfUse.MakeSchemaCompliant();
        }
    }
}