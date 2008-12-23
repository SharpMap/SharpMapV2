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
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ModeType")]
    public enum Mode
    {
        [XmlEnum(Name = "linear")] Linear,
        [XmlEnum(Name = "cosine")] Cosine,
        [XmlEnum(Name = "cubic")] Cubic,
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "MethodType")]
    public enum Method
    {
        [XmlEnum(Name = "numeric")] Numeric,
        [XmlEnum(Name = "color")] Color,
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "InterpolateType")]
    [XmlRoot("Interpolate", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class InterpolateExpression : SymbologyFunctionExpression
    {
        private ParameterValue _lookupValue;
        private InterpolationPointExpression[] _interpolationPoint;
        private Mode _mode;
        private bool _modeSpecified;
        private Method _method;
        private bool _methodSpecified;

        [XmlElement(Order = 0)]
        public ParameterValue LookupValue
        {
            get { return _lookupValue; }
            set { _lookupValue = value; }
        }

        [XmlElement("InterpolationPoint", Order = 1)]
        public InterpolationPointExpression[] InterpolationPoint
        {
            get { return _interpolationPoint; }
            set { _interpolationPoint = value; }
        }

        [XmlAttribute(AttributeName = "mode")]
        public Mode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        [XmlIgnore]
        public bool ModeSpecified
        {
            get { return _modeSpecified; }
            set { _modeSpecified = value; }
        }

        [XmlAttribute(AttributeName = "method")]
        public Method Method
        {
            get { return _method; }
            set { _method = value; }
        }

        [XmlIgnore]
        public bool MethodSpecified
        {
            get { return _methodSpecified; }
            set { _methodSpecified = value; }
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