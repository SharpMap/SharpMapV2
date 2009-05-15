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
        [XmlIgnore] private string _altitudeModeGroup;
        [XmlIgnore] private Link _Link;

        [XmlIgnore] private Location _Location;
        [XmlIgnore] private List<ModelObjectExtensionGroup> _ModelObjectExtensionGroup;
        [XmlIgnore] private List<string> _ModelSimpleExtensionGroup;

        [XmlIgnore] private Orientation _Orientation;
        [XmlIgnore] private ResourceMap _ResourceMap;

        [XmlIgnore] private Scale _Scale;

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return _altitudeModeGroup; }
            set { _altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (Location), ElementName = "Location", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Location Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        [XmlElement(Type = typeof (Orientation), ElementName = "Orientation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Orientation Orientation
        {
            get { return _Orientation; }
            set { _Orientation = value; }
        }

        [XmlElement(Type = typeof (Scale), ElementName = "Scale", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Scale Scale
        {
            get { return _Scale; }
            set { _Scale = value; }
        }

        [XmlElement(Type = typeof (Link), ElementName = "Link", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Link Link
        {
            get { return _Link; }
            set { _Link = value; }
        }

        [XmlElement(Type = typeof (ResourceMap), ElementName = "ResourceMap", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ResourceMap ResourceMap
        {
            get { return _ResourceMap; }
            set { _ResourceMap = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "ModelSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ModelSimpleExtensionGroup
        {
            get
            {
                if (_ModelSimpleExtensionGroup == null) _ModelSimpleExtensionGroup = new List<string>();
                return _ModelSimpleExtensionGroup;
            }
            set { _ModelSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ModelObjectExtensionGroup), ElementName = "ModelObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ModelObjectExtensionGroup> ModelObjectExtensionGroup
        {
            get
            {
                if (_ModelObjectExtensionGroup == null)
                    _ModelObjectExtensionGroup = new List<ModelObjectExtensionGroup>();
                return _ModelObjectExtensionGroup;
            }
            set { _ModelObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}