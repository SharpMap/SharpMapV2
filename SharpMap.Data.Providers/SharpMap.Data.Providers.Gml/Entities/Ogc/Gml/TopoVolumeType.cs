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
    [Serializable, XmlType(TypeName = "TopoVolumeType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class TopoVolumeType : AbstractTopologyType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<DirectedTopoSolid> _directedTopoSolid;
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
            get { return DirectedTopoSolid.Count; }
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

        [XmlIgnore]
        public DirectedTopoSolid this[int index]
        {
            get { return DirectedTopoSolid[index]; }
        }

        public void Add(DirectedTopoSolid obj)
        {
            DirectedTopoSolid.Add(obj);
        }

        public void Clear()
        {
            DirectedTopoSolid.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return DirectedTopoSolid.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DirectedTopoSolid _c in DirectedTopoSolid)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(DirectedTopoSolid obj)
        {
            return DirectedTopoSolid.Remove(obj);
        }

        public DirectedTopoSolid Remove(int index)
        {
            DirectedTopoSolid obj = DirectedTopoSolid[index];
            DirectedTopoSolid.Remove(obj);
            return obj;
        }
    }
}