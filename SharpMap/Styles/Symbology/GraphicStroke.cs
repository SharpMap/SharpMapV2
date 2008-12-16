using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "GraphicStroke")]
    [XmlRoot("GraphicStroke", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class GraphicStroke
    {
        private Graphic _graphic;
        private ParameterValue _initialGap;
        private ParameterValue _gap;

        public Graphic Graphic
        {
            get { return _graphic; }
            set { _graphic = value; }
        }

        public ParameterValue InitialGap
        {
            get { return _initialGap; }
            set { _initialGap = value; }
        }

        public ParameterValue Gap
        {
            get { return _gap; }
            set { _gap = value; }
        }
    }
}