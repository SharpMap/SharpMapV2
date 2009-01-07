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

        public ProviderQueryExpression(ProviderPropertiesExpression providerPropertiesExpression, SelectExpression projection, LogicExpression predicate)
            : base(projection, predicate)
        {
            _providerPropertiesExpression = providerPropertiesExpression;
        }


        public ProviderQueryExpression(IEnumerable<ProviderPropertyExpression> providerProperties, SelectExpression projection, LogicExpression predicate)
            : this(new ProviderPropertiesExpression(providerProperties), projection, predicate)
        { }
    }
}
