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
    [XmlType(TypeName = "AbstractColorStyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public abstract class AbstractColorStyleType : AbstractSubStyleType
    {
        [XmlIgnore] private List<AbstractColorStyleObjectExtensionGroup> __AbstractColorStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> __AbstractColorStyleSimpleExtensionGroup;
        [XmlIgnore] private byte[] __color;

        [XmlIgnore] private ColorMode __colorMode;

        [XmlIgnore] public bool __colorModeSpecified;

        public AbstractColorStyleType()
        {
            colorMode = ColorMode.Normal;
        }

        [XmlElement(ElementName = "color", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] color
        {
            get { return __color; }
            set { __color = value; }
        }


        [XmlElement(ElementName = "colorMode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public ColorMode colorMode
        {
            get { return __colorMode; }
            set
            {
                __colorMode = value;
                __colorModeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "AbstractColorStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> AbstractColorStyleSimpleExtensionGroup
        {
            get
            {
                if (__AbstractColorStyleSimpleExtensionGroup == null)
                    __AbstractColorStyleSimpleExtensionGroup = new List<string>();
                return __AbstractColorStyleSimpleExtensionGroup;
            }
            set { __AbstractColorStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (AbstractColorStyleObjectExtensionGroup),
            ElementName = "AbstractColorStyleObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<AbstractColorStyleObjectExtensionGroup> AbstractColorStyleObjectExtensionGroup
        {
            get
            {
                if (__AbstractColorStyleObjectExtensionGroup == null)
                    __AbstractColorStyleObjectExtensionGroup = new List<AbstractColorStyleObjectExtensionGroup>();
                return __AbstractColorStyleObjectExtensionGroup;
            }
            set { __AbstractColorStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}