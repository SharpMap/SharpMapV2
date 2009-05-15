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
        [XmlIgnore] private Link _link;

        [XmlIgnore] private Location _location;
        [XmlIgnore] private List<ModelObjectExtensionGroup> _modelObjectExtensionGroup;
        [XmlIgnore] private List<string> _modelSimpleExtensionGroup;

        [XmlIgnore] private Orientation _orientation;
        [XmlIgnore] private ResourceMap _resourceMap;

        [XmlIgnore] private Scale _scale;

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
            get { return _location; }
            set { _location = value; }
        }

        [XmlElement(Type = typeof (Orientation), ElementName = "Orientation", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Orientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        [XmlElement(Type = typeof (Scale), ElementName = "Scale", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Scale Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        [XmlElement(Type = typeof (Link), ElementName = "Link", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Link Link
        {
            get { return _link; }
            set { _link = value; }
        }

        [XmlElement(Type = typeof (ResourceMap), ElementName = "ResourceMap", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ResourceMap ResourceMap
        {
            get { return _resourceMap; }
            set { _resourceMap = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "ModelSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ModelSimpleExtensionGroup
        {
            get
            {
                if (_modelSimpleExtensionGroup == null) _modelSimpleExtensionGroup = new List<string>();
                return _modelSimpleExtensionGroup;
            }
            set { _modelSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ModelObjectExtensionGroup), ElementName = "ModelObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ModelObjectExtensionGroup> ModelObjectExtensionGroup
        {
            get
            {
                if (_modelObjectExtensionGroup == null)
                    _modelObjectExtensionGroup = new List<ModelObjectExtensionGroup>();
                return _modelObjectExtensionGroup;
            }
            set { _modelObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}