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
    [Serializable, XmlType(TypeName = "AbstractCoordinateSystemType", Namespace = Declarations.SchemaVersion)]
    public abstract class AbstractCoordinateSystemType : IdentifiedObjectType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<Axis> _axis;
        [XmlIgnore] public bool AggregationTypeSpecified;

        [XmlAttribute(AttributeName = "aggregationType")]
        public AggregationType AggregationType
        {
            get { return _aggregationType; }
            set
            {
                _aggregationType = value;
                AggregationTypeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Axis), ElementName = "axis", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Axis> Axis
        {
            get
            {
                if (_axis == null)
                {
                    _axis = new List<Axis>();
                }
                return _axis;
            }
            set { _axis = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return Axis.Count; }
        }

        [XmlIgnore]
        public Axis this[int index]
        {
            get { return Axis[index]; }
        }

        public void Add(Axis obj)
        {
            Axis.Add(obj);
        }

        public void Clear()
        {
            Axis.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Axis.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (Axis _c in Axis)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(Axis obj)
        {
            return Axis.Remove(obj);
        }

        public Axis Remove(int index)
        {
            Axis obj = Axis[index];
            Axis.Remove(obj);
            return obj;
        }
    }
}