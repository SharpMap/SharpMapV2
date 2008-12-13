using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{

    [Serializable()]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum ExpressionType
    {

        /// <remarks/>
        Add,

        /// <remarks/>
        Div,

        /// <remarks/>
        Function,

        /// <remarks/>
        Literal,

        /// <remarks/>
        Mul,

        /// <remarks/>
        PropertyName,

        /// <remarks/>
        Sub,
    }

    [Serializable]
    [XmlType(TypeName = "FunctionType", Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("Function", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class FunctionType1 : Expression
    {

        private String _functionName;
        private Expression[] _expressions;
        private ExpressionType[] _expressionTypes;

        [XmlElement("Add", typeof(BinaryOperatorType), Order = 0)]
        [XmlElement("Div", typeof(BinaryOperatorType), Order = 0)]
        [XmlElement("Function", typeof(FunctionType1), Order = 0)]
        [XmlElement("Literal", typeof(LiteralType), Order = 0)]
        [XmlElement("Mul", typeof(BinaryOperatorType), Order = 0)]
        [XmlElement("PropertyName", typeof(PropertyNameType), Order = 0)]
        [XmlElement("Sub", typeof(BinaryOperatorType), Order = 0)]
        [XmlChoiceIdentifier("ExpressionElementTypes")]
        public Expression[] Items
        {
            get
            {
                return this._expressions;
            }
            set
            {
                this._expressions = value;
            }
        }

        [XmlElement("ExpressionElementTypes", Order = 1)]
        [XmlIgnore]
        public ExpressionType[] ExpressionElementTypes
        {
            get
            {
                return this._expressionTypes;
            }
            set
            {
                this._expressionTypes = value;
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
