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
    [XmlType(TypeName = "ModelType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ModelType : AbstractGeometryType
    {
        [XmlIgnore] private string __altitudeModeGroup;
        [XmlIgnore] private Link __Link;

        [XmlIgnore] private Location __Location;
        [XmlIgnore] private List<ModelObjectExtensionGroup> __ModelObjectExtensionGroup;
        [XmlIgnore] private List<string> __ModelSimpleExtensionGroup;

        [XmlIgnore] private Orientation __Orientation;
        [XmlIgnore] private ResourceMap __ResourceMap;

        [XmlIgnore] private Scale __Scale;

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return __altitudeModeGroup; }
            set { __altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (Location), ElementName = "Location", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Location Location
        {
            get
            {
                if (__Location == null) __Location = new Location();
                return __Location;
            }
            set { __Location = value; }
        }

        [XmlElement(Type = typeof (Orientation), ElementName = "Orientation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Orientation Orientation
        {
            get
            {
                if (__Orientation == null) __Orientation = new Orientation();
                return __Orientation;
            }
            set { __Orientation = value; }
        }

        [XmlElement(Type = typeof (Scale), ElementName = "Scale", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Scale Scale
        {
            get
            {
                if (__Scale == null) __Scale = new Scale();
                return __Scale;
            }
            set { __Scale = value; }
        }

        [XmlElement(Type = typeof (Link), ElementName = "Link", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Link Link
        {
            get
            {
                if (__Link == null) __Link = new Link();
                return __Link;
            }
            set { __Link = value; }
        }

        [XmlElement(Type = typeof (ResourceMap), ElementName = "ResourceMap", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ResourceMap ResourceMap
        {
            get
            {
                if (__ResourceMap == null) __ResourceMap = new ResourceMap();
                return __ResourceMap;
            }
            set { __ResourceMap = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "ModelSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ModelSimpleExtensionGroup
        {
            get
            {
                if (__ModelSimpleExtensionGroup == null) __ModelSimpleExtensionGroup = new List<string>();
                return __ModelSimpleExtensionGroup;
            }
            set { __ModelSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ModelObjectExtensionGroup), ElementName = "ModelObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ModelObjectExtensionGroup> ModelObjectExtensionGroup
        {
            get
            {
                if (__ModelObjectExtensionGroup == null)
                    __ModelObjectExtensionGroup = new List<ModelObjectExtensionGroup>();
                return __ModelObjectExtensionGroup;
            }
            set { __ModelObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}