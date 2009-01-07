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
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "PropertyNameType")]
    [XmlRoot("PropertyName", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public sealed class PropertyNameExpression : Expression
    {
        private String _propertyName;

        public PropertyNameExpression() { }

        public PropertyNameExpression(String propertyName)
        {
            _propertyName = propertyName;
        }

        [XmlText(typeof(String))]
        public String PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (_propertyName != null)
                {
                    throw new ObjectReadonlyException();
                }

                _propertyName = value;
            }
        }

        public override ExpressionType ExpressionType
        {
            get { return ExpressionType.PropertyName; }
        }

        public override Boolean Contains(Expression other)
        {
            return Equals(other);
        }

        public override Expression Clone()
        {
            return new PropertyNameExpression(_propertyName);
        }

        public override Boolean Equals(Expression other)
        {
            if (other == null)
            {
                return false;
            }

            PropertyNameExpression propertyNameExpression = other as PropertyNameExpression;
            return String.Compare(_propertyName, propertyNameExpression._propertyName, true) == 0; 
        }

    }
}
