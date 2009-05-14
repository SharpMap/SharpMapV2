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
    [XmlType(TypeName = "ResourceMapType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ResourceMapType : AbstractObjectType
    {
        [XmlIgnore] private List<Alias> __Alias;
        [XmlIgnore] private List<ResourceMapObjectExtensionGroup> __ResourceMapObjectExtensionGroup;

        [XmlIgnore] private List<string> __ResourceMapSimpleExtensionGroup;

        [XmlElement(Type = typeof (Alias), ElementName = "Alias", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Alias> Alias
        {
            get
            {
                if (__Alias == null) __Alias = new List<Alias>();
                return __Alias;
            }
            set { __Alias = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "ResourceMapSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ResourceMapSimpleExtensionGroup
        {
            get
            {
                if (__ResourceMapSimpleExtensionGroup == null) __ResourceMapSimpleExtensionGroup = new List<string>();
                return __ResourceMapSimpleExtensionGroup;
            }
            set { __ResourceMapSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ResourceMapObjectExtensionGroup), ElementName = "ResourceMapObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ResourceMapObjectExtensionGroup> ResourceMapObjectExtensionGroup
        {
            get
            {
                if (__ResourceMapObjectExtensionGroup == null)
                    __ResourceMapObjectExtensionGroup = new List<ResourceMapObjectExtensionGroup>();
                return __ResourceMapObjectExtensionGroup;
            }
            set { __ResourceMapObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}