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
    [XmlType(TypeName = "StyleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class StyleType : AbstractStyleSelectorType
    {
        [XmlIgnore] private BalloonStyle __BalloonStyle;
        [XmlIgnore] private IconStyle __IconStyle;

        [XmlIgnore] private LabelStyle __LabelStyle;

        [XmlIgnore] private LineStyle __LineStyle;
        [XmlIgnore] private ListStyle __ListStyle;

        [XmlIgnore] private PolyStyle __PolyStyle;
        [XmlIgnore] private List<StyleObjectExtensionGroup> __StyleObjectExtensionGroup;
        [XmlIgnore] private List<string> __StyleSimpleExtensionGroup;

        [XmlElement(Type = typeof (IconStyle), ElementName = "IconStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public IconStyle IconStyle
        {
            get
            {
                
                return __IconStyle;
            }
            set { __IconStyle = value; }
        }

        [XmlElement(Type = typeof (LabelStyle), ElementName = "LabelStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LabelStyle LabelStyle
        {
            get
            {
                
                return __LabelStyle;
            }
            set { __LabelStyle = value; }
        }

        [XmlElement(Type = typeof (LineStyle), ElementName = "LineStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LineStyle LineStyle
        {
            get
            {
                
                return __LineStyle;
            }
            set { __LineStyle = value; }
        }

        [XmlElement(Type = typeof (PolyStyle), ElementName = "PolyStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PolyStyle PolyStyle
        {
            get
            {
                
                return __PolyStyle;
            }
            set { __PolyStyle = value; }
        }

        [XmlElement(Type = typeof (BalloonStyle), ElementName = "BalloonStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public BalloonStyle BalloonStyle
        {
            get
            {
                
                return __BalloonStyle;
            }
            set { __BalloonStyle = value; }
        }

        [XmlElement(Type = typeof (ListStyle), ElementName = "ListStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ListStyle ListStyle
        {
            get
            {
                
                return __ListStyle;
            }
            set { __ListStyle = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "StyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> StyleSimpleExtensionGroup
        {
            get
            {
                if (__StyleSimpleExtensionGroup == null) __StyleSimpleExtensionGroup = new List<string>();
                return __StyleSimpleExtensionGroup;
            }
            set { __StyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (StyleObjectExtensionGroup), ElementName = "StyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<StyleObjectExtensionGroup> StyleObjectExtensionGroup
        {
            get
            {
                if (__StyleObjectExtensionGroup == null)
                    __StyleObjectExtensionGroup = new List<StyleObjectExtensionGroup>();
                return __StyleObjectExtensionGroup;
            }
            set { __StyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}