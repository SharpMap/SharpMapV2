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
    [Serializable, XmlType(TypeName = "RingType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class RingType : AbstractRingType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<CurveMember> _curveMember;
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
            get { return CurveMember.Count; }
        }

        [XmlElement(Type = typeof (CurveMember), ElementName = "curveMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<CurveMember> CurveMember
        {
            get
            {
                if (_curveMember == null)
                {
                    _curveMember = new List<CurveMember>();
                }
                return _curveMember;
            }
            set { _curveMember = value; }
        }

        [XmlIgnore]
        public CurveMember this[int index]
        {
            get { return CurveMember[index]; }
        }

        public void Add(CurveMember obj)
        {
            CurveMember.Add(obj);
        }

        public void Clear()
        {
            CurveMember.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return CurveMember.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (CurveMember _c in CurveMember)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(CurveMember obj)
        {
            return CurveMember.Remove(obj);
        }

        public CurveMember Remove(int index)
        {
            CurveMember obj = CurveMember[index];
            CurveMember.Remove(obj);
            return obj;
        }
    }
}