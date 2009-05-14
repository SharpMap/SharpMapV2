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
        [XmlIgnore] private double __heading;

        [XmlIgnore] public bool __headingSpecified;
        [XmlIgnore] private List<OrientationObjectExtensionGroup> __OrientationObjectExtensionGroup;
        [XmlIgnore] private List<string> __OrientationSimpleExtensionGroup;
        [XmlIgnore] private double __roll;

        [XmlIgnore] public bool __rollSpecified;


        [XmlIgnore] private double __tilt;

        [XmlIgnore] public bool __tiltSpecified;

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

        [XmlElement(Type = typeof (string), ElementName = "OrientationSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> OrientationSimpleExtensionGroup
        {
            get
            {
                if (__OrientationSimpleExtensionGroup == null) __OrientationSimpleExtensionGroup = new List<string>();
                return __OrientationSimpleExtensionGroup;
            }
            set { __OrientationSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (OrientationObjectExtensionGroup), ElementName = "OrientationObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<OrientationObjectExtensionGroup> OrientationObjectExtensionGroup
        {
            get
            {
                if (__OrientationObjectExtensionGroup == null)
                    __OrientationObjectExtensionGroup = new List<OrientationObjectExtensionGroup>();
                return __OrientationObjectExtensionGroup;
            }
            set { __OrientationObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}