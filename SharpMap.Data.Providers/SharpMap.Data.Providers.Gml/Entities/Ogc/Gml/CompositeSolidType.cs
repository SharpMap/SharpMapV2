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
    [Serializable, XmlType(TypeName = "CompositeSolidType", Namespace = Declarations.SchemaVersion)]
    public class CompositeSolidType : AbstractSolidType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<SolidMember> _solidMember;
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
            get { return SolidMember.Count; }
        }

        [XmlIgnore]
        public SolidMember this[int index]
        {
            get { return SolidMember[index]; }
        }

        [XmlElement(Type = typeof (SolidMember), ElementName = "solidMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<SolidMember> SolidMember
        {
            get
            {
                if (_solidMember == null)
                {
                    _solidMember = new List<SolidMember>();
                }
                return _solidMember;
            }
            set { _solidMember = value; }
        }

        public void Add(SolidMember obj)
        {
            SolidMember.Add(obj);
        }

        public void Clear()
        {
            SolidMember.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return SolidMember.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (SolidMember _c in SolidMember)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(SolidMember obj)
        {
            return SolidMember.Remove(obj);
        }

        public SolidMember Remove(int index)
        {
            SolidMember obj = SolidMember[index];
            SolidMember.Remove(obj);
            return obj;
        }
    }
}