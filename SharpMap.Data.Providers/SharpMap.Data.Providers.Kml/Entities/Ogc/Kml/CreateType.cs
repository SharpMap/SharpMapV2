// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
    [XmlType(TypeName = "CreateType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (FolderType))]
    [XmlInclude(typeof (DocumentType))]
    public class CreateType
    {
        [XmlIgnore] private List<AbstractContainerGroup> __AbstractContainerGroup;

        [XmlIgnore]
        public AbstractContainerGroup this[int index]
        {
            get { return AbstractContainerGroup[index]; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractContainerGroup.Count; }
        }

        [XmlElement(Type = typeof (AbstractContainerGroup), ElementName = "AbstractContainerGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractContainerGroup> AbstractContainerGroup
        {
            get
            {
                if (__AbstractContainerGroup == null) __AbstractContainerGroup = new List<AbstractContainerGroup>();
                return __AbstractContainerGroup;
            }
            set { __AbstractContainerGroup = value; }
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractContainerGroup.GetEnumerator();
        }

        public void Add(AbstractContainerGroup obj)
        {
            AbstractContainerGroup.Add(obj);
        }

        public void Clear()
        {
            AbstractContainerGroup.Clear();
        }

        public AbstractContainerGroup Remove(int index)
        {
            AbstractContainerGroup obj = AbstractContainerGroup[index];
            AbstractContainerGroup.Remove(obj);
            return obj;
        }

        public bool Remove(AbstractContainerGroup obj)
        {
            return AbstractContainerGroup.Remove(obj);
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}