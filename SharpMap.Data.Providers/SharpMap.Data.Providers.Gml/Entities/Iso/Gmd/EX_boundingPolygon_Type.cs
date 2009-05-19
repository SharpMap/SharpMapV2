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
using SharpMap.Entities.Iso.Gss;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "EX_boundingPolygon_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class EX_boundingPolygon_Type : AbstractEX_geographicExtent_Type
    {
        [XmlIgnore] private List<GM_object_PropertyType> _polygon;

        [XmlIgnore]
        public int Count
        {
            get { return Polygon.Count; }
        }

        [XmlIgnore]
        public GM_object_PropertyType this[int index]
        {
            get { return Polygon[index]; }
        }

        [XmlElement(Type = typeof (GM_object_PropertyType), ElementName = "polygon", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<GM_object_PropertyType> Polygon
        {
            get
            {
                if (_polygon == null)
                {
                    _polygon = new List<GM_object_PropertyType>();
                }
                return _polygon;
            }
            set { _polygon = value; }
        }

        public void Add(GM_object_PropertyType obj)
        {
            Polygon.Add(obj);
        }

        public void Clear()
        {
            Polygon.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Polygon.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (GM_object_PropertyType _c in Polygon)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(GM_object_PropertyType obj)
        {
            return Polygon.Remove(obj);
        }

        public GM_object_PropertyType Remove(int index)
        {
            GM_object_PropertyType obj = Polygon[index];
            Polygon.Remove(obj);
            return obj;
        }
    }
}