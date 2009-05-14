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
    [XmlType(TypeName = "AbstractStyleSelectorType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public abstract class AbstractStyleSelectorType : AbstractObjectType
    {
        [XmlIgnore] private List<AbstractStyleSelectorObjectExtensionGroup> __AbstractStyleSelectorObjectExtensionGroup;
        [XmlIgnore] private List<string> __AbstractStyleSelectorSimpleExtensionGroup;

        [XmlElement(Type = typeof (string), ElementName = "AbstractStyleSelectorSimpleExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<string> AbstractStyleSelectorSimpleExtensionGroup
        {
            get
            {
                if (__AbstractStyleSelectorSimpleExtensionGroup == null)
                    __AbstractStyleSelectorSimpleExtensionGroup = new List<string>();
                return __AbstractStyleSelectorSimpleExtensionGroup;
            }
            set { __AbstractStyleSelectorSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (AbstractStyleSelectorObjectExtensionGroup),
            ElementName = "AbstractStyleSelectorObjectExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractStyleSelectorObjectExtensionGroup> AbstractStyleSelectorObjectExtensionGroup
        {
            get
            {
                if (__AbstractStyleSelectorObjectExtensionGroup == null)
                    __AbstractStyleSelectorObjectExtensionGroup = new List<AbstractStyleSelectorObjectExtensionGroup>();
                return __AbstractStyleSelectorObjectExtensionGroup;
            }
            set { __AbstractStyleSelectorObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}