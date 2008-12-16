using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ColorReplacementType")]
    [XmlRoot("ColorReplacement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class ColorReplacementExpression
    {
        private RecodeExpression _recode;

        public RecodeExpression Recode
        {
            get { return _recode; }
            set { _recode = value; }
        }
    }
}