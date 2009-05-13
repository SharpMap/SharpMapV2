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
    [XmlRoot("Location", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LocationType : AbstractObjectType
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;
        private double latitudeField;

        private bool latitudeFieldSpecified;

        private AbstractObjectType[] locationObjectExtensionGroupField;
        private string[] locationSimpleExtensionGroupField;
        private double longitudeField;

        private bool longitudeFieldSpecified;

        public LocationType()
        {
            longitudeField = 0;
            latitudeField = 0;
            altitudeField = 0;
        }

        /// <remarks/>
        public double longitude
        {
            get { return longitudeField; }
            set { longitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool longitudeSpecified
        {
            get { return longitudeFieldSpecified; }
            set { longitudeFieldSpecified = value; }
        }

        /// <remarks/>
        public double latitude
        {
            get { return latitudeField; }
            set { latitudeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool latitudeSpecified
        {
            get { return latitudeFieldSpecified; }
            set { latitudeFieldSpecified = value; }
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
        [XmlElement("LocationSimpleExtensionGroup")]
        public string[] LocationSimpleExtensionGroup
        {
            get { return locationSimpleExtensionGroupField; }
            set { locationSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LocationObjectExtensionGroup")]
        public AbstractObjectType[] LocationObjectExtensionGroup
        {
            get { return locationObjectExtensionGroupField; }
            set { locationObjectExtensionGroupField = value; }
        }
    }
}