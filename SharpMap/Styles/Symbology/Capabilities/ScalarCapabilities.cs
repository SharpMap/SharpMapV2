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
        private object[] arithmeticOperatorsField;
        private ComparisonOperator comparisonOperatorsField;
        private LogicalOperators logicalOperatorsField;

        /// <remarks/>
        public LogicalOperators LogicalOperators
        {
            get { return logicalOperatorsField; }
            set { logicalOperatorsField = value; }
        }

        public ComparisonOperator ComparisonOperators
        {
            get { return comparisonOperatorsField; }
            set { comparisonOperatorsField = value; }
        }

        /// <remarks/>
        [XmlArrayItem("Functions", typeof (FunctionsType), IsNullable = false)]
        [XmlArrayItem("SimpleArithmetic", typeof (SimpleArithmetic), IsNullable = false)]
        public object[] ArithmeticOperators
        {
            get { return arithmeticOperatorsField; }
            set { arithmeticOperatorsField = value; }
        }
    }
}