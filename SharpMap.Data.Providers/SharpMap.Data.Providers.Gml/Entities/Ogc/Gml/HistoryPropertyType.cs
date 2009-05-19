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
    [Serializable, XmlType(TypeName = "HistoryPropertyType", Namespace = Declarations.SchemaVersion),
     XmlInclude(typeof (MovingObjectStatusType))]
    public class HistoryPropertyType
    {
        [XmlIgnore] private List<AbstractTimeSlice> _abstractTimeSlice;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public HistoryPropertyType()
        {
            Owns = false;
        }

        [XmlElement(Type = typeof (AbstractTimeSlice), ElementName = "AbstractTimeSlice", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractTimeSlice> AbstractTimeSlice
        {
            get
            {
                if (_abstractTimeSlice == null)
                {
                    _abstractTimeSlice = new List<AbstractTimeSlice>();
                }
                return _abstractTimeSlice;
            }
            set { _abstractTimeSlice = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractTimeSlice.Count; }
        }

        [XmlIgnore]
        public AbstractTimeSlice this[int index]
        {
            get { return AbstractTimeSlice[index]; }
        }

        [XmlAttribute(AttributeName = "owns", DataType = "boolean")]
        public bool Owns
        {
            get { return _owns; }
            set
            {
                _owns = value;
                OwnsSpecified = true;
            }
        }

        public void Add(AbstractTimeSlice obj)
        {
            AbstractTimeSlice.Add(obj);
        }

        public void Clear()
        {
            AbstractTimeSlice.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractTimeSlice.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractTimeSlice _c in AbstractTimeSlice)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractTimeSlice obj)
        {
            return AbstractTimeSlice.Remove(obj);
        }

        public AbstractTimeSlice Remove(int index)
        {
            AbstractTimeSlice obj = AbstractTimeSlice[index];
            AbstractTimeSlice.Remove(obj);
            return obj;
        }
    }
}