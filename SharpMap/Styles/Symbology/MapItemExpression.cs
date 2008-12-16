using System;
using System.Xml.Serialization;
using SharpMap.Expressions;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "MapItemType")]
    [XmlRoot("MapItem", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class MapItemExpression : Expression
    {
        private Double _data;
        private ParameterValue _value;

        [XmlElement(Order = 0)]
        public Double Data
        {
            get { return _data; }
            set { _data = value; }
        }

        [XmlElement(Order = 1)]
        public ParameterValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}