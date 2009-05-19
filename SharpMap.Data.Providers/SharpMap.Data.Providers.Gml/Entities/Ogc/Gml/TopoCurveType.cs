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
    [Serializable, XmlType(TypeName = "TopoCurveType", Namespace = Declarations.SchemaVersion)]
    public class TopoCurveType : AbstractTopologyType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<DirectedEdge> _directedEdge;
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
            get { return DirectedEdge.Count; }
        }

        [XmlElement(Type = typeof (DirectedEdge), ElementName = "directedEdge", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<DirectedEdge> DirectedEdge
        {
            get
            {
                if (_directedEdge == null)
                {
                    _directedEdge = new List<DirectedEdge>();
                }
                return _directedEdge;
            }
            set { _directedEdge = value; }
        }

        [XmlIgnore]
        public DirectedEdge this[int index]
        {
            get { return DirectedEdge[index]; }
        }

        public void Add(DirectedEdge obj)
        {
            DirectedEdge.Add(obj);
        }

        public void Clear()
        {
            DirectedEdge.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return DirectedEdge.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DirectedEdge _c in DirectedEdge)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(DirectedEdge obj)
        {
            return DirectedEdge.Remove(obj);
        }

        public DirectedEdge Remove(int index)
        {
            DirectedEdge obj = DirectedEdge[index];
            DirectedEdge.Remove(obj);
            return obj;
        }
    }
}