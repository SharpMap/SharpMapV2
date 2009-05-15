// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
        [XmlIgnore] private BalloonStyle _balloonStyle;
        [XmlIgnore] private IconStyle _iconStyle;

        [XmlIgnore] private LabelStyle _labelStyle;

        [XmlIgnore] private LineStyle _lineStyle;
        [XmlIgnore] private ListStyle _listStyle;

        [XmlIgnore] private PolyStyle _polyStyle;
        [XmlIgnore] private List<StyleObjectExtensionGroup> _styleObjectExtensionGroup;
        [XmlIgnore] private List<string> _styleSimpleExtensionGroup;

        [XmlElement(Type = typeof (IconStyle), ElementName = "IconStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public IconStyle IconStyle
        {
            get { return _iconStyle; }
            set { _iconStyle = value; }
        }

        [XmlElement(Type = typeof (LabelStyle), ElementName = "LabelStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LabelStyle LabelStyle
        {
            get { return _labelStyle; }
            set { _labelStyle = value; }
        }

        [XmlElement(Type = typeof (LineStyle), ElementName = "LineStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LineStyle LineStyle
        {
            get { return _lineStyle; }
            set { _lineStyle = value; }
        }

        [XmlElement(Type = typeof (PolyStyle), ElementName = "PolyStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public PolyStyle PolyStyle
        {
            get { return _polyStyle; }
            set { _polyStyle = value; }
        }

        [XmlElement(Type = typeof (BalloonStyle), ElementName = "BalloonStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public BalloonStyle BalloonStyle
        {
            get { return _balloonStyle; }
            set { _balloonStyle = value; }
        }

        [XmlElement(Type = typeof (ListStyle), ElementName = "ListStyle", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ListStyle ListStyle
        {
            get { return _listStyle; }
            set { _listStyle = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "StyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> StyleSimpleExtensionGroup
        {
            get
            {
                if (_styleSimpleExtensionGroup == null) _styleSimpleExtensionGroup = new List<string>();
                return _styleSimpleExtensionGroup;
            }
            set { _styleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (StyleObjectExtensionGroup), ElementName = "StyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<StyleObjectExtensionGroup> StyleObjectExtensionGroup
        {
            get
            {
                if (_styleObjectExtensionGroup == null)
                    _styleObjectExtensionGroup = new List<StyleObjectExtensionGroup>();
                return _styleObjectExtensionGroup;
            }
            set { _styleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}