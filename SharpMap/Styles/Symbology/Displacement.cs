using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "DisplacementType")]
    [XmlRoot("Displacement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Displacement
    {
        private ParameterValue _displacementX;
        private ParameterValue _displacementY;

        public ParameterValue DisplacementX
        {
            get { return _displacementX; }
            set { _displacementX = value; }
        }

        public ParameterValue DisplacementY
        {
            get { return _displacementY; }
            set { _displacementY = value; }
        }
    }
}