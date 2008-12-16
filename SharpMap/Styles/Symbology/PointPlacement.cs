using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se")]
    [XmlRoot("PointPlacement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    internal class PointPlacement
    {
        private AnchorPoint _anchorPoint;

        private Displacement _displacement;

        private ParameterValue _rotation;

        public AnchorPoint AnchorPoint
        {
            get { return _anchorPoint; }
            set { _anchorPoint = value; }
        }

        public Displacement Displacement
        {
            get { return _displacement; }
            set { _displacement = value; }
        }

        public ParameterValue Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }
    }
}