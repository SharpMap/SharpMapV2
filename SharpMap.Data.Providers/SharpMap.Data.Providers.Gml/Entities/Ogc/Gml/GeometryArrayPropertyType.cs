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
    [Serializable, XmlType(TypeName = "GeometryArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (GridType)), XmlInclude(typeof (AbstractGeometricAggregateType)),
     XmlInclude(typeof (GeometricComplexType)), XmlInclude(typeof (AbstractGeometricPrimitiveType))]
    public class GeometryArrayPropertyType
    {
        [XmlIgnore] private List<AbstractGeometry> _abstractGeometry;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public GeometryArrayPropertyType()
        {
            Owns = false;
        }

        [XmlElement(Type = typeof (AbstractGeometry), ElementName = "AbstractGeometry", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<AbstractGeometry> AbstractGeometry
        {
            get
            {
                if (_abstractGeometry == null)
                {
                    _abstractGeometry = new List<AbstractGeometry>();
                }
                return _abstractGeometry;
            }
            set { _abstractGeometry = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractGeometry.Count; }
        }

        [XmlIgnore]
        public AbstractGeometry this[int index]
        {
            get { return AbstractGeometry[index]; }
        }

        [XmlAttribute(AttributeName = "owns", DataType = "boolean")]
        public bool Owns
        {
            get { return _owns; }
            set
            {
                _owns = value;
                OwnsSpecified = true;
            }
        }

        public void Add(AbstractGeometry obj)
        {
            AbstractGeometry.Add(obj);
        }

        public void Clear()
        {
            AbstractGeometry.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractGeometry.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractGeometry _c in AbstractGeometry)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractGeometry obj)
        {
            return AbstractGeometry.Remove(obj);
        }

        public AbstractGeometry Remove(int index)
        {
            AbstractGeometry obj = AbstractGeometry[index];
            AbstractGeometry.Remove(obj);
            return obj;
        }
    }
}