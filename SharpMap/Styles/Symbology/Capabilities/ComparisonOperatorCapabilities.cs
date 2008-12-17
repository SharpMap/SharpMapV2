using System;
using System.Xml.Serialization;
using SharpMap.Expressions;

namespace SharpMap.Styles.Symbology.Capabilities
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "ComparisonOperatorsType")]
    public class ComparisonOperatorCapabilities
    {
        private ComparisonOperator[] _comparisonOperator;

        [XmlElement("ComparisonOperator")]
        public ComparisonOperator[] ComparisonOperator
        {
            get { return _comparisonOperator; }
            set { _comparisonOperator = value; }
        }
    }
}