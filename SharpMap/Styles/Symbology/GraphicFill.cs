using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "GraphicFillType")]
    [XmlRoot("GraphicFill", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class GraphicFill
    {
        private Graphic _graphic;

        public Graphic Graphic
        {
            get { return _graphic; }
            set { _graphic = value; }
        }
    }
}