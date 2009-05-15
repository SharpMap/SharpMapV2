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
    [XmlType(TypeName = "StyleMapType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class StyleMapType : AbstractStyleSelectorType
    {
        [XmlIgnore] private List<Pair> _pair;
        [XmlIgnore] private List<StyleMapObjectExtensionGroup> _styleMapObjectExtensionGroup;

        [XmlIgnore] private List<string> _styleMapSimpleExtensionGroup;

        [XmlElement(Type = typeof (Pair), ElementName = "Pair", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Pair> Pair
        {
            get
            {
                if (_pair == null) _pair = new List<Pair>();
                return _pair;
            }
            set { _pair = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "StyleMapSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> StyleMapSimpleExtensionGroup
        {
            get
            {
                if (_styleMapSimpleExtensionGroup == null) _styleMapSimpleExtensionGroup = new List<string>();
                return _styleMapSimpleExtensionGroup;
            }
            set { _styleMapSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (StyleMapObjectExtensionGroup), ElementName = "StyleMapObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<StyleMapObjectExtensionGroup> StyleMapObjectExtensionGroup
        {
            get
            {
                if (_styleMapObjectExtensionGroup == null)
                    _styleMapObjectExtensionGroup = new List<StyleMapObjectExtensionGroup>();
                return _styleMapObjectExtensionGroup;
            }
            set { _styleMapObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}