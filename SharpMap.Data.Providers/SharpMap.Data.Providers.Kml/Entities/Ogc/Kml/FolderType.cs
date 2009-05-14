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
    [XmlType(TypeName = "FolderType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (AbstractContainerType))]
    [XmlInclude(typeof (PlacemarkType))]
    [XmlInclude(typeof (NetworkLinkType))]
    [XmlInclude(typeof (AbstractOverlayType))]
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
    public class FolderType : AbstractContainerType
    {
        [XmlIgnore] private List<AbstractFeatureGroup> __AbstractFeatureGroup;
        [XmlIgnore] private List<FolderObjectExtensionGroup> __FolderObjectExtensionGroup;

        [XmlIgnore] private List<string> __FolderSimpleExtensionGroup;

        [XmlElement(Type = typeof (AbstractFeatureGroup), ElementName = "AbstractFeatureGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractFeatureGroup> AbstractFeatureGroup
        {
            get
            {
                if (__AbstractFeatureGroup == null) __AbstractFeatureGroup = new List<AbstractFeatureGroup>();
                return __AbstractFeatureGroup;
            }
            set { __AbstractFeatureGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "FolderSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> FolderSimpleExtensionGroup
        {
            get
            {
                if (__FolderSimpleExtensionGroup == null) __FolderSimpleExtensionGroup = new List<string>();
                return __FolderSimpleExtensionGroup;
            }
            set { __FolderSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (FolderObjectExtensionGroup), ElementName = "FolderObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<FolderObjectExtensionGroup> FolderObjectExtensionGroup
        {
            get
            {
                if (__FolderObjectExtensionGroup == null)
                    __FolderObjectExtensionGroup = new List<FolderObjectExtensionGroup>();
                return __FolderObjectExtensionGroup;
            }
            set { __FolderObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}