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
        [XmlIgnore] private byte[] __bgColor;
        [XmlIgnore] private List<ItemIcon> __ItemIcon;
        [XmlIgnore] private listItemTypeEnumType __listItemType;

        [XmlIgnore] public bool __listItemTypeSpecified;
        [XmlIgnore] private List<ListStyleObjectExtensionGroup> __ListStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> __ListStyleSimpleExtensionGroup;
        [XmlIgnore] private int __maxSnippetLines;

        [XmlIgnore] public bool __maxSnippetLinesSpecified;

        public ListStyleType()
        {
            listItemType = listItemTypeEnumType.check;
            maxSnippetLines = 2;
        }


        [XmlElement(ElementName = "listItemType", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public listItemTypeEnumType listItemType
        {
            get { return __listItemType; }
            set
            {
                __listItemType = value;
                __listItemTypeSpecified = true;
            }
        }

        [XmlElement(ElementName = "bgColor", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] bgColor
        {
            get { return __bgColor; }
            set { __bgColor = value; }
        }

        [XmlElement(Type = typeof (ItemIcon), ElementName = "ItemIcon", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ItemIcon> ItemIcon
        {
            get
            {
                if (__ItemIcon == null) __ItemIcon = new List<ItemIcon>();
                return __ItemIcon;
            }
            set { __ItemIcon = value; }
        }


        [XmlElement(ElementName = "maxSnippetLines", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "int", Namespace = Declarations.SchemaVersion)]
        public int maxSnippetLines
        {
            get { return __maxSnippetLines; }
            set
            {
                __maxSnippetLines = value;
                __maxSnippetLinesSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ListStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ListStyleSimpleExtensionGroup
        {
            get
            {
                if (__ListStyleSimpleExtensionGroup == null) __ListStyleSimpleExtensionGroup = new List<string>();
                return __ListStyleSimpleExtensionGroup;
            }
            set { __ListStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ListStyleObjectExtensionGroup), ElementName = "ListStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ListStyleObjectExtensionGroup> ListStyleObjectExtensionGroup
        {
            get
            {
                if (__ListStyleObjectExtensionGroup == null)
                    __ListStyleObjectExtensionGroup = new List<ListStyleObjectExtensionGroup>();
                return __ListStyleObjectExtensionGroup;
            }
            set { __ListStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}