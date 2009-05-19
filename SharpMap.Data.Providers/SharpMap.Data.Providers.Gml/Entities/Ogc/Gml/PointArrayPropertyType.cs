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
    [Serializable, XmlType(TypeName = "PointArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class PointArrayPropertyType
    {
        [XmlIgnore] private bool _owns;
        [XmlIgnore] private List<Point> _point;
        [XmlIgnore] public bool OwnsSpecified;

        public PointArrayPropertyType()
        {
            Owns = false;
        }

        [XmlIgnore]
        public int Count
        {
            get { return Point.Count; }
        }

        [XmlIgnore]
        public Point this[int index]
        {
            get { return Point[index]; }
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

        [XmlElement(Type = typeof (Point), ElementName = "Point", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public List<Point> Point
        {
            get
            {
                if (_point == null)
                {
                    _point = new List<Point>();
                }
                return _point;
            }
            set { _point = value; }
        }

        public void Add(Point obj)
        {
            Point.Add(obj);
        }

        public void Clear()
        {
            Point.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Point.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (Point _c in Point)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(Point obj)
        {
            return Point.Remove(obj);
        }

        public Point Remove(int index)
        {
            Point obj = Point[index];
            Point.Remove(obj);
            return obj;
        }
    }
}