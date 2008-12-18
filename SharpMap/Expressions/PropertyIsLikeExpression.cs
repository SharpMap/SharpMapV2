using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "PropertyIsLikeType")]
    [XmlRoot("PropertyIsLike", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class PropertyIsLikeExpression : ComparisonExpression
    {
        private PropertyNameExpression _propertyName;
        private LiteralExpression _literal;
        private String _wildCard;
        private String _singleChar;
        private String _escapeChar;

        public PropertyIsLikeExpression(Expression left, ComparisonOperator op, Expression right) : base(left, op, right) {}

        public PropertyNameExpression PropertyName
        {
            get { return _propertyName; }
            set { _propertyName = value; }
        }

        public LiteralExpression Literal
        {
            get { return _literal; }
            set { _literal = value; }
        }

        [XmlAttribute(AttributeName = "wildCard")]
        public String WildCard
        {
            get { return _wildCard; }
            set { _wildCard = value; }
        }

        [XmlAttribute(AttributeName = "singleChar")]
        public String SingleChar
        {
            get { return _singleChar; }
            set { _singleChar = value; }
        }

        [XmlAttribute(AttributeName = "escapeChar")]
        public String EscapeChar
        {
            get { return _escapeChar; }
            set { _escapeChar = value; }
        }
    }
}