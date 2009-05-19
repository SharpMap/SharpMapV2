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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "EdgeType", Namespace = Declarations.SchemaVersion)]
    public class EdgeType : AbstractTopoPrimitiveType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private TopoSolidPropertyType _container;
        [XmlIgnore] private CurveProperty _curveProperty;
        [XmlIgnore] private List<DirectedFace> _directedFace;
        [XmlIgnore] private List<DirectedNode> _directedNode;
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

        [XmlElement(Type = typeof (TopoSolidPropertyType), ElementName = "container", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TopoSolidPropertyType Container
        {
            get { return _container; }
            set { _container = value; }
        }

        [XmlElement(Type = typeof (CurveProperty), ElementName = "curveProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CurveProperty CurveProperty
        {
            get { return _curveProperty; }
            set { _curveProperty = value; }
        }

        [XmlElement(Type = typeof (DirectedFace), ElementName = "directedFace", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
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

        [XmlElement(Type = typeof (DirectedNode), ElementName = "directedNode", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<DirectedNode> DirectedNode
        {
            get
            {
                if (_directedNode == null)
                {
                    _directedNode = new List<DirectedNode>();
                }
                return _directedNode;
            }
            set { _directedNode = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DirectedNode _c in DirectedNode)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}