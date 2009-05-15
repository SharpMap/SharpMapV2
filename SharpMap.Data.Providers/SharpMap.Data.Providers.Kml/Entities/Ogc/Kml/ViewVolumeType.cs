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
    [XmlType(TypeName = "ViewVolumeType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ViewVolumeType : AbstractObjectType
    {
        [XmlIgnore] private double _bottomFov;

        [XmlIgnore] public bool _bottomFovSpecified;
        [XmlIgnore] private double _leftFov;

        [XmlIgnore] public bool _leftFovSpecified;
        [XmlIgnore] private double _near;

        [XmlIgnore] public bool _nearSpecified;


        [XmlIgnore] private double _rightFov;

        [XmlIgnore] public bool _rightFovSpecified;


        [XmlIgnore] private double _topFov;

        [XmlIgnore] public bool _topFovSpecified;
        [XmlIgnore] private List<ViewVolumeObjectExtensionGroup> _ViewVolumeObjectExtensionGroup;
        [XmlIgnore] private List<string> _ViewVolumeSimpleExtensionGroup;

        public ViewVolumeType()
        {
            leftFov = 0.0;
            rightFov = 0.0;
            bottomFov = 0.0;
            topFov = 0.0;
            near = 0.0;
        }

        [XmlElement(ElementName = "leftFov", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double leftFov
        {
            get { return _leftFov; }
            set
            {
                _leftFov = value;
                _leftFovSpecified = true;
            }
        }

        [XmlElement(ElementName = "rightFov", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double rightFov
        {
            get { return _rightFov; }
            set
            {
                _rightFov = value;
                _rightFovSpecified = true;
            }
        }

        [XmlElement(ElementName = "bottomFov", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double bottomFov
        {
            get { return _bottomFov; }
            set
            {
                _bottomFov = value;
                _bottomFovSpecified = true;
            }
        }


        [XmlElement(ElementName = "topFov", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double topFov
        {
            get { return _topFov; }
            set
            {
                _topFov = value;
                _topFovSpecified = true;
            }
        }


        [XmlElement(ElementName = "near", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double near
        {
            get { return _near; }
            set
            {
                _near = value;
                _nearSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ViewVolumeSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ViewVolumeSimpleExtensionGroup
        {
            get
            {
                if (_ViewVolumeSimpleExtensionGroup == null) _ViewVolumeSimpleExtensionGroup = new List<string>();
                return _ViewVolumeSimpleExtensionGroup;
            }
            set { _ViewVolumeSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ViewVolumeObjectExtensionGroup), ElementName = "ViewVolumeObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ViewVolumeObjectExtensionGroup> ViewVolumeObjectExtensionGroup
        {
            get
            {
                if (_ViewVolumeObjectExtensionGroup == null)
                    _ViewVolumeObjectExtensionGroup = new List<ViewVolumeObjectExtensionGroup>();
                return _ViewVolumeObjectExtensionGroup;
            }
            set { _ViewVolumeObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}