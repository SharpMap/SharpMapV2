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
    [XmlType(TypeName = "ChangeType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (DataType))]
    [XmlInclude(typeof (AbstractTimePrimitiveType))]
    [XmlInclude(typeof (SchemaDataType))]
    [XmlInclude(typeof (ItemIconType))]
    [XmlInclude(typeof (AbstractLatLonBoxType))]
    [XmlInclude(typeof (OrientationType))]
    [XmlInclude(typeof (AbstractStyleSelectorType))]
    [XmlInclude(typeof (ResourceMapType))]
    [XmlInclude(typeof (LocationType))]
    [XmlInclude(typeof (AbstractSubStyleType))]
    [XmlInclude(typeof (RegionType))]
    [XmlInclude(typeof (AliasType))]
    [XmlInclude(typeof (AbstractViewType))]
    [XmlInclude(typeof (AbstractFeatureType))]
    [XmlInclude(typeof (AbstractGeometryType))]
    [XmlInclude(typeof (BasicLinkType))]
    [XmlInclude(typeof (PairType))]
    [XmlInclude(typeof (ImagePyramidType))]
    [XmlInclude(typeof (ScaleType))]
    [XmlInclude(typeof (LodType))]
    [XmlInclude(typeof (ViewVolumeType))]
    public class ChangeType
    {
        [XmlIgnore] private List<AbstractObjectGroup> __AbstractObjectGroup;

        [XmlIgnore]
        public AbstractObjectGroup this[int index]
        {
            get { return AbstractObjectGroup[index]; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return AbstractObjectGroup.Count; }
        }

        [XmlElement(Type = typeof (AbstractObjectGroup), ElementName = "AbstractObjectGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractObjectGroup> AbstractObjectGroup
        {
            get
            {
                if (__AbstractObjectGroup == null) __AbstractObjectGroup = new List<AbstractObjectGroup>();
                return __AbstractObjectGroup;
            }
            set { __AbstractObjectGroup = value; }
        }

        [DispId(-4)]
        public IEnumerator GetEnumerator()
        {
            return AbstractObjectGroup.GetEnumerator();
        }

        public void Add(AbstractObjectGroup obj)
        {
            AbstractObjectGroup.Add(obj);
        }

        public void Clear()
        {
            AbstractObjectGroup.Clear();
        }

        public AbstractObjectGroup Remove(int index)
        {
            AbstractObjectGroup obj = AbstractObjectGroup[index];
            AbstractObjectGroup.Remove(obj);
            return obj;
        }

        public bool Remove(AbstractObjectGroup obj)
        {
            return AbstractObjectGroup.Remove(obj);
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}