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
    [XmlType(TypeName = "LocationType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class LocationType : AbstractObjectType
    {
        [XmlIgnore] private double _altitude;

        [XmlIgnore] public bool _altitudeSpecified;
        [XmlIgnore] private double _latitude;

        [XmlIgnore] public bool _latitudeSpecified;
        [XmlIgnore] private List<LocationObjectExtensionGroup> _locationObjectExtensionGroup;
        [XmlIgnore] private List<string> _locationSimpleExtensionGroup;
        [XmlIgnore] private double _longitude;

        [XmlIgnore] public bool _longitudeSpecified;

        public LocationType()
        {
            longitude = 0.0;
            latitude = 0.0;
            altitude = 0.0;
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

        [XmlElement(Type = typeof (string), ElementName = "LocationSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LocationSimpleExtensionGroup
        {
            get
            {
                if (_locationSimpleExtensionGroup == null) _locationSimpleExtensionGroup = new List<string>();
                return _locationSimpleExtensionGroup;
            }
            set { _locationSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LocationObjectExtensionGroup), ElementName = "LocationObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LocationObjectExtensionGroup> LocationObjectExtensionGroup
        {
            get
            {
                if (_locationObjectExtensionGroup == null)
                    _locationObjectExtensionGroup = new List<LocationObjectExtensionGroup>();
                return _locationObjectExtensionGroup;
            }
            set { _locationObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}