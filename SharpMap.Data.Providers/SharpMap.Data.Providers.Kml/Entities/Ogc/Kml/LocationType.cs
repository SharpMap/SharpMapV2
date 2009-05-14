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
        [XmlIgnore] private double __altitude;

        [XmlIgnore] public bool __altitudeSpecified;
        [XmlIgnore] private double __latitude;

        [XmlIgnore] public bool __latitudeSpecified;
        [XmlIgnore] private List<LocationObjectExtensionGroup> __LocationObjectExtensionGroup;
        [XmlIgnore] private List<string> __LocationSimpleExtensionGroup;
        [XmlIgnore] private double __longitude;

        [XmlIgnore] public bool __longitudeSpecified;

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

        [XmlElement(Type = typeof (string), ElementName = "LocationSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LocationSimpleExtensionGroup
        {
            get
            {
                if (__LocationSimpleExtensionGroup == null) __LocationSimpleExtensionGroup = new List<string>();
                return __LocationSimpleExtensionGroup;
            }
            set { __LocationSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LocationObjectExtensionGroup), ElementName = "LocationObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LocationObjectExtensionGroup> LocationObjectExtensionGroup
        {
            get
            {
                if (__LocationObjectExtensionGroup == null)
                    __LocationObjectExtensionGroup = new List<LocationObjectExtensionGroup>();
                return __LocationObjectExtensionGroup;
            }
            set { __LocationObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}