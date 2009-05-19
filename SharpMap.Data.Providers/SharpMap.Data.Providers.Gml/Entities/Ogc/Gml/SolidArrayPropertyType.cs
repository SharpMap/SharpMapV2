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
    [Serializable, XmlType(TypeName = "SolidArrayPropertyType", Namespace = Declarations.SchemaVersion)]
    public class SolidArrayPropertyType
    {
        [XmlIgnore] private List<AbstractSolid> _abstractSolid;
        [XmlIgnore] private bool _owns;
        [XmlIgnore] public bool OwnsSpecified;

        public SolidArrayPropertyType()
        {
            Owns = false;
        }

        [XmlElement(Type = typeof (AbstractSolid), ElementName = "AbstractSolid", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractSolid> AbstractSolid
        {
            get
            {
                if (_abstractSolid == null)
                {
                    _abstractSolid = new List<AbstractSolid>();
                }
                return _abstractSolid;
            }
            set { _abstractSolid = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractSolid.Count; }
        }

        [XmlIgnore]
        public AbstractSolid this[int index]
        {
            get { return AbstractSolid[index]; }
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

        public void Add(AbstractSolid obj)
        {
            AbstractSolid.Add(obj);
        }

        public void Clear()
        {
            AbstractSolid.Clear();
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractSolid.GetEnumerator();
        }

        public virtual void MakeSchemaCompliant()
        {
            foreach (AbstractSolid _c in AbstractSolid)
            {
                _c.MakeSchemaCompliant();
            }
        }

        public bool Remove(AbstractSolid obj)
        {
            return AbstractSolid.Remove(obj);
        }

        public AbstractSolid Remove(int index)
        {
            AbstractSolid obj = AbstractSolid[index];
            AbstractSolid.Remove(obj);
            return obj;
        }
    }
}