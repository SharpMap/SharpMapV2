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
    [XmlType(TypeName = "LinearRingType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class LinearRingType : AbstractGeometryType
    {
        [XmlIgnore] private string _altitudeModeGroup;
        [XmlIgnore] private string _coordinates;
        [XmlIgnore] private bool _extrude;

        [XmlIgnore] public bool _extrudeSpecified;
        [XmlIgnore] private List<LinearRingObjectExtensionGroup> _LinearRingObjectExtensionGroup;
        [XmlIgnore] private List<string> _LinearRingSimpleExtensionGroup;


        [XmlIgnore] private bool _tessellate;

        [XmlIgnore] public bool _tessellateSpecified;

        public LinearRingType()
        {
            extrude = false;
            tessellate = false;
        }

        [XmlElement(ElementName = "extrude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool extrude
        {
            get { return _extrude; }
            set
            {
                _extrude = value;
                _extrudeSpecified = true;
            }
        }


        [XmlElement(ElementName = "tessellate", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean"
            , Namespace = Declarations.SchemaVersion)]
        public bool tessellate
        {
            get { return _tessellate; }
            set
            {
                _tessellate = value;
                _tessellateSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return _altitudeModeGroup; }
            set { _altitudeModeGroup = value; }
        }

        [XmlElement(ElementName = "coordinates", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string coordinates
        {
            get { return _coordinates; }
            set { _coordinates = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "LinearRingSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LinearRingSimpleExtensionGroup
        {
            get
            {
                if (_LinearRingSimpleExtensionGroup == null) _LinearRingSimpleExtensionGroup = new List<string>();
                return _LinearRingSimpleExtensionGroup;
            }
            set { _LinearRingSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LinearRingObjectExtensionGroup), ElementName = "LinearRingObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LinearRingObjectExtensionGroup> LinearRingObjectExtensionGroup
        {
            get
            {
                if (_LinearRingObjectExtensionGroup == null)
                    _LinearRingObjectExtensionGroup = new List<LinearRingObjectExtensionGroup>();
                return _LinearRingObjectExtensionGroup;
            }
            set { _LinearRingObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}