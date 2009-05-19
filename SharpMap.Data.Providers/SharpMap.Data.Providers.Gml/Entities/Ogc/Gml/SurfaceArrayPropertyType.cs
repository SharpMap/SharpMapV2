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
    [Serializable, XmlType(TypeName = "SurfaceArrayPropertyType", Namespace = Declarations.SchemaVersion),
     XmlInclude(typeof (CompositeSurfaceType)), XmlInclude(typeof (OrientableSurfaceType)),
     XmlInclude(typeof (PolygonType)), XmlInclude(typeof (SurfaceType))]
    public class SurfaceArrayPropertyType
    {
        [XmlIgnore] private List<AbstractSurface> _abstractSurface;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public SurfaceArrayPropertyType()
        {
            Owns = false;
        }

        [XmlElement(Type = typeof (AbstractSurface), ElementName = "AbstractSurface", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractSurface> AbstractSurface
        {
            get
            {
                if (_abstractSurface == null)
                {
                    _abstractSurface = new List<AbstractSurface>();
                }
                return _abstractSurface;
            }
            set { _abstractSurface = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractSurface.Count; }
        }

        [XmlIgnore]
        public AbstractSurface this[int index]
        {
            get { return AbstractSurface[index]; }
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

        public void Add(AbstractSurface obj)
        {
            AbstractSurface.Add(obj);
        }

        public void Clear()
        {
            AbstractSurface.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractSurface.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractSurface _c in AbstractSurface)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractSurface obj)
        {
            return AbstractSurface.Remove(obj);
        }

        public AbstractSurface Remove(int index)
        {
            AbstractSurface obj = AbstractSurface[index];
            AbstractSurface.Remove(obj);
            return obj;
        }
    }
}