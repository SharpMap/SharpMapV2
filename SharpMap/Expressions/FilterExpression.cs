using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    public class FilterExpression : Expression
    {
        #region Overrides of Expression

        public override ExpressionType ExpressionType
        {
            get { throw new System.NotImplementedException(); }
        }

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