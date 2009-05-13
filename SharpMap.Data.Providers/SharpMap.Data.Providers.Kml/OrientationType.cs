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
    [XmlRoot("Orientation", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class OrientationType : AbstractObjectType
    {
        private double headingField;

        private bool headingFieldSpecified;
        private AbstractObjectType[] orientationObjectExtensionGroupField;
        private string[] orientationSimpleExtensionGroupField;

        private double rollField;

        private bool rollFieldSpecified;
        private double tiltField;

        private bool tiltFieldSpecified;

        public OrientationType()
        {
            headingField = 0;
            tiltField = 0;
            rollField = 0;
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
        [XmlElement("OrientationSimpleExtensionGroup")]
        public string[] OrientationSimpleExtensionGroup
        {
            get { return orientationSimpleExtensionGroupField; }
            set { orientationSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("OrientationObjectExtensionGroup")]
        public AbstractObjectType[] OrientationObjectExtensionGroup
        {
            get { return orientationObjectExtensionGroupField; }
            set { orientationObjectExtensionGroupField = value; }
        }
    }
}