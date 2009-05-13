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
    [XmlRoot("LatLonAltBox", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LatLonAltBoxType : AbstractLatLonBoxType
    {
        private altitudeModeEnumType itemField;

        private AbstractObjectType[] latLonAltBoxObjectExtensionGroupField;
        private string[] latLonAltBoxSimpleExtensionGroupField;
        private double maxAltitudeField;

        private bool maxAltitudeFieldSpecified;
        private double minAltitudeField;

        private bool minAltitudeFieldSpecified;

        public LatLonAltBoxType()
        {
            minAltitudeField = 0;
            maxAltitudeField = 0;
            itemField = altitudeModeEnumType.clampToGround;
        }

        /// <remarks/>
        public double minAltitude
        {
            get { return minAltitudeField; }
            set { minAltitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool minAltitudeSpecified
        {
            get { return minAltitudeFieldSpecified; }
            set { minAltitudeFieldSpecified = value; }
        }

        /// <remarks/>
        public double maxAltitude
        {
            get { return maxAltitudeField; }
            set { maxAltitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxAltitudeSpecified
        {
            get { return maxAltitudeFieldSpecified; }
            set { maxAltitudeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonAltBoxSimpleExtensionGroup")]
        public string[] LatLonAltBoxSimpleExtensionGroup
        {
            get { return latLonAltBoxSimpleExtensionGroupField; }
            set { latLonAltBoxSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LatLonAltBoxObjectExtensionGroup")]
        public AbstractObjectType[] LatLonAltBoxObjectExtensionGroup
        {
            get { return latLonAltBoxObjectExtensionGroupField; }
            set { latLonAltBoxObjectExtensionGroupField = value; }
        }
    }
}