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
    [Serializable, XmlType(TypeName = "TopoSurfaceType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TopoSurfaceType : AbstractTopologyType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<DirectedFace> _directedFace;
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
            get { return DirectedFace.Count; }
        }

        [XmlElement(Type = typeof (DirectedFace), ElementName = "directedFace", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<DirectedFace> DirectedFace
        {
            get
            {
                if (_directedFace == null)
                {
                    _directedFace = new List<DirectedFace>();
                }
                return _directedFace;
            }
            set { _directedFace = value; }
        }

        [XmlIgnore]
        public DirectedFace this[int index]
        {
            get { return DirectedFace[index]; }
        }

        public void Add(DirectedFace obj)
        {
            DirectedFace.Add(obj);
        }

        public void Clear()
        {
            DirectedFace.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return DirectedFace.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DirectedFace _c in DirectedFace)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(DirectedFace obj)
        {
            return DirectedFace.Remove(obj);
        }

        public DirectedFace Remove(int index)
        {
            DirectedFace obj = DirectedFace[index];
            DirectedFace.Remove(obj);
            return obj;
        }
    }
}