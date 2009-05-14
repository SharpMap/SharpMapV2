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
    [XmlType(TypeName = "LabelStyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class LabelStyleType : AbstractColorStyleType
    {
        [XmlIgnore] private List<LabelStyleObjectExtensionGroup> __LabelStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> __LabelStyleSimpleExtensionGroup;
        [XmlIgnore] private double __scale;

        [XmlIgnore] public bool __scaleSpecified;

        public LabelStyleType()
        {
            scale = 1.0;
        }


        [XmlElement(ElementName = "scale", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double scale
        {
            get { return __scale; }
            set
            {
                __scale = value;
                __scaleSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "LabelStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LabelStyleSimpleExtensionGroup
        {
            get
            {
                if (__LabelStyleSimpleExtensionGroup == null) __LabelStyleSimpleExtensionGroup = new List<string>();
                return __LabelStyleSimpleExtensionGroup;
            }
            set { __LabelStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LabelStyleObjectExtensionGroup), ElementName = "LabelStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LabelStyleObjectExtensionGroup> LabelStyleObjectExtensionGroup
        {
            get
            {
                if (__LabelStyleObjectExtensionGroup == null)
                    __LabelStyleObjectExtensionGroup = new List<LabelStyleObjectExtensionGroup>();
                return __LabelStyleObjectExtensionGroup;
            }
            set { __LabelStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}