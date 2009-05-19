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
    [Serializable, XmlType(TypeName = "AbstractTimePrimitiveType", Namespace = Declarations.SchemaVersion)]
    public abstract class AbstractTimePrimitiveType : AbstractTimeObjectType
    {
        [XmlIgnore] private List<RelatedTimeType> _relatedTime;

        [XmlIgnore]
        public int Count
        {
            get { return RelatedTime.Count; }
        }

        [XmlIgnore]
        public RelatedTimeType this[int index]
        {
            get { return RelatedTime[index]; }
        }

        [XmlElement(Type = typeof (RelatedTimeType), ElementName = "relatedTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<RelatedTimeType> RelatedTime
        {
            get
            {
                if (_relatedTime == null)
                {
                    _relatedTime = new List<RelatedTimeType>();
                }
                return _relatedTime;
            }
            set { _relatedTime = value; }
        }

        public void Add(RelatedTimeType obj)
        {
            RelatedTime.Add(obj);
        }

        public void Clear()
        {
            RelatedTime.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return RelatedTime.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }

        public bool Remove(RelatedTimeType obj)
        {
            return RelatedTime.Remove(obj);
        }

        public RelatedTimeType Remove(int index)
        {
            RelatedTimeType obj = RelatedTime[index];
            RelatedTime.Remove(obj);
            return obj;
        }
    }
}