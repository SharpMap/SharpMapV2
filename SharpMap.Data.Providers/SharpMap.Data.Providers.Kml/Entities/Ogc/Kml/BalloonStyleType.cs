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
    [XmlType(TypeName = "BalloonStyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class BalloonStyleType : AbstractSubStyleType
    {
        [XmlIgnore] private List<BalloonStyleObjectExtensionGroup> __BalloonStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> __BalloonStyleSimpleExtensionGroup;
        [XmlIgnore] private byte[] __bgColor;
        [XmlIgnore] private byte[] __color;
        [XmlIgnore] private displayModeEnumType __displayMode;

        [XmlIgnore] public bool __displayModeSpecified;
        [XmlIgnore] private string __text;
        [XmlIgnore] private byte[] __textColor;

        public BalloonStyleType()
        {
            displayMode = displayModeEnumType.@default;
        }

        [XmlElement(ElementName = "color", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] color
        {
            get { return __color; }
            set { __color = value; }
        }

        [XmlElement(ElementName = "bgColor", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] bgColor
        {
            get { return __bgColor; }
            set { __bgColor = value; }
        }

        [XmlElement(ElementName = "textColor", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "hexBinary", Namespace = Declarations.SchemaVersion)]
        public byte[] textColor
        {
            get { return __textColor; }
            set { __textColor = value; }
        }

        [XmlElement(ElementName = "text", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string text
        {
            get { return __text; }
            set { __text = value; }
        }


        [XmlElement(ElementName = "displayMode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public displayModeEnumType displayMode
        {
            get { return __displayMode; }
            set
            {
                __displayMode = value;
                __displayModeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "BalloonStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> BalloonStyleSimpleExtensionGroup
        {
            get
            {
                if (__BalloonStyleSimpleExtensionGroup == null) __BalloonStyleSimpleExtensionGroup = new List<string>();
                return __BalloonStyleSimpleExtensionGroup;
            }
            set { __BalloonStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (BalloonStyleObjectExtensionGroup), ElementName = "BalloonStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BalloonStyleObjectExtensionGroup> BalloonStyleObjectExtensionGroup
        {
            get
            {
                if (__BalloonStyleObjectExtensionGroup == null)
                    __BalloonStyleObjectExtensionGroup = new List<BalloonStyleObjectExtensionGroup>();
                return __BalloonStyleObjectExtensionGroup;
            }
            set { __BalloonStyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}