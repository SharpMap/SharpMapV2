using System;
using NPack.Interfaces;

namespace SharpMap.Expressions
{
    public class NumericExpression<TNumber> : Expression
        where TNumber : IComputable<TNumber, Double>
    {
        private readonly TNumber _value;

        public NumericExpression(TNumber value)
        {
            _value = value;
        }

        public TNumber value
        {
            get { return _value; }
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
