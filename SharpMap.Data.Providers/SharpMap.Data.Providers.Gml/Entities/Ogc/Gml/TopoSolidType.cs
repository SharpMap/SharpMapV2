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
    [Serializable, XmlType(TypeName = "TopoSolidType", Namespace = Declarations.SchemaVersion)]
    public class TopoSolidType : AbstractTopoPrimitiveType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<DirectedFace> _directedFace;
        [XmlIgnore] private List<NodeOrEdgePropertyType> _isolated;
        [XmlIgnore] private SolidProperty _solidProperty;
        [XmlIgnore] private bool _universal;
        [XmlIgnore] public bool AggregationTypeSpecified;
        [XmlIgnore] public bool UniversalSpecified;

        public TopoSolidType()
        {
            Universal = false;
        }

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

        [XmlElement(Type = typeof (NodeOrEdgePropertyType), ElementName = "isolated", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<NodeOrEdgePropertyType> Isolated
        {
            get
            {
                if (_isolated == null)
                {
                    _isolated = new List<NodeOrEdgePropertyType>();
                }
                return _isolated;
            }
            set { _isolated = value; }
        }

        [XmlElement(Type = typeof (SolidProperty), ElementName = "solidProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public SolidProperty SolidProperty
        {
            get { return _solidProperty; }
            set { _solidProperty = value; }
        }

        [XmlAttribute(AttributeName = "universal", DataType = "boolean")]
        public bool Universal
        {
            get { return _universal; }
            set
            {
                _universal = value;
                UniversalSpecified = true;
            }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DirectedFace _c in DirectedFace)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}