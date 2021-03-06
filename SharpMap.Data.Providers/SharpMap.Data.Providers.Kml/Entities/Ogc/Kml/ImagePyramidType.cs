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
        [XmlIgnore] private GridOrigin _gridOrigin;

        [XmlIgnore] public bool _gridOriginSpecified;
        [XmlIgnore] private List<ImagePyramidObjectExtensionGroup> _imagePyramidObjectExtensionGroup;
        [XmlIgnore] private List<string> _imagePyramidSimpleExtensionGroup;
        [XmlIgnore] private int _maxHeight;

        [XmlIgnore] public bool _maxHeightSpecified;
        [XmlIgnore] private int _maxWidth;

        [XmlIgnore] public bool _maxWidthSpecified;
        [XmlIgnore] private int _tileSize;

        [XmlIgnore] public bool _tileSizeSpecified;

        public ImagePyramidType()
        {
            _tileSize = 256;
            _maxWidth = 0;
            _maxHeight = 0;
            _gridOrigin = GridOrigin.LowerLeft;
        }


        [XmlElement(ElementName = "tileSize", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int TileSize
        {
            get { return _tileSize; }
            set
            {
                _tileSize = value;
                _tileSizeSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxWidth", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                _maxWidth = value;
                _maxWidthSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxHeight", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                _maxHeight = value;
                _maxHeightSpecified = true;
            }
        }


        [XmlElement(ElementName = "gridOrigin", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public GridOrigin GridOrigin
        {
            get { return _gridOrigin; }
            set
            {
                _gridOrigin = value;
                _gridOriginSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ImagePyramidSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ImagePyramidSimpleExtensionGroup
        {
            get
            {
                if (_imagePyramidSimpleExtensionGroup == null) _imagePyramidSimpleExtensionGroup = new List<string>();
                return _imagePyramidSimpleExtensionGroup;
            }
            set { _imagePyramidSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ImagePyramidObjectExtensionGroup), ElementName = "ImagePyramidObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ImagePyramidObjectExtensionGroup> ImagePyramidObjectExtensionGroup
        {
            get
            {
                if (_imagePyramidObjectExtensionGroup == null)
                    _imagePyramidObjectExtensionGroup = new List<ImagePyramidObjectExtensionGroup>();
                return _imagePyramidObjectExtensionGroup;
            }
            set { _imagePyramidObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}