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
    [Serializable, XmlInclude(typeof (CompositeCurveType)), XmlInclude(typeof (CurveType)),
     XmlType(TypeName = "CurveArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2"),
     XmlInclude(typeof (OrientableCurveType)), XmlInclude(typeof (LineStringType))]
    public class CurveArrayPropertyType
    {
        [XmlIgnore] private List<AbstractCurve> _abstractCurve;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public CurveArrayPropertyType()
        {
            Owns = false;
        }

        [XmlElement(Type = typeof (AbstractCurve), ElementName = "AbstractCurve", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<AbstractCurve> AbstractCurve
        {
            get
            {
                if (_abstractCurve == null)
                {
                    _abstractCurve = new List<AbstractCurve>();
                }
                return _abstractCurve;
            }
            set { _abstractCurve = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractCurve.Count; }
        }

        [XmlIgnore]
        public AbstractCurve this[int index]
        {
            get { return AbstractCurve[index]; }
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

        public void Add(AbstractCurve obj)
        {
            AbstractCurve.Add(obj);
        }

        public void Clear()
        {
            AbstractCurve.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractCurve.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractCurve _c in AbstractCurve)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractCurve obj)
        {
            return AbstractCurve.Remove(obj);
        }

        public AbstractCurve Remove(int index)
        {
            AbstractCurve obj = AbstractCurve[index];
            AbstractCurve.Remove(obj);
            return obj;
        }
    }
}