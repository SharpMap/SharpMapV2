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
using System.ComponentModel;
using System.Xml.Serialization;
using SharpMap.Expressions;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FormatNumberType")]
    [XmlRoot("FormatNumber", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class FormatNumberExpression : SymbologyFunctionExpression
    {
        private ParameterValue _numericValue;
        private string _pattern;
        private string _negativePattern;
        private string _decimalPoint;
        private string _groupingSeparator;

        public FormatNumberExpression()
        {
            _decimalPoint = ".";
            _groupingSeparator = ",";
        }

        [XmlElement(Order = 0)]
        public ParameterValue NumericValue
        {
            get { return _numericValue; }
            set { _numericValue = value; }
        }

        [XmlElement(Order = 1)]
        public string Pattern
        {
            get { return _pattern; }
            set { _pattern = value; }
        }

        [XmlElement(Order = 2)]
        public string NegativePattern
        {
            get { return _negativePattern; }
            set { _negativePattern = value; }
        }

        [XmlAttribute(AttributeName = "decimalPoint")]
        [DefaultValue(".")]
        public string DecimalPoint
        {
            get { return _decimalPoint; }
            set { _decimalPoint = value; }
        }

        [XmlAttribute(AttributeName = "groupingSeparator")]
        [DefaultValue(",")]
        public string GroupingSeparator
        {
            get { return _groupingSeparator; }
            set { _groupingSeparator = value; }
        }

        #region Overrides of Expression

        public override bool Contains(Expression other)
        {
            throw new System.NotImplementedException();
        }

        public override Expression Clone()
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(Expression other)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}