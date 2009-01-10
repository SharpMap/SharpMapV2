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
    [XmlType(TypeName = "FunctionType", Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("Function", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public abstract class FunctionExpression : Expression
    {
        private String _functionName;
        private Expression[] _parameters;
        private ExpressionType[] _parameterTypes;

        [XmlElement("Add", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlElement("Div", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlElement("Function", typeof(FunctionExpression), Order = 0)]
        [XmlElement("Literal", typeof(LiteralExpression), Order = 0)]
        [XmlElement("Mul", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlElement("PropertyName", typeof(PropertyNameExpression), Order = 0)]
        [XmlElement("Sub", typeof(BinaryOperatorExpression), Order = 0)]
        [XmlChoiceIdentifier("ParameterTypes")]
        public Expression[] Parameters
        {
            get
            {
                return _parameters;
            }
            set
            {
                _parameters = value;
            }
        }

        [XmlElement("ParameterTypes", Order = 1)]
        [XmlIgnore]
        public ExpressionType[] ParameterTypes
        {
            get
            {
                return _parameterTypes;
            }
            set
            {
                _parameterTypes = value;
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
