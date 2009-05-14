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
        [XmlIgnore] private double __altitude;
        [XmlIgnore] private string __altitudeModeGroup;

        [XmlIgnore] public bool __altitudeSpecified;
        [XmlIgnore] private List<CameraObjectExtensionGroup> __CameraObjectExtensionGroup;
        [XmlIgnore] private List<string> __CameraSimpleExtensionGroup;


        [XmlIgnore] private double __heading;

        [XmlIgnore] public bool __headingSpecified;
        [XmlIgnore] private double __latitude;

        [XmlIgnore] public bool __latitudeSpecified;
        [XmlIgnore] private double __longitude;

        [XmlIgnore] public bool __longitudeSpecified;
        [XmlIgnore] private double __roll;

        [XmlIgnore] public bool __rollSpecified;


        [XmlIgnore] private double __tilt;

        [XmlIgnore] public bool __tiltSpecified;

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
            get { return __longitude; }
            set
            {
                __longitude = value;
                __longitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "latitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double latitude
        {
            get { return __latitude; }
            set
            {
                __latitude = value;
                __latitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double altitude
        {
            get { return __altitude; }
            set
            {
                __altitude = value;
                __altitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "heading", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double heading
        {
            get { return __heading; }
            set
            {
                __heading = value;
                __headingSpecified = true;
            }
        }


        [XmlElement(ElementName = "tilt", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double tilt
        {
            get { return __tilt; }
            set
            {
                __tilt = value;
                __tiltSpecified = true;
            }
        }


        [XmlElement(ElementName = "roll", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double roll
        {
            get { return __roll; }
            set
            {
                __roll = value;
                __rollSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return __altitudeModeGroup; }
            set { __altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "CameraSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> CameraSimpleExtensionGroup
        {
            get
            {
                if (__CameraSimpleExtensionGroup == null) __CameraSimpleExtensionGroup = new List<string>();
                return __CameraSimpleExtensionGroup;
            }
            set { __CameraSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (CameraObjectExtensionGroup), ElementName = "CameraObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<CameraObjectExtensionGroup> CameraObjectExtensionGroup
        {
            get
            {
                if (__CameraObjectExtensionGroup == null)
                    __CameraObjectExtensionGroup = new List<CameraObjectExtensionGroup>();
                return __CameraObjectExtensionGroup;
            }
            set { __CameraObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}