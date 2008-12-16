using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "GraphicType")]
    [XmlRoot("Graphic", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Graphic
    {
        private object[] _items;

        private ParameterValue _opacity;

        private ParameterValue _size;

        private ParameterValue _rotation;

        private AnchorPoint _anchorPoint;

        private Displacement _displacement;

        /// <remarks/>
        [XmlElement("ExternalGraphic", typeof (ExternalGraphic))]
        [XmlElement("Mark", typeof (Mark))]
        public object[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        /// <remarks/>
        public ParameterValueType Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }

        /// <remarks/>
        public ParameterValueType Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <remarks/>
        public ParameterValueType Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <remarks/>
        public AnchorPointType AnchorPoint
        {
            get { return _anchorPoint; }
            set { _anchorPoint = value; }
        }

        /// <remarks/>
        public DisplacementType Displacement
        {
            get { return _displacement; }
            set { _displacement = value; }
        }
    }
}