using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ExternalGraphicType")]
    [XmlRoot("ExternalGraphic", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class ExternalGraphic
    {
        private object _item;
        private string _format;
        private ColorReplacementExpression[] _colorReplacement;

        [XmlElement("InlineContent", typeof (InlineContent))]
        [XmlElement("OnlineResource", typeof (OnlineResource))]
        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public string Format
        {
            get { return _format; }
            set { _format = value; }
        }

        [XmlElement("ColorReplacement")]
        public ColorReplacementExpression[] ColorReplacement
        {
            get { return _colorReplacement; }
            set { _colorReplacement = value; }
        }
    }
}