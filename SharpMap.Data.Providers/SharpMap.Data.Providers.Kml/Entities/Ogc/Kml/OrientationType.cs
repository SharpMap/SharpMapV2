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
    [XmlType(TypeName = "OrientationType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class OrientationType : AbstractObjectType
    {
        [XmlIgnore] private double _heading;

        [XmlIgnore] public bool _headingSpecified;
        [XmlIgnore] private List<OrientationObjectExtensionGroup> _orientationObjectExtensionGroup;
        [XmlIgnore] private List<string> _orientationSimpleExtensionGroup;
        [XmlIgnore] private double _roll;

        [XmlIgnore] public bool _rollSpecified;


        [XmlIgnore] private double _tilt;

        [XmlIgnore] public bool _tiltSpecified;

        public OrientationType()
        {
            heading = 0.0;
            tilt = 0.0;
            roll = 0.0;
        }

        [XmlElement(ElementName = "heading", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double heading
        {
            get { return _heading; }
            set
            {
                _heading = value;
                _headingSpecified = true;
            }
        }


        [XmlElement(ElementName = "tilt", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double tilt
        {
            get { return _tilt; }
            set
            {
                _tilt = value;
                _tiltSpecified = true;
            }
        }


        [XmlElement(ElementName = "roll", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double roll
        {
            get { return _roll; }
            set
            {
                _roll = value;
                _rollSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "OrientationSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> OrientationSimpleExtensionGroup
        {
            get
            {
                if (_orientationSimpleExtensionGroup == null) _orientationSimpleExtensionGroup = new List<string>();
                return _orientationSimpleExtensionGroup;
            }
            set { _orientationSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (OrientationObjectExtensionGroup), ElementName = "OrientationObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<OrientationObjectExtensionGroup> OrientationObjectExtensionGroup
        {
            get
            {
                if (_orientationObjectExtensionGroup == null)
                    _orientationObjectExtensionGroup = new List<OrientationObjectExtensionGroup>();
                return _orientationObjectExtensionGroup;
            }
            set { _orientationObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}