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
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "CategorizeType")]
    [XmlRoot("Categorize", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class CategorizeExpression : SymbologyFunctionExpression
    {
        private ParameterValue _lookupValue;
        private ParameterValue _value;
        private ParameterValue[] _threshold;
        private ParameterValue[] _value1;
        private ThreshholdsBelongToType _threshholdsBelongTo;
        private Boolean _threshholdsBelongToSpecified;

        [XmlElement(Order = 0)]
        public ParameterValue LookupValue
        {
            get { return _lookupValue; }
            set { _lookupValue = value; }
        }

        [XmlElement(Order = 1)]
        public ParameterValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        [XmlElement("Threshold", Order = 2)]
        public ParameterValue[] Threshold
        {
            get { return _threshold; }
            set { _threshold = value; }
        }

        [XmlElement("Value", Order = 3)]
        public ParameterValue[] Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }

        [XmlAttribute(AttributeName = "threshholdsBelongTo")]
        public ThreshholdsBelongToType ThreshholdsBelongTo
        {
            get { return _threshholdsBelongTo; }
            set { _threshholdsBelongTo = value; }
        }

        [XmlIgnore]
        public Boolean ThreshholdsBelongToSpecified
        {
            get { return _threshholdsBelongToSpecified; }
            set { _threshholdsBelongToSpecified = value; }
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

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ThreshholdsBelongToType")]
    public enum ThreshholdsBelongToType
    {
        [XmlEnum(Name = "succeeding")] Succeeding,

        [XmlEnum(Name = "preceding")] Preceding,
    }
}