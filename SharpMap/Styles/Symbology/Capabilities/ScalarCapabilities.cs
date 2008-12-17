#region Namespace imports

using System;
using System.Xml.Serialization;
using SharpMap.Expressions;

#endregion

namespace SharpMap.Styles.Symbology.Capabilities
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "Scalar_CapabilitiesType")]
    public class ScalarCapabilities
    {
        private object[] _arithmeticOperators;
        private ComparisonOperatorCapabilities _comparisonOperators;
        private LogicalOperatorsCapabilities _logicalOperators;

        public LogicalOperatorsCapabilities LogicalOperators
        {
            get { return _logicalOperators; }
            set { _logicalOperators = value; }
        }

        public ComparisonOperatorCapabilities ComparisonOperators
        {
            get { return _comparisonOperators; }
            set { _comparisonOperators = value; }
        }

        [XmlArrayItem("Functions", typeof (FunctionsType), IsNullable = false)]
        [XmlArrayItem("SimpleArithmetic", typeof (SimpleArithmetic), IsNullable = false)]
        public object[] ArithmeticOperators
        {
            get { return _arithmeticOperators; }
            set { _arithmeticOperators = value; }
        }
    }
}