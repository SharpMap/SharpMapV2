using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FillType")]
    [XmlRoot("Fill", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Fill
    {
        private GraphicFill _graphicFill;

        private SvgParameter[] _svgParameter;

        public GraphicFill GraphicFill
        {
            get { return _graphicFill; }
            set { _graphicFill = value; }
        }

        [XmlElement("SvgParameter")]
        public SvgParameter[] SvgParameter
        {
            get { return _svgParameter; }
            set { _svgParameter = value; }
        }
    }
}