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

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "StringPositionType")]
    [XmlRoot("StringPosition", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class StringPositionExpression : SymbologyFunctionExpression
    {
        private ParameterValue _lookupString;
        private ParameterValue _stringValue;
        private SearchDirection _searchDirection;
        private bool _searchDirectionSpecified;

        [XmlElement(Order = 0)]
        public ParameterValue LookupString
        {
            get { return _lookupString; }
            set { _lookupString = value; }
        }

        [XmlElement(Order = 1)]
        public ParameterValue StringValue
        {
            get { return _stringValue; }
            set { _stringValue = value; }
        }

        [XmlAttribute(AttributeName = "searchDirection")]
        public SearchDirection SearchDirection
        {
            get { return _searchDirection; }
            set { _searchDirection = value; }
        }

        [XmlIgnore]
        public bool SearchDirectionSpecified
        {
            get { return _searchDirectionSpecified; }
            set { _searchDirectionSpecified = value; }
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

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "searchDirectionType")]
    public enum SearchDirection
    {
        [XmlEnum(Name = "frontToBack")] FrontToBack,
        [XmlEnum(Name = "backToFront")] BackToFront,
    }
}