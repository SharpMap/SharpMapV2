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
    [XmlType(TypeName = "LookAtType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class LookAtType : AbstractViewType
    {
        [XmlIgnore] private double _altitude;
        [XmlIgnore] private string _altitudeModeGroup;

        [XmlIgnore] public bool _altitudeSpecified;


        [XmlIgnore] private double _heading;

        [XmlIgnore] public bool _headingSpecified;
        [XmlIgnore] private double _latitude;

        [XmlIgnore] public bool _latitudeSpecified;
        [XmlIgnore] private double _longitude;

        [XmlIgnore] public bool _longitudeSpecified;
        [XmlIgnore] private List<LookAtObjectExtensionGroup> _LookAtObjectExtensionGroup;
        [XmlIgnore] private List<string> _LookAtSimpleExtensionGroup;
        [XmlIgnore] private double _range;

        [XmlIgnore] public bool _rangeSpecified;


        [XmlIgnore] private double _tilt;

        [XmlIgnore] public bool _tiltSpecified;

        public LookAtType()
        {
            longitude = 0.0;
            latitude = 0.0;
            altitude = 0.0;
            heading = 0.0;
            tilt = 0.0;
            range = 0.0;
        }

        [XmlElement(ElementName = "longitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                _longitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "latitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                _latitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double altitude
        {
            get { return _altitude; }
            set
            {
                _altitude = value;
                _altitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "heading", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double heading
        {
            get { return _heading; }
            set
            {
                _heading = value;
                _headingSpecified = true;
            }
        }


        [XmlElement(ElementName = "tilt", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double tilt
        {
            get { return _tilt; }
            set
            {
                _tilt = value;
                _tiltSpecified = true;
            }
        }


        [XmlElement(ElementName = "range", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double range
        {
            get { return _range; }
            set
            {
                _range = value;
                _rangeSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string altitudeModeGroup
        {
            get { return _altitudeModeGroup; }
            set { _altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "LookAtSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LookAtSimpleExtensionGroup
        {
            get
            {
                if (_LookAtSimpleExtensionGroup == null) _LookAtSimpleExtensionGroup = new List<string>();
                return _LookAtSimpleExtensionGroup;
            }
            set { _LookAtSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LookAtObjectExtensionGroup), ElementName = "LookAtObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LookAtObjectExtensionGroup> LookAtObjectExtensionGroup
        {
            get
            {
                if (_LookAtObjectExtensionGroup == null)
                    _LookAtObjectExtensionGroup = new List<LookAtObjectExtensionGroup>();
                return _LookAtObjectExtensionGroup;
            }
            set { _LookAtObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}