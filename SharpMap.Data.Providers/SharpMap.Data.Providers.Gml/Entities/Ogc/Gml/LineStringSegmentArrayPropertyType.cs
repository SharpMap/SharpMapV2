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
    [Serializable,
     XmlType(TypeName = "LineStringSegmentArrayPropertyType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class LineStringSegmentArrayPropertyType
    {
        [XmlIgnore] private List<LineStringSegment> _lineStringSegment;

        [XmlIgnore]
        public int Count
        {
            get { return LineStringSegment.Count; }
        }

        [XmlIgnore]
        public LineStringSegment this[int index]
        {
            get { return LineStringSegment[index]; }
        }

        [XmlElement(Type = typeof (LineStringSegment), ElementName = "LineStringSegment", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<LineStringSegment> LineStringSegment
        {
            get
            {
                if (_lineStringSegment == null)
                {
                    _lineStringSegment = new List<LineStringSegment>();
                }
                return _lineStringSegment;
            }
            set { _lineStringSegment = value; }
        }

        public void Add(LineStringSegment obj)
        {
            LineStringSegment.Add(obj);
        }

        public void Clear()
        {
            LineStringSegment.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return LineStringSegment.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (LineStringSegment _c in LineStringSegment)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(LineStringSegment obj)
        {
            return LineStringSegment.Remove(obj);
        }

        public LineStringSegment Remove(int index)
        {
            LineStringSegment obj = LineStringSegment[index];
            LineStringSegment.Remove(obj);
            return obj;
        }
    }
}