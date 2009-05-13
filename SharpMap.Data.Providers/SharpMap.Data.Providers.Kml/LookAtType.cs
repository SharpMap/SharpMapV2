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
    [XmlRoot("LookAt", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class LookAtType : AbstractViewType
    {
        private double altitudeField;

        private bool altitudeFieldSpecified;

        private double headingField;

        private bool headingFieldSpecified;

        private altitudeModeEnumType itemField;
        private double latitudeField;

        private bool latitudeFieldSpecified;
        private double longitudeField;

        private bool longitudeFieldSpecified;

        private AbstractObjectType[] lookAtObjectExtensionGroupField;
        private string[] lookAtSimpleExtensionGroupField;
        private double rangeField;

        private bool rangeFieldSpecified;
        private double tiltField;

        private bool tiltFieldSpecified;

        public LookAtType()
        {
            longitudeField = 0;
            latitudeField = 0;
            altitudeField = 0;
            headingField = 0;
            tiltField = 0;
            rangeField = 0;
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
        public double range
        {
            get { return rangeField; }
            set { rangeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rangeSpecified
        {
            get { return rangeFieldSpecified; }
            set { rangeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("altitudeMode")]
        public altitudeModeEnumType Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement("LookAtSimpleExtensionGroup")]
        public string[] LookAtSimpleExtensionGroup
        {
            get { return lookAtSimpleExtensionGroupField; }
            set { lookAtSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("LookAtObjectExtensionGroup")]
        public AbstractObjectType[] LookAtObjectExtensionGroup
        {
            get { return lookAtObjectExtensionGroupField; }
            set { lookAtObjectExtensionGroupField = value; }
        }
    }
}