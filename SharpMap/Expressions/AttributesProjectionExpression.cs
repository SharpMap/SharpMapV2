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
            : this(Enumerable.Transform(attributes, (Func<String, AttributeExpression>)delegate(String name)
                                                    {
                                                        return new AttributeExpression(name);
                                                    })) { }

        public AttributesProjectionExpression(IEnumerable<AttributeExpression> attributes)
            : this(new CollectionExpression(attributes)) { }

        public AttributesProjectionExpression(CollectionExpression attributes)
        {
            _attributes = attributes;
        }

        public CollectionExpression Attributes
        {
            get { return _attributes; }
        }

        public override bool Matches(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Expression other)
        {
            throw new NotImplementedException();
        }
    }
}
