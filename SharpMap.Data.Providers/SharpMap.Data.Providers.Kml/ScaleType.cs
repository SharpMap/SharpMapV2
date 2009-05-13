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
    [XmlRoot("Scale", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ScaleType : AbstractObjectType
    {
        private AbstractObjectType[] scaleObjectExtensionGroupField;
        private string[] scaleSimpleExtensionGroupField;
        private double xField;

        private bool xFieldSpecified;

        private double yField;

        private bool yFieldSpecified;

        private double zField;

        private bool zFieldSpecified;

        public ScaleType()
        {
            xField = 1;
            yField = 1;
            zField = 1;
        }

        /// <remarks/>
        public double x
        {
            get { return xField; }
            set { xField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool xSpecified
        {
            get { return xFieldSpecified; }
            set { xFieldSpecified = value; }
        }

        /// <remarks/>
        public double y
        {
            get { return yField; }
            set { yField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool ySpecified
        {
            get { return yFieldSpecified; }
            set { yFieldSpecified = value; }
        }

        /// <remarks/>
        public double z
        {
            get { return zField; }
            set { zField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool zSpecified
        {
            get { return zFieldSpecified; }
            set { zFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ScaleSimpleExtensionGroup")]
        public string[] ScaleSimpleExtensionGroup
        {
            get { return scaleSimpleExtensionGroupField; }
            set { scaleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ScaleObjectExtensionGroup")]
        public AbstractObjectType[] ScaleObjectExtensionGroup
        {
            get { return scaleObjectExtensionGroupField; }
            set { scaleObjectExtensionGroupField = value; }
        }
    }
}