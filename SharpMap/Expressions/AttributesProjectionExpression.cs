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
using System.Collections.Generic;
using GeoAPI.DataStructures;

namespace SharpMap.Expressions
{
    public class AttributesProjectionExpression : ProjectionExpression
    {
        private readonly CollectionExpression _attributes;

        public AttributesProjectionExpression(IEnumerable<String> attributes)
            // The explicit cast is needed when compiling under ToolsVersion=2.0
            : this(Enumerable.Transform(attributes, (Func<String, PropertyNameExpression>)delegate(String name)
                                                    {
                                                        return new PropertyNameExpression(name);
                                                    })) { }

        public AttributesProjectionExpression(IEnumerable<PropertyNameExpression> attributes)
            : this(new CollectionExpression(attributes)) { }

        public AttributesProjectionExpression(CollectionExpression attributes)
        {
            _attributes = attributes;
        }

        public CollectionExpression Attributes
        {
            get { return _attributes; }
        }

        public override Boolean Contains(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            throw new NotImplementedException();
        }

        public override Boolean Equals(Expression other)
        {
            throw new NotImplementedException();
        }
    }
}
