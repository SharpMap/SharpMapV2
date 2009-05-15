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
    [XmlType(TypeName = "IconStyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class IconStyleType : AbstractColorStyleType
    {
        [XmlIgnore] private double __heading;

        [XmlIgnore] public bool __headingSpecified;
        [XmlIgnore] private HotSpot __hotSpot;
        [XmlIgnore] private Icon __Icon;
        [XmlIgnore] private List<IconStyleObjectExtensionGroup> __IconStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> __IconStyleSimpleExtensionGroup;
        [XmlIgnore] private double __scale;

        [XmlIgnore] public bool __scaleSpecified;

        public IconStyleType()
        {
            scale = 1.0;
            heading = 0.0;
        }


        [XmlElement(ElementName = "scale", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double scale
        {
            get { return __scale; }
            set
            {
                __scale = value;
                __scaleSpecified = true;
            }
        }


        [XmlElement(ElementName = "heading", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double heading
        {
            get { return __heading; }
            set
            {
                __heading = value;
                __headingSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Icon), ElementName = "Icon", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Icon Icon
        {
            get
            {
                
                return __Icon;
            }
            set { __Icon = value; }
        }

        [XmlElement(Type = typeof (HotSpot), ElementName = "hotSpot", IsNullable = false, Form = XmlSchemaForm.Qualified
            , Namespace = Declarations.SchemaVersion)]
        public HotSpot hotSpot
        {
            get
            {
                
                return __hotSpot;
            }
            set { __hotSpot = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "IconStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> IconStyleSimpleExtensionGroup
        {
            get
            {
                if (__IconStyleSimpleExtensionGroup == null) __IconStyleSimpleExtensionGroup = new List<string>();
                return __IconStyleSimpleExtensionGroup;
            }
            set { __IconStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (IconStyleObjectExtensionGroup), ElementName = "IconStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<IconStyleObjectExtensionGroup> IconStyleObjectExtensionGroup
        {
            get
            {
                if (__IconStyleObjectExtensionGroup == null)
                    __IconStyleObjectExtensionGroup = new List<IconStyleObjectExtensionGroup>();
                return __IconStyleObjectExtensionGroup;
            }
            set { __IconStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}