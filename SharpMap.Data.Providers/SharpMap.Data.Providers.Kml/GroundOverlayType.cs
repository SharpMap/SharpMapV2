using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("GroundOverlay", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class GroundOverlayType : AbstractOverlayType
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;
        private AbstractObjectType[] groundOverlayObjectExtensionGroupField;
        private string[] groundOverlaySimpleExtensionGroupField;

        private altitudeModeEnumType item4Field;

        private LatLonBoxType latLonBoxField;

        public GroundOverlayType()
        {
            altitudeField = 0;
            item4Field = altitudeModeEnumType.clampToGround;
        }

        /// <remarks/>
        public double altitude
        {
            get { return altitudeField; }
            set { altitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool altitudeSpecified
        {
            get { return altitudeFieldSpecified; }
            set { altitudeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item4
        {
            get { return item4Field; }
            set { item4Field = value; }
        }

        /// <remarks/>
        public LatLonBoxType LatLonBox
        {
            get { return latLonBoxField; }
            set { latLonBoxField = value; }
        }

        /// <remarks/>
        [XmlElement("GroundOverlaySimpleExtensionGroup")]
        public string[] GroundOverlaySimpleExtensionGroup
        {
            get { return groundOverlaySimpleExtensionGroupField; }
            set { groundOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("GroundOverlayObjectExtensionGroup")]
        public AbstractObjectType[] GroundOverlayObjectExtensionGroup
        {
            get { return groundOverlayObjectExtensionGroupField; }
            set { groundOverlayObjectExtensionGroupField = value; }
        }
    }
}