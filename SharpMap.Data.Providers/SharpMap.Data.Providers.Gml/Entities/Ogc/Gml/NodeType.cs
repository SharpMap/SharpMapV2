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
    [Serializable, XmlType(TypeName = "NodeType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class NodeType : AbstractTopoPrimitiveType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private FaceOrTopoSolidPropertyType _container;
        [XmlIgnore] private List<DirectedEdge> _directedEdge;
        [XmlIgnore] private PointProperty _pointProperty;
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

        [XmlElement(Type = typeof (FaceOrTopoSolidPropertyType), ElementName = "container", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public FaceOrTopoSolidPropertyType Container
        {
            get { return _container; }
            set { _container = value; }
        }

        [XmlElement(Type = typeof (DirectedEdge), ElementName = "directedEdge", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
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

        [XmlElement(Type = typeof (PointProperty), ElementName = "pointProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public PointProperty PointProperty
        {
            get { return _pointProperty; }
            set { _pointProperty = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}