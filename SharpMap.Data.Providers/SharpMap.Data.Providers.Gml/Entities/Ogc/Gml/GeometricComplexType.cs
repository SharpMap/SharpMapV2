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
    [Serializable, XmlType(TypeName = "GeometricComplexType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class GeometricComplexType : AbstractGeometryType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<GeometricPrimitivePropertyType> _element;
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

        [XmlIgnore]
        public int Count
        {
            get { return Element.Count; }
        }

        [XmlElement(Type = typeof (GeometricPrimitivePropertyType), ElementName = "element", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<GeometricPrimitivePropertyType> Element
        {
            get
            {
                if (_element == null)
                {
                    _element = new List<GeometricPrimitivePropertyType>();
                }
                return _element;
            }
            set { _element = value; }
        }

        [XmlIgnore]
        public GeometricPrimitivePropertyType this[int index]
        {
            get { return Element[index]; }
        }

        public void Add(GeometricPrimitivePropertyType obj)
        {
            Element.Add(obj);
        }

        public void Clear()
        {
            Element.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Element.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (GeometricPrimitivePropertyType _c in Element)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(GeometricPrimitivePropertyType obj)
        {
            return Element.Remove(obj);
        }

        public GeometricPrimitivePropertyType Remove(int index)
        {
            GeometricPrimitivePropertyType obj = Element[index];
            Element.Remove(obj);
            return obj;
        }
    }
}