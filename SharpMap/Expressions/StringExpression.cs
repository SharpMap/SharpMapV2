using System;
using System.Collections.Generic;
using SharpMap.Utilities;

namespace SharpMap.Expressions
{
    public class StringExpression : ValueExpression<String>
    {
        private readonly StringComparison _comparison;

        public StringExpression(String value)
            : this(value, StringComparison.CurrentCultureIgnoreCase) { }

        public StringExpression(String value, StringComparison comparison)
            : this(value, comparison, StringEqualityComparer.GetComparer(comparison)) { }
        
        private StringExpression(String value, StringComparison comparison, IEqualityComparer<String> comparer)
            : base(value, comparer)
        {
            _comparison = comparison;   
        }

        public StringComparison Comparison
        {
            get { return _comparison; }
        }

        public override Boolean Matches(Expression other)
        {
            throw new NotImplementedException();
        }

        public override Expression Clone()
        {
            return new StringExpression(Value, _comparison, Comparer);
        }
    }
}
