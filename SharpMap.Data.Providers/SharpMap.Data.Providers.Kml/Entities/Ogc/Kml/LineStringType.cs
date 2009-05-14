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
    [XmlType(TypeName = "LineStringType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class LineStringType : AbstractGeometryType
    {
        [XmlIgnore] private string __altitudeModeGroup;
        [XmlIgnore] private string __coordinates;
        [XmlIgnore] private bool __extrude;

        [XmlIgnore] public bool __extrudeSpecified;
        [XmlIgnore] private List<LineStringObjectExtensionGroup> __LineStringObjectExtensionGroup;
        [XmlIgnore] private List<string> __LineStringSimpleExtensionGroup;


        [XmlIgnore] private bool __tessellate;

        [XmlIgnore] public bool __tessellateSpecified;

        public LineStringType()
        {
            extrude = false;
            tessellate = false;
        }

        [XmlElement(ElementName = "extrude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool extrude
        {
            get { return __extrude; }
            set
            {
                __extrude = value;
                __extrudeSpecified = true;
            }
        }


        [XmlElement(ElementName = "tessellate", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean"
            , Namespace = Declarations.SchemaVersion)]
        public bool tessellate
        {
            get { return __tessellate; }
            set
            {
                __tessellate = value;
                __tessellateSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return __altitudeModeGroup; }
            set { __altitudeModeGroup = value; }
        }

        [XmlElement(ElementName = "coordinates", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string coordinates
        {
            get { return __coordinates; }
            set { __coordinates = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "LineStringSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LineStringSimpleExtensionGroup
        {
            get
            {
                if (__LineStringSimpleExtensionGroup == null) __LineStringSimpleExtensionGroup = new List<string>();
                return __LineStringSimpleExtensionGroup;
            }
            set { __LineStringSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LineStringObjectExtensionGroup), ElementName = "LineStringObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LineStringObjectExtensionGroup> LineStringObjectExtensionGroup
        {
            get
            {
                if (__LineStringObjectExtensionGroup == null)
                    __LineStringObjectExtensionGroup = new List<LineStringObjectExtensionGroup>();
                return __LineStringObjectExtensionGroup;
            }
            set { __LineStringObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}