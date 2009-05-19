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
    [Serializable, XmlInclude(typeof (TriangleType)),
     XmlType(TypeName = "SurfacePatchArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (RectangleType)), XmlInclude(typeof (AbstractParametricCurveSurfaceType)),
     XmlInclude(typeof (PolygonPatchType))]
    public class SurfacePatchArrayPropertyType
    {
        [XmlIgnore] private List<AbstractSurfacePatch> _abstractSurfacePatch;

        [XmlElement(Type = typeof (AbstractSurfacePatch), ElementName = "AbstractSurfacePatch", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<AbstractSurfacePatch> AbstractSurfacePatch
        {
            get
            {
                if (_abstractSurfacePatch == null)
                {
                    _abstractSurfacePatch = new List<AbstractSurfacePatch>();
                }
                return _abstractSurfacePatch;
            }
            set { _abstractSurfacePatch = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractSurfacePatch.Count; }
        }

        [XmlIgnore]
        public AbstractSurfacePatch this[int index]
        {
            get { return AbstractSurfacePatch[index]; }
        }

        public void Add(AbstractSurfacePatch obj)
        {
            AbstractSurfacePatch.Add(obj);
        }

        public void Clear()
        {
            AbstractSurfacePatch.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractSurfacePatch.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractSurfacePatch _c in AbstractSurfacePatch)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractSurfacePatch obj)
        {
            return AbstractSurfacePatch.Remove(obj);
        }

        public AbstractSurfacePatch Remove(int index)
        {
            AbstractSurfacePatch obj = AbstractSurfacePatch[index];
            AbstractSurfacePatch.Remove(obj);
            return obj;
        }
    }
}