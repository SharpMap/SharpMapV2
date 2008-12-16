using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "RecodeType")]
    [XmlRoot("Recode", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class RecodeExpression : FunctionSymbolizerExpression
    {
        private ParameterValue _lookupValue;
        private MapItemExpression[] _mapItem;

        [XmlElement(Order = 0)]
        public ParameterValue LookupValue
        {
            get { return _lookupValue; }
            set { _lookupValue = value; }
        }

        [XmlElement("MapItem", Order = 1)]
        public MapItemExpression[] MapItem
        {
            get { return _mapItem; }
            set { _mapItem = value; }
        }
    }
}