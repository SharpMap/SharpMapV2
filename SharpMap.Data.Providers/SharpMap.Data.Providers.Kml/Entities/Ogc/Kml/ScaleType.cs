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
    [XmlType(TypeName = "ScaleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ScaleType : AbstractObjectType
    {
        [XmlIgnore] private List<ScaleObjectExtensionGroup> _scaleObjectExtensionGroup;
        [XmlIgnore] private List<string> _scaleSimpleExtensionGroup;
        [XmlIgnore] private double _x;

        [XmlIgnore] public bool _xSpecified;


        [XmlIgnore] private double _y;

        [XmlIgnore] public bool _ySpecified;


        [XmlIgnore] private double _z;

        [XmlIgnore] public bool _zSpecified;

        public ScaleType()
        {
            x = 1.0;
            y = 1.0;
            z = 1.0;
        }

        [XmlElement(ElementName = "x", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double x
        {
            get { return _x; }
            set
            {
                _x = value;
                _xSpecified = true;
            }
        }

        [XmlElement(ElementName = "y", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double y
        {
            get { return _y; }
            set
            {
                _y = value;
                _ySpecified = true;
            }
        }


        [XmlElement(ElementName = "z", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double z
        {
            get { return _z; }
            set
            {
                _z = value;
                _zSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ScaleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ScaleSimpleExtensionGroup
        {
            get
            {
                if (_scaleSimpleExtensionGroup == null) _scaleSimpleExtensionGroup = new List<string>();
                return _scaleSimpleExtensionGroup;
            }
            set { _scaleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ScaleObjectExtensionGroup), ElementName = "ScaleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ScaleObjectExtensionGroup> ScaleObjectExtensionGroup
        {
            get
            {
                if (_scaleObjectExtensionGroup == null)
                    _scaleObjectExtensionGroup = new List<ScaleObjectExtensionGroup>();
                return _scaleObjectExtensionGroup;
            }
            set { _scaleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}