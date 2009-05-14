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
    [XmlType(TypeName = "MultiGeometryType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (MultiGeometryType))]
    [XmlInclude(typeof (LinearRingType))]
    [XmlInclude(typeof (PolygonType))]
    [XmlInclude(typeof (ModelType))]
    [XmlInclude(typeof (LineStringType))]
    [XmlInclude(typeof (PointType))]
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
    public class MultiGeometryType : AbstractGeometryType
    {
        [XmlIgnore] private List<AbstractGeometryGroup> __AbstractGeometryGroup;
        [XmlIgnore] private List<MultiGeometryObjectExtensionGroup> __MultiGeometryObjectExtensionGroup;

        [XmlIgnore] private List<string> __MultiGeometrySimpleExtensionGroup;

        [XmlElement(Type = typeof (AbstractGeometryGroup), ElementName = "AbstractGeometryGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractGeometryGroup> AbstractGeometryGroup
        {
            get
            {
                if (__AbstractGeometryGroup == null) __AbstractGeometryGroup = new List<AbstractGeometryGroup>();
                return __AbstractGeometryGroup;
            }
            set { __AbstractGeometryGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "MultiGeometrySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> MultiGeometrySimpleExtensionGroup
        {
            get
            {
                if (__MultiGeometrySimpleExtensionGroup == null)
                    __MultiGeometrySimpleExtensionGroup = new List<string>();
                return __MultiGeometrySimpleExtensionGroup;
            }
            set { __MultiGeometrySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (MultiGeometryObjectExtensionGroup), ElementName = "MultiGeometryObjectExtensionGroup"
            , IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<MultiGeometryObjectExtensionGroup> MultiGeometryObjectExtensionGroup
        {
            get
            {
                if (__MultiGeometryObjectExtensionGroup == null)
                    __MultiGeometryObjectExtensionGroup = new List<MultiGeometryObjectExtensionGroup>();
                return __MultiGeometryObjectExtensionGroup;
            }
            set { __MultiGeometryObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}