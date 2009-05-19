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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TimeCalendarType", Namespace = Declarations.SchemaVersion)]
    public class TimeCalendarType : TimeReferenceSystemType
    {
        [XmlIgnore] private List<TimeCalendarEraPropertyType> _referenceFrame;

        [XmlIgnore]
        public int Count
        {
            get { return ReferenceFrame.Count; }
        }

        [XmlIgnore]
        public TimeCalendarEraPropertyType this[int index]
        {
            get { return ReferenceFrame[index]; }
        }

        [XmlElement(Type = typeof (TimeCalendarEraPropertyType), ElementName = "referenceFrame", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<TimeCalendarEraPropertyType> ReferenceFrame
        {
            get
            {
                if (_referenceFrame == null)
                {
                    _referenceFrame = new List<TimeCalendarEraPropertyType>();
                }
                return _referenceFrame;
            }
            set { _referenceFrame = value; }
        }

        public void Add(TimeCalendarEraPropertyType obj)
        {
            ReferenceFrame.Add(obj);
        }

        public void Clear()
        {
            ReferenceFrame.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return ReferenceFrame.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (TimeCalendarEraPropertyType _c in ReferenceFrame)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(TimeCalendarEraPropertyType obj)
        {
            return ReferenceFrame.Remove(obj);
        }

        public TimeCalendarEraPropertyType Remove(int index)
        {
            TimeCalendarEraPropertyType obj = ReferenceFrame[index];
            ReferenceFrame.Remove(obj);
            return obj;
        }
    }
}