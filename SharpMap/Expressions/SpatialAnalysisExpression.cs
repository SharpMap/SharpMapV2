/*
 *  The attached / following is part of SharpMap
 *  SharpMap is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

using System;

namespace SharpMap.Expressions
{
    [Serializable]
    public abstract class SpatialAnalysisExpression : Expression, IEquatable<SpatialAnalysisExpression>
    {
        private readonly SpatialAnalysisOperation _operation;
        protected GeometryExpression _leftExpression;
        protected Expression _rightExpression;

        protected SpatialAnalysisExpression(GeometryExpression expressionLeft, SpatialAnalysisOperation operation,
                                            Expression expressionRight)
        {
            _leftExpression = expressionLeft;
            _rightExpression = expressionRight;
            _operation = operation;
        }

        public GeometryExpression LeftExpression
        {
            get { return _leftExpression; }
        }

        public Expression RightExpression
        {
            get { return _rightExpression; }
        }

        public SpatialAnalysisOperation SpatialAnalysisOperator
        {
            get { return _operation; }
        }

        #region IEquatable<SpatialAnalysisExpression> Members

        public bool Equals(SpatialAnalysisExpression other)
        {
            if (Equals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(_leftExpression, other._leftExpression) && Equals(_rightExpression, other._rightExpression) &&
                   Equals(_operation, other._operation);
        }

        #endregion
    }

    public abstract class SpatialAnalysisExpression<TResultExpression, TOtherExpression>
        : SpatialAnalysisExpression,
          IEquatable<SpatialAnalysisExpression<TResultExpression, TOtherExpression>>
        where TResultExpression : Expression
        where TOtherExpression : Expression
    {
        protected SpatialAnalysisExpression(GeometryExpression expressionLeft, SpatialAnalysisOperation operation,
                                            TOtherExpression expressionRight)
            : base(expressionLeft, operation, expressionRight)
        {
        }


        public new TOtherExpression RightExpression
        {
            get { return (TOtherExpression) base.RightExpression; }
        }

        #region IEquatable<SpatialAnalysisExpression<TResultExpression,TOtherExpression>> Members

        public bool Equals(SpatialAnalysisExpression<TResultExpression, TOtherExpression> other)
        {
            if (Equals(other, null))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Equals(LeftExpression, other.LeftExpression) &&
                   Equals(SpatialAnalysisOperator, other.SpatialAnalysisOperator) &&
                   Equals(RightExpression, other.RightExpression);
        }

        #endregion

        protected abstract TResultExpression GetValue();
    }
}