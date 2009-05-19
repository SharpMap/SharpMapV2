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
    [Serializable, XmlType(TypeName = "TimeTopologyComplexType", Namespace = Declarations.SchemaVersion)]
    public abstract class TimeTopologyComplexType : AbstractTimeComplexType
    {
        [XmlIgnore] private List<TimeTopologyPrimitivePropertyType> _primitive;

        [XmlIgnore]
        public int Count
        {
            get { return Primitive.Count; }
        }

        [XmlIgnore]
        public TimeTopologyPrimitivePropertyType this[int index]
        {
            get { return Primitive[index]; }
        }

        [XmlElement(Type = typeof (TimeTopologyPrimitivePropertyType), ElementName = "primitive", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<TimeTopologyPrimitivePropertyType> Primitive
        {
            get
            {
                if (_primitive == null)
                {
                    _primitive = new List<TimeTopologyPrimitivePropertyType>();
                }
                return _primitive;
            }
            set { _primitive = value; }
        }

        public void Add(TimeTopologyPrimitivePropertyType obj)
        {
            Primitive.Add(obj);
        }

        public void Clear()
        {
            Primitive.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Primitive.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (TimeTopologyPrimitivePropertyType _c in Primitive)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(TimeTopologyPrimitivePropertyType obj)
        {
            return Primitive.Remove(obj);
        }

        public TimeTopologyPrimitivePropertyType Remove(int index)
        {
            TimeTopologyPrimitivePropertyType obj = Primitive[index];
            Primitive.Remove(obj);
            return obj;
        }
    }
}