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
    [XmlType(TypeName = "AbstractLatLonBoxType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public abstract class AbstractLatLonBoxType : AbstractObjectType
    {
        [XmlIgnore] private List<AbstractLatLonBoxObjectExtensionGroup> _AbstractLatLonBoxObjectExtensionGroup;
        [XmlIgnore] private List<string> _AbstractLatLonBoxSimpleExtensionGroup;
        [XmlIgnore] private double _east;

        [XmlIgnore] public bool _eastSpecified;
        [XmlIgnore] private double _north;

        [XmlIgnore] public bool _northSpecified;


        [XmlIgnore] private double _south;

        [XmlIgnore] public bool _southSpecified;


        [XmlIgnore] private double _west;

        [XmlIgnore] public bool _westSpecified;

        public AbstractLatLonBoxType()
        {
            north = 180.0;
            south = -180.0;
            east = 180.0;
            west = -180.0;
        }

        [XmlElement(ElementName = "north", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double north
        {
            get { return _north; }
            set
            {
                _north = value;
                _northSpecified = true;
            }
        }

        [XmlElement(ElementName = "south", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double south
        {
            get { return _south; }
            set
            {
                _south = value;
                _southSpecified = true;
            }
        }

        [XmlElement(ElementName = "east", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double east
        {
            get { return _east; }
            set
            {
                _east = value;
                _eastSpecified = true;
            }
        }


        [XmlElement(ElementName = "west", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double west
        {
            get { return _west; }
            set
            {
                _west = value;
                _westSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "AbstractLatLonBoxSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> AbstractLatLonBoxSimpleExtensionGroup
        {
            get
            {
                if (_AbstractLatLonBoxSimpleExtensionGroup == null)
                    _AbstractLatLonBoxSimpleExtensionGroup = new List<string>();
                return _AbstractLatLonBoxSimpleExtensionGroup;
            }
            set { _AbstractLatLonBoxSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (AbstractLatLonBoxObjectExtensionGroup),
            ElementName = "AbstractLatLonBoxObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<AbstractLatLonBoxObjectExtensionGroup> AbstractLatLonBoxObjectExtensionGroup
        {
            get
            {
                if (_AbstractLatLonBoxObjectExtensionGroup == null)
                    _AbstractLatLonBoxObjectExtensionGroup = new List<AbstractLatLonBoxObjectExtensionGroup>();
                return _AbstractLatLonBoxObjectExtensionGroup;
            }
            set { _AbstractLatLonBoxObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}