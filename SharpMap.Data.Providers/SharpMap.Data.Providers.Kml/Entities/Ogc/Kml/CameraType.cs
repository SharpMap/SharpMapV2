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
    [XmlType(TypeName = "CameraType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class CameraType : AbstractViewType
    {
        [XmlIgnore] private double _altitude;
        [XmlIgnore] private string _altitudeModeGroup;

        [XmlIgnore] public bool _altitudeSpecified;
        [XmlIgnore] private List<CameraObjectExtensionGroup> _cameraObjectExtensionGroup;
        [XmlIgnore] private List<string> _cameraSimpleExtensionGroup;


        [XmlIgnore] private double _heading;

        [XmlIgnore] public bool _headingSpecified;
        [XmlIgnore] private double _latitude;

        [XmlIgnore] public bool _latitudeSpecified;
        [XmlIgnore] private double _longitude;

        [XmlIgnore] public bool _longitudeSpecified;
        [XmlIgnore] private double _roll;

        [XmlIgnore] public bool _rollSpecified;


        [XmlIgnore] private double _tilt;

        [XmlIgnore] public bool _tiltSpecified;

        public CameraType()
        {
            longitude = 0.0;
            latitude = 0.0;
            altitude = 0.0;
            heading = 0.0;
            tilt = 0.0;
            roll = 0.0;
        }

        [XmlElement(ElementName = "longitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                _longitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "latitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                _latitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double altitude
        {
            get { return _altitude; }
            set
            {
                _altitude = value;
                _altitudeSpecified = true;
            }
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

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return _altitudeModeGroup; }
            set { _altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "CameraSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> CameraSimpleExtensionGroup
        {
            get
            {
                if (_cameraSimpleExtensionGroup == null) _cameraSimpleExtensionGroup = new List<string>();
                return _cameraSimpleExtensionGroup;
            }
            set { _cameraSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (CameraObjectExtensionGroup), ElementName = "CameraObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<CameraObjectExtensionGroup> CameraObjectExtensionGroup
        {
            get
            {
                if (_cameraObjectExtensionGroup == null)
                    _cameraObjectExtensionGroup = new List<CameraObjectExtensionGroup>();
                return _cameraObjectExtensionGroup;
            }
            set { _cameraObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}