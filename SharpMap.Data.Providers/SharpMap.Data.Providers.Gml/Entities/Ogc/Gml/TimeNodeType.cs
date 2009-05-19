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
    [Serializable, XmlType(TypeName = "TimeNodeType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TimeNodeType : AbstractTimeTopologyPrimitiveType
    {
        [XmlIgnore] private List<TimeEdgePropertyType> _nextEdge;
        [XmlIgnore] private Position _position;
        [XmlIgnore] private List<TimeEdgePropertyType> _previousEdge;

        [XmlElement(Type = typeof (TimeEdgePropertyType), ElementName = "nextEdge", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<TimeEdgePropertyType> NextEdge
        {
            get
            {
                if (_nextEdge == null)
                {
                    _nextEdge = new List<TimeEdgePropertyType>();
                }
                return _nextEdge;
            }
            set { _nextEdge = value; }
        }

        [XmlElement(Type = typeof (Position), ElementName = "position", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        [XmlElement(Type = typeof (TimeEdgePropertyType), ElementName = "previousEdge", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<TimeEdgePropertyType> PreviousEdge
        {
            get
            {
                if (_previousEdge == null)
                {
                    _previousEdge = new List<TimeEdgePropertyType>();
                }
                return _previousEdge;
            }
            set { _previousEdge = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}