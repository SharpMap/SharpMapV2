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
    [XmlType(TypeName = "ScaleType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class ScaleType : AbstractObjectType
    {
        [XmlIgnore] private List<ScaleObjectExtensionGroup> __ScaleObjectExtensionGroup;
        [XmlIgnore] private List<string> __ScaleSimpleExtensionGroup;
        [XmlIgnore] private double __x;

        [XmlIgnore] public bool __xSpecified;


        [XmlIgnore] private double __y;

        [XmlIgnore] public bool __ySpecified;


        [XmlIgnore] private double __z;

        [XmlIgnore] public bool __zSpecified;

        public ScaleType()
        {
            x = 1.0;
            y = 1.0;
            z = 1.0;
        }

        [XmlElement(ElementName = "x", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double x
        {
            get { return __x; }
            set
            {
                __x = value;
                __xSpecified = true;
            }
        }

        [XmlElement(ElementName = "y", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double y
        {
            get { return __y; }
            set
            {
                __y = value;
                __ySpecified = true;
            }
        }


        [XmlElement(ElementName = "z", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double z
        {
            get { return __z; }
            set
            {
                __z = value;
                __zSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "ScaleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> ScaleSimpleExtensionGroup
        {
            get
            {
                if (__ScaleSimpleExtensionGroup == null) __ScaleSimpleExtensionGroup = new List<string>();
                return __ScaleSimpleExtensionGroup;
            }
            set { __ScaleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (ScaleObjectExtensionGroup), ElementName = "ScaleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<ScaleObjectExtensionGroup> ScaleObjectExtensionGroup
        {
            get
            {
                if (__ScaleObjectExtensionGroup == null)
                    __ScaleObjectExtensionGroup = new List<ScaleObjectExtensionGroup>();
                return __ScaleObjectExtensionGroup;
            }
            set { __ScaleObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}