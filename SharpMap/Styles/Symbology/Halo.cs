using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "HaloType")]
    [XmlRoot("Halo", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Halo
    {
        private ParameterValue _radius;

        private Fill _fill;

        public ParameterValue Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public Fill Fill
        {
            get { return _fill; }
            set { _fill = value; }
        }
    }
}