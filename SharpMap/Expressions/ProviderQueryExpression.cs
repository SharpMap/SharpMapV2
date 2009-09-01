using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Expressions
{
    public class ProviderQueryExpression : QueryExpression
    {
        private ProviderPropertiesExpression _providerPropertiesExpression;
        public ProviderPropertiesExpression ProviderProperties
        {
            get { return _providerPropertiesExpression; }
        }
        public ProviderQueryExpression(
           ProviderPropertiesExpression providerPropertiesExpression,
           ProjectionExpression projection,
           PredicateExpression predicate)
            : this(providerPropertiesExpression, projection, predicate, null)
        {

        }
        public ProviderQueryExpression(
            ProviderPropertiesExpression providerPropertiesExpression,
            ProjectionExpression projection,
            PredicateExpression predicate,
            SortExpressionCollectionExpression sort)
            : base(projection, predicate, sort)
        {
            _providerPropertiesExpression = providerPropertiesExpression;
        }

        public ProviderQueryExpression(
           IEnumerable<ProviderPropertyExpression> providerProperties,
           ProjectionExpression projection,
           PredicateExpression predicate)
            : this(providerProperties, projection, predicate, null)
        {

        }
        public ProviderQueryExpression(
            IEnumerable<ProviderPropertyExpression> providerProperties,
            ProjectionExpression projection,
            PredicateExpression predicate,
            SortExpressionCollectionExpression sort)
            : this(new ProviderPropertiesExpression(providerProperties), projection, predicate, sort)
        { }
    }
}
