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
    [XmlType(TypeName = "LodType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class LodType : AbstractObjectType
    {
        [XmlIgnore] private List<LodObjectExtensionGroup> _lodObjectExtensionGroup;
        [XmlIgnore] private List<string> _lodSimpleExtensionGroup;
        [XmlIgnore] private double _maxFadeExtent;

        [XmlIgnore] public bool _maxFadeExtentSpecified;
        [XmlIgnore] private double _maxLodPixels;

        [XmlIgnore] public bool _maxLodPixelsSpecified;


        [XmlIgnore] private double _minFadeExtent;

        [XmlIgnore] public bool _minFadeExtentSpecified;
        [XmlIgnore] private double _minLodPixels;

        [XmlIgnore] public bool _minLodPixelsSpecified;

        public LodType()
        {
            minLodPixels = 0.0;
            maxLodPixels = -1.0;
            minFadeExtent = 0.0;
            maxFadeExtent = 0.0;
        }

        [XmlElement(ElementName = "minLodPixels", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double minLodPixels
        {
            get { return _minLodPixels; }
            set
            {
                _minLodPixels = value;
                _minLodPixelsSpecified = true;
            }
        }

        [XmlElement(ElementName = "maxLodPixels", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double maxLodPixels
        {
            get { return _maxLodPixels; }
            set
            {
                _maxLodPixels = value;
                _maxLodPixelsSpecified = true;
            }
        }


        [XmlElement(ElementName = "minFadeExtent", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double minFadeExtent
        {
            get { return _minFadeExtent; }
            set
            {
                _minFadeExtent = value;
                _minFadeExtentSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxFadeExtent", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double maxFadeExtent
        {
            get { return _maxFadeExtent; }
            set
            {
                _maxFadeExtent = value;
                _maxFadeExtentSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "LodSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LodSimpleExtensionGroup
        {
            get
            {
                if (_lodSimpleExtensionGroup == null) _lodSimpleExtensionGroup = new List<string>();
                return _lodSimpleExtensionGroup;
            }
            set { _lodSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LodObjectExtensionGroup), ElementName = "LodObjectExtensionGroup", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LodObjectExtensionGroup> LodObjectExtensionGroup
        {
            get
            {
                if (_lodObjectExtensionGroup == null) _lodObjectExtensionGroup = new List<LodObjectExtensionGroup>();
                return _lodObjectExtensionGroup;
            }
            set { _lodObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}