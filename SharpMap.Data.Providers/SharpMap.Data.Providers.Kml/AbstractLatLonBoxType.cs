using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (LatLonBoxType))]
    [XmlInclude(typeof (LatLonAltBoxType))]
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class AbstractLatLonBoxType : AbstractObjectType
    {
        private AbstractObjectType[] abstractLatLonBoxObjectExtensionGroupField;
        private string[] abstractLatLonBoxSimpleExtensionGroupField;
        private double eastField;

        private bool eastFieldSpecified;
        private double northField;

        private bool northFieldSpecified;

        private double southField;

        private bool southFieldSpecified;

        private double westField;

        private bool westFieldSpecified;

        public AbstractLatLonBoxType()
        {
            northField = 180;
            southField = -180;
            eastField = 180;
            westField = -180;
        }

        /// <remarks/>
        public double north
        {
            get { return northField; }
            set { northField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool northSpecified
        {
            get { return northFieldSpecified; }
            set { northFieldSpecified = value; }
        }

        /// <remarks/>
        public double south
        {
            get { return southField; }
            set { southField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool southSpecified
        {
            get { return southFieldSpecified; }
            set { southFieldSpecified = value; }
        }

        /// <remarks/>
        public double east
        {
            get { return eastField; }
            set { eastField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool eastSpecified
        {
            get { return eastFieldSpecified; }
            set { eastFieldSpecified = value; }
        }

        /// <remarks/>
        public double west
        {
            get { return westField; }
            set { westField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool westSpecified
        {
            get { return westFieldSpecified; }
            set { westFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractLatLonBoxSimpleExtensionGroup")]
        public string[] AbstractLatLonBoxSimpleExtensionGroup
        {
            get { return abstractLatLonBoxSimpleExtensionGroupField; }
            set { abstractLatLonBoxSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractLatLonBoxObjectExtensionGroup")]
        public AbstractObjectType[] AbstractLatLonBoxObjectExtensionGroup
        {
            get { return abstractLatLonBoxObjectExtensionGroupField; }
            set { abstractLatLonBoxObjectExtensionGroupField = value; }
        }
    }
}