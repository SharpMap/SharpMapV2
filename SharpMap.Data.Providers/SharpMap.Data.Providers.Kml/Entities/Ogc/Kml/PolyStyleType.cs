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
    [XmlType(TypeName = "PolyStyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class PolyStyleType : AbstractColorStyleType
    {
        [XmlIgnore] private bool __fill;

        [XmlIgnore] public bool __fillSpecified;


        [XmlIgnore] private bool __outline;

        [XmlIgnore] public bool __outlineSpecified;
        [XmlIgnore] private List<PolyStyleObjectExtensionGroup> __PolyStyleObjectExtensionGroup;


        [XmlIgnore] private List<string> __PolyStyleSimpleExtensionGroup;

        public PolyStyleType()
        {
            fill = true;
            outline = true;
        }

        [XmlElement(ElementName = "fill", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool fill
        {
            get { return __fill; }
            set
            {
                __fill = value;
                __fillSpecified = true;
            }
        }

        [XmlElement(ElementName = "outline", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool outline
        {
            get { return __outline; }
            set
            {
                __outline = value;
                __outlineSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "PolyStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> PolyStyleSimpleExtensionGroup
        {
            get
            {
                if (__PolyStyleSimpleExtensionGroup == null) __PolyStyleSimpleExtensionGroup = new List<string>();
                return __PolyStyleSimpleExtensionGroup;
            }
            set { __PolyStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (PolyStyleObjectExtensionGroup), ElementName = "PolyStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PolyStyleObjectExtensionGroup> PolyStyleObjectExtensionGroup
        {
            get
            {
                if (__PolyStyleObjectExtensionGroup == null)
                    __PolyStyleObjectExtensionGroup = new List<PolyStyleObjectExtensionGroup>();
                return __PolyStyleObjectExtensionGroup;
            }
            set { __PolyStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}