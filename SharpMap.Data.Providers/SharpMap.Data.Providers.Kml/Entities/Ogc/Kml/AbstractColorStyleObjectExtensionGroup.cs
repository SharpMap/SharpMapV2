// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlRoot(ElementName = "AbstractColorStyleObjectExtensionGroup", Namespace = Declarations.SchemaVersion,
        IsNullable = false), Serializable]
    public abstract class AbstractColorStyleObjectExtensionGroup
    {
        [XmlIgnore] private string _id;
        [XmlIgnore] private List<string> _objectSimpleExtensionGroup;
        [XmlIgnore] private string _targetId;

        [XmlIgnore]
        public string this[int index]
        {
            get { return ObjectSimpleExtensionGroup[index]; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return ObjectSimpleExtensionGroup.Count; }
        }

        [XmlAttribute(AttributeName = "id", DataType = "ID")]
        public string id
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlAttribute(AttributeName = "targetId", DataType = "NCName")]
        public string targetId
        {
            get { return _targetId; }
            set { _targetId = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "ObjectSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ObjectSimpleExtensionGroup
        {
            get
            {
                if (_objectSimpleExtensionGroup == null) _objectSimpleExtensionGroup = new List<string>();
                return _objectSimpleExtensionGroup;
            }
            set { _objectSimpleExtensionGroup = value; }
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return ObjectSimpleExtensionGroup.GetEnumerator();
        }

        public void Add(string obj)
        {
            ObjectSimpleExtensionGroup.Add(obj);
        }

        public void Clear()
        {
            ObjectSimpleExtensionGroup.Clear();
        }

        public string Remove(int index)
        {
            string obj = ObjectSimpleExtensionGroup[index];
            ObjectSimpleExtensionGroup.Remove(obj);
            return obj;
        }

        public bool Remove(string obj)
        {
            return ObjectSimpleExtensionGroup.Remove(obj);
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}