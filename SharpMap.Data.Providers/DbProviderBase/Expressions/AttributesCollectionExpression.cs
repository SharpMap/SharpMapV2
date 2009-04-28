using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.DataStructures;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db.Expressions
{
    public class AttributesCollectionExpression :
        ProviderPropertyExpression<CollectionExpression<PropertyNameExpression>>
    {
        public AttributesCollectionExpression(IEnumerable<String> names)
            : this(Processor.Transform(names, delegate(string o) { return new PropertyNameExpression(o); }))
        {
        }
        public AttributesCollectionExpression(PropertyNameExpression attributeExpression)
            : this(new[] { attributeExpression })
        { }

        public AttributesCollectionExpression(IEnumerable<PropertyNameExpression> attributeExpressions)
            : this(new CollectionExpression<PropertyNameExpression>(attributeExpressions))
        { }

        public AttributesCollectionExpression(CollectionExpression<PropertyNameExpression> attributesCollectionExpression)
            : base("AttributesCollection", attributesCollectionExpression)
        { }

        AttributesProjectionExpression AttributesProjection
        {
            get
            { 
                return new AttributesProjectionExpression(
                (CollectionExpression<PropertyNameExpression>)base.PropertyValueExpression.Clone());
            }
        }

        public override Expression Clone()
        {
            return base.PropertyValueExpression.Clone();
        }
    }
    
}
