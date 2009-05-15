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
        [XmlIgnore] private BalloonStyle _BalloonStyle;
        [XmlIgnore] private IconStyle _IconStyle;

        [XmlIgnore] private LabelStyle _LabelStyle;

        [XmlIgnore] private LineStyle _LineStyle;
        [XmlIgnore] private ListStyle _ListStyle;

        [XmlIgnore] private PolyStyle _PolyStyle;
        [XmlIgnore] private List<StyleObjectExtensionGroup> _StyleObjectExtensionGroup;
        [XmlIgnore] private List<string> _StyleSimpleExtensionGroup;

        [XmlElement(Type = typeof (IconStyle), ElementName = "IconStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public IconStyle IconStyle
        {
            get { return _IconStyle; }
            set { _IconStyle = value; }
        }

        [XmlElement(Type = typeof (LabelStyle), ElementName = "LabelStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LabelStyle LabelStyle
        {
            get { return _LabelStyle; }
            set { _LabelStyle = value; }
        }

        [XmlElement(Type = typeof (LineStyle), ElementName = "LineStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LineStyle LineStyle
        {
            get { return _LineStyle; }
            set { _LineStyle = value; }
        }

        [XmlElement(Type = typeof (PolyStyle), ElementName = "PolyStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PolyStyle PolyStyle
        {
            get { return _PolyStyle; }
            set { _PolyStyle = value; }
        }

        [XmlElement(Type = typeof (BalloonStyle), ElementName = "BalloonStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public BalloonStyle BalloonStyle
        {
            get { return _BalloonStyle; }
            set { _BalloonStyle = value; }
        }

        [XmlElement(Type = typeof (ListStyle), ElementName = "ListStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ListStyle ListStyle
        {
            get { return _ListStyle; }
            set { _ListStyle = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "StyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> StyleSimpleExtensionGroup
        {
            get
            {
                if (_StyleSimpleExtensionGroup == null) _StyleSimpleExtensionGroup = new List<string>();
                return _StyleSimpleExtensionGroup;
            }
            set { _StyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (StyleObjectExtensionGroup), ElementName = "StyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<StyleObjectExtensionGroup> StyleObjectExtensionGroup
        {
            get
            {
                if (_StyleObjectExtensionGroup == null)
                    _StyleObjectExtensionGroup = new List<StyleObjectExtensionGroup>();
                return _StyleObjectExtensionGroup;
            }
            set { _StyleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}