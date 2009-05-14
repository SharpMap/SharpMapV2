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
    [XmlType(TypeName = "RegionType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class RegionType : AbstractObjectType
    {
        [XmlIgnore] private LatLonAltBox __LatLonAltBox;

        [XmlIgnore] private Lod __Lod;
        [XmlIgnore] private List<RegionObjectExtensionGroup> __RegionObjectExtensionGroup;

        [XmlIgnore] private List<string> __RegionSimpleExtensionGroup;

        [XmlElement(Type = typeof (LatLonAltBox), ElementName = "LatLonAltBox", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LatLonAltBox LatLonAltBox
        {
            get
            {
                if (__LatLonAltBox == null) __LatLonAltBox = new LatLonAltBox();
                return __LatLonAltBox;
            }
            set { __LatLonAltBox = value; }
        }

        [XmlElement(Type = typeof (Lod), ElementName = "Lod", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Lod Lod
        {
            get
            {
                if (__Lod == null) __Lod = new Lod();
                return __Lod;
            }
            set { __Lod = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "RegionSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> RegionSimpleExtensionGroup
        {
            get
            {
                if (__RegionSimpleExtensionGroup == null) __RegionSimpleExtensionGroup = new List<string>();
                return __RegionSimpleExtensionGroup;
            }
            set { __RegionSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (RegionObjectExtensionGroup), ElementName = "RegionObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<RegionObjectExtensionGroup> RegionObjectExtensionGroup
        {
            get
            {
                if (__RegionObjectExtensionGroup == null)
                    __RegionObjectExtensionGroup = new List<RegionObjectExtensionGroup>();
                return __RegionObjectExtensionGroup;
            }
            set { __RegionObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}