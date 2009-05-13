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
    [XmlRoot("hotSpot", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class vec2Type
    {
        private double xField;

        private unitsEnumType xunitsField;
        private double yField;

        private unitsEnumType yunitsField;

        public vec2Type()
        {
            xField = 1;
            yField = 1;
            xunitsField = unitsEnumType.fraction;
            yunitsField = unitsEnumType.fraction;
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(1)]
        public double x
        {
            get { return xField; }
            set { xField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(1)]
        public double y
        {
            get { return yField; }
            set { yField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(unitsEnumType.fraction)]
        public unitsEnumType xunits
        {
            get { return xunitsField; }
            set { xunitsField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(unitsEnumType.fraction)]
        public unitsEnumType yunits
        {
            get { return yunitsField; }
            set { yunitsField = value; }
        }
    }
}