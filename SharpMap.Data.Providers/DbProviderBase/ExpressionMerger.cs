using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db
{
    public static class ExpressionMerger
    {
        public static Expression MergeExpressions(Expression expr1, Expression expr2)
        {
            if (Equals(null, expr1))
                return expr2;
            if (Equals(null, expr2))
                return expr1;

            if (expr1 is BinaryExpression && expr2 is BinaryExpression)
                return MergePredicateExpressions((BinaryExpression)expr1, (BinaryExpression)expr2);

            if (expr1 is QueryExpression && expr2 is QueryExpression)
                return MergeQueryExpressions((QueryExpression)expr1, (QueryExpression)expr2);

            if (expr1 is FeatureQueryExpression && expr2 is LogicExpression)
                return MergeFeatureQueryAndPredicateExpression((FeatureQueryExpression)expr1, (LogicExpression)expr2);

            if (expr1 is LogicExpression && expr2 is FeatureQueryExpression)
                return MergeFeatureQueryAndPredicateExpression((FeatureQueryExpression)expr2, (LogicExpression)expr1);

            if (expr1 is ProviderQueryExpression && expr2 is LogicExpression)
                return MergeProviderQueryExpressionAndSpatialBinaryExpression((ProviderQueryExpression)expr1,
                                                                              (LogicExpression)expr2);


            throw GetMergeException(expr1, expr2);
        }

        private static Expression MergeProviderQueryExpressionAndSpatialBinaryExpression(ProviderQueryExpression expression, LogicExpression binaryExpression)
        {
            return new ProviderQueryExpression(expression.ProviderProperties, expression.Projection,
                                               MergePredicateExpressions(expression.Predicate, binaryExpression));
        }

        private static Expression MergeFeatureQueryAndPredicateExpression(FeatureQueryExpression expression, LogicExpression predicateExpression)
        {
            return new FeatureQueryExpression((SelectExpression)expression.Projection,
                                              (LogicExpression)MergePredicateExpressions(expression.Predicate, predicateExpression));
        }

        private static Exception GetMergeException(Expression expr1, Expression expr2)
        {
            return new NotImplementedException(string.Format("No merge routine for Expressions of Types {0} and {1}",
                                                            expr1.GetType(),
                                                            expr2.GetType()));
        }

        private static LogicExpression MergePredicateExpressions(LogicExpression expression, LogicExpression binaryExpression)
        {
            return new BinaryExpression(expression, BinaryLogicOperator.And, binaryExpression);
        }


        private static Expression MergeQueryExpressions(QueryExpression expr1, QueryExpression expr2)
        {
            return new QueryExpression(MergeProjectionExpressions(expr1.Projection, expr2.Projection), MergePredicateExpressions(expr1.Predicate, expr2.Predicate));
        }

        private static SelectExpression MergeProjectionExpressions(SelectExpression projection, SelectExpression expression)
        {
            if (projection is AttributesProjectionExpression && expression is AttributesProjectionExpression)
            {
                var expr = new List<PropertyNameExpression>();
                expr.AddRange((projection as AttributesProjectionExpression).Attributes.Collection);
                expr.AddRange((expression as AttributesProjectionExpression).Attributes.Collection);
                return new AttributesProjectionExpression(expr);
            }
            throw GetMergeException(projection, expression);
        }
    }
}
