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
    [Serializable, XmlType(TypeName = "TimeOrdinalReferenceSystemType", Namespace = Declarations.SchemaVersion)]
    public class TimeOrdinalReferenceSystemType : TimeReferenceSystemType
    {
        [XmlIgnore] private List<TimeOrdinalEraPropertyType> _component;

        [XmlElement(Type = typeof (TimeOrdinalEraPropertyType), ElementName = "component", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<TimeOrdinalEraPropertyType> Component
        {
            get
            {
                if (_component == null)
                {
                    _component = new List<TimeOrdinalEraPropertyType>();
                }
                return _component;
            }
            set { _component = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return Component.Count; }
        }

        [XmlIgnore]
        public TimeOrdinalEraPropertyType this[int index]
        {
            get { return Component[index]; }
        }

        public void Add(TimeOrdinalEraPropertyType obj)
        {
            Component.Add(obj);
        }

        public void Clear()
        {
            Component.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Component.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (TimeOrdinalEraPropertyType _c in Component)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(TimeOrdinalEraPropertyType obj)
        {
            return Component.Remove(obj);
        }

        public TimeOrdinalEraPropertyType Remove(int index)
        {
            TimeOrdinalEraPropertyType obj = Component[index];
            Component.Remove(obj);
            return obj;
        }
    }
}