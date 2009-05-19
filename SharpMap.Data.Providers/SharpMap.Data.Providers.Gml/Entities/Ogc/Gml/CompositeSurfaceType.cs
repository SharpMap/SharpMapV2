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
    [Serializable, XmlType(TypeName = "CompositeSurfaceType", Namespace = Declarations.SchemaVersion)]
    public class CompositeSurfaceType : AbstractSurfaceType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<SurfaceMember> _surfaceMember;
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
            get { return SurfaceMember.Count; }
        }

        [XmlIgnore]
        public SurfaceMember this[int index]
        {
            get { return SurfaceMember[index]; }
        }

        [XmlElement(Type = typeof (SurfaceMember), ElementName = "surfaceMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SurfaceMember> SurfaceMember
        {
            get
            {
                if (_surfaceMember == null)
                {
                    _surfaceMember = new List<SurfaceMember>();
                }
                return _surfaceMember;
            }
            set { _surfaceMember = value; }
        }

        public void Add(SurfaceMember obj)
        {
            SurfaceMember.Add(obj);
        }

        public void Clear()
        {
            SurfaceMember.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return SurfaceMember.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (SurfaceMember _c in SurfaceMember)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(SurfaceMember obj)
        {
            return SurfaceMember.Remove(obj);
        }

        public SurfaceMember Remove(int index)
        {
            SurfaceMember obj = SurfaceMember[index];
            SurfaceMember.Remove(obj);
            return obj;
        }
    }
}