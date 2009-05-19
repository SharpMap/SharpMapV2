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
    [Serializable, XmlType(TypeName = "FaceType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class FaceType : AbstractTopoPrimitiveType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<DirectedEdge> _directedEdge;
        [XmlIgnore] private List<DirectedTopoSolid> _directedTopoSolid;
        [XmlIgnore] private List<NodePropertyType> _isolated;
        [XmlIgnore] private SurfaceProperty _surfaceProperty;
        [XmlIgnore] private bool _universal;
        [XmlIgnore] public bool AggregationTypeSpecified;
        [XmlIgnore] public bool UniversalSpecified;

        public FaceType()
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

        [XmlElement(Type = typeof (DirectedTopoSolid), ElementName = "directedTopoSolid", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<DirectedTopoSolid> DirectedTopoSolid
        {
            get
            {
                if (_directedTopoSolid == null)
                {
                    _directedTopoSolid = new List<DirectedTopoSolid>();
                }
                return _directedTopoSolid;
            }
            set { _directedTopoSolid = value; }
        }

        [XmlElement(Type = typeof (NodePropertyType), ElementName = "isolated", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<NodePropertyType> Isolated
        {
            get
            {
                if (_isolated == null)
                {
                    _isolated = new List<NodePropertyType>();
                }
                return _isolated;
            }
            set { _isolated = value; }
        }

        [XmlElement(Type = typeof (SurfaceProperty), ElementName = "surfaceProperty", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public SurfaceProperty SurfaceProperty
        {
            get { return _surfaceProperty; }
            set { _surfaceProperty = value; }
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
            foreach (DirectedEdge _c in DirectedEdge)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}