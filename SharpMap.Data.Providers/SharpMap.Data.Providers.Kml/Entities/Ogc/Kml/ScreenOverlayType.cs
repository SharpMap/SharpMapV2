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
        [XmlIgnore] private OverlayXY _overlayXY;
        [XmlIgnore] private double _rotation;

        [XmlIgnore] public bool _rotationSpecified;
        [XmlIgnore] private RotationXY _rotationXY;
        [XmlIgnore] private List<ScreenOverlayObjectExtensionGroup> _screenOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> _screenOverlaySimpleExtensionGroup;

        [XmlIgnore] private ScreenXY _screenXY;
        [XmlIgnore] private Size _size;

        public ScreenOverlayType()
        {
            _rotation = 0.0;
        }

        [XmlElement(Type = typeof (OverlayXY), ElementName = "overlayXY", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public OverlayXY OverlayXY
        {
            get { return _overlayXY; }
            set { _overlayXY = value; }
        }

        [XmlElement(Type = typeof (ScreenXY), ElementName = "screenXY", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ScreenXY ScreenXY
        {
            get { return _screenXY; }
            set { _screenXY = value; }
        }

        [XmlElement(Type = typeof (RotationXY), ElementName = "rotationXY", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public RotationXY RotationXY
        {
            get { return _rotationXY; }
            set { _rotationXY = value; }
        }

        [XmlElement(Type = typeof (Size), ElementName = "size", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Size Size
        {
            get { return _size; }
            set { _size = value; }
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

        [XmlElement(Type = typeof (string), ElementName = "ScreenOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ScreenOverlaySimpleExtensionGroup
        {
            get
            {
                if (_screenOverlaySimpleExtensionGroup == null)
                    _screenOverlaySimpleExtensionGroup = new List<string>();
                return _screenOverlaySimpleExtensionGroup;
            }
            set { _screenOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ScreenOverlayObjectExtensionGroup), ElementName = "ScreenOverlayObjectExtensionGroup"
            , IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ScreenOverlayObjectExtensionGroup> ScreenOverlayObjectExtensionGroup
        {
            get
            {
                if (_screenOverlayObjectExtensionGroup == null)
                    _screenOverlayObjectExtensionGroup = new List<ScreenOverlayObjectExtensionGroup>();
                return _screenOverlayObjectExtensionGroup;
            }
            set { _screenOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}