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

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "UpperBoundaryType")]
    public class UpperBoundaryExpression : Expression
    {
        private Expression _expression;
        private ExpressionType _expressionType;

        [XmlElement("Add", typeof(BinaryOperationExpression))]
        [XmlElement("Div", typeof(BinaryOperationExpression))]
        [XmlElement("Function", typeof(FunctionExpression))]
        [XmlElement("Literal", typeof(LiteralExpression))]
        [XmlElement("Mul", typeof(BinaryOperationExpression))]
        [XmlElement("PropertyName", typeof(PropertyNameExpression))]
        [XmlElement("Sub", typeof(BinaryOperationExpression))]
        [XmlChoiceIdentifier("ExpressionElementType")]
        public Expression Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
            }
        }

        [XmlIgnore]
        public ExpressionType ExpressionElementType
        {
            get
            {
                return _expressionType;
            }
            set
            {
                _expressionType = value;
            }
        }

        #region Overrides of Expression

        public override Boolean Contains(Expression other)
        {
            throw new System.NotImplementedException();
        }

        public override Expression Clone()
        {
            throw new System.NotImplementedException();
        }

        public override Boolean Equals(Expression other)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
