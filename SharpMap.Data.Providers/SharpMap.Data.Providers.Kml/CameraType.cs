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
    [XmlRoot("Camera", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class CameraType : AbstractViewType
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;
        private AbstractObjectType[] cameraObjectExtensionGroupField;
        private string[] cameraSimpleExtensionGroupField;

        private double headingField;

        private bool headingFieldSpecified;
        private altitudeModeEnumType itemField;
        private double latitudeField;

        private bool latitudeFieldSpecified;
        private double longitudeField;

        private bool longitudeFieldSpecified;

        private double rollField;

        private bool rollFieldSpecified;
        private double tiltField;

        private bool tiltFieldSpecified;

        public CameraType()
        {
            longitudeField = 0;
            latitudeField = 0;
            altitudeField = 0;
            headingField = 0;
            tiltField = 0;
            rollField = 0;
            itemField = altitudeModeEnumType.clampToGround;
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
        public double heading
        {
            get { return headingField; }
            set { headingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool headingSpecified
        {
            get { return headingFieldSpecified; }
            set { headingFieldSpecified = value; }
        }

        /// <remarks/>
        public double tilt
        {
            get { return tiltField; }
            set { tiltField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool tiltSpecified
        {
            get { return tiltFieldSpecified; }
            set { tiltFieldSpecified = value; }
        }

        /// <remarks/>
        public double roll
        {
            get { return rollField; }
            set { rollField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rollSpecified
        {
            get { return rollFieldSpecified; }
            set { rollFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("CameraSimpleExtensionGroup")]
        public string[] CameraSimpleExtensionGroup
        {
            get { return cameraSimpleExtensionGroupField; }
            set { cameraSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("CameraObjectExtensionGroup")]
        public AbstractObjectType[] CameraObjectExtensionGroup
        {
            get { return cameraObjectExtensionGroupField; }
            set { cameraObjectExtensionGroupField = value; }
        }
    }
}