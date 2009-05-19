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
    [Serializable, XmlInclude(typeof (LineStringSegmentType)), XmlInclude(typeof (OffsetCurveType)),
     XmlInclude(typeof (ArcStringType)), XmlInclude(typeof (BSplineType)),
     XmlType(TypeName = "CurveSegmentArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (ClothoidType)), XmlInclude(typeof (ArcStringByBulgeType)), XmlInclude(typeof (CubicSplineType)),
     XmlInclude(typeof (ArcByCenterPointType)), XmlInclude(typeof (GeodesicStringType))]
    public class CurveSegmentArrayPropertyType
    {
        [XmlIgnore] private List<AbstractCurveSegment> _abstractCurveSegment;

        [XmlElement(Type = typeof (AbstractCurveSegment), ElementName = "AbstractCurveSegment", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<AbstractCurveSegment> AbstractCurveSegment
        {
            get
            {
                if (_abstractCurveSegment == null)
                {
                    _abstractCurveSegment = new List<AbstractCurveSegment>();
                }
                return _abstractCurveSegment;
            }
            set { _abstractCurveSegment = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractCurveSegment.Count; }
        }

        [XmlIgnore]
        public AbstractCurveSegment this[int index]
        {
            get { return AbstractCurveSegment[index]; }
        }

        public void Add(AbstractCurveSegment obj)
        {
            AbstractCurveSegment.Add(obj);
        }

        public void Clear()
        {
            AbstractCurveSegment.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractCurveSegment.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractCurveSegment _c in AbstractCurveSegment)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractCurveSegment obj)
        {
            return AbstractCurveSegment.Remove(obj);
        }

        public AbstractCurveSegment Remove(int index)
        {
            AbstractCurveSegment obj = AbstractCurveSegment[index];
            AbstractCurveSegment.Remove(obj);
            return obj;
        }
    }
}