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
    [XmlType(TypeName = "ListStyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ListStyleType : AbstractSubStyleType
    {
        [XmlIgnore] private byte[] _bgColor;
        [XmlIgnore] private List<ItemIcon> _itemIcon;
        [XmlIgnore] private ListItemType _listItemType;

        [XmlIgnore] public bool _listItemTypeSpecified;
        [XmlIgnore] private List<ListStyleObjectExtensionGroup> _listStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> _listStyleSimpleExtensionGroup;
        [XmlIgnore] private int _maxSnippetLines;

        [XmlIgnore] public bool _maxSnippetLinesSpecified;

        public ListStyleType()
        {
            listItemType = ListItemType.Check;
            maxSnippetLines = 2;
        }


        [XmlElement(ElementName = "listItemType", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public ListItemType listItemType
        {
            get { return _listItemType; }
            set
            {
                _listItemType = value;
                _listItemTypeSpecified = true;
            }
        }

        [XmlElement(ElementName = "bgColor", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] bgColor
        {
            get { return _bgColor; }
            set { _bgColor = value; }
        }

        [XmlElement(Type = typeof (ItemIcon), ElementName = "ItemIcon", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ItemIcon> ItemIcon
        {
            get
            {
                if (_itemIcon == null) _itemIcon = new List<ItemIcon>();
                return _itemIcon;
            }
            set { _itemIcon = value; }
        }


        [XmlElement(ElementName = "maxSnippetLines", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "int", Namespace = Declarations.SchemaVersion)]
        public int maxSnippetLines
        {
            get { return _maxSnippetLines; }
            set
            {
                _maxSnippetLines = value;
                _maxSnippetLinesSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ListStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ListStyleSimpleExtensionGroup
        {
            get
            {
                if (_listStyleSimpleExtensionGroup == null) _listStyleSimpleExtensionGroup = new List<string>();
                return _listStyleSimpleExtensionGroup;
            }
            set { _listStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ListStyleObjectExtensionGroup), ElementName = "ListStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ListStyleObjectExtensionGroup> ListStyleObjectExtensionGroup
        {
            get
            {
                if (_listStyleObjectExtensionGroup == null)
                    _listStyleObjectExtensionGroup = new List<ListStyleObjectExtensionGroup>();
                return _listStyleObjectExtensionGroup;
            }
            set { _listStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}