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
    [XmlType(TypeName = "AliasType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class AliasType : AbstractObjectType
    {
        [XmlIgnore] private List<AliasObjectExtensionGroup> __AliasObjectExtensionGroup;
        [XmlIgnore] private List<string> __AliasSimpleExtensionGroup;
        [XmlIgnore] private string __sourceHref;
        [XmlIgnore] private string __targetHref;

        [XmlElement(ElementName = "targetHref", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string targetHref
        {
            get { return __targetHref; }
            set { __targetHref = value; }
        }

        [XmlElement(ElementName = "sourceHref", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string sourceHref
        {
            get { return __sourceHref; }
            set { __sourceHref = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "AliasSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> AliasSimpleExtensionGroup
        {
            get
            {
                if (__AliasSimpleExtensionGroup == null) __AliasSimpleExtensionGroup = new List<string>();
                return __AliasSimpleExtensionGroup;
            }
            set { __AliasSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (AliasObjectExtensionGroup), ElementName = "AliasObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AliasObjectExtensionGroup> AliasObjectExtensionGroup
        {
            get
            {
                if (__AliasObjectExtensionGroup == null)
                    __AliasObjectExtensionGroup = new List<AliasObjectExtensionGroup>();
                return __AliasObjectExtensionGroup;
            }
            set { __AliasObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}