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
    [XmlType(TypeName = "PairType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (StyleMapType))]
    [XmlInclude(typeof (StyleType))]
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
    public class PairType : AbstractObjectType
    {
        [XmlIgnore] private AbstractStyleSelectorGroup __AbstractStyleSelectorGroup;
        [XmlIgnore] private styleStateEnumType __key;

        [XmlIgnore] public bool __keySpecified;
        [XmlIgnore] private List<PairObjectExtensionGroup> __PairObjectExtensionGroup;
        [XmlIgnore] private List<string> __PairSimpleExtensionGroup;


        [XmlIgnore] private string __styleUrl;

        public PairType()
        {
            key = styleStateEnumType.normal;
        }

        [XmlElement(ElementName = "key", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public styleStateEnumType key
        {
            get { return __key; }
            set
            {
                __key = value;
                __keySpecified = true;
            }
        }

        [XmlElement(ElementName = "styleUrl", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string styleUrl
        {
            get { return __styleUrl; }
            set { __styleUrl = value; }
        }

        [XmlElement(Type = typeof (AbstractStyleSelectorGroup), ElementName = "AbstractStyleSelectorGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractStyleSelectorGroup AbstractStyleSelectorGroup
        {
            get { return __AbstractStyleSelectorGroup; }
            set { __AbstractStyleSelectorGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "PairSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> PairSimpleExtensionGroup
        {
            get
            {
                if (__PairSimpleExtensionGroup == null) __PairSimpleExtensionGroup = new List<string>();
                return __PairSimpleExtensionGroup;
            }
            set { __PairSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (PairObjectExtensionGroup), ElementName = "PairObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PairObjectExtensionGroup> PairObjectExtensionGroup
        {
            get
            {
                if (__PairObjectExtensionGroup == null)
                    __PairObjectExtensionGroup = new List<PairObjectExtensionGroup>();
                return __PairObjectExtensionGroup;
            }
            set { __PairObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}