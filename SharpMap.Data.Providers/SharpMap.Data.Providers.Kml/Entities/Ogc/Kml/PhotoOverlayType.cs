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
    [XmlType(TypeName = "PhotoOverlayType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class PhotoOverlayType : AbstractOverlayType
    {
        [XmlIgnore] private ImagePyramid __ImagePyramid;
        [XmlIgnore] private List<PhotoOverlayObjectExtensionGroup> __PhotoOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> __PhotoOverlaySimpleExtensionGroup;
        [XmlIgnore] private Point __Point;
        [XmlIgnore] private double __rotation;

        [XmlIgnore] public bool __rotationSpecified;
        [XmlIgnore] private Shape __shape;

        [XmlIgnore] public bool __shapeSpecified;


        [XmlIgnore] private ViewVolume __ViewVolume;

        public PhotoOverlayType()
        {
            rotation = 0.0;
            shape = Shape.Rectangle;
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

        [XmlElement(Type = typeof (ViewVolume), ElementName = "ViewVolume", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ViewVolume ViewVolume
        {
            get
            {
                if (__ViewVolume == null) __ViewVolume = new ViewVolume();
                return __ViewVolume;
            }
            set { __ViewVolume = value; }
        }

        [XmlElement(Type = typeof (ImagePyramid), ElementName = "ImagePyramid", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ImagePyramid ImagePyramid
        {
            get
            {
                if (__ImagePyramid == null) __ImagePyramid = new ImagePyramid();
                return __ImagePyramid;
            }
            set { __ImagePyramid = value; }
        }

        [XmlElement(Type = typeof (Point), ElementName = "Point", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Point Point
        {
            get
            {
                if (__Point == null) __Point = new Point();
                return __Point;
            }
            set { __Point = value; }
        }


        [XmlElement(ElementName = "shape", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Shape shape
        {
            get { return __shape; }
            set
            {
                __shape = value;
                __shapeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "PhotoOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> PhotoOverlaySimpleExtensionGroup
        {
            get
            {
                if (__PhotoOverlaySimpleExtensionGroup == null) __PhotoOverlaySimpleExtensionGroup = new List<string>();
                return __PhotoOverlaySimpleExtensionGroup;
            }
            set { __PhotoOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (PhotoOverlayObjectExtensionGroup), ElementName = "PhotoOverlayObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PhotoOverlayObjectExtensionGroup> PhotoOverlayObjectExtensionGroup
        {
            get
            {
                if (__PhotoOverlayObjectExtensionGroup == null)
                    __PhotoOverlayObjectExtensionGroup = new List<PhotoOverlayObjectExtensionGroup>();
                return __PhotoOverlayObjectExtensionGroup;
            }
            set { __PhotoOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}