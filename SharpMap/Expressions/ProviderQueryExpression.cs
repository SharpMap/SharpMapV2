using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Expressions
{
    public class ProviderQueryExpression : QueryExpression
    {
        private ProviderPropertiesExpression _providerPropertiesExpression;
        public ProviderPropertiesExpression ProviderPropertiesExpression
        {
            get { return _providerPropertiesExpression; }
        }

        public ProviderQueryExpression(ProviderPropertiesExpression providerPropertiesExpression, ProjectionExpression projection, PredicateExpression predicate)
            : base(projection, predicate)
        {
            _providerPropertiesExpression = providerPropertiesExpression;
        }


        public ProviderQueryExpression(IEnumerable<ProviderPropertyExpression> providerProperties, ProjectionExpression projection, PredicateExpression predicate)
            : this(new ProviderPropertiesExpression(providerProperties), projection, predicate)
        { }
    }
}
