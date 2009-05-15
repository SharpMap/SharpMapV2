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
        [XmlIgnore] private ImagePyramid _imagePyramid;
        [XmlIgnore] private List<PhotoOverlayObjectExtensionGroup> _photoOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> _photoOverlaySimpleExtensionGroup;
        [XmlIgnore] private Point _point;
        [XmlIgnore] private double _rotation;

        [XmlIgnore] public bool _rotationSpecified;
        [XmlIgnore] private Shape _shape;

        [XmlIgnore] public bool _shapeSpecified;


        [XmlIgnore] private ViewVolume _viewVolume;

        public PhotoOverlayType()
        {
            _rotation = 0.0;
            _shape = Shape.Rectangle;
        }

        [XmlElement(ElementName = "rotation", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _rotationSpecified = true;
            }
        }

        [XmlElement(Type = typeof (ViewVolume), ElementName = "ViewVolume", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ViewVolume ViewVolume
        {
            get { return _viewVolume; }
            set { _viewVolume = value; }
        }

        [XmlElement(Type = typeof (ImagePyramid), ElementName = "ImagePyramid", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ImagePyramid ImagePyramid
        {
            get { return _imagePyramid; }
            set { _imagePyramid = value; }
        }

        [XmlElement(Type = typeof (Point), ElementName = "Point", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Point Point
        {
            get { return _point; }
            set { _point = value; }
        }


        [XmlElement(ElementName = "shape", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Shape Shape
        {
            get { return _shape; }
            set
            {
                _shape = value;
                _shapeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "PhotoOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> PhotoOverlaySimpleExtensionGroup
        {
            get
            {
                if (_photoOverlaySimpleExtensionGroup == null) _photoOverlaySimpleExtensionGroup = new List<string>();
                return _photoOverlaySimpleExtensionGroup;
            }
            set { _photoOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (PhotoOverlayObjectExtensionGroup), ElementName = "PhotoOverlayObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PhotoOverlayObjectExtensionGroup> PhotoOverlayObjectExtensionGroup
        {
            get
            {
                if (_photoOverlayObjectExtensionGroup == null)
                    _photoOverlayObjectExtensionGroup = new List<PhotoOverlayObjectExtensionGroup>();
                return _photoOverlayObjectExtensionGroup;
            }
            set { _photoOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}