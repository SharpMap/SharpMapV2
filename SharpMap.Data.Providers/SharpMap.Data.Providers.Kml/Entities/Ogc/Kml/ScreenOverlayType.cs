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
    [XmlType(TypeName = "ScreenOverlayType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ScreenOverlayType : AbstractOverlayType
    {
        [XmlIgnore] private overlayXY __overlayXY;
        [XmlIgnore] private double __rotation;

        [XmlIgnore] public bool __rotationSpecified;
        [XmlIgnore] private rotationXY __rotationXY;
        [XmlIgnore] private List<ScreenOverlayObjectExtensionGroup> __ScreenOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> __ScreenOverlaySimpleExtensionGroup;

        [XmlIgnore] private screenXY __screenXY;
        [XmlIgnore] private size __size;

        public ScreenOverlayType()
        {
            rotation = 0.0;
        }

        [XmlElement(Type = typeof (overlayXY), ElementName = "overlayXY", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public overlayXY overlayXY
        {
            get
            {
                if (__overlayXY == null) __overlayXY = new overlayXY();
                return __overlayXY;
            }
            set { __overlayXY = value; }
        }

        [XmlElement(Type = typeof (screenXY), ElementName = "screenXY", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public screenXY screenXY
        {
            get
            {
                if (__screenXY == null) __screenXY = new screenXY();
                return __screenXY;
            }
            set { __screenXY = value; }
        }

        [XmlElement(Type = typeof (rotationXY), ElementName = "rotationXY", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public rotationXY rotationXY
        {
            get
            {
                if (__rotationXY == null) __rotationXY = new rotationXY();
                return __rotationXY;
            }
            set { __rotationXY = value; }
        }

        [XmlElement(Type = typeof (size), ElementName = "size", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public size size
        {
            get
            {
                if (__size == null) __size = new size();
                return __size;
            }
            set { __size = value; }
        }


        [XmlElement(ElementName = "rotation", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double rotation
        {
            get { return __rotation; }
            set
            {
                __rotation = value;
                __rotationSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ScreenOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ScreenOverlaySimpleExtensionGroup
        {
            get
            {
                if (__ScreenOverlaySimpleExtensionGroup == null)
                    __ScreenOverlaySimpleExtensionGroup = new List<string>();
                return __ScreenOverlaySimpleExtensionGroup;
            }
            set { __ScreenOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ScreenOverlayObjectExtensionGroup), ElementName = "ScreenOverlayObjectExtensionGroup"
            , IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ScreenOverlayObjectExtensionGroup> ScreenOverlayObjectExtensionGroup
        {
            get
            {
                if (__ScreenOverlayObjectExtensionGroup == null)
                    __ScreenOverlayObjectExtensionGroup = new List<ScreenOverlayObjectExtensionGroup>();
                return __ScreenOverlayObjectExtensionGroup;
            }
            set { __ScreenOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}