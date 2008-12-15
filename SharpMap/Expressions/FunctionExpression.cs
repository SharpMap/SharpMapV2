using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable()]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum ExpressionType
    {
        Add,
        Div,
        Function,
        Literal,
        Mul,
        PropertyName,
        Sub,
    }

    [Serializable]
    [XmlType(TypeName = "FunctionType", Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("Function", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public abstract class FunctionExpression : Expression
    {
        private String _functionName;
        private Expression[] _parameters;
        private ExpressionType[] _parameterTypes;

        [XmlElement("Add", typeof(BinaryExpression), Order = 0)]
        [XmlElement("Div", typeof(BinaryExpression), Order = 0)]
        [XmlElement("Function", typeof(FunctionExpression), Order = 0)]
        [XmlElement("Literal", typeof(LiteralExpression), Order = 0)]
        [XmlElement("Mul", typeof(BinaryExpression), Order = 0)]
        [XmlElement("PropertyName", typeof(PropertyNameExpression), Order = 0)]
        [XmlElement("Sub", typeof(BinaryExpression), Order = 0)]
        [XmlChoiceIdentifier("ParameterTypes")]
        public Expression[] Parameters
        {
            get
            {
                return this._parameters;
            }
            set
            {
                this._parameters = value;
            }
        }

        [XmlElement("ParameterTypes", Order = 1)]
        [XmlIgnore]
        public ExpressionType[] ParameterTypes
        {
            get
            {
                return this._parameterTypes;
            }
            set
            {
                this._parameterTypes = value;
            }
        }

        [XmlAttribute(AttributeName = "name")]
        public String Name
        {
            get
            {
                return _functionName;
            }
            set
            {
                if (_functionName != null)
                {
                    throw new InvalidOperationException("FunctionExpression is read-only after setting.");
                }

                _functionName = value;
            }
        }
    }
}
