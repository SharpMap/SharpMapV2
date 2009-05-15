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
    [XmlType(TypeName = "DeleteType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (AbstractContainerType))]
    [XmlInclude(typeof (PlacemarkType))]
    [XmlInclude(typeof (NetworkLinkType))]
    [XmlInclude(typeof (AbstractOverlayType))]
    public class DeleteType
    {
        [XmlIgnore] private List<AbstractFeatureGroup> _AbstractFeatureGroup;

        [XmlIgnore]
        public AbstractFeatureGroup this[int index]
        {
            get { return AbstractFeatureGroup[index]; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractFeatureGroup.Count; }
        }

        [XmlElement(Type = typeof (AbstractFeatureGroup), ElementName = "AbstractFeatureGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractFeatureGroup> AbstractFeatureGroup
        {
            get
            {
                if (_AbstractFeatureGroup == null) _AbstractFeatureGroup = new List<AbstractFeatureGroup>();
                return _AbstractFeatureGroup;
            }
            set { _AbstractFeatureGroup = value; }
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractFeatureGroup.GetEnumerator();
        }

        public void Add(AbstractFeatureGroup obj)
        {
            AbstractFeatureGroup.Add(obj);
        }

        public void Clear()
        {
            AbstractFeatureGroup.Clear();
        }

        public AbstractFeatureGroup Remove(int index)
        {
            AbstractFeatureGroup obj = AbstractFeatureGroup[index];
            AbstractFeatureGroup.Remove(obj);
            return obj;
        }

        public bool Remove(AbstractFeatureGroup obj)
        {
            return AbstractFeatureGroup.Remove(obj);
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}