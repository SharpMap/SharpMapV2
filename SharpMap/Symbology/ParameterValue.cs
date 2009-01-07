// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Xml.Serialization;
using SharpMap.Expressions;

namespace SharpMap.Symbology
{
    //[Serializable]
    //[XmlType(Namespace = "http://www.opengis.net/se", IncludeInSchema = false)]
    //public enum ItemsChoiceType2
    //{
    //    [XmlEnum("http://www.opengis.net/ogc:Add")] Add,
    //    [XmlEnum("http://www.opengis.net/ogc:Div")] Div,
    //    [XmlEnum("http://www.opengis.net/ogc:Function")] Function,
    //    [XmlEnum("http://www.opengis.net/ogc:Literal")] Literal,
    //    [XmlEnum("http://www.opengis.net/ogc:Mul")] Mul,
    //    [XmlEnum("http://www.opengis.net/ogc:PropertyName")] PropertyName,
    //    [XmlEnum("http://www.opengis.net/ogc:Sub")] Sub,
    //}

    [XmlInclude(typeof(SvgParameter))]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ParameterValueType")]
    // [XmlRoot("InitialGap", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class ParameterValue
    {
        private Expression[] _expressions;
        private ExpressionType[] _expressionElementTypes;
        private String[] _text;
        private readonly Object _value;

        public ParameterValue() { }

        public ParameterValue(Object defaultValue)
        {
            _value = defaultValue;
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("Add", typeof(BinaryOperationExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("Div", typeof(BinaryOperationExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("Function", typeof(FunctionExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("Literal", typeof(LiteralExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("Mul", typeof(BinaryOperationExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("PropertyName", typeof(PropertyNameExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("Sub", typeof(BinaryOperationExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlChoiceIdentifier("ExpressionElementTypes")]
        public Expression[] Expressions
        {
            get { return _expressions; }
            set { _expressions = value; }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("ExpressionElementTypes")]
        [XmlIgnore]
        public ExpressionType[] ExpressionElementTypes
        {
            get { return _expressionElementTypes; }
            set { _expressionElementTypes = value; }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlText]
        public String[] Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public static explicit operator Expression(ParameterValue value)
        {
            Expression valueExpression = (value.Expressions == null || value.Expressions.Length == 0)
                                                 ? null
                                                 : value.Expressions[0];

            if (valueExpression == null && value.Text != null)
            {
                LiteralExpression<String> literal = String.Join(" ", value.Text).Trim();
                valueExpression = literal;
            }

            if (valueExpression != null && value._value != null)
            {
                return new LiteralExpression<Object>(value._value);
            }

            return valueExpression;
        }
    }
}