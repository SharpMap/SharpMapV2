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
    [XmlType(TypeName = "GroundOverlayType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class GroundOverlayType : AbstractOverlayType
    {
        [XmlIgnore] private double _altitude;
        [XmlIgnore] private string _altitudeModeGroup;

        [XmlIgnore] public bool _altitudeSpecified;
        [XmlIgnore] private List<GroundOverlayObjectExtensionGroup> _groundOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> _groundOverlaySimpleExtensionGroup;
        [XmlIgnore] private LatLonBox _latLonBox;

        public GroundOverlayType()
        {
            _altitude = 0.0;
        }


        [XmlElement(ElementName = "altitude", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "double",
            Namespace = Declarations.SchemaVersion)]
        public double Altitude
        {
            get { return _altitude; }
            set
            {
                _altitude = value;
                _altitudeSpecified = true;
            }
        }

        [XmlElement(ElementName = "altitudeModeGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public string AltitudeModeGroup
        {
            get { return _altitudeModeGroup; }
            set { _altitudeModeGroup = value; }
        }

        [XmlElement(Type = typeof (LatLonBox), ElementName = "LatLonBox", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public LatLonBox LatLonBox
        {
            get { return _latLonBox; }
            set { _latLonBox = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "GroundOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> GroundOverlaySimpleExtensionGroup
        {
            get
            {
                if (_groundOverlaySimpleExtensionGroup == null)
                    _groundOverlaySimpleExtensionGroup = new List<string>();
                return _groundOverlaySimpleExtensionGroup;
            }
            set { _groundOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (GroundOverlayObjectExtensionGroup), ElementName = "GroundOverlayObjectExtensionGroup"
            , IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<GroundOverlayObjectExtensionGroup> GroundOverlayObjectExtensionGroup
        {
            get
            {
                if (_groundOverlayObjectExtensionGroup == null)
                    _groundOverlayObjectExtensionGroup = new List<GroundOverlayObjectExtensionGroup>();
                return _groundOverlayObjectExtensionGroup;
            }
            set { _groundOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}