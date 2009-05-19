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
    [Serializable, XmlType(TypeName = "rows", Namespace = "http://www.opengis.net/gml/3.2")]
    public class RowsCollection
    {
        [XmlIgnore] private List<Row> _row;

        [XmlIgnore]
        public int Count
        {
            get { return Row.Count; }
        }

        [XmlIgnore]
        public Row this[int index]
        {
            get { return Row[index]; }
        }

        [XmlElement(Type = typeof (Row), ElementName = "Row", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.opengis.net/gml/3.2")]
        public List<Row> Row
        {
            get
            {
                if (_row == null)
                {
                    _row = new List<Row>();
                }
                return _row;
            }
            set { _row = value; }
        }

        public void Add(Row obj)
        {
            Row.Add(obj);
        }

        public void Clear()
        {
            Row.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return Row.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (Row _c in Row)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(Row obj)
        {
            return Row.Remove(obj);
        }

        public Row Remove(int index)
        {
            Row obj = Row[index];
            Row.Remove(obj);
            return obj;
        }
    }
}