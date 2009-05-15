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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "ItemIconType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ItemIconType : AbstractObjectType
    {
        [XmlIgnore] private string _href;
        [XmlIgnore] private List<ItemIconObjectExtensionGroup> _itemIconObjectExtensionGroup;
        [XmlIgnore] private List<string> _itemIconSimpleExtensionGroup;
        [XmlIgnore] private string _state;

        [XmlElement(ElementName = "state", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        [XmlElement(ElementName = "href", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string Href
        {
            get { return _href; }
            set { _href = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "ItemIconSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ItemIconSimpleExtensionGroup
        {
            get
            {
                if (_itemIconSimpleExtensionGroup == null) _itemIconSimpleExtensionGroup = new List<string>();
                return _itemIconSimpleExtensionGroup;
            }
            set { _itemIconSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ItemIconObjectExtensionGroup), ElementName = "ItemIconObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ItemIconObjectExtensionGroup> ItemIconObjectExtensionGroup
        {
            get
            {
                if (_itemIconObjectExtensionGroup == null)
                    _itemIconObjectExtensionGroup = new List<ItemIconObjectExtensionGroup>();
                return _itemIconObjectExtensionGroup;
            }
            set { _itemIconObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}