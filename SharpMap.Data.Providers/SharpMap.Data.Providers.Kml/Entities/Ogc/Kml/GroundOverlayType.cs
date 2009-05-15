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
    [XmlType(TypeName = "GroundOverlayType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class GroundOverlayType : AbstractOverlayType
    {
        [XmlIgnore] private double __altitude;
        [XmlIgnore] private string __altitudeModeGroup;

        [XmlIgnore] public bool __altitudeSpecified;
        [XmlIgnore] private List<GroundOverlayObjectExtensionGroup> __GroundOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> __GroundOverlaySimpleExtensionGroup;
        [XmlIgnore] private LatLonBox __LatLonBox;

        public GroundOverlayType()
        {
            altitude = 0.0;
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

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return __altitudeModeGroup; }
            set { __altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (LatLonBox), ElementName = "LatLonBox", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LatLonBox LatLonBox
        {
            get
            {
                
                return __LatLonBox;
            }
            set { __LatLonBox = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "GroundOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> GroundOverlaySimpleExtensionGroup
        {
            get
            {
                if (__GroundOverlaySimpleExtensionGroup == null)
                    __GroundOverlaySimpleExtensionGroup = new List<string>();
                return __GroundOverlaySimpleExtensionGroup;
            }
            set { __GroundOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (GroundOverlayObjectExtensionGroup), ElementName = "GroundOverlayObjectExtensionGroup"
            , IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<GroundOverlayObjectExtensionGroup> GroundOverlayObjectExtensionGroup
        {
            get
            {
                if (__GroundOverlayObjectExtensionGroup == null)
                    __GroundOverlayObjectExtensionGroup = new List<GroundOverlayObjectExtensionGroup>();
                return __GroundOverlayObjectExtensionGroup;
            }
            set { __GroundOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}