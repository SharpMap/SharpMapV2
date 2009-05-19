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

namespace SharpMap.Entities.Iso.Gco
{
    [Serializable, XmlType(TypeName = "Multiplicity_type", Namespace = "http://www.isotc211.org/2005/gco")]
    public class MultiplicityType : AbstractObjectType
    {
        [XmlIgnore] private List<MultiplicityRangePropertyType> _range;

        [XmlIgnore]
        public int Count
        {
            get { return Range.Count; }
        }

        [XmlIgnore]
        public MultiplicityRangePropertyType this[int index]
        {
            get { return Range[index]; }
        }

        [XmlElement(Type = typeof (MultiplicityRangePropertyType), ElementName = "range", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gco")]
        public List<MultiplicityRangePropertyType> Range
        {
            get
            {
                if (_range == null)
                {
                    _range = new List<MultiplicityRangePropertyType>();
                }
                return _range;
            }
            set { _range = value; }
        }

        public void Add(MultiplicityRangePropertyType obj)
        {
            Range.Add(obj);
        }

        public void Clear()
        {
            Range.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Range.GetEnumerator();
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (MultiplicityRangePropertyType _c in Range)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(MultiplicityRangePropertyType obj)
        {
            return Range.Remove(obj);
        }

        public MultiplicityRangePropertyType Remove(int index)
        {
            MultiplicityRangePropertyType obj = Range[index];
            Range.Remove(obj);
            return obj;
        }
    }
}