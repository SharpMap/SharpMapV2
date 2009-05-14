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
    [XmlType(TypeName = "ImagePyramidType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ImagePyramidType : AbstractObjectType
    {
        [XmlIgnore] private gridOriginEnumType __gridOrigin;

        [XmlIgnore] public bool __gridOriginSpecified;
        [XmlIgnore] private List<ImagePyramidObjectExtensionGroup> __ImagePyramidObjectExtensionGroup;
        [XmlIgnore] private List<string> __ImagePyramidSimpleExtensionGroup;
        [XmlIgnore] private int __maxHeight;

        [XmlIgnore] public bool __maxHeightSpecified;
        [XmlIgnore] private int __maxWidth;

        [XmlIgnore] public bool __maxWidthSpecified;
        [XmlIgnore] private int __tileSize;

        [XmlIgnore] public bool __tileSizeSpecified;

        public ImagePyramidType()
        {
            tileSize = 256;
            maxWidth = 0;
            maxHeight = 0;
            gridOrigin = gridOriginEnumType.lowerLeft;
        }


        [XmlElement(ElementName = "tileSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int tileSize
        {
            get { return __tileSize; }
            set
            {
                __tileSize = value;
                __tileSizeSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxWidth", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int maxWidth
        {
            get { return __maxWidth; }
            set
            {
                __maxWidth = value;
                __maxWidthSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxHeight", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int maxHeight
        {
            get { return __maxHeight; }
            set
            {
                __maxHeight = value;
                __maxHeightSpecified = true;
            }
        }


        [XmlElement(ElementName = "gridOrigin", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public gridOriginEnumType gridOrigin
        {
            get { return __gridOrigin; }
            set
            {
                __gridOrigin = value;
                __gridOriginSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ImagePyramidSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ImagePyramidSimpleExtensionGroup
        {
            get
            {
                if (__ImagePyramidSimpleExtensionGroup == null) __ImagePyramidSimpleExtensionGroup = new List<string>();
                return __ImagePyramidSimpleExtensionGroup;
            }
            set { __ImagePyramidSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ImagePyramidObjectExtensionGroup), ElementName = "ImagePyramidObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ImagePyramidObjectExtensionGroup> ImagePyramidObjectExtensionGroup
        {
            get
            {
                if (__ImagePyramidObjectExtensionGroup == null)
                    __ImagePyramidObjectExtensionGroup = new List<ImagePyramidObjectExtensionGroup>();
                return __ImagePyramidObjectExtensionGroup;
            }
            set { __ImagePyramidObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}